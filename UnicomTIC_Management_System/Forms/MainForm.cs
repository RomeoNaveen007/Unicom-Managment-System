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
        private Login_info login_session;


        public MainForm(Login_info user)
        {
            InitializeComponent();
            login_session = user;

            SetButtonVisibilityByRole(login_session.login_role);
            button1.Visible = false;
            
        }

        private void SetButtonVisibilityByRole(string role)
        {
            try
            {
                HideAllButtonsExcept("button17");

                switch (role)
                {
                    case "Admin":
                        ShowAllButtons();
                        break;

                    case "Staff":
                        ShowButtonsInRange("button1", "button10");
                        break;

                    case "Lecturer":
                        ShowButtonsInRange("button1", "button5");
                        break;

                    case "Student":
                        ShowButtonsInRange("button4", "button1"); // reverse order supported
                        break;

                    default:
                        MessageBox.Show("Unknown role: access denied.");
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("UI error: " + ex.Message);
            }
        }

        private void HideAllButtonsExcept(string exceptionName)
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Button btn && btn.Name != exceptionName)
                    btn.Visible = false;
            }
        }

        private void ShowAllButtons()
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Button btn)
                    btn.Visible = true;
            }
        }

        private void ShowButtonsInRange(string startBtn, string endBtn)
        {
            // Sort buttons alphabetically to ensure predictable order
            var buttons = this.Controls.OfType<Button>()
                .OrderBy(b => b.Name)
                .ToList();

            bool inRange = false;

            foreach (var btn in buttons)
            {
                if (btn.Name == startBtn || btn.Name == endBtn)
                {
                    inRange = true;
                    btn.Visible = true;

                    if (startBtn == endBtn)
                        break;

                    continue;
                }

                if (inRange)
                    btn.Visible = true;

                // Exit when we’ve passed both
                if ((btn.Name == endBtn && startBtn != endBtn) ||
                    (btn.Name == startBtn && startBtn != endBtn))
                {
                    break;
                }
            }
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
            label2.Text = $" Welcome {login_session.Login_user}  ";
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
