using TPS.Core;
using TPS.Core.Models;

namespace TPS.Desktop
{
    public partial class SettingsForm : Form
    {
        private ShortcutKey currentShortcut = Scanner.Settings.InteractionKey;

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void InteractionKey_KeyDown(object sender, KeyEventArgs e)
        {
            InteractionKey.Clear();

            currentShortcut = new ShortcutKey
            {
                Alt = e.Alt,
                Control = e.Control,
                Shift = e.Shift,
                Key = e.KeyCode.ToString()
            };

            InteractionKey.Text = currentShortcut.ToString();
            e.SuppressKeyPress = true;

        }

        private void InteractionKey_Leave(object sender, EventArgs e)
        {
            InteractionKey.Text = currentShortcut.ToString();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            ScannerName.Text = Scanner.Settings.ScannerName;
            InteractionKey.Text = currentShortcut.ToString();

            foreach (var category in Settings.DefaultCategories)
            {
                CategoriesSelector.Items.Add(category.Name);
            }

            foreach (var categoryName in Scanner.Settings.CategoriesToScan)
            {

                CategoriesSelector.SetItemChecked(Settings.DefaultCategoryNames.IndexOf(categoryName), true);
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var settings = new Settings
            {
                ScannerName = ScannerName.Text.Trim(),
                InteractionKey = currentShortcut,
                CategoriesToScan = CategoriesSelector.CheckedItems.Cast<string>().ToList()
            };

            settings.SaveSettings();
            Scanner.Settings = settings;
            DialogResult = DialogResult.OK;
            Hide();

        }
    }

}
