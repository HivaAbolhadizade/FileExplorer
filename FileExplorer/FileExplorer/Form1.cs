using System.Data.SQLite;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace FileExplorer
{
    public partial class mainForm : Form
    {
        private string connectionString = "Data Source=fileExplorer.db;Version=3;";

        private void LoadNamesWithParentId(int parentId)
        {
            // ایجاد اتصال به پایگاه داده SQLite
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // اجرای کوئری برای بازیابی اسامی با parentId مشخص شده
                string query = "SELECT * FROM Files WHERE parentId = @ParentId";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@ParentId", parentId);

                // اجرای کوئری و بازیابی نتایج
                SQLiteDataReader reader = command.ExecuteReader();

                // ایجاد یک DataTable برای ذخیره اسامی
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);

                // نمایش اسامی در DataGridView
                dataGridView1.DataSource = dataTable;

                // بستن اتصال و رفع منابع
                reader.Close();
                connection.Close();
            }
        }
        public mainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
            // بارگزاری اسامی با parentId خالی در ابتدا
            LoadNamesWithParentId(0);
            dataGridView1.Columns["Id"].Visible = false;
            dataGridView1.Columns["IsDirectory"].Visible = false;
            dataGridView1.Columns["parentId"].Visible = false;

        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // استخراج آیدی از شناسه سطر کلیک شده
                int selectedId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);

                MessageBox.Show("The row index = " + selectedId.ToString());
                // نمایش اسامی مرتبط با آیدی استخراج شده
                //LoadNamesWithParentId(selectedId);
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // استخراج آیدی از شناسه سطر کلیک شده
                int selectedId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);

                //MessageBox.Show("The row index = " + selectedId.ToString());
                // نمایش اسامی مرتبط با آیدی استخراج شده
                LoadNamesWithParentId(selectedId);
            }

        }
    }
}