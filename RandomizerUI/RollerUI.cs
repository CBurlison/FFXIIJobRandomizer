using RandomizerRoller.Controllers;
using RandomizerRoller.Models;
using RandomizerUI.Models;
using RandomizerUI.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace RandomizerUI
{
    public partial class RollerUI : Form
    {
        private Roll _current = null;

        public RollerUI()
        {
            InitializeComponent();
            ClearAll();
            toolTip.SetToolTip(chkUnique, "Rolls jobs without any duplicates.");
            toolTip.SetToolTip(chkWeapon, "Rolls required weapon based on jobs assigned. You must use this type of weapon once they become available, regardless of acquisition method. The use of a wiki is suggested.");
            toolTip.SetToolTip(chkClassic, "Rolls Main jobs that existed in Final Fantasy 1. No Sub jobs can be used.");
            toolTip.SetToolTip(chkMainOnly, "Rolls only Main jobs.");
            toolTip.SetToolTip(chkCharacters, "Rolls character assignments.");

            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("Theme")))
            {
                UpdateConfigValue("Theme", "Light");
            }
            SetTheme();

            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("Layout")))
            {
                UpdateConfigValue("Layout", "Horizontal");
            }
            SetLayout();

            _current = new Roll();

            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("SaveLocation")))
            {
                UpdateConfigValue("SaveLocation", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            }

            BuildContextMenus();
        }

        private void BuildContextMenus()
        {
            var chars = Roller.BuildCharacterList();
            var jobs = Roller.BuildJobList();
            CharacterContextMenus(chars);
        }

        private void CharacterContextMenus(string[] chars)
        {
            MenuItem[] menuItems = chars.ToMenuItems();
            
            picChar1.ContextMenu = new ContextMenu(menuItems);
            picChar2.ContextMenu = new ContextMenu(menuItems);
            picChar3.ContextMenu = new ContextMenu(menuItems);
        }

        void btnRoll_Click(object sender, EventArgs e)
        {
            ClearAll();
            Roller roller = new Roller(chkUnique.Checked, chkWeapon.Checked, chkClassic.Checked, chkMainOnly.Checked, chkCharacters.Checked);
            roller.Roll();

            _current = roller.Assignments;
            PopulateUI();
        }

        private void PopulateUI()
        {
            chkUnique.Checked = _current.Unique;
            chkWeapon.Checked = _current.Weapons;
            chkClassic.Checked = _current.Classic;
            chkMainOnly.Checked = _current.MainOnly;
            chkCharacters.Checked = _current.RandomCharacters;

            var labels = GetControlsOfType<Label>();
            var pictures = GetControlsOfType<PictureBox>();

            for (int i = 0; i < _current.Characters.Count; i++)
            {
                Label main = labels.First(a => a.Tag != null && a.Tag.ToString() == $"Main{i + 1}");
                main.Text = _current.Characters[i].Main.Name;
                Label sub = labels.First(a => a.Tag != null && a.Tag.ToString() == $"Sub{i + 1}");
                sub.Text = _current.Characters[i].Sub.Name;
                Label weapon = labels.First(a => a.Tag != null && a.Tag.ToString() == $"Weapon{i + 1}");
                weapon.Text = _current.Characters[i].Weapon;
                Label character = labels.First(a => a.Tag != null && a.Tag.ToString() == $"Char{i + 1}");
                character.Text = _current.Characters[i].Name;

                PictureBox imgCharacter = pictures.First(a => a.Tag != null && a.Tag.ToString() == $"Char{i + 1}");
                if (_current.Characters[i].Name != "Any")
                    imgCharacter.BackgroundImage = (Image)Resources.ResourceManager.GetObject(_current.Characters[i].Name);

                PictureBox imgMain = pictures.First(a => a.Tag != null && a.Tag.ToString() == $"Main{i + 1}");
                if (_current.Characters[i].Main.Name != "None")
                    imgMain.BackgroundImage = (Image)Resources.ResourceManager.GetObject(_current.Characters[i].Main.Name);

                PictureBox imgSub = pictures.First(a => a.Tag != null && a.Tag.ToString() == $"Sub{i + 1}");
                if (_current.Characters[i].Sub.Name != "None")
                    imgSub.BackgroundImage = (Image)Resources.ResourceManager.GetObject(_current.Characters[i].Sub.Name);

                PictureBox imgWeapon = pictures.First(a => a.Tag != null && a.Tag.ToString() == $"Weapon{i + 1}");
                if (_current.Characters[i].Weapon != "Any")
                    imgWeapon.BackgroundImage = (Image)Resources.ResourceManager.GetObject(_current.Characters[i].Weapon);
            }
        }

        void ClearAll()
        {
            var labels = GetControlsOfType<Label>();
            var toClear = labels.Where(a => a.Tag != null && 
            (a.Tag.ToString().StartsWith("Main") || a.Tag.ToString().StartsWith("Sub") 
            || a.Tag.ToString().StartsWith("Weapon") || a.Tag.ToString().StartsWith("Char")));

            foreach (Label lbl in toClear)
                lbl.Text = string.Empty;

            var images = GetControlsOfType<PictureBox>();
            var imagesToClear = images.Where(a => a.Tag != null &&
            (a.Tag.ToString().StartsWith("Main") || a.Tag.ToString().StartsWith("Sub")
            || a.Tag.ToString().StartsWith("Weapon") || a.Tag.ToString().StartsWith("Char")));

            foreach (PictureBox img in imagesToClear)
                img.BackgroundImage = null;
        }

        private IEnumerable<T> GetControlsOfType<T>()
        {
            return from field in GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                   where typeof(Component).IsAssignableFrom(field.FieldType)
                   let component = (Component)field.GetValue(this)
                   where component != null && component is T
                   select (T)Convert.ChangeType(component, typeof(T));
        }

        private void SetSaveLocation(string file)
        {
            var split = file.Split('\\').ToList();
            split.RemoveAt(split.Count - 1);
            UpdateConfigValue("SaveLocation", string.Join("\\", split));
        }

        private void UpdateConfigValue(string setting, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var key = config.AppSettings.Settings[setting];

            if (key == null)
                config.AppSettings.Settings.Add(setting, value);
            else
                config.AppSettings.Settings[setting].Value = value;

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void SetTheme()
        {
            string theme = ConfigurationManager.AppSettings.Get("Theme");

            if (theme.ToLower().Equals("light"))
                SetControlAndChildrenColors(this, Theme.Light);
            else
                SetControlAndChildrenColors(this, Theme.Dark);
        }

        private void SetControlAndChildrenColors(Control control, Theme theme)
        {
            control.BackColor = theme.Background;
            control.ForeColor = theme.Text;
            if (control.HasChildren)
            {
                // Recursively call this method for each child control.
                foreach (Control childControl in control.Controls)
                {
                    SetControlAndChildrenColors(childControl, theme);
                }
            }
        }

        private void SetLayout()
        {
            string theme = ConfigurationManager.AppSettings.Get("Layout");

            if (theme.ToLower().Equals("horizontal"))
                SetHorizontal();
            else
                SetVertical();
        }

        private void SetHorizontal()
        {
            box1.Location = new Point(box1.Location.X, 30);
            box2.Location = new Point(box1.Size.Width + box1.Location.X + 10, 30);
            box3.Location = new Point(box1.Size.Width + box2.Location.X + 10, 30);

            this.Size = new Size(box1.Size.Width + box3.Location.X + 25, box1.Size.Height + 75);
            Refresh();
        }

        private void SetVertical()
        {
            box1.Location = new Point(box1.Location.X, 30);
            box2.Location = new Point(box1.Location.X, box1.Location.Y + box1.Size.Height + 5);
            box3.Location = new Point(box1.Location.X, box2.Location.Y + box1.Size.Height + 5);

            this.Size = new Size(box1.Location.X + box1.Size.Width + 25, box3.Location.Y + box1.Size.Height + 50);
            Refresh();
        }

        #region >>> Menu Strip Events

        private void weaponsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://finalfantasy.fandom.com/wiki/Final_Fantasy_XII_weapons");
        }

        private void jobsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://finalfantasy.fandom.com/wiki/License_Board#List_of_jobs");
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_current.Characters.Count > 0)
            {
                Stream myStream;
                using (SaveFileDialog saveFileDialog1 = new SaveFileDialog())
                {
                    saveFileDialog1.Title = "Save current roll.";
                    saveFileDialog1.Filter = "save files (*.save)|*.save";
                    saveFileDialog1.FilterIndex = 1;
                    saveFileDialog1.InitialDirectory = ConfigurationManager.AppSettings.Get("SaveLocation");
                    saveFileDialog1.RestoreDirectory = true;

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        SetSaveLocation(saveFileDialog1.FileName);
                        _current.SaveLoc = saveFileDialog1.FileName;

                        if ((myStream = saveFileDialog1.OpenFile()) != null)
                        {
                            IFormatter formatter = new BinaryFormatter();
                            formatter.Serialize(myStream, _current);
                            myStream.Close();
                        }
                    }
                }
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Load roll.";
                openFileDialog.Filter = "save files (*.save)|*.save";
                openFileDialog.FilterIndex = 1;
                openFileDialog.InitialDirectory = ConfigurationManager.AppSettings.Get("SaveLocation");
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SetSaveLocation(openFileDialog.FileName);
                    var fileStream = openFileDialog.OpenFile();

                    IFormatter formatter = new BinaryFormatter();
                    _current = (Roll)formatter.Deserialize(fileStream);
                    fileStream.Close();
                    PopulateUI();
                }
            }
        }

        private void lightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateConfigValue("Theme", "Light");
            SetTheme();
        }

        private void darkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateConfigValue("Theme", "Dark");
            SetTheme();
        }

        private void horizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateConfigValue("Layout", "Horizontal");
            SetLayout();
        }

        private void verticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateConfigValue("Layout", "Vertical");
            SetLayout();
        }

        #endregion >>> Menu Strip Events
    }

    public static class ExtensionMethods
    {
        public static MenuItem[] ToMenuItems(this string[] list)
        {
            MenuItem[] ret = new MenuItem[list.Length];

            for (int i = 0; i < list.Length; i++)
            {
                MenuItem item = new MenuItem(list[i]);
                item.Click += Item_Click;
                ret[i] = item;
            }

            return ret;
        }

        private static void Item_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}