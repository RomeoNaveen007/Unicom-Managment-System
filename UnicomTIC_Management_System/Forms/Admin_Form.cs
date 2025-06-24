using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTIC_Management_System;
using UnicomTIC_Management_System.Controller;
using UnicomTIC_Management_System.Data.DB_Connection;
using UnicomTIC_Management_System.Data.log_session;
using UnicomTIC_Management_System.Model;
using UnicomTIC_Management_System.Service;

namespace UnicomTIC_Management_System.Forms
{
    public partial class Admin_Form : Form
    {
        private Admin_Service admin_service;
        private int Clicked_admin_id = -1;

        public Admin_Form()
        {
            InitializeComponent();
            admin_service = new Admin_Service();
            get_admin_info();
        }

        private void ClearInputs()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox5.Text = "";
            label5.Text = "";
            comboBox1.Text = "";

        }
        private void get_admin_info()
        {
            dataGridView1.ReadOnly = true;
            List<Admin> admins = admin_service.Get_All();

            List<Admin> activeAdmins = admins
                .Where(a => a.Admin_Status == "Active")
                .Select(a => new Admin
                {
                    Admin_ID = a.Admin_ID,
                    Admin_Name = a.Admin_Name,
                    Admin_Address = a.Admin_Address,
                    Admin_NIC = a.Admin_NIC,
                    User_Name = a.User_Name,
                    User_ID = a.User_ID,
                    Password = a.Password,
                    Role = a.Role,
                    Admin_Status = a.Admin_Status
                })
                .ToList();

            dataGridView1.DataSource = activeAdmins;

            // Hide irrelevant columns
            dataGridView1.Columns["Admin_ID"].Visible = false;
            dataGridView1.Columns["User_ID"].Visible = false;
            dataGridView1.Columns["Password"].Visible = false;
            dataGridView1.Columns["Role"].Visible = false;
            dataGridView1.Columns["Admin_Status"].Visible = false;

            dataGridView1.ClearSelection();
            ClearInputs();
        }


