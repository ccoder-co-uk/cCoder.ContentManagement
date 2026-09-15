// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.RegularExpressions;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class MarkupRenderService
{
    private const string ComponentPattern =
        "\\[component\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\](?<options>[^\\]]*)\\]";

    private TagHandlingOperation RenderComponentTagHandlingOperationCore(
        TagHandlingOperation tagHandlingOperation)
    {

        if (tagHandlingOperation.Editable)
        {
            return tagHandlingOperation;
        }

        tagHandlingOperation.Content = regularExpressionBroker.Replace(
            input: tagHandlingOperation.Content,
            pattern: ComponentPattern,
            evaluator: (value, groups) => ReplaceComponentTag(
                operation: tagHandlingOperation,
                match: new RegularExpressionMatch
                {
                    Value = value,
                    Groups = groups
                }));

        return tagHandlingOperation;
    }

    private string ReplaceComponentTag(
        TagHandlingOperation operation,
        RegularExpressionMatch match)
    {
        string name = match.Groups["name"];

        PageRenderComponent component = ResolveComponent(
            session: operation.Session,
            name: name);

        if (component is null)
        {
            return "[[Missing Component:" + name + "]]";
        }

        string[] options = match.Groups["options"]
            .Split(
                separator: ' ',
                options: StringSplitOptions.RemoveEmptyEntries);

        string optionalClass = string.Join(
            separator: " ",
            values: options
                .Where(predicate: option => option.StartsWith(value: "class="))
                .Select(selector: option => option.Replace(
                    oldValue: "class=",
                    newValue: string.Empty)));

        string otherOptions = string.Join(
            separator: " ",
            values: options.Where(predicate: option =>
                !option.StartsWith(value: "class=")));

        string token = $"[[render-fragment:{Guid.NewGuid():N}]]";

        operation.Fragments.Add(item: new TagHandlingFragment
        {
            Token = token,
            Operation = new TagHandlingOperation
            {
                Session = operation.Session,
                ResourceKey = component.ResourceKey,
                Content = $"<section name='{component.Name}' class='component {optionalClass}' data-id='{component.Id}' data-resource-key='{component.ResourceKey}' {otherOptions}>\n                        {component.Content}\n                        <script type='text/javascript' nonce='{ContentSecurityPolicyNonceContract.Placeholder}'>{component.Script}</script>\n                    </section>",
                AllowContentTags = false,
                Editable = operation.Editable,
                Replacements = operation.Replacements,
                Fragments = new List<TagHandlingFragment>()
            }
        });

        return token;
    }

    private PageRenderComponent ResolveComponent(
        RenderSession session,
        string name)
    {
        if (session.ComponentsByName.TryGetValue(
            key: name,
            value: out PageRenderComponent component))
        {
            return component;
        }

        Component dataComponent = contentRenderBroker.GetComponent(
            appId: session.Request.AppId,
            name: name);

        if (dataComponent is not null)
        {
            component = new PageRenderComponent
            {
                Id = dataComponent.Id,
                Name = dataComponent.Name ?? string.Empty,
                ResourceKey = dataComponent.ResourceKey ?? string.Empty,
                Content = dataComponent.Content ?? string.Empty,
                Script = dataComponent.Script ?? string.Empty
            };

            session.ComponentsByName[name] = component;
            return component;
        }

        return session.CommonComponentsByName.TryGetValue(
            key: name,
            value: out component)
                ? component
                : null;
    }
}