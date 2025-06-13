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
                                        CHECK (Cousre_ID >=1001 AND Cousre_ID <= 1050));

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
                                        CHECK (CS_ID >= 1001 AND CS_ID <= 9999));
                
                                CREATE TABLE IF NOT EXISTS Exam (
                                        Exam_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Exam_type TEXT NOT NULL,
                                        Exam_Date TEXT NOT NULL,
                                        Exam_Duration TEXT NOT NULL,
                                        CS_ID INTEGER , 
                                        FOREIGN KEY (CS_ID) REFERENCES Course_Subject(CS_ID),
                                        CHECK (Exam_ID >= 10001 AND Exam_ID <= 99999));                        
                                       
                                CREATE TABLE IF NOT EXISTS Timetable (
                                        Timetable_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Time_Slot TEXT NOT NULL,
                                        Timetable_Date TEXT NOT NULL,
                                        Timetable_Status TEXT NOT NULL,
                                        CS_ID INTEGER, 
                                        Room_ID INTEGER,
                                        FOREIGN KEY (Room_ID) REFERENCES Room(Room_ID),
                                        FOREIGN KEY (CS_ID) REFERENCES Course_Subject(CS_ID),
                                        CHECK (Timetable_ID >= 100001 AND Timetable_ID <= 299999));                        
                                       
                                CREATE TABLE IF NOT EXISTS Room (
                                        Room_ID INTEGER PRIMARY KEY AUTOINCREMENT, 
                                        Room_Name TEXT NOT NULL,
                                        Room_Type TEXT NOT NULL,
                                        CHECK (Room_ID >= 300001 AND Room_ID <= 300500 ));

                                 CREATE TABLE IF NOT EXISTS Marks (
                                        Exam_ID INTEGER, 
                                        Student_ID INTEGER,
                                        Score DOUBLE NOT NULL,
                                        FOREIGN KEY (Exam_ID) REFERENCES Exam(Exam_ID),
                                        FOREIGN KEY (Student_ID) REFERENCES Student(Student_ID),
                                        PRIMARY KEY (Exam_ID,Student_ID));

                                CREATE TABLE IF NOT EXISTS Student (
                                        Student_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Student_Name TEXT NOT NULL,
                                        Student_Address TEXT NOT NULL,
                                        Student_NIC TEXT UNIQUE NOT NULL,
                                        Student_Status TEXT NOT NULL,
                                        CS_ID INTEGER,
                                        Batch_ID INTEGER,
                                        FOREIGN KEY (CS_ID) REFERENCES Course_Subject(CS_ID),
                                        FOREIGN KEY (Batch_ID) REFERENCES Batch(Batch_ID),
                                        CHECK (Student_ID >= 400001 AND Student_ID <=999999));
                                       
                                CREATE TABLE IF NOT EXISTS Lecturer (
                                        Lecturer_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Lecturer_Name TEXT NOT NULL,
                                        Lecturer_Address TEXT NOT NULL,
                                        Lecturer_NIC TEXT UNIQUE NOT NULL,
                                        Lecturer_Status TEXT NOT NULL,
                                        Lecturer_Degree TEXT NOT NULL,
                                        CHECK (Lecturer_ID >= 1000001 AND Lecturer_ID <=1999999));
                                        
                                        
                                       
                                 CREATE TABLE IF NOT EXISTS Batch (
                                        Batch_ID INTEGER PRIMARY KEY AUTOINCREMENT, 
                                        Batch_Name TEXT NOT NULL UNIQUE,
                                        Batch_Year INTEGER NOT NULL,
                                        CHECK (Batch_ID >= 0001 AND Batch_ID <=0100));      
                                                                                                           
                                        
                                 




";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }


            }

        }

    }
}
