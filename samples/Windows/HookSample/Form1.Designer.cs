namespace HookSample
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
            button1 = new Button();
            button2 = new Button();
            groupBox1 = new GroupBox();
            lsOutput = new ListBox();
            button3 = new Button();
            button4 = new Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(12, 33);
            button1.Name = "button1";
            button1.Size = new Size(214, 29);
            button1.TabIndex = 0;
            button1.Text = "KeyboardHook Install";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(241, 33);
            button2.Name = "button2";
            button2.Size = new Size(214, 29);
            button2.TabIndex = 1;
            button2.Text = "KeyboardHook UnInstall";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(lsOutput);
            groupBox1.Location = new Point(1, 167);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(787, 282);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Output";
            // 
            // lsOutput
            // 
            lsOutput.Dock = DockStyle.Fill;
            lsOutput.FormattingEnabled = true;
            lsOutput.Location = new Point(3, 23);
            lsOutput.Name = "lsOutput";
            lsOutput.Size = new Size(781, 256);
            lsOutput.TabIndex = 0;
            // 
            // button3
            // 
            button3.Location = new Point(12, 68);
            button3.Name = "button3";
            button3.Size = new Size(214, 29);
            button3.TabIndex = 3;
            button3.Text = "MouseHook Install";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Location = new Point(241, 68);
            button4.Name = "button4";
            button4.Size = new Size(214, 29);
            button4.TabIndex = 4;
            button4.Text = "MouseHook UnInstall";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(groupBox1);
            Controls.Add(button2);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Form1";
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private Button button2;
        private GroupBox groupBox1;
        private ListBox lsOutput;
        private Button button3;
        private Button button4;
    }
}
