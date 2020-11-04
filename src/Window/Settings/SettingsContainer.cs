using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Calista.MainWindow
{
    
    public static class SettingsContainer
    {

        public static Settings Settings { get; set; } 


        public static void Load()
        {
            if (!Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\FireXer"))
            {
                Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\FireXer");
                Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\FireXer\\Colors");
            }
            Settings = Settings.Load();
        }
    }
}
