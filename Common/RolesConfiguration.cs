using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum Permissions
    {
        Read = 0,
    }

    public enum Roles
    {
        Reader = 0,
    }
    public class RolesConfiguration
    {
        static string[] ReaderPermissions = new string[] { Permissions.Read.ToString() };
        static string[] Empty = new string[] { };

        public static string[] GetPermissions(string role)
        {

            switch (role)
            {
                case "Reader": return ReaderPermissions;
                default: return Empty;
            }
        }
    }
}
