using System.Web;
namespace RazorTemplateDemo.Models
{
    public class SimpleModel
    {
        public string Name
        {
            get
            {
                return "My name is Simple, Simple Model";
            }
        }

        //http://stackoverflow.com/questions/1064274/get-current-asp-net-trust-level-programmatically/4774231#4774231
        public AspNetHostingPermissionLevel GetTrustLevel
        {
            get
            {

                foreach (AspNetHostingPermissionLevel trustLevel in
                        new AspNetHostingPermissionLevel[] {
                            AspNetHostingPermissionLevel.Unrestricted,
                            AspNetHostingPermissionLevel.High,
                            AspNetHostingPermissionLevel.Medium,
                            AspNetHostingPermissionLevel.Low,
                            AspNetHostingPermissionLevel.Minimal 
                        })
                {
                    try
                    {
                        new AspNetHostingPermission(trustLevel).Demand();
                    }
                    catch (System.Security.SecurityException)
                    {
                        continue;
                    }

                    return trustLevel;
                }

                return AspNetHostingPermissionLevel.None;

            }
        }
    }
}

