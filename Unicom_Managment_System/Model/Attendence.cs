using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicom_Managment_System.Model
{
    internal class Attendence
    {
        public int Attendence_ID { get; set; }
        public string Attendence_Status { get; set; }
        public int Marked_Student_Att {  get; set; }
        public int Timetable_ID { get; set; }
        public int User_ID { get; set; }
    }
}
