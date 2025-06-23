using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTIC_Management_System.Service;

namespace UnicomTIC_Management_System.Forms
{
    public partial class Student_Form : Form
    {
        private Student_Services student_services;

        public Student_Form()
        {
            InitializeComponent();
        }



        private void Student_Form_Load(object sender, EventArgs e)
        {

            LoadCourseNames();
            LoadBatchNames();

        }
        private void LoadCourseNames()
        {
            student_services = new Student_Services();
            var service = new Student_Services();
            List<string> courseNames = service.GetActiveCourseNames();

            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(courseNames.ToArray());
            comboBox1.SelectedIndex = -1;
        }

        private void LoadBatchNames()
        {
            student_services = new Student_Services();
            var service = new Student_Services();
            List<string> batches = service.GetActiveBatchNames();

            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(batches.ToArray());
            comboBox2.SelectedIndex = -1;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
