using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pyment_System
{
    public partial class SignIn : Form
    {
        string rfidTag;
        SerialPort port = new System.IO.Ports.SerialPort("COM3", 9600, System.IO.Ports.Parity.None, 8
         , System.IO.Ports.StopBits.One);
        //string config = ConfigurationManager.ConnectionStrings["PaymentSystem"].ConnectionString;
        DatabaseAccess DbAccess = new DatabaseAccess();
        DataTable dtUser= new DataTable();
        public SignIn()
        {
            InitializeComponent();
        }
        private void btnScann_Click(object sender, EventArgs e)
        {
            try
            {
                port.Open();
                port.DtrEnable = true;
                txtPassword.Text = "";
            }
            catch (Exception error)
            {

                MessageBox.Show(error.Message);
            }
            try
            {
                int numberBytesToRead = 4;
                byte[] data = new byte[numberBytesToRead];
                port.ReadTimeout = 1000;
                port.Read(data, 0, numberBytesToRead);

                rfidTag = "";
                for (int i = 0; i < numberBytesToRead; i++)
                {
                    rfidTag = rfidTag + data[i].ToString("X");
                };


                txtPassword.Text = rfidTag;
                port.Close();
            }
            catch (Exception error)
            {

                MessageBox.Show(error.Message);
            }
            if (txtPassword.Text== "9B132447")
            {
                txtUsername.Text = "Manis";
            }

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username, password;
            username = txtUsername.Text;
            password = txtPassword.Text;
            if (username.Equals(""))
            {
                MessageBox.Show("Please Enter User Name");

            }
            else if (password.Equals(""))
            {
                MessageBox.Show("Please Enter Password");
            }
            else
            {
                string query = "Select * from Loging where BrukerNavn = '" + username + "'AND Passord ='" + password + "'";
                DbAccess.readDatathroughAdapter(query, dtUser);
                if (dtUser.Rows.Count==1)
                {
                    MessageBox.Show("you are logged in");
                    DbAccess.closeConn();
                    this.Hide();
                    HomePage home = new HomePage();
                    home.Show();
                    

                }
                else
                {
                    MessageBox.Show("The User Name or Password are not correct");
                }
            }
        }

       
    }
}
