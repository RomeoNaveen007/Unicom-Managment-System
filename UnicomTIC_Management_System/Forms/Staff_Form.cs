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
            //SetupDataGridView();
            get_STaff_info();
            dataGridView1.CellClick += dataGridView1_CellClick;


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
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            comboBox1.Text = "";
            label6.Text = string.Empty;
            label7.Text = string.Empty;

        }

        private void get_STaff_info()                               //.............DGV Method ............
        {
            dataGridView1.ReadOnly = true;
            List<Staff> stf = new List<Staff>();
            List<Staff> staff = staff_service.Get_All();
            

            foreach (Staff st in staff)
            {
                if (st.Staff_Status == "Active")
                {
                    stf.Add(new Staff
                    {
                        Staff_ID = st.Staff_ID,
                        Staff_Name = st.Staff_Name,
                        Staff_Address = st.Staff_Address,
                        Staff_Status = st.Staff_Status,
                        Staff_NIC = st.Staff_NIC,
                        User_Name = st.User_Name,
                        User_ID = st.User_ID,
                    });
                }
            }
            dataGridView1.DataSource = stf;
            dataGridView1.Columns["Staff_Status"].Visible = false;
            dataGridView1.Columns["Staff_ID"].Visible = false;
            dataGridView1.Columns["User_ID"].Visible = false;
            dataGridView1.Columns["Password"].Visible = false;
            dataGridView1.Columns["Role"].Visible = false;

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
                Staff st = new Staff();
                st.Staff_ID = Clicked_Staf_id;
                st.Staff_Name = CapitalizeFirstLetter(textBox1.Text.Trim());
                st.Staff_Address = CapitalizeFirstLetter(textBox2.Text.Trim());
                st.Staff_NIC = CapitalizeFirstLetter(textBox3.Text.Trim());
                
                staff_service = new Staff_Service();
                staff_service.staff_Update(st);

                get_STaff_info();

            }

            if (comboBox1.Text == "Delete")
            {
                Staff staff = new Staff();
                staff.Staff_ID = Clicked_Staf_id;
                staff.Staff_Name = CapitalizeFirstLetter(textBox1.Text.Trim());
                staff.Staff_Status = "Inactive";

                staff_service = new Staff_Service();
                staff_service.Delete_Staff(staff);

                get_STaff_info();
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
                get_STaff_info();

            }
            else if (comboBox1.Text == "Update" ||  comboBox1.Text == "Delete" )
            {
                textBox5.Visible = false;
                label6.Visible = true;
                get_STaff_info();
            }
            else if (comboBox1.Text == "")
            {
                textBox5.Visible = false;
                label6.Visible = true;
                get_STaff_info();
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

        private void button2_Click(object sender, EventArgs e)
        {
            string search_name = textBox4.Text.Trim();

            if (string.IsNullOrWhiteSpace(search_name))
            {
                MessageBox.Show("Please enter a name to search.");
                return;
            }

            // Get matching staff from DB
            List<Staff> allMatched = staff_service.Get_searched_staff_name(search_name);

            // Filter for only active staff
            List<Staff> activeMatched = allMatched
                .Where(st => st.Staff_Status == "Active")
                .Select(st => new Staff
                {
                    Staff_ID = st.Staff_ID,
                    Staff_Name = st.Staff_Name,
                    Staff_Address = st.Staff_Address,
                    Staff_NIC = st.Staff_NIC,
                    Staff_Status = st.Staff_Status,
                    User_Name = st.User_Name,
                    User_ID = st.User_ID
                })
                .ToList();

            // Update the DataGridView
            dataGridView1.ReadOnly = true;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = activeMatched;

            // Hide unwanted columns if they exist
            if (dataGridView1.Columns.Contains("Staff_Status")) dataGridView1.Columns["Staff_Status"].Visible = false;
            if (dataGridView1.Columns.Contains("Staff_ID")) dataGridView1.Columns["Staff_ID"].Visible = false;
            if (dataGridView1.Columns.Contains("User_ID")) dataGridView1.Columns["User_ID"].Visible = false;
            if (dataGridView1.Columns.Contains("Password")) dataGridView1.Columns["Password"].Visible = false;
            if (dataGridView1.Columns.Contains("Role")) dataGridView1.Columns["Role"].Visible = false;

            dataGridView1.ClearSelection();
            ClearInputs();

            if (activeMatched.Count == 0)
            {
                MessageBox.Show("No active staff found with that name.");
            }
        }
    }    

}
