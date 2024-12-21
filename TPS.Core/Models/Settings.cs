using System.Text;
using System.Text.Json;
using Microsoft.Win32;

namespace TPS.Core.Models;


public class Settings
{
    public static readonly List<Category> DefaultCategories = [Categories.Tools, Categories.Resources, Categories.Consumables, Categories.Furniture];
    public static readonly List<string> DefaultCategoryNames = DefaultCategories.Select(c => c.Name).ToList();
    private static readonly string SettingsFilePath = Path.Combine("..", "settings.json");


    public static Settings LoadSettings()
    {
        Settings settings;
        if (File.Exists(SettingsFilePath))
        {
            try
            {
                settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(SettingsFilePath));
                settings.CategoriesToScan ??= DefaultCategoryNames;
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not read settings.json, using defaults");
                settings = new Settings();
                File.WriteAllText(SettingsFilePath, JsonSerializer.Serialize(settings));
            }
        }
        else
        {
            settings = new Settings();
            File.WriteAllText(SettingsFilePath, JsonSerializer.Serialize(settings));
        }

        return settings;
    }

    public void SaveSettings()
    {
        File.WriteAllText(SettingsFilePath, JsonSerializer.Serialize(this));
    }

    public ShortcutKey InteractionKey { get; set; } = new() { Key = "E" };
    public string ScannerName { get; set; }

    public List<string> CategoriesToScan { get; set; } = DefaultCategoryNames;
}

public class ShortcutKey
{
    public bool Shift { get; set; }
    public bool Control { get; set; }
    public bool Alt { get; set; }
    public string Key { get; set; }

    public string ToAhkKey()
    {
        var ahkShortcut = string.Empty;

        if (Control)
            ahkShortcut += "^"; // Control in AHK
        if (Shift)
            ahkShortcut += "+"; // Shift in AHK
        if (Alt)
            ahkShortcut += "!"; // Alt in AHK

        if (!string.IsNullOrEmpty(Key))
        {
            ahkShortcut += Key.ToUpper() switch
            {
                "ESCAPE" => "Esc",
                "ENTER" => "Enter",
                "SPACE" => "Space",
                "TAB" => "Tab",
                "UP" => "Up",
                "DOWN" => "Down",
                "LEFT" => "Left",
                "RIGHT" => "Right",
                _ => Key.ToLowerInvariant()
            };

        }

        return ahkShortcut;
    }

    public override string ToString()
    {
        var shortcut = "";

        if (Control)
            shortcut += "Ctrl + ";
        if (Shift)
            shortcut += "Shift + ";
        if (Alt)
            shortcut += "Alt + ";

        if (!string.IsNullOrEmpty(Key))
        {
            shortcut += Key;
        }

        if (shortcut.EndsWith(" + "))
        {
            shortcut = shortcut[..^3];
        }

        return shortcut;
    }
}