/*
 Name:		Box.ino
 Created:	11/11/2015 11:01:46
 Author:	justu_000
*/

/*
LiquidCrystal Library - display() and noDisplay()

Demonstrates the use a 16x2 LCD display.  The LiquidCrystal
library works with all LCD displays that are compatible with the
Hitachi HD44780 driver. There are many of them out there, and you
can usually tell them by the 16-pin interface.

This sketch prints "Hello World!" to the LCD and uses the
display() and noDisplay() functions to turn on and off
the display.

The circuit:
* LCD RS pin to digital pin 12
* LCD Enable pin to digital pin 11
* LCD D4 pin to digital pin 5
* LCD D5 pin to digital pin 4
* LCD D6 pin to digital pin 3
* LCD D7 pin to digital pin 2
* LCD R/W pin to ground
* 10K resistor:
* ends to +5V and ground
* wiper to LCD VO pin (pin 3)

Library originally added 18 Apr 2008
by David A. Mellis
library modified 5 Jul 2009
by Limor Fried (http://www.ladyada.net)
example added 9 Jul 2009
by Tom Igoe
modified 22 Nov 2010
by Tom Igoe

This example code is in the public domain.

http://www.arduino.cc/en/Tutorial/LiquidCrystalDisplay

*/

// include the library code:
#include <LiquidCrystal.h>

// initialize the library with the numbers of the interface pins
LiquidCrystal lcd(12, 11, 5, 4, 3, 2);

const int BUTTONPIN = A2;
const int LEDPIN[] = {8, 9, 10};


// stuff for blinking LEDs
unsigned long previousMillis[] = {0, 0, 0};
int ledState[] = {LOW, LOW, LOW};
const long interval = 200;           // interval at which to blink (milliseconds)
boolean ledsOn = false;

// handle messages from PC
const String ERRORIDENTIFIER = "Errors:";
int nbrOfErrors = 0;

void setup() {
	// set up the LCD's number of columns and rows:
  lcd.begin(20, 2);
  printStandardMessage();
  
	pinMode(LEDPIN[0], OUTPUT);
	pinMode(LEDPIN[1], OUTPUT);
	pinMode(LEDPIN[2], OUTPUT);
	pinMode(BUTTONPIN, INPUT);

	Serial.begin(9600);
}

void loop() {
	// print data from PC
	if (Serial.available() > 0) {
		String input = Serial.readString();
		if (input.startsWith(ERRORIDENTIFIER)) {
			nbrOfErrors += input.substring(ERRORIDENTIFIER.length()).toInt();
			if (nbrOfErrors > 1) {
				input = String(nbrOfErrors) + " Errors";
			}
			else
			{
				input = String(nbrOfErrors) + " Error";
			}
			
		}
    lcd.clear();
    lcd.print(input);
   ledsOn = true;
   unsigned long m = millis();
   previousMillis[0] = m-interval;
   previousMillis[1] = m-interval/3;
   previousMillis[2] = m-interval/3*2;
   
	}

	// reset with button
	if (digitalRead(BUTTONPIN) == HIGH) {
		printStandardMessage();
		lightsOff();
		ledsOn = false;
		nbrOfErrors = 0;
	}

  // manage blinking
  if (ledsOn) {
    unsigned long currentMillis = millis();
    for (int i=0; i < 3; i++){
      if (currentMillis - previousMillis[i] >= interval) {
        previousMillis[i] = currentMillis;
        toggleLED(i);
      }
    }
  }
}

void toggleLED(int ledPin) {
    if (ledState[ledPin] == LOW) {
      ledState[ledPin] = HIGH;
    } else {
      ledState[ledPin] = LOW;
    }
    digitalWrite(LEDPIN[ledPin], ledState[ledPin]);
}

void printStandardMessage(){
  lcd.clear();
  lcd.print("500 for the piece");
  lcd.setCursor(0, 1);
  lcd.print("bullets on the house");
}

void lightsOn() {
	digitalWrite(LEDPIN[0], HIGH);
	digitalWrite(LEDPIN[1], HIGH);
	digitalWrite(LEDPIN[2], HIGH);
}

void lightsOff() {
	digitalWrite(LEDPIN[0], LOW);
	digitalWrite(LEDPIN[1], LOW);
	digitalWrite(LEDPIN[2], LOW);
}

