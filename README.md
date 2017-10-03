Blinky Skynet Example
===============

This code is the modification of the Blinky sample from [Azure GitHub](https://developer.microsoft.com/en-us/windows/iot/samples/helloblinky)) to work with Cognitive Services - Language Understanding Intelligent Service (LUIS) without a NuGet package.

Futhermore we change the project to work with two LED lights (instead of only one). 

## How to make it work:

Set your Raspberry Pi with PINs 5 and 6 to LED lights in a breadbord board.

You’ll need a few components:

* two LED (one green and another yellow)

* two a 220 Ω resistor for the Raspberry Pi

* a breadboard and a couple of connector wires

Connect the shorter leg of the LED **green** to GPIO 5 (pin 29 on the expansion header) on the Raspberry Pi.
Connect the shorter leg of the LED **yellow** to GPIO 6 (pin 31 on the expansion header) on the Raspberry Pi.

Connect the longer leg of the LED **green** to the first resistor.
Connect the longer leg of the LED **yellow** to the second resistor.

Connect the other end of the resistor to one of the 3.3V pins on the Raspberry Pi.
Connect the other end of the resistor to one of the 3.3V pins on the Raspberry Pi.

Note that the polarity of the LED is important. (This configuration is commonly known as Active Low)

Type a command into the text box and click the button "GO". Be sure to be connected so the Raspberry Pi can connect to the LUIS Rest API.

##Source Code

Download the code and change property LuisUrl inside the class WebClientAccess.cs for you LUIS query URL

## Additional resources
* [Windows 10 IoT Core home page](https://developer.microsoft.com/en-us/windows/iot/)
* [Congnitive Services - LUIS](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/home)

