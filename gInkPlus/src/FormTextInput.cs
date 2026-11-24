using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace gInkPlus
{
    public partial class FormTextInput : Form
    {

        private Point textLocation;
        private bool isEditing;
        public event Action<string, Point> TextCommitted;



        public FormTextInput()
        {
            InitializeComponent();
            KeyDown += FormTextInput_KeyDown;


        }

        public void BeginTextInput(Point location, string existingText = "")
        {
            textLocation = location;
            isEditing = !string.IsNullOrEmpty(existingText);

            txtInput.Text = existingText;
            txtInput.SelectionStart = txtInput.Text.Length;

            Location = location;
            Show();
            Activate();

            BeginInvoke(new Action(() =>
            {
                txtInput.Focus();
            }));
        }
        private void CommitText()
        {
            string text = txtInput.Text;

            if (!string.IsNullOrWhiteSpace(text))
            {
                TextCommitted?.Invoke(text, textLocation);
            }

            Hide();
        }

        private void CancelText()
        {
            Hide();
        }

        private void FormTextInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.SuppressKeyPress = true;
                CommitText();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                CancelText();
            }
        }


    }
}
