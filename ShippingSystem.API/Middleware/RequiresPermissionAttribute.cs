namespace ShippingSystem.API.Middleware
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequiresPermissionAttribute : Attribute
    {
        public string PermissionName { get; }

        public RequiresPermissionAttribute(string permissionName)
        {
            PermissionName = permissionName;
        }
    }
}
