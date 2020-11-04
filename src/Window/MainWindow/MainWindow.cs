using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Calista.FireplaySupport;
using System.Xml;
using static System.Windows.Forms.ListBox;
using System.Reflection;
using System.Threading;

namespace Calista.MainWindow
{
    public partial class MainWindow : Form
    {
        private CancellationTokenSource cts { get => _cts_holder.IsCancellationRequested ? (_cts_holder = new CancellationTokenSource()) : _cts_holder; }
        private CancellationTokenSource _cts_holder = new CancellationTokenSource();


        public MainWindow()
        {
            InitializeComponent();
            SettingsContainer.Load();
            Setup();
            SetColor();
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            List<int> selected = new List<int>();
            for (int i = 0; i < listBox.Items.Count; i++)
            {
                if (listBox.GetSelected(i))
                {
                    selected.Add(i);
                }
            }

            
            foreach (var index in selected)
            {
                if (index == 0)
                {
                    return;
                }
                var el = Files.ElementAt(index - 1);
                Files.RemoveAt(index - 1);
                Files.Insert(index, el);
            }

            listBox.ClearSelected();
            foreach (var index in selected)
            {
                listBox.SetSelected(index - 1, true);
            }
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            var selected = selectedlistBoxIndicies().Reverse<int>();
            foreach (var index in selected)
            {
                if (index == listBox.Items.Count-1)
                {
                    return;
                }
                Files.Insert(index+2, Files.ElementAt(index));
                Files.RemoveAt(index);
            }

            listBox.ClearSelected();
            new List<int>(selected.Select(s => s + 1)).ForEach(f => listBox.SetSelected(f, true));
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = "FirePlay files (*.fps.xml)|*.fps.xml|Text (*.txt)|*.txt",
                Title = "Save",
                DefaultExt = ".fps.xml",
                AddExtension = true,
                RestoreDirectory = true,
                CheckFileExists = false,
                CheckPathExists = false,
                InitialDirectory = SettingsContainer.Settings.DefaultSavePath
            };

            sfd.ShowDialog();

            saveTextBox.Text = sfd.FileName;
            
            
        }

        private async void startButton_Click(object sender, EventArgs e)
        {
            PlayList list = null;
            bool procede = true;
            progressBar.Visible = true;
            string path = saveTextBox.Text;
            string[] files = Files.ToArray();

            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                MessageBox.Show($"The folder {Path.GetDirectoryName(path)} does not exist!", "Missing Folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            cancelButton.Enabled = true;

            if (File.Exists(path))
            {
                var Result = MessageBox.Show("The file you are trying to save exists!\nDo you want to owerwrite it?", "Warning: file aleary exists!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (Result != DialogResult.Yes)
                {
                    procede = false;
                }

            }


            if (procede)
            {
                Progress<int> prog = new Progress<int>();
                prog.ProgressChanged += (a, b) => progressBar.Value = b;

                try
                {
                    list = (await PlayList.DeserializeParallelAsync(files, cts.Token, prog));
                    using (FileStream fs = File.Open(path, FileMode.OpenOrCreate))
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
                    {
                        progressBar.Visible = false;
                        WriteStatus("Creating file...", false);
                        //_cts_holder.Token.ThrowIfCancellationRequested();
                        await Task.Run(() => list.Serialize(sw));

                    }
                    WriteStatus("Done.");
                }
                catch (OperationCanceledException)
                {
                    progressBar.Visible = false;
                    this.WriteStatus("Cancled.");
                }
                catch (ArgumentException ee)
                {
                    if (ee.Message == "The path is not of a legal form." && saveTextBox.Text != string.Empty)
                    {
                        MessageBox.Show("The path of the save file is incorrect.", "Wrong path", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.Message}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.WriteStatus("Error!");
                }
                finally
                {
                    list?.Items?.ForEach(i => i.Dispose(true));
                    list?.Dispose();
                    list = null;
                    cancelButton.Enabled = false;
                    progressBar.Visible = false;
                    progressBar.Value = 0;
                    GC.Collect();
                }
            }
        }

        private void cancleButton_Click(object sender, EventArgs e)
        {
            cts?.Cancel();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            Files.Clear();
        }

        private void filesButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "FirePlay Files (*.fps.xml)|*.fps.xml|Xml (*.xml)|*.xml",
                CheckFileExists = true,
                Multiselect = true,
                ValidateNames = true,
                Title = "Select files",
                RestoreDirectory = true,
                InitialDirectory = SettingsContainer.Settings.DefaultFilesPath
            };

            ofd.ShowDialog();

            AddFilesAsync(ofd.FileNames);
            
        }

        private void SetColor()
        {
            var colors = SettingsContainer.Settings.Colors;
            this.BackColor = colors.Base;

            foreach (var item in this.Controls.Cast<Control>())
            {
                if (!item.GetType().IsAssignableFrom(typeof(Label)) && !item.GetType().IsAssignableFrom(typeof(ProgressBar)))
                {
                    ((Control)item).BackColor = colors.Side1;
                }
                if (item.GetType().IsAssignableFrom(typeof(Button)))
                {
                    ((Button)item).FlatStyle = FlatStyle.Flat;
                    if (!((Button)item).Enabled)
                    {
                        ((Button)item).BackColor = colors.Side2;
                    }

                }
                if (item.GetType().IsAssignableFrom(typeof(ListBox)))
                {
                    ((ListBox)item).BorderStyle = BorderStyle.FixedSingle;
                }
                if (item.GetType().IsAssignableFrom(typeof(TextBox)))
                {
                    ((TextBox)item).BorderStyle = BorderStyle.FixedSingle;
                    ((TextBox)item).ForeColor = Color.Black;
                }
            }
        }
    }
}
