using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.IO;

namespace Calista.FireplaySupport
{
    [XmlRoot("PlayItem")]
    public class PlayItem : IDisposable
    {

        protected PlayItem()
        {

            
        }

        public PlayItem(PlayItem item)
        {
            _ID = item._ID;
            _Naziv = item._Naziv;
            _Autor = item._Autor;
            _Album = item._Album;
            _Info = item._Info;
            _Tip = item._Tip;
            _Color = item._Color;
            _NaKanalu = item._NaKanalu;
            PathName = item.PathName;
            _ItemType = item._ItemType;
            _StartCue = item._StartCue;
            _EndCue = item._EndCue;
            _Pocetak = item._Pocetak;
            _Trajanje = item._Trajanje;
            _Vrijeme = item._Vrijeme;
            _StvarnoVrijemePocetka = item._StvarnoVrijemePocetka;
            _VrijemeMinTermin = item._VrijemeMinTermin;
            _VrijemeMaxTermin = item._VrijemeMaxTermin;
            _PrviU_Bloku = item._PrviU_Bloku;
            _ZadnjiU_Bloku = item._ZadnjiU_Bloku;
            _JediniU_Bloku = item._JediniU_Bloku;
            _FiksniU_Terminu = item._FiksniU_Terminu;
            _Reklama = item._Reklama;
            _WaveIn = item._WaveIn;
            _SoftIn = item._SoftIn;
            _SoftOut = item._SoftOut;
            _Volume = item._Volume;
            _OriginalStartCue = item._OriginalStartCue;
            _OriginalEndCue = item._OriginalEndCue;
            _OriginalPocetak = item._OriginalPocetak;
            _OriginalTrajanje = item._OriginalTrajanje;

        }

        public PlayItem(string path)
        {
            if (System.IO.Path.GetExtension(path) == ".mp3" || System.IO.Path.GetExtension(path) == ".wav")
            {
                Path = path;
                _ID = "-2";
                _Naziv = System.IO.Path.GetFileNameWithoutExtension(path);
                _Autor = "";
                _Album = "";
                _Info = "";
                _Tip = "10";
                _Color = "0x0000000a";
                _NaKanalu = "0";
                PathName = path.ToLower();//new System.IO.DirectoryInfo(path).FullName.ToLower() + "\\" + System.IO.Path.GetFileName(path);
                _ItemType = "3";
                _StartCue = "0";
                _EndCue = "120";
                _Pocetak = "0";
                _Trajanje = _EndCue;
                _Vrijeme = DateTime.MinValue.ToString("o");
                _StvarnoVrijemePocetka = DateTime.MinValue.ToString("o");
                _VrijemeMinTermin = DateTime.MinValue.ToString("o");
                _VrijemeMaxTermin = DateTime.MinValue.ToString("o");
                _PrviU_Bloku = "0";
                _ZadnjiU_Bloku = "0";
                _JediniU_Bloku = "0";
                _FiksniU_Terminu = "0";
                _Reklama = "false";
                _WaveIn = "false";
                _SoftIn = "0";
                _SoftOut = "0";
                _Volume = "65536";
                _OriginalStartCue = _StartCue;
                _OriginalEndCue = _EndCue;
                _OriginalPocetak = _Pocetak;
                _OriginalTrajanje = _Trajanje;
                
            }

        }

        ////////////////////////////////////////////////////////////////////////
        ///////////////////////////   STATIC VARS    ///////////////////////////
        ////////////////////////////////////////////////////////////////////////

        //private static List<PlayItem> _all_songs = new List<PlayItem>();
        //public static List<PlayItem> AllSongs { get { return _all_songs; } }

        /////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////   SONG VARS   ///////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////

        public string Path { get; set; }

