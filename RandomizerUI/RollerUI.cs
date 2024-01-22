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
            string[] chars = Roller.BuildCharacterList();
            Job[] jobs = Roller.BuildJobList();
            CharacterContextMenus(chars);
            JobContextMenus(jobs);
        }

        private void CharacterContextMenus(string[] chars)
        {
            picChar1.ContextMenu = new ContextMenu(ToMenuItems(chars, 0, ImageType.Character));
            picChar2.ContextMenu = new ContextMenu(ToMenuItems(chars, 1, ImageType.Character));
            picChar3.ContextMenu = new ContextMenu(ToMenuItems(chars, 2, ImageType.Character));
        }

        private void JobContextMenus(Job[] jobs)
        {
            picMain1.ContextMenu = new ContextMenu(ToMenuItems(jobs, 0, ImageType.Main));
            picMain1.BackgroundImageChanged += JobUpdate_BackgroundImageChanged;
            picMain2.ContextMenu = new ContextMenu(ToMenuItems(jobs, 1, ImageType.Main));
            picMain2.BackgroundImageChanged += JobUpdate_BackgroundImageChanged;
            picMain3.ContextMenu = new ContextMenu(ToMenuItems(jobs, 2, ImageType.Main));
            picMain3.BackgroundImageChanged += JobUpdate_BackgroundImageChanged;

            picSub1.ContextMenu = new ContextMenu(ToMenuItems(jobs, 0, ImageType.Sub));
            picSub1.BackgroundImageChanged += JobUpdate_BackgroundImageChanged;
            picSub2.ContextMenu = new ContextMenu(ToMenuItems(jobs, 1, ImageType.Sub));
            picSub2.BackgroundImageChanged += JobUpdate_BackgroundImageChanged;
            picSub3.ContextMenu = new ContextMenu(ToMenuItems(jobs, 2, ImageType.Sub));
            picSub3.BackgroundImageChanged += JobUpdate_BackgroundImageChanged;
        }

        private void JobUpdate_BackgroundImageChanged(object sender, EventArgs e)
        {
            PictureBox box = (PictureBox)sender;
            int index = 0;
            PictureBox picBox = null;
            Label lbl = null;

            if (((string)box.Tag).EndsWith("1"))
            {
                index = 0;
                picBox = picWeapon1;
                lbl = lblWeapon1;
            }
            else if (((string)box.Tag).EndsWith("2"))
            {
                index = 1;
                picBox = picWeapon2;
                lbl = lblWeapon2;
            }
            else if (((string)box.Tag).EndsWith("3"))
            {
                index = 2;
                picBox = picWeapon3;
                lbl = lblWeapon3;
            }

            Character character = _current.Characters[index];
            if ((character.Main == null || !character.Main.Weapons.Contains(character.Weapon))
                && (character.Sub == null || !character.Sub.Weapons.Contains(character.Weapon)))
            {
                lbl.Text = string.Empty;
                picBox.BackgroundImage = null;
            }

            List<string> weapons = new List<string>();

            if (character.Main != null)
            {
                weapons.AddRange(character.Main.Weapons);
            }

            if (character.Sub != null)
            {
                weapons.AddRange(character.Sub.Weapons);
            }

            picBox.ContextMenu = new ContextMenu(ToMenuItems(weapons.ToArray(), index, ImageType.Weapon));
        }

        private void btnRoll_Click(object sender, EventArgs e)
        {
            ClearAll();
            Roller roller = new Roller(chkUnique.Checked, chkWeapon.Checked, chkClassic.Checked, chkMainOnly.Checked, chkCharacters.Checked);

            DateTime now = DateTime.Now;
            _current = roller.Roll(now.Year + now.Month + now.Day + now.Hour + now.Minute + now.Second + now.Millisecond);
            PopulateUI();
        }

        private void PopulateUI()
        {
            chkUnique.Checked = _current.Unique;
            chkWeapon.Checked = _current.Weapons;
            chkClassic.Checked = _current.Classic;
            chkMainOnly.Checked = _current.MainOnly;
            chkCharacters.Checked = _current.RandomCharacters;

            IEnumerable<Label> labels = GetControlsOfType<Label>();
            IEnumerable<PictureBox> pictures = GetControlsOfType<PictureBox>();

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
                {
                    imgCharacter.BackgroundImage = (Image)Resources.ResourceManager.GetObject(_current.Characters[i].Name);
                }

                PictureBox imgMain = pictures.First(a => a.Tag != null && a.Tag.ToString() == $"Main{i + 1}");
                if (_current.Characters[i].Main.Name != "None")
                {
                    imgMain.BackgroundImage = (Image)Resources.ResourceManager.GetObject(_current.Characters[i].Main.Name);
                }

                PictureBox imgSub = pictures.First(a => a.Tag != null && a.Tag.ToString() == $"Sub{i + 1}");
                if (_current.Characters[i].Sub.Name != "None")
                {
                    imgSub.BackgroundImage = (Image)Resources.ResourceManager.GetObject(_current.Characters[i].Sub.Name);
                }

                PictureBox imgWeapon = pictures.First(a => a.Tag != null && a.Tag.ToString() == $"Weapon{i + 1}");
                if (_current.Characters[i].Weapon != "Any")
                {
                    imgWeapon.BackgroundImage = (Image)Resources.ResourceManager.GetObject(_current.Characters[i].Weapon);
                }
            }
        }

        private void ClearAll()
        {
            IEnumerable<Label> labels = GetControlsOfType<Label>();
            IEnumerable<Label> toClear = labels.Where(a => a.Tag != null &&
            (a.Tag.ToString().StartsWith("Main") || a.Tag.ToString().StartsWith("Sub")
            || a.Tag.ToString().StartsWith("Weapon") || a.Tag.ToString().StartsWith("Char")));

            foreach (Label lbl in toClear)
            {
                lbl.Text = string.Empty;
            }

            IEnumerable<PictureBox> images = GetControlsOfType<PictureBox>();
            IEnumerable<PictureBox> imagesToClear = images.Where(a => a.Tag != null &&
            (a.Tag.ToString().StartsWith("Main") || a.Tag.ToString().StartsWith("Sub")
            || a.Tag.ToString().StartsWith("Weapon") || a.Tag.ToString().StartsWith("Char")));

            foreach (PictureBox img in imagesToClear)
            {
                img.BackgroundImage = null;
            }
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
            List<string> split = file.Split('\\').ToList();
            split.RemoveAt(split.Count - 1);
            UpdateConfigValue("SaveLocation", string.Join("\\", split));
        }

        private void UpdateConfigValue(string setting, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            KeyValueConfigurationElement key = config.AppSettings.Settings[setting];

            if (key == null)
            {
                config.AppSettings.Settings.Add(setting, value);
            }
            else
            {
                config.AppSettings.Settings[setting].Value = value;
            }

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void SetTheme()
        {
            string theme = ConfigurationManager.AppSettings.Get("Theme");

            if (theme.ToLower().Equals("light"))
            {
                SetControlAndChildrenColors(this, Theme.Light);
            }
            else
            {
                SetControlAndChildrenColors(this, Theme.Dark);
            }
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
            {
                SetHorizontal();
            }
            else
            {
                SetVertical();
            }
        }

        private void SetHorizontal()
        {
            box1.Location = new Point(box1.Location.X, 30);
            box2.Location = new Point(box1.Size.Width + box1.Location.X + 10, 30);
            box3.Location = new Point(box1.Size.Width + box2.Location.X + 10, 30);

            Size = new Size(box1.Size.Width + box3.Location.X + 25, box1.Size.Height + 75);
            Refresh();
        }

        private void SetVertical()
        {
            box1.Location = new Point(box1.Location.X, 30);
            box2.Location = new Point(box1.Location.X, box1.Location.Y + box1.Size.Height + 5);
            box3.Location = new Point(box1.Location.X, box2.Location.Y + box1.Size.Height + 5);

            Size = new Size(box1.Location.X + box1.Size.Width + 25, box3.Location.Y + box1.Size.Height + 50);
            Refresh();
        }

        public MenuItem[] ToMenuItems(string[] list, int index, ImageType type)
        {
            MenuItem[] ret = new MenuItem[list.Length + 1];

            for (int i = 0; i < list.Length; i++)
            {
                MenuItem item = new MenuItem(list[i]);
                switch (type)
                {
                    case ImageType.Character: item.Click += Character_Click; break;
                    case ImageType.Weapon: item.Click += Weapon_Click; break;
                }

                ret[i] = item;
                item.Tag = index;
            }

            MenuItem clear = new MenuItem("Clear");
            switch (type)
            {
                case ImageType.Character: clear.Click += Character_Click; break;
                case ImageType.Weapon: clear.Click += Weapon_Click; break;
            }

            ret[list.Length] = clear;
            clear.Tag = index;

            return ret;
        }

        public MenuItem[] ToMenuItems(Job[] jobs, int index, ImageType type)
        {
            MenuItem[] ret = new MenuItem[jobs.Length + 1];

            for (int i = 0; i < jobs.Length; i++)
            {
                Job job = jobs[i];
                MenuItem item = new MenuItem(job.Name);
                switch (type)
                {
                    case ImageType.Main: item.Click += Main_Click; break;
                    case ImageType.Sub: item.Click += Sub_Click; break;
                }

                ret[i] = item;
                item.Tag = new Tuple<int, Job>(index, job);
            }

            MenuItem clear = new MenuItem("Clear");
            switch (type)
            {
                case ImageType.Main: clear.Click += Main_Click; break;
                case ImageType.Sub: clear.Click += Sub_Click; break;
            }

            ret[jobs.Length] = clear;
            clear.Tag = new Tuple<int, Job>(index, Job.None);

            return ret;
        }

        private void Character_Click(object sender, EventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            int box = (int)mi.Tag;
            string text = mi.Text;
            _current.Characters[box].Name = text;

            PictureBox pictureBox = null;
            Label nameLbl = null;

            switch (box)
            {
                case 0:
                    pictureBox = picChar1;
                    nameLbl = lblChar1;
                    break;
                case 1:
                    pictureBox = picChar2;
                    nameLbl = lblChar2;
                    break;
                case 2:
                    pictureBox = picChar3;
                    nameLbl = lblChar3;
                    break;
            }

            if (text != "Clear")
            {
                pictureBox.BackgroundImage = (Image)Resources.ResourceManager.GetObject(text);
                nameLbl.Text = text;
            }
            else
            {
                pictureBox.BackgroundImage = null;
                nameLbl.Text = string.Empty;
            }
        }

        private void Weapon_Click(object sender, EventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            int box = (int)mi.Tag;
            string text = mi.Text;

            PictureBox pictureBox = null;
            Label nameLbl = null;

            switch (box)
            {
                case 0:
                    pictureBox = picWeapon1;
                    nameLbl = lblWeapon1;
                    break;
                case 1:
                    pictureBox = picWeapon2;
                    nameLbl = lblWeapon2;
                    break;
                case 2:
                    pictureBox = picWeapon3;
                    nameLbl = lblWeapon3;
                    break;
            }

            if (text != "Clear")
            {
                pictureBox.BackgroundImage = (Image)Resources.ResourceManager.GetObject(text);
                nameLbl.Text = text;
                _current.Characters[box].Weapon = text;
            }
            else
            {
                pictureBox.BackgroundImage = null;
                nameLbl.Text = string.Empty;
                _current.Characters[box].Weapon = string.Empty;
            }
        }

        private void Main_Click(object sender, EventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            Tuple<int, Job> box = (Tuple<int, Job>)mi.Tag;
            string text = mi.Text;
            _current.Characters[box.Item1].Main = box.Item2;

            PictureBox pictureBox = null;
            Label nameLbl = null;

            switch (box.Item1)
            {
                case 0:
                    pictureBox = picMain1;
                    nameLbl = lblMain1;
                    break;
                case 1:
                    pictureBox = picMain2;
                    nameLbl = lblMain2;
                    break;
                case 2:
                    pictureBox = picMain3;
                    nameLbl = lblMain3;
                    break;
            }

            if (text != "Clear")
            {
                pictureBox.BackgroundImage = (Image)Resources.ResourceManager.GetObject(text);
                nameLbl.Text = text;
            }
            else
            {
                pictureBox.BackgroundImage = null;
                nameLbl.Text = string.Empty;
            }
        }

        private void Sub_Click(object sender, EventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            Tuple<int, Job> box = (Tuple<int, Job>)mi.Tag;
            string text = mi.Text;

            _current.Characters[box.Item1].Sub = box.Item2;

            PictureBox pictureBox = null;
            Label nameLbl = null;

            switch (box.Item1)
            {
                case 0:
                    pictureBox = picSub1;
                    nameLbl = lblSub1;
                    break;
                case 1:
                    pictureBox = picSub2;
                    nameLbl = lblSub2;
                    break;
                case 2:
                    pictureBox = picSub3;
                    nameLbl = lblSub3;
                    break;
            }

            if (text != "Clear")
            {
                pictureBox.BackgroundImage = (Image)Resources.ResourceManager.GetObject(text);
                nameLbl.Text = text;
            }
            else
            {
                pictureBox.BackgroundImage = null;
                nameLbl.Text = string.Empty;
            }
        }

        #region >>> Menu Strip Events

        private void weaponsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ = Process.Start("https://finalfantasy.fandom.com/wiki/Final_Fantasy_XII_weapons");
        }

        private void jobsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ = Process.Start("https://finalfantasy.fandom.com/wiki/License_Board#List_of_jobs");
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
                    Stream fileStream = openFileDialog.OpenFile();

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

    public enum ImageType
    {
        Character,
        Main,
        Sub,
        Weapon
    }
}