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
using lab1;


namespace lab1
{
    public partial class Form1 : Form
    {
        private static SqlReader sqlReader;
        private string whomst = "StudiosAnime";
        private SqlConnection connection;

        public Form1()
        {
            this.connection = new SqlConnection();
            this.connection.ConnectionString = @"Data Source = USER\SQLEXPRESS01; Initial Catalog = Anime; Integrated Security = true";
            InitializeComponent();
        }
        private void LoadQueries()
        {
            Dictionary<string, string> sqls = new Dictionary<string, string>();

        }
        private void PopulateParentGrid()
        {
            sqlReader = SqlReader.Create(whomst);
            try
            {
                DataSet dataSet = new DataSet();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlReader.GetQuery("getParent"), connection);
                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
                dataAdapter.Fill(dataSet);
                dataGridView1.ReadOnly = true;
                dataGridView1.DataSource = dataSet.Tables[0];
                dataGridView2.DataSource = null;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            object[] range = new object[6] { 1, 2, 3, 4, 5, 6 };
            comboBox1.Items.AddRange(range);
            PopulateParentGrid();
        }

        private void populateChildGrid()
        {
            DataSet dataSet = new DataSet();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlReader.GetQuery("getChild") + dataGridView1.SelectedCells[0].Value.ToString(), connection);
            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
            dataAdapter.Fill(dataSet);
            dataGridView2.ReadOnly = true;
            dataGridView2.DataSource = dataSet.Tables[0];
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(dataGridView1.CurrentCell.ColumnIndex != 0)
            {
                MessageBox.Show("Please select the ID field", "Error");
                return;
            }
            try
            {
                populateChildGrid();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            
        }

        private void tittleBox_Click(object sender, EventArgs e)
        {
            tittleBox.Text = "";
        }

        private void countBox_Click(object sender, EventArgs e)
        {
            countBox.Text = "";
        }

        private string newAnime()
        {
            
            string data = "";
                //dataSet.Tables["Anime"].Rows.Add(new Object[] {,, checkBox1.Checked, count, Convert.ToInt32(comboBox1.SelectedValue), rating, Convert.ToInt32(dataGridView1.SelectedCells[0].Value) });
                data += ",";
                data += tittleBox.Text;
                data += ",";
                data += checkBox1.Checked;
                data += ",";
                data += countBox.Text;
                data += ",";
                data += comboBox1.SelectedValue;
                data += ",";
                data += ratingBox.Text;
                data += ",";
                data += dataGridView1.SelectedCells[0].Value;
                data += ")";

  
            return data;
        }

        private string newOST()
        {
            string data = "";
            data += ",";
            data += tittleBox.Text;
            data += ",";
            data += countBox.Text;
            data += ",";
            data += ratingBox.Text;
            data += ",";
            data += dataGridView1.SelectedCells[0].Value;
            data += ")";
            return data;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            string newElement = "";
            if (whomst == "StudiosAnime")
                newElement = newAnime();
            else
                newElement = newOST();
            try
            {
                connection.Open();
                SqlCommand command1 = new SqlCommand(sqlReader.GetQuery("countChild"), connection);
                Random random = new Random();
                int result = (int)command1.ExecuteScalar();
                result += random.Next(result, result * 7);
                connection.Close();

                connection.Open();
                string query = sqlReader.GetQuery("addChild") + result.ToString() + newElement;
                SqlCommand command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();
                connection.Close();
                populateChildGrid();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentCell.ColumnIndex != 0)
            {
                MessageBox.Show("Please select the ID field of the object that you want to delete", "Error");
                return;
            }
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlReader.GetQuery("deleteChild") + dataGridView1.SelectedCells[0].Value.ToString(), connection);
                command.ExecuteNonQuery();
                connection.Close();
                populateChildGrid();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentCell.ColumnIndex != 0)
            {
                MessageBox.Show("Please select the ID field of the object that you want to update", "Error");
                return;
            }
            try
            {
                string newElement = "";
                if (whomst == "StudiosAnime")
                    newElement = newAnime();
                else
                    newElement = newOST();
                int id = Convert.ToInt32(dataGridView1.SelectedCells[0].Value);
                connection.Open();
                SqlCommand command = new SqlCommand(sqlReader.GetQuery("deleteChild") + id.ToString(), connection);
                command.ExecuteNonQuery();
                string query = sqlReader.GetQuery("addChild") + id.ToString() + newElement;
                SqlCommand command2 = new SqlCommand(query, connection);
                command2.ExecuteNonQuery();
                connection.Close();
                populateChildGrid();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                whomst = "StudiosAnime";
                PopulateParentGrid();
                checkBox1.Visible = true;
                comboBox1.Text = "Manga ID";
                tittleBox.Text = "Title";
                countBox.Text = "Episode Count";
                ratingBox.Text = "Rating";
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                whomst = "ComposersOST";
                PopulateParentGrid();
                checkBox1.Visible = false;
                comboBox1.Text = "Character ID";
                tittleBox.Text = "Length";
                countBox.Text = "Genre";
                ratingBox.Text = "Album";
            }
        }

    }
}
