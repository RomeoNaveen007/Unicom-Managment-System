using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTIC_Management_System.Controller;
using UnicomTIC_Management_System.Model;
using UnicomTIC_Management_System.Service;


namespace UnicomTIC_Management_System.Forms
{
    public partial class Staff_Form : Form
    {
        private int Clicked_Staf_id = -1;
        private Staff_Service staff_service;

        public Staff_Form()
        {
            InitializeComponent();
            textBox5.Visible = false;
            staff_service = new Staff_Service();
            SetupDataGridView();
            get_STaff_info();

        }

        private string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }

        private void SetupDataGridView()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            // Staff_ID
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Staff ID",
                DataPropertyName = "Staff_ID",
                Name = "Staff_ID"
            });

            // Staff_Name
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Staff Name",
                DataPropertyName = "Staff_Name",
                Name = "Staff_Name"
            });

            // Staff_Address
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Staff Address",
                DataPropertyName = "Staff_Address",
                Name = "Staff_Address"
            });

            // Staff_NIC
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Staff NIC",
                DataPropertyName = "Staff_NIC",
                Name = "Staff_NIC"
            });

            // User_Name
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "User Name",
                DataPropertyName = "User_Name",
                Name = "User_Name"
            });
        }

        private void ClearInputs()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox5.Text = "";
            comboBox1.Text = "";
        }

        private void get_STaff_info()
        {
            dataGridView1.ReadOnly = true;

            List<Staff> staff = staff_service.Get_All();
            dataGridView1.DataSource = staff;

            // Hide unwanted columns
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                if (col.Name != "Staff_Name" && col.Name != "Staff_Address" &&
                    col.Name != "Staff_NIC" && col.Name != "User_Name")
                {
                    col.Visible = false;
                }
            }

            dataGridView1.ClearSelection();
            ClearInputs();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void textBox4_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "Add")
            { 


                Staff staff = new Staff();
                staff.Staff_Name = CapitalizeFirstLetter(textBox1.Text.Trim());
                staff.Staff_Address = CapitalizeFirstLetter(textBox2.Text.Trim());
                staff.Staff_NIC = CapitalizeFirstLetter(textBox3.Text.Trim());
                staff.User_Name = CapitalizeFirstLetter(textBox5.Text.Trim());
                staff.Password = "Staff123@";
                staff.Role = "Staff";
                staff.Staff_Status = "Active";

                Staff_Service staff_Service = new Staff_Service();
                staff_Service.GetAll_staff(staff);

                get_STaff_info();
            }

            if (comboBox1.Text == "Update")
            {
                staff_service = new Staff_Service();
                //staff_service.

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

            }
            else if (comboBox1.Text == "Update" ||  comboBox1.Text == "Delete" )
            {
                textBox5.Visible = false;
                label6.Visible = true;
            }
            else if (comboBox1.Text == "")
            {
                textBox5.Visible = false;
                label6.Visible = true;
            }
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void Staff_Form_Load(object sender, EventArgs e)
        {

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

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= -1) // include first row
            {
                Clicked_Staf_id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Staff_ID"].Value);

                var selectedStaff = staff_service.Get_Staff_id(Clicked_Staf_id);

                if (selectedStaff != null)
                {
                    textBox1.Text = CapitalizeFirstLetter(selectedStaff.Staff_Name);
                    textBox2.Text = CapitalizeFirstLetter( selectedStaff.Staff_Address); 
                    textBox3.Text = CapitalizeFirstLetter (selectedStaff.Staff_NIC);
                    label6.Text = CapitalizeFirstLetter( selectedStaff.User_Name);


                }
                else
                {
                    MessageBox.Show("Staff not found!");
                }
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
