using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTIC_Management_System.Service;
using UnicomTIC_Management_System.Model;

namespace UnicomTIC_Management_System.Forms
{
    public partial class Lecturer_CS_Form : Form
    {
        private CS_Lecturer_Service cs_lecturer_service;
        private bool isInitializing = false;
        private int oldLecturerId = -1;
        private int oldCSId = -1;
        private int newLecturerId = -1;
        private int newCsId = -1;

        public Lecturer_CS_Form()
        {
            InitializeComponent();
            this.Load += Lecturer_CS_Load;
        }

        private void Lecturer_CS_Load(object sender, EventArgs e)
        {
            LoadLecturerNames();
            LoadMappings();
            ClearInputs();
        }
        private void LoadMappings()
        {
            try
            {
                var service = new CS_Lecturer_Service();
                dataGridView1.DataSource = service.GetAllMappingsWithNames();
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading assignments: " + ex.Message);
            }
        }


        private void ClearInputs()
        {
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            oldLecturerId = -1;
            oldCSId = -1;
            comboBox2.Items.Clear();
        }


        private void LoadLecturerNames()
        {
            try
            {
                isInitializing = true;

                cs_lecturer_service = new CS_Lecturer_Service();
                var lecturers = cs_lecturer_service.GetActiveLecturers();

                if (lecturers.Count == 0)
                {
                    MessageBox.Show("No active lecturers found.");
                    comboBox1.DataSource = null;
                    return;
                }

                comboBox1.DataSource = new BindingSource(lecturers, null);
                comboBox1.DisplayMember = "Value";
                comboBox1.ValueMember = "Key";
                comboBox1.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error setting lecturer combo box: " + ex.Message);
            }
            finally
            {
                isInitializing = false;
            }
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isInitializing || comboBox1.SelectedIndex == -1)
                return;

            try
            {
                comboBox2.Items.Clear();

                int selectedLecturerId = ((KeyValuePair<int, string>)comboBox1.SelectedItem).Key;
                cs_lecturer_service = new CS_Lecturer_Service();
                List<string> courseList = cs_lecturer_service.GetCoursesByLecturer(selectedLecturerId);

                if (courseList.Count == 0)
                {
                    MessageBox.Show("No courses found for this lecturer’s specialization.");
                    return;
                }

                comboBox2.Items.AddRange(courseList.ToArray());
                comboBox2.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading courses: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                if (comboBox1.SelectedIndex == -1 || comboBox2.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select both a lecturer and course.");
                    return;
                }

                cs_lecturer_service = new CS_Lecturer_Service();
                int lecturerId = ((KeyValuePair<int, string>)comboBox1.SelectedItem).Key;
                int csId = cs_lecturer_service.GetCS_ID_FromCourseName(comboBox2.Text.Trim());

                cs_lecturer_service = new CS_Lecturer_Service();
                if (cs_lecturer_service.AddMapping(lecturerId, csId))
                {
                    MessageBox.Show("Assignment added successfully.");
                    LoadMappings();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding assignment: " + ex.Message);
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {

            try
            {
                if (oldLecturerId <= 0 || oldCSId <= 0)
                {
                    MessageBox.Show("Please select a mapping to update.");
                    return;
                }

                if (comboBox1.SelectedIndex == -1 || comboBox2.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select both a new lecturer and course.");
                    return;
                }

                int newLecturerId = ((KeyValuePair<int, string>)comboBox1.SelectedItem).Key;
                int newCsId = cs_lecturer_service.GetCS_ID_FromCourseName(comboBox2.Text.Trim());
                if (newLecturerId == oldLecturerId && newCsId == oldCSId)
                {
                    MessageBox.Show("No changes were made.");
                    return;
                }

                cs_lecturer_service = new CS_Lecturer_Service();

                // Prevent duplicate update (e.g., already exists)
                if (!cs_lecturer_service.DeleteMapping(oldLecturerId, oldCSId))
                {
                    MessageBox.Show("Failed to remove old mapping. Cannot update.");
                    return;
                }

                if (!cs_lecturer_service.AddMapping(newLecturerId, newCsId))
                {
                    MessageBox.Show("Update failed: mapping already exists.");
                    return;
                }

                MessageBox.Show("Assignment updated.");
                LoadMappings();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating assignment: " + ex.Message);
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (oldLecturerId <= 0 || oldCSId <= 0)
                {
                    MessageBox.Show("Please select a mapping to delete.");
                    return;
                }

                var service = new CS_Lecturer_Service();
                if (service.DeleteMapping(oldLecturerId, oldCSId))
                {
                    MessageBox.Show("Assignment deleted.");
                    LoadMappings();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting assignment: " + ex.Message);
            }
        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    string lecturerName = dataGridView1.Rows[e.RowIndex].Cells["Lecturer"].Value?.ToString();
                    string courseName = dataGridView1.Rows[e.RowIndex].Cells["Course"].Value?.ToString(); cs_lecturer_service = new CS_Lecturer_Service();
                    cs_lecturer_service = new CS_Lecturer_Service();

                    if (!string.IsNullOrWhiteSpace(lecturerName) && !string.IsNullOrWhiteSpace(courseName))
                    {
                        comboBox1.Text = lecturerName;
                        comboBox2.Text = courseName;

                        oldLecturerId = cs_lecturer_service.GetLecturerID_FromName(lecturerName);
                        oldCSId = cs_lecturer_service.GetCS_ID_FromCourseName(courseName);
                    }
                    else
                    {
                        MessageBox.Show("Could not extract lecturer or course from selected row.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error selecting row: " + ex.Message);
            }
        }


    }

}
