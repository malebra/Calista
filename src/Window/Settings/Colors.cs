using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Calista.FireplaySupport;

namespace Calista.MainWindow
{
    

    [XmlRoot("Colors")]
    public class Colors
    {

        [XmlElement("Color", IsNullable = true)]
        public HSLColor Base { get => _base; set => _base = value; }
        private HSLColor _base = null;
        [XmlElement("SideColor1", IsNullable = true)]
        public HSLColor Side1 { get { if (_side1 != null) return _side1; if (Base == null) return null; var tmp = new HSLColor(Base); tmp.Luminosity *= 1.8f; return tmp; } set { _side1 = value; } }
        private HSLColor _side1 = null;
        [XmlElement("SideColor2", IsNullable = true)]
        public HSLColor Side2 { get { if (_side2 != null) return _side2; if (Base == null) return null; var tmp = new HSLColor(Base); tmp.Hue *= 1f; tmp.Saturation *= 0.45f; tmp.Luminosity *= 1.25f; return tmp; } set { _side2 = value; } }
        private HSLColor _side2 = null;


        public static Colors Default => new Colors()
        {
            //Base = new HSLColor(233.907287597656d, 187.772026062012d, 90.8235311508179d),
            Base = new HSLColor(176, 0, 4)
        };

        public static Colors GetColors()
        {
            var path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\FireXer";
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path, "*.fxcol", SearchOption.AllDirectories);

                XmlSerializer ser = new XmlSerializer(typeof(Colors));

                try
                {
                    using (StreamReader sr = new StreamReader(File.Open($"{path}\\ColorScheme.fxcol", FileMode.Open)))
                    {
                        return (Colors)ser.Deserialize(sr);
                    }
                }
                catch (Exception)
                {

                }
            }
            return Colors.Default;
        }

        public static bool SaveColors(Colors colors)
        {
            var path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\FireXer";
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path, "*.fxcol", SearchOption.AllDirectories);

                XmlSerializer ser = new XmlSerializer(typeof(Colors));

                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");


                try
                {
                    using (StreamWriter sw = new StreamWriter(File.Open($"{path}\\ColorScheme.fxcol", FileMode.OpenOrCreate)))
                    using (XmlWriter xw = XmlWriter.Create(sw, new XmlWriterSettings() { Indent = true, OmitXmlDeclaration = true }))
                    {
                        ser.Serialize(xw, colors, ns);
                    }
                    return true;

                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }
    }

}
