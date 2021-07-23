using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Windows.Forms;

namespace XBCAD_Stock_Taking_application
{
    public partial class Add_Account : Form
    {
        public Add_Account()
        {
            InitializeComponent();
        }
      

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            string Password = txtPassword.Text;
            string hashedPassword = Crypto.HashPassword(Password);

            string username = string.Empty;

            string checkQuery = "Select * from [Reg_Users] where Username = @Username";

            SqlConnection con = new SqlConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["Connect"].ToString();
            SqlCommand cmd = new SqlCommand(checkQuery, con);
            con.Open();
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@Username", txtUsername.Text);

            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                username = rd["Username"].ToString();
                break;
            }
            if (username == txtUsername.Text)
            {
                MessageBox.Show("This Username has already been used. \n Please pick another.");
            }
            else
            {
                string insertQuery = "Insert into [Reg_Users](Username, Password, Role)" +
                                      "Values(@valUsername, @valPassword, @valRole)";

                con.Close();
                con.Open();
                SqlCommand cmd2 = new SqlCommand(insertQuery, con);

                //Using parameters help defend against SQLinjection attacks.
                cmd2.Parameters.AddWithValue("valUsername", txtUsername.Text);
                cmd2.Parameters.AddWithValue("valPassword", hashedPassword);
                cmd2.Parameters.AddWithValue("valRole", cbRole.SelectedItem.ToString());

                int result = cmd2.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("Your Account has been created successfully");
                }
            }
            
        }
    }
}
