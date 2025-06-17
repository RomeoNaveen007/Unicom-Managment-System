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
        private string User_role= string.Empty;
        private Admin_Service admin_service;
        private int Clicked_admin_id = -1;


        public Admin_Form()
        {
            InitializeComponent();
            admin_service = new Admin_Service();
            get_admin_info();
        }

        public void Findrole()               // shouls add a method in user service table 
        {
            _user = new User();
            User_role = _user.Role;
        }

        private void get_admin_info()
        {
            List<Admin> admin = admin_service.show_Output();
            dataGridView1.DataSource = admin;
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

        }

        private string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }

        private bool IsUsernameTaken(string username)
        {
            using (var conn = DB_Config.getConnection())
            {
                string query = "SELECT COUNT(*) FROM Users WHERE LOWER(User_Name) = LOWER(@UserName)";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserName", username);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            string inputUsername = textBox5.Text.Trim();

            if (string.IsNullOrEmpty(inputUsername))
            {
                label5.Text = "";                // optional label to show status
                return;
            }

            if (IsUsernameTaken(inputUsername))
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
            Findrole();

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (User_role == "Admin" && comboBox1.Text == "Add")
            {
                string username = textBox5.Text.Trim();

                if (IsUsernameTaken(username))
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
                    Admin_Status = "Active",
                    Password = "User123@",
                    Role = "Admin"
                };

                Admin_Service admin_Service = new Admin_Service(admin);

                MessageBox.Show("Admin added successfully!\nDefault password: User123@", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                get_admin_info();                            // Refresh DataGridView...............
            }

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Rows[e.RowIndex].Cells["ID"].Value != null)
            {
                try
                {
                    Clicked_admin_id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["ID"].Value);

                    var admin = admin_service.Get_Admin_id(Clicked_admin_id);
                    if (admin != null)
                    {
                        textBox1.Text = admin.Admin_Name;
                        textBox2.Text = admin.Admin_Address;
                        textBox3.Text = admin.Admin_NIC;
                        textBox5.Text = admin.User_Name;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading admin details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void label5_Click(object sender, EventArgs e)
        {


        }
    }
}
