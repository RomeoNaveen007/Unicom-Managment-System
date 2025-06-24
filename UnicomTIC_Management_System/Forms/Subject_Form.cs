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

namespace UnicomTIC_Management_System.Forms
{
    public partial class Subject_Form : Form
    {
        private Subject_Service subject_Service;
        private int Clicked_Subject_ID = -1;

        public Subject_Form()
        {
            InitializeComponent();
            Get_Subject_Info();
            Role_access();
        }
       
        private string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }
        private void Role_access()
        {
            Login login = new Login();
            if (login.login_role == "Student")
            {
                comboBox1.Visible = false;


            }
            else if (login.login_role == "Lecturer")
            {
                comboBox1.Visible = false;
            }
            else if (login.login_role == "staff" || login.login_role == "Admin")
            {
                comboBox1.Visible = true;

            }
        }
        private void ClearInputs() 
        {
            comboBox1.Text = string.Empty;
            textBox1.Text= string.Empty;
            label3.Text = string.Empty;
        }

        private void Get_Subject_Info()
        {
            dataGridView1.ReadOnly = true;
            dataGridView1.AutoGenerateColumns = true;

            Subject_Service subjectService = new Subject_Service();
            List<Subject> allSubjects = subjectService.Get_All_Subjects();

            dataGridView1.DataSource = allSubjects;
            dataGridView1.Columns["Subject_ID"].Visible = false;

            dataGridView1.ClearSelection();
            ClearInputs();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;

            if (comboBox1.Text == "Add")
            {
                ClearInputs();
                Get_Subject_Info();        // Refresh DataGridView
            }
            else if (comboBox1.Text == "Update" || comboBox1.Text == "Delete")
            {
                ClearInputs();
                Get_Subject_Info();        // Refresh DataGridView
            }
            else if (string.IsNullOrEmpty(comboBox1.Text))
            {
                Get_Subject_Info();        // Refresh DataGridView
            }
        }

        private void Subject_Form_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string subjectName = CapitalizeFirstLetter(textBox1.Text.Trim());

            if (string.IsNullOrEmpty(subjectName))
            {
                MessageBox.Show("Please enter a subject name.");
                return;
            }

            Subject_Service subjectService = new Subject_Service();

            if (comboBox1.Text == "Add")
            {
                // Check for duplicate subject
                if (subjectService.IsSubjectExists(subjectName))
                {
                    MessageBox.Show("Subject already exists!");
                    return;
                }

                // Check if lecturer exists for subject
                if (subjectService.IsLecturerExistsForSubject(subjectName))
                {
                    Subject subject = new Subject
                    {
                        Subject_Name = subjectName
                    };

                    subjectService.AddSubject(subject);
                    Get_Subject_Info();
                    MessageBox.Show($"{subjectName} added successfully.");
                    ClearInputs();
                }
                else
                {
                    MessageBox.Show("First create a Lecturer for the relevant subject!");
                    ClearInputs();
                }
            }
            else if (comboBox1.Text == "Update")
            {
                if (Clicked_Subject_ID == -1)
                {
                    MessageBox.Show("Please select a subject from the list first.");
                    ClearInputs();
                    return;
                }

                Subject subject = new Subject
                {
                    Subject_ID = Clicked_Subject_ID,
                    Subject_Name = subjectName
                };

                subjectService.Update_Subject(subject);
                Get_Subject_Info();
                label3.Visible = false;
                ClearInputs();

                Clicked_Subject_ID = -1;
            }
            else if (comboBox1.Text == "Delete")
            {

                if (Clicked_Subject_ID == -1)
                {
                    MessageBox.Show("Please select a subject from the list before deleting.");
                    ClearInputs();
                    return;
                }

                Subject subject = new Subject
                {
                    Subject_ID = Clicked_Subject_ID
                };

                subjectService.Delete_Subject(subject);  // Mark inactive or delete
                label3.Visible = false;
                ClearInputs();

                Get_Subject_Info();
                Clicked_Subject_ID = -1;
            }
            else
            {
                MessageBox.Show("Please select a valid operation from the dropdown.");
            }


        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string enteredSubjectName = textBox1.Text.Trim();
            label3.Visible = true;

            if (string.IsNullOrWhiteSpace(enteredSubjectName))
            {
                label3.Text = "Subject name cannot be empty.";
                label3.ForeColor = System.Drawing.Color.Red;
                return;
            }

            Subject_Service subjectService = new Subject_Service(); // Ensure service class is accessible
            List<Subject> allSubjects = subjectService.Get_All_Subjects();

            bool isDuplicate = allSubjects.Any(s =>
                s.Subject_Name.Equals(enteredSubjectName, StringComparison.OrdinalIgnoreCase));

            if (isDuplicate)
            {
                label3.Text = "Subject name already exists.";
                label3.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                label3.Text = "Subject name is available.";
                label3.ForeColor = System.Drawing.Color.Green;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Only valid data rows (not header)
            {
                object val = dataGridView1.Rows[e.RowIndex].Cells["Subject_ID"].Value;
                if (val != null && int.TryParse(val.ToString(), out int clickedSubjectId))
                {
                    Clicked_Subject_ID = clickedSubjectId;  // Make sure this variable is declared in your class

                    Subject_Service subjectService = new Subject_Service();
                    var selectedSubject = subjectService.GetSubjectById(Clicked_Subject_ID);
                    label3.Visible = false;

                    if (selectedSubject != null)
                    {
                        textBox1.Text = CapitalizeFirstLetter(selectedSubject.Subject_Name ?? "");
                    }
                    else
                    {
                        MessageBox.Show("Subject not found!");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Subject ID.");
                }
            }

        }
    }
}
