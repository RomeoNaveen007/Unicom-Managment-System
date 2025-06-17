using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicomTIC_Management_System.Model
{
    internal class Student_Attendance
    {
        public string Marked_Date { get; set; }
        public int Marked_Student_Att { get; set; }
        public string Attendance_Status { get; set; }
        public int Timetable_ID { get; set; }

    }
}
