using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicomTIC_Management_System.Data.log_session;

namespace UnicomTIC_Management_System.Model
{
    internal class Login 
    {
        public string Login_user { get; set; }
        public string Login_password { get; set; }
        public string Login_role {  get; set; }

    }
}
