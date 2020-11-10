# flight-simulator-web-app
We have written a web application in the REST architecture that serves the following addresses<br />
 /display/127.0.0.1/5402 :<br />
o We modeled the position of the aircraft (LON, LAT) from the flight simulator located on 1.0.0.127 at port 5402.<br />
o The browser will display the position of the plane as a small icon on the screen.<br />
An image will be displayed which will be displayed on the screen and will cover the whole.<br />
o The screen borders will form the following coordinates:<br />
 latitude - the TOP of the screen will be the value -90 (minus 90) while the BOTTOM will be the value 90.<br />
 Longitude - the RIGHT of the screen will be the value 180 while the LEFT will be the value -180 (minus 180.)<br />
o Please note that we only sample the location of the aircraft once and present it.<br />
We will explain in detail:<br />

/ save / <IP> / <port> / <seconds> / <seconds-interval> / <file-name><br />

Where IP & port are the IP & port that the flight simulator listens on, seconds is the sampling rate,<br />
seconds-interval is the interval of seconds during which the samples will be taken,<br />
and file-name is the name of the file where the samples will be saved.<br />
