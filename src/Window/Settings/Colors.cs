using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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

        [XmlElement("Color")]
        public HSLColor Base { get; set; }
        [XmlIgnore]
        public HSLColor Side1 { get { var tmp = new HSLColor(Base); tmp.Luminosity *= 1.8f; return tmp; }  } 
        [XmlIgnore]
        public HSLColor Side2 { get { var tmp = new HSLColor(Base); tmp.Hue *= 1f; tmp.Saturation *= 0.45f; tmp.Luminosity *= 1.25f; return tmp; } }


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
