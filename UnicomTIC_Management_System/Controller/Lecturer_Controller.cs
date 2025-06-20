using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicomTIC_Management_System.Model;
using UnicomTIC_Management_System.Service;

namespace UnicomTIC_Management_System.Controller
{
    internal class Lecturer_Controller
    {
            private readonly Lecturer_Service lecturer_service;

            public Lecturer_Controller()
            {
                lecturer_service = new Lecturer_Service();
            }

            public List<Lecturer> GetAllLecturers() => lecturer_service.Get_All_Lecturers();

            public void AddLecturer_DB(Lecturer lecturer) => lecturer_service.AddLecturer(lecturer);

            public void UpdateLecturer(Lecturer lecturer) => lecturer_service.Update_Lecturer(lecturer);

            public void DeleteLecturer(Lecturer lecturer) => lecturer_service.Delete_Lecturer(lecturer);
  

    }
}
