
namespace Daugiagijio_antras_praktinis
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
            this.encrypt = new System.Windows.Forms.Button();
            this.decrypt = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.pauseButton = new System.Windows.Forms.Button();
            this.continueButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // encrypt
            // 
            this.encrypt.Location = new System.Drawing.Point(48, 40);
            this.encrypt.Name = "encrypt";
            this.encrypt.Size = new System.Drawing.Size(206, 37);
            this.encrypt.TabIndex = 0;
            this.encrypt.Text = "Šifruoti pasirinkto katalogo failus";
            this.encrypt.UseVisualStyleBackColor = true;
            this.encrypt.Click += new System.EventHandler(this.encrypt_Click);
            // 
            // decrypt
            // 
            this.decrypt.Location = new System.Drawing.Point(48, 107);
            this.decrypt.Name = "decrypt";
            this.decrypt.Size = new System.Drawing.Size(206, 37);
            this.decrypt.TabIndex = 1;
            this.decrypt.Text = "Dešfruoti pasirinkto katalogo failus";
            this.decrypt.UseVisualStyleBackColor = true;
            this.decrypt.Click += new System.EventHandler(this.decrypt_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 180);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(286, 23);
            this.progressBar.TabIndex = 2;
            // 
            // pauseButton
            // 
            this.pauseButton.Location = new System.Drawing.Point(12, 231);
            this.pauseButton.Name = "pauseButton";
            this.pauseButton.Size = new System.Drawing.Size(75, 23);
            this.pauseButton.TabIndex = 3;
            this.pauseButton.Text = "PAUZĖ";
            this.pauseButton.UseVisualStyleBackColor = true;
            this.pauseButton.Click += new System.EventHandler(this.pauseButton_Click);
            // 
            // continueButton
            // 
            this.continueButton.Location = new System.Drawing.Point(117, 231);
            this.continueButton.Name = "continueButton";
            this.continueButton.Size = new System.Drawing.Size(75, 23);
            this.continueButton.TabIndex = 4;
            this.continueButton.Text = "TĘSTI";
            this.continueButton.UseVisualStyleBackColor = true;
            this.continueButton.Click += new System.EventHandler(this.continueButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(223, 231);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 5;
            this.stopButton.Text = "ATŠAUKTI";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(310, 276);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.continueButton);
            this.Controls.Add(this.pauseButton);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.decrypt);
            this.Controls.Add(this.encrypt);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button encrypt;
        private System.Windows.Forms.Button decrypt;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button pauseButton;
        private System.Windows.Forms.Button continueButton;
        private System.Windows.Forms.Button stopButton;
    }
}

