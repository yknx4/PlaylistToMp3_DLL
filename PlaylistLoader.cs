using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TagLib;



namespace PlaylistToMp3_DLL
{
    /// <summary>
    /// Class for loading the .m3u8 playlist.
    /// </summary>
public static class PlaylistLoader
    {
        private static string _error;
        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public static String Error { get { return _error != null ? _error : ""; } }
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
        public static List< TagLib.File> Load(string path)
        {
            var result = new List< TagLib.File>();
            try
            {
                foreach (string entry in System.IO.File.ReadAllLines(path))
                {
                    string entry_f = entry;
                    if (entry[1] != ':')
                    {
                        entry_f = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) , entry_f);
                        
                    }
                    FileInfo m_Entry = new FileInfo(Path.GetFullPath(entry_f));
                    TagLib.File file = null;
                    if (m_Entry.Exists)
                    {
                        try
                        {
                            file = TagLib.File.Create(m_Entry.FullName);
                        }
                        catch (TagLib.UnsupportedFormatException)
                        {
                            Console.WriteLine("UNSUPPORTED FILE: " + m_Entry.FullName);
                            Console.WriteLine(String.Empty);
                            Console.WriteLine("---------------------------------------");
                            Console.WriteLine(String.Empty);
                            continue;
                        }
                        result.Add(file);
                    }
                    else
                    {
                        throw new ArgumentException(m_Entry.FullName);
                    }


                }
            }

            catch (SystemException ex)
            {
                _error = ex.ToString();
                //throw ex;
            }

            return result;
        }
    }
}
