namespace gInkPlus
{
    partial class FormTextInput
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
            txtInput = new System.Windows.Forms.TextBox();
            SuspendLayout();
            // 
            // txtInput
            // 
            txtInput.AcceptsReturn = true;
            txtInput.AcceptsTab = true;
            txtInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            txtInput.Dock = System.Windows.Forms.DockStyle.Fill;
            txtInput.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            txtInput.Location = new System.Drawing.Point(0, 0);
            txtInput.Multiline = true;
            txtInput.Name = "txtInput";
            txtInput.Size = new System.Drawing.Size(800, 450);
            txtInput.TabIndex = 0;
            // 
            // FormTextInput
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(txtInput);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            KeyPreview = true;
            Name = "FormTextInput";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = "Text Input";
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox txtInput;
    }
}