using Skynet.WebClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Media.MediaProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System.IO;
using Windows.Foundation;

namespace Blinky
{
    public sealed partial class MainPage : Page
    {
        private const int LED_GREEN_PIN = 5;
        private const int LED_YELLOW_PIN = 6;
        private GpioPin pinGreen;
        private GpioPin pinYellow;
        private MediaCapture medCapture = new MediaCapture();
        private bool isCameraInitialized = false;

        //Hard-coded authorization
        //Eduardo -> has authority on the Green led
        //Marc -> has authority on the Yellow led
        private IDictionary<Entity, string> AuthorizedPeople = new Dictionary<Entity, string>
        {
            { Entity.Green, "Eduardo" },
            { Entity.Yellow, "Marc" },
        };

        public MainPage()
        {
            InitializeComponent();
            InitGPIO();
        }

        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            pinGreen = gpio.OpenPin(LED_GREEN_PIN);
            pinYellow = gpio.OpenPin(LED_YELLOW_PIN);
            pinGreen.SetDriveMode(GpioPinDriveMode.Output);
            pinYellow.SetDriveMode(GpioPinDriveMode.Output);
        }

        private GpioPin[] GetAuthorizedPins(Entity entity, IList<string> authorizedPeople)
        {
            var result = new List<GpioPin>();
            switch (entity)
            {
                case Entity.Green:
                    CheckAccessForLight(result, authorizedPeople, entity, pinGreen);
                    break;
                case Entity.Yellow:
                    CheckAccessForLight(result, authorizedPeople, entity, pinYellow);
                    break;
                case Entity.All:
                    CheckAccessForLight(result, authorizedPeople, Entity.Green, pinGreen);
                    CheckAccessForLight(result, authorizedPeople, Entity.Yellow, pinYellow);
                    break;
                default:
                    break;
            }

            return result.ToArray();
        }

        private void CheckAccessForLight(IList<GpioPin> pinList, IList<string> identifiedPeople, Entity light, GpioPin pin)
        {
            if (identifiedPeople.Contains(AuthorizedPeople[light]))
            {
                pinList.Add(pin);
            }
            else
            {
                MessagesTextBox.Text += $"Permission denied for {light} light\n";
            }
        }

        private async void GO_Click(object sender, RoutedEventArgs e)
        {
            //take a picture of the person giving the order
            var peopleIdentified = await TakePictureAndIdentifyAsync();

            var result = await WebClientAccess.Order(CommandTextBox.Text);

            var pins = GetAuthorizedPins(result.Entity, peopleIdentified);
            foreach (var pin in pins)
            {
                switch (result.Intent)
                {
                    case Intent.LightOn:
                        pin.Write(GpioPinValue.Low);
                        break;
                    case Intent.LightOff:
                        pin.Write(GpioPinValue.High);
                        break;
                    case Intent.None:
                    default:
                        break;
                }
            }

            CommandTextBox.Text = "";
            CommandTextBox.Focus(FocusState.Keyboard);
        }

        private async Task<IList<string>> TakePictureAndIdentifyAsync()
        {
            IList<string> peopleIdentified = new List<string>();
            if (!isCameraInitialized)
            {
                await medCapture.InitializeAsync();
                isCameraInitialized = true;
            }

            var imgFmt = ImageEncodingProperties.CreateJpeg();
            var bmImage = new BitmapImage();
            var fileName = Guid.NewGuid().ToString() + ".jpg";
            var collisionOption = CreationCollisionOption.GenerateUniqueName;
            StorageFile file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(fileName, collisionOption);

            // captures and stores image file
            MessagesTextBox.Text = $"Taking picture\n";
            await medCapture.CapturePhotoToStorageFileAsync(imgFmt, file);
            using (var strm = await file.OpenReadAsync())
            {
                //start with clean list
                await bmImage.SetSourceAsync(strm);
                image.Source = bmImage;
                image.HorizontalAlignment = HorizontalAlignment.Center;

                strm.Seek(0);
                MessagesTextBox.Text = $"Sending pictue for detection\n";
                peopleIdentified = await WebClientAccess.GetPeopleAsync(strm.AsStreamForRead());
            }

            //delete image file
            await file.DeleteAsync();

            return peopleIdentified;
        }
    }
}
