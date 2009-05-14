/*
Copyright 2009 Len Popp

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

// Define the input bits and output pins for the various switches.
#define pinGreen      3
#define pinMaskGreen  0x01
#define pinRed        4
#define pinMaskRed    0x02
#define pinYellow     5
#define pinMaskYellow 0x04
#define pinBlue       6
#define pinMaskBlue   0x08
#define pinOrange     12
#define pinMaskOrange 0x10
#define pinStrum      11
#define pinMaskStrum  0x20
#define pinPower      10
#define pinMaskPower  0x40
#define pinWhammy     9
#define pinMaskWhammy 0x80
#define pinLED        13

// Momentary timer for the strum bar
bool fStrumOn = false;
unsigned long tStrumOff = 0;
#define dtStrum      30

void initPin(int pin)
{
  pinMode(pin, OUTPUT);
  digitalWrite(pin, LOW);
}

void setup()
{
  initPin(pinGreen);
  initPin(pinRed);
  initPin(pinYellow);
  initPin(pinBlue);
  initPin(pinOrange);
  initPin(pinStrum);
  initPin(pinPower);
  initPin(pinWhammy);
  initPin(pinLED);
  Serial.begin(57600);
  //Serial.println("Started");
}

void switchOnOff(byte bCommand, int pin, byte pinMask)
{
  if (bCommand & pinMask) {
    digitalWrite(pin, HIGH);
  } else {
    digitalWrite(pin, LOW);
  }
}

void loop()
{
  // Check if it's time to un-strum.
  if (fStrumOn && millis() >= tStrumOff) {
    fStrumOn = false;
    digitalWrite(pinStrum, LOW);
    digitalWrite(pinLED, LOW);
  }
  
  // Handle commands from the serial port.
  int bCommand;
  while (Serial.available() > 0) {
    bCommand = Serial.read();
    if (bCommand == 0) {
      // Turn all off.
      digitalWrite(pinGreen, LOW);
      digitalWrite(pinRed, LOW);
      digitalWrite(pinYellow, LOW);
      digitalWrite(pinBlue, LOW);
      digitalWrite(pinOrange, LOW);
      digitalWrite(pinStrum, LOW);
      digitalWrite(pinPower, LOW);
      digitalWrite(pinWhammy, LOW);
      digitalWrite(pinLED, LOW);
      fStrumOn = false;
    } else {
      // Turn various buttons on/off.
      // NOTE: Whammy bar is not yet supported.
      switchOnOff(bCommand, pinGreen, pinMaskGreen);
      switchOnOff(bCommand, pinRed, pinMaskRed);
      switchOnOff(bCommand, pinYellow, pinMaskYellow);
      switchOnOff(bCommand, pinBlue, pinMaskBlue);
      switchOnOff(bCommand, pinOrange, pinMaskOrange);
      switchOnOff(bCommand, pinPower, pinMaskPower);
      if (bCommand & pinMaskStrum) {
        // Strum - Turn the Strum switch on, and set a timeout to turn it off.
        if (fStrumOn) {
          // oops, strum switch is already on. Turn it off briefly, so we can strum again!
          digitalWrite(pinStrum, LOW);
          delay(1); // is that long enough to register a new strum?
        }
        fStrumOn = true;
        tStrumOff = millis() + dtStrum;
        digitalWrite(pinStrum, HIGH);
        digitalWrite(pinLED, HIGH);  // also provide a visual activity indicator
      }
    }
    //Serial.print("Received: ");
    //Serial.println(bCommand, HEX);
  }
}
