using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicomTIC_Management_System.Model
{
    internal class Log_Table
    {
        public string Log_ID { get; set; }
        public string Timestamp { get; set; }
        public string Action { get; set; }
        public string Log_Status { get; set; }
        public string User_ID { get; set; }
    }
}
