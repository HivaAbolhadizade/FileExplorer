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
        private List<int> parentIdHistory = new List<int>();
        private int selectedId; // متغیری برای ذخیره آیدی انتخاب شده
        private int parentId;

        private void LoadNamesWithParentId(int parentId)
        {
            // بررسی تاریخچه parentId و در صورت خالی بودن، parentId صفر را اضافه کنید
            if (parentIdHistory.Count == 0)
            {
                parentIdHistory.Add(0);
            }
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
            //dataGridView1.Columns["IsDirectory"].Visible = false; //db
            dataGridView1.Columns["parentId"].Visible = false;

        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // استخراج آیدی از شناسه سطر کلیک شده
                selectedId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);

                //MessageBox.Show("The row index = " + selectedId.ToString());
                // نمایش اسامی مرتبط با آیدی استخراج شده
                //LoadNamesWithParentId(selectedId);
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int isDirectory = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[2].Value);
                if (isDirectory == 1)
                {
                    // استخراج آیدی از شناسه سطر کلیک شده
                    selectedId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);

                    //MessageBox.Show("The row index = " + selectedId.ToString());
                    // اضافه کردن آیدی به تاریخچه parentId
                    parentIdHistory.Add(selectedId);
                    // نمایش اسامی مرتبط با آیدی استخراج شده
                    LoadNamesWithParentId(selectedId);
                }
            }

        }


        private void btnBack_Click(object sender, EventArgs e)
        {
            // بررسی وجود مقادیر در تاریخچه parentId
            if (parentIdHistory.Count > 1)
            {
                // حذف آخرین parentId از تاریخچه
                parentIdHistory.RemoveAt(parentIdHistory.Count - 1);

                // بازیابی آخرین parentId
                int lastParentId = parentIdHistory[parentIdHistory.Count - 1];

                // نمایش اسامی مرتبط با آخرین parentId
                LoadNamesWithParentId(lastParentId);
            }
        }
        private void btnAddFile_Click(object sender, EventArgs e)
        {
            string fileName = txtAddFile.Text.Trim();

            // بررسی وجود اسم خالی
            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Please enter the file name.");
                return;
            }

            // بررسی تکراری بودن اسم فایل
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string existingFileName = row.Cells["Name"].Value.ToString();
                if (fileName == existingFileName && row.Cells["IsDirectory"].Value.ToString() == "0")
                {
                    MessageBox.Show("The file name is duplicate.");
                    return;
                }
            }

            // گرفتن parentId از جدول
            parentId = parentIdHistory[parentIdHistory.Count - 1];

            // ایجاد اتصال به پایگاه داده SQLite
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // اجرای کوئری برای افزودن رکورد جدید
                string query = "INSERT INTO Files (Name, IsDirectory, ParentId) VALUES (@Name, 0, @ParentId)";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@Name", fileName);
                command.Parameters.AddWithValue("@ParentId", parentId);
                command.ExecuteNonQuery();

                // بستن اتصال
                connection.Close();
                txtAddFile.Text = string.Empty;
            }

            // بارگزاری اسامی با parentId فعلی برای به‌روزرسانی DataGridView
            LoadNamesWithParentId(parentId);
        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            string folderName = txtAddFolder.Text.Trim();

            // بررسی وجود اسم خالی
            if (string.IsNullOrEmpty(folderName))
            {
                MessageBox.Show("Please enter the folder name.");
                return;
            }

            // بررسی تکراری بودن اسم فایل
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string existingFileName = row.Cells["Name"].Value.ToString();
                if (folderName == existingFileName && row.Cells["IsDirectory"].Value.ToString() == "1")
                {
                    MessageBox.Show("The folder name is duplicate.");
                    return;
                }
            }

            // گرفتن parentId از جدول
            int parentId = parentIdHistory[parentIdHistory.Count - 1];

            // ایجاد اتصال به پایگاه داده SQLite
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // اجرای کوئری برای افزودن رکورد جدید
                string query = "INSERT INTO Files (Name, IsDirectory, ParentId) VALUES (@Name, 1, @ParentId)";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@Name", folderName);
                command.Parameters.AddWithValue("@ParentId", parentId);
                command.ExecuteNonQuery();

                // بستن اتصال
                connection.Close();
                txtAddFolder.Text = string.Empty;
            }

            // بارگزاری اسامی با parentId فعلی برای به‌روزرسانی DataGridView
            LoadNamesWithParentId(parentId);
        }
        
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                selectedId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);
                selectedName = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                parentId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[3].Value);
            }
        }
        private void DeleteRecord(int id)
        {
            // حذف رکورد با آیدی مشخص شده
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string deleteQuery = "DELETE FROM Files WHERE Id = @Id";
                SQLiteCommand command = new SQLiteCommand(deleteQuery, connection);
                command.Parameters.AddWithValue("@Id", id);

                command.ExecuteNonQuery();

                connection.Close();
            }
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string deleteQuery = "DELETE FROM Files WHERE parentId = @Id";
                SQLiteCommand command = new SQLiteCommand(deleteQuery, connection);
                command.Parameters.AddWithValue("@Id", id);

                command.ExecuteNonQuery();

                connection.Close();
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedId != 0)
            {
                string selectedName = dataGridView1.Rows.Cast<DataGridViewRow>()
                    .Where(row => Convert.ToInt32(row.Cells["Id"].Value) == selectedId)
                    .Select(row => row.Cells["Name"].Value.ToString())
                    .FirstOrDefault();

                // نمایش پیغام هشدار و دریافت پاسخ کاربر
                DialogResult result = MessageBox.Show($"Are you sure you want to delete the '{selectedName}'?",
                    "Warning: Deletion Alert.", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // حذف رکورد با آیدی مشخص شده
                    DeleteRecord(selectedId);

                    // بروزرسانی جدول DataGridView
                    LoadNamesWithParentId(parentId);
                }
            }


        }
        
        private void UpdateParentId(int id, int newParentId)
        {
            // ایجاد اتصال به پایگاه داده SQLite و بروزرسانی parentId رکورد
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE Files SET parentId = @NewParentId WHERE Id = @Id";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@NewParentId", newParentId);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();

                connection.Close();
            }
        }
        private bool CheckDuplicateName(string name)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[1].Value != null && row.Cells[1].Value.ToString() == name)
                {
                    return true;
                }
            }
            return false;
        }
        private bool CheckDuplicateInTable(string name, int parentId)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[1].Value != null && row.Cells[1].Value.ToString() == name)
                {
                    MessageBox.Show("Im here"); //db
                    int rowParentId = Convert.ToInt32(row.Cells[3].Value);
                    if (rowParentId == parentId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void btnPaste_Click(object sender, EventArgs e)
        {
            if (isCutMode)
            {
                int lastParentId = parentIdHistory[parentIdHistory.Count - 1];
                int destinationParentId = lastParentId;
                MessageBox.Show(selectedName); //db
                // شرط اول: بررسی تکراری بودن اسم قبل از انجام عملیات پیست
                bool isDuplicateName = CheckDuplicateName(selectedName);    
                if (isDuplicateName)
                {
                    MessageBox.Show("نام تکراری وجود دارد. پیست امکان‌پذیر نیست.");
                    return;
                }

                // شرط دوم: بررسی تکراری بودن اسم در جدول
                bool isDuplicateInTable = CheckDuplicateInTable(selectedName, destinationParentId);
                if (isDuplicateInTable)
                {
                    MessageBox.Show("نام تکراری وجود دارد. پیست امکان‌پذیر نیست.");
                    return;
                }

                // انجام عملیات paste
                UpdateParentId(selectedIdcut, destinationParentId);
                isCutMode = false;
                LoadNamesWithParentId(lastParentId);
            }
        }

        private int selectedParentId;
        private int selectedIdcut;
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
        string selectedName;
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // اگر روی سلول کلیک راست شد
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    // خواندن Id سلول از DataGridView
                    selectedName = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                    MessageBox.Show(selectedName); //db
                    selectedIdcut = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);
                    int parentId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[3].Value);

                    // شرط اول: اگر ParentId برابر با 0 یا 1 باشد، کاربر نمی‌تواند روی سلول کلیک کند
                    if (parentId == 0 || parentId == 1)
                    {
                        return;
                    }

                    // ذخیره کردن Id در خصوصیت Tag سلول
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag = selectedIdcut;

                    // نمایش منوی راست کلیک
                    contextMenuStrip1.Show(dataGridView1, e.Location);
                }
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
        }
        bool isCutMode;
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isCutMode=true;
        }
    }
}