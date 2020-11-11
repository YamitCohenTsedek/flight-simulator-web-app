# flight-simulator-web-app
In this project, we have written a web application in REST architecture, whose purpose is to sample, display, and save the values obtained from the [FlightGear](https://www.flightgear.org/about/) simulator.<br />

The following image is displayed on the screen and covers it:<br />
![export-map-share](https://user-images.githubusercontent.com/45918740/98774866-8bd64580-23f4-11eb-8567-4a3f7b0d2b95.png)
<br /><br /> The screen borders form the following coordinates:
* **Latitude** - the TOP of the screen is the value -90 (minus 90), while the BOTTOM is the value 90.<br />
* **Longitude** - the RIGHT of the screen is the value 180, while the LEFT is the value -180 (minus 180).<br />

The application serves the following addresses:<br />
* /display/\<IP\>/\<port\> <br />
Where IP & port are the IP & port that the flight simulator listens on, for example: /display/127.0.0.1/5402.<br />
o The position of the aircraft (lon & lat) will be sampled from the flight simulator listening on the given IP & port.<br />
o The browser will display the position of the aircraft as a small icon on the screen.<br />
o NOTE: the location of the aircraft will be sampled only once and will be presented on the screen.<br />

* /display/\<IP\>/\<port\>/\<seconds\> <br />
Where IP & port are the IP & port that the flight simulator listens on, and seconds is the sampling rate.<br />
For example, for the next address: /display/127.0.0.1/5402/4, the position of the aircraft will be sampled from the flight simulator (the server) listening on IP 127.0.0.1 and port 5402 at a rate of once in every 4 seconds, and will be displayed on the screen.<br />

* /save/\<IP\>/\<port\>/\<seconds\>/\<seconds-interval\>/\<file-name\> <br />
Where IP & port are the IP & port that the flight simulator listens on, seconds is the sampling rate,
seconds-interval is the interval of seconds during which the samples will be taken,
and file-name is the name of the file where the flight data will be saved.<br />
That is, a data file about the flight will be built so that we can recover the data in real-time.<br />
The flight data that will be sampled: position (lon & lat), altitude, direction, speed.<br />

* /display/\<file-name\>/\<seconds\> <br />
Where file-name is the name of the file where the flight data was saved and seconds is the sampling rate.<br />
The flight data will be loaded from the database (the file containing the flight data), and will be displayed as animation at a rate of the given second number.
We assume that the file exists in the working directory. <br />
After the animation ends, an alert will pop up, announcing that the scenario is over.<br />

### Watch on YouTube:
[![](https://user-images.githubusercontent.com/45918740/98780612-bb3d8000-23fd-11eb-90d8-e1a026099e16.JPG)](https://youtu.be/xeTU92tk5qE)
