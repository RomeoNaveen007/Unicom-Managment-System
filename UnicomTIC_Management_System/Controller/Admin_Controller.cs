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
            admin_Service = new Admin_Service();

        }

            public List<Admin> GetAllAdmins() => admin_service.show_Output();

            public void AddSection(Section section) => _sectionService.Add(section);

            //public void UpdateSection(Section section) => _sectionService.Update(section);

            //public void DeleteSection(int sectionId) => _sectionService.Delete(sectionId);



        }
    }
