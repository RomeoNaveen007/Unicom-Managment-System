using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTIC_Management_System.Data.log_session;
using UnicomTIC_Management_System.Model;
using UnicomTIC_Management_System.Service;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace UnicomTIC_Management_System.Forms
{
    public partial class CS_Form : Form
    {
        private CS_Services cs_services;
        private int Clicked_CS_ID = -1;

        public CS_Form()
        {
            InitializeComponent();
            LoadCourseSubjectData();
            PopulateCourseComboBox();
            PopulateSubjectComboBox();
            Role_access();

        }

        private void CS_Form_Load(object sender, EventArgs e)
        {
            
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
                button1.Visible = false;
                button2.Visible = false;
                button3.Visible = false;
            }
            else if (login.login_role == "staff" || login.login_role == "Admin")
            {
                button1.Visible = true;
                button2.Visible = true;
                button3.Visible = true;
            }
            else
            {
                MessageBox.Show("Unknown role detected. Please contact the administrator.");
            }
        }


        private void PopulateCourseComboBox()
        {
            try
            {
                cs_services = new CS_Services();
                DataTable courseData = cs_services.LoadCoursesIntoComboBox();

                comboBox1.DataSource = courseData;
                comboBox1.DisplayMember = "Course_Name";
                comboBox1.ValueMember = "Course_ID";
                comboBox1.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load courses: " + ex.Message);
            }
        }

        private void PopulateSubjectComboBox()
        {
            try
            {
                cs_services = new CS_Services();
                DataTable subjectData = cs_services.LoadSubjectsForComboBox();

                comboBox2.DataSource = subjectData;
                comboBox2.DisplayMember = "Subject_Name";
                comboBox2.ValueMember = "Subject_ID";
                comboBox2.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load subjects: " + ex.Message);
            }
        }

        private void ClearInputs()
        {
            comboBox1.Text = "";
            comboBox2.Text = "";

        }
        private void LoadCourseSubjectData()
        {
            cs_services = new CS_Services();
            var data = cs_services.GetAllCourseSubjects();

            if (data == null || data.Count == 0)
            {
                dataGridView1.DataSource = null;
                MessageBox.Show("No course-subject entries found.");
                return;
            }

            dataGridView1.DataSource = data;
            dataGridView1.ReadOnly = true;
            dataGridView1.Columns["CS_ID"].Visible = false;
            dataGridView1.Columns["Course_ID"].Visible = false;
            dataGridView1.Columns["Subject_ID"].Visible = false;
            dataGridView1.ClearSelection();
            ClearInputs();
        }




        private void button1_Click(object sender, EventArgs e)
        {
          
            if (comboBox1.SelectedIndex == -1 || comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Please select both a course and a subject.");
                return;
            }

            int selectedCourseId = Convert.ToInt32(comboBox1.SelectedValue);
            string selectedCourseName = comboBox1.Text;

            int selectedSubjectId = Convert.ToInt32(comboBox2.SelectedValue);
            string selectedSubjectName = comboBox2.Text;

            cs_services = new CS_Services();
            cs_services.AddCourseSubject(selectedCourseId, selectedCourseName, selectedSubjectId, selectedSubjectName);
            LoadCourseSubjectData();

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Only valid data rows (not header)
            {
                object val = dataGridView1.Rows[e.RowIndex].Cells["CS_ID"].Value;
                if (val != null && int.TryParse(val.ToString(), out int clickedCourseSubjectId))
                {
                    Clicked_CS_ID = clickedCourseSubjectId;

                    cs_services = new CS_Services();
                    var selectedCS = cs_services.Get_CourseSubject_By_Id(Clicked_CS_ID);

                    if (selectedCS != null)
                    {
                        // Fill form controls with selected data
                        comboBox1.Text = selectedCS.Course_Name ?? "";
                        comboBox2.Text = selectedCS.Subject_Name ?? "";

                    }
                    else
                    {
                        MessageBox.Show("Course-Subject entry not found!");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Course-Subject ID.");
                }
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Clicked_CS_ID <= 0)
            {
                MessageBox.Show("Select a Course-Subject entry to update.");
                return;
            }

            if (comboBox1.SelectedIndex == -1 || comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Please select both a course and a subject.");
                return;
            }

            var updatedCS = new Course_Subject
            {
                CS_ID = Clicked_CS_ID,
                Course_ID = Convert.ToInt32(comboBox1.SelectedValue),
                Subject_ID = comboBox2.SelectedValue.ToString(),
                Course_Name = comboBox1.Text,
                Subject_Name = comboBox2.Text
            };

            cs_services = new CS_Services();
            bool updated = cs_services.UpdateCourseSubject(updatedCS);
            if (updated)
            {
                MessageBox.Show($"Course-Subject record updated: {updatedCS.Course_Name} ➝ {updatedCS.Subject_Name}");
                LoadCourseSubjectData();
                ClearInputs();
                Clicked_CS_ID = -1;

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
          
            if (Clicked_CS_ID <= 0)
            {
                MessageBox.Show("Select a Course-Subject entry to delete.");
                return;
            }

            DialogResult confirm = MessageBox.Show("Are you sure you want to delete this entry?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (confirm != DialogResult.Yes) return;

            cs_services = new CS_Services();
            bool deleted = cs_services.DeleteCourseSubject(Clicked_CS_ID);
            if (deleted)
            {
                MessageBox.Show("Course-Subject record deleted successfully.");
                LoadCourseSubjectData();
                ClearInputs();
                Clicked_CS_ID = -1;
            }
        }

       
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}


