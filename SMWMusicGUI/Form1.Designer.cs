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
            this.SuspendLayout();
            // 
            // Select_ROM
            // 
            this.Select_ROM.Location = new System.Drawing.Point(12, 408);
            this.Select_ROM.Name = "Select_ROM";
            this.Select_ROM.Size = new System.Drawing.Size(117, 30);
            this.Select_ROM.TabIndex = 0;
            this.Select_ROM.Text = "Select ROM";
            this.Select_ROM.UseVisualStyleBackColor = true;
            this.Select_ROM.Click += new System.EventHandler(this.Select_ROM_Click);
            // 
            // Output
            // 
            this.Output.Location = new System.Drawing.Point(13, 13);
            this.Output.Name = "Output";
            this.Output.ReadOnly = true;
            this.Output.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.Output.Size = new System.Drawing.Size(818, 389);
            this.Output.TabIndex = 2;
            this.Output.Text = "";
            this.Output.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.Output_LinkClicked);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 450);
            this.Controls.Add(this.Output);
            this.Controls.Add(this.Select_ROM);
            this.Name = "Form1";
            this.Text = "SMW Music Identifier";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Select_ROM;
        private System.Windows.Forms.RichTextBox Output;
    }
}

