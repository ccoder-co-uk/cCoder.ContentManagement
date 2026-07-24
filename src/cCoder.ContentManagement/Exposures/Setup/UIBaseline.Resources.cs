// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Exposures.Setup;

public static partial class UIBaseline
{
    static Package Resources =>
        new()
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
  "Key": "CMS",
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
}