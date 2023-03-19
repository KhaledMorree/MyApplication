using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pyment_System
{
    public partial class HomePage : Form
    {
     private   double cost;
        public HomePage()
        {
            InitializeComponent();
            double cost;
            dataGridView1.ColumnCount = 2;
            dataGridView1.Columns[0].Name = "Item Name";
            dataGridView1.Columns[1].Name = "Item Price";

            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
          
        }
        
        public double Cost_of_Item()
        {
            double sum = 0;
            
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                sum +=Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value);
            }
            return sum;
        }
        private void AddCost()
        {
            double tax, q;
            tax = 25;
            if (dataGridView1.Rows.Count >0)
            {
                txtTax.Text = string.Format(new CultureInfo("nb-No"),"{0:c2}", (((Cost_of_Item() * tax) / 100)));
                txtSubTotal.Text = string.Format(new CultureInfo("nb-No"), "{0:c2}", (Cost_of_Item()));
                q = ((Cost_of_Item() * tax) / 100);
                txtTotal.Text = string.Format(new CultureInfo("nb-No"), "{0:c2}", ((Cost_of_Item() + q)));

            }
        }
        private void AddCostFromBarcode()
        {
            double tax, q;
            tax = 15;
            if (dataGridView1.Rows.Count > 0)
            {
                txtTax.Text = string.Format(new CultureInfo("nb-No"), "{0:c2}", (((Cost_of_Item() * tax) / 100)));
                txtSubTotal.Text = string.Format(new CultureInfo("nb-No"), "{0:c2}", (Cost_of_Item()));
                q = ((Cost_of_Item() * tax) / 100);
                txtTotal.Text = string.Format(new CultureInfo("nb-No"), "{0:c2}", ((Cost_of_Item() + q)));
                

            }
        }
        private void Change()
        {
            double tax, q, c;
            tax = 15;
            if (dataGridView1.Rows.Count >0)
            {
                q = ((Cost_of_Item() * tax) / 100) + Cost_of_Item();
                c = Convert.ToDouble(txtCost.Text);
                txtChange.Text = string.Format(new CultureInfo("nb-No"), "{0:c2}", c - q );
                
            }
        }


        Bitmap bitmap;
        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDocument2;
            printDialog.UseEXDialog = true;
            DialogResult printResult = printDialog.ShowDialog();

            if (printResult==DialogResult.OK)
            {
                printDocument2.DocumentName = "Print";
                printDocument2.Print();

            }
            //try
            //{
            //    int height = dataGridView1.Height;
            //    dataGridView1.Height = dataGridView1.RowCount * dataGridView1.RowTemplate.Height * 2;
            //    bitmap = new Bitmap(dataGridView1.Width, dataGridView1.Height);
            //    dataGridView1.DrawToBitmap(bitmap, new Rectangle(0, 0, dataGridView1.Width, dataGridView1.Height));
            //    printPreviewDialog1.PrintPreviewControl.Zoom = 1;
            //    printPreviewDialog1.ShowDialog();
            //    dataGridView1.Height = height;
            //}
            //catch (Exception error)
            //{

            //    MessageBox.Show(error.Message);
            //}
            
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            try
            {
                e.Graphics.DrawImage(bitmap, 0, 0);
            }
            catch (Exception error)
            {

                MessageBox.Show(error.Message);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            

            try
            {
                dataGridView1.Rows.Clear();
                dataGridView1.Refresh();

                txtName.Text = "";
                txtBarcode.Text = "";
                txtCost.Text = "0";
                
                txtSubTotal.Text = "";
                txtTax.Text = "";
                txtTotal.Text = "";
                cboPayment.Text = "";
                txtChange.Text = "";
            }
            catch (Exception error)
            {

                MessageBox.Show(error.Message);
            }
        }

        private void HomePage_Load(object sender, EventArgs e)
        {
            cboPayment.Items.Add("Cash");
            cboPayment.Items.Add("Visa Card");
            cboPayment.Items.Add("Master Card");
           
        }

        private void txtprice_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtBarcode_TextChanged(object sender, EventArgs e)
        {
            SqlConnection conn= new SqlConnection("Data Source=localhost\\SQLEXPRESS;Initial Catalog=PaymentSystem;Integrated Security=True");
            conn.Open();
            if (txtBarcode.Text != "")
            {
                SqlCommand cmd = new SqlCommand("Select VarePris, VareName from Varer where Barcode=@barcode", conn);
                cmd.Parameters.AddWithValue("@barcode", (txtBarcode.Text));
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    txtShowPrice.Text = dr.GetValue(0).ToString();
                    txtName.Text = dr.GetValue(1).ToString();
                    cost +=Convert.ToDouble(txtShowPrice.Text);
                    //txtCost.Text = cost.ToString();
                    txtBarcode.Text = "";
                    dataGridView1.Rows.Add(txtName.Text, txtShowPrice.Text);
                    
                    AddCostFromBarcode();

                }
                conn.Close();
                
            }



        }


        SqlConnection conn = new SqlConnection("Data Source=localhost\\SQLEXPRESS;Initial Catalog=PaymentSystem;Integrated Security=True");
        private void btnPay_Click(object sender, EventArgs e)
        {
            if (cboPayment.Text=="Cash")
            {
                Change();
                if (Convert.ToDouble(txtCost.Text) < 0)
                {
                    MessageBox.Show("");
                }

            }
            else
            {
                double salgSum;
                string date;
                date = DateTime.Now.ToString("yyyy-MM-dd");
                salgSum =Convert.ToDouble(txtCost.Text);
                conn.Open();
                SqlCommand cmd = new SqlCommand("exec insertIntoSalgTabel '" + salgSum + "','" + date + "'", conn);
                cmd.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Payment completed successfully");
                txtChange.Text = "";
                txtCost.Text = "0";
            }
            

        }

        private void btnRemoveItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in this.dataGridView1.SelectedRows)
            {
                dataGridView1.Rows.Remove(row);
            }
            AddCost();
            if (cboPayment.Text == "Cash")
            {
                Change();
            }
            else
            {
                txtChange.Text = "";
                txtCost.Text = "0";
            }
        }

        private void NumbersOnly(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            if (txtCost.Text=="0")
            {
                txtCost.Text = "";
                txtCost.Text = b.Text;
            }
            else if (b.Text==".")
            {
                if(! txtCost.Text.Contains("."))
                {
                    txtCost.Text = txtCost.Text + b.Text;
                }
            }
            else
            {
                txtCost.Text = txtCost.Text + b.Text;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtCost.Text = "0";
        }

        private void btnCigarette_Click(object sender, EventArgs e)
        {
            double cost = 90;
            foreach (DataGridViewRow row in this.dataGridView1.SelectedRows)
            {
                if ((bool)(row.Cells[0].Value="Cigarette"))
                {
                    row.Cells[0].Value = double.Parse((string)row.Cells[1].Value + 1);
                    row.Cells[1].Value = double.Parse((string)row.Cells[1].Value) *cost;
                    
                }
                
            }
            dataGridView1.Rows.Add("Cigarette",cost);
            txtShowPrice.Text = cost.ToString();
            txtName.Text = "Cigarette";
            AddCostFromBarcode();

        }

        private void btnAlkohol_Click(object sender, EventArgs e)
        {
            double cost = 110;
            foreach (DataGridViewRow row in this.dataGridView1.SelectedRows)
            {
                if ((bool)(row.Cells[0].Value = ""))
                {
                    row.Cells[0].Value = double.Parse((string)row.Cells[1].Value + 1);
                    row.Cells[1].Value = double.Parse((string)row.Cells[1].Value) * cost;

                }

            }
            dataGridView1.Rows.Add("Alkohol", cost);
            txtShowPrice.Text = cost.ToString();
            txtName.Text = "Alkohol";
            AddCostFromBarcode();
        }

        private void btnSnus_Click(object sender, EventArgs e)
        {
            double cost = 127;
            foreach (DataGridViewRow row in this.dataGridView1.SelectedRows)
            {
                if ((bool)(row.Cells[0].Value = "Snus"))
                {
                    row.Cells[0].Value = double.Parse((string)row.Cells[1].Value + 1);
                    row.Cells[1].Value = double.Parse((string)row.Cells[1].Value) * cost;

                }

            }
            dataGridView1.Rows.Add("Snus", cost);
            txtShowPrice.Text = cost.ToString();
            txtName.Text = "Snus";
            AddCostFromBarcode();

        }

        private void btnCandy_Click(object sender, EventArgs e)
        {
            double cost = 179;
            foreach (DataGridViewRow row in this.dataGridView1.SelectedRows)
            {
                if ((bool)(row.Cells[0].Value = "Candy"))
                {
                    row.Cells[0].Value = double.Parse((string)row.Cells[1].Value + 1);
                    row.Cells[1].Value = double.Parse((string)row.Cells[1].Value) * cost;

                }

            }
            dataGridView1.Rows.Add("Candy", cost);
            txtShowPrice.Text = cost.ToString();
            txtName.Text = "Candy";
            AddCostFromBarcode();
        }

        private void btnAvis_Click(object sender, EventArgs e)
        {
            double cost = 35;
            foreach (DataGridViewRow row in this.dataGridView1.SelectedRows)
            {
                if ((bool)(row.Cells[0].Value = "Avis"))
                {
                    row.Cells[0].Value = double.Parse((string)row.Cells[1].Value + 1);
                    row.Cells[1].Value = double.Parse((string)row.Cells[1].Value) * cost;

                }

            }
            dataGridView1.Rows.Add("Avis", cost);
            txtShowPrice.Text = cost.ToString();
            txtName.Text = "Avis";
            AddCostFromBarcode();
        }

        private void btnBatteri_Click(object sender, EventArgs e)
        {
            double cost = 79;
            foreach (DataGridViewRow row in this.dataGridView1.SelectedRows)
            {
                if ((bool)(row.Cells[0].Value = "Batteri"))
                {
                    row.Cells[0].Value = double.Parse((string)row.Cells[1].Value + 1);
                    row.Cells[1].Value = double.Parse((string)row.Cells[1].Value) * cost;

                }

            }
            dataGridView1.Rows.Add("Batteri", cost);
            txtShowPrice.Text = cost.ToString();
            txtName.Text = "Batteri";
            AddCostFromBarcode();
        }

        private void cboPayment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((cboPayment.Text=="Visa Card")||(cboPayment.Text=="Master Card"))
            {
                txtCost.Text = txtTotal.Text;
                
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Hide();
            SignIn home = new SignIn();
            home.Show();
        }

        private void printDocument2_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Bitmap printDataBitmap = new Bitmap(this.dataGridView1.Width, this.dataGridView1.Height);
            dataGridView1.DrawToBitmap(printDataBitmap, new Rectangle(0,0,this.dataGridView1.Width, this.dataGridView1.Height));
            e.Graphics.DrawImage(printDataBitmap, 0, 0);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
