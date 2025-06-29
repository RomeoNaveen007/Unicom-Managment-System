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
using UnicomTIC_Management_System.Forms;
using UnicomTIC_Management_System.Model;
using static System.Collections.Specialized.BitVector32;


namespace UnicomTIC_Management_System
{
    public partial class MainForm : Form 
    {
        
        public MainForm()
        {
            InitializeComponent();
        }

        private  void Button_Access()
        {
            string Role = Login_info.Login_info_role;


            button2.Visible = false;
            button3.Visible = false;
            button4.Visible = false;
            button13.Visible = false;
            button8.Visible = false;
            button5.Visible = false;
            button6.Visible = false;
            button14.Visible = false;
            button9.Visible = false;
            button10.Visible = false;
            button11.Visible = false;
            button12.Visible = false;


            switch (Role)
            {

                case "Admin":
                    ShowAllButtons();
                    break;
                case "Staff":

                    button2.Visible = true;
                    button3.Visible = true;
                    button4.Visible = true;
                    button13.Visible = true;
                    button8.Visible = true;
                    button5.Visible = true;
                    button6.Visible = true;
                    button14.Visible = true;
                    button9.Visible = true;
                    break;

                case "Lecturer":
                    button2.Visible = true;
                    button3.Visible = true;
                    button4.Visible = true;
                    button13.Visible = true;
                    button8.Visible = true;
                    break;

                case "Student":
                    button2.Visible = true;
                    button3.Visible = true;
                    break;

                default:
                    MessageBox.Show("Role not recognized.");
                    break;
            }

        }

        private void ShowAllButtons()
        {
            button2.Visible = true;
            button3.Visible = true;
            button4.Visible = true;
            button13.Visible = true;
            button8.Visible = true;
            button5.Visible = true;
            button6.Visible = true;
            button14.Visible = true;
            button9.Visible = true;
            button10.Visible = true;
            button11.Visible = true;
            button12.Visible = true;
        }

       
        private void OpenFormInPanel(Form OpenForm)
        {
            panel4.Controls.Clear();              // Remove previous form
            OpenForm.TopLevel = false;                  // Important: treat as control
            OpenForm.FormBorderStyle = FormBorderStyle.None;
            OpenForm.Dock = DockStyle.Fill;
            panel4.Controls.Add(OpenForm);      // Add form to panel
            OpenForm.Show();
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFormInPanel(new Timetable_Form());

        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFormInPanel(new Exam_Form());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFormInPanel(new Mark_Form());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFormInPanel(new Student_Form());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFormInPanel(new Subject_Form());
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFormInPanel(new Course_Form());
        }

        private void button7_Click(object sender, EventArgs e)
        {
            OpenFormInPanel(new Attendance_Form());
        }

        private void button8_Click(object sender, EventArgs e)
        {
            OpenFormInPanel(new Lecturer_Form());
        }

        private void button9_Click(object sender, EventArgs e)
        {
            OpenFormInPanel(new Staff_Form());
        }

        private void button10_Click(object sender, EventArgs e)
        {
            OpenFormInPanel(new Room_Form());
        }

        private void button11_Click(object sender, EventArgs e)
        {
            OpenFormInPanel(new Batch_Form());
        }

        private void button12_Click(object sender, EventArgs e)
        {
            OpenFormInPanel(new Admin_Form());
        }

        private void button15_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            label2.Text = $"Welcome to Unicom TIC {Login_info.Login_info_user}";
            label2.ForeColor = Color.Gray;
            Button_Access();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {
            OpenFormInPanel(new CS_Form());

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button14_Click(object sender, EventArgs e)
        {
            OpenFormInPanel(new Lecturer_CS_Form());
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            OpenFormInPanel(new Batch_Form());
        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            OpenFormInPanel(new Admin_Form());
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            OpenFormInPanel(new Room_Form());
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            OpenFormInPanel(new Staff_Form());
        }

        private void button16_Click(object sender, EventArgs e)
        {
          //  OpenFormInPanel(ne());
        }

        private void button17_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
                Login_Form login = new Login_Form();
                login.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Logout failed: " + ex.Message);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
           

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            OpenFormInPanel(new Subject_Form());
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            OpenFormInPanel(new Course_Form());

        }
    }
}
