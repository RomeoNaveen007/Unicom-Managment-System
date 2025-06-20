using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicomTIC_Management_System.Model;
using UnicomTIC_Management_System.Service;

namespace UnicomTIC_Management_System.Controller
{
    internal class Admin_Controller
    {
        private readonly Admin_Service admin_service;

        public Admin_Controller()
        {
            admin_service = new Admin_Service();

        }

        public List<Admin> GetAllAdmins() => admin_service.Get_All();

        public void Add_All_Admin(Admin admin) => admin_service.Addadmin(admin);

        public void UpdateSection(Admin up_admin) => admin_service.Updateadmin(up_admin);

        public void DeleteSection(Admin del_admin) => admin_service.Deleteadmin(del_admin);



    }
    }
