using System;
using NAudio.Wave;

namespace TeaChat.Audio
{
    /// <summary>
    /// Audio handling class to record audio data from microphone and play recorded audio data
    /// </summary>
    public class AudioHandler
    {
        private WaveIn audio_recoder;

        private DirectSoundOut audio_player;
        private BufferedWaveProvider audio_out_provider;

        /// <summary>
        /// Constructor
        /// </summary>
        public AudioHandler()
        {
            // initialize audio recorder
            this.audio_recoder = new WaveIn();
            this.audio_recoder.DataAvailable += new EventHandler<WaveInEventArgs>(this.AudioDataAvailable);

            // initialize output audio data provider for audio data storage
            this.audio_out_provider = new BufferedWaveProvider(this.audio_recoder.WaveFormat);
            this.audio_out_provider.DiscardOnBufferOverflow = true;

            // inotialize audio data player
            this.audio_player = new DirectSoundOut();
            this.audio_player.Init(this.audio_out_provider);
        }

        /// <summary>
        /// Input data into output streaming of media device.
        /// </summary>
        /// <param name="data">sound data</param>
        /// <param name="data_size">size of sound data</param>
        public void SetStreamingAudioData(byte[] data, int data_size)
        {
            // invalid parameters handling
            if (data == null || data_size <= 0 || data.Length <= 0) return;

            // invalid out-of-index data size handling
            if (data_size > data.Length) data_size = data.Length;

            this.audio_out_provider.ClearBuffer(); // refresh
            this.audio_out_provider.AddSamples(data, 0, data_size); // set new samples
            this.audio_player.Play();
            //System.Console.WriteLine("Play voice");
        }

        /// <summary>
        /// Start recording from micorphone and playing audio data to media device
        /// </summary>
        public void Enable()
        {
            try
            {
                this.audio_recoder.StartRecording();
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.ToString()); // debug
            }
        }

        /// <summary>
        /// Stop recording from micorphone and playing audio data to media device 
        /// </summary>
        public void Disable()
        {
            this.audio_recoder.StopRecording();
            this.audio_player.Stop();
        }

        void AudioDataAvailable(object sender, WaveInEventArgs e)
        {
            // form data into packet and send packet to server
            // sudo operation
            this.SetStreamingAudioData(e.Buffer, e.BytesRecorded);
        }
    }
}
