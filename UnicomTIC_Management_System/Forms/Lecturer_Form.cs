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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace UnicomTIC_Management_System.Forms
{
    public partial class Lecturer_Form : Form
    {
        private int Clicked_Lecturer_ID = -1;
        private Lecturer_Service lecturerService;

        public Lecturer_Form()
        {
            InitializeComponent();
            Get_Lecturer_Info();
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
                comboBox2.Visible = false;

            }
            else if (login.login_role == "Lecturer")
            {
                comboBox2.Visible = false;

            }
            else if (login.login_role == "staff" || login.login_role == "Admin")
            {
                comboBox2.Visible = true;

            }
        }
        private void ClearInputs()
        {
            textBox1.Text = ""; // Lecturer_Name
            textBox2.Text = ""; // Lecturer_Address
            textBox3.Text = ""; // Lecturer_NIC
            textBox4.Text = ""; // Maybe Special_In or other field
            textBox5.Text = ""; // User_Name
            textBox6.Text = "";
            comboBox2.Text = "";
            label7.Text = "";
            label8.Text = "";
        }

        private void Get_Lecturer_Info()  // Loads lecturer data into DataGridView
        {
            dataGridView1.ReadOnly = true;

            lecturerService = new Lecturer_Service();
            List<Lecturer> allLecturers = lecturerService.Get_All_Lecturers();

            List<Lecturer> activeLecturers = new List<Lecturer>();

            foreach (var lecturer in allLecturers)
            {
                if (lecturer.Lecturer_Status == "Active")
                {
                    activeLecturers.Add(new Lecturer
                    {
                        Lecturer_ID = lecturer.Lecturer_ID,
                        Lecturer_Name = lecturer.Lecturer_Name,
                        Lecturer_Address = lecturer.Lecturer_Address,
                        Lecturer_NIC = lecturer.Lecturer_NIC,
                        Lecturer_Status = lecturer.Lecturer_Status,
                        Special_In = lecturer.Special_In,
                        User_Name = lecturer.User_Name,
                        User_ID = lecturer.User_ID,
                        Role = lecturer.Role,
                        Password = lecturer.Password // This will be hidden below
                    });
                }
            }

            dataGridView1.DataSource = activeLecturers;

            // Hide sensitive or irrelevant columns
            dataGridView1.Columns["Lecturer_Status"].Visible = false;
            dataGridView1.Columns["Lecturer_ID"].Visible = false;
            dataGridView1.Columns["User_ID"].Visible = false;
            dataGridView1.Columns["Password"].Visible = false;
            dataGridView1.Columns["Role"].Visible = false;

            dataGridView1.ClearSelection();
            ClearInputs();
        }




        private void button2_Click(object sender, EventArgs e)
        {
            string search_name = textBox6.Text.Trim();

            if (string.IsNullOrWhiteSpace(search_name))
            {
                MessageBox.Show("Please enter a name to search.");
                return;
            }

            // Get matching lecturers from DB
            lecturerService = new Lecturer_Service();
            List<Lecturer> allMatched = lecturerService.Get_Searched_Lecturer_Name(search_name);

            // Filter for only active lecturers
            List<Lecturer> activeMatched = allMatched
                .Where(lec => lec.Lecturer_Status == "Active")
                .Select(lec => new Lecturer
                {
                    Lecturer_ID = lec.Lecturer_ID,
                    Lecturer_Name = lec.Lecturer_Name,
                    Lecturer_Address = lec.Lecturer_Address,
                    Lecturer_NIC = lec.Lecturer_NIC,
                    Lecturer_Status = lec.Lecturer_Status,
                    Special_In = lec.Special_In,
                    User_Name = lec.User_Name,
                    User_ID = lec.User_ID
                })
                .ToList();

            // Update the DataGridView
            dataGridView1.ReadOnly = true;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = activeMatched;

            // Hide unwanted columns if they exist
            if (dataGridView1.Columns.Contains("Lecturer_Status")) dataGridView1.Columns["Lecturer_Status"].Visible = false;
            if (dataGridView1.Columns.Contains("Lecturer_ID")) dataGridView1.Columns["Lecturer_ID"].Visible = false;
            if (dataGridView1.Columns.Contains("User_ID")) dataGridView1.Columns["User_ID"].Visible = false;
            if (dataGridView1.Columns.Contains("Password")) dataGridView1.Columns["Password"].Visible = false;
            if (dataGridView1.Columns.Contains("Role")) dataGridView1.Columns["Role"].Visible = false;

            dataGridView1.ClearSelection();
            ClearInputs();

            if (activeMatched.Count == 0)
            {
                MessageBox.Show("No active lecturers found with that name.");
            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Lecturer_Form_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Only valid data rows (not header)
            {
                object val = dataGridView1.Rows[e.RowIndex].Cells["Lecturer_ID"].Value;
                if (val != null && int.TryParse(val.ToString(), out int clickedLecturerId))
                {
                    Clicked_Lecturer_ID = clickedLecturerId;

                    lecturerService = new Lecturer_Service();
                    var selectedLecturer = lecturerService.Get_Lecturer_By_Id(Clicked_Lecturer_ID);

                    if (selectedLecturer != null)
                    {
                        textBox1.Text = CapitalizeFirstLetter(selectedLecturer.Lecturer_Name ?? "");
                        textBox2.Text = CapitalizeFirstLetter(selectedLecturer.Lecturer_Address ?? "");
                        textBox3.Text = CapitalizeFirstLetter(selectedLecturer.Lecturer_NIC ?? "");
                        textBox4.Text = CapitalizeFirstLetter(selectedLecturer.Special_In ?? "");
                        label8.Text = CapitalizeFirstLetter(selectedLecturer.User_Name ?? "");
                    }
                    else
                    {
                        MessageBox.Show("Lecturer not found!");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Lecturer ID.");
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (comboBox2.Text == "Add")
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                    string.IsNullOrWhiteSpace(textBox2.Text) ||
                    string.IsNullOrWhiteSpace(textBox3.Text) ||
                    string.IsNullOrWhiteSpace(textBox4.Text) ||
                    string.IsNullOrWhiteSpace(textBox5.Text))
                {
                    MessageBox.Show("Please fill in all lecturer details.");
                    return;
                }

                try
                {
                    Lecturer lecturer = new Lecturer
                    {
                        Lecturer_Name = CapitalizeFirstLetter(textBox1.Text.Trim()),
                        Lecturer_Address = CapitalizeFirstLetter(textBox2.Text.Trim()),
                        Lecturer_NIC = CapitalizeFirstLetter(textBox3.Text.Trim()),
                        Special_In = CapitalizeFirstLetter(textBox4.Text.Trim()),
                        User_Name = CapitalizeFirstLetter(textBox5.Text.Trim()),
                        Password = "Lecturer123@",
                        Role = "Lecturer",
                        Lecturer_Status = "Active"
                    };

                    new Lecturer_Service().AddLecturer(lecturer);
                    MessageBox.Show($"User Password is {lecturer.Password}");
                    Get_Lecturer_Info(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unexpected error while adding lecturer:\n" + ex.Message);
                }
            }



            if (comboBox2.Text == "Update")
            {
                if (Clicked_Lecturer_ID == -1)
                {
                    MessageBox.Show("Please select a Lecturer from the list before deleting.");
                    ClearInputs();
                    return;
                }
                Lecturer lecturer = new Lecturer();
                lecturer.Lecturer_ID = Clicked_Lecturer_ID;
                lecturer.Lecturer_Name = CapitalizeFirstLetter(textBox1.Text.Trim());
                lecturer.Lecturer_Address = CapitalizeFirstLetter(textBox2.Text.Trim());
                lecturer.Lecturer_NIC = CapitalizeFirstLetter(textBox3.Text.Trim());
                lecturer.Special_In = CapitalizeFirstLetter(textBox4.Text.Trim());
                lecturer.Lecturer_Status = "Active";
                lecturerService = new Lecturer_Service();
                lecturerService.Update_Lecturer(lecturer); // Use an appropriate method name

                Get_Lecturer_Info();
                Clicked_Lecturer_ID = -1;
            }

            if (comboBox2.Text == "Delete")
            {

                if (Clicked_Lecturer_ID == -1)
                {
                    MessageBox.Show("Please select a Lecturer from the list before deleting.");
                    ClearInputs();
                    return;
                }

                Lecturer lecturer = new Lecturer();
                lecturer.Lecturer_ID = Clicked_Lecturer_ID;
                lecturer.Lecturer_Status = "Inactive";

                lecturerService = new Lecturer_Service();
                lecturerService.Delete_Lecturer(lecturer); // Use soft delete logic

                Get_Lecturer_Info();
                Clicked_Lecturer_ID = -1;

            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;

            if (comboBox2.Text == "Add")
            {
                textBox5.Visible = true;    // Show User_Name input when adding
                label8.Visible = false; 
                label7.Visible = true;
                ClearInputs();              // Clear form inputs
                Get_Lecturer_Info();        // Refresh DataGridView
            }
            else if (comboBox2.Text == "Update" || comboBox2.Text == "Delete")
            {
                textBox5.Visible = false;   // Hide User_Name input when updating or deleting
                label8.Visible = true;     
                Get_Lecturer_Info();        // Refresh DataGridView
            }
            else if (string.IsNullOrEmpty(comboBox2.Text))
            {
                textBox5.Visible = false;   // Hide User_Name input if no selection
                label8.Visible = true;      
                Get_Lecturer_Info();        // Refresh DataGridView
            }


        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            string inputUsername = textBox5.Text.Trim();
            User_Service cmd = new User_Service();
            var users = cmd.Show_All_Users();
            
            bool usernameExists = users.Any(u => u.User_Name.Equals(inputUsername, StringComparison.OrdinalIgnoreCase));

            if (usernameExists)
            {
                label7.Text = "Username already taken";
                label7.ForeColor = Color.Red;
            }
            else
            {
                label7.Text = "Username available";
                label7.ForeColor = Color.Green;
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }
}
