using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using System.Threading;

namespace Calista.FireplaySupport
{
    [XmlRoot("PlayList")]
    public partial class PlayList : IDisposable
    {

        #region ### Constructor & Destructor ###

        protected PlayList()
        {

        }

        public PlayList(List<PlayItem> items) : this()
        {
            _items = items;
        }

        public PlayList(PlayItem[] items) : this(items?.ToList())
        {

        }

        public PlayList(IEnumerable<PlayList> lists) : this()
        {
            _items = lists?.SelectMany(s => s?.Items).ToList();
            lists.Select(l => { l.Dispose(true); return l; });
        }
        

        ~PlayList()
        {
            Dispose();
        }

        #endregion

        #region ### Properties ###

        [XmlIgnore]
        private string _path { get; set; }

        [XmlElement("PlayItem")]
        public List<PlayItem> Items
        {
            get => _items;
            set => _items = value;
        }
        [XmlIgnore]
        private List<PlayItem> _items = new List<PlayItem>();

        /// <summary>
        /// Returns the <see cref="DateTime"/> of the first item in the list or the <see cref="DateTime.MinValue"/> if empty.
        /// </summary>
        [XmlIgnore]
        public DateTime Start
        {
            get
            {
                return _items != null ? _items.FirstOrDefault().Time : DateTime.MinValue;
            }

            set
            {
                if (_items != null)
                {
                    _items.FirstOrDefault().Time = value;
                }
            }
        }

        /// <summary>
        /// Returns the <see cref="DateTime"/> of the last item in the list or the <see cref="DateTime.MinValue"/> if empty.
        /// </summary>
        [XmlIgnore]
        public DateTime End
        {
            get
            {
                return _items != null ? (DateTime)_items.LastOrDefault()?.Time.Add((TimeSpan)(_items.LastOrDefault()?.Duration)) : DateTime.MinValue;
            }
        }

        /// <summary>
        /// Returns the Length of the list
        /// </summary>
        [XmlIgnore]
        public TimeSpan Length { get => _items != null ? _items.Select(i => i.Duration).Aggregate((a, b) => a+b) : TimeSpan.Zero; }


        
        public delegate void ListChangeEventHandler(object sender, EventArgs e);
        public event ListChangeEventHandler ListChange;

        #endregion

        #region ### Methods ###

        /// <summary>
        /// Add song to the end of the list
        /// </summary>
        /// <param name="item"></param>
        public virtual void Push(PlayItem item)
        {
            if (_items == null)
                _items = new List<PlayItem>();

            _items.Add(item);
            OnEdit();
        }

        /// <summary>
        /// Return first item in the list, or <see cref="null"/> if empty.
        /// </summary>
        /// <returns></returns>
        public virtual PlayItem Pop()
        {
            PlayItem temp = null;
            if (_items != null && _items.Count > 0)
            {
                temp = _items[0];
                _items.RemoveAt(0);
            }
            return temp;
        }

        /// <summary>
        /// Insert in at the specific index in the list, or add first if empty
        /// </summary>
        /// <param name="index"></param>
        public virtual void Insert(int index, PlayItem item)
        {
            if (_items != null && _items.Count > index)
            {
                Items.Insert(index, item);
                OnEdit();
            }
            else
            {
                Push(item);
            }
        }

        /// <summary>
        /// Removes the element at the specific index.
        /// </summary>
        /// <param name="index"></param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public virtual void RemoveAt(int index)
        {
            _items.RemoveAt(index);
            OnEdit();
        }

        /// <summary>
        /// Removes the first occurance of a specific object from the <see cref="Items"/> list.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>True if the item was successfully removed and false if either the item isn't present, the list is empty or other errors.</returns>
        public virtual bool Remove(PlayItem item)
        {
            var result = _items == null ? false : _items.Remove(item);
            if (result)
            {
                OnEdit();
            }
            return result;
        }


        /// <summary>
        /// Serializes the playlist to the <paramref name="sw"/> <see cref="StreamWriter"/>
        /// </summary>
        /// <paramref name="sw">The <see cref="StreamWriter"/> used to serialize the list.</paramref>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"/>
        public void Serialize(StreamWriter sw)
        {
            StreamWriter sww = new StreamWriter(sw.BaseStream, Encoding.Default);
            XmlWriter xw = XmlWriter.Create(sww, new XmlWriterSettings() { Indent = true, OmitXmlDeclaration = true, NewLineOnAttributes = true });
            try
            {
                xmlSerializer.Serialize(xw, this, xmlsns);
            }
            catch (Exception e)
            {
                xw.Close();
                sw.Close();
                throw e;
            }
        }


        /// <summary>
        /// Call when the list has been edited. Set event.
        /// </summary>
        protected virtual void OnEdit()
        {
            ListChange?.Invoke(this, new EventArgs());
        }

        #endregion

        #region ### Garbage Collecion ###

        public virtual void Dispose()
        {
            Dispose(true);
            GC.Collect();
        }

        private void Dispose(bool tr)
        {
            if (tr)
            {
                _items = null;
            }
        }

        #endregion





        #region ### Static Methods ###

        public static PlayList operator +(PlayList a, PlayList b)
        {
            return new PlayList(a?.Items?.Select(x => x?.GetClone())?.Concat(b?.Items?.Select(x => x?.GetClone()))?.ToList());
        }



        

        #endregion
    }
}
