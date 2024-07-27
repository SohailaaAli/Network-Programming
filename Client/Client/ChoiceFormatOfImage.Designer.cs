namespace Client
{
    partial class ChoiceFormatOfImage
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
            this.btnCompressed = new System.Windows.Forms.Button();
            this.imgShowBox = new System.Windows.Forms.PictureBox();
            this.btnOrdinary = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.imgShowBox)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCompressed
            // 
            this.btnCompressed.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCompressed.Location = new System.Drawing.Point(95, 30);
            this.btnCompressed.Name = "btnCompressed";
            this.btnCompressed.Size = new System.Drawing.Size(215, 31);
            this.btnCompressed.TabIndex = 17;
            this.btnCompressed.Text = "Compressed Image";
            this.btnCompressed.UseVisualStyleBackColor = true;
            this.btnCompressed.Click += new System.EventHandler(this.btnCompressed_Click);
            // 
            // imgShowBox
            // 
            this.imgShowBox.Location = new System.Drawing.Point(163, 98);
            this.imgShowBox.Name = "imgShowBox";
            this.imgShowBox.Size = new System.Drawing.Size(412, 265);
            this.imgShowBox.TabIndex = 15;
            this.imgShowBox.TabStop = false;
            // 
            // btnOrdinary
            // 
            this.btnOrdinary.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOrdinary.Location = new System.Drawing.Point(472, 30);
            this.btnOrdinary.Name = "btnOrdinary";
            this.btnOrdinary.Size = new System.Drawing.Size(185, 31);
            this.btnOrdinary.TabIndex = 18;
            this.btnOrdinary.Text = "Ordinary Image";
            this.btnOrdinary.UseVisualStyleBackColor = true;
            this.btnOrdinary.Click += new System.EventHandler(this.btnOrdinary_Click);
            // 
            // ChoiceFormatOfImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnOrdinary);
            this.Controls.Add(this.btnCompressed);
            this.Controls.Add(this.imgShowBox);
            this.Name = "ChoiceFormatOfImage";
            this.Text = "ChoiceFormatOfImage";
            ((System.ComponentModel.ISupportInitialize)(this.imgShowBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnCompressed;
        private System.Windows.Forms.PictureBox imgShowBox;
        private System.Windows.Forms.Button btnOrdinary;
    }
}