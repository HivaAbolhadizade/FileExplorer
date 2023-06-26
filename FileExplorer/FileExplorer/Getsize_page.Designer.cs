namespace FileExplorer
{
    partial class Getsize_page
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.size_textbox = new System.Windows.Forms.TextBox();
            this.btn_save = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // size_textbox
            // 
            this.size_textbox.Location = new System.Drawing.Point(90, 47);
            this.size_textbox.Name = "size_textbox";
            this.size_textbox.Size = new System.Drawing.Size(205, 27);
            this.size_textbox.TabIndex = 0;
            this.size_textbox.TextChanged += new System.EventHandler(this.size_textbox_TextChanged);
            // 
            // btn_save
            // 
            this.btn_save.Location = new System.Drawing.Point(310, 47);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(56, 27);
            this.btn_save.TabIndex = 1;
            this.btn_save.Text = "Save";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // Getsize_page
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(487, 124);
            this.Controls.Add(this.btn_save);
            this.Controls.Add(this.size_textbox);
            this.Name = "Getsize_page";
            this.Text = "Getsize_page";
            this.Load += new System.EventHandler(this.Getsize_page_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox size_textbox;
        private Button btn_save;
    }
}