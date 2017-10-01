using Skynet.WebClient;
using System;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
namespace Blinky
{
    public sealed partial class MainPage : Page
    {
        private const int LED_GREEN_PIN = 5;
        private const int LED_YELLOW_PIN = 6;
        private GpioPin pinGreen;
        private GpioPin pinYellow;

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

        private GpioPin[] GetPins(Entity entity)
        {
            GpioPin[] result = new GpioPin[] { };
            switch (entity)
            {
                case Entity.Green:
                    result = new GpioPin[] { pinGreen };
                    break;
                case Entity.Yellow:
                    result = new GpioPin[] { pinYellow };
                    break;
                case Entity.All:
                    result = new GpioPin[] { pinGreen, pinYellow };
                    break;
                default:
                    break;
            }

            return result;
        }

        private async void GO_Click(object sender, RoutedEventArgs e)
        {
            var result = await WebClientAccess.Order(CommandTextBox.Text);

            var pins = GetPins(result.Entity);
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
    }
}