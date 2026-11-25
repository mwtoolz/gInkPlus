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
        private bool isClosing;



        public FormTextInput()
        {
            InitializeComponent();
            KeyDown += FormTextInput_KeyDown;
            Deactivate += FormTextInput_Deactivate;


        }

        public void BeginTextInput(Point location, string existingText = "")
        {

            isClosing = false;
            Root.IsInputtingText = true;

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

            if (isClosing) return;

            isClosing = true;

            string text = txtInput.Text;

            if (!string.IsNullOrWhiteSpace(text))
            {
                TextCommitted?.Invoke(text, textLocation);
            }

            Root.IsInputtingText = false;
            Hide();
        }

        private void CancelText()
        {
            if (isClosing) return;
            
            isClosing = true;
            Root.IsInputtingText = false;
            Hide();
        }

        private void FormTextInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                CommitText();
            }

            else if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                Root.SuppressEscapeUntilReleased = true;
                CancelText();
            }
        }

        private void FormTextInput_Deactivate(object sender, EventArgs e)
        {
            if (!isClosing)
            {
                CommitText();
            }
        }


    }
}