        protected string _ID = String.Empty;
        protected string _Naziv = String.Empty;
        protected string _Autor = String.Empty;
        protected string _Album = String.Empty;
        protected string _Info = String.Empty;
        protected string _Tip = String.Empty;
        protected string _Color = String.Empty;
        protected string _NaKanalu = String.Empty;
        protected string _PathName = String.Empty;
        protected string _ItemType = String.Empty;
        protected string _StartCue
        {
            get => _Start.TotalSeconds.ToString().Replace(',', '.');
            set => _Start = value != null ? TimeSpan.FromSeconds(Convert.ToDouble(value.Replace('.', ','))) : TimeSpan.Zero;
        }
        protected string _EndCue
        {
            get => _Finish.TotalSeconds.ToString().Replace(',', '.');
            set => _Finish = value != null ? TimeSpan.FromSeconds(Convert.ToDouble(value.Replace('.', ','))) : TimeSpan.Zero;
        }
        protected string _Pocetak
        {
            get => _Begining.TotalSeconds.ToString().Replace(',', '.');
            set => _Begining = value != null ? TimeSpan.FromSeconds(Convert.ToDouble(value.Replace('.', ','))) : TimeSpan.Zero;
        }
        protected string _Trajanje
        {
            get => _Duration.TotalSeconds.ToString().Replace(',', '.');
            set => _Duration = value != null ? TimeSpan.FromSeconds(Convert.ToDouble(value.Replace('.', ','))) : TimeSpan.Zero;
        }
        protected string _Vrijeme
        {
            get => _Time.ToString("o");
            set => _Time = value != null ? DateTime.Parse(value, null, System.Globalization.DateTimeStyles.RoundtripKind) : DateTime.MinValue;
        }
        protected DateTime _Time;
        protected TimeSpan _Duration = TimeSpan.Zero;
        protected TimeSpan _Begining = TimeSpan.Zero;
        protected TimeSpan _Start = TimeSpan.Zero;
        protected TimeSpan _Finish = TimeSpan.Zero;
        protected string _StvarnoVrijemePocetka = String.Empty;
        protected string _VrijemeMinTermin = String.Empty;
        protected string _VrijemeMaxTermin = String.Empty;
        protected string _PrviU_Bloku = String.Empty;
        protected string _ZadnjiU_Bloku = String.Empty;
        protected string _JediniU_Bloku = String.Empty;
        protected string _FiksniU_Terminu = String.Empty;
        protected string _Reklama = String.Empty;
        protected string _WaveIn = String.Empty;
        protected string _SoftIn
        {
            get => _fadeIn.Milliseconds.ToString();
            set => _fadeIn = value != null ? TimeSpan.FromMilliseconds(Int32.Parse(value)) : TimeSpan.Zero;
        }
        protected string _SoftOut
        {
            get => _fadeOut.Milliseconds.ToString();
            set => _fadeOut = value != null ? TimeSpan.FromMilliseconds(Int32.Parse(value)) : TimeSpan.Zero;
        }
        protected TimeSpan _fadeIn = TimeSpan.Zero;
        protected TimeSpan _fadeOut = TimeSpan.Zero;
        protected string _Volume = String.Empty;
        protected string _OriginalStartCue = String.Empty;
        protected string _OriginalEndCue = String.Empty;
        protected string _OriginalPocetak = String.Empty;
        protected string _OriginalTrajanje = String.Empty;


        [XmlElement]
        public string ID { get { return _ID; } set { _ID = value; OnSongChange(); } }
        [XmlElement]
        public string Naziv { get { return _Naziv; } set { _Naziv = value; OnSongChange(); } }
        [XmlElement]
        public string Autor { get { return _Autor; } set { _Autor = value; OnSongChange(); } }
        [XmlElement]
        public string Album { get { return _Album; } set { _Album = value; OnSongChange(); } }
        [XmlElement]
        public string Info { get { return _Info; } set { _Info = value; OnSongChange(); } }
        [XmlElement]
        public string Tip { get { return _Tip; } set { _Tip = value; OnSongChange(); } }
        [XmlElement]
        public string Color { get { return _Color; } set { _Color = value; OnSongChange(); } }
        [XmlElement]
        public string NaKanalu { get { return _NaKanalu; } set { _NaKanalu = value; OnSongChange(); } }
        [XmlElement]
        public string PathName { get { return _PathName; } set { _PathName = value; /*play.ReloadAudio(value);*/ OnSongChange(); } }
        [XmlElement]
        public string ItemType { get { return _ItemType; } set { _ItemType = value; OnSongChange(); } }
        [XmlElement]
        public string StartCue { get { return _StartCue; } set { _StartCue = value; OnSongChange(); } }
        [XmlElement]
        public string EndCue { get { return _EndCue; } set { _EndCue = value; OnSongChange(); } }
        [XmlElement]
        public string Pocetak { get { return _Pocetak; } set { _Pocetak = value; OnSongChange(); } }
        [XmlElement]
        public string Trajanje { get { return _Trajanje; } set { _Trajanje = value; OnSongChange(); } }////////////////////////////////////////////////////
        [XmlElement]
        public string Vrijeme { get { return _Vrijeme; } set { _Vrijeme = value; OnSongChange(); } }/////////////////////////////////////////
        [XmlIgnore]
        public DateTime Time { get { return _Time; } set { _Time = value; OnSongChange(); } }

