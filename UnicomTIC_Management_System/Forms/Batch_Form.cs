using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnicomTIC_Management_System.Forms
{
    public partial class Batch_Form : Form
    {
        public Batch_Form()
        {
            InitializeComponent();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "yyyy"; // Show only the year
            dateTimePicker1.ShowUpDown = true;     // Use up/down buttons instead of calendar
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
