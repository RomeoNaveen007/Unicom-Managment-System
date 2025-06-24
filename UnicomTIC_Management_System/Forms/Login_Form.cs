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
    public partial class Login_Form : Form
    {
        public Login_Form()
        {
            InitializeComponent();

        }
       
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Login_Form_Load(object sender, EventArgs e)
        {
            Login_Service login_Service = new Login_Service();
            login_Service.EnsureDefaultAdmin();
        }

        private void Login_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both username and password.");
                return;
            }

            try
            {
                User_Service userService = new User_Service();

                if (userService.Authenticate(username, password))
                {
                    User loggedUser = userService.GetUserByUsername(username);

                    //  Create session info
                    Login user = new Login
                    {
                        Login_user = loggedUser.User_Name,
                        login_role = loggedUser.Role
                    };

                    MessageBox.Show($"Login successful! Welcome, {loggedUser.User_Name} ({loggedUser.Role})");

                    // Launch main form with session
                    MainForm main = new MainForm(user);
                    main.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Invalid credentials.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login error:\n" + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
