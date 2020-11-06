using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Calista.FireplaySupport
{
    public partial class PlayList
    {
        public static bool stopDeserialization = false;

        static readonly XmlSerializer xmlSerializer = new XmlSerializer(typeof(PlayList));
        static readonly XmlSerializerNamespaces xmlsns = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });
        

        #region ### Deserialization

        #region Synchronous 

        /// <summary>
        /// Deserialize a XML file containing the playlist data.
        /// <para>Returns <see cref="null"/> if the file isn't valid.</para>
        /// </summary>
        /// <param name="path">The path of the playlist file.</param>
        /// <param name="ignoreError">If set to true stops exceptions and returns null on error.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="OperationCanceledException"/>
        public static PlayList Deserialize(string path, bool ignoreError = false)
        {
            PlayList result = null;
            try
            {
                using (StreamReader sr = new StreamReader(path, Encoding.Default))
                using (XmlReader xr = XmlReader.Create(sr))
                {
                    if (stopDeserialization) throw new OperationCanceledException();

                    if (!xmlSerializer.CanDeserialize(xr))
                    {
                        throw new InvalidOperationException("The file does not contain valid Playlist data!");
                    }
                    var popo = xmlSerializer.Deserialize(xr);
                    result = (PlayList)popo;
                    popo = null;


                    if (stopDeserialization) throw new OperationCanceledException();
                }
            }
            catch (OperationCanceledException ex)
            {
                stopDeserialization = false;
                throw ex;
            }
            catch (InvalidOperationException ex)
            {
                var mess = $"There was a problem parsing the XML date from the list at the location: {path}!\n{ex.Message}";
                Logger.Error(mess);
                if (!ignoreError) throw new InvalidOperationException(message: mess);
            }
            if (result != null)
            {
                Logger.Log($"Playlist created from: {path}.");
            }
            return result;
        }

        /// <summary>
        /// Deserialize multiple XML files containing playlist data in list order asynchronously.
        /// <para>Returns <see cref="null"/> if all file isn't valid.</para>
        /// </summary>
        /// <param name="paths">The paths of the playlist files.</param>
        /// <param name="ignoreError">If set to true stops exceptions and returns null on error.</param>
        /// <param name="progress">Reports the percentage of completed lists.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"/>
        public static PlayList Deserialize(string[] paths, IProgress<int> progress = null, bool ignoreError = false)
        {
            List<PlayList> pll = new List<PlayList>();
            int val = 0;
            foreach (var p in paths)
            {
                pll.Add(Deserialize(p, ignoreError));
                progress.Report((++val * 100) / paths.Count());
            }
            return new PlayList(pll);
        }

        #endregion

        #region Asynchronous

        /// <summary>
        /// Deserialize a XML file containing the playlist data <b>asynchronously</b>.
        /// <para>Returns <see cref="null"/> if the file isn't valid.</para>
        /// </summary>
        /// <param name="path">The path of the playlist file.</param>
        /// <param name="ignoreError">If set to true stops exceptions and returns null on error.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"/>
        public async static Task<PlayList> DeserializeAsync(string path, bool ignoreError = false)
        {
            Exception exc = null;
            var res = await Task.Run(() =>
            {
                try
                {
                    return Deserialize(path, ignoreError);
                }
                catch (Exception ex)
                {
                    exc = ex;
                    return null;
                }
            });

            if (exc != null)
            {
                throw exc;
            }
            return null;
        }

        /// <summary>
        /// Deserialize a XML file containing the playlist data <b>asynchronously</b>.
        /// <para>Returns <see cref="null"/> if the file isn't valid.</para>
        /// </summary>
        /// <param name="path">The path of the playlist file.</param>
        /// <param name="ignoreError">If set to true stops exceptions and returns null on error.</param>
        /// <param name="ct">Cancellation token. Can be used to cancle the operation.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"/>
        public async static Task<PlayList> DeserializeAsync(string path, CancellationToken ct, bool ignoreError = false)
        {
            Exception exc = null;
            var result = await Task.Run(() =>
            {
                try
                {
                    var l = Deserialize(path, ignoreError);
                    if (ct.IsCancellationRequested)
                    {
                        stopDeserialization = true;
                        return new PlayList();
                    }
                    return l;
                }
                catch (Exception ex)
                {
                    exc = ex;
                    return new PlayList();
                }
            }, ct);

            if (exc != null)
            {
                throw exc;
            }
            ct.ThrowIfCancellationRequested();
            return result;
        }

        /// <summary>
        /// Deserialize multiple XML files containing playlist data in list order <b>asynchronously</b>.
        /// <para>Returns <see cref="null"/> if all file isn't valid.</para>
        /// </summary>
        /// <param name="paths">The paths of the playlist files.</param>
        /// <param name="ignoreError">If set to true stops exceptions and returns null on error.</param>
        /// <param name="progress">Reports the percentage of completed lists.</param>
        /// <returns>A new <see cref="PlayList"/></returns>
        /// <exception cref="InvalidOperationException"/>
        public async static Task<PlayList> DeserializeAsync(string[] paths, IProgress<int> progress = null, bool ignoreError = false)
        {
            Exception exc = null;
            int val = 0;
            List<Task<PlayList>> tasks = new List<Task<PlayList>>();
            foreach (string path in paths)
            {
                Task<PlayList> t = Task.Run(() =>
                {
                    try
                    {
                        var res = Deserialize(path, ignoreError);
                        progress.Report((++val * 100) / paths.Count());
                        return res;
                    }
                    catch (Exception ex)
                    {
                        if (exc == null) exc = ex;
                        return new PlayList();
                    }
                });

                tasks.Add(t);
            }
            var results = new PlayList(await Task.WhenAll(tasks));
            if (exc != null) throw exc;
            return results;
        }

        /// <summary>
        /// Deserialize multiple XML files containing playlist data in list order <b>asynchronously</b>.
        /// <para>Returns <see cref="null"/> if all file isn't valid.</para>
        /// </summary>
        /// <param name="paths">The paths of the playlist files.</param>
        /// <param name="ignoreError">If set to true stops exceptions and returns null on error.</param>
        /// <param name="ct">Cancellation token. Can be used to cancle the operation.</param>
        /// <param name="progress">Reports the percentage of completed lists.</param>
        /// <returns>A new <see cref="PlayList"/>; if cancled, returns empty <see cref="PlayList"/></returns>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="OperationCanceledException"/>
        public async static Task<PlayList> DeserializeAsync(string[] paths, CancellationToken ct, IProgress<int> progress = null, bool ignoreError = false)
        {
            Exception exc = null;
            int val = 0;
            List<Task<PlayList>> tasks = new List<Task<PlayList>>();
            foreach (string path in paths)
            {
                Task<PlayList> t = Task.Run(() =>
                {
                    try
                    {
                        var res = Deserialize(path, ignoreError);

                        if (ct.IsCancellationRequested)
                        {
                            return new PlayList();
                        }

                        progress.Report((++val * 100) / paths.Count());
                        return res;
                    }
                    catch (Exception ex)
                    {
                        if (exc == null) exc = ex;
                        return new PlayList();
                    }
                }, ct);

                tasks.Add(t);

            }
            var results = await Task.WhenAll(tasks);
            ct.ThrowIfCancellationRequested();
            if (exc != null) throw exc;
            return new PlayList(results);
        }

        /// <summary>
        /// Deserialize multiple XML files containing playlist data in list order <b>asynchronously</b>.
        /// <para>Returns <see cref="null"/> if all file isn't valid.</para>
        /// </summary>
        /// <param name="paths">The paths of the playlist files.</param>
        /// <param name="ignoreError">If set to true stops exceptions and returns null on error.</param>
        /// <param name="progress">Reports the percentage of completed lists.</param>
        /// <returns>A new <see cref="PlayList"/></returns>
        /// <exception cref="InvalidOperationException"/>
        public async static Task<PlayList> DeserializeParallelAsync(string[] paths, IProgress<int> progress = null, bool ignoreError = false)
        {
            Exception exc = null;
            int val = 0;
            List<Task<PlayList>> tasks = new List<Task<PlayList>>();
            foreach (string path in paths)
            {
                Task<PlayList> t = new Task<PlayList>(() =>
                {
                    try
                    {
                        var res = Deserialize(path, ignoreError);
                        progress.Report((++val * 100) / paths.Count());
                        return res;
                    }
                    catch (Exception ex)
                    {
                        if (exc == null )exc = ex;
                        return new PlayList();
                    }
                });

                tasks.Add(t);
            }
            await Task.Run(() => Parallel.ForEach(tasks, (task) => task.Start()));

            Task.WaitAll(tasks.ToArray());

            var results = tasks.Select(t => t.Result);
            tasks.ForEach(t => t?.Dispose());
            if (exc != null) throw exc;
            return new PlayList(results);
        }

        /// <summary>
        /// Deserialize multiple XML files containing playlist data in list order <b>asynchronously</b>.
        /// <para>Returns <see cref="null"/> if all file isn't valid.</para>
        /// </summary>
        /// <param name="paths">The paths of the playlist files.</param>
        /// <param name="ignoreError">If set to true stops exceptions and returns null on error.</param>
        /// <param name="progress">Reports the percentage of completed lists.</param>
        /// <returns>A new <see cref="PlayList"/>; if cancled, returns empty <see cref="PlayList"/></returns>
        /// <exception cref="InvalidOperationException"/>
        public async static Task<PlayList> DeserializeParallelAsync(string[] paths, CancellationToken ct, IProgress<int> progress = null, bool ignoreError = false)
        {
            Exception exc = null;
            int val = 0;
            List<Task<PlayList>> tasks = new List<Task<PlayList>>();
            foreach (string path in paths)
            {
                Task<PlayList> t = new Task<PlayList>(() =>
                {
                    try
                    {
                        var res = Deserialize(path, ignoreError);

                        if (ct.IsCancellationRequested)
                        {
                            return new PlayList();
                        }

                        progress.Report((++val * 100) / paths.Count());
                        return res;
                    }
                    catch (Exception ex)
                    {
                        if (exc == null) exc = ex;
                        return new PlayList();
                    }
                });

                tasks.Add(t);
            }
            await Task.Run(() => Parallel.ForEach<Task<PlayList>>(tasks, (task) => task.Start()));
            Task.WaitAll(tasks.ToArray());
            ct.ThrowIfCancellationRequested();
            if (exc != null) throw exc;
            return new PlayList(tasks.Select(t => t.Result));
        }

        #endregion

        #endregion



        #region Serialize

        /// <summary>
        /// Serializes the <paramref name="list"/> <see cref="PlayList"/> to the <paramref name="sw"/> <see cref="StreamWriter"/>
        /// </summary>
        /// <param name="list">The list to synchronize.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"/>
        public static void Serialize(StreamWriter sw, PlayList list)
        {

            StreamWriter sww = new StreamWriter(sw.BaseStream, Encoding.Default);
            XmlWriter xw = XmlWriter.Create(sww, new XmlWriterSettings() { Indent = true, OmitXmlDeclaration = true, NewLineOnAttributes = true });
            try
            {
                xmlSerializer.Serialize(xw, list, xmlsns);
            }
            catch (Exception e)
            {
                xw.Close();
                sw.Close();
                throw e;
            }
        }


        #endregion


    }
}
