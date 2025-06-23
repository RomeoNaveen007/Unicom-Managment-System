using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTIC_Management_System.Forms;
using UnicomTIC_Management_System.Model;

namespace UnicomTIC_Management_System
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
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
            this.Close();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

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
    }
}
