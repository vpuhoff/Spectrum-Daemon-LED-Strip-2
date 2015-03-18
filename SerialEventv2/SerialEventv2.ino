#include <Adafruit_NeoPixel.h>

#define PIN 6

// Parameter 1 = number of pixels in strip
// Parameter 2 = pin number (most are valid)
// Parameter 3 = pixel type flags, add together as needed:
//   NEO_KHZ800  800 KHz bitstream (most NeoPixel products w/WS2812 LEDs)
//   NEO_KHZ400  400 KHz (classic 'v1' (not v2) FLORA pixels, WS2811 drivers)
//   NEO_GRB     Pixels are wired for GRB bitstream (most NeoPixel products)
//   NEO_RGB     Pixels are wired for RGB bitstream (v1 FLORA pixels, not v2)
Adafruit_NeoPixel strip = Adafruit_NeoPixel(24, PIN, NEO_GRB + NEO_KHZ800);

// Fill the dots one after the other with a color
void colorWipe(uint32_t c) {
  for(uint16_t i=0; i<strip.numPixels(); i++) {
      strip.setPixelColor(i, c);
  }
  strip.show();
}
/*
  Serial Event example
 
 When new serial data arrives, this sketch adds it to a String.
 When a newline is received, the loop prints the string and 
 clears it.
 
 A good test for this is to try it with a GPS receiver 
 that sends out NMEA 0183 sentences. 
 
 Created 9 May 2011
 by Tom Igoe
 
 This example code is in the public domain.
 
 http://www.arduino.cc/en/Tutorial/SerialEvent
 
 */

String inputString = "";         // a string to hold incoming data
boolean stringComplete = false;  // whether the string is complete

void setup() {
  // initialize serial:
  strip.begin();
  strip.show(); // Initialize all pixels to 'off'
  colorWipe(strip.Color(120, 0, 0)); // Red
  colorWipe(strip.Color(0, 120, 0)); // Green
  colorWipe(strip.Color(0, 0, 120)); // Blue
  colorWipe(strip.Color(0, 0, 0)); // Blue
  Serial.begin(115200);
  // reserve 200 bytes for the inputString:
  inputString.reserve(200);
}
byte R=0;
byte B=0;
byte G=0;
byte chk=0;
byte num=0;
String s;
void loop() {
  // print the string when a newline arrives:
  if (stringComplete) {
   //Serial.println(inputString);
   s=(inputString.substring(0, 3));R=s.toInt();
   s=(inputString.substring(3, 6));G=s.toInt();
   s=(inputString.substring(6, 9));B=s.toInt();
   s=(inputString.substring(9, 12));chk=s.toInt();
   //Serial.println(R);
   //Serial.println(G);
  // Serial.println(B);
   //Serial.println(chk);
   if (chk==111){
    colorWipe(strip.Color(R, G, B)); 
   }
   if (chk==222){
     s=(inputString.substring(12, 15));num=s.toInt();
     strip.setPixelColor(num, strip.Color(R, G, B));
   }
    if (chk==223){
     s=(inputString.substring(12, 15));num=s.toInt();
     strip.setPixelColor(num, strip.Color(R, G, B));
     strip.show();
   }
   
   if (chk==250){
     strip.show();
   }
   
    // clear the string:
    inputString = "";
    stringComplete = false;
    Serial.println("OK");
  }
}

/*
  SerialEvent occurs whenever a new data comes in the
 hardware serial RX.  This routine is run between each
 time loop() runs, so using delay inside loop can delay
 response.  Multiple bytes of data may be available.
 */
void serialEvent() {
  while (Serial.available()) {
    // get the new byte:
    char inChar = (char)Serial.read(); 
    // add it to the inputString:
    inputString += inChar;
    // if the incoming character is a newline, set a flag
    // so the main loop can do something about it:
    if (inChar == '\n') {
      stringComplete = true;
    } 
  }
}


