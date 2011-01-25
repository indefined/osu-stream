using System;
using MonoTouch.AVFoundation;
using MonoTouch;
using MonoTouch.Foundation;
using MonoTouch.AudioToolbox;
using System.IO;

namespace osum
{
	public class BackgroundAudioPlayerIphone : IBackgroundAudioPlayer
	{
		AVAudioPlayer player;
		
		public BackgroundAudioPlayerIphone()
		{
#if !SIMULATOR
			AudioSession.Initialize();
			AudioSession.Category = AudioSessionCategory.SoloAmbientSound;
			AudioSession.SetActive(true);
#endif
		}
		
		public bool Loop {
			get
			{
				return player.NumberOfLoops == -1;	
			}
			
			set
			{
				player.NumberOfLoops = value ? -1 : 0;
			}
		}
		
		public float Volume {
			get {
				return player.Volume;
			}
			set {
				player.Volume = value;
			}
		}
		
		public float CurrentPower {
			get {
                if (player == null) return 0;

                player.UpdateMeters();
				return player.AveragePower(0);
			}
		}
		

		public bool Play()
		{
			if (player == null)
				return false;
			
			player.Play();
			
			return true;
		}
		
		public bool Pause()
		{
			if (player == null)
				return false;
			
			player.Pause();
			
			return true;
		}
		
		public void Update()
		{
			
		}

        public unsafe bool Load(byte[] bytes)
        {
            if (player != null)
                player.Dispose();

            NSError error = null;


            fixed (byte* ptr = bytes)
            {
                NSData data = NSData.FromBytes((IntPtr)ptr,(uint)bytes.Length);

                player = AVAudioPlayer.FromData(data,out error);
                player.MeteringEnabled = true;
            }

            return error == null;
        }

        public bool Load(string filename)
        {
            if (player != null)
                player.Dispose();

            string path = filename;//NSBundle.MainBundle.BundlePath + "/test.mp3";
            NSError error;

            NSUrl url = NSUrl.FromFilename(path);

            player = AVAudioPlayer.FromUrl(url,out error);
            player.MeteringEnabled = true;

            return error == null;
        }

        public bool Stop ()
        {
        	if (player != null)
			{
				player.Stop();
				player.Dispose();
				player = null;
				
				return true;
			}
			
			return false;
        }
        
		public double CurrentTime
        {
            get
            {
                return player == null ? 0 : player.CurrentTime;
            }
        }

		public bool SeekTo(int milliseconds)
		{
			if (player == null)
				return false;
			player.CurrentTime = milliseconds/1000d;
			return true;
		}
	}
}