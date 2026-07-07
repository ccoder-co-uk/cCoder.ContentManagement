using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Exposures.Setup;

public static class UIBaseline
{
    public static Package[] Packages => [
        Components,
        Pages,
        Resources,
        Layouts,
        Templates,
        Scripts,
        PageRoles
    ];

    static Package Components => new()
    {
        Name = "Content Management Components",
        Category = "CMS",
        Description = "Content Management Components.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "AppManagement",
  "Key": "CMS",
  "ResourceKey": "CMS",
  "Script": "AppManagement = {\n    toolbar: true,\n\n    init: async function (app, container) {\n        app = app || session.app;\n        container = container || $(\".component[name=AppManagement]\")[0];\n        app = await api.get(\"ContentManagement/App(\" + app.Id + \")\");\n        app.Config = JSON.parse(app.ConfigJson);\n        if (AppManagement.toolbar) {\n            $(\"[name=appToolbar]\").kendoToolBar({\n                items: [\n                    {\n                        text: \"[resource_displayname[save]]\",\n                        type: \"button\",\n                        template: `\n                            <div class=\"btn-group btn-group-sm\">\n                                <button class=\"btn btn-primary\" name=\"appSave\">\n                                    <span class='k-icon k-i-save'></span> [resource_displayname[Save]]\n                                </button>\n                                <button class=\"btn btn-primary\" name=\"appMigrate\" data-bs-toggle=\"modal\" data-bs-target=\"#app-migrator-model\">\n                                    <span class='k-icon k-i-arrow-up'></span> [resource_displayname[Migrate]]\n                                </button>\n                            </div>`\n                    }\n                ]\n            });\n        } else {\n            $(\"[name=appToolbar]\", container).remove();\n        }\n\n        this.initNavTabIds(app, container);\n        await this.addClickListenerForComponents(app, container);\n\n        if ($(\".tab-pane[name=config] > div[name=editor]\", container)[0].childElementCount === 0) {\n            var configEditor = new MonacoEditor($(\".tab-pane[name=config] > div[name=editor]\", container)[0], {\n                code: app.ConfigJson,\n                language: \"json\",\n                automaticLayout: true\n            });\n            configEditor.onChange = () => app.ConfigJson = configEditor.getValue();\n            configEditor.init();\n            $(\".tab-pane[name=config] > div[name=editor]\", container).data(\"configEditor\", configEditor);\n            $(\"[name=appSave]\", container).click(() => AppManagement.save(app, configEditor));\n            $(\"[name=appMigrate]\", container).click(() => AppManagement.migrate(app, container));\n        }\n    },\n\n    initNavTabIds: function(app, container) {\n        var nav = $('#app-management-nav-tab', container);\n        nav.attr('id', `${nav.attr('id')}-${app.Id}`);\n        var content = $('#app-management-nav-tabContent', container);\n        content.attr('id', `${content.attr('id')}-${app.Id}`);\n\n        var buttons = $('button[role=tab]', nav);\n        buttons.each((button) => {\n            var button = $(buttons[button], container);\n            button.attr('data-bs-target', `${button.attr('data-bs-target')}-${app.Id}`);\n        });\n\n        var tabs = $('.tab-pane', content);\n        tabs.each((tab) => {\n            var tab = $(tabs[tab], container);\n            tab.attr('id', `${tab.attr('id')}-${app.Id}`);\n        });\n    },\n\n    addClickListenerForComponents: async function(app, container) {\n        var buttons = $('button[role=tab]', container);\n        buttons.each((button) => {\n            var button = $(buttons[button], container);\n            var target = $(button.attr('data-bs-target'));\n            var component = target.attr('data-component');\n\n            if(component != null) {\n                button.click(async () => {\n                    console.log(`Loading component: ${component}`);\n\n                    var existing = $(`.component[name=${component}]`, $(target, container));\n\n                    if(existing.length == 0) {\n                        await loadComponent($(target, container), component, async (c) => {\n                            await c.init(app, $(`.component[name=${component}]`, $(target, container)));\n                        });\n                    }\n                });\n            }\n        });\n    },\n\n    migrate: async function (app, container) {\n        // var d = new Dialog({ title: \"[resource_displayname[migrate]]\", width: 620, height: \"auto\" });\n        // d.template = $(\"[name=appMigratorComponent]\").first().html();\n        var element = $(\"[name=AppMigrator]\", container);\n        await AppMigrator.init(app, element, app);\n    },\n\n    save: async function (app, configEditor) {\n        var newApp = {\n            ConfigJson: configEditor.getValue(),\n            Cultures: app.Cultures,\n            DefaultCultureId: app.DefaultCultureId,\n            DefaultTheme: app.DefaultTheme,\n            Domain: app.Domain,\n            Id: app.Id,\n            Name: app.Name,\n            TenantId: app.TenantId\n        };\n        try {\n            JSON.parse(newApp.ConfigJson);\n        } catch(err) {\n            error(\"[resource_displayname[AppConfigParsingError]]\");\n            return;\n        }\n        notification.info(\"[resource_displayname[saving]]\");\n        await api.update(\"ContentManagement/App(\" + app.Id + \")\", newApp).then(() => {\n            notification.success(\"[resource_displayname[saved]]\")\n        }).catch((err) => error(err));\n    }\n};",
  "Content": "<div class=\"k-toolbar\" name=\"appToolbar\"></div>\n\n<div class=\"hidden\">\n   [component[AppMigrator]]\n</div>\n\n<div class=\"tab-control\" name=\"tabs\">\n   <nav>\n      <div class=\"nav nav-tabs\" id=\"app-management-nav-tab\" role=\"tablist\">\n         <button class=\"nav-link bg active\" id=\"app-management-config-tab\" data-bs-toggle=\"tab\" data-bs-target=\"#app-management-config\" type=\"button\" role=\"tab\" aria-controls=\"app-management-config\" aria-selected=\"true\">\n            <span class=\"k-icon k-i-file-config\"></span>[resource_displayname[configuration]]\n         </button>\n         <button class=\"nav-link bg\" id=\"app-management-theming-tab\" data-bs-toggle=\"tab\" data-bs-target=\"#app-management-theming\" type=\"button\" role=\"tab\" aria-controls=\"app-management-theming\" aria-selected=\"false\" tabindex=\"-1\">\n            <span class=\"k-icon k-i-palette\"></span>[resource_displayname[theming]]\n         </button>\n         <button class=\"nav-link bg\" id=\"app-management-cultures-tab\" data-bs-toggle=\"tab\" data-bs-target=\"#app-management-cultures\" type=\"button\" role=\"tab\" aria-controls=\"app-management-cultures\" aria-selected=\"false\" tabindex=\"-1\">\n            <span class=\"k-icon k-i-globe\"></span>[resource_displayname[cultures]]\n         </button>\n         <button class=\"nav-link bg\" id=\"app-management-layouts-tab\" data-bs-toggle=\"tab\" data-bs-target=\"#app-management-layouts\" type=\"button\" role=\"tab\" aria-controls=\"app-management-layouts\" aria-selected=\"false\" tabindex=\"-1\">\n            <span class=\"k-icon k-i-grid-layout\"></span>[resource_displayname[layouts]]\n         </button>\n         <button class=\"nav-link bg\" id=\"app-management-templates-tab\" data-bs-toggle=\"tab\" data-bs-target=\"#app-management-templates\" type=\"button\" role=\"tab\" aria-controls=\"app-management-templates\" aria-selected=\"false\" tabindex=\"-1\">\n            <span class=\"k-icon k-i-css\"></span>[resource_displayname[templates]]\n         </button>\n         <button class=\"nav-link bg\" id=\"app-management-components-tab\" data-bs-toggle=\"tab\" data-bs-target=\"#app-management-components\" type=\"button\" role=\"tab\" aria-controls=\"app-management-components\" aria-selected=\"false\" tabindex=\"-1\">\n            <span class=\"k-icon k-i-source-code\"></span>[resource_displayname[components]]\n         </button>\n         <button class=\"nav-link bg\" id=\"app-management-resources-tab\" data-bs-toggle=\"tab\" data-bs-target=\"#app-management-resources\" type=\"button\" role=\"tab\" aria-controls=\"app-management-resources\" aria-selected=\"false\" tabindex=\"-1\">\n            <span class=\"k-icon k-i-foreground-color\"></span>[resource_displayname[resources]]\n         </button>\n         <button class=\"nav-link bg\" id=\"app-management-roles-tab\" data-bs-toggle=\"tab\" data-bs-target=\"#app-management-roles\" type=\"button\" role=\"tab\" aria-controls=\"app-management-roles\" aria-selected=\"false\" tabindex=\"-1\">\n            <span class=\"k-icon k-i-lockIcon\"></span>[resource_displayname[security]]\n         </button>\n         <button class=\"nav-link bg\" id=\"app-management-scheduling-tab\" data-bs-toggle=\"tab\" data-bs-target=\"#app-management-scheduling\" type=\"button\" role=\"tab\" aria-controls=\"app-management-scheduling\" aria-selected=\"false\" tabindex=\"-1\">\n            <span class=\"k-icon k-i-clock\"></span>[resource_displayname[scheduling]]\n         </button>\n         <button class=\"nav-link bg\" id=\"app-management-mailmanagement-tab\" data-bs-toggle=\"tab\" data-bs-target=\"#app-management-mailmanagement\" type=\"button\" role=\"tab\" aria-controls=\"app-management-mailmanagement\" aria-selected=\"false\" tabindex=\"-1\">\n            <span class=\"k-icon k-i-inbox\"></span>[resource_displayname[mailmanagement]]\n         </button>\n         <button class=\"nav-link bg\" id=\"app-management-logstream-tab\" data-bs-toggle=\"tab\" data-bs-target=\"#app-management-logstream\" type=\"button\" role=\"tab\" aria-controls=\"app-management-logstream\" aria-selected=\"false\" tabindex=\"-1\">\n            <span class=\"k-icon k-i-clipboard-text\"></span>[resource_displayname[logstream]]\n         </button>\n      </div>\n   </nav>\n\n   <div class=\"tab-content\" id=\"app-management-nav-tabContent\">\n      <div class=\"tab-pane fade active show\" id=\"app-management-config\" role=\"tabpanel\" aria-labelledby=\"app-management-config-tab\" name=\"config\">\n         <div name=\"editor\" class=\"editor\" style=\"min-height: 500px;\"></div>\n      </div>\n      <div class=\"tab-pane fade\" id=\"app-management-theming\" role=\"tabpanel\" aria-labelledby=\"app-management-theming-tab\" name=\"theming\" data-component=\"AppTheming\"></div>\n      <div class=\"tab-pane fade\" id=\"app-management-cultures\" role=\"tabpanel\" aria-labelledby=\"app-management-cultures-tab\" name=\"cultures\" data-component=\"CultureManagement\"></div>\n      <div class=\"tab-pane fade\" id=\"app-management-layouts\" role=\"tabpanel\" aria-labelledby=\"app-management-layouts-tab\" name=\"layouts\" data-component=\"LayoutManagement\"></div>\n      <div class=\"tab-pane fade\" id=\"app-management-templates\" role=\"tabpanel\" aria-labelledby=\"app-management-templates-tab\" name=\"templates\" data-component=\"TemplateManagement\"></div>\n      <div class=\"tab-pane fade\" id=\"app-management-components\" role=\"tabpanel\" aria-labelledby=\"app-management-components-tab\" name=\"components\" data-component=\"ComponentManagement\"></div>\n      <div class=\"tab-pane fade\" id=\"app-management-resources\" role=\"tabpanel\" aria-labelledby=\"app-management-resources-tab\" name=\"resources\" data-component=\"ResourceManagement\"></div>\n      <div class=\"tab-pane fade\" id=\"app-management-roles\" role=\"tabpanel\" aria-labelledby=\"app-management-roles-tab\" name=\"roles\" data-component=\"RoleManagement\"></div>\n      <div class=\"tab-pane fade\" id=\"app-management-scheduling\" role=\"tabpanel\" aria-labelledby=\"app-management-scheduling-tab\" name=\"scheduling\" data-component=\"Scheduling\"></div>\n      <div class=\"tab-pane fade\" id=\"app-management-mailmanagement\" role=\"tabpanel\" aria-labelledby=\"app-management-mailmanagement-tab\" name=\"mailmanagement\" data-component=\"MailManagement\"></div>\n      <div class=\"tab-pane fade\" id=\"app-management-logstream\" role=\"tabpanel\" aria-labelledby=\"app-management-logstream-tab\" name=\"logstream\" data-component=\"LogStream\"></div>\n   </div>\n</div>",
  "LastUpdated": "2024-11-19T18:18:30.4244504+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "AppMigrator",
  "Key": "CMS",
  "ResourceKey": "CMS",
  "Script": "AppMigrator = {\r\n    init: async function (app, container, appToMigrate, dialog) {\r\n        app = app || session.app;\r\n        container = container || $(\".component[name=AppMigrator]\");\r\n        if(!appToMigrate)\r\n            return;\r\n\r\n        var environments = JSON.parse(appToMigrate.ConfigJson).Deployment.Targets;\r\n        var apiSelect = $('[name=api]', container);\r\n\r\n        environments.forEach((env) => {\r\n            apiSelect.append('<option value=\"' + env.Api + '\">' + env.EnvironmentName + '</option>');\r\n        });\r\n\r\n        var packages = await api.get(\"ContentManagement/App(\" + appToMigrate.Id + \")/Export()\");\r\n        packages.value.push({ Name: \"DMS\" });\r\n        var packageGrid = new GridWidget($(\"[name=selectPackagesGrid]\", container), {\r\n            data: packages.value,\r\n            sort: {\r\n                field: \"Name\",\r\n                dir: \"asc\"\r\n            }\r\n        });\r\n        packageGrid.groupable = false;\r\n        packageGrid.pageable = false;\r\n        packageGrid.editable = false;\r\n        packageGrid.columns = [\r\n            {\r\n                selectable: true,\r\n                width: 50\r\n            },\r\n            {\r\n                field: \"Name\"\r\n            }\r\n        ];\r\n        packageGrid.init(() => {\r\n            packageGrid.kendoObject.select(packageGrid.kendoObject.tbody.find(\">tr\"));\r\n        });\r\n        $(\"button[name=migrate]\", container).off().on(\"click\", async function (e) {\r\n            if(dialog)\r\n                dialog.events.close();\r\n            \r\n            await AppMigrator.migrate(app, container, appToMigrate, packageGrid);\r\n        });\r\n    },\r\n\r\n    migrate: async function (app, container, sourceApp, packageGrid) {\r\n        var packagesSelected = [];\r\n        packageGrid.kendoObject.select().each(function () {\r\n            packagesSelected.push(packageGrid.kendoObject.dataItem(this).Name);\r\n        });\r\n        var functions = {\r\n            [script[MigrateApp]]\r\n        };\r\n        var vars = {\r\n            Domain: JSON.parse(sourceApp.ConfigJson).Deployment.Targets.filter(\r\n                r => r.Api == $(\"[name=api]\", container).val()\r\n            )[0].Domain,\r\n            RemoteAuth: {\r\n                User: session.user,\r\n                Pass: $(\"[name=pass]\", container).val(),\r\n                Api: JSON.parse(sourceApp.ConfigJson).Deployment.Targets.filter(\r\n                    r => r.Api == $(\"[name=api]\", container).val()\r\n                )[0].Api,\r\n            },\r\n            SelectedPackageNames: packagesSelected,\r\n            SourceApp: sourceApp\r\n        };\r\n        if(packagesSelected.filter(r => r == \"DMS\").length > 0) {\r\n            vars.DMSPaths = JSON.parse(sourceApp.ConfigJson).Deployment.DMS;\r\n        }\r\n        var d = new Dialog({\r\n            width: 800,\r\n            height: 500,\r\n            title: \"[resource_displayname[migrating]]\"\r\n        });\r\n        d.init(async () => {\r\n            await loadComponent($(d.element), \"ScriptRunner\", (scriptRunner) =>\r\n                scriptRunner.init(app, $(\".component[name=ScriptRunner]\", d.element), vars, functions, d));\r\n        });\r\n    }\r\n}",
  "Content": "<div class=\"modal fade\" id=\"app-migrator-model\" tabindex=\"-1\" aria-labelledby=\"app-migrator-model-label\" aria-hidden=\"true\">\r\n    <div class=\"modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable\">\r\n        <div class=\"modal-content\">\r\n            <div class=\"modal-header\">\r\n                <h3 id=\"app-migrator-model-label\">\r\n                    App Migrator\r\n                </h1>\r\n                <button type=\"button\" class=\"btn-close\" data-bs-dismiss=\"modal\" aria-label=\"Close\"></button>\r\n            </div>\r\n            <div class=\"modal-body\">\r\n                <div class=\"input-group input-group-sm\">\r\n                    <span class=\"input-group-text\" id=\"app-migrator-environment\">[resource_displayname[Environment]]</span>\r\n                    <select class=\"form-select\" name=\"api\">\r\n                        <option selected disabled>Please select...</option>\r\n                    </select>\r\n                </div>\r\n                <div class=\"input-group input-group-sm\">\r\n                    <span class=\"input-group-text\" id=\"app-migrator-password\">[resource_displayname[ConfirmPassword]]</span>\r\n                    <input type=\"password\" class=\"form-control\" name=\"pass\" />\r\n                </div>\r\n                <div name=\"selectPackagesGrid\"></div>\r\n            </div>\r\n            <div class=\"modal-footer\">\r\n                <button type=\"button\" name=\"migrate\" class=\"btn btn-sm btn-primary\">\r\n                    <span class=\"k-icon k-i-arrow-up\"></span>[resource_displayname[Migrate]]\r\n                </button>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>",
  "LastUpdated": "2024-06-10T14:56:38.4925338+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "AppThemeList",
  "Key": "CMS",
  "ResourceKey": "CMS",
  "Script": "AppThemeList = {\n    init: async function(app, container) {\n        app = app || session.app;\n        container = container || $(\".component[name=AppThemeList]\");\n        var appKeys = Object.keys(app.Config.Themes);\n        var appThemes = appKeys.map(k => ({\n            name: k,\n            theme: app.Config.Themes[k]\n        }));\n        var themeGrid = new GridWidget(container, appThemes);\n        themeGrid.toolbar = [\n\t\t\t{\n\t\t\t\ttemplate: `<div class=\"btn-group btn-group-sm\">\n                    <button class=\"btn btn-primary\" name=\"add\">\n                        <span class=\"k-icon k-i-plus\"></span>[resource_displayname[new]]\n                    </button>\n                </div>`\n\t\t\t}\n\t\t];\n        themeGrid.groupable = false;\n        themeGrid.pageable = false;\n        themeGrid.sortable = true;\n        themeGrid.columns = [{ field: \"name\", title: \"[resource_displayname[name]]\" }];\n        themeGrid.commands.push({ name: \"edit\", icon: \"k-i-pencil\", text: \"[resource_displayname[edit]]\" });\n        themeGrid.commands.push({ name: \"remove\", icon: \"k-i-trash\", text: \"[resource_displayname[delete]]\" });\n        themeGrid.dataBound = function () {\n            $(\"[name=edit]\", themeGrid.gridElement).on(\"click\", (e) => AppThemeList.editTheme(e, app, themeGrid));\n            $(\"[name=remove]\", themeGrid.gridElement).on(\"click\", (e) => AppThemeList.removeTheme(e, app, themeGrid));\n        };\n        await themeGrid.init();\n        $(\"button[name=add]\", themeGrid.gridElement).on(\"click\", () => AppThemeList.addTheme(app, themeGrid));\n    },\n\n    editTheme: async function(e, app, themeGrid) {\n        e.preventDefault();\n        var item = themeGrid.dataItem($(e.currentTarget).closest(\"tr\"));\n        var d = new BootstrapDialog({\n            title: \"[resource_displayname[edittheme]]\"\n        });\n        d.name = \"EditTheme\";\n        d.template = $(\"[name=editThemeTemplate]\").first().html();\n        d.footer = `\n            <button type=\"button\" class=\"btn btn-sm btn-primary\" name=\"save\">\n                <span class=\"k-icon k-i-save\"></span>[resource_displayname[save]]\n            </button>    \n        `;\n        d.width = 'xl'\n        d.init(async () => {\n            var c = await loadComponent($(\"[name=themeContent]\", d.element), \"ThemeBuilder\");\n            c.init(app, $(\".component[name=ThemeBuilder]\", d.element), item.theme);\n            $(\"[name=save]\", d.element).on(\"click\",  async function(e) {\n                item.theme = ThemeBuilder.build($(\".component[name=ThemeBuilder]\", d.element));\n                await AppThemeList.save(app, themeGrid);\n            });\n        });\n        d.modal.show();\n    },\n\n    save: async function(app, themeGrid) {\n        var themes = themeGrid.dataSource().data();\n        var dictionary = {};\n        for(let i = 0; i < themes.length; i++) {\n            dictionary[themes[i].name] = themes[i].theme;\n        }\n        var app = await api.get(\"ContentManagement/App(\" + app.Id + \")\");\n        var config = JSON.parse(app.ConfigJson);\n        config.Themes = dictionary;\n        app.ConfigJson = JSON.stringify(config, null, 4);\n        await api.put(\"ContentManagement/App(\" + app.Id + \")\", app).then(() => {\n            notification.success(\"[resource_displayname[saved]]\");\n            var appKeys = Object.keys(config.Themes);\n            var appThemes = appKeys.map(k => ({name: k, theme: config.Themes[k]}));\n            themeGrid.kendoObject.setDataSource({data: appThemes});\n        }).catch((err) => error(err));\n    },\n    \n    addTheme: function (app, themeGrid) {\n        var model = kendo.observable({ name: \"\", theme: JSON.parse(JSON.stringify(app.Config.Themes.Default)) });\n        var addThemeDialog = new Dialog({ title: \"[resource_displayname[addtheme]]\" });\n        addThemeDialog.template = $(\"[name=addThemeDialog]\").html();\n        addThemeDialog.events.confirm = async function () {\n            themeGrid.dataSource().insert(0, model);\n            await AppThemeList.save(app, themeGrid);\n            addThemeDialog.events.close();\n        };\n        addThemeDialog.init(() => {  \n            kendo.bind(addThemeDialog.element, model);  \n        });\n    },\n\n    removeTheme: async function (e, app, themeGrid) {\n        e.preventDefault();\n\n\t\tvar d = new ConfirmDialog({\n\t\t\ttitle: \"[resource_displayname[areyousure]]\",\n\t\t\tconfirm: \"[resource_displayname[confirm]]\",\n\t\t\tclose: \"[resource_displayname[close]]\"\n\t\t});\n\n\t\td.events.confirm = async function () {\n\t\t\tvar item = themeGrid.dataItem($(e.currentTarget).closest(\"tr\"));\n            themeGrid.dataSource().remove(item);\n            await AppThemeList.save(app, themeGrid);\n\t\t\td.events.close();\n\t\t};\n\n\t\td.init();\n    }\n}",
  "Content": "<script type=\"text/template\" name=\"addThemeDialog\">\n    <div class=\"input-group input-group-sm mb-1\">\n        <span class=\"input-group-text\">[resource_displayname[name]]</span>\n        <input type=\"text\" class=\"form-control\" name=\"name\" data-bind=\"value: name\" />\n    </div>\n\n    <hr />\n\n    <button class=\"btn btn-sm btn-primary float-end\" name=\"confirm\">\n        <span class=\"k-icon k-i-plus\"></span>[resource_displayname[add]]\n    </button>\n</script>\n\n<script type=\"text/template\" name=\"editThemeTemplate\">\n    <div name=\"themeContent\"></div>\n</script>\n\n<div class=\"modal fade\" id=\"addThemeModal\" tabindex=\"-1\" aria-labelledby=\"add-theme-label\" aria-hidden=\"true\">\n    <div class=\"modal-dialog\">\n        <div class=\"modal-content\">\n            <div class=\"modal-header\">\n                <h5 class=\"modal-title\" id=\"add-theme-label\">[resource_displayname[newtheme]]</h5>\n                <button type=\"button\" class=\"btn-close\" data-bs-dismiss=\"modal\" aria-label=\"Close\"></button>\n            </div>\n            <div class=\"modal-body\">\n                <form class=\"form\" name=\"add-theme\">\n                    \n                </form>\n            </div>\n            <div class=\"modal-footer\">\n                <button type=\"button\" class=\"btn btn-secondary\" data-bs-dismiss=\"modal\">\n                    Close\n                </button>\n                <button type=\"button\" class=\"btn btn-primary\">\n                    Save changes\n                </button>\n            </div>\n        </div>\n    </div>\n</div>",
  "LastUpdated": "2024-11-19T18:18:30.45521+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "AppTheming",
  "Key": "CMS",
  "ResourceKey": "CMS",
  "Script": "AppTheming = {\n    init: async function(app, container) {\n        app = app || session.app;\n        container = container || $(\".component[name=AppTheming]\");\n        \n        var themeListContainer = $('[name=app-theme-list]', container);\n        await loadComponent(themeListContainer, 'AppThemeList', async (c) => {\n            await c.init(app, $(`.component[name=AppThemeList]`, themeListContainer));\n        });\n    \n        AppTheming.wireUpEvents(container);\n    },\n    \n    wireUpEvents: function(container) {\n        AppTheming.wireUpFileUploading(\n            $('input[name=brand-logo]', container),\n            $('img[name=brand-logo-preview]', container),\n            'Content/CompanyLogo.png');\n            \n        AppTheming.wireUpFileUploading(\n            $('input[name=project-logo]', container),\n            $('img[name=project-logo-preview]', container),\n            'Content/CompanyLogo2.png');\n        \n        AppTheming.wireUpFileUploading(\n            $('input[name=animation]', container),\n            $('img[name=animation-preview]', container),\n            'Content/background.gif');\n    },\n\n    wireUpFileUploading: function(element, preview, path) { \n        element.on('change', (e) => AppTheming.handleFileUploadEvent(e, preview, path));\n        element.on('dragend',(e) => AppTheming.handleFileUploadEvent(e, preview, path));\n    },\n\n    handleFileUploadEvent: async function(event, element, path) {\n        if(event.target.files.length > 1) {\n            notification.error('[resource_displayname[onlyonefile]]');\n            return;\n        }\n\n        var file = event.target.files[0] || null;\n        if(file == null)\n            return;\n        \n        AppTheming.handleFileUploadPreview(file, element);\n        await api.file.upload(path, file);\n    },\n\n    handleFileUploadPreview: function(file, element) {\n        var preview = URL.createObjectURL(file);\n        element.attr('src', preview);\n    }\n};",
  "Content": "<div class=\"row\">\n    <div class=\"col-md-6\">\n        <div class=\"input-group input-group-sm mb-1\">\n            <label class=\"input-group-text\" for=\"app-theming-brand-logo\">\n                [resource_displayname[brandLogo]]\n            </label>\n            <input type=\"file\" class=\"form-control\" name=\"brand-logo\" id=\"app-theming-brand-logo\" />\n            <span class=\"input-group-text file-upload-preview\"><img src=\"[app[root]]Api/DMS/Content/CompanyLogo.png\" name=\"brand-logo-preview\"</span>\n        </div>\n        <div class=\"input-group input-group-sm mb-1\">\n            <label class=\"input-group-text\" for=\"app-theming-project-logo\">\n                [resource_displayname[projectLogo]]\n            </label>\n            <input type=\"file\" class=\"form-control\" name=\"project-logo\" id=\"app-theming-project-logo\" />\n            <span class=\"input-group-text file-upload-preview\"><img src=\"[app[root]]Api/DMS/Content/CompanyLogo2.png\" name=\"project-logo-preview\"></span>\n        </div>\n        <div class=\"input-group input-group-sm mb-1\">\n            <label class=\"input-group-text\" for=\"app-theming-animation\">\n                [resource_displayname[animation]]\n            </label>\n            <input type=\"file\" class=\"form-control\" name=\"animation\" id=\"app-theming-animation\" />\n            <span class=\"input-group-text file-upload-preview\"><img src=\"[app[root]]Api/DMS/Content/background.gif\" name=\"animation-preview\"></span>\n        </div>\n    </div>\n    <div class=\"col-md-6\">\n        <h4>\n            [resource_displayname[themelist]]\n        </h4>\n        <div name=\"app-theme-list\"></div>\n    </div>\n</div>",
  "LastUpdated": "2024-11-19T18:18:30.4375728+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "Border",
  "Key": "Theming",
  "ResourceKey": "CMS",
  "Script": "Border = {\n    init: async function (app, container, observable) {\n        app = app || session.app;\n        container = container || $(\".component[name=Border]\");\n        if(!observable)\n            return;\n        \n        $(\"[name=borderStyleDropdown]\", container).kendoDropDownList({\n            dataTextField: \"text\",\n            dataValueField: \"name\",\n            change: function(e) {\n                e.preventDefault();\n                var existingBorderValue = observable.get(\"border.style\");\n                var parts = existingBorderValue.split(\" \");\n                parts[0] = this.value();\n                observable.set(\"border.style\", parts.join(\" \"));\n            },\n            dataSource: {\n                data: [\n                    { name: \"dotted\", text: \"[resource_displayname[dotted]]\" },\n                    { name: \"dashed\", text: \"[resource_displayname[dashed]]\" },\n                    { name: \"solid\", text: \"[resource_displayname[solid]]\" },\n                    { name: \"double\", text: \"[resource_displayname[double]]\" },\n                    { name: \"groove\", text: \"[resource_displayname[groove]]\" },\n                    { name: \"ridge\", text: \"[resource_displayname[ridge]]\" },\n                    { name: \"inset\", text: \"[resource_displayname[inset]]\" },\n                    { name: \"outset\", text: \"[resource_displayname[outset]]\" },\n                    { name: \"none\", text: \"[resource_displayname[none]]\" },\n                    { name: \"hidden\", text: \"[resource_displayname[hidden]]\" }\n                ]\n            }\n        });\n\n        $(\"[name=borderColorPicker]\", container).kendoColorPicker({\n            buttons: false,\n            value: observable.get(\"border.style\").split(\" \")[2],\n            change: function(e) {\n                e.preventDefault();\n                var existingBorderValue = observable.get(\"border.style\");\n                var parts = existingBorderValue.split(\" \");\n                parts[2] = this.value();\n                observable.set(\"border.style\", parts.join(\" \"));\n            }\n        });\n\n        await Border.initSliders(container, observable);\n    },\n\n    initSliders: async function(container, observable) {\n        $(\"[name=borderWidthSlider]\", container).kendoSlider({ \n            min: 0, \n            max: 20, \n            smallStep: 1, \n            largeStep: 2, \n            value: parseFloat(observable.get(\"border.style\").split(\" \")[1].replaceAll(\"px\", \"\")),\n            change: function(e) {\n                e.preventDefault();\n                var existingBorderValue = (observable.get(\"border.style\") || \"solid 0px\");\n                var parts = existingBorderValue.split(\" \");\n                parts[1] = this.value() + \"px\";\n                observable.set(\"border.style\", parts.join(\" \"));\n                observable.set(\"border.width\", this.value() + \"px\");\n            }\n        });\n\n        $(\"[name=borderRadiusSlider]\", container).kendoSlider({ \n            min: 0, \n            max: 20, \n            smallStep: 1, \n            largeStep: 2, \n            value: parseFloat((observable.get(\"border.radius\") || \"0px\").replaceAll(\"px\", \"\")),\n            change: function(e) {\n                e.preventDefault();\n                var existingBorderValue = (observable.get(\"border.radius\") || \"0px\");\n                var parts = existingBorderValue.split(\" \");\n                parts[1] = this.value() + \"px\";\n                observable.set(\"border.radius\", this.value() + \"px\");\n            }\n        });\n    }\n}",
  "Content": "<div class=\"row\">\n    <div class=\"col-md-6\">\n        <div class=\"input-group input-group-sm mb-1\">\n            <span class=\"input-group-text\">[resource_displayname[style]]</span>\n            <input class=\"form-control\" name=\"borderStyleDropdown\" />\n        </div>\n\t\t\n        <div class=\"input-group input-group-sm mb-1\">\n            <span class=\"input-group-text\">[resource_displayname[width]]</span>\n            <input type=\"text\" class=\"form-control\" name=\"borderWidthSlider\" />\n        </div>\n\n        <div class=\"input-group input-group-sm mb-1\">\n            <span class=\"input-group-text\">[resource_displayname[colour]]</span>\n            <input class=\"form-control\" name=\"borderColorPicker\" />\n        </div>\n\t\t\n        <div class=\"input-group input-group-sm mb-1\">\n            <span class=\"input-group-text\">[resource_displayname[radius]]</span>\n            <input type=\"text\" class=\"form-control\" name=\"borderRadiusSlider\" />\n        </div>\n    </div>\n</div>",
  "LastUpdated": "2024-11-19T18:18:31.0891403+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "CalendarManagement",
  "Key": "CMS",
  "ResourceKey": "CMS",
  "Script": "CalendarManagement = {\n\tdays: {\n\t\t0: '[resource_displayname[sunday]]',\n\t\t1: '[resource_displayname[monday]]',\n\t\t2: '[resource_displayname[tuesday]]',\n\t\t3: '[resource_displayname[wednesday]]',\n\t\t4: '[resource_displayname[thursday]]',\n\t\t5: '[resource_displayname[friday]]',\n\t\t6: '[resource_displayname[saturday]]'\n\t},\n\tmonths: {\n\t\t1: \"[resource_displayname[month-1]]\",\n\t\t2: \"[resource_displayname[month-2]]\",\n\t\t3: \"[resource_displayname[month-3]]\",\n\t\t4: \"[resource_displayname[month-4]]\",\n\t\t5: \"[resource_displayname[month-5]]\",\n\t\t6: \"[resource_displayname[month-6]]\",\n\t\t7: \"[resource_displayname[month-7]]\",\n\t\t8: \"[resource_displayname[month-8]]\",\n\t\t9: \"[resource_displayname[month-9]]\",\n\t\t10: \"[resource_displayname[month-10]]\",\n\t\t11: \"[resource_displayname[month-11]]\",\n\t\t12: \"[resource_displayname[month-12]]\"\n\t},\n\n\trepeats: {\n\t\t\"never\": \"[resource_displayname[never]]\",\n\t\t\"weekly\": \"[resource_displayname[weekly]]\"\n\t},\n\n\tinit: async function (app, container, args) {\n\t\tapp = app || session.app;\n\t\tcontainer = container || $(\".component[name=CalendarManagement]\");\n\n\t\tif (args && args.fromDate && args.toDate) {\n\t\t\tif (args.toDate < args.fromDate) {\n\t\t\t\tlet copy = new Date(toDate.getTime());\n\t\t\t\targs.toDate = args.fromDate;\n\t\t\t\targs.fromDate = copy;\n\t\t\t}\n\t\t}\n\n\t\tvar currentMonth = new Date().getMonth() + 1;\n\t\tvar monthData = [];\n\t\tfor (const [key, value] of Object.entries(CalendarManagement.months)) {\n\t\t\tmonthData.push({\n\t\t\t\tName: value,\n\t\t\t\tNumber: key\n\t\t\t});\n\t\t}\n\t\tvar yearData = [];\n\t\tvar currentYear = new Date().getFullYear();\n\t\tfor (var i = currentYear - 10; i < currentYear + 10; i++) {\n\t\t\tyearData.push(i);\n\t\t}\n\n\t\tvar defaultArgs = {\n\t\t\tcalendarId: app.Config.Calendars.primary,\n\t\t\tmonth: parseInt($(\"[name=month]\", container).val()) - 1,\n\t\t\tyear: parseInt($(\"[name=year]\", container).val()),\n\t\t\treadOnly: (args && args.readOnly) ? true : false,\n\t\t\teventSource: (args && args.eventSource) ? args.eventSource : null,\n\t\t\tfromDate: (args && args.fromDate) ? args.fromDate : null,\n\t\t\ttoDate: (args && args.toDate) ? args.toDate : null,\n\t\t\tagendaView: (args && args.agendaView) ? true : false\n\t\t};\n\n\t\tvar monthDropdown = $(\"[name=month]\", container).kendoDropDownList({\n\t\t\tdataTextField: \"Name\",\n\t\t\tdataValueField: \"Number\",\n\t\t\tvalue: currentMonth,\n\t\t\tdataSource: monthData,\n\t\t\tchange: async function () {\n\t\t\t\tawait CalendarManagement.buildCalendar(container, defaultArgs);\n\t\t\t}\n\t\t}).data(\"kendoDropDownList\");\n\n\t\tvar yearDropdown = $(\"[name=year]\", container).kendoDropDownList({\n\t\t\tdataSource: yearData,\n\t\t\tvalue: currentYear,\n\t\t\tchange: async function () {\n\t\t\t\tawait CalendarManagement.buildCalendar(container, defaultArgs);\n\t\t\t}\n\t\t}).data(\"kendoDropDownList\");\n\n\t\tif (args && args.fromDate != null) {\n\t\t\tmonthDropdown.value(args.fromDate.getMonth());\n\t\t\tyearDropdown.value(args.fromDate.getFullYear());\n\t\t}\n\n\t\t// await CalendarManagement.initCalendar(container, defaultArgs());\n\t\t$(\"[name=newcalendar]\", container).on(\"click\", (e) => CalendarManagement.newCalendar(e, app));\n\t\t$(\"[name=setprimary]\", container).on(\"click\", (e) => CalendarManagement.setPrimary(e, app, container));\n\t\t$(\"[name=deletecalendar]\", container).on(\"click\", (e) => CalendarManagement.deleteCalendar(e, container));\n\t\t\n\t\tif (args && args.readOnly)\n\t\t\t$(\"[name=toolbar]\", container).remove();\n\t\t\n\t\tawait CalendarManagement.buildCalendar(container, defaultArgs);\n\t},\n\t\n\tinitialiseCalendarGrid: function(container) {\n\t\tvar calendar = $('[name=calendar]', container);\n\t\tvar calendarContent = '';\n\t\tvar week = `\n\t\t<div class=\"row\">\n\t\t\t<div class=\"col-calendar\" name=\"{WEEK}-0\" data-date=\"{FULLDATE}\">\n                <button class=\"btn btn-primary btn-sm float-end mt-1 me-1\" name=\"add-event\" data-date=\"{FULLDATE}\"><span class=\"k-icon k-i-plus\"></span>[resource_displayname[newevent]]</button>\n\t\t\t\t<div class=\"calendar-bar w-100\"><p name=\"day\">{DATE} [resource_displayname[sunday]]</p></div>\n\t\t\t\t<div class=\"events w-100\"></div>\n\t\t\t</div>\n\t\t\t<div class=\"col-calendar\" name=\"{WEEK}-1\" data-date=\"{FULLDATE}\">\n                <button class=\"btn btn-primary btn-sm float-end mt-1 me-1\" name=\"add-event\" data-date=\"{FULLDATE}\"><span class=\"k-icon k-i-plus\"></span>[resource_displayname[newevent]]</button>\n\t\t\t\t<div class=\"calendar-bar w-100\"><p name=\"day\">{DATE} [resource_displayname[monday]]</p></div>\n\t\t\t\t<div class=\"events w-100\"></div>\n\t\t\t</div>\n\t\t\t<div class=\"col-calendar\" name=\"{WEEK}-2\" data-date=\"{FULLDATE}\">\n                <button class=\"btn btn-primary btn-sm float-end mt-1 me-1\" name=\"add-event\" data-date=\"{FULLDATE}\"><span class=\"k-icon k-i-plus\"></span>[resource_displayname[newevent]]</button>\n\t\t\t\t<div class=\"calendar-bar w-100\"><p name=\"day\">{DATE} [resource_displayname[tuesday]]</p></div>\n\t\t\t\t<div class=\"events w-100\"></div>\n\t\t\t</div>\n\t\t\t<div class=\"col-calendar\" name=\"{WEEK}-3\" data-date=\"{FULLDATE}\">\n                <button class=\"btn btn-primary btn-sm float-end mt-1 me-1\" name=\"add-event\" data-date=\"{FULLDATE}\"><span class=\"k-icon k-i-plus\"></span>[resource_displayname[newevent]]</button>\n\t\t\t\t<div class=\"calendar-bar w-100\"><p name=\"day\">{DATE} [resource_displayname[wednesday]]</p></div>\n\t\t\t\t<div class=\"events w-100\"></div>\n\t\t\t</div>\n\t\t\t<div class=\"col-calendar\" name=\"{WEEK}-4\" data-date=\"{FULLDATE}\">\n                <button class=\"btn btn-primary btn-sm float-end mt-1 me-1\" name=\"add-event\" data-date=\"{FULLDATE}\"><span class=\"k-icon k-i-plus\"></span>[resource_displayname[newevent]]</button>\n\t\t\t\t<div class=\"calendar-bar w-100\"><p name=\"day\">{DATE} [resource_displayname[thursday]]</p></div>\n\t\t\t\t<div class=\"events w-100\"></div>\n\t\t\t</div>\n\t\t\t<div class=\"col-calendar\" name=\"{WEEK}-5\" data-date=\"{FULLDATE}\">\n                <button class=\"btn btn-primary btn-sm float-end mt-1 me-1\" name=\"add-event\" data-date=\"{FULLDATE}\"><span class=\"k-icon k-i-plus\"></span>[resource_displayname[newevent]]</button>\n\t\t\t\t<div class=\"calendar-bar w-100\"><p name=\"day\">{DATE} [resource_displayname[friday]]</p></div>\n\t\t\t\t<div class=\"events w-100\"></div>\n\t\t\t</div>\n\t\t\t<div class=\"col-calendar\" name=\"{WEEK}-6\" data-date=\"{FULLDATE}\">\n                <button class=\"btn btn-primary btn-sm float-end mt-1 me-1\" name=\"add-event\" data-date=\"{FULLDATE}\"><span class=\"k-icon k-i-plus\"></span>[resource_displayname[newevent]]</button>\n\t\t\t\t<div class=\"calendar-bar w-100\"><p name=\"day\">{DATE} [resource_displayname[saturday]]</p></div>\n\t\t\t\t<div class=\"events w-100\"></div>\n\t\t\t</div>\n\t\t</div>`;\n\t\t\n\t\tfor(var w = 0; w < 6; w++) {\n\t\t\tvar temp = week;\n\t\t\tcalendarContent += temp.replaceAll('{WEEK}', w);\n\t\t}\n\t\t\n\t\tcalendar.html(calendarContent);\n\t},\n\n\tsunday: 0,\n\n\tpopulateCalendarDates: function(container, startDate, endDate) {\n\t\tvar current = new Date(startDate.toISOString()); // Create a copy of the start date\n\t\tvar end = endDate\n\t\t\t? new Date(endDate)\n\t\t\t: null;\n\n\t\tif(end == null) {\n\t\t\tend = new Date(CalendarManagement.formatDateToYMD(startDate));\n\t\t\tend.setMonth(end.getMonth() + 1);\n\t\t\tend.setDate(0);\n\t\t}\n\n\t\tvar week = 0;\n\n\t\t// Step 1: Fill week 0 correctly (even if the month doesn't start on Sunday)\n\t\twhile (current <= end) {\n\t\t\tvar dayOfWeek = current.getDay();\n\t\t\tif (current.getDate() === 1 && dayOfWeek !== CalendarManagement.sunday) {\n\t\t\t\t// Ensure the first week is filled even if the month doesn't start on Sunday\n\t\t\t\tfor (var emptyDay = 0; emptyDay < dayOfWeek; emptyDay++) {\n\t\t\t\t\tvar placeholderDate = $(`[name=${week}-${emptyDay}]`, container);\n\t\t\t\t\tplaceholderDate.html('{DATE}');\n\t\t\t\t}\n\t\t\t}\n\n\t\t\t// Step 2: Fill the current date in the grid\n\t\t\tvar calendarDate = $(`[name=${week}-${dayOfWeek}]`, container);\n\t\t\tcalendarDate.html(calendarDate.html().replaceAll('{DATE}', current.getDate()));\n\n\t\t\t// Set the correct data-date for the element\n\t\t\tvar fullDate = CalendarManagement.formatDateToYMD(current);\n\t\t\tcalendarDate.attr('data-date', fullDate);\n\t\t\t$('button[name=add-event]', calendarDate).attr('data-date', fullDate);\n\n\t\t\t// Step 3: Move to the next day\n\t\t\tcurrent.setDate(current.getDate() + 1);\n\n\t\t\t// Step 4: Increment the week counter if the day is Sunday (end of week)\n\t\t\tif (dayOfWeek === 6 && current <= end) {\n\t\t\t\tweek++;\n\t\t\t}\n\t\t}\n\n\t\t// Step 5: Clear out any remaining placeholders (those that still contain '{DATE}')\n\t\tvar invalid = $(container).find('.col-calendar:contains({DATE})');\n\t\tinvalid.each(function() {\n\t\t\t$(this).html(''); // Clear the placeholders\n\t\t});\n\n\t\t// Step 6: Clear out any fully invalid rows\n\t\tvar rows = $(container).find('.row');\n\t\trows.each(function() {\n\t\t\tvar dates = $(this).find('.col-calendar');\n\n\t\t\tvar rowIsEmpty = true;\n\t\t\tfor(var date of dates) {\n\t\t\t\tif($(date).html() != '')\n\t\t\t\t\trowIsEmpty = false;\n\t\t\t}\n\n\t\t\tif(rowIsEmpty)\n\t\t\t\t$(this).remove();\n\t\t});\n\t},\n\n\tpopulateCalendarEvents: function(container, events) {\n\t\tfor(var event of events) {\n\t\t\tvar eventDate = new Date(event.Start.split('T')[0]).toISOString().split('T');\n\n\t\t\tvar eventCode = `<div class=\"calendarEvent\">\n\t\t\t\t<button class=\"btn btn-sm btn-primary float-end\" name=\"delete-event\" data-eventid=\"${event.Id}\">\n\t\t\t\t\t<span class=\"k-icon k-i-trash\"></span>\n\t\t\t\t</button>\n\t\t\t\t<span class=\"k-icon k-i-calendar\"></span>\n\t\t\t\t<label>${event.Name}</label>\n\t\t\t</div>`;\n\n\t\t\tvar dateBlock = $(`.col-calendar[data-date=${eventDate[0]}] > .events`, container);\n\t\t\tdateBlock.append(eventCode);\n\t\t}\n\t},\n\n\tbuildCalendar: async function(container, args) {\n\t\t// Reset the calendar state.\n\t\tCalendarManagement.initialiseCalendarGrid(container);\n\n\t\tvar selectedYear = $('[name=year]', container).val();\n\t\tvar selectedMonth = $('[name=month]', container).val().padStart(2, '0');\n\t\tvar selectedCalendar = args.calendarId;\n\n\t\tvar start = (args && args.fromDate)\n\t\t\t? new Date(args.fromDate)\n\t\t\t: new Date(selectedYear, selectedMonth - 1, 1);\n\n\t\tvar end = (args && args.toDate) \n\t\t\t? new Date(args.toDate) \n\t\t\t: new Date(start.getFullYear(), start.getMonth() + 1, 0);\n\n\t\tCalendarManagement.populateCalendarDates($('[name=calendar]', container), start, end);\n\n\t\tvar events;\n\t\tif (args && args.eventSource) {\n\t\t\tevents = args.eventSource;\n\t\t} else {\n\t\t\tevents = (await api.get(`Workflow/CalendarEvent?$filter=CalendarId eq ${selectedCalendar} and Start ge ${CalendarManagement.formatDateToYMD(start)} and Start le ${CalendarManagement.formatDateToYMD(end)}`)).value;\n\t\t}\n\n\t\tawait CalendarManagement.populateCalendarEvents(container, events);\n\t\tCalendarManagement.wireUpClickEvents(container, args);\n\t},\n\n\twireUpClickEvents: async function(container, args) {\n\t\t$('[name=add-event]', container).on('click', async (e) => {\n\t\t\tvar selectedCalendar = $('[name=calendars]', container).val();\n\t\t\tvar eventDate = $(e.target).attr('data-date');\n\t\t\tawait CalendarManagement.newCalendarEvent(e, selectedCalendar, eventDate, container, args);\n\t\t});\n\n\t\t$('[name=delete-event]', container).on('click', async (e) => {\n\t\t\tvar eventId = $(e.target).attr('data-eventid');\n\t\t\tawait CalendarManagement.deleteCalendarEvent(e, eventId, container, args);\n\t\t});\n\n\t\tif(args && args.readOnly) {\n\t\t\t$('[name=add-event]', container).remove();\n\t\t\t$('[name=delete-event]', container).remove();\n\t\t}\n\t},\n\n\tsetPrimary: async function (e, app, container) {\n\t\te.preventDefault();\n\t\tvar serverApp = await api.get(\"ContentManagement/App(\" + app.Id + \")\");\n\t\tvar config = JSON.parse(serverApp.ConfigJson);\n\t\tconfig.Calendars.primary = $(\"[name=calendars]\", container).val();\n\t\tserverApp.ConfigJson = JSON.stringify(config, null, 4);\n\t\tawait api.update(\"ContentManagement/App(\" + serverApp.Id + \")\", serverApp)\n\t\t\t.then(() => notification.success(\"[resource_displayname[saved]]\"))\n\t\t\t.catch((err) => error(err));\n\t},\n\n\tdeleteCalendar: async function (e, container) {\n\t\te.preventDefault();\n\t\tvar calendar = $(\"[name=calendars]\", container).val();\n\t\tawait api.destroy(\"Workflow/Calendar(\" + calendar + \")\")\n\t\t\t.catch((err) => error(err));\n\n\t\twindow.location.reload();\n\t},\n\n\tnewCalendar: async function (e, app) {\n\t\te.preventDefault();\n\t\tvar calendar = {\n\t\t\tId: 0,\n\t\t\tAppId: app.Id,\n\t\t\tName: \"\",\n\t\t};\n\t\tvar args = {\n\t\t\tfields: [\n\t\t\t\t{ field: \"Name\", title: \"[resource_displayname[name]]\", description: \"[resource_description[name]]\" },\n\t\t\t],\n\t\t\ttitle: \"[resource_displayname[newcalendar]]\",\n\t\t\tdata: calendar,\n\t\t\tconfirm: \"[resource_displayname[confirm]]\",\n\t\t\tclose: \"[resource_displayname[close]]\"\n\t\t};\n\t\tvar addCalendarDialog = new EditorDialog(args);\n\t\taddCalendarDialog.events.confirm = async function () {\n\t\t\tcalendar = addCalendarDialog.data.toJSON();//Remove all the kendo observable properties.\n\t\t\tawait api.add(\"Workflow/Calendar\", calendar).then(async (calendarCreated) => {\n\t\t\t\taddCalendarDialog.events.close();\n\t\t\t\tvar currentApp = await api.get(\"ContentManagement/App(\" + app.Id + \")\");\n\t\t\t\tvar config = JSON.parse(currentApp.ConfigJson);\n\t\t\t\tconfig.Calendars[calendarCreated.Name] = calendarCreated.Id;\n\t\t\t\tcurrentApp.ConfigJson = JSON.stringify(config, null, 4);\n\t\t\t\tawait api.put(\"ContentManagement/App(\" + currentApp.Id + \")\", currentApp).then(() => {\n\t\t\t\t\tnotification.success(\"[resource_displayname[created]]\");\n\t\t\t\t\twindow.location.reload();\n\t\t\t\t}).catch((err) => error(err));\n\t\t\t}).catch((err) => error(err));\n\t\t};\n\t\taddCalendarDialog.init();\n\t},\n\n\tnewCalendarEvent: async function (e, calendarId, start, container, args) {\n\t\te.preventDefault();\n\t\tvar startInformation = new Date(start);\n\t\tvar trueStart = new Date(Date.UTC(startInformation.getUTCFullYear(), startInformation.getUTCMonth(), startInformation.getUTCDate()));\n\t\tvar calendarEvent = kendo.observable({\n\t\t\tId: 0,\n\t\t\tCalendarId: args.calendarId,\n\t\t\tName: \"\",\n\t\t\tStart: trueStart.toISOString(),\n\t\t\tDurationInTicks: 1000000 * 24 * 60 * 60\n\t\t});\n\n\t\tvar addCalendarEventDialog = new Dialog({ title: \"[resource_displayname[neweventfor]] \" + CalendarManagement.days[new Date(start).getDay()] });\n\t\taddCalendarEventDialog.template = $(\"[name=newCalendarEventDialog]\", container).first().html();\n\t\taddCalendarEventDialog.events.confirm = async function () {\n\t\t\tcalendarEvent = calendarEvent.toJSON();\n\t\t\tvar repeatsEvery = $(\"[name=repeatsEvery]\", addCalendarEventDialog.element).val();\n\n\t\t\tif (repeatsEvery == \"never\") {\n\t\t\t\tawait api.add(\"Workflow/CalendarEvent\", calendarEvent)\n\t\t\t\t\t.then(async () => {\n\t\t\t\t\t\taddCalendarEventDialog.events.close();\n\t\t\t\t\t\tnotification.success(\"[resource_displayname[created]]\");\n\t\t\t\t\t\tawait CalendarManagement.buildCalendar(container, args);\n\t\t\t\t\t})\n\t\t\t\t\t.catch((err) => error(err));\n\t\t\t} else if (repeatsEvery == \"weekly\") {\n\t\t\t\tvar start = new Date(trueStart);\n\t\t\t\tvar untilDate = $(\"[name=until]\", addCalendarEventDialog.element).data(\"kendoDateTimePicker\").value();\n\t\t\t\tvar events = [];\n\t\t\t\twhile (start < untilDate) {\n\t\t\t\t\tevents.push({\n\t\t\t\t\t\tId: calendarEvent.Id,\n\t\t\t\t\t\tCalendarId: calendarEvent.CalendarId,\n\t\t\t\t\t\tName: calendarEvent.Name,\n\t\t\t\t\t\tStart: start.toISOString(),\n\t\t\t\t\t\tDurationInTicks: calendarEvent.DurationInTicks\n\t\t\t\t\t});\n\t\t\t\t\tstart.setDate(start.getDate() + 7);\n\t\t\t\t}\n\t\t\t\tnotification.info(\"[resource_displayname[adding]]\");\n\n\t\t\t\t// Post all events to \"Workflow/CalendarEvent\" individually\n\t\t\t\tfor (let i = 0; i < events.length; i++) {\n\t\t\t\t\tawait api.add(\"Workflow/CalendarEvent\", events[i])\n\t\t\t\t\t\t.catch((err) => error(err));\n\t\t\t\t}\n\n\t\t\t\taddCalendarEventDialog.events.close();\n\t\t\t\tnotification.success(\"[resource_displayname[created]]\");\n\t\t\t\tawait CalendarManagement.buildCalendar(container, args);\n\t\t\t}\n\t\t};\n\n\t\taddCalendarEventDialog.init(() => {\n\t\t\tkendo.bind(addCalendarEventDialog.element, calendarEvent);\n\t\t\t$(\"[name=repeatsEvery]\", addCalendarEventDialog.element).kendoDropDownList({\n\t\t\t\tdataSource: Object.keys(CalendarManagement.repeats).map(r => ({\n\t\t\t\t\tText: CalendarManagement.repeats[r],\n\t\t\t\t\tValue: r\n\t\t\t\t})),\n\t\t\t\tdataTextField: \"Text\",\n\t\t\t\tdataValueField: \"Value\"\n\t\t\t});\n\t\t\t$(\"[name=until]\", addCalendarEventDialog.element).kendoDatePicker({\n\t\t\t\tautoBind: false,\n\t\t\t\tvalue: new Date(),\n\t\t\t\tdateInput: true,\n\t\t\t\tformat: type.dateFormat\n\t\t\t});\n\t\t});\n\t},\n\n\n\tdeleteCalendarEvent: async function (e, eventId, container, args) {\n\t\te.preventDefault();\n\t\tawait api.destroy(\"Workflow/CalendarEvent(\" + eventId + \")\").then(async () => {\n\t\t\tnotification.success(\"[resource_displayname[deleted]]\");\n\t\t\tawait CalendarManagement.buildCalendar(container, args);\n\t\t}).catch((err) => error(err));\n\t},\n\n\tformatDateToYMD: function(date) {\n\t\treturn `${date.getFullYear()}-${(date.getMonth() + 1).toString().padStart(2, '0')}-${date.getDate().toString().padStart(2, '0')}`;\n\t}\n}",
  "Content": "\n<div class=\"row k-toolbar\" name=\"toolbar\">\n    <div class=\"col-md-2\">\n        <label class=\"me-3\">Month</label>\n        <select class=\"form-control\" name=\"month\"></select>\n    </div>\n    <div class=\"col-md-2\">\n        <label class=\"me-3\">Year</label>\n        <select class=\"form-control\" name=\"year\"></select>\n    </div>\n</div>\n\n<div class=\"calendar\" name=\"calendar\"></div>\n\n<script type=\"text/template\" name=\"newCalendarEventDialog\">\n\t<div class=\"input-group input-group-sm mb-1\">\n\t\t<span class=\"input-group-text\">[resource_displayname[name]]</span>\n\t\t<input type=\"text\" class=\"form-control\" data-bind=\"value: Name\" />\n\t</div>\n\n\t<div class=\"input-group input-group-sm mb-1\">\n\t\t<span class=\"input-group-text\">[resource_displayname[repeats]]</span>\n\t\t<input type=\"text\" class=\"form-control\" name=\"repeatsEvery\" />\n\t</div>\n\n\t<div class=\"input-group input-group-sm mb-1\">\n\t\t<span class=\"input-group-text\">[resource_displayname[until]]</span>\n\t\t<input type=\"text\" class=\"form-control\" name=\"until\" />\n\t</div>\n\n    <hr />\n\n    <button class=\"btn btn-sm btn-primary float-end\" name=\"confirm\">\n        <span class=\"k-icon k-i-plus\"></span> [resource_displayname[add]]\n    </button>\n</script>",
  "LastUpdated": "2024-11-19T18:18:31.2867299+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "CMS",
  "Key": "Content Management",
  "ResourceKey": "CMS",
  "Script": "var CMS = {\r\n\tinit: async function (app, container) {\r\n\t\tapp = app || session.app;\r\n\t\tcontainer = container || $(\".component[name=CMS]\");\r\n\r\n\t\tapp = await api.get(\"ContentManagement/App(\" + app.Id + \")\");\r\n\t\tapi.addToMetaCache([\r\n\t\t\t{\r\n\t\t\t\t\"Name\": \"Core\",\r\n\t\t\t\t\"Types\": [\r\n\t\t\t\t\t[meta[ContentManagement/Page]],\r\n\t\t\t\t\t[meta[ContentManagement/Layout]]\r\n\t\t\t\t]\r\n\t\t\t}]);\r\n\t\t\r\n\t\t// $(window).on(\"resize\", () => CMS.resize(container));\r\n\t\t// CMS.resize(container);\r\n\r\n\t\tvar treeRoot = $(\".pageTree\", container);\r\n\r\n\t\t$(\"[name=splitter]\", container).kendoSplitter({\r\n\t\t\tscrollable: false,\r\n\t\t\tpanes: [\r\n\t\t\t\t{ collapsible: false },\r\n\t\t\t\t{ collapsible: false, scrollable: false }\r\n\t\t\t]\r\n\t\t});\r\n\r\n\t\tvar tree = new ODataTree(\r\n\t\t\tnew ODataTreeOptions()\r\n\t\t\t\t.setElement(treeRoot)\r\n\t\t\t\t.setEndpoint(\"ContentManagement/Page\")\r\n\t\t\t\t.setODataAppend(\"?$filter=AppId eq \" + app.Id + \"&$expand=Pages($orderBy=Order asc),PageInfo,Contents&$orderBy=Order asc\")\r\n\t\t);\r\n\r\n\t\tlet pageTypeData = await api.getType(\"ContentManagement/Page\");\r\n\r\n\t\ttree.prepareData = (p) => {\r\n\t\t\tvar hasChildren = p.Pages.length > 0;\r\n\t\t\tvar pageName = p.PageInfo.filter(r => r.CultureId === \"\")[0].Title;\r\n\t\t\tif (p.PageInfo && p.PageInfo.length > 0) {\r\n\t\t\t\tvar matchedPageInfo = p.PageInfo.filter(r => r.CultureId === session.culture);\r\n\t\t\t\tpageName = matchedPageInfo.length > 0 ? matchedPageInfo[0].Title : pageName;\r\n\t\t\t}\r\n\t\t\tmodel.prepareItem(p, pageTypeData);\r\n\t\t\treturn { text: pageName, spriteCssClass: \"page\", type: \"Page\", data: p, expanded: false, hasChildren: hasChildren };\r\n\t\t};\r\n\r\n\t\ttree.select = (e) => CMS.tree.selectNode(e, tree);\r\n\t\ttree.drop = (e) => CMS.tree.drop(e, tree);\r\n\t\ttreeRoot.on(\"contextmenu\", \".k-treeview-leaf\", (e) => CMS.tree.rightClick(e, app, tree, container));\n\n\t\tawait tree.init();\n\t\tCMS.wrapWithRootNode(app, tree);\n\t},\n\n\twrapWithRootNode: function(app, tree) {\n\t\tlet rootItems = tree.kendoObject.dataSource.data().toJSON();\n\t\tlet rootNode = {\n\t\t\ttext: app.Domain,\n\t\t\tspriteCssClass: \"page\",\n\t\t\ttype: \"Root\",\n\t\t\tdata: null,\n\t\t\texpanded: true,\n\t\t\thasChildren: true,\n\t\t\titems: rootItems\n\t\t};\n\n\t\ttree.kendoObject.setDataSource(new kendo.data.HierarchicalDataSource({\n\t\t\tdata: [rootNode]\n\t\t}));\n\t\ttree.kendoObject.expand(\".k-treeview-item:first\");\n\t},\n\r\n\tresize: function (container) {\r\n\t\tvar headerHeight = $(\"body > header\").height();\r\n\t\tvar footerHeight = $(\"body > footer\").height();\r\n\t\tvar bodyHeight = $(\"body\").height();\r\n\t\tcontainer.height(bodyHeight - (headerHeight + footerHeight) - 3);\r\n\t\tcontainer.width(\"100%\");\r\n\t},\r\n\r\n\ttree: {\r\n\t\tupdatePages: async function (treePages) {\r\n\t\t\tfor (let i = 0; i < treePages.length; i++) {\r\n\t\t\t\ttreePages[i].data.Order = i;\r\n\t\t\t}\r\n\r\n\t\t\tfor (let i = 0; i < updateData.length; i++) {\r\n\t\t\t\tawait updateData[i].save();\r\n\t\t\t}\r\n\t\t},\r\n\r\n\t\tdrop: async function (e, tree) {\r\n\t\t\te.preventDefault();\r\n\t\t\tvar dropNode = tree.dataItem($(e.dropTarget).closest(\".k-item\"));\r\n\t\t\tvar dragNode = tree.dataItem($(e.sourceNode));\r\n\t\t\tvar parent = $(e.sourceNode).parent();\r\n\t\t\tvar targetParent = $(e.destinationNode).parent();\r\n\t\t\tvar index = tree.dataItem(parent).items.indexOf(dragNode);\r\n\t\t\tif (e.dropPosition === \"over\") {\r\n\t\t\t\t// apply move\r\n\t\t\t\ttree.dataItem(parent).items.splice(index, 1);\r\n\t\t\t\tif (dropNode.type != \"Root\") {\r\n\t\t\t\t\tdragNode.data.ParentId = dropNode.data.Id;\r\n\t\t\t\t}\r\n\t\t\t\telse {\r\n\t\t\t\t\tdragNode.data.ParentId = null;\r\n\t\t\t\t}\r\n\t\t\t\tdragNode.data.save();\r\n\r\n\t\t\t} else if (e.dropPosition === \"after\") {\r\n\t\t\t\tvar newIndex = tree.dataItem(targetParent).items.indexOf(tree.dataItem(e.destinationNode));\r\n\t\t\t\tif (newIndex < tree.dataItem(targetParent).items.length) {\r\n\t\t\t\t\ttree.dataItem(parent).items.splice(index, 1);\r\n\t\t\t\t\ttree.dataItem(targetParent).items.splice(newIndex, 0, dragNode);\r\n\t\t\t\t\tdragNode.data.Order = newIndex;\r\n\t\t\t\t\tdragNode.data.ParentId = tree.dataItem(targetParent).data.Id;\r\n\t\t\t\t\tawait CMS.tree.updatePages(tree.dataItem(targetParent).items);\r\n\t\t\t\t}\r\n\t\t\t} else if (e.dropPosition === \"before\") {\r\n\t\t\t\tvar newIndex = tree.dataItem(targetParent).items.indexOf(tree.dataItem(e.destinationNode)) - 1;\r\n\t\t\t\tif (newIndex >= 0) {\r\n\t\t\t\t\ttree.dataItem(parent).items.splice(index, 1);\r\n\t\t\t\t\ttree.dataItem(targetParent).items.splice(newIndex + 1, 0, dragNode);\r\n\t\t\t\t\tdragNode.data.Order = newIndex;\r\n\t\t\t\t} else {\r\n\t\t\t\t\ttree.dataItem(parent).items.splice(index, 1);\r\n\t\t\t\t\ttree.dataItem(targetParent).items.splice(0, 0, dragNode);\r\n\t\t\t\t\tdragNode.data.Order = 0;\r\n\t\t\t\t}\r\n\t\t\t\tdragNode.data.ParentId = tree.dataItem(targetParent).data.Id;\r\n\t\t\t\tawait CMS.tree.updatePages(tree.dataItem(targetParent).items);\r\n\t\t\t}\r\n\t\t},\r\n\r\n\t\trightClick: async function (e, app, tree, container) {\r\n\t\t\te.preventDefault();\r\n\t\t\tvar page = tree.dataItem(e.target).data;\r\n\r\n\t\t\tvar contextMenu = new ContextMenuWidget(container);\r\n\t\t\t\t\t\tcontextMenu.commands.push({ name: \"newChild\", icon: \"k-i-plusIcon\", text: \"[resource_displayname[newchildpage]]\" });\r\n\t\t\tcontextMenu.commands.push({ name: \"delete\", icon: \"k-i-trashIcon\", text: \"[resource_displayname[delete]]\" });\r\n\t\t\tcontextMenu.commands.push({ name: \"properties\", icon: \"k-i-fileConfigIcon\", text: \"[resource_displayname[properties]]\" });\r\n\t\t\tcontextMenu.init(e.pageX, e.pageY);\r\n\r\n\t\t\t$(\"[name=newChild]\", contextMenu.contextMenuElement).on(\"click\", (e) => CMS.tree.newChild(e, app, page, tree));\r\n\t\t\t$(\"[name=delete]\", contextMenu.contextMenuElement).on(\"click\", (e) => CMS.tree.destroy(e, page, tree));\r\n\t\t\t$(\"[name=properties]\", contextMenu.contextMenuElement).on(\"click\", (e) => CMS.tree.properties(e, page, app));\r\n\t\t},\r\n\r\n\t\tnewChild: async function (e, app, page, tree) {\r\n\t\t\te.preventDefault();\r\n\r\n\t\t\tvar dataItem = tree.dataItem(e.target);\r\n\t\t\tvar meta = await api.getType(\"ContentManagement/Page\");\r\n\t\t\tvar layoutDataSource = await model.getDatasource({ endpoint: \"ContentManagement/Layout\", odataAppend: \"?$filter=AppId eq \" + app.Id });\r\n\r\n\t\t\tvar newPage = new kendo.data.ObservableObject({\r\n\t\t\t\tId: \"new\",\r\n\t\t\t\tName: \"\",\r\n\t\t\t\tParentId: (page != null) ? page.Id : null,\r\n\t\t\t\tAppId: app.Id,\r\n\t\t\t\tShowOnMenus: true,\r\n\t\t\t\tLayout: \"Default\",\r\n\t\t\t\tOrder: 0,\r\n\t\t\t\tPageInfo: [\r\n\t\t\t\t\t{ CultureId: \"\", Title: \"\", Description: \"\", Keywords: \"\" }\r\n\t\t\t\t],\r\n\t\t\t\tContents: [\r\n\t\t\t\t\t{ CultureId: \"\", Html: \"\", Name: \"body\" }\r\n\t\t\t\t]\r\n\t\t\t});\r\n\r\n\t\t\tvar newChildDialog = new Dialog({ width: 510, height: 300, title: \"[resource_displayname[newpage]]\" });\r\n\t\t\tnewChildDialog.template = $(\"[name=newChildPageTemplate]\").first().html();\r\n\r\n\t\t\tnewChildDialog.events.submit = async () => {\r\n\t\t\t\tmodel.prepareItem(newPage, meta);\r\n\t\t\t\tawait newPage.save();\r\n\t\t\t\tdataItem.append({ text: newPage.PageInfo[0].Title, type: \"Page\", spriteCssClass: \"page\", expanded: false, hasChildren: true, data: newPage, draggable: true, droppable: [\"Page\"] });\r\n\t\t\t\tnotification.success(\"[resource_displayname[created]]\");\r\n\t\t\t};\r\n\r\n\t\t\tnewChildDialog.init(() => {\r\n\t\t\t\tkendo.bind(newChildDialog.element, newPage);\r\n\t\t\t\t$(\"[name=layout]\", newChildDialog.element).kendoDropDownList({ dataTextField: \"Name\", dataValueField: \"Name\", dataSource: layoutDataSource });\r\n\t\t\t});\r\n\t\t},\r\n\r\n\t\tdestroy: function (e, page, tree) {\r\n\t\t\te.preventDefault();\r\n\t\t\tvar d = new ConfirmDialog({ question: \"[resource_description[areyousure]]\", title: \"[resource_description[areyousure]]: \" + page.Path, confirm: \"[resource_displayname[confirm]]\", close: \"[resource_displayname[close]]\" });\r\n\r\n\t\t\td.events.confirm = async function (e) {\r\n\t\t\t\tawait page.destroy(e);\r\n\t\t\t\tnotification.success(\"[resource_description[PageDeleted]]\");\r\n\t\t\t\td.events.close();\r\n\t\t\t\ttree.remove(e.target);\r\n\t\t\t};\r\n\t\t\td.init();\r\n\t\t},\r\n\r\n\t\tproperties: function (e, page, app) {\n\t\t\te.preventDefault();\n\t\t\tvar d = new Dialog();\n\t\t\td.width = 900;\r\n\t\t\td.height = 'auto';\r\n\t\t\td.title = \"[resource_displayname[pageproperties]]: \" + page.Path;\r\n\t\t\td.component = \"PageProperties\";\r\n\t\t\td.init(() => PageProperties.init(app, $(\".component[name=PageProperties]\", $(d.element)), page));\r\n\t\t},\r\n\r\n\t\tselectNode: function (e, tree) {\n\t\t\tvar item = tree.dataItem(e.node);\n\t\t\tif (!item || !item.data) {\n\t\t\t\treturn;\n\t\t\t}\n\n\t\t\tvar page = item.data;\n\t\t\t$(\".panel[name=workspace]\").html(\"<iframe src='[app[root]]\" + page.Path + \"?edit=true'></iframe>\");\n\t\t}\n\t}\n};\n",
  "Content": "<style>\n    .component[name=CMS] [name=splitter] { margin: 0; border: none; box-shadow: none; width: 100%; min-height: 70vh; }\n    .component[name=CMS] [name=splitter] > .panel { height: 100%; padding: 10px; }\n    .component[name=CMS] [name=workspace] { overflow: visible; right: 0; }\n    .component[name=CMS] [name=workspace] > .component { margin: 0; width: 100%; height: 100%; overflow: visible; }\n    .component[name=CMS] .pageTree { min-height: 70vh; }\n</style>\n\n<script type=\"text/template\" name=\"deleteFolderCheck\">\n    <label>[resource_displayname[deleteconfirmation]]</label>\n    <div class=\"value\">\n        <button name=\"confirm\">[resource_displayname[confirm]]</input>\r\n        <button name=\"close\">[resource_displayname[cancel]]</input>\r\n    </div>\r\n</script>\r\n\r\n<script type=\"text/template\" name=\"newChildPageTemplate\">\r\n\t<ul class=\"fieldList\" name=\"page\">\r\n\t\t<li>\r\n\t\t\t<label data-resource-key=\"ContentManagement/Page\" data-resource-name=\"ShowOnMenus\" >[resource_displayname[isshownonmenu]]</label>\r\n\t\t\t<div class=\"value\">\r\n\t\t\t\t<input name=\"ShowOnMenus\" type=\"checkbox\" data-bind=\"checked: ShowOnMenus\" required/>\r\n\t\t\t</div>\r\n\t\t</li>\r\n\t\t<li>\r\n\t\t\t<label data-resource-key=\"ContentManagement/Page\" data-resource-name=\"Layout\">[resource_displayname[layout]]</label>\r\n\t\t\t<div class=\"value\">\r\n\t\t\t\t<input type=\"custom\" name=\"layout\">\r\n\t\t\t</div>\r\n\t\t</li>\r\n\t\t<li>\r\n\t\t\t<label data-resource-key=\"ContentManagement/Page\" data-resource-name=\"Name\">[resource_displayname[name]]</label>\r\n\t\t\t<div class=\"value\">\r\n\t\t\t\t<input name=\"Name\" type=\"text\" data-bind=\"value: Name\" required />\r\n\t\t\t</div>\r\n\t\t</li>\r\n\t\t<li>\r\n\t\t\t<label data-resource-key=\"ContentManagement/Page\" data-resource-name=\"Title\">[resource_displayname[title]]</label>\r\n\t\t\t<div class=\"value\">\r\n\t\t\t\t<input name=\"Title\" type=\"text\" data-bind=\"value: PageInfo[0].Title\" required />\r\n\t\t\t</div>\r\n\t\t</li>\r\n\t\t<li>\r\n\t\t\t<label data-resource-key=\"ContentManagement/Page\" data-resource-name=\"Description\">[resource_displayname[description]]</label>\r\n\t\t\t<div class=\"value\">\r\n\t\t\t\t<input name=\"Description\" type=\"text\" data-bind=\"value: PageInfo[0].Description\" required />\r\n\t\t\t</div>\r\n\t\t</li>\r\n\t\t<li>\r\n\t\t\t<label data-resource-key=\"ContentManagement/Page\" data-resource-name=\"Keywords\">[resource_displayname[keywords]]</label>\r\n\t\t\t<div class=\"value\">\r\n\t\t\t\t<input name=\"Keywords\" type=\"text\" data-bind=\"value: PageInfo[0].Keywords\" required />\r\n\t\t\t</div>\r\n\t\t</li>\r\n\t</ul>\r\n\t<hr>\r\n\t<div class=\"value\" style=\"float: right;\">\r\n\t\t<button name=\"submit\">[resource_displayname[submit]]</button>\r\n\t</div>\r\n</script>\r\n\r\n<div name=\"splitter\">\r\n\t<div class=\"panel left\">\r\n\t\t<div class=\"pageTree\"></div>\r\n\t</div>\r\n\t<div class=\"panel right\" name=\"workspace\"></div>\r\n</div>\r\n",
  "LastUpdated": "2025-05-30T18:44:57.6789397Z"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "Colours",
  "Key": "Theming",
  "ResourceKey": "CMS",
  "Script": "Colours = {\r\n    init: async function(app, container, observable) {\r\n        app = app || session.app;\r\n        container = container || $(\".component[name=Colours]\");\r\n        if(!observable)\r\n            return;\r\n        \r\n        $(\"[name=picker]\", container).kendoColorPicker({\r\n            buttons: false,\r\n            views: [\"gradient\"],\r\n            change: (e) => e.preventDefault()\r\n        });\r\n\r\n        $(\"[name=marginSlider]\", container).kendoSlider({ \r\n            min: 0, \r\n            max: 20, \r\n            smallStep: 1, \r\n            largeStep: 2, \r\n            value: parseFloat((observable.get(\"colours.margins\") || \"4px\").replaceAll(\"px\", \"\")),\r\n            change: function(e) {\r\n                e.preventDefault();\r\n                observable.set(\"colours.margins\", this.value() + \"px\");\r\n            }\r\n        });\r\n    }\r\n}",
  "Content": "<div class=\"row\">\r\n    <div class=\"col-md-6\">\r\n        <h4>[resource_displayname[baseColours]]</h4>\r\n        \r\n        <div class=\"input-group input-group-sm mb-1\">\r\n            <span class=\"input-group-text\">[resource_displayname[primary]]</span>\r\n            <input class=\"form-control\" name=\"picker\" data-bind=\"value: colours.primary\" />\r\n        </div>\r\n\t\t\r\n        <div class=\"input-group input-group-sm mb-1\">\r\n            <span class=\"input-group-text\">[resource_displayname[secondary]]</span>\r\n            <input class=\"form-control\" name=\"picker\" data-bind=\"value: colours.secondary\" />\r\n        </div>\r\n\t\t\r\n        <div class=\"input-group input-group-sm mb-1\">\r\n            <span class=\"input-group-text\">[resource_displayname[background]]</span>\r\n            <input class=\"form-control\" name=\"picker\" data-bind=\"value: colours.background\" />\r\n        </div>\r\n\t\t\r\n        <div class=\"input-group input-group-sm mb-1\">\r\n            <span class=\"input-group-text\">[resource_displayname[text]]</span>\r\n            <input class=\"form-control\" name=\"picker\" data-bind=\"value: colours.text\" />\r\n        </div>\r\n\t\t\r\n        <div class=\"input-group input-group-sm mb-1\">\r\n            <span class=\"input-group-text\">[resource_displayname[text2]]</span>\r\n            <input class=\"form-control\" name=\"picker\" data-bind=\"value: colours.text2\" />\r\n        </div>\r\n\t\t\r\n        <div class=\"input-group input-group-sm mb-1\">\r\n            <span class=\"input-group-text\">[resource_displayname[links]]</span>\r\n            <input class=\"form-control\" name=\"picker\" data-bind=\"value: colours.links\" />\r\n        </div>\r\n\t\t\r\n        <div class=\"input-group input-group-sm mb-1\">\r\n            <span class=\"input-group-text\">[resource_displayname[margins]]</span>\r\n            <input type=\"text\" class=\"form-control\" name=\"marginSlider\" />\r\n        </div>\r\n    </div>\r\n    <div class=\"col-md-6\">\r\n        <h4>[resource_displayname[chartColours]]</h4>\r\n\t\t\r\n        <div class=\"input-group input-group-sm mb-1\">\r\n            <span class=\"input-group-text\">[resource_displayname[colourone]]</span>\r\n            <input class=\"form-control\" name=\"picker\" data-bind=\"value: colours.charts[0]\" />\r\n        </div>\r\n\r\n        <div class=\"input-group input-group-sm mb-1\">\r\n            <span class=\"input-group-text\">[resource_displayname[colourtwo]]</span>\r\n            <input class=\"form-control\" name=\"picker\" data-bind=\"value: colours.charts[1]\" />\r\n        </div>\r\n\r\n        <div class=\"input-group input-group-sm mb-1\">\r\n            <span class=\"input-group-text\">[resource_displayname[colourthree]]</span>\r\n            <input class=\"form-control\" name=\"picker\" data-bind=\"value: colours.charts[2]\" />\r\n        </div>\r\n\t\t\r\n        <div class=\"input-group input-group-sm mb-1\">\r\n            <span class=\"input-group-text\">[resource_displayname[colourfour]]</span>\r\n            <input class=\"form-control\" name=\"picker\" data-bind=\"value: colours.charts[3]\" />\r\n        </div>\r\n\t\t\r\n        <div class=\"input-group input-group-sm mb-1\">\r\n            <span class=\"input-group-text\">[resource_displayname[colourfive]]</span>\r\n            <input class=\"form-control\" name=\"picker\" data-bind=\"value: colours.charts[4]\" />\r\n        </div>\r\n\r\n        <div class=\"input-group input-group-sm mb-1\">\r\n            <span class=\"input-group-text\">[resource_displayname[coloursix]]</span>\r\n            <input class=\"form-control\" name=\"picker\" data-bind=\"value: colours.charts[5]\" />\r\n        </div>\r\n    </div>\r\n</div>",
  "LastUpdated": "2024-11-19T18:18:31.0433683+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "CommonCacheComponents",
  "Key": "CMS",
  "ResourceKey": "CMS",
  "Script": "CommonCacheComponents = {\r\n    init: async function(app, container) {\r\n        app = app || session.app;\r\n        container = container || $(\".component[name=CommonCacheComponents]\");\r\n        \r\n        var componentData = await CommonCacheComponents.getDataSource(\"ContentManagement/CommonObject/Latest()?type=ContentManagement/Component&$orderby=Name asc\");\r\n        await CommonCacheComponents.setupComponentGrid(componentData, container);\r\n    },\r\n\r\n    getDataSource(url) {\r\n        return new kendo.data.DataSource({\r\n            transport: {\r\n                read: { url: api.apiRoot + url, dataType: \"json\" }\r\n            },\r\n            pageSize: 50,\r\n            schema: {\r\n                data: function (response) {\r\n                    for(let i = 0; i < response.value.length; i = i + 1) {\r\n                        response.value[i].CreatedOn = new Date(response.value[i].CreatedOn);\r\n                        response.value[i].LastUpdated = new Date(response.value[i].LastUpdated);\r\n                        response.value[i].Entity = JSON.parse(response.value[i].Json); \r\n                    }\r\n                    return response.value;\r\n                },\r\n                total: (response) => response.value.length\r\n            }\r\n        });;\r\n    },\r\n\r\n      setupComponentGrid: async function (ds, container) {\r\n        var cacheGrid = new GridWidget( container, ds);\r\n        cacheGrid.columns = [\r\n           \r\n            { \r\n                field: \"Entity.Key\", \r\n                title: \"[resource_shortdisplayname[key]]\",\r\n                width: 200\r\n            },\r\n            { \r\n                field: \"Entity.ResourceKey\", \r\n                title: \"[resource_shortdisplayname[resourceKey]]\",\r\n                width: 200\r\n            },\r\n            { \r\n                field: \"Name\", \r\n                title: \"[resource_shortdisplayname[name]]\",\r\n                width: 480\r\n            },\r\n            { \r\n                field: \"LastUpdated\", \r\n                title: \"[resource_displayname[lastupdated]]\", \r\n                type: \"date\", \r\n                format: \"{0: \" + type.dateFormat + \" HH:mm}\", \r\n\t\t\t\twidth: \"[theme[columns.small]]\"\r\n            },\r\n            { \r\n                field: \"LastUpdatedBy\", \r\n                title: \"[resource_displayname[lastupdatedby]]\", \r\n\t\t\t\twidth: \"[theme[columns.small]]\"  \r\n            }\r\n        ];\r\n        cacheGrid.groupable = false;\r\n        cacheGrid.filterable = true;\r\n        cacheGrid.pageable = true;\r\n        cacheGrid.editable = false;\r\n        cacheGrid.detailTemplate = kendo.template($(\"[name=componentDetails]\", container).html()),\r\n            cacheGrid.detailExpand = function (e) {\r\n                $(e.detailRow).find(\".monaco-editor\").css(\"visbility\", \"visible\");\r\n                if ($(e.detailRow).find(\".monaco-editor\").length === 0) {\r\n                    var component = cacheGrid.dataItem(e.masterRow).Entity;\r\n                    var contentEditor = $(e.detailRow).find(\"div[name=content]\", container.parent());\r\n                    var scriptEditor = $(e.detailRow).find(\"div[name=script]\", container.parent());\r\n                    var html = new HTMLMonacoEditor(contentEditor[0], { code: component.Content });\r\n                    var script = new JavaScriptMonacoEditor(scriptEditor[0], { code: component.Script });\r\n                    html.onChange = (e) => {  component.Content = html.getValue();  };\r\n                    script.onChange = (e) => {  component.Script = script.getValue();  };\r\n                    html.init();\r\n                    script.init();\r\n                }\r\n            };\r\n        cacheGrid.commands.push({ name: \"viewVersions\", icon: \"k-i-clock\", text: \"[resource_displayname[viewversions]]\" });\r\n        cacheGrid.commands.push({ name: \"save\", icon: \"k-i-save\", text: \"[resource_displayname[save]]\" });\r\n        cacheGrid.commands.push({ name: \"remove\", icon: \"k-i-trash\", text: \"[resource_displayname[remove]]\" });\r\n        cacheGrid.toolbar = \"<button class='btn btn-sm btn-primary' name='add'><span class='k-icon k-i-plus'></span> [resource_displayname[add]]</button>\";\r\n        cacheGrid.dataBound = function (e) {\r\n            $(\"[name=viewVersions]\", cacheGrid.gridElement).off(\"click\").on(\"click\", (e) => CommonCacheComponents.viewVersions(e, cacheGrid));\r\n            $(\"[name=save]\", cacheGrid.gridElement).off(\"click\").on(\"click\", (e) => CommonCacheComponents.save(e, cacheGrid));\r\n            $(\"[name=remove]\", cacheGrid.gridElement).off(\"click\").on(\"click\", (e) => CommonCacheComponents.removeFromCache(e, cacheGrid));\r\n        };\r\n        await cacheGrid.init();\r\n        $(\"[name=add]\", cacheGrid.gridElement).off(\"click\").on(\"click\", (e) => CommonCacheComponents.newComponent(e, cacheGrid));\r\n        cacheGrid.kendoObject.dataSource.group({ field: \"Entity.Key\" });\r\n    },\r\n\r\n    viewVersions: async function(e, grid) {\r\n        var item = grid.dataItem($(e.currentTarget).closest(\"tr\"));\r\n        var versionDialog = new Dialog({title: \"[resource_displayname[viewhistory]]\", height: 800, width: 1800 });\r\n        versionDialog.init(() => CommonCacheComponents.versionDialogInit(versionDialog, item));\r\n    },\r\n\r\n    versionDialogInit: async function(dialog, item) {\r\n        let dataSource = (await api.get(\"ContentManagement/CommonObject?$filter=Type eq 'ContentManagement/Component' and Name eq '\" + item.Name + \"'&$orderby=Version desc\"))\r\n            .value;\r\n        var grid = new GridWidget(dialog.element, {data: dataSource, pageSize: 20 });\r\n        grid.columns = [\r\n            { field: \"Version\", title: \"[resource_displayname[version]]\"},\r\n            { field: \"LastUpdated\", title: \"[resource_displayname[lastupdated]]\", width: 120, type: \"date\", format: \"{0: \" + type.dateFormat + \" HH:mm}\" },\r\n            { field: \"LastUpdatedBy\", title: \"[resource_displayname[lastupdatedby]]\", width: 200 },\r\n        ];\r\n        grid.groupable = false;\r\n        grid.detailTemplate = `<div>\r\n            <div name='contentEditor' style='height:600px; padding-bottom: 40px;  '><h4> [resource_displayname[recentcontentchanges]]</h4></div>\r\n            <div name='scriptEditor' style='height:600px; padding-bottom: 40px; '><h4> [resource_displayname[recentscriptchanges]]</h4></div>\r\n        </div>`;\r\n        grid.detailExpand = function(e) {\r\n            var versionedComponent = JSON.parse(item.Json);\r\n            var originalComponent = JSON.parse(grid.dataItem(e.masterRow).Json);\r\n            var expandContainer = $(e.detailRow);\r\n\r\n            var originalContentModel = monaco.editor.createModel(originalComponent.Content, \"text/html\" );\r\n            var modifiedContentModel = monaco.editor.createModel(versionedComponent.Content, \"text/html\");\r\n\r\n            var diffEditorContent = monaco.editor.createDiffEditor($(\"[name=contentEditor]\", expandContainer)[0], { automaticLayout: true });\r\n            diffEditorContent.setModel({ original: originalContentModel, modified: modifiedContentModel });\r\n\r\n            monaco.editor.createDiffNavigator(diffEditorContent, {  followsCaret: true,  ignoreCharChanges: true });\r\n\r\n            var originalScriptModel = monaco.editor.createModel(originalComponent.Script, \"text/javascript\" );\r\n            var modifiedScriptModel = monaco.editor.createModel(versionedComponent.Script, \"text/javascript\");\r\n\r\n            var diffEditorScript = monaco.editor.createDiffEditor($(\"[name=scriptEditor]\", expandContainer)[0], { automaticLayout: true });\r\n            diffEditorScript.setModel({ original: originalScriptModel, modified: modifiedScriptModel });\r\n\r\n            monaco.editor.createDiffNavigator(diffEditorScript, {  followsCaret: true,  ignoreCharChanges: true });\r\n        };\r\n        await grid.init();\r\n    },\r\n\r\n    save: async function(e, grid) {\r\n        var item = grid.dataItem($(e.currentTarget).closest(\"tr\"));\r\n        var dupe = JSON.parse(JSON.stringify(item));\r\n        dupe.Entity.Key = dupe.Key;\r\n        dupe.Json = JSON.stringify(dupe.Entity);\r\n        delete dupe.Entity;\r\n        await api.update(\"ContentManagement/CommonObject(\" + item.Id + \")\", dupe).then(async () => {\r\n            notification.success(\"[resource_displayname[saved]]\");\r\n        }).catch((err) => error(err));\r\n    },\r\n\r\n      newComponent: async function (e, cacheGrid) {\r\n        e.preventDefault();\r\n        var newComponentDialog = new Dialog({ width: 520, height: \"auto\", title: \"[resource_displayname[newcomponent]]\" });\r\n        newComponentDialog.template = $(\"[name=newComponentDialog]\").html();\r\n        newComponentDialog.events.create = async function (e) {\r\n            var component = {\r\n                Key: $(\"[name=key]\", newComponentDialog.element).val(),\r\n                Name: $(\"[name=name]\", newComponentDialog.element).val(),\r\n                ResourceKey: $(\"[name=resourceKey]\", newComponentDialog.element).val(),\r\n                Content: \"\",\r\n                Script: \"\",\r\n                CreatedOn: (new Date()).toISOString(),\r\n                LastUpdated: (new Date()).toISOString()\r\n            };\r\n            var commonObject = {\r\n                Id: 0,\r\n                Key: component.Key,\r\n                Name: component.Name,\r\n                CreatedOn: component.CreatedOn,\r\n                LastUpdated: component.LastUpdated,\r\n                Json: JSON.stringify(component),\r\n                Type: \"ContentManagement/Component\",\r\n                Culture: \"\",\r\n                Version: 1\r\n            };\r\n            await api.add(\"ContentManagement/CommonObject\", commonObject).then(async () => {\r\n                notification.success(\"[resource_displayname[added]]\");\r\n                await api.get(\"RefreshCache\").then(() => notification.success(\"[resource_displayname[rebuilt]]\"));\r\n                cacheGrid.refresh();\r\n            }).catch((err) => error(err));\r\n            newComponentDialog.events.close();\r\n        };\r\n        newComponentDialog.init();\r\n    },\r\n\r\n    removeFromCache: async function (e, grid) {\r\n        var item = grid.kendoObject.dataItem($(e.currentTarget).closest(\"tr\"));\r\n        await api.destroy(\"ContentManagement/CommonObject(\" + item.Id + \")\").then(async () => {\r\n            notification.success(\"[resource_displayname[deleted]]\");\r\n            await api.get(\"RefreshCache\").then(() => notification.success(\"[resource_displayname[rebuilt]]\"));\r\n            grid.refresh();\r\n        }).catch((err) => error(err));\r\n    }\r\n}",
  "Content": "<script type=\"text/template\" name=\"newComponentDialog\">\r\n\r\n\t<div class=\"input-group input-group-sm mb-2\">\r\n\t\t<span class=\"input-group-text\">[resource_shortdisplayname[name]]</span>\r\n\t\t<input type=\"text\" class=\"form-control\" name=\"name\" />\r\n\t</div>\r\n\r\n\t<div class=\"input-group input-group-sm mb-2\">\r\n\t\t<span class=\"input-group-text\">[resource_shortdisplayname[key]]</span>\r\n\t\t<input type=\"text\" class=\"form-control\" name=\"key\" />\r\n\t</div>\r\n\r\n\t<div class=\"input-group input-group-sm mb-2\">\r\n\t\t<span class=\"input-group-text\">[resource_shortdisplayname[resourcekey]]</span>\r\n\t\t<input type=\"text\" class=\"form-control\" name=\"resourceKey\" />\r\n\t</div>\r\n\r\n\t<hr />\r\n\r\n\t<div class=\"d-flex justify-content-end\">\r\n\t\t<button type=\"button\" class=\"btn btn-sm btn-primary\" name=\"create\">\r\n\t\t\t<span class=\"k-icon k-i-plus\"></span>\r\n\t\t\t[resource_shortdisplayname[create]]\r\n\t\t</button>\r\n\t</div>\r\n\r\n</script>\r\n\r\n\r\n<script type=\"text/template\" name=\"componentDetails\">\r\n<div name=\"componentEditor\">\r\n\t <div class=\"editorContainer\" style=\"width:49%;\">\r\n\t\t<h4>\r\n\t\t\t<span class='k-icon k-i-source-code'></span>\r\n\t\t\t[resource_displayname[content]]\r\n\t\t</h4>\r\n\t\t<div name=\"content\"></div>\r\n\t </div>\r\n\t <div class=\"editorContainer\">\r\n\t\t<h4>\r\n\t\t\t<span class='k-icon k-i-js'></span>\r\n\t\t\t[resource_displayname[script]]\r\n\t\t</h4>\r\n\t\t<div name=\"script\"></div>\r\n\t </div>\r\n</div>\r\n</script>\r\n<style type=\"text/css\" scoped>\r\n .editorContainer \t\t\t\t     { display: inline-block; width: 49.5%; height: 500px; margin-right: 10px; margin-bottom: 40px; }\r\n .editorContainer > textarea\t{ width: 99%; height: 100%; }\r\n .component[name=CommonCacheComponents]  { height: 100%; }\r\n   .component [name=CommonCacheComponents] { flex: none !important; height: 100%; }\r\n   .component[name=CommonCacheComponents] > [name=CommonCacheComponentsGrid] { flex: none !important; height: 100%; }\r\n</style>",
  "LastUpdated": "2026-03-05T10:32:03.0462872+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "CommonCacheEndpoint",
  "Key": "CMS",
  "ResourceKey": "CMS",
  "Script": "CommonCacheEndpoint = {\r\n    init: async function (app, container) {\r\n        app = app || session.app;\r\n        container = container || $(\".component[name=CommonCacheEndpoint]\");\r\n        container.addClass(\"large\");\r\n\r\n    api.addToMetaCache([{\r\n        \"Name\": \"Core\",\r\n        \"Types\": [\r\n            [meta[ContentManagement/CommonObject]],\r\n            [meta[ContentManagement/Component]],\r\n            [meta[ContentManagement/Script]],\r\n            [meta[ContentManagement/Resource]]\r\n        ]\r\n    }]);\r\n\r\n        this.initNavTabIds(container);\r\n        await this.addClickListenerForComponents(container);\r\n\r\n        var firstPane = $('.tab-pane.active', container);\r\n        var component = firstPane.attr('data-component');\r\n\r\n        if (component) {\r\n            await loadComponent(firstPane, component, async (c) => {\r\n                await c.init(session.app, $(`.component[name=${component}]`, firstPane));\r\n            });\r\n        }\r\n\r\n        $(\"[name=appToolbar]\").kendoToolBar({\r\n            items: [\r\n                {\r\n                    type: \"button\",\r\n                    template: \"<button class='btn btn-sm btn-primary' name='refreshCache'><span class='k-icon k-i-arrow-rotate-cw'></span> [resource_displayname[refreshCache]]</button>\"\r\n                },\r\n                {\r\n                    type: \"button\",\r\n                    template: \"<button class='btn btn-sm btn-primary' name='migrate' data-bs-toggle='modal' data-bs-target='#common-cache-migrate-modal'><span class='k-icon k-i-arrow-up'></span> [resource_displayname[migrate]]</button>\"\r\n                },\r\n                {\r\n                    type: \"button\",\r\n                    template: \"<button class='btn btn-sm btn-primary' name='recentChanges'><span class='k-icon k-i-clock'></span> [resource_displayname[recent]]</button>\"\r\n                }\r\n            ]\r\n        });\r\n\r\n        $(\"[name=refreshCache]\", container).click(async () => await CommonCacheEndpoint.rebuildCache());\r\n        $(\"[name=migrate]\", container).click(async () => await CommonCacheEndpoint.migrate(app, container));\r\n        $(\"[name=recentChanges]\", container).click(async () => await CommonCacheEndpoint.recentChanges());\r\n    },\r\n\r\n    migrate: async function (app) {\r\n        var distinctTypes = await api.get(\"ContentManagement/CommonObject?$apply=groupby((Type))\");\r\n        var migrateDialog = new Dialog({ title: \"[resource_displayname[migrate]]\", width: 620, height: \"auto\" });\r\n        var targets = app.Config.Deployment.Targets;\r\n        var typeGrid = null;\r\n        migrateDialog.template = $(\"[name=migrateCommonCacheDialog]\").html();\r\n        migrateDialog.events.migrate = async () => {\r\n            var userLogin = { User: session.user, Pass: $(\"[name=password]\", migrateDialog.element).val() };\r\n            CommonCacheEndpoint.doMigration(typeGrid, $(\"[name=environment]\", migrateDialog.element).val(), userLogin, app);\r\n            migrateDialog.events.close();\r\n        };\r\n        migrateDialog.init(async () => {\r\n            typeGrid = await CommonCacheEndpoint.setupCommonTypesGrid(migrateDialog.element, distinctTypes);\r\n            $(\"[name=environment]\", migrateDialog.element).kendoDropDownList(\r\n                { autoBind: false, optionLabel: \"[resource_displayname[selectenvironment]]\", dataTextField: \"EnvironmentName\", dataValueField: \"EnvironmentName\", dataSource: targets }\r\n            );\r\n        });\r\n    },\r\n\r\n    doMigration: async function (typeGrid, targetSelected, userLogin, app) {\r\n        notification.info(\"[resource_displayname[importing]]\");\r\n        var typesSelected = typeGrid.select().map(c => c.Type);\r\n        var exportedSet = [];\r\n        for (let i = 0; i < typesSelected.length; i = i + 1) {\r\n            exportedSet.push((await api.get(\"ContentManagement/CommonObject/Latest()?type=\" + typesSelected[i])).value);\r\n        }\r\n        var targetApi = app.Config.Deployment.Targets.filter(r => r.EnvironmentName == targetSelected)[0].Api;\r\n        var newApi = new Api({ apiRoot: targetApi });\r\n        await newApi.login(userLogin.User, userLogin.Pass, true);\r\n        await newApi.post(\"ContentManagement/CommonObject/Import\", { value: exportedSet.flat() }).then(async () => {\r\n            await newApi.get(\"RefreshCache\");\r\n            notification.success(\"[resource_displayname[imported]]\");\r\n        }).catch((err) => error(err));\r\n    },\r\n\r\n    setupCommonTypesGrid: async function (container, dataSource) {\r\n        var grid = new GridWidget($(\"[name=migrateGrid]\", container), dataSource);\r\n        grid.groupable = false;\r\n        grid.pageable = false;\r\n        grid.columns = [\r\n            { selectable: true, width: 40 },\r\n            { field: \"Type\", title: \"[resource_displayname[type]]\" }\r\n        ];\r\n        await grid.init();\r\n        return grid;\r\n    },\r\n\r\n    recentChanges: async function () {\r\n        var recentDialog = new Dialog({ title: \"[resource_displayname[recent]]\", width: 1000, height: 800 });\r\n        var recentChangesDataSource = await model.getDatasource({ endpoint: \"ContentManagement/CommonObject\", sort: { field: \"CreatedOn\", dir: \"desc\" } })\r\n        recentDialog.template = \"<div name='recentGrid' style='width:100%;height:100%;'></div>\";\r\n        recentDialog.init(async () => {\r\n            var recentGrid = new GridWidget($(\"[name=recentGrid]\", recentDialog.element), recentChangesDataSource);\r\n            recentGrid.filterable = true;\r\n            recentGrid.editable = false;\r\n            recentGrid.detailTemplate = \"<div name='content' style='min-height:400px;'></div>\";\r\n            recentGrid.detailExpand = (e) => {\r\n                if (!($(e.detailRow).attr(\"expanded\") === \"true\")) {\r\n                    $(e.detailRow).attr(\"expanded\", \"true\");\r\n                    var commonObject = recentGrid.dataItem($(e.masterRow).closest(\"tr\"));\r\n                    var codeBlock = commonObject.Json;\r\n                    var entity = JSON.parse(commonObject.Json);\r\n                    if (commonObject.Type == \"ContentManagement/Script\") { codeBlock = entity.Content; }\r\n                    else if (commonObject.Type == \"ContentManagement/Component\") { codeBlock = entity.Content + \"\\n\\n\" + entity.Script }\r\n                    var editor = new MonacoEditor($(\"[name=content]\", $(e.detailRow))[0], {\r\n                        language: \"json\",\r\n                        code: codeBlock\r\n                    });\r\n                    editor.init();\r\n                }\r\n            };\r\n            recentGrid.columns = [\r\n                {\r\n                    field: \"Name\",\r\n                    title: \"[resource_displayname[name]]\"\r\n                },\r\n                {\r\n                    field: \"Type\",\r\n                    title: \"[resource_displayname[type]]\"\r\n                },\r\n                {\r\n                    field: \"CreatedBy\",\r\n                    title: \"[resource_displayname[createdby]]\",\r\n                    width: \"[theme[columns.small]]\"\r\n                },\r\n                {\r\n                    field: \"CreatedOn\",\r\n                    title: \"[resource_displayname[createdon]]\",\r\n                    format: \"{0: \" + type.dateFormat + \"}\",\r\n                    width: \"[theme[columns.small]]\"\r\n                },\r\n                {\r\n                    field: \"Version\",\r\n                    title: \"[resource_displayname[version]]\"\r\n                }\r\n            ];\r\n            await recentGrid.init();\r\n        });\r\n    },\r\n\r\n    initNavTabIds: function (container) {\r\n        var idSuffix = Date.now();\r\n\r\n        var nav = $('#common-cache-nav-tab', container);\r\n        nav.attr('id', `${nav.attr('id')}-${idSuffix}`);\r\n\r\n        var content = $('#common-cache-nav-tabContent', container);\r\n        content.attr('id', `${content.attr('id')}-${idSuffix}`);\r\n\r\n        $('button[role=tab]', nav).each(function () {\r\n            var btn = $(this);\r\n            btn.attr('data-bs-target', `${btn.attr('data-bs-target')}-${idSuffix}`);\r\n        });\r\n\r\n        $('.tab-pane', content).each(function () {\r\n            var pane = $(this);\r\n            pane.attr('id', `${pane.attr('id')}-${idSuffix}`);\r\n        });\r\n    },\r\n\r\n\r\n\r\n    addClickListenerForComponents: async function (container) {\r\n\r\n        $('button[role=tab]', container).each(function () {\r\n\r\n            var button = $(this);\r\n\r\n            button.on('shown.bs.tab', async function (e) {\r\n\r\n                // ÃƒÆ’Ã†â€™Ãƒâ€ Ã¢â‚¬â„¢ÃƒÆ’Ã¢â‚¬Â ÃƒÂ¢Ã¢â€šÂ¬Ã¢â€žÂ¢ÃƒÆ’Ã†â€™ÃƒÂ¢Ã¢â€šÂ¬Ã‚Â ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬ÃƒÂ¢Ã¢â‚¬Å¾Ã‚Â¢ÃƒÆ’Ã†â€™Ãƒâ€ Ã¢â‚¬â„¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬Ãƒâ€¦Ã‚Â¡ÃƒÆ’Ã†â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â°ÃƒÆ’Ã†â€™Ãƒâ€ Ã¢â‚¬â„¢ÃƒÆ’Ã¢â‚¬Â ÃƒÂ¢Ã¢â€šÂ¬Ã¢â€žÂ¢ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â¦ÃƒÆ’Ã†â€™Ãƒâ€ Ã¢â‚¬â„¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬Ãƒâ€¦Ã‚Â¡ÃƒÆ’Ã†â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â¸ÃƒÆ’Ã†â€™Ãƒâ€ Ã¢â‚¬â„¢ÃƒÆ’Ã¢â‚¬Â ÃƒÂ¢Ã¢â€šÂ¬Ã¢â€žÂ¢ÃƒÆ’Ã†â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã†â€™Ãƒâ€ Ã¢â‚¬â„¢ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã¢â‚¬Â¦Ãƒâ€šÃ‚Â¡ÃƒÆ’Ã†â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã†â€™Ãƒâ€ Ã¢â‚¬â„¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬Ãƒâ€¦Ã‚Â¡ÃƒÆ’Ã†â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚ÂÃƒÆ’Ã†â€™Ãƒâ€ Ã¢â‚¬â„¢ÃƒÆ’Ã¢â‚¬Â ÃƒÂ¢Ã¢â€šÂ¬Ã¢â€žÂ¢ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã¢â‚¬Â¦Ãƒâ€šÃ‚Â¡ÃƒÆ’Ã†â€™Ãƒâ€ Ã¢â‚¬â„¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬Ãƒâ€¦Ã‚Â¡ÃƒÆ’Ã†â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â¥ Use event target instead of re-querying\r\n                var targetSelector = $(e.target).attr('data-bs-target');\r\n                var target = $(targetSelector, container);\r\n\r\n                var component = target.attr('data-component');\r\n                if (!component) return;\r\n\r\n                var existing = $(`.component[name=${component}]`, target);\r\n\r\n                if (existing.length === 0) {\r\n                    await loadComponent(target, component, async (c) => {\r\n                        await c.init(session.app, $(`.component[name=${component}]`, target));\r\n                    });\r\n                }\r\n            });\r\n        });\r\n    },\r\n\r\n    rebuildCache: async function () {\r\n        await api.get(\"RefreshCache\").then(() => notification.success(\"[resource_displayname[rebuilt]]\"));\r\n    }\r\n}",
  "Content": "<div class=\"k-toolbar\" name=\"appToolbar\"></div>\r\n\r\n<div class=\"tab-control\" name=\"tabs\">\r\n   <nav>\r\n      <div class=\"nav nav-tabs\" id=\"common-cache-nav-tab\" role=\"tablist\">\r\n\r\n         <button class=\"nav-link active\"\r\n                 id=\"common-cache-components-tab\"\r\n                 data-bs-toggle=\"tab\"\r\n                 data-bs-target=\"#common-cache-components\"\r\n                 type=\"button\"\r\n                 role=\"tab\"\r\n                 aria-controls=\"common-cache-components\"\r\n                 aria-selected=\"true\">\r\n            <span class=\"k-icon k-i-source-code\"></span>\r\n            [resource_displayname[components]]\r\n         </button>\r\n\r\n         <button class=\"nav-link\"\r\n                 id=\"common-cache-resources-tab\"\r\n                 data-bs-toggle=\"tab\"\r\n                 data-bs-target=\"#common-cache-resources\"\r\n                 type=\"button\"\r\n                 role=\"tab\"\r\n                 aria-controls=\"common-cache-resources\"\r\n                 aria-selected=\"false\">\r\n            <span class=\"k-icon k-i-foreground-color\"></span>\r\n            [resource_displayname[resources]]\r\n         </button>\r\n\r\n         <button class=\"nav-link\"\r\n                 id=\"common-cache-scripts-tab\"\r\n                 data-bs-toggle=\"tab\"\r\n                 data-bs-target=\"#common-cache-scripts\"\r\n                 type=\"button\"\r\n                 role=\"tab\"\r\n                 aria-controls=\"common-cache-scripts\"\r\n                 aria-selected=\"false\">\r\n            <span class=\"k-icon k-i-js\"></span>\r\n            [resource_displayname[scripts]]\r\n         </button>\r\n\r\n      </div>\r\n   </nav>\r\n\r\n   <div class=\"tab-content\" id=\"common-cache-nav-tabContent\">\r\n\r\n      <div class=\"tab-pane fade show active\"\r\n           id=\"common-cache-components\"\r\n           role=\"tabpanel\"\r\n           aria-labelledby=\"common-cache-components-tab\"\r\n           name=\"ComponentGrid\"\r\n           data-component=\"CommonCacheComponents\">\r\n      </div>\r\n\r\n      <div class=\"tab-pane fade\"\r\n           id=\"common-cache-resources\"\r\n           role=\"tabpanel\"\r\n           aria-labelledby=\"common-cache-resources-tab\"\r\n           name=\"ResourceGrid\"\r\n           data-component=\"CommonCacheResources\">\r\n      </div>\r\n\r\n      <div class=\"tab-pane fade\"\r\n           id=\"common-cache-scripts\"\r\n           role=\"tabpanel\"\r\n           aria-labelledby=\"common-cache-scripts-tab\"\r\n           name=\"ScriptGrid\"\r\n           data-component=\"CommonCacheScripts\">\r\n      </div>\r\n\r\n   </div>\r\n</div>\r\n\r\n<script type=\"text/template\" name=\"migrateCommonCacheDialog\">\r\n\r\n\t<div class=\"input-group input-group-sm mb-2\">\r\n\t\t<span class=\"input-group-text\">[resource_displayname[environment]]</span>\r\n\t\t<input type=\"text\" class=\"form-control\" name=\"environment\" />\r\n\t</div>\r\n\r\n\t<div class=\"input-group input-group-sm mb-2\">\r\n\t\t<span class=\"input-group-text\">[resource_displayname[password]]</span>\r\n\t\t<input type=\"password\" class=\"form-control\" name=\"password\" />\r\n\t</div>\r\n\r\n\t<div class=\"input-group input-group-sm mb-2\">\r\n\t\t<span class=\"input-group-text\">[resource_displayname[selecttypes]]</span>\r\n\t\t<div class=\"form-control p-0 border-0\">\r\n\t\t\t<div name=\"migrateGrid\" style=\"max-height:400px;\"></div>\r\n\t\t</div>\r\n\t</div>\r\n\r\n\t<hr />\r\n\r\n\t<div class=\"d-flex justify-content-end\">\r\n\t\t<button type=\"button\" class=\"btn btn-sm btn-primary\" name=\"migrate\">\r\n\t\t\t<span class=\"k-icon k-i-arrow-up\"></span>\r\n\t\t\t[resource_displayname[migrate]]\r\n\t\t</button>\r\n\t</div>\r\n\r\n</script>\r\n\r\n<style scoped>\r\n\t.component[name=CommonCacheEndpoint] > [name=appToolbar] { flex: none;  height: 5%; }\r\n\t.component[name=CommonCacheEndpoint] { height: 99%; }\r\n\tdiv[name=tabs] {flex: 1; max-height: 95% }\r\n\t.content {overflow: hidden !important; }\r\n\t</style>",
  "LastUpdated": "2026-03-05T09:48:19.7442884+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "CommonCacheResources",
  "Key": "CMS",
  "ResourceKey": "CMS",
  "Script": "CommonCacheResources = {\r\n    init: async function(app, container) {\r\n        app = app || session.app;\r\n        container = container || $(\".component[name=CommonCacheResources]\");\r\n        var resourceData = await CommonCacheResources.getDataSource(\"ContentManagement/CommonObject/Latest()?type=ContentManagement/Resource&$filter=Culture eq ''&$orderby=Name asc\");\r\n        await CommonCacheResources.setupResourceGrid(resourceData, container);\r\n    },\r\n\r\n    getDataSource(url) {\r\n        return new kendo.data.DataSource({\r\n            transport: {\r\n                read: { url: api.apiRoot + url, dataType: \"json\" }\r\n            },\r\n            pageSize: 50,\r\n            schema: {\r\n                data: function (response) {\r\n                    for(let i = 0; i < response.value.length; i = i + 1) {\r\n                        response.value[i].CreatedOn = new Date(response.value[i].CreatedOn);\r\n                        response.value[i].LastUpdated = new Date(response.value[i].LastUpdated);\r\n                        response.value[i].Entity = JSON.parse(response.value[i].Json); \r\n                    }\r\n                    return response.value;\r\n                },\r\n                total: function(response) {\r\n                    return response.value.length;\r\n                }\r\n            }\r\n        });;\r\n    },\r\n\r\n\tescapeHtml: function(html) {\r\n    \treturn html.replaceAll('&', '&amp;').replaceAll('<', '&lt;').replaceAll('>', '&gt;').replaceAll('\"', '&quot;').replaceAll(\"'\", '&#039;');\r\n\t},\r\n\r\n    setupResourceGrid: async function (ds, container) {\r\n        var cacheGrid = new GridWidget(container, ds);\r\n        cacheGrid.groupable = false;\r\n        cacheGrid.filterable = true;\r\n        cacheGrid.editable = true;\r\n        cacheGrid.commandWidth = 100;\r\n        cacheGrid.columns = [\r\n            { \r\n                field: \"Entity.Key\", \r\n                editable: false, \r\n                title: \"[resource_shortdisplayname[Key]]\",\t\t\t\t\r\n                width: \"[theme[columns.small]]\"\r\n            },\r\n            { \r\n                field: \"Name\", \r\n                editable: false, \r\n                title: \"[resource_shortdisplayname[Name]]\", \r\n\t\t\t\twidth: \"[theme[columns.small]]\"  \r\n            },\r\n\t\t\t{ \r\n                field: \"Entity.DisplayName\", \r\n                title: \"[resource_shortdisplayname[displayname]]\", \r\n                template: \"#=(Entity.DisplayName && Entity.DisplayName.length > 80) ? CommonCacheResources.escapeHtml(Entity.DisplayName.substring(0, 80)) + '...' : CommonCacheResources.escapeHtml(Entity.DisplayName)#\", encoded: true },\r\n\t\t\t{ \r\n                field: \"Entity.ShortDisplayName\", \r\n                title: \"[resource_shortdisplayname[shortdisplayname]]\", \r\n                template: \"#=(Entity.ShortDisplayName && Entity.ShortDisplayName.length > 80) ? CommonCacheResources.escapeHtml(Entity.ShortDisplayName.substring(0, 80)) + '...' : CommonCacheResources.escapeHtml(Entity.ShortDisplayName)#\", \r\n                encoded: true, \r\n\t\t\t\twidth: \"[theme[columns.small]]\" \r\n            },\r\n\t\t\t{ \r\n                field: \"Entity.Description\", \r\n                title: \"[resource_shortdisplayname[description]]\", \r\n                template: \"#=(Entity.Description && Entity.Description.length > 80) ? CommonCacheResources.escapeHtml(Entity.Description.substring(0, 80)) + '...' : CommonCacheResources.escapeHtml(Entity.Description)#\", \r\n                encoded: true \r\n            },\r\n            { \r\n                field: \"LastUpdated\", \r\n                title: \"[resource_displayname[lastupdated]]\", \r\n                type: \"date\", \r\n                format: \"{0: \" + type.dateFormat + \" HH:mm}\", \r\n\t\t\t\twidth: \"[theme[columns.small]]\"\r\n            },\r\n            { \r\n                field: \"LastUpdatedBy\", \r\n                title: \"[resource_displayname[lastupdatedby]]\", \r\n\t\t\t\twidth: \"[theme[columns.small]]\"\r\n            },\r\n        ];\r\n        cacheGrid.commands.push({ name: \"save\", icon: \"k-i-save\", text: \"[resource_displayname[save]]\" });\r\n        cacheGrid.commands.push({ name: \"remove\", icon: \"k-i-trash\", text: \"[resource_displayname[remove]]\" });\r\n        cacheGrid.toolbar = \"<button class='btn btn-sm btn-primary' name='add'><span class='k-icon k-i-plus'></span> [resource_displayname[add]]</button>\";\r\n        cacheGrid.dataBound = function (e) {\r\n            $(\"[name=save]\", cacheGrid.gridElement).off(\"click\").on(\"click\", (e) => CommonCacheResources.save(e, cacheGrid));\r\n            $(\"[name=remove]\", cacheGrid.gridElement).off(\"click\").on(\"click\", (e) => CommonCacheResources.removeFromCache(e, cacheGrid));\r\n        };\r\n        cacheGrid.detailTemplate = \"<div name='translationsGrid'></div>\";\r\n        cacheGrid.detailExpand = async (e) => await CommonCacheResources.expandResourceGrid(e, cacheGrid);\r\n        await cacheGrid.init();\r\n        $(\"[name=add]\", cacheGrid.gridElement).off(\"click\").on(\"click\", (e) => CommonCacheResources.newResource(e, cacheGrid));\r\n        cacheGrid.kendoObject.dataSource.group({ field: \"Entity.Key\" });\r\n    },\r\n\r\n\r\n    save: async function(e, grid) {\r\n        var item = grid.dataItem($(e.currentTarget).closest(\"tr\"));\r\n        var dupe = JSON.parse(JSON.stringify(item));\r\n        dupe.Entity.Key = dupe.Key;\r\n        dupe.Json = JSON.stringify(dupe.Entity);\r\n        delete dupe.Entity;\r\n        await api.update(\"ContentManagement/CommonObject(\" + item.Id + \")\", dupe).then(async () => {\r\n            notification.success(\"[resource_displayname[saved]]\");\r\n            await api.get(\"RefreshCache\").then(() => notification.success(\"[resource_displayname[rebuilt]]\"));\r\n        }).catch((err) => error(err));\r\n    },\r\n\r\n    removeFromCache: async function (e, grid) {\r\n        var item = grid.kendoObject.dataItem($(e.currentTarget).closest(\"tr\"));\r\n        await api.destroy(\"ContentManagement/CommonObject(\" + item.Id + \")\").then(async () => {\r\n            notification.success(\"[resource_displayname[deleted]]\");\r\n            await api.get(\"RefreshCache\").then(() => {\r\n                notification.success(\"[resource_displayname[rebuilt]]\")\r\n                grid.refresh();\r\n            }).catch((err) => error(err));\r\n        }).catch((err) => error(err));\r\n    },\r\n\r\n\r\n    expandResourceGrid: async function (e, grid) {\r\n        var container = $(e.detailRow);\r\n        if($(container).attr(\"expanded\") === \"true\") { return; }\r\n        $(container).attr(\"expanded\", \"true\");\r\n        var resource = grid.dataItem(e.masterRow).Entity;\r\n        var translations = await CommonCacheResources.getDataSource(\"ContentManagement/CommonObject/Latest()?type=ContentManagement/Resource&$filter=Name eq '\" + resource.Name + \"' and Culture ne '' and Key eq '\" + resource.Key + \"'\");\r\n        var subGrid = new GridWidget($(\"[name=translationsGrid]\", container), translations);\r\n        subGrid.pageable = false;\r\n        subGrid.groupable = false;\r\n        subGrid.toolbar = \"<div><button name='createTranslation'><span class='k-icon k-i-plus'></span>[resource_displayname[NewTranslation]]</button></div>\";\r\n        subGrid.columns = [\r\n            { field: \"Culture\", editable: false, title: \"[resource_shortdisplayname[Culture]]\", width: 100 },\r\n            { field: \"Name\", editable: false, title: \"[resource_shortdisplayname[Name]]\", width: 200 },\r\n\t\t\t{ field: \"Entity.DisplayName\", title: \"[resource_shortdisplayname[displayname]]\", template: \"#=(Entity.DisplayName && Entity.DisplayName.length > 80) ? CommonCacheResources.escapeHtml(Entity.DisplayName.substring(0, 80)) + '...' : CommonCacheResources.escapeHtml(Entity.DisplayName)#\", encoded: true },\r\n\t\t\t{ field: \"Entity.ShortDisplayName\", title: \"[resource_shortdisplayname[shortdisplayname]]\", template: \"#=(Entity.ShortDisplayName && Entity.ShortDisplayName.length > 80) ? CommonCacheResources.escapeHtml(Entity.ShortDisplayName.substring(0, 80)) + '...' : CommonCacheResources.escapeHtml(Entity.ShortDisplayName)#\", encoded: true },\r\n\t\t\t{ field: \"Entity.Description\", title: \"[resource_shortdisplayname[description]]\", template: \"#=(Entity.Description && Entity.Description.length > 80) ? CommonCacheResources.escapeHtml(Entity.Description.substring(0, 80)) + '...' : CommonCacheResources.escapeHtml(Entity.Description)#\", encoded: true },\r\n            { field: \"LastUpdated\", title: \"[resource_displayname[lastupdated]]\", width: 120, type: \"date\", format: \"{0: \" + type.dateFormat + \" HH:mm}\" },\r\n            { field: \"LastUpdatedBy\", title: \"[resource_displayname[lastupdatedby]]\", width: 200 },\r\n        ];\r\n        subGrid.commands.push({ name: \"save\", icon: \"k-i-save\", text: \"[resource_shortdisplayname[save]]\" });\r\n        subGrid.commands.push({ name: \"destroy\", icon: \"k-i-trash\", text: \"[resource_shortdisplayname[delete]]\" });\r\n        subGrid.dataBound = async function () {\r\n            $(\"[name=save]\", subGrid.gridElement).on(\"click\", async (e) => await CommonCacheResources.save(e, subGrid));\r\n            $(\"[name=destroy]\", subGrid.gridElement).on(\"click\", async (e) => await CommonCacheResources.deleteTranslation(e, subGrid));\r\n        };\r\n        await subGrid.init();\r\n        $(\"button[name=createTranslation]\", subGrid.gridElement).on(\"click\", async function (e) { await CommonCacheResources.createTranslation(e, subGrid, resource); });\r\n    },\r\n\r\n\r\n    deleteTranslation: async function (e, grid) {\r\n        e.preventDefault();\r\n        var item = grid.dataItem($(e.currentTarget).closest(\"tr\"));\r\n        await api.destroy(\"ContentManagement/CommonObject(\" + item.Id + \")\").then(async () => {\r\n            await api.get(\"RefreshCache\").then(() => {\r\n                notification.success(\"[resource_displayname[rebuilt]]\");\r\n                grid.refresh();\r\n                notification.success(\"[resource_displayname[deleted]]\");\r\n            }).catch((err) => error(err));\r\n        }).catch((err) => error(err));\r\n    },\r\n\r\n    createTranslation: async function (e, grid, item) {\r\n        e.preventDefault();\r\n        var translations = (await api.get(\"ContentManagement/CommonObject/Latest()?type=ContentManagement/Resource&$filter=Name eq '\" + item.Name + \"' and Key eq '\" + item.Key + \"'\")).value;\r\n        var cultures = (await api.get(\"ContentManagement/Culture\")).value;\r\n        var options = cultures.filter((c) => c.Id !== \"\" && translations.filter((r)  => r.Culture === c.Id).length === 0);\r\n        var createTranslationDialog = new Dialog({ width: 520, height: \"auto\", title: \"[resource_displayname[newtranslation]]\" });\r\n        createTranslationDialog.template = $(\"[name=createTranslationDialog]\").html();\r\n        createTranslationDialog.events.confirm = async function (e) {\r\n            var resource = {\r\n                Culture: culture.data(\"kendoDropDownList\").value(),\r\n                Key: item.Key,\r\n                Name: item.Name,\r\n                DisplayName: $(\"[name=displayName]\", createTranslationDialog.element).val(),\r\n                ShortDisplayName: $(\"[name=shortDisplayName]\", createTranslationDialog.element).val(),\r\n                Description: $(\"[name=description]\", createTranslationDialog.element).val()\r\n            };\r\n            var commonObject = {\r\n                Id: 0,\r\n                Key: resource.Key,\r\n                Name: resource.Name,\r\n                CreatedOn: resource.CreatedOn,\r\n                LastUpdated: resource.LastUpdated,\r\n                Json: JSON.stringify(resource),\r\n                Type: \"ContentManagement/Resource\",\r\n                Culture: resource.Culture,\r\n                Version: 1\r\n            };\r\n            await api.add(\"ContentManagement/CommonObject\", commonObject).then(async () => {\r\n                notification.success(\"[resource_displayname[added]]\");\r\n                await api.get(\"RefreshCache\")\r\n                    .then(() => {\r\n                        notification.success(\"[resource_displayname[rebuilt]]\");\r\n                        grid.refresh();\r\n                    })\r\n                    .catch((err) => error(err));\r\n                createTranslationDialog.events.close();\r\n            }).catch((err) => error(err));\r\n        };\r\n        createTranslationDialog.init(() => {\r\n            culture = $(\"[name=culture]\", createTranslationDialog.element).kendoDropDownList({ dataTextField: \"Name\", dataValueField: \"Id\", dataSource: options, index: 0 });\r\n        });\r\n    },\r\n\r\n    newResource: async function (e, cacheGrid) {\r\n        e.preventDefault();\r\n        var newResourceDialog = new Dialog({ width: 520, height: \"auto\", title: \"[resource_displayname[newresource]]\" });\r\n        newResourceDialog.template = $(\"[name=newResourceDialog]\").html();\r\n        newResourceDialog.events.create = async function (e) {\r\n            var resource = {\r\n                Name: $(\"[name=name]\", newResourceDialog.element).val(),\r\n                Key: $(\"[name=key]\", newResourceDialog.element).val(),\r\n                DisplayName: $(\"[name=displayName]\", newResourceDialog.element).val(),\r\n                ShortDisplayName: $(\"[name=shortDisplayName]\", newResourceDialog.element).val(),\r\n                Description: $(\"[name=description]\", newResourceDialog.element).val(),\r\n                CreatedOn: (new Date()).toISOString(),\r\n                LastUpdated: (new Date()).toISOString(),\r\n                Culture: \"\"\r\n            };\r\n            var commonObject = {\r\n                Id: 0, Name: resource.Name,\r\n                Key: resource.Key,\r\n                CreatedOn: resource.CreatedOn,\r\n                LastUpdated: resource.LastUpdated,\r\n                Json: JSON.stringify(resource),\r\n                Type: \"ContentManagement/Resource\",\r\n                Culture: resource.Culture,\r\n                Version: 1\r\n            };\r\n            await api.add(\"ContentManagement/CommonObject\", commonObject).then(async () => {\r\n                notification.success(\"[resource_displayname[added]]\");\r\n                await api.get(\"RefreshCache\").then(() => notification.success(\"[resource_displayname[rebuilt]]\"));\r\n                cacheGrid.refresh();\r\n            }).catch((err) => error(err));\r\n            newResourceDialog.events.close();\r\n        };\r\n        newResourceDialog.init();\r\n    }\r\n}",
  "Content": "<script type=\"text/template\" name=\"newResourceDialog\">\r\n\r\n\t<div class=\"input-group input-group-sm mb-2\">\r\n\t\t<span class=\"input-group-text\">[resource_shortdisplayname[key]]</span>\r\n\t\t<input type=\"text\" class=\"form-control\" name=\"key\" />\r\n\t</div>\r\n\r\n\t<div class=\"input-group input-group-sm mb-2\">\r\n\t\t<span class=\"input-group-text\">[resource_shortdisplayname[name]]</span>\r\n\t\t<input type=\"text\" class=\"form-control\" name=\"name\" />\r\n\t</div>\r\n\r\n\t<div class=\"input-group input-group-sm mb-2\">\r\n\t\t<span class=\"input-group-text\">[resource_shortdisplayname[displayname]]</span>\r\n\t\t<input type=\"text\" class=\"form-control\" name=\"displayName\" />\r\n\t</div>\r\n\r\n\t<div class=\"input-group input-group-sm mb-2\">\r\n\t\t<span class=\"input-group-text\">[resource_shortdisplayname[shortdisplayname]]</span>\r\n\t\t<input type=\"text\" class=\"form-control\" name=\"shortDisplayName\" />\r\n\t</div>\r\n\r\n\t<div class=\"input-group input-group-sm mb-2\">\r\n\t\t<span class=\"input-group-text\">[resource_shortdisplayname[description]]</span>\r\n\t\t<input type=\"text\" class=\"form-control\" name=\"description\" />\r\n\t</div>\r\n\r\n\t<hr />\r\n\r\n\t<div class=\"d-flex justify-content-end\">\r\n\t\t<button type=\"button\" class=\"btn btn-sm btn-primary\" name=\"create\">\r\n\t\t\t<span class=\"k-icon k-i-plus\"></span>\r\n\t\t\t[resource_shortdisplayname[create]]\r\n\t\t</button>\r\n\t</div>\r\n\r\n</script>\r\n\r\n<script type=\"text/template\" name=\"createTranslationDialog\">\r\n<div class='dialog'>\r\n   <ul class='fieldList'>\r\n      <li>\r\n         <label>[resource_displayname[culture]]</label>\r\n         <div class='value'>\r\n            <select name='culture'></select>\r\n         </div>\r\n      </li>\r\n      <li>\r\n     \t<label>[resource_shortdisplayname[displayname]]</label>\r\n        <div class=\"value\">\r\n        \t<input type=\"text\" name=\"displayName\"></input>\r\n        </div>\r\n     </li>\r\n     <li>\r\n     \t<label>[resource_shortdisplayname[shortdisplayname]]</label>\r\n        <div class=\"value\">\r\n        \t<input type=\"text\" name=\"shortDisplayName\"></input>\r\n        </div>\r\n     </li>\r\n     <li>\r\n     \t<label>[resource_shortdisplayname[description]]</label>\r\n        <div class=\"value\">\r\n        \t<input type=\"text\" name=\"description\"></input>\r\n        </div>\r\n     </li>\r\n   </ul>\r\n   <hr>\r\n   <div class=\"value\">\r\n      <button name=\"confirm\">[resource_displayname[create]]</button>\r\n   </div>\r\n   </li>\r\n</div>\r\n</script>\r\n\r\n<script type=\"template\" name=\"resourceDetails\">\r\n  <div name=\"translations\" style=\"min-height:250px;\"></div>\r\n</script>\r\n\r\n<style scoped>\r\n   .component[name=CommonCacheResources]  { height: 100%; }\r\n   .component [name=CommonCacheResources] { flex: none !important; height: 100%; }\r\n   .component[name=CommonCacheResources] > [name=CommonCacheResourcesGrid] { flex: none !important; height: 100%; }\r\n   </style>",
  "LastUpdated": "2026-03-05T09:57:01.1646896+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "CommonCacheScripts",
  "Key": "CMS",
  "ResourceKey": "CMS",
  "Script": "CommonCacheScripts = {\r\n    init: async function (app, container) {\r\n        app = app || session.app;\r\n        container = container || $(\".component[name=CommonCacheScripts]\");\r\n        var scriptData = await CommonCacheScripts.getDataSource(\"ContentManagement/CommonObject/Latest()?type=ContentManagement/Script&$orderby=Name asc\");\r\n        await CommonCacheScripts.setupScriptGrid(scriptData, container);\r\n    },\r\n\r\n    getDataSource(url) {\r\n        return new kendo.data.DataSource({\r\n            transport: {\r\n                read: { url: api.apiRoot + url, dataType: \"json\" }\r\n            },\r\n            pageSize: 50,\r\n            schema: {\r\n                data: function (response) {\r\n                    for (let i = 0; i < response.value.length; i++) {\r\n                        response.value[i].CreatedOn = new Date(response.value[i].CreatedOn);\r\n                        response.value[i].LastUpdated = new Date(response.value[i].LastUpdated);\r\n                        response.value[i].Entity = JSON.parse(response.value[i].Json);\r\n                    }\r\n                    return response.value;\r\n                },\r\n                total: (response) => response.value.length\r\n            },\r\n            group: { field: \"Key\" }\r\n        });\r\n    },\r\n\r\n    setupScriptGrid: async function (ds, container) {\r\n        var cacheGrid = new GridWidget(container, ds);\r\n        cacheGrid.columns = [\r\n            { field: \"Key\", title: \"[resource_shortdisplayname[key]]\", width: \"[theme[columns.small]]\", width: 100},\r\n            { field: \"Name\", title: \"[resource_shortdisplayname[name]]\", width: 600 },\r\n            { field: \"LastUpdated\", title: \"[resource_displayname[lastupdated]]\", type: \"date\", format: \"{0: \" + type.dateFormat + \" HH:mm}\", width: \"[theme[columns.small]]\", width: 160},\r\n            { field: \"LastUpdatedBy\", title: \"[resource_displayname[lastupdatedby]]\", width: \"[theme[columns.small]]\", width: 300 },\r\n        ];\r\n        cacheGrid.groupable = false;\r\n        cacheGrid.filterable = true;\r\n        cacheGrid.editable = false;\r\n        cacheGrid.pageable = true;\r\n        cacheGrid.detailTemplate = kendo.template($(\"[name=scriptDetails]\", container).html());\r\n\r\n        cacheGrid.detailExpand = function (e) {\r\n            var container = $(e.detailRow);\r\n\r\n            if (container.find(\".monaco-editor\").length === 0) {\r\n                var scriptEntity = cacheGrid.dataItem(e.masterRow).Entity;\r\n                var scriptEditorDiv = container.find(\"div[name=script]\")[0];\r\n\r\n                var script = new JavaScriptMonacoEditor(scriptEditorDiv, { code: scriptEntity.Content });\r\n                script.onChange = () => {\r\n                    scriptEntity.Content = script.getValue();\r\n                };\r\n                script.init();\r\n            }\r\n        };\r\n\r\n        cacheGrid.commands.push({ name: \"viewVersions\", icon: \"k-i-clock\", text: \"[resource_displayname[viewversions]]\" });\r\n        cacheGrid.commands.push({ name: \"save\", icon: \"k-i-save\", text: \"[resource_displayname[save]]\" });\r\n        cacheGrid.commands.push({ name: \"remove\", icon: \"k-i-trash\", text: \"[resource_displayname[remove]]\" });\r\n\r\n        cacheGrid.toolbar = \"<button class='btn btn-sm btn-primary' name='add'><span class='k-icon k-i-plus'></span> [resource_displayname[add]]</button>\";\r\n\r\n        cacheGrid.dataBound = function () {\r\n            $(\"[name=save]\", cacheGrid.gridElement).off(\"click\").on(\"click\", (e) => CommonCacheScripts.save(e, cacheGrid));\r\n            $(\"[name=remove]\", cacheGrid.gridElement).off(\"click\").on(\"click\", (e) => CommonCacheScripts.removeFromCache(e, cacheGrid));\r\n            $(\"[name=viewVersions]\", cacheGrid.gridElement).off(\"click\").on(\"click\", (e) => CommonCacheScripts.viewVersions(e, cacheGrid));\r\n        };\r\n\r\n        await cacheGrid.init();\r\n        $(\"[name=add]\", cacheGrid.gridElement).off(\"click\").on(\"click\", (e) => CommonCacheScripts.newScript(e, cacheGrid));\r\n    },\r\n\r\n    save: async function (e, grid) {\r\n        var item = grid.dataItem($(e.currentTarget).closest(\"tr\"));\r\n        var dupe = JSON.parse(JSON.stringify(item));\r\n        dupe.Entity.Key = dupe.Key;\r\n        dupe.Json = JSON.stringify(dupe.Entity);\r\n        delete dupe.Entity;\r\n\r\n        await api.update(\"ContentManagement/CommonObject(\" + item.Id + \")\", dupe)\r\n            .then(async () => {\r\n                notification.success(\"[resource_displayname[saved]]\");\r\n                await api.get(\"RefreshCache\");\r\n                notification.success(\"[resource_displayname[rebuilt]]\");\r\n                grid.refresh();\r\n            })\r\n            .catch((err) => error(err));\r\n    },\r\n\r\n    newScript: async function (e, cacheGrid) {\r\n        e.preventDefault();\r\n        var newScriptDialog = new Dialog({ width: 520, height: \"auto\", title: \"[resource_displayname[newscript]]\" });\r\n        newScriptDialog.template = $(\"[name=newScriptDialog]\").html();\r\n\r\n        newScriptDialog.events.create = async function () {\r\n            var script = {\r\n                Name: $(\"[name=name]\", newScriptDialog.element).val(),\r\n                Key: $(\"[name=key]\", newScriptDialog.element).val(),\r\n                Content: \"\",\r\n                CreatedOn: (new Date()).toISOString(),\r\n                LastUpdated: (new Date()).toISOString()\r\n            };\r\n\r\n            var commonObject = {\r\n                Id: 0,\r\n                Key: script.Key,\r\n                Name: script.Name,\r\n                CreatedOn: script.CreatedOn,\r\n                LastUpdated: script.LastUpdated,\r\n                Json: JSON.stringify(script),\r\n                Type: \"ContentManagement/Script\",\r\n                Culture: \"\",\r\n                Version: 1\r\n            };\r\n\r\n            await api.add(\"ContentManagement/CommonObject\", commonObject)\r\n                .then(async () => {\r\n                    notification.success(\"[resource_displayname[added]]\");\r\n                    await api.get(\"RefreshCache\");\r\n                    notification.success(\"[resource_displayname[rebuilt]]\");\r\n                    cacheGrid.refresh();\r\n                })\r\n                .catch((err) => error(err));\r\n\r\n            newScriptDialog.events.close();\r\n        };\r\n\r\n        newScriptDialog.init();\r\n    },\r\n\r\n    removeFromCache: async function (e, grid) {\r\n        var item = grid.kendoObject.dataItem($(e.currentTarget).closest(\"tr\"));\r\n        await api.destroy(\"ContentManagement/CommonObject(\" + item.Id + \")\")\r\n            .then(async () => {\r\n                notification.success(\"[resource_displayname[deleted]]\");\r\n                await api.get(\"RefreshCache\");\r\n                notification.success(\"[resource_displayname[rebuilt]]\");\r\n                grid.refresh();\r\n            })\r\n            .catch((err) => error(err));\r\n    },\r\n\r\n    viewVersions: async function (e, grid) {\r\n        var item = grid.dataItem($(e.currentTarget).closest(\"tr\"));\r\n        var versionDialog = new Dialog({ title: \"[resource_displayname[viewhistory]]\", height: 800, width: 1200 });\r\n        versionDialog.init(() => CommonCacheScripts.versionDialogInit(versionDialog, item));\r\n    },\r\n\r\n    versionDialogInit: async function (dialog, item) {\r\n        let dataSource = (await api.get(\r\n            \"ContentManagement/CommonObject?$filter=Type eq 'ContentManagement/Script' and Name eq '\" + item.Name + \"'&$orderby=Version desc\"\r\n        )).value;\r\n\r\n        var grid = new GridWidget(dialog.element, { data: dataSource, pageSize: 20 });\r\n        grid.columns = [\r\n            { field: \"Version\", title: \"[resource_displayname[version]]\" },\r\n            { field: \"LastUpdated\", title: \"[resource_displayname[lastupdated]]\", width: 120, type: \"date\", format: \"{0: \" + type.dateFormat + \" HH:mm}\" },\r\n            { field: \"LastUpdatedBy\", title: \"[resource_displayname[lastupdatedby]]\", width: 200 }\r\n        ];\r\n\r\n        grid.groupable = false;\r\n        grid.detailTemplate = `<div style='height:600px; padding-bottom: 40px;'>\r\n            <div name='scriptEditor'><h4>[resource_displayname[recentscriptchanges]]</h4></div>\r\n        </div>`;\r\n\r\n        grid.detailExpand = function (e) {\r\n            var versionedScript = JSON.parse(item.Json);\r\n            var originalScript = JSON.parse(grid.dataItem(e.masterRow).Json);\r\n            var expandContainer = $(e.detailRow);\r\n\r\n            var originalScriptModel = monaco.editor.createModel(originalScript.Content, \"text/javascript\");\r\n            var modifiedScriptModel = monaco.editor.createModel(versionedScript.Content, \"text/javascript\");\r\n\r\n            var diffEditorScript = monaco.editor.createDiffEditor(\r\n                $(\"[name=scriptEditor]\", expandContainer)[0],\r\n                { automaticLayout: true }\r\n            );\r\n\r\n            diffEditorScript.setModel({ original: originalScriptModel, modified: modifiedScriptModel });\r\n\r\n            monaco.editor.createDiffNavigator(diffEditorScript, { followsCaret: true, ignoreCharChanges: true });\r\n        };\r\n\r\n\r\n        await grid.init();\r\n    }\r\n};\r\n",
  "Content": "<script type=\"template\" name=\"scriptDetails\">\r\n<div name=\"scriptEditor\">\r\n\t<div class=\"scriptEditorContainer\">\r\n\t\t<h4>[resource_displayname[Script]]</h4>\r\n\t\t<div name=\"script\"></div>\r\n\t</div>\r\n</div>\r\n</script>\r\n\r\n<script type=\"text/template\" name=\"newScriptDialog\">\r\n\r\n\t<div class=\"input-group input-group-sm mb-2\">\r\n\t\t<span class=\"input-group-text\">[resource_shortdisplayname[key]]</span>\r\n\t\t<input type=\"text\" class=\"form-control\" name=\"key\" />\r\n\t</div>\r\n\r\n\t<div class=\"input-group input-group-sm mb-2\">\r\n\t\t<span class=\"input-group-text\">[resource_shortdisplayname[name]]</span>\r\n\t\t<input type=\"text\" class=\"form-control\" name=\"name\" />\r\n\t</div>\r\n\r\n\t<hr />\r\n\r\n\t<div class=\"d-flex justify-content-end\">\r\n\t\t<button type=\"button\" class=\"btn btn-sm btn-primary\" name=\"create\">\r\n\t\t\t<span class=\"k-icon k-i-plus\"></span>\r\n\t\t\t[resource_shortdisplayname[create]]\r\n\t\t</button>\r\n\t</div>\r\n\r\n</script>\r\n\r\n<style type=\"text/css\">\r\n .scriptEditorContainer { display: inline-block; width: 100%; height: 500px; margin-right: 10px; margin-bottom: 40px; }\r\n .scriptEditorContainer > textarea\t{ width: 99%; height: 100%; }\r\n   .component[name=CommonCacheScripts]  { height: 100%; }\r\n   .component[name=CommonCacheScripts] { flex: none !important; height: 100%; }\r\n   .component[name=CommonCacheScripts] > [name=CommonCacheScriptsGrid] { flex: none !important; height: 100%; }\r\n   div[name=scriptEditor] > div.monaco-diff-editor {height: 500px !important;}\r\n</style>",
  "LastUpdated": "2026-03-05T10:30:04.1263747+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "ComponentManagement",
  "Key": "CMS",
  "ResourceKey": "CMS",
  "Script": "ComponentManagement = {\r\n\tinit: async function (app, container) {\r\n\t\tapp = app || session.app;\r\n\t\tcontainer = container || $(\".component[name=ComponentManagement]\");\r\n\t\tapi.addToMetaCache([\n\t\t\t{\n\t\t\t\t\"Name\": \"ContentManagement\",\n\t\t\t\t\"Types\": [\n\t\t\t\t\t[meta[ContentManagement/Component]]\n\t\t\t\t]\n\t\t\t}\n\t\t]);\r\n\t\tvar config = {\r\n\t\t\tendpoint: \"ContentManagement/Component\",\r\n\t\t\todataAppend: \"?$filter=AppId eq \" + app.Id,\r\n\t\t\tgroup: {\r\n\t\t\t\tfield: \"Key\"\r\n\t\t\t},\r\n\t\t\tsort: [\r\n\t\t\t\t{\r\n\t\t\t\t\tfield: \"Key\", dir: \"asc\"\r\n\t\t\t\t},\r\n\t\t\t\t{\r\n\t\t\t\t\tfield: \"Name\", dir: \"asc\"\r\n\t\t\t\t}\r\n\t\t\t]\r\n\t\t};\r\n\t\tlet ds = await model.getDatasource(config);\r\n\t\tvar grid = new GridWidget(container, ds);\r\n\t\tgrid.toolbar = [\r\n\t\t\t{\r\n\t\t\t\ttemplate: `<div class=\"btn-group btn-group-sm\">\r\n\t<button class=\"btn btn-primary\" name=\"create\">\r\n\t\t<span class=\"k-icon k-i-plus\"></span>[resource_displayname[new]]\r\n\t</button>\r\n\t<button class=\"btn btn-primary\" name=\"migrate\">\r\n\t\t<span class=\"k-icon k-i-arrow-up\"></span>[resource_displayname[migrate]]\r\n\t</button>\r\n</div>`\r\n\t\t\t}\r\n\t\t];\r\n\t\tgrid.search = {\r\n\t\t\tfields: [\"Key\", \"ResourceKey\", \"Name\", \"LastUpdatedBy\"]\r\n\t\t};\r\n\t\tgrid.columns = [\r\n\t\t\t{\r\n\t\t\t\tfield: \"Key\",\r\n\t\t\t\ttitle: \"[resource_displayname[key]]\",\r\n                width: \"[theme[columns.small]]\"\r\n\t\t\t},\r\n\t\t\t{\r\n\t\t\t\tfield: \"ResourceKey\",\r\n\t\t\t\ttitle: \"[resource_displayname[resourcekey]]\",\r\n                width: \"[theme[columns.small]]\"\r\n\t\t\t},\r\n\t\t\t{\r\n\t\t\t\tfield: \"Name\",\r\n\t\t\t\ttitle: \"[resource_displayname[name]]\"\r\n\t\t\t},\r\n\t\t\t{ \r\n                field: \"LastUpdated\", \r\n                title: \"[resource_displayname[lastupdated]]\", \r\n                type: \"date\", \r\n                format: \"{0: \" + type.dateFormat + \" HH:mm}\",\r\n                width: \"[theme[columns.small]]\" \r\n            },\r\n            { \r\n                field: \"LastUpdatedBy\", \r\n                title: \"[resource_displayname[lastupdated]]\",\r\n                width: \"[theme[columns.small]]\"\r\n            }\r\n\t\t];\r\n\t\tgrid.commands.push({\r\n\t\t\tname: \"save\",\r\n\t\t\ticon: \"k-i-save\",\r\n\t\t\ttext: \"[resource_displayname[save]]\"\r\n\t\t});\r\n\t\tgrid.commands.push({\r\n\t\t\tname: \"delete\",\r\n\t\t\ticon: \"k-i-trash\",\r\n\t\t\ttext: \"[resource_displayname[delete]]\"\r\n\t\t});\r\n\t\tgrid.detailTemplate = kendo.template($(\"[name=componentDetails]\", container).html()),\r\n\t\tgrid.groupable = false;\r\n\t\tgrid.searchable = true;\r\n\t\tgrid.filterable = true;\r\n\t\tgrid.detailExpand = async function (e) {\r\n\t\t\tif ($(e.detailRow).find(\".monaco-editor\").length === 0) {\r\n\t\t\t\tvar component = this.dataItem(e.masterRow);\r\n\r\n\t\t\t\tvar expandContainer = $(e.detailRow);\r\n\t\t\t\tvar replaced = expandContainer.html().replaceAll('{ID}', Guid());\r\n\t\t\t\t\r\n\t\t\t\texpandContainer.html(replaced);\r\n\r\n\t\t\t\tvar contentEditor = $(e.detailRow).find(\"div[name=content] > div[name=editorContainer]\", expandContainer);\r\n\t\t\t\tvar scriptEditor = $(e.detailRow).find(\"div[name=script] > div[name=editorContainer]\", expandContainer);\r\n\t\t\t\tvar html = new HTMLMonacoEditor(contentEditor[0], {\r\n\t\t\t\t\tcode: component.Content\r\n\t\t\t\t});\r\n\t\t\t\tvar script = new JavaScriptMonacoEditor(scriptEditor[0], {\r\n\t\t\t\t\tcode: component.Script\r\n\t\t\t\t});\r\n\t\t\t\thtml.onChange = (e) => {\r\n\t\t\t\t\tcomponent.Content = html.getValue();\r\n\t\t\t\t}; \r\n\t\t\t\tscript.onChange = (e) => {\r\n\t\t\t\t\tcomponent.Script = script.getValue();\r\n\t\t\t\t};\r\n\t\t\t\thtml.init();\r\n\t\t\t\tscript.init();\r\n\r\n\t\t\t\t$('[name=body-tab-button]', expandContainer).on('click', () => {\r\n\t\t\t\t\thtml.editor.layout();\r\n\t\t\t\t});\r\n\r\n\t\t\t\t$('[name=script-tab-button]', expandContainer).on('click', () => {\r\n\t\t\t\t\tscript.editor.layout();\r\n\t\t\t\t});\r\n\t\t\t}\r\n\t\t};\r\n\t\tgrid.detailCollapse = function (e) {\r\n\t\t\t$(e.detailRow).find(\".monaco-editor\").css(\"visbility\", \"hidden\");\r\n\t\t};\r\n\t\tgrid.dataBound = function () {\r\n\t\t\t$(\"button[name=save]\", grid.gridElement).on(\"click\", (e) => ComponentManagement.save(e, grid));\r\n\t\t\t$(\"button[name=delete]\", grid.gridElement).on(\"click\", (e) => ComponentManagement.destroy(e, grid));\r\n\t\t};\r\n\t\tawait grid.init();\r\n\t\t$(\"button[name=create]\", grid.gridElement).on(\"click\", (e) => ComponentManagement.newComponent(e, grid, app));\r\n\t\t$(\"button[name=migrate]\", grid.gridElement).off(\"click\").on(\"click\", (e) => ComponentManagement.migrate(e, app));\r\n\t\t\r\n\t\tawait loadComponent($('[name=componentMigrationComponent', container), 'ComponentMigration', async (c) => {\r\n\t\t\tawait c.init(app, $('[name=componentMigrationComponent', container));\r\n\t\t});\r\n\t},\r\n\r\n\tmigrate: function (e, app) {\r\n\t\te.preventDefault();\r\n\t\tvar d = new Dialog({\r\n\t\t\twidth: 1100,\r\n\t\t\theight: 650,\r\n\t\t\ttitle: \"[resource_displayname[migrateComponents]]\"\r\n\t\t});\r\n\t\td.template = $(\"[name=componentMigrationComponent]\").first().html();\r\n\t\t// d.init(() => ComponentMigration.init(app, $(\".component[name=ComponentMigration]\", d.element)));\r\n\t\td.init();\r\n\r\n\t\tComponentMigration.initDropdown($(\".component[name=ComponentMigration]\", d.element));\r\n\t},\r\n\r\n\tnewComponent: async function (e, grid, app) {\r\n\t\te.preventDefault();\r\n\t\tvar newComponent = {\r\n\t\t\tId: 0,\r\n\t\t\tAppId: app.Id,\r\n\t\t\tName: \"\",\r\n\t\t\tKey: \"\",\r\n\t\t\tResourceKey: \"\",\r\n\t\t\tContent: \"\",\r\n\t\t\tScript: \"\"\r\n\t\t};\r\n\t\tvar args = {\r\n\t\t\tfields: [\r\n\t\t\t\t{\r\n\t\t\t\t\tfield: \"Name\",\r\n\t\t\t\t\ttitle: \"[resource_displayname[name]]\",\r\n\t\t\t\t\tdescription: \"[resource_description[name]]\"\r\n\t\t\t\t},\r\n\t\t\t\t{\r\n\t\t\t\t\tfield: \"Key\",\r\n\t\t\t\t\ttitle: \"[resource_displayname[key]]\",\r\n\t\t\t\t\tdescription: \"[resource_description[key]]\"\r\n\t\t\t\t},\r\n\t\t\t\t{\r\n\t\t\t\t\tfield: \"ResourceKey\",\r\n\t\t\t\t\ttitle: \"[resource_displayname[resourcekey]]\",\r\n\t\t\t\t\tdescription: \"[resource_description[resourcekey]]\"\r\n\t\t\t\t}\r\n\t\t\t],\r\n\t\t\ttitle: \"[resource_displayname[CreateNewComponent]]\",\r\n\t\t\tresourceKey: \"CMS\",\r\n\t\t\tdata: newComponent,\r\n\t\t\tconfirm: \"<span class='k-icon k-i-plus'></span>[resource_displayname[create]]\",\r\n\t\t\tclose: \"[resource_displayname[close]]\"\r\n\t\t};\r\n\t\tvar addComponentDialog = new EditorDialog(args);\r\n\t\taddComponentDialog.events.confirm = async function () {\r\n\t\t\tnewComponent = addComponentDialog.data.toJSON();//Remove all the kendo observable properties.\r\n\t\t\tawait api.add(\"ContentManagement/Component\", newComponent).then(() => {\r\n\t\t\t\taddComponentDialog.events.close();\r\n\t\t\t\tnotification.success(\"[resource_displayname[componentcreated]]\");\r\n\t\t\t\tgrid.refresh();\r\n\t\t\t}).catch((err) => error(err));\r\n\t\t};\r\n\t\taddComponentDialog.init();\r\n\t},\r\n\r\n\tdelay: function (callback, ms) {\r\n\t\tvar timer = 0;\r\n\t\treturn function () {\r\n\t\t\tvar context = this, args = arguments;\r\n\t\t\tclearTimeout(timer);\r\n\t\t\ttimer = setTimeout(function () {\r\n\t\t\t\tcallback.apply(context, args);\r\n\t\t\t}, ms || 0);\r\n\t\t};\r\n\t},\r\n\r\n\tsave: async function (e, grid) {\r\n\t\te.preventDefault();\r\n\t\tvar component = grid.dataItem($(e.currentTarget).closest(\"tr\"));\r\n\t\tawait component.save(e);\r\n\t\tnotification.success(\"Component Saved\");\r\n\t},\r\n\r\n\tdestroy: function (e, grid) {\r\n\t\te.preventDefault();\r\n\t\tvar d = new ConfirmDialog({\r\n\t\t\ttitle: \"[resource_displayname[areyousure]]\",\r\n\t\t\tquestion: \"[resource_displayname[areyousure]]\",\r\n\t\t\tconfirm: \"[resource_displayname[confirm]]\",\r\n\t\t\tclose: \"[resource_displayname[close]]\"\r\n\t\t});\r\n\t\td.events.confirm = function () {\r\n\t\t\tvar component = grid.dataItem($(e.currentTarget).closest(\"tr\"));\r\n\t\t\tcomponent.destroy(e);\r\n\t\t\tnotification.success(\"[resource_displayname[deleted]]\");\r\n\t\t\tgrid.refresh();\r\n\t\t\td.events.close();\r\n\t\t};\r\n\t\td.init();\r\n\t}\r\n};",
  "Content": "<div name=\"appmigrator\"></div>\r\n<script type=\"text/template\" name=\"componentDetails\">\r\n<div class=\"tab-control\" name=\"tabs\">\r\n\t<nav>\r\n\t\t<div class=\"nav nav-tabs\" id=\"app-component-nav-tab-{ID}\" role=\"tablist\">\r\n\t\t\t<button class=\"nav-link bg active\" id=\"app-component-content-tab-{ID}\" data-bs-toggle=\"tab\" data-bs-target=\"\\#app-component-content-{ID}\" type=\"button\" role=\"tab\" aria-controls=\"app-component-content-{ID}\" aria-selected=\"false\" tabindex=\"-1\" name=\"content-tab-button\">\r\n\t\t\t\t<span class=\"k-icon k-i-clipboardHtmlIcon\"></span>[resource_displayname[content]]\r\n\t\t\t</button>\r\n\t\t\t<button class=\"nav-link bg\" id=\"app-component-script-tab-{ID}\" data-bs-toggle=\"tab\" data-bs-target=\"\\#app-component-script-{ID}\" type=\"button\" role=\"tab\" aria-controls=\"app-component-script-{ID}\" aria-selected=\"true\" name=\"script-tab-button\">\r\n\t\t\t\t<span class=\"k-icon k-i-jsIcon\"></span>[resource_displayname[script]]\r\n\t\t\t</button>\r\n\t\t</div>\r\n\t</nav>\r\n\r\n\t<div class=\"tab-content\" id=\"app-component-nav-tab-{ID}Content\">\r\n\t\t<div class=\"tab-pane fade active show\" id=\"app-component-content-{ID}\" role=\"tabpanel\" aria-labelledby=\"app-component-content-tab-{ID}\" name=\"content\">\r\n\t\t\t<div name=\"editorContainer\"></div>\r\n\t\t</div>\r\n\t\t<div class=\"tab-pane fade\" id=\"app-component-script-{ID}\" role=\"tabpanel\" aria-labelledby=\"app-component-script-tab-{ID}\" name=\"script\">\r\n\t\t\t<div name=\"editorContainer\"></div>\r\n\t\t</div>\r\n\t</div>\r\n</div>\r\n</script>\r\n\r\n<div name=\"componentMigrationComponent\" style=\"display:none;\"></div>",
  "LastUpdated": "2024-11-19T18:18:30.4457869+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "ComponentMigration",
  "Key": "CMS",
  "ResourceKey": "CMS",
  "Script": "ComponentMigration = {\r\n    init: async function (app, container) {\r\n        app = app || session.app;\r\n        container = container || $(\".component[name=ComponentMigration]\");\r\n        api.addToMetaCache([\r\n        {\r\n            \"Name\": \"Core\",\r\n            \"Types\": [\r\n                [meta[ContentManagement/App]]\r\n            ]\r\n        }]);\r\n        let ds = await model.getDatasource({ endpoint: \"ContentManagement/App\" });\r\n        $(\"[name=app]\", container).kendoDropDownList({\r\n            autoBind: false,\r\n            optionLabel: \"[resource_displayname[selectapp]]\",\r\n            dataTextField: \"Name\",\r\n            dataValueField: \"Id\",\r\n            dataSource: ds\r\n        });\r\n        var componentCategories = await api.get(\"ContentManagement/Component?$filter=AppId eq \" + app.Id + \"&$select=Key\");\r\n        var list = [...new Set(componentCategories.value.map(item => item.Key))].map(x => ({ Key: x }))\r\n        var g = new GridWidget($(\"[name=componentGrid]\", container), list);\r\n        g.columns = [\r\n            { selectable: true, width: 40 },\r\n            { field: \"Key\" }\r\n        ];\r\n        g.groupable = false;\r\n        g.init(() => {\r\n            g.kendoObject.dataSource.sort({ field: \"Key\", dir: \"asc\" });\r\n            g.kendoObject.select(g.kendoObject.tbody.find(\">tr\"));\r\n        });\r\n        $(\"[name=migrate]\", container).on(\"click\", async function (e) {\r\n            var packages = await ComponentMigration.getPackages($(\"[name=componentGrid]\", container), app);\r\n            var appId = $(\"[name=app]\", container).val();\r\n            await api.add(\"Packaging/Package/ImportThis?appId=\" + appId, packages);\r\n            notification.success(\"[resource_displayname[migrated]]\");\r\n        });\r\n    },\r\n\r\n    initDropdown: async function(container) {\r\n        let ds = await model.getDatasource({\r\n            endpoint: \"ContentManagement/App\"\r\n        });\r\n\r\n        $(\"[name=app]\", container).kendoDropDownList({\r\n            optionLabel: \"[resource_displayname[selectapp]]\",\r\n            dataTextField: \"Name\",\r\n            dataValueField: \"Id\",\r\n            dataSource: ds\r\n        });\r\n    },\r\n\r\n    getPackages: async function (container, app) {\r\n        var packages = [{\r\n            Id: \"00000000-0000-0000-0000-000000000000\",\r\n            Name: \"Components\",\r\n            Description: \"Generated by ComponentManagement\",\r\n            Category: \"Dynamic\",\r\n            SourceApi: session.apiRoot,\r\n            Items: [\r\n                await ComponentMigration.getPackage(container, app)\r\n            ]\r\n        }];\r\n        return packages;\r\n    },\r\n\r\n    getPackage: async function (container, app) {\r\n        var grid = $(container).find(\".k-grid\").data(\"kendoGrid\");\r\n        var categories = [];\r\n        grid.select().each(function () {\r\n            categories.push(grid.dataItem(this).Key);\r\n        });\r\n        var query = \"\";\r\n        for (var i = 0; i < categories.length; i = i + 1) {\r\n            query += \"'\" + categories[i] + \"',\";\r\n        }\r\n        query = query.substring(0, query.length - 1);\r\n        var data = await api.get(\"ContentManagement/Component?$filter=AppId eq \" + app.Id + \" and Key in (\" + query + \")\");\r\n        for (var i = 0; i < data.value.length; i = i + 1) {\r\n            delete data.value[i].Id;\r\n        }\r\n        return {\r\n            Id: \"00000000-0000-0000-0000-000000000000\",\r\n            PackageId: \"00000000-0000-0000-0000-000000000000\",\r\n            Type: \"ContentManagement/Component\",\r\n            Data: JSON.stringify(data.value)\r\n        };\r\n    }\r\n};",
  "Content": "<div class=\"row\">\r\n    <div class=\"col-md-12\">\r\n        <div class=\"input-group input-group-sm mb-1\">\r\n            <span class=\"input-group-text\">[resource_displayname[app]]</span>\r\n            <input type=\"text\" class=\"form-control\" name=\"app\" />\r\n        </div>\r\n    </div>\r\n    <div class=\"col-md-12\">\r\n        <div name=\"componentGrid\"></div>\r\n    </div>\r\n</div>\r\n\r\n<hr />\r\n\r\n<button class=\"btn btn-sm btn-primary float-end\" name=\"migrate\">\r\n    <span class=\"k-icon k-i-arrow-up\"></span>[resource_displayname[migrate]]\r\n</button>",
  "LastUpdated": "2024-11-19T18:18:31.0606466+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "CoreManagement",
  "Key": "CMS",
  "ResourceKey": "CMS",
  "Script": "CoreManagement = {\n    init: async function (app, container) {\n        app = app || session.app;\n        container = container || $(\".component[name=CoreManagement]\");\n        api.addToMetaCache([\n            {\n                \"Name\": \"Core\",\n                \"Types\": [\n                    [meta[ContentManagement/App]],\n                    [meta[ContentManagement/Culture]]\n                ]\n            }\n        ]);\n        var cultureDataSource = await model.getDatasource({endpoint: \"ContentManagement/Culture\"});\n        var ds = await model.getDatasource({ endpoint: \"ContentManagement/App\", odataAppend: \"?$expand=Cultures\" });\n        var appGrid = new GridWidget(container, ds);\n        appGrid.groupable = false;\n        appGrid.toolbar = \"<button name='create'><span class='k-icon k-i-plus'></span>[resource_displayname[newapp]]</button>\";\n        appGrid.columns = [\n            { field: \"Name\", title: \"[resource_shortdisplayname[name]]\" },\n            { field: \"Domain\", title: \"[resource_shortdisplayname[domain]]\" },\n            { field: \"DefaultTheme\", title: \"[resource_shortdisplayname[defaulttheme]]\" },\n            { \n                field: \"DefaultCultureId\", \n                title: \"[resource_shortdisplayname[defaultculture]]\", \n                editor: function(container, options) {\n                    $(\"<input name='\" + options.field + \"' />\").appendTo(container).kendoDropDownList({\n                        template: \"#=Name# (#=Id#)\",\n                        dataTextField: \"Id\",\n                        dataValueField: \"Id\",\n                        dataSource: cultureDataSource\n                    }).data(\"kendoDropDownList\").value(options.model.DefaultCultureId);\n                } \n            }\n        ];\n        appGrid.commands.push({name: \"visit\", href: \"https://#=Domain#/\", icon: \"k-i-share\", text: \"[resource_displayname[visit]]\"});\n        appGrid.commands.push({name: \"save\", icon: \"k-i-save\", text: \"[resource_displayname[save]]\"});\n        appGrid.commands.push({name: \"migrate\", icon: \"k-i-copy\", text: \"[resource_displayname[migrate]]\"});\n        appGrid.commands.push({name: \"destroy\", icon: \"k-i-delete\", text: \"[resource_displayname[delete]]\"});\n        appGrid.detailTemplate = \"<div name='CoreManagementExpanded'></div>\";\n        appGrid.detailExpand = (e) => CoreManagement.expand(e, appGrid);\n        appGrid.dataBound = function() {\n            $(\"[name=migrate]\", appGrid.gridElement).on(\"click\", (e) => CoreManagement.migrate(e, appGrid));\n            $(\"[name=destroy]\", appGrid.gridElement).on(\"click\", (e) => CoreManagement.destroy(e, appGrid));\n            $(\"[name=save]\", appGrid.gridElement).on(\"click\", (e) => CoreManagement.save(e, appGrid));\n        };\n        await appGrid.init();\n        $(\"button[name=create]\", appGrid.gridElement).off('click').on(\"click\",  (e) => CoreManagement.create(e, appGrid));\n    },\n\n    expand: function (e, grid) {\n        var container = $(e.detailRow);\n        var app = grid.dataItem(e.masterRow);\n        if ($(container).find(\"div[name=tabs]\").length === 0) {\n            loadComponent($(\"[name='CoreManagementExpanded']\", container), \"AppManagement\", (c) => {\n                AppManagement.toolbar = false;\n                AppManagement.init(app, $(\".component[name=AppManagement]\", $(\"[name='CoreManagementExpanded']\", container)));\n                $(e.detailRow).closest(\"tr\").prev().find(\"[name=save]\").off(\"click\").on(\"click\", function (e) {\n                    var configEditor = $(\"[name=config] > .editor\", $(\".component[name=AppManagement]\", $(\"[name='CoreManagementExpanded']\", container))).data(\"configEditor\");\n                    app.ConfigJson = configEditor.getValue();\n                    CoreManagement.save(e, grid, app);\n                });\n            });\n        }\n    },\n\n    create: async function (e, grid) {\n        e.preventDefault();\n        var newApp = kendo.observable({  Id: 0, Name: \"\", Domain: \"\", DefaultCultureId: \"\", DefaultTheme: \"Default\" });\n        var newAppDialog = new Dialog({title: \"[resource_displayname[newapp]]\"});\n        newAppDialog.template = $(\"[name=newApp]\").first().html();\n        newAppDialog.events.create = async function() {\n            await api.add(\"ContentManagement/App\", newApp.toJSON()).then(() => {\n                newAppDialog.events.close();\n                notification.success('[resource_displayname[AppCreated]]'.replace(\"[name]\", newApp.Name));\n                grid.refresh();\n            }).catch((err) => error(err));\n        };\n        newAppDialog.init(() => {\n            kendo.bind($(newAppDialog.element), newApp);\n        });\n    },\n\n    destroy: function (e, grid) {\n        var d = new ConfirmDialog({\n            question: \"[resource_displayname[AreYouSure]]\",\n            title: \"[resource_displayname[AreYouSure]]\",\n            confirm: \"[resource_displayname[Confirm]]\",\n            close: \"[resource_displayname[Cancel]]\"\n        });\n\n        d.events.confirm = async function () {\n            var appRow = grid.dataItem($(e.currentTarget).closest(\"tr\"));\n            await appRow.destroy(e);\n            notification.success(\"[resource_description[AppDeleted]]\");\n            grid.refresh();\n            d.events.close();\n        };\n        d.init();\n    },\n\n    save: async function (e, grid, app) {\n        e.preventDefault();\n        // perform the save\n        if(!app) {\n            app = grid.dataItem($(e.currentTarget).closest(\"tr\"));\n        }\n\n        await app.save(e);\n        notification.success(\"[resource_displayname[AppSaved]]\");\n        grid.refresh();\n    },\n\n    migrate: function (e, grid) {\n        var app = grid.dataItem($(e.currentTarget).closest(\"tr\"));\n        var migrateDialog = new Dialog({title: \"[resource_displayname[MigrateApp]]\", width: 620, height: \"auto\" });\n        migrateDialog.init();\n        loadComponent(migrateDialog.element, \"AppMigrator\", function (c) {\n            AppMigrator.init(session.app, migrateDialog.element, app, migrateDialog);\n        });\n    }\n};",
  "Content": "<script type=\"text/template\" name=\"newApp\">\n\t<ul class=\"fieldList\">\n\t<li>\n     \t   <label>[resource_displayname[name]]</label>\n        <div class=\"value\">\n        \t<input type=\"text\" name=\"name\" data-bind=\"value: Name\" />\n        </div>\n     </li>\n\t <li>\n     \t   <label>[resource_displayname[domain]]</label>\n        <div class=\"value\">\n        \t<input type=\"text\" name=\"domain\" data-bind=\"value: Domain\" />\n        </div>\n     </li>\n      <li>\n     \t   <label>[resource_displayname[defaultculture]]</label>\n\t\t\t<div class=\"value\">\n\t\t\t\t<input type=\"text\" name=\"defaultCulture\" data-bind=\"value: DefaultCultureId\" />\n\t\t\t</div>\n     </li>\n\t <li>\n\t\t <label>[resource_displayname[tenant]]</label>\n\t\t <div class=\"value\">\n\t\t\t <input type=\"text\" name=\"tenantId\" data-bind=\"value: TenantId\" />\n\t\t</div>\n\t</li>\n</ul>\n<hr>\n<div class=\"value\">\n   <button name=\"create\">[resource_displayname[create]]</button>\n</div>\n</script>",
  "LastUpdated": "2024-10-17T10:37:00.7544378+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "DebtorPortal",
  "Key": "Tools",
  "ResourceKey": "CMS",
  "Script": "DebtorPortal = {\r\n    init: async function(app, container) {\r\n        app = app || session.app;\r\n        container = container || $(\".component[name=DebtorPortal]\");\r\n\r\n        var buildInfo = {\r\n            \"SourceAppId\": 0,\r\n            \"App\": {\r\n                \"Name\": \"\",\r\n                \"DefaultTheme\": \"Default\",\r\n                \"DefaultCultureId\": \"\",\r\n                \"Config\": {\r\n                    \"B2B\": {\r\n                        \"SourceSystem\": \"\",\r\n                        \"TransactionSource\": \"\"\r\n                    },\r\n                    \"Components\": {\r\n                        \"Grids\": {\r\n                            \"Details\": \"Expand,Link\"\r\n                        }\r\n                    },\r\n                    \"Calendars\": {\r\n                        \"primary\": 1\r\n                    }\r\n                }\r\n            }\r\n        }\r\n        var data = await api.get(\"ContentManagement/App\");\r\n        $(\"[name=templateapp]\", container).kendoDropDownList({\r\n            optionLabel: \"[resource_displayname[Template]]\",\r\n            dataTextField: \"Name\",\r\n            dataValueField: \"Id\",\r\n            dataSource: data.value\r\n        }).data(\"kendoDropDownList\");\r\n        buildInfo = new kendo.observable(buildInfo);\r\n        kendo.bind($(\"[name=DebtorBuilder]\", container), buildInfo);\r\n        $(\"[name=builder]\", container).data(\"model\", model);\r\n        $(\"button[name=submit]\", container).on(\"click\", function (e) {\r\n            DebtorPortal.submit(app, buildInfo, container);\r\n        });\r\n        ThemeBuilder.init(app, $(\".component[name=ThemeBuilder]\", container));\r\n    },\r\n\r\n    submit(theApp, buildInfo, container) {\r\n        var app = buildInfo.App;\r\n        var domain = window.location.hostname;\r\n        app.Domain = app.Name + \".\" + domain;\r\n        app.Config.Themes = {\r\n            \"Default\": ThemeBuilder.build($(\"[name=ThemeBuilder]\", container))\r\n        };\r\n        var debtorPortalFunctions = {\r\n            [dms[Scripts/AppCreation/DebtorPortal.js]]\r\n        };\r\n\r\n        var d = new Dialog({\r\n            width: 800,\r\n            height: 500\r\n        });\r\n        d.init(() => {\r\n            loadComponent($(d.element), \"ScriptRunner\", (component) => {\r\n                component.init(theApp, d.element, buildInfo, debtorPortalFunctions, d);\r\n            });\r\n        });\r\n\r\n\r\n        //execute(buildInfo, debtorPortalFunctions);\r\n    }\r\n}\r\n",
  "Content": "<div name=\"DebtorBuilder\">\r\n\t<div class=\"container\">\r\n\t\t<h3>[resource_displayname[AppDetails]]</h3>\r\n\t\t<ul class=\"fieldList\">\r\n\t\t\t<li>\r\n\t\t\t\t<label>[resource_displayname[Name]]</label>\r\n\t\t\t\t<div class=\"value\">\r\n\t\t\t\t\t<input type=\"text\" data-bind=\"value: App.Name\" />\r\n                </div>\r\n\t\t\t</li>\r\n\t\t\t<li>\r\n\t\t\t\t<label>[resource_displayname[templateapp]]</label>\r\n\t\t\t\t<div class=\"value\">\r\n\t\t\t\t\t<input type=\"text\" data-bind= \"value: SourceAppId\" name=\"templateapp\">\r\n                </div>\r\n\t\t\t</li>\r\n\t\t\t<li>\r\n\t\t\t\t<label>[resource_displayname[defaultTheme]]</label>\r\n\t\t\t\t<div class=\"value\">\r\n\t\t\t\t\t<input type=\"text\" data-bind= \"value: App.DefaultTheme\">\r\n                </div>\r\n\t\t\t</li>\r\n\t\t\t<li>\r\n\t\t\t\t<label>[resource_displayname[defaultCultureId]]</label>\r\n\t\t\t\t<div class=\"value\">\r\n\t\t\t\t\t<input type=\"text\" data-bind= \"value: App.DefaultCultureId\">\r\n                </div>\r\n\t\t\t</li>\r\n\t\t</ul>\r\n\r\n\t\t[component[ThemeBuilder]]\r\n\r\n\t\t<div class=\"container\">\r\n\t\t\t<h3>[resource_displayname[B2BDetails]]</h3>\r\n\t\t\t<ul class=\"fieldList\">\r\n\t\t\t\t<li>\r\n\t\t\t\t\t<label>[resource_displayname[SourceSystem]]</label>\r\n\t\t\t\t\t<div class=\"value\">\r\n\t\t\t\t\t\t<input type=\"text\" data-bind=\"value: App.Config.B2B.SourceSystem\" />\r\n                </div>\r\n\t\t\t\t</li>\r\n\t\t\t\t<li>\r\n\t\t\t\t\t<label>[resource_displayname[TransactionSource]]</label>\r\n\t\t\t\t\t<div class=\"value\">\r\n\t\t\t\t\t\t<input type=\"text\" data-bind=\"value: App.Config.B2B.TransactionSource\" />\r\n                </div>\r\n\t\t\t\t</li>\r\n\t\t\t</ul>\r\n\t\t</div>\r\n\t\t<hr>\r\n\t\t<div class=\"value\">\r\n\t\t\t<button name=\"submit\">[resource_displayname[Submit]]</button>\r\n\t\t</div>\r\n\t</div>\r\n</div>\r\n\r\n<div style='visibility:hidden' name='scriptRunner'></div>\r\n\r\n<style type=\"text/css\">\r\n\t.component[name=ThemeBuilder] {\r\n\t\tmargin: 0px;\r\n\t}\r\n</style>",
  "LastUpdated": "2021-11-19T21:26:28.002612+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "Documentation",
  "Key": "Documentation",
  "ResourceKey": "Documentation",
  "Script": "Documentation = {\r\n    init: async function(app, container) {\r\n        app = app || session.app;\r\n        container = container || $(\".component[name=Documentation]\");\r\n        $(\"[name=tabs]\", container).kendoTabStrip({ animation: { open: { effects: \"fadeIn\" } } });\r\n\r\n        var businessDocs = await loadComponent($(\".tab[name=businessDocs]\", container), \"FolderManagement\");\r\n        businessDocs.init(app, $(\".component[name=FolderManagement]\", $(\".tab[name=businessDocs]\", container)), { Id: '6A23BDC8-2CA7-4B91-A62F-08D7725C8272', Path: 'documentation/business docs', Name: 'Business Requirements' }, true);\r\n\r\n        var technicalDocs = await loadComponent($(\".tab[name=technicalDocs]\", container), \"FolderManagement\");\r\n        technicalDocs.init(app, $(\".component[name=FolderManagement]\", $(\".tab[name=technicalDocs]\", container)), { Id: '487DB4A6-D50B-49F6-A630-08D7725C8272', Path: 'documentation/technical docs', Name: 'Technical Requirements' }, true);\r\n    }\r\n};",
  "Content": "<h3>[resource_displayname[documentationlibrary]]</h3>\r\n<div name=\"tabs\" class=\"tabs\">\r\n\t<ul>\r\n\t\t<li class=\"k-state-active\" name=\"businessDocs\">[resource_displayname[businessDocs]]</li>\r\n\t\t<li name=\"technicalDocs\">[resource_displayname[technicalDocs]]</li>\r\n\t</ul>\r\n\t<div class=\"tab\" name=\"businessDocs\"></div>\r\n\t<div class=\"tab\" name=\"technicalDocs\"></div>\r\n</div>",
  "LastUpdated": "2023-03-16T12:41:30.6771141+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "Etc",
  "Key": "Theming",
  "ResourceKey": "CMS",
  "Script": "Etc = {\n    init: async function (app, container, observable) {\n        app = app || session.app;\n        container = container || $(\".component[name=Etc]\");\n\n        if(!observable)\n            return;\n        \n        kendo.bind(container, observable);\n\n        $(\"[name=cultureFlagLayout]\", container).kendoDropDownList({\n            dataSource: [\n                {key: \"text\", text: \"[resource_displayname[text]]\"},\n                {key: \"picture\", text: \"[resource_displayname[picture]]\"}\n            ],\n            dataTextField: \"text\",\n            dataValueField: \"key\",\n        });\n    }\n}",
  "Content": "<div class=\"row\">\n    <div class=\"col-md-6\">\n        <div class=\"input-group input-group-sm mb-1\">\n            <span class=\"input-group-text\">[resource_displayname[paintLoginMid]]</span>\n            <div class=\"input-group-text\">\n                <input type=\"checkbox\" class=\"form-check-input\" data-bind=\"value: colours.paintLoginMid\" />\n            </div>\n        </div>\n\n        <div class=\"input-group input-group-sm mb-1\">\n            <span class=\"input-group-text\">[resource_displayname[paintLoginBottom]]</span>\n            <div class=\"input-group-text\">\n                <input type=\"checkbox\" class=\"form-check-input\" data-bind=\"value: colours.paintLoginBottom\" />\n            </div>\n        </div>\n\n        <div class=\"input-group input-group-sm mb-1\">\n            <span class=\"input-group-text\">[resource_displayname[font]]</span>\n            <input type=\"text\" class=\"form-control\" name=\"cultureFlagLayout\" data-bind=\"value: cultureFlagLayout\" />\n        </div>\n    </div>\n</div>",
  "LastUpdated": "2024-11-19T18:18:31.1258524+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "Font",
  "Key": "Theming",
  "ResourceKey": "CMS",
  "Script": "Font = {\n    init: function (app, container, observable) {\n        app = app || session.app;\n        container = container || $(\".component[name=Font]\");\n        if (!observable)\n            return;\n        \n        $(\"[name=font-size-slider]\", container).kendoSlider({ \n            min: 6,\n            max: 72,\n            smallStep: 2,\n            largeStep: 8,\n            value: parseFloat((observable.get(\"font.size\") || \"4px\").replaceAll(\"px\", \"\")),\n            change: function(e) {\n                e.preventDefault();\n                observable.set(\"font.size\", this.value() + \"px\");\n                $('[name=font-preview]', container).css('font-size', this.value());\n            }\n        });\n\n        queryLocalFonts()\n            .then((fonts) => {\n                if(fonts.length == 0)\n                    return;\n                \n                var fontFamilies = [... new Set(fonts.map(f => f.family))];\n                \n                $(\"[name=font-dropdown]\", container).kendoDropDownList({\n                    dataSource: fontFamilies,\n                    change: function(e) {\n                        observable.set('font.family', this.value());\n                        $('[name=font-preview]', container).css('font-family', this.value());\n                    }\n                }).data(\"kendoDropDownList\");\n            });\n    }\n}",
  "Content": "<div class=\"row\">\n    <div class=\"col-md-6\">\n        <h4>\n            [resource_displayname[font]]\n        </h4>\n        <div class=\"input-group input-group-sm mb-1\">\n            <span class=\"input-group-text\">[resource_displayname[fontsize]]</span>\n            <input type=\"text\" class=\"form-control\" name=\"font-size-slider\" />\n        </div>\n\t\t\n        <div class=\"input-group input-group-sm mb-1\">\n            <span class=\"input-group-text\">[resource_displayname[font]]</span>\n            <input type=\"text\" class=\"form-control\" name=\"font-dropdown\" data-bind=\"value: font.family\" />\n        </div>\n    </div>\n    <div class=\"col-md-6\">\n        <h4>\n            [resource_displayname[preview]]\n        </h4>\n        <div name=\"font-preview\">\n            [resource_displayname[sampletext]].\n        </div>\n    </div>\n</div>",
  "LastUpdated": "2024-11-19T18:18:31.0800836+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "FormManagement",
  "Key": "Tools",
  "ResourceKey": "Forms",
  "Script": "FormManagement = {\n    init: function (app) {\n\t\tif(!app) { app = session.app; }\n        var root = $(\".component[name=Forms]\");\n        var config = { \n\t\t\tendpoint: \"ContentManagement/Form\", \n\t\t\tfilter: {\n\t\t\t\tlogic: \"and\",\n\t\t\t\tfilters: [\n\t\t\t\t\t{ field: \"AppId\", operator: \"eq\", value: app.Id }\n\t\t\t\t]\n\t\t\t} \n\t\t};\n\n\t\tmodel.getDatasource(config, function (ds) {\n\t\t\tvar grid = $(\"[name=grid]\", root).kendoGrid({\n\t\t\t\tdataSource: ds,\n\t\t\t\teditable: true,\n\t\t\t\tsortable: true,\n\t\t\t\tfilterable: true,\n\t\t\t\tpageable: {\n\t\t\t\t\trefresh: true,\n\t\t\t\t\tpageSizes: true,\n\t\t\t\t\tbuttonCount: 5\n\t\t\t\t},\n\t\t\t\ttoolbar: kendo.template(\"<div><button name='create'><span class='k-icon k-i-plus'></span>New</button></div>\"),\n\t\t\t\tcolumns: [\n\t\t\t\t\t\"Name\",\n\t\t\t\t\t\"RootMetaItem\",\n\t\t\t\t\t{ \n                       \twidth: 310, template: \"<a name='preview' href=''><span class='k-icon k-i-preview'></span>Preview</a>\" +\n                              \"<a name='edit' href=''><span class='k-icon k-i-edit'></span>Edit</a>\" +\n                              \"<a name='save' href=''><span class='k-icon k-i-save'></span>Save</a>\" +\n                              \"<a name='destroy' href=''><span class='k-icon k-i-delete'></span>Delete</a>\"\n                    }\n\t\t\t\t],\n\t\t\t\tdetailTemplate: kendo.template($(\"[name=details]\", root).html()),\n\t\t\t\tdetailExpand: function(e) {\n\t\t\t\t\tif($(e.detailRow).find(\".CodeMirror\").length === 0) {\n\t\t\t\t\t\tvar theForm = this.dataItem(e.masterRow);\n\t\t\t\t\t\tvar fsTemplateEditor = $(e.detailRow).find(\"textarea[name=fieldsetTemplate]\");\n\t\t\t\t\t\tvar fs = CodeMirror.fromTextArea(fsTemplateEditor[0],  getEditorConfig(\"htmlmixed\"));\n\t\t\t\t\t\tfs.on(\"change\", function() { \n\t\t\t\t\t\t\ttheForm.FieldsetTemplate = fs.getValue(); \n\t\t\t\t\t\t});\n                       \n                       var fTemplateEditor = $(e.detailRow).find(\"textarea[name=fieldTemplate]\");\n\t\t\t\t\t\tvar f = CodeMirror.fromTextArea(fTemplateEditor[0],  getEditorConfig(\"htmlmixed\"));\n\t\t\t\t\t\tf.on(\"change\", function() { \n\t\t\t\t\t\t\ttheForm.FieldTemplate = f.getValue(); \n\t\t\t\t\t\t});\n\t\t\t\t\t}\n\t\t\t\t},\n\t\t\t\tdataBound: function() {\n                    $(\"a[name=preview]\", grid).on(\"click\", FormManagement.preview);\n                    $(\"a[name=edit]\", grid).on(\"click\", FormManagement.edit);\n\t\t\t\t    $(\"a[name=save]\", grid).on(\"click\", FormManagement.save);\n\t\t\t\t\t$(\"a[name=destroy]\", grid).on(\"click\", FormManagement.destroy);\n\t\t\t\t}\n\t\t\t});\n\t\t  \n\t\t\t$(\"button[name=create]\", grid).on(\"click\", function(e) { FormManagement.newForm(e, app); });\n\t\t});\n    },\n   \n   preview: function(e) {\n      e.preventDefault();\n   \t  var root = $(\".component[name=Forms]\");\n      var grid = $(\"[name=grid]\", root).data(\"kendoGrid\");\n      var theForm = grid.dataItem($(e.currentTarget).closest(\"tr\"));\n      $(document.body).append($(\"<div name='formPreview' style='margin: 10px;'></div>\"));\n      var dialog = $(\"[name=formPreview]\");\n  \t  var d = dialog.kendoWindow({\n                visible: false,\n                modal: true,\n                resizable: false,\n                title: theForm.Name,\n                deactivate: function (e) {\n                    this.destroy();\n                },\n       }).data(\"kendoWindow\");\n       api.call(\"ContentManagement/Form/Render()?theme=\" + session.theme + \"&culture=\" + session.culture, theForm, function (result) {\n            $(dialog).append(result.value);\n            d.center();\n\t\t\td.open();\n       });\n   },\n   \n    newForm: function(e, app) {\n      var root = $(\".component[name=Forms]\");\n      $(document.body).append($(\"<div name='formCreator'>\" + $(\"[name=new]\", root).html() + \"</div>\"));\n      var dialog = $(\"[name=formCreator]\");\n  \t  var d = dialog.kendoWindow({\n                visible: false,\n                modal: true,\n                resizable: false,\n                width: 510,\n                title: \"New Form\",\n                deactivate: function (e) {\n                    this.destroy();\n                },\n           \t}).data(\"kendoWindow\");\n\t\t\td.center();\n\t\t\td.open();\n      \t\t$(\"button[name=create]\", dialog).on(\"click\", function() {\n              $(\"button[name=create]\", dialog).off(\"click\");\n               api.get(\"ContentManagement/Form/NewForm()\", function(newForm) {\n                  \tnewForm.AppId = app.Id;\n                  \t newForm.Name = $(\"input[name=name]\", dialog).val();\n                     api.add(\"ContentManagement/Form\", newForm, function(r) {\n                           d.close();\n                           notification.success('Form \"' + newForm.Name + '\" created.');\n                           var grid = $(\".component[name=Forms] [name=grid]\").data(\"kendoGrid\");\n                           grid.dataSource.read();\n                           grid.refresh();\n                      });\n               });\n          });\n    },\n   \n   edit: function(e) {\n   \t\te.preventDefault();\n        var root = $(\".component[name=Forms]\");\n\t\tvar grid = $(\"[name=grid]\", root).data(\"kendoGrid\");\n        var theForm = grid.dataItem($(e.currentTarget).closest(\"tr\"));\n        $(document.body).append($(\"<div name='designer'></div>\"));\n        var dialog = $(\"[name=designer]\");\n  \t    var d = dialog.kendoWindow({\n                visible: false,\n                modal: true,\n                resizable: false,\n           \t\twidth: '95%',\n           \t\theight: '85%',\n                title: \"Form Designer\",\n                deactivate: function (e) {\n                    this.destroy();\n                },\n          }).data(\"kendoWindow\");\n      \t  loadComponent(dialog, \"Form Builder\", function(c) {\n          \t    FormBuilder.init(theForm);\n             \td.center();\n\t\t\t\td.open();\n          });\n   },\n\t\n    save: function (e) {\n        e.preventDefault();\n\t\tvar root = $(\".component[name=Forms]\");\n\t\tvar grid = $(\"[name=grid]\", root).data(\"kendoGrid\");\n        var theForm = grid.dataItem($(e.currentTarget).closest(\"tr\"));\n        theForm.save(e, function () {\n            notification.success(\"[resource_description[FormSaved]]\");\n        });\n    },\n\n    destroy: function (e) {\n        e.preventDefault();\n\t\tvar root = $(\".component[name=Forms]\");\n\t\tvar grid = $(\"[name=grid]\", root).data(\"kendoGrid\");\n        var theForm = grid.dataItem($(e.currentTarget).closest(\"tr\"));\n        theForm.destroy(e, function () {\n            notification.success(\"[resource_description[FormDeleted]]\");\n            var grid = $(\".component[name=Forms] [name=grid]\").data(\"kendoGrid\");\n            grid.dataSource.read();\n            grid.refresh();\n        });\n    }\n};\n\nfunction getEditorConfig(mode) { \n\treturn { \n       //lineNumbers: true,  /// these don't appear to be to working ... TODO: figure out why\n       tabsize: 3, \n       indentUnit: 3, \n       mode: mode \n    };\n}\n\n",
  "Content": "<script src=\"https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.38.0/codemirror.min.js\"></script>\n<script src=\"https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.38.0/mode/javascript/javascript.min.js\"></script>\n<script src=\"https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.38.0/mode/xml/xml.min.js\"></script>\n<script src=\"https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.38.0/mode/css/css.min.js\"></script>\n<script src=\"https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.38.0/mode/htmlmixed/htmlmixed.min.js\"></script>\n<link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.38.0/codemirror.min.css\" />\n\n<div name=\"grid\"></div>\n<script type=\"template\" name=\"details\">\n<div name=\"componentEditor\">\n\t <div class=\"editorContainer\">\n\t\t<h4>Fieldset Template</h4>\n\t\t<textarea name=\"fieldsetTemplate\">#:FieldsetTemplate#</textarea>\n\t </div>\n\t <div class=\"editorContainer\">\n\t\t<h4>Field Template</h4>\n\t\t<textarea name=\"fieldTemplate\">#:FieldTemplate#</textarea>\n\t </div>\n</div>\n</script>\n<script type=\"template\" name=\"new\">\n<ul class=\"fieldList\">\n\t<li>\n     \t<label>[resource_displayname[Name]]</label>\n        <div class=\"value\">\n        \t<input type=\"text\" name=\"name\"></input>\n        </div>\n     </li>\n     <li>\n     \t<label></label>\n        <div class=\"value\">\n        \t<button name=\"create\">Create</input>\n        </div>\n     </li>\n</ul>\n</script>\n<style scoped>\n .CodeMirror \t\t\t\t\t{ height: 300px; }\n .editorContainer \t\t\t\t{ display: inline-block; width: 49%; height: 500px; margin-right: 10px; }\n .editorContainer > textarea\t{ width: 98.5%; height: 100%; }\n [name=grid] \t\t\t\t\t{ margin: 10px; }\n   [name=designer] { height: 100%; }\n</style>\n",
  "LastUpdated": "2021-11-19T21:26:49.4769264+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "GridBuilder",
  "Key": "Tools",
  "ResourceKey": "Tools",
  "Script": "GridBuilder = {\n\tinit:function(app, container) {\n      app = app || session.app;\n      container = container || $('.component[name=GridBuilder]');\n      $('[name=tabstrip]', container).kendoTabStrip({\n\t\tactivate:function(e){\n\t\t\tif($(e.item).attr('name') === 'preview') {\n\t\t\t\tGridBuilder.showgridView(container);\n\t\t\t}\n\t\t}\n\t});\n    \n\t\tGridBuilder.filterable = true;\n\t\tGridBuilder.GridName = '';\n\t\t\n\t\tGridBuilder.root = $('[name=\"GBWizard\"].component');\n\t\t// go get a list of endpoints from api\n\t\tapi.get('', function(endpoints){\n\t\t\tvar endpointDD = $('[name=endpoints]', GridBuilder.root);\n\t\t\t\n\t\t\t// create a drop down with a change event\n\t\t\tendpointDD.kendoDropDownList({\n\t\t\t\tdataSource: endpoints.value,\n\t\t\t\tdataTextField:'name',\n\t\t\t\tdataValueField: 'url',\n\t\t\t\tchange: GridBuilder.endpointChange\n\t\t\t});\n\t\t\t\n\t\t\t// select first and trigger change event\n\t\t\tendpointDD.data('kendoDropDownList').select(0);\n\t\t\tendpointDD.data('kendoDropDownList').trigger('change');\n\t\t\t\n\t\t\t// set filterable flag\n\t\t\t$('[name=filterable]', GridBuilder.root).off('click').on('click', function(){\n\t\t\t\tGridBuilder.filterable = this.checked;\n\t\t\t});\n\n\t\t\t// set gridname\n\t\t\t$('[name=gridname]', GridBuilder.root).off('keyup').on('keyup', function(){\n\t\t\t\tGridBuilder.GridName = this.value;\n\t\t\t});\n\t\t})\n\t},\n\t\n\tsetupFilters:function(){\t\t\n\t\tvar root = $('[name=tbfilters]');\n\t\t\n\t\tGridBuilder.checkOld($('[name=grid]', root));\n\t\t\n\t\t$('[name=grid]', root).kendoGrid({\n\t\t\ttoolbar:'<button class=\"btn\" name=\"add\">Add New Filter</button>',\n\t\t\tdataSource:[],\n\t\t\teditable:true,\n\t\t\tcolumns:[\n\t\t\t\t{ field: 'field' , editor: GridBuilder.fieldEditor },\n\t\t\t\t{ field: 'operator', editor: GridBuilder.operatorEditor },\n\t\t\t\t{ field: 'value', editor: GridBuilder.valueEditor },\n\t\t\t\t{ template: '<span class=\"k-icon k-i-close-circle\" name=\"delete\"></span>', width:25 }\n\t\t\t],\n\t\t\tdataBound:function(e){\n\t\t\t\tvar grid = e.sender.element;\n\t\t\t\t$('[name=add]', grid).off('click').on('click', {grid: grid}, GridBuilder.addFilter);\n\t\t\t\t$('[name=delete]', grid).off('click').on('click', {grid: grid}, GridBuilder.removeFilter);\n\t\t\t}\n\t\t});\n\t\t\n\t\tGridBuilder.setUpactions(root);\t\t\n\t},\n\t\n   \tsetUpactions: function(root){\n\t\tGridBuilder.checkOld($('[name=actionGrid]', root));\n\t\t\n\t\tvar actions = [\n\t\t\t{ \n\t\t\t\tname: 'dataBound', \n\t\t\t\tcode: `var grid = e.sender.element; \n$('.clx-loader', grid.parent()).hide(); \n$('.k-grid-content', grid).css('max-height', (window.innerHeight * 0.7) + 'px');\n\n$('[name=export]', grid).off('click').on('click', function(ev){\n\tkendo.ui.progress(grid, true);\n\t\n\tvar columns = e.sender.columns;\n\tvar url = unescape(e.sender.dataSource.lastGet.url);\n\turl = url.replace(/\\\\&\\\\$top=\\d+/g, '').replace(/\\\\?\\\\$top=\\d+/g, '');\n\texportData.byQuery(url).toExcel(columns,\"[GRIDNAME]\", function(){ kendo.ui.progress(grid, false); });\n});` \n\t\t\t}\t\t\n\t\t];\n\t\t\n\t\t$('[name=actionGrid]', root).kendoGrid({\n\t\t\ttoolbar:'<button class=\"btn\" name=\"add\">Add New Action</button>',\n\t\t\tdataSource:actions,\n\t\t\teditable:true,\n\t\t\tcolumns:[\n\t\t\t\t{ field: 'name' }\n\t\t\t],\n\t\t\tdetailTemplate: kendo.template('function(e){<br/><textarea data-bind=\"value: code\">#:code#</textarea><br/>}'),\n\t\t\tdetailExpand:function(e){\n\t\t\t\tvar di = this.dataItem(e.masterRow);\n\t\t\t\tif(e.detailRow.find('.CodeMirror').length === 0){\n\t\t\t\t\te.detailRow.css('background', '#cbcbcb');\n\n\t\t\t\t\tvar f = CodeMirror.fromTextArea($('textarea', e.detailCell)[0],  { tabsize: 3, indentUnit: 3, mode: 'javascript' });\n\t\t\t\t\tf.on(\"change\", function() { \n\t\t\t\t\t\tdi.code= f.getValue(); \n\t\t\t\t\t});\n\t\t\t\t}\n\t\t\t},\n\t\t\tdataBound:function(e){\n\t\t\t\t$('[name=add]', e.sender.element).off('click').on('click', function(){\n\t\t\t\t\te.sender.dataSource.add({\n\t\t\t\t\t\tname:'new',\n\t\t\t\t\t\tcode:''\n\t\t\t\t\t});\n\t\t\t\t})\n\t\t\t}\n\t\t});\t\n\t},\n\t\t\t\n\taddFilter:function(e){\n\t\te.data.grid.data('kendoGrid').dataSource.add({\n\t\t\tfield:GridBuilder.fields[0].name,\n\t\t\toperator:'eq',\n\t\t\tvalue:'',\n\t\t\ttype: GridBuilder.fields[0].type\n\t\t});\t\n\t\t\n\t\tGridBuilder.updateFilters();\n\t},\n\t\n\tfieldEditor:function(container, options){\n\t\t$('<input name=\"' + options.field + '\" data-bind=\"value:' + options.field + '\"/>')\n\t\t\t.appendTo(container)\n\t\t\t.kendoDropDownList({\n\t\t\t\tdataSource: GridBuilder.fields,\n\t\t\t\tdataTextField:'name',\n\t\t\t\tdataValueField:'name',\n\t\t\t\tchange:function(e){\n\t\t\t\t\toptions.model.type = this.dataItem().type;\n\t\t\t\t\tGridBuilder.updateFilters();\n\t\t\t\t}\n\t\t\t});\n\t},\n\t\n\tvalueEditor:function(container, options){\n\t\tvar input = $('<input name=\"' + options.field + '\" data-bind=\"value:' + options.field + '\"/>').appendTo(container);\n\t\t\n\t\tif(options.model.type.toLowerCase().indexOf('date') > -1){\n\t\t\tinput.kendoDateTimePicker();\n\t\t}\n\t},\n\t\n\toperatorEditor:function(container, options){\n\t\t$('<input name=\"' + options.field + '\" data-bind=\"value:' + options.field + '\"/>')\n\t\t\t.appendTo(container)\n\t\t\t.kendoDropDownList({\n\t\t\t\tdataSource:[ \n\t\t\t\t\t{ id:'eq', name: 'Equals' }, \n\t\t\t\t\t{ id: 'ne', name: 'Not'}, \n\t\t\t\t\t{ id: 'gt', name:'Greater than' }, \n\t\t\t\t\t{ id: 'lt', name:'Less than' }, \n\t\t\t\t\t{ id: 'gte', name:'Greater than or equal to' }, \n\t\t\t\t\t{ id: 'lte', name:'Less than or equal to' }, \n\t\t\t\t\t{ id: 'contains', name:'Contains' }, \n\t\t\t\t\t{ id: 'startswith', name:'Starts with' }\n\t\t\t\t],\n\t\t\t\tdataTextField:'name',\n\t\t\t\tdataValueField:'id'\n\t\t\t});\n\t},\n\t\n\tremoveFilter:function(e){\n\t\tvar row = $(e.target).closest('tr');\n\t\tvar di = e.data.grid.data('kendoGrid').dataItem(row);\n\t\t\n\t\te.data.grid.data('kendoGrid').dataSource.remove(di);\n\t\tGridBuilder.updateFilters();\n\t},\n\t\n\tshowgridView:function(e){\n\t\t// get the query and load in content for displaying grids\n\t\tvar query = GridBuilder.generateQuery();\n\n\t\tvar viewRoot = $('[name=tbpreview]');\n\t\t\n\t\tGridBuilder.checkOld($('[name=grid]', viewRoot));\n\n\t\t$('[name=save]', viewRoot).off('click').on('click', {root: viewRoot},GridBuilder.saveGrid);\n\t\t\n\t\t// build final grid\n\t\tGridBuilder.buildGrid(viewRoot, query);\t\n\t},\n\t\n\t// save the grid out as a component\n\tsaveGrid:function(e){\n\t\tif(GridBuilder.GridName){\n\t\t\tapi.add('ContentManagement/Component', {\n\t\t\t\tId:0,\n\t\t\t\tAppId: session.app.Id,\n\t\t\t\tName: GridBuilder.GridName.replace(/ /g, ''),\n\t\t\t\tResourceKey: 'Component/' + GridBuilder.GridName.replace(/ /g, ''),\n\t\t\t\tContent:'',\n\t\t\t\tScript: GridBuilder.config,\n\t\t\t\tCategory:'Grid'\n\t\t\t}, function(res){\n\t\t\t\tnotification.success('Grid Component successfully added');\n\t\t\t});\n\t\t}\t\n\t\telse{\n\t\t\tnotification.error('Grid must be given a name in order to save');\n\t\t}\n\t},\n\t\n\tbuildGrid:function(root, query){\n\t\twindow[GridBuilder.GridName] = {};\t\t\n\t\t\n\t\tif(GridBuilder.GridName){\n\t\t\t$('[name=fullQuery]', root).text(query.full);\n\n\t\t\tvar toolbar = $('[name=toolbar]', root.parent()).val() || null; // if we have defined a toolbar we want to use it\n\t\t\tvar actions = $('[name=actionGrid]', root.parent()).data('kendoGrid').dataSource.data();\n\t\t\t\n\t\t\tactions.map(function(a){\n\t\t\t\twindow[GridBuilder.GridName][a.name] = new Function(\"e\", a.code.replace('[GRIDNAME]', GridBuilder.GridName.replace(/ /g, '')));\n\t\t\t});\n\n\t\t\t// define the config for model.dataSource\n\t\t\tvar config = {\n\t\t\t\tendpoint: query.base,\n\t\t\t\todataAppend: query.append,\n\t\t\t\tfilter: { logic:'and', filters: GridBuilder.filters },\n\t\t\t\tgroup: GridBuilder.selects.filter(GridBuilder.isGrouped).sort(function(a,b){ return a.order - b.order }).map(function(d){ return { field: d.name } }),\n\t\t\t\tsort: GridBuilder.selects.filter(GridBuilder.isSorted).sort(function(a,b){ return a.order - b.order }).map(function(d){ return { field: d.name , dir: 'desc'} }),\n\t\t\t};\n\n\t\t\t// build the grid and see that it works\n\t\t\tmodel.getDatasource(config, function(ds){\n\t\t\t\troot.prepend('<div class=\"clx-loader\"></div>');\n\n\t\t\t\t// config for grid\n\t\t\t\t$('[name=grid]', root).kendoGrid({\n\t\t\t\t\ttoolbar: '<button name=\"export\"><span class=\"k-icon k-i-excel\"></span>  [resource_displayname[ExportToExcel]]</button>' + (toolbar || (GridBuilder.GridName ? '<h3>'+GridBuilder.GridName+'</h3>' : null)), // if we define a toolbar use, else try use grid name, else null\n\t\t\t\t\tdataSource:ds,\n\t\t\t\t\tcolumns: GridBuilder.columnfy(GridBuilder.selects), // turn our selects into columns\n\t\t\t\t\tpageable: true,\n\t\t\t\t\tfilterable: GridBuilder.filterable,\n\t\t\t\t\tsortable:true,\n\t\t\t\t\tdataBound:(window[GridBuilder.GridName] || {}).dataBound || function(e){\n\t\t\t\t\t\t// remove the loading thing\n\t\t\t\t\t\t$('.clx-loader', root).remove();\n\t\t\t\t\t}\n\t\t\t\t});\n\t\t\t});\t\t\n\n\t\t\tGridBuilder.buildConfig(config);\n\t\t}\n\t\telse{\n\t\t\tnotification.error('Please give a name to your grid');\n\t\t}\n\t},\n\t\n\tbuildConfig:function(config){\n\t\tvar name = GridBuilder.GridName.replace(/ /g, '');\n\t\tvar actions = $('[name=actionGrid]').data('kendoGrid').dataSource.data();\n\t\tvar databound = actions.filter(function(a){ return a.name === 'dataBound'})[0].code\n\t\tvar toolbar = $('[name=toolbar]').val() || null; // if we have defined a toolbar we want to use it\n\t\t\n\t\tGridBuilder.config =(\n\t\t\t`${name} = {\n\t\t\t\tinit:function(){\n\t\t\t\t\tvar root = $(\"[name=${name}].component\");\n\t\t\t\t\tvar grid = $(\"<div></div>\").appendTo(root);\n\t\t\t\t\tgrid.prepend(\"<div class='clx-loader'></div>\");\n\n\t\t\t\t\tmodel.getDatasource(${JSON.stringify(config)}, function(ds){\n\t\t\t\t\t\tgrid.kendoGrid({\n\t\t\t\t\t\t\ttoolbar: '${'<button name=\"export\"><span class=\"k-icon k-i-excel\"></span>  [resource_displayname[ExportToExcel]]</button>' +  (toolbar || (GridBuilder.GridName ? '<h3>'+GridBuilder.GridName+'</h3>' : null))}',\n\t\t\t\t\t\t\tdataSource:ds,\n\t\t\t\t\t\t\tcolumns: ${JSON.stringify(GridBuilder.columnfy(GridBuilder.selects, true))},\n\t\t\t\t\t\t\tpageable:true,\n\t\t\t\t\t\t\tsortable:true,\n\t\t\t\t\t\t\tfilterable: ${GridBuilder.filterable},\n\t\t\t\t\t\t\tdataBound:function(e){\n\t\t\t\t\t\t\t\t${databound.replace('[GRIDNAME]', name)}\n\t\t\t\t\t\t\t}\n\t\t\t\t\t\t});\n\t\t\t\t\t\t\n\t\t\t\t\t\troot.kendoTooltip({\n\t\t\t\t\t\t\tfilter:'th',\n\t\t\t\t\t\t\tposition: \"top\",\n\t\t\t\t\t\t\tcontent:function(e){\n\t\t\t\t\t\t\t\tvar target = e.target;\n\t\t\t\t\t\t\t\tif($(target).text()){\n\t\t\t\t\t\t\t\t\te.sender.popup.element.css(\"visibility\", \"visible\");\n\t\t\t\t\t\t\t\t\treturn $(target).text();\n\t\t\t\t\t\t\t\t}\n\t\t\t\t\t\t\t\telse{\t\t\t\t\t\n\t\t\t\t\t\t\t\t\te.sender.popup.element.css(\"visibility\", \"hidden\");\n\t\t\t\t\t\t\t\t}\n\t\t\t\t\t\t\t}\n\t\t\t\t\t\t});\n\t\t\t\t\t});\n\t\t\t\t}\n\t\t\t\t${actions.filter(function(a){ return a.name !== 'dataBound'}).map(function(a){\n\t\t\t\t\treturn `,${a.name}:function(e){\n\t\t\t\t\t\t${a.code.replace('[GRIDNAME]', name)}\n\t\t\t\t\t}`\n\t\t\t\t}).join('')}\n\t\t\t};\n\t\t\t$(function(){ \n\t\t\t\t${name}.init()\n\t\t\t})`\n\t\t);\n\t},\n\t\n\t// turn a meta property into a column\n\tcolumnfy:function(cols, forConfig){\n\t\tcols = cols.sort(function(a,b){ return parseInt(a.order) - parseInt(b.order) });\n\t\t\n\t\tvar columns = cols.map(function(col){ \n\t\t\treturn {\n\t\t\t\tfield: col.name,\n\t\t\t\theaderTemplate: (forConfig && col.title.trim()) ? '<span data-resource=\"'+col.title+'\" >[resource_shortdisplayname['+col.title+']]</span>' : null,\n\t\t\t\ttemplate: col.template,\n\t\t\t\tattributes: col.attributes ? col.attributes : null,\n\t\t\t\ttitle: col.title,\n\t\t\t\twidth: col.width,\n\t\t\t\thidden: col.hidden\n\t\t\t}\n\t \t});\t\t\n\t\t\n\t\treturn columns;\n\t},\n\t\n\tgenerateQuery:function(){\n\t\t// make sure we have latest\n\t\tGridBuilder.updateSelects();\n\t\tGridBuilder.updateExpands();\n\t\tGridBuilder.updateFilters();\n\t\t\n\t\t// identify the sections that will make up final query\n\t\tvar base = GridBuilder.endpoint + '/' + GridBuilder.context;\n\t\tvar selects = GridBuilder.selects.filter(function(s){ return s.name }).map(function(s){ return s.name }).join(',');\n\t\tvar expands = GridBuilder.expandQuery();\n\t\tvar odataAppend = (selects ? ('?$select=' + selects) : '') + ( selects && expands ? '&' : ( expands ? '?' : '' ) ) + (expands ? ('$expand=' + expands) : '');\n\t\t\n\t\tvar query = base + odataAppend;\n\t\t// return object with all info available\n\t\treturn{\n\t\t\tbase: base,\n\t\t\tappend: odataAppend,\n\t\t\tfull: query\n\t\t};\n\t},\n\t\n\texpandQuery:function(query, expands){\n\t\tvar exquery = '';\n\t\tvar selquery = '';\n\t\t\n\t\tquery = query || '';\n\t\t// get list of expands relevant\n\t\texpands = (expands || GridBuilder.expands) || [];\n\t\t\n\t\texpands.map(function(ex){\n\t\t\t// field will be needed no matter what\n\t\t\tquery += ex.field;\n\t\t\t\n\t\t\tif(ex.selects){\n\t\t\t\t// selects is just a string list\n\t\t\t\tselquery = ex.selects.map(function(s){ return s.name }).join(',');\n\t\t\t}\n\t\t\tif(ex.expands){\n\t\t\t\t// if we have expands within this one then recursively build the string\n\t\t\t\texquery =  GridBuilder.expandQuery('', ex.expands);\n\t\t\t}\n\t\t\t\n\t\t\tif(selquery || exquery){\n\t\t\t\t// generate final query\n\t\t\t\tquery += '(' + (selquery ? ('$select=' + selquery + ';') : \"\") + (exquery ? ('$expand=' + exquery + \"\" ): \"\") + '),';\n\t\t\t}\n\t\t\telse{\n\t\t\t\tquery += ',';\n\t\t\t}\n\t\t});\n\t\t\n\t\tif(query[query.length -1] === \",\"){\n\t\t\tquery = query.substring(0, query.length - 1);\n\t\t}\n\t\t\n\t\treturn query;\n\t},\n\t\n\tendpointChange:function(e){\n\t\t// define endpoint\n\t\tGridBuilder.endpoint = endpoint = e.sender.value(); \n\t\t\n\t\tvar contextsDD = $('[name=contexts]', GridBuilder.root);\n\t\t\n\t\t// get a list of contexts from this endpoint\n\t\tapi.get(endpoint, function(contexts){\n\t\t\t// create a drop down with a change event\n\t\t\tcontextsDD.kendoDropDownList({\n\t\t\t\tdataSource: contexts.value,\n\t\t\t\tdataTextField:'name',\n\t\t\t\tdataValueField: 'url',\n\t\t\t\tchange: GridBuilder.contextChange\n\t\t\t});\n\t\t\t\n\t\t\t// select first and trigger change event\n\t\t\tcontextsDD.data('kendoDropDownList').select(0);\n\t\t\tcontextsDD.data('kendoDropDownList').trigger('change');\n\t\t});\n\t},\n\t\n\tcontextChange:function(e){\n\t\t// define context\n\t\tGridBuilder.context = context = e.sender.value(); \n\t\tGridBuilder.setupFilters();\n\t\t// get meta data for endpoint/context\n\t\tapi.get(GridBuilder.endpoint + '/' + context + '/GetMetadata', function(meta){\n\t\t\tvar selectsGrid = $('[name=selects] > [name=grid]', GridBuilder.root);\n\t\t\tvar expandsGrid = $('[name=expands] > [name=grid]', GridBuilder.root);\n\t\t\t\n\t\t\t// format the properties\n\t\t\tvar props = meta.Properties.map(GridBuilder.formatProperty);\n\t\t\tGridBuilder.fields = props;\n\t\t\t// build grids\n\t\t\tGridBuilder.setSelects(selectsGrid, props.filter(GridBuilder.isScalar));\n\t\t\tGridBuilder.setExpands(expandsGrid, props.filter(GridBuilder.notScalar));\n\t\t\t\n\t\t\t$('body').kendoTooltip({\n\t\t\t\tfilter:'th',\n\t\t\t\tposition: \"top\",\n\t\t\t\tcontent:function(e){\n\t\t\t\t\tvar target = e.target;\n\t\t\t\t\tif($(target).text()){\n\t\t\t\t\t\te.sender.popup.element.css(\"visibility\", \"visible\");\n\t\t\t\t\t\treturn $(target).text();\n\t\t\t\t\t}\n\t\t\t\t\telse{\t\t\t\t\t\n\t\t\t\t\t\te.sender.popup.element.css(\"visibility\", \"hidden\");\n\t\t\t\t\t}\n\t\t\t\t}\n\t\t\t});\n\t\t});\n\t},\n\t\n\tformatProperty:function(prop){\n\t\tvar scalar = type.ofProperty.isScalar(prop);\n\t\t\n\t\t// reformat before grid\n\t\treturn {\n\t\t\tname: prop.Name,\n\t\t\ttemplateType:'',\n\t\t\ttitle: prop.Name,\n\t\t\ttype: prop.ServerType,\n\t\t\tisScalar: scalar,\n\t\t\t// auto select all scalar properties\n\t\t\tselected: false,\n\t\t\ttemplate:'#: '+prop.Name+' #',\n\t\t\torder:0,\n\t\t\twidth:'auto',\n\t\t\tgroup:false,\n\t\t\tsort:false,\n\t\t\thidden: false,\n\t\t};\n\t},\n\t\n\tformatChildProperty:function(prop){\n\t\t// reformat from grid\n\t\treturn {\n\t\t\tname: prop.name,\n\t\t\ttemplateType:'',\n\t\t\ttitle:prop.title,\n\t\t\tselected: prop.selected,\n\t\t\tisScalar: prop.isScalar,\n\t\t\ttemplate: prop.template,\n\t\t\ttype: prop.type,\n\t\t\torder: prop.order,\n\t\t\twidth:prop.width,\n\t\t\tgroup: prop.group,\n\t\t\tsort: prop.sort,\n\t\t\thidden: prop.hidden\n\t\t};\n\t},\n\t\n\t// return if property is scalar\n\tisScalar:function(item){ return item.isScalar },\n\t\n\t// return if property is not scalar\n\tnotScalar:function(item){ return !item.isScalar },\n\t\n\tisGrouped: function(item){ return item.group },\n\t\n\tisSorted: function(item){ return item.sort },\n\t\n\t// return if property is selected\n\tisSelected:function(item){ return item.selected },\n\t\n\t// check to see if kendo grid is defined on object and if so destroy\n\tcheckOld:function(grid){\n\t\tif(grid.data('kendoGrid')){ \n\t\t\tgrid.data('kendoGrid').destroy();\n\t\t\tgrid.empty();\n\t\t}\n\t},\n\t\n\tsetExpands:function(grid, data){\t\n\t\tGridBuilder.checkOld(grid);\n\t\t// build kendo grid for expands grid\n\t\tgrid.kendoGrid({\n\t\t\tdataSource: data,\n\t\t\teditable: true,\n\t\t\tcolumns:[\n\t\t\t\t{ template: '<input type=\"checkbox\" #: selected ? \"checked\" : \"\" # />', width:25 },\n\t\t\t\t{ field: 'name' }\n\t\t\t],\n\t\t\tdataBound: GridBuilder.onBind,\n\t\t\tdetailInit: GridBuilder.detail\n\t\t});\n\t},\n\t\n\tDefaultEditor:function(con, op){\n\t\t// deafult editor(inline)\n\t\tvar input = $('<input type=\"text\" class=\"k-input k-textbox\" name=\"'+op.field+'\" data-bind=\"value:'+op.field+'\">'); \n\t\tinput.appendTo(con); \n\t},\n\t\n\tDisabledEditor:function(con, op){\n\t\tcon.append( '<span style=\"padding:0 .3em\">' + op.model[op.field] + '</span>');\n\t},\n\t\n\tsetSelects:function(grid, data){\n\t\t// the only time we want them all selected is on this grid\n\t\tdata.map(function(d, i){ d.order = i; d.selected = true; return d; });\n\t\t\n\t\tGridBuilder.checkOld(grid);\n\t\t// build kendo grid for selects grid\n\t\tgrid.kendoGrid({\n\t\t\ttoolbar:'<button class=\"btn\" name=\"add\">Add Custom Column</button>',\n\t\t\tdataSource: {\n\t\t\t\tdata: data,\n\t\t\t\tsort:{ field:'order', dir:'asc' }\n\t\t\t},\n\t\t\teditable: true,\n\t\t\tsave:function(e){ grid.data('kendoGrid').refresh(); },\n\t\t\tcolumns:[\n\t\t\t\t{ field: 'order', width: 50 },\n\t\t\t\t{ template: '<input class=\"chkSelected\" type=\"checkbox\" #: selected ? \"checked\" : \"\" # />', width:100 , headerTemplate: '<input type=\"checkbox\" name=\"checkall\" checked/> Select'},\n\t\t\t\t{ title:'Group', template: '<input class=\"chkGrouped\" type=\"checkbox\" #: group ? \"checked\" : \"\" # #: name ? \"\" : \"disabled\" #/>', width:50 },\n\t\t\t\t{ title:'Sort', template: '<input class=\"chkSort\" type=\"checkbox\" #: sort ? \"checked\" : \"\" # #: name ? \"\" : \"disabled\" #/>', width:50 },\n\t\t\t\t{ title:'Hide', template: '<input class=\"chkHidden\" type=\"checkbox\" #: hidden ? \"checked\" : \"\" # #: name ? \"\" : \"disabled\" #/>', width:50 },\n\t\t\t\t{ field: 'name', editor: GridBuilder.DisabledEditor, template:'#: name ? name : \"Custom\" #' },\n\t\t\t\t{ field: 'title', editor: GridBuilder.DefaultEditor },\n\t\t\t\t{ \n\t\t\t\t\tfield: 'template', \n\t\t\t\t\teditor: function(con,op){ \n\t\t\t\t\t\t// we need a custom editor so someone can only edit this when custome is selected\n\t\t\t\t\t\top.model.templateType !== 'date' ? GridBuilder.DefaultEditor(con, op) : GridBuilder.DisabledEditor(con, op);\n\t\t\t\t\t}\n\t\t\t\t},\n\t\t\t\t// create custom template checkboxes\n\t\t\t\t{ field:'width' },\n\t\t\t\t{ width:50, title:'Date', template: '<input type=\"radio\" name=\"#:name#_template\" value=\"Date\" #: GridBuilder.checkTemplatetype(templateType, \"date\") # />' },\n\t\t\t\t{ width:50, title:'Currency', template: '<input type=\"radio\" name=\"#:name#_template\" value=\"Currency\" #: GridBuilder.checkTemplatetype(templateType, \"currency\") # />' },\n\t\t\t\t{ width:50, title:'Number', template: '<input type=\"radio\" name=\"#:name#_template\" value=\"Number\" #: GridBuilder.checkTemplatetype(templateType, \"number\") # />' },\n\t\t\t\t{ width:50, title:'Custom', template: '<input type=\"radio\" name=\"#:name#_template\" value=\"Custom\" #: GridBuilder.checkTemplatetype(templateType, \"\") # />' }\n\t\t\t],\n\t\t\tdataBound: GridBuilder.onBind\n\t\t});\n\t},\n\t\n\tcheckTemplatetype:function(type, target){\n\t\t// check wether we need to check this item\n\t\treturn type === target ? 'checked' : '';\n\t},\n\t\n\thandleRadio:function(e){\n\t\t// whena  radio button is checked we need to change the template\n\t\tvar row = $(e.target).closest('tr');\n\t\tvar dataitem = e.data.grid.data('kendoGrid').dataItem(row);\n\t\t\n\t\t// get the type \n\t\tvar type = $(e.target).attr('value');\n\t\tdataitem.attributes = {};\n\t\t// set dataitem template and template type dependent on type\n\t\tswitch(type){\n\t\t\tcase 'Date':\n\t\t\t\tdataitem.template = \"#:kendo.toString(new Date(\"+dataitem.name+\"),'dd/MM/yyyy')#\";\n\t\t\t\tdataitem.templateType = \"date\";\n\t\t\tbreak;\n\t\t\tcase 'Currency':\n\t\t\t\tdataitem.template = \"#:my.formatCurrency(\"+dataitem.name+\")#\";\n\t\t\t\tdataitem.attributes = {'style' : 'text-align: right;' };\n\t\t\t\tdataitem.templateType = \"currency\";\n\t\t\tbreak;\n\t\t\tcase 'Number':\n\t\t\t\tdataitem.templateType = \"number\";\n\t\t\t\tdataitem.attributes = {'style' : 'text-align: right;' };\n\t\t\tbreak;\n\t\t\tcase 'Custom':\n\t\t\t\tdataitem.template = \"#:\"+dataitem.name+\"#\";\n\t\t\t\tdataitem.templateType = \"\";\n\t\t\tbreak;\n\t\t};\n\t\t\n\t\t// refresh grid and selects\n\t\te.data.grid.data('kendoGrid').refresh();\n\t\tGridBuilder.updateSelects();\n\t},\n\t\n\thandleCheck:function(e){\n\t\tvar row = $(e.target).closest('tr');\n\t\tvar dataitem = e.data.grid.data('kendoGrid').dataItem(row);\n\t\t\n\t\tdataitem.selected = this.checked;\n\t\t\n\t\t// empty expands and selects\n\t\tGridBuilder.expands = [];\n\t\tGridBuilder.selects = [];\n\t\t// update all selects and expands\n\t\tGridBuilder.updateSelects();\n\t\tGridBuilder.updateExpands(e.data.grid);\n\t},\n\t\n\thandleGroupCheck:function(e){\n\t\tvar row = $(e.target).closest('tr');\n\t\tvar dataitem = e.data.grid.data('kendoGrid').dataItem(row);\n\t\t\n\t\tdataitem.group = this.checked;\n\t},\n\t\n\thandleSortCheck:function(e){\n\t\tvar row = $(e.target).closest('tr');\n\t\tvar dataitem = e.data.grid.data('kendoGrid').dataItem(row);\n\t\t\n\t\tdataitem.sort = this.checked;\n\t},\n\t\n\thandleHiddenCheck:function(e){\n\t\tvar row = $(e.target).closest('tr');\n\t\tvar dataitem = e.data.grid.data('kendoGrid').dataItem(row);\n\t\t\n\t\tdataitem.hidden = this.checked;\n\t},\n\t\n\tdetail:function(e){\n\t\te.data.id = e.data.id || e.data.name;\n\t\t\n\t\t// get meta data value for expanded item\n\t\tdebugger;\n\t\tvar ctx = e.data.type.split(',')[0].split('.');\n\t\tctx = ctx[ctx.length - 1];\n\t\t\n\t\tapi.get(GridBuilder.endpoint + '/' + ctx + '/GetMetadata()', function(meta){\n\t\t\tvar data = meta.Properties.map(GridBuilder.formatProperty);\n\t\t\t\n\t\t\tdata.map(function(d){ d.id = e.data.id + '/' + d.name; return d; });\n\t\t\t\n\t\t\tvar grid = $('<div></div>').appendTo(e.detailCell);\n\t\t\t// build expands grid\n\t\t\tGridBuilder.setExpands(grid, data);\n\t\t})\n\t},\n\t\n\tupdateSelects:function(){\n\t\t// selects are just all items selected in selects grid\t\t\n\t\tGridBuilder.selects = $('[name=selects] [name=grid]').data('kendoGrid').dataSource.data().toJSON().filter(GridBuilder.isSelected);\t\n\t},\n\t\n\tupdateExpands:function(grid){\n\t\tvar end_data = [];\n\t\tvar grids = $('[name=expands] .k-grid').get();\n\t\t\n\t\t// get a list of all data from all expand grids\n\t\tfor(var i = 0; i < grids.length; i++){\n\t\t\tvar data = $(grids[i]).data('kendoGrid').dataSource.data().filter(GridBuilder.isSelected);\n\t\t\tend_data.push(data)\n\t\t}\n\t\t\n\t\t// format that data and set GridBuilder.expands\n\t\tend_data.map(function(dataset){\n\t\t\tdataset = dataset.map(function(d){ d.parts = (d.id ? d.id.split('/') : [d.name]); return d; });\n\t\t\t\n\t\t\tGridBuilder.setExpandValue(dataset);\n\t\t});\n\t},\n\t\n\tupdateFilters:function(grid){\n\t\tvar grid = $('[name=tbfilters] [name=grid]');\n\t\tvar data = grid.data('kendoGrid').dataSource.data().toJSON();\n\t\t\n\t\tdata.map(function(d){\n\t\t\t// if our value type is number then convert\n\t\t\tif(\n\t\t\t\td.type.toLowerCase().indexOf('int') > -1 ||\n\t\t\t\td.type.toLowerCase().indexOf('decimal') > -1 \n\t\t\t  ){\n\t\t\t\td.value = parseFloat(d.value);\n\t\t\t}\n\t\t})\n\t\t\n\t\tGridBuilder.filters = data;\n\t},\n\t\n\tsetExpandValue:function(data){\n\t\t// function to find the base level object\n\t\tfunction findBase(parts, parent){\n\t\t\tif(parts.length === 0){\n\t\t\t\treturn parent;\n\t\t\t}\n\t\t\telse{\n\t\t\t\tvar next = parent.expands.filter(function(x){ return x.field == parts[0] })[0];\n\t\t\t\tif(next){\n\t\t\t\t\tparts.shift();\n\t\t\t\t\treturn findBase(parts, next);\n\t\t\t\t}\n\t\t\t}\n\t\t}\n\t\t\n\t\t// we know a given dataset will all have the same parent;\n\t\tif(data.length > 0){\n\t\t\tvar parts = data[0].parts;\n\t\t\tparts.pop();\n\t\t\tvar parent = null;\n\t\t\tvar formattedData = data.filter(GridBuilder.notScalar).map(function(d){ return { field: d.name, selects:[], expands:[] }});\n\t\t\t\n\t\t\tif(parts.length === 0){\n\t\t\t\t// we know there are no parents so we can set the base.\n\t\t\t\tGridBuilder.expands = formattedData;\n\t\t\t}\n\t\t\telse{\n\t\t\t\t// we need to find the parent;\n\t\t\t\tparent = parent || (GridBuilder.expands || []).filter(function(x){ return x.field === parts[0] })[0]; // if there isn't one for the top parts option then we dont want tog o further\n\t\t\t\tif(parent){\n\t\t\t\t\t// remove top part\n\t\t\t\t\tparts.shift();\n\t\t\t\t\t// find the base level object\n\t\t\t\t\tvar key = findBase(parts, parent);\n\t\t\t\t\t// set value\n\t\t\t\t\tkey.selects = data.filter(GridBuilder.isScalar).map(GridBuilder.formatChildProperty);\n\t\t\t\t\tkey.expands = formattedData;\n\t\t\t\t}\n\t\t\t}\n\t\t}\n\t},\n\t\t\n\tonBind:function(e){\n\t\tvar grid = e.sender.element;\t\t\n\t\t\n\t\t$('[name=add]', grid).off('click').on('click',{grid, grid}, GridBuilder.addCol);\n\t\t\n\t\tvar rows = $('tr', grid);\n\t\t\t\n\t\t// make sure any columns that are for scalar properties do not give detail init functionality\n\t\tfor(var r = 0; r < rows.length; r++){\n\t\t\tvar row = rows[r];\n\t\t\tvar dataitem = grid.data('kendoGrid').dataItem(row);\n\t\t\t\n\t\t\tif(dataitem && dataitem.isScalar){\n\t\t\t\t$('.k-icon', row).remove();\n\t\t\t}\n\t\t}\n\t\t\n\t\t// handle checkbox click\n\t\t$('[type=checkbox]', grid).off('click').on('click', {grid: grid}, GridBuilder.handleCheck);\n\t\t$('[type=checkbox].chkGrouped', grid).off('click').on('click', {grid: grid}, GridBuilder.handleGroupCheck);\n\t\t$('[type=checkbox].chkSort', grid).off('click').on('click', {grid: grid}, GridBuilder.handleSortCheck);\n\t\t$('[type=checkbox].chkHidden', grid).off('click').on('click', {grid: grid}, GridBuilder.handleHiddenCheck);\n\t\t\n\t\t$('[type=radio]', grid).off('click').on('click', {grid: grid}, GridBuilder.handleRadio);\n\t\t\n\t\t$('[name=checkall]', grid).off('click').on('click', function(e){\n\t\t\tvar checked = this.checked;\n\t\t\tvar datagrid = grid.data('kendoGrid');\n\t\t\t\n\t\t\tdatagrid.dataSource.data(\n\t\t\t\tdatagrid.dataSource.data().map(function(d){ d.selected = checked; return d ;})\n\t\t\t)\n\t\t});\n\t\t\n\t\tGridBuilder.updateSelects(grid);\n\t\tGridBuilder.updateExpands(grid);\n\t},\n\t\n\taddCol:function(e){\n\t\te.data.grid.data('kendoGrid').dataSource.add({\n\t\t\tname:'',\n\t\t\ttitle:'',\n\t\t\ttemplate:'',\n\t\t\ttemplateType:'',\n\t\t\tisScalar:false,\n\t\t\tselected:true,\n\t\t\tgroup: false,\n\t\t\torder:0,\n\t\t\twidth:'auto',\n\t\t\tsort: false,\n\t\t\thidden: false\n\t\t});\n\t}\n}",
  "Content": "<script src=\"https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.38.0/codemirror.min.js\"></script>\n<script src=\"https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.38.0/mode/javascript/javascript.min.js\"></script>\n<script src=\"https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.38.0/mode/xml/xml.min.js\"></script>\n<script src=\"https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.38.0/mode/css/css.min.js\"></script>\n<script src=\"https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.38.0/mode/htmlmixed/htmlmixed.min.js\"></script>\n<link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.38.0/codemirror.min.css\" />\n\n<div name=\"tabstrip\">\n\t<ul>\n\t\t<li name=\"query\" class=\"k-state-active\">Query</li>\n\t\t<li name=\"filters\">Filters & Actions</li>\n\t\t<li name=\"preview\">Preview</li>\n\t</ul>\n\t<div name=\"GBWizard\" class=\"component tab-content\" >\n\t\t<div class=\"container\">\t\t\t\n\t\t\t<ul class=\"fieldList\">\t\t\t\t\n\t\t\t\t<li>\n\t\t\t\t\t<label>Grid Name</label>\n\t\t\t\t\t\t<div class=\"value\">\n\t\t\t\t\t\t<input type=\"text\" name=\"gridname\" />\n\t\t\t\t\t</div>\n\t\t\t\t</li>\n\t\t\t\t<li>\n\t\t\t\t\t<label>Filterable</label>\n\t\t\t\t\t\t<div class=\"value\">\n\t\t\t\t\t\t<input type=\"checkbox\" name=\"filterable\" checked />\n\t\t\t\t\t</div>\n\t\t\t\t</li>\n\t\t\t</ul>\n\t\t\t<hr />\n\t\t\t<p> If you can do it in javascript you can do it here! ...Almost</p>\n\t\t\t<p> Inside your template you have access to all selected properties, if you keep you're code between the hashes like this --  #: {Code Here} # -- then you can use javascript </p>\n\t\t\t<p> e.g. to multiply your Id by 2 simply put #: Id * 2 # in the template </p>\n\t\t\t<p class=\"error\">Reminder: remember to null check when doing complex equations </p>\n\t\t\t<p><a href=\"https://docs.telerik.com/kendo-ui/framework/templates/overview\" target=\"_blank\" >More info...</a></p>\n\t\t\t<hr />\n\t\t\t<input name=\"endpoints\" />\n\t\t\t<input name=\"contexts\" />\n\t\t</div>\n\t\t<div class=\"container\">\n\t\t\t<div name=\"toolbarDesigner\" >\n\t\t\t\t<ul class=\"fieldList\">\n\t\t\t\t\t<li>\n\t\t\t\t\t\t<label>[resource_displayname[ToolbarMarkup]]</label>\n\t\t\t\t\t\t<div >\n\t\t\t\t\t\t\t<input name=\"toolbar\" />\n\t\t\t\t\t\t</div>\n\t\t\t\t\t</li>\n\t\t\t\t</ul>\n\t\t\t</div>\n\t\t</div>\n\t\t<p>\n\t\t\tif you want to include a field in the OData query that the grid uses then make sure that the 'Select' column is checked,\n\t\t\tAs long as it's selected you can use it in the template of any column.\n\t\t\tIf you do not want to see that column but have it selected for any reason then you can check the 'hide' column and the column will not appear in the final grid.\n\t\t</p>\n\t\t<div class=\"container\">\n\t\t\t<div name=\"selects\" >\n\t\t\t\t<div name=\"grid\"></div>\n\t\t\t</div>\n\t\t</div>\n\t\t<div class=\"container\">\n\t\t\t<div name=\"expands\" >\n\t\t\t\t<div name=\"grid\"></div>\n\t\t\t</div>\n\t\t</div>\n\t</div>\n\t<div name=\"tbfilters\" class=\"tab-content\">\n\t\t<div name=\"grid\"></div>\n\t\t<br>\n\t\t<div name=\"actionGrid\"></div>\n\t</div>\n\t<div name=\"tbpreview\" class=\"tab-content\">\n\t\t<p name=\"fullQuery\"></p>\n\t\t<p class=\"error\">Headers for these columns will be resourced when grid is saved and can then be edited from resource management if required</p>\n\t\t<div name=\"grid\"></div>\n\t\t<div class=\"container\">\n\t\t\t<button name=\"save\" class=\"btn to-right\">Save</button>\n\t\t</div>\n\t</div>\n</div>\n\n<style>\n\t[name=toolbarDesigner] .fieldList div{\n\t\twidth: calc(100% - 205px);\n\t}\n\t[name=toolbarDesigner] .fieldList div input{\n\t\twidth:100%\n\t}\n\t.k-grid-content {\n\t\t\n\t}\n\tdiv.k-window {\n\t\toverflow: hidden;\n\t}\n\t.k-callout-s{\n\t\tborder-top-color: #e2721d !important;\n\t}\n\t.k-tooltip{\n\t\tbackground:#e2721d !important;\n\t}\n\t.k-tooltip *{\n\t\tcolor:#fff !important;\n\t}\n\t[name=GBWizard]{\n\t\tposition: relative;\n\t}\n\t.k-tabstrip>.k-content{\n\t\tmargin:0;\n\t}\n\tp{\n\t\tword-wrap: break-word;\n\t}\n\t.k-window .k-window-content{\n\t\tborder:0;\n\t\tbox-shadow: none;\n\t}\n\t.tab-content{\n\t\toverflow-y: auto;\n\t\twidth: calc(100% - 21px);\n\t}\n</style>",
  "LastUpdated": "2021-11-19T21:27:01.461463+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "LayoutManagement",
  "Key": "CMS",
  "ResourceKey": "CMS",
  "Script": "LayoutManagement = {\n    init: async function (app, container) {\n        app = app || session.app;\n        container = container || $(\".component[name=LayoutManagement]\");\n        api.addToMetaCache([\n            {\n                \"Name\": \"Core\",\n                \"Types\": [\n                    [meta[ContentManagement/Layout]]\n                ]\n            }\n        ]);\n        var config = {\n            endpoint: \"ContentManagement/Layout\",\n            odataAppend: \"?$filter=AppId eq \" + app.Id\n        };\n        let ds = await model.getDatasource(config);\n        await LayoutManagement.initGrid(app, container, ds);\n\n        await loadComponent($('[name=layoutMigrationComponent]', container), 'LayoutMigration');\n        await LayoutMigration.init(app, $(`.component[name=LayoutMigration]`, container));\n    },\n\n    initGrid: async function(app, container, dataSource) {\n        var grid = new GridWidget(container, dataSource);\n        grid.groupable = false;\n        grid.toolbar = [\n        {\n            template: \"<div class='btn-group btn-group-sm'> \\\n                <button class='btn btn-primary' name='create'> \\\n                    <span class='k-icon k-i-plus'></span>[resource_displayname[new]] \\\n                </button> \\\n                <button class='btn btn-primary' name='migrate'> \\\n                    <span class='k-icon k-i-arrow-up'></span>[resource_displayname[migrate]] \\\n                </button> \\\n            </div>\"\n        }];\n        \n        grid.columns = [\n            { \n                field: \"Name\", \n                title: '[resource_displayname[name]]' \n            },\n\t\t\t{ \n                field: \"LastUpdated\", \n                title: \"[resource_displayname[lastupdated]]\", \n                type: \"date\", \n                format: \"{0: \" + type.dateFormat + \" HH:mm}\",\n                width: \"[theme[columns.small]]\" \n            },\n            { \n                field: \"LastUpdatedBy\", \n                title: \"[resource_displayname[lastupdated]]\",\n                width: \"[theme[columns.small]]\"\n            }\n        ];\n        grid.commands.push({name: \"save\", icon: \"k-i-save\", text: \"[resource_displayname[save]]\"});\n        grid.commands.push({name: \"destroy\", icon: \"k-i-trash\", text: \"[resource_displayname[delete]]\"});\n        grid.detailTemplate = kendo.template($(\"[name=layoutDetails]\", container).html());\n        grid.detailExpand = async function(e) {\n            var item = grid.kendoObject.dataItem(e.masterRow);\n            var expandContainer = $(e.detailRow);\n            var replaced = expandContainer.html().replaceAll('{ID}', Guid());\n            \n            expandContainer.html(replaced);\n\n            if ($(e.detailRow).find(\".monaco-editor\").length === 0) {\n                await LayoutManagement.layoutExpand(item, expandContainer);\n            };\n        };\n        grid.dataBound = function(e) {\n            $(\"button[name=save]\", grid.gridElement).on(\"click\", (e) => LayoutManagement.save(e, grid));\n            $(\"button[name=destroy]\", grid.gridElement).on(\"click\", (e) => LayoutManagement.destroy(e, grid));\n        };\n        grid.resizable = {\n            rows: true\n        };\n        await grid.init();\n        $(\"button[name=migrate]\", grid.gridElement).on(\"click\",  (e) => LayoutManagement.migrate(e, app, container));\n        $(\"button[name=create]\", grid.gridElement).on(\"click\", (e) => LayoutManagement.create(e, app, grid));\n        return grid;\n    },\n\n    layoutExpand: async function(item, container) {\n        var headerEditor = new HTMLMonacoEditor($(\"div[name=header] > div[name=editorContainer]\", container)[0], {\n            code: item.HeaderHtml\n        });\n        var bodyEditor = new HTMLMonacoEditor($(\"div[name=body] > div[name=editorContainer]\", container)[0], {\n            code: item.Html\n        });\n        headerEditor.onChange = () => item.HeaderHtml = headerEditor.getValue();\n        bodyEditor.onChange = () => item.Html = bodyEditor.getValue();\n        headerEditor.init();\n        bodyEditor.init();\n\n        $('[name=header-tab-button]', container).on('click', () => {\n            headerEditor.editor.layout();\n        });\n\n        $('[name=body-tab-button]', container).on('click', () => {\n            bodyEditor.editor.layout();\n        });\n    },\n\n    migrate: async function (e, app, container) {\n        e.preventDefault();\n        var migrateDialog = new Dialog({\n            width: 620,\n            height: \"auto\",\n            title: \"[resource_displayname[migrateLayouts]]\"\n        });\n\n        migrateDialog.template = $(\"[name=layoutMigrationComponent]\", container).first().html();\n        await migrateDialog.init();\n\n        await LayoutMigration.initDropdown($('[name=LayoutMigration]', migrateDialog.element));\n    },\n\n    create: async function (e, app, grid) {\n        e.preventDefault();\n        var createLayoutDialog = new Dialog({\n            width: 600,\n            height: \"auto\",\n            title: \"[resource_displayname[newlayout]]\"\n        });\n        createLayoutDialog.template = $(\"[name=newLayoutDialog]\").first().html();\n        createLayoutDialog.events.create = async function (e) {\n            await api.add(\"ContentManagement/Layout\", {\n                Id: 0,\n                AppId: app.Id,\n                Name: $(\"[name=name]\", createLayoutDialog.element).val(),\n                HeaderHtml: \"\", \n                Html: \"\"\n            }).then(() => {\n                notification.success('[resource_displayname[created]]');\n                createLayoutDialog.events.close();\n                grid.refresh();\n            }).catch((e) => notification.error(e));\n        };\n        await createLayoutDialog.init();\n    },\n\n    save: async function (e, grid) {\n        e.preventDefault();\n        var layout = grid.dataItem($(e.currentTarget).closest(\"tr\"));\n        await layout.save(e);\n        notification.success(\"[resource_description[saved]]\");\n    },\n\n    destroy: async function (e, grid) {\n        e.preventDefault();\n        var layout = grid.dataItem($(e.currentTarget).closest(\"tr\"));\n        await layout.destroy(e);\n        notification.success(\"[resource_description[LayoutDeleted]]\");\n        grid.refresh();\n    }\n}",
  "Content": "<script type=\"text/template\" name=\"layoutDetails\">\n<div class=\"tab-control\" name=\"tabs\">\n\t<nav>\n\t\t<div class=\"nav nav-tabs\" id=\"app-layout-nav-tab-{ID}\" role=\"tablist\">\n\t\t\t<button class=\"nav-link bg active\" id=\"app-layout-header-tab-{ID}\" data-bs-toggle=\"tab\" data-bs-target=\"\\#app-layout-header-{ID}\" type=\"button\" role=\"tab\" aria-controls=\"app-layout-header-{ID}\" aria-selected=\"true\" name=\"header-tab-button\">\n\t\t\t\t<span class=\"k-icon k-i-clipboardHtmlIcon\"></span>[resource_displayname[header]]\n\t\t\t</button>\n\t\t\t<button class=\"nav-link bg\" id=\"app-layout-body-tab-{ID}\" data-bs-toggle=\"tab\" data-bs-target=\"\\#app-layout-body-{ID}\" type=\"button\" role=\"tab\" aria-controls=\"app-layout-body-{ID}\" aria-selected=\"false\" tabindex=\"-1\" name=\"body-tab-button\">\n\t\t\t\t<span class=\"k-icon k-i-jsIcon\"></span>[resource_displayname[body]]\n\t\t\t</button>\n\t\t</div>\n\t</nav>\n\n\t<div class=\"tab-content\" id=\"app-layout-nav-tab-{ID}Content\">\n\t\t<div class=\"tab-pane fade active show\" id=\"app-layout-header-{ID}\" role=\"tabpanel\" aria-labelledby=\"app-layout-header-tab-{ID}\" name=\"header\">\n\t\t\t<div name=\"editorContainer\"></div>\n\t\t</div>\n\t\t<div class=\"tab-pane fade\" id=\"app-layout-body-{ID}\" role=\"tabpanel\" aria-labelledby=\"app-layout-body-tab-{ID}\" name=\"body\">\n\t\t\t<div name=\"editorContainer\"></div>\n\t\t</div>\n\t</div>\n</div>\n</script>\n\n<script type=\"text/template\" name=\"newLayoutDialog\">\n\t<div class=\"input-group input-group-sm mb-1\">\n\t\t<span class=\"input-group-text\">[resource_displayname[name]]</span>\n\t\t<input type=\"text\" class=\"form-control\" name=\"name\" />\n\t</div>\n\n\t<hr />\n\t\n\t<button class=\"btn btn-sm btn-primary float-end\" name=\"create\">\n\t\t<span class=\"k-icon k-i-plus\"></span>[resource_displayname[create]]\n\t</button>\n</script>\n<div name=\"layoutMigrationComponent\" style=\"display: none\">\n\t<div class=\"component\" name=\"LayoutMigration\"></div>\n</div>",
  "LastUpdated": "2024-11-19T18:18:30.4653119+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "LayoutMigration",
  "Key": "CMS",
  "ResourceKey": "CMS",
  "Script": "LayoutMigration = {\n    init: async function (app, container) {\n        app = app || session.app;\n        container = container || $(\".component[name=LayoutMigration]\");\n        api.addToMetaCache([\n            {\n                \"Name\": \"Core\",\n                \"Types\": [\n                    [meta[ContentManagement/App]]\n                ]\n            }\n        ]);\n\n        var list = (await api.get(\"ContentManagement/Layout?$filter=AppId eq \" + app.Id + \"&$select=Name\")).value;\n        var g = new GridWidget($(\"[name=layoutGrid]\", container), list);\n        g.columns = [\n            { selectable: true, width: 40 },\n            {\n                field: \"Name\",\n            }\n        ];\n        g.groupable = false;\n        g.init(() => {\n            g.kendoObject.dataSource.sort({ field: \"Name\", dir: \"asc\" });\n            g.kendoObject.select(g.kendoObject.tbody.find(\">tr\"));\n        });\n        $(\"[name=migrate]\", container).on(\"click\", async function (e) {\n            var packages = await LayoutMigration.getPackages($(\"[name=layoutGrid]\", container), app);\n            var appId = $(\"[name=app]\", container).val();\n            await api.add(\"Packaging/Package/ImportThis?appId=\" + appId, packages);\n            notification.success(\"[resource_displayname[migrated]]\");\n        });\n    },\n\n    initDropdown: async function(container) {\n        let ds = await model.getDatasource({\n            endpoint: \"ContentManagement/App\"\n        });\n\n        $(\"[name=app]\", container).kendoDropDownList({\n            optionLabel: \"[resource_displayname[selectapp]]\",\n            dataTextField: \"Name\",\n            dataValueField: \"Id\",\n            dataSource: ds\n        });\n    },\n\n    getPackages: async function (container, app) {\n        var packages = [{\n            Id: \"00000000-0000-0000-0000-000000000000\",\n            Name: \"Layouts\",\n            Description: \"Generated by LayoutManagement\",\n            Category: \"Dynamic\",\n            SourceApi: session.apiRoot,\n            Items: [\n                await LayoutMigration.getPackage(container, app)\n            ]\n        }];\n        return packages;\n    },\n\n    getPackage: async function (container, app) {\n        var grid = $(container).find(\".k-grid\").data(\"kendoGrid\");\n        var layouts = [];\n        grid.select().each(function () {\n            layouts.push(grid.dataItem(this).Name);\n        });\n        var query = \"\";\n        for (var i = 0; i < layouts.length; i = i + 1) {\n            query += \"'\" + layouts[i] + \"',\";\n        }\n        query = query.substring(0, query.length - 1);\n        var data = await api.get(\"ContentManagement/Layout?$filter=AppId eq \" + app.Id + \" and Name in (\" + query + \")\");\n        for (var i = 0; i < data.value.length; i = i + 1) {\n            delete data.value[i].Id;\n        }\n        return {\n            Id: \"00000000-0000-0000-0000-000000000000\",\n            PackageId: \"00000000-0000-0000-0000-000000000000\",\n            Type: \"ContentManagement/Layout\",\n            Data: JSON.stringify(data.value)\n        };\n    }\n};",
  "Content": "<div class=\"row\">\n    <div class=\"col-md-12\">\n        <div class=\"input-group input-group-sm mb-1\">\n            <span class=\"input-group-text\">[resource_displayname[app]]</span>\n            <input type=\"text\" class=\"form-control\" name=\"app\" />\n        </div>\n    </div>\n    <div class=\"col-md-12\">\n        <div name=\"layoutGrid\"></div>\n    </div>\n</div>\n\n<hr />\n\n<button class=\"btn btn-sm btn-primary float-end\" name=\"migrate\">\n    <span class=\"k-icon k-i-arrow-up\"></span>[resource_displayname[migrate]]\n</button>",
  "LastUpdated": "2024-11-19T18:18:31.1343789+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "Metadata",
  "Key": "Tools",
  "ResourceKey": "Debug",
  "Script": "Metadata = {\n    init: async function (app, container) {\n        app = app || session.app;\n        container = container || $(\".component[name=Metadata]\");\n        var swaggerDefinition = await api.get(\"../swagger/v1/swagger.json\");\n        var dataSource = await Metadata.getDatasource();\n        var grid = new GridWidget(container, { data: dataSource, pageSize: 1000, group: { field: \"Category\", dir: \"asc\"} });\n        grid.editable = false;\n        grid.columns = [\n            { field: \"Category\", title: \"[resource_displayname[context]]\", width: 200 },\n            { field: \"DisplayName\", title: \"[resource_displayname[displayname]]\", width: 200 },\n            { field: \"url\", title: \"[resource_displayname[url]]\", template: \"#:session.apiRoot##:Category#/#:DisplayName#\" }\n        ];\n\n        grid.detailTemplate = $(\"script[name=metadataDetails]\", container).first().html();\n        grid.detailExpand = async function(e) {\n            var container = $(e.detailRow);\n            var meta = this.dataItem(e.masterRow);\n            if($(\".k-tabstrip\", container).length == 0) {\n                $(\"[name=tabs]\", container).kendoTabStrip({ animation: { open: { effects: \"fadeIn\" } } });\n                var editor = new MonacoEditor($(\"[name=rawJson]\", container)[0], {\n                    code: JSON.stringify(meta, null, '\\t'),\n                    language: \"json\",\n                    automaticLayout: true\n                });\n                editor.init();\n                await Metadata.setupMetaPropertiesGrid(container, meta);\n                await Metadata.setupEndpoints(container, meta, swaggerDefinition.paths, app);\n                await Metadata.setupDependencies(container, meta);\n            }\n        };\n        await grid.init();\n\n    },\n\n    getDatasource: async function() {\n        var data = [];\n        var allMeta = await api.get(\"Getmetadata\");\n\n        var contexts = allMeta.filter(i => i.UriBase);\n        contexts.forEach((context) => {\n            var entities = context.Types.filter(t => t.IsEntity && t.HasEndpoint).sort((t1, t2) => t1.DisplayName > t2.DisplayName ? 1 : -1);\n            entities.forEach((item) => {\n                item.id = item.Category + '/' + item.DisplayName;\n                item.testStatus = \"loading\";\n                item.isUp = \"loading\";\n                data.push(item);\n            })\n        });\n\n        window.allMeta = data;;\n\n        return data;\n    },\n\n    setupMetaPropertiesGrid: async function (container, meta) {\n        var propertyGrid = new GridWidget($(\"[name=properties]\", container), { data: meta.Properties, pageSize: 50 });\n        propertyGrid.exports = false;\n        propertyGrid.editable = false;\n        propertyGrid.groupable = false;\n\n        propertyGrid.columns = [\n            { field: \"Name\", title: \"[resource_displayname[name]]\" },\n            { field: \"Type\", title: \"[resource_displayname[type]]\" },\n            { field: \"ServerType\", title: \"[resource_displayname[servertype]]\" },\n            { field: \"ServerTypeName\", title: \"[resource_displayname[servertypename]]\" },\n            { field: \"Template\", title: \"[resource_displayname[template]]\" },\n            { field: \"DisplayName\", title: \"[resource_displayname[displayname]]\" },\n            { field: \"ShortDisplayName\", title: \"[resource_displayname[shortdisplayname]]\" },\n            { field: \"Description\", title: \"[resource_displayname[description]]\" },\n            { field: \"IsGeneric\", title: \"[resource_displayname[isgeneric]]\", type: \"boolean\" },\n            { field: \"IsValueType\", title: \"[resource_displayname[isvaluetype]]\", type: \"boolean\" },\n            { field: \"IsReadOnly\", title: \"[resource_displayname[isreadonly]]\", type:\"boolean\" },\n            { field: \"IsRequired\", title: \"[resource_displayname[isrequired]]\", type: \"boolean\" }\n        ];\n        await propertyGrid.init();\n    },\n\n    setupEndpoints: async function(container, meta, swaggerPaths, app) {\n        var applicableEndpointKeys = Object.keys(swaggerPaths).filter(r => r.indexOf(\"/Api/\" + meta.id) !== -1);\n        var applicableEndpoints = applicableEndpointKeys.map(endpointURL => \n            Metadata.getHttpMethods(swaggerPaths[endpointURL]).map(httpMethodName => ({\n                \"endpoint\": endpointURL,\n                \"data\": swaggerPaths[endpointURL][httpMethodName.toLowerCase()],\n                \"method\": httpMethodName\n            }))).flat();\n\n        var endpointGrid = new GridWidget($(\"[name=endpoints]\", container), { data: applicableEndpoints, pageSize: 20 });\n        endpointGrid.groupable = false;\n        endpointGrid.columns = [\n            { field: \"method\", title: \"[resource_displayname[methods]]\", template: \"#=method#\", width: 80 },\n            { field: \"endpoint\", title: \"[resource_displayname[endpoint]]\", template: \"#=endpoint#\" },\n        ];\n        endpointGrid.dataBound = function() {\n            $(\"[name=explore]\", endpointGrid.gridElement).on(\"click\", (e) => Metadata.explore(e, app, endpointGrid));\n        };\n\n        endpointGrid.commands.push({name: \"explore\", icon: \"k-i-search\", text: \"[resource_displayname[explore]]\" });\n\n        await endpointGrid.init();\n    },\n\n    setupDependencies: async function(container, meta) {\n        var foreignKeys = meta.Properties.filter(p => p.Name.endsWith(\"Id\") && p.Name != \"Id\" &&  \n            meta.Properties.filter(p2 => p2.Name == p.Name.substring(0, p.Name.length - 2) && p2.Type == \"object\").length > 0);\n\n        foreignKeys.map(fk => {\n            var object = meta.Properties.filter(p2 => p2.Name == fk.Name.substring(0, fk.Name.length - 2) && p2.Type == \"object\")[0];\n            var serverTypeInformation = object.ServerType;\n            var entity = window.allMeta.filter(metaEntry => metaEntry.ServerType.startsWith(serverTypeInformation))[0];\n            fk.RelatedEntity = entity;\n        });\n\n        var dependsOnGrid = new GridWidget($(\"[name=dependson]\", container), { data: foreignKeys, pageSize: 50 });\n        dependsOnGrid.exports = false;\n        dependsOnGrid.editable = false;\n        dependsOnGrid.columns = [\n            { field: \"Name\", title: \"[resource_displayname[foreignkey]]\" },\n            { field: \"Type\", title: \"[resource_displayname[type]]\"},\n            { field: \"IsRequired\", type:\"boolean\", title: \"[resource_displayname[isrequired]]\"},\n            { field: \"ServerType\", title: \"[resource_displayname[servertype]]\"},\n            { field: \"RelatedEntity.id\", title: \"[resource_displayname[relatedentity]]\"}\n        ];\n        dependsOnGrid.init();\n    },\n\n    explore: async function(e, app, endpointGrid) {\n        e.preventDefault();\n        var item = endpointGrid.dataItem($(e.currentTarget).closest(\"tr\"));\n\n        var exploreDialog = new Dialog({title: \"[resource_displayname[explore]]\", width: 1900, height: 805 });\n        exploreDialog.init(async () => {\n            var apiTesterComponent = await loadComponent(exploreDialog.element, \"ApiTester\");\n            var endpointPath = item.endpoint.substring(item.endpoint.indexOf(\"/Api/\")+5);\n            if(item.data.parameters && item.data.parameters.filter(r => r.name == \"queryOptions\" && r.schema[\"$ref\"].indexOf(\"ODataQueryOptions\") !== -1).length > 0) {\n                endpointPath += \"?$top=10\";\n            }\n\n            apiTesterComponent.init(app, $(\".component[name=ApiTester]\", exploreDialog.element), item.method, endpointPath);\n        });\n    },\n\n    getHttpMethods: function(swaggerPathEntry) {\n        return Object.keys(swaggerPathEntry).map(o => o.toUpperCase());\n    }\n};\n\n\n",
  "Content": "<script name=\"metadataDetails\" type=\"template\">\n   <div name=\"tabs\">\n      <ul>\n      \t  <li class=\"k-active\">[resource_displayname[rawjson]]</li>\n          <li>[resource_displayname[properties]]</li>\n          <li>[resource_displayname[endpoints]]</li>\n          <li>[resource_displayname[dependson]]</li>\n      </ul>\n      <div class=\"tab\" name=\"rawJson\"></div>\n      <div class=\"tab\" name=\"properties\"></div>\n      <div class=\"tab\" name=\"endpoints\"></div>\n      <div class=\"tab\" name=\"dependson\"></div>\n   </div>\n</script>",
  "LastUpdated": "2026-05-05T14:31:12.0958104+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "Notifications",
  "Key": "Theming",
  "ResourceKey": "CMS",
  "Script": "Notifications = {\n    init: async function(app, container, observable) {\n        app = app || session.app;\n        container = container || $(\".component[name=Notifications]\");\n\n        if(!observable)\n            return;\n\n        kendo.bind(container, observable);\n\n        $(\"[name=notifications-picker]\", container).kendoColorPicker({\n            buttons: false,\n            views: [\"gradient\"],\n            change: (e) => e.preventDefault()\n        });\n    }\n}",
  "Content": "<div class=\"row\">\n    <div class=\"col-md-6\">\n        <div class=\"input-group input-group-sm mb-1\">\n            <span class=\"input-group-text\">[resource_displayname[notificationsErrorText]]</span>\n            <input class=\"form-control\" name=\"notifications-picker\" data-bind=\"value: colours.primary\" />\n        </div>\n\n        <div class=\"input-group input-group-sm mb-1\">\n            <span class=\"input-group-text\">[resource_displayname[notificationsWarningText]]</span>\n            <input class=\"form-control\" name=\"notifications-picker\" data-bind=\"value: notifications.warning.text\" />\n        </div>\n        \n        <div class=\"input-group input-group-sm mb-1\">\n            <span class=\"input-group-text\">[resource_displayname[notificationsInfoText]]</span>\n            <input class=\"form-control\" name=\"notifications-picker\" data-bind=\"value: notifications.info.text\" />\n        </div>\n        \n        <div class=\"input-group input-group-sm mb-1\">\n            <span class=\"input-group-text\">[resource_displayname[notificationsSuccessText]]</span>\n            <input class=\"form-control\" name=\"notifications-picker\" data-bind=\"value: notifications.success.text\" />\n        </div>\n    </div>\n    <div class=\"col-md-6\">\n        <div class=\"input-group input-group-sm mb-1\">\n            <span class=\"input-group-text\">[resource_displayname[notificationsErrorBackground]]</span>\n            <input class=\"form-control\" name=\"notifications-picker\" data-bind=\"value: notifications.error.background\" />\n        </div>\n        \n        <div class=\"input-group input-group-sm mb-1\">\n            <span class=\"input-group-text\">[resource_displayname[notificationsWarningBackground]]</span>\n            <input class=\"form-control\" name=\"notifications-picker\" data-bind=\"value: notifications.warning.background\" />\n        </div>\n        \n        <div class=\"input-group input-group-sm mb-1\">\n            <span class=\"input-group-text\">[resource_displayname[notificationsInfoBackground]]</span>\n            <input class=\"form-control\" name=\"notifications-picker\" data-bind=\"value: notifications.info.background\" />\n        </div>\n        \n        <div class=\"input-group input-group-sm mb-1\">\n            <span class=\"input-group-text\">[resource_displayname[notificationsSuccessBackground]]</span>\n            <input class=\"form-control\" name=\"notifications-picker\" data-bind=\"value: notifications.success.background\" />\n        </div>\n    </div>\n</div>",
  "LastUpdated": "2024-11-19T18:18:31.0978632+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "PageInfo",
  "Key": "Content Management",
  "ResourceKey": "CMS",
  "Script": "PageInfo = {\n\tinit: async function (app, container, page) {\n\t\tapp = app || session.app;\n\t\tcontainer = container || $(\".component[name=PageInfo]\");\n\n\t\tif(!page) { return; }\n\n        api.addToMetaCache([\n        {\n            \"Name\": \"ContentManagement\",\n            \"Types\": [\n                [meta[ContentManagement/PageInfo]]\n            ]\n        },\n        {\n            \"Name\": \"Core\",\n            \"Types\": [\n                [meta[ContentManagement/Culture]]\n            ]\n        }]);\n\n\t\tvar cultures = model.getDatasource({\n\t\t\tendpoint: \"ContentManagement/Culture\",\n\t\t\tsort: { field: \"Name\", dir: \"asc\"}\n\t\t});\n\t\t\n\t\tvar pageInfos = await model.getDatasource({\n\t\t\tendpoint: \"ContentManagement/PageInfo\", \n\t\t\todataAppend: \"?$filter=PageId eq \" + page.Id\n\t\t});\n\n\t\tvar grid = new GridWidget(container, pageInfos);\n\t\tgrid.groupable = false;\n\t\tgrid.pageable = false;\n\t\tgrid.toolbar = `<button class=\"btn\" name=\"create\"><span class=\"k-icon k-i-plus\"></span>[resource_displayname[new]]</button>`;\n\t\tgrid.columns = [\n\t\t\t{ \n\t\t\t\tfield: \"CultureId\",\n\t\t\t\t title: \"[resource_displayname[culture]]\", \t\t\t\t\n\t\t\t\t editor: (container, options) => $('<input name=\"' + options.field + '\"/>').appendTo(container).kendoDropDownList({ autoBind: false, dataTextField: \"Name\", dataValueField: \"Id\", dataSource: cultures })\n\t\t\t},\n\t\t\t{ field: \"Title\", title: \"[resource_shortdisplayname[title]]\" },\n\t\t\t{ field: \"Description\", title: \"[resource_shortdisplayname[description]]\" },\n\t\t\t{ field: \"Keywords\", title: \"[resource_shortdisplayname[keywords]]\" }\n\t\t];\n\t\tgrid.dataBound = function() {\n\t\t\t$(\"[name=save]\", grid.gridElement).off(\"click\").on(\"click\", (e) => PageInfo.save(e, grid));\n\t\t\t$(\"[name=delete]\", grid.gridElement).off(\"click\").on(\"click\", (e) => PageInfo.destroy(e,grid));\n\t\t};\n\n\t\tgrid.commands.push({name: \"save\", icon: \"k-i-save\", text: \"[resource_displayname[save]]\"});\n\t\tgrid.commands.push({name: \"delete\", icon: \"k-i-trashIcon\", text: \"[resource_displayname[delete]]\"});\n\n\t\tawait grid.init();\n\n\t\t$(\"[name=create]\", grid.gridElement).on(\"click\", (e) => PageInfo.create(e, app, page, grid));\n\t},\n\n\tcreate: async function (e, app, page, grid) {\n\t\te.preventDefault();\n\n\t\tvar defaultCulture = grid.dataSource().data().filter(r => r.CultureId === \"\");\n\n\t\tvar newPI = await model.item.createInstance(\"ContentManagement/PageInfo\");\n\t\tnewPI.PageId = page.Id;\n\t\tif(defaultCulture.length > 0) {\n\t\t\tnewPI.Title = defaultCulture[0].Title;\n\t\t\tnewPI.Keywords = defaultCulture[0].Keywords;\n\t\t\tnewPI.Description = defaultCulture[0].Description;\n\t\t} else {\n\t\t\tnewPI.Title = page.Name;\n\t\t\tnewPI.Keywords = page.Name;\n\t\t\tnewPI.Description = page.Name;\n\t\t}\n\t\tawait newPI.save().then(() => {\n\t\t\tnotification.success(\"[resource_displayname[added]]\");\n\t\t\tgrid.refresh();\n\t\t}).catch((err) => error(err));\n\t},\n\n\tdestroy: async function (e, grid) {\n\t\tvar dataItem = grid.dataItem($(e.target).closest(\"tr\"));\n\t\tawait dataItem.destroy(e);\n\n\t\tnotification.success(\"[resource_displayname[deleted]]\");\n\t\tgrid.refresh();\n\t},\n\n\tsave: async function (e, grid) {\n\t\te.preventDefault();\n\t\tvar dataItem = grid.dataItem($(e.target).closest(\"tr\"));\n\t\tawait dataItem.save();\n\n\t\tnotification.success(\"[resource_displayname[updated]]\");\n\t\tgrid.refresh();\n\t}\n};",
  "Content": "",
  "LastUpdated": "2024-11-19T18:18:31.1766255+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "PageProperties",
  "Key": "Content Management",
  "ResourceKey": "CMS",
  "Script": "PageProperties = {\n\tinit: async function (app, container, page, saveAction) {\n\t\tapp = app || session.app;\n\t\tcontainer = container || $(\".component[name=PageProperties]\");\n\t\tsaveAction = saveAction || function() { page.save(); };\n\n\t\tif (!page) { return; }\n\n        api.addToMetaCache([\n        {\n            \"Name\": \"Core\",\n            \"Types\": [\n                [meta[ContentManagement/Layout]]\n            ]\n        }]);\n\n\t\tvar layouts = await model.getDatasource({\n\t\t\tendpoint: \"ContentManagement/Layout\",\n\t\t\todataAppend: \"?$filter=AppId eq \" + app.Id + \"&$select=Name\"\n\t\t});\n\n\t\t$(\"input[name=layout]\", container).kendoDropDownList({ \n\t\t\tdataTextField: \"Name\", \n\t\t\tdataValueField: \"Name\", \n\t\t\tdataSource: layouts, \n\t\t\tvalue: page.Layout || \"Default\",\n\t\t\tchange: function(e) {\n\t\t\t\tpage.Layout = this.value();\n\t\t\t\tsaveAction(e);\n\t\t\t}\n\t\t });\n\n\t\t$(container).css(\"width\", \"100%\");\n\t\t\n\t\t$(\"input[name=showOnMenus]\", container).prop(\"checked\", page.ShowOnMenus);\n\t\t$(\"input[name=resourceKey]\", container).val(page.ResourceKey);\n\n\t\t$(\"input[name=showOnMenus]\", container).on(\"click\", (e) => {\n\t\t\tpage.ShowOnMenus = $(\"input[name=showOnMenus]\", container).is(\":checked\");\n\t\t\tsaveAction(e);\n\t\t});\n\t\t$(\"input[name=resourceKey]\", container).on(\"click\", (e) => {\n\t\t\tpage.ResourceKey = $(\"input[name=resourceKey]\", container).val();\n\t\t\tsaveAction(e);\n\t\t});\n        $(\"[name=tabs]\", container).kendoTabStrip({ animation: { open: { effects: \"fadeIn\" } } });\n\t\t\n\t\tPageInfo.init(app, $(\".component[name=PageInfo]\", $(\".tab[name=information]\", container)), page);\n\t}\n};",
  "Content": "   <div name=\"tabs\">\n      <ul>\n         <li class=\"k-active\">\n\t\t\t <span class=\"k-icon k-i-gearsIcon\"></span>[resource_displayname[general]]\n\t\t</li>\n         <li>\n\t\t\t <span class='k-icon k-i-infoSolidIcon'></span>[resource_displayname[information]]\n\t\t</li>\n      </ul>\n      <div class=\"tab\" name=\"general\">\n\t\t<ul class=\"fieldList\">\n\t\t\t<li name=\"Layout\">\n\t\t\t\t<label>[resource_displayname[layout]]</label>\n\t\t\t\t<div class=\"value\">\n\t\t\t\t\t<input type=\"custom\" name=\"layout\" />\n\t\t\t\t</div>\n\t\t\t</li>\n\t\t\t<li name=\"ShowOnMenus\">\n\t\t\t\t<label>[resource_displayname[showonmenus]]</label>\n\t\t\t\t<div class=\"value\">\n\t\t\t\t\t<input type=\"checkbox\" name=\"showOnMenus\" />\n\t\t\t\t</div>\n\t\t\t</li>\n\t\t\t<li name=\"ResourceKey\">\n\t\t\t\t<label>[resource_displayname[resourcekey]]</label>\n\t\t\t\t<div class=\"value\">\n\t\t\t\t\t<input type=\"text\" name=\"resourceKey\" />\n\t\t\t\t</div>\n\t\t\t</li>\n\t\t</ul>\n\t</div>\n      <div class=\"tab\" name=\"information\" >\n\t\t  [component[PageInfo]]\n\t  </div>\n   </div>\n",
  "LastUpdated": "2024-11-19T18:18:31.1651641+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "PortalBuilderForm",
  "Key": "CMS",
  "ResourceKey": "CMS",
  "Script": "PortalBuilderForm = {\n    init: async function (app, container) {\n        if (!app) { app = session.app; }\n        if (!container) { container = $(\".component[name=PortalBuilderForm]\"); }\n\n        var appCreationForms = (await api.get(\"ContentManagement/Component?$filter=AppId eq \" + app.Id + \" and Category eq 'App Creation'\")).value;\n        $(\"[name=AppType]\", container).kendoDropDownList({\n            optionLabel: \"[resource_displayname[CreationFormSelect]]\",\n            dataTextField: \"Name\",\n            dataValueField: \"Name\",\n            change: function (e) {\n                var value = this.value();\n                loadComponent($(\"[name=AppCreationForm]\", container), value, function (componentObject) {\n                    componentObject.init(app, $(\"[name=AppCreationForm]\", container));\n                });\n            },\n            dataSource: {\n                data: appCreationForms\n            }\n        });\n    }\n}",
  "Content": "<div class=\"container\">\n\t<h3>[resource_displayname[AppType]]</h3>\n\t<ul class=\"fieldList\">\n\t\t<li>\n\t\t\t<label>[resource_displayname[AppType]]</label>\n\t\t\t<div class='value'>\n\t\t\t\t<input type=\"text\" name=\"AppType\">\n        </div>\n\t\t</li>\n\t</ul>\n</div>\n<div name=\"AppCreationForm\" style=\"margin-bottom:50px;\"></div>\n<style type=\"text/css\">\n[name=ThemeBuilder] { background-color:white;margin:0px; float: initial; border:0px; }\n[name=DebtorPortal] { margin:0px; float: initial; border:0px; }\n</style>",
  "LastUpdated": "2024-06-10T14:56:38.3625434+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "QueryBuilder",
  "Key": "Tools",
  "ResourceKey": "QueryBuilder",
  "Script": "QueryBuilder = {\n\tdefaultGridHeight: 300,\n\tinit: function (app, container, dmsPath) {\n        if(!app) { app = session.app; }\n        if (!container) { container = $(\".component[name=QueryBuilder]\"); }\n        if(!dmsPath) { dmsPath = \"Data/QueryBuilder/\" + session.user.Id; }\n       \n\t\t// pass api root to the api root label on the page\n\t\t$(\"span[name=apiRoot]\").text(session.apiRoot);\n\n\t\t// fetch a list of contexts\n\t\tapi.get('', function (data) {\n            $('[name=contextList]', container).kendoDropDownList({\n                dataTextField: \"name\",\n                dataValueField: \"url\",\n                dataSource: data.value,\n                index: 0,\n                change: function (e) { QueryBuilder.onContextChange(container); }\n\t\t\t});\n\n\t\t\t// raise selection changed handler to build next portion\n\t\t\tQueryBuilder.onContextChange(container);\n\t\t});\n\n        $('button[name=build]', container).on('click', function(e) { QueryBuilder.build(app, container, dmsPath); });\n    },\n   \n    build: function(app, container, dmsPath) {\n            $(document.body).append($(\"<div name='componentBuilder'>\" + $(\"[name=generatedComponent]\", container).html() + \"</div>\"));\n            var dialog = $(\"[name=componentBuilder]\").kendoWindow({\n\t\t\t\ttitle: \"[resource_displayname[ComponentDetails]]\",\n\t\t\t\tmodal: true,\n                visible: false,\n                width: \"80%\",\n\t\t\t\tclose: function(){ this.destroy(); $(\"body > [name=componentBuilder]\").remove(); }\n            });\n           \n            var config = QueryBuilder.buildGridConfiguration(container);\n            var markup = \"<div name='query'></div>\";\n            var code = \n                  \"NewComponent = {\\n\" +\n                  \"    init: function(app, container) {\\n\" +\n                  \"          if(!app) { app = session.app; }\\n\" +\n                  \"          if(!container) { container = $('.component[name=NewComponent]'); }\\n\" +\n                  \"          model.prepConfigForGrid($('div[name=query]', container), NewComponent.getConfig(), function(c) {\\n\" + \n                  \"                  $('[name=grid]', container).kendoGrid(c);\\n\" + \n                  \"           });\\n\" +\n                  \"     },\\n\" +\n                  \"     getConfig: function() {\\n\" +\n                  \"          return JSON.parse(\" + JSON.stringify(config, null, '    ') + \");\\n\" +\n                  \"      }\\n\" +\n                  \"}\\n\" + \n                  \"$(function() { NewComponent.init(); });\";\n           \n           $(\"pre[name=html]\", dialog).text(markup);\n\t\t   $(\"pre[name=script]\", dialog).text(code);\n\t\t   dialog.data(\"kendoWindow\").center().open();\n       \n       \t   $(\"button[name=create]\", dialog).on(\"click\", function(e) {\n                 model.item.createInstance(\"ContentManagement/Component\", function(component) {\n                    component.AppId = app.Id;\n                    component.Name = $(\"input[name=name]\", dialog).val();\n                    component.Description = $(\"input[name=description]\", dialog).val();\n                    component.Category = $(\"input[name=category]\", dialog).val();\n\t\t\t\t\tcomponent.Html =  $(\"pre[name=html]\", dialog).text();\n                    component.Script = $(\"pre[name=script]\", dialog).text();\n                    component.save(function(res) {\n                       notification.success(\"[resource_description[ComponentCreated]]\");\n                    });\n                 });\n           });\n       \n       \t   $(\"button[name=saveConfig]\", dialog).on(\"click\", function(e) {\n                 api.add(\"DMS/\" + path, function(res) {\n                      notification.success(\"[resource_description[ConfigurationSaved]]\");\n              \t });\n           });\n    },\n\n\tonContextChange: function (container) {\n\t\tapi.get($('[name=contextList]', container).val(), function (data) {\n            var endpointList = $('[name=endpointList]', container);\n\n\t\t\t// remove the old if there is one\n\t\t\tvar oldCtrl = endpointList.data(\"kendoDropDownList\");\n\t\t\tif (oldCtrl) {\n\t\t\t\toldCtrl.destroy();\n\t\t\t\tendpointList.empty();\n\t\t\t}\n\n\t\t\t// buld the new\n            endpointList.kendoDropDownList({\n                dataTextField: \"name\",\n                dataValueField: \"url\",\n                dataSource: data.value,\n                index: 0,\n                change: function (e) { QueryBuilder.onEndpointChange(container); }\n\t\t\t});\n\n\t\t\t// raise selection changed handler to build next portion\n\t\t\tQueryBuilder.onEndpointChange(container);\n\t\t});\n\t},\n\n\t// for when endpoint selection is changed\n\tonEndpointChange: function (container) {\n\t\tvar ctxList = $('[name=contextList]', container);\n\t\tvar endpointList = $('[name=endpointList]', container);\n\n\t\tlog('endpoint selected ' + ctxList.val() + \"/\" + endpointList.val(), 'debug');\n\t\ttype.get(ctxList.val() + '/' + endpointList.val(), function (meta) {\n\t\t\tlog(meta, 'debug');\n\t\t\tQueryBuilder.buildSelectGrid(meta, container);\n\t\t\tQueryBuilder.buildExpandGrid(meta, container);\n\t\t\tQueryBuilder.buildFilterGrid(container);\n\t\t\tQueryBuilder.generate(container);\n\t\t});\n\t},\n\n\t// builds the column / selection grid\n\tbuildSelectGrid: function (meta, container) {\n\t\tvar colList = $('[name=columnList]', container);\n\t\tvar count = 0;\n\t\tvar data = JSLINQ(meta.Properties)\n\t\t\t.Select(function (i) {\n\t\t\t\ti.id = i.Name;\n\t\t\t\ti.Selected = type.ofProperty.isScalar(i);\n\t\t\t\ti.DisplayOrder = count++;\n\t\t\t\ti.Width = 0;\n\t\t\t\ti.TemplateSource = QueryBuilder.buildDefaultTemplateSourceFor(i);\n\n\t\t\t\treturn i;\n\t\t\t})\n\t\t\t.ToArray();\n\n\t\tvar oldCtrl = colList.data(\"kendoGrid\");\n\t\tif (oldCtrl) {\n\t\t\toldCtrl.destroy();\n\t\t\tcolList.empty();\n\t\t}\n\n\t\tvar config = {\n\t\t\tscrollable: true,\n\t\t\tsortable: false,\n\t\t\tfilterable: false,\n\t\t\tpageable: false,\n\t\t\teditable: true,\n            height: 300,\n\t\t\ttoolbar: [\"create\"],\n\t\t\tcolumns: [\n\t\t\t\t{ title: \"\", width: 70, template: \"<input name='Selected' type='checkbox' data-bind='checked: Selected' #= data.Selected ? checked='checked' : '' #/>\" },\n\t\t\t\t{ field: 'DisplayOrder', title: '[resource_shortdisplayname[DisplayOrder]]', width: 60 },\n\t\t\t\t{ field: 'DisplayName', title: '[resource_shortdisplayname[DisplayName]]', width: 200 },\n\t\t\t\t{ field: 'Width', title: '[resource_shortdisplayname[Width]]', width: 100 },\n\t\t\t\t{ field: 'TemplateSource', title: '[resource_shortdisplayname[TemplateSource]]' }\n            ],\n            dataSource: JSLINQ(data).Where(function (c) { return type.ofProperty.isScalar(c); }).ToArray()\n\t\t};\n\t\tcolList.kendoGrid(config);\n        $(\"input\", colList).on('change', function (e) { QueryBuilder.generate(cotnainer); });\n\t\t$(\"input[name=Selected]\", colList).off('change');\n\n\t\tcolList.on(\"change\", \"input[name=Selected]\", function (e) {\n\t\t\tvar g = colList.data(\"kendoGrid\");\n\t\t\tvar row = $(e.target).closest(\"tr\");\n\t\t\tvar item = g.dataItem(row);\n\t\t\titem.Selected = $(e.target).is(\":checked\") ? true : false;\n\t\t\tQueryBuilder.generate(container);\n\t\t});\n\t},\n\n\t// builds the fetch / expansion grid\n\tbuildExpandGrid: function (meta, container) {\n        var expandList = $('[name=expandList]', container);\n\t\tvar data = JSLINQ(meta.Properties)\n\t\t\t.Select(function (i) {\n\t\t\t\ti.id = i.Name;\n\t\t\t\ti.parentId = null;\n\t\t\t\ti.isScalar = type.ofProperty.isScalar(i);\n\t\t\t\ti.Selected = false;\n\t\t\t\ti.TemplateSource = QueryBuilder.buildDefaultTemplateSourceFor(i);\n\t\t\t\treturn i;\n\t\t\t})\n\t\t\t.Where(function (i) { return !i.isScalar; })\n\t\t\t.ToArray();\n\n\t\t// ifthere's a previous grid, rip it out\n\t\tvar oldGrid = expandList.data(\"kendoTreeList\");\n\t\tif (oldGrid) {\n\t\t\toldGrid.destroy();\n\t\t\texpandList.empty();\n\t\t}\n\t\t// build config for the new grid\n\t\tvar config = {\n\t\t\tscrollable: true,\n\t\t\tsortable: false,\n\t\t\tfilterable: false,\n\t\t\tpageable: false,\n\t\t\teditable: true,\n            height: 300,\n\t\t\tcolumns: [\n\t\t\t\t{ title: '', width: 100, template: \"<span class='k-icon #:isScalar ? '' : 'k-i-expand' # not-loaded'>&nbsp;</span>\" },\n\t\t\t\t{ title: \"\", width: 70, template: \"<input name='Selected' type='checkbox' data-bind='checked: Selected' #= data.Selected ? checked='checked' : '' #/>\" },\n\t\t\t\t{ field: 'Name', title: '[resource_shortdisplayname[Name]]' },\n\t\t\t\t{ field: 'Type', title: '[resource_shortdisplayname[Type]]' }\n            ],\n\t\t\tdataSource: new kendo.data.TreeListDataSource({\n\t\t\t\tdata: data,\n\t\t\t\tschema: { model: { id: \"id\", expanded: true } }\n\t\t\t})\n\t\t};\n\n\t\t// tell kendo to build it\n\t\texpandList.kendoTreeList(config);\n\n\t\t// hook up input element value changes to handle re-building the result automatically when user \"changes stuff\"\n\t\t$(\"input\", expandList).on('change', function(e) { QueryBuilder.generate(container); });\n\t\t$(\"input[name=Selected]\", expandList).off('change');\n\n\t\texpandList.on(\"change\", \"input[name=Selected]\", function (e) {\n\t\t\tvar g = expandList.data(\"kendoTreeList\");\n\t\t\tvar row = $(e.target).closest(\"tr\");\n\t\t\tvar item = g.dataItem(row);\n\t\t\titem.Selected = $(e.target).is(\":checked\") ? true : false;\n\t\t\tQueryBuilder.generate(container);\n\t\t});\n\n\t\texpandList.off('click', '.k-i-expand.not-loaded');\n\t\texpandList.on('click', '.k-i-expand.not-loaded', function (e) {\n\t\t\tvar source = expandList.data(\"kendoTreeList\");\n\t\t\tvar selectedItem = source.dataItem($(e.currentTarget).closest('tr'));\n            var typeDefParts = selectedItem.ServerType.split(',')[0].split('.');\n            var endpoint = $('[name=contextList]', container).val() + '/' + typeDefParts[typeDefParts.length-1];\n\t\t\ttype.get(endpoint, function (meta) {\n\t\t\t\tvar newData = JSLINQ(meta.Properties)\n\t\t\t\t\t.Select(function (i) {\n\t\t\t\t\t\ti.id = selectedItem.id + \"/\" + i.Name;\n\t\t\t\t\t\ti.parentId = selectedItem.id;\n\t\t\t\t\t\ti.isScalar = type.ofProperty.isScalar(i);\n\t\t\t\t\t\ti.Selected = false;\n\t\t\t\t\t\ti.TemplateSource = QueryBuilder.buildDefaultTemplateSourceFor(i);\n\t\t\t\t\t\treturn i;\n\t\t\t\t\t})\n\t\t\t\t\t.ToArray();\n\n\t\t\t\tfor (var i in newData) { source.dataSource.add(newData[i]); }\n\n\t\t\t\t$(e.currentTarget).remove();\n\t\t\t\tsource.expand(selectedItem);\n\t\t\t\tQueryBuilder.buildFilterGrid(container);\n\t\t\t\tQueryBuilder.generate(container);\n\t\t\t});\n\t\t});\n\t},\n\n\tbuildFilterGrid: function (container) {\n        var colList = $('[name=columnList]', container);\n        var expandList = $('[name=expandList]', container);\n        var filterList = $('[name=filterList]', container);\n\t\tvar colOptions = JSLINQ(expandList.data(\"kendoTreeList\").dataSource.data().toJSON())\n\t\t\t.Where(function (c) { return c.Selected && type.ofProperty.isScalar(c); })\n\t\t\t.Concat(JSLINQ(colList.data(\"kendoGrid\").dataSource.data().toJSON()).ToArray())\n\t\t\t.ToArray();\n\n\t\t// http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part2-url-conventions/odata-v4.0-errata03-os-part2-url-conventions-complete.html#_Toc453752358\n\t\tvar conditionOptions = [\n\t\t\t{ id: 'eq',\ttext: 'is equal to' },\n\t\t\t{ id: 'neq',text: 'is not equal to' },\n\t\t\t{ id: 'gt', text: 'is greater than' },\n\t\t\t{ id: 'lt', text: 'is less than' },\n\t\t\t{ id: 'le', text: 'is less than or equal to' },\n\t\t\t{ id: 'ge', text: 'is greater than or equal to' }\n        ];\n\n\t\t// if there's a previous grid, rip it out\n\t\tvar oldGrid = filterList.data(\"kendoGrid\");\n\t\tif (oldGrid) {\n\t\t\toldGrid.destroy();\n\t\t\tfilterList.empty();\n\t\t}\n\n\t\tvar fieldEditor = function (container, options) {\n\t\t\t$(\"<input name='field' />\")\n\t\t\t\t.appendTo(container)\n\t\t\t\t.kendoDropDownList({\n\t\t\t\t\tdataTextField: \"id\",\n\t\t\t\t\tdataValueField: \"id\",\n\t\t\t\t\tdataSource: colOptions,\n\t\t\t\t\tindex: 0,\n\t\t\t\t\tselect: function (e) {\n\t\t\t\t\t\tvar grid = $(filterList).data(\"kendoGrid\");\n\t\t\t\t\t\tvar row = $(e.sender.element).closest('tr');\n\t\t\t\t\t\tvar filter = grid.dataItem(row);\n\t\t\t\t\t\tfilter.meta = this.dataItem(e.item ? e.item.index() : e.sender.selectedIndex);\n\t\t\t\t\t\tfilter.field = filter.meta.id;\n\t\t\t\t\t}\n\t\t\t\t});\n\n\t\t\t$(\"[name=field]\", container).data(\"kendoDropDownList\").trigger(\"select\");\n\t\t};\n\n\t\tvar conditionEditor = function (container, options) {\n\t\t\t$(\"<input name='condition' />\")\n\t\t\t\t.appendTo(container)\n\t\t\t\t.kendoDropDownList({\n\t\t\t\t\tdataTextField: \"text\",\n\t\t\t\t\tdataValueField: \"id\",\n\t\t\t\t\tdataSource: conditionOptions,\n\t\t\t\t\tindex: 0,\n\t\t\t\t\tselect: function (e) {\n\t\t\t\t\t\tvar grid = $(filterList).data(\"kendoGrid\");\n\t\t\t\t\t\tvar row = $(e.sender.element).closest('tr');\n\t\t\t\t\t\tvar filter = grid.dataItem(row);\n\t\t\t\t\t\tfilter.operator = this.dataItem(e.item ? e.item.index() : e.sender.selectedIndex).id;\n\t\t\t\t\t}\n\t\t\t\t});\n\n\t\t\t$(\"[name=condition]\", container).data(\"kendoDropDownList\").trigger(\"select\");\n\t\t};\n\n\t\t// build config for the new grid\n\t\tvar config = {\n\t\t\tscrollable: true,\n\t\t\tsortable: false,\n\t\t\tfilterable: false,\n\t\t\tpageable: false,\n\t\t\teditable: 'inline',\n\t\t\ttoolbar: [\"create\"],\n\t\t\tcolumns: [\n\t\t\t\t{ field: 'field', title: '[resource_shortdisplayname[Field]]', editor: fieldEditor },\n\t\t\t\t{ field: 'operator', title: '[resource_shortdisplayname[Operator]]', editor: conditionEditor },\n\t\t\t\t{ field: 'Value', title: '[resource_shortdisplayname[Value]]' },\n\t\t\t\t{ command: ['destroy'] }\n            ],\n\t\t\tdataSource: new kendo.data.DataSource({ data: new Array() }),\n            dataBound: function (e) { $(e.sender.element).css('max-height' + QueryBuilder.defaultGridHeight + 'px'); }\n\t\t};\n\n\t\t// tell kendo to build it\n\t\tfilterList.kendoGrid(config);\n\n\t\t// hook up input element value changes to handle re-building the result automatically when user \"changes stuff\"\n        $(filterList).on('change', \"input\", function () { QueryBuilder.generate(container); });\n\t},\n\n\tbuildDefaultTemplateSourceFor: function (col) {\n        if (col.Type === 'date') { return \"#: kendo.toString(new Date(\" + col.Name + \"), 'dd/MM/yyyy') #\"; }\n        else { return \"#: \" + col.Name + \" #\"; }\n\t},\n\n    generate: function (container) {\n        var grid = $('[name=grid]', container);\n        var oldGrid = grid.data(\"kendoGrid\");\n        if (oldGrid) {\n            oldGrid.destroy();\n            grid.empty();\n        }\n        model.prepConfigForGrid( QueryBuilder.buildGridConfiguration(container), function (config) { grid.kendoGrid(config); });\t\t\t\n    },\n\n\tbuildGridConfiguration: function (container) {\n        var ctxList = $('[name=contextList]', container);\n        var endpointList = $('[name=endpointList]', container);\n        var colList = $('[name=columnList]', container);\n        var expandList = $('[name=expandList]', container);\n        var filterList = $('[name=filterList]', container);\n\n\t\tvar cols = JSLINQ(colList.data(\"kendoGrid\").dataSource.data().toJSON())\n\t\t\t.Where(function (c) { return c.Selected; })\n            .ToArray()\n            .sort((a, b) => a.DisplayOrder - b.DisplayOrder);\n\n\t\tvar expands = JSLINQ(expandList.data(\"kendoTreeList\").dataSource.data().toJSON())\n\t\t\t.Where(function (c) { return c.Selected; })\n\t\t\t.ToArray();\n\n\t\tvar query = QueryBuilder.buildQuery(cols, expands, container);\n\t\t$(\"span[name=query]\").text(query);\n\n\t\treturn {\n\t\t\tscrollable: true,\n\t\t\tsortable: true,\n\t\t\tfilterable: true,\n\t\t\tpageable: { refresh: true, pageSizes: true, buttonCount: 5 },\n\t\t\tcolumns: QueryBuilder.buildColumns(cols, expands),\n\t\t\tsourceInitParams: {\n\t\t\t\tendpoint: ctxList.val() + \"/\" + endpointList.val(),\n\t\t\t\todataAppend: query,\n\t\t\t\tfilter: {\n\t\t\t\t\tlogic: 'and',\n\t\t\t\t\t//not sure how to handle complex object or collection filters yet\n\t\t\t\t\t//TODO: figure out complex filtering scenarios and how to handle them\n\t\t\t\t\tfilters: JSLINQ(filterList.data(\"kendoGrid\").dataSource.data())\n\t\t\t\t\t\t.Where(function (f) { return type.ofProperty.isScalar(f.meta); })\n\t\t\t\t\t\t.ToArray()\n\t\t\t\t}\n\t\t\t}\n\t\t};\n\t},\n\n\tbuildColumns: function (selectedCols) {\n\t\treturn JSLINQ(selectedCols).Select(function (col) {\n\t\t\tvar template = col.TemplateSource ? col.TemplateSource : $(\"#\" + col.Template).html();\n\t\t\treturn { field: col.Name, title: \"[resource_shortdisplayname[\" + col.Name + \"]]\", template: kendo.template(template) };\n\t\t}).ToArray();\n\t},\n\n\tbuildQuery: function (selectedCols, expands, container) {\n\t\tvar colList = $('[name=columnList]', container);\n\t\tvar result = '';\n\n\t\t// build selection array\n\t\tvar selects = JSLINQ(selectedCols)\n\t\t\t.Where(function (c) { return type.ofProperty.isScalar(c); })\n\t\t\t.Select(function (c) { return c.Name; })\n\t\t\t.ToArray();\n\n\t\t// clear it if everything is selected (we don't need a sub select in this case)\n\t\tif (JSLINQ(colList.data(\"kendoGrid\").dataSource.data().toJSON()).All(function (c) { return c.Selected; })) {\n\t\t\tselects = new Array();\n\t\t}\n\n\t\tvar select = function (c) {\n\t\t\tvar subSelections = JSLINQ(expands)\n\t\t\t\t.Where(function (c2) { return type.ofProperty.isScalar(c2) && c2.parentId === c.id; })\n\t\t\t\t.Select(select)\n\t\t\t\t.ToArray();\n\n\t\t\tvar subIncludes = JSLINQ(expands)\n\t\t\t\t.Where(function (c2) { return !type.ofProperty.isScalar(c2) && c2.parentId === c.id; })\n\t\t\t\t.Select(select)\n\t\t\t\t.ToArray();\n\n\t\t\tif (subSelections.length === 0 && subIncludes.length === 0) {\n\t\t\t\treturn c.Name;\n\t\t\t} else {\n\t\t\t\tvar result = c.Name + '(';\n\t\t\t\tif (subSelections.length > 0) { result += '$select=' + subSelections.join(); }\n                if (subIncludes.length > 0)   { result += ';$expand=' + subIncludes.join() + ')'; }\n\t\t\t\treturn result + ')';\n\t\t\t}\n\t\t};\n\n\t\t// build an expands array\n\t\texpands = JSLINQ(expands)\n\t\t\t.Where(function (c) { return !type.ofProperty.isScalar(c) && !c.id.includes('/'); })\n\t\t\t.Select(select)\n\t\t\t.ToArray();\n\n\t\t// if we have selections, build out the select part ofthe query\n\t\tif (selects.length > 0) { result += \"?$select=\" + selects.join(); }\n\n\t\t// if we have any expands, build out the expand part of the query\n\t\tif (expands.length > 0) {\n\t\t\tif (selects.length > 0) { result += \"&$expand=\" + expands.join(); }\n            else { result += \"?$expand=\" + expands.join(); }\n\t\t}\n\n\t\t// return the resulting query\n\t\treturn result;\n\t}\n};\n",
  "Content": "<ul class=\"fieldList\">\n   <li class=\"baseUrl\">\n      <label>[resource_displayname[From]]</label>\n      <div class=\"value\">\n         <span name=\"apiRoot\"></span> <input name=\"contextList\" style=\"display: inline;\" /> / <input name=\"endpointList\" style=\"display: inline-block;\" />\n         <span name=\"query\" style=\"display: inline-block; line-height: 20px; margin-bottom: -3px; margin-top: 5px; word-break: break-word; word-wrap: break-word;\"></span>\n      </div>\n   </li>\n   <li>\n      <label>[resource_displayname[Columns]]</label><div class=\"value\"><div name=\"columnList\"></div></div>\n   </li>\n   <li>\n      <label>[resource_displayname[AlsoFetch]]</label><div class=\"value\"><div name=\"expandList\"></div></div>\n   </li>\n   <li>\n      <label>[resource_displayname[AndApplyFilters]]</label><div class=\"value\"><div name=\"filterList\"></div></div>\n   </li>\n   <li>\n      <label></label><div class=\"value\"><button name=\"build\">Component Info</button></div>\n   </li>\n</ul>\n<div style=\"margin: 20px;\">\n   <hr />\n   <div name=\"grid\"></div>\n</div>\n\n<script type=\"text/template\" name=\"generatedComponent\">\n   <ul class=\"fieldList\">\n   <li><label>[resource_displayname[Name]]</label><div class=\"value\"><input name=\"name\" value='NewComponent'></input></div></li>\n   <li><label>[resource_displayname[Description]]</label><div class=\"value\"><input name=\"description\" value='A Query Builder Generated Component'></input></div></li>\n   <li><label>[resource_displayname[Category]]</label><div class=\"value\"><input name=\"category\" value=\"Generated\"></input></div></li>\n   <li>\n      <div class=\"container\"><h4>Content</h4><pre name=\"html\" contenteditable></pre></div>\n      <div class=\"container\"><h4>Script</h4><pre name=\"script\" contenteditable></pre></div>\n   </li>\n</ul>\n<div style=\"margin: 20px; margin-top: -20px;\">\n   <button name=\"create\">[resource_displayname[Create]]</button>\n   <button name=\"saveConfig\">[resource_displayname[SaveconfigToDMS]]</button>\n<div>\n</script>\n\n<style scoped>\n   .fieldList li { margin: 5px 0; }\n   .not-loaded   { margin-left: -15px; cursor: pointer; }\n   [name=componentBuilder] .container { display: inline-block; width: 49%; margin: 0 5px;  }\n   [name=componentBuilder] .container > pre {  margin-top: 5px; height: 300px; overflow-y: auto; }\n   .component[name=QueryBuilder] .fieldList > li  { width: 48%; margin: 5px; display: inline-block; }\n   .component[name=QueryBuilder] .fieldList > li.baseUrl { width: 80%; }\n   .component[name=QueryBuilder] .fieldList > li > label { width: 200px; font-size: 120%; font-weight: bold; margin: 10px; }\n   .component[name=QueryBuilder] .fieldList > li > .value { width: 100%; }\n   pre { padding: 5px 10px; }\n</style>",
  "LastUpdated": "2022-01-07T13:19:44.4073555+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "ResourceManagement",
  "Key": "CMS",
  "ResourceKey": "CMS",
  "Script": "ResourceManagement = {\n\tinit: async function (app, container) {\n\t\tapp = app || session.app;\n\t\tcontainer = container || $(\".component[name=Resourcing]\");\n\t\tapi.addToMetaCache([\n\t\t{\n\t\t\t\t\"Name\": \"Core\",\n\t\t\t\t\"Types\": [\n\t\t\t\t\t[meta[ContentManagement/Resource]]\n\t\t\t\t]\n\t\t}]);\n\t\tvar config = {\n\t\t\tendpoint: \"ContentManagement/Resource\",\n\t\t\todataAppend: \"?$filter=AppId eq \" + app.Id + \" and Culture eq ''\",\n\t\t\tgroup: { field: \"Key\" }\n\t\t};\n\t\tlet cultures = await api.get(\"ContentManagement/Culture\");\n\t\tlet resourceDataSource = await model.getDatasource(config);\n\t\tlet typeInfo = await api.getType(\"ContentManagement/Resource\");\n\t\tawait ResourceManagement.setupGrid(app,container,resourceDataSource, cultures, typeInfo);\n\n\t\tawait loadComponent($('[name=resourceMigrationComponent]', container), 'ResourceMigration', async (c) => {\n\t\t\tawait c.init(app, $('[name=ResourceMigration]', container));\n\t\t});\n\t},\n\n\tsetupGrid: async function(app,container, resourceDataSource, cultures, typeInfo) {\n\t\tvar grid = new GridWidget(container, resourceDataSource);\n\t\tgrid.toolbar = [\n\t\t\t{\n\t\t\t\ttemplate: \"<div class='btn-group btn-group-sm'> \\\n\t<button class='btn btn-primary' name='create'> \\\n\t\t<span class='k-icon k-i-plus'></span>[resource_displayname[new]] \\\n\t</button> \\\n\t<button class='btn btn-primary' name='migrate'> \\\n\t\t<span class='k-icon k-i-arrow-up'></span>[resource_displayname[migrate]] \\\n\t</button> \\\n</div>\"\n\t\t\t}\n\t\t];\n\t\tgrid.filterable = true;\n\t\tgrid.searchable = true;\n\t\tgrid.search = { \n\t\t\tfields: [\"Key\", \"Name\", \"DisplayName\", \"ShortDisplayName\", \"Description\", \"LastUpdatedBy\"] \n\t\t}; \n\n\t\tgrid.columns = [\n\t\t\t{ \n\t\t\t\tfield: \"Key\", \n\t\t\t\teditable: false, \n\t\t\t\ttitle: \"[resource_shortdisplayname[key]]\", \n\t\t\t\twidth: \"[theme[columns.small]]\" \n\t\t\t},\n\t\t\t{ \n\t\t\t\tfield: \"Name\", \n\t\t\t\teditable: false, \n\t\t\t\ttitle: \"[resource_shortdisplayname[name]]\"\n\t\t\t},\n\t\t\t{ \n\t\t\t\tfield: \"DisplayName\", \n\t\t\t\ttitle: \"[resource_shortdisplayname[displayname]]\", \n\t\t\t\ttemplate: \"#=(DisplayName && DisplayName.length > 80) ? ResourceManagement.escapeHtml(DisplayName.substring(0, 80)) + '...' : ResourceManagement.escapeHtml(DisplayName)#\", \n\t\t\t\tencoded: true \n\t\t\t},\n\t\t\t{ \n\t\t\t\tfield: \"ShortDisplayName\", \n\t\t\t\ttitle: \"[resource_shortdisplayname[shortdisplayname]]\", \n\t\t\t\ttemplate: \"#=(ShortDisplayName && ShortDisplayName.length > 80) ? ResourceManagement.escapeHtml(ShortDisplayName.substring(0, 80)) + '...' : ResourceManagement.escapeHtml(ShortDisplayName)#\", \n\t\t\t\tencoded: true, \n\t\t\t\twidth: \"[theme[columns.small]]\" \n\t\t\t},\n\t\t\t{ \n\t\t\t\tfield: \"Description\", \n\t\t\t\ttitle: \"[resource_shortdisplayname[description]]\", \n\t\t\t\ttemplate: \"#=(Description && Description.length > 80) ? ResourceManagement.escapeHtml(Description.substring(0, 80)) + '...' : ResourceManagement.escapeHtml(Description)#\", \n\t\t\t\tencoded: true \n\t\t\t},\n\t\t\t{ \n\t\t\t\tfield: \"LastUpdated\", \n\t\t\t\ttitle: \"[resource_displayname[lastupdated]]\",\n\t\t\t\ttype: \"date\", \n\t\t\t\tformat: \"{0: \" + type.dateFormat + \" HH:mm}\", \n\t\t\t\twidth: \"[theme[columns.small]]\" \n\t\t\t},\n\t\t\t{ \n\t\t\t\tfield: \"LastUpdatedBy\", \n\t\t\t\ttitle: \"[resource_displayname[lastupdated]]\", \n\t\t\t\twidth: \"[theme[columns.small]]\" \n\t\t\t}\n\t\t];\n\n\t\tgrid.commands.push({name: \"save\", icon: \"k-i-save\", text: \"[resource_displayname[save]]\"});\n\t\tgrid.commands.push({name: \"destroy\", icon: \"k-i-trash\", text: \"[resource_displayname[delete]]\"});\n\t\tgrid.detailTemplate = \"<div name='translations'></div>\";\n\t\tgrid.detailExpand = (e) => ResourceManagement.expandResource(e, app, grid, cultures.value, typeInfo);\n\n\t\tgrid.dataBound = () => {\n\t\t\t$(\"button[name=save]\", grid.gridElement).on(\"click\", (e) => ResourceManagement.save(e, grid));\n\t\t\t$(\"button[name=destroy]\", grid.gridElement).on(\"click\", (e) => ResourceManagement.destroy(e, grid));\n\t\t};\n\n\t\tawait grid.init();\n\t\t$(\"button[name=migrate]\", grid.gridElement).on(\"click\", (e) => ResourceManagement.migrate(app));\n\t\t$(\"button[name=create]\", grid.gridElement).on(\"click\", (e) => ResourceManagement.newResource(e, app, grid));\n\t\t$(\"input[name='searchResourcing']\", grid.gridElement).off(\"keyup\").keyup(ResourceManagement.delay((e) => ResourceManagement.search(e, grid, container, app, typeInfo), 500));\n\t},\n\n\tdelay: function (callback, ms) {\n\t\tvar timer = 0;\n\t\treturn function () {\n\t\t\tvar context = this, args = arguments;\n\t\t\tclearTimeout(timer);\n\t\t\ttimer = setTimeout(function () {\n\t\t\t\tcallback.apply(context, args);\n\t\t\t}, ms || 0);\n\t\t};\n\t},\n\n\tsearch: async function (grid, app, typeInfo) {\n\t\tvar searchTerm = $(\"input[name='searchResourcing']\", grid.gridElement).val();\n\t\tlet data = await api.get(\"ContentManagement/Resource?$filter=AppId eq \" + app.Id + \" and Culture eq '' and (contains(Name, '\" + searchTerm + \"') or contains(DisplayName, '\" + searchTerm + \"') or contains(Description, '\" + searchTerm + \"') or contains(ShortDisplayName, '\" + searchTerm + \"') or contains(Key, '\" + searchTerm + \"'))\");\n\t\t$.each(data.value, function (idx, item) {\n\t\t\tmodel.prepareItem(item, typeInfo);\n\t\t});\n\t\tgrid.dataSource().data(data.value);\n\t\tgrid.refresh();\n\t},\n\n\texpandResource: async function (e, app, grid, cultures, typeInfo) {\n\t\tvar resource = grid.dataItem(e.masterRow);\n\t\tvar container = $(e.detailRow);\n\t\tif($(\"[name=translationsGrid]\", container).length === 0) {\n\t\t\tlet translations = (await api.get(\"ContentManagement/Resource?$filter=AppId eq \" + app.Id + \" and Key eq '\" + resource.Key + \"' and Name eq '\" + resource.Name + \"' and Culture ne '' \")).value.map(r => {\n\t\t\t\tmodel.prepareItem(r, typeInfo);\n\t\t\t\treturn r;\n\t\t\t});\n\t\t\tawait ResourceManagement.setupTranslationsGrid(app, $(\"[name=translations]\", container), resource, translations, cultures, typeInfo);\n\t\t}\n\t},\n\n\tescapeHtml: function(html) {\n    \treturn html.replaceAll('&', '&amp;').replaceAll('<', '&lt;').replaceAll('>', '&gt;').replaceAll('\"', '&quot;').replaceAll(\"'\", '&#039;');\n\t},\n\n\tsetupTranslationsGrid: async function(app, container, resource, translations, cultures, typeInfo) {\n\t\tvar translationsGrid = new GridWidget(container, translations);\n\t\ttranslationsGrid.groupable = false;\n\t\ttranslationsGrid.pageable = false;\n\t\ttranslationsGrid.filterable = false;\n\t\ttranslationsGrid.toolbar = kendo.template(\"<div class='btn-group btn-group-sm'> \\\n\t<button class='btn btn-primary' name='createTranslation'> \\\n\t\t<span class='k-icon k-i-plus'></span>[resource_displayname[newtranslation]] \\\n\t</button> \\\n</div>\");\n\t\ttranslationsGrid.columns = [\n\t\t\t{ field: \"Culture\", editable: false, title: \"[resource_shortdisplayname[culture]]\" },\n\t\t\t{ field: \"Name\", editable: false, title: \"[resource_shortdisplayname[name]]\" },\n\t\t\t{ field: \"DisplayName\", title: \"[resource_shortdisplayname[displayname]]\", template: \"#=(DisplayName && DisplayName.length > 80) ? ResourceManagement.escapeHtml(DisplayName.substring(0, 80)) + '...' : ResourceManagement.escapeHtml(DisplayName)#\", encoded: true },\n\t\t\t{ field: \"ShortDisplayName\", title: \"[resource_shortdisplayname[shortdisplayname]]\", template: \"#=(ShortDisplayName && ShortDisplayName.length > 80) ? ResourceManagement.escapeHtml(ShortDisplayName.substring(0, 80)) + '...' : ResourceManagement.escapeHtml(ShortDisplayName)#\", encoded: true },\n\t\t\t{ field: \"Description\", title: \"[resource_shortdisplayname[description]]\", template: \"#=(Description && Description.length > 80) ? ResourceManagement.escapeHtml(Description.substring(0, 80)) + '...' : ResourceManagement.escapeHtml(Description)#\", encoded: true },\n\t\t\t{ field: \"LastUpdated\", title: \"[resource_displayname[lastupdated]]\", width: 120, type: \"date\", format: \"{0: \" + type.dateFormat + \" HH:mm}\" },\n\t\t\t{ field: \"LastUpdatedBy\", title: \"[resource_displayname[lastupdatedby]]\", width: 200 },\n\t\t];\n\t\ttranslationsGrid.commands.push({\n\t\t\tname: \"save\",\n\t\t\ticon: \"k-i-save\",\n\t\t\ttext: \"[resource_displayname[save]]\"\n\t\t});\n\t\ttranslationsGrid.commands.push({\n\t\t\tname: \"delete\",\n\t\t\ticon: \"k-i-trash\",\n\t\t\ttext: \"[resource_displayname[delete]]\"\n\t\t});\n\t\ttranslationsGrid.dataBound = function() {\n\t\t\t$(\"button[name=save]\", translationsGrid.gridElement).on(\"click\",  (e) => ResourceManagement.save(e, translationsGrid));\n\t\t\t$(\"button[name=delete]\", translationsGrid.gridElement).on(\"click\", (e) => ResourceManagement.destroy(e, translationsGrid));\n\t\t};\n\t\tawait translationsGrid.init();\n\t\t$(\"button[name=createTranslation]\", translationsGrid.gridElement).on(\"click\", (e) => ResourceManagement.createTranslation(e, app, resource, translationsGrid, cultures, typeInfo));\n\t},\n\n\tcreateTranslation: async function(e, app, resource, translationsGrid, cultures, typeInfo) {\n\t\te.preventDefault();\n\t\tvar options = cultures.filter((c) => c.Id !== '' && translationsGrid.dataSource().data().filter((r) => r.Culture === c.Id).length === 0);\n\t\tResourceManagement.getCultureFromUser(options, async function (culture) {\n\t\t\tvar newTranslation = {\n\t\t\t\tId: 0,\n\t\t\t\tAppId: app.Id,\n\t\t\t\tCulture: culture,\n\t\t\t\tKey: resource.Key,\n\t\t\t\tName: resource.Name,\n\t\t\t\tDisplayName: resource.DisplayName,\n\t\t\t\tShortDisplayName: resource.ShortDisplayName,\n\t\t\t\tDescription: resource.Description\n\t\t\t};\n\t\t\tawait api.add(\"ContentManagement/Resource\", newTranslation).then((r) => {\n\t\t\t\tnotification.success(\"[resource_displayname[translationcreated]]\".replace(\"[culture]\", r.Culture));\n\t\t\t\tmodel.prepareItem(r, typeInfo);\n\t\t\t\ttranslationsGrid.dataSource().add(r);\n\t\t\t}).catch((err) => error(err));\n\t\t});\n\t},\n\n\tgetCultureFromUser: function (options, callback) {\n\t\tvar culture = null;\n\t\tvar d = new Dialog({\n\t\t\twidth: 600,\n\t\t\ttitle: \"[resource_displayname[newtranslation]]\"\n\t\t});\n\t\td.template = $(\"script[name=newTranslation]\").html();\n\t\td.events.confirm = function () {\n\t\t\tcallback(culture.val());\n\t\t\td.events.close();\n\t\t};\n\t\td.init(() => {\n\t\t\tculture = $(\"[name=culture]\", d.element)\n\t\t\t\t.kendoDropDownList({\n\t\t\t\t\tdataTextField: \"Name\",\n\t\t\t\t\tdataValueField: \"Id\",\n\t\t\t\t\tdataSource: options,\n\t\t\t\t\tindex: 0\n\t\t\t\t});\n\t\t});\n\t},\n\n\tmigrate: function (app) {\n\t\tvar d = new Dialog({\n\t\t\twidth: 1100,\n\t\t\theight: 600,\n\t\t\ttitle: \"[resource_displayname[migrate]]\"\n\t\t});\n\t\td.template = $(\"[name=resourceMigrationComponent]\").first().html();\n\t\td.init(() => ResourceMigration.initDropdown($(\".component[name=ResourceMigration]\", d.element)));\n\t},\n\n\n\tnewResource: async function (e, app, grid) {\n\t\tvar newResource = { Id: 0, AppId: app.Id, Culture: \"\", Key: \"\", Name: \"\", DisplayName: \"\", ShortDisplayName: \"\", Description: \"\", };\n\t\tvar args = {\n\t\t\tfields: [\n\t\t\t\t{ field: \"Key\", title: \"[resource_displayname[key]]\", description: \"[resource_description[key]]\" },\n\t\t\t\t{ field: \"Name\", title: \"[resource_displayname[name]]\", description: \"[resource_description[name]]\" },\n\t\t\t\t{ field: \"DisplayName\", title: \"[resource_displayname[displayname]]\", description: \"[resource_description[displayname]]\" },\n\t\t\t\t{ field: \"ShortDisplayName\", title: \"[resource_displayname[shortdisplayname]]\", description: \"[resource_description[shortdisplayname]]\" },\n\t\t\t\t{ field: \"Description\", title: \"[resource_displayname[description]]\", description: \"[resource_description[description]]\" }\n\t\t\t],\n\t\t\ttitle: \"[resource_displayname[newresource]]\",\n\t\t\twidth: 510,\n\t\t\tresourceKey: \"CMS\",\n\t\t\tdata: newResource,\n\t\t\tconfirm: \"<span class='k-icon k-i-plus'></span>[resource_displayname[confirm]]\",\n\t\t\tclose:\"[resource_displayname[close]]\"\n\t\t};\n\t\tvar resourceDialog = new EditorDialog(args);\n\t\tresourceDialog.events.confirm = async function () {\n\t\t\tnewResource = resourceDialog.data.toJSON();\n\t\t\tawait api.add(\"ContentManagement/Resource\", newResource).then(() => {\n\t\t\t\tresourceDialog.events.close();\n\t\t\t\tnotification.success(\"[resource_displayname[created]]\");\n\t\t\t\tgrid.refresh();\n\t\t\t}).catch((err) => error(err));\n\t\t};\n\t\tawait resourceDialog.init(args.data);\n\t},\n\n\tsave: async function (e, grid) {\n\t\te.preventDefault();\n\t\tvar resource = grid.dataItem($(e.currentTarget).closest(\"tr\"));\n\t\tawait resource.save().then(notification.success(\"[resource_displayname[saved]]\"));\n\t\tgrid.refresh();\n\t},\n\n\tdestroy: function (e, grid) {\n\t\te.preventDefault();\n\t\tvar resource = grid.dataItem($(e.currentTarget).closest(\"tr\"));\n\t\tvar d = new ConfirmDialog({ \n\t\t\ttitle: \"[resource_displayname[areyousure]]\",\n\t\t\tquestion: \"[resource_displayname[thiscannotbeundone]]\",\n\t\t\tconfirm: \"[resource_displayname[confirm]]\",\n\t\t\tclose: \"[resource_displayname[close]]\"\n\t\t});\n\t\td.events.confirm = async function () {\n\t\t\td.events.close();\n\t\t\tawait api.destroy(\"ContentManagement/Resource(\" + resource.Id + \")\").then(async () => {\n\t\t\t\tnotification.success(\"[resource_displayname[deleted]]\");\n\t\t\t\tgrid.refresh();\n\t\t\t}).catch((err) => error(err));\n\t\t};\n\t\td.init();\n\t}\n};",
  "Content": "<script type=\"text/template\" name=\"newTranslation\">\n\t<div class=\"input-group input-group-sm mb-1\">\n\t\t<span class=\"input-group-text\">[resource_displayname[culture]]</span>\n\t\t<input type=\"text\" class=\"form-control\" name=\"culture\" />\n\t</div>\n\n   <hr />\n   \n   <button class=\"btn btn-sm btn-primary float-end\" name=\"confirm\">\n      <span class=\"k-icon k-i-plus\"></span>[resource_displayname[create]]\n   </button>\n</script>\n\n<div name=\"resourceMigrationComponent\" style=\"display: none;\"></div>",
  "LastUpdated": "2024-11-19T18:18:30.5057301+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "ResourceMigration",
  "Key": "CMS",
  "ResourceKey": "CMS",
  "Script": "ResourceMigration = {\n    init: async function (app, container) {\n        app = app || session.app;\n        container = container || $(\".component[name=ResourceMigration]\");\n        api.addToMetaCache([\n        {\n            \"Name\": \"Core\",\n            \"Types\": [\n                [meta[ContentManagement/App]]\n            ]\n        }]);\n\n        var resourceKeys = await api.get(\"ContentManagement/Resource?$filter=AppId eq \" + app.Id + \"&$select=Key\");\n        var list = [...new Set(resourceKeys.value.map(item => item.Key))].map(x => ({ Key: x }))\n        var g = new GridWidget($(\"[name=resourceGrid]\", container), list);\n        g.columns = [\n            { selectable: true, width: 40 },\n            {\n                field: \"Key\",\n            }\n        ];\n        g.groupable = false;\n        g.init(() => {\n            g.kendoObject.dataSource.sort({ field: \"Key\", dir: \"asc\" });\n            g.kendoObject.select(g.kendoObject.tbody.find(\">tr\"));\n        });\n        $(\"[name=migrate]\", container).on(\"click\", async function (e) {\n            notification.info(\"[resource_displayname[migrating]]\");\n            var packages = await ResourceMigration.getPackages($(\"[name=resourceGrid]\", container), app);\n            var appId = $(\"[name=app]\", container).val();\n            await api.add(\"Packaging/Package/ImportThis()?appId=\" + appId, packages);\n            notification.success(\"[resource_displayname[migrated]]\");\n        });\n    },\n\n    initDropdown: async function(container) {\n        let ds = await model.getDatasource({\n            endpoint: \"ContentManagement/App\"\n        });\n        $(\"[name=app]\", container).kendoDropDownList({\n            autoBind: false,\n            optionLabel: \"[resource_displayname[selectapp]]\",\n            dataTextField: \"Name\",\n            dataValueField: \"Id\",\n            dataSource: ds\n        });\n    },\n\n    getPackages: async function (container, app) {\n        var packages = [{\n            Id: \"00000000-0000-0000-0000-000000000000\",\n            Name: \"Resources\",\n            Description: \"Generated by ResourceMigration\",\n            Category: \"Dynamic\",\n            SourceApi: session.apiRoot,\n            Items: [\n                await ResourceMigration.getPackage(container, app)\n            ]\n        }];\n        return packages;\n    },\n\n    getPackage: async function (container, app) {\n        var grid = $(container).find(\".k-grid\").data(\"kendoGrid\");\n        var keys = [];\n        grid.select().each(function () {\n            keys.push(grid.dataItem(this).Key);\n        });\n        var query = \"\";\n        for (var i = 0; i < keys.length; i = i + 1) {\n            query += \"'\" + keys[i] + \"',\";\n        }\n        query = query.substring(0, query.length - 1);\n        var data = await api.get(\"ContentManagement/Resource?$filter=AppId eq \" + app.Id + \" and Key in (\" + query + \")\");\n        for (var i = 0; i < data.value.length; i = i + 1) {\n            delete data.value[i].Id;\n        }\n        return {\n            Id: \"00000000-0000-0000-0000-000000000000\",\n            PackageId: \"00000000-0000-0000-0000-000000000000\",\n            Type: \"ContentManagement/Resource\",\n            Data: JSON.stringify(data.value)\n        };\n    }\n};",
  "Content": "<div class=\"row\">\n    <div class=\"col-md-12\">\n        <div class=\"input-group input-group-sm mb-1\">\n            <span class=\"input-group-text\">[resource_displayname[app]]</span>\n            <input type=\"text\" class=\"form-control\" name=\"app\" />\n        </div>\n    </div>\n    <div class=\"col-md-12\">\n        <div name=\"resourceGrid\"></div>\n    </div>\n</div>\n\n<hr />\n\n<button class=\"btn btn-sm btn-primary float-end\" name=\"migrate\">\n    <span class=\"k-icon k-i-arrow-up\"></span>[resource_displayname[migrate]]\n</button>",
  "LastUpdated": "2024-11-19T18:18:31.1858135+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "Scheduling",
  "Key": "Content Management",
  "ResourceKey": "CMS",
  "Script": "Scheduling = {\n\tinit: async function (app, container) {\n\t\tapp = app || session.app;\n\t\tcontainer = container || $(\".container[name=Scheduling]\");\n\t\tapi.addToMetaCache([\n\t\t\t{\n\t\t\t\t\"Name\": \"Core\",\n\t\t\t\t\"Types\": [\n\t\t\t\t\t[meta[Workflow/ScheduledTask]],\n\t\t\t\t\t[meta[Workflow/FlowDefinition]],\n\t\t\t\t\t[meta[AppSecurity/User]]\n\t\t\t\t]\n\t\t\t}\n\t\t]);\n\t\tvar scheduledTasks = await model.getDatasource({\n\t\t\tendpoint: \"Workflow/ScheduledTask\",\n\t\t\todataAppend: `?$filter=AppId eq ${app.Id}&$expand=Flow`,\n\t\t\tsort: {\n\t\t\t\tfield: \"Name\",\n\t\t\t\tdir: \"asc\"\n\t\t\t}\n\t\t});\n\t\tvar flowDefinitions = await model.getDatasource({\n\t\t\tendpoint: \"Workflow/FlowDefinition\",\n\t\t\todataAppend: `?$filter=AppId eq ${app.Id}&$orderBy=Name asc`\n\t\t});\n\t\tvar users = await model.getDatasource({\n\t\t\tendpoint: \"AppSecurity/User\",\n\t\t\todataAppend: `?$filter=Roles/any(r: r/Role/AppId eq ${app.Id})`,\n\t\t\tpageSize: 1000\n\t\t});\n\n\t\tawait Scheduling.initGrid(app, container, scheduledTasks, flowDefinitions, users);\n\n\t\tawait loadComponent($('[name=flowInstanceManagementComponent]', container), 'FlowInstanceManagement');\n\t},\n\n\tsecondInTicks: 10000000,\n\n\tticksToFriendly: function (value) {\n\t\tif (value === 0) { return \"[resource_displayname[notimeperiod]]\" }\n\t\tvar seconds = value / Scheduling.secondInTicks;\n\t\tvar days = Math.floor((seconds % 31536000) / 86400);\n\t\tvar hours = Math.floor(((seconds % 31536000) % 86400) / 3600);\n\t\tvar minutes = Math.floor((((seconds % 31536000) % 86400) % 3600) / 60);\n\t\tvar seconds = (((seconds % 31536000) % 86400) % 3600) % 60;\n\t\tvar string = \"\";\n\n\t\tif (days > 1)\n\t\t\tstring += days + \" [resource_displayname[days]] \";\n\t\telse if (days === 1)\n\t\t\tstring += days + \" [resource_displayname[day]] \";\n\n\t\tif (hours > 1)\n\t\t\tstring += hours + \" [resource_displayname[hours]] \";\n\t\telse if (hours === 1)\n\t\t\tstring += hours + \" [resource_displayname[hour]] \";\n\n\t\tif (minutes > 1)\n\t\t\tstring += minutes + \" [resource_displayname[minutes]] \";\n\t\telse if (minutes === 1)\n\t\t\tstring += minutes + \" [resource_displayname[minute]] \";\n\n\t\tif (seconds > 1)\n\t\t\tstring += seconds + \" [resource_displayname[seconds]] \";\n\t\telse if (seconds === 1)\n\t\t\tstring += seconds + \" [resource_displayname[second]] \";\n\n\t\treturn string;\n\t},\n\n\tinitGrid: async function (app, container, scheduledTasks, flowDefinitions, users) {\n\t\tvar grid = new GridWidget(container, scheduledTasks);\n\t\tgrid.groupable = false;\n\t\tgrid.toolbar = [\n\t\t\t{\n\t\t\t\ttemplate: `<div class='btn-group btn-group-sm'>\n\t\t\t\t\t<button class='btn btn-primary' name='create'>\n\t\t\t\t\t\t<span class='k-icon k-i-plus'></span>[resource_displayname[new]]\n\t\t\t\t\t</button>\n\t\t\t\t</div>`\n\t\t\t}\n\t\t];\n\t\tgrid.columns = [\n\t\t\t{ field: \"Name\", title: \"[resource_shortdisplayname[Name]]\" },\n\t\t\t{ field: \"Description\", title: \"[resource_shortdisplayname[Description]]\" },\n\t\t\t{\n\t\t\t\tfield: \"FlowId\",\n\t\t\t\ttitle: \"[resource_shortdisplayname[Flow]]\",\n\t\t\t\ttemplate: \"#=Flow.Name#\",\n\t\t\t\teditor: function (container, options) {\n\t\t\t\t\t$('<input required name=\"' + options.field + '\"/>')\n\t\t\t\t\t\t.appendTo(container)\n\t\t\t\t\t\t.kendoDropDownList({ autoBind: false, dataTextField: \"Name\", dataValueField: \"Id\", dataSource: flowDefinitions });\n\t\t\t\t}\n\t\t\t},\n\t\t\t{\n\t\t\t\tfield: \"LastExecuted\",\n\t\t\t\ttitle: \"[resource_shortdisplayname[lastexecuted]]\",\n\t\t\t\ttype: \"date\",\n\t\t\t\tformat: \"{0:\" + type.dateFormat + \" HH:mm}\",\n\t\t\t\twidth: \"[theme[columns.small]]\",\n\t\t\t\teditable: () => false\n\t\t\t},\n\t\t\t{\n\t\t\t\tfield: \"NextExecution\",\n\t\t\t\ttitle: \"[resource_shortdisplayname[nextexecution]]\",\n\t\t\t\ttype: \"date\",\n\t\t\t\tformat: \"{0:\" + type.dateFormat + \" HH:mm}\",\n\t\t\t\twidth: \"[theme[columns.small]]\",\n\t\t\t\teditor: function (container, options) {\n\t\t\t\t\t$('<input required name=\"' + options.field + '\"/>')\n\t\t\t\t\t\t.appendTo(container)\n\t\t\t\t\t\t.kendoDateTimePicker({\n\t\t\t\t\t\t\tautoBind: false,\n\t\t\t\t\t\t\tvalue: new Date(),\n\t\t\t\t\t\t\tdateInput: true,\n\t\t\t\t\t\t\tformat: type.dateFormat + \" HH:mm\"\n\t\t\t\t\t\t});\n\t\t\t\t}\n\t\t\t},\n\t\t];\n\n\t\tgrid.commands.push({ name: \"execute\", text: \"[resource_displayname[execute]]\", icon: \"k-i-play-sm\" });\n\t\tgrid.commands.push({ name: \"delete\", text: \"[resource_displayname[delete]]\", icon: \"k-i-trash\" });\n\n\t\tgrid.dataBound = function () {\n\t\t\t$(\"button[name=delete]\", grid.gridElement).on(\"click\", (e) => Scheduling.destroy(e, grid));\n\t\t\t$(\"button[name=execute]\", grid.gridElement).on(\"click\", async (e) => await Scheduling.execute(e, grid));\n\t\t};\n\n\t\tgrid.detailTemplate = `<div class=\"scheduled-task-detail-grid\"></div>`;\n\t\tgrid.detailExpand = async (e) => {\n\t\t\tvar item = grid.dataItem($(e.masterRow));\n\t\t\tvar detailRow = $(\"div\", e.detailRow);\n\t\t\tvar uid = item.uid;\n\n\t\t\tvar draft = {\n\t\t\t\tScheduleInTicks: item.ScheduleInTicks,\n\t\t\t\tNextExecution: item.NextExecution,\n\t\t\t\tExecuteAs: item.ExecuteAs,\n\t\t\t\tExecutionArgs: item.ExecutionArgs\n\t\t\t};\n\n\t\t\tvar html = $(\"script[name=scheduledTaskTabs]\")\n\t\t\t\t.html()\n\t\t\t\t.replace(/{UID}/g, uid);\n\n\t\t\tdetailRow.html(html);\n\n\t\t\tdetailRow.find('.nav-tabs button').each(function () {\n\t\t\t\tnew bootstrap.Tab(this);\n\t\t\t});\n\n\t\t\tbootstrap.Tab.getOrCreateInstance(\n\t\t\t\tdetailRow.find('.nav-tabs button:first')[0]\n\t\t\t).show();\n\n\t\t\tvar flowTab = detailRow.find('[name=FlowInstanceManagement]');\n\n\t\t\tsetTimeout(() => {\n\t\t\t\tFlowInstanceManagement.init(\n\t\t\t\t\tsession.app,\n\t\t\t\t\tflowTab,\n\t\t\t\t\titem.FlowId,\n\t\t\t\t\titem.Flow?.InstanceReportingComponentName\n\t\t\t\t);\n\t\t\t}, 0);\n\n\t\t\tvar detailsTab = detailRow.find(`#taskDetailsTab-${uid}`);\n\t\t\tvar formHtml = $(\"script[name=scheduledTaskDetailForm]\").html();\n\n\t\t\tdetailsTab.find(\".nested-grid-container\").html(formHtml);\n\n\t\t\tvar form = detailsTab.find(\".scheduled-task-form\");\n\n\t\t\tform.find(\"[name=ScheduleInTicks]\").kendoSlider({\n\t\t\t\tmin: 0,\n\t\t\t\tmax: 86400 * Scheduling.secondInTicks * 14,\n\t\t\t\tsmallStep: Scheduling.secondInTicks * 3600,\n\t\t\t\tlargeStep: Scheduling.secondInTicks * 3600 * 24,\n\t\t\t\tvalue: draft.ScheduleInTicks || 0,\n\t\t\t\ttickPlacement: \"none\",\n\t\t\t\ttooltip: { template: \"#=Scheduling.ticksToFriendly(value)#\" },\n\t\t\t\tchange: (e) => {\n\t\t\t\t\tdraft.ScheduleInTicks = e.value;\n\t\t\t\t\tform.find(\"[name=schedulePreview]\").text(Scheduling.ticksToFriendly(e.value));\n\t\t\t\t}\n\t\t\t});\n\n\t\t\tform.find(\"[name=schedulePreview]\")\n\t\t\t\t.text(Scheduling.ticksToFriendly(draft.ScheduleInTicks));\n\n\t\t\tform.find(\"[name=NextExecution]\").kendoDateTimePicker({\n\t\t\t\tvalue: draft.NextExecution,\n\t\t\t\tdateInput: true,\n\t\t\t\tformat: type.dateFormat + \" HH:mm\",\n\t\t\t\tchange: (e) => draft.NextExecution = e.sender.value()\n\t\t\t});\n\n\t\t\tform.find(\"[name=ExecuteAs]\").kendoDropDownList({\n\t\t\t\tautoBind: false,\n\t\t\t\tdataTextField: \"Id\",\n\t\t\t\tdataValueField: \"Id\",\n\t\t\t\tdataSource: users,\n\t\t\t\tvalue: draft.ExecuteAs,\n\t\t\t\tchange: (e) => draft.ExecuteAs = e.sender.value()\n\t\t\t});\n\n\t\t\tvar argsEditor = new MonacoEditor(\n\t\t\t\tform.find(\"[name=ExecutionArgs]\")[0],\n\t\t\t\t{\n\t\t\t\t\tcode: draft.ExecutionArgs,\n\t\t\t\t\tlanguage: \"json\",\n\t\t\t\t\tautomaticLayout: true\n\t\t\t\t}\n\t\t\t);\n\n\t\t\targsEditor.onChange = () => {\n\t\t\t\tdraft.ExecutionArgs = argsEditor.getValue();\n\t\t\t};\n\n\t\t\targsEditor.init();\n\n\n\t\t\tform.find(\"button[name=saveDetail]\").on(\"click\", async () => {\n\n\t\t\t\titem.set(\"ScheduleInTicks\", draft.ScheduleInTicks);\n\t\t\t\titem.set(\"NextExecution\", draft.NextExecution);\n\t\t\t\titem.set(\"ExecuteAs\", draft.ExecuteAs);\n\t\t\t\titem.ExecutionArgs = draft.ExecutionArgs;\n\n\t\t\t\tvar payload = {\n\t\t\t\t\tId: item.Id,\n\t\t\t\t\tAppId: item.AppId,\n\t\t\t\t\tName: item.Name,\n\t\t\t\t\tDescription: item.Description,\n\t\t\t\t\tFlowId: item.FlowId,\n\t\t\t\t\tExecuteAs: item.ExecuteAs,\n\t\t\t\t\tNextExecution: item.NextExecution,\n\t\t\t\t\tExecutionArgs: item.ExecutionArgs,\n\t\t\t\t\tScheduleInTicks: item.ScheduleInTicks\n\t\t\t\t};\n\n\t\t\t\ttry {\n\t\t\t\t\tawait api.put(`Workflow/ScheduledTask(${item.Id})`, payload);\n\t\t\t\t\tnotification.success(\"[resource_displayname[saved]]\");\n\n\t\t\t\t} catch (err) {\n\t\t\t\t\terror(err);\n\t\t\t\t}\n\t\t\t});\n\t\t};\n\n\n\n\t\tawait grid.init();\n\t\t$(\"button[name=create]\", grid.gridElement).on(\"click\", () => Scheduling.newScheduledTask(app, grid, flowDefinitions));\n\t},\n\n\teditArgs: async function (e, grid) {\n\t\te.preventDefault();\n\t\tvar task = grid.kendoObject.dataItem($(e.currentTarget).closest(\"tr\"));\n\t\tvar argsEditor = null;\n\t\tvar d = new Dialog({\n\t\t\ttitle: \"[resource_displayname[editArgs]]\",\n\t\t\twidth: 800\n\t\t});\n\t\td.template = $(\"script[name=editArgs]\").html();\n\t\td.events.save = async function (e) {\n\t\t\ttask.save(e, function () { notification.success(\"[resource_displayname[ScheduledTaskSaved]]\"); d.events.close(); });\n\t\t};\n\t\td.init(() => {\n\t\t\targsEditor = new MonacoEditor($(\"[name=executionArgs]\", d.element)[0], { code: task.ExecutionArgs, language: \"json\", automaticLayout: true });\n\t\t\targsEditor.onChange = (e) => task.ExecutionArgs = argsEditor.getValue();\n\t\t\targsEditor.init();\n\t\t\targsEditor.editor.layout();\n\t\t});\n\t},\n\n\tnewScheduledTask: async function (app, grid, flowDefinitions) {\n\t\tvar d = new Dialog({ title: \"[resource_displayname[newscheduledtask]]\", width: 620, height: 660 });\n\t\td.template = $(\"[name=newScheduledTask]\").html();\n\t\tvar appRoles = (await api.get(\"AppSecurity/Role?$expand=Users&$filter=AppId eq \" + app.Id)).value;\n\t\tvar userIds = appRoles.map(r => r.Users).flat().map(r => r.UserId);\n\t\tvar argsEditor = null;\n\t\td.events.create = async function (e) {\n\t\t\tvar newScheduledTask = {\n\t\t\t\tName: $(\"[name=name]\", d.element).val(),\n\t\t\t\tDescription: $(\"[name=description]\", d.element).val(),\n\t\t\t\tFlowId: $(\"[name=flowDefinition]\", d.element).val(),\n\t\t\t\tAppId: app.Id,\n\t\t\t\tExecuteAs: $(\"[name=executeas]\", d.element).val(),\n\t\t\t\tNextExecution: (new Date($(\"[name=nextexecution]\", d.element).val())).toISOString(),\n\t\t\t\tExecutionArgs: argsEditor.getValue(),\n\t\t\t\tScheduleInTicks: parseInt($(\"[name=scheduleInTicks]\", d.element).val()) || 0\n\t\t\t};\n\t\t\td.events.close();\n\t\t\tawait api.add(\"Workflow/ScheduledTask\", newScheduledTask).then(() => {\n\t\t\t\tnotification.success(\"[resource_displayname[schedulecreated]]\");\n\t\t\t\tgrid.refresh();\n\t\t\t}).catch((err) => error(err));\n\t\t};\n\n\t\td.init(async () => {\n\t\t\t$(\"[name=scheduleInTicks]\", d.element).kendoSlider({\n\t\t\t\tmin: 0,\n\t\t\t\tmax: 86400 * Scheduling.secondInTicks * 14, // 0 to 14 days\n\t\t\t\tsmallStep: Scheduling.secondInTicks * 3600, // 1 hour\n\t\t\t\tlargeStep: Scheduling.secondInTicks * 3600 * 24, // 1 day\n\t\t\t\ttooltip: {\n\t\t\t\t\ttemplate: \"#=Scheduling.ticksToFriendly(value)#\"\n\t\t\t\t}\n\t\t\t});\n\n\t\t\t$(\"[name=flowDefinition]\", d.element).kendoDropDownList({ dataTextField: \"Name\", dataValueField: \"Id\", dataSource: flowDefinitions });\n\t\t\t$(\"[name=nextexecution]\", d.element).kendoDateTimePicker({ value: new Date(), dateInput: true });\n\t\t\t$(\"[name=executeas]\").kendoDropDownList({\n\t\t\t\tdataSource: { data: userIds }\n\t\t\t});\n\t\t\targsEditor = new MonacoEditor($(\"[name=executionArgs]\", d.element)[0], { code: \"\", language: \"json\", automaticLayout: true });\n\t\t\targsEditor.init();\n\t\t\tgrid.refresh();\n\t\t});\n\t},\n\n\texecute: async function (e, grid) {\n\t\te.preventDefault();\n\t\tvar task = grid.dataItem($(e.currentTarget).closest(\"tr\"));\n\t\tawait api.post(\"Workflow/ScheduledTask(\" + task.Id + \")/Execute?incrementNextExecution=false\").then(() => {\n\t\t\tnotification.success(\"[resource_displayname[executed]]\");\n\t\t}).catch((err) => error(err));\n\t},\n\n\tsaveChild: async function (e, nestedGrid, parentGrid) {\n\t\te && e.preventDefault();\n\n\t\tvar kendoGrid = nestedGrid.kendoObject || nestedGrid.grid;\n\t\tif (!kendoGrid) return;\n\n\t\tvar row = $(e.currentTarget).closest(\"tr\");\n\t\tvar childItem = kendoGrid.dataItem(row);\n\t\tif (!childItem) return;\n\n\t\trow.find(\"input, select, textarea\").each(function () {\n\t\t\tvar field = $(this).attr(\"name\");\n\t\t\tif (!field) return;\n\n\t\t\tlet value = $(this).data(\"kendoDropDownList\")?.value()\n\t\t\t\t|| $(this).data(\"kendoDateTimePicker\")?.value()\n\t\t\t\t|| $(this).val();\n\n\t\t\tif (field === \"ScheduleInTicks\") value = parseInt(value) || 0;\n\n\t\t\tchildItem.set(field, value);\n\t\t});\n\n\t\tvar payload = {\n\t\t\tId: childItem.Id,\n\t\t\tAppId: childItem.AppId,\n\t\t\tName: childItem.Name,\n\t\t\tDescription: childItem.Description,\n\t\t\tFlowId: childItem.FlowId,\n\t\t\tExecuteAs: childItem.ExecuteAs,\n\t\t\tNextExecution: childItem.NextExecution,\n\t\t\tExecutionArgs: childItem.ExecutionArgs,\n\t\t\tScheduleInTicks: childItem.ScheduleInTicks\n\t\t};\n\n\t\ttry {\n\t\t\tawait api.put(\"Workflow/ScheduledTask(\" + payload.Id + \")\", payload);\n\t\t\tchildItem.dirty = false;\n\t\t\tnotification.success(\"[resource_displayname[saved]]\");\n\n\t\t\tif (parentGrid) parentGrid.refresh();\n\n\t\t} catch (err) {\n\t\t\terror(err);\n\t\t}\n\n\t\tkendoGrid.refresh();\n\t},\n\n\tdestroy: function (e, grid) {\n\t\te.preventDefault();\n\t\tvar task = grid.dataItem($(e.currentTarget).closest(\"tr\"));\n\t\ttask.destroy(e).then(() => {\n\t\t\tnotification.success(\"[resource_displayname[scheduledeleted]]\");\n\t\t\tgrid.refresh();\n\t\t}).catch((err) => error(err));\n\t}\n}",
  "Content": "<script type=\"text/template\" name=\"editArgs\">\n    <div name=\"executionArgs\"></div>\n    <hr />\n    <button class=\"btn btn-sm btn-primary float-end\" name=\"save\">\n        <span class=\"k-icon k-i-check\"></span>[resource_displayname[save]]\n    </button>\n</script>\n\n<script type=\"text/template\" name=\"newScheduledTask\">\n    <div class=\"input-group input-group-sm mb-1\">\n        <span class=\"input-group-text\">[resource_displayname[name]]</span>\n        <input type=\"text\" class=\"form-control\" name=\"name\" />\n    </div>\n\n    <div class=\"input-group input-group-sm mb-1\">\n        <span class=\"input-group-text\">[resource_displayname[description]]</span>\n        <input type=\"text\" class=\"form-control\" name=\"description\" />\n    </div>\n\n    <div class=\"input-group input-group-sm mb-1\">\n        <span class=\"input-group-text\">[resource_displayname[executeas]]</span>\n        <input type=\"text\" class=\"form-control\" name=\"executeas\" />\n    </div>\n\n    <div class=\"input-group input-group-sm mb-1\">\n        <span class=\"input-group-text\">[resource_displayname[nextexecution]]</span>\n        <input type=\"datetime-local\" class=\"form-control\" name=\"nextexecution\" />\n    </div>\n\n    <div class=\"input-group input-group-sm mb-1\">\n        <span class=\"input-group-text\">[resource_displayname[flowDefinition]]</span>\n        <input type=\"text\" class=\"form-control\" name=\"flowDefinition\" />\n    </div>\n\n    <div class=\"input-group input-group-sm mb-1\">\n        <span class=\"input-group-text\">[resource_displayname[schedule]]</span>\n        <input type=\"text\" class=\"form-control\" name=\"scheduleInTicks\" />\n    </div>\n\n    <div class=\"input-group input-group-sm mb-1\">\n        <span class=\"input-group-text\">[resource_displayname[executionArgs]]</span>\n    </div>\n    <div name=\"executionArgs\"></div>\n    <hr />\n    <button class=\"btn btn-sm btn-primary float-end\" name=\"create\">\n        <span class=\"k-icon k-i-plus\"></span>[resource_displayname[add]]\n    </button>\n</script>\n\n<script name=\"flowExecution\" type=\"text/template\">\n    <div class=\"execution\">\n        <ul>\n            <li>\n                <label>On Execution Complete</label>\n                <ul class=\"value\">\n                    <li><input type=\"checkbox\" name=\"autoClose\" checked>Auto Close</input></li>\n                </ul>\n            </li>\n        </ul>\n        <div class=\"executionConsole\"></div>\n    </div>\n</script>\n\n<script name=\"executionLog\" type=\"text/template\">\n    <div class=\"execConsole\" name=\"execConsole\">\n        <div class=\"flowConsole\"></div>\n    </div>\n</script>\n\n<script type=\"text/template\" name=\"scheduledTaskTabs\">\n    <ul class=\"nav nav-tabs\" role=\"tablist\">\n        <li class=\"nav-item\">\n            <button class=\"nav-link active\" id=\"taskDetailsTab-{UID}-button\"\n                    data-bs-toggle=\"tab\"\n                    data-bs-target=\"#taskDetailsTab-{UID}\"\n                    type=\"button\"\n                    role=\"tab\"\n                    aria-controls=\"taskDetailsTab-{UID}\"\n                    aria-selected=\"true\">\n                <span class=\"k-icon k-i-info-circle\"></span>[resource_displayname[details]]\n            </button>\n        </li>\n        <li class=\"nav-item\">\n            <button class=\"nav-link\" id=\"taskInstancesTab-{UID}-button\"\n                    data-bs-toggle=\"tab\"\n                    data-bs-target=\"#taskInstancesTab-{UID}\"\n                    type=\"button\"\n                    role=\"tab\"\n                    aria-controls=\"taskInstancesTab-{UID}\"\n                    aria-selected=\"false\">\n                <span class=\"k-icon k-i-clock-arrow-rotate\"></span>[resource_displayname[instances]]\n            </button>\n        </li>\n    </ul>\n\n    <div class=\"tab-content mt-2\">\n        <div class=\"tab-pane fade show active\" id=\"taskDetailsTab-{UID}\" role=\"tabpanel\" aria-labelledby=\"taskDetailsTab-{UID}-button\">\n            <div class=\"nested-grid-container\"></div>\n        </div>\n\n        <div class=\"tab-pane fade\" id=\"taskInstancesTab-{UID}\" role=\"tabpanel\" aria-labelledby=\"taskInstancesTab-{UID}-button\">\n            <div class=\"component\" name=\"FlowInstanceManagement\"></div>\n        </div>\n    </div>\n</script>\n\n<script type=\"text/x-kendo-template\" name=\"scheduledTaskDetailForm\">\n  <div class=\"scheduled-task-form\">\n\n    <div class=\"input-group input-group-sm mb-2\">\n      <span class=\"input-group-text\">[resource_displayname[schedule]]</span>\n      <input name=\"ScheduleInTicks\" class=\"form-control\" />\n    </div>\n    <div name=\"schedulePreview\" class=\"ms-2 mb-2\"></div>\n\n    <div class=\"input-group input-group-sm mb-2\">\n      <span class=\"input-group-text\">[resource_displayname[nextexecution]]</span>\n      <input name=\"NextExecution\" class=\"form-control\" />\n    </div>\n\n    <div class=\"input-group input-group-sm mb-2\">\n      <span class=\"input-group-text\">[resource_displayname[executeas]]</span>\n      <input name=\"ExecuteAs\" class=\"form-control\" />\n    </div>\n\n    <div class=\"input-group input-group-sm mb-2\">\n    <span class=\"input-group-text\">[resource_displayname[executionArgs]]</span>\n    </div>\n\n    <div name=\"ExecutionArgs\"\n        class=\"form-control mb-2\">\n    </div>\n\n    <hr />\n\n    <div class=\"d-flex justify-content-end\">\n      <button name=\"saveDetail\" class=\"btn btn-sm btn-primary\">\n        <span class=\"k-icon k-i-save\"></span>[resource_displayname[save]]\n      </button>\n    </div>\n\n  </div>\n</script>\n\n\n\n<div name=\"flowInstanceManagementComponent\" style=\"display:none;\"></div>\n\n<style scoped>\n    div[name=ExecutionArgs] > .monaco-editor {height: 200px !important;}\n    div[name=ExecutionArgs] > .monaco-editor > .overflow-guard {height: 200px !important;}\n    </style>",
  "LastUpdated": "2025-05-30T18:44:57.6789397Z"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "Shadows",
  "Key": "Theming",
  "ResourceKey": "CMS",
  "Script": "Shadows = {\n    init: async function (app, container, observable) {\n        app = app || session.app;\n        container = container || $(\".component[name=Shadows]\");\n        if (!observable)\n            return;\n\n        $(\"[name=colour]\", container).kendoColorPicker({\n            buttons: false,\n            value: observable.get(\"shadows\").split(\" \")[3],\n            change: function (e) {\n                e.preventDefault();\n                var existingShadowValue = observable.get(\"shadows\");\n                var parts = existingShadowValue.split(\" \");\n                parts[3] = this.value();\n                observable.set(\"shadows\", parts.join(\" \"));\n            }\n        });\n\n        await Shadows.initSliders(container, observable);\n    },\n\n    initSliders: async function (container, observable) {\n        $(\"[name=horizontaloffset]\", container).kendoSlider({\n            min: 0,\n            max: 20,\n            smallStep: 1,\n            largeStep: 2,\n            value: parseFloat(observable.get(\"shadows\").split(\" \")[0].replaceAll(\"px\", \"\")),\n            change: function (e) {\n                e.preventDefault();\n                var existingShadowValue = observable.get(\"shadows\");\n                var parts = existingShadowValue.split(\" \");\n                parts[0] = this.value() + \"px\";\n                observable.set(\"shadows\", parts.join(\" \"));\n            }\n        });\n        $(\"[name=verticaloffset]\", container).kendoSlider({\n            min: 0,\n            max: 20,\n            smallStep: 1,\n            largeStep: 2,\n            value: parseFloat(observable.get(\"shadows\").split(\" \")[1].replaceAll(\"px\", \"\")),\n            change: function (e) {\n                e.preventDefault();\n                var existingShadowValue = observable.get(\"shadows\");\n                var parts = existingShadowValue.split(\" \");\n                parts[1] = this.value() + \"px\";\n                observable.set(\"shadows\", parts.join(\" \"));\n            }\n        });\n        $(\"[name=blurradius]\", container).kendoSlider({\n            min: 0,\n            max: 20,\n            smallStep: 1,\n            largeStep: 2,\n            value: parseFloat(observable.get(\"shadows\").split(\" \")[2].replaceAll(\"px\", \"\")),\n            change: function (e) {\n                e.preventDefault();\n                var existingShadowValue = observable.get(\"shadows\");\n                var parts = existingShadowValue.split(\" \");\n                parts[2] = this.value() + \"px\";\n                observable.set(\"shadows\", parts.join(\" \"));\n            }\n        });\n    }\n}",
  "Content": "<div class=\"row\">\n    <div class=\"col-md-6\">\n        <div class=\"input-group input-group-sm mb-1\">\n            <span class=\"input-group-text\">[resource_displayname[horizontaloffset]]</span>\n            <input class=\"form-control\" name=\"horizontaloffset\" />\n        </div>\n\t\t\n        <div class=\"input-group input-group-sm mb-1\">\n            <span class=\"input-group-text\">[resource_displayname[verticaloffset]]</span>\n            <input type=\"text\" class=\"form-control\" name=\"verticaloffset\" />\n        </div>\n\t\t\n        <div class=\"input-group input-group-sm mb-1\">\n            <span class=\"input-group-text\">[resource_displayname[blurradius]]</span>\n            <input type=\"text\" class=\"form-control\" name=\"blurradius\" />\n        </div>\n\n        <div class=\"input-group input-group-sm mb-1\">\n            <span class=\"input-group-text\">[resource_displayname[colour]]</span>\n            <input class=\"form-control\" name=\"colour\" />\n        </div>\n    </div>\n</div>",
  "LastUpdated": "2024-11-19T18:18:31.1168767+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "Sidenav",
  "Key": "CMS",
  "ResourceKey": "CMS",
  "Script": "Sidenav = {\n    init: async function(app, container) {\n        app = app || session.app;\n        container = container || $(\".component[name=Sidenav]\");\n\n        var host = $(\"[name=documentationTree]\", container);\n        var result = await api.get(\"ContentManagement/Page?$filter=AppId eq \" + app.Id + \" and ShowOnMenus eq true&$orderby=Order asc&$expand=PageInfo\");\n        var pages = (result.value || [])\n            .filter(page => page.Path === \"Documentation\" || page.Path.startsWith(\"Documentation/\"));\n\n        var html = Sidenav.renderTree(pages, \"Documentation\", window.location.pathname.replace(/^\\//, \"\"));\n        host.html(html);\n    },\n\n    renderTree: function(pages, rootPath, currentPath) {\n        var children = pages\n            .filter(page => Sidenav.parentPath(page.Path) === rootPath)\n            .sort((left, right) => (left.Order || 0) - (right.Order || 0) || Sidenav.title(left).localeCompare(Sidenav.title(right)));\n\n        if (children.length === 0) {\n            return \"\";\n        }\n\n        var items = children.map(page => {\n            var active = page.Path === currentPath ? \" active\" : \"\";\n            var childTree = Sidenav.renderTree(pages, page.Path, currentPath);\n            var icon = active ? \"k-i-arrow-right\" : \"k-i-file\";\n\n            return `<li class=\"nav-item\"><span class=\"k-icon ${icon}\"></span><a class=\"nav-link${active}\" href=\"/${page.Path}\">${Sidenav.title(page)}</a>${childTree}</li>`;\n        }).join(\"\");\n\n        return `<ul class=\"navbar-nav submenu\">${items}</ul>`;\n    },\n\n    parentPath: function(path) {\n        var index = path.lastIndexOf(\"/\");\n        return index < 0 ? \"\" : path.substring(0, index);\n    },\n\n    title: function(page) {\n        var info = (page.PageInfo || []).filter(item => item.CultureId === session.culture)[0]\n            || (page.PageInfo || []).filter(item => item.CultureId === \"\")[0];\n\n        return (info && info.Title) || page.Name;\n    }\n}",
  "Content": "<nav name=\"documentationTree\" class=\"documentation-tree\"></nav>",
  "LastUpdated": "2024-06-24T12:26:22.683782+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "SSOMetadata",
  "Key": "Tools",
  "ResourceKey": "Debug",
  "Script": "SSOMetadata = {\r\n    init: async function (app, container) {\r\n        app = app || session.app;\r\n        container = container || $(\".component[name=SSOMetadata]\");\r\n        var swaggerDefinition = await api.get(\"../swagger/v1/swagger.json\");\r\n        var dataSource = await SSOMetadata.getDatasource();\r\n        var grid = new GridWidget(container, { data: dataSource, pageSize: 1000, group: { field: \"Category\", dir: \"asc\"} });\r\n        grid.editable = false;\r\n        grid.columns = [\r\n            { field: \"Category\", title: \"[resource_displayname[context]]\", width: 200 },\r\n            { field: \"DisplayName\", title: \"[resource_displayname[displayname]]\", width: 200 },\r\n            { field: \"url\", title: \"[resource_displayname[url]]\", template: \"#:session.apiRoot##:Category#/#:DisplayName#\" }\r\n        ];\r\n\r\n        grid.detailTemplate = $(\"script[name=metadataDetails]\", container).first().html();\r\n        grid.detailExpand = async function(e) {\r\n            var container = $(e.detailRow);\r\n            var meta = this.dataItem(e.masterRow);\r\n            if($(\".k-tabstrip\", container).length == 0) {\r\n                $(\"[name=tabs]\", container).kendoTabStrip({ animation: { open: { effects: \"fadeIn\" } } });\r\n                var editor = new MonacoEditor($(\"[name=rawJson]\", container)[0], {\r\n                    code: JSON.stringify(meta, null, '\\t'),\r\n                    language: \"json\",\r\n                    automaticLayout: true\r\n                });\r\n                editor.init();\r\n                await SSOMetadata.setupMetaPropertiesGrid(container, meta);\r\n                await SSOMetadata.setupEndpoints(container, meta, swaggerDefinition.paths, app);\r\n                await SSOMetadata.setupDependencies(container, meta);\r\n            }\r\n        };\r\n        await grid.init();\r\n\r\n    },\r\n\r\n    getDatasource: async function() {\r\n        var data = [];\r\n        var allMeta = await api.get(\"Getmetadata\");\r\n\r\n        var contexts = allMeta.filter(i => i.UriBase && i.Name === \"Security\");\r\n        contexts.forEach((context) => {\r\n            var entities = context.Types.filter(t => t.IsEntity && t.HasEndpoint).sort((t1, t2) => t1.DisplayName > t2.DisplayName ? 1 : -1);\r\n            entities.forEach((item) => {\r\n                item.id = item.Category + '/' + item.DisplayName;\r\n                item.testStatus = \"loading\";\r\n                item.isUp = \"loading\";\r\n                data.push(item);\r\n            })\r\n        });\r\n\r\n        window.allMeta = data;;\r\n\r\n        return data;\r\n    },\r\n\r\n    setupMetaPropertiesGrid: async function (container, meta) {\r\n        var propertyGrid = new GridWidget($(\"[name=properties]\", container), { data: meta.Properties, pageSize: 50 });\r\n        propertyGrid.exports = false;\r\n        propertyGrid.editable = false;\r\n        propertyGrid.groupable = false;\r\n\r\n        propertyGrid.columns = [\r\n            { field: \"Name\", title: \"[resource_displayname[name]]\" },\r\n            { field: \"Type\", title: \"[resource_displayname[type]]\" },\r\n            { field: \"ServerType\", title: \"[resource_displayname[servertype]]\" },\r\n            { field: \"ServerTypeName\", title: \"[resource_displayname[servertypename]]\" },\r\n            { field: \"Template\", title: \"[resource_displayname[template]]\" },\r\n            { field: \"DisplayName\", title: \"[resource_displayname[displayname]]\" },\r\n            { field: \"ShortDisplayName\", title: \"[resource_displayname[shortdisplayname]]\" },\r\n            { field: \"Description\", title: \"[resource_displayname[description]]\" },\r\n            { field: \"IsGeneric\", title: \"[resource_displayname[isgeneric]]\", type: \"boolean\" },\r\n            { field: \"IsValueType\", title: \"[resource_displayname[isvaluetype]]\", type: \"boolean\" },\r\n            { field: \"IsReadOnly\", title: \"[resource_displayname[isreadonly]]\", type:\"boolean\" },\r\n            { field: \"IsRequired\", title: \"[resource_displayname[isrequired]]\", type: \"boolean\" }\r\n        ];\r\n        await propertyGrid.init();\r\n    },\r\n\r\n    setupEndpoints: async function(container, meta, swaggerPaths, app) {\r\n        var applicableEndpointKeys = Object.keys(swaggerPaths).filter(r => r.indexOf(\"/Api/\" + meta.id) !== -1);\r\n        var applicableEndpoints = applicableEndpointKeys.map(endpointURL => \r\n            SSOMetadata.getHttpMethods(swaggerPaths[endpointURL]).map(httpMethodName => ({\r\n                \"endpoint\": endpointURL,\r\n                \"data\": swaggerPaths[endpointURL][httpMethodName.toLowerCase()],\r\n                \"method\": httpMethodName\r\n            }))).flat();\r\n\r\n        var endpointGrid = new GridWidget($(\"[name=endpoints]\", container), { data: applicableEndpoints, pageSize: 20 });\r\n        endpointGrid.groupable = false;\r\n        endpointGrid.columns = [\r\n            { field: \"method\", title: \"[resource_displayname[methods]]\", template: \"#=method#\", width: 80 },\r\n            { field: \"endpoint\", title: \"[resource_displayname[endpoint]]\", template: \"#=endpoint#\" },\r\n        ];\r\n        endpointGrid.dataBound = function() {\r\n            $(\"[name=explore]\", endpointGrid.gridElement).on(\"click\", (e) => SSOMetadata.explore(e, app, endpointGrid));\r\n        };\r\n\r\n        endpointGrid.commands.push({name: \"explore\", icon: \"k-i-search\", text: \"[resource_displayname[explore]]\" });\r\n\r\n        await endpointGrid.init();\r\n    },\r\n\r\n    setupDependencies: async function(container, meta) {\r\n        var foreignKeys = meta.Properties.filter(p => p.Name.endsWith(\"Id\") && p.Name != \"Id\" &&  \r\n            meta.Properties.filter(p2 => p2.Name == p.Name.substring(0, p.Name.length - 2) && p2.Type == \"object\").length > 0);\r\n\r\n        foreignKeys.map(fk => {\r\n            var object = meta.Properties.filter(p2 => p2.Name == fk.Name.substring(0, fk.Name.length - 2) && p2.Type == \"object\")[0];\r\n            var serverTypeInformation = object.ServerType;\r\n            var entity = window.allMeta.filter(metaEntry => metaEntry.ServerType.startsWith(serverTypeInformation))[0];\r\n            fk.RelatedEntity = entity;\r\n        });\r\n\r\n        var dependsOnGrid = new GridWidget($(\"[name=dependson]\", container), { data: foreignKeys, pageSize: 50 });\r\n        dependsOnGrid.exports = false;\r\n        dependsOnGrid.editable = false;\r\n        dependsOnGrid.columns = [\r\n            { field: \"Name\", title: \"[resource_displayname[foreignkey]]\" },\r\n            { field: \"Type\", title: \"[resource_displayname[type]]\"},\r\n            { field: \"IsRequired\", type:\"boolean\", title: \"[resource_displayname[isrequired]]\"},\r\n            { field: \"ServerType\", title: \"[resource_displayname[servertype]]\"},\r\n            { field: \"RelatedEntity.id\", title: \"[resource_displayname[relatedentity]]\"}\r\n        ];\r\n        dependsOnGrid.init();\r\n    },\r\n\r\n    explore: async function(e, app, endpointGrid) {\r\n        e.preventDefault();\r\n        var item = endpointGrid.dataItem($(e.currentTarget).closest(\"tr\"));\r\n\r\n        var exploreDialog = new Dialog({title: \"[resource_displayname[explore]]\", width: 1900, height: 805 });\r\n        exploreDialog.init(async () => {\r\n            var apiTesterComponent = await loadComponent(exploreDialog.element, \"ApiTester\");\r\n            var endpointPath = item.endpoint.substring(item.endpoint.indexOf(\"/Api/\")+5);\r\n            if(item.data.parameters && item.data.parameters.filter(r => r.name == \"queryOptions\" && r.schema[\"$ref\"].indexOf(\"ODataQueryOptions\") !== -1).length > 0) {\r\n                endpointPath += \"?$top=10\";\r\n            }\r\n\r\n            apiTesterComponent.init(app, $(\".component[name=ApiTester]\", exploreDialog.element), item.method, endpointPath);\r\n        });\r\n    },\r\n\r\n    getHttpMethods: function(swaggerPathEntry) {\r\n        return Object.keys(swaggerPathEntry).map(o => o.toUpperCase());\r\n    }\r\n};\r\n\r\n\r\n",
  "Content": "<script name=\"metadataDetails\" type=\"template\">\r\n   <div name=\"tabs\">\r\n      <ul>\r\n      \t  <li class=\"k-active\">[resource_displayname[rawjson]]</li>\r\n          <li>[resource_displayname[properties]]</li>\r\n          <li>[resource_displayname[endpoints]]</li>\r\n          <li>[resource_displayname[dependson]]</li>\r\n      </ul>\r\n      <div class=\"tab\" name=\"rawJson\"></div>\r\n      <div class=\"tab\" name=\"properties\"></div>\r\n      <div class=\"tab\" name=\"endpoints\"></div>\r\n      <div class=\"tab\" name=\"dependson\"></div>\r\n   </div>\r\n</script>",
  "LastUpdated": "2024-07-15T14:54:05.1365248+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "TemplateManagement",
  "Key": "CMS",
  "ResourceKey": "CMS",
  "Script": "TemplateManagement = {\n    init: async function (app, container) {\n        app = app || session.app;\n        container = container || $(\".component[name=TemplateManagement]\");\n\t\tapi.addToMetaCache([\n\t\t{\n\t\t\t\t\"Name\": \"Core\",\n\t\t\t\t\"Types\": [\n\t\t\t\t\t[meta[ContentManagement/Template]]\n\t\t\t\t]\n\t\t}]);\n        var config = { endpoint: \"ContentManagement/Template\", odataAppend: \"?$filter=AppId eq \" + app.Id };\n        let ds = await model.getDatasource(config);\n        await TemplateManagement.initGrid(app, container, ds);\n    },\n\n    initGrid: async function(app, container, dataSource) {\n        var grid = new GridWidget(container, dataSource);\n        grid.groupable = false;\n        grid.columns = [\n            { \n                field: \"ResourceKey\", \n                title: \"[resource_displayname[resourcekey]]\",\n                width: \"[theme[columns.small]]\"  \n            },\n            { \n                field: \"Name\", \n                title: '[resource_displayname[Name]]'\n            },\n            { \n                field: \"LastUpdated\", \n                title: \"[resource_displayname[lastupdated]]\", \n                type: \"date\", \n                format: \"{0: \" + type.dateFormat + \" HH:mm}\",\n                width: \"[theme[columns.small]]\" \n            },\n            { \n                field: \"LastUpdatedBy\", \n                title: \"[resource_displayname[lastupdated]]\",\n                width: \"[theme[columns.small]]\"\n            }\n        ];\n        grid.detailTemplate = kendo.template($(\"[name=templateDetails]\", container).html());\n        grid.commands.push({name: \"save\", icon: \"k-i-save\", text: \"[resource_displayname[save]]\"});\n        grid.commands.push({name: \"destroy\", icon: \"k-i-trash\", text: \"[resource_displayname[delete]]\"});\n        grid.toolbar = [\n        {\n            template: `<div class='btn-group btn-group-sm'>\n                <button class='btn btn-primary' name='create'>\n                    <span class='k-icon k-i-plus'></span>[resource_displayname[new]]\n                </button>\n                <button class='btn btn-primary' name='migrate'>\n                    <span class='k-icon k-i-arrow-up'></span>[resource_displayname[migrate]]\n                </button>\n            </div>`\n        }];\n        grid.detailExpand = async function(e) {\n                var item = grid.kendoObject.dataItem(e.masterRow);\n                var expandContainer = $(\"[name=editorContainer]\", e.detailRow);\n                if($(\".monaco-editor\", expandContainer).length == 0) {\n                    await TemplateManagement.templateExpand(item, expandContainer);\n                }\n        };\n        grid.dataBound = (e) => {\n            $(\"button[name=save]\", grid.gridElement).on(\"click\", (e) => TemplateManagement.save(e, grid));\n            $(\"button[name=destroy]\", grid.gridElement).on(\"click\", (e) => TemplateManagement.destroy(e, grid));\n        };\n        grid.resizable = {\n            rows: true\n        };\n        await grid.init();\n        grid.rowResize = function(e) {\n            console.log('resizing');\n            $('.monaco-editor', e.detailRow).data('monacoEditor').layout();\n        };\n        $(\"button[name=migrate]\", grid.gridElement).on(\"click\", function (e) { TemplateManagement.migrate(e, app); });\n        $(\"button[name=create]\", grid.gridElement).on(\"click\", function (e) { TemplateManagement.create(e, app, grid); });\n        return grid;\n    },\n\n    templateExpand: async function(item, container) {\n        var htmlEditor = new HTMLMonacoEditor(container[0], { code: item.RawString });\n        htmlEditor.onChange = (e) => { item.RawString = htmlEditor.getValue(); };\n        htmlEditor.init();\n    },\n\n    migrate: function (e, app) {\n        e.preventDefault();\n        var d = new Dialog({ width: 610, height: \"auto\", title: \"[resource_displayname[migrateTemplates]]\" });\n        d.template = $(\"[name=templateMigrationComponent]\").first().html();\n        d.init(() => TemplateMigration.init(app, $(\".component[name=TemplateMigration]\", d.element)));\n    },\n\n    create: async function (e, app, grid) {\n        var createTemplateDialog = new Dialog({width: 600, height: \"auto\", title: \"[resource_displayname[newtemplate]]\"});\n        createTemplateDialog.template = $(\"[name=newTemplateDialog]\").first().html();\n        createTemplateDialog.events.create = async function (e) {\n            var template = { \n                Id: 0, \n                AppId: app.Id, \n                Name: $(\"[name=name]\", createTemplateDialog.element).val(), \n                RawString: \"\", \n                ResourceKey: $(\"[name=resourcekey]\",createTemplateDialog.element).val()\n            };\n            await api.add(\"ContentManagement/Template\", template).then(() => {\n                notification.success(\"[resource_displayname[templatecreated]]\");\n                grid.refresh();\n                createTemplateDialog.events.close();\n            }).catch((e) => error(e));\n        };\n        await createTemplateDialog.init();\n    },\n\n    save: async function (e, grid) {\n        e.preventDefault();\n        var template = grid.dataItem($(e.currentTarget).closest(\"tr\"));\n        await template.save(e).then(() => {\n            notification.success(\"[resource_description[saved]]\");\n        }).catch((err) => error(err));\n    },\n\n    destroy: async function (e, grid) {\n        e.preventDefault();\n        var template = grid.dataItem($(e.currentTarget).closest(\"tr\"));\n        await template.destroy(e).then(() => {\n            notification.success(\"[resource_description[deleted]]\");\n            grid.refresh();\n        }).catch((err) => error(err));\n    }\n}\n",
  "Content": "<script type=\"text/template\" name=\"templateDetails\">\n\t<div name=\"editorContainer\"></div>\n</script>\n<script type=\"text/template\" name=\"newTemplateDialog\">\n\t<div class=\"input-group input-group-sm mb-1\">\n\t\t<span class=\"input-group-text\">[resource_displayname[name]]</span>\n\t\t<input type=\"text\" class=\"form-control\" name=\"name\" />\n\t</div>\n\t\n\t<div class=\"input-group input-group-sm mb-1\">\n\t\t<span class=\"input-group-text\">[resource_displayname[resourcekey]]</span>\n\t\t<input type=\"text\" class=\"form-control\" name=\"resourcekey\" />\n\t</div>\n\n\t<hr />\n\t\n\t<button class=\"btn btn-sm btn-primary float-end\" name=\"create\">\n\t\t<span class=\"k-icon k-i-plus\"></span>[resource_displayname[create]]\n\t</button>\n</script>\n<div name=\"templateMigrationComponent\" style=\"display:none;\">\n    [component[TemplateMigration]]\n</div>",
  "LastUpdated": "2024-11-19T18:18:30.4848209+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "TemplateMigration",
  "Key": "CMS",
  "ResourceKey": "CMS",
  "Script": "TemplateMigration = {\n    init: async function (app, container) {\n        app = app || session.app;\n        container = container || $(\".component[name=TemplateMigration]\");\n\t\tapi.addToMetaCache([\n\t\t{\n\t\t\t\t\"Name\": \"Core\",\n\t\t\t\t\"Types\": [\n\t\t\t\t\t[meta[ContentManagement/Template]],\n                    [meta[ContentManagement/App]]\n\t\t\t\t]\n\t\t}]);\n        let ds = await model.getDatasource({ endpoint: \"ContentManagement/App\" });\n        $(\"[name=app]\", container).kendoDropDownList({\n            autoBind: false,\n            optionLabel: \"[resource_displayname[selectapp]]\",\n            dataTextField: \"Name\",\n            dataValueField: \"Id\",\n            dataSource: ds\n        });\n        var templates = (await api.get(\"ContentManagement/Template?$filter=AppId eq \" + app.Id + \"&$select=Name\")).value;\n        var grid = new GridWidget($(\"[name=templateGrid]\", container), templates);\n        grid.columns = [\n            { selectable: true, width: 40 },\n            { field: \"Name\", title: \"[resource_shortdisplayname[name]]\" }\n        ];\n        grid.pageable = false;\n        grid.groupable = false;\n        grid.init(() => {\n            grid.kendoObject.dataSource.sort({ field: \"Category\", dir: \"asc\" });\n            grid.kendoObject.select(grid.kendoObject.tbody.find(\">tr\"));\n        });\n        $(\"[name=migrate]\", container).on(\"click\", async function (e) {\n            var appId = $(\"[name=app]\", container).val();\n            if($(\"[name=overwrite]\", container).is(\":checked\")) {\n                var names = grid.select().map(r => \"'\" + r.Name + \"'\");\n\n                var templateIds = await api.get(\"ContentManagement/Template?$filter=AppId eq \" + appId + \" and Name in (\" + names.join() + \")&$select=Id\");\n                await api.post(\"ContentManagement/Template/DeleteAll\", { value: templateIds.value.map(t => t.Id) })\n                    .catch((err) => error(err));\n            }\n            var packages = await TemplateMigration.getPackages(app, grid);\n            await api.add(\"Packaging/Package/ImportThis?appId=\" + appId, packages);\n            notification.success(\"[resource_displayname[migrated]]\");\n        });\n    },\n\n    getPackages: async function (app, grid) {\n        var packages = [{\n            Id: \"00000000-0000-0000-0000-000000000000\",\n            Name: \"Templates\",\n            Description: \"Generated by TemplateMigration\",\n            Category: \"Dynamic\",\n            SourceApi: session.apiRoot,\n            Items: [\n                await TemplateMigration.getPackage(app, grid)\n            ]\n        }];\n        return packages;\n    },\n\n    getPackage: async function (app, grid) {\n        var names = grid.select().map(r => \"'\" + r.Name + \"'\");\n        var data = await api.get(\"ContentManagement/Template?$filter=AppId eq \" + app.Id + \" and Name in (\" + names.join() + \")\");\n        for (var i = 0; i < data.value.length; i = i + 1) {\n            delete data.value[i].Id;\n        }\n        return {\n            Id: \"00000000-0000-0000-0000-000000000000\",\n            PackageId: \"00000000-0000-0000-0000-000000000000\",\n            Type: \"ContentManagement/Template\",\n            Data: JSON.stringify(data.value)\n        };\n    }\n};",
  "Content": "<div class=\"input-group input-group-sm mb-1\">\n    <span class=\"input-group-text\">[resource_displayname[app]]</span>\n    <input type=\"text\" class=\"form-control\" name=\"app\" />\n</div>\n\n<div class=\"input-group input-group-sm mb-1\">\n    <span class=\"input-group-text\">[resource_displayname[overwrite]]</span>\n    <div class=\"input-group-text\">\n        <input type=\"checkbox\" class=\"form-check-input\" name=\"overwrite\" />\n    </div>\n</div>\n\n<div name=\"templateGrid\"></div>\n\n<hr />\n\n<button class=\"btn btn-sm btn-primary float-end\">\n    <span class=\"k-icon k-i-arrow-up\"></span>[resource_displayname[migrate]]\n</button>",
  "LastUpdated": "2024-11-19T18:18:31.1450363+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "TestimonialManagement",
  "Key": "CMS",
  "ResourceKey": "CMS",
  "Script": "TestimonialManagement = {\n    init: async function(app, container) {\n        app = app || session.app;\n        container = container || $(\".component[name=TestimonialManagement]\");\n        $(\"[name=testimonialToolbar]\").kendoToolBar({\n            items: [\n                {\n                    text: \"[resource_displayname[new]]\",\n                    type: \"button\",\n                    template: `<div class=\"btn-group btn-group-sm\">\n                        <button class=\"btn btn-primary\" name=\"addTestimonial\">\n                            <span class=\"k-icon k-i-plus\"></span>[resource_displayname[new]]\n                        </button>\n                    </div>`\n                },\n            ]\n        });\n        //Gets the html files and inits the foldermanagement component\n        var testimonialFiles = (await api.get(\"DocumentManagement/Folder?$filter=AppId eq \" + app.Id + \" and Path eq 'content/testimonials/html'&$expand=Files\")).value[0];\n        FolderManagement.init(app, $(\".component[name=FolderManagement]\", $(\"[name=testimonialsFolder]\", container)), testimonialFiles, true, true);\n\n        $(\"[name=addTestimonial]\", container).off(\"click\").on(\"click\", async (e) => await TestimonialManagement.addTestimonial(e, container));\n    },\n\naddTestimonial: async function(e) {\n    e.preventDefault();\n    var newTestimonyDialog = new Dialog({ width: 600, height: \"auto\", title: \"[resource_displayname[newtestimony]]\" });\n    newTestimonyDialog.template = $(\"[name=newTestimonyDialog]\").html();\n    newTestimonyDialog.events.create = async function (e) {\n        e.preventDefault();\n        var companyName = $(\"[name=companyName]\").val();\n        var contactName = $(\"[name=contactName]\").val();\n        var jobTitle = $(\"[name=jobTitle]\").val();\n        var content = $(\"[name=content]\").val();\n        var htmlContent = `<img src=\"[api[root]]DMS/content/testimonials/images/${companyName}logo.png\" style=\"margin: 15px 0 -5px 0; max-height: 70px;\"> <br>`;\n\n        if (contactName) {\n            htmlContent += `<p> ${contactName}</p>`;\n        }\n\n        if (jobTitle) {\n            htmlContent += `<p> ${jobTitle}</p>`;\n        }\n\n        htmlContent += `<p style=\"padding-top: 15px; font-size: 14px; font-weight: normal;\">\"${content}\"</p>`;\n\n        await api.sendRaw(\"POST\", \"DMS/Content/Testimonials/html/\" + companyName + \"Testimonial.html\", htmlContent, \"text/plain\").then(async () => {\n            notification.success(\"[resource_displayname[testimonialsubmitted]]\");\n        }).catch((err) => error(err));\n        newTestimonyDialog.events.close();\n    };\n    newTestimonyDialog.init(() => {\n        TestimonialManagement.initCompanyLogoFileUpload(newTestimonyDialog.element);\n    });\n},\n\n\n    initCompanyLogoFileUpload: async function (container) {\n        var companyLogoDrop = new FileDropContainerWidget($(\"[name=companyLogo]\", container));\n        companyLogoDrop.events.drop = async (e) => {\n            e.preventDefault();\n            e.stopPropagation();\n            var companyName = $(\"[name=companyName]\", container).val();\n            var files = e.target.files;\n            if (files.length === 0) {\n                files = e.originalEvent.dataTransfer.files;\n            }\n            if (files.length > 1) {\n                error(\"[resource_displayname[onlyonefile]]\");\n                return;\n            }\n            var file = files[0];\n            await api.file.upload(\"Content/Testimonials/Images/\" + companyName + \"Logo.png\", file);\n            $(\"[name=companyLogo]\", container).attr(\"src\", \"[api[root]]/DMS/Content/Testimonials/Images/\" + companyName + \"Logo.png?time=\" + (new Date()).toISOString());\n        };\n        companyLogoDrop.init();\n    },\n    \n};",
  "Content": "<div name=\"testimonialToolbar\"></div>\n<div name=\"testimonialsFolder\">\n        [component[FolderManagement]]\n</div>\n\n<script type=\"text/template\" name=\"newTestimonyDialog\">\n\t<ul class=\"fieldList testimonials\">\n    <li>\n        <label>[resource_displayname[companyname]]</label>\n        <div class=\"value\">\n            <input type=\"text\" name=\"companyName\" />\n        </div>\n\t</li>\n\t<li>\n\t\t<label>[resource_displayname[companyLogo]]</label>\n\t\t<div class=\"value\">\n\t\t\t<p>[resource_displayname[draganddrop]]</p>\n\t\t\t<img name=\"companyLogo\" />\n\t\t        </div>\n\t</li>\n    <li>\n        <label>[resource_displayname[contactname]]</label>\n        <div class=\"value\">\n            <input type=\"text\" name=\"contactName\" />\n        </div>\n    </li>\n\t<li>\n\t\t<label>[resource_displayname[jobtitle]]</label>\n\t\t<div class=\"value\">\n\t\t\t<input type=\"text\" name=\"jobTitle\" />\n\t        </div>\n\t</li>\n\t<li>\n\t\t<label>[resource_displayname[content]]</label>\n\t\t<div class=\"value\">\n\t\t\t<input type=\"textarea\" name=\"content\" />\n        </div>\n\t</li>\n    <hr>\n<div class=\"value\">\n   <button name=\"create\"><span class='k-icon k-i-plus'></span>[resource_shortdisplayname[create]]</input>\n</div>\n</ul>\n</script>",
  "LastUpdated": "2024-11-19T18:18:30.7764359+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "ThemeBuilder",
  "Key": "CMS",
  "ResourceKey": "CMS",
  "Script": "ThemeBuilder = {\r\n    init: async function(app, container, theme) {\r\n        app = app || session.app;\r\n        container = container || $(\".component[name=ThemeBuilder]\");\r\n        if(!theme)\r\n            return;\r\n\r\n        if(!theme.colours.charts) {\r\n            theme.colours.charts = [\r\n                \"#000000\",\r\n                \"#000000\",\r\n                \"#000000\",\r\n                \"#000000\",\r\n                \"#000000\",\r\n                \"#000000\"\r\n            ];\r\n        }\r\n\r\n        if(!theme.cultureFlagLayout) {\r\n            theme.cultureFlagLayout = \"text\";\r\n        }\r\n\r\n        ThemeBuilder.initNavTabIds(app, container);\r\n\r\n        var coloursComponent = await loadComponent($('[name=colours]', container), 'Colours');\r\n\r\n        var model = new kendo.observable(theme);\r\n        kendo.bind(container, model);\r\n        $(container).data(\"model\", model);\r\n        \r\n        await ThemeBuilder.addClickListenerForComponents(app, container, model);\r\n        \r\n        await coloursComponent.init(app, $('[name=colours]', container), model);\r\n\r\n    \r\n        $(\"li[data-container='border']\").on('click', () => {\r\n            border.initSliders(container, model);\r\n        });\r\n        $(\"li[data-container='shadows']\").on('click', () => {\r\n            shadows.initSliders(container, model);\r\n        });\r\n    },\r\n\r\n    initNavTabIds: function(app, container) {\r\n        var id = Guid();\r\n        var template = $('script[name=edit-theme]', container).html();\r\n        template = template.replaceAll('{ID}', id);\r\n\r\n        container.append(template);\r\n    },\r\n\r\n    addClickListenerForComponents: async function(app, container, model) {\r\n        var buttons = $('button[role=tab]', container);\r\n        buttons.each((button) => {\r\n            var button = $(buttons[button], container);\r\n            var target = $(button.attr('data-bs-target'));\r\n            var component = target.attr('data-component');\r\n\r\n            if(component != null) {\r\n                button.click(async () => {\r\n                    var existing = $(`.component[name=${component}]`, $(target, container));\r\n\r\n                    if(existing.length == 0) {\r\n                        await loadComponent($(target, container), component, async (c) => {\r\n                            await c.init(app, $(`.component[name=${component}]`, $(target, container)), model);\r\n                        });\r\n                    }\r\n                });\r\n            }\r\n        });\r\n    },\r\n\r\n    build: function(container) {\r\n        return $(container).data(\"model\").toJSON();\r\n    }\r\n}",
  "Content": "<script type=\"text/template\" name=\"edit-theme\">\r\n<div class=\"tab-control\" name=\"tabs\">\r\n\t<nav>\r\n\t\t<div class=\"nav nav-tabs\" id=\"app-theme-nav-tab-{ID}\" role=\"tablist\">\r\n\t\t\t<button class=\"nav-link bg active\" id=\"app-theme-colours-tab-{ID}\" data-bs-toggle=\"tab\" data-bs-target=\"#app-theme-colours-{ID}\" type=\"button\" role=\"tab\" aria-controls=\"app-theme-colours-{ID}\" aria-selected=\"true\">\r\n\t\t\t\t<span class=\"k-icon k-i-edit\"></span>[resource_displayname[colours]]\r\n\t\t\t</button>\r\n\t\t\t<button class=\"nav-link bg\" id=\"app-theme-font-tab-{ID}\" data-bs-toggle=\"tab\" data-bs-target=\"#app-theme-font-{ID}\" type=\"button\" role=\"tab\" aria-controls=\"app-theme-font-{ID}\" aria-selected=\"false\" tabindex=\"-1\">\r\n\t\t\t\t<span class=\"k-icon k-i-palette\"></span>[resource_displayname[font]]\r\n\t\t\t</button>\r\n\t\t\t<button class=\"nav-link bg\" id=\"app-theme-border-tab-{ID}\" data-bs-toggle=\"tab\" data-bs-target=\"#app-theme-border-{ID}\" type=\"button\" role=\"tab\" aria-controls=\"app-theme-border-{ID}\" aria-selected=\"false\" tabindex=\"-1\">\r\n\t\t\t\t<span class=\"k-icon k-i-globe\"></span>[resource_displayname[border]]\r\n\t\t\t</button>\r\n\t\t\t<button class=\"nav-link bg\" id=\"app-theme-notifications-tab-{ID}\" data-bs-toggle=\"tab\" data-bs-target=\"#app-theme-notifications-{ID}\" type=\"button\" role=\"tab\" aria-controls=\"app-theme-notifications-{ID}\" aria-selected=\"false\" tabindex=\"-1\">\r\n\t\t\t\t<span class=\"k-icon k-i-grid-layout\"></span>[resource_displayname[notifications]]\r\n\t\t\t</button>\r\n\t\t\t<button class=\"nav-link bg\" id=\"app-theme-shadows-tab-{ID}\" data-bs-toggle=\"tab\" data-bs-target=\"#app-theme-shadows-{ID}\" type=\"button\" role=\"tab\" aria-controls=\"app-theme-shadows-{ID}\" aria-selected=\"false\" tabindex=\"-1\">\r\n\t\t\t\t<span class=\"k-icon k-i-css\"></span>[resource_displayname[shadows]]\r\n\t\t\t</button>\r\n\t\t\t<button class=\"nav-link bg\" id=\"app-theme-etc-tab-{ID}\" data-bs-toggle=\"tab\" data-bs-target=\"#app-theme-etc-{ID}\" type=\"button\" role=\"tab\" aria-controls=\"app-theme-etc-{ID}\" aria-selected=\"false\" tabindex=\"-1\">\r\n\t\t\t\t<span class=\"k-icon k-i-source-code\"></span>[resource_displayname[etc]]\r\n\t\t\t</button>\r\n\t\t</div>\r\n\t</nav>\r\n\r\n\t<div class=\"tab-content\" id=\"app-theme-nav-tab-{ID}Content\">\r\n\t\t<div class=\"tab-pane fade active show\" id=\"app-theme-colours-{ID}\" role=\"tabpanel\" aria-labelledby=\"app-management-config-tab-{ID}\" name=\"colours\"></div>\r\n\t\t<div class=\"tab-pane fade\" id=\"app-theme-font-{ID}\" role=\"tabpanel\" aria-labelledby=\"app-theme-font-tab-{ID}\" name=\"fonts\" data-component=\"Font\"></div>\r\n\t\t<div class=\"tab-pane fade\" id=\"app-theme-border-{ID}\" role=\"tabpanel\" aria-labelledby=\"app-theme-border-tab-{ID}\" name=\"borders\" data-component=\"Border\"></div>\r\n\t\t<div class=\"tab-pane fade\" id=\"app-theme-notifications-{ID}\" role=\"tabpanel\" aria-labelledby=\"app-theme-notifications-tab-{ID}\" name=\"notifications\" data-component=\"Notifications\"></div>\r\n\t\t<div class=\"tab-pane fade\" id=\"app-theme-shadows-{ID}\" role=\"tabpanel\" aria-labelledby=\"app-theme-shadows-tab-{ID}\" name=\"shadows\" data-component=\"Shadows\"></div>\r\n\t\t<div class=\"tab-pane fade\" id=\"app-theme-etc-{ID}\" role=\"tabpanel\" aria-labelledby=\"app-theme-etc-tab-{ID}\" name=\"etc\" data-component=\"Etc\"></div>\r\n\t</div>\r\n</div>\r\n</script>",
  "LastUpdated": "2024-11-19T18:18:31.0343546+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "TopNav",
  "Key": "CMS",
  "ResourceKey": "CMS",
  "Script": "TopNav = {\n    init: function(app, container, page) {\n        app = app || session.app;\n        container = container || $(\".component[name=TopNav]\");\n        page = page || session.page;\n        var query = \"ContentManagement/Page?$filter=AppId eq \" + app.Id + \" and ParentId eq null and ShowOnMenus eq true&$orderby=Order asc&$expand=PageInfo,Pages($filter=ShowOnMenus eq true;$orderby=Order asc;$expand=PageInfo,Pages($filter=ShowOnMenus eq true;$orderby=Order asc;$expand=PageInfo))\";\n        api.get(query).then(function(pages) {\n            var menu = $(\"ul[name=menu]\", container);\n            menu.empty();\n            TopNav.generateNavMenu(menu, TopNav.orderedVisiblePages(pages.value || []));\n        });\n    },\n\n    generateNavMenu: function(nav, pages) {\n        for(var i = 0; i < pages.length; i++) {\n            var page = pages[i];\n            var visibleChildren = TopNav.orderedVisiblePages(page.Pages || []);\n\n            if(visibleChildren.length > 0)\n                TopNav.generateNavDropdown(nav, page, visibleChildren);\n            else\n                TopNav.generateNavItem(nav, page, false);\n        }\n    },\n\n    generateNavItem: function(nav, page, dropdown) {\n        var navItem = $('<li class=\"' + TopNav.itemClass(page, dropdown) + '\"></li>');\n        navItem.append($('<a href=\"/' + TopNav.pathOf(page) + '\" class=\"' + TopNav.linkClass(page, dropdown) + '\">' + TopNav.titleOf(page) + '</a>'));\n        nav.append(navItem);\n    },\n\n    generateNavDropdown: function(nav, page, children) {\n        var navItem = $('<li class=\"' + TopNav.itemClass(page, true) + '\"></li>');\n        navItem.append($('<a href=\"/' + TopNav.pathOf(page) + '\" class=\"nav-link dropdown-toggle ' + (TopNav.isCurrentPage(page, true) ? 'active' : '') +'\" aria-expanded=\"false\">' + TopNav.titleOf(page) + '</a>'));\n        var dropdown = $('<ul class=\"submenu dropdown-menu\"></ul>');\n        for(var i = 0; i < children.length; i++) {\n            TopNav.generateNavDropdownItem(dropdown, children[i]);\n        }\n        navItem.append(dropdown);\n        nav.append(navItem);\n    },\n\n    generateNavDropdownItem: function(dropdown, page) {\n        var visibleChildren = TopNav.orderedVisiblePages(page.Pages || []);\n        var navItem = $('<li class=\"' + (visibleChildren.length > 0 ? 'dropdown' : '') + '\"></li>');\n        navItem.append($('<a href=\"/' + TopNav.pathOf(page) + '\" class=\"' + TopNav.linkClass(page, true) + '\">' + TopNav.titleOf(page) + '</a>'));\n        if(visibleChildren.length > 0) {\n            var submenu = $('<ul class=\"submenu dropdown-menu\"></ul>');\n            for(var i = 0; i < visibleChildren.length; i++) {\n                TopNav.generateNavDropdownItem(submenu, visibleChildren[i]);\n            }\n            navItem.append(submenu);\n        }\n        dropdown.append(navItem);\n    },\n\n    orderedVisiblePages: function(pages) {\n        return pages\n            .filter(function(page) { return page && page.ShowOnMenus === true; })\n            .sort(function(left, right) { return (left.Order || 0) - (right.Order || 0); });\n    },\n\n    pathOf: function(page) {\n        return page.Path || '';\n    },\n\n    titleOf: function(page) {\n        if(!page.PageInfo || page.PageInfo.length === 0)\n            return page.Name;\n\n        var culture = ($(\"html\").attr(\"lang\")) ? $(\"html\").attr(\"lang\") : \"\";\n        var matchingPageInfos = page.PageInfo.filter(function(pi){\n            return pi.CultureId == culture;\n        });\n\n        if(matchingPageInfos.length > 0)\n            return matchingPageInfos[0].Title;\n\n        var defaultPageInfo = page.PageInfo.filter(function(pi){\n            return pi.CultureId == \"\";\n        })[0];\n\n        return defaultPageInfo ? defaultPageInfo.Title : page.Name;\n    },\n\n    itemClass: function(page, dropdown) {\n        var isCurrentPage = TopNav.isCurrentPage(page, false);\n\n        if(dropdown == true)\n            return isCurrentPage\n                ? 'nav-item dropdown active'\n                : 'nav-item dropdown';\n        return isCurrentPage\n            ? 'nav-item active'\n            : 'nav-item';\n    },\n\n    linkClass: function(page, dropdown) {\n        var isCurrentPage = TopNav.isCurrentPage(page, false);\n        if(dropdown == true)\n            return isCurrentPage ? 'dropdown-item active' : 'dropdown-item';\n        return isCurrentPage ? 'nav-link active' : 'nav-link';\n    },\n\n    isCurrentPage: function(page, isParent) {\n        var currentPath = (session.page && session.page.Path) ? session.page.Path : '';\n        var pagePath = page.Path || '';\n\n        if(isParent == true)\n            return pagePath !== '' && currentPath.indexOf(pagePath + '/') === 0;\n\n        return currentPath === pagePath;\n    }\n};",
  "Content": "<nav class=\"navbar navbar-expand-lg\">\n    <div class=\"container-fluid\">\n        <button class=\"navbar-toggler\" type=\"button\" data-bs-toggle=\"collapse\" data-bs-target=\"#navbarText\" aria-controls=\"navbarText\" aria-expanded=\"false\" aria-label=\"Toggle navigation\">\n            <span class=\"navbar-toggler-icon\"></span>\n        </button>\n        <div class=\"collapse navbar-collapse\" id=\"navbarText\">\n            <ul class=\"navbar-nav me-auto mb-2 mb-lg-0\" name=\"menu\"></ul>\n        </div>\n    </div>\n</nav>\n<style scoped>\n    .component[name=TopNav] .navbar-nav li { position: relative; }\n    .component[name=TopNav] .navbar-nav li:hover > .submenu { display: block; }\n    .component[name=TopNav] .navbar-nav .submenu { display: none; margin-top: 0; }\n    .component[name=TopNav] .navbar-nav .submenu .submenu { left: 100%; top: 0; }\n</style>",
  "LastUpdated": "2024-11-19T18:18:30.3300071+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "DetailedNav",
  "Key": "Content Management",
  "ResourceKey": "DetailedNav",
  "Script": "DetailedNav = {\n    init: async function (app, container) {\n        app = app || session.app;\n        container = container || $(\".component[name=DetailedNav]\");\n        container.empty();\n\n        const grid = $('<div class=\"docs-nav-grid\"></div>');\n        container.append(grid);\n\n        var query = \"ContentManagement/Page?$filter=AppId eq \" + session.app.Id + \" and ParentId eq \" + session.page.PageId + \"&$expand=PageInfo,Pages($expand=PageInfo,Pages($expand=PageInfo))\";\n        await api.get(query).then(function (pages) {\n            const visiblePages = (pages.value || [])\n                .filter(page => page.ShowOnMenus)\n                .sort((a, b) => (a.Order || 0) - (b.Order || 0));\n\n            grid.attr(\"data-columns\", Math.min(3, Math.max(1, visiblePages.length)));\n\n            for (const page of visiblePages) {\n                DetailedNav.createItem(page, grid);\n            }\n        });\n    },\n    createItem: function (page, parentElement) {\n        if (page.Path === null) { page.Path = \"\"; }\n        const card = $('<article class=\"docs-nav-card\"></article>');\n        const title = $('<h3 class=\"docs-nav-title\"></h3>').text(DetailedNav.titleOf(page));\n        const description = $('<p></p>').text(DetailedNav.descriptionOf(page));\n        const actions = $('<div class=\"docs-nav-actions\"></div>');\n        const link = $('<a class=\"btn btn-primary\"></a>')\n            .attr('href', '/' + page.Path)\n            .text('[resource_displayname[go]]');\n\n        actions.append(link);\n        card.append(title);\n        card.append(description);\n        card.append(actions);\n        parentElement.append(card);\n    },\n    titleOf: function (page) {\n        const culture = $(\"html\").attr(\"lang\");\n        for (const info of page.PageInfo) { if (info.CultureId === culture) return info.Title; }\n        for (const info of page.PageInfo) { if (info.CultureId === \"\") return info.Title; }\n        return \"unknown\";\n    },\n    descriptionOf: function (page) {\n        const culture = $(\"html\").attr(\"lang\");\n        for (const info of page.PageInfo) { if (info.CultureId === culture) return info.Description; }\n        for (const info of page.PageInfo) { if (info.CultureId === \"\") return info.Description; }\n        return \"unknown\";\n    }\n};",
  "Content": "",
  "LastUpdated": "2026-05-05T14:31:12.0993113+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "CultureManagement",
  "Key": "Content Management",
  "ResourceKey": "CultureManagement",
  "Script": "CultureManagement = {\n    init: async function(app, container) {\n        app = app || session.app;\n        container = container || $(\".component[name=CultureManagement]\");\n        api.addToMetaCache([{\n            Name: \"Core\",\n            Types: [\n                [meta[ContentManagement/AppCulture]],\n                [meta[ContentManagement/Culture]]\n            ]\n        }]);\n        var ds = await model.getDatasource({ endpoint: \"ContentManagement/AppCulture\", odataAppend: \"?$filter=AppId eq \" + app.Id + \"&$expand=Culture\" });\n        var cultureGrid = new GridWidget($(\"[name=cultureGrid]\", container), ds);\n        cultureGrid.groupable = false;\n        cultureGrid.filterable = true;\n        cultureGrid.columns = [\n            { field: \"CultureId\", title: \"[resource_displayname[cultureid]]\" },\n            { field: \"Culture.Name\", title: \"[resource_displayname[name]]\" }\n        ];\n        cultureGrid.commands.push({ name: \"save\", icon: \"k-i-save\", text: \"[resource_displayname[save]]\" });\n        cultureGrid.commands.push({ name: \"delete\", icon: \"k-i-trash\", text: \"[resource_displayname[delete]]\" });\n        cultureGrid.dataBound = function() {\n            $(\"[name=save]\", cultureGrid.gridElement).off(\"click\").on(\"click\", async function(e) {\n                e.preventDefault();\n                await cultureGrid.dataItem($(e.currentTarget).closest(\"tr\")).save().then(() => notification.success(\"[resource_displayname[saved]]\")).catch((err) => error(err));\n            });\n            $(\"[name=delete]\", cultureGrid.gridElement).off(\"click\").on(\"click\", async function(e) {\n                e.preventDefault();\n                var item = cultureGrid.dataItem($(e.currentTarget).closest(\"tr\"));\n                await api.destroy(\"ContentManagement/AppCulture(AppId=\" + item.AppId + \",CultureId='\" + item.CultureId + \"')\").then(() => cultureGrid.refresh()).catch((err) => error(err));\n            });\n        };\n        await cultureGrid.init();\n    }\n};",
  "Content": "<div name=\"cultureGrid\"></div>",
  "LastUpdated": "2026-04-20T10:20:08.3493735+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Component",
                Data = """
{
  "Name": "RolePrivManagement",
  "Key": "Content Management",
  "ResourceKey": "RolePrivManagement",
  "Script": "RolePrivManagement = {\n    init: async function(app, container, role) {\n        app = app || session.app;\n        container = container || $(\".component[name=RolePrivManagement]\");\n        if(!role) { return; }\n        api.addToMetaCache([{\n            Name: \"AppSecurity\",\n            Types: [\n                [meta[AppSecurity/Privilege]],\n                [meta[AppSecurity/Role]]\n            ]\n        }]);\n        var privileges = await api.get(\"AppSecurity/Privilege?$orderby=Type asc,Operation asc\");\n        var rolePrivs = (role.Privs || \"\").split(\",\").map(function(priv) { return priv.trim(); }).filter(function(priv) { return priv.length > 0; });\n        var data = (privileges.value || []).map(function(privilege) {\n            privilege.Selected = rolePrivs.indexOf(privilege.Id) >= 0;\n            return privilege;\n        });\n        var gridContainer = $(\"[name=privilegeGrid]\", container);\n        if(gridContainer.length === 0) { gridContainer = container; }\n        var grid = new GridWidget(gridContainer, { data: data });\n        grid.groupable = false;\n        grid.filterable = true;\n        grid.pageable = false;\n        grid.editable = true;\n        grid.toolbar = `<div class=\"btn-group btn-group-sm\"><button class=\"btn btn-primary\" name=\"savePrivs\"><span class=\"k-icon k-i-save\"></span>[resource_displayname[save]]</button></div>`;\n        grid.columns = [\n            { field: \"Selected\", title: \" \", width: 80 },\n            { field: \"Type\", title: \"[resource_displayname[type]]\", editable: false },\n            { field: \"Operation\", title: \"[resource_displayname[operation]]\", editable: false },\n            { field: \"Description\", title: \"[resource_displayname[description]]\", editable: false }\n        ];\n        await grid.init();\n        $(\"[name=savePrivs]\", grid.gridElement).off(\"click\").on(\"click\", async function(e) {\n            e.preventDefault();\n            var selected = grid.dataSource().data().filter(function(privilege) { return privilege.Selected === true; }).map(function(privilege) { return privilege.Id; });\n            role.Privs = selected.join(\",\");\n            await api.update(\"AppSecurity/Role(\" + role.Id + \")\", role).then(function() {\n                notification.success(\"[resource_displayname[saved]]\");\n            }).catch(function(err) { error(err); });\n        });\n    }\n};",
  "Content": "<div name=\"privilegeGrid\"></div>",
  "LastUpdated": "2026-04-20T10:20:08.3493735+01:00"
}
"""
            },
        ]
    };

    static Package Pages => new()
    {
        Name = "Content Management Pages",
        Category = "CMS",
        Description = "Content Management Pages.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/Page",
                Data = """
{
  "Path": "Admin/AppManagement",
  "Name": "App Management",
  "ResourceKey": "",
  "ShowOnMenus": true,
  "Order": 4,
  "LastUpdated": "2024-04-04T16:30:04.9492482+01:00",
  "Layout": "Default",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "[component[appmanagement]]"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "Manage various aspects of the application, including the app's Themes (colours and border styles for example), Cultures (what language translations we support) and general things like page Layouts and Templates.",
      "Keywords": "manage, admin, app, roles, cultures",
      "Title": "App Management"
    }
  ]
}
"""
            },
            new PackageItem
            {
                Type = "Core/Page",
                Data = """
{
  "Path": "Admin",
  "Name": "Admin",
  "ResourceKey": "",
  "ShowOnMenus": true,
  "Order": 9,
  "LastUpdated": "2024-04-04T15:46:42.0374745+01:00",
  "Layout": "Default",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "[component[detailednav]]"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "Admin",
      "Keywords": "Admin",
      "Title": "Admin"
    }
  ]
}
"""
            },
            new PackageItem
            {
                Type = "Core/Page",
                Data = """
{
  "Path": "Documentation",
  "Name": "Documentation",
  "ResourceKey": "",
  "ShowOnMenus": true,
  "Order": 4,
  "LastUpdated": "2024-08-22T12:04:11.6084004+01:00",
  "Layout": "Documentation",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "[component[detailednav]]"
    },
    {
      "CultureId": "en-GB",
      "Name": "body",
      "Html": "[component[detailednav]]"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "Documentation",
      "Keywords": "Documentation",
      "Title": "Documentation"
    }
  ]
}
"""
            },
            new PackageItem
            {
                Type = "Core/Page",
                Data = """
{
  "Path": "Admin/CoreManagement",
  "Name": "Core Management",
  "ResourceKey": "",
  "ShowOnMenus": true,
  "Order": 9,
  "LastUpdated": "2024-04-04T16:48:39.5776288+01:00",
  "Layout": "Default",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "[component[CoreManagement]]"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "Manage the applications in this environment. You can open App Management for each app from this page.",
      "Keywords": "Core Management",
      "Title": "Core Management"
    }
  ]
}
"""
            },
            new PackageItem
            {
                Type = "Core/Page",
                Data = """
{
  "Path": "Admin/CommonCache",
  "Name": "Common Cache Endpoint",
  "ResourceKey": "",
  "ShowOnMenus": true,
  "Order": 0,
  "LastUpdated": "2024-04-04T16:54:01.9656924+01:00",
  "Layout": "Default",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": " [component[CommonCacheEndpoint]]"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "Contains any shared Components, Resources and Scripts that are commonly used across all portals within this environment.",
      "Keywords": "Common Cache",
      "Title": "Common Cache"
    }
  ]
}
"""
            },
            new PackageItem
            {
                Type = "Core/Page",
                Data = """
{
  "Path": "Documentation/CoreDocumentation",
  "Name": "Core Documentation",
  "ShowOnMenus": true,
  "Order": 0,
  "LastUpdated": "2024-08-22T12:04:12.1299873+01:00",
  "Layout": "Documentation",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "<h2 style=\"background-color:#ffffff;font-size:21.6px;\">Core Documentation</h2><p style=\"background-color:#ffffff;font-size:12px;\">Core houses the functionality for managing things like the client's theme and other visual aspects of the system.</p><p style=\"background-color:#ffffff;font-size:12px;\">This section of our documentation contains all of the articles required to understand our Core system. It offers explanations of how to manage a client's theme, upload files to our Corporate LinX Document Management system (DMS) among other important things you might want to customise.</p><p style=\"background-color:#ffffff;font-size:12px;\">Follow these links to find out more about our Core system:</p><p>[component[detailednav]]\n </p>"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "This page provides access to how-to guides for cCoder platform functionality.",
      "Title": "Core Documentation"
    }
  ]
}
"""
            },
            new PackageItem
            {
                Type = "Core/Page",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/AppManagement",
  "Name": "App Management",
  "ShowOnMenus": true,
  "Order": 0,
  "LastUpdated": "2024-08-22T12:04:12.1580715+01:00",
  "Layout": "Documentation",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "<div class=\"documentation\"><h2>Accessing App Management </h2><p class=\"mainText\">Everything you need to manage an app, from its theme and its layout to its offer generation\n        schedule. The page sits under the Admin tab as this is predominantly used by admins.\n    </p><p class=\"mainText\">Once you have successfully logged into the portal, you can access the page by hovering over the\n        <strong>&ldquo;Admin&rdquo;</strong> button in the navigation bar and clicking <strong>&ldquo;App Management&rdquo;</strong> button.\n    </p><h2>The UI</h2><p class=\"mainText\">When you access the page, you&rsquo;re greeted with something that looks like this: </p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/AppManagementUI-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">Each Tab in on this page allows you to manage different aspects of the portal you&rsquo;re in. For\n        example, the theming tab allows you to manage the app&rsquo;s colour scheme, logos and other images that are used site\n        wide. Each tab and its functionality are broken down further in their specific pages:\n    </p><h2>App Migration</h2><p class=\"mainText\">When you click the <strong>&ldquo;Migrate&rdquo;</strong> button will open a dialog that looks something\n        like this:</p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/AppMigrationDialog-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">Here is where you can migrate certain elements of the app to different environment, for example\n        if you want any changes you&rsquo;ve made to Pages and Resources in the app you&rsquo;re currently in and you want them to\n        be &ldquo;pushed&rdquo; to Test, you would select the Test environment from the drop down list, enter your password and\n        select the Pages and Resources using the check boxes. To confirm this action you click the migrate button.\n        Within a few seconds to a minute your changes should be visible in the test environment.\n    </p><h2>App Management Tabs </h2><p class=\"mainText\">Each tab has it&rsquo;s own individual &ldquo;how-to&rdquo; guide, there you can find information on what each\n        tab is used for and how to achieve certain goals with it. Clicking on the relevant button below will take you to\n        it&rsquo;s documentation page and give you the information you need. </p>[component[DetailedNav]]\n</div>"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "How to access and manage different aspects of the application.",
      "Title": "App Management"
    }
  ]
}
"""
            },
            new PackageItem
            {
                Type = "Core/Page",
                Data = """
{
  "Path": "Admin/ContentManagement",
  "Name": "Content Management",
  "ShowOnMenus": true,
  "Order": 0,
  "LastUpdated": "2024-09-06T15:45:48.1505582+01:00",
  "Layout": "Default",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "[component[cms]]"
    },
    {
      "CultureId": "en-GB",
      "Name": "body",
      "Html": "[component[cms]]"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "Manage the app's pages, including what's displayed on them, who has access to them and its name.",
      "Title": "Content Management"
    }
  ]
}
"""
            },
            new PackageItem
            {
                Type = "Core/Page",
                Data = """
{
  "Path": "Documentation/SSODocumentation",
  "Name": "SSO Documentation",
  "ShowOnMenus": true,
  "Order": 0,
  "LastUpdated": "2024-08-22T12:04:13.0596338+01:00",
  "Layout": "Documentation",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "<h2 style=\"background-color:#ffffff;font-size:21.6px;\">SSO Documentation</h2><p style=\"background-color:#ffffff;font-size:12px;\">SSO houses the functionality for managing things like your user profile and logging in and registering.</p><p style=\"background-color:#ffffff;font-size:12px;\">This section of our documentation contains all of the articles required to understand our SSO system. It offers explanations of how to manage your user profile as well as how to reset your password should you forget it.</p><p style=\"background-color:#ffffff;font-size:12px;\">Follow these links to find out more about our SSO system:</p><p>[component[detailednav]]\n </p>"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "This page provides access to how-to guides for platform sign-in and identity functionality.",
      "Title": "SSO Documentation"
    }
  ]
}
"""
            },
            new PackageItem
            {
                Type = "Core/Page",
                Data = """
{
  "Path": "Documentation/Metadata",
  "Name": "Metadata",
  "ShowOnMenus": true,
  "Order": 0,
  "LastUpdated": "2025-02-19T11:00:12.7221335+00:00",
  "Layout": "Documentation",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "[component[metadata]]"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "This page provides crucial information about our API, detailing what endpoints and calls can be made to carry out different tasks.",
      "Keywords": "Metadata",
      "Title": "Metadata"
    }
  ]
}
"""
            },
            new PackageItem
            {
                Type = "Core/Page",
                Data = """
{
  "Path": "Documentation/SSODocumentation/SSOAPI/SSOMetadata",
  "Name": "SSO Metadata",
  "ShowOnMenus": true,
  "Order": 0,
  "LastUpdated": "2024-08-22T12:04:13.1822007+01:00",
  "Layout": "Documentation",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "[component[SSOMetadata]]"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "SSO Metadata",
      "Keywords": "SSO Metadata",
      "Title": "SSO Metadata"
    }
  ]
}
"""
            },
        ]
    };

    static Package Resources => new()
    {
        Name = "Content Management Resources",
        Category = "CMS",
        Description = "Content Management Resources.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "Culture",
  "Key": "Key",
  "Name": "Name",
  "DisplayName": "DisplayName",
  "ShortDisplayName": "ShortDisplayName",
  "Description": "Description",
  "LastUpdated": "2022-03-18T10:41:54.1889948+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "resourcekey",
  "DisplayName": "Resource Key",
  "ShortDisplayName": "Resource Key",
  "Description": "Resource Key",
  "LastUpdated": "2022-03-18T10:41:54.1890196+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "confirmpassword",
  "DisplayName": "Confirm Password",
  "ShortDisplayName": "Confirm Password",
  "Description": "Confirm Password",
  "LastUpdated": "2022-03-18T10:41:54.1890291+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "id",
  "DisplayName": "ID",
  "ShortDisplayName": "ID",
  "Description": "ID",
  "LastUpdated": "2022-03-18T10:41:54.1890337+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "cultureremoved",
  "DisplayName": "Culture removed",
  "ShortDisplayName": "cultureremoved",
  "Description": "cultureremoved",
  "LastUpdated": "2022-03-18T10:41:54.1890383+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "cultureadded",
  "DisplayName": "Culture added",
  "ShortDisplayName": "cultureadded",
  "Description": "cultureadded",
  "LastUpdated": "2022-03-18T10:41:54.1890428+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "content",
  "DisplayName": "Content",
  "ShortDisplayName": "Content",
  "Description": "Content",
  "LastUpdated": "2022-03-18T10:41:54.1890475+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "script",
  "DisplayName": "Script",
  "ShortDisplayName": "Script",
  "Description": "Script",
  "LastUpdated": "2022-03-18T10:41:54.1890521+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "newtemplate",
  "DisplayName": "New Template",
  "ShortDisplayName": "New Template",
  "Description": "New Template",
  "LastUpdated": "2022-03-18T10:41:54.1890588+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "displayname",
  "DisplayName": "Display Name",
  "ShortDisplayName": "Display Name",
  "Description": "Display Name",
  "LastUpdated": "2022-03-18T10:41:54.1890634+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "shortdisplayname",
  "DisplayName": "Short Display Name",
  "ShortDisplayName": "Short Display Name",
  "Description": "Short Display Name",
  "LastUpdated": "2022-03-18T10:41:54.1890682+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "description",
  "DisplayName": "Description",
  "ShortDisplayName": "Description",
  "Description": "description",
  "LastUpdated": "2024-09-06T15:45:40.1634209+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "key",
  "DisplayName": "Key",
  "ShortDisplayName": "Key",
  "Description": "Key",
  "LastUpdated": "2024-09-06T15:45:40.2425527+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "key",
  "DisplayName": "Key",
  "ShortDisplayName": "Key",
  "Description": "Key",
  "LastUpdated": "2022-03-18T10:41:54.1890821+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "description",
  "DisplayName": "Description",
  "ShortDisplayName": "Description",
  "Description": "description",
  "LastUpdated": "2022-03-18T10:41:54.189098+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "showonmenus",
  "DisplayName": "Show on Menu?",
  "ShortDisplayName": "Show on Menu?",
  "Description": "Show on Menu?",
  "LastUpdated": "2022-03-18T10:41:54.189112+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "pageinfo",
  "DisplayName": "Page Information",
  "ShortDisplayName": "Page Information",
  "Description": "Page Information",
  "LastUpdated": "2022-03-18T10:41:54.1891167+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "roles",
  "DisplayName": "Roles",
  "ShortDisplayName": "Roles",
  "Description": "Roles",
  "LastUpdated": "2022-03-18T10:41:54.1891214+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "updatedby",
  "DisplayName": "Updated By",
  "ShortDisplayName": "Updated By",
  "Description": "Updated By",
  "LastUpdated": "2022-03-18T10:41:54.1891426+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "lastupdated",
  "DisplayName": "Last Updated",
  "ShortDisplayName": "Last Updated",
  "Description": "Last Updated",
  "LastUpdated": "2022-03-18T10:41:54.1891519+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "newlayout",
  "DisplayName": "New Layout",
  "ShortDisplayName": "New Layout",
  "Description": "New Layout",
  "LastUpdated": "2022-03-18T10:41:54.1891566+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "newresource",
  "DisplayName": "New Resource",
  "ShortDisplayName": "New Resource",
  "Description": "New Resource",
  "LastUpdated": "2022-03-18T10:41:54.1891613+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "newtranslation",
  "DisplayName": "New Translation",
  "ShortDisplayName": "New Translation",
  "Description": "New Translation",
  "LastUpdated": "2022-03-18T10:41:54.1891675+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "translationcreated",
  "DisplayName": "Translation Created",
  "ShortDisplayName": "Translation Created",
  "Description": "Translation Created",
  "LastUpdated": "2022-03-18T10:41:54.1891724+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "resourcecreated",
  "DisplayName": "Resource Created",
  "ShortDisplayName": "Resource Created",
  "Description": "Resource Created",
  "LastUpdated": "2022-03-18T10:41:54.189177+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "resourcesaved",
  "DisplayName": "Resource has been saved",
  "ShortDisplayName": "Resource has been saved",
  "Description": "Resource has been saved",
  "LastUpdated": "2022-03-18T10:41:54.1891816+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "areyousure",
  "DisplayName": "Are you sure?",
  "ShortDisplayName": "Are you sure?",
  "Description": "Are you sure?",
  "LastUpdated": "2022-03-18T10:41:54.1891863+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "pagedeleted",
  "DisplayName": "Page deleted",
  "ShortDisplayName": "Page deleted",
  "Description": "Page deleted",
  "LastUpdated": "2022-03-18T10:41:54.1891909+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "pagedeletefailed",
  "DisplayName": "Page delete failed",
  "ShortDisplayName": "Page delete failed",
  "Description": "Page delete failed",
  "LastUpdated": "2022-03-18T10:41:54.1892061+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "layout",
  "DisplayName": "Layout",
  "ShortDisplayName": "Layout",
  "Description": "Layout",
  "LastUpdated": "2022-03-18T10:41:54.1892104+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "isshownonmenu",
  "DisplayName": "Visible dans le menu?",
  "ShortDisplayName": "Visible dans le menu?",
  "Description": "Visible dans le menu?",
  "LastUpdated": "2022-03-18T10:41:54.1893551+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "pagemoved",
  "DisplayName": "Page Moved",
  "ShortDisplayName": "Page Moved",
  "Description": "pagemoved",
  "LastUpdated": "2022-03-18T10:41:54.1893595+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "successfullysavedchanges",
  "DisplayName": "Modifications enregistrées avec succès",
  "ShortDisplayName": "Modifications enregistrées avec succès",
  "Description": "Modifications enregistrées avec succès",
  "LastUpdated": "2022-03-18T10:41:54.1893639+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "submit",
  "DisplayName": "Soumettre",
  "ShortDisplayName": "Soumettre",
  "Description": "Soumettre",
  "LastUpdated": "2022-03-18T10:41:54.1893683+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "defaulttheme",
  "DisplayName": "Thème par défaut",
  "ShortDisplayName": "Thème par défaut",
  "Description": "Thème par défaut",
  "LastUpdated": "2022-03-18T10:41:54.1894238+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "authfailed",
  "DisplayName": "Authentication failed",
  "ShortDisplayName": "Authenntication FAILED",
  "Description": "authfailed",
  "LastUpdated": "2022-03-18T10:41:54.1894282+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "tokenverificationfailed",
  "DisplayName": "La vérification du jeton a échoué",
  "ShortDisplayName": "La vérification du jeton a échoué",
  "Description": "La vérification du jeton a échoué",
  "LastUpdated": "2022-03-18T10:41:54.1894822+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "close",
  "DisplayName": "Close",
  "ShortDisplayName": "Close",
  "Description": "Close",
  "LastUpdated": "2022-03-18T10:41:54.1895464+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "tokenverificationsuccess",
  "DisplayName": "Jeton vérifié avec succès",
  "ShortDisplayName": "Jeton vérifié avec succès",
  "Description": "Jeton vérifié avec succès",
  "LastUpdated": "2022-03-18T10:41:54.1895507+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "delete",
  "DisplayName": "Effacer",
  "ShortDisplayName": "Effacer",
  "Description": "Effacer",
  "LastUpdated": "2022-03-18T10:41:54.1895566+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "user",
  "DisplayName": "Utilisateur",
  "ShortDisplayName": "Utilisateur",
  "Description": "Utilisateur",
  "LastUpdated": "2022-03-18T10:41:54.1895936+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "packagecreated",
  "DisplayName": "Package Created",
  "ShortDisplayName": "Package Created",
  "Description": "packagecreated",
  "LastUpdated": "2022-03-18T10:41:54.189598+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "packageimported",
  "DisplayName": "Package Imported",
  "ShortDisplayName": "Package Imported",
  "Description": "packageimported",
  "LastUpdated": "2022-03-18T10:41:54.1896022+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "packagedeleted",
  "DisplayName": "Package Deleted",
  "ShortDisplayName": "Package Deleted",
  "Description": "packagedeleted",
  "LastUpdated": "2022-03-18T10:41:54.1896065+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "copyapp",
  "DisplayName": "Copy App",
  "ShortDisplayName": "Copy App",
  "Description": "copyapp",
  "LastUpdated": "2022-03-18T10:41:54.1896108+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "password",
  "DisplayName": "Mot de passe",
  "ShortDisplayName": "Mot de passe",
  "Description": "Mot de passe",
  "LastUpdated": "2022-03-18T10:41:54.1896151+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "apiurl",
  "DisplayName": "Api URL",
  "ShortDisplayName": "API URL",
  "Description": "apiurl",
  "LastUpdated": "2022-03-18T10:41:54.1896194+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "packages",
  "DisplayName": "Packages",
  "ShortDisplayName": "Packages",
  "Description": "packages",
  "LastUpdated": "2022-03-18T10:41:54.1896253+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "username",
  "DisplayName": "Username",
  "ShortDisplayName": "username",
  "Description": "username",
  "LastUpdated": "2022-03-18T10:41:54.1896297+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "copy",
  "DisplayName": "Copy",
  "ShortDisplayName": "Copy",
  "Description": "copy",
  "LastUpdated": "2022-03-18T10:41:54.189634+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "category",
  "DisplayName": "Category",
  "ShortDisplayName": "Category",
  "Description": "category",
  "LastUpdated": "2022-03-18T10:41:54.1896384+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "username",
  "DisplayName": "Nom d'utilisateur",
  "ShortDisplayName": "Nom d'utilisateur",
  "Description": "Nom d'utilisateur",
  "LastUpdated": "2022-03-18T10:41:54.1896428+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "forgotpassword",
  "DisplayName": "Mot de passe oublié ?",
  "ShortDisplayName": "Mot de passe oublié ?",
  "Description": "Mot de passe oublié ?",
  "LastUpdated": "2022-03-18T10:41:54.1896472+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "email",
  "DisplayName": "Email",
  "ShortDisplayName": "Email",
  "Description": "Email",
  "LastUpdated": "2022-03-18T10:41:54.1896517+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "newpackage",
  "DisplayName": "New Package",
  "ShortDisplayName": "New Package",
  "Description": "newpackage",
  "LastUpdated": "2022-03-18T10:41:54.1896561+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "import",
  "DisplayName": "Import",
  "ShortDisplayName": "Import",
  "Description": "import",
  "LastUpdated": "2022-03-18T10:41:54.1896621+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "register",
  "DisplayName": "Registre",
  "ShortDisplayName": "Registre",
  "Description": "Registre",
  "LastUpdated": "2022-03-18T10:41:54.1896665+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "deleteresourcetitle",
  "DisplayName": "Delete Resource?",
  "ShortDisplayName": "Delete Resource?",
  "Description": "Delete Resource?",
  "LastUpdated": "2022-03-18T10:41:54.1896709+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "export",
  "DisplayName": "Exportation",
  "ShortDisplayName": "Exportation",
  "Description": "Exportation",
  "LastUpdated": "2022-03-18T10:41:54.1896753+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "source",
  "DisplayName": "Source",
  "ShortDisplayName": "Source",
  "Description": "source",
  "LastUpdated": "2022-03-18T10:41:54.1896797+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "apiurl",
  "DisplayName": "URL de l'API",
  "ShortDisplayName": "URL de l'API",
  "Description": "\tURL de l'API",
  "LastUpdated": "2022-03-18T10:41:54.1896842+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "packagecreated",
  "DisplayName": "Package créé",
  "ShortDisplayName": "Package créé",
  "Description": "Package créé",
  "LastUpdated": "2022-03-18T10:41:54.1896885+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "checkyouremail",
  "DisplayName": "Vérifiez votre email",
  "ShortDisplayName": "Vérifiez votre email",
  "Description": "Vérifiez votre email",
  "LastUpdated": "2022-03-18T10:41:54.1896946+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "packageimported",
  "DisplayName": "Package importé",
  "ShortDisplayName": "Package importé",
  "Description": "Package importé",
  "LastUpdated": "2022-03-18T10:41:54.189699+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "authfailed",
  "DisplayName": "Authentification échouée",
  "ShortDisplayName": "Authentification échouée",
  "Description": "Authentification échouée",
  "LastUpdated": "2022-03-18T10:41:54.1897034+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "newrole",
  "DisplayName": "Nouveau rôle",
  "ShortDisplayName": "Nouveau rôle",
  "Description": "Nouveau rôle",
  "LastUpdated": "2022-03-18T10:41:54.1897078+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "appsaved",
  "DisplayName": "Application enregistrée",
  "ShortDisplayName": "Application enregistrée",
  "Description": "Application enregistrée",
  "LastUpdated": "2022-03-18T10:41:54.1897122+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "appdeleted",
  "DisplayName": "Application supprimée",
  "ShortDisplayName": "Application supprimée",
  "Description": "\tApplication supprimée",
  "LastUpdated": "2022-03-18T10:41:54.1897166+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "appcreated",
  "DisplayName": "Application créée",
  "ShortDisplayName": "Application créée",
  "Description": "Application créée",
  "LastUpdated": "2022-03-18T10:41:54.189721+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "pageinfo",
  "DisplayName": "Informations sur la page",
  "ShortDisplayName": "Informations sur la page",
  "Description": "Informations sur la page",
  "LastUpdated": "2022-03-18T10:41:54.1897254+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "forgotpasspleaseenteremail",
  "DisplayName": "Si vous avez oublié votre mot de passe, veuillez saisir votre email ici",
  "ShortDisplayName": "Si vous avez oublié votre mot de passe, veuillez saisir votre email ici",
  "Description": "Si vous avez oublié votre mot de passe, veuillez saisir votre email ici",
  "LastUpdated": "2022-03-18T10:41:54.1897314+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "resourcekey",
  "DisplayName": "Clé de ressource",
  "ShortDisplayName": "Clé de ressource",
  "Description": "Clé de ressource",
  "LastUpdated": "2022-03-18T10:41:54.1897359+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "name",
  "DisplayName": "Nom",
  "ShortDisplayName": "Nom",
  "Description": "Nom",
  "LastUpdated": "2022-03-18T10:41:54.1897403+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "copyapp",
  "DisplayName": "Copier l'application",
  "ShortDisplayName": "Copier l'application",
  "Description": "Copier l'application",
  "LastUpdated": "2022-03-18T10:41:54.1897447+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "isshownonmenu",
  "DisplayName": "Is Shown On Menu?",
  "ShortDisplayName": "Is Shown On Menu?",
  "Description": "Is Shown On Menu?",
  "LastUpdated": "2022-03-18T10:41:54.1897491+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "newtranslation",
  "DisplayName": "Nouvelle traduction",
  "ShortDisplayName": "Nouvelle traduction",
  "Description": "Nouvelle traduction",
  "LastUpdated": "2022-03-18T10:41:54.1897536+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "resourcesaved",
  "DisplayName": "La ressource a été enregistrée",
  "ShortDisplayName": "La ressource a été enregistrée",
  "Description": "La ressource a été enregistrée",
  "LastUpdated": "2022-03-18T10:41:54.1897579+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "newtemplate",
  "DisplayName": "Nouveau modèle",
  "ShortDisplayName": "Nouveau modèle",
  "Description": "Nouveau modèle",
  "LastUpdated": "2022-03-18T10:41:54.1897639+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "configureapp",
  "DisplayName": "Configurer l'application",
  "ShortDisplayName": "Configurer l'application",
  "Description": "Configurer l'application",
  "LastUpdated": "2022-03-18T10:41:54.1897683+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "copy",
  "DisplayName": "Copy",
  "ShortDisplayName": "Copy",
  "Description": "copy",
  "LastUpdated": "2022-03-18T10:41:54.1897727+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "accessdenied",
  "DisplayName": "Accès refusé",
  "ShortDisplayName": "Accès refusé",
  "Description": "Accès refusé",
  "LastUpdated": "2022-03-18T10:41:54.1897771+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "packagedeleted",
  "DisplayName": "Package supprimé",
  "ShortDisplayName": "Package supprimé",
  "Description": "packagedeleted",
  "LastUpdated": "2022-03-18T10:41:54.1897815+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "packagesaved",
  "DisplayName": "Package Saved",
  "ShortDisplayName": "Package Saved",
  "Description": "packagesaved",
  "LastUpdated": "2022-03-18T10:41:54.1897859+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "accountactive",
  "DisplayName": "Compte actif",
  "ShortDisplayName": "Compte actif",
  "Description": "Compte actif",
  "LastUpdated": "2022-03-18T10:41:54.1897904+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "domain",
  "DisplayName": "Domaine",
  "ShortDisplayName": "Domaine",
  "Description": "Domaine",
  "LastUpdated": "2022-03-18T10:41:54.1897963+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "create",
  "DisplayName": "Créer",
  "ShortDisplayName": "Créer",
  "Description": "Créer",
  "LastUpdated": "2022-03-18T10:41:54.1898009+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "visit",
  "DisplayName": "Visite",
  "ShortDisplayName": "Visite",
  "Description": "Visite",
  "LastUpdated": "2022-03-18T10:41:54.1898054+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "id",
  "DisplayName": "ID",
  "ShortDisplayName": "ID",
  "Description": "ID",
  "LastUpdated": "2022-03-18T10:41:54.1898098+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "defaultculture",
  "DisplayName": "Culture par défaut",
  "ShortDisplayName": "Culture par défaut",
  "Description": "Culture par défaut",
  "LastUpdated": "2022-03-18T10:41:54.1898184+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "import",
  "DisplayName": "Importation",
  "ShortDisplayName": "Importation",
  "Description": "Importation",
  "LastUpdated": "2022-03-18T10:41:54.1898228+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "description",
  "DisplayName": "Description",
  "ShortDisplayName": "Description",
  "Description": "Description",
  "LastUpdated": "2022-03-18T10:41:54.1898791+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "AppType",
  "DisplayName": "App Type",
  "ShortDisplayName": "App Type",
  "Description": "App Type",
  "LastUpdated": "2022-03-18T10:41:54.1898925+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "apppath",
  "DisplayName": "App Path",
  "ShortDisplayName": "App Path",
  "Description": "App Path",
  "LastUpdated": "2022-03-18T10:41:54.1898969+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "appname",
  "DisplayName": "App Name",
  "ShortDisplayName": "App Name",
  "Description": "App Name",
  "LastUpdated": "2022-03-18T10:41:54.1899138+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "createdemo",
  "DisplayName": "Create Demo App",
  "ShortDisplayName": "Create Demo App",
  "Description": "Create Demo App",
  "LastUpdated": "2022-03-18T10:41:54.1899186+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "sourcesystem",
  "DisplayName": "Source System",
  "ShortDisplayName": "Source System",
  "Description": "Source System",
  "LastUpdated": "2022-03-18T10:41:54.1899234+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "transactionsourcesystem",
  "DisplayName": "Transaction Source System",
  "ShortDisplayName": "Transaction Source System",
  "Description": "Transaction Source System",
  "LastUpdated": "2022-03-18T10:41:54.189928+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "category",
  "DisplayName": "Catégorie",
  "ShortDisplayName": "Catégorie",
  "Description": "Catégorie",
  "LastUpdated": "2022-03-18T10:41:54.1899372+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "newpackage",
  "DisplayName": "Nouveau package",
  "ShortDisplayName": "Nouveau package",
  "Description": "Nouveau package",
  "LastUpdated": "2022-03-18T10:41:54.1899419+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "newapp",
  "DisplayName": "Nouvelle appli",
  "ShortDisplayName": "Nouvelle appli",
  "Description": "Nouvelle appli",
  "LastUpdated": "2022-03-18T10:41:54.1899479+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "migrateapp",
  "DisplayName": "Migrer l'application",
  "ShortDisplayName": "Migrer l'application",
  "Description": "Migrer l'application",
  "LastUpdated": "2022-03-18T10:41:54.1899526+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "migrate",
  "DisplayName": "Importation",
  "ShortDisplayName": "Importation",
  "Description": "Importation",
  "LastUpdated": "2022-03-18T10:41:54.1899573+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "source",
  "DisplayName": "Source",
  "ShortDisplayName": "Source",
  "Description": "Source",
  "LastUpdated": "2022-03-18T10:41:54.1899666+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "packagesaved",
  "DisplayName": "Package enregistré",
  "ShortDisplayName": "Package enregistré",
  "Description": "Package enregistré",
  "LastUpdated": "2022-03-18T10:41:54.1899713+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "resourcedeleteconfirmation",
  "DisplayName": "Are you sure you want to delete this Resource? It will be gone forever.",
  "ShortDisplayName": "Are you sure you want to delete this Resource? It will be gone forever.",
  "Description": "Are you sure you want to delete this Resource? It will be gone forever.",
  "LastUpdated": "2022-03-18T10:41:54.1899761+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "resourcename",
  "DisplayName": "Resource Name",
  "ShortDisplayName": "Resource Name",
  "Description": "Resource Name",
  "LastUpdated": "2022-03-18T10:41:54.1899808+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "resourcename",
  "DisplayName": "Nom de la ressource",
  "ShortDisplayName": "Nom de la ressource",
  "Description": "Nom de la ressource",
  "LastUpdated": "2022-03-18T10:41:54.1899871+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "shortdisplayname",
  "DisplayName": "Nom d'affichage court",
  "ShortDisplayName": "Nom d'affichage court",
  "Description": "Nom d'affichage court",
  "LastUpdated": "2022-03-18T10:41:54.189992+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "displayname",
  "DisplayName": "Afficher un nom",
  "ShortDisplayName": "Afficher un nom",
  "Description": "Afficher un nom",
  "LastUpdated": "2022-03-18T10:41:54.1899966+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "newresource",
  "DisplayName": "Nouvelle ressource",
  "ShortDisplayName": "Nouvelle ressource",
  "Description": "Nouvelle ressource",
  "LastUpdated": "2022-03-18T10:41:54.1900014+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "key",
  "DisplayName": "Clé",
  "ShortDisplayName": "Clé",
  "Description": "Clé",
  "LastUpdated": "2022-03-18T10:41:54.1900061+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "layout",
  "DisplayName": "Disposition",
  "ShortDisplayName": "Disposition",
  "Description": "Disposition",
  "LastUpdated": "2022-03-18T10:41:54.1900109+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "new",
  "DisplayName": "New",
  "ShortDisplayName": "New",
  "Description": "New",
  "LastUpdated": "2022-03-18T10:41:54.1900156+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "translationcreated",
  "DisplayName": "Traduction créée",
  "ShortDisplayName": "Traduction créée",
  "Description": "Traduction créée",
  "LastUpdated": "2022-03-18T10:41:54.1900219+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "resourcecreated",
  "DisplayName": "Resource Created",
  "ShortDisplayName": "Resource Created",
  "Description": "Resource Created",
  "LastUpdated": "2022-03-18T10:41:54.1900268+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "cancel",
  "DisplayName": "Cancel",
  "ShortDisplayName": "Cancel",
  "Description": "Cancel",
  "LastUpdated": "2022-03-18T10:41:54.1900314+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "pageproperties",
  "DisplayName": "Page Properties",
  "ShortDisplayName": "Page Properties",
  "Description": "Page Properties",
  "LastUpdated": "2022-03-18T10:41:54.1900361+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "showonmenus",
  "DisplayName": "Afficher dans le menu?",
  "ShortDisplayName": "Afficher dans le menu?",
  "Description": "Afficher dans le menu?",
  "LastUpdated": "2022-03-18T10:41:54.1900407+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "newrole",
  "DisplayName": "New Role",
  "ShortDisplayName": "New Role",
  "Description": "New Role",
  "LastUpdated": "2022-03-18T10:41:54.1900456+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "newcomponent",
  "DisplayName": "New Component",
  "ShortDisplayName": "New Component",
  "Description": "New Component",
  "LastUpdated": "2022-03-18T10:41:54.1900503+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "deletethisapptitle",
  "DisplayName": "Supprimer cette appli?",
  "ShortDisplayName": "Supprimer cette appli?",
  "Description": "Supprimer cette appli?",
  "LastUpdated": "2022-03-18T10:41:54.190055+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "areyousureyouwanttodeletethisapp",
  "DisplayName": "Voulez-vous vraiment supprimer cette application? Il ne peut pas être inversé.",
  "ShortDisplayName": "Voulez-vous vraiment supprimer cette application? Il ne peut pas être inversé.",
  "Description": "Voulez-vous vraiment supprimer cette application? Il ne peut pas être inversé.",
  "LastUpdated": "2022-03-18T10:41:54.1900614+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "close",
  "DisplayName": "Close",
  "ShortDisplayName": "Close",
  "Description": "Close",
  "LastUpdated": "2022-03-18T10:41:54.1900661+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "newlayout",
  "DisplayName": "Nouvelle présentation",
  "ShortDisplayName": "Nouvelle présentation",
  "Description": "Nouvelle présentation",
  "LastUpdated": "2022-03-18T10:41:54.1900708+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "pagemoved",
  "DisplayName": "Page déplacée",
  "ShortDisplayName": "Page déplacée",
  "Description": "Page déplacée",
  "LastUpdated": "2022-03-18T10:41:54.1900755+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "roles",
  "DisplayName": "Roles",
  "ShortDisplayName": "Roles",
  "Description": "Roles",
  "LastUpdated": "2022-03-18T10:41:54.1900802+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "newchildpage",
  "DisplayName": "New Child Page",
  "ShortDisplayName": "New Child Page",
  "Description": "New Child Page",
  "LastUpdated": "2022-03-18T10:41:54.190085+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "tokenverificationsuccess",
  "DisplayName": "Token Verification Success",
  "ShortDisplayName": "Token Verification Success",
  "Description": "tokenverificationsuccess",
  "LastUpdated": "2022-03-18T10:41:54.1900899+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "email",
  "DisplayName": "Email",
  "ShortDisplayName": "Email",
  "Description": "email",
  "LastUpdated": "2022-03-18T10:41:54.1900961+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "forgotpasspleaseenteremail",
  "DisplayName": "If you forgot your password, please enter your email here",
  "ShortDisplayName": "If you forgot your password, please enter your email here",
  "Description": "forgotpasspleaseenteremail",
  "LastUpdated": "2022-03-18T10:41:54.1901009+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "submit",
  "DisplayName": "Submit",
  "ShortDisplayName": "Submit",
  "Description": "submit",
  "LastUpdated": "2022-03-18T10:41:54.1901055+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "register",
  "DisplayName": "Register",
  "ShortDisplayName": "Register",
  "Description": "Register",
  "LastUpdated": "2022-03-18T10:41:54.1901102+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "forgotpassword",
  "DisplayName": "Forgotten your Password?",
  "ShortDisplayName": "Forgotten your Password?",
  "Description": "forgotpassword",
  "LastUpdated": "2022-03-18T10:41:54.190115+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "password",
  "DisplayName": "Password",
  "ShortDisplayName": "Password",
  "Description": "password",
  "LastUpdated": "2022-03-18T10:41:54.1901197+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "user",
  "DisplayName": "User",
  "ShortDisplayName": "User",
  "Description": "User",
  "LastUpdated": "2022-03-18T10:41:54.1901243+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "accountactive",
  "DisplayName": "Account Active",
  "ShortDisplayName": "Account Active",
  "Description": "accountactive",
  "LastUpdated": "2022-03-18T10:41:54.190129+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "tokenverificationfailed",
  "DisplayName": "Token Verification Failed",
  "ShortDisplayName": "Token Verification Failed",
  "Description": "tokenverificationfailed",
  "LastUpdated": "2022-03-18T10:41:54.1901353+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "passwordresetemailtitle",
  "DisplayName": "Password Reset Email",
  "ShortDisplayName": "Password Reset Email",
  "Description": "passwordresetemailtitle",
  "LastUpdated": "2022-03-18T10:41:54.19014+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "checkyouremail",
  "DisplayName": "Check your email",
  "ShortDisplayName": "Check your email",
  "Description": "Check your email",
  "LastUpdated": "2022-03-18T10:41:54.1901446+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "accessdenied",
  "DisplayName": "Access Denied",
  "ShortDisplayName": "Access Denied",
  "Description": "Access Denied",
  "LastUpdated": "2022-03-18T10:41:54.1901493+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "newpage",
  "DisplayName": "New Page",
  "ShortDisplayName": "New Page",
  "Description": "New Page",
  "LastUpdated": "2022-03-18T10:41:54.190154+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "passwordresetemailtitle",
  "DisplayName": "Réinitialisation du mot de passe",
  "ShortDisplayName": "Réinitialisation du mot de passe",
  "Description": "Réinitialisation du mot de passe",
  "LastUpdated": "2022-03-18T10:41:54.1901586+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "newcomponent",
  "DisplayName": "Nouveau composant",
  "ShortDisplayName": "Nouveau composant",
  "Description": "Nouveau composant",
  "LastUpdated": "2022-03-18T10:41:54.1901633+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "areyousureyouwanttodeletethisapp",
  "DisplayName": "Are you sure you want to delete this App? It cannot be reversed.",
  "ShortDisplayName": "Are you sure you want to delete this App? It cannot be reversed.",
  "Description": "Are you sure you want to delete this App? It cannot be reversed.",
  "LastUpdated": "2022-03-18T10:41:54.1901696+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "save",
  "DisplayName": "Sauvegarder",
  "ShortDisplayName": "Sauvegarder",
  "Description": "Sauvegarder",
  "LastUpdated": "2022-03-18T10:41:54.1901743+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "confirm",
  "DisplayName": "Confirm",
  "ShortDisplayName": "Confirm",
  "Description": "Confirm",
  "LastUpdated": "2022-03-18T10:41:54.1902974+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "properties",
  "DisplayName": "Properties",
  "ShortDisplayName": "Properties",
  "Description": "Properties",
  "LastUpdated": "2022-03-18T10:41:54.1903062+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "deleteconfirmation",
  "DisplayName": "Are you sure you want to delete?",
  "ShortDisplayName": "Are you sure you want to delete?",
  "Description": "Are you sure you want to delete?",
  "LastUpdated": "2022-03-18T10:41:54.1903116+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "successfullysavedchanges",
  "DisplayName": "Successfully saved changes",
  "ShortDisplayName": "Successfully saved changes",
  "Description": "Successfully saved changes",
  "LastUpdated": "2022-03-18T10:41:54.1903168+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "createapptitle",
  "DisplayName": "App Creation",
  "ShortDisplayName": "App Creation",
  "Description": "App Creation",
  "LastUpdated": "2022-03-18T10:41:54.190322+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "areyousureyouwanttodelete",
  "DisplayName": "Are you sure you want to delete this? It cannot be reversed.",
  "ShortDisplayName": "Are you sure you want to delete this? It cannot be reversed.",
  "Description": "Are you sure you want to delete this? It cannot be reversed.",
  "LastUpdated": "2022-03-18T10:41:54.1903295+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "areyousureyouwanttodelete",
  "DisplayName": "Voulez-vous vraiment supprimer cela? Il ne peut pas être inversé.",
  "ShortDisplayName": "Voulez-vous vraiment supprimer cela? Il ne peut pas être inversé.",
  "Description": "Voulez-vous vraiment supprimer cela? Il ne peut pas être inversé.",
  "LastUpdated": "2022-03-18T10:41:54.1903346+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "environment",
  "DisplayName": "Environment",
  "ShortDisplayName": "Environment",
  "Description": "Environment",
  "LastUpdated": "2022-03-18T10:41:54.1903397+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "deletethisapptitle",
  "DisplayName": "Delete this App?",
  "ShortDisplayName": "Delete this App?",
  "Description": "Delete this App?",
  "LastUpdated": "2022-03-18T10:41:54.1903447+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "environment",
  "DisplayName": "Environnement",
  "ShortDisplayName": "Environnement",
  "Description": "Environnement",
  "LastUpdated": "2022-03-18T10:41:54.1903496+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "packages",
  "DisplayName": "Paquets",
  "ShortDisplayName": "Paquets",
  "Description": "Paquets",
  "LastUpdated": "2022-03-18T10:41:54.1903546+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "name",
  "DisplayName": "Name",
  "ShortDisplayName": "Name",
  "Description": "Name",
  "LastUpdated": "2022-03-18T10:41:54.1903765+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "configureapp",
  "DisplayName": "Configure App",
  "ShortDisplayName": "Configure App",
  "Description": "configureapp",
  "LastUpdated": "2022-03-18T10:41:54.1903815+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "appsaved",
  "DisplayName": "App Saved",
  "ShortDisplayName": "App Saved",
  "Description": "appsaved",
  "LastUpdated": "2022-03-18T10:41:54.1903865+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "fr-FR",
  "Key": "CMS",
  "Name": "confirmpassword",
  "DisplayName": "Confirmez le mot de passe",
  "ShortDisplayName": "Confirmez le mot de passe",
  "Description": "Confirmez le mot de passe",
  "LastUpdated": "2022-03-18T10:41:54.1904288+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "migrateapp",
  "DisplayName": "Migrate App",
  "ShortDisplayName": "Migrate App",
  "Description": "migrateapp",
  "LastUpdated": "2022-03-18T10:41:54.1904338+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "en-GB",
  "Key": "CMS",
  "Name": "new",
  "DisplayName": "New",
  "ShortDisplayName": "new",
  "Description": "new",
  "LastUpdated": "2022-10-12T16:58:03.4590576+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "appdeleted",
  "DisplayName": "App Deleted",
  "ShortDisplayName": "App Deleted",
  "Description": "App Deleted",
  "LastUpdated": "2024-09-06T15:45:40.2571562+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "export",
  "DisplayName": "Export",
  "ShortDisplayName": "Export",
  "Description": "export",
  "LastUpdated": "2022-03-18T10:41:54.1904505+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "domain",
  "DisplayName": "Domain",
  "ShortDisplayName": "Domain",
  "Description": "Domain",
  "LastUpdated": "2022-03-18T10:41:54.1904556+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "defaultculture",
  "DisplayName": "Default Culture",
  "ShortDisplayName": "Default Culture",
  "Description": "defaultculture",
  "LastUpdated": "2022-03-18T10:41:54.1904606+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "create",
  "DisplayName": "Create",
  "ShortDisplayName": "Create",
  "Description": "Create",
  "LastUpdated": "2022-03-18T10:41:54.1904656+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "new",
  "DisplayName": "New",
  "ShortDisplayName": "New",
  "Description": "New",
  "LastUpdated": "2022-03-18T10:41:54.1904706+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "visit",
  "DisplayName": "Visit",
  "ShortDisplayName": "Visit",
  "Description": "visit",
  "LastUpdated": "2022-03-18T10:41:54.1904757+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "save",
  "DisplayName": "Save",
  "ShortDisplayName": "Save",
  "Description": "Save",
  "LastUpdated": "2022-03-18T10:41:54.1904808+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "migrate",
  "DisplayName": "Migrate",
  "ShortDisplayName": "Migrate",
  "Description": "migrate",
  "LastUpdated": "2022-03-18T10:41:54.1904876+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "defaulttheme",
  "DisplayName": "Default Theme",
  "ShortDisplayName": "Default Theme",
  "Description": "defaulttheme",
  "LastUpdated": "2022-03-18T10:41:54.1904927+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "delete",
  "DisplayName": "Delete",
  "ShortDisplayName": "Delete",
  "Description": "delete",
  "LastUpdated": "2022-03-18T10:41:54.1904978+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "newapp",
  "DisplayName": "New App",
  "ShortDisplayName": "New App",
  "Description": "New App",
  "LastUpdated": "2022-03-18T10:41:54.1905028+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "appcreated",
  "DisplayName": "App Created",
  "ShortDisplayName": "App Created",
  "Description": "appcreated",
  "LastUpdated": "2022-03-18T10:41:54.1905079+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "appconfigparsingerror",
  "DisplayName": "App config parsing error",
  "ShortDisplayName": "App config parsing error",
  "Description": "App config parsing error",
  "LastUpdated": "2022-03-18T10:41:54.1905228+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Core",
  "Name": "Month-2",
  "DisplayName": "February",
  "ShortDisplayName": "February",
  "Description": "February",
  "LastUpdated": "2022-03-18T10:41:54.1906142+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Core",
  "Name": "Month-3",
  "DisplayName": "March",
  "ShortDisplayName": "March",
  "Description": "March",
  "LastUpdated": "2022-03-18T10:41:54.1906192+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Core",
  "Name": "Month-4",
  "DisplayName": "April",
  "ShortDisplayName": "April",
  "Description": "April",
  "LastUpdated": "2022-03-18T10:41:54.1906243+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Core",
  "Name": "Month-5",
  "DisplayName": "May",
  "ShortDisplayName": "May",
  "Description": "May",
  "LastUpdated": "2022-03-18T10:41:54.1906293+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Core",
  "Name": "Month-6",
  "DisplayName": "June",
  "ShortDisplayName": "June",
  "Description": "June",
  "LastUpdated": "2022-03-18T10:41:54.1906343+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Core",
  "Name": "Month-7",
  "DisplayName": "July",
  "ShortDisplayName": "July",
  "Description": "July",
  "LastUpdated": "2022-03-18T10:41:54.1906393+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Default",
  "Name": "login",
  "DisplayName": "Login",
  "ShortDisplayName": "Login",
  "Description": "Login",
  "LastUpdated": "2022-03-18T10:41:54.1907669+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "mailmanagement",
  "DisplayName": "Mail Management",
  "ShortDisplayName": "Mail Management",
  "Description": "Mail Management",
  "LastUpdated": "2022-03-18T10:41:54.190777+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "saving",
  "DisplayName": "Saving",
  "ShortDisplayName": "Saving",
  "Description": "Saving",
  "LastUpdated": "2022-03-18T10:41:54.1907871+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Core",
  "Name": "Month-1",
  "DisplayName": "January",
  "ShortDisplayName": "January",
  "Description": "January",
  "LastUpdated": "2022-03-18T10:41:54.190871+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Default",
  "Name": "Month-7",
  "DisplayName": "July",
  "ShortDisplayName": "July",
  "Description": "July",
  "LastUpdated": "2022-03-18T10:41:54.190876+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Default",
  "Name": "Month-6",
  "DisplayName": "June",
  "ShortDisplayName": "June",
  "Description": "June",
  "LastUpdated": "2022-03-18T10:41:54.1908828+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Default",
  "Name": "Month-5",
  "DisplayName": "May",
  "ShortDisplayName": "May",
  "Description": "May",
  "LastUpdated": "2022-03-18T10:41:54.1908878+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Default",
  "Name": "Month-4",
  "DisplayName": "April",
  "ShortDisplayName": "April",
  "Description": "April",
  "LastUpdated": "2022-03-18T10:41:54.1908928+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Default",
  "Name": "Month-3",
  "DisplayName": "March",
  "ShortDisplayName": "March",
  "Description": "March",
  "LastUpdated": "2022-03-18T10:41:54.1908978+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Default",
  "Name": "Month-2",
  "DisplayName": "February",
  "ShortDisplayName": "February",
  "Description": "February",
  "LastUpdated": "2022-03-18T10:41:54.1909029+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Default",
  "Name": "Month-1",
  "DisplayName": "January",
  "ShortDisplayName": "January",
  "Description": "January",
  "LastUpdated": "2022-03-18T10:41:54.1909078+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "refreshcache",
  "DisplayName": "Refresh Cache",
  "ShortDisplayName": "Refresh Cache",
  "Description": "Refresh Cache",
  "LastUpdated": "2022-03-18T10:41:54.1909128+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "add",
  "DisplayName": "Add",
  "ShortDisplayName": "Add",
  "Description": "Add",
  "LastUpdated": "2022-03-18T10:41:54.1909194+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "scripts",
  "DisplayName": "Scripts",
  "ShortDisplayName": "Scripts",
  "Description": "Scripts",
  "LastUpdated": "2022-03-18T10:41:54.1909246+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "remove",
  "DisplayName": "Remove",
  "ShortDisplayName": "Remove",
  "Description": "Remove",
  "LastUpdated": "2022-03-18T10:41:54.1909296+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Default",
  "Name": "oldpassword",
  "DisplayName": "Old Password",
  "ShortDisplayName": "Old Password",
  "Description": "Old Password",
  "LastUpdated": "2022-03-18T10:41:54.1909346+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Default",
  "Name": "newpassword",
  "DisplayName": "New Password",
  "ShortDisplayName": "New Password",
  "Description": "New Password",
  "LastUpdated": "2022-03-18T10:41:54.1909396+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Default",
  "Name": "close",
  "DisplayName": "Close",
  "ShortDisplayName": "Close",
  "Description": "Close",
  "LastUpdated": "2022-03-18T10:41:54.1909446+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Default",
  "Name": "phonenumber",
  "DisplayName": "Phone Number",
  "ShortDisplayName": "Phone Number",
  "Description": "Phone Number",
  "LastUpdated": "2022-03-18T10:41:54.1909496+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Default",
  "Name": "updatepassword",
  "DisplayName": "Update Password",
  "ShortDisplayName": "Update Password",
  "Description": "Update Password",
  "LastUpdated": "2022-03-18T10:41:54.1909545+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Default",
  "Name": "update",
  "DisplayName": "Update",
  "ShortDisplayName": "Update",
  "Description": "Update",
  "LastUpdated": "2022-03-18T10:41:54.1909611+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "saved",
  "DisplayName": "Saved",
  "ShortDisplayName": "Saved",
  "Description": "Saved",
  "LastUpdated": "2022-03-18T10:41:54.1909661+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "rebuilt",
  "DisplayName": "Rebuilt",
  "ShortDisplayName": "Rebuilt",
  "Description": "Rebuilt",
  "LastUpdated": "2022-03-18T10:41:54.1909712+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "created",
  "DisplayName": "Created",
  "ShortDisplayName": "Created",
  "Description": "Created",
  "LastUpdated": "2022-03-18T10:41:54.1909762+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "primary",
  "DisplayName": "Primary",
  "ShortDisplayName": "Primary",
  "Description": "Primary",
  "LastUpdated": "2022-03-18T10:41:54.1911924+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "secondary",
  "DisplayName": "Secondary",
  "ShortDisplayName": "Secondary",
  "Description": "Secondary",
  "LastUpdated": "2022-03-18T10:41:54.1911975+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "background",
  "DisplayName": "Background",
  "ShortDisplayName": "Background",
  "Description": "Background",
  "LastUpdated": "2022-03-18T10:41:54.1912026+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "text",
  "DisplayName": "Text",
  "ShortDisplayName": "Text",
  "Description": "Text",
  "LastUpdated": "2022-03-18T10:41:54.1912077+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "text2",
  "DisplayName": "Text2",
  "ShortDisplayName": "Text2",
  "Description": "Text2",
  "LastUpdated": "2022-03-18T10:41:54.1912127+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "links",
  "DisplayName": "Links",
  "ShortDisplayName": "Links",
  "Description": "Links",
  "LastUpdated": "2022-03-18T10:41:54.1912177+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "font",
  "DisplayName": "Font",
  "ShortDisplayName": "Font",
  "Description": "Font",
  "LastUpdated": "2022-03-18T10:41:54.1912227+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "size",
  "DisplayName": "Size",
  "ShortDisplayName": "Size",
  "Description": "Size",
  "LastUpdated": "2022-03-18T10:41:54.1912277+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "family",
  "DisplayName": "Family",
  "ShortDisplayName": "Family",
  "Description": "Family",
  "LastUpdated": "2022-03-18T10:41:54.1912344+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "colours",
  "DisplayName": "Colours",
  "ShortDisplayName": "Colours",
  "Description": "Colours",
  "LastUpdated": "2022-03-18T10:41:54.1912394+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "border",
  "DisplayName": "Border",
  "ShortDisplayName": "Border",
  "Description": "Border",
  "LastUpdated": "2022-03-18T10:41:54.1912443+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "style",
  "DisplayName": "Style",
  "ShortDisplayName": "Style",
  "Description": "Style",
  "LastUpdated": "2022-03-18T10:41:54.1912494+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "width",
  "DisplayName": "Width",
  "ShortDisplayName": "Width",
  "Description": "Width",
  "LastUpdated": "2022-03-18T10:41:54.1912544+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "radius",
  "DisplayName": "Radius",
  "ShortDisplayName": "Radius",
  "Description": "Radius",
  "LastUpdated": "2022-03-18T10:41:54.1912594+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "notifications",
  "DisplayName": "Notifications",
  "ShortDisplayName": "Notifications",
  "Description": "Notifications",
  "LastUpdated": "2022-03-18T10:41:54.1912745+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "notifications",
  "Name": "notificationserrortext",
  "DisplayName": "Notifications Error Text",
  "ShortDisplayName": "Notifications Error Text",
  "Description": "Notifications Error Text",
  "LastUpdated": "2022-03-18T10:41:54.1912808+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "notificationserrorbackground",
  "DisplayName": "Notifications Error Background",
  "ShortDisplayName": "Notifications Error Background",
  "Description": "Notifications Error Background",
  "LastUpdated": "2022-03-18T10:41:54.1912857+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "notificationswarningtext",
  "DisplayName": "Notifications Warning Text",
  "ShortDisplayName": "Notifications Warning Text",
  "Description": "Notifications Warning Text",
  "LastUpdated": "2024-09-06T15:45:40.2621133+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "notificationswarningtext",
  "DisplayName": "Notifications Warning Text",
  "ShortDisplayName": "Notifications Warning Text",
  "Description": "Notifications Warning Text",
  "LastUpdated": "2022-03-18T10:41:54.1912953+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "notificationswarningtext",
  "DisplayName": "Notifications Warning Text",
  "ShortDisplayName": "Notifications Warning Text",
  "Description": "Notifications Warning Text",
  "LastUpdated": "2022-03-18T10:41:54.1913001+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "notificationserrortext",
  "DisplayName": "Notifications Error Text",
  "ShortDisplayName": "Notifications Error Text",
  "Description": "Notifications Error Text",
  "LastUpdated": "2022-03-18T10:41:54.1913166+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "notificationswarningbackground",
  "DisplayName": "Notifications Warning Background",
  "ShortDisplayName": "Notifications Warning Background",
  "Description": "Notifications Warning Background",
  "LastUpdated": "2022-03-18T10:41:54.1913212+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "notificationsinfotext",
  "DisplayName": "Notifications Info Text",
  "ShortDisplayName": "Notifications Info Text",
  "Description": "Notifications Info Text",
  "LastUpdated": "2022-03-18T10:41:54.1913259+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "notificationsinfobackground",
  "DisplayName": "Notifications Info Background",
  "ShortDisplayName": "Notifications Info Background",
  "Description": "Notifications Info Background",
  "LastUpdated": "2022-03-18T10:41:54.191342+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "notificationssuccesstext",
  "DisplayName": "Notifications Success Text",
  "ShortDisplayName": "Notifications Success Text",
  "Description": "Notifications Success Text",
  "LastUpdated": "2022-03-18T10:41:54.1913464+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "notificationssuccessbackground",
  "DisplayName": "Notifications Success Background",
  "ShortDisplayName": "Notifications Success Background",
  "Description": "Notifications Success Background",
  "LastUpdated": "2022-03-18T10:41:54.1913507+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "shadows",
  "DisplayName": "Shadows",
  "ShortDisplayName": "Shadows",
  "Description": "Shadows",
  "LastUpdated": "2022-03-18T10:41:54.1913551+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "deleted",
  "DisplayName": "Deleted",
  "ShortDisplayName": "Deleted",
  "Description": "Deleted",
  "LastUpdated": "2023-03-13T15:35:35.019476+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "context",
  "DisplayName": "Context",
  "ShortDisplayName": "Context",
  "Description": "Context",
  "LastUpdated": "2023-03-23T15:54:18.6532785+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "displayname",
  "DisplayName": "Display Name",
  "ShortDisplayName": "Display Name",
  "Description": "Display Name",
  "LastUpdated": "2023-03-23T15:54:48.5199141+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "url",
  "DisplayName": "Url",
  "ShortDisplayName": "Url",
  "Description": "Url",
  "LastUpdated": "2023-03-23T15:56:47.2767372+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "rawjson",
  "DisplayName": "Raw JSON",
  "ShortDisplayName": "Raw JSON",
  "Description": "Raw JSON",
  "LastUpdated": "2023-03-23T15:57:39.5924481+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "properties",
  "DisplayName": "Properties",
  "ShortDisplayName": "Properties",
  "Description": "Properties",
  "LastUpdated": "2023-03-23T15:58:12.9515171+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "endpoints",
  "DisplayName": "Endpoints",
  "ShortDisplayName": "Endpoints",
  "Description": "Endpoints",
  "LastUpdated": "2023-03-23T15:58:48.7042054+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "dependson",
  "DisplayName": "Depends On",
  "ShortDisplayName": "Depends On",
  "Description": "Depends On",
  "LastUpdated": "2023-03-23T15:59:05.5110222+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "name",
  "DisplayName": "Name",
  "ShortDisplayName": "Name",
  "Description": "Name",
  "LastUpdated": "2023-03-23T16:00:00.7315076+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "type",
  "DisplayName": "Type",
  "ShortDisplayName": "Type",
  "Description": "Type",
  "LastUpdated": "2023-03-23T16:00:13.2127947+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "servertype",
  "DisplayName": "Server Type",
  "ShortDisplayName": "Server Type",
  "Description": "Server Type",
  "LastUpdated": "2023-03-23T16:00:29.5051989+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "servertypename",
  "DisplayName": "Server Type Name",
  "ShortDisplayName": "Server Type Name",
  "Description": "Server Type Name",
  "LastUpdated": "2023-03-23T16:01:34.3578392+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "template",
  "DisplayName": "Template",
  "ShortDisplayName": "Template",
  "Description": "Template",
  "LastUpdated": "2023-03-23T16:01:50.0355951+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "shortdisplayname",
  "DisplayName": "Short Display Name",
  "ShortDisplayName": "Short Display Name",
  "Description": "Short Display Name",
  "LastUpdated": "2023-03-23T16:02:17.6061646+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "description",
  "DisplayName": "Description",
  "ShortDisplayName": "Description",
  "Description": "Description",
  "LastUpdated": "2023-03-23T16:02:37.7283901+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "isgeneric",
  "DisplayName": "Is Generic",
  "ShortDisplayName": "Is Generic",
  "Description": "Is Generic",
  "LastUpdated": "2023-03-23T16:02:53.4807459+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "isvaluetype",
  "DisplayName": "Is Value Type",
  "ShortDisplayName": "Is Value Type",
  "Description": "Is Value Type",
  "LastUpdated": "2023-03-23T16:03:11.3497613+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "isreadonly",
  "DisplayName": "Is Read Only",
  "ShortDisplayName": "Is Read Only",
  "Description": "Is Read Only",
  "LastUpdated": "2023-03-23T16:03:27.9645783+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "isrequired",
  "DisplayName": "Is Required",
  "ShortDisplayName": "Is Required",
  "Description": "Is Required",
  "LastUpdated": "2023-03-23T16:03:44.6688443+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "methods",
  "DisplayName": "Methods",
  "ShortDisplayName": "Methods",
  "Description": "Methods",
  "LastUpdated": "2023-03-23T16:05:21.2740882+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "endpoint",
  "DisplayName": "Endpoint",
  "ShortDisplayName": "Endpoint",
  "Description": "Endpoint",
  "LastUpdated": "2023-03-23T16:05:36.6000531+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "foreignkey",
  "DisplayName": "Foreign Key",
  "ShortDisplayName": "Foreign Key",
  "Description": "Foreign Key",
  "LastUpdated": "2023-03-23T16:06:12.8044599+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "relatedentity",
  "DisplayName": "Related Entity",
  "ShortDisplayName": "Related Entity",
  "Description": "Related Entity",
  "LastUpdated": "2023-03-23T16:08:01.2413259+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "request",
  "DisplayName": "Request",
  "ShortDisplayName": "Request",
  "Description": "Request",
  "LastUpdated": "2023-07-04T13:20:58.6966514+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "path",
  "DisplayName": "Path",
  "ShortDisplayName": "Path",
  "Description": "Path",
  "LastUpdated": "2023-07-04T13:21:48.4124456+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "httpmethod",
  "DisplayName": "HTTP Method",
  "ShortDisplayName": "HTTP Method",
  "Description": "HTTP Method",
  "LastUpdated": "2023-07-04T13:22:18.3468373+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "headers",
  "DisplayName": "Headers",
  "ShortDisplayName": "Headers",
  "Description": "Headers",
  "LastUpdated": "2023-07-04T13:22:40.1242231+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "add",
  "DisplayName": "Add",
  "ShortDisplayName": "Add",
  "Description": "Add",
  "LastUpdated": "2023-07-04T13:23:00.1450217+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "delete",
  "DisplayName": "Delete",
  "ShortDisplayName": "Delete",
  "Description": "Delete",
  "LastUpdated": "2023-07-04T13:25:27.3691088+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "body",
  "DisplayName": "Body",
  "ShortDisplayName": "Body",
  "Description": "Body",
  "LastUpdated": "2023-07-04T13:27:13.4329543+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "response",
  "DisplayName": "Response",
  "ShortDisplayName": "Response",
  "Description": "Response",
  "LastUpdated": "2023-07-04T13:27:34.1989595+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "value",
  "DisplayName": "Value",
  "ShortDisplayName": "Value",
  "Description": "Value",
  "LastUpdated": "2023-07-04T13:28:02.9395264+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "apiurl",
  "DisplayName": "Api Url",
  "ShortDisplayName": "Api Url",
  "Description": "Api Url",
  "LastUpdated": "2023-07-04T13:28:55.6491212+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "send",
  "DisplayName": "Send",
  "ShortDisplayName": "Send",
  "Description": "Send",
  "LastUpdated": "2023-07-04T13:29:27.6162927+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "Debug",
  "Name": "explore",
  "DisplayName": "Explore",
  "ShortDisplayName": "Explore",
  "Description": "Explore",
  "LastUpdated": "2023-07-24T10:54:59.7677267+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "viewhistory",
  "DisplayName": "View History",
  "ShortDisplayName": "View History",
  "Description": "View History",
  "LastUpdated": "2023-07-28T14:01:47.4945707+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "version",
  "DisplayName": "Version",
  "ShortDisplayName": "Version",
  "Description": "Version",
  "LastUpdated": "2023-07-28T14:02:05.322874+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "type",
  "DisplayName": "Type",
  "ShortDisplayName": "Type",
  "Description": "Type",
  "LastUpdated": "2023-08-01T17:34:59.620792+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "appdeleted",
  "DisplayName": "App Deleted",
  "ShortDisplayName": "App Deleted",
  "Description": "App Deleted",
  "LastUpdated": "2023-11-15T14:22:49.0408731+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Resource",
                Data = """
{
  "Culture": "",
  "Key": "CMS",
  "Name": "tenantHasAppsCannotDelete",
  "DisplayName": "Tenant has apps. Cannot delete tenant.",
  "ShortDisplayName": "Tenant has apps. Cannot delete tenant.",
  "Description": "Tenant has apps. Cannot delete tenant.",
  "LastUpdated": "2026-03-02T10:20:33.3808822+00:00"
}
"""
            },
        ]
    };

    static Package Layouts => new()
    {
        Name = "Content Management Layouts",
        Category = "CMS",
        Description = "Content Management Layouts.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/Layout",
                Data = """
{
  "Name": "Default",
  "HeaderHtml": "<meta property=\"keywords\" content=\"[page[keywords]]\" />\n<meta property=\"description\" content=\"[page[description]]\" />\n<meta property=\"og:locale\" content=\"[[culture]]\" />\n<meta property=\"og:type\" content=\"website\" />\n<meta property=\"og:title\" content=\"Welcome to [app[name]]\" />\n<meta property=\"og:description\" content=\"[resource_displayname[sitedescription]]\" />\n<meta property=\"og:url\" content=\"[app[root]]\" />\n<meta property=\"og:site_name\" content=\"[app[name]]\" />\n<meta name=\"theme-color\" content=\"#0E4A7A\">\n<meta charset=\"utf-8\"/>\n\n<link rel=\"canonical\" href=\"[page[url]]\" />\n<link rel=\"stylesheet\" media=\"screen\" href=\"/everything.min.css\" />\n<link rel=\"stylesheet\" href=\"https://cdn.jsdelivr.net/npm/file-icon-vectors@1.0.0/dist/file-icon-vectors.min.css\" />\n\n[theme[template]]",
  "Html": "<nav class=\"navbar fixed-top bg-body border-bottom-separator\">\n\t<div class=\"container\">\n\t\t<div class=\"col-md-2\">\n\t\t\t<a href=\"/\" title=\"[app[name]]\"><img class=\"header-logo\" src=\"[app[root]]Api/DMS/Content/CompanyLogoTransparent.png\" alt=\"[app[name]]\" /></a>\n\t\t</div>\n\t\t<div class=\"col-md-8\">\n\t\t\t[component[topnav]]\n\n\t\t\t<h2>[page[title]]</h2>\n\t\t</div>\n\t\t<div class=\"col-md-2 text-end details\" style=\"padding-right: 10px;\">\n\t\t\t[component[CultureFlags]]\n\t\t\t<div style=\"width: 100%; clear: both;\">\n\t\t\t\t<span id=\"date\"></span> <span id=\"time\"></span>\n\t\t\t</div>\n\t\t\t[component[UserProfile]]\n\t\t\t<span class=\"k-icon k-i-infoCircleIcon\"></span><a href=\"/Documentation\" class=\"ps-1\">[resource_displayname[help]]</a>\n\t\t</div>\n\t</div>\n</nav>\n\n<div class=\"content-body\">\n\t[content[body]class=container-xxl]\n</div>\n<span class=\"backgroundAnimation\"></span>\n\n<nav class=\"navbar fixed-bottom bg-body-tertiary\">\n\t<footer class=\"container pageFooter\">\n\t\t<div class=\"col-md-12 text-end\">\n\t\t\t&copy; 2026, cCoder\n\t\t</div>\n\t</footer>\n</nav>\n\n<script src=\"/everything.min.js\" crossorigin=\"anonymous\"></script>\n<script src=\"https://cdnjs.cloudflare.com/ajax/libs/monaco-editor/0.20.0/min/vs/loader.min.js\"></script>\n<script src=\"/dependencies/kendo/kendo-ui-license.js\"></script>\n\n<script>\n    [script[DefaultResourcing]]\n    [script[KendoCultures]]\n    type.dateFormat = \"[resource_displayname[dateformat]]\";\n    type.shortDateFormat = \"[resource_displayname[shortdateformat]]\";\n    type.moneyFormat = \"[resource_displayname[moneyformat]]\";\n    type.aggregateMoneyFormat = \"[resource_displayname[aggregateMoneyFormat]]\";\n\n    initContent();\n\n    kendo.setDefaults('iconType', 'svg');\n\n    const targetNode = document.body;\n    const observerConfig = { attributes: true, childList: true, subtree: true };\n    const observer = new MutationObserver(initIcons);\n    observer.observe(targetNode, observerConfig);\n\n    function initIcons() {\n        observer.disconnect();\n\n        $(\".k-icon:not(.k-svg-icon):not(.k-font-icon)\").each(function() {\n            var iconName = $(this).attr('class').split(' ').filter(function(cls) {\n                return cls !== 'k-icon';\n            })[0].replace(/^k-i-/, \"\").replace(\"source-code\", \"code\");\n\n            var svgIcon = kendo.ui.icon({ icon: iconName, type: 'svg' });\n\n            if(svgIcon != `<span class=\"k-icon k-svg-icon\" aria-hidden=\"true\"></span>`) {\n                $(this).replaceWith(svgIcon);\n            }\n        });\n\n        observer.observe(targetNode, observerConfig);\n    }\n</script>",
  "LastUpdated": "2025-08-20T15:02:56.1065123+01:00",
  "Script": null
}
"""
            },
            new PackageItem
            {
                Type = "Core/Layout",
                Data = """
{
  "Name": "Documentation",
  "HeaderHtml": "<meta property=\"keywords\" content=\"[page[keywords]]\" />\n<meta property=\"description\" content=\"[page[description]]\" />\n<meta property=\"og:locale\" content=\"[[culture]]\" />\n<meta property=\"og:type\" content=\"website\" />\n<meta property=\"og:title\" content=\"Welcome to [app[name]]\" />\n<meta property=\"og:description\" content=\"[resource_displayname[sitedescription]]\" />\n<meta property=\"og:url\" content=\"[app[root]]\" />\n<meta property=\"og:site_name\" content=\"[app[name]]\" />\n<meta name=\"theme-color\" content=\"#0E4A7A\">\n<meta charset=\"utf-8\"/>\n\n<link rel=\"canonical\" href=\"[page[url]]\" />\n<link rel=\"stylesheet\" media=\"screen\" href=\"/everything.min.css\" />\n<link rel=\"stylesheet\" href=\"https://cdn.jsdelivr.net/npm/file-icon-vectors@1.0.0/dist/file-icon-vectors.min.css\" />\n\n[theme[template]]\n\n<style>\n    :root {\n\t\t--header-height: 118px;\n\t}\n\n    td, th {\n        border: 1px solid #CCC;\n        padding: 2px 4px;\n    }\n\n    section[name=body] h2, h3, h4 {\n        margin-bottom: 10px;\n        margin-left: 0;\n        padding-left: 0;\n        margin-bottom: 0;\n    }\n\n    p { max-width: 90%; }\n    img { max-width: 99%; }\n\n    .border-right { border-right: [theme[border.style]]; }\n\n    .Documentation .component {\n        box-shadow: none;\n        border: none;\n    }\n\n\t.component[name=Sidenav] {\n\t\theight: auto;\n        padding-left: 20px;\n\t}\n\n\t.component[name=Sidenav] ul.submenu {\n\t\tlist-style-type: none;\n        --bs-nav-link-padding-y: 4px;\n        margin: 5px 0;\n        padding-left: 14px;\n\t}\n\n    .component[name=Sidenav] .navbar-nav .nav-link.active {\n        color: [theme[colours.secondary]];\n        border-bottom: solid 2px [theme[colours.secondary]];\n    }\n\n    .component[name=Sidenav] .navbar-nav .nav-link {\n        padding: 2px;\n        margin: 2px;\n        display: inline;\n    }\n</style>",
  "Html": "<nav class=\"navbar fixed-top bg-body border-bottom-separator\">\n\t<div class=\"container\">\n\t\t<div class=\"col-md-2\">\n\t\t\t<a href=\"/\" title=\"[app[name]]\"><img class=\"header-logo\" src=\"[app[root]]Api/DMS/Content/CompanyLogoTransparent.png\" alt=\"[app[name]]\" /></a>\n\t\t</div>\n\t\t<div class=\"col-md-8\">\n\t\t\t[component[topnav]]\n\n\t\t\t<h2>[page[title]]</h2>\n\t\t</div>\n\t\t<div class=\"col-md-2 text-end details\" style=\"padding-right: 10px;\">\n\t\t\t[component[CultureFlags]]\n\t\t\t<div style=\"width: 100%; clear: both;\">\n\t\t\t\t<span id=\"date\"></span> <span id=\"time\"></span>\n\t\t\t</div>\n\t\t\t[component[UserProfile]]\n\t\t\t<span class=\"k-icon k-i-infoCircleIcon\"></span><a href=\"/Documentation\" class=\"ps-1\">[resource_displayname[help]]</a>\n\t\t</div>\n\t</div>\n</nav>\n<div class=\"content-body\">\n\t<div class=\"row\">\n\t\t<div class=\"col-xxl-3 col-xl-4 col-lg-5 align-content-start border-right\">\n\t\t\t<h3 style=\"margin: 10px 20px;\">cCoder Documentation</h3>\n\t\t\t[component[Sidenav]]\n\t\t</div>\n\t\t<div class=\"col-xxl-9 col-xl-8 col-lg-7\" style=\"padding-left: 20px;\">\n\t\t\t[content[body]class=container-xxl]\n\t\t</div>\n\t</div>\n</div>\n<nav class=\"navbar fixed-bottom bg-body-tertiary\">\n\t<footer class=\"container pageFooter\">\n\t\t<div class=\"col-md-12 text-end\">\n\t\t\t&copy; 2026, cCoder\n\t\t</div>\n\t</footer>\n</nav>\n\n<script src=\"/everything.min.js\" crossorigin=\"anonymous\"></script>\n<script src=\"https://cdnjs.cloudflare.com/ajax/libs/monaco-editor/0.20.0/min/vs/loader.min.js\"></script>\n<script src=\"/dependencies/kendo/kendo-ui-license.js\"></script>\n\n<script>\n\t$(function() {\n      $(\".navbar-nav > .nav-item\").on(\"mouseover\", async function(e) {\n            var parent = $(e.currentTarget);\n            if(!parent.attr(\"data-loaded\"))\n            {\n                 parent.attr(\"data-loaded\", \"true\");\n                  var id = parent.attr(\"data-id\");\n                  var subMenu = await api.get(\"ContentManagement/Page(\" + id + \")/Menu()?culture=\" + session.culture);\n                  parent.append(subMenu.Item);\n            }\n      });\n\n    [script[DefaultResourcing]]\n    [script[KendoCultures]]\n    type.dateFormat = \"[resource_displayname[dateformat]]\";\n    type.shortDateFormat = \"[resource_displayname[shortdateformat]]\";\n    type.moneyFormat = \"[resource_displayname[moneyformat]]\";\n    type.aggregateMoneyFormat = \"[resource_displayname[aggregateMoneyFormat]]\";\n    initContent();\n   });\n</script>",
  "LastUpdated": "2025-08-20T15:03:08.0443952+01:00",
  "Script": null
}
"""
            },
        ]
    };

    static Package Templates => new()
    {
        Name = "Content Management Templates",
        Category = "CMS",
        Description = "Content Management Templates.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/Template",
                Data = """
{
  "Name": "ForgotPassword",
  "ResourceKey": "ForgotPassword",
  "RawString": "<html style=\"font-family: [theme[font.family]]; width:800px; margin:0 auto; padding:0;\">\n    <head>\n        <title>[email[subject]]</title>\n        <style>\n            * { font-size: [theme[font.size]]; font-family: [theme[font.family]]; color: #1F2933; }\n            a { color: [theme[colours.links]]; cursor: pointer; }\n            hr { border-top: [theme[border.style]]; }\n        </style>\n    </head>\n    <body style=\"width: 800px; margin: 20px auto; padding: 0; background: white;\">\n        <header style=\"padding: 20px 30px 0;\">\n            <a href=\"[app[root]]\" style=\"font-size: 28px; font-weight: 700; text-decoration: none; color: [theme[colours.primary]];\">cCoder</a>\n            <h2 style=\"background: [theme[colours.primary]]; color: [theme[colours.text2]]; padding: 12px 16px; font-size: 140%; margin-top: 16px;\">Reset your password</h2>\n        </header>\n        <div style=\"margin: 10px auto; padding: 5px 40px 30px;\">\n            <p>[resource_displayname[ForgotPasswordBody]]</p><p>[resource_displayname[PleaseClick]] <a href=\"[app[root]]/ResetPassword?token=[model[EncodedToken]]&uid=[model[CoreUser.Id]]\">[resource_displayname[Here]]</a> [resource_displayname[ToReset]]</p>\n        </div>\n        <div style=\"background-color: [theme[colours.primary]]; color: [theme[colours.text2]]; width: 100%;\">\n            <p style=\"padding: 10px; text-align: right; background: [theme[colours.primary]]; color: [theme[colours.text2]]; margin: 0;\">&copy; 2026, cCoder</p>\n        </div>\n    </body>\n</html>",
  "LastUpdated": "2024-06-12T15:21:02.2427236+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Template",
                Data = """
{
  "Name": "UserInvite",
  "RawString": "<html style=\"font-family: [theme[font.family]]; width:800px; margin:0 auto; padding:0;\">\n    <head>\n        <title>[email[subject]]</title>\n        <style>\n            * { font-size: [theme[font.size]]; font-family: [theme[font.family]]; color: #1F2933; }\n            a { color: [theme[colours.links]]; cursor: pointer; }\n            hr { border-top: [theme[border.style]]; }\n        </style>\n    </head>\n    <body style=\"width: 800px; margin: 20px auto; padding: 0; background: white;\">\n        <header style=\"padding: 20px 30px 0;\">\n            <a href=\"[app[root]]\" style=\"font-size: 28px; font-weight: 700; text-decoration: none; color: [theme[colours.primary]];\">cCoder</a>\n            <h2 style=\"background: [theme[colours.primary]]; color: [theme[colours.text2]]; padding: 12px 16px; font-size: 140%; margin-top: 16px;\">You have been invited</h2>\n        </header>\n        <div style=\"margin: 10px auto; padding: 5px 40px 30px;\">\n            <p>[resource_description[InvitationStatement]]</p><p>[resource_displayname[Click]] <a href=\"[app[root]]/AcceptInvite?user=[model[SSOUser.Id]]&e=[model[CoreUser.Email]]&t=[model[EncodedToken]]\">[resource_displayname[Here]]</a> to complete your account setup and sign in.</p>\n        </div>\n        <div style=\"background-color: [theme[colours.primary]]; color: [theme[colours.text2]]; width: 100%;\">\n            <p style=\"padding: 10px; text-align: right; background: [theme[colours.primary]]; color: [theme[colours.text2]]; margin: 0;\">&copy; 2026, cCoder</p>\n        </div>\n    </body>\n</html>",
  "LastUpdated": "2022-11-04T11:17:52.8650502+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Template",
                Data = """
{
  "Name": "ConfirmRegistration",
  "ResourceKey": "Register",
  "RawString": "<html style=\"font-family: [theme[font.family]]; width:800px; margin:0 auto; padding:0;\">\n    <head>\n        <title>[email[subject]]</title>\n        <style>\n            * { font-size: [theme[font.size]]; font-family: [theme[font.family]]; color: #1F2933; }\n            a { color: [theme[colours.links]]; cursor: pointer; }\n            hr { border-top: [theme[border.style]]; }\n        </style>\n    </head>\n    <body style=\"width: 800px; margin: 20px auto; padding: 0; background: white;\">\n        <header style=\"padding: 20px 30px 0;\">\n            <a href=\"[app[root]]\" style=\"font-size: 28px; font-weight: 700; text-decoration: none; color: [theme[colours.primary]];\">cCoder</a>\n            <h2 style=\"background: [theme[colours.primary]]; color: [theme[colours.text2]]; padding: 12px 16px; font-size: 140%; margin-top: 16px;\">Confirm your registration</h2>\n        </header>\n        <div style=\"margin: 10px auto; padding: 5px 40px 30px;\">\n            <p>[resource_displayname[Greeting]] [model[CoreUser.DisplayName]]</p><p>[resource_displayname[Body]]</p><p>[resource_displayname[Click]] <a href=\"[app[root]]/MyRegistrations?u=[model[SSOUser.Id]]&t=[model[Token]]\">[resource_displayname[Here]]</a> [resource_displayname[ToConfirm]]</p>\n        </div>\n        <div style=\"background-color: [theme[colours.primary]]; color: [theme[colours.text2]]; width: 100%;\">\n            <p style=\"padding: 10px; text-align: right; background: [theme[colours.primary]]; color: [theme[colours.text2]]; margin: 0;\">&copy; 2026, cCoder</p>\n        </div>\n    </body>\n</html>",
  "LastUpdated": "2022-11-14T12:20:33.4463482+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Template",
                Data = """
{
  "Name": "Theme-Default",
  "RawString": "<style>\t\r\n\t/* Variables. */\r\n\t:root {\r\n\t\t--bs-body-font-size: 12px;\r\n\t\t--bs-nav-link-font-size: 14px;\r\n\t\t--bs-breakpoint-xs: 0;\r\n\t\t--bs-breakpoint-sm: 720px;\r\n\t\t--bs-breakpoint-md: 1080px;\r\n\t\t--bs-breakpoint-lg: 1440px;\r\n\t\t--bs-breakpoint-xl: 1920px;\r\n\t\t--bs-breakpoint-xxl: 2160px;\r\n\r\n\t\t--header-height: 121px;\r\n\t\t--footer-height: 38px;\r\n\r\n\t\t--bs-body-font-size: [theme[font.size]];\r\n\t\t--kendo-font-size: [theme[font.size]];\r\n\t\t--bs-body-font-family: [theme[font.family]];\r\n\t\t--kendo-font-family: [theme[font.family]];\r\n\t}\r\n\r\n\t.tab-control { height: calc(100% - 5px); border-radius: [theme[border.radius]]; } \r\n\t.tab-control > nav > .nav-tabs > button { margin-left: 10px; }\r\n\t.tab-content { height: calc(100% - 40px); border-radius: [theme[border.radius]]; }\r\n\t.tab-content > .tab-pane > .component { height: 100%; border-radius: [theme[border.radius]]; }\r\n\t.tab-content .editor { height: calc(100% - 5px - [theme[margins]]) !important; }\r\n\r\n\t.k-toolbar ~ .tab-control, .k-toolbar ~ .k-grid {\r\n\t\theight: calc(100% - 56px); // 51px height for kendo toolbar + 5px margin top on nav.\r\n\t\tborder-radius: [theme[border.radius]];\r\n\t}\r\n\r\n\t.k-tabstrip {\r\n\t\theight: 100%;\r\n\t}\r\n\r\n\t.dropdown-menu {\r\n\t\t--bs-dropdown-link-active-color: [theme[colours.secondary]]; \r\n\t\t--bs-dropdown-link-active-bg: transparent; \r\n\t\t--bs-dropdown-link-hover-color: [theme[colours.secondary]];\r\n\t\t--bs-dropdown-link-hover-bg: transparent;\r\n\t}\r\n\r\n\t.nav-link {\r\n\t\t--bs-nav-link-color: [theme[colours.primary]];\r\n\t}\r\n\r\n\t.nav-link:focus, .nav-link:hover {\r\n\t\t--bs-nav-link-hover-color: [theme[colours.secondary]];\r\n\t\tborder-bottom: [theme[border.style]];\r\n\t}\r\n\r\n\t.nav-link.active {\r\n\t\t--bs-nav-tabs-link-active-color: [theme[colours.secondary]];\r\n\t\tborder-bottom: [theme[border.style]];\r\n\t}\r\n\r\n\t.btn-primary {\r\n\t\t--bs-btn-color: [theme[colours.text2]];\r\n\t\t--bs-btn-bg: [theme[colours.primary]];\r\n\t\t--bs-btn-border-color: [theme[colours.primary]];\r\n\t\t--bs-btn-hover-color: [theme[colours.text2]];\r\n\t\t--bs-btn-hover-bg: [theme[colours.secondary]];\r\n\t\tborder: [theme[border.style]];\r\n\t\t--bs-btn-hover-border-color: [theme[colours.primary]];\r\n\t\t--bs-btn-focus-shadow-rgb: 49,132,253;\r\n\t\t--bs-btn-active-color: [theme[colours.text2]];\r\n\t\t--bs-btn-active-bg: [theme[colours.secondary]];\r\n\t\t--bs-btn-active-border-color: [theme[colours.primary]];\r\n\t\t--bs-btn-active-shadow: [theme[shadows]];\r\n\t\t--bs-btn-disabled-color: [theme[colours.text2]];\r\n\t\t--bs-btn-disabled-bg: [theme[colours.primary]];\r\n\t\t--bs-btn-disabled-border-color: [theme[colours.primary]];\r\n\t}\r\n\r\n\t.btn-primary > span.k-icon {\r\n\t\tcolor: [theme[colours.secondary]];\r\n\t}\r\n\r\n\t.btn-primary:hover > span.k-icon {\r\n\t\tcolor: [theme[colours.primary]];\r\n\t}\r\n\r\n\t.btn-secondary {\r\n\t\t--bs-btn-color: [theme[colours.text2]];\r\n\t\t--bs-btn-bg: [theme[colours.secondary]];\r\n\t\t--bs-btn-border-color: [theme[colours.secondary]];\r\n\t\t--bs-btn-hover-color: [theme[colours.text2]];\r\n\t\t--bs-btn-hover-bg: [theme[colours.primary]];\r\n\t\tborder: [theme[border.style]];\r\n\t\t--bs-btn-hover-border-color: [theme[colours.secondary]];\r\n\t\t--bs-btn-focus-shadow-rgb: 49,132,253;\r\n\t\t--bs-btn-active-color: [theme[colours.text2]];\r\n\t\t--bs-btn-active-bg: [theme[colours.primary]];\r\n\t\t--bs-btn-active-border-color: [theme[colours.secondary]];\r\n\t\t--bs-btn-active-shadow: [theme[shadows]];\r\n\t\t--bs-btn-disabled-color: [theme[colours.text2]];\r\n\t\t--bs-btn-disabled-bg: [theme[colours.secondary]];\r\n\t\t--bs-btn-disabled-border-color: [theme[colours.secondary]];\r\n\t}\r\n\r\n\t.btn-secondary > span.k-icon {\r\n\t\tcolor: [theme[colours.primary]];\r\n\t}\r\n\r\n\t.btn-secondary:hover > span.k-icon {\r\n\t\tcolor: [theme[colours.secondary]];\r\n\t}\r\n\r\n\t/* Layout */\r\n\t.header-logo { max-height: 100px; padding: 5px 10px; }\r\n\r\n\t.content-body {\r\n\t\theight: calc(100vh - var(--header-height) - var(--footer-height));\r\n\t\tmargin-top: var(--header-height);\r\n\t\tmargin-bottom: var(--footer-height);\r\n\t\toverflow: auto;\r\n\t\twidth: calc(100% - 2px);\r\n\t}\r\n\r\n\t.content-body div.row \t\t{ --bs-gutter-x: 0; --bs-gutter-y: [theme[margins]]; margin-top: 0; max-height: 100%; }\r\n\t.content-body div.row > * \t{ --bs-gutter-y: 0; }\r\n\r\n\t.pageFooter \t\t{ background: [theme[colours.primary]]; color: white; }\r\n\t.pageFooter > div \t{ padding: 10px; }\r\n\r\n\th2 {\r\n\t\tpadding: 10px 25px;\r\n\t\tcolor: [theme[colours.primary]];\r\n\t\tfont-size: 180%;\r\n\t}\r\n\r\n\t.border-bottom-separator {\r\n\t\tborder-bottom: solid 4px [theme[colours.primary]];\r\n\t}\r\n\r\n\th3 {\r\n\t\tpadding: 10px;\r\n\t\tborder-bottom: solid 2px [theme[colours.secondary]];\r\n\t\tcolor: [theme[colours.secondary]];\r\n\t\twidth: 80%;\r\n\t\tfont-size: 120%;\r\n\t\tmargin-bottom: 0;\r\n\t}\r\n\r\n\th3.red {\r\n\t\tcolor: red;\r\n\t\tborder-bottom: solid 2px red;\r\n\t}\r\n\r\n\th4 {\r\n\t\tpadding: 8px;\r\n\t\tborder-bottom: solid 2px [theme[colours.secondary]];\r\n\t\tcolor: [theme[colours.secondary]];\r\n\t\tfont-size: 110%;\r\n\t\twidth: 80%;\r\n\t\tmargin-bottom: 0;\r\n\t}\r\n\r\n\tmarquee { \r\n\t\tfont-weight: bold; \r\n\t\tcolor: [theme[colours.primary]]; \r\n\t\tfont-size: 180%; \r\n\t\tbox-shadow: [theme[shadows]];\r\n\t\t-moz-box-shadow: [theme[shadows]];\r\n\t\t-webkit-box-shadow: [theme[shadows]];\r\n\t\tborder-width: [theme[border.width]];\r\n\t\tborder-radius: [theme[border.radius]];\r\n\t\tborder: [theme[border.style]];\r\n\t\tmargin: [theme[margins]];\r\n\t\tpadding: 15px;\r\n\r\n\t\tbackground: rgba(255, 255, 255, 0.4);\r\n\t\tbackdrop-filter: blur(4px);\r\n\t\t-webkit-backdrop-filter: blur(4px);\r\n\t}\r\n\r\n\t/* Platform configuration */\r\n\t.component[name=TopNav] { display: inline-block; }\r\n\r\n\t.component {\r\n\t\tbox-shadow: [theme[shadows]];\r\n\t\t-moz-box-shadow: [theme[shadows]];\r\n\t\t-webkit-box-shadow: [theme[shadows]];\r\n\t\tborder-width: [theme[border.width]];\r\n\t\tborder-radius: [theme[border.radius]];\r\n\t\tborder: [theme[border.style]];\r\n\t\t\r\n\t\tmargin: [theme[margins]];\r\n\t\theight: calc(100% - [theme[margins]] - [theme[margins]]);\r\n\r\n\t\tbackground: rgba(255, 255, 255, 0.4);\r\n\t\t//backdrop-filter: blur(4px);\r\n\t\t-webkit-backdrop-filter: blur(4px);\r\n\t}\r\n\r\n\t.hidden > .component {\r\n\t\tbox-shadow: none;\r\n\t\t-moz-box-shadow: none;\r\n\t\t-webkit-box-shadow: none;\r\n\t\tborder-width: 0;\r\n\t\tborder-radius: 0;\r\n\t\tborder: none;\r\n\t\tmargin: 0;\r\n\t\theight: 0;\r\n\t\tbackground: none;\r\n\t\t-webkit-backdrop-filter: none;\r\n\t}\r\n\r\n\t.component > h3 { border-radius: [theme[border.radius]] [theme[border.radius]] 0 0; }\r\n\t.component > h4 { border-radius: [theme[border.radius]] [theme[border.radius]] 0 0; }\r\n\t.component > .k-grid { margin: 0; max-height: 100%; }\r\n\r\n\t/* .component .chart, .component .k-chart {\r\n\t\theight: 350px;\r\n\t\tpadding-bottom: 5px;\r\n\t} */\r\n\r\n\t.component.details > .col { flex: 1; vertical-align: top; width: calc(50% - [theme[margins]]); display: inline-block; }\r\n\r\n\t.component.details > .col > .details {\r\n\t\tbox-shadow: [theme[shadows]];\r\n\t\t-moz-box-shadow: [theme[shadows]];\r\n\t\t-webkit-box-shadow: [theme[shadows]];\r\n\t\tborder-width: [theme[border.width]];\r\n\t\tborder-radius: [theme[border.radius]];\r\n\t\tborder: [theme[border.style]]; \r\n\t\tmargin: [theme[margins]]; \r\n\t}\r\n\r\n\t.component.details > .col.right { margin-left: -[theme[margins]]; }\r\n\r\n\t.component > p {\r\n\t\tpadding: 10px;\r\n\t}\r\n\r\n\t.k-detail-cell>.k-tabstrip {\r\n\t\tmax-height: 750px;\r\n\t}\r\n\r\n\t.k-detail-cell .component {\r\n\t\tbackground: transparent;\r\n\t}\r\n\r\n\t/*Mail Management Email Preview */\r\n\r\n\t.mail-content-frame {\r\n\t\twidth: 100%;\r\n\t\theight: 600px;\r\n\t}\r\n\r\n\t/* Bootstrap overrides. */\r\n\t.navbar > .container { max-width: 100%; padding: 0px; }\r\n\t.navbar { padding: 0px; }\r\n\t.navbar .dropdown-item, .navbar .nav-item { min-width: 150px; }\r\n\t.navbar .dropdown-item a { width: 80%; }\r\n\t.navbar .dropdown-item a.nav-link.active { border-bottom: solid 2px [theme[colours.primary]]; }\r\n\r\n\t.dropdown-item:hover > .dropdown-menu { \r\n\t\tdisplay: block; \t\t\r\n\t\tbox-shadow: [theme[shadows]];\r\n\t\t-moz-box-shadow: [theme[shadows]];\r\n\t\t-webkit-box-shadow: [theme[shadows]];\r\n\t\tborder-width: [theme[border.width]];\r\n\t\tborder-radius: [theme[border.radius]];\r\n\t\tborder: [theme[border.style]]; \r\n\t}\r\n\r\n\t.navbar .dropdown-item.active { border-bottom: solid 2px [theme[colours.primary]]; }\r\n\t\r\n\t.container-xxl {\r\n\t\tmax-height: 100%;\r\n\t\tpadding-top: 1px; /* NEVER REMOVE THIS OR YOU'RE FIRED! */\r\n\t}\r\n\r\n\t.component[name=TopNav] { border: none; box-shadow: none; }\r\n\r\n\t/* Bootstrap Cards */\r\n\th3.card-title {color: [theme[colours.secondary]]; margin-bottom: unset;}\r\n\t.card-body{padding-top: 0px; padding-left: 0px; }\r\n\t.card-text{padding-left: 10px; padding-top: 10px;}\r\n\r\n\t/* Login */\r\n\t.component[name=Login] { \r\n\t\tbox-shadow: none; \r\n\t\t-moz-box-shadow: none; \r\n\t\t-webkit-box-shadow: none; \r\n\t\tborder: none;\r\n\t}\r\n\r\n\t.component[name=Login] .row { margin-bottom: calc([theme[margins]] + 20px); }\r\n\r\n\t.component[name=Login] .col { margin: [theme[margins]]; }\r\n\t\r\n\t.component[name=Login] img {\r\n\t\tbox-shadow: [theme[shadows]];\r\n\t\t-moz-box-shadow: [theme[shadows]];\r\n\t\t-webkit-box-shadow: [theme[shadows]];\r\n\t\tborder-width: [theme[border.width]];\r\n\t\tborder-radius: [theme[border.radius]];\r\n\t\tborder: [theme[border.style]]; \r\n\t\tmargin: [theme[margins]]; \r\n\t}\r\n\r\n\t.component[name=Login] form {\r\n\t\tbox-shadow: [theme[shadows]];\r\n\t\t-moz-box-shadow: [theme[shadows]];\r\n\t\t-webkit-box-shadow: [theme[shadows]];\r\n\t\tborder-width: [theme[border.width]];\r\n\t\tborder-radius: [theme[border.radius]];\r\n\t\tborder: [theme[border.style]]; \r\n\t\tmargin: [theme[margins]]; \r\n\t\tpadding: 20px;\r\n\t\tpadding-bottom: 0;\r\n\t}\r\n\r\n\t/* Detailed Nav */\r\n\t.component[name=DetailedNav] > ul.menu > li { \t\t\r\n\t\tbox-shadow: [theme[shadows]];\r\n\t\t-moz-box-shadow: [theme[shadows]];\r\n\t\t-webkit-box-shadow: [theme[shadows]];\r\n\t\tborder-width: [theme[border.width]];\r\n\t\tborder-radius: [theme[border.radius]];\r\n\t\tborder: [theme[border.style]]; \r\n\t\tmargin: [theme[margins]]; \r\n\t\tpadding: 10px;\r\n\t\theight: 130px;\r\n\t}\r\n\r\n\t/* next cutoff details */\r\n\t.component[name=NextCutoffDetails] .nextCutOff { padding: 20px; font-weight: bold; font-size: 150%; }\r\n\t.component[name=NextCutoffDetails] .offerStats { padding: 5px; } \r\n\t.component[name=NextCutoffDetails] .offerStats li { list-style-type: none; padding: 5px; }\r\n\t.component[name=NextCutoffDetails] .offerStats li span { margin-right: 10px; }\r\n\t\r\n\t[name=editorContainer] {\r\n\t\tmin-height: 500px;\r\n\t}\r\n\r\n\t.component[name=Slideshow] {\r\n\t\tmax-height: 210px;\r\n\t} \r\n\r\n\tsection[name=DMS] > div[name=splitter] {\r\n\t\theight: 100%;\r\n\t}\r\n\r\n\t.component[name=InviteUsers] [name=companiesGridContainer] {\r\n\t\t@media(min-height: 750px) {\r\n\t\t\theight: 50vh;\r\n\t\t}\r\n\t}\r\n\r\n\t/* Context Menus */\r\n\t.contextMenu {  \r\n\t\tbackground: white; \r\n\t\tbox-shadow: [theme[shadows]];\r\n\t\t-moz-box-shadow: [theme[shadows]];\r\n\t\t-webkit-box-shadow: [theme[shadows]];\r\n\t\tborder-width: [theme[border.width]];\r\n\t\tborder-radius: [theme[border.radius]];\r\n\t\tborder: [theme[border.style]]; \r\n\t\tz-index: 1000;\r\n\t}\r\n\t.contextMenu > ul { list-style-type: none; padding: 2px; margin: 0; }\r\n\t.contextMenu > ul > li { padding: 4px; border-radius: [theme[border.radius]]; }\r\n\t.contextMenu > ul > li > .k-icon { margin-top:-2px; margin-bottom: 2px; margin-right: 4px; }\r\n\t.contextMenu > ul > li:hover { background: [theme[colours.primary]]; color: white; }\r\n\t.contextMenu > ul > li:hover > .k-icon { color: [theme[colours.secondary]]; }\r\n\r\n\t/*Sprite Based Icon Attachements e.g. Those used in DMS*/\r\n\t.k-sprite { background-image: url(\"[app[root]]Api/DMS/Content/icons.png\"); margin-top: 0; }\r\n\t.root { background-position: 0 0; }\r\n\t.folder { background-position: 0 -16px; }\r\n\t.pdf { background-position: 0 -32px; }\r\n\t.page { background-position: 0 -48px; }\r\n\t.image { background-position: 0 -64px; }\r\n\t.question { background-position: 0 -80px; }\r\n\t.add { background-position: 0 -96px; }\r\n\t.query { background-position: 0 -112px; }\r\n\r\n\t[name=splitter] > .panel.right > .component { \r\n\t\theight: 100%; \r\n\t\tborder: none;\t\t\r\n\t\tbox-shadow: none;\r\n\t\t-moz-box-shadow: none;\r\n\t\t-webkit-box-shadow: none; \r\n\t}\r\n\r\n\tdiv[name=CoreManagementExpanded] {\r\n\t\theight: 100%;\r\n\t\tmin-height: 750px;\r\n\t}\r\n\r\n\tsection[name=AppTheming] > .row {\r\n\t\tmin-height: 350px;\r\n\t}\r\n\r\n\t::-webkit-scrollbar {\r\n\t\twidth: 8px;\r\n\t\theight: 8px;\r\n\t}\r\n\r\n\t/* Track */\r\n\t::-webkit-scrollbar-track {\r\n\t\t/* background: [theme[colours.primary]]; */\r\n\t}\r\n\r\n\t/* Handle */\r\n\t::-webkit-scrollbar-thumb {\r\n\t\tbackground: [theme[colours.secondary]];\r\n        border-radius: [theme[border.radius]];\r\n\t}\r\n\r\n\t/* Handle on hover */\r\n\t::-webkit-scrollbar-thumb:hover {\r\n\t\t/* background: [theme[colours.secondary]]; */\r\n\t}\r\n\r\n\t/*Grid Page Selection Dropdown*/\r\n\tspan.k-input-inner { display: unset !important; }\r\n\t.k-input-md .k-input-inner, .k-picker-md .k-input-inner {padding-inline: 0.5rem; }\r\n\t.k-pager-md .k-pager-sizes .k-dropdownlist, .k-pager-md .k-pager-sizes > select {width: auto;}\r\n\r\n\r\n\t.component[name=CMS] [name=workspace] > iframe {\r\n\t\twidth: 100%;\r\n\t\theight: 100%;\r\n\t}\r\n\r\n\t.component[name=CMS] span[name=showOnMenu] {\r\n\t\tcursor: pointer;\r\n\t}\r\n\t\r\n\t.logLevelOptions > .option { display: inline; }\r\n\r\n</style>",
  "LastUpdated": "2026-02-02T10:15:49.1950809+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Template",
                Data = """
{
  "Name": "Theme-Dark",
  "RawString": "<style name=\"Theme-Default\">\n\n* { font-size: [model[font.size]]; font-family: [model[font.family]]; color: [model[colours.text]]; }\n\na           { color: [model[colours.links]]; cursor: pointer; }\nhr          { border-top: [model[border.style]]; }\n\n/* Headings */\nh1 > a            { font-family: [model[font.family]]; color: [model[colours.primary]]; cursor: pointer; }\nh2                  { font-family: [model[font.family]];color: [model[colours.text2]]; background-color: [model[colours.primary]]; border-top: [model[borders]]; background: [model[colours.primary]]; }\nh2 *               { font-family: [model[font.family]]; color: [model[colours.text2]]; background-color: [model[colours.primary]];  }\nh3                  { padding: 6px 12px;; font-size: 110%; font-family: [model[font.family]]; background: [model[colours.secondary]]; color: [model[colours.text2]]; }\nh3 > span       { font-size: 110%; font-family: [model[font.family]]; color: [model[colours.text2]]; }\nh4                  { padding: 5px 12px; font-size: 100%; font-family: [model[font.family]]; background: [model[colours.secondary]]; color: [model[colours.text2]]; }\nh4 > span       { padding: 5px 12px; font-size: 100%; font-family: [model[font.family]]; color: [model[colours.text2]]; }\n\n/* Forms related tags */\nfieldset            { border: none; }\npre                 { border-width: [model[border.width]]; border-radius: [model[border.radius]]; border: [model[border.style]]; }\ninput               { border-width: [model[border.width]]; border-radius: [model[border.radius]]; border: [model[border.style]]; }\nselect              { background: white; border-width: [model[border.width]]; border-radius: [model[border.radius]]; border: [model[border.style]]; }\ntextarea          { background: white; border-width: [model[border.width]]; border-radius: [model[border.radius]]; border: [model[border.style]]; }\ntextarea:focus { \n    box-shadow: [model[shadows]]; -moz-box-shadow: [model[shadows]]; -webkit-box-shadow: [model[shadows]]; \n    border-width: [model[border.width]]; border-radius: [model[border.radius]]; border: [model[border.style]]; \n}\nselect:hover, \ntextarea:hover          { background-color: white; color: #000; }\nbutton, .button { \n    background-color: [model[colours.secondary]]; color: [model[colours.text2]];\n    box-shadow: [model[shadows]]; -moz-box-shadow: [model[shadows]]; -webkit-box-shadow: [model[shadows]]; \n    border-width: [model[border.width]]; border-radius: [model[border.radius]]; border: [model[border.style]]; \n    padding: 1px 8px;\n    line-height: 24px;\n}\nbutton:hover, .button:hover { background-color: [model[colours.primary]]; color: [model[colours.text2]]; }\nbutton:active, .button:active {  box-shadow: none; -moz-box-shadow: none; -webkit-box-shadow: none;  }\nbutton > span.k-icon, .button > span.k-icon { line-height: 22px; }\n\n/* layout */\nbody { background-repeat: no-repeat;  background-position: bottom right; background-origin: content-box; background-size: 500px; }\nbody > header                          { }\nbody > header > .details           { min-width: 500px;  }\nbody > header > .details > img { max-height: 90px; margin-top: -10px; }\nbody > header > .details > ul    { width: 200px; float: right; }\nbody > header > .details > ul > li { padding:3px; }\nbody > footer                            { margin: 0; background: [model[colours.primary]]; border-top-width: [model[border.width]]; border-top: [model[border.style]]; box-shadow: [model[shadows]]; -moz-box-shadow: [model[shadows]]; -webkit-box-shadow: [model[shadows]];  }\nbody > footer .component         {  background: [model[colours.primary]]; }\nbody > footer .component label { color: #FFFFFF; }\nbody > footer p \t\t                 { color: #FFFFFF; }\nbody > .content[name=body]    {  overflow: auto; }\n   \nmarquee { font-family: [model[font.family]]; color: [model[colours.primary]]; padding: 10px; font-size: 180%; border-width: [model[border.width]]; border: [model[border.style]];  box-shadow: [model[shadows]]; -moz-box-shadow: [model[shadows]]; -webkit-box-shadow: [model[shadows]]; }\n   \n/* kendo hacks */\nspan.k-icon                                       { margin-top: -2px; margin-right: 5px; }\ninput[type=text]:hover                       { background: white; }\ninput[type=text]:focus                       { border-width: [model[border.width]]; border-radius: [model[border.radius]]; border: [model[border.style]]; }\ninput[type=password]:hover              { background: white; }\ninput[type=password]:focus              { border-width: [model[border.width]]; border-radius: [model[border.radius]]; border: [model[border.style]]; }\ninput[type=email]:hover                    { background-color: white; }\ninput[type=email]:focus                    { border-width: [model[border.width]]; border-radius: [model[border.radius]]; border: [model[border.style]]; }\ninput[data-role=datepicker],\ninput[data-role=numerictextbox]       { border-width: [model[border.width]]; border-radius: [model[border.radius]]; border: [model[border.style]]; }\ninput[data-role=datepicker]:focus,\ninput[data-role=numerictextbox]:focus  { border: none; }\n\n/* tabs  */\n.k-tabstrip-wrapper, .k-widget.k-tabstrip  { height: 100%; max-width: calc(100% - (([model[border.width]] + [model[margins]])*2)); max-height: 99.5%; padding-left: calc(([model[border.width]] + [model[margins]])*2) }\n.k-widget.k-tabstrip > .tab.k-content { height: 100%; max-width: 100%; max-height: 99.5%; padding: 0; margin: 0 -3px; }\n[data-role=tabstrip] [role=tab].k-state-active,\n[data-role=tabstrip] [role=tab].k-state-active > span { background: [model[colours.primary]]; color: [model[colours.text2]]; border-bottom: none; }\n\n/* grids */\n.k-grid { max-width: 100%; padding: 0; margin: 0; overflow: hidden; }\n.k-grid a { color: [model[colours.links]]; cursor: pointer; }\n.k-grid th.k-header > a.k-link { font-weight: bold; }\n.k-grid td[role=gridcell] { padding: 3px 5px; color: [model[colours.text]]; }\n.k-grid .k-detail-cell [data-role=tabstrip] .tab.k-state-active { margin-left: 0; border-width: [model[border.width]]; border-radius: [model[border.radius]]; border: [model[border.style]]; }\n.k-header.k-grid-toolbar > button, .k-toolbar > button { margin: 3px; margin-left: 10px; }\n\n.k-filter-menu >  .k-filter-menu-container > .k-action-buttons > .k-button { height: 30px; margin: 0; color: [model[colours.links]]; box-shadow: none; }\n.k-filter-menu >  .k-filter-menu-container > .k-action-buttons > .k-button:hover { background: [model[colours.secondary]]; color: [model[colours.text2]];  box-shadow: none; }\n\n.k-list .k-item.k-state-selected, .k-list-optionlabel.k-state-selected { background-color: [model[colours.secondary]]; }\n.k-list .k-item.k-state-hover.k-state-selected, .k-list .k-item:hover.k-state-selected, .k-list-optionlabel.k-state-hover.k-state-selected, .k-list-optionlabel:hover.k-state-selected { background-color: [model[colours.primary]]; }\n\n/* Notifications */\n.notification                   { padding: 3px 10px; min-width: 200px; max-width: 400px; max-height: 200px; word-wrap: normal; word-break: keep-all; }\n.notification > .k-icon     { margin-right: 10px; }\n.notification > *             { display: inline-block; }\n\n.k-widget.k-notification { border: none; background: transparent; box-shadow: none; }\n.k-widget.k-notification > .notification { color: #222; margin-bottom: 10px; border-radius: 5px; }\n.k-widget.k-notification.k-notification-success > .notification  { color: [model[notifications.success.text]]; background: [model[notifications.success.background]]; border-width: [model[border.width]]; border-radius: [model[border.radius]]; border: [model[border.style]]; border-bottom: solid 3px green; }\n.k-widget.k-notification.k-notification-info    > .notification  { color: [model[notifications.info.text]]; background: [model[notifications.info.background]]; border-width: [model[border.width]]; border-radius: [model[border.radius]]; border: [model[border.style]]; border-bottom: solid 3px blue; }\n.k-widget.k-notification.k-notification-warning > .notification  { color: [model[notifications.warning.text]]; background: [model[notifications.warning.background]]; border-width: [model[border.width]]; border-radius: [model[border.radius]]; border: [model[border.style]]; border-bottom: solid 3px yellow; }\n.k-widget.k-notification.k-notification-error   > .notification  { color: [model[notifications.error.text]]; background: [model[notifications.error.background]]; border-width: [model[border.width]]; border-radius: [model[border.radius]]; border: [model[border.style]]; border-bottom: solid 3px red; }\n.k-numeric-wrap { overflow: hidden; }\n\n/* Component styles */\n.content { margin: 0; padding: 0; max-height: 100%; }\n.component           { \n    vertical-align: top; box-sizing: border-box; display:inline-block; overflow: hidden;\n    margin: [model[margins]]; width: 100%; height: 100%; max-width: calc(100% - (([model[border.width]] + [model[margins]])*2)); max-height: calc(100% - (([model[border.width]] + [model[margins]])*2)); padding: 0; background: [model[colours.background]]; \n    border-width: [model[border.width]]; border-radius: [model[border.radius]]; border: [model[border.style]];  box-shadow: [model[shadows]]; -moz-box-shadow: [model[shadows]]; -webkit-box-shadow: [model[shadows]];  \n}  \n.k-window .component          { border: none; box-shadow: none; }\n.k-tabstrip > .tab > section.component { padding: 0; margin: 0; width: 100%; border: none; border-radius: 0; box-shadow: none; }\n  \n.component[name=DetailedNav] .menu { border: none; box-shadow: none; background:  transparent; }\n.component[name=DetailedNav] .menu > li    {  background: [model[colours.background]]; border: [model[border.style]] [model[border.width]] [model[border.radius]]; margin: 10px; box-shadow: [model[shadows]]; -moz-box-shadow: [model[shadows]]; -webkit-box-shadow: [model[shadows]]; }\n.component[name=DetailedNav] .menu > li:hover { border: [model[border.style]] [model[border.width]] [model[border.radius]]; box-shadow: [model[shadows]]; -moz-box-shadow: [model[shadows]]; -webkit-box-shadow: [model[shadows]];}\n.component[name=DetailedNav] .menu > li a { background: [model[colours.primary]]; color: [model[colours.text2]]; } \n   \n/* validation helpers */\ninput.error,\ntextarea.error          { background: white; border: dashed 1px [model[notifications.error.text]]; }\ninput.error:hover,\ntextarea.error:hover    { background: [model[notifications.error.background]]; }\nlabel.error             { color: [model[notifications.error.text]]; }\n\n/* sprite based icon attachements */\n.k-sprite           { background-image: url(\"[api[root]]DMS/Content/icons.png\"); margin-top: 0; }\n.root               { background-position: 0 0; }\n.folder             { background-position: 0 -16px; }\n.pdf                { background-position: 0 -32px; }\n.page               { background-position: 0 -48px; }\n.image              { background-position: 0 -64px; }\n.question           { background-position: 0 -80px; }\n.add                { background-position: 0 -96px; }\n.query              { background-position: 0 -112px; }\ntr a\t\t\t{ margin-right: 20px;}\n   \n/* forms */\n.fieldList > li { padding: 2px; margin: 1px; }\n.fieldList > li > .value > .k-invalid           { color: [model[colours.error]]; border: dashed 1px [model[colours.error]]; }\n.fieldList > li > .value > .k-widget.k-tooltip.k-tooltip-validation       { margin: 5px 0; background: white; color: [model[colours.error]]; }\n.fieldList > li > .value > .k-widget.k-tooltip.k-tooltip-validation.k-invalid-msg  { color: [model[colours.error]]; }\n\n/* Menus */\nbody > header > nav \t\t\t\t\t\t\t\t\t\t\t      { border: none; box-shadow: none; background: transparent; margin: 0; margin-top: -33px; min-height: 33px; font-weight: bold;  }\nbody > header > nav .menu                                        { margin-left: 550px; }\nbody > header > nav .menu li                                     { border-radius: [model[border.radius]] [model[border.radius]] 0 0; background-color: [model[colours.secondary]]; color: [model[colours.text2]];  }\nbody > header > nav .menu li > a                         \t   { background-color: [model[colours.secondary]]; color: [model[colours.text2]]; }\nbody > header > nav .menu li:hover,\nbody > header > nav .menu li:hover > a                     { background: [model[colours.primary]]; }\nbody > header > nav .menu li.selected                \t     { background: [model[colours.primary]]; border-bottom: [model[border.style]] [model[border.width]] [model[border.radius]]; box-shadow:none;  }\nbody > header > nav .menu .sep                       \t\t   { color: [model[colours.background]]; background: [model[colours.background]]; }\nbody > header > nav .menu li .submenu                     { box-shadow: [model[shadows]]; -moz-box-shadow: [model[shadows]]; -webkit-box-shadow: [model[shadows]]; border-radius: 0 0 [model[border.radius]] [model[border.radius]]; overflow: hidden; }\nbody > header > nav .menu li:hover .submenu li       { background: [model[colours.secondary]]; border-radius: 0; }\nbody > header > nav .menu li:hover .submenu li:hover,\nbody > header > nav .menu li:hover .submenu li:hover > a { background: [model[colours.primary]]; }\n   \n.component[name=DetailedNav] .menu { border: none; box-shadow: none; background:  transparent; }\n.component[name=DetailedNav] .menu > li    { font-weight: bold; background: [model[colours.background]]; border: [model[border.style]] [model[border.width]] [model[border.radius]]; margin: 10px; box-shadow: [model[shadows]]; -moz-box-shadow: [model[shadows]]; -webkit-box-shadow: [model[shadows]]; }\n.component[name=DetailedNav] .menu > li.selected { color: [model[colours.secondary]]; }\n.component[name=DetailedNav] .menu > li:hover { border: [model[border.style]] [model[border.width]] [model[border.radius]]; box-shadow: [model[shadows]]; -moz-box-shadow: [model[shadows]]; -webkit-box-shadow: [model[shadows]]; }\n.component[name=DetailedNav] .menu > li a { background: [model[colours.primary]]; color: [model[colours.text2]]; }\n\n/* Tooltips */\n.tooltip > .tooltiptext             { display: none; background-color: white; color: [model[colours.text]]; border-width: [model[border.width]]; border-radius: [model[border.radius]]; border: [model[border.style]]; }\n\n/* FlexRow */\n.row                            { display: inline-block; display: flex; flex-wrap: wrap; -webkit-flex-direction: row; flex-direction: row; overflow-x: hidden; }\n.row *                          { box-sizing: border-box; }\n.row>h2                         { width: 100%; }\n.row.header                     { padding: 0; border-top: #e0e0e0 1px solid; }\n.row>.col                       { align-content: flex-start; flex: 20%; }\n.row>.col-sm                    { flex: 0 25%; min-width: 25%; max-width: 25%;}\n.row>.col-half                  { flex: 0 50%; min-width: 50%; max-width: 50%; }\n.row>.col-lg \t\t\t\t\t{ flex: 0 75%; min-width: 75%; max-width: 75%; }\n.row>.col-full                  { flex: 1 100%; }\n   \n/* Dialogs */\n.k-window\t\t\t\t\t            { overflow: hidden; }\n.k-window .k-window-content  { \n    padding: 0; \n    background: white;  border-width: [model[border.width]]; border-radius: [model[border.radius]]; border: [model[border.style]]; \n    box-shadow: [model[shadows]]; -moz-box-shadow: [model[shadows]]; -webkit-box-shadow: [model[shadows]]; \n}\n   \n.k-window .k-window-titlebar { color: [model[colours.text2]]; background: [model[colours.secondary]]; }\n.k-window .k-window-content.component   { margin: 0; border: none; -moz-box-shadow: none; -webkit-box-shadow: none; box-shadow: none; }\n.k-window > .component h3               { display: none; }\n.k-window > .k-window-titlebar.k-header { color: [model[colours.text2]], background: [model[colours.primary]]; border: none; }\n.k-window > .k-window-titlebar.k-header > .k-window-title { font-size: 100%; color: [model[colours.text2]];  background-position: 0 -16px; height: 17px; }\n.k-widget.k-window > .k-window-titlebar.k-header > .k-window-actions { margin: 4px; margin-right: 0; }\n.k-widget.k-window > .k-window-titlebar.k-header { color: [model[colours.text2]]; }\n.k-window-title { color:  [model[colours.text2]]; }\n.dialog { overflow: hidden; }\n.dialog > p { margin: 10px; }\n.dialog > .value > button { float: right; margin: 10px; margin-left: 0; }\n\n/* Charts */\n .chart { width: 629px; hieght: 370px; display: inline-block; background: transparent; }\n   .k-chart,  .k-chart-area { background: transparent; }\n</style>",
  "LastUpdated": "2022-03-18T10:41:54.1913804+00:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Template",
                Data = """
{
  "Name": "Theme-Default-Old",
  "ResourceKey": "",
  "RawString": "<style name=\"Theme-Default\">\r\n\r\n* { font-size: [theme[font.size]]; font-family: [theme[font.family]]; }\r\n\r\na           { color: [theme[colours.links]]; cursor: pointer; }\r\nhr          { border-top: [theme[border.style]]; }\r\n\r\nhtml {\r\n    overflow-y: auto !important;\r\n}\r\n\r\n/* Headings */\r\nh1 > a            { font-family: [theme[font.family]]; color: [theme[colours.primary]]; cursor: pointer; }\r\nh2                  { font-family: [theme[font.family]];color: [theme[colours.text2]]; background-color: [theme[colours.primary]]; border-top: [theme[borders]]; background: [theme[colours.primary]]; }\r\nh2 *               { font-family: [theme[font.family]]; color: [theme[colours.text2]]; background-color: [theme[colours.primary]];  }\r\nh3                  { padding: 6px 12px;; font-size: 110%; font-family: [theme[font.family]]; background: [theme[colours.secondary]]; color: [theme[colours.text2]]; }\r\nh3 > span       { font-size: 110%; font-family: [theme[font.family]]; color: [theme[colours.text2]]; }\r\nh4                  { padding: 5px 12px; font-size: 100%; font-family: [theme[font.family]]; background: [theme[colours.secondary]]; color: [theme[colours.text2]]; }\r\nh4 > span       { padding: 5px 12px; font-size: 100%; font-family: [theme[font.family]]; color: [theme[colours.text2]]; }\r\n\r\n/* Forms related tags */\r\nfieldset            { border: none; }\r\npre                 { border-width: [theme[border.width]]; border-radius: [theme[border.radius]]; border: [theme[border.style]]; }\r\ninput               { padding: 6px 12px; border-width: [theme[border.width]]; border: [theme[border.style]]; }\r\nselect              { background: white; border-width: [theme[border.width]]; border-radius: [theme[border.radius]]; border: [theme[border.style]]; }\r\ntextarea          { background: white; border-width: [theme[border.width]]; border-radius: [theme[border.radius]]; border: [theme[border.style]]; }\r\ntextarea:focus { \r\n    box-shadow: [theme[shadows]]; -moz-box-shadow: [theme[shadows]]; -webkit-box-shadow: [theme[shadows]]; \r\n    border-width: [theme[border.width]]; border-radius: [theme[border.radius]]; border: [theme[border.style]]; \r\n}\r\nselect:hover, \r\ntextarea:hover          { background-color: white; color: #000; }\r\nbutton { \r\n    background-color: [theme[colours.secondary]]; color: [theme[colours.text2]];\r\n    box-shadow: [theme[shadows]]; -moz-box-shadow: [theme[shadows]]; -webkit-box-shadow: [theme[shadows]]; \r\n    border-width: [theme[border.width]]; border-radius: [theme[border.radius]]; border: [theme[border.style]]; \r\n    padding: 2px 16px;\r\n}\r\nbutton:hover { background-color: [theme[colours.primary]]; color:white; cursor: pointer; }\r\nbutton:hover > span { color: white}; \r\nbutton:active {  box-shadow: none; -moz-box-shadow: none; -webkit-box-shadow: none;  }\r\n\r\n/* layout */\r\nbody { background-repeat: no-repeat;  background-position: bottom right; background-origin: content-box; background-size: 500px; }\r\nbody > header                          { }\r\nbody > header > .details           { min-width: 500px;  }\r\nbody > header > .details > img { max-height: 90px; margin-top: -10px; }\r\nbody > header > .details > ul    { width: 200px; float: right; }\r\nbody > header > .details > ul > li { padding:3px; }\r\nbody > footer                            { margin: 0; background: [theme[colours.primary]]; border-top-width: [theme[border.width]]; border-top: [theme[border.style]]; box-shadow: [theme[shadows]]; -moz-box-shadow: [theme[shadows]]; -webkit-box-shadow: [theme[shadows]]; position: fixed; bottom: 0; width: 100%; }\r\nbody > footer .component         {  background: [theme[colours.primary]]; }\r\nbody > footer .component label { color: #FFFFFF; }\r\nbody > footer p \t\t                 { color: #FFFFFF; }\r\nbody > .content[name=body]    {display: flex; flex-wrap: wrap; justify-content: flex-start; align-items: flex-start; align-content: flex-start; gap: 0; flex-direction: row;}\t\r\nbody > .content[name=body] .small-container { display: flex; flex-direction: row; flex-wrap: wrap; padding: 0; margin: 0; margin-right: -10; margin-bottom: -10; }\t\r\nbody > .content[name=body] .component > .chart { height: 100%; display: flex; flex-direction: column; }\t\r\nbody > .content[name=body] .component.small {  height: 170px;  width: 275px; }\t\r\nbody > .content[name=body] .small-container { height: 350px; width: 570px; display: flex; flex-direction: row; flex-wrap: wrap; padding: 0; margin: 0; margin-right: -10; margin-bottom: -10; }\t\r\nbody > .content[name=body] .component.dashboardComponent { height: 350px; width: 560px; }\t\r\nbody > .content[name=body] .component { height: 350px; width: 560px; }\t\r\nbody > .content[name=body] .component.large {  display: flex; flex: 1; flex-direction: column; min-height: calc(100% - 36px - (([theme[border.width]] + [theme[margins]]) * 2)); max-height: calc(100% - 36px - (([theme[border.width]] + [theme[margins]]) * 2));width: calc(100% - (([theme[border.width]] + [theme[margins]]) * 2)); }\r\nbody > .content[name=body] .component.details {  display: flex ; flex: 1 ; flex-direction: row !important; min-height: calc(100% - 36px - (([theme[border.width]] + [theme[margins]]) * 2)) ; max-height: calc(100% - (([theme[border.width]] + [theme[margins]]) * 2)) ; min-width: calc(100% - (([theme[border.width]] + [theme[margins]]) * 2)) !important; overflow: auto; }\r\nbody > .content[name=body] .component  .component { display: flex; flex: 1; flex-direction: column; width: 100%; height: 100%; }\r\n.clock { text-align: right; }\r\n.companyLogo2 { height: 70px; margin-top: 2px; margin-right: 1vw; }\r\nmarquee { font-family: [theme[font.family]]; color: [theme[colours.primary]]; padding: 10px; font-size: 180%; border-width: [theme[border.width]]; border: [theme[border.style]];  box-shadow: [theme[shadows]]; -moz-box-shadow: [theme[shadows]]; -webkit-box-shadow: [theme[shadows]]; }\r\n.header-details { justify-content: space-around; padding-right: 5px; }\r\n.backgroundAnimation { background: url('[api[root]]DMS/content/background.gif'); background-size: 100%; background-repeat: no-repeat; position: fixed; bottom: -85px; right: 50px; height: 200px; width: 400px; z-index: -1;} \r\n/* kendo hacks */\r\nspan.k-icon                                       { margin-top: -2px; margin-right: 5px; }\r\ninput[type=text]:hover                       { background: white; }\r\ninput[type=text]:focus                       { border-width: [theme[border.width]]; border-radius: [theme[border.radius]]; border: [theme[border.style]]; }\r\ninput[type=password]:hover              { background: white; }\r\ninput[type=password]:focus              { border-width: [theme[border.width]]; border-radius: [theme[border.radius]]; border: [theme[border.style]]; }\r\ninput[type=email]:hover                    { background-color: white; }\r\ninput[type=email]:focus                    { border-width: [theme[border.width]]; border-radius: [theme[border.radius]]; border: [theme[border.style]]; }\r\ninput[data-role=datepicker],\r\ninput[data-role=numerictextbox]       { border-width: [theme[border.width]]; border-radius: [theme[border.radius]]; border: [theme[border.style]]; }\r\ninput[data-role=datepicker]:focus,\r\ninput[data-role=numerictextbox]:focus  { border: none; }\r\n.k-toolbar > button {display: initial; }\r\n.k-tooltip.k-tooltip-closable.k-chart-tooltip > .k-tooltip-content {color: white !important; }\r\n.k-grid-header .k-grid-filter.k-active, .k-grid-header .k-header-column-menu.k-active, .k-grid-header .k-grid-header-menu.k-active, .k-grid-header .k-hierarchy-cell .k-icon.k-active, .k-grid-header .k-hierarchy-cell .k-svg-icon.k-active { background-color: #F8C785 !important; } \r\n.k-pager-sizes > .k-picker > .k-button-solid-base:hover { background-color: #EBEBEB !important; }\r\n.k-pager-numbers >  .k-selected { font-weight: bold; }\r\n.k-sorted > .k-cell-inner > .k-link > .k-column-title { color: white; }\r\n.k-pager-numbers > .k-button-flat-primary { color: [theme[colours.primary]] !important; }\r\n.k-pager-numbers >  .k-button-flat-primary:hover { font-weight: bold;  color: [theme[colours.primary]]}\r\n.k-pager-nav:hover > .k-icon {color: [theme[colours.primary]]}\r\n.k-pager-refresh:hover > .k-icon {color: [theme[colours.primary]]}\r\n\r\n/* grids */\r\n.k-grid { padding: 0; margin: 0;  max-height: 100%; }\r\n.k-grid a { color: [theme[colours.links]]; cursor: pointer; }\r\n.k-grid th.k-header > a.k-link { font-weight: bold; }\r\ntr > th.k-table-th.k-header {padding: 5px 5px;}\r\n.k-grid th.k-header.k-sorted { color:[theme[colours.text2]]; background-color: [theme[colours.secondary]] !important; }\r\n.k-grid th.k-header.k-sorted > * { color:[theme[colours.text2]]; }\r\n.k-grid td[role=gridcell] { padding: 3px 5px; color: [theme[colours.text]]; }\r\n.k-header.k-grid-toolbar > button { margin: 3px; }\r\n.k-grid.k-grid-display-block {display: flex !important;}\r\n.content > .component > .k-grid > .k-grid-content.k-auto-scrollable, .k-grid-footer-wrap.k-auto-scrollable, .k-grid-header-wrap.k-auto-scrollable { display:flex !important; flex: 1 1 0; flex-direction:column; min-width: 0; }\r\n.k-splitter {display: flex; flex: 1; flex-direction: column; }\r\n:not(td) > .k-tabstrip-wrapper > .k-tabstrip > .k-content > .component > .k-grid > .k-grid-content { flex: 1 0 0 !important; } \r\nspan.k-datepicker > button.k-input-button:hover {background-color: [theme[colours.primary]]; color: [theme[colours.text2]];}\r\n\r\n.k-button-solid-primary, .k-button-solid-base  { background-color: [theme[colours.secondary]]; color: [theme[colours.text2]]; border: 0px; }\r\n.k-button-solid-primary:hover, .k-button-solid-base:hover  { background-color: [theme[colours.primary]]; color: [theme[colours.text2]]; border: 0px; }\r\n.k-button-text { color: white; }\r\n\r\n.k-filter-menu >  .k-filter-menu-container > .k-action-buttons > .k-button { height: 40px; margin: 0; color: [theme[colours.links]]; }\r\n.k-filter-menu >  .k-filter-menu-container > .k-action-buttons > .k-button:hover { background: [theme[colours.secondary]]; color: [theme[colours.text2]]; }\r\n\r\n/*Fix weird width issue on grid filters*/\r\n.k-filter-menu.k-popup .k-filter-menu-container, .k-grid-filter-popup.k-popup .k-filter-menu-container { width: unset; }\r\n.k-filter-menu.k-popup {width: fit-content; }\r\n.k-calendar-view {width: unset; inline-size: unset; }\r\nk-calendar-view.k-calendar-monthview {width: fit-content !important;}\r\ndiv.k-calendar-container > div[data-role=calendar] > div.k-header > a > span {color: [theme[colours.primary]]}\r\ndiv.k-calendar > div.k-footer > button > span {color: [theme[colours.primary]]}\r\n\r\n.k-list .k-item.k-state-selected, .k-list-optionlabel.k-state-selected { background-color: [theme[colours.secondary]]; }\r\n.k-list .k-item.k-state-hover.k-state-selected, .k-list .k-item:hover.k-state-selected, .k-list-optionlabel.k-state-hover.k-state-selected, .k-list-optionlabel:hover.k-state-selected { background-color: [theme[colours.primary]]; }\r\n\r\n/* Notifications */\r\n.notification                   { padding: 3px 10px; min-width: 200px; max-width: 400px; max-height: 200px; word-wrap: normal; word-break: keep-all; }\r\n\r\n.k-notification-success {color: [theme[notifications.success.text]]; background: [theme[notifications.success.background]]; border-width: [theme[border.width]]; border-radius: [theme[border.radius]]; border: [theme[border.style]]; border-bottom: solid 3px green; }\r\n.k-notification-warning { color: [theme[notifications.warning.text]]; background: [theme[notifications.warning.background]]; border-width: [theme[border.width]]; border-radius: [theme[border.radius]]; border: [theme[border.style]]; border-bottom: solid 3px yellow;}\r\n.k-notification-error { color: [theme[notifications.error.text]]; background: [theme[notifications.error.background]]; border-width: [theme[border.width]]; border-radius: [theme[border.radius]]; border: [theme[border.style]]; border-bottom: solid 3px red; }\r\n.k-notification-info {color: [theme[notifications.info.text]]; background: [theme[notifications.info.background]]; border-width: [theme[border.width]]; border-radius: [theme[border.radius]]; border: [theme[border.style]]; border-bottom: solid 3px blue; }\r\n\r\n.k-widget.k-notification { border: none; background: transparent; box-shadow: none; }\r\n.k-widget.k-notification > .notification { color: #222; margin-bottom: 10px; border-radius: 5px; }\r\n.k-numeric-wrap { overflow: hidden; }\r\n\r\n/*Kendo Icons*/\r\n.k-icon { width: auto !important; color: [theme[colours.primary]]}\r\n\r\n/* Component styles */\t\r\n.content { margin: 0; padding: 0; max-height: 100%; display:flex; flex-wrap:wrap; gap: 0; justify-content: flex-start; align-items: start; }\t\r\n.component { \t\r\n    flex-shrink: 0; display:flex; flex-direction: column; max-height: 100%; max-width: 100%; \t\r\n    margin: [theme[margins]]; padding: 0; background: [theme[colours.background]]; \t\r\n    border-width: [theme[border.width]]; border-radius: [theme[border.radius]]; border: [theme[border.style]];  box-shadow: [theme[shadows]]; -moz-box-shadow: [theme[shadows]]; -webkit-box-shadow: [theme[shadows]];  \t\r\n}  \t\r\n.k-window .component { border: none; box-shadow: none; }\t\r\n.tab > .component { padding: 0; margin: 0px;  border: none; box-shadow: none; border-radius: 0px; display:flex; flex:auto;}\r\n\r\n.component[name=DetailedNav] .menu { border: none; box-shadow: none; background:  transparent; }\r\n.component[name=DetailedNav] .menu > li    {  background: [theme[colours.background]]; border: [theme[border.style]] [theme[border.width]] [theme[border.radius]]; margin: 10px; box-shadow: [theme[shadows]]; -moz-box-shadow: [theme[shadows]]; -webkit-box-shadow: [theme[shadows]]; }\r\n.component[name=DetailedNav] .menu > li:hover { border: [theme[border.style]] [theme[border.width]] [theme[border.radius]]; box-shadow: [theme[shadows]]; -moz-box-shadow: [theme[shadows]]; -webkit-box-shadow: [theme[shadows]];}\r\n.component[name=DetailedNav] .menu > li a { background: [theme[colours.primary]]; color: [theme[colours.text2]]; } \r\n   \r\n/* validation helpers */\r\ninput.error,\r\ntextarea.error          { background: white; border: dashed 1px [theme[notifications.error.text]]; }\r\ninput.error:hover,\r\ntextarea.error:hover    { background: [theme[notifications.error.background]]; }\r\nlabel.error             { color: [theme[notifications.error.text]]; }\r\n\r\n/* sprite based icon attachements */\r\n.k-sprite           { background-image: url(\"[app[root]]Api/DMS/Content/icons.png\"); margin-top: 0; }\r\n.root               { background-position: 0 0; }\r\n.folder             { background-position: 0 -16px; }\r\n.pdf                { background-position: 0 -32px; }\r\n.page               { background-position: 0 -48px; }\r\n.image              { background-position: 0 -64px; }\r\n.question           { background-position: 0 -80px; }\r\n.add                { background-position: 0 -96px; }\r\n.query              { background-position: 0 -112px; }\r\ntr > td[role=gridcell] > a\t\t\t{ margin-right: 20px;}\r\n   \r\n/* forms */\r\n.fieldList > li { padding: 2px; }\r\n.fieldList > li > .value > .k-invalid           { color: [theme[colours.error]]; border: dashed 1px [theme[colours.error]]; }\r\n.fieldList > li > .value > .k-widget.k-tooltip.k-tooltip-validation       { margin: 5px 0; background: white; color: [theme[colours.error]]; }\r\n.fieldList > li > .value > .k-widget.k-tooltip.k-tooltip-validation.k-invalid-msg  { color: [theme[colours.error]]; }\r\n\r\n/* Menus */\r\n.navbar-collapse { border: none; box-shadow: none; background: transparent; margin: -3px; min-height: 29px; font-weight: bold; clear: both; align-self: flex-end;}\r\n.navbar-collapse .navbar-nav li { border-radius: [theme[border.radius]] [theme[border.radius]] 0 0; background-color: [theme[colours.secondary]]; color: [theme[colours.text2]]; white-space:nowrap; position: relative; display: inline; padding: 14px; margin-right: 1px; }\r\n.navbar-collapse .navbar-nav li > a { color: [theme[colours.text2]]; }\r\n.navbar-collapse .navbar-nav li:hover { background: [theme[colours.primary]]; }\r\n.navbar-collapse .navbar-nav li:hover > a { background: [theme[colours.primary]]; }\r\n.navbar-collapse .navbar-nav li.selected { background: [theme[colours.primary]]; border-bottom: [theme[border.style]] [theme[border.width]] [theme[border.radius]]; box-shadow:none;  }\r\n.navbar-collapse .navbar-nav .sep { color: [theme[colours.background]]; background: [theme[colours.background]]; }\r\n.navbar-collapse .navbar-nav li .submenu { box-shadow: [theme[shadows]]; -moz-box-shadow: [theme[shadows]]; -webkit-box-shadow: [theme[shadows]]; border-radius: 0 0 [theme[border.radius]] [theme[border.radius]]; overflow: hidden; position: absolute; left: 0; top: 40px; display: none;}\r\n.navbar-collapse .navbar-nav li:hover .submenu li { background: [theme[colours.secondary]]; border-radius: 0; }\r\n.navbar-collapse .navbar-nav li:hover .submenu li:hover, nav .menu li:hover .submenu li:hover > a { background: [theme[colours.primary]]; }\r\n\r\nheader .navbar-collapse .navbar-nav li:hover .submenu { display: block; }\r\nheader .navbar-collapse .navbar-nav li:hover .submenu li { min-width: 200px; display: block; }\r\n   \r\n.component[name=DetailedNav] .menu { border: none; box-shadow: none; background:  transparent; }\r\n.component[name=DetailedNav] .menu > li    { font-weight: bold; background: [theme[colours.background]]; border: [theme[border.style]] [theme[border.width]] [theme[border.radius]]; margin: 10px; box-shadow: [theme[shadows]]; -moz-box-shadow: [theme[shadows]]; -webkit-box-shadow: [theme[shadows]]; }\r\n.component[name=DetailedNav] .menu > li.selected { color: [theme[colours.secondary]]; }\r\n.component[name=DetailedNav] .menu > li:hover { border: [theme[border.style]] [theme[border.width]] [theme[border.radius]]; box-shadow: [theme[shadows]]; -moz-box-shadow: [theme[shadows]]; -webkit-box-shadow: [theme[shadows]]; }\r\n.component[name=DetailedNav] .menu > li a { background: [theme[colours.primary]]; color: [theme[colours.text2]]; }\r\n\r\n/* Tooltips */\r\n.tooltip > .tooltiptext             { display: none; background-color: white; color: [theme[colours.text]]; border-width: [theme[border.width]]; border-radius: [theme[border.radius]]; border: [theme[border.style]]; }\r\n\r\n/* FlexRow */\r\n.row                            { display: inline-block; display: flex; flex-wrap: wrap; -webkit-flex-direction: row; flex-direction: row; overflow-x: hidden; }\r\n.row *                          { box-sizing: border-box; }\r\n.row>h2                         { width: 100%; }\r\n.row.header                     { padding: 0; border-top: #e0e0e0 1px solid; }\r\n.row>.col                       { align-content: flex-start; flex: 20%; }\r\n.row>.col-sm                    { flex: 0 25%; min-width: 25%; max-width: 25%;}\r\n.row>.col-half                  { flex: 0 50%; min-width: 50%; max-width: 50%; }\r\n.row>.col-lg \t\t\t\t\t{ flex: 0 75%; min-width: 75%; max-width: 75%; }\r\n.row>.col-full                  { flex: 1 100%; }\r\n   \r\n/* Dialogs */\r\n.k-window\t\t\t\t\t            { overflow: hidden; }\r\n.k-window .k-window-content  { \r\n    display: flex; flex: 1; flex-direction: column;\r\n    padding: 0; \r\n    background: white;  border-width: [theme[border.width]]; border-radius: [theme[border.radius]]; border: [theme[border.style]]; \r\n    box-shadow: [theme[shadows]]; -moz-box-shadow: [theme[shadows]]; -webkit-box-shadow: [theme[shadows]]; \r\n}\r\n   \r\n.k-window .k-window-titlebar { color: [theme[colours.text2]]; background: [theme[colours.secondary]]; }\r\n.k-window .k-window-content.component   { margin: 0; border: none; -moz-box-shadow: none; -webkit-box-shadow: none; box-shadow: none; }\r\n.k-window > .component h3               { display: none; }\r\n.k-window > .k-window-titlebar.k-header { color: [theme[colours.text2]], background: [theme[colours.primary]]; border: none; }\r\n.k-window > .k-window-titlebar.k-header > .k-window-title { font-size: 100%; color: [theme[colours.text2]];  background-position: 0 -16px; height: 17px; }\r\n.k-widget.k-window > .k-window-titlebar.k-header > .k-window-actions { margin: 4px; margin-right: 0; }\r\n.k-widget.k-window > .k-window-titlebar.k-header { color: [theme[colours.text2]]; }\r\n.k-window-title { color:  [theme[colours.text2]]; }\r\n.dialog { overflow: hidden; }\r\n.dialog > p { margin: 8px; }\r\n.dialog > .value > button { float: right; margin: 10px; margin-left: 0; }\r\n\r\n/* Charts */\r\n .chart { max-width: 550px; max-height: 340px; background: transparent; }\r\n   .k-chart,  .k-chart-area { background: transparent; }\r\n\r\n/*Tabs*/\t\r\n    .k-tabstrip {flex: 1; width: 100%; }\t\r\n    .k-tabstrip-wrapper {display: flex; flex:auto; flex-direction: column;}\t\r\n    .tab { flex:auto; flex-direction: column;}\t\r\n    .tab .component { height: 100%; width: 100%; }\r\n    .k-tabstrip>.k-content.k-state-active {display: flex !important;}\r\n    .k-tabstrip>.k-content {padding:0px !important;}\r\n    .k-tabstrip-items-wrapper .k-item { color: [theme[colours.primary]]}\r\n    .k-tabstrip-items-wrapper .k-item.k-hover {color: [theme[colours.secondary]]}\r\n    tab k-tabstrip-content k-content k-active { height: initial !important; min-height: initial !important; }\r\n    [data-role=tabstrip] [role=tab].k-active,\r\n    [data-role=tabstrip] [role=tab].k-active > span { background: [theme[colours.primary]]; color: white; border-bottom: none; }\r\n    [data-role=tabstrip] [role=tab].k-active > span >span  {  color: white; }\r\n    .tab { padding: 0px; }\r\n\r\n    /*Checkboxes*/\r\n    input[type=\"checkbox\"] {min-width: 15px; min-height: 15px; }\r\n    .k-checkbox:checked, .k-checkbox.k-checked { background-color: [theme[colours.secondary]]; border-color: [theme[colours.secondary]]; }\r\n    .k-checkbox:checked:focus, .k-checkbox.k-checked { box-shadow: 0 0 0 2px rgba(237, 137, 0, 0.5)}\r\n    .k-check-all-wrap { padding-inline: 0px !important; }\r\n    .k-multicheck-wrap > .k-item {padding-inline:0px !important;}\r\n    .k-multicheck-wrap > li {padding-bottom: 4px; }\r\n    .k-multicheck-wrap > li > input {margin-right: 4px; margin-left: 3px; }\r\n    .k-grid td.k-selected, .k-grid .k-table-row.k-selected>td, .k-grid .k-table-td.k-selected, .k-grid .k-table-row.k-selected>.k-table-td {background-color: rgba(238,138,0,0.3); }\r\n    \r\n    /*Tree Views*/\r\n    .k-treeview-leaf.k-selected {background-color: [theme[colours.primary]]; color: white; }\r\n   .k-treeview-leaf.k-selected> .k-treeview-leaf-text { color: white; }\r\n\r\n    /*Dropdowns*/\r\n   .k-list-item.k-selected, .k-selected.k-list-optionlabel {background-color: [theme[colours.secondary]];}\r\n    .k-list-item.k-selected:hover, .k-selected.k-list-optionlabel:hover {background-color: [theme[colours.secondary]];}\r\n    .k-list-item.k-selected > .k-list-item-text {color: white !important; }\r\n    .k-input-button:hover {background-color: #E9E9E9; }\r\n\r\n    /*ThemeBuilder*/\r\n    div.k-coloreditor-header.k-hstack > div> div > button:hover > span {color: [theme[colours.primary]]; background-color: #F2F2F2}\r\n    div.k-coloreditor-views.k-vstack > div > div.k-colorgradient-inputs.k-hstack > div > button:hover > span {color: [theme[colours.primary]]; background-color: #F2F2F2 }\r\n    .k-colorpicker > button:hover > span {color: [theme[colours.primary]] }\r\n</style>",
  "LastUpdated": "2024-06-07T11:12:33.5479762+01:00"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Template",
                Data = """
{
  "Name": "Test",
  "ResourceKey": "Test",
  "RawString": "<html>\r\n<head>\r\n    <title>User Activity Report for [model[start]] to [model[end]]</title>\r\n    <style>\r\n        body { padding: 0 20px; font-size: [theme[font.size]]; font-family: [theme[font.family]]; }\r\n\r\n        h1 {\r\n            width: 80%;\r\n            border-bottom: solid 2px [theme[colours.primary]];\r\n\t\t    color: [theme[colours.primary]];\r\n        }\r\n\r\n        h2 {\r\n            width: 80%;\r\n            border-bottom: solid 2px [theme[colours.secondary]];\r\n\t\t    color: [theme[colours.secondary]];\r\n        }\r\n\r\n        table { \t\t\r\n            box-shadow: [theme[shadows]];\r\n            -moz-box-shadow: [theme[shadows]];\r\n            -webkit-box-shadow: [theme[shadows]];\r\n            border-width: [theme[border.width]];\r\n            border-radius: [theme[border.radius]];\r\n            border: [theme[border.style]];\r\n            margin-bottom: 20px; \r\n        }\r\n\r\n        thead { background: #EFEFEF; }\r\n        tr { border: [theme[border.style]]; }\r\n        th { text-align: left; padding: 2px 6px; }\r\n        td { padding: 4px; }\r\n    </style>\r\n</head>\r\n<body>\r\n    <h1 style=\"color: [theme[colours.primary]];\">User Activity Report for [model[fromYear]]-[model[fromMonth]]-01 to [model[toYear]]-[model[toMonth]]-01</h1>\r\n    <h2 style=\"color: [theme[colours.primary]];\">Users</h2>\r\n    <table>\r\n        <thead>\r\n            <tr>\r\n                <th style=\"width: 150px;\">Id</th><th style=\"width: 200px;\">Name</th><th style=\"width: 300px;\">Email</th>\r\n            </tr>\r\n        </thead>\r\n        <tbody>\r\n            [execute]\r\n                string result = string.Empty;\r\n\r\n                foreach(var user in model.Data.Users)\r\n                    result += $\"<tr><td>{user.Id}</td><td>{user.DisplayName}</td><td>{user.Email}</td></tr>\";\r\n\r\n                return result;\r\n            [/execute]\r\n        </tbody>\r\n    </table>\r\n\r\n    <h2 style=\"color: [theme[colours.primary]];\">Sessions</h2>\r\n    <table>\r\n        <thead>\r\n            <tr>\r\n                <th>Date</th><th>Name</th><th>Email</th><th>Count</th><th>Page Hits</th><th>Api Calls</th>\r\n            </tr>\r\n        </thead>\r\n        <tbody>\r\n            [execute]\r\n                string result = string.Empty;\r\n\r\n                foreach(var session in model.Data.Sessions)\r\n                    result += $\"<tr><td>{session.Date}</td><td>{session.DisplayName}</td><td>{session.Email}</td><td>{session.SessionCount}</td><td>{session.PageHits}</td><td>{session.ApiCalls}</td></tr>\";\r\n\r\n                return result;\r\n            [/execute]\r\n        </tbody>\r\n    </table>\r\n</body>\r\n</html>",
  "LastUpdated": "2024-11-20T18:35:49.5446071+00:00"
}
"""
            },
        ]
    };

    static Package Scripts => new()
    {
        Name = "Content Management Scripts",
        Category = "CMS",
        Description = "Content Management Scripts.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/Script",
                Data = """
{
  "Name": "DefaultResourcing",
  "Content": "api.addToResourceCache([\r\n\t{Key: \"Default\", Name: \"month-1\", ShortDisplayName: \"[resource_shortdisplayname[month-1]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"month-2\", ShortDisplayName: \"[resource_shortdisplayname[month-2]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"month-3\", ShortDisplayName: \"[resource_shortdisplayname[month-3]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"month-4\", ShortDisplayName: \"[resource_shortdisplayname[month-4]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"month-5\", ShortDisplayName: \"[resource_shortdisplayname[month-5]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"month-6\", ShortDisplayName: \"[resource_shortdisplayname[month-6]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"month-7\", ShortDisplayName: \"[resource_shortdisplayname[month-7]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"month-8\", ShortDisplayName: \"[resource_shortdisplayname[month-8]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"month-9\", ShortDisplayName: \"[resource_shortdisplayname[month-9]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"month-10\", ShortDisplayName: \"[resource_shortdisplayname[month-10]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"month-11\", ShortDisplayName: \"[resource_shortdisplayname[month-11]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"month-12\", ShortDisplayName: \"[resource_shortdisplayname[month-12]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"export\", ShortDisplayName: \"[resource_shortdisplayname[export]]\", DisplayName: \"[resource_displayname[export]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"exportas\", ShortDisplayName: \"[resource_shortdisplayname[exportAs]]\", DisplayName: \"[resource_displayname[exportAs]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"download\", ShortDisplayName: \"[resource_shortdisplayname[download]]\", DisplayName: \"[resource_displayname[download]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"close\", ShortDisplayName: \"[resource_shortdisplayname[close]]\", DisplayName: \"[resource_displayname[close]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"greaterthan\", ShortDisplayName: \"[resource_shortdisplayname[greaterthan]]\", DisplayName: \"[resource_displayname[greaterthan]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"lessthan\", ShortDisplayName: \"[resource_shortdisplayname[lessthan]]\", DisplayName: \"[resource_displayname[lessthan]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"isequalto\", ShortDisplayName: \"[resource_shortdisplayname[isequalto]]\", DisplayName: \"[resource_displayname[isequalto]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"startswith\", ShortDisplayName: \"[resource_shortdisplayname[startswith]]\", DisplayName: \"[resource_displayname[startswith]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"isnotequalto\", ShortDisplayName: \"[resource_shortdisplayname[isnotequalto]]\", DisplayName: \"[resource_displayname[isnotequalto]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"contains\", ShortDisplayName: \"[resource_shortdisplayname[contains]]\", DisplayName: \"[resource_displayname[contains]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"fromdate\", ShortDisplayName: \"[resource_shortdisplayname[fromdate]]\", DisplayName: \"[resource_displayname[fromdate]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"todate\", ShortDisplayName: \"[resource_shortdisplayname[todate]]\", DisplayName: \"[resource_displayname[todate]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"exportcsv\", ShortDisplayName: \"[resource_shortdisplayname[exportcsv]]\", DisplayName: \"[resource_displayname[exportcsv]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"exportxml\", ShortDisplayName: \"[resource_shortdisplayname[exportxml]]\", DisplayName: \"[resource_displayname[exportxml]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"exportexcel\", ShortDisplayName: \"[resource_shortdisplayname[exportexcel]]\", DisplayName: \"[resource_displayname[exportexcel]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"exportjson\", ShortDisplayName: \"[resource_shortdisplayname[exportjson]]\", DisplayName: \"[resource_displayname[exportjson]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"grouping\", ShortDisplayName: \"[resource_shortdisplayname[grouping]]\", DisplayName: \"[resource_displayname[grouping]]\", Description: \"[resource_description[grouping]]\", Culture: \"\"},\r\n\t{Key: \"Default\", Name: \"selecteditemsformat\", ShortDisplayName: \"[resource_shortdisplayname[selecteditemsformat]]\", DisplayName: \"[resource_displayname[selecteditemsformat]]\", Description: \"[resource_description[selecteditemsformat]]\", Culture: \"\"}\r\n]);",
  "CreatedOn": "2021-10-05T09:17:55.376Z",
  "LastUpdated": "2021-10-05T09:17:55.376Z",
  "Key": "Core"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Script",
                Data = """
{
  "Name": "KendoCultures",
  "Content": "var config = {\r\n    // <language code>-<country/region code>\r\n    name: \"fr-FR\",\r\n      // The \"numberFormat\" defines general number formatting rules.\r\n    numberFormat: {\r\n        //numberFormat has only negative pattern unlike the percent and currency\r\n        //negative pattern: one of (n)|-n|- n|n-|n -\r\n        pattern: [\"-n\"],\r\n        //number of decimal places\r\n        decimals: 2,\r\n        //string that separates the number groups (1,000,000)\r\n        \",\": \" \",\r\n        // A string that separates a number from the fractional point.\r\n        \".\": \",\",\r\n        //the length of each number group\r\n        groupSize: [3],\r\n        //formatting rules for percent number\r\n        percent: {\r\n            //[negative pattern, positive pattern]\r\n            // negativePattern: one of -n %|-n%|-%n|%-n|%n-|n-%|n%-|-% n|n %-|% n-|% -n|n- %\r\n            //positivePattern: one of n %|n%|%n|% n\r\n            pattern: [\"-n %\", \"n %\"],\r\n            // The number of decimal places.\r\n            decimals: 2,\r\n            // The string that separates the number groups (1,000,000 %).\r\n            \",\": \" \",\r\n            // The string that separates a number from the fractional point.\r\n            \".\": \",\",\r\n            // The length of each number group.\r\n            groupSize: [3],\r\n            //percent symbol\r\n            symbol: \"%\"\r\n        },\r\n        currency: {\r\n            // [negative pattern, positive pattern]\r\n            // negativePattern: one of \"($n)|-$n|$-n|$n-|(n$)|-n$|n-$|n$-|-n $|-$ n|n $-|$ n-|$ -n|n- $|($ n)|(n $)\"\r\n            //positivePattern: one of \"$n|n$|$ n|n $\"\r\n            pattern: [\"-$n\", \"$n\"],\r\n            // The number of decimal places.\r\n            decimals: 2,\r\n            // The string that separates the number groups (1,000,000 $).\r\n            \",\": \" \",\r\n            // The string that separates a number from the fractional point.\r\n            \".\": \",\",\r\n            // The length of each number group.\r\n            groupSize: [3],\r\n            // The currency symbol.\r\n            symbol: \"\"\r\n        }\r\n    },\r\n    calendars: {\r\n        standard: {\r\n            days: {\r\n                // The full day names.\r\n                names: [\"[resource_displayname[Sunday]]\", \"[resource_displayname[Monday]]\", \"[resource_displayname[Tuesday]]\", \"[resource_displayname[Wednesday]]\", \"[resource_displayname[Thursday]]\", \"[resource_displayname[Friday]]\", \"resource_displayname[Saturday]]\"],\r\n                // The abbreviated day names.\r\n                 namesAbbr: [\"[resource_shortdisplayname[Sunday]]\", \"[resource_shortdisplayname[Monday]]\", \"[resource_shortdisplayname[Tuesday]]\", \"[resource_shortdisplayname[Wednesday]]\", \"[resource_shortdisplayname[Thursday]]\", \"[resource_shortdisplayname[Friday]]\", \"resource_shortdisplayname[Saturday]]\"],\r\n                // The shortest day names.\r\n                namesShort: [\"[resource_shortdisplayname[Sunday]]\", \"[resource_shortdisplayname[Monday]]\", \"[resource_shortdisplayname[Tuesday]]\", \"[resource_shortdisplayname[Wednesday]]\", \"[resource_shortdisplayname[Thursday]]\", \"[resource_shortdisplayname[Friday]]\", \"resource_shortdisplayname[Saturday]]\"],\r\n            },\r\n            months: {\r\n                // The full month names.\r\n                names: [\"[resource_displayname[January]]\", \"[resource_displayname[February]]\", \"[resource_displayname[March]]\", \"[resource_displayname[April]]\", \"[resource_displayname[May]]\", \"[resource_displayname[June]]\", \"[resource_displayname[July]]\", \"[resource_displayname[August]]\", \"[resource_displayname[September]]\", \"[resource_displayname[October]]\", \"[resource_displayname[November]]\", \"[resource_displayname[December]]\"],\r\n                // abbreviated month names\r\n                namesAbbr: [\"[resource_shortdisplayname[January]]\", \"[resource_shortdisplayname[February]]\", \"[resource_shortdisplayname[March]]\", \"[resource_shortdisplayname[April]]\", \"[resource_shortdisplayname[May]]\", \"[resource_shortdisplayname[June]]\", \"[resource_shortdisplayname[July]]\", \"[resource_shortdisplayname[August]]\", \"[resource_shortdisplayname[September]]\", \"[resource_shortdisplayname[October]]\", \"[resource_shortdisplayname[November]]\", \"[resource_shortdisplayname[December]]\"],\r\n            },\r\n              // The AM and PM designators.\r\n              // [standard,lowercase,uppercase]\r\n            AM: [ \"AM\", \"am\", \"AM\" ],\r\n            PM: [ \"PM\", \"pm\", \"PM\" ],\r\n              // The set of predefined date and time patterns used by the culture.\r\n            patterns: {\r\n                standard: \"[resource_displayname[dateformat]]\",\r\n                d: \"[resource_displayname[dateformat]]\",\r\n                D: \"dddd, MMMM dd, yyyy\",\r\n                F: \"dddd, MMMM dd, yyyy h:mm:ss tt\",\r\n                g: \"[resource_displayname[dateformat]] h:mm tt\",\r\n                G: \"[resource_displayname[dateformat]] h:mm:ss tt\",\r\n                m: \"MMMM dd\",\r\n                M: \"MMMM dd\",\r\n                s: \"yyyy'-'MM'-'ddTHH':'mm':'ss\",\r\n                t: \"h:mm tt\",\r\n                T: \"h:mm:ss tt\",\r\n                u: \"yyyy'-'MM'-'dd HH':'mm':'ss'Z'\",\r\n                y: \"MMMM, yyyy\",\r\n                Y: \"MMMM, yyyy\"\r\n            },\r\n              // The first day of the week (0 = Sunday, 1 = Monday, and so on).\r\n            firstDay: 0\r\n        }\r\n    }\r\n};\r\nkendo.cultures[\"fr-FR\"] = config;\r\nkendo.culture(session.culture);",
  "CreatedOn": "2021-10-05T09:15:21.984Z",
  "LastUpdated": "2021-10-05T09:15:21.984Z",
  "Key": "Core"
}
"""
            },
            new PackageItem
            {
                Type = "Core/Script",
                Data = """
{
  "Name": "MigrateApp",
  "Content": "exportPackages: async function(vars) {\r\n    var exportedData = (await api.get(\"ContentManagement/App(\" + vars.SourceApp.Id + \")/Export()?$expand=Items\")).value;\r\n    if(vars.ForceUpdate == true) {\r\n        for(var package of exportedData) {\r\n            for(var item of package.Items) {\r\n                var data = JSON.parse(item.Data);\r\n                for(var record of data) {\r\n                    if(record.hasOwnProperty('LastUpdated')) {\r\n                        record.LastUpdated = (new Date()).toISOString();\r\n                    } else if(record.hasOwnProperty('CreatedOn')) {\r\n                        record.CreatedOn = (new Date()).toISOString();\r\n                    }\r\n                }\r\n                item.Data = JSON.stringify(data);\r\n            }\r\n        }\r\n    }\r\n    \r\n    vars.ExportedPackages = exportedData;\r\n},\r\ngetSelectedPackages: async function (vars) {\r\n    vars.Packages = vars.ExportedPackages.filter(r => vars.SelectedPackageNames.filter(x => x == r.Name).length > 0);\r\n},\r\ngetAppCultures: async function (vars) {\r\n    vars.AppCultures = (await api.get(\"ContentManagement/App(\" + vars.SourceApp.Id + \")?$expand=Cultures\")).Cultures.map(c => ({ CultureId: c.CultureId }));\r\n},\r\ncreateExternalApi: async function (vars) {\r\n    var apiSendBackup = api.send;\r\n    api.send = function (type, query, data) {\r\n        var d = new Date();\r\n        var time = d.getHours() + \":\" + d.getMinutes() + \":\" + d.getSeconds();\r\n        var message = type + \"-\" + query;\r\n        if (data !== null && !query.includes(\"ImportThis\")) {\r\n            var json = JSON.stringify(data, null, 2);\r\n            message = message + \"<br>\" + json; // spacing level = 2\r\n        }\r\n        console.debug(message);\r\n        return apiSendBackup.apply(api, [type, query, data]);\r\n    };\r\n\r\n},\r\nloginExternalApi: async function(vars) {\r\n    console.log(\"Logging into API instance: \" + vars.RemoteAuth.Api);\r\n    vars.ExternalApi = new Api({\r\n        apiRoot: vars.RemoteAuth.Api\r\n    });\r\n\r\n    await vars.ExternalApi.login(vars.RemoteAuth.User, vars.RemoteAuth.Pass, true);\r\n},\r\nupdateApp: async function(vars) {\r\n    vars.AppExists = (await vars.ExternalApi.get(\"ContentManagement/App?$filter=Domain eq '\" + vars.Domain + \"'\")).value;\r\n    vars.App = vars.AppExists[0];\r\n    console.log(\"Updating default theme, default culture id & linked cultures on target app\");\r\n    vars.App.DefaultCultureId = vars.SourceApp.DefaultCultureId;\r\n    vars.App.DefaultTheme = vars.SourceApp.DefaultTheme;\r\n    if (vars.SelectedPackageNames.filter(r => r == \"AppCultures\").length > 0) {\r\n        vars.App.Cultures = vars.AppCultures;\r\n    }\r\n    await vars.ExternalApi.update(\"ContentManagement/App(\" + vars.App.Id + \")?$expand=Cultures\", vars.App);\r\n},\r\nimportPackages: async function (vars) {\r\n    for (var i = 0; i < vars.Packages.length; i = i + 1) {\r\n        console.log(\"Importing \" + vars.Packages[i].Name);\r\n        await vars.ExternalApi.add(\"Packaging/Package/ImportThis?appId=\" + vars.App.Id, [vars.Packages[i]]);\r\n    }\r\n},\r\ngetFiles: async function (vars) {\r\n    vars.Files = [];\r\n    if (vars.SelectedPackageNames.filter(r => r == \"DMS\").length > 0) {\r\n        var query = \"\";\r\n        for (var i = 0; i < vars.DMSPaths.length; i = i + 1) {\r\n            query += \"'\" + vars.DMSPaths[i] + \"',\";\r\n        }\r\n        query = query.substring(0, query.length - 1);\r\n        var folderContents = (await api.get(\"DocumentManagement/Folder?$select=Id&$filter=AppId eq \" + vars.SourceApp.Id + \" and Path in (\" + query + \")\")).value;\r\n        for (var i = 0; i < folderContents.length; i = i + 1) {\r\n            var folderFiles = (await api.get(\"DocumentManagement/File?$filter=FolderId eq \" + folderContents[i].Id)).value;\r\n            for (var j = 0; j < folderFiles.length; j = j + 1) {\r\n                vars.Files.push(folderFiles[j]);\r\n            }\r\n        }\r\n    }\r\n},\r\nuploadDMS: async function(vars) {\r\n    for (var i = 0; i < vars.Files.length; i = i + 1) {\r\n        var blob = await new Promise(function (resolve, reject) {\r\n            console.log(\"Retrieving \" + vars.Files[i].Path);\r\n            var sourceReq = new XMLHttpRequest();\r\n            sourceReq.open(\"GET\", \"/Api/DMS/\" + vars.Files[i].Path + \"?t=\" + session.token, true);\r\n            sourceReq.setRequestHeader(\"Authorization\", \"bearer \" + session.token);\r\n            sourceReq.responseType = \"blob\";\r\n            sourceReq.onload = function (oEvent) {\r\n                if (this.readyState === 4 && this.status >= 200 & this.status <= 300) {\r\n                    resolve(this.response);\r\n                } else {\r\n                    reject(this.status);\r\n                }\r\n            };\r\n            sourceReq.send();\r\n        });\r\n        await new Promise(function (resolve, reject) {\r\n            console.log(\"Sending \" + vars.Files[i].Path);\r\n            var destReq = new XMLHttpRequest();\r\n            destReq.open(\"PUT\", vars.ExternalApi.apiRoot + \"DMS/\" + vars.Files[i].Path, true);\r\n            destReq.setRequestHeader(\"Authorization\", \"bearer \" + vars.ExternalApi.token);\r\n            destReq.setRequestHeader(\"Content-Type\", vars.Files[i].MimeType);\r\n            destReq.onload = function (oEvent) {\r\n                if (this.readyState === 4 && this.status >= 200 && this.status <= 300) {\r\n                    resolve(this.response);\r\n                } else {\r\n                    reject(this.status);\r\n                }\r\n            };\r\n            destReq.send(blob);\r\n        });\r\n    }\r\n},\r\nfinish: async function(vars) {\r\n}\r\n",
  "CreatedOn": "2021-09-24T09:46:14.885Z",
  "LastUpdated": "2021-09-24T09:46:14.885Z",
  "Key": "Core"
}
"""
            },
        ]
    };

    static Package PageRoles => new()
    {
        Name = "Content Management Page Roles",
        Category = "CMS",
        Description = "Content Management Page Roles.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Admin/AppManagement",
  "Role": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Admin",
  "Role": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation",
  "Role": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation",
  "Role": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation",
  "Role": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Admin/CoreManagement",
  "Role": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Admin/CommonCache",
  "Role": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation",
  "Role": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation",
  "Role": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation",
  "Role": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/AppManagement",
  "Role": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/AppManagement",
  "Role": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/AppManagement",
  "Role": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Admin/ContentManagement",
  "Role": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/SSODocumentation",
  "Role": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/SSODocumentation",
  "Role": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/SSODocumentation",
  "Role": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/Metadata",
  "Role": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/Metadata",
  "Role": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/Metadata",
  "Role": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/SSODocumentation/SSOAPI/SSOMetadata",
  "Role": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/SSODocumentation/SSOAPI/SSOMetadata",
  "Role": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/SSODocumentation/SSOAPI/SSOMetadata",
  "Role": "Guests"
}
"""
            },
        ]
    };
}
