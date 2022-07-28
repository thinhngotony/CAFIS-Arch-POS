using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPSaturnEMoney.Common
{
    class WMPControl
    {
        public WMPLib.WindowsMediaPlayer Player;
        public bool IsPlaying = false;

        public WMPControl()
        {
            Player = new WMPLib.WindowsMediaPlayer();
            Player.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(Player_PlayStateChange);
            Player.MediaError += new WMPLib._WMPOCXEvents_MediaErrorEventHandler(Player_MediaError);
        }

        private void Player_PlayStateChange(int NewState)
        {
            if ((WMPLib.WMPPlayState)NewState == WMPLib.WMPPlayState.wmppsStopped)
            {
                IsPlaying = false;
                Utilities.Log.Info($"Finished play sound {Player.currentMedia.name}.");
            }
        }
        private void Player_MediaError(object pMediaObject)
        {
            IsPlaying = false;
            Utilities.Log.Error($"▲ Cannot play media file {Player.currentMedia.name}.");
        }

        public void Play(string url)
        {
            Utilities.Log.Info($"Play sound {Path.GetFileName(url)}.");

            IsPlaying = true;
            Player.URL = url;
            Player.controls.play();
        }

        public void CloseCurrent()
        {
            Utilities.Log.Info($"Stop play sound {Player.currentMedia.name}.");

            Player.URL = null;
            Player.controls.stop();
            Player.close();
            IsPlaying = false;
        }
    }
}
