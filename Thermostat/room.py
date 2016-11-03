import sys
import time
import json
import socket
import threading
onPi = False
if (onPi): import RPi.GPIO as GPIO

from PyQt4 import QtGui, QtCore
from functools import partial

if (onPi): GPIO.setmode(GPIO.BOARD)
VENT_OPEN = 11.5
VENT_CLOSED = 3

class Vent(object):
    pin = None
    pwm = None

    def __init__(self, pin):
        self.pin = pin
        if (onPi): GPIO.setup(pin, GPIO.OUT)
        if (onPi): self.pwm = GPIO.PWM(pin, 50)
        if (onPi): self.pwm.start(0)

    def open_vent(self):
        if (onPi): self.pwm.ChangeDutyCycle(VENT_OPEN)
        else: print("Vent open..")
        time.sleep(0.3)
        if (onPi): self.pwm.ChangeDutyCycle(0)

    def close_vent(self):
        if (onPi): self.pwm.ChangeDutyCycle(VENT_CLOSED)
        else: print("Vent closed..")
        time.sleep(0.3)
        if (onPi): self.pwm.ChangeDutyCycle(0)
        

class Room(object):
    current_temp = 0
    set_temp = 70
    name = "Test"
    probe_ip = None
    vents = []
    schedules = []

    def __init__(self, _current_temp = None, _set_temp = None, _name = None, _probe_ip = None):
        self.current_temp = _current_temp
        self.set_temp = _set_temp
        self.name = _name
        self.probe_ip = _probe_ip

    def inc_set_temp(self):
        self.set_temp += 1

    def dec_set_temp(self):
        self.set_temp -= 1

    def change_name(self, new_name):
        self.name = new_name

    def to_json(self):
        data = {
            'current_temp':self.current_temp,
            'set_temp':self.set_temp,
            'name':self.name,
            'vents':self.vents,
            'probe_ip':self.probe_ip,
            'schedules':self.schedules
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

    def add_vent(self, vent):
        self.vents.append(vent)

    def add_temp_probe(self, probe):
        self.temperature_probe = probe

    def open_vents(self):
        for vent in self.vents:
            vent.open_vent()

    def close_vents(self):
        for vent in self.vents:
            vent.close_vent()

class Button(QtGui.QPushButton):
    def __init__(self, text, window, _x, _y, _width = None, _height = None):
        super(Button, self).__init__(text, window)
        if (_width != None and _height != None):
            self.resize(_width, _height)
        self.move(_x, _y)

class Label(QtGui.QLabel):
    def __init__(self, text, window, _x, _y, _width, _height, _fontSize):
        super(Label, self).__init__(window)
        self.setText(text)
        self.resize(_width, _height)
        self.move(_x, _y)
        self.setStyleSheet('color: white')
        self.setFont(QtGui.QFont("Serif", _fontSize, QtGui.QFont.Bold))


class Window(QtGui.QMainWindow):
    def __init__(self, _title, _nextWindow=None):
        super(Window, self).__init__()
        self.setWindowTitle(_title)
        self.nextWindow = _nextWindow

        self.refresh_window(self)

        if (_title != "main"):
            homeBtn = Button("HOME", self, 0, 0)
            homeBtn.clicked.connect(self.go_home)

    def close_application(self):
        print("custom close")
        sys.exit()

    def change_windows(self):
        self.hide()
        self.refresh_window(self.nextWindow)
        self.nextWindow.show()

    def refresh_window(self, window):
        window.setGeometry(xPos, yPos, width, height)
        window.setPalette(colorPal)

    def go_home(self):
        self.nextWindow = MainW
        self.change_windows()

class MainWindow(Window):
    def setup(self):
        # btn = QtGui.QPushButton("Quit", self)
        # btn.clicked.connect(self.close_application)
        # btn.resize(100, 100)
        # btn.move(0, 0)

        # btn2 = QtGui.QPushButton("Settings", self)
        # btn2.clicked.connect(self.goto_settings)
        # btn2.resize(100, 100)
        # btn2.move(200, 0)

        start_y = 60
        for i in range(len(rooms)):
            print(i)
            btn = Button("Room " + rooms[i].name + " - " + str(rooms[i].current_temp),
                         self, 250, start_y, 200, 60)
            this_room = rooms[i]
            btn.clicked.connect( partial(self.goto_room, room = rooms[i]) )
            start_y += 60
        return

    def show_window(self):
        self.show()

    def goto_settings(self):
        self.nextWindow = SettingsW
        self.change_windows()

    def goto_room(self, room):
        self.nextWindow = RoomW
        RoomW.update_room(room)
        self.change_windows()

class SettingsWindow(Window):
    red = False
    maximized = False

    def setup(self):
        btn = QtGui.QPushButton("Change color", self)
        btn.clicked.connect(self.change_color)
        btn.resize(100, 100)
        btn.move(100, 0)

        btn3 = QtGui.QPushButton("Change size", self)
        btn3.clicked.connect(self.change_size)
        btn3.resize(100, 100)
        btn3.move(200, 0)

    def show_window(self):
        self.show()

    def change_color(self):
        whitePal = QtGui.QPalette()
        whitePal.setColor(QtGui.QPalette.Background, QtCore.Qt.white)

        redPal = QtGui.QPalette()
        redPal.setColor(QtGui.QPalette.Background, QtCore.Qt.red)

        if self.red:
            # colorPal = whitePal
            colorPal.setColor(QtGui.QPalette.Background, QtCore.Qt.white)
            self.setPalette(colorPal)
            self.red = False
        else:
            # colorPal = redPal
            colorPal.setColor(QtGui.QPalette.Background, QtCore.Qt.red)
            self.setPalette(colorPal)
            self.red = True
        return

    def change_size(self):
        global width, height
        if self.maximized:
            width = 480
            height = 320
            self.setGeometry(xPos, yPos, width, height)
            self.maximized = False
        else:
            width = 960
            height = 640
            self.setGeometry(xPos, yPos, width, height)
            self.maximized = True

class RoomWindow(Window):
    current_room = Room(70, 70, "Placeholder")
    roomLabel = None
    actTemp = None
    setTemp = None

    def setup(self):

        self.roomLabel = Label(self.current_room.name, self, 20, 20, 240, 50, 12)
        self.actTemp = Label(str(self.current_room.current_temp), self, 30, 30, 240, 240, 70)


        btn3 = Button("/ Up \\", self, 240, 20, 100, 100)
        btn3.clicked.connect(self.inc_temp)

        btn4 = Button("\\ Down /", self, 240, 130, 100, 100)
        btn4.clicked.connect(self.dec_temp)

        # setTempLabel = Label("Set Temp", self, 370, 200, 30, 55, 10)
        self.setTemp = Label(str(self.current_room.set_temp), self, 370, 80, 100, 100, 30)

    def inc_temp(self):
        self.current_room.inc_set_temp()
        self.setTemp.setText(str(self.current_room.set_temp))
        self.current_room.open_vents()

    def dec_temp(self):
        self.current_room.dec_set_temp()
        self.setTemp.setText(str(self.current_room.set_temp))
        self.current_room.close_vents()

    def update_room(self, room):
        self.current_room = room
        self.roomLabel.setText(self.current_room.name)
        self.actTemp.setText(str(self.current_room.current_temp))
        self.setTemp.setText(str(self.current_room.set_temp))

class ScheduleWindow(Window):
    def setup(self):
        return

xPos = 100
yPos = 100
width = 480
height = 320
colorPal = QtGui.QPalette()
colorPal.setColor(QtGui.QPalette.Background, QtCore.Qt.gray)

app = QtGui.QApplication(sys.argv)

MainW = MainWindow("main")
SettingsW = SettingsWindow("settings")
RoomW = RoomWindow("room")
ScheduleW = ScheduleWindow("schedule")

rooms = []
rooms.append(Room(1, 1, "Room 1", "192.168.41.44"))
rooms.append(Room(2, 2, "Room 2", "192.168.41.44"))
rooms.append(Room(3, 3, "Room 3", "192.168.41.44"))


def create_room(_current_temp, _set_temp, _name, _vents):
    new_room = Room(_current_temp, _set_temp, _name)
    for vent in _vents:
        new_room.add_vent(vent)
    rooms.append(new_room)

def load_rooms():
    with open('rooms.json', 'r') as fp:
        data = json.load(fp)
        for i in range(len(data)):
            room = json.loads(data[i])
            _current_temp = room["current_temp"]
            _set_temp = room["set_temp"]
            _vents = room["vents"]
            _name = room["name"]
            print(_current_temp, _set_temp, _name)
            #rooms.append(room)

def save_rooms():
    room_data = []
    for room in rooms:
        room_data.append(room.to_json())
    with open('rooms.json', 'w') as fp:
        json.dump(room_data, fp)

def get_tempF_from_probe(probe_ip):
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.connect(probe_ip, 80)
    s.send("temp")
    temp = ''
    while 1:
        data = s.recv(1024)
        if not data: break
        temp += data
    s.close()
    temp = float(temp) * 9 / 5 + 32
    return temp

def monitor_system():
    ac_mode = "cool"

    while(True):
        rooms_to_condition = []
        too_high = []
        too_low = []

        for room in rooms:
            #room.current_temp = get_tempF_from_probe(room.probe_ip)
            if (room.current_temp > room.set_temp + 2):
                too_high.append(room)
            elif (room.current_temp < room.set_temp - 2):
                too_low.append(room)

        if (len(too_high) > 0 and len(too_low) > 0):
            # notify user that rooms need cool AND heat
            pass
        elif (len(too_high) == 0 and len(too_low) == 0):
            print("Rooms are conditioned..")

        # decide which mode to use
        if (len(too_high) > len(too_low)):
            # need to cool
            ac_mode = "cool"
            rooms_to_condition = too_high
        elif (len(too_low) > len(too_high)):
            # need to heat
            ac_mode = "heat"
            rooms_to_condition = too_low
        else:
            if (ac_mode == "cool"):
                rooms_to_condition = too_high
            elif (ac_mode == "heat"):
                rooms_to_condition = too_low

        if (len(rooms_to_condition) > (len(rooms) / 3)):
            # condition rooms
            pass

        if (len(rooms_to_condition) > 0):
            print("Rooms to " + ac_mode)
            for room in rooms_to_condition:
                print room.name,
            print
        time.sleep(5)

# motor pin 7

save_rooms()
#load_rooms()

MainW.setup()
RoomW.setup()
SettingsW.setup()
ScheduleW.setup()
MainW.show()

t = threading.Thread(target=monitor_system)
t.daemon = True
t.start()

sys.exit(app.exec_())


