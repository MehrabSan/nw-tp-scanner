namespace TPS.Desktop
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            DrawPanel = new Panel();
            ControlsPanel = new Panel();
            ControlsTable = new TableLayoutPanel();
            flowLayoutPanel1 = new Panel();
            CancelLabel = new Label();
            SettingsLabel = new Label();
            ScanLabel = new Label();
            label1 = new Label();
            label2 = new Label();
            LogBox = new TextBox();
            ControlsPanel.SuspendLayout();
            ControlsTable.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // DrawPanel
            // 
            DrawPanel.Dock = DockStyle.Fill;
            DrawPanel.Location = new Point(0, 0);
            DrawPanel.Name = "DrawPanel";
            DrawPanel.Size = new Size(1390, 750);
            DrawPanel.TabIndex = 0;
            DrawPanel.Paint += DrawPanel_Paint;
            // 
            // ControlsPanel
            // 
            ControlsPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ControlsPanel.BackColor = Color.Black;
            ControlsPanel.Controls.Add(ControlsTable);
            ControlsPanel.Location = new Point(5, 295);
            ControlsPanel.Margin = new Padding(0);
            ControlsPanel.Name = "ControlsPanel";
            ControlsPanel.Size = new Size(590, 450);
            ControlsPanel.TabIndex = 1;
            // 
            // ControlsTable
            // 
            ControlsTable.ColumnCount = 1;
            ControlsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            ControlsTable.Controls.Add(flowLayoutPanel1, 0, 0);
            ControlsTable.Controls.Add(LogBox, 0, 1);
            ControlsTable.Dock = DockStyle.Fill;
            ControlsTable.Location = new Point(0, 0);
            ControlsTable.Margin = new Padding(0);
            ControlsTable.Name = "ControlsTable";
            ControlsTable.RowCount = 2;
            ControlsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            ControlsTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            ControlsTable.Size = new Size(590, 450);
            ControlsTable.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(CancelLabel);
            flowLayoutPanel1.Controls.Add(SettingsLabel);
            flowLayoutPanel1.Controls.Add(ScanLabel);
            flowLayoutPanel1.Controls.Add(label1);
            flowLayoutPanel1.Controls.Add(label2);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(3, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(584, 38);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // CancelLabel
            // 
            CancelLabel.AutoSize = true;
            CancelLabel.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            CancelLabel.ForeColor = Color.White;
            CancelLabel.Location = new Point(211, 11);
            CancelLabel.Margin = new Padding(0);
            CancelLabel.Name = "CancelLabel";
            CancelLabel.Size = new Size(121, 20);
            CancelLabel.TabIndex = 5;
            CancelLabel.Text = "Cancel (CTRL-C)";
            CancelLabel.Visible = false;
            // 
            // SettingsLabel
            // 
            SettingsLabel.AutoSize = true;
            SettingsLabel.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            SettingsLabel.ForeColor = Color.White;
            SettingsLabel.Location = new Point(381, 11);
            SettingsLabel.Margin = new Padding(0);
            SettingsLabel.Name = "SettingsLabel";
            SettingsLabel.Size = new Size(99, 20);
            SettingsLabel.TabIndex = 4;
            SettingsLabel.Text = "Settings (F6)";
            // 
            // ScanLabel
            // 
            ScanLabel.AutoSize = true;
            ScanLabel.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ScanLabel.ForeColor = Color.White;
            ScanLabel.Location = new Point(211, 11);
            ScanLabel.Margin = new Padding(0);
            ScanLabel.Name = "ScanLabel";
            ScanLabel.Size = new Size(112, 20);
            ScanLabel.TabIndex = 3;
            ScanLabel.Text = "Start Scan (F3)";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.WhiteSmoke;
            label1.Location = new Point(4, 3);
            label1.Name = "label1";
            label1.Size = new Size(193, 32);
            label1.TabIndex = 1;
            label1.Text = "NWMP Scanner";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.White;
            label2.Location = new Point(504, 11);
            label2.Margin = new Padding(0);
            label2.Name = "label2";
            label2.Size = new Size(68, 20);
            label2.TabIndex = 2;
            label2.Text = "Exit (F8)";
            // 
            // LogBox
            // 
            LogBox.BackColor = Color.Black;
            LogBox.Dock = DockStyle.Fill;
            LogBox.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LogBox.ForeColor = Color.WhiteSmoke;
            LogBox.Location = new Point(3, 47);
            LogBox.Multiline = true;
            LogBox.Name = "LogBox";
            LogBox.ReadOnly = true;
            LogBox.Size = new Size(584, 400);
            LogBox.TabIndex = 1;
            LogBox.TabStop = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.LimeGreen;
            ClientSize = new Size(1390, 750);
            Controls.Add(ControlsPanel);
            Controls.Add(DrawPanel);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            Name = "MainForm";
            Text = "NWMP Trading Post Scanner";
            TopMost = true;
            TransparencyKey = Color.LimeGreen;
            Load += MainForm_Load;
            ControlsPanel.ResumeLayout(false);
            ControlsTable.ResumeLayout(false);
            ControlsTable.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        internal Panel DrawPanel;
        private System.ComponentModel.BackgroundWorker MainWorker;
        private Panel ControlsPanel;
        private TableLayoutPanel ControlsTable;
        private Panel flowLayoutPanel1;
        private Label label1;
        internal TextBox LogBox;
        private Label label2;
        private Label SettingsLabel;
        private Label ScanLabel;
        private Label CancelLabel;
    }
}
