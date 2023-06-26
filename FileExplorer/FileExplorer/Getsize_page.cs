using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileExplorer
{
    public partial class Getsize_page : Form
    {
        public Getsize_page()
        {
            InitializeComponent();
        }

        private void Getsize_page_Load(object sender, EventArgs e)
        {

        }

        private void size_textbox_TextChanged(object sender, EventArgs e)
        {
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            string size = size_textbox.Text;
            File.WriteAllText(@"Texts/Size.txt", size);
            this.Close();
        }
    }
}
