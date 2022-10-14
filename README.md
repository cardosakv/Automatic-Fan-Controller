# Automatic-Fan-Controller
Embedded Systems course project: Temperature based and human presence automatic fan controller.

### Serial Data Format

Data to be transferred from the app to the Arduino:<br/>
  1. Mode<br/>
  2. ActivationTemp<br/>
  3. StartFanSpeed<br/>
  
Data to be transferred from the Arduino to the app:

  1. PeopleCount<br/>
  2. Temperature<br/>
  3. FanSpeed<br/>

In writing data to the serial port, follow the format (value must be 2 digits): `DataType:Value`<br/><br/>
  `PeopleCount:08`<br/>
  `StartFanSpeed:50`<br/>
  `Temperature:38`<br/>
  `ActivationTemp:25`<br/>
