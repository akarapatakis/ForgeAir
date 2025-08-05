namespace ForgeAir.NAudio.Forms
{
    partial class ConfigurationPage
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
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            comboBox1 = new ComboBox();
            pictureBox1 = new PictureBox();
            comboBox2 = new ComboBox();
            label1 = new Label();
            label2 = new Label();
            comboBox4 = new ComboBox();
            label3 = new Label();
            label4 = new Label();
            textBox1 = new TextBox();
            label5 = new Label();
            label6 = new Label();
            button1 = new Button();
            comboBox3 = new ComboBox();
            label7 = new Label();
            label8 = new Label();
            button2 = new Button();
            button3 = new Button();
            comboBox5 = new ComboBox();
            comboBox6 = new ComboBox();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Location = new Point(12, 103);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(776, 306);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(comboBox6);
            tabPage1.Controls.Add(comboBox5);
            tabPage1.Controls.Add(button2);
            tabPage1.Controls.Add(label8);
            tabPage1.Controls.Add(label7);
            tabPage1.Controls.Add(comboBox3);
            tabPage1.Controls.Add(button1);
            tabPage1.Controls.Add(label6);
            tabPage1.Controls.Add(label5);
            tabPage1.Controls.Add(textBox1);
            tabPage1.Controls.Add(label4);
            tabPage1.Controls.Add(label3);
            tabPage1.Controls.Add(comboBox4);
            tabPage1.Controls.Add(label2);
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(comboBox2);
            tabPage1.Controls.Add(comboBox1);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(768, 278);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Devices";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(6, 94);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(424, 23);
            comboBox1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.naudio_banner;
            pictureBox1.Location = new Point(16, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(768, 85);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // comboBox2
            // 
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(6, 148);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(424, 23);
            comboBox2.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 76);
            label1.Name = "label1";
            label1.Size = new Size(78, 15);
            label1.TabIndex = 2;
            label1.Text = "Main Output:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 130);
            label2.Name = "label2";
            label2.Size = new Size(131, 15);
            label2.TabIndex = 3;
            label2.Text = "Monitor/Booth Output:";
            // 
            // comboBox4
            // 
            comboBox4.FormattingEnabled = true;
            comboBox4.Location = new Point(537, 94);
            comboBox4.Name = "comboBox4";
            comboBox4.Size = new Size(62, 23);
            comboBox4.TabIndex = 5;
            comboBox4.Text = "192000";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(513, 97);
            label3.Name = "label3";
            label3.Size = new Size(18, 15);
            label3.TabIndex = 6;
            label3.Text = "@";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(513, 151);
            label4.Name = "label4";
            label4.Size = new Size(18, 15);
            label4.TabIndex = 7;
            label4.Text = "@";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(8, 232);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(100, 23);
            textBox1.TabIndex = 8;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(6, 214);
            label5.Name = "label5";
            label5.Size = new Size(130, 15);
            label5.TabIndex = 9;
            label5.Text = "Desired Buffer/Latency:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(114, 235);
            label6.Name = "label6";
            label6.Size = new Size(23, 15);
            label6.TabIndex = 10;
            label6.Text = "ms";
            // 
            // button1
            // 
            button1.Enabled = false;
            button1.Location = new Point(630, 94);
            button1.Name = "button1";
            button1.Size = new Size(122, 23);
            button1.TabIndex = 11;
            button1.Text = "ASIO Configuration";
            button1.UseVisualStyleBackColor = true;
            // 
            // comboBox3
            // 
            comboBox3.FormattingEnabled = true;
            comboBox3.Location = new Point(537, 148);
            comboBox3.Name = "comboBox3";
            comboBox3.Size = new Size(62, 23);
            comboBox3.TabIndex = 12;
            comboBox3.Text = "192000";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(605, 98);
            label7.Name = "label7";
            label7.Size = new Size(21, 15);
            label7.TabIndex = 13;
            label7.Text = "Hz";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(605, 151);
            label8.Name = "label8";
            label8.Size = new Size(21, 15);
            label8.TabIndex = 14;
            label8.Text = "Hz";
            // 
            // button2
            // 
            button2.Enabled = false;
            button2.Location = new Point(630, 147);
            button2.Name = "button2";
            button2.Size = new Size(122, 23);
            button2.TabIndex = 15;
            button2.Text = "ASIO Configuration";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(709, 415);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 2;
            button3.Text = "Apply";
            button3.UseVisualStyleBackColor = true;
            // 
            // comboBox5
            // 
            comboBox5.FormattingEnabled = true;
            comboBox5.Location = new Point(446, 94);
            comboBox5.Name = "comboBox5";
            comboBox5.Size = new Size(61, 23);
            comboBox5.TabIndex = 16;
            comboBox5.Text = "Stereo";
            // 
            // comboBox6
            // 
            comboBox6.FormattingEnabled = true;
            comboBox6.Location = new Point(446, 148);
            comboBox6.Name = "comboBox6";
            comboBox6.Size = new Size(61, 23);
            comboBox6.TabIndex = 17;
            comboBox6.Text = "Stereo";
            // 
            // ConfigurationPage
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button3);
            Controls.Add(pictureBox1);
            Controls.Add(tabControl1);
            Name = "ConfigurationPage";
            Text = "Configure NAudio";
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabPage1;
        private ComboBox comboBox1;
        private PictureBox pictureBox1;
        private ComboBox comboBox2;
        private Label label1;
        private Label label2;
        private Label label4;
        private Label label3;
        private ComboBox comboBox4;
        private Label label6;
        private Label label5;
        private TextBox textBox1;
        private Button button1;
        private Label label8;
        private Label label7;
        private ComboBox comboBox3;
        private Button button2;
        private Button button3;
        private ComboBox comboBox5;
        private ComboBox comboBox6;
    }
}