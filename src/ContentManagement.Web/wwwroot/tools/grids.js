window.ContentManagementGrids = {
    apiRoot: "/Api/ContentManagement",
    initialized: false,
    contexts: {
        appId: null,
        pageId: null
    },
    appRows: [],
    pageRows: [],

    pageChildEntitySets: [
        {
            name: "PageInfo",
            title: "Page Info",
            description: "Metadata rows owned by this Page",
            key: "Id",
            context: { type: "page", field: "PageId" },
            fields: {
                Id: { type: "number", editable: false },
                PageId: { type: "number" },
                CultureId: { type: "string" },
                Title: { type: "string" },
                Description: { type: "string" },
                Keywords: { type: "string" }
            },
            columns: ["Id", "PageId", "CultureId", "Title", "Description", "Keywords"]
        },
        {
            name: "Content",
            title: "Content",
            description: "Rendered content rows owned by this Page",
            key: "Id",
            context: { type: "page", field: "PageId" },
            fields: {
                Id: { type: "number", editable: false },
                PageId: { type: "number" },
                CultureId: { type: "string" },
                Name: { type: "string" },
                Html: { type: "string" }
            },
            columns: ["Id", "PageId", "CultureId", "Name", "Html"]
        },
        {
            name: "PageRole",
            title: "Page Roles",
            description: "Role links owned by this Page",
            composite: true,
            context: { type: "page", field: "PageId" },
            fields: {
                PageId: { type: "number" },
                RoleId: { type: "string" }
            },
            columns: ["PageId", "RoleId"]
        },
    ],

    entitySets: [
        {
            name: "App",
            title: "Apps",
            description: "Aggregate roots",
            key: "Id",
            fields: {
                Id: { type: "number", editable: false },
                DefaultCultureId: { type: "string" },
                TenantId: { type: "string" },
                Name: { type: "string" },
                Domain: { type: "string" },
                DefaultTheme: { type: "string" },
                ConfigJson: { type: "string" }
            },
            columns: ["Id", "Name", "Domain", "TenantId", "DefaultCultureId", "DefaultTheme", "ConfigJson"]
        },
        {
            name: "AppCulture",
            title: "App Cultures",
            description: "Cultures enabled for the selected App",
            composite: true,
            context: { type: "app", field: "AppId" },
            fields: {
                AppId: { type: "number" },
                CultureId: { type: "string" }
            },
            columns: ["AppId", "CultureId"]
        },
        {
            name: "Page",
            title: "Pages",
            description: "Pages owned by the selected App",
            key: "Id",
            stamp: "standard",
            context: { type: "app", field: "AppId" },
            fields: {
                Id: { type: "number", editable: false },
                ParentId: { type: "number" },
                AppId: { type: "number" },
                Order: { type: "number" },
                ShowOnMenus: { type: "boolean" },
                Name: { type: "string" },
                Path: { type: "string" },
                ResourceKey: { type: "string" },
                Layout: { type: "string" }
            },
            columns: ["Id", "AppId", "ParentId", "Order", "ShowOnMenus", "Name", "Path", "ResourceKey", "Layout"]
        },
        {
            name: "Component",
            title: "Components",
            description: "Reusable components owned by the selected App",
            key: "Id",
            stamp: "standard",
            context: { type: "app", field: "AppId" },
            fields: {
                Id: { type: "number", editable: false },
                AppId: { type: "number" },
                Name: { type: "string" },
                Description: { type: "string" },
                ResourceKey: { type: "string" },
                Content: { type: "string" },
                Script: { type: "string" },
                Key: { type: "string" }
            },
            columns: ["Id", "AppId", "Name", "Key", "ResourceKey", "Description", "Content", "Script"]
        },
        {
            name: "Layout",
            title: "Layouts",
            description: "Layouts owned by the selected App",
            key: "Id",
            stamp: "standard",
            context: { type: "app", field: "AppId" },
            fields: {
                Id: { type: "number", editable: false },
                AppId: { type: "number" },
                Name: { type: "string" },
                Description: { type: "string" },
                HeaderHtml: { type: "string" },
                Html: { type: "string" },
                Script: { type: "string" }
            },
            columns: ["Id", "AppId", "Name", "Description", "HeaderHtml", "Html", "Script"]
        },
        {
            name: "Resource",
            title: "Resources",
            description: "Resources owned by the selected App",
            key: "Id",
            stamp: "standard",
            context: { type: "app", field: "AppId" },
            fields: {
                Id: { type: "number", editable: false },
                AppId: { type: "number" },
                Name: { type: "string" },
                Description: { type: "string" },
                Key: { type: "string" },
                Culture: { type: "string" },
                DisplayName: { type: "string" },
                ShortDisplayName: { type: "string" }
            },
            columns: ["Id", "AppId", "Name", "Key", "Culture", "DisplayName", "ShortDisplayName", "Description"]
        },
        {
            name: "Script",
            title: "Scripts",
            description: "Scripts owned by the selected App",
            key: "Id",
            stamp: "standard",
            context: { type: "app", field: "AppId" },
            fields: {
                Id: { type: "number", editable: false },
                AppId: { type: "number" },
                Name: { type: "string" },
                Description: { type: "string" },
                Key: { type: "string" },
                Content: { type: "string" }
            },
            columns: ["Id", "AppId", "Name", "Key", "Description", "Content"]
        },
        {
            name: "Template",
            title: "Templates",
            description: "Templates owned by the selected App",
            key: "Id",
            stamp: "standard",
            context: { type: "app", field: "AppId" },
            fields: {
                Id: { type: "number", editable: false },
                AppId: { type: "number" },
                Name: { type: "string" },
                Description: { type: "string" },
                ResourceKey: { type: "string" },
                RawString: { type: "string" }
            },
            columns: ["Id", "AppId", "Name", "ResourceKey", "Description", "RawString"]
        },
        {
            name: "Culture",
            title: "Cultures",
            description: "Reference aggregate roots",
            key: "Id",
            keyType: "string",
            fields: {
                Id: { type: "string" },
                Name: { type: "string" }
            },
            columns: ["Id", "Name"]
        },
        {
            name: "CommonObject",
            title: "Common Objects",
            description: "Versioned common data roots",
            key: "Id",
            stamp: "standard",
            fields: {
                Id: { type: "number", editable: false },
                Name: { type: "string" },
                Description: { type: "string" },
                Version: { type: "number" },
                Key: { type: "string" },
                Type: { type: "string" },
                Json: { type: "string" },
                Culture: { type: "string" }
            },
            columns: ["Id", "Name", "Key", "Type", "Version", "Culture", "Description", "Json"]
        },
        {
            name: "Submission",
            title: "Submissions",
            description: "Submissions owned by the selected App",
            key: "Id",
            keyType: "guid",
            stamp: "submission",
            context: { type: "app", field: "AppId" },
            fields: {
                Id: { type: "string" },
                AppId: { type: "number" },
                SourceComponent: { type: "string" },
                State: { type: "string" },
                DataJson: { type: "string" }
            },
            columns: ["Id", "AppId", "SourceComponent", "State", "DataJson"]
        }
    ],

    init: function () {
        if (this.initialized || !ContentManagementApi.isAuthenticated()) {
            return;
        }

        this.initialized = true;
        this.buildEntitySurfaces();
        this.entitySets.forEach(config => this.createGrid(config));
    },

    buildEntitySurfaces: function () {
        const nav = document.getElementById("entity-nav");
        const surfaces = document.getElementById("entity-surfaces");

        this.entitySets.forEach((config, index) => {
            const surfaceId = this.surfaceId(config);
            const button = document.createElement("button");
            button.className = `cm-nav-item${index === 0 ? " active" : ""}`;
            button.type = "button";
            button.dataset.workspaceTarget = surfaceId;
            button.textContent = config.title;
            button.addEventListener("click", () => this.showSurface(button));
            nav.appendChild(button);

            const section = document.createElement("section");
            section.id = surfaceId;
            section.className = `cm-surface${index === 0 ? " active" : ""}`;
            section.innerHTML =
                `<div class="cm-toolbar">` +
                `<div><h2>${config.title}</h2><span>${config.description}</span></div>` +
                this.contextHtml(config) +
                `</div>` +
                `<div id="${this.gridId(config)}" class="cm-grid"></div>`;
            surfaces.appendChild(section);
        });

        document
            .querySelectorAll("[data-context-type='app']")
            .forEach(select => select.addEventListener("change", event => this.setAppContext(event.target.value)));

        document
            .querySelectorAll("[data-context-type='page']")
            .forEach(select => select.addEventListener("change", event => this.setPageContext(event.target.value)));
    },

    contextHtml: function (config) {
        if (!config.context) {
            return "";
        }

        const label = config.context.type === "app" ? "App" : "Page";
        const selectorClass = config.context.type === "app" ? "app-context" : "page-context";

        return `<label class="cm-context">` +
            `<span>${label}</span>` +
            `<select class="form-select form-select-sm ${selectorClass}" data-context-type="${config.context.type}">` +
            `<option value="">Select ${label}</option>` +
            `</select>` +
            `</label>`;
    },

    showSurface: function (button) {
        const target = button.dataset.workspaceTarget;

        document
            .querySelectorAll("[data-workspace-target]")
            .forEach(item => item.classList.toggle("active", item === button));

        document
            .querySelectorAll(".cm-surface")
            .forEach(surface => surface.classList.toggle("active", surface.id === target));
    },

    createGrid: function (config) {
        $(`#${this.gridId(config)}`).kendoGrid({
            dataSource: {
                transport: {
                    read: options => this.read(config, options),
                    create: options => this.create(config, options),
                    update: options => this.update(config, options),
                    destroy: options => this.destroy(config, options)
                },
                schema: {
                    model: {
                        id: config.composite ? "_rowKey" : config.key,
                        fields: this.modelFields(config)
                    }
                },
                pageSize: 20
            },
            toolbar: [{ name: "create", text: `Create ${config.title}` }],
            editable: {
                mode: "popup",
                confirmation: false,
                window: {
                    width: "720px"
                }
            },
            pageable: true,
            sortable: true,
            filterable: true,
            resizable: true,
            reorderable: true,
            scrollable: true,
            selectable: "row",
            columns: this.columns(config),
            detailTemplate: config.name === "Page" ? this.pageDetailTemplate() : undefined,
            detailInit: config.name === "Page" ? event => this.onPageDetailInit(event) : undefined,
            noRecords: true,
            messages: {
                noRecords: this.noRecordsMessage(config)
            },
            change: () => this.onSelectionChanged(config),
            edit: event => this.onEdit(config, event),
            save: () => ContentManagementApi.notify("Saving..."),
            remove: () => ContentManagementApi.notify("Deleting..."),
            dataBound: () => this.onDataBound(config)
        });
    },

    modelFields: function (config) {
        const fields = Object.assign(
            config.composite ? { _rowKey: { editable: false } } : {},
            config.fields);

        if (config.context) {
            fields[config.context.field] = Object.assign({}, fields[config.context.field], { editable: false });
        }

        return fields;
    },

    columns: function (config) {
        const columns = config.columns.map(field => ({
            field: field,
            title: this.label(field),
            width: this.widthFor(field)
        }));

        columns.push({
            command: [
                { name: "edit", text: "Edit" },
                { name: "destroy", text: "Delete" }
            ],
            title: "Actions",
            width: 180
        });

        return columns;
    },

    read: async function (config, options, contextValues) {
        try {
            if (config.context && !this.contextValue(config.context.type, contextValues)) {
                options.success([]);
                return;
            }

            const body = await ContentManagementApi.get(this.readUrl(config, contextValues));
            const rows = ContentManagementApi.unwrapCollection(body)
                .map(row => this.withRowState(config, row, contextValues));
            options.success(rows);
        } catch (error) {
            options.error(error);
        }
    },

    readUrl: function (config, contextValues) {
        let url = `${this.apiRoot}/${config.name}?$top=500`;

        if (config.context) {
            const filter = `${config.context.field} eq ${this.contextValue(config.context.type, contextValues)}`;
            url += `&$filter=${encodeURIComponent(filter)}`;
        }

        return url;
    },

    create: async function (config, options, contextValues) {
        try {
            if (config.context && !this.contextValue(config.context.type, contextValues)) {
                throw new Error(`Select a ${config.context.type} before creating ${config.title}.`);
            }

            const payload = this.preparePayload(config, options.data, true, contextValues);
            const result = await ContentManagementApi.post(`${this.apiRoot}/${config.name}`, payload);
            options.success(this.withRowState(config, result ?? payload, contextValues));
            ContentManagementApi.notify(`${config.title} created`);
        } catch (error) {
            options.error(error);
        }
    },

    update: async function (config, options, contextValues) {
        try {
            const payload = this.preparePayload(config, options.data, false, contextValues);
            let result;

            if (config.composite) {
                await this.deleteComposite(config, options.data._original ?? options.data, contextValues);
                result = await ContentManagementApi.post(`${this.apiRoot}/${config.name}`, payload);
            } else {
                result = await ContentManagementApi.put(
                    `${this.apiRoot}/${config.name}(${this.formatKey(config, options.data[config.key])})`,
                    payload);
            }

            options.success(this.withRowState(config, result ?? payload, contextValues));
            ContentManagementApi.notify(`${config.title} updated`);
        } catch (error) {
            options.error(error);
        }
    },

    destroy: async function (config, options, contextValues) {
        try {
            if (config.composite) {
                await this.deleteComposite(config, options.data._original ?? options.data, contextValues);
            } else {
                await ContentManagementApi.delete(
                    `${this.apiRoot}/${config.name}(${this.formatKey(config, options.data[config.key])})`);
            }

            options.success(options.data);
            ContentManagementApi.notify(`${config.title} deleted`);
        } catch (error) {
            options.error(error);
        }
    },

    deleteComposite: function (config, data, contextValues) {
        return ContentManagementApi.post(
            `${this.apiRoot}/${config.name}/DeleteAll`,
            [this.preparePayload(config, data, false, contextValues)]);
    },

    preparePayload: function (config, data, isCreate, contextValues) {
        const payload = {};

        Object.keys(config.fields).forEach(field => {
            const value = data[field];

            if (value !== undefined) {
                payload[field] = value;
            }
        });

        if (config.context) {
            payload[config.context.field] = Number(this.contextValue(config.context.type, contextValues));
        }

        if (config.keyType === "guid" && isCreate && !payload[config.key]) {
            payload[config.key] = crypto.randomUUID();
        }

        this.applyStamp(config, payload, isCreate);

        return payload;
    },

    applyStamp: function (config, payload, isCreate) {
        if (!config.stamp) {
            return;
        }

        const now = new Date().toISOString();
        const userId = ContentManagementApi.currentUserId();

        if (config.stamp === "standard") {
            if (isCreate) {
                payload.CreatedOn = payload.CreatedOn || now;
                payload.CreatedBy = payload.CreatedBy || userId;
            }

            payload.LastUpdated = now;
            payload.LastUpdatedBy = userId;
        }

        if (config.stamp === "submission") {
            if (isCreate) {
                payload.CreatedOn = payload.CreatedOn || now;
                payload.CreatedBy = payload.CreatedBy || userId;
            }

            payload.LastUpdatedOn = now;
            payload.LastUpdatedBy = userId;
        }
    },

    onEdit: function (config, event, contextValues) {
        if (config.context) {
            event.model.set(config.context.field, Number(this.contextValue(config.context.type, contextValues)));
        }
    },

    pageDetailTemplate: function () {
        const tabs = this.pageChildEntitySets
            .map((config, index) => `<li class="${index === 0 ? "k-active" : ""}">${config.title}</li>`)
            .join("");
        const panes = this.pageChildEntitySets
            .map(config =>
                `<div>` +
                `<div class="cm-detail-heading">` +
                `<h3>${config.title}</h3>` +
                `<span>${config.description}</span>` +
                `</div>` +
                `<div class="cm-child-grid" data-page-child-grid="${config.name}"></div>` +
                `</div>`)
            .join("");

        return `<div class="cm-page-detail">` +
            `<div class="cm-page-tabs">` +
            `<ul>${tabs}</ul>` +
            panes +
            `</div>` +
            `</div>`;
    },

    onPageDetailInit: function (event) {
        const page = event.data;
        const detail = event.detailRow;
        const contextValues = { page: page.Id };

        detail.find(".cm-page-tabs").kendoTabStrip();
        this.pageChildEntitySets.forEach(config =>
            this.createChildGrid(
                detail.find(`[data-page-child-grid='${config.name}']`),
                config,
                contextValues));
    },

    createChildGrid: function (element, config, contextValues) {
        element.kendoGrid({
            dataSource: {
                transport: {
                    read: options => this.read(config, options, contextValues),
                    create: options => this.create(config, options, contextValues),
                    update: options => this.update(config, options, contextValues),
                    destroy: options => this.destroy(config, options, contextValues)
                },
                schema: {
                    model: {
                        id: config.composite ? "_rowKey" : config.key,
                        fields: this.modelFields(config)
                    }
                },
                pageSize: 10
            },
            toolbar: [{ name: "create", text: `Create ${config.title}` }],
            editable: {
                mode: "popup",
                confirmation: false,
                window: {
                    width: "720px"
                }
            },
            pageable: true,
            sortable: true,
            filterable: true,
            resizable: true,
            reorderable: true,
            scrollable: true,
            columns: this.columns(config),
            noRecords: true,
            messages: {
                noRecords: `No ${config.title} found for this Page.`
            },
            edit: childEvent => this.onEdit(config, childEvent, contextValues),
            save: () => ContentManagementApi.notify("Saving..."),
            remove: () => ContentManagementApi.notify("Deleting...")
        });
    },

    onSelectionChanged: function (config) {
        const grid = $(`#${this.gridId(config)}`).data("kendoGrid");
        const row = grid.dataItem(grid.select());

        if (!row) {
            return;
        }

        if (config.name === "App") {
            this.setAppContext(row.Id);
        }

        if (config.name === "Page") {
            this.setPageContext(row.Id);
            grid.expandRow(grid.select());
        }
    },

    onDataBound: function (config) {
        if (config.name === "App") {
            this.appRows = this.gridRows(config);
            this.refreshAppSelectors();

            if (!this.contexts.appId && this.appRows.length > 0) {
                this.setAppContext(this.appRows[0].Id);
            }
        }

        if (config.name === "Page") {
            this.pageRows = this.gridRows(config);
            this.refreshPageSelectors();

            if (!this.contexts.pageId && this.pageRows.length > 0) {
                this.setPageContext(this.pageRows[0].Id);
            }
        }

        this.updateCreateButtons();
        ContentManagementApi.notify("Ready");
    },

    gridRows: function (config) {
        const grid = $(`#${this.gridId(config)}`).data("kendoGrid");
        return grid?.dataSource?.data()?.toJSON?.() ?? [];
    },

    setAppContext: function (value, refresh = true) {
        const appId = value ? Number(value) : null;

        if (this.contexts.appId === appId) {
            return;
        }

        this.contexts.appId = appId;
        this.contexts.pageId = null;
        this.pageRows = [];
        this.refreshAppSelectors();
        this.refreshPageSelectors();

        if (refresh) {
            this.refreshContextGrids("app");
            this.refreshContextGrids("page");
        }
    },

    setPageContext: function (value, refresh = true) {
        const pageId = value ? Number(value) : null;

        if (this.contexts.pageId === pageId) {
            return;
        }

        this.contexts.pageId = pageId;
        this.refreshPageSelectors();

        if (refresh) {
            this.refreshContextGrids("page");
        }
    },

    refreshContextGrids: function (contextType) {
        this.entitySets
            .filter(config => config.context?.type === contextType)
            .forEach(config => {
                const grid = $(`#${this.gridId(config)}`).data("kendoGrid");

                if (grid) {
                    grid.dataSource.read();
                }
            });
    },

    refreshAppSelectors: function () {
        this.fillContextSelectors(
            ".app-context",
            this.appRows,
            this.contexts.appId,
            app => `${app.Id} - ${app.Name ?? app.Domain ?? "App"}`);
    },

    refreshPageSelectors: function () {
        this.fillContextSelectors(
            ".page-context",
            this.pageRows,
            this.contexts.pageId,
            page => `${page.Id} - ${page.Name ?? page.Path ?? "Page"}`);
    },

    fillContextSelectors: function (selector, rows, selectedValue, labelFactory) {
        document.querySelectorAll(selector).forEach(select => {
            const current = selectedValue ? String(selectedValue) : "";
            select.innerHTML = `<option value="">Select ${select.dataset.contextType}</option>`;

            rows.forEach(row => {
                const option = document.createElement("option");
                option.value = row.Id;
                option.textContent = labelFactory(row);
                select.appendChild(option);
            });

            select.value = current;
        });
    },

    updateCreateButtons: function () {
        this.entitySets.forEach(config => {
            const grid = $(`#${this.gridId(config)}`).data("kendoGrid");
            const button = grid?.wrapper?.find(".k-grid-add");

            if (!button?.length || !config.context) {
                return;
            }

            const enabled = Boolean(this.contextValue(config.context.type));
            button.toggleClass("k-disabled", !enabled);
            button.attr("aria-disabled", String(!enabled));
        });
    },

    contextValue: function (contextType, contextValues) {
        if (contextValues && Object.prototype.hasOwnProperty.call(contextValues, contextType)) {
            return contextValues[contextType];
        }

        return contextType === "app" ? this.contexts.appId : this.contexts.pageId;
    },

    noRecordsMessage: function (config) {
        if (!config.context) {
            return `No ${config.title} found.`;
        }

        const contextName = config.context.type === "app" ? "App" : "Page";
        return `Select a ${contextName} to manage ${config.title}.`;
    },

    withRowState: function (config, row, contextValues) {
        const copy = Object.assign({}, row);

        if (config.composite) {
            copy._rowKey = this.compositeKey(config, copy);
            copy._original = this.preparePayload(config, copy, false, contextValues);
        }

        return copy;
    },

    compositeKey: function (config, row) {
        return Object.keys(config.fields)
            .map(field => row[field])
            .join("|");
    },

    formatKey: function (config, value) {
        if (config.keyType === "string") {
            return `'${String(value).replace(/'/g, "''")}'`;
        }

        return value;
    },

    surfaceId: function (config) {
        return `surface-${config.name.toLowerCase()}`;
    },

    gridId: function (config) {
        return `grid-${config.name.toLowerCase()}`;
    },

    label: function (field) {
        return field.replace(/([a-z])([A-Z])/g, "$1 $2");
    },

    widthFor: function (field) {
        if (field === "Id") {
            return 110;
        }

        if (field.endsWith("Id")) {
            return 150;
        }

        if (["Json", "ConfigJson", "DataJson", "Html", "RawString", "Content", "Script"].includes(field)) {
            return 340;
        }

        return 190;
    }
};

document.addEventListener("content-management-auth-changed", event => {
    if (event.detail.isAuthenticated) {
        window.ContentManagementGrids.init();
    }
});

document.addEventListener("DOMContentLoaded", () => window.ContentManagementGrids.init());
