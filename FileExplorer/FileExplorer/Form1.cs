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
            dataGridView1.Columns["IsDirectory"].Visible = false; //db
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
                int availableSize = Convert.ToInt32(File.ReadAllText(sizePath));

                // بررسی وجود اسم خالی
                if (string.IsNullOrEmpty(fileName))
                {
                    MessageBox.Show("Please enter the file name.");
                    return;
                }

                //بررسی وجود فضای خالی
                if (availableSize - fileSize < 0)
                {
                    MessageBox.Show("There is not enough space.");
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



                // ایجاد اتصال به پایگاه داده SQLite
                addToTable(fileName, 0, parentId);
                //using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                //{
                //    connection.Open();

                //    // اجرای کوئری برای افزودن رکورد جدید
                //    string query = "INSERT INTO Files (Name, IsDirectory, ParentId) VALUES (@Name, 0, @ParentId)";
                //    SQLiteCommand command = new SQLiteCommand(query, connection);
                //    command.Parameters.AddWithValue("@Name", fileName);
                //    command.Parameters.AddWithValue("@ParentId", parentId);
                //    command.ExecuteNonQuery();

                //    // بستن اتصال
                //    connection.Close();
                //    txtAddFile.Text = string.Empty;
                //}
                //کم کردن حافظه
                //File.WriteAllText(sizePath, (availableSize- fileSize).ToString());

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
                //using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                //{
                //    connection.Open();

                //    // اجرای کوئری برای افزودن رکورد جدید
                //    string query = "INSERT INTO Files (Name, IsDirectory, ParentId) VALUES (@Name, 1, @ParentId)";
                //    SQLiteCommand command = new SQLiteCommand(query, connection);
                //    command.Parameters.AddWithValue("@Name", folderName);
                //    command.Parameters.AddWithValue("@ParentId", parentId);
                //    command.ExecuteNonQuery();

                //    // بستن اتصال
                //    connection.Close();
                //    txtAddFolder.Text = string.Empty;
                //}

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
            int lastParentId = parentIdHistory[parentIdHistory.Count - 1];
            if (string.IsNullOrEmpty(selectedName))
            {
                MessageBox.Show("Can not delete nothing!!");
                return;
            }
            if (parentId == 0 )
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
                        // حذف رکورد با آیدی مشخص شده
                        DeleteRecord(selectedId);

                        // بروزرسانی جدول DataGridView
                        LoadNamesWithParentId(parentId);
                    }

                    //اضافه کردن حافظه todo

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

                    //MessageBox.Show(selectedNamecut); //db
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
                //MessageBox.Show($"{isCopyMode}"); //db
                if (isCopyMode)
                {

                    //MessageBox.Show("incopy");
                    bool isDuplicateName = CheckDuplicateName(selectedNamecut);

                    if (isDuplicateName)
                    {
                        MessageBox.Show("There is a duplicate name. Paste operation is not possible.");
                        return;
                    }
                    Dictionary<int, int> equivalentPairs = new Dictionary<int, int>();

                    int currentNodeId = selectedIdcut;

                    
                    int lastrowid = addToTable_copy(selectedNamecut,selectedIsDirectory, destinationParentId);
                    equivalentPairs[currentNodeId] = lastrowid; // متناظر سازی نود قبلی با نود ایجاد شده

                    //MessageBox.Show(lastrowid.ToString()); //db
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
                            //MessageBox.Show(node["Name"].ToString());
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
                    //MessageBox.Show(selectedName); //db
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
            //int lastParentId = parentIdHistory[parentIdHistory.Count - 1];
            //LoadNamesWithParentId(lastParentId);
            /*dataGridView1.Refresh()*/
            ;
            //for(int item = 0;item < dataGridView1.Rows.Count;item++)
            //{
            //    string existingFileName = dataGridView1.Rows[item].Cells["Name"].Value.ToString();
            //    if (name == existingFileName)
            //    {

            //        return true;
            //    }
            //}
            //return false;
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
        private List<Dictionary<string, object>> children(int id) // Name, IsDirectory, parentId, Id
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



                // ایجاد اتصال به پایگاه داده SQLite
                addToTable(folderName, 1, 1);
                txtAddPartition.Text = string.Empty;
                LoadNamesWithParentId(parentId);
            }

        }


        /*
        private void copyToTable(int startingID, int copyParent)
        {

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    // کوئری کپی کردن گره‌ها
                    string copyQuery = @"
                WITH RECURSIVE rCTE AS (
                  SELECT Id,
                         Name,                                                 
                         IsDirectory,
                         parentId,
                         1 AS Level
                  FROM Files
                  WHERE Id = @StartingID
                  UNION ALL
                  SELECT D.Id,
                         D.Name,
                         D.IsDirectory,
                         D.parentId,
                         r.Level+1
                  FROM Files D
                  JOIN rCTE r ON D.parentId = r.Id
                )
                INSERT INTO Files (Name,IsDirectory,parentId)
                SELECT CASE r.Id WHEN @StartingID THEN @CopyParent ELSE k.NewID END AS parentId,
                       r.Name,
                       r.IsDirectory
                FROM rCTE r
                LEFT JOIN (
                  SELECT Id,
                         MAX(Level) AS NewID
                  FROM rCTE
                  GROUP BY Id
                ) k ON r.Id = k.Id
                WHERE r.Level > 1;";

                    using (SQLiteCommand copyCommand = new SQLiteCommand(copyQuery, connection, transaction))
                    {
                        copyCommand.Parameters.AddWithValue("@StartingID", startingID);
                        copyCommand.Parameters.AddWithValue("@CopyParent", copyParent);
                        copyCommand.ExecuteNonQuery();
                    }

                    // کوئری بروزرسانی parentId ها
                    string updateQuery = @"
                UPDATE Files
                SET parentId = CASE WHEN parentId = @StartingID THEN @CopyParent ELSE parentId END
                WHERE Id <> @StartingID;";

                    using (SQLiteCommand updateCommand = new SQLiteCommand(updateQuery, connection, transaction))
                    {
                        updateCommand.Parameters.AddWithValue("@StartingID", startingID);
                        updateCommand.Parameters.AddWithValue("@CopyParent", copyParent);
                        updateCommand.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }

        }
        */
    }
}