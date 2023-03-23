namespace Aeon.Recognizer
{
    partial class MainForm
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
            CloseButton = new Button();
            CommandWindow = new RichTextBox();
            SuspendLayout();
            // 
            // CloseButton
            // 
            CloseButton.Location = new Point(400, 294);
            CloseButton.Name = "CloseButton";
            CloseButton.Size = new Size(75, 23);
            CloseButton.TabIndex = 0;
            CloseButton.Text = "Close";
            CloseButton.UseVisualStyleBackColor = true;
            CloseButton.Click += CloseButtonClick;
            // 
            // CommandWindow
            // 
            CommandWindow.BackColor = SystemColors.InactiveBorder;
            CommandWindow.Location = new Point(18, 30);
            CommandWindow.Name = "CommandWindow";
            CommandWindow.Size = new Size(445, 237);
            CommandWindow.TabIndex = 2;
            CommandWindow.Text = "";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(478, 326);
            Controls.Add(CommandWindow);
            Controls.Add(CloseButton);
            MaximumSize = new Size(494, 365);
            MinimizeBox = false;
            MinimumSize = new Size(494, 365);
            Name = "MainForm";
            Text = "Voice Recognition Form";
            ResumeLayout(false);
        }

        #endregion

        private Button CloseButton;
        private RichTextBox CommandWindow;
    }
}