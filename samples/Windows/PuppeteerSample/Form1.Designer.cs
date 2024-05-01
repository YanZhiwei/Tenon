namespace PuppeteerSample
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
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            numericUpDown2 = new NumericUpDown();
            numericUpDown1 = new NumericUpDown();
            button10 = new Button();
            button9 = new Button();
            button8 = new Button();
            button2 = new Button();
            button1 = new Button();
            tabPage2 = new TabPage();
            button7 = new Button();
            button6 = new Button();
            button5 = new Button();
            button4 = new Button();
            button3 = new Button();
            listBox1 = new ListBox();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(800, 315);
            tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(numericUpDown2);
            tabPage1.Controls.Add(numericUpDown1);
            tabPage1.Controls.Add(button10);
            tabPage1.Controls.Add(button9);
            tabPage1.Controls.Add(button8);
            tabPage1.Controls.Add(button2);
            tabPage1.Controls.Add(button1);
            tabPage1.Location = new Point(4, 26);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(792, 285);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Browser ";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // numericUpDown2
            // 
            numericUpDown2.Location = new Point(96, 93);
            numericUpDown2.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numericUpDown2.Minimum = new decimal(new int[] { 10000, 0, 0, int.MinValue });
            numericUpDown2.Name = "numericUpDown2";
            numericUpDown2.Size = new Size(60, 23);
            numericUpDown2.TabIndex = 6;
            numericUpDown2.Value = new decimal(new int[] { 436, 0, 0, 0 });
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(30, 93);
            numericUpDown1.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numericUpDown1.Minimum = new decimal(new int[] { 10000, 0, 0, int.MinValue });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(60, 23);
            numericUpDown1.TabIndex = 5;
            numericUpDown1.Value = new decimal(new int[] { 1819, 0, 0, int.MinValue });
            // 
            // button10
            // 
            button10.Location = new Point(162, 93);
            button10.Name = "button10";
            button10.Size = new Size(75, 23);
            button10.TabIndex = 4;
            button10.Text = "AttachTo";
            button10.UseVisualStyleBackColor = true;
            button10.Click += button10_Click;
            // 
            // button9
            // 
            button9.Location = new Point(360, 38);
            button9.Name = "button9";
            button9.Size = new Size(117, 23);
            button9.TabIndex = 3;
            button9.Text = "GetPagesByUrl";
            button9.UseVisualStyleBackColor = true;
            button9.Click += button9_Click;
            // 
            // button8
            // 
            button8.Location = new Point(226, 38);
            button8.Name = "button8";
            button8.Size = new Size(117, 23);
            button8.TabIndex = 2;
            button8.Text = "GetPagesByTitle";
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // button2
            // 
            button2.Location = new Point(126, 38);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 1;
            button2.Text = "Close";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.Location = new Point(29, 38);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 0;
            button1.Text = "Open";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(button7);
            tabPage2.Controls.Add(button6);
            tabPage2.Controls.Add(button5);
            tabPage2.Controls.Add(button4);
            tabPage2.Controls.Add(button3);
            tabPage2.Location = new Point(4, 26);
            tabPage2.Name = "tabPage2";
            tabPage2.Size = new Size(792, 285);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Page";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            button7.Location = new Point(571, 26);
            button7.Name = "button7";
            button7.Size = new Size(107, 23);
            button7.TabIndex = 8;
            button7.Text = "BringToFront";
            button7.UseVisualStyleBackColor = true;
            button7.Click += button7_Click;
            // 
            // button6
            // 
            button6.Location = new Point(447, 26);
            button6.Name = "button6";
            button6.Size = new Size(101, 23);
            button6.TabIndex = 7;
            button6.Text = "IsActive";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // button5
            // 
            button5.Location = new Point(333, 26);
            button5.Name = "button5";
            button5.Size = new Size(99, 23);
            button5.TabIndex = 6;
            button5.Text = "IsReady";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // button4
            // 
            button4.Location = new Point(169, 26);
            button4.Name = "button4";
            button4.Size = new Size(145, 23);
            button4.TabIndex = 5;
            button4.Text = "EvaluateFunction";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button3
            // 
            button3.Location = new Point(8, 26);
            button3.Name = "button3";
            button3.Size = new Size(155, 23);
            button3.TabIndex = 4;
            button3.Text = "InjectScriptTag";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 17;
            listBox1.Location = new Point(4, 317);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(792, 140);
            listBox1.TabIndex = 2;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(listBox1);
            Controls.Add(tabControl1);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            tabPage2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabPage1;
        private Button button1;
        private Button button2;
        private ListBox listBox1;
        private TabPage tabPage2;
        private Button button5;
        private Button button4;
        private Button button3;
        private Button button6;
        private Button button7;
        private Button button9;
        private Button button10;
        private Button button8;
        private NumericUpDown numericUpDown2;
        private NumericUpDown numericUpDown1;
    }
}
