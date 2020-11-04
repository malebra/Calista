using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Calista.MainWindow
{
    [XmlRoot("Settings")]
    public class Settings
    {
        [XmlIgnore]
        private static readonly string settingsFolder = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\FireXer";
        [XmlIgnore]
        private static readonly string filePath = settingsFolder + "\\Settings.xml";

        [XmlElement("DefaultFilesPath")]
        public string DefaultFilesPath { get; set; } = string.Empty;

        [XmlElement("DefaultSavePath")]
        public string DefaultSavePath { get; set; } = string.Empty;

        [XmlElement("Colors")]
        public Colors Colors { get; set; } = Colors.Default;


        public void Save()
        {
            XmlSerializer xs = new XmlSerializer(typeof(Settings));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            using (StreamWriter sw = new StreamWriter(File.Open(filePath, FileMode.OpenOrCreate), Encoding.UTF8))
            using (XmlWriter xw = XmlWriter.Create(sw, new XmlWriterSettings() { Indent = true, OmitXmlDeclaration = true }))
            {
                try
                {
                    xs.Serialize(xw, this, ns);
                }
                catch (Exception)
                {
                    //add axception to thoew to know what happened
                }
            }


        }

        public static Settings Load()
        {
            Settings settings = null;
            XmlSerializer xs = new XmlSerializer(typeof(Settings));
            try
            {
                using (StreamReader sr = new StreamReader(File.Open(filePath, FileMode.Open), Encoding.UTF8))
                {
                    var str = sr.ReadToEnd();
                }
                using (StreamReader sr = new StreamReader(File.Open(filePath, FileMode.Open), Encoding.UTF8))
                {
                    settings = (Settings)xs.Deserialize(sr);
                }
            }
            catch (FileNotFoundException)
            {
                settings = new Settings();
                settings.Save();
            }
            catch (DirectoryNotFoundException)
            {
                settings = new Settings();
                settings.Save();
            }
            return settings;
        }
    }
}
