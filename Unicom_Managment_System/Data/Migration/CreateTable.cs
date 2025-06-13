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
                string query = @"CREATE TABLE IF NOT EXISTS Course (
                                        Cousre_ID INTEGER PRIMARY KEY AUTOINCREMENT, 
                                        Course_Name TEXT NOT NULL UNIQUE,
                                        Duration TEXT NOT NULL,
                                        CHECK (Cousre_ID > 1000 AND Cousre_ID <= 1050));

                                CREATE TABLE IF NOT EXISTS Subject (
                                        Subject_ID INTEGER PRIMARY KEY AUTOINCREMENT, 
                                        Subject_Name TEXT NOT NULL ,
                                        Duration TEXT NOT NULL,
                                        CHECK (Subject_ID >= 1 AND Subject_ID <= 300));

                                  CREATE TABLE IF NOT EXISTS Course_Subject (
                                        CS_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Subject_ID INTEGER, 
                                        Cousre_ID INTEGER,
                                        FOREIGN KEY (Subject_ID) REFERENCES Subject(Subject_ID),
                                        FOREIGN KEY (Cousre_ID) REFERENCES Course(Cousre_ID),
                                        CHECK (CS_ID >= 1001 AND CS_ID <= 10000));
      
                                        




















";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }


            }

        }

    }
}
