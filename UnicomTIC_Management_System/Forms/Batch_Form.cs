using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using UnicomTIC_Management_System.Model;
using UnicomTIC_Management_System.Service;
using System.Reflection.Emit;

namespace UnicomTIC_Management_System.Forms
{
    public partial class Batch_Form : Form
    {
        private Batch_Service batchService;
        private int Clicked_Batch_ID = -1;


        public Batch_Form()
        {
            InitializeComponent();
            Get_Batch_Info();
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
            comboBox2.Text = "";
            label5.Text = "";
            
        }

      
        private void Get_Batch_Info()  // Loads batch data into DataGridView
        {
            dataGridView1.ReadOnly = true;

            batchService = new Batch_Service();
            List<Batch> allBatches = batchService.GetAllBatches();

            List<Batch> activeBatches = new List<Batch>();

            foreach (var batch in allBatches)
            {
                if (batch.Batch_Status == "Active")
                {
                    activeBatches.Add(new Batch
                    {
                        Batch_ID = batch.Batch_ID,
                        Batch_Name = batch.Batch_Name,
                        Year = batch.Year,
                        Batch_Status = batch.Batch_Status
                    });
                }
            }

            dataGridView1.DataSource = activeBatches;

            // Hide columns not needed or sensitive
            if (dataGridView1.Columns["Batch_Status"] != null)
                dataGridView1.Columns["Batch_Status"].Visible = false;
            if (dataGridView1.Columns["Batch_ID"] != null)
                dataGridView1.Columns["Batch_ID"].Visible = false;

            dataGridView1.ClearSelection();
            ClearInputs();
        }


        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "yyyy";
            dateTimePicker1.ShowUpDown = true;

            // Restrict to current year and future years:
            dateTimePicker1.MinDate = new DateTime(DateTime.Today.Year, 1, 1);
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void Batch_Form_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox2.Text == "Add")
            {
                Batch batch = new Batch();
                batch.Batch_Name =CapitalizeFirstLetter ( textBox2.Text.Trim());
                batch.Batch_Status = "Active";

                // Get the year from DateTimePicker
                batch.Year = dateTimePicker1.Value.Year;

                batchService = new Batch_Service();
                batchService.AddBatch(batch);

                Get_Batch_Info();
                ClearInputs();
            }

            if (comboBox2.Text == "Update")
            {
                if (Clicked_Batch_ID == -1)
                {
                    MessageBox.Show("Please select a batch from the list first.");
                    ClearInputs();
                    return;
                }

                Batch batch = new Batch();
                batch.Batch_ID = Clicked_Batch_ID;
                batch.Batch_Name = textBox2.Text.Trim();

                // Assuming dateTimePicker1 holds the year for the batch
                batch.Year = dateTimePicker1.Value.Year;

                batchService = new Batch_Service();
                batchService.UpdateBatch(batch);

                Get_Batch_Info();
                Clicked_Batch_ID = -1;
            }

            if (comboBox2.Text == "Delete")
            {
                if (Clicked_Batch_ID == -1)
                {
                    MessageBox.Show("Please select a batch from the list first.");
                    ClearInputs();
                    return;
                }

                Batch batch = new Batch();
                batch.Batch_ID = Clicked_Batch_ID;
                batch.Batch_Status = "Inactive";

                batchService = new Batch_Service();
                batchService.Delete_Batch(batch);

                Get_Batch_Info();
                Clicked_Batch_ID = -1;
            }


        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            string enteredBatchName = textBox2.Text.Trim();
            label5.Visible = true;
            batchService = new  Batch_Service();
            List<Batch> allBatches = new List<Batch>();
            allBatches = batchService.GetAllBatches();

            bool isDuplicate = allBatches.Any(b => b.Batch_Name.Equals(enteredBatchName, StringComparison.OrdinalIgnoreCase));

            if (isDuplicate)
            {
                label5.Text = "Batch name already exists.";
                label5.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                label5.Text = "Batch name is available.";
                label5.ForeColor = System.Drawing.Color.Green;
            }

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Only valid data rows (not header)
            {
                object val = dataGridView1.Rows[e.RowIndex].Cells["Batch_ID"].Value;
                if (val != null && int.TryParse(val.ToString(), out int clickedBatchId))
                {
                    Clicked_Batch_ID = clickedBatchId;

                    batchService = new Batch_Service();
                    var selectedBatch = batchService.GetBatchById(Clicked_Batch_ID);  // You need to implement this method
                    label5.Visible = false;

                    if (selectedBatch != null)
                    {
                        textBox2.Text = CapitalizeFirstLetter(selectedBatch.Batch_Name ?? "");
                        // Set the DateTimePicker value to the year of the selected batch
                        dateTimePicker1.Value = new DateTime(selectedBatch.Year, 1, 1);
                    }
                    else
                    {
                        MessageBox.Show("Batch not found!");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Batch ID.");
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string search_name = textBox1.Text.Trim();

            if (string.IsNullOrWhiteSpace(search_name))
            {
                MessageBox.Show("Please enter a batch name to search.");
                return;
            }

            // Get matching batches from DB
            Batch_Service batchService = new Batch_Service();
            List<Batch> allMatched = batchService.Get_Searched_Batch_Name(search_name);

            // Filter for only active batches
            List<Batch> activeMatched = allMatched
                .Where(batch => batch.Batch_Status == "Active")
                .Select(batch => new Batch
                {
                    Batch_ID = batch.Batch_ID,
                    Batch_Name = batch.Batch_Name,
                    Year = batch.Year,
                    Batch_Status = batch.Batch_Status
                })
                .ToList();

            // Update the DataGridView
            dataGridView1.ReadOnly = true;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = activeMatched;

            // Hide unwanted columns if they exist
            if (dataGridView1.Columns.Contains("Batch_Status")) dataGridView1.Columns["Batch_Status"].Visible = false;
            if (dataGridView1.Columns.Contains("Batch_ID")) dataGridView1.Columns["Batch_ID"].Visible = false;

            dataGridView1.ClearSelection();
            ClearInputs();

            if (activeMatched.Count == 0)
            {
                MessageBox.Show("No active batches found with that name.");
            }

        }
    }
}
