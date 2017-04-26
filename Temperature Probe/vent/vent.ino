#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>

// WiFi parameters to be configured
const char* ssid = "crunchytown";
const char* password = "zxasqw12";

#define SERVO_PIN 2 
#define VENT_OPEN 900
#define VENT_CLOSED 1500

float oldTemp;
WiFiServer server(80);
String therm_ip;

void setup(void)
{
  delay(1000);
  Serial.begin(9600);
  pinMode(SERVO_PIN, OUTPUT);
  
  WiFi.begin(ssid, password);
  
  while (WiFi.status() != WL_CONNECTED) {
     delay(500);
     Serial.print(".");
  }
  //print a new line, then print WiFi connected and the IP address
  Serial.println("");
  Serial.println("WiFi connected");  

  //wifiManager.autoConnect("crunchytown", "zxasqw12");
  
  // Print the IP address
  Serial.println(WiFi.localIP());
  //therm_ip = getThermIP();
  
  server.begin();
}

void loop(void)
{
  float temp;
  //Serial.println("Getting client..");
  WiFiClient client = server.available();
  if (!client) {
    return;
  }
  else {
    String req;
    Serial.println("Waiting for available client..");
    while(!client.available()){
      delay(1);
    }
    Serial.println("Waiting for request..");
    req = client.readString();
    Serial.print("Got request: ");
    Serial.println(req);
    client.flush();

    int state;
    if (req.indexOf("open") != -1){
      state = VENT_OPEN;
      Serial.println("Opening..");
    }
    else if (req.indexOf("close") != -1){
      state = VENT_CLOSED;
      Serial.println("Closing..");
    }
    else {
      return;
    }
    Serial.println("Changing state..");
    for (int i = 0; i < 100; i++){
      digitalWrite(SERVO_PIN, 1);
      delayMicroseconds(state);
      digitalWrite(SERVO_PIN, 0);
      delayMicroseconds(20000 - state);
    }
    Serial.println("Done..");
    /*
    unsigned long start_time = millis();
    while (millis() < (start_time + 3000)){
      digitalWrite(SERVO_PIN, 1);
      delayMicroseconds(state);
      digitalWrite(SERVO_PIN, 0);
      delayMicroseconds(20000 - state);
    }*/
  } 
}