        /// <summary>
        /// <see cref="TimeSpan"/> form of <see cref="Trajanje"/>
        /// </summary>
        [XmlIgnore]
        public TimeSpan Duration { get { return _Duration; } set { _Duration = value; OnSongChange(); } }

        /// <summary>
        /// <see cref="TimeSpan"/> form of <see cref="Pocetak"/>
        /// </summary>
        [XmlIgnore]
        public TimeSpan Begining { get { return _Begining; } set { _Begining = value; OnSongChange(); } }

        /// <summary>
        /// <see cref="TimeSpan"/> form of <see cref="StartCue"/>
        /// </summary>
        [XmlIgnore]
        public TimeSpan Start { get { return _Start; } set { _Start = value; OnSongChange(); } }

        /// <summary>
        /// <see cref="TimeSpan"/> form of <see cref="EndCue"/>
        /// </summary>
        [XmlIgnore]
        public TimeSpan Finish { get { return _Finish; } set { _Finish = value; OnSongChange(); } }
        [XmlElement]
        public string StvarnoVrijemePocetka { get { return _StvarnoVrijemePocetka; } set { _StvarnoVrijemePocetka = value; OnSongChange(); } }
        [XmlElement]
        public string VrijemeMinTermin { get { return _VrijemeMinTermin; } set { _VrijemeMinTermin = value; OnSongChange(); } }
        [XmlElement]
        public string VrijemeMaxTermin { get { return _VrijemeMaxTermin; } set { _VrijemeMaxTermin = value; OnSongChange(); } }
        [XmlElement]
        public string PrviU_Bloku { get { return _PrviU_Bloku; } set { _PrviU_Bloku = value; OnSongChange(); } }
        [XmlElement]
        public string ZadnjiU_Bloku { get { return _ZadnjiU_Bloku; } set { _ZadnjiU_Bloku = value; OnSongChange(); } }
        [XmlElement]
        public string JediniU_Bloku { get { return _JediniU_Bloku; } set { _JediniU_Bloku = value; OnSongChange(); } }
        [XmlElement]
        public string FiksniU_Terminu { get { return _FiksniU_Terminu; } set { _FiksniU_Terminu = value; OnSongChange(); } }
        [XmlElement]
        public string Reklama { get { return _Reklama; } set { _Reklama = value; OnSongChange(); } }
        [XmlElement]
        public bool ReklamaBool { get { return (_Reklama == "true") ? true : false; } }
        [XmlElement]
        public string WaveIn { get { return _WaveIn; } set { _WaveIn = value; OnSongChange(); } }
        [XmlElement]
        public bool WaveInBool { get { return (_WaveIn == "true") ? true : false; } }
        [XmlElement]
        public string SoftIn { get { return _SoftIn; } set { _SoftIn = value; OnSongChange(); } }
        [XmlElement]
        public string SoftOut { get { return _SoftOut; } set { _SoftOut = value; OnSongChange(); } }
        [XmlElement]
        public string Volume { get { return _Volume; } set { _Volume = value; OnSongChange(); } }
        [XmlIgnore]
        public TimeSpan FadeIn { get => _fadeIn; set => _fadeIn = value; }
        [XmlIgnore]
        public TimeSpan FadeOut { get => _fadeOut; set => _fadeOut = value; }
        [XmlElement]
        public string OriginalStartCue { get { return _OriginalStartCue; } set { _OriginalStartCue = value; OnSongChange(); } }
        [XmlElement]
        public string OriginalEndCue { get { return _OriginalEndCue; } set { _OriginalEndCue = value; OnSongChange(); } }
        [XmlElement]
        public string OriginalPocetak { get { return _OriginalPocetak; } set { _OriginalPocetak = value; OnSongChange(); } }
        [XmlElement]
        public string OriginalTrajanje { get { return _OriginalTrajanje; } set { _OriginalTrajanje = value; OnSongChange(); } }
        [XmlIgnore]
        

        public int _index = -1;
        [XmlIgnore]
        public int Index
        {
            get { return _index; }
            set
            {
                _index = value;
            }
        }


        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////   EVENTS   ////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////



        public delegate void SongChangedEvent(PlayItem m, EventArgs e);
        public event SongChangedEvent SongChanged;

        protected void OnSongChange()
        {
            SongChanged?.Invoke(this, new EventArgs());
        }



        /////////////////////////////////////////////////////////////////////////
        //////////////////////////////   METHODS   //////////////////////////////
        /////////////////////////////////////////////////////////////////////////



        

