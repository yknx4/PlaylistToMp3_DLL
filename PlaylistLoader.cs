using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;




namespace PlaylistToMp3_DLL
{
    /// <summary>
    /// Class for loading the .m3u8 playlist.
    /// </summary>
    /// 

    public static class PlaylistLoader
    {
        /// <summary>
        /// Event Handler of Error Thrown
        /// </summary>
        /// <param name="e">The <see cref="PlaylistLoader.ErrorThrownEventArgs"/> instance containing the event data.</param>
        public delegate void ErrorThrownEventHandler(PlaylistLoader.ErrorThrownEventArgs e);
        /// <summary>
        /// Occurs when [error thrown].
        /// </summary>
        public static event ErrorThrownEventHandler ErrorThrown;
        /// <summary>
        /// Error Thrown event args
        /// </summary>
        public class ErrorThrownEventArgs : EventArgs
        {
            public String Error { get; set; }
        }
        static void OnErrorThrown(ErrorThrownEventArgs e)
        {
            if (ErrorThrown != null)
                ErrorThrown(e);
        }
        public delegate void LogEventHandler(PlaylistLoader.LogEventArgs e);
        /// <summary>
        /// Occurs when [log].
        /// </summary>
        public static event LogEventHandler Log;
        public class LogEventArgs : EventArgs
        {
            public String Message { get; set; }
        }
        static void OnLog(LogEventArgs e)
        {
            if (Log != null)
                Log(e);
        }

        /// <summary>
        /// Loads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// A list with all music files within the list.
        /// </returns>
        /// 
        public static List<MusicFile> GetPlaylist(string path)
        {
            var result = new List<MusicFile>();
            foreach (var entry in Load(path))
            {
                result.Add(new MusicFile(entry));
            }
            return result;
        }

        /// <summary>
        /// Loads the specified playlist.
        /// </summary>
        /// <param name="path">The playlist path.</param>
        /// <returns></returns>
        public static List<TagLib.File> Load(string path)
        {

            var result = new List<TagLib.File>();
            try
            {
                FileInfo pathInfo = new FileInfo(path);
                foreach (string entry in System.IO.File.ReadAllLines(path))
                {
                    if (entry.Length == 0 || entry[0] == '#') continue;
                    string entry_f = entry;
                    if (entry[1] != ':')
                    {
                        entry_f = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), entry_f);

                    }
                    try
                    {
                        FileInfo m_Entry = new FileInfo(Path.GetFullPath(entry_f));
                        TagLib.File file = null;
                        if (m_Entry.Exists)
                        {
                            file = loadFileTags(ref m_Entry);


                        }
                        else
                        {
                            string altPath = Path.Combine(pathInfo.Directory.FullName, entry);
                            m_Entry = new FileInfo(Path.GetFullPath(altPath));
                            if (m_Entry.Exists)
                                file = loadFileTags(ref m_Entry);
                            else
                            {
                                OnLog(new LogEventArgs { Message = m_Entry.FullName + " doesn't exist." });
                                continue;
                            }
                            /*throw new ArgumentException(m_Entry.FullName);*/
                        }
                        if (file != null)
                            result.Add(file);
                    }
                    catch (Exception e)
                    {
                        OnLog(new LogEventArgs { Message = entry_f + " cannot be loaded." });
                        OnErrorThrown(new ErrorThrownEventArgs { Error = e.ToString() });
                    }



                }
            }

            catch (Exception ex)
            {
                OnErrorThrown(new ErrorThrownEventArgs { Error = ex.ToString() });

                //throw ex;
            }

            return result;
        }

        private static TagLib.File loadFileTags(ref FileInfo m_Entry)
        {
            TagLib.File res = null;
            try
            {
                res = TagLib.File.Create(m_Entry.FullName);
            }
            catch (TagLib.UnsupportedFormatException)
            {
                res = null;
                Console.WriteLine("UNSUPPORTED FILE: " + m_Entry.FullName);
                Console.WriteLine(String.Empty);
                Console.WriteLine("---------------------------------------");
                Console.WriteLine(String.Empty);
                OnLog(new LogEventArgs { Message = "UNSUPPORTED FILE: " + m_Entry.FullName });
            } return res;
        }
    }
}
