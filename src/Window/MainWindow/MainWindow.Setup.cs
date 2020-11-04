using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Calista.MainWindow
{
    public partial class MainWindow
    {
        
        private BindingList<string> Files = new BindingList<string>();

        private void Setup()
        {
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(keyDown);
            this.AllowDrop = true;
            this.DragDrop += new DragEventHandler(dragDropFiles);
            this.DragEnter += new DragEventHandler(dragEnter);

            listBox.Enabled = true;
            listBox.DataSource = Files;
            listBox.SelectionMode = SelectionMode.MultiExtended;

            cancelButton.EnabledChanged += (a, b) => cancelButton.BackColor = cancelButton.Enabled ? SettingsContainer.Settings.Colors.Side1 : SettingsContainer.Settings.Colors.Side2;

            saveTextBox.LostFocus += (a, b) =>
            {
                if (saveTextBox.Text != string.Empty && !saveTextBox.Text.ToLower().EndsWith(".fps.xml"))
                {
                    saveTextBox.Text += ".fps.xml";
                }
            };

        }

        private void dragDropFiles(object sender, DragEventArgs e)
        {
            if (ModifierKeys.HasFlag(Keys.Control))
            {
                saveTextBox.Text = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
                return;
            }
            AddFilesAsync(((string[])e.Data.GetData(DataFormats.FileDrop)).Where(l => Path.GetExtension(l) == ".xml"));

        }

        private void dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Return:
                    if (saveTextBox.Focused) this.Focus();
                    startButton_Click(sender, new EventArgs());
                    break;
                case Keys.O | Keys.Control:
                    filesButton_Click(this, new EventArgs());
                    break;
                case Keys.Control | Keys.S:
                    browseButton_Click(this, new EventArgs());
                    saveTextBox.Focus();
                    break;
                case Keys.Delete:
                    if (saveTextBox.Focused) break;
                    new List<int>(selectedlistBoxIndicies()).Reverse<int>().ToList().ForEach(U => Files.RemoveAt(U));
                    listBox.ClearSelected();
                    break;
                case Keys.Control | Keys.A:
                    if (saveTextBox.Focused) break;
                    for (int i = 0; i < listBox.Items.Count; i++)
                    {
                        listBox.SetSelected(i, true);
                    }
                    break;
                case Keys.Escape:
                    this.Close();
                    break;
                case Keys.Control | Keys.PageUp:
                    upButton_Click(this, new EventArgs());
                    break;
                case Keys.Control | Keys.PageDown:
                    downButton_Click(this, new EventArgs());
                    break;
                default:
                    break;
            }
        }

        private IEnumerable<int> selectedlistBoxIndicies()
        {
            List<int> indexes = new List<int>();

            for (int i = 0; i < listBox.Items.Count; i++)
                if (listBox.GetSelected(i))
                    indexes.Add(i);
            return indexes;
        }

        private async void AddFilesAsync(IEnumerable<string> paths)
        {
            foreach (var item in paths)
            {
                bool add = false;
                await Task.Run(() =>
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(item, Encoding.Default))
                        {
                            add = sr.ReadLine() == "<PlayList>";
                        }
                    }
                    catch (Exception)
                    {

                    }
                });
                if (add)
                {
                    Files.Add(item);
                }
            }
        }

        private void WriteStatus(string status, bool run_timer = true)
        {
            doneLabel.Visible = true;
            doneLabel.Text = status;
            if (run_timer)
            {
                Timer t = new Timer();
                t.Interval = 3000;
                t.Tick += (a, b) =>
                {
                    doneLabel.Visible = false;
                    ((Timer)a).Stop();
                };
                t.Start(); 
            }
        }
    }
}
