import urllib2
import time
import RPi.GPIO as GPIO

GPIO.setmode(GPIO.BOARD)
GPIO.setup(11, GPIO.OUT)
def internet_on():
    while(True):
        try:
            urllib2.urlopen('192.168.43.1', timeout=1)
            GPIO.output(11, 0)
        except:
            GPIO.output(11, 1)
        time.sleep(1)

internet_on()

