namespace SMWMusicGUI
{
    partial class Form1
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
            this.Select_ROM = new System.Windows.Forms.Button();
            this.Output = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Select_ROM
            // 
            this.Select_ROM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Select_ROM.Location = new System.Drawing.Point(9, 332);
            this.Select_ROM.Margin = new System.Windows.Forms.Padding(2);
            this.Select_ROM.Name = "Select_ROM";
            this.Select_ROM.Size = new System.Drawing.Size(88, 24);
            this.Select_ROM.TabIndex = 0;
            this.Select_ROM.Text = "Select ROM";
            this.Select_ROM.UseVisualStyleBackColor = true;
            this.Select_ROM.Click += new System.EventHandler(this.Select_ROM_Click);
            // 
            // Output
            // 
            this.Output.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Output.Location = new System.Drawing.Point(10, 11);
            this.Output.Margin = new System.Windows.Forms.Padding(2);
            this.Output.Name = "Output";
            this.Output.ReadOnly = true;
            this.Output.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.Output.Size = new System.Drawing.Size(614, 317);
            this.Output.TabIndex = 2;
            this.Output.Text = "";
            this.Output.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.Output_LinkClicked);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label1.Location = new System.Drawing.Point(520, 338);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "patched by wepeelis";
            this.label1.DoubleClick += new System.EventHandler(this.Click_patched);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 366);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Output);
            this.Controls.Add(this.Select_ROM);
            this.Icon = global::SMWMusicGUI.Properties.Resources.Icon1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 200);
            this.Name = "Form1";
            this.Text = "SMW Music Identifier";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Select_ROM;
        private System.Windows.Forms.RichTextBox Output;
        private System.Windows.Forms.Label label1;
    }
}

