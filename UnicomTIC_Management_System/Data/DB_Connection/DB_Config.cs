using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicomTIC_Management_System.Data.DB_Connection
{
    static class DB_Config
    {
        public static SQLiteConnection getConnection()
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=Unicom.db;Version=3;");
            conn.Open();
            return conn;
        }
    }
}
