using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTIC_Management_System.Data.DB_Connection;
using UnicomTIC_Management_System.Data.log_session;
using UnicomTIC_Management_System.Model;


namespace UnicomTIC_Management_System.Service
{
    internal class Login_Service
    {
        public Login_Service()
        {
            set_Login_info_role();
        }
        public void set_Login_info_role()
        {
            Login login = new Login();
            Login_info.Login_info_user = login.Login_role;
            Login_info.Login_info_role = login.Login_role;

        }


        public void EnsureDefaultAdmin()
        {
            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    string checkQuery = "SELECT COUNT(*) FROM User WHERE User_Name = 'admin'";
                    using (var checkCmd = new SQLiteCommand(checkQuery, conn))
                    {
                        int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (exists == 0)
                        {
                            string insertQuery = @"
                            INSERT INTO User (User_Name, Password, Role)
                            VALUES ('admin', 'admin123@', 'Admin')";
                            using (var insertCmd = new SQLiteCommand(insertQuery, conn))
                            {
                                insertCmd.ExecuteNonQuery();
                            }

                            MessageBox.Show("Default admin created: admin / admin123@");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating default admin:\n" + ex.Message);
            }
        }

        public Login GetLoginByUsername(string username)
        {
            try
            {
                using (var conn = DB_Config.getConnection())
                {

                    string query = "SELECT Login_user, Login_password, login_role FROM Login WHERE Login_user = @User";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@User", username);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Login
                                {
                                    Login_user = reader["Login_user"].ToString(),
                                    Login_password = reader["Login_password"].ToString(),
                                    Login_role = reader["login_role"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching login details:\n" + ex.Message);
            }

            return null; // Not found or failed
        }

    }
}
