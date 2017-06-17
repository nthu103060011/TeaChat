using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;


namespace TeaChat.Audio
{
    /// <summary>
    /// ConferenceCallWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ConferenceCallWindow : Window
    {
        private static readonly int CC_STATE_ON_CALL = 1;
        private static readonly int CC_STATE_ON_CHAT = 2;
        private static readonly int CC_STATE_TO_END = 3;

        private static readonly String CC_STATE_TEXT_ON_CALL = "Now Calling";
        private static readonly String CC_STATE_TEXT_ON_CHAT = "Chat On";

        private int conf_call_state = 0;

        private AudioHandler audio_handler = new AudioHandler();

        private BitmapImage on_call_image = null;
        private BitmapImage to_end_image = null;

        public ConferenceCallWindow()
        {
            InitializeComponent();

            this.InitializeImages();

            this.SetConferenceCallStateOnCall();
        }

        public void SetSocketandChatRoom(LogInWindow home_window, ChatWindow chat_room)
        {
            this.audio_handler.SetAudioPacketHandler(home_window, chat_room);
        }

        ~ConferenceCallWindow()
        {
            this.SetConferenceCallStateToEnd();
        }

        /// <summary>
        /// Set state of conference call to on-call state.
        /// Wait for the connect to conference call.
        /// </summary>
        public void SetConferenceCallStateOnCall()
        {
            // set state to on call
            this.conf_call_state = CC_STATE_ON_CALL;

            // set on-call 
            this.stateImage.Visibility = Visibility.Visible;
            this.stateImage.Source = this.on_call_image;

            // display state botton
            this.stateButton.Visibility = Visibility.Collapsed;

            // set state text to on-call text
            this.stateTextBox.Text = CC_STATE_TEXT_ON_CALL;
        }

        /// <summary>
        /// Set state of conference call to on-chat state
        /// Chat on conference call.
        /// </summary>
        public void SetConferenceCallStateOnChat()
        {
            this.audio_handler.Enable();

            // set state to on chat
            this.conf_call_state = CC_STATE_ON_CHAT;

            // set to-end image
            this.stateImage.Source = this.to_end_image;

            // display state botton
            this.stateButton.Visibility = Visibility.Hidden;

            // set state text to on-chat text
            this.stateTextBox.Text = CC_STATE_TEXT_ON_CHAT;
        }

        /// <summary>
        /// Set state of conference call to to-end state
        /// Close conference call.
        /// </summary>
        public void SetConferenceCallStateToEnd()
        {
            // set state to to end
            this.conf_call_state = CC_STATE_TO_END;

            this.audio_handler.Disable();
        }

        public void PlayAudioData(byte[] data, int data_size)
        {
            this.audio_handler.SetStreamingAudioData(data, data_size);
        }

        private void InitializeImages()
        {
            try
            {
                this.on_call_image = new BitmapImage(new Uri("Images/on_call.png", UriKind.RelativeOrAbsolute));
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("on_call.png not found");
                Console.WriteLine(e.ToString());
                this.on_call_image = new BitmapImage();
            }

            try
            {
                this.to_end_image = new BitmapImage(new Uri("Images/to_end.png", UriKind.RelativeOrAbsolute));
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("to_end.png not found");
                Console.WriteLine(e.ToString());
                this.to_end_image = new BitmapImage();
            }
        }

        private void stateButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.conf_call_state == CC_STATE_ON_CHAT)
            {
                this.Close();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.SetConferenceCallStateToEnd();
        }
    }
}
