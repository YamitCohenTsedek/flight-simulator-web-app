# flight-simulator-web-app
We have written a web application in the REST architecture that serves the following addresses
: /display/127.0.0.1/5402 
o We modeled the position of the aircraft (LON, LAT) from the flight simulator located on 1.0.0.127 at port 5402.
o The browser will display the position of the plane as a small icon on the screen.
An image will be displayed which will be displayed on the screen and will cover the whole.
o The screen borders will form the following coordinates:
 latitude - the TOP of the screen will be the value -90 (minus 90) while the BOTTOM will be the value 90
 Longitude - the RIGHT of the screen will be the value 180 while the LEFT will be the value -180 (minus 180.)
o Please note that we only sample the location of the aircraft once and present it.
We will explain in detail:
/ save / <IP> / <port> / <seconds> / <seconds-interval> / <file-name>

Where IP & port are the IP & port that the flight simulator listens on, seconds is the sampling rate,
seconds-interval is the interval of seconds during which the samples will be taken,
and file-name is the name of the file where the samples will be saved.