        public PlayItem GetClone()
        {
            PlayItem temp = PlayItem.CreateTemp();

            temp.ID = this.ID;
            temp.Naziv = this.Naziv;
            temp.Autor = this.Autor;
            temp.Album = this.Album;
            temp.Info = this.Info;
            temp.Tip = this.Tip;
            temp.Color = this.Color;
            temp.NaKanalu = this.NaKanalu;
            temp.PathName = this.PathName;
            temp.ItemType = this.ItemType;
            temp._Start = this.Start;
            temp._Finish = this.Finish;
            temp._Begining = this.Begining;
            temp._Duration = this.Duration;
            temp._Time = this.Time;
            temp.StvarnoVrijemePocetka = this.StvarnoVrijemePocetka;
            temp.VrijemeMinTermin = this.VrijemeMinTermin;
            temp.VrijemeMaxTermin = this.VrijemeMaxTermin;
            temp.PrviU_Bloku = this.PrviU_Bloku;
            temp.ZadnjiU_Bloku = this.ZadnjiU_Bloku;
            temp.JediniU_Bloku = this.JediniU_Bloku;
            temp.FiksniU_Terminu = this.FiksniU_Terminu;
            temp.Reklama = this.Reklama;
            temp.WaveIn = this.WaveIn;
            temp.SoftIn = this.SoftIn;
            temp.SoftOut = this.SoftOut;
            temp.Volume = this.Volume;
            temp.OriginalStartCue = this.OriginalStartCue;
            temp.OriginalEndCue = this.OriginalEndCue;
            temp.OriginalPocetak = this.OriginalPocetak;
            temp.OriginalTrajanje = this._OriginalTrajanje;

            return temp;
        }

        /// <summary>
        /// Defaults the song (works fine)
        /// </summary>
        public void DefaultTheSong()
        {
            this._SoftIn = "0";
            this._SoftOut = "0";
            this._StartCue = this._OriginalStartCue;
            this._EndCue = this._OriginalEndCue;
            this._Pocetak = this._OriginalPocetak;
            this._Trajanje = this._OriginalTrajanje;

        
        }

        /// <summary>
        /// Convert the object to xml string byte array.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"/>
        public byte[] GetXmlStringBytes()
        {
            XmlSerializer ser = new XmlSerializer(typeof(PlayItem));
            byte[] str;

            using (MemoryStream ms = new MemoryStream())
            using (StreamWriter sw = new StreamWriter(ms, Encoding.Default))
            {
                ser.Serialize(sw, this);
                str = ms.ToArray();
            }
            return str;
        }

        /// <summary>
        /// Convert to xml string.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"/>
        public string XmlString()
        {
            return Encoding.Default.GetString(GetXmlStringBytes());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.Collect();
        }

