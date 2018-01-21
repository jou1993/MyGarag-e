using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using myGarag_e_MAINPROJECT.DbFiles;
using MySql.Data.MySqlClient;
using System.IO;
using myGarag_e_MAINPROJECT.Classes;

namespace myGarag_e_MAINPROJECT
{
    public partial class myGarage_ConsumerMain : Form
    {
        Proion selectedProion = new Proion();
        Pelatis tempPelatis = new Pelatis();
        int idKatastimatarxi;
        string id = "start";
        int StoreCode;

        public myGarage_ConsumerMain()
        {
            InitializeComponent();
            
        }
        MySqlConnection connection = new MySqlConnection("datasource=jabc.zapto.org;port=3306;Initial Catalog='adopse';username=TzRe;password=A6dT.R4e;");
        public MySqlConnection Connection { get => connection; set => connection = value; }
        
        private void myGarage_ConsumerMain_Load(object sender, EventArgs e)
        {
            FillDGV("");
        }

        private void FillDGV(string SearchTextBox)
        {
            MySqlCommand command = new MySqlCommand("SELECT onomasiaKatastimatos,kodikosKatastimatos  FROM katastima", Connection);

            MySqlDataAdapter adapter = new MySqlDataAdapter(command);

            DataTable table = new DataTable();

            adapter.Fill(table);

            dataGridView1.AllowUserToAddRows = false;

            dataGridView1.DataSource = table;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            addtoCart.Text = "Επιλογη";
            deleteFromCart.Visible = false;
            deleteFromCart.Enabled = false;
            id = "store";

        }
        private void dataGridView1_Click(object sender, EventArgs e)
        {
            if (id.Equals("product"))
            {
                 try
                 {
                      Byte[] img = (Byte[])dataGridView1.CurrentRow.Cells[2].Value;

                      MemoryStream ms = new MemoryStream(img);

                      selectedProion.Eikona = Image.FromStream(ms);

                      selectedProion.KodikosProiontos = (int)dataGridView1.CurrentRow.Cells[0].Value;
                      selectedProion.Perigrafi = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                      selectedProion.Timi = (int)dataGridView1.CurrentRow.Cells[3].Value;
                      selectedProion.Kataskeuastis = dataGridView1.CurrentRow.Cells[4].Value.ToString();
                      selectedProion.XoraKataskeuis = dataGridView1.CurrentRow.Cells[5].Value.ToString();
                 }
                 catch (NullReferenceException) { }  
            }
            else if(id.Equals("store"))
            {
                StoreCode = (int)dataGridView1.CurrentRow.Cells[1].Value;
                id = "product";
            }
           
        }

        public DataSet GetID()
        {
            MySqlCommand command = new MySqlCommand("SELECT k.kodikosKatastimatos FROM katastimatarxis k " +
                                                            "inner join katastima kat on k.kodikosKatastimatos=kat.kodikosKatastimatos" +
                                                            "inner join ApothikiKatastimatos ap on kat.kodikosKatastimatos = ap.kodikosKatastimatos"+
                                                            "inner join Proion p on ap.kodikosKatastimatosp = p.kodikosKatastimatos where p.kodikosProiontos="+ selectedProion.KodikosProiontos,Connection);

            MySqlDataAdapter adapter = new MySqlDataAdapter(command);

            DataTable table = new DataTable();

            DataSet data = new DataSet();
            adapter.Fill(data);
            return data;
        }

        private void addtoCart_Click(object sender, EventArgs e)
        {        
            if (id.Equals("cart"))
            {
                tempPelatis.addcart(selectedProion);
                cartCount.Text = String.Concat("", tempPelatis.num_product());
            }
            else
            {
                for(int i = 0; i < tempPelatis.Cart.Count; i++)
                {
                   
                    MySqlCommand command = new MySqlCommand("INSERT INTO sinalagi(kodikosPelati, kodikosKatastimatarxi, kodikosProiontos, oloklirothike) VALUES (@kodikosPelati,@kodikosKatastimatarxi,@kodikosProiontos,@oloklirothike)", Connection);
                    command.Parameters.Add("@kodikosPelati", MySqlDbType.VarChar).Value = DbMethods.user.Id;
                    command.Parameters.Add("@kodikosKatastimatarxi", MySqlDbType.VarChar).Value = GetID();
                    command.Parameters.Add("@kodikosProiontos", MySqlDbType.VarChar).Value = tempPelatis.Cart.ElementAt(i).KodikosProiontos;
                    command.Parameters.Add("@oloklirothike", MySqlDbType.VarChar).Value = 0;
    
                    ExecMyQuery(command, "Data Inserted");
                }
                dataGridView1.Rows.Clear();
                dataGridView1.Refresh();

            }
            
        }

        public void ExecMyQuery(MySqlCommand mcomd, string myMsg)
        {
            Connection.Open();
            if (mcomd.ExecuteNonQuery() == 1)
            {

                MessageBox.Show(myMsg);

            }
            else
            {

                MessageBox.Show("Query Not Executed");

            }

            Connection.Close();
        }

        private void CartMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = tempPelatis.Cart;
            id = "cart";
            addtoCart.Text = "Αγορα";
            deleteFromCart.Visible = true;
            deleteFromCart.Enabled = true;
        }
        private void StoreMenuItem_Click(object sender, EventArgs e)
        {
            MySqlCommand command = new MySqlCommand("SELECT onomasiaKatastimatos,kodikosKatastimatos  FROM katastima", Connection);

            MySqlDataAdapter adapter = new MySqlDataAdapter(command);

            DataTable table = new DataTable();

            adapter.Fill(table);

            dataGridView1.AllowUserToAddRows = false;

            dataGridView1.DataSource = table;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            addtoCart.Text = "Επιλογη";
             deleteFromCart.Visible = false;
             deleteFromCart.Enabled = false;
             id ="store";
        }

        private void deleteFromCart_Click(object sender, EventArgs e)
        {
                int row = dataGridView1.CurrentCell.RowIndex;
                dataGridView1.Rows.RemoveAt(row);
                cartCount.Text = tempPelatis.num_product().ToString();
        }
    }
}
