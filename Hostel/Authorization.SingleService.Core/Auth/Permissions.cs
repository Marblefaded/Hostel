﻿using System.ComponentModel;
using System.Reflection;

namespace Suo.Autorization.Data.Models.Responce;

public static class Permissions
{
    //[DisplayName("Products")]
    //[Description("Products Permissions")]
    //public static class Products
    //{
    //    public const string View = "Permissions.Products.View";
    //    public const string Create = "Permissions.Products.Create";
    //    public const string Edit = "Permissions.Products.Edit";
    //    public const string Delete = "Permissions.Products.Delete";
    //    public const string Export = "Permissions.Products.Export";
    //    public const string Search = "Permissions.Products.Search";
    //}

    [DisplayName("Project")]
    [Description("Project Permissions")]
    public static class Project
    {
        public const string View = "Permissions.Project.View";
        public const string Create = "Permissions.Project.Create";
        public const string Edit = "Permissions.Project.Edit";
        public const string Delete = "Permissions.Project.Delete";
        public const string Export = "Permissions.Project.Export";
        public const string Search = "Permissions.Project.Search";
        public const string Import = "Permissions.Project.Import";
    }
    [DisplayName("Users")]
    [Description("Users Permissions")]
    public static class Users
    {
        public const string View = "Permissions.Users.View";
        public const string Create = "Permissions.Users.Create";
        public const string Edit = "Permissions.Users.Edit";
        public const string Delete = "Permissions.Users.Delete";
        public const string Export = "Permissions.Users.Export";
        public const string Search = "Permissions.Users.Search";
    }
    //[DisplayName("Documents")]
    //[Description("Documents Permissions")]
    //public static class Documents
    //{
    //    public const string View = "Permissions.Documents.View";
    //    public const string Create = "Permissions.Documents.Create";
    //    public const string Edit = "Permissions.Documents.Edit";
    //    public const string Delete = "Permissions.Documents.Delete";
    //    public const string Search = "Permissions.Documents.Search";
    //}

    //[DisplayName("Document Types")]
    //[Description("Document Types Permissions")]
    //public static class DocumentTypes
    //{
    //    public const string View = "Permissions.DocumentTypes.View";
    //    public const string Create = "Permissions.DocumentTypes.Create";
    //    public const string Edit = "Permissions.DocumentTypes.Edit";
    //    public const string Delete = "Permissions.DocumentTypes.Delete";
    //    public const string Export = "Permissions.DocumentTypes.Export";
    //    public const string Search = "Permissions.DocumentTypes.Search";
    //}

    //[DisplayName("Document Extended Attributes")]
    //[Description("Document Extended Attributes Permissions")]
    //public static class DocumentExtendedAttributes
    //{
    //    public const string View = "Permissions.DocumentExtendedAttributes.View";
    //    public const string Create = "Permissions.DocumentExtendedAttributes.Create";
    //    public const string Edit = "Permissions.DocumentExtendedAttributes.Edit";
    //    public const string Delete = "Permissions.DocumentExtendedAttributes.Delete";
    //    public const string Export = "Permissions.DocumentExtendedAttributes.Export";
    //    public const string Search = "Permissions.DocumentExtendedAttributes.Search";
    //}



    //[DisplayName("Roles")]
    //[Description("Roles Permissions")]
    //public static class Roles
    //{
    //    public const string View = "Permissions.Roles.View";
    //    public const string Create = "Permissions.Roles.Create";
    //    public const string Edit = "Permissions.Roles.Edit";
    //    public const string Delete = "Permissions.Roles.Delete";
    //    public const string Search = "Permissions.Roles.Search";
    //}

    //[DisplayName("Role Claims")]
    //[Description("Role Claims Permissions")]
    //public static class RoleClaims
    //{
    //    public const string View = "Permissions.RoleClaims.View";
    //    public const string Create = "Permissions.RoleClaims.Create";
    //    public const string Edit = "Permissions.RoleClaims.Edit";
    //    public const string Delete = "Permissions.RoleClaims.Delete";
    //    public const string Search = "Permissions.RoleClaims.Search";
    //}

    //[DisplayName("Communication")]
    //[Description("Communication Permissions")]
    //public static class Communication
    //{
    //    public const string Chat = "Permissions.Communication.Chat";
    //}

    //[DisplayName("Preferences")]
    //[Description("Preferences Permissions")]
    //public static class Preferences
    //{
    //    public const string ChangeLanguage = "Permissions.Preferences.ChangeLanguage";

    //    //TODO - add permissions
    //}

    //[DisplayName("Dashboards")]
    //[Description("Dashboards Permissions")]
    //public static class Dashboards
    //{
    //    public const string View = "Permissions.Dashboards.View";
    //}

    //[DisplayName("Hangfire")]
    //[Description("Hangfire Permissions")]
    //public static class Hangfire
    //{
    //    public const string View = "Permissions.Hangfire.View";
    //}

    //[DisplayName("Audit Trails")]
    //[Description("Audit Trails Permissions")]
    //public static class AuditTrails
    //{
    //    public const string View = "Permissions.AuditTrails.View";
    //    public const string Export = "Permissions.AuditTrails.Export";
    //    public const string Search = "Permissions.AuditTrails.Search";
    //}

    /// <summary>
    /// Returns a list of Permissions.
    /// </summary>
    /// <returns></returns>
    public static List<string> GetRegisteredPermissions()
    {
        var permissions = new List<string>();
        foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
        {
            var propertyValue = prop.GetValue(null);
            if (propertyValue is not null)
                permissions.Add(propertyValue.ToString());
        }
        return permissions;
    }
}