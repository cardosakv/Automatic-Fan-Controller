# Automatic-Fan-Controller
Embedded Systems course project: Automatic Fan Controller Based on Temperature and Human Presence<br/><br/>
Team: [Khyle Vincent Cardosa](https://github.com/kvcards26) & [Daryl Paul Lagusay](https://github.com/darylpaul213)<br/><br/>

### Serial Data Format

Data to be transferred from the app to the Arduino:<br/>
  1. Mode<br/>
  2. ActivationTemp<br/>
  3. StartFanSpeed<br/>
  4. FanSpeed<br/>
  
  `A258030`<br/>
  `A`  - mode (A, auto or M, manual)<br/>
  `25` - activation temperature<br/>
  `80` - start fan speed<br/>
  `30` - manual fan speed<br/>
  
Data to be transferred from the Arduino to the app:

  1. PeopleCount<br/>
  2. Temperature<br/>
  3. FanSpeed<br/>

  `6,38,70`<br/>
  `6`  - person count<br/>
  `38` - temperature<br/>
  `70` - fan speed<br/>
