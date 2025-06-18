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
using UnicomTIC_Management_System.Model;
using UnicomTIC_Management_System.Service;

namespace UnicomTIC_Management_System.Forms
{
    public partial class Admin_Form : Form
    {
        private User _user;
        private string User_role = string.Empty;
        private Admin_Service admin_service;
        private int Clicked_admin_id = -1;
        private User_Service user_Service;

        public Admin_Form()
        {
            InitializeComponent();
            admin_service = new Admin_Service();
            get_admin_info();
        }

        /* public void Findrole()               // shouls add a method in user service table 
         {
             List<User> user = user_Service.Show_All_Users();
             User_role = user.Role;
             User_role = user.Role;
         }*/

        private void get_admin_info()
        {
            List<Admin> admin = admin_service.Show_Output();

            var displayList = admin.Select(a => new
            {
                Admin_ID = a.Admin_ID,
                Admin_Name = a.Admin_Name,
                Admin_Address = a.Admin_Address,
                Admin_NIC = a.Admin_NIC,
                User_Name = a.User_Name
            }).ToList();

            dataGridView1.DataSource = displayList;
            //  dataGridView1.Columns["Admin_ID"].Visible = false;
            dataGridView1.ClearSelection();
            ClearInputs();
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
            //Findrole();

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (comboBox1.Text == "Add")
            {
                string username = textBox5.Text.Trim();

                
                /* if (IsUsernameTaken(username))
                 {
                     MessageBox.Show("Username already exists. Please choose another one.", "Duplicate Username", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                     return;
                 }*/

                Admin admin = new Admin
                {
                    Admin_Name = CapitalizeFirstLetter(textBox1.Text.Trim()),
                    Admin_Address = CapitalizeFirstLetter(textBox2.Text.Trim()),
                    Admin_NIC = textBox3.Text.Trim(),
                    User_Name = CapitalizeFirstLetter(username),
                    Admin_Status = "Active",
                    Password = "User123@",
                    Role = "Admin"
                };

                Admin_Service admin_Service = new Admin_Service();
                admin_Service.Addadmin(admin);

                MessageBox.Show("Admin added successfully!\nDefault password: User123@", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                get_admin_info();                            // Refresh DataGridView...............
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
                    Admin admin = admin_service.Get_Admin_id(adminId);

                    // Populate text boxes with admin details if found
                    if (admin != null)
                    {
                        textBox1.Text = admin.Admin_Name ?? string.Empty;
                        textBox2.Text = admin.Admin_Address ?? string.Empty;
                        textBox3.Text = admin.Admin_NIC ?? string.Empty;
                        textBox5.Text = admin.User_Name ?? string.Empty;

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
    }
}
