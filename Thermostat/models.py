import sys
import time
import json
import socket
#from ComfortControlGui import onPi
onPi = True

if (onPi): import RPi.GPIO as GPIO

from PyQt4 import QtGui, QtCore
from functools import partial

if (onPi): GPIO.setmode(GPIO.BOARD)
VENT_OPEN = 3.6
VENT_CLOSED = 7.0
VENT_TRANSITION_TIME = 0.4

class Vent:
    pin = None
    pwm = None
    isOpen = None
    probe_ip = None

    def __init__(self, pin, probe_ip = None):
        self.pin = pin
        self.probe_ip = probe_ip
        if (onPi): GPIO.setup(pin, GPIO.OUT)
        if (onPi): self.pwm = GPIO.PWM(pin, 50)
        if (onPi): self.pwm.start(VENT_OPEN)
        time.sleep(VENT_TRANSITION_TIME)
        if (onPi): self.pwm.start(0)
        self.open_vent()

    def open_vent(self):
        if self.isOpen:
            return

        ''' This is for direct connection to Pi'''
        #if (onPi): self.pwm.ChangeDutyCycle(VENT_OPEN)
        #time.sleep(VENT_TRANSITION_TIME)
        #if (onPi): self.pwm.ChangeDutyCycle(0)

        ''' This is for probe connection through ip '''
        try:
            s_open = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            s_open.connect((self.probe_ip, 80))
            s_open.send("open")
            s_open.close()
        except:
            print("Failed to open vent")

        print("Vent " + str(self.pin) + " open..")
        self.isOpen = True

    def close_vent(self):
        if not self.isOpen:
            return
        ''' This is for direction connection to Pi '''
        # if (onPi): self.pwm.ChangeDutyCycle(VENT_CLOSED)
        # time.sleep(VENT_TRANSITION_TIME)
        # if (onPi): self.pwm.ChangeDutyCycle(0)

        ''' This is for probe connection through ip '''
        try:
            s_close = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            s_close.connect((self.probe_ip, 80))
            s_close.send("close")
            s_close.close()
        except:
            print("Failed to close vent")

        print("Vent " + str(self.pin) + " closed..")
        self.isOpen = False


class Room:
    current_temp = 0
    set_temp = 70
    name = "Test"
    probe_ip = None
    # vents = []
    schedules = []

    def __init__(self, _current_temp=None, _set_temp=None, _name=None, _probe_ip=None):
        self.current_temp = _current_temp
        self.set_temp = _set_temp
        self.name = _name
        self.probe_ip = _probe_ip
        self.vents = []

    def inc_set_temp(self):
        self.set_temp += 1

    def dec_set_temp(self):
        self.set_temp -= 1

    def change_name(self, new_name):
        self.name = new_name

    def to_json(self):
        data = {
            'current_temp': self.current_temp,
            'set_temp': self.set_temp,
            'name': self.name,
            'vents': self.vents,
            'probe_ip': self.probe_ip,
            'schedules': self.schedules
        }
        return json.dumps(data)

    def load_json(self, json):
        data = json.loads(json)
        self.current_temp = data['current_temp']
        self.set_temp = data['set_temp']
        self.name = data['name']
        self.probe_ip = data['probe_ip']
        for vent in data['vents']:
            self.vents.append(vent)
        for schedule in data['schedules']:
            self.schedules.append(schedule)

    def add_vent(self, vent_pin, vent_ip):
        new_vent = Vent(vent_pin, vent_ip)
        self.vents.append(new_vent)

    def add_temp_probe(self, probe):
        self.temperature_probe = probe

    def open_vents(self):
        for vent in self.vents:
            vent.open_vent()

    def close_vents(self):
        for vent in self.vents:
            vent.close_vent()


class Button(QtGui.QPushButton):
    command_style = "background-color: #D6DCE5; color: #333F50; font-weight: bold; border: 0;"

    def __init__(self, text, window, _x, _y, _width=None, _height=None):
        super(Button, self).__init__(text, window)
        if (_width != None and _height != None):
            self.resize(_width, _height)
        self.move(_x, _y)
        self.setStyleSheet("background-color: white; border: 0;")

class Label(QtGui.QLabel):
    def __init__(self, text, window, _x, _y, _width, _height, _fontSize):
        super(Label, self).__init__(window)
        self.setText(text)
        self.resize(_width, _height)
        self.move(_x, _y)
        self.setStyleSheet('color: white')
        #self.setFont(QtGui.QFont("Serif", _fontSize, QtGui.QFont.Bold))

width = 480
height = 320
colorPal = QtGui.QPalette()
colorPal.setColor(QtGui.QPalette.Background, QtCore.Qt.gray)