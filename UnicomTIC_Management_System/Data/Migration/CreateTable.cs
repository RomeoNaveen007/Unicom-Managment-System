using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicomTIC_Management_System.Data.DB_Connection;

namespace UnicomTIC_Management_System.Data.Migration
{
    internal class CreateTable
    {
        public void table_Creation()
        {
            using (var conn = DB_Config.getConnection())
            {
                string query = @"
                                 CREATE TABLE IF NOT EXISTS Room (
                                    Room_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Room_Name TEXT NOT NULL,
                                    Room_Type TEXT NOT NULL,
                                    CHECK (Room_ID >= 300001 AND Room_ID <= 300500)
                                );

                                CREATE TABLE IF NOT EXISTS User (
                                    User_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                    User_Name TEXT NOT NULL UNIQUE,
                                    Password TEXT NOT NULL,
                                    Role TEXT NOT NULL CHECK (Role IN ('Admin', 'Staff', 'Lecturer', 'Student'))
                                );

                                CREATE TABLE IF NOT EXISTS Course (
                                    Course_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Course_Name TEXT NOT NULL UNIQUE,
                                    Duration TEXT NOT NULL,
                                    CHECK (Course_ID >= 1001 AND Course_ID <= 1050)
                                );

                                CREATE TABLE IF NOT EXISTS Subject (
                                    Subject_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Subject_Name TEXT NOT NULL,
                                    CHECK (Subject_ID >= 1 AND Subject_ID <= 300)
                                );

                                CREATE TABLE IF NOT EXISTS Course_Subject (
                                    CS_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Subject_ID INTEGER,
                                    Course_ID INTEGER,
                                    FOREIGN KEY (Subject_ID) REFERENCES Subject(Subject_ID),
                                    FOREIGN KEY (Course_ID) REFERENCES Course(Course_ID),
                                    CHECK (CS_ID >= 1001 AND CS_ID <= 4999)
                                );

                                CREATE TABLE IF NOT EXISTS Exam (
                                    Exam_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Exam_type TEXT NOT NULL,
                                    Exam_Date DATE NOT NULL,
                                    Exam_Duration TEXT NOT NULL,
                                    CS_ID INTEGER,
                                    FOREIGN KEY (CS_ID) REFERENCES Course_Subject(CS_ID),
                                    CHECK (Exam_ID >= 10001 AND Exam_ID <= 99999)
                                );

                                CREATE TABLE IF NOT EXISTS Timetable (
                                    Timetable_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Time_Slot TEXT NOT NULL,
                                    Timetable_Date DATE NOT NULL,
                                    Timetable_Status TEXT NOT NULL,
                                    CS_ID INTEGER,
                                    Room_ID INTEGER,
                                    FOREIGN KEY (Room_ID) REFERENCES Room(Room_ID),
                                    FOREIGN KEY (CS_ID) REFERENCES Course_Subject(CS_ID),
                                    CHECK (Timetable_ID >= 100001 AND Timetable_ID <= 299999)
                                );

                                CREATE TABLE IF NOT EXISTS Batch (
                                    Batch_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Batch_Name TEXT NOT NULL UNIQUE,
                                    Year INTEGER NOT NULL,
                                    CHECK (Batch_ID >= 5001 AND Batch_ID <= 9999)
                                );

                                CREATE TABLE IF NOT EXISTS Student (
                                    Student_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Student_Name TEXT NOT NULL,
                                    Student_Address TEXT NOT NULL,
                                    Student_NIC TEXT UNIQUE NOT NULL,
                                    Student_Status TEXT NOT NULL,
                                    CS_ID INTEGER,
                                    Batch_ID INTEGER,
                                    User_ID INTEGER,
                                    FOREIGN KEY (User_ID) REFERENCES User (User_ID),
                                    FOREIGN KEY (CS_ID) REFERENCES Course_Subject(CS_ID),
                                    FOREIGN KEY (Batch_ID) REFERENCES Batch(Batch_ID),
                                    CHECK (Student_ID >= 400001 AND Student_ID <= 999999)
                                );

                                CREATE TABLE IF NOT EXISTS Marks (
                                    Exam_ID INTEGER,
                                    Student_ID INTEGER,
                                    Score INTEGER NOT NULL,
                                    FOREIGN KEY (Exam_ID) REFERENCES Exam(Exam_ID),
                                    FOREIGN KEY (Student_ID) REFERENCES Student(Student_ID),
                                    PRIMARY KEY (Exam_ID, Student_ID)
                                );

                                CREATE TABLE IF NOT EXISTS Lecturer (
                                    Lecturer_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Lecturer_Name TEXT NOT NULL,
                                    Lecturer_Address TEXT NOT NULL,
                                    Lecturer_NIC TEXT UNIQUE NOT NULL,
                                    Lecturer_Status TEXT NOT NULL,
                                    Special_In TEXT NOT NULL,
                                    User_ID INTEGER NOT NULL,
                                    FOREIGN KEY (User_ID) REFERENCES User (User_ID),        
                                    CHECK (Lecturer_ID >= 1000001 AND Lecturer_ID <= 1999999)
                                );

                                CREATE TABLE IF NOT EXISTS CS_Lecturer (
                                    CS_ID INTEGER,
                                    Lecturer_ID INTEGER,
                                    PRIMARY KEY (CS_ID, Lecturer_ID),
                                    FOREIGN KEY (CS_ID) REFERENCES Course_Subject(CS_ID),
                                    FOREIGN KEY (Lecturer_ID) REFERENCES Lecturer(Lecturer_ID)
                                );

                                CREATE TABLE IF NOT EXISTS Lecturer_Batch (
                                    Lecturer_ID INTEGER,
                                    Batch_ID INTEGER,
                                    PRIMARY KEY (Lecturer_ID, Batch_ID),
                                    FOREIGN KEY (Lecturer_ID) REFERENCES Lecturer(Lecturer_ID),
                                    FOREIGN KEY (Batch_ID) REFERENCES Batch(Batch_ID)
                                );

                                CREATE TABLE IF NOT EXISTS Staff (
                                    Staff_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Staff_Name TEXT NOT NULL,
                                    Staff_Address TEXT NOT NULL,
                                    Staff_NIC TEXT UNIQUE NOT NULL,
                                    Staff_Status TEXT NOT NULL,
                                    User_ID INTEGER NOT NULL,
                                    FOREIGN KEY (User_ID) REFERENCES User (User_ID),
                                    CHECK (Staff_ID >= 2000001 AND Staff_ID <= 2999999)
                                );

                                CREATE TABLE IF NOT EXISTS Admin (
                                    Admin_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Admin_Name TEXT NOT NULL,
                                    Admin_Address TEXT NOT NULL,
                                    Admin_NIC TEXT UNIQUE NOT NULL,
                                    Admin_Status TEXT NOT NULL,
                                    User_ID INTEGER NOT NULL,
                                    FOREIGN KEY (User_ID) REFERENCES User (User_ID),
                                    CHECK (Admin_ID >= 3000001 AND Admin_ID <= 3000199)
                                );

                                CREATE TABLE IF NOT EXISTS Student_Attendance (
                                    Marked_Student_Att INTEGER NOT NULL,
                                    Marked_Date TEXT NOT NULL,
                                    Attendance_Status TEXT NOT NULL,
                                    Timetable_ID INTEGER,
                                    PRIMARY KEY (Marked_Student_Att, Marked_Date),
                                    FOREIGN KEY (Marked_Student_Att) REFERENCES Student(Student_ID),
                                    FOREIGN KEY (Timetable_ID) REFERENCES Timetable(Timetable_ID)
                                );

                                CREATE TABLE IF NOT EXISTS Lecturer_Attendance (
                                    Marked_Lecturer_Att INTEGER NOT NULL,
                                    Marked_Date TEXT NOT NULL,
                                    Attendance_Status TEXT NOT NULL,
                                    Timetable_ID INTEGER,
                                    PRIMARY KEY (Marked_Lecturer_Att, Marked_Date),
                                    FOREIGN KEY (Marked_Lecturer_Att) REFERENCES Lecturer (Lecturer_ID),
                                    FOREIGN KEY (Timetable_ID) REFERENCES Timetable(Timetable_ID)
                                );

                                 CREATE TABLE IF NOT EXISTS Staff_Attendance (
                                    Marked_Staff_Att INTEGER NOT NULL,
                                    Marked_Date TEXT NOT NULL,
                                    Attendance_Status TEXT NOT NULL,
                                    PRIMARY KEY (Marked_Staff_Att, Marked_Date),
                                    FOREIGN KEY (Marked_Staff_Att) REFERENCES Staff(Staff_ID)
                                );

                                CREATE TABLE IF NOT EXISTS Log_Table (
                                    Log_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                    User_ID INTEGER NOT NULL,
                                    Action TEXT NOT NULL,
                                    Log_Status TEXT NOT NULL,
                                    Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
                                    FOREIGN KEY (User_ID) REFERENCES User(User_ID)
                                );
                                ";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();

                }
            }
        }
    }
}
