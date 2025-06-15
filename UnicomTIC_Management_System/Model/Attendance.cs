using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicomTIC_Management_System.Model
{
    internal class Attendance
    {
        public int Attendence_ID { get; set; }
        public string Attendence_Status { get; set; }
        public int Marked_Student_Att { get; set; }
        public int Timetable_ID { get; set; }
        public int User_ID { get; set; }

    }
}
