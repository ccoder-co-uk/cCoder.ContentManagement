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
            Type clrType = GetClrType(model, item.EntityType);
            if ((object)clrType != null)
            {
                list.Add(model.GetExtendedMetadataForType(contextName, clrType));
            }
        }
        foreach (IEdmSchemaType item2 in model.SchemaElements.OfType<IEdmSchemaType>())
        {
            if (item2 is IEdmComplexType || item2 is IEdmEntityType)
            {
                Type clrType2 = GetClrType(model, item2);
                if ((object)clrType2 != null)
                {
                    bool hasEndpoint = model.EntityContainer.FindEntitySet(clrType2.Name) != null;
                    list.Add(model.GetExtendedMetadataForType(contextName, clrType2, hasEndpoint));
                }
            }
        }
        return list.DistinctBy((ExtendedMetadataContainer type) => type.ServerTypeName);
    }

    public static ExtendedMetadataContainer GetExtendedMetadataForType(this IEdmModel model, string context, Type type, bool hasEndpoint = true)
    {
        ExtendedMetadataContainer result = new ExtendedMetadataContainer(type, isEntity: true, hasEndpoint)
        {
            Category = context
        };
        IEdmEntitySet edmEntitySet = model.EntityContainer.FindEntitySet(type.Name);
        if (edmEntitySet == null)
        {
            result.HasEndpoint = false;
            return result;
        }
        IEnumerable<OperationContainer> second = from operation in model.FindDeclaredBoundOperations(edmEntitySet.Type)
                                                 select new OperationContainer
                                                 {
                                                     Name = operation.Name,
                                                     Url = $"{result.Category}/{type.Name}/{operation.Name}()",
                                                     Queryable = operation.IsFunction(),
                                                     HttpVerb = (operation.IsFunction() ? "GET" : "POST"),
                                                 ReturnType = BuildMetaFor(operation.GetReturn()?.Type?.Definition),
                                                 Parameters = (from parameter in operation.Parameters?.Where((IEdmOperationParameter parameter) => parameter.Name != "bindingParameter")
                                                               select new
                                                               {
                                                                       Name = parameter.Name,
                                                                   TypeName = parameter.Type.FullName()
                                                               }).ToDictionary(item => item.Name, item => item.TypeName)
                                                 };
        result.Operations = GetBaseCrudOperations(result).Union(second).ToList();
        return result;
    }

    private static Type GetClrType(IEdmModel model, IEdmSchemaType edmType)
    {
        return model.GetAnnotationValue<ClrTypeAnnotation>(edmType)?.ClrType;
    }

    private static MetadataContainer BuildMetaFor(IEdmType definition)
    {
        if (definition == null || definition.TypeKind != EdmTypeKind.Collection)
        {
            return null;
        }
        Type type = Type.GetType(definition.FullTypeName(), throwOnError: false);
        return ((object)type == null) ? null : new MetadataContainer(type, isEntity: true, hasEndpoint: true);
    }

    private static IEnumerable<OperationContainer> GetBaseCrudOperations(MetadataContainer type)
    {
        return type.IsJoinEntity ? GetBaseCrudOperationsForJoinEntity(type) : GetBaseCrudOperationsForEntity(type);
    }

    private static IEnumerable<OperationContainer> GetBaseCrudOperationsForJoinEntity(MetadataContainer type)
    {
        return new OperationContainer[4]
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
                    Type.GetType(type.ServerType)?.GetIdProperty()?.GetType().FullName
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
    }

    private static IEnumerable<OperationContainer> GetBaseCrudOperationsForEntity(MetadataContainer type)
    {
        return new OperationContainer[5]
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
                        Type.GetType(type.ServerType)?.GetIdProperty()?.GetType().FullName
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
                    Type.GetType(type.ServerType)?.GetIdProperty()?.GetType().FullName
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
}
