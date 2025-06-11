using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unicom_Managment_System.Data.DB_Connection;

namespace Unicom_Managment_System.Data.Migration
{
    internal class CreateTable
    {
        public void table_Creation()
        {
            using (var conn = DBconfig.getConnection())
            {
                string query = @"" ;

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }


            }

        }

    }
}