        public void Dispose(bool tr)
        {
            if (tr)
            {
                _ID = null;
                _Naziv = null;
                _Autor = null;
                _Album = null;
                _Info = null;
                _Tip = null;
                _Color = null;
                _NaKanalu = null;
                _PathName = null;
                _ItemType = null;
                _StvarnoVrijemePocetka = null;
                _VrijemeMinTermin = null;
                _VrijemeMaxTermin = null;
                _PrviU_Bloku = null;
                _ZadnjiU_Bloku = null;
                _JediniU_Bloku = null;
                _FiksniU_Terminu = null;
                _Reklama = null;
                _WaveIn = null;
                _SoftIn = null;
                _SoftOut = null;
                _Volume = null;
                _OriginalStartCue = null;
                _OriginalEndCue = null;
                _OriginalPocetak = null;
                _OriginalTrajanje = null;
                _StartCue = null;
                _EndCue = null;
                _Pocetak = null;
                _Trajanje = null;
                _Vrijeme = null;
                Path = null;
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////   STATIC METHODS   ////////////////////////////
        ////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Defaults the song (works fine)
        /// </summary>
        /// <param name="song"></param>
        public static void DefaultTheSong(PlayItem song)
        {
            song._SoftIn = "0";
            song._SoftOut = "0";
            song._StartCue = song._OriginalStartCue;
            song._EndCue = song._OriginalEndCue;
            song._Pocetak = song._OriginalPocetak;
            song._Trajanje = song._OriginalTrajanje;

        }

        public static PlayItem CreateSong()
        {
            PlayItem n = new PlayItem();
            //_all_songs.Add(n);
            return n;
        }

        public static PlayItem CreateTemp()
        {
            return new PlayItem();
        }


        public async static Task<PlayItem> CreatorSongFromXMLNodeAsync(XmlNode node)
        {
            PlayItem new_song = null;

            if (node.Name == "PlayItem" && node.HasChildNodes)
            {
                await Task.Run(() =>
                {
                    new_song = PlayItem.CreateFromNode(node);
                });
            }
            return new_song;
        }


        /*public static PlayItem CreatorFromNode(XmlNode node)
        {
            PlayItem new_song = PlayItem.CreateSong();

            if (node.Name == "PlayItem" && node.HasChildNodes)
            {
                foreach (XmlNode c in node.ChildNodes)
                {
                    if (c.Name == "ID") new_song.ID = c.InnerText;
                    else if (c.Name == "Naziv") new_song.Naziv = c.InnerText;
                    else if (c.Name == "Autor") new_song.Autor = c.InnerText;
                    else if (c.Name == "Album") new_song.Album = c.InnerText;
                    else if (c.Name == "Indo") new_song.Info = c.InnerText;
                    else if (c.Name == "Tip") new_song.Tip = c.InnerText;
                    else if (c.Name == "Color") new_song.Color = c.InnerText;
                    else if (c.Name == "NaKanalu") new_song.NaKanalu = c.InnerText;
                    else if (c.Name == "PathName") new_song.PathName = c.InnerText;
                    else if (c.Name == "ItemType") new_song.ItemType = c.InnerText;
                    else if (c.Name == "StartCue") new_song.StartCue = c.InnerText;
                    else if (c.Name == "EndCue") new_song.EndCue = c.InnerText;
                    else if (c.Name == "Pocetak") new_song.Pocetak = c.InnerText;
                    else if (c.Name == "Trajanje") new_song.Trajanje = c.InnerText;
                    else if (c.Name == "Vrijeme") new_song.Vrijeme = c.InnerText;
                    else if (c.Name == "StvarnoVrijemePocetka") new_song.StvarnoVrijemePocetka = c.InnerText;
                    else if (c.Name == "VrijemeMinTermin") new_song.VrijemeMinTermin = c.InnerText;
                    else if (c.Name == "VrijemeMaxTermin") new_song.VrijemeMaxTermin = c.InnerText;
                    else if (c.Name == "PrviU_Bloku") new_song.PrviU_Bloku = c.InnerText;
                    else if (c.Name == "ZadnjiU_Bloku") new_song.ZadnjiU_Bloku = c.InnerText;
                    else if (c.Name == "JediniU_Bloku") new_song.JediniU_Bloku = c.InnerText;
                    else if (c.Name == "FiksniU_Terminu") new_song.FiksniU_Terminu = c.InnerText;
                    else if (c.Name == "Reklama") new_song.Reklama = c.InnerText;
                    else if (c.Name == "WaveIn") new_song.WaveIn = c.InnerText;
                    else if (c.Name == "SoftIn") new_song.SoftIn = c.InnerText;
                    else if (c.Name == "SoftOut") new_song.SoftOut = c.InnerText;
                    else if (c.Name == "Volume") new_song.Volume = c.InnerText;
                    else if (c.Name == "OriginalStartCue") new_song.OriginalStartCue = c.InnerText;
                    else if (c.Name == "OriginalEndCue") new_song.OriginalEndCue = c.InnerText;
                    else if (c.Name == "OriginalPocetak") new_song.OriginalPocetak = c.InnerText;
                    else if (c.Name == "OriginalTrajanje") new_song.OriginalTrajanje = c.InnerText;
                }
            }


            new_song.SongXML();
            return new_song;
        }*/

        public static PlayItem CreateFromNode(XmlNode node)
        {
            PlayItem result;
            using (MemoryStream mstr = new MemoryStream(Encoding.Default.GetBytes(node.OuterXml)))
            {
                XmlSerializer ser = new XmlSerializer(typeof(PlayItem));
                result = (PlayItem)ser.Deserialize(mstr);
            }
                return result;
        }


        

        public static string[] Elements = { "ID", "Naziv", "Autor", "Album", "Info", "Tip", "Color", "NaKanalu", "PathName", "ItemType", "StartCue", "EndCue", "Pocetak", "Trajanje", "Vrijeme", "StvarnoVrijemePocetka", "VrijemeMinTermin", "VrijemeMaxTermin", "PrviU_Bloku", "ZadnjiU_Bloku", "JediniU_Bloku", "FiksniU_Terminu", "Reklama", "WaveIn", "SoftIn", "SoftOut", "Volume", "OriginalStartCue", "OriginalEndCue", "OriginalPocetak", "OriginalTrajanje" };

    }
}
