//---------------------------------------------------------------
// Date: 10/26/2014
// Rights: 
// FileName: WindowsVoiceBox.cs
//---------------------------------------------------------------

namespace CDP.UWP.Components.Speech
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Template10.Common;
    using Windows.Media.Core;
    using Windows.Media.Playback;
    using Windows.Media.SpeechSynthesis;

    /// <summary>
    /// Handles all of the voice projection for the app.  TODO: I don't think this is used anymore...
    /// </summary>
    public sealed class WindowsVoiceBox
    {
        #region Private members

        /// <summary>
        /// The media element to play the phrases over audio
        /// </summary>
        private MediaPlayer mediaPlayer = new MediaPlayer();

        /// <summary>
        /// The text to speech synthesizer, handles converting text strings to spoken word.
        /// </summary>
        private SpeechSynthesizer synth;
        private IDispatcherWrapper dispatcher;

        /// <summary>
        /// Occurs when [speech ended].
        /// </summary>
        public event EventHandler SpeechEnded;

        /// <summary>
        /// Gets a value indicating whether this instance is playing.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is playing; otherwise, <c>false</c>.
        /// </value>
        public bool IsPlaying { get { return this.mediaPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing; } }

        #endregion

        #region Public Static Properties

        #endregion

        #region Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsVoiceBox"/> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        public WindowsVoiceBox(IDispatcherWrapper dispatcher)
        {
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            this.dispatcher = dispatcher;
            synth = new SpeechSynthesizer
            {
                Voice = SpeechSynthesizer.AllVoices.Where(v => v.Gender == VoiceGender.Female).First()
            };
        }

        /// <summary>
        /// Medias the player media ended.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The arguments.</param>
        private void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
        {
            SpeechEnded?.Invoke(this, null);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Says it asynchronously.
        /// </summary>
        /// <param name="whatToSay">The what to say.</param>
        /// <returns></returns>
        public async Task SayItAsync(string whatToSay)
        {
            try
            {
                using (var stream = await synth.SynthesizeTextToStreamAsync(whatToSay))
                {
                    mediaPlayer.Source = MediaSource.CreateFromStream(stream, stream.ContentType);
                }
                
                mediaPlayer.Play();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Sounds the horn.
        /// </summary>
        /// <returns></returns>
        public async Task SoundHorn()
        {
            try
            {
                await Task.Run(() => 
                {
                    Uri pathUri = new Uri("ms-appx:///Assets/3-Second-Horn.wav");
                    mediaPlayer.Source = MediaSource.CreateFromUri(pathUri);
                    mediaPlayer.Play();
                });
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates the SMSS string.
        /// </summary>
        /// <param name="whatToSay">The what to say.</param>
        /// <returns></returns>
        private static string CreateSmssString(string whatToSay)
        {
            return "<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\"" +
                " xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"" +
                " xsi:schemaLocation=\"http://www.w3.org/TR/speech-synthesis/synthesis.xsd\"" +
                " xml:lang=\"en-US\">" +
                "<voice gender=\"female\" age=\"30\"><p>" +
                whatToSay +
                "</p></voice></speak>";
        }

        /// <summary>
        /// Synthes the SMSS.
        /// </summary>
        /// <param name="smss">The SMSS.</param>
        /// <returns></returns>
        private async Task<SpeechSynthesisStream> SynthSmss(string smss)
        {
            var whatToSay = CreateSmssString(smss);
            return await this.synth.SynthesizeSsmlToStreamAsync(whatToSay);
        }

        #endregion
    }
}