using System.Reflection;

namespace NetworkManager.Web.References
{
    public static class ReferencedAssemblies
    {
        public static Assembly Services
        {
            get { return Assembly.Load("NetworkManager.Services"); }
        }

        public static Assembly Repositories
        {
            get { return Assembly.Load("NetworkManager.Data"); }
        }

        public static Assembly Dto
        {
            get
            {
                return Assembly.Load("NetworkManager.Dto");
            }
        }

        public static Assembly Domain
        {
            get
            {
                return Assembly.Load("NetworkManager.Core");
            }
        }
    }
}
