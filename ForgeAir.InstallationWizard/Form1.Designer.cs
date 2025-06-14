namespace ForgeAir.InstallationWizard
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            progressBar1 = new ProgressBar();
            label1 = new Label();
            button1 = new Button();
            button2 = new Button();
            pictureBox1 = new PictureBox();
            label2 = new Label();
            label3 = new Label();
            textBox1 = new TextBox();
            installerWorker = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(7, 421);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(776, 23);
            progressBar1.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.ForeColor = SystemColors.Info;
            label1.Location = new Point(215, 486);
            label1.Name = "label1";
            label1.Size = new Size(350, 15);
            label1.TabIndex = 1;
            label1.Text = "Copyright 2025 - Karapatakis Aggelos aka ChocolateAdventurouz";
            // 
            // button1
            // 
            button1.Location = new Point(629, 482);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 2;
            button1.Text = "Install";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(708, 482);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 3;
            button2.Text = "Cancel";
            button2.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.BackgroundImageLayout = ImageLayout.Center;
            pictureBox1.Image = Properties.Resources.app_logo;
            pictureBox1.Location = new Point(7, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(111, 107);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 4;
            pictureBox1.TabStop = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.ForeColor = SystemColors.Info;
            label2.Location = new Point(134, 12);
            label2.Name = "label2";
            label2.Size = new Size(247, 25);
            label2.TabIndex = 5;
            label2.Text = "ForgeAir Installation Wizard";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.ForeColor = SystemColors.Info;
            label3.Location = new Point(12, 159);
            label3.Name = "label3";
            label3.Size = new Size(231, 13);
            label3.TabIndex = 6;
            label3.Text = "This wizard will install ForgeAir in one click!";
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.Black;
            textBox1.BorderStyle = BorderStyle.None;
            textBox1.ForeColor = SystemColors.Info;
            textBox1.Location = new Point(7, 175);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(492, 214);
            textBox1.TabIndex = 7;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.default_background_dark;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(795, 517);
            Controls.Add(textBox1);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(pictureBox1);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(label1);
            Controls.Add(progressBar1);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "Form1";
            ShowIcon = false;
            Text = "ForgeAir Installation Wizard";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ProgressBar progressBar1;
        private Label label1;
        private Button button1;
        private Button button2;
        private PictureBox pictureBox1;
        private Label label2;
        private Label label3;
        private TextBox textBox1;
        private System.ComponentModel.BackgroundWorker installerWorker;
    }
}
