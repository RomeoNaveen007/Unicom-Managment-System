using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTIC_Management_System.Data.log_session;
using UnicomTIC_Management_System.Model;
using UnicomTIC_Management_System.Service;

namespace UnicomTIC_Management_System.Forms
{
    public partial class Student_Form : Form
    {
        private Student_Services student_services;
        private int selectedStudentID = -1;
        public Student_Form()
        {
            InitializeComponent();
            LoadStudents();

        }

        private void Student_Form_Load(object sender, EventArgs e)
        {
            label9.Visible= false;
            LoadCourseNames();
            LoadBatchNames();

        }
        private string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;
            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }
        private void ClearInputs()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            label9.Text = "";
            label9.Visible = false;
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
        }


        private void LoadStudents()
        {
            try
            {
                student_services = new Student_Services();
                var students = student_services.GetAllStudentsWithNames();

                dataGridView1.DataSource = students;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.ReadOnly = true;

                // Hide columns: IDs and Status
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    if (col.Name == "Student_ID" || col.Name == "Student_Status" || col.Name == "CS_ID" || col.Name == "Batch_ID" || col.Name == "User_ID")
                    {
                        col.Visible = false;
                    }
                }

                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading students: " + ex.Message);
            }
        }


        private void LoadCourseNames()
        {
            student_services = new Student_Services();
            var service = new Student_Services();
            List<string> courseNames = service.GetActiveCourseNames();

            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(courseNames.ToArray());
            comboBox1.SelectedIndex = -1;
        }

        private void LoadBatchNames()
        {
            student_services = new Student_Services();
            var service = new Student_Services();
            List<string> batches = service.GetActiveBatchNames();

            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(batches.ToArray());
            comboBox2.SelectedIndex = -1;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text) ||
                comboBox1.SelectedIndex == -1 ||
                comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Please complete all required student details.");
                ClearInputs();
                return;
            }

            student_services = new Student_Services();

            try
            {
                Student student = new Student
                {
                    Student_Name = CapitalizeFirstLetter(textBox1.Text.Trim()),
                    Student_Address = CapitalizeFirstLetter(textBox2.Text.Trim()),
                    Student_NIC = textBox3.Text.Trim(),
                    Student_Status = "Active",
                    CS_ID = student_services.GetCS_ID_FromCourseName(comboBox1.Text.Trim()),
                    Batch_ID = student_services.GetBatchID_FromBatchName(comboBox2.Text.Trim()),
                };

                User user = new User
                {
                    User_Name = CapitalizeFirstLetter(textBox4.Text.Trim()),
                    Password = "Student123@",
                    Role = "Student"
                };

                //...................... Add to database using service
                var service = new Student_Services();
                bool success = service.AddStudentWithUser(student, user);

                if (success)
                {
                    MessageBox.Show($"Student '{student.Student_Name}' added.\nPassword: {user.Password}");
                    ClearInputs();
                    LoadStudents();
                }
                else
                {
                    MessageBox.Show("Student could not be added. Possibly duplicate NIC or username.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error while adding student:\n" + ex.Message);
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

            string enteredUsername = CapitalizeFirstLetter(textBox4.Text.Trim());

            if (string.IsNullOrWhiteSpace(enteredUsername))
            {
                label8.Text = "";
                return;
            }

            User_Service cmd = new User_Service();
            var users = cmd.Show_All_Users();

            bool usernameExists = users.Any(u => u.User_Name.Equals(enteredUsername, StringComparison.OrdinalIgnoreCase));

            if (usernameExists)
            {
                label8.Text = "Username already taken";
                label8.ForeColor = Color.Red;
            }
            else
            {
                label8.Text = "Username available";
                label8.ForeColor = Color.Green;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                object val = row.Cells["Student_ID"].Value;

                if (val != null && int.TryParse(val.ToString(), out int id))
                {
                    selectedStudentID = id;

                    student_services = new Student_Services();
                    var student = student_services.GetStudentById(id);

                    if (student != null)
                    {
                        textBox1.Text = student.Student_Name;
                        textBox2.Text = student.Student_Address;
                        textBox3.Text = student.Student_NIC;
                        comboBox1.Text = student.Course_Name;
                        comboBox2.Text = student.Batch_Name;
                        label9.Visible = true;
                        label9.Text = student.User_Name ?? "N/A";
                    }
                    else
                    {
                        MessageBox.Show("Could not load student details.");
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (selectedStudentID <= 0)
            {
                MessageBox.Show("Please select a student to update.");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                comboBox1.SelectedIndex == -1 ||
                comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Please complete all fields before updating.");
                return;
            }

            try
            {
                var service = new Student_Services();

                var updatedStudent = new Student
                {
                    Student_ID = selectedStudentID,
                    Student_Name = CapitalizeFirstLetter(textBox1.Text.Trim()),
                    Student_Address = CapitalizeFirstLetter(textBox2.Text.Trim()),
                    Student_NIC = textBox3.Text.Trim(),
                    Student_Status = "Active", // assumed always active after update
                    CS_ID = service.GetCS_ID_FromCourseName(comboBox1.Text.Trim()),
                    Batch_ID = service.GetBatchID_FromBatchName(comboBox2.Text.Trim())
                };

                bool updated = service.UpdateStudentDetails(updatedStudent);

                if (updated)
                {
                    MessageBox.Show("Student updated successfully.");
                    LoadStudents();
                    ClearInputs();
                    label9.Text = "";
                    selectedStudentID = -1;
                }
                else
                {
                    MessageBox.Show("Student update failed.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during student update:\n" + ex.Message);
            }
        }

        

        private void button3_Click(object sender, EventArgs e)
        {
           

            if (selectedStudentID < 0)
            {
                MessageBox.Show("Please select a student record to deactivate.");
                return;
            }

            DialogResult confirm = MessageBox.Show("Mark this student as inactive?", "Confirm", MessageBoxButtons.YesNo);
            if (confirm != DialogResult.Yes) return;

            try
            {
                student_services = new Student_Services();
                bool deactivated = student_services.DeactivateStudentById(selectedStudentID);

                if (deactivated)
                {
                    MessageBox.Show("Student successfully Deleted.");
                    LoadStudents();
                    ClearInputs();
                    selectedStudentID = -1;
                }
                else
                {
                    MessageBox.Show("Failed to Delete student.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error:\n" + ex.Message);
            }
        }
    }
}

