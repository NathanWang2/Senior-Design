#include <DallasTemperature.h>
#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>
#include <OneWire.h>

// WiFi parameters to be configured
const char* ssid = "crunchytown";
const char* password = "zxasqw12";

#define ONE_WIRE_BUS 2  // DS18B20 pin
OneWire oneWire(ONE_WIRE_BUS);
DallasTemperature DS18B20(&oneWire);

float oldTemp;
WiFiServer server(80);

void setup(void)
{
  delay(1000);
  Serial.begin(9600);
  
  // Connect to WiFi
  WiFi.begin(ssid, password);
  
  // while wifi not connected yet, print '.'
  // then after it connected, get out of the loop
  while (WiFi.status() != WL_CONNECTED) {
     delay(500);
     Serial.print(".");
  }
  //print a new line, then print WiFi connected and the IP address
  Serial.println("");
  Serial.println("WiFi connected");  
  
  // Print the IP address
  Serial.println(WiFi.localIP());
  server.begin();
}

void loop(void)
{
  float temp;
  WiFiClient client = server.available();
  if (!client) {
    return;
  }
  else {
    String req;

    while(!client.available()){
      delay(1);
    }
    
    req = client.readString();
    Serial.println(req);
    client.flush();
    
    if (req.indexOf("temp") != -1){
      do {
        DS18B20.requestTemperatures(); 
        temp = DS18B20.getTempCByIndex(0);
      } while (temp == 85.0 || temp == (-127.0));  
    Serial.print("Temperature: ");
    Serial.println(temp * 9 / 5 + 32);
      
    client.print(temp);
      
    }
  }  
}


