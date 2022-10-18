// components' pin values
#define sonic1TrigPin 10
#define sonic1EchoPin 11
#define sonic2TrigPin 5
#define sonic2EchoPin 6
#define temperaturePin A0
#define fanPin 3


long sonic1Dist = 0;  // in cm
long sonic2Dist = 0;  // in cm
long sonic1DistInit = 0;
long sonic2DistInit = 0;
bool sonic1Triggered = false;
bool sonic2Triggered = false;
int sonicSensitivity = 10; // in cm

unsigned long int lastCheckTempTime = 0;
int temperature;               // in Celsius
int checkTempInterval = 10000; // default 10 secs
int activationTemp = 25;       // in Celsius

int mode = 1;                  // 1 - auto, 0 - manual
int fanSpeed = 0;              // in percentage 0-100
int startFanSpeed = 50;        // in percentage 0-100
int personCount = 0;


void setup()
{  
  pinMode(sonic1TrigPin, OUTPUT);
  pinMode(sonic1EchoPin, INPUT);
  
  pinMode(sonic2TrigPin, OUTPUT);
  pinMode(sonic2EchoPin, INPUT);
  
  pinMode(temperaturePin, INPUT);
  pinMode(fanPin, OUTPUT);

  getSonicInitialDistances();

  Serial.setTimeout(2);
  Serial.begin(115200);
}

void loop()
{
  Serial.println(sonic1Dist);
  Serial.println(sonic1DistInit);
  Serial.println(sonic2Dist);
  Serial.println(sonic2DistInit);
  parseSerialDataFromApp();
  
  readTemperature();
  setFanSpeed();
  countPersons();
  
  sendSensorDataToApp();
}


void parseSerialDataFromApp()
{
  if (Serial.available())
  {
    String serialData = Serial.readStringUntil(':');

    String dataName = serialData.substring(0, serialData.indexOf(':'));
    int dataValue = Serial.parseInt();

    if (dataName == "Mode")
    {
      mode = dataValue;
    }
    else if (dataName == "ActivationTemp")
    {
      activationTemp = dataValue;
    }
    else if (dataName == "StartFanSpeed")
    {
      startFanSpeed = dataValue;
    }
    else if (dataName == "FanSpeed")
    {
      fanSpeed = dataValue;
    }
  }
}

void readTemperature() // happens every interval set (5 sec)
{
  if (millis() - lastCheckTempTime >= checkTempInterval)
  {
    int sensorInput = analogRead(temperaturePin); 
    double temp = (double)sensorInput / 1024; 
    temp = temp * 5; 
    temp = temp - 0.5; 
    temp = temp * 100;

    temperature = temp;

    lastCheckTempTime += checkTempInterval;
  }
}

void setFanSpeed()
{
  long analogFanSpeed = 0;

  if (mode == 1) // auto mode
  {
    if (temperature >= activationTemp && personCount > 0)
    {
      long maxFanSpeedTemperature = activationTemp + 15;
      fanSpeed = map(temperature, activationTemp, maxFanSpeedTemperature, startFanSpeed, 100);

      if (fanSpeed > 100)
      {
        fanSpeed = 100;
      }
      
      analogFanSpeed = map(fanSpeed, 0, 100, 0, 255);
    }
    else
    {
      fanSpeed = 0;
      analogFanSpeed = 0;
    }
  }
  else // manual mode
  {
    analogFanSpeed = map(fanSpeed, 0, 100, 0, 255);
  }

  analogWrite(fanPin, analogFanSpeed);
}

void countPersons()
{
  readSonic(sonic1TrigPin, sonic1EchoPin, sonic1Dist); delay(30);
  readSonic(sonic2TrigPin, sonic2EchoPin, sonic2Dist); delay(30);
  
  if(sonic1Dist < sonic1DistInit - sonicSensitivity && !sonic1Triggered)
  {
    sonic1Triggered = true;
    
  	if (!sonic2Triggered)
    {
      personCount++;
    }
  }
        
  if(sonic2Dist < sonic2DistInit - sonicSensitivity && !sonic2Triggered)
  {
    sonic2Triggered = true;
    
  	if (!sonic1Triggered && personCount > 0)
    {
      personCount--;
    }
  }

  if(sonic1Dist >= sonic1DistInit - 5 && sonic1Triggered && 
     sonic2Dist >= sonic2DistInit - 5 && sonic2Triggered)
  {
    sonic1Triggered = false;
    sonic2Triggered = false;
  }
}

void getSonicInitialDistances()
{
  while (sonic1DistInit == 0 || sonic2DistInit == 0)
  {
    readSonic(sonic1TrigPin, sonic1EchoPin, sonic1DistInit);
    readSonic(sonic2TrigPin, sonic2EchoPin, sonic2DistInit);
  }
}

void readSonic(int triggerPin, int echoPin, long &sonicDist)
{
  digitalWrite(triggerPin, LOW);
  delayMicroseconds(2);
  digitalWrite(triggerPin, HIGH);
  delayMicroseconds(10);
  long time =  pulseIn(echoPin, HIGH);
  
  sonicDist =  time / 29 / 2;
}

void sendSensorDataToApp()
{
  Serial.print("Temperature:");
  Serial.println(temperature);
  Serial.print("PersonCount:");
  Serial.println(personCount);
  Serial.print("FanSpeed:");
  Serial.println(fanSpeed);
}