        private string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }


        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            string inputUsername = textBox5.Text.Trim();
            User_Service cmd = new User_Service();
            var users = cmd.Show_All_Users();

            bool usernameExists = users.Any(u => u.User_Name.Equals(inputUsername, StringComparison.OrdinalIgnoreCase));

            if (usernameExists)
            {
                label5.Text = "Username already taken";
                label5.ForeColor = Color.Red;
            }
            else
            {
                label5.Text = "Username available";
                label5.ForeColor = Color.Green;
            }
        }



        private void Admin_Form_Load(object sender, EventArgs e)
        {
           

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (comboBox1.Text == "Add")
            {
                string username = textBox5.Text.Trim();
                
                admin_service = new Admin_Service();
                if (admin_service.IsUsernameTaken(username))
                {
                    MessageBox.Show("Username already exists. Please choose another one.", "Duplicate Username", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Admin admin = new Admin
                {
                    Admin_Name = CapitalizeFirstLetter(textBox1.Text.Trim()),
                    Admin_Address = CapitalizeFirstLetter(textBox2.Text.Trim()),
                    Admin_NIC = textBox3.Text.Trim(),
                    User_Name = CapitalizeFirstLetter(username),
                    Password = "User123@",
                    Role = "Admin",
                    Admin_Status = "Active"
                };

                Admin_Service admin_Service = new Admin_Service();
                admin_Service.Addadmin(admin);

                MessageBox.Show("Admin added successfully!\nDefault password: User123@", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                get_admin_info();  // Refresh the admin list/grid
                ClearInputs();
            }

            else if (comboBox1.Text == "Update")
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox3.Text))
                {
                    MessageBox.Show("Please fill in all fields before updating.");
                    return;
                }

                if (Clicked_admin_id == -1)
                {
                    MessageBox.Show("Please select an Admin from the list first.");
                    ClearInputs();
                    return;
                }

                Admin admin = new Admin
                {
                    Admin_ID = Clicked_admin_id,
                    Admin_Name = CapitalizeFirstLetter(textBox1.Text.Trim()),
                    Admin_Address = CapitalizeFirstLetter(textBox2.Text.Trim()),
                    Admin_NIC = label6.Text.Trim()
                };

                Admin_Service admin_Service = new Admin_Service();
                admin_Service.Updateadmin(admin);

                MessageBox.Show("Admin updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                get_admin_info();
                Clicked_admin_id = -1;
                ClearInputs();
            }

            else if (comboBox1.Text == "Delete")
            {

                if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox3.Text))
                {
                    MessageBox.Show("Please fill in all fields before deleting.");
                    return;
                }

                if (Clicked_admin_id == -1)
                {
                    MessageBox.Show("Please select an Admin from the list first.");
                    ClearInputs();
                    return;
                }

                Admin admin = new Admin
                {
                    Admin_ID = Clicked_admin_id,
                    Admin_Status = "Inactive"
                };

                Admin_Service admin_Service = new Admin_Service();
                admin_Service.Deleteadmin(admin);

                MessageBox.Show("Admin deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                get_admin_info();
                ClearInputs();
                Clicked_admin_id = -1;
            }


        }


        private void label5_Click(object sender, EventArgs e)
        {


        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {


        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.RowIndex >= dataGridView1.Rows.Count)
                return;

            try
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                object idValue = row.Cells["Admin_ID"].Value;

                // Check if the Admin_ID cell contains a valid integer
                if (idValue != null && int.TryParse(idValue.ToString(), out int adminId))
                {
                    Clicked_admin_id = adminId;

                    // Initialize the admin service and fetch admin details
                    admin_service = new Admin_Service();
                    Admin admin = admin_service.Get_Admin_By_ID(adminId);

                    // Populate text boxes with admin details if found
                    if (admin != null)
                    {
                        textBox1.Text = CapitalizeFirstLetter(admin.Admin_Name ?? string.Empty);
                        textBox2.Text = CapitalizeFirstLetter(admin.Admin_Address ?? string.Empty);
                        textBox3.Text = CapitalizeFirstLetter(admin.Admin_NIC ?? string.Empty);
                        label6.Text = CapitalizeFirstLetter(admin.User_Name ?? string.Empty);

                    }
                    else
                    {
                        MessageBox.Show("No admin found for this ID.");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Admin ID.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading admin details:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string search_name = textBox4.Text.Trim();

            if (string.IsNullOrWhiteSpace(search_name))
            {
                MessageBox.Show("Please enter a name to search.");
                return;
            }

            // Get matching admins from DB
            List<Admin> allMatched = admin_service.Search_Admin_By_Name(search_name);

            // Filter only active admins
            List<Admin> activeMatched = allMatched
                .Where(ad => ad.Admin_Status == "Active")
                .Select(ad => new Admin
                {
                    Admin_ID = ad.Admin_ID,
                    Admin_Name = ad.Admin_Name,
                    Admin_Address = ad.Admin_Address,
                    Admin_NIC = ad.Admin_NIC,
                    Admin_Status = ad.Admin_Status,
                    User_Name = ad.User_Name,
                    User_ID = ad.User_ID,
                    Password = ad.Password,
                    Role = ad.Role
                })
                .ToList();

            // Update the DataGridView
            dataGridView1.ReadOnly = true;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = activeMatched;

            // Hide unnecessary columns
            if (dataGridView1.Columns.Contains("Admin_Status")) dataGridView1.Columns["Admin_Status"].Visible = false;
            if (dataGridView1.Columns.Contains("Admin_ID")) dataGridView1.Columns["Admin_ID"].Visible = false;
            if (dataGridView1.Columns.Contains("User_ID")) dataGridView1.Columns["User_ID"].Visible = false;
            if (dataGridView1.Columns.Contains("Password")) dataGridView1.Columns["Password"].Visible = false;
            if (dataGridView1.Columns.Contains("Role")) dataGridView1.Columns["Role"].Visible = false;

            dataGridView1.ClearSelection();
            ClearInputs();

            if (activeMatched.Count == 0)
            {
                MessageBox.Show("No active admin found with that name.");
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;

            if (comboBox1.Text == "Add")
            {
                textBox5.Visible = true;
                label6.Visible = false;
                ClearInputs();
                get_admin_info();

            }
            else if (comboBox1.Text == "Update" || comboBox1.Text == "Delete")
            {
                textBox5.Visible = false;
                label6.Visible = true;
                get_admin_info();
            }
            else if (comboBox1.Text == "")
            {
                textBox5.Visible = false;
                label6.Visible = true;
                get_admin_info();
            }
        }
    }
}
