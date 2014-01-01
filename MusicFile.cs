using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace PlaylistToMp3_DLL
{
    public delegate void ConvertedEventHandler(object sender, EventArgs e);
    public class MusicFile
    {
        private TagLib.File _file;

        public event ConvertedEventHandler Converted;
        private bool _isConverted;

        protected virtual void OnConverted(EventArgs e)
        {
            if (Converted != null)
            {
                Converted(this, e);
            }
        }

        public MusicFile(TagLib.File file)
        {
            _file = file;
        }
        [Browsable(false)]
        public FileInfo FileInformation
        {
            get
            {
                return new FileInfo(FileName);
            }
        }
        [Browsable(false)]
        public string FileName { get { return _file.Name; } }
        [DisplayName("File Name")]
        public string ShortFileName { get { return FileInformation.Name; } }
        public string Format { get { return (new FileInfo(_file.Name)).Extension; } }
        public string Artist
        {
            get
            {
                try
                {
                    return _file.Tag.Performers.First() != null ? _file.Tag.Performers.First().ToString() : "";
                }
                catch (Exception)
                {
                    return "No AArtist";
                }
            }
        }
        public string Album
        {
            get
            {
                try
                {
                    return validateString(_file.Tag.Album.ToString());
                }
                catch (Exception)
                {
                    return "No Album";
                }

            }
        }
        private string validateString(string input)
        {
            if (input != null)
            {
                return input;
            }
            return string.Empty;
        }
        public string Title
        {
            get
            {
                try { return validateString(_file.Tag.Title.ToString()); }
                catch (Exception) { return ShortFileName; }

            }
        }
        public TimeSpan Duration
        {
            get
            {
                if (_file.Properties.MediaTypes != TagLib.MediaTypes.None)
                    return _file.Properties.Duration;
                return new TimeSpan();
            }
        }
        public int Bitrate
        {
            get
            {
                foreach (TagLib.ICodec codec in _file.Properties.Codecs)
                {
                    TagLib.IAudioCodec acodec = codec as TagLib.IAudioCodec;
                    TagLib.IVideoCodec vcodec = codec as TagLib.IVideoCodec;
                    if (acodec != null && (acodec.MediaTypes & TagLib.MediaTypes.Audio) != TagLib.MediaTypes.None)
                    {
                        Console.WriteLine("Audio Properties : " + acodec.Description);
                        return acodec.AudioBitrate;
                    }
                }
                return 0;
            }
        }

        public int Progress
        {
            get;
            set;
        }
        [DisplayName("Converted")]
        public bool isConverted
        {
            get
            {
                return _isConverted;
            }
            set
            {
                OnConverted(EventArgs.Empty);
                _isConverted = value;
            }
        }
        public void resetEvents()
        {
            Converted = null;
        }

    }
}