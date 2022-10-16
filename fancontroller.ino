// components' pin values
#define sonic1TrigPin 10
#define sonic1EchoPin 11
#define sonic2TrigPin 5
#define sonic2EchoPin 6
#define temperaturePin A0
#define fanPin 3


long sonic1Dist = 0;
long sonic2Dist = 0;
bool sonic1Passed = false;
bool sonic2Passed = false;

long temperature = 0; 
long activationTemp = 0;

long fanSpeed = 0; // in percentage 0-100
long startFanSpeed = 0; // in percentage 0-100

int personCount = 0;
char mode; // 'M' - manual, 'A' - auto

void setup()
{  
  pinMode(sonic1TrigPin, OUTPUT);
  pinMode(sonic1EchoPin, INPUT);
  
  pinMode(sonic2TrigPin, OUTPUT);
  pinMode(sonic2EchoPin, INPUT);
  
  pinMode(temperaturePin, INPUT);
  pinMode(fanPin, OUTPUT);
  
  Serial.begin(9600);
}

void loop()
{
  parseSerialDataFromApp();
  
  readTemperature();
  setFanSpeed();

  countPersons();
}


void sendSensorDataToApp()
{
  String data = String(personCount) + "," + 
                String(temperature) + "," + 
                String(fanSpeed);

  Serial.println(data);
}

void parseSerialDataFromApp()
{
  while (Serial.available())
  {
    String serialData = Serial.readString();

    mode = serialData.charAt(0);
    activationTemp = serialData.substring(1, 3).toInt();
    startFanSpeed = serialData.substring(3, 5).toInt();
    fanSpeed = serialData.substring(5, 7).toInt();
  }
}

void readTemperature()
{
  temperature = analogRead(temperaturePin) * 0.48828125;
}

// 30, 20, 40, 80, 100
void setFanSpeed()
{
  long analogFanSpeed = 0;

  if (mode == 'A') // auto mode
  {
    if (temperature >= activationTemp)
    {
      long maxFanSpeedTemperature = activationTemp + (100 - startFanSpeed) / 2;
      fanSpeed = map(temperature, activationTemp, maxFanSpeedTemperature, startFanSpeed, 100);
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
  readSonic(sonic1TrigPin, sonic1EchoPin, sonic1Dist);
  delay(30);

  readSonic(sonic2TrigPin, sonic2EchoPin, sonic2Dist);
  delay(30);
  
  if(sonic1Dist > 20 && !sonic1Passed)
  {
    sonic1Passed = true;
    
  	if (!sonic2Passed)
    {
      personCount++;
    }
  }
        
  if(sonic2Dist > 20 && !sonic2Passed)
  {
    sonic2Passed = true;
    
  	if (!sonic1Passed)
    {
      personCount--;
    }
  }

  if(sonic1Dist <= 5 && sonic1Passed && 
     sonic2Dist <= 5 && sonic2Passed)
  {
    sonic1Passed = false;
    sonic2Passed = false;
  }
}

void readSonic(int triggerPin, int echoPin, long &sonicDist)
{
  long time;
  
  digitalWrite(triggerPin, LOW);
  delayMicroseconds(2);
  digitalWrite(triggerPin, HIGH);
  delayMicroseconds(10);
  time =  pulseIn(echoPin, HIGH);
  
  sonicDist =  time / 29 / 2;
}
          
