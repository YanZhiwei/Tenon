namespace AutomationSample
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
            splitContainer1 = new SplitContainer();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            numericUpDown4 = new NumericUpDown();
            numericUpDown3 = new NumericUpDown();
            numericUpDown2 = new NumericUpDown();
            numericUpDown1 = new NumericUpDown();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            button3 = new Button();
            button2 = new Button();
            button1 = new Button();
            tabPage2 = new TabPage();
            button5 = new Button();
            button4 = new Button();
            listBox1 = new ListBox();
            button6 = new Button();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(listBox1);
            splitContainer1.Size = new Size(800, 450);
            splitContainer1.SplitterDistance = 314;
            splitContainer1.TabIndex = 0;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(800, 314);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(numericUpDown4);
            tabPage1.Controls.Add(numericUpDown3);
            tabPage1.Controls.Add(numericUpDown2);
            tabPage1.Controls.Add(numericUpDown1);
            tabPage1.Controls.Add(label4);
            tabPage1.Controls.Add(label3);
            tabPage1.Controls.Add(label2);
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(button3);
            tabPage1.Controls.Add(button2);
            tabPage1.Controls.Add(button1);
            tabPage1.Location = new Point(4, 26);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(792, 284);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "WindowsHighlightRectangle";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // numericUpDown4
            // 
            numericUpDown4.Location = new Point(497, 25);
            numericUpDown4.Maximum = new decimal(new int[] { 1080, 0, 0, 0 });
            numericUpDown4.Name = "numericUpDown4";
            numericUpDown4.Size = new Size(71, 23);
            numericUpDown4.TabIndex = 14;
            numericUpDown4.Value = new decimal(new int[] { 300, 0, 0, 0 });
            // 
            // numericUpDown3
            // 
            numericUpDown3.Location = new Point(365, 24);
            numericUpDown3.Maximum = new decimal(new int[] { 1080, 0, 0, 0 });
            numericUpDown3.Name = "numericUpDown3";
            numericUpDown3.Size = new Size(71, 23);
            numericUpDown3.TabIndex = 13;
            numericUpDown3.Value = new decimal(new int[] { 300, 0, 0, 0 });
            // 
            // numericUpDown2
            // 
            numericUpDown2.Location = new Point(235, 22);
            numericUpDown2.Name = "numericUpDown2";
            numericUpDown2.Size = new Size(71, 23);
            numericUpDown2.TabIndex = 12;
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(134, 22);
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(71, 23);
            numericUpDown1.TabIndex = 11;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(442, 25);
            label4.Name = "label4";
            label4.Size = new Size(49, 17);
            label4.TabIndex = 9;
            label4.Text = "Height:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(315, 26);
            label3.Name = "label3";
            label3.Size = new Size(45, 17);
            label3.TabIndex = 8;
            label3.Text = "Width:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(211, 25);
            label2.Name = "label2";
            label2.Size = new Size(18, 17);
            label2.TabIndex = 6;
            label2.Text = "Y:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(112, 28);
            label1.Name = "label1";
            label1.Size = new Size(19, 17);
            label1.TabIndex = 4;
            label1.Text = "X:";
            // 
            // button3
            // 
            button3.Location = new Point(112, 60);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 2;
            button3.Text = "Close";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button2
            // 
            button2.Location = new Point(30, 60);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 1;
            button2.Text = "Hide";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.Location = new Point(30, 22);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 0;
            button1.Text = "Show";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(button6);
            tabPage2.Controls.Add(button5);
            tabPage2.Controls.Add(button4);
            tabPage2.Location = new Point(4, 26);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(792, 284);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "ChromiumAccessibility";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            button5.Location = new Point(145, 26);
            button5.Name = "button5";
            button5.Size = new Size(161, 28);
            button5.TabIndex = 1;
            button5.Text = "GetTabRectangle";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // button4
            // 
            button4.Location = new Point(25, 26);
            button4.Name = "button4";
            button4.Size = new Size(114, 28);
            button4.TabIndex = 0;
            button4.Text = "GetRectangle";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // listBox1
            // 
            listBox1.Dock = DockStyle.Fill;
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 17;
            listBox1.Location = new Point(0, 0);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(800, 132);
            listBox1.TabIndex = 0;
            // 
            // button6
            // 
            button6.Location = new Point(321, 31);
            button6.Name = "button6";
            button6.Size = new Size(142, 23);
            button6.TabIndex = 2;
            button6.Text = "GetActualRect";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(splitContainer1);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown4).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            tabPage2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private ListBox listBox1;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Button button1;
        private Button button2;
        private Button button3;
        private Label label3;
        private Label label2;
        private Label label1;
        private Label label4;
        private NumericUpDown numericUpDown4;
        private NumericUpDown numericUpDown3;
        private NumericUpDown numericUpDown2;
        private NumericUpDown numericUpDown1;
        private Button button4;
        private Button button5;
        private Button button6;
    }
}
