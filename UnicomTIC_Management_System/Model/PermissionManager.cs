using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnicomTIC_Management_System.Model
{
    public static class PermissionManager
    {
        public static bool HasPermission(string role, string action, string table)
        {
            try
            {
                switch (role)
                {
                    case "Admin":
                        return true;

                    case "Staff":
                        if (table == "Staff" && (action == "Add" || action == "Update" || action == "Delete"))
                            return false;
                        return true;

                    case "Lecturer":
                        if (table == "Marks")
                            return true;
                        return action == "View";

                    case "Student":
                        return action == "View";

                    default:
                        return false;
                }
            }
            catch
            {
                MessageBox.Show("Access control failure.");
                return false;
            }
        }
    }

}
