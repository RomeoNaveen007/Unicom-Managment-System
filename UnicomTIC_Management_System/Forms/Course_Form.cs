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
using UnicomTIC_Management_System.Model;
using UnicomTIC_Management_System.Service;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace UnicomTIC_Management_System.Forms
{
    public partial class Course_Form : Form
    {
        private Course_Service courseService;
        private int Clicked_Course_ID = -1;


        public Course_Form()
        {
            InitializeComponent();
            Get_Course_Info();
        }

        private string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }

        private void ClearInputs()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            comboBox1.Text = "";
            label6.Text = "";
        }



        private void Get_Course_Info()
        {
            dataGridView1.ReadOnly = true;

            courseService = new Course_Service();
            List<Course> allCourses = courseService.GetAllCourse();

            
            List<Course> activeCourses = new List<Course>();

            foreach (var course in allCourses)
            {
                if (course.Course_Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                {
                    activeCourses.Add(new Course
                    {
                        Course_ID = course.Course_ID,
                        Course_Name = course.Course_Name,
                        Duration = course.Duration,
                        Course_Status = course.Course_Status
                    });
                }
            }

            dataGridView1.DataSource = activeCourses;

            // Hide columns you don't want the user to see
            dataGridView1.Columns["Course_ID"].Visible = false;
            dataGridView1.Columns["Course_Status"].Visible = false;

            // Optional: customize column headers
            dataGridView1.Columns["Course_Name"].HeaderText = "Course Name";
            dataGridView1.Columns["Duration"].HeaderText = "Duration";
            

            dataGridView1.ClearSelection();
            ClearInputs();  // Your existing method to clear form inputs
        }


        private void Course_Form_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1 .Text == "Add")
            {
                Course course = new Course();
                course.Course_Name =  CapitalizeFirstLetter(textBox1.Text.Trim());
                course.Duration = CapitalizeFirstLetter(textBox2.Text.Trim());
                course.Course_Status ="Active";

                courseService = new Course_Service();
                courseService.AddCourse(course);
                Get_Course_Info();
                ClearInputs();

            }

            if (comboBox1.Text == "Update")
            {
                if (Clicked_Course_ID == -1)
                {
                    MessageBox.Show("Please select a course from the list first.");
                    ClearInputs();
                    label6.Visible = false;
                    return;
                }

                Course course = new Course();
                course.Course_ID = Clicked_Course_ID;
                course.Course_Name = CapitalizeFirstLetter(textBox1.Text.Trim());
                course.Duration = CapitalizeFirstLetter(textBox2.Text.Trim());

                courseService = new Course_Service();
                courseService.Update_Course(course);
                Get_Course_Info();
                label6.Visible = false;
                Clicked_Course_ID = -1;
            }


            if (comboBox1.Text == "Delete")
            {

                if (Clicked_Course_ID == -1)
                {
                    MessageBox.Show("Please select a course from the list first.");
                    ClearInputs();
                    label6.Visible = false;
                    return;
                }

                Course course = new Course();
                course.Course_ID = Clicked_Course_ID;
                course.Course_Status = "Inactive";

                courseService = new Course_Service();
                courseService.Delete_Course(course);
                Get_Course_Info();
                label6.Visible = false;
                Clicked_Course_ID = -1;

            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
                string enteredCourseName = textBox1.Text.Trim();
                label6.Visible = true;
                List<Course> allCourses = new List<Course>();
                allCourses=courseService.GetAllCourse();
               
            bool isDuplicate = allCourses.Any(c => c.Course_Name.Equals(enteredCourseName, StringComparison.OrdinalIgnoreCase));

                if (isDuplicate)
                {
                    label6.Text = "Course name already exists.";
                    label6.ForeColor = System.Drawing.Color.Red;
            }
                else
                {
                    label6.Text = "Course name is available.";
                    label6.ForeColor = System.Drawing.Color.Green;
                }
            

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Only valid data rows (not header)
            {
                object val = dataGridView1.Rows[e.RowIndex].Cells["Course_ID"].Value;
                if (val != null && int.TryParse(val.ToString(), out int clickedCourseId))
                {
                    Clicked_Course_ID = clickedCourseId;
                    

                    courseService = new Course_Service();
                    var selectedCourse = courseService.GetCourseById(Clicked_Course_ID);  // You need to implement this method
                    label6.Visible = false;

                    if (selectedCourse != null)
                    {
                        textBox1.Text = CapitalizeFirstLetter(selectedCourse.Course_Name ?? "");
                        textBox2.Text = CapitalizeFirstLetter(selectedCourse.Duration ?? "");
                        
                    }
                    else
                    {
                        MessageBox.Show("Course not found!");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Course ID.");
                }
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;

            if (comboBox1.Text == "Add")
            {
                
                Get_Course_Info();        // Refresh DataGridView
            }
            else if (comboBox1.Text == "Update" || comboBox1.Text == "Delete")
            {
               
                Get_Course_Info();        // Refresh DataGridView
            }
            else if (string.IsNullOrEmpty(comboBox1.Text))
            {
                
                Get_Course_Info();        // Refresh DataGridView
            }
        }
    }
}
