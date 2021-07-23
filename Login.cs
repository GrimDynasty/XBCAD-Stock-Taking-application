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
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            this.Hide();
            Add_Account AAC = new Add_Account();
            AAC.ShowDialog();
            this.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {

            string password = txtPassword.Text;
            string departmentCode = txtDepartmentCode.Text;

            string hashedPassword = string.Empty;
            string role = string.Empty;

            string query = "Select * from [Reg_Users] where Username = @username";

            SqlConnection con = new SqlConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["Connect"].ToString();           
            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            cmd.Connection = con;
            //Using parameters help defend against SQLinjection attacks.
            cmd.Parameters.AddWithValue("@username", txtUsername.Text);            

            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                role = rd["Role"].ToString();
                hashedPassword = rd["Password"].ToString();
            }
            bool passValid = Crypto.VerifyHashedPassword(hashedPassword, password);

            if (role == "Finance" && passValid == true && departmentCode == "F1289NC") 
            {
                this.Hide();
                View_Stock_Finance VSF = new View_Stock_Finance();
                VSF.ShowDialog();
                this.Close();
            }
            else if (role == "Genral" && passValid == true && departmentCode == "G4578RL")
            {
                this.Hide();
                View_Stock_General VSG = new View_Stock_General();
                VSG.ShowDialog();
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            View_Stock_Finance VSF = new View_Stock_Finance();
            VSF.ShowDialog();
            this.Close();
        }
    }
}
