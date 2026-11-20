using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace AcousticFileReaderDemo
{
    public class MediaPlayer
    {
        public const int MM_MCINOTIFY = 0x3B9;

        private string FileName { get; set; }
        private bool IsOpen { get; set; }
        private Form NotifyForm { get; set; }
        private string MediaName { get; set; }
        public bool IsPaused { get; private set; }
        public bool IsPlaying { get; private set; }

        public MediaPlayer()
        {
            FileName = string.Empty;
            IsOpen = false;
            NotifyForm = null;
            MediaName = "media";
            IsPaused = false;
            IsPlaying = false;
        }

        public void Play(string fileName, Form notifyForm)
        {
            if (!IsPaused)
            {
                FileName = fileName;
                NotifyForm = notifyForm;
                OpenMediaFile();
                PlayMediaFile();
            }
            else
            {
                IsPaused = false;
                PlayMediaFile();
            }
        }

        public void Stop()
        {
            if (IsOpen)
            {
                string stopCommand = "Stop " + MediaName;
                mciSendString(stopCommand, null, 0, IntPtr.Zero);
                IsPaused = false;
                IsPlaying = false;
            }
        }

        public void Pause()
        {
            if (IsOpen)
            {
                string pauseCommand = "Pause " + MediaName;
                mciSendString(pauseCommand, null, 0, IntPtr.Zero);
                IsPaused = true;
                IsPlaying = false;
            }
        }

        private void ClosePlayer()
        {
            if (IsOpen)
            {
                string playCommand = "Close " + MediaName;
                mciSendString(playCommand, null, 0, IntPtr.Zero);
                IsOpen = false;
            }
        }

        private void OpenMediaFile()
        {
            ClosePlayer();
            string playCommand = "Open \"" + FileName + "\" type mpegvideo alias " + MediaName;
            mciSendString(playCommand, null, 0, IntPtr.Zero);
            IsOpen = true;
        }


        private void PlayMediaFile()
        {
            if (IsOpen)
            {
                string playCommand = "Play " + MediaName + " notify";
                mciSendString(playCommand, null, 0, NotifyForm.Handle);
                IsPlaying = true;
            }
        }

        //mciSendString 
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string command, StringBuilder returnValue, int returnLength, IntPtr winHandle);
    }
}
