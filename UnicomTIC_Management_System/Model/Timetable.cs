using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicomTIC_Management_System.Model
{
    internal class Timetable
    {
        public int Timetable_ID { get; set; }
        public string Time_Slot { get; set; }
        public string Timetable_Date { get; set; }
        public string Timetable_Status { get; set; }
        public int CS_ID { get; set; }
        public int Room_ID { get; set; }

    }
}
