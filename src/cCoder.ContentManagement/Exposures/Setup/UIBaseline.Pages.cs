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
            new PackageItem
            {
                Type = "Core/Page",
                Data = """
{
  "Path": "",
  "Name": "Home",
  "ResourceKey": "",
  "ShowOnMenus": true,
  "Order": 1,
  "LastUpdated": "2024-09-06T15:45:48.0367217+01:00",
  "Layout": "Default",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "<div class=\"row g-4\">\n    <div class=\"col-lg-6\">\n        <div class=\"component\" name=\"Other\">\n            <h3>Welcome to cCoder</h3>\n            <p>Your environment is ready. This starter app gives you a working platform instance with content management, application configuration, documentation, workflows, and security already wired together.</p>\n            <p>Use this space as a launch point for your own pages, layouts, resources, components, and integrations.</p>\n        </div>\n    </div>\n    <div class=\"col-lg-6\">\n        <div class=\"component\" name=\"Other\">\n            <h3>What to do next</h3>\n            <ul>\n                <li><a href=\"/Admin\">Open the admin area</a> to explore platform management tools.</li>\n                <li><a href=\"/Admin/AppManagement\">Manage pages and content</a> for your first app experience.</li>\n                <li><a href=\"/Admin/AppManagement/Configuration\">Review app configuration</a> and tailor the starter environment.</li>\n                <li><a href=\"/Documentation\">Browse the built-in documentation</a> for platform guidance.</li>\n            </ul>\n        </div>\n    </div>\n    <div class=\"col-12\">\n        <div class=\"component\" name=\"Other\">\n            <h3>Starter guidance</h3>\n            <p>The default data set is intended to help you explore the platform quickly. Keep what is useful, reshape what is not, and treat this app as the baseline for your own implementation.</p>\n        </div>\n    </div>\n</div>"
    },
    {
      "CultureId": "en-GB",
      "Name": "body",
      "Html": "<div class=\"row g-4\">\n    <div class=\"col-lg-6\">\n        <div class=\"component\" name=\"Other\">\n            <h3>Welcome to cCoder</h3>\n            <p>Your environment is ready. This starter app gives you a working platform instance with content management, application configuration, documentation, workflows, and security already wired together.</p>\n            <p>Use this space as a launch point for your own pages, layouts, resources, components, and integrations.</p>\n        </div>\n    </div>\n    <div class=\"col-lg-6\">\n        <div class=\"component\" name=\"Other\">\n            <h3>What to do next</h3>\n            <ul>\n                <li><a href=\"/Admin\">Open the admin area</a> to explore platform management tools.</li>\n                <li><a href=\"/Admin/AppManagement\">Manage pages and content</a> for your first app experience.</li>\n                <li><a href=\"/Admin/AppManagement/Configuration\">Review app configuration</a> and tailor the starter environment.</li>\n                <li><a href=\"/Documentation\">Browse the built-in documentation</a> for platform guidance.</li>\n            </ul>\n        </div>\n    </div>\n    <div class=\"col-12\">\n        <div class=\"component\" name=\"Other\">\n            <h3>Starter guidance</h3>\n            <p>The default data set is intended to help you explore the platform quickly. Keep what is useful, reshape what is not, and treat this app as the baseline for your own implementation.</p>\n        </div>\n    </div>\n</div>"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "Home",
      "Keywords": "Home",
      "Title": "Home"
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
  "Path": "Documentation/StandardUserGuide",
  "Name": "Standard User Guide",
  "ShowOnMenus": false,
  "Order": 0,
  "LastUpdated": "2024-08-22T12:04:12.1026254+01:00",
  "Layout": "Documentation",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "<div class=\"documentation\">\n\t<h2>1. Connecting to the Portal</h1>\n\t<h3>1.1 Existing User\n</h2>\n\t<p>If you have an account already, please fill in the username and password you used to register.</p>\n\t<p style=\"text-align:center;\">\n\t\t<img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/loginpage-en.png\"\n\t\t     alt=\"Login Page\"/>\n\t</p>\n\t<h3>1.2 New User</h2>\n\t<p>It is possible to register an account with us by clicking the &ldquo;Register&rdquo; Button&nbsp;<img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/RegisterButton-en.png\"\n\t\t     alt=\"Register\"/>\n\t</p>\n\t<p>Which opens the registration form: </p>\n\t<p style=\"text-align:center;\">\n\t\t<img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/registrationform-en.png\"\n\t\t     alt=\"Registration Form\"/>\n\t</p>\n\t<p>The following must be provided:</p>\n\t<ul>\n\t\t<li>First and Last Name,</li>\n\t\t<li>Email,</li>\n\t\t<li>Password,</li>\n\t\t<li>Confirmation of password,</li>\n\t\t<li>You must also accept the terms and conditions of use and the data protection policy.</li>\n\t</ul>\n\t<p>Click&nbsp;<img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/submitbutton-en.png\"\n\t\t     alt=\"Submit Button\"/>\n\t</p>\n\t<p>You will then receive a confirmation email to the email address you provided.</p>\n\t<h3>1.3 Adding a User Account</h2>\n\t<p>Once you are logged in, you must navigate to the page.&nbsp;<img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/myregistrations-en.png\"\n\t\t     alt=\"My Registrations Button\"/>\n\t</p>\n\t<p style=\"text-align:center;\">\n\t\t<img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/useronboarding-en.png\"\n\t\t     alt=\"User Onboarding\"\n\t\t     width=\"1257\"\n\t\t     height=\"259\"/>\n\t\t<br/>\n\t</p>\n\t<p>Click the &ldquo;New&rdquo; button and fill in the form with the relevant information and submit. The form will look\n    like this:</p>\n\t<p style=\"text-align:center;\">\n\t\t<img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/useronboardingform-en.png\"\n\t\t     alt=\"User Onboarding Form\"/>\n\t</p>\n\t<h2>2. Home Page</h1>\n\t<p>When you first log in to the portal, the first screen to appear is the home page, it has 3 different grids and a\n    count down to the next cut off details.</p>\n\t<ol>\n\t\t<li>The number of active transactions. It provides a quick analysis of all approved invoices in the system.\n        The chart can be filtered by currency depending on what it is you want to view.</li>\n\t\t<li>The net outstanding balance by due date, it includes the total value of invoices in the given date range.</li>\n\t\t<li>The next cut off indicates the deadline for requesting early settlement before\n        the next due date. The supplier must submit a request for prepayment of their approved invoices before this\n        date if they want a discount.</li>\n\t\t<li>This shows the amount of, and the total value of proposed, accepted, and cancelled offers in the system.\n        These relate to the values on the transactions and do not consider discounts.</li>\n\t\t<li>A forecast of future due dates based on the active transactions in the system.<br/>\n\t\t</li>\n\t</ol>\n\t<p style=\"text-align:center;\">\n\t\t<img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/homepage-en.png\"\n\t\t     alt=\"Home Page\"\n\t\t     width=\"1636\"\n\t\t     height=\"802\"/>\n\t</p>\n\t<p>You can set the language which you&rsquo;d like to view the portal in as part of your user profile.</p>\n\t<p style=\"text-align:center;\">\n\t\t<img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/userprofile-en.png\"\n\t\t     alt=\"User Profile\"/>\n\t</p>\n\t<p>Select the culture you&rsquo;d prefer from the dropdown and click the update button to confirm your choice.<br/>\n\t</p>\n\t<h2>3. Transactions</h1>\n\t<p>The transactions page displays all pending invoices and debit/credit notes.</p>\n\t<p style=\"text-align:center;\">\n\t\t<img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/transactions-en.png\"\n\t\t     alt=\"Transactions\"/>\n\t</p>\n\t<p>\n\t\t<strong>Invoice value by month: </strong>this chart has the months on the horizontal axis and invoice value on\n    the vertical axis. Each column shows the total value of transactions recorded for the last 12 months. The column\n    colours indicate the currency. Hovering over the columns you can see the exact value of the transactions for the\n    selected month.</p>\n\t<p>Value of Credits, by Month: this chart has the months on the horizontal axis and the face value of credits on the\n    vertical axis. The graph provides the total face value of credits submitted to the portal per month. The column\n    colours indicate the currency. Hovering over the columns you can see the exact value of the transactions for the\n    selected month.</p>\n\t<h3>3.1 Active Transactions</h2>\n\t<p>Navigate to the Active Transactions page. This page provides all the supplier transactions sent to the portal.\n    These are likely to be eligible for funding and have an &ldquo;approved&rdquo; or &ldquo;pending&rdquo; status.</p>\n\t<p style=\"text-align:center;\">\n\t\t<img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/activetransactions-en.png\"\n\t\t     alt=\"Active Transactions\"/>\n\t</p>\n\t<h3>3.2 Viewing a Transaction&rsquo;s Details</h2>\n\t<p>In the Active Transaction grids, you can expand the row of the transaction you&rsquo;re interested in to view\n    more details about it. You can see any linked credits or any offers that have been made.<br/>\n\t</p>\n\t<p style=\"text-align:center;\">\n\t\t<img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/invoicedetails-en.png\"\n\t\t     alt=\"Invoice Details\"\n\t\t     width=\"1677\"\n\t\t     height=\"502\"/>\n\t</p>\n\t<p>On the left-hand side:</p>\n\t<ol>\n\t\t<li>The first box gives you all the details about that particular transaction, Number, amount etc.</li>\n\t\t<li>The second box shows any information regarding the financing of the transaction,</li>\n\t\t<li>The bottom box tells you all the history of the transactions i.e., all the actions/changes made to\n        the transaction since its import into the portal.</li>\n\t</ol>\n\t<p>On the right-hand side:</p>\n\t<ol>\n\t\t<li>The top box shows you the unique reference for the transaction,</li>\n\t\t<li>The second grid shows you the lines on the transaction, </li>\n\t\t<li>The company details section tells you which parties are involved in that particular transaction.</li>\n\t</ol>\n\t<p>&nbsp;</p>\n\t<h2>4. Early Payments</h1>\n\t<h3>4.1 Accept Offers</h2>\n\t<p>Navigate to the Accept Offers page and you will be greeted with:<br/>\n\t</p>\n\t<p style=\"text-align:center;\">\n\t\t<img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/proposedoffers-en.png\"\n\t\t     alt=\"Proposed Offers\"\n\t\t     width=\"1363\"\n\t\t     height=\"595\"/>\n\t</p>\n\t<ol>\n\t\t<li>The next cut off indicates the deadline for requesting early settlement before the next due date. The\n        supplier must submit a request for prepayment of their approved invoices before this date if they want a\n        discount. </li>\n\t\t<li>This shows the amount of, and the total value of proposed, accepted, and cancelled offers in the\n        system.\n        These relate to the values on the transactions and do not consider discounts.</li>\n\t\t<li>The tick boxes allow you to select which of the proposed offers you would like to accept.</li>\n\t\t<li>This text box allows you to enter a value, clicking the button to the left will then select all the\n        offers such that the value of them does not exceed this upper limit.</li>\n\t\t<li>These buttons allow you to either accept all proposed offers or accept the ones you have selected in\n        the grid.</li>\n\t</ol>\n\t<p>\n\t\t<strong>Please note:</strong>\n\t</p>\n\t<ul>\n\t\t<li>If you &ldquo;Accept All&rdquo; or &ldquo;Accept the Selection&rdquo; this choice is final and cannot be\n        undone.\n    </li>\n\t\t<li>Invoices are sorted by the oldest to the newest by default, in the descending order of their value, if using\n        the limit functionality, it will accept all offers starting from the oldest. It will not exceed the upper limit\n        set.\n    </li>\n\t</ul>\n\t<p style=\"text-align:center;\">\n\t\t<img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/acceptedoffers-en.png\"\n\t\t     alt=\"Accepted Offers\"/>\n\t</p>\n\t<p>\n\t\t<strong>Once the offers are accepted, they can be viewed in the accepted offers tab on the same page.</strong>\n\t</p>\n\t<h3>4.2 Offer History</h2>\n\t<p>Navigate to the &ldquo;History&rdquo; page under Early Payments. This image shows you what you will be greeted\n    with:\n</p>\n\t<p style=\"text-align:center;\">\n\t\t<img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/history-en.png\"\n\t\t     alt=\"History\"/>\n\t</p>\n\t<p>This table shows all the transactions that have been proposed for early payment, it also tells you the state of\n    the transactions.</p>\n\t<ul>\n\t\t<li>The Fundable Value column indicates the amount from which the discount was calculated,</li>\n\t\t<li>The value column indicates the amount paid to the supplier,</li>\n\t\t<li>The cost column tells you the amount of discount,</li>\n\t\t<li>The buyer payment date corresponds to the initial due date,</li>\n\t\t<li>The Funder payment date corresponds to the date which the supplier has opted for the funding to be paid.\n    </li>\n\t</ul>\n\t<p>It is possible to view the details of the offers by clicking the &ldquo;View&rdquo; button on the row\n    you&rsquo;re\n    interested in.</p>\n\t<p>Throughout the portal, the data is fully exportable, in .csv or .xlsx format. This can be don by clicking the\n    &ldquo;Export&rdquo; button at the top of the grid.</p>\n\t<p>\n\t\t<img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/exportbutton-en.png\"\n\t\t     alt=\"Export\"/>\n\t</p>\n\t<h2>5. Payments</h1>\n\t<p style=\"text-align:center;\">\n\t\t<img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/payments-en.png\"\n\t\t     alt=\"Payments\"\n\t\t     width=\"1341\"\n\t\t     height=\"216\"/>\n\t</p>\n\t<p>On this page you can see all invoices that have been marked as paid in the system. It should be noted that the\n    amount\n    corresponds to the initial amount of the transaction it is associated to. It excludes any application of\n    discounts.\n</p>\n\t<h3>5.1 Payment Details</h2>\n\t<p style=\"text-align:center;\">\n\t\t<img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/paymentdetails-en.png\"\n\t\t     alt=\"Payment Details\"\n\t\t     width=\"1212\"\n\t\t     height=\"291\"/>\n\t</p>\n\t<p>You can expand the row in the grid by clicking the arrow at the start of it. It tells you all the information\n    about\n    the payment and shows any associated invoices or credit notes.</p>\n\t<h2>6. Summary</h1>\n\t<p>The summary pages contain several graphs that provide us with a quick analysis of the transactional data. There\n    are 3\n    different charts:</p>\n\t<p style=\"text-align:center;\">\n\t\t<img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/summary-en.png\"\n\t\t     alt=\"Summary\"/>\n\t</p>\n\t<ol>\n\t\t<li>This graph shows the number of active transactions per month,</li>\n\t\t<li>Show the total monthly value, by invoice date,</li>\n\t\t<li>Shows the active supplier companies in the portal.</li>\n\t</ol>\n</div>"
    },
    {
      "CultureId": "fr-FR",
      "Name": "body",
      "Html": "<div class=\"documentation\"><h2>1. Connexion</h2><h3>1.1 Utilisateur existant\n</h3><p>L&rsquo;utilisateur d&eacute;j&agrave; inscrit est invit&eacute; &agrave; renseigner son nom et son mot de passe.</p><p>&nbsp;</p><p style=\"text-align:center;\"><img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/loginpage-fr.png\" alt=\"Login Page\" /><br /></p><h3>1.2 Nouvel utilisateur</h3><p>Il est possible &eacute;galement de s&rsquo;inscrire &agrave; partir de cette page en cliquant sur\n    La fen&ecirc;tre suivante s&rsquo;ouvre alors.</p><p style=\"text-align:center;\"><img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/registrationform-fr.png\" alt=\"Registration Form\" /></p><p>Les &eacute;l&eacute;ments suivants doivent &ecirc;tre fournis : </p><ul><li>Pr&eacute;nom et nom</li><li>Email</li><li>Mot de passe</li><li>Confirmation du mot de passe</li><li>Il faut &eacute;galement valider les conditions d&rsquo;utilisation et la politique de protection des\n        donn&eacute;esEt cliquer sur&nbsp;<img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/submitbutton-fr.png\" alt=\"Submit\" />&nbsp;afin qu&rsquo;un mail de confirmation soit envoy&eacute;.</li></ul><h3>1.3 Ajout d&rsquo;un compte utilisateur</h3><p>Il est possible ensuite d&rsquo;ajouter un utilisateur depuis le portail en se rendant sur l&rsquo;onglet &laquo;\n    adh&eacute;sion fournisseurs &raquo;.</p><p style=\"text-align:center;\"><img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/useronboarding-fr.png\" alt=\"User Onboarding\" /></p><p>Cliquer ensuite sur &laquo; nouveau &raquo;, saisir les informations du contact et valider.</p><p style=\"text-align:center;\"><img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/useronboardingform-fr.png\" alt=\"User Onboarding Form\" /></p><p><strong>NB : Seul le premier onglet, les coordonn&eacute;es du contact, est &agrave; renseigner.</strong></p><h2>2. Accueil</h2><h3>2.1&nbsp;Page d&rsquo;accueil</h3><ul><li>Lors de la connexion au portail, le premier &eacute;cran &agrave; appara&icirc;tre est la page d&rsquo;accueil.\n        Elle\n        comporte trois grilles diff&eacute;rentes et un compteur temps\n        (cf. copie &eacute;cran page suivante).<br /></li></ul><ol><li><strong>Le montant des transactioans actives selon le statut attribu&eacute; chez BEL (approuv&eacute;e, en\n            attente, pay&eacute;e ou annul&eacute;e)</strong> fournit une analyse rapide\n        de toutes les factures approuv&eacute;es au sein de BEL. Ce graphique permet &eacute;galement aux utilisateurs\n        d'appliquer des filtres sur la devise des\n        factures que vous souhaitez consulter</li><li><strong>Le montant de l&rsquo;encours net par date d&rsquo;&eacute;ch&eacute;ance</strong> fournit le montant\n        total des factures et avoirs par date d&rsquo;&eacute;ch&eacute;ance.</li><li><strong>La prochaine Heure Limite </strong>signale la date limite pour demander un r&egrave;glement\n        anticip&eacute; avant la prochaine &eacute;ch&eacute;ance. Une date limite signifie\n        que le fournisseur doit soumettre une demande de paiement anticip&eacute; de ses factures approuv&eacute;es\n        avant cette date s&rsquo;il souhaite un paiement\n        anticip&eacute; contre escompte. Voir &eacute;galement Financement (section 3) pour plus de d&eacute;tails sur\n        la fa&ccedil;on de soumettre des factures pour paiement\n        anticip&eacute;.</li><li><strong>Le compteur financement</strong> indique, &agrave; date, le montant et le nombre d&rsquo;offres\n        disponibles &eacute;ligibles au financement, le montant et le nombre\n        d&rsquo;offres d&eacute;j&agrave; accept&eacute;es et le montant et le nombre d&rsquo;offres rejet&eacute;es.\n        Les montants correspondent au montant initial de la transaction,\n        escompte non d&eacute;duit.</li><li><strong>Pr&eacute;visions des &eacute;ch&eacute;ances futures sur la base des factures\n            valid&eacute;es</strong><strong></strong></li></ol><p>&nbsp;</p><h1 style=\"font-size:42px;\"><ul style=\"font-weight:400;\"><li><img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/homepage-fr.png\" alt=\"Home Page\" width=\"1587\" style=\"display:block;margin-left:auto;margin-right:auto;\" height=\"780\" /></li><li></li></ul></h1><p>&nbsp;</p><p>Choix de langue par d&eacute;faut\n</p><p>Il est possible de d&eacute;finir la langue par d&eacute;faut pour chaque utilisateur\n</p><p>Cliquez sur votre identifiant en haut &agrave; droite - La fen&ecirc;tre contextuelle ci-dessous s&rsquo;affiche.</p><p><img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/userprofile-fr.png\" alt=\"User Profile\" width=\"1750\" height=\"554\" style=\"display:block;margin-left:auto;margin-right:auto;\" /></p><p>S&eacute;lectionnez &laquo; French &raquo; dans le menu d&eacute;roulant &laquo; Culture &raquo; et cliquez sur\n    &laquo; update &raquo; pour valider</p><p><em></em><em></em><em></em></p><h2>3. Transactions<em></em></h2><p>L&rsquo;onglet &laquo; Transactions &raquo; affiche toutes les factures et notes de d&eacute;bit/avoirs &laquo; en\n    cours &raquo;.</p><p style=\"text-align:center;\"><img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/transactions-fr.png\" alt=\"Transactions \" /></p><p><strong>Valeur des factures, par mois: </strong>ce graphique repr&eacute;sente les mois sur la ligne horizontale et\n    la valeur des factures sur la ligne verticale o&ugrave; chaque\n    colonne indique la valeur totale des transactions enregistr&eacute;es pour les 12 derniers mois. Les couleurs de la\n    colonne indiquent le type de devise\n    pour chaque valeur totale. En survolant la colonne, on voit la valeur exacte de la transaction pour le mois\n    s&eacute;lectionn&eacute;. La date utilis&eacute;e pour\n    regrouper les factures par mois est la date de facture.</p><p><strong>Valeur des avoirs, par mois:</strong> ce graphique repr&eacute;sente les mois sur la ligne horizontale et la\n    valeur nominale des cr&eacute;dits sur la ligne verticale. Le\n    graphique fournit la valeur nominale totale des cr&eacute;dits soumis au portail par mois o&ugrave; chaque colonne\n    est le total de la valeur faciale pour ce\n    mois, et la couleur indique la devise.</p><h3>3.1 Transactions en cours</h3><p>Cliquez sur l'en-t&ecirc;te &laquo; Transactions &raquo; puis &laquo;Transactions en cours &raquo; dans le menu\n    d&eacute;roulant pour afficher toutes les transactions en cours.\n    Cette page fournit des d&eacute;tails sur toutes les transactions d&rsquo;un fournisseur envoy&eacute;es au portail.\n    Cellesci sont susceptibles d&rsquo;&ecirc;tre\n    &eacute;ligibles au financement et ont un statut &laquo; approuv&eacute; &raquo; ou &laquo; en attente &raquo;. </p><p style=\"text-align:center;\"><img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/activetransactions-fr.png\" alt=\"Active Transactions\" /></p><p><em>&nbsp;</em></p><h3>3.2 Comment afficher le d&eacute;tail d&rsquo;une facture ou d&rsquo;un avoir</h3><p>Vous pouvez afficher le d&eacute;tail d&rsquo;une facture ou d&rsquo;un avoir &agrave; partir du tableau de la liste\n    des transactions en cours en d&eacute;veloppant la transaction\n    s&eacute;lectionn&eacute;e en cliquant sur la fl&egrave;che en d&eacute;but de ligne. Vous trouverez ici plus de\n    d&eacute;tails sur la transaction s&eacute;lectionn&eacute;e, y compris un lien vers\n    les avoirs associ&eacute;s et/ou le cas &eacute;ch&eacute;ant, le r&egrave;glement anticip&eacute;</p><p style=\"text-align:center;\"><img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/invoicedetails-fr.png\" alt=\"Invoice Details\" width=\"1714\" height=\"387\" /></p><p>A gauche de l&rsquo;&eacute;cran:</p><ol><li>Le premier cadre en haut donne l&rsquo;ensemble des informations sur le document : Num&eacute;ro, montant,\n        fournisseur, date de cr&eacute;ation /date\n        de r&eacute;ception des donn&eacute;es dans le portail, date d&rsquo;&eacute;ch&eacute;ance&hellip;</li><li>Le deuxi&egrave;me cadre en-dessous donne, le cas &eacute;ch&eacute;ant, les informations relatives au\n        financement de la transaction,</li><li>Le cadre en bas relate l&rsquo;historique du document, c&rsquo;est-&agrave;-dire l&rsquo;ensemble des actions /\n        changements de statuts notamment subis par le\n        document depuis son import sur le portail.</li></ol><p>A droite de l&rsquo;&eacute;cran:</p><ol><li>Le cadre en haut &agrave; indique la r&eacute;f&eacute;rence unique de la transaction (information\n        syst&egrave;me),</li><li>Le deuxi&egrave;me tableau donne le d&eacute;tail des lignes du document en pr&eacute;cisant les lignes\n        finan&ccedil;ables ou non,</li><li>Le dernier tableau en bas indique les parties prenantes de la transaction.<br /></li></ol><p>&nbsp;</p><h2>4. R&egrave;glements anticip&eacute;s\n</h2><h3>4.1 Accepter les propositions &ndash; Mode &laquo; A LA CARTE &raquo;</h3><p>Dans le menu &laquo; R&egrave;glements anticip&eacute;s &raquo;, choisissez &laquo; Accepter les propositions\n    &raquo;. L&rsquo;&eacute;cran qui s&rsquo;affiche est le suivant:</p><p style=\"text-align:center;\"><img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/proposedoffers-fr.png\" alt=\"Proposed Offers\" width=\"1630\" height=\"690\" /></p><ol><li><strong>La prochaine Heure Limite</strong> signale la date limite pour demander un r&egrave;glement\n        anticip&eacute; avant la prochaine &eacute;ch&eacute;ance. Une date limite\n        signifie que le fournisseur doit soumettre une demande de paiement anticip&eacute; de ses factures\n        approuv&eacute;es avant cette date s&rsquo;il souhaite\n        un paiement anticip&eacute; contre escompte.</li><li><strong>Le compteur financement indique</strong>, &agrave; date, le montant - hors escompte - et le nombre\n        d&rsquo;offres disponibles &eacute;ligibles au financement, le\n        montant et le nombre d&rsquo;offres d&eacute;j&agrave; accept&eacute;es et le montant et le nombre\n        d&rsquo;offres rejet&eacute;es.</li><li>Possibilit&eacute; de s&eacute;lectionner individuellement les propositions ou l&rsquo;ensemble des factures\n        affich&eacute;es sur l&rsquo;&eacute;cran (page en cours)</li><li><strong>Fonctionnalit&eacute; recommand&eacute;e : Renseigner un montant de r&egrave;glement anticip&eacute;\n            souhait&eacute;</strong> dans la case &laquo; <strong>S&eacute;lectionnez jusqu&rsquo;&agrave; (montant\n            en &euro;</strong><strong>)</strong> &raquo;. Le portail s&eacute;lectionnera alors automatiquement les\n        offres, en fonction de la date de facture jusqu&rsquo;&agrave; concurrence du montant\n        indiqu&eacute; (cf. page 18 pour un montant de 40 000 euros).</li></ol><p>Il suffit ensuite d<strong>&rsquo;accepter la s&eacute;lection</strong> pour que les offres se retrouvent dans\n    l&rsquo;onglet &laquo; offres accept&eacute;es &raquo; (cf. page 19). Il est possible\n    &eacute;galement de s&eacute;lectionner l&rsquo;int&eacute;gralit&eacute; des offres &eacute;ligibles au financement\n    en une seule fois sans qu&rsquo;il soit n&eacute;cessaire de les s&eacute;lectionner\n    au pr&eacute;alable -&gt; dans ce cas cliquez sur &laquo; Accepter l&rsquo;int&eacute;gralit&eacute; &raquo;</p><table style=\"height:41.7031px;width:807px;\"><tbody><tr style=\"height:100%;\"><td style=\"width:100%;\"><strong><em>NB1 : Le fait de cliquer sur &laquo; Accepter\n                        l&rsquo;int&eacute;gralit&eacute; &raquo; ou &laquo; Accepter la s&eacute;lection &raquo; rend\n                        ce choix d&eacute;finitif, il ne sera pas possible de le corriger\n                        ensuite.</em></strong>\n </td></tr></tbody></table><p>NB2: <em>Les montants affich&eacute;s et notamment les montants des offres sont exprim&eacute;s en TTC.</em></p><p>NB3: <em>Les factures sont s&eacute;lectionn&eacute;es, par d&eacute;faut, en fonction de leur date\n        d&rsquo;&eacute;mission - de la plus ancienne &agrave; la plus r&eacute;cente - et de leur montant,\n        par ordre d&eacute;croissant de valeur. Si un autre filtre est activ&eacute;, la s&eacute;lection sera mise\n        &agrave; jour en prenant en compte ce filtre suppl&eacute;mentaire</em></p><p><strong><em>NB4 : Il est possible de s&eacute;lectionner jusqu&rsquo;&agrave; 1 000 offres au maximum en une fois. Si\n            le montant maximum de financement n&rsquo;est pas atteint, il\n            est alors n&eacute;cessaire d&rsquo;accepter la premi&egrave;re s&eacute;lection et d&rsquo;en op&eacute;rer\n            une seconde en tenant &eacute;videmment compte du premier montant de\n            financement atteint lors de la premi&egrave;re s&eacute;lection des offres parmi les 1 000\n            premi&egrave;res</em></strong></p><p>NB5 : <em>Il est possible d&rsquo;ajuster le montant propos&eacute; initialement en cochant ou d&eacute;cochant\n        manuellement les offres</em></p><p style=\"text-align:center;\"><strong><em>&nbsp;</em></strong></p><p style=\"text-align:center;\"><img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/acceptedoffers-fr.png\" alt=\"Accepted Offers\" /></p><img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/acceptedoffers-fr.png\" alt=\"Accepted Offers\" /><p style=\"text-align:center;\"><strong><em>Une fois les offres accept&eacute;es, elles sont positionn&eacute;es dans\n            l&rsquo;onglet &laquo; Offres accept&eacute;es &raquo;</em></strong></p><h3>4.2&nbsp;Historique des propositions\n</h3><p>Dans le menu &laquo; Financement &raquo;, choisissez &laquo; Historique des propositions &raquo;.\n    L&rsquo;&eacute;cran qui s&rsquo;affiche est le suivant:<br /></p><p style=\"text-align:center;\"><img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/history-fr.png\" alt=\"History\" /></p><p>Le tableau pr&eacute;sente l&rsquo;ensemble des transactions qui ont &eacute;t&eacute; propos&eacute;es au\n    r&egrave;glement anticip&eacute; et leur statut.</p><ul><li>La colonne &laquo; valeur finan&ccedil;able &raquo; indique le montant &agrave; partir duquel l&rsquo;escompte a\n        &eacute;t&eacute; calcul&eacute;,</li><li>La colonne &laquo; montant &raquo; indique le montant pay&eacute; au fournisseur,</li><li>La colonne &laquo; co&ucirc;ts &raquo; indique le montant de l&rsquo;escompte,</li><li>La &laquo; date de paiement client &raquo; correspond &agrave; la date d&rsquo;&eacute;ch&eacute;ance initiale,\n    </li><li>La &laquo; date de paiement anticip&eacute; &raquo; correspond &agrave; la date &agrave; laquelle le fournisseur\n        ayant opt&eacute; pour le financement sera r&eacute;gl&eacute;.\n    </li></ul><p>Il est possible de consulter le d&eacute;tail de l&rsquo;op&eacute;ration en cliquant sur &laquo; vue &raquo; en fin\n    de ligne.</p><ul><li>Enfin, comme cela est possible sur l&rsquo;ensemble du portail, les donn&eacute;es sont totalement exportables,\n        notamment au format .csv ou .xlsx. via la\n        fonctionnalit&eacute; &laquo; Export &raquo;.</li></ul><p><img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/exportbutton-fr.png\" alt=\"Export Button\" /></p><h2>5. R&egrave;glements</h2><p style=\"text-align:center;\"><img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/payments-fr.png\" alt=\"Payments\" /></p><table><tbody><tr style=\"height:100%;\"><td style=\"width:100%;\"><strong>Sur cette page, vous pouvez consulter toutes les factures qui sont\n                    r&eacute;gl&eacute;es par BEL. Il convient d&rsquo;indiquer que le montant correspond au\n                    montant initial de la facture / note de d&eacute;bit, hors application de l&rsquo;escompte. Il est\n                    possible &eacute;galement d&rsquo;exporter les r&egrave;glements r&eacute;alis&eacute;s sous\n                    Microsoft Excel.</strong></td></tr></tbody></table><h3>5.1 D&eacute;tail d&rsquo;un r&egrave;glement</h3><p style=\"text-align:center;\"><img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/paymentdetails-fr.png\" alt=\"Payment Details\" width=\"1691\" height=\"364\" /></p><p>Cliquer sur la fl&egrave;che en d&eacute;but de ligne permet d&rsquo;obtenir l&rsquo;ensemble des informations\n    relatives &agrave; un r&egrave;glement, notamment la liste des factures\n    et avoirs concern&eacute;s par le paiement s&eacute;lectionn&eacute;.</p><h2>6. R&eacute;sum&eacute;</h2><p>Les pages du r&eacute;sum&eacute; contiennent plusieurs graphiques qui fournissent une analyse rapide des\n    donn&eacute;es transactionnelles et un aper&ccedil;u de la\n    tendance d'int&eacute;gration du portail fournisseur dans votre organisation. Il existe, &agrave; ce jour, trois\n    graphiques diff&eacute;rents:</p><p style=\"text-align:center;\"><img src=\"[app[root]]Api/DMS/Content/documentation/standarduserguide/summary-fr.png\" alt=\"Summary\" width=\"1644\" height=\"395\" /></p><ul><li><strong>Nombre de transaction, par date de facturation</strong>: Ce graphique pr&eacute;sente le nombre de\n        transactions actives par mois,</li><li><strong>Valeur totale mensuelle des factures, par date de facturation</strong>: Ce graphique pr&eacute;sente le\n        montant mensuel de facturation,</li><li>Soci&eacute;t&eacute;s fournisseurs actives, par date d&rsquo;adh&eacute;sion: Ce graphique montre le nombre de\n        fournisseurs actifs (en l&rsquo;occurrence 1 lors de la\n        consultation par un utilisateur fournisseur)<strong></strong></li><li><strong></strong></li></ul><h2>7. Glossaire</h2><p>Plusieurs statuts peuvent s&rsquo;afficher dans le portail, notamment dans les menus d&eacute;roulants. Pour autant,\n    dans le cadre des transactions r&eacute;alis&eacute;es\n    avec BEL, les seuls statuts utilis&eacute;s sont les suivants:</p><p>STATUT FACTURE / AVOIR / NOTE DE D&Eacute;BIT</p><table style=\"width:861px;margin-left:auto;margin-right:auto;\"><tbody><tr style=\"height:25%;\"><td style=\"width:21.7531%;text-align:center;\">AFFICHAGE</td><td style=\"width:78.4165%;text-align:center;\">SIGNIFICATION</td></tr><tr style=\"height:25%;\"><td style=\"width:21.7531%;text-align:center;\">Approuv&eacute;(e)</td><td style=\"width:78.4165%;text-align:center;\">Valid&eacute;(e)</td></tr><tr style=\"height:25%;\"><td style=\"width:21.7531%;text-align:center;\">En attente</td><td style=\"width:78.4165%;text-align:center;\">En attente d'une confirmation</td></tr><tr style=\"height:25%;\"><td style=\"width:21.7531%;text-align:center;\">Pay&eacute;(e)</td><td style=\"width:78.4165%;text-align:center;\">R&eacute;gl&eacute;(e)</td></tr><tr style=\"height:25%;\"><td style=\"width:21.7531%;text-align:center;\">Annul&eacute;(e)</td><td style=\"width:78.4165%;text-align:center;\">Annul&eacute;(e)\n </td></tr></tbody></table><p>STATUT OFFRE</p><table style=\"height:65.1094px;margin-left:auto;margin-right:auto;width:866px;\"><tbody><tr style=\"height:32.3013%;\"><td style=\"width:22.6012%;text-align:center;\">AFFICHAGE</td><td style=\"width:77.6301%;text-align:center;\">SIGNIFICATION\n </td></tr><tr style=\"height:35.3973%;\"><td style=\"width:22.6012%;text-align:center;\">Propos&eacute;(e)</td><td style=\"width:77.6301%;text-align:center;\">SIGNIFICATION\n Propos&eacute;(e) Facture valid&eacute;e, envoy&eacute;(e) au portail et propos&eacute;e au financement\n            </td></tr><tr style=\"height:32.3013%;\"><td style=\"width:22.6012%;text-align:center;\">Paiement anticip&eacute; accept&eacute; </td><td style=\"width:77.6301%;text-align:center;\">Proposition de financement accept&eacute;e : Facture\n                s&eacute;lectionn&eacute;e par le\n                Fournisseur pour paiement anticip&eacute; contre escompte</td></tr></tbody></table></div>"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "A brief guide on how to use our portals",
      "Title": "Standard User Guide"
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
  "Path": "Documentation/CoreDocumentation/AppManagement/Configuration",
  "Name": "Configuration",
  "ResourceKey": "",
  "ShowOnMenus": true,
  "Order": 0,
  "LastUpdated": "2024-08-22T12:04:12.1712225+01:00",
  "Layout": "Documentation",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "<div class=\"documentation\"><h2>Accessing Configuration Management </h2><p class=\"mainText\">You Access this page through the App Management Tabs. From here you can manage many different\n        things, for example the deployment targets - which determines where you can migrate things like templates, pages\n        and layouts to. It is just a blob of JSON which can control a lot of different things.</p><h2>The JSON Blob </h2><h3>Deployment </h3><pre>\"Deployment\":&nbsp;{\n&nbsp;&nbsp;&nbsp;&nbsp;\"Targets\":&nbsp;[\n&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{\n&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"Api\":&nbsp;\"https://demo.staging.corporatelinx.com/Api/\",\n&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"Domain\":&nbsp;\"demo.staging.corporatelinx.com\",\n&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"Name\":&nbsp;\"Demo&nbsp;(Staging)\",\n&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"EnvironmentName\":&nbsp;\"Staging\"\n&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}\n&nbsp;&nbsp;&nbsp;&nbsp;],\n&nbsp;&nbsp;&nbsp;&nbsp;\"DMS\":&nbsp;[\n&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"Content\",\n&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"Content/Slideshow\"\n&nbsp;&nbsp;&nbsp;&nbsp;]\n&nbsp;&nbsp;},\n    </pre><p class=\"mainText\">Here is where you set up the targets you wish to be able to migrate to from the app that you&rsquo;re\n        currently in <strong>&ldquo;Api&rdquo;</strong> is the Api you wish to communicate with, it&rsquo;s a URL to one of our apps in\n        any environment\n        followed by &ldquo;/Api/. <strong>&ldquo;Domain&rdquo; </strong>is the domain of the app you want to migrate to,\n        <strong>&ldquo;Name&rdquo;</strong> is how it will appear in\n        the drop down to select where you want to migrate to. <strong>&ldquo;Environment&rdquo;</strong> can be Dev, Test, Staging\n        or Production.\n        This is just an indicator of which environment your migration will be pushed to.\n    </p><p>The <strong>&ldquo;DMS&rdquo;</strong>array determines what folders will be migrated when you select DMS in the migration\n        dialog. This is\n        particularly handy if you&rsquo;re wanting to migrate their logos, or their log in page files.</p><h3>B2B</h3><pre>&nbsp;&nbsp;\"B2B\":&nbsp;{\n    &nbsp;&nbsp;&nbsp;&nbsp;\"SourceSystem\":&nbsp;\"Demo\",\n    &nbsp;&nbsp;&nbsp;&nbsp;\"TransactionSource\":&nbsp;\"Demo\",\n    &nbsp;&nbsp;&nbsp;&nbsp;\"Currencies\":&nbsp;[\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"EUR\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"GBP\"\n    &nbsp;&nbsp;&nbsp;&nbsp;],\n    &nbsp;&nbsp;&nbsp;&nbsp;\"DefaultCurrency\":&nbsp;\"EUR\",\n    &nbsp;&nbsp;&nbsp;&nbsp;\"RootBucket\":&nbsp;\"a4b5cc45-b36d-474c-9d7c-c8462859e5d0\",\n    &nbsp;&nbsp;&nbsp;&nbsp;\"ExpiryTime\":&nbsp;\"23:59\",\n    &nbsp;&nbsp;&nbsp;&nbsp;\"References\":&nbsp;{\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"Systems\":&nbsp;[\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"GlobalTax\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"Demo\"\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;]\n    &nbsp;&nbsp;&nbsp;&nbsp;}\n    &nbsp;&nbsp;},\n    </pre><p class=\"mainText\">This object is where you set up important things for the importation of Transactions into our\n        system.\n        <strong>&ldquo;Source\n System&rdquo;</strong> is the system in which the transactions sit, similar to the <strong>&ldquo;Transaction\n Source&rdquo;</strong>. <br /><br />The <strong>&ldquo;Currencies&rdquo;</strong> array determines what currencies will be accepted in the system when\n        transactions are imported\n        &amp; this determines whether or not charts will render correctly, if transactions are imported with a currency that\n        is missing from this array, then they will not be visible in the charts.\n        <br /><br /><strong>&ldquo;Default Currency&rdquo;</strong> sets the currency that the vast majority of Transactions will be in.\n        <br /><br /><strong>&ldquo;Root Bucket&rdquo;</strong> this is the ID of the bucket where all Transactions will &ldquo;live&rdquo; and how we link\n        the transaction to\n        the particular app that you&rsquo;re in.\n        <br /><br /><strong>&ldquo;Expiry Time&rdquo;</strong> is the time at which any early payment offers expire, here it is set in GMT, so\n        all offers will\n        expire at 23:59 GMT.\n        <br /><br /><strong>&ldquo;References&rdquo; </strong>are the references that are accepted by our system in this particular app, the\n        GlobalTax reference\n        is the Tax reference for the company you&rsquo;re importing and your unique company reference will usually be denoted\n        as &ldquo;Demo|Reference&rdquo; in our system and may be displayed as such in our UI.\n    </p><h3>Components </h3><pre>&nbsp;&nbsp;\"Components\":&nbsp;{\n    &nbsp;&nbsp;&nbsp;&nbsp;\"Grids\":&nbsp;{\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"DefaultPageSize\":&nbsp;40,\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"Details\":&nbsp;\"Expand,Link\"\n    &nbsp;&nbsp;&nbsp;&nbsp;}\n    },\n    </pre><p class=\"mainText\">This defines key important features of grids in the UI, like the default page sizes, which\n        ensures that there&rsquo;s\n        not an overwhelming amount of data being displayed in them. The <strong>&ldquo;Details&rdquo;</strong> indicates that they\n        can be viewed\n        either by using a link within the table or through being expanded in the grid. </p><h3>Calendars </h3><pre>    \"Calendars\":&nbsp;{\n        &nbsp;&nbsp;&nbsp;&nbsp;\"primary\":&nbsp;\"37\",\n        &nbsp;&nbsp;&nbsp;&nbsp;\"PaymentEventName\":&nbsp;\"Payment\",\n        &nbsp;&nbsp;&nbsp;&nbsp;\"CutoffEventName\":&nbsp;\"Cutoff\"\n        },\n    </pre><p class=\"mainText\">This object determines what calendar will be used within the app by setting\n        <strong>&ldquo;primary&rdquo;</strong> to the ID\n        of the preferred calendar. It also sets up the possible events that can be set up within the calendar. The most\n        common events are payment events and Cut Off events. What these names are set to, determine what they must be\n        created as when setting up the calendar using the UI.\n    </p><h3>Themes</h3><p class=\"mainText\">This object is what defines a lot of the customisable theming options, like what colours the\n        clients would prefer to use, what they would like their borders to look like etc. </p><h4>Default</h4><pre>&nbsp;&nbsp;&nbsp;&nbsp;\"Default\":&nbsp;{\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"paintLoginMid\":&nbsp;false,\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"margins\":&nbsp;\"4px\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"paintLoginBottom\":&nbsp;false,\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"shadows\":&nbsp;\"2px&nbsp;2px&nbsp;5px&nbsp;#333\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"colours\":&nbsp;{\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"primary\":&nbsp;\"#2F4A74\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"secondary\":&nbsp;\"#ED8900\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"background\":&nbsp;\"white\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"text\":&nbsp;\"#222\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"text2\":&nbsp;\"#FFFFFF\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"links\":&nbsp;\"#214A71\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"error\":&nbsp;\"red\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"charts\":&nbsp;[\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"#2F4A74\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"#ED8900\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"#2F4A74\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"#ED8900\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"#2F4A74\"\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;]\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;},\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"font\":&nbsp;{\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"size\":&nbsp;\"11px\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"family\":&nbsp;\"Quicksand,&nbsp;sans-serif\"\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;},\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"border\":&nbsp;{\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"style\":&nbsp;\"solid&nbsp;1px&nbsp;#ccc\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"width\":&nbsp;\"1px\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"radius\":&nbsp;0\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;},\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"notifications\":&nbsp;{\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"error\":&nbsp;{\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"text\":&nbsp;\"#222\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"background\":&nbsp;\"#FFECEC\"\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;},\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"warning\":&nbsp;{\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"text\":&nbsp;\"#222\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"background\":&nbsp;\"#FFF4D9\"\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;},\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"info\":&nbsp;{\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"text\":&nbsp;\"#222\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"background\":&nbsp;\"#E5F5FA\"\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;},\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"success\":&nbsp;{\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"text\":&nbsp;\"#222\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"background\":&nbsp;\"#EAF7EC\"\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}\n    &nbsp;&nbsp;&nbsp;&nbsp;},\n    </pre><table><tbody><tr><td>paintLoginMid </td><td>This is what determines whether the app&rsquo;s login page is displayed in the centre, if it is set to\n                    false then it will not be.</td></tr><tr><td>PaintLoginBottom </td><td>This is what determines whether the app&rsquo;s login page is displayed at the bottom, when it is set to\n                    true it will sit above the footer. </td></tr><tr><td>Margins </td><td>This sets the default margins which are used by all components, it ensures that all white space\n                    between them are equal. </td></tr><tr><td>Colours - Primary </td><td>This is one of two main colours used across the portals, it is specified by the client and should\n                    fit with their logo &amp; be provided to us to ensure that the portal meets their expectations. </td></tr><tr><td>Colours - Secondary</td><td>This is one of two main colours used across the portals, it is specified by the client and should\n                    fit with their logo &amp; be provided to us to ensure that the portal meets their expectations. </td></tr><tr><td>Colours - Background </td><td>This is the background colour of the portal, it is generally set to white. </td></tr><tr><td>Colours - Text </td><td>This is the colour of the main body text, the colour of the vast majority of text within the app.\n                </td></tr><tr><td>Colours - Text2 </td><td>This is the colour of the second most used style of text. </td></tr><tr><td>Colours - Links </td><td>This is the colour that any links across the portal will be displayed in. </td></tr><tr><td>Colours - Error </td><td>Determines the colour of any errors that appear across the portals. </td></tr><tr><td>Colours - Charts </td><td>This array sets the colours of any charts, they need to all be unique colours so that all data can\n                    be visualised in the portal correctly. </td></tr><tr><td>Font - Size </td><td>This determines the size of all text across the portal.</td></tr><tr><td>Font - Family </td><td>Sets the font of all text across the portal. </td></tr><tr><td>Border - Style </td><td>This sets up a default style for the borders of components and anything else requiring one, you can\n                    set the colour and whether you want it dotted or solid (as an example).</td></tr><tr><td>Border - Width </td><td>Sets the default border width. </td></tr><tr><td>Border - Radius </td><td>This sets the border radius which determines how round the corners will be displayed.</td></tr><tr><td>Notifications - Errors </td><td>You use this to set up the default notifications to be used on the app, typically error messages\n                    appear in red.</td></tr><tr><td>Notifications - Warning </td><td>You use this to set up the default notifications to be used on the app, typically Warning messages\n                    appear in yellow.</td></tr><tr><td>Notifications - Info </td><td>You use this to set up the default notifications to be used on the app, typically info messages\n                    appear in blue.</td></tr><tr><td>Notifications - Success </td><td>You use this to set up the default notifications to be used on the app, typically success messages\n                    appear in green.</td></tr></tbody></table><h4>Dark</h4><pre>&nbsp;&nbsp;&nbsp;&nbsp;\"Dark\":&nbsp;{\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"colours\":&nbsp;{\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"primary\":&nbsp;\"#193855\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"secondary\":&nbsp;\"#E2721D\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"background\":&nbsp;\"#36393F\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"text\":&nbsp;\"#222\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"text2\":&nbsp;\"white\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"links\":&nbsp;\"#E2721D\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"error\":&nbsp;\"red\"\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;},\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"font\":&nbsp;{\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"size\":&nbsp;\"11px\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"family\":&nbsp;\"Verdana,&nbsp;Arial,&nbsp;'Segoe&nbsp;UI',&nbsp;Tahoma,&nbsp;Geneva,&nbsp;sans-serif\"\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;},\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"border\":&nbsp;{\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"style\":&nbsp;\"solid&nbsp;1px&nbsp;white\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"width\":&nbsp;\"1px\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"radius\":&nbsp;0\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;},\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"notifications\":&nbsp;{\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"error\":&nbsp;{\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"text\":&nbsp;\"#222\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"background\":&nbsp;\"#FFECEC\"\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;},\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"warning\":&nbsp;{\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"text\":&nbsp;\"#222\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"background\":&nbsp;\"#FFF4D9\"\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;},\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"info\":&nbsp;{\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"text\":&nbsp;\"#222\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"background\":&nbsp;\"#E5F5FA\"\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;},\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"success\":&nbsp;{\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"text\":&nbsp;\"#222\",\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"background\":&nbsp;\"#EAF7EC\"\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;},\n    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"shadows\":&nbsp;\"2px&nbsp;2px&nbsp;5px&nbsp;#333\"\n    &nbsp;&nbsp;&nbsp;&nbsp;}\n    </pre><p class=\"mainText\">This is where any custom dark theme would be set up, things that would be changed here are the\n        colours to ensure that it&rsquo;s just that little bit easier on the eyes and isn&rsquo;t so glaring as black text on a\n        white background. This is particularly useful when you have a sensitivity to bright lights. Please see above\n        table of the explanations if you&rsquo;re unsure of what changes you need to make here</p></div>"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "How to configure deployment targets, B2B transactions, UI components, calendars, and themes, follow this how-to guide using JSON.",
      "Title": "Configuration"
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
  "Path": "Documentation/CoreDocumentation/AppManagement/Theming",
  "Name": "Theming",
  "ShowOnMenus": false,
  "Order": 0,
  "LastUpdated": "2024-08-22T12:04:12.1832828+01:00",
  "Layout": "Documentation",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "<div class=\"documentation\"><h2>Accessing Theme Management </h2><p class=\"mainText\">You Access this page through the App Management Tabs. From here you can manage what the current\n        app&rsquo;s theme, this includes the logos that are displayed in the site&rsquo;s header and the colours that are used\n        throughout the application.</p><p class=\"mainText\">&nbsp;</p><h2>The UI </h2><p>When you go to the <strong>&ldquo;Theming&rdquo;</strong> tab you&rsquo;re greeted with something that looks like this:</p><p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Theming/ThemeManagementUI-en.png\" /></p><p class=\"mainText\">Here, the <strong>&ldquo;Brand Logo&rdquo;</strong> is the logo that is displayed at the top left of the\n        screen and the\n        <strong>&ldquo;Project Logo&rdquo;</strong> is typically the logo of the project, for example, for this demo app this could\n        incorporate the\n        words &ldquo;Corporate LinX Demo&rdquo;. We also support having a background animation which is mostly visible on the Login\n        and Home pages of the site which can be uploaded by dragging and dropping your image from your local file\n        explorer to the upload box. You may also change the existing files in DMS by dragging and dropping a new image\n        over the previews. From this tab, you can also manage the themes that exist in the current application.</p><p class=\"mainText\">&nbsp;</p><h2>Creating a Theme </h2><p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Theming/ThemeGrid-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /></p><p class=\"mainText\">This grid is where you manage the themes that exist in the application, to create a new theme\n        you click on the <strong>&ldquo;New&rdquo;</strong> button which will bring up a dialog that looks something like this:</p><p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Theming/CreateThemeDialog-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /></p><p class=\"mainText\">Creating a new theme is simple as it only requires you to give it a name and click the\n        <strong>&ldquo;Confirm&rdquo;</strong> button. Creating a &ldquo;Test&rdquo; theme you can see that it&rsquo;s added into the grid:\n    </p><p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Theming/TestTheme-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /></p><p>&nbsp;</p><h2>Updating a Theme </h2><p class=\"mainText\">To update the theme that was just created, you click on the <strong>&ldquo;Edit&rdquo;</strong> button on\n        the relevant row,\n        this will bring up a dialog that looks something like this:</p><p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Theming/EditThemeDialog-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /></p><p>&nbsp;</p><h3>Colours </h3><p class=\"mainText\">To understand what each of these colours represent, the dialog is broken down into two\n        sections, <strong>&ldquo;Base Colours&rdquo;</strong> and <strong>&ldquo;Chart Colours&rdquo;</strong> </p><p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Theming/EditBaseColours-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /></p><p class=\"mainText\">The <strong>&ldquo;Base Colours&rdquo;</strong> section is where you set the &ldquo;main&rdquo; colours that are used\n        across the\n        application, most obviously, the <strong>&ldquo;Primary&rdquo;</strong> colour defines the colour of the header bar that\n        goes across the\n        screen and the <strong>&ldquo;Secondary&rdquo;</strong> colour defines the colour of any headings for charts or dialogs for\n        example. The\n        <strong>&ldquo;Background&rdquo;</strong> colour determines the colour of the background of the site as a whole. We allow\n        for two different\n        text colours so that they can be used in situations where the contrast maybe too low for the users reading it.\n        The\n        <strong>&ldquo;Links&rdquo;</strong> colour determines what colour any links within the portal will be displayed as and the\n        <strong>&ldquo;Margins&rdquo;</strong> sets up\n        the value for the default margins across the portal, which determines how much spaces there is between the\n        components.\n    </p><p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Theming/EditChartColours-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /></p><p class=\"mainText\">The <strong>&ldquo;Chart Colours&rdquo;</strong> section is where you set up the colours for the charts\n        across the\n        application, ideally these should all be different colours and not similar shades of the same colour as this\n        will mean that the data displayed in chart format is more difficult to read.</p><p class=\"mainText\">To change any of the colours in this section of the dialog, you click onto the dropdown that\n        corresponds to the colour you want to edit, this will bring up a dialog that looks something like this:</p><p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Theming/ColourPicker-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /></p><p class=\"mainText\">The rectangles in the top right are there for previewing the colour you currently have\n        selected. You can change the colour by either using the colour picker using your cursor, or by entering a HEX\n        colour code which is often provided to you on a style sheet.</p><h3>Font </h3><p class=\"mainText\">Clicking on the <strong>&ldquo;Font&rdquo;</strong> tab within this dialog displays something that looks\n        like this:</p><p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Theming/EditFont-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /></p><p class=\"mainText\">The <strong>&ldquo;Size&rdquo;</strong> can be selected using the slider, as you can see in screenshot the\n        size is set to 11\n        px and the <strong>&ldquo;Family&rdquo;</strong> determines what font that&rsquo;s used across the portal. </p><h3>Border </h3><p class=\"mainText\">Clicking on the <strong>&ldquo;Border&rdquo;</strong> tab within this dialog displays something that looks\n        like this:</p><p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Theming/EditBorder-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /></p><p class=\"mainText\">The <strong>&ldquo;Style&rdquo; </strong>dropdown allows a default border style to be set across the\n        portal, these borders\n        will be displayed around every component that you see. It would be recommended to keep this value low at\n        potentially 1 or 2px which you can set with the <strong>&ldquo;Width&rdquo;</strong> slider below. The\n        <strong>&ldquo;Colour&rdquo;</strong> of these borders are\n        determined by the colour picker in this dialog. The <strong>&ldquo;Radius&rdquo;</strong> sets the CSS property,\n        border-radius. this is what\n        determines how rounded the corners of the border are displayed, small values result in more pointed-looking\n        corners and higher values will make them much more rounded.\n    </p><h3>Notifications </h3><p class=\"mainText\">Clicking on the <strong>&ldquo;Notifications&rdquo;</strong> tab within this dialog displays something that\n        looks like this:</p><p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Theming/EditNotifications-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /></p><p class=\"mainText\">This section of the dialog defines what colours the notifications appear are. These can be fully\n        customised using the colour pickers within this section, it&rsquo;s important to ensure that the text colour isn&rsquo;t too\n        dark or too light in comparison to the background colour as this would mean that the text is less readable and\n        users may have issues reading the description of the error they&rsquo;ve received.</p><h3>Shadows</h3><p class=\"mainText\">Clicking on the <strong>&ldquo;Shadows&rdquo;</strong> tab within this dialog displays something that looks\n        like this:</p><p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Theming/EditShadows-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /></p><p class=\"mainText\">The <strong>&ldquo;Horizontal Offset&rdquo;</strong> slider determines how far in pixels the shadow appears\n        to the left or\n        the right, the <strong>&ldquo;Vertical Offset&rdquo;</strong> defines how far in pixels the shadow appears to the top or the\n        bottom of the\n        container. <strong>&ldquo;Blur Radius&rdquo;</strong> determines how blurry the shadow appears, setting this to 0 will mean\n        that the shadow\n        will look like a solid black border. The higher this value is set to the further the colour is diffused across.\n        The <strong>&ldquo;Colour&rdquo; </strong>of this shadow can be set to whatever you want, however if you want it to look\n        like a more\n        &ldquo;natural&rdquo; shadow then this should be set to black or a dark grey colour.</p><h3>Etc </h3><p class=\"mainText\">Clicking on the &ldquo;Etc&rdquo; tab within this dialog displays something that looks like this:</p><p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Theming/EditThemeEtc-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /></p><p class=\"mainText\">The <strong>&ldquo;Paint Login Mid&rdquo;</strong> sets the login component on the login page of the portal\n        to be displayed\n        in the middle of the screen and <strong>&ldquo;Paint Login Bottom\"</strong> sets it such that the login component is\n        displayed at the\n        bottom of the page. It is supported such that we can use a flag or an image display to display what cultures can\n        be selected within the portal, this can be changed by selecting &ldquo;Picture&rdquo; in the <strong>&ldquo;Culture Flag\n            Layout&rdquo;</strong> dropdown.\n    </p><p class=\"mainText\">To save any changes made within this dialog, you just click the <strong>&ldquo;Save&rdquo;</strong> button.\n        This will save\n        any changes to the application&rsquo;s configuration and they should be now visible in the portal.</p><p class=\"mainText\">&nbsp;</p><h2>Deleting a Theme </h2><p class=\"mainText\">You can delete a theme in the themes grid by clicking the <strong>&ldquo;Delete&rdquo;</strong> button on the row of the\n        theme you want to delete.</p><p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Theming/DeleteTheme-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /></p><p class=\"mainText\">This will delete the theme from the application&rsquo;s config and remove it from the grid.</p></div>"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Title": "Theming"
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
  "Path": "Documentation/CoreDocumentation/AppManagement/Cultures",
  "Name": "Cultures",
  "ShowOnMenus": true,
  "Order": 0,
  "LastUpdated": "2024-08-22T12:04:12.2082126+01:00",
  "Layout": "Documentation",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "<div class=\"documentation\"><h2>Accessing Culture Management</h2><p class=\"mainText\">You Access this page through the App Management Tabs. From here you can manage what Cultures\n            (languages) you wish to support in your App.</p><h2>The UI</h2><p class=\"mainText\">When you access the cultures tab, you&rsquo;re greeted with something that looks like this: </p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Cultures/CultureManagementGrid-en.png\" /><p class=\"mainText\">As you can see here, we currently only allow there to be French and English Cultures within\n            our system. If, for example you de-select French from this list, you will no longer be able to select it\n            from the top right hand corner. The following is a screenshot of the top right hand corner, where the user\n            can select the language that they want to view the page in.\n        </p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Cultures/SelectingCulture-en.png\" width=\"382\" height=\"145\" style=\"display:block;margin-left:auto;margin-right:auto;\" /></div>"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "How to select the cultures you want to support in the portal.",
      "Title": "Cultures"
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
  "Path": "Documentation/CoreDocumentation/AppManagement/Layouts",
  "Name": "Layouts",
  "ShowOnMenus": true,
  "Order": 0,
  "LastUpdated": "2024-08-22T12:04:12.232722+01:00",
  "Layout": "Documentation",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "<div class=\"documentation\"><h2>Accessing Layout Management</h2><p class=\"mainText\">You Access this page through the App Management Tabs. From here you can manage the app&rsquo;s\n            layout, where you can alter the app&rsquo;s logos and their placements and you can manage their sites navigation\n            bar styles.</p><h2>The UI</h2><p class=\"mainText\">When you access the layouts tab, you&rsquo;re greeted with something that looks like this: </p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Layouts/LayoutsGrid-en.png\" /><p class=\"mainText\">This grid contains any relevant layouts, which can be set on a page-by-page basis, for\n            example, the documentation pages you&rsquo;re looking at right now will have a different layout to the app&rsquo;s home\n            page.</p><p class=\"mainText\">&nbsp;</p><h2>Creating a Layout</h2><p class=\"mainText\">To create a new layout in this UI, you click on the <strong>&ldquo;New Layout&rdquo;</strong> button\n            in the header bar.\n            Clicking this will bring up a dialog that looks something like this:</p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Layouts/CreateLayoutDialog-en.png\" width=\"631\" height=\"154\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">Creating a Layout in our system is incredibly simple &amp; only requires you to give it a name.\n            The name should be relevant to what the layout is going to be used for. Once you&rsquo;ve filled in the field, you\n            can press create and create your new layout.\n        </p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Layouts/CreatedLayout-en.png\" /><p class=\"mainText\">For now, I have created a Test layout. On this row you can see that there are save and\n            delete functions and there&rsquo;s the ability expand the component.</p><p class=\"mainText\">&nbsp;</p><h2>Updating a Layout </h2><p class=\"mainText\">On expanding a new layout, you will come across 2 boxes in which you can implement your\n            layout. In the header box, you will typically find any required\n            <meta />and\n <link />tags for any pages that use that particular layout. In the body box, it&rsquo;s a simple breakdown of the\n            page layout - header, body and footer and all of the page&rsquo;s content get&rsquo;s populated into the page&rsquo;s body.\n        </p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Layouts/UpdateLayoutContent-en.png\" /><p class=\"mainText\">To save any changes you&rsquo;ve made within a layout you click the <strong>&ldquo;Save&rdquo;</strong> button\n            on the relevant\n            row. This will save any changes you&rsquo;ve made to this layout.</p><p class=\"mainText\">&nbsp;</p><h2>Deleting a Layout </h2><p class=\"mainText\">To delete a layout that you no longer use or need, then you click the <strong>&ldquo;Delete&rdquo;</strong> button that\n            will remove your layout and its content from the UI.\n        </p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Layouts/DeleteLayout-en.png\" /></div>"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "How to access and manage the layouts in the portal.",
      "Title": "Layouts"
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
  "Path": "Documentation/CoreDocumentation/AppManagement/Templates",
  "Name": "Templates",
  "ShowOnMenus": true,
  "Order": 0,
  "LastUpdated": "2024-08-22T12:04:12.2456356+01:00",
  "Layout": "Documentation",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "<div class=\"documentation\"><h2>Accessing Template Management</h2><p class=\"mainText\">You Access this page through the App Management Tabs. From here you can manage the app&rsquo;s\n            template, managing things like the colour things appear &amp; any CSS changes that need to happen within the\n            app,</p><h2>The UI</h2><p class=\"mainText\">When you access the templates tab, you&rsquo;re greeted with something that looks like this: </p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Templates/TemplatesGrid-en.png\" /><p class=\"mainText\">This grid contains any relevant templates for the given app, the most commonly used one\n            across all portals is the Theme-Default, which can be customised to ensure that our theming matches the\n            client&rsquo;s requested theme.</p><p class=\"mainText\">&nbsp;</p><h2>Creating a Template</h2><p class=\"mainText\">To create a new template in this UI, you click on the <strong>&ldquo;New Template&rdquo;</strong> button\n            in the header bar. Clicking this will bring up a dialog that looks something like this:</p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Templates/CreateTemplateDialog-en.png\" width=\"705\" height=\"208\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">Here, <strong>&ldquo;Resource Key&rdquo; </strong>is the way that the system selects which resources to\n            use in this\n            template. For example, if this is set to B2B, it will use the resources that have the matching key.\n            <strong>&ldquo;Name&rdquo;</strong>\n is the name you want your template to have that should be related to the places it will be used. Once you&rsquo;ve\n            filled these text boxes in with the relevant information, you can press confirm to create your new template.\n        </p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Templates/CreatedTemplate-en.png\" /><p class=\"mainText\">For now, I have created a Test template. On this row you can see that there are save and\n            delete functions and there&rsquo;s the ability expand the component.</p><p class=\"mainText\">&nbsp;</p><h2>Updating a Template</h2><p class=\"mainText\">On expanding a new template, you will come across a large text box, this box is where you\n            implement any css you want to be used across the site.</p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Templates/UpdateTemplateContent-en.png\" /><p class=\"mainText\">To save any CSS changes you&rsquo;ve made within a template you click the <strong>&ldquo;Save&rdquo;\n </strong>button on the\n            relevant row. This will save any changes you&rsquo;ve made to this template.</p><p class=\"mainText\">&nbsp;</p><h2>Deleting a Template </h2><p class=\"mainText\">To delete a template that you no longer use or need, then you click the &ldquo;Delete&rdquo; button that\n            will remove your template and its content from the UI.\n        </p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Templates/DeleteTemplate-en.png\" /></div>"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "How to access and manage the templates in the portal.",
      "Title": "Templates"
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
  "Path": "Documentation/CoreDocumentation/AppManagement/Components",
  "Name": "Components",
  "ShowOnMenus": true,
  "Order": 0,
  "LastUpdated": "2024-08-22T12:04:12.2578727+01:00",
  "Layout": "Documentation",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "<div class=\"documentation\"><h2>Accessing Component Management</h2><p class=\"mainText\">You Access this page through the App Management Tabs. From here you can manage any\n            components that are specific to the app that you&rsquo;re working in. This should have a very limited number of\n            components in it as the majority are Common Cached.</p><h2>The UI</h2><p class=\"mainText\">When you access the components tab, you&rsquo;re greeted with something that looks like this: </p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Components/EmptyComponentGrid-en.png\" /><p class=\"mainText\">The reason this grid is empty, is because all of this app&rsquo;s Components live in the Common\n            cache. However, it&rsquo;s still important to understand how to use this functionality should there ever be\n            anything that only needs to exist in this app.</p><p class=\"mainText\">&nbsp;</p><h2>Creating a Component</h2><p class=\"mainText\">To create a new component in this UI, you click on the <strong>\"New\"</strong> button in the\n            header bar. Clicking this will bring up a dialog that looks something like this:</p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Components/CreateComponentDialog-en.png\" width=\"616\" height=\"215\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">Here, <strong>&ldquo;Name&rdquo; </strong>is the name you want your component to have that needs to be\n            related to the\n            contents of it and its function. <strong>&ldquo;Key&rdquo;</strong> is the way that we split the components into\n            sections, for example we\n            have components with the key B2B which contain all things relating to company data and transactions.\n            <strong>&ldquo;Resource Key&rdquo;</strong> is the way that the system selects which resources to use in this component.\n            For example, if\n            this is set to B2B, it will use the resources that have the matching key. Once you&rsquo;ve filled these text\n            boxes in with the relevant information, you can press confirm to create your new component.\n        </p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Components/CreatedComponent-en.png\" /><p class=\"mainText\">For now, I have created a Test component. On this row you can see that there are save and\n            delete functions and there is the ability expand the component.</p><p class=\"mainText\">&nbsp;</p><h2>Updating a Component</h2><p class=\"mainText\">On expanding a new component, you will come across 2 boxes in which you can implement your\n            component code. It is broken down into two parts, HTML and JavaScript. In which you can view the code that\n            already exists and make any changes you want to.</p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Components/UpdatingComponent-en.png\" /><p class=\"mainText\">The left box is where you implement the html breakdown for the component and the right hand\n            box is where you implement the JavaScript code. Once you&rsquo;ve made your changes in either of these boxes, you\n            click the &ldquo;Save&rdquo; button on the relevant row. This will save any changes you&rsquo;ve made to this component.</p><p class=\"mainText\">&nbsp;</p><h2>Deleting a Component</h2><p class=\"mainText\">To delete a component that you no longer use or need, then you click the &ldquo;Delete&rdquo; button\n            that will remove your component and its content from the UI.</p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Components/DeletingComponent-en.png\" /></div>"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "How to access and manage the components in the portal.",
      "Title": "Components"
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
  "Path": "Documentation/CoreDocumentation/AppManagement/Resources",
  "Name": "Resources",
  "ShowOnMenus": true,
  "Order": 0,
  "LastUpdated": "2024-08-22T12:04:12.2710124+01:00",
  "Layout": "Documentation",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "<div class=\"documentation\"><h2>Accessing Resource Management</h2><p class=\"mainText\">You Access this page through the App Management Tabs. From here you can manage any resources\n            that are specific to the app that you&rsquo;re working in. This should have a very limited number of resources in\n            it as the majority are Common Cached.</p><h2>The UI</h2><p class=\"mainText\">When you access the resources tab, you&rsquo;re greeted with something that looks like this: </p><p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Resources/ResourcesGrid-en.png\" /></p><p class=\"mainText\">Here you can see the limited number of resources within our demo app, this is because the\n            vast majority of them are in the common cache which essentially means that they need to exist in every\n            portal we create. You will find that the minimal ones here are specific to this app only. When you expand a\n            row in the UI, it allows you to see the translations for that particular resource:\n        </p><p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Resources/ResourceExpand-en.png\" /></p><p class=\"mainText\">Within this particular piece of UI, it allows you to manage the translations for the\n            resource.</p><p class=\"mainText\">&nbsp;</p><h2>Creating a Resource</h2><p class=\"mainText\">To create a new resource in this UI, you click on the <strong>&ldquo;New Resource&rdquo;</strong>\n button in the header\n            bar. Clicking this will bring up a dialog that looks something like this:</p><p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Resources/CreateResourceDialog-en.png\" width=\"598\" height=\"322\" style=\"display:block;margin-left:auto;margin-right:auto;\" /></p><p class=\"mainText\">Here, <strong>&ldquo;Key&rdquo;</strong> is the &ldquo;group&rdquo; essentially that this resources fits in, this is\n            used by a\n            component to define what group of resources are used within it. When using a resource within a component or\n            a page, you must use our tags <strong>&ldquo;[resource_displayname[blah]]&rdquo; </strong>for example, here, the\n            <strong>&ldquo;Name&rdquo;</strong> would be filled\n            in as &ldquo;blah&rdquo;. The fields <strong>&ldquo;Display Name&rdquo;, &ldquo;Short Display Name&rdquo;</strong> and\n            <strong>&ldquo;Description&rdquo;</strong> are used to define how you\n            want it to be displayed within the portal. for example you may want your &ldquo;blah&rdquo; resource to have a capital B\n            so you would fill the display name field in as &ldquo;Blah&rdquo;. If you don&rsquo;t want them to have a different short\n            display name or an in depth description, then you need to fill all three fields with your chosen value. Once\n            you&rsquo;ve filled your fields with your desired resource values, click confirm, this will create it and it will\n            then be visible in the UI. Creating the example blah resource returns the following in the UI.\n        </p><p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Resources/CreatedResource-en.png\" /></p><h2>&nbsp;</h2><h3>Creating a Translation for a Resource</h3><p class=\"mainText\">When you expand this new resource you see that there are no translations that exists for it\n            just yet:</p><p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Resources/ResourceExpandCreate-en.png\" /></p><p class=\"mainText\">To create a translation for this resource, you click on the <strong>&ldquo;New\n Translation&rdquo;</strong> button.\n            Clicking this will bring up a dialog that looks something like this: </p><p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Resources/CreateTranslationDialog-en.png\" width=\"656\" height=\"153\" style=\"display:block;margin-left:auto;margin-right:auto;\" /></p><p class=\"mainText\">Within this dialog, you select the culture you wish to create the translation for, on\n            clicking create this will create a row in the translations grid that looks something like this:</p><p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Resources/CreateTranslation-en.png\" /></p><p class=\"mainText\">This creates a French translation row in the table, you can understand how to alter the\n            values in the &ldquo;Updating a Resource&rdquo; section of this page.</p><p class=\"mainText\">&nbsp;</p><h2>Updating a Resource</h2><p class=\"mainText\">To update the resource you have created, perhaps there&rsquo;s a typo or you want to change what\n            appears in the UI, you have to click on the text you want to change which will allow you to change the text\n            within it. </p><p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Resources/UpdateResource-en.png\" /></p><p class=\"mainText\">To save the changes you&rsquo;ve made, you must click the <strong>&ldquo;Save&rdquo;</strong> button on the\n            row relevant to the resource you&rsquo;re changing.</p><p class=\"mainText\">&nbsp;</p><h3>Updating a Translation for a Resource</h3><p class=\"mainText\">In order to alter a translation for a resource, you expand the resource you&rsquo;re interested in\n            and click onto the content you wish to alter, this makes it editable.</p><p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Resources/UpdateTranslation-en.png\" /></p><p class=\"mainText\">Change the content of the <strong>&ldquo;Display Name&rdquo;, &ldquo;Short Display Name&rdquo;</strong> and <strong>&ldquo;Description&rdquo;</strong> to the appropriate French translation and save it by clicking the save button within the translation row in the UI.</p><h1>&nbsp;</h1><h2>Deleting a Resource </h2><p class=\"mainText\">To delete a resource that you no longer use or need, then you click the <strong>&ldquo;Delete&rdquo;</strong> button that will remove your resource and its content from the UI.</p><p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Resources/DeleteResource-en.png\" /></p><p>&nbsp;</p><h3>Deleting a Translation for a Resource</h3><p class=\"mainText\">To delete a translation resource that you no longer use or need, then you click the <strong>&ldquo;Delete&rdquo;</strong> button that will remove that particular translation and its content from the UI.</p><p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Resources/DeleteTranslation-en.png\" /></p></div>"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "How to access and manage the resources in the portal.",
      "Title": "Resources"
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
  "Path": "Documentation/CoreDocumentation/ContentManagementSystem",
  "Name": "Content Management System",
  "ShowOnMenus": true,
  "Order": 0,
  "LastUpdated": "2024-08-22T12:04:12.3666151+01:00",
  "Layout": "Documentation",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "<div class=\"documentation\"><h2>Accessing our Content Management System </h2><p class=\"mainText\">Our Content Management System (CMS) sits under the Admin tab as this is predominantly used by\n        admins. </p><p>Once you have successfully logged into the portal, you can access the page by hovering over the\n        <strong>&ldquo;Admin&rdquo;</strong> button in the navigation bar and clicking <strong>&ldquo;Content Management&rdquo;</strong> button.</p><h2>The UI</h2><p class=\"mainText\">When you access the page, you&rsquo;re greeted with something that looks like this: </p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/PageSelector-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">Like with DMS, when you first access this page, there appears to be nothing on it. However, as\n        you can see on the left, there&rsquo;s a list of all the pages in the portal. To view a page&rsquo;s content you select it\n        from the list. For example, expanding <strong>&ldquo;Documentation&rdquo;</strong> and then expanding <strong>&ldquo;Core\n Documentation&rdquo; </strong> then selecting\n        <strong>&ldquo;Document Management System&rdquo;</strong> will bring up a page that looks something like this:\n    </p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/ContentArea-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">This is an editable view of the page that is selected, you will notice that the right hand side\n        is an editable box that allows you to change any text on the page quickly and easily. There are also buttons on\n        the top that allow you to perform different actions on the page.</p><p class=\"mainText\">&nbsp;</p><h2>Select a Page</h2><p class=\"mainText\">This functionality is the same as that on the left hand menu - it allows you to select the page\n        you would like to edit. Clicking on the <strong>&ldquo;Select Page&rdquo;</strong> button will bring up a dialog that looks\n        like this:</p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/SelectPageDialog-en.png\" width=\"435\" height=\"441\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">When you select a page on this dialog, it will load in an editable format allowing you to make\n        changes to it quickly and easily.</p><p class=\"mainText\">&nbsp;</p><h2>Creating a Page </h2><h3>New Root Page </h3><p class=\"mainText\">Clicking on the <strong>&ldquo;New Root Page&rdquo;</strong> button brings up a dialog that looks something\n        like this:</p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/NewPageDialog-en.png\" width=\"873\" height=\"218\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">Entering a name for your new page and clicking <strong>&ldquo;Create&rdquo;</strong> will create a new page\n        with that name\n        that is a root page, this means that it is typically a parent page that child pages will sit under. This means\n        that the path essentially becomes Parent. For example, App Management is a root page.</p><h3>New Child Page </h3><p class=\"mainText\">Clicking on the <strong>&ldquo;New Child Page&rdquo;</strong> button brings up a dialog that looks\n        something like this:</p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/NewChildPageDialog-en.png\" width=\"868\" height=\"215\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">Entering a name for your new page and clicking <strong>&ldquo;Create&rdquo;</strong> will create a new page\n        with that name\n        that is a &ldquo;Child&rdquo; of the page you&rsquo;re editing. This means that the path essentially becomes Parent/Child. For\n        example, App Management is a child page of the Admin page.</p><p class=\"mainText\">&nbsp;</p><h2>Updating a Page </h2><h3>Page Properties </h3><h4>General </h4><p class=\"mainText\">Clicking on the <strong>&ldquo;Page Properties&rdquo;</strong> button brings up a dialog that looks\n        something like this:</p>src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/NewChildPageDialog-en.png\" /&gt;\n    <img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/PagePropertiesGeneral-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">In the <strong>&ldquo;General&rdquo;</strong> tab contains the page&rsquo;s general settings, including the\n        <strong>&ldquo;Layout&rdquo;</strong> that the page\n        uses, <strong>&ldquo;Show on Menu&rdquo;</strong> determines whether or not the page is shown in the navigation menu,\n        de-selecting this will\n        remove it from the navigation menu. The &ldquo;Resource Key&rdquo; field corresponds to the resource set you want to be used\n        on this page, for example if you have B2B resources you need to use, then you would set this key to B2B.\n    </p><h4>Information </h4><p class=\"mainText\">In the <strong>&ldquo;Information&rdquo;</strong> tab, you can manage a few things related to the page. You\n        can set up the\n        &ldquo;Culture&rdquo; which allows you to set the language of the page information, <strong>&ldquo;Title&rdquo;</strong> this is the\n        title of the page,\n        you can create new rows in the grid that will allow you to set up different translations of the page&rsquo;s title.\n    </p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/PagePropertiesInformation-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">Clicking on the <strong> &ldquo;New&rdquo; </strong> will create a new row in the dialog, which you can fill\n        in with the\n        information you desire. For example, if you want to create a French translation for the page&rsquo;s title, you&rsquo;ll\n        select <strong>&ldquo;French (France)&rdquo;</strong>from the drop down in the cultures column, and set the title to the\n        French translation\n        of <strong>&ldquo;Document Management System&rdquo;</strong>. This means that when the culture is set to French in the\n        portal, the title will\n        be shown as the French translation set here.</p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/PagePropertiesEditInformation-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">The <strong>&ldquo;Description&rdquo;</strong> and <strong>&ldquo;Keywords&rdquo;</strong> columns aren&rsquo;t required,\n        however if you wish to provided a description or any keywords that relate to the page you&rsquo;re creating then they\n        can be filled in here. </p><h4>Roles </h4><p class=\"mainText\">In the <strong>&ldquo;Roles&rdquo;</strong> tab, you can manage what users can see the page you&rsquo;ve created,\n        for example if\n        it&rsquo;s only relevant for Administrators, then you would assign only that role to the page so that only\n        Administrators can view it. You can change these by selecting or de-selecting them in the grid.</p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/PagePropertiesRoles-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><h3>Content Area </h3><p class=\"mainText\">As you&rsquo;re aware, the content area of the page, is an editable text box. When you click this, a\n        toolbar appears that allows you to add text as you would in word and control the formatting of it. The toolbar\n        looks something like this, and the icon in the top left of it allows you to freely move it around the screen for\n        ease of use.</p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/EditPageToolbar-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/TextStyleEditor-en.png\" /><p class=\"mainText\">This tool bar allows you to modify any content on the page, for example the <strong>B</strong>\n button allows you\n        to set the selected text style to bold, <strong>I</strong> allows you to set the selected text to be itallic and\n        <strong>U</strong> allows you to\n        set whether or not you want that text to be displayed underlined.\n    </p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/UndoRedoButtons-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">The next two buttons that contain the arrows allow you to undo and redo any changes you&rsquo;ve made\n        in the content. Allowing you to quickly revert any mistakes you may have made along the way. </p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/TextAlignButtons-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">The group of three buttons following the undo and redo buttons allow you to control the text\n        alignment on the page. You may recognise these things from programs like Microsoft word. The left one allows you\n        to align text to the left-hand side of the page, the middle button allows you to centre-align the content on the\n        page and the third button allows you to right-align the text.</p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/BulletPointButton-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">The first button of the next group allows you to embed a link on the page, clicking it opens a\n        dialog that looks something like this:</p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/LinkEmbedDialog-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">The <strong>&ldquo;Web address&rdquo;</strong> box need to be populated with the link you want on the page,\n        the <strong>&ldquo;Text&rdquo;</strong> box\n        needs to be populated with the text you want to replace the link with (ideally this needs to be relevant to\n        where the link will take you). <strong>&ldquo;ToolTip&rdquo;</strong> will provide any extra information to the user when\n        the link is hovered\n        over. Selecting the <strong>&ldquo;Open link in new window&rdquo;</strong> will ensure that when the link is clicked, it\n        will open it up in a\n        new window as opposed to directing them away from the page that they are looking at. To confirm your choices and\n        to embed the link onto the page within the content, click the <strong>&ldquo;Insert&rdquo;</strong> button.</p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/ImageEmbedButton-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">The next, non-greyed out button is used to embed an image on the page, clicking this button\n        opens a dialog that looks something like this: </p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/ImageEmbedDialog-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">The <strong>&ldquo;Web address&rdquo;</strong> is the link where the image can be found, for example, to get\n        an image from DMS in the current portal, this link will need to look something like this:</p><pre> [app[root]]Api/DMS/Content/Documentation/image.png </pre><p class=\"mainText\">The <strong>&ldquo;Alternate text&rdquo;</strong> field needs to be filled in with a brief description of\n        the image, as this\n        will be displayed if the image can&rsquo;t be displayed for some reason. The <strong>&ldquo;width&rdquo;</strong> and\n        <strong>&ldquo;height&rdquo;</strong> fields need to be\n        populated with the desired width and height (in px) of the image you&rsquo;re placing.\n    </p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/CreateTableButton-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">This button allows you to create a table in the content area, clicking on this button gives you\n        a dialog that looks something like this: </p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/CreateTableDialog-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">When you hover over the boxes, you will see a different level of highlighting, you can use this\n        to determine the size of the table that you want. By highlighting the appropriate number of columns and rows and\n        simply just clicking. This will then create an empty table in the content area which you can then add content\n        to. However, if you wish to have more control over the table&rsquo;s styling you may want to create your table using\n        the <strong>&ldquo;Table Wizard&rdquo;</strong>. Clicking this button will bring up a dialog that looks something like this:\n    </p><h4>Table Tab </h4><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/TableWizardTable-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\"><strong>&ldquo;Width&rdquo;</strong> here determines the total width of the table in px, and\n        <strong>&ldquo;Height&rdquo;</strong> determines the total\n        height of the table. <strong>&ldquo;Columns&rdquo;</strong> is used to set the total number of columns in the table and\n        <strong>&ldquo;Rows&rdquo;</strong> will be the\n        total number of rows. The <strong>&ldquo;Cell Spacing&rdquo;</strong> refers to the amount of space you want there to be\n        between cells (in\n        px) in the table and the <strong>Cell Padding&rdquo;</strong>&ldquo; relates to the padding (white space) around any content\n        within the cells\n        in px. <strong>&ldquo;Alignment&rdquo;</strong> allows you to set up how you want the text to be aligned within the table&rsquo;s\n        cells.\n        <strong>&ldquo;Background&rdquo;</strong> allows you to select a preferred background colour. The <strong>&ldquo;CSS\n Class&rdquo;</strong> field can be populated with a\n        known CSS class, which may define the &ldquo;default&rdquo; table formatting in the portal you&rsquo;re in, setting an\n        <strong>&ldquo;ID&rdquo;</strong> will\n        allow you to easily select this element using CSS to apply any specific styling you want to it.\n        <strong>&ldquo;Border&rdquo;</strong> accepts\n        a value for the border width (in px) in the text box, this determines the &ldquo;weight&rdquo;/thickness of it. The colour\n        picker next to this field allows you to specify a preferred border colour for the table you&rsquo;re creating.\n        <strong>&ldquo;Border\n Style&rdquo;</strong> allows you to specify the style of border you&rsquo;d like, whether you&rsquo;d like it to be dotted,\n        solid or dashed\n        for example.\n    </p><h4>Cell Tab </h4><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/TableWizardCell-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">When <strong>&ldquo;Select All Cells&rdquo;</strong> is selected, any changes made and confirmed within\n        this dialog will be\n        applied to all cells within the table you&rsquo;re creating. <strong>&ldquo;Width&rdquo;</strong>&gt; and <strong>&ldquo;Height&rdquo;</strong>\n here apply to the cells only and not\n        to the table as a whole. This is useful when you only want one or a few cells within the table to have differing\n        styles. All of the styles that can be set in this tab apply only to the cell you&rsquo;ve selected and not to the\n        whole table like in the <strong>&ldquo;Table&rdquo;</strong> tab. When <strong>&ldquo;Wrap text&rdquo;</strong> is not selected, it will\n        mean that the text does not wrap\n        onto new lines when reaching the limits of the cell size.</p><h4>Accessibility Tab </h4><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/TableWizardAccessibility-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">These settings enhance the table&rsquo;s accessibility, the <strong>&ldquo;Header Rows&rdquo;</strong> and\n        <strong>&ldquo;Header Columns&rdquo;</strong>\n determine how many header rows and columns there are in the defined table. <strong>&ldquo;Caption&rdquo;</strong> should\n        provide the user\n        with a concise explanation of the information that is displayed in the table you&rsquo;re creating. The\n        <strong>&ldquo;Associate\n headers&rdquo; </strong> determine links between the header columns and rows and how you expect them to be linked\n        to one\n        another.\n    </p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/TextHierarchy-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">The dropdown with the text <strong>&ldquo;Heading 1&rdquo;</strong> allows you to set the &ldquo;tag&rdquo; of the\n        content, for example\n        &ldquo;Heading 1&rdquo; would be representative of a main heading and the content would usually be contained in\n    </p><p>html tags. Setting this to <strong>&ldquo;Paragraph&rdquo;</strong> would ensure that your text is the same style as\n    typical main body\n    text.</p><p>&nbsp;</p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/FontColouring-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">The first colour picker allows you to select what colour you would like the font to be\n        displayed as and the second colour picker enables you to specify a highlight colour, if you want the text to be\n        highlighted.</p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/ViewSourceButton-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">The final button in the toolbar is the <strong>&ldquo;View Source&rdquo;</strong> button, when you click\n        this it comes up\n        with an editor that looks something like this:</p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/ViewSourceDialog-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">This editor is particularly useful if you would like to input content on the site in the form\n        of HTML and it is where it can be placed such that the content within it is displayed on the page you&rsquo;re\n        editing.</p><p>&nbsp;</p><h2>Deleting a Page </h2><p class=\"mainText\">To delete a page from the system, you click the <strong>&ldquo;Delete Page&rdquo;</strong> button from the toolbar at the top\n        of the page. This will remove the selected page from the system and it will no longer be visible in the UI. </p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/CMS/DeletePage-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /></div>"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "How to manage the pages within the application.",
      "Title": "Content Management System"
    }
  ]
}
"""
            }
        ]
    };
}