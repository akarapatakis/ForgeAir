namespace ForgeAir.InstallationWizard
{
    partial class DatabaseSetup
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
            button1 = new Button();
            dbPasswordBox = new TextBox();
            dbPortBox = new TextBox();
            label2 = new Label();
            label1 = new Label();
            label3 = new Label();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(258, 213);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 0;
            button1.Text = "OK";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // dbPasswordBox
            // 
            dbPasswordBox.Location = new Point(12, 119);
            dbPasswordBox.Name = "dbPasswordBox";
            dbPasswordBox.Size = new Size(263, 23);
            dbPasswordBox.TabIndex = 1;
            dbPasswordBox.UseSystemPasswordChar = true;
            // 
            // dbPortBox
            // 
            dbPortBox.Location = new Point(12, 171);
            dbPortBox.MaxLength = 8;
            dbPortBox.Name = "dbPortBox";
            dbPortBox.PlaceholderText = "3306";
            dbPortBox.Size = new Size(80, 23);
            dbPortBox.TabIndex = 2;
            dbPortBox.Text = "3306";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(87, 26);
            label2.Name = "label2";
            label2.Size = new Size(138, 25);
            label2.TabIndex = 6;
            label2.Text = "MariaDB Setup";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 101);
            label1.Name = "label1";
            label1.Size = new Size(108, 15);
            label1.TabIndex = 7;
            label1.Text = "Database Password";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 153);
            label3.Name = "label3";
            label3.Size = new Size(80, 15);
            label3.TabIndex = 8;
            label3.Text = "Database Port";
            // 
            // DatabaseSetup
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(345, 243);
            Controls.Add(label3);
            Controls.Add(label1);
            Controls.Add(label2);
            Controls.Add(dbPortBox);
            Controls.Add(dbPasswordBox);
            Controls.Add(button1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DatabaseSetup";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Setup Database";
            FormClosing += DatabaseSetup_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label label2;
        private Label label1;
        private Label label3;
        public TextBox dbPasswordBox;
        public TextBox dbPortBox;
    }
}