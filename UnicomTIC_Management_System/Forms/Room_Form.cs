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
using UnicomTIC_Management_System.Model;
using UnicomTIC_Management_System.Service;

namespace UnicomTIC_Management_System.Forms
{
    public partial class Room_Form : Form
    {
        private Room_Service roomService;
        private int Clicked_Room_ID = -1; 


        public Room_Form()
        {
            InitializeComponent();
            Roomtype();
            LoadRoomData();
        }

        private void Roomtype()
        {
            comboBox1.Items.Add("Lecturer Hall");
            comboBox1.Items.Add("Computer Lab");
            comboBox1.SelectedIndex = -1;
        }
        private void Room_Form_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();             // Clear any previous items

            string selectedType = comboBox1.SelectedItem?.ToString();

            if (selectedType == "Lecturer Hall")
            {
                comboBox2.Items.Add("Hall A");
                comboBox2.Items.Add("Hall B");
            }
            else if (selectedType == "Computer Lab")
            {
                comboBox2.Items.Add("Lab 1");
                comboBox2.Items.Add("Lab 2");
            }

            comboBox2.SelectedIndex = -1;
        }

        private void LoadRoomData()
        {
            try
            {
                roomService = new Room_Service(); 
                var roomList = roomService.GetAllRooms(); 
                dataGridView1.DataSource = roomList;

                dataGridView1.ClearSelection();
                dataGridView1.ReadOnly = true;
                dataGridView1.Columns["Room_ID"].Visible = false; 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading rooms: " + ex.Message);
            }
        }

        private void ClearInputs()
        {
            comboBox1.SelectedIndex = -1; 
            comboBox2.SelectedIndex = -1;
            comboBox2.Items.Clear();
            Clicked_Room_ID = -1;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedIndex == -1 || comboBox2.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select both Room Type and Room Name.");
                    return;
                }

                Room newRoom = new Room
                {
                    Room_Type = comboBox1.Text.Trim(),
                    Room_Name = comboBox2.Text.Trim()
                };

                roomService = new Room_Service(); 
                bool added = roomService.AddRoom(newRoom);

                if (added)
                {
                    MessageBox.Show($"Room added successfully: {newRoom.Room_Type} ➝ {newRoom.Room_Name}");
                    LoadRoomData();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error while adding room: " + ex.Message);
            }
        }

        
        private void button2_Click(object sender, EventArgs e)
        {

            try
            {
                roomService = new Room_Service();

                if (Clicked_Room_ID <= 0)
                {
                    MessageBox.Show("Please select a room to update.");
                    return;
                }

                Room updatedRoom = new Room
                {
                    Room_ID = Clicked_Room_ID,
                    Room_Type = comboBox1.Text,
                    Room_Name = comboBox2.Text
                };

                if (roomService.UpdateRoom(updatedRoom))
                {
                    MessageBox.Show($"Room updated successfully: {updatedRoom.Room_Type} ➝ {updatedRoom.Room_Name}");
                    LoadRoomData();
                    ClearInputs();
                }
                else
                {
                    MessageBox.Show("Failed to update the room.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while updating room: " + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                roomService = new Room_Service();

                if (Clicked_Room_ID <= 0)
                {
                    MessageBox.Show("Please select a room to delete.");
                    return;
                }

                DialogResult confirm = MessageBox.Show("Are you sure you want to delete this room?", "Confirm Delete", MessageBoxButtons.YesNo);
                if (confirm != DialogResult.Yes) return;

                Room deletedRoom = new Room
                {
                    Room_ID = Clicked_Room_ID,
                    Room_Type = comboBox1.Text,
                    Room_Name = comboBox2.Text
                };

                if (roomService.DeleteRoom(deletedRoom.Room_ID))
                {
                    MessageBox.Show($"Room deleted successfully: {deletedRoom.Room_Type} ➝ {deletedRoom.Room_Name}");
                    LoadRoomData();
                    ClearInputs();
                    Clicked_Room_ID = -1;
                }
                else
                {
                    MessageBox.Show("Failed to delete the room.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while deleting room: " + ex.Message);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) 
            {
                object val = dataGridView1.Rows[e.RowIndex].Cells["Room_ID"].Value;
                if (val != null && int.TryParse(val.ToString(), out int clickedRoomId))
                {
                    Clicked_Room_ID = clickedRoomId;

                    roomService = new Room_Service(); 
                    var selectedRoom = roomService.GetRoomById(Clicked_Room_ID);

                    if (selectedRoom != null)
                    {
                        comboBox1.Text = selectedRoom.Room_Type ?? "";
                        comboBox2.Items.Clear();

                        if (selectedRoom.Room_Type == "Lecturer Hall")
                        {
                            comboBox2.Items.Add("Hall A");
                            comboBox2.Items.Add("Hall B");
                        }
                        else if (selectedRoom.Room_Type == "Computer Lab")
                        {
                            comboBox2.Items.Add("Lab 1");
                            comboBox2.Items.Add("Lab 2");
                        }

                        comboBox2.Text = selectedRoom.Room_Name ?? "";
                    }
                    else
                    {
                        MessageBox.Show("Room entry not found!");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Room ID.");
                }
            }

        }
    }
}

