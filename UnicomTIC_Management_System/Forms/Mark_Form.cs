using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTIC_Management_System.Data.DB_Connection;
using UnicomTIC_Management_System.Data.log_session;
using UnicomTIC_Management_System.Model;
using UnicomTIC_Management_System.Service;



namespace UnicomTIC_Management_System.Forms
{
    public partial class Mark_Form : Form
    {
        private Mark_Service mark_Service;
        public Mark_Form()
        {
            InitializeComponent();
            Role_access();

        }

        private void Role_access()
        {
            Login login = new Login();
            if (login.login_role == "Student")
            {
                button1.Visible = false;
                button2.Visible = false;
                button3.Visible = false;

            }
            else if (login.login_role == "Lecturer")
            {
                button1.Visible = true;
                button2.Visible = true;
                button3.Visible = false;
            }
            else if (login.login_role == "staff" || login.login_role == "Admin")
            {
                button1.Visible = true;
                button2.Visible = true;
                button3.Visible = true;
            }
        }


        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox4.Items.Clear();

            var students = mark_Service.GetStudentNames(comboBox1.Text, comboBox3.Text, comboBox2.Text);
            comboBox4.Items.AddRange(students.ToArray());
        }

        private void Mark_Form_Load(object sender, EventArgs e)
        {

            mark_Service = new Mark_Service();
            LoadCourses();
            LoadMarks();
        }

        private void LoadCourses()
        {
            var courses = mark_Service.GetCoursesFromCourseSubject();
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(courses.ToArray());
            comboBox1.SelectedIndex = -1;

            comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            comboBox4.Items.Clear();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
            comboBox2.Items.Clear();
            comboBox4.Items.Clear();

            var subjects = mark_Service.GetSubjectsByCourse(comboBox1.Text);
            comboBox3.Items.AddRange(subjects.ToArray());
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            comboBox4.Items.Clear();

            var exams = mark_Service.GetExamTypes(comboBox1.Text, comboBox3.Text);
            comboBox2.Items.AddRange(exams.ToArray());
        }

        private void ClearForm()
        {
            comboBox1.SelectedIndex = -1;
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            comboBox4.Items.Clear();
            textBox1.Clear();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void LoadMarks()
        {
            mark_Service = new Mark_Service();
            var table = mark_Service.GetAllMarks();
            dataGridView1.DataSource = table;

            dataGridView1.Columns["Exam_ID"].HeaderText = "Exam ID";
            dataGridView1.Columns["Exam_type"].HeaderText = "Exam Type";
            dataGridView1.Columns["Course_Name"].HeaderText = "Course";
            dataGridView1.Columns["Subject_Name"].HeaderText = "Subject";
            dataGridView1.Columns["Student_Name"].HeaderText = "Student";
            dataGridView1.Columns["Score"].HeaderText = "Score";
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1 || comboBox2.SelectedIndex == -1 ||
               comboBox3.SelectedIndex == -1 || comboBox4.SelectedIndex == -1 ||
               string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Please complete all fields.");
                return;
            }

            if (!int.TryParse(textBox1.Text.Trim(), out int score) || score < 0 || score > 100)
            {
                MessageBox.Show("Please enter a valid numeric score between 0 and 100.");
                return;
            }

            try
            {
                mark_Service = new Mark_Service();
                int examId = mark_Service.GetExamID(comboBox1.Text, comboBox3.Text, comboBox2.Text);
                int studentId = mark_Service.GetStudentID(comboBox4.Text);

                if (examId <= 0 || studentId <= 0)
                {
                    MessageBox.Show("Invalid exam or student details.");
                    return;
                }

                bool success = mark_Service.AddMark(examId, studentId, score);

                if (success)
                {
                    MessageBox.Show("Mark added successfully.");
                    LoadMarks();
                    ClearForm();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error adding mark:\n" + ex.Message);
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && dataGridView1.Rows[e.RowIndex] != null)
                {
                    var row = dataGridView1.Rows[e.RowIndex];

                    comboBox1.Text = row.Cells["Course_Name"]?.Value?.ToString() ?? "";
                    comboBox3.Text = row.Cells["Subject_Name"]?.Value?.ToString() ?? "";
                    comboBox2.Text = row.Cells["Exam_type"]?.Value?.ToString() ?? "";
                    comboBox4.Text = row.Cells["Student_Name"]?.Value?.ToString() ?? "";
                    textBox1.Text = row.Cells["Score"]?.Value?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load mark details:\n" + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (!int.TryParse(textBox1.Text.Trim(), out int score) || score < 0 || score > 100)
            {
                MessageBox.Show("Please enter a valid score (0–100).");
                return;
            }

            try
            {
                int examId = mark_Service.GetExamID(comboBox1.Text, comboBox3.Text, comboBox2.Text);
                int studentId = mark_Service.GetStudentID(comboBox4.Text);

                if (examId <= 0 || studentId <= 0)
                {
                    MessageBox.Show("Invalid exam or student details.");
                    return;
                }

                bool updated = mark_Service.UpdateMark(examId, studentId, score);

                if (updated)
                {
                    MessageBox.Show("Mark updated successfully.");
                    LoadMarks();
                }
                else
                {
                    MessageBox.Show("No changes made. Mark might not exist.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating mark:\n" + ex.Message);
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            

            var result = MessageBox.Show("Are you sure you want to delete this mark?", "Confirm", MessageBoxButtons.YesNo);
            if (result != DialogResult.Yes) return;

            try
            {
                int examId = mark_Service.GetExamID(comboBox1.Text, comboBox3.Text, comboBox2.Text);
                int studentId = mark_Service.GetStudentID(comboBox4.Text);

                if (examId <= 0 || studentId <= 0)
                {
                    MessageBox.Show("Invalid exam or student selection.");
                    return;
                }

                bool deleted = mark_Service.DeleteMark(examId, studentId);

                if (deleted)
                {
                    MessageBox.Show("Mark deleted successfully.");
                    ClearForm();
                    LoadMarks();
                }
                else
                {
                    MessageBox.Show("Unable to delete. The mark might not exist.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting mark:\n" + ex.Message);
            }
        }

    }
}



