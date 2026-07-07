using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Exposures.Setup;

public static partial class UIBaseline
{
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
}