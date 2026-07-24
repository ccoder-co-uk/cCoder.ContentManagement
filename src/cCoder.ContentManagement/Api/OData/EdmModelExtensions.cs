// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace cCoder.ContentManagement.Api.OData;

public static class EdmModelExtensions
{
    public static IEnumerable<ExtendedMetadataContainer> GetMetadata(this IEdmModel model, string contextName)
    {
        List<ExtendedMetadataContainer> list = new List<ExtendedMetadataContainer>();

        foreach (IEdmEntitySet item in model.EntityContainer.EntitySets())
        {
            Type clrType = GetClrType(model: model, edmType: item.EntityType);

            if ((object)clrType != null)
            {
                list.Add(item: model.GetExtendedMetadataForType(context: contextName, type: clrType));
            }
        }

        foreach (IEdmSchemaType item2 in model.SchemaElements.OfType<IEdmSchemaType>())
        {
            if (item2 is IEdmComplexType || item2 is IEdmEntityType)
            {
                Type clrType2 = GetClrType(model: model, edmType: item2);

                if ((object)clrType2 != null)
                {
                    bool hasEndpoint = model.EntityContainer.FindEntitySet(setName: clrType2.Name) != null;
                    list.Add(item: model.GetExtendedMetadataForType(context: contextName, type: clrType2, hasEndpoint: hasEndpoint));
                }
            }
        }

        return list.DistinctBy(keySelector: (ExtendedMetadataContainer type) => type.ServerTypeName);
    }

    public static ExtendedMetadataContainer GetExtendedMetadataForType(this IEdmModel model, string context, Type type, bool hasEndpoint = true)
    {
        ExtendedMetadataContainer result = new ExtendedMetadataContainer(type: type, isEntity: true, hasEndpoint: hasEndpoint)
        {
            Category = context
        };

        IEdmEntitySet edmEntitySet = model.EntityContainer.FindEntitySet(setName: type.Name);

        if (edmEntitySet == null)
        {
            result.HasEndpoint = false;
            return result;
        }

        IEnumerable<OperationContainer> second = model.FindDeclaredBoundOperations(bindingType: edmEntitySet.Type)
            .Select(selector: operation => new OperationContainer
            {
                Name = operation.Name,
                Url = $"{result.Category}/{type.Name}/{operation.Name}()",
                Queryable = operation.IsFunction(),
                HttpVerb = (operation.IsFunction() ? "GET" : "POST"),
                ReturnType = BuildMetaFor(definition: operation.GetReturn()?.Type?.Definition),
                Parameters = operation.Parameters?
                    .Where(predicate: parameter => parameter.Name != "bindingParameter")
            .Select(selector: parameter => new
            {
                Name = parameter.Name,
                TypeName = parameter.Type.FullName()
            })
            .ToDictionary(keySelector: item => item.Name, elementSelector: item => item.TypeName)
            });

        result.Operations = GetBaseCrudOperations(type: result)
            .Union(second: second)
            .ToList();

        return result;
    }

    private static Type GetClrType(IEdmModel model, IEdmSchemaType edmType) =>
        model.GetAnnotationValue<ClrTypeAnnotation>(element: edmType)?.ClrType;

    private static MetadataContainer BuildMetaFor(IEdmType definition)
    {
        if (definition == null || definition.TypeKind != EdmTypeKind.Collection)
        {
            return null;
        }

        Type type = Type.GetType(typeName: definition.FullTypeName(), throwOnError: false);
        return ((object)type == null) ? null : new MetadataContainer(type: type, isEntity: true, hasEndpoint: true);
    }

    private static IEnumerable<OperationContainer> GetBaseCrudOperations(MetadataContainer type) =>
        type.IsJoinEntity ? GetBaseCrudOperationsForJoinEntity(type: type) : GetBaseCrudOperationsForEntity(type: type);

    private static IEnumerable<OperationContainer> GetBaseCrudOperationsForJoinEntity(MetadataContainer type) =>
        new OperationContainer[4]
        {
            new OperationContainer
            {
                Name = "Add",
                Url = type.Category + "/" + type.Name,
                Queryable = true,
                HttpVerb = "POST",
                ReturnType = type,
                Parameters = new Dictionary<string, string> { { "body:entity", type.ServerType } }
            },
            new OperationContainer
            {
                Name = "Get",
                Url = type.Category + "/" + type.Name + "({Left=leftKey,Right=rightKey})",
                Queryable = true,
                HttpVerb = "GET",
                ReturnType = type,
                Parameters = new Dictionary<string, string> {
                {
                    "odata:key",
                    Type.GetType(typeName: type.ServerType)?.GetIdProperty()?.GetType()
            .FullName
                } }
            },
            new OperationContainer
            {
                Name = "Get All",
                Url = type.Category + "/" + type.Name,
                Queryable = true,
                HttpVerb = "GET",
                ReturnType = type
            },
            new OperationContainer
            {
                Name = "Delete",
                Url = type.Category + "/" + type.Name + "({Left=leftKey,Right=rightKey})",
                HttpVerb = "DELETE"
            }
        };

    private static IEnumerable<OperationContainer> GetBaseCrudOperationsForEntity(MetadataContainer type) =>
        new OperationContainer[5]
        {
            new OperationContainer
            {
                Name = "Add",
                Url = type.Category + "/" + type.Name,
                Queryable = true,
                HttpVerb = "POST",
                ReturnType = type,
                Parameters = new Dictionary<string, string> { { "body:entity", type.ServerType } }
            },
            new OperationContainer
            {
                Name = "Update",
                Url = type.Category + "/" + type.Name + "({key})",
                Queryable = true,
                HttpVerb = "PUT",
                ReturnType = type,
                Parameters = new Dictionary<string, string>
                {
                    {
                        "odata:key",
                        Type.GetType(typeName: type.ServerType)?.GetIdProperty()?.GetType()
            .FullName
                    },
                    { "body:entity", type.ServerType }
                }
            },
            new OperationContainer
            {
                Name = "Get",
                Url = type.Category + "/" + type.Name + "({key})",
                Queryable = true,
                HttpVerb = "GET",
                ReturnType = type,
                Parameters = new Dictionary<string, string> {
                {
                    "odata:key",
                    Type.GetType(typeName: type.ServerType)?.GetIdProperty()?.GetType()
            .FullName
                } }
            },
            new OperationContainer
            {
                Name = "Get All",
                Url = type.Category + "/" + type.Name,
                Queryable = true,
                HttpVerb = "GET",
                ReturnType = type
            },
            new OperationContainer
            {
                Name = "Delete",
                Url = type.Category + "/" + type.Name + "({key})",
                HttpVerb = "DELETE"
            }
        };

}