namespace acl_openstack_identity.Helpers
{
    public class scopeSelector
    {
        public static int GetScopeInt(string scope)
        {
            int scopeInt = -1;
            switch (scope)
            {
                case "Organization":
                    scopeInt = 0;
                    break;
                case "Folder":
                    scopeInt = 1;
                    break;
                case "Project":
                    scopeInt = 2;
                    break;
            }

            return scopeInt;
        }

        public static string GetScopeString(int scope)
        {
            string scopeString = string.Empty;

            switch (scope)
            {
                case 0:
                    scopeString = "Organization";
                    break;
                case 1:
                    scopeString = "Folder";
                    break;
                case 2:
                    scopeString = "Project";
                    break;
            }

            return scopeString;
        }
    }
}
