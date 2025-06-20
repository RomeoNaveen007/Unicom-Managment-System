using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnicomTIC_Management_System.Data.DB_Connection;
using static System.Net.Mime.MediaTypeNames;
using UnicomTIC_Management_System.Model;

namespace UnicomTIC_Management_System.Data.Migration
{
    internal class CreateTable
    {
        public void table_Creation()
        {
            using (var conn = DB_Config.getConnection())
            {
                string query = @" CREATE TABLE IF NOT EXISTS Room(
                Room_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Room_Name TEXT NOT NULL,
                Room_Type TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS User(
                User_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                User_Name TEXT NOT NULL ,
                Password TEXT NOT NULL,
                Role TEXT NOT NULL CHECK(Role IN('Admin', 'Staff', 'Lecturer', 'Student'))
                );

                CREATE TABLE IF NOT EXISTS Course(
                    Course_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Course_Name TEXT NOT NULL UNIQUE,
                    Duration TEXT NOT NULL,
                    Course_Status TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Subject(
                    Subject_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Subject_Name TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Course_Subject(
                    CS_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Subject_ID INTEGER NOT NULL,
                    Course_ID INTEGER NOT NULL,
                    FOREIGN KEY (Subject_ID) REFERENCES Subject(Subject_ID),
                    FOREIGN KEY (Course_ID) REFERENCES Course(Course_ID)
                );

                CREATE TABLE IF NOT EXISTS Exam(
                    Exam_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Exam_type TEXT NOT NULL,
                    Exam_Date DATE NOT NULL,
                    Exam_Duration TEXT NOT NULL,
                    CS_ID INTEGER NOT NULL,
                    FOREIGN KEY (CS_ID) REFERENCES Course_Subject(CS_ID)
                );

                CREATE TABLE IF NOT EXISTS Timetable(
                    Timetable_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Time_Slot TEXT NOT NULL,
                    Timetable_Date DATE NOT NULL,
                    Timetable_Status TEXT NOT NULL,
                    CS_ID INTEGER NOT NULL,
                    Room_ID INTEGER NOT NULL,
                    FOREIGN KEY (Room_ID) REFERENCES Room(Room_ID),
                    FOREIGN KEY (CS_ID) REFERENCES Course_Subject(CS_ID)
                );

                CREATE TABLE IF NOT EXISTS Batch(
                    Batch_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Batch_Name TEXT NOT NULL UNIQUE,
                    Year INTEGER NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Student(
                    Student_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Student_Name TEXT NOT NULL,
                    Student_Address TEXT NOT NULL,
                    Student_NIC TEXT UNIQUE NOT NULL,
                    Student_Status TEXT NOT NULL,
                    CS_ID INTEGER,
                    Batch_ID INTEGER,
                    User_ID INTEGER NOT NULL,
                    FOREIGN KEY (User_ID) REFERENCES User(User_ID),
                    FOREIGN KEY(CS_ID) REFERENCES Course_Subject(CS_ID),
                    FOREIGN KEY(Batch_ID) REFERENCES Batch(Batch_ID)
);

                CREATE TABLE IF NOT EXISTS Marks(
                    Exam_ID INTEGER NOT NULL,
                    Student_ID INTEGER NOT NULL,
                    Score INTEGER NOT NULL,
                    PRIMARY KEY (Exam_ID, Student_ID),
                    FOREIGN KEY (Exam_ID) REFERENCES Exam(Exam_ID),
                    FOREIGN KEY (Student_ID) REFERENCES Student(Student_ID)
                );

                CREATE TABLE IF NOT EXISTS Lecturer(
                    Lecturer_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Lecturer_Name TEXT NOT NULL,
                    Lecturer_Address TEXT NOT NULL,
                    Lecturer_NIC TEXT UNIQUE NOT NULL,
                    Lecturer_Status TEXT NOT NULL,
                    Special_In TEXT NOT NULL,
                    User_ID INTEGER NOT NULL,
                    FOREIGN KEY (User_ID) REFERENCES User(User_ID)
);

                CREATE TABLE IF NOT EXISTS CS_Lecturer(
                    CS_ID INTEGER NOT NULL,
                    Lecturer_ID INTEGER NOT NULL,
                    PRIMARY KEY (CS_ID, Lecturer_ID),
                    FOREIGN KEY (CS_ID) REFERENCES Course_Subject(CS_ID),
                    FOREIGN KEY (Lecturer_ID) REFERENCES Lecturer(Lecturer_ID)
                );

                CREATE TABLE IF NOT EXISTS Lecturer_Batch(
                    Lecturer_ID INTEGER NOT NULL,
                    Batch_ID INTEGER NOT NULL,
                    PRIMARY KEY (Lecturer_ID, Batch_ID),
                    FOREIGN KEY (Lecturer_ID) REFERENCES Lecturer(Lecturer_ID),
                    FOREIGN KEY (Batch_ID) REFERENCES Batch(Batch_ID)
                );

                CREATE TABLE IF NOT EXISTS Staff(
                    Staff_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Staff_Name TEXT NOT NULL,
                    Staff_Address TEXT NOT NULL,
                    Staff_NIC TEXT UNIQUE NOT NULL,
                    Staff_Status TEXT NOT NULL,
                    User_ID INTEGER NOT NULL,
                    User_Name TEXT  NOT NULL,
                    FOREIGN KEY (User_ID) REFERENCES User(User_ID)
);

                CREATE TABLE IF NOT EXISTS Admin(
                    Admin_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Admin_Name TEXT NOT NULL,
                    Admin_Address TEXT NOT NULL,
                    Admin_NIC TEXT UNIQUE NOT NULL,
                    Admin_Status TEXT NOT NULL,
                    User_ID INTEGER NOT NULL,
                    FOREIGN KEY (User_ID) REFERENCES User(User_ID)
);

                CREATE TABLE IF NOT EXISTS Student_Attendance(
                    Marked_Student_Att INTEGER NOT NULL,
                    Marked_Date TEXT NOT NULL,
                    Attendance_Status TEXT NOT NULL,
                    Timetable_ID INTEGER NOT NULL,
                    PRIMARY KEY (Marked_Student_Att, Marked_Date),
                    FOREIGN KEY (Marked_Student_Att) REFERENCES Student(Student_ID),
                    FOREIGN KEY (Timetable_ID) REFERENCES Timetable(Timetable_ID)
                );

                CREATE TABLE IF NOT EXISTS Lecturer_Attendance(
                    Marked_Lecturer_Att INTEGER NOT NULL,
                    Marked_Date TEXT NOT NULL,
                    Attendance_Status TEXT NOT NULL,
                    Timetable_ID INTEGER NOT NULL,
                    PRIMARY KEY (Marked_Lecturer_Att, Marked_Date),
                    FOREIGN KEY (Marked_Lecturer_Att) REFERENCES Lecturer(Lecturer_ID),
                    FOREIGN KEY (Timetable_ID) REFERENCES Timetable(Timetable_ID)
                );

                CREATE TABLE IF NOT EXISTS Staff_Attendance(
                    Marked_Staff_Att INTEGER NOT NULL,
                    Marked_Date TEXT NOT NULL,
                    Attendance_Status TEXT NOT NULL,
                    PRIMARY KEY (Marked_Staff_Att, Marked_Date),
                    FOREIGN KEY (Marked_Staff_Att) REFERENCES Staff(Staff_ID)
                );

                CREATE TABLE IF NOT EXISTS Log_Table(
                    Log_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    User_ID INTEGER NOT NULL,
                    Action TEXT NOT NULL,
                    Log_Status TEXT NOT NULL,
                    Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (User_ID) REFERENCES User(User_ID)
                    );";
                    



                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();

                }
            }
        }
    }
}
