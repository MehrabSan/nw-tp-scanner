namespace TPS.Desktop
{
    partial class SettingsForm
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            InteractionKey = new TextBox();
            label2 = new Label();
            ScannerName = new TextBox();
            label1 = new Label();
            SaveButton = new Button();
            ButtonCancel = new Button();
            label3 = new Label();
            CategoriesSelector = new CheckedListBox();
            SuspendLayout();
            // 
            // InteractionKey
            // 
            InteractionKey.Location = new Point(21, 120);
            InteractionKey.Margin = new Padding(3, 4, 3, 4);
            InteractionKey.Name = "InteractionKey";
            InteractionKey.ReadOnly = true;
            InteractionKey.Size = new Size(289, 27);
            InteractionKey.TabIndex = 3;
            InteractionKey.KeyDown += InteractionKey_KeyDown;
            InteractionKey.Leave += InteractionKey_Leave;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(18, 96);
            label2.Name = "label2";
            label2.Size = new Size(258, 20);
            label2.TabIndex = 2;
            label2.Text = "Interaction Key (Click then hit the key)";
            // 
            // ScannerName
            // 
            ScannerName.Location = new Point(21, 49);
            ScannerName.Margin = new Padding(3, 4, 3, 4);
            ScannerName.Name = "ScannerName";
            ScannerName.Size = new Size(289, 27);
            ScannerName.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(18, 25);
            label1.Name = "label1";
            label1.Size = new Size(105, 20);
            label1.TabIndex = 0;
            label1.Text = "Scanner Name";
            // 
            // SaveButton
            // 
            SaveButton.FlatStyle = FlatStyle.Flat;
            SaveButton.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            SaveButton.ForeColor = Color.WhiteSmoke;
            SaveButton.Location = new Point(349, 341);
            SaveButton.Margin = new Padding(3, 4, 3, 4);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(86, 32);
            SaveButton.TabIndex = 2;
            SaveButton.Text = "Save";
            SaveButton.UseVisualStyleBackColor = true;
            SaveButton.Click += SaveButton_Click;
            // 
            // ButtonCancel
            // 
            ButtonCancel.FlatStyle = FlatStyle.Flat;
            ButtonCancel.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ButtonCancel.ForeColor = Color.WhiteSmoke;
            ButtonCancel.Location = new Point(441, 341);
            ButtonCancel.Margin = new Padding(3, 4, 3, 4);
            ButtonCancel.Name = "ButtonCancel";
            ButtonCancel.Size = new Size(86, 32);
            ButtonCancel.TabIndex = 3;
            ButtonCancel.Text = "Cancel";
            ButtonCancel.UseVisualStyleBackColor = true;
            ButtonCancel.Click += ButtonCancel_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(332, 25);
            label3.Name = "label3";
            label3.Size = new Size(133, 20);
            label3.TabIndex = 4;
            label3.Text = "Categories to Scan";
            // 
            // CategoriesSelector
            // 
            CategoriesSelector.CheckOnClick = true;
            CategoriesSelector.FormattingEnabled = true;
            CategoriesSelector.Location = new Point(337, 49);
            CategoriesSelector.Name = "CategoriesSelector";
            CategoriesSelector.Size = new Size(190, 114);
            CategoriesSelector.TabIndex = 5;
            // 
            // SettingsForm
            // 
            AcceptButton = SaveButton;
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            CancelButton = ButtonCancel;
            ClientSize = new Size(547, 384);
            Controls.Add(CategoriesSelector);
            Controls.Add(label3);
            Controls.Add(ButtonCancel);
            Controls.Add(SaveButton);
            Controls.Add(InteractionKey);
            Controls.Add(label2);
            Controls.Add(ScannerName);
            Controls.Add(label1);
            Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ForeColor = Color.WhiteSmoke;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Margin = new Padding(3, 4, 3, 4);
            Name = "SettingsForm";
            Text = "NWMP Scanner Settings";
            Load += SettingsForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox ScannerName;
        private Label label1;
        private TextBox InteractionKey;
        private Label label2;
        private Button SaveButton;
        private Button ButtonCancel;
        private Label label3;
        private CheckedListBox CategoriesSelector;
    }
}