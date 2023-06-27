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
        string selectedName;
        private int selectedParentId;
        private int selectedIdcut;
        private string selectedNamecut;
        private string sizePath = @"Texts/Size.txt";
        private int fileSize = 5;
        int selectedIsDirectory;

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

            foreach (DataGridViewRow row in this.dataGridView1.Rows)
            {
                if (Convert.ToInt16(row.Cells[2].Value) == 0)
                {
                    row.DefaultCellStyle.BackColor = Color.Gray;
                }
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
                selectedId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);
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

                    // اضافه کردن آیدی به تاریخچه parentId
                    parentIdHistory.Add(selectedId);

                    // نمایش اسامی مرتبط با آیدی استخراج شده
                    LoadNamesWithParentId(selectedId);
                }
            }

        }
        public int GetIsDirectoryById(int id)
        {
            int isDirectory = 0;
            string query = "SELECT IsDirectory FROM Files WHERE Id = @Id;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    isDirectory = Convert.ToInt32(result);
                }
            }
            return isDirectory;
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

        private bool decrease_size(int size) //اگر مقدار خروجی تابع درست بود, یعنی عملیات انجام شد.
        {
            int availableSize = Convert.ToInt32(File.ReadAllText(sizePath));

            //بررسی وجود فضای خالی
            if (availableSize - size < 0)
            {
                MessageBox.Show("There is not enough space.");
                return false;
            }

            //کم کردن حافظه
            File.WriteAllText(sizePath, (availableSize - size).ToString());
            return true;
        }

        private void increase_size(int size)
        {
            int availableSize = Convert.ToInt32(File.ReadAllText(sizePath));

            //اضافه کردن حافظه
            File.WriteAllText(sizePath, (availableSize + size).ToString());
        }

        private void addToTable(string name, int directory, int parentId)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // اجرای کوئری برای افزودن رکورد جدید
                string query = "INSERT INTO Files (Name, IsDirectory, ParentId) VALUES (@Name, @IsDirectory, @ParentId)";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@IsDirectory", directory);
                command.Parameters.AddWithValue("@ParentId", parentId);
                command.ExecuteNonQuery();

                // بستن اتصال
                connection.Close();
            }
        }
        private void btnAddFile_Click(object sender, EventArgs e)
        {
            // گرفتن parentId از جدول
            parentId = parentIdHistory[parentIdHistory.Count - 1];
            if (parentId == 0 || parentId == 1)
            {
                MessageBox.Show("Can not add file or folder to ThisPC.");
                txtAddFile.Text = string.Empty;
            }
            if (parentId > 1)
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

                if (!decrease_size(fileSize))
                {
                    return;
                }

                // اضافه کردن رکورد جدید به جدول
                addToTable(fileName, 0, parentId);

                
                // بارگزاری اسامی با parentId فعلی برای به‌روزرسانی DataGridView
                txtAddFile.Text = string.Empty;
                LoadNamesWithParentId(parentId);
            }
        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            // گرفتن parentId از جدول
            int parentId = parentIdHistory[parentIdHistory.Count - 1];
            if (parentId == 0 || parentId == 1)
            {
                MessageBox.Show("Can not add file or folder to ThisPC.");
                txtAddFile.Text = string.Empty;
            }
            if (parentId > 1)
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

                // ایجاد اتصال به پایگاه داده SQLite
                addToTable(folderName, 1, parentId);

                // بارگزاری اسامی با parentId فعلی برای به‌روزرسانی DataGridView
                txtAddFolder.Text = string.Empty;
                LoadNamesWithParentId(parentId);
            }
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
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int lastParentId = parentIdHistory[parentIdHistory.Count - 1];
            if (string.IsNullOrEmpty(selectedName))
            {
                MessageBox.Show("Can not delete nothing!!");
                return;
            }
            if (parentId == 0)
            {
                MessageBox.Show("Can not delete ThisPC.");
                return;
            }

            if (lastParentId > 0)
            {
                if (selectedId != 0 && !string.IsNullOrEmpty(selectedName))
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
                        //گرفتن ایدی راس انتخاب شده
                        int currentNodeId = selectedId;

                        //ایجاد لیست برای پیمایش زیردرخت
                        List<Dictionary<string, object>> currentLevel = new List<Dictionary<string, object>>();
                        List<Dictionary<string, object>> nextLevel = new List<Dictionary<string, object>>();

                        // پیمایش اول سطح
                        foreach (Dictionary<string, object> child in children(currentNodeId))
                        {
                            currentLevel.Add(child);
                        }

                        if (GetIsDirectoryById(currentNodeId) == 0)
                        {
                            increase_size(fileSize);
                        }
                        DeleteRecord(currentNodeId);

                        while (currentLevel.Count > 0)
                        {
                            foreach (Dictionary<string, object> node in currentLevel)
                            {
                                currentNodeId = Convert.ToInt32(node["Id"]);
                                foreach (Dictionary<string, object> child in children(currentNodeId))
                                {
                                    nextLevel.Add(child);
                                }

                                //اضافه کردن به حافظه در صورت فایل بودن
                                if (GetIsDirectoryById(currentNodeId) == 0)
                                {
                                    increase_size(fileSize);
                                }
                                DeleteRecord(currentNodeId);
                            }

                            currentLevel.Clear();
                            foreach (Dictionary<string, object> node in nextLevel)
                            {
                                currentLevel.Add(node);
                            }
                            nextLevel.Clear();
                        }

                        // بروزرسانی جدول DataGridView
                        LoadNamesWithParentId(parentId);
                    }
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

        private void btnPaste_Click(object sender, EventArgs e)
        {
            int lastParentId = parentIdHistory[parentIdHistory.Count - 1];
            int destinationParentId = lastParentId;
            if (lastParentId == 0 || lastParentId == 1)
            {
                MessageBox.Show("Can not paste file or folder to ThisPC.");
                txtAddFile.Text = string.Empty;
            }
            if (lastParentId > 1)
            {
                if (isCutMode)
                {
                    // شرط اول: بررسی تکراری بودن اسم قبل از انجام عملیات پیست
                    bool isDuplicateName = CheckDuplicateName(selectedNamecut);

                    if (isDuplicateName)
                    {
                        MessageBox.Show("There is a duplicate name. Paste operation is not possible.");
                        return;
                    }

                    //انجام عملیات paste
                    UpdateParentId(selectedIdcut, destinationParentId);
                    isCutMode = false;
                    LoadNamesWithParentId(lastParentId);
                }
                if (isCopyMode)
                {
                    bool isDuplicateName = CheckDuplicateName(selectedNamecut);

                    if (isDuplicateName)
                    {
                        MessageBox.Show("There is a duplicate name. Paste operation is not possible.");
                        return;
                    }

                    //پیمایش اول سطح زیر درخت با ریشه ایدی past شده
                    Dictionary<int, int> equivalentPairs = new Dictionary<int, int>();

                    int currentNodeId = selectedIdcut;
                    
                    int lastrowid = addToTable_copy(selectedNamecut,selectedIsDirectory, destinationParentId);
                    equivalentPairs[currentNodeId] = lastrowid; // متناظر سازی نود قبلی با نود ایجاد شده

                    List<Dictionary<string, object>> nodelist = new List<Dictionary<string, object>>();
                    List<Dictionary<string, object>> partlist = new List<Dictionary<string, object>>();

                    //اضافه کردن ریشه درخت به لیست راس ها
                    foreach (Dictionary<string, object> child in children(currentNodeId))
                    {
                        nodelist.Add(child);
                    }

                    while (nodelist.Count > 0)
                    {
                        foreach (Dictionary<string, object> node in nodelist)
                        {
                            //دریافت ایدی راس دیده شده
                            currentNodeId = Convert.ToInt32(node["Id"]);

                            //ساخت راس جدید با والد متناظر جدید
                            lastrowid = addToTable_copy(node["Name"].ToString(), Convert.ToInt32(node["IsDirectory"]), equivalentPairs[Convert.ToInt32(node["parentId"])]);

                            //ایجاد تناظر بین راس ایجاد شده و راس قبلی
                            equivalentPairs[currentNodeId] = lastrowid;

                            foreach (Dictionary<string, object> child in children(currentNodeId))
                            {
                                partlist.Add(child);
                            }
                        }

                        nodelist.Clear();

                        foreach (Dictionary<string, object> node in partlist)
                        {
                            nodelist.Add(node);
                        }
                        partlist.Clear();
                    }
                    isCopyMode = false;
                    LoadNamesWithParentId(lastParentId);
                }
            }
        }

        private int addToTable_copy(string name, int directory, int parentId) // 0 if is file, 1 if is directory
        {
            long lastrowid;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // اجرای کوئری برای افزودن رکورد جدید
                string query = "INSERT INTO Files (Name, IsDirectory, ParentId) VALUES (@Name, @IsDirectory, @ParentId)";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@IsDirectory", directory);
                command.Parameters.AddWithValue("@ParentId", parentId);
                command.ExecuteNonQuery();

                command.CommandText = "select last_insert_rowid()";
                lastrowid = (long)command.ExecuteScalar();

                // بستن اتصال
                connection.Close();
            }
            return (int)lastrowid;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // اگر روی سلول کلیک راست شد
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    // خواندن Id سلول از DataGridView
                    selectedNamecut = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                    selectedIdcut = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);
                    int parentId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[3].Value);
                    selectedIsDirectory = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[2].Value);

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
        private bool CheckDuplicateName(string name)
        {
            foreach (DataGridViewRow row in this.dataGridView1.Rows)
            {
                if (name == row.Cells[1].Value.ToString())
                {
                    return true;
                }
            }
            return false;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
        }
        bool isCutMode = false;
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isCutMode = true;
        }

        bool isCopyMode = false;
        private void copyToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
        }
        private void txtAddFile_TextChanged(object sender, EventArgs e)
        {
        }
        private List<Dictionary<string, object>> children(int id)
        {
            LoadNamesWithParentId(id);
            List<Dictionary<string, object>> rowDataList = new List<Dictionary<string, object>>();

            for (int rowIndex = 0; rowIndex < dataGridView1.Rows.Count; rowIndex++)
            {
                Dictionary<string, object> rowData = new Dictionary<string, object>();

                DataGridViewRow row = dataGridView1.Rows[rowIndex];

                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    string columnName = column.Name;
                    object columnValue = row.Cells[columnName].Value;

                    rowData[columnName] = columnValue;
                }

                rowDataList.Add(rowData);
            }
            return rowDataList;
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isCopyMode = true;
        }

        private void btnAddPartition_Click(object sender, EventArgs e)
        {
            // گرفتن parentId از جدول
            int parentId = parentIdHistory[parentIdHistory.Count - 1];
            if (parentId == 1)
            {
                string folderName = txtAddPartition.Text.Trim();

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

                // ایجاد رکورد جدید
                addToTable(folderName, 1, 1);
                txtAddPartition.Text = string.Empty;
                LoadNamesWithParentId(parentId);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(File.ReadAllText(sizePath));
            return;
        }
    }
}