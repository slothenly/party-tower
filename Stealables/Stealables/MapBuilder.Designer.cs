namespace Egg
{
    partial class Mappy
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
            this.txtFile = new System.Windows.Forms.TextBox();
            this.rad5 = new System.Windows.Forms.RadioButton();
            this.rad4 = new System.Windows.Forms.RadioButton();
            this.button151 = new System.Windows.Forms.Button();
            this.rad3 = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.tabletWidth = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabletHeight = new System.Windows.Forms.TextBox();
            this.rad2 = new System.Windows.Forms.RadioButton();
            this.rad1 = new System.Windows.Forms.RadioButton();
            this.chkDelete = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtFile
            // 
            this.txtFile.Location = new System.Drawing.Point(40, 552);
            this.txtFile.Name = "txtFile";
            this.txtFile.Size = new System.Drawing.Size(156, 20);
            this.txtFile.TabIndex = 31;
            this.txtFile.Text = "<text file name>";
            // 
            // rad5
            // 
            this.rad5.AutoSize = true;
            this.rad5.Checked = true;
            this.rad5.Location = new System.Drawing.Point(38, 356);
            this.rad5.Name = "rad5";
            this.rad5.Size = new System.Drawing.Size(58, 17);
            this.rad5.TabIndex = 30;
            this.rad5.TabStop = true;
            this.rad5.Text = "Normal";
            this.rad5.UseVisualStyleBackColor = true;
            // 
            // rad4
            // 
            this.rad4.AutoSize = true;
            this.rad4.Location = new System.Drawing.Point(38, 447);
            this.rad4.Name = "rad4";
            this.rad4.Size = new System.Drawing.Size(60, 17);
            this.rad4.TabIndex = 28;
            this.rad4.Text = "Moving";
            this.rad4.UseVisualStyleBackColor = true;
            // 
            // button151
            // 
            this.button151.Location = new System.Drawing.Point(38, 593);
            this.button151.Name = "button151";
            this.button151.Size = new System.Drawing.Size(163, 64);
            this.button151.TabIndex = 27;
            this.button151.Text = "Export to Text";
            this.button151.UseVisualStyleBackColor = true;
            this.button151.Click += new System.EventHandler(this.Export);
            // 
            // rad3
            // 
            this.rad3.AutoSize = true;
            this.rad3.Location = new System.Drawing.Point(38, 426);
            this.rad3.Name = "rad3";
            this.rad3.Size = new System.Drawing.Size(86, 17);
            this.rad3.TabIndex = 25;
            this.rad3.Text = "Non-Collision";
            this.rad3.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(127, 495);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 24;
            this.label3.Text = "Tablet Width:";
            // 
            // tabletWidth
            // 
            this.tabletWidth.Location = new System.Drawing.Point(130, 517);
            this.tabletWidth.Name = "tabletWidth";
            this.tabletWidth.Size = new System.Drawing.Size(66, 20);
            this.tabletWidth.TabIndex = 23;
            this.tabletWidth.Text = "16";
            this.tabletWidth.TextChanged += new System.EventHandler(this.HeightWidthChange);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 495);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Tablet Height:";
            // 
            // tabletHeight
            // 
            this.tabletHeight.Location = new System.Drawing.Point(40, 517);
            this.tabletHeight.Name = "tabletHeight";
            this.tabletHeight.Size = new System.Drawing.Size(64, 20);
            this.tabletHeight.TabIndex = 21;
            this.tabletHeight.Text = "9";
            this.tabletHeight.TextChanged += new System.EventHandler(this.HeightWidthChange);
            // 
            // rad2
            // 
            this.rad2.AutoSize = true;
            this.rad2.Location = new System.Drawing.Point(38, 402);
            this.rad2.Name = "rad2";
            this.rad2.Size = new System.Drawing.Size(96, 17);
            this.rad2.TabIndex = 20;
            this.rad2.Text = "Non-Damaging";
            this.rad2.UseVisualStyleBackColor = true;
            // 
            // rad1
            // 
            this.rad1.AutoSize = true;
            this.rad1.Location = new System.Drawing.Point(38, 379);
            this.rad1.Name = "rad1";
            this.rad1.Size = new System.Drawing.Size(73, 17);
            this.rad1.TabIndex = 19;
            this.rad1.Text = "Damaging";
            this.rad1.UseVisualStyleBackColor = true;
            // 
            // chkDelete
            // 
            this.chkDelete.AutoSize = true;
            this.chkDelete.Location = new System.Drawing.Point(142, 357);
            this.chkDelete.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkDelete.Name = "chkDelete";
            this.chkDelete.Size = new System.Drawing.Size(57, 17);
            this.chkDelete.TabIndex = 32;
            this.chkDelete.Text = "Delete";
            this.chkDelete.UseVisualStyleBackColor = true;
            this.chkDelete.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged_1);
            // 
            // Mappy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1186, 709);
            this.Controls.Add(this.chkDelete);
            this.Controls.Add(this.txtFile);
            this.Controls.Add(this.rad5);
            this.Controls.Add(this.rad4);
            this.Controls.Add(this.button151);
            this.Controls.Add(this.rad3);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tabletWidth);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tabletHeight);
            this.Controls.Add(this.rad2);
            this.Controls.Add(this.rad1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Mappy";
            this.Text = "MapBuilder";
            this.Load += new System.EventHandler(this.MapBuilder_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFile;
        private System.Windows.Forms.RadioButton rad5;
        private System.Windows.Forms.RadioButton rad4;
        private System.Windows.Forms.Button button151;
        private System.Windows.Forms.RadioButton rad3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tabletWidth;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tabletHeight;
        private System.Windows.Forms.RadioButton rad2;
        private System.Windows.Forms.RadioButton rad1;
        private System.Windows.Forms.CheckBox chkDelete;
    }
}