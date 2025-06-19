using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicomTIC_Management_System.Model;
using UnicomTIC_Management_System.Service;

namespace UnicomTIC_Management_System.Controller
{
    internal class Staff_Controller
    {
        private readonly Staff_Service staff_service;

        public Staff_Controller() 
        {
            staff_service = new Staff_Service();
        }

        public List<Staff> GetAllStaff() => staff_service.Get_All();
        public void AddStaff_DB(Staff staff) => staff_service.GetAll_staff( staff);
        public void update_staff(Staff staff) => staff_service.staff_Update(staff);
    }
}
