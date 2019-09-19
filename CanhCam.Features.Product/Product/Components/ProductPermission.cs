/// Author:			        Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
/// Created:				2014-06-23
/// Last Modified:			2014-08-25

using System;
using System.Linq;

namespace CanhCam.Web.ProductUI
{
    public static class ProductPermission
    {
        public const string ViewList = "Permissions.Product.ViewList";
        public const string Create = "Permissions.Product.Create";
        public const string Update = "Permissions.Product.Update";
        public const string Delete = "Permissions.Product.Delete";
        
        public const string ManageTags = "Permissions.Product.ManageTags";
        public const string ManageOrders = "Permissions.Product.ManageOrders";

        public const string DeleteComment = "Permissions.Product.DeleteComment";
        public const string ApproveComment = "Permissions.Product.ApproveComment";
        public const string UpdateComment = "Permissions.Product.UpdateComment";
        public const string ManageComment = "Permissions.Product.ManageComment";

        public static bool CanViewList
        {
            get { return SiteUtils.UserHasPermission(ViewList); }
        }
        public static bool CanCreate
        {
            get { return SiteUtils.UserHasPermission(Create); }
        }
        public static bool CanUpdate
        {
            get { return SiteUtils.UserHasPermission(Update); }
        }
        public static bool CanDelete
        {
            get { return SiteUtils.UserHasPermission(Delete); }
        }
        public static bool CanManageTags
        {
            get { return SiteUtils.UserHasPermission(ManageTags); }
        }
        public static bool CanManageOrders
        {
            get { return SiteUtils.UserHasPermission(ManageOrders); }
        }
        public static bool CanDeleteComment
        {
            get { return SiteUtils.UserHasPermission(DeleteComment); }
        }
        public static bool CanApproveComment
        {
            get { return SiteUtils.UserHasPermission(ApproveComment); }
        }
        public static bool CanUpdateComment
        {
            get { return SiteUtils.UserHasPermission(UpdateComment); }
        }
        public static bool CanManageComment
        {
            get { return SiteUtils.UserHasPermission(ManageComment); }
        }
    }
}