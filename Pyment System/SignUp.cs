using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;


namespace Pyment_System
{
    public partial class SignUp : Form
    {
        DatabaseAccess dBaccess = new DatabaseAccess();
        public SignUp()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string userName, passord;
            userName = txtBrukeNavn.Text;
            passord = txtPassord.Text;
            if (userName.Equals(""))
            {
                MessageBox.Show("Please Enter Your user name");
            }
            else if (passord.Equals(""))
            {
                MessageBox.Show("Please Enter Your passord");
            }
            else
            {
                SqlCommand insertCommand = new SqlCommand("insert into Login(BrukerNavn, Passord)values(@userName,@passord)");
                insertCommand.Parameters.AddWithValue("@userName", userName);
                insertCommand.Parameters.AddWithValue("@passord", passord);
                int row=dBaccess.executeQuery(insertCommand);
                if (row==1)
                {
                    MessageBox.Show("Account Created Successfully");
                    this.Hide();
                    HomePage home = new HomePage();
                    home.Show();

                }
                else
                {
                    MessageBox.Show("Error Try again");
                }


            }
        }
    }
}
