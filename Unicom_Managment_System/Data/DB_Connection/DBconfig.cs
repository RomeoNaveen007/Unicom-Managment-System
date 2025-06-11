using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicom_Managment_System.Data.DB_Connection
{
    static class DBconfig
    {
        public static SQLiteConnection getConnection()
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=Unicom_Managment_System_DB.db;Version=3;");
            conn.Open();
            return conn;
        }

    }
}
