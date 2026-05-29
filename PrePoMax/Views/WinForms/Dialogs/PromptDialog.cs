using System;
using System.Windows.Forms;

namespace PrePoMax.Views.WinForms.Dialogs
{
    internal static class PromptDialog
    {
        /// <summary>
        /// Muestra un cuadro de diálogo simple con un label, textbox y botón OK.
        /// </summary>
        /// <param name="text">Texto de instrucción.</param>
        /// <param name="caption">Título de la ventana.</param>
        /// <param name="defaultValue">Valor inicial opcional.</param>
        /// <returns>Texto ingresado por el usuario, o string.Empty si canceló.</returns>
        public static string Show(string text, string caption, string defaultValue = "")
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 150,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MinimizeBox = false,
                MaximizeBox = false
            };

            Label lblText = new Label() { Left = 20, Top = 20, Text = text, Width = 340 };
            TextBox txtInput = new TextBox() { Left = 20, Top = 50, Width = 340, Text = defaultValue };
            Button btnOk = new Button() { Text = "OK", Left = 280, Width = 80, Top = 80, DialogResult = DialogResult.OK };
            Button btnCancel = new Button() { Text = "Cancel", Left = 190, Width = 80, Top = 80, DialogResult = DialogResult.Cancel };

            prompt.Controls.Add(lblText);
            prompt.Controls.Add(txtInput);
            prompt.Controls.Add(btnOk);
            prompt.Controls.Add(btnCancel);

            prompt.AcceptButton = btnOk;
            prompt.CancelButton = btnCancel;

            return prompt.ShowDialog() == DialogResult.OK ? txtInput.Text : string.Empty;
        }
    }
}
