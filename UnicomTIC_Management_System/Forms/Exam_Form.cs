using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTIC_Management_System.Model;
using UnicomTIC_Management_System.Service;

namespace UnicomTIC_Management_System.Forms
{
    public partial class Exam_Form : Form
    {
        private Exam_Services exam_services;
        private Student_Services student_services;
        private int selectedExamID = -1;

        public Exam_Form()
        {
            InitializeComponent();
        }
        private void Exam_Form_Load(object sender, EventArgs e)
        {
            LoadCourses();
            LoadDurations();
            LoadBatchNames();
            LoadExams();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker1.MinDate = DateTime.Today;
        }
        private void LoadBatchNames()
        {
            student_services = new Student_Services(); 
            var batches = student_services.GetActiveBatchNames();
            comboBox5.Items.Clear();
            comboBox5.Items.AddRange(batches.ToArray());
            comboBox5.SelectedIndex = -1;
        }

        private void LoadExams()
        {
            try
            {
                exam_services = new Exam_Services();
                var table = exam_services.GetAllExamsWithDetails();

                dataGridView1.DataSource = table;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.ReadOnly = true;

                // Hide internal ID columns
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    if (col.Name == "Exam_ID" || col.Name == "CS_ID" || col.Name == "Batch_ID")
                        col.Visible = false;
                }

                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load exam records:\n" + ex.Message);
            }
        }


        private void ClearExamFields()
        {
            comboBox1.Text = "";
            comboBox2.SelectedIndex = -1;
            comboBox3.SelectedIndex = -1;
            comboBox4.SelectedIndex = -1;
            comboBox5.SelectedIndex = -1;
            dateTimePicker1.Value = DateTime.Today;
        }

        private void LoadCourses()
        {
            exam_services = new Exam_Services();
            var courses = exam_services.GetCourseNames();
            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(courses.ToArray());
            comboBox2.SelectedIndex = -1;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedCourse = comboBox2.Text;
            exam_services = new Exam_Services();
            var subjects = exam_services.GetSubjectsForCourse(selectedCourse);
            comboBox3.Items.Clear();
            comboBox3.Items.AddRange(subjects.ToArray());
            comboBox3.SelectedIndex = -1;
        }
        private void LoadDurations()
        {
            comboBox4.Items.Clear();
            comboBox4.Items.AddRange(new string[]
            {
                "1.00 hour", "1.30 hours", "2.00 hours", "3.00 hours"
            });
            comboBox4.SelectedIndex = -1;

        }


        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
           

            if (string.IsNullOrWhiteSpace(comboBox1.Text) ||
                comboBox2.SelectedIndex == -1 ||              
                comboBox3.SelectedIndex == -1 ||             
                comboBox4.SelectedIndex == -1 ||              
                comboBox5.SelectedIndex == -1)
            {
                MessageBox.Show("Please complete all required exam details.");
                return;
            }

            try
            {

                exam_services = new Exam_Services();
                int cs_id = exam_services.GetCS_ID(comboBox2.Text, comboBox3.Text);
                int batch_id = exam_services.GetBatchID_FromBatchName(comboBox5.Text);

                if (cs_id <= 0 || batch_id <= 0)
                {
                    MessageBox.Show("Invalid Course-Subject or Batch selection.");
                    return;
                }

                Exam exam = new Exam
                {
                    Exam_type = comboBox1.Text.Trim(),
                    Exam_Date = dateTimePicker1.Value.ToString("yyyy-MM-dd"),
                    Exam_Duration = comboBox4.Text.Trim(),
                    CS_ID = cs_id,
                    Batch_ID = batch_id
                };
                if (exam_services.IsDuplicateExam(exam))
                {
                    MessageBox.Show("This exam already exists for the selected course, subject, batch, and date.");
                    return;
                }
                bool added = exam_services.AddExam(exam);

                if (added)
                {
                    MessageBox.Show("Exam added successfully.");
                    LoadExams();
                    ClearExamFields(); 
                }
                else
                {
                    MessageBox.Show("Exam could not be added.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error:\n" + ex.Message);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                if (row.Cells["Exam_ID"].Value != null &&
                    int.TryParse(row.Cells["Exam_ID"].Value.ToString(), out int examId))
                {
                    selectedExamID = examId;

                    exam_services= new Exam_Services();
                    var exam = exam_services.GetExamById(examId);

                    if (exam != null)
                    {
                        comboBox1.Text = exam.Exam_type;
                        dateTimePicker1.Value = DateTime.TryParse(exam.Exam_Date, out var date)
                            ? date : DateTime.Today;
                        comboBox2.Text = exam.Course_Name;
                        comboBox3.Text = exam.Subject_Name;
                        comboBox4.Text = exam.Exam_Duration;
                        comboBox5.Text = exam.Batch_Name;
                    }
                    else
                    {
                        MessageBox.Show("Exam record not found.");
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (selectedExamID <= 0)
            {
                MessageBox.Show("Select an exam record to update.");
                return;
            }

            if (string.IsNullOrWhiteSpace(comboBox1.Text) || comboBox2.SelectedIndex == -1 ||
                comboBox3.SelectedIndex == -1 || comboBox4.SelectedIndex == -1 || comboBox5.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill in all required fields.");
                return;
            }

            try
            {
                exam_services = new Exam_Services();

                int cs_id = exam_services.GetCS_ID(comboBox2.Text, comboBox3.Text);
                int batch_id = exam_services.GetBatchID_FromBatchName(comboBox5.Text);

                if (cs_id <= 0 || batch_id <= 0)
                {
                    MessageBox.Show("Invalid course-subject or batch selection.");
                    return;
                }

                Exam updatedExam = new Exam
                {
                    Exam_ID = selectedExamID,
                    Exam_type = comboBox1.Text.Trim(),
                    Exam_Date = dateTimePicker1.Value.ToString("yyyy-MM-dd"),
                    Exam_Duration = comboBox4.Text.Trim(),
                    CS_ID = cs_id,
                    Batch_ID = batch_id
                };

                bool success = exam_services.UpdateExam(updatedExam);

                if (success)
                {
                    MessageBox.Show("Exam updated successfully.");
                    LoadExams(); 
                    ClearExamFields();
                    selectedExamID = -1;
                }
                else
                {
                    MessageBox.Show("Failed to update exam.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during exam update:\n" + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (selectedExamID <= 0)
            {
                MessageBox.Show("Please select an exam to delete.");
                return;
            }

            DialogResult confirm = MessageBox.Show("Are you sure you want to delete this exam?", "Confirm", MessageBoxButtons.YesNo);
            if (confirm != DialogResult.Yes) return;

            try
            {
                exam_services = new Exam_Services();
                bool deleted = exam_services.DeleteExamById(selectedExamID);

                if (deleted)
                {
                    MessageBox.Show("Exam deleted successfully.");
                    LoadExams();
                    ClearExamFields();
                    selectedExamID = -1;
                }
                else
                {
                    MessageBox.Show("Unable to delete exam.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting exam:\n" + ex.Message);
            }
        }
    }
}

