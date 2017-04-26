import socket
import threading
#from DatabaseTest import *
from models import *
import wifi
#import netifaces as ni
import requests

onPi = True
if (onPi): import RPi.GPIO as GPIO

from PyQt4 import QtGui, QtCore
from functools import partial

if (onPi): GPIO.setmode(GPIO.BOARD)
VENT_OPEN = 3.6
VENT_CLOSED = 7.0
VENT_TRANSITION_TIME = 0.4


class Relay:
    HEAT_PIN = 36
    COOL_PIN = 38
    FAN_PIN = 40

    def __init__(self):
        GPIO.setup(self.COOL_PIN, GPIO.OUT)
        GPIO.setup(self.HEAT_PIN, GPIO.OUT)
        GPIO.setup(self.FAN_PIN, GPIO.OUT)

    def turnAllOff(self):
        GPIO.output(self.COOL_PIN, 0)
        GPIO.output(self.HEAT_PIN, 0)
        GPIO.output(self.FAN_PIN, 0)

    def turnCoolOn(self):
        self.turnHeatOff()
        GPIO.output(self.COOL_PIN, 1)

    def turnCoolOff(self):
        GPIO.output(self.COOL_PIN, 0)

    def turnHeatOn(self):
        self.turnCoolOff()
        GPIO.output(self.HEAT_PIN, 1)

    def turnHeatOff(self):
        GPIO.output(self.HEAT_PIN, 0)

    def turnFanOn(self):
        GPIO.output(self.FAN_PIN, 1)

    def turnFanOff(self):
        GPIO.output(self.FAN_PIN, 0)


class Window(QtGui.QMainWindow):
    def __init__(self, _title, _nextWindow=None):
        super(Window, self).__init__()
        self.setWindowTitle(_title)
        self.nextWindow = _nextWindow

        self.refresh_window(self)

        if (_title != "main"):
            homeBtn = Button("HOME", self, 30, 250, 200, 50)
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
        window.setStyleSheet("background-color: #333F50")

    def go_home(self):
        self.nextWindow = MainW
        self.change_windows()

class MainWindow(Window):
    num_room_buttons = 20
    room_buttons = []
    room_page = 0

    def setup(self):
        up_btn = Button("UP", self, 250, 20, 200, 50)
        up_btn.clicked.connect(self.up_room)
        up_btn.setStyleSheet(Button.command_style)

        dn_btn = Button("DOWN", self, 250, 250, 200, 50)
        dn_btn.clicked.connect(self.down_room)
        dn_btn.setStyleSheet(Button.command_style)

        refresh_btn = Button("REFRESH", self, 30, 250, 200, 50)
        refresh_btn.clicked.connect(self.update_rooms)

        #add_room_btn = Button("ADD ROOM", self, 30, 190, 200, 50)
        #add_room_btn.clicked.connect(self.goto_addRoom)

        y_inc = 60
        place_y = 70
        for i in range(self.num_room_buttons): #len(rooms)):
            try:
                btn = Button(rooms[i].name + " - " + str(rooms[i].current_temp),
                         self, 250, place_y, 200, 60)
                btn.clicked.connect(partial(self.goto_room, room=rooms[i]))
                if not (i >= self.room_page * 3 and i < (self.room_page * 3) + 3):
                    self.hide()
            except:
                btn = Button("", self, 250, place_y, 200, 60)
                btn.hide()

            self.room_buttons.append(btn)
            place_y += 60
            if (place_y > 3 * y_inc + 10):
                place_y = 70

    def show_window(self):
        self.show()

    def goto_settings(self):
        self.nextWindow = SettingsW
        self.change_windows()

    def goto_room(self, room):
        self.nextWindow = RoomW
        RoomW.update_to_room(room)
        self.change_windows()

    def goto_addRoom(self):
        self.nextWindow = AddRoomW
        self.change_windows()

    def update_rooms(self):
        global rooms
        for i in range(self.num_room_buttons):
            try:
                self.room_buttons[i].setText(rooms[i].name)
                self.room_buttons[i].clicked.connect(partial(self.goto_room, room=rooms[i]))
                if (i >= self.room_page * 3 and i < (self.room_page * 3) + 3):
                    self.room_buttons[i].show()
                else:
                    self.room_buttons[i].hide()
            except:
                self.room_buttons[i].hide()
        return

    def up_room(self):
        if (self.room_page > 0):
            self.room_page -= 1
        self.update_rooms()

    def down_room(self):
        if (len(rooms) > (self.room_page + 1) * 3):
            self.room_page += 1
        self.update_rooms()


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

class AddRoomWindow(Window):
    numBedrooms = 0
    numBathrooms = 0
    numLivingRooms = 0
    numKitchens = 0
    numZones = 0

    def setup(self):
        bed_btn = Button("Bedroom", self, 30, 30, 200, 50)
        bed_btn.clicked.connect(self.add_bed)

        bath_btn = Button("Bathroom", self, 30, 90, 200, 50)
        bath_btn.clicked.connect(self.add_bath)

        living_btn = Button("Living Room", self, 30, 150, 200, 50)
        living_btn.clicked.connect(self.add_living)

        kitchen_btn = Button("Kitchen", self, 240, 30, 200, 50)
        kitchen_btn.clicked.connect(self.add_kitchen)

        zone_btn = Button("Zone", self, 240, 90, 200, 50)
        zone_btn.clicked.connect(self.add_zone)

    def add_bed(self):
        rooms.append(Room(None, 70, "Bedroom " + str(self.numBedrooms + 1)))
        self.numBedrooms += 1
        self.go_home()

    def add_bath(self):
        rooms.append(Room(None, 70, "Bathroom " + str(self.numBathrooms + 1)))
        self.numBathrooms += 1
        self.go_home()

    def add_living(self):
        rooms.append(Room(None, 70, "Living Room " + str(self.numLivingRooms + 1)))
        self.numLivingRooms += 1
        self.go_home()

    def add_kitchen(self):
        rooms.append(Room(None, 70, "Kitchen " + str(self.numKitchens + 1)))
        self.numKitchens += 1
        self.go_home()

    def add_zone(self):
        rooms.append(Room(None, 70, "Zone " + str(self.numZones + 1)))
        self.numZones += 1
        self.go_home()

    def go_home(self):
        self.nextWindow = MainW
        self.change_windows()

class RoomWindow(Window):
    current_room = Room(70, 70, "Placeholder")
    roomLabel = None
    actTemp = None
    setTemp = None

    def setup(self):
        self.roomLabel = Label(self.current_room.name, self, 20, 20, 240, 50, 12)
        self.roomLabel.setStyleSheet("font-size: 20px; color: white; font: \"Serif\"")
        self.actTemp = Label(str(self.current_room.current_temp), self, 50, 60, 180, 180, 100)
        self.actTemp.setStyleSheet("font-size: 130px; font-weight: bold; color: white; font: \"Serif\"")

        btn3 = Button("/ Up \\", self, 240, 20, 100, 100)
        btn3.clicked.connect(self.inc_temp)

        btn4 = Button("\\ Down /", self, 240, 130, 100, 100)
        btn4.clicked.connect(self.dec_temp)

        # setTempLabel = Label("Set Temp", self, 370, 200, 30, 55, 10)
        self.setTemp = Label(str(self.current_room.set_temp), self, 370, 80, 100, 100, 30)
        self.setTemp.setStyleSheet("font-size: 70px; color: white")

    def inc_temp(self):
        self.current_room.inc_set_temp()
        UpdateSetTemp(self.current_room.set_temp, self.current_room.name)
        self.setTemp.setText(str(self.current_room.set_temp))

    def dec_temp(self):
        self.current_room.dec_set_temp()
        UpdateSetTemp(self.current_room.set_temp, self.current_room.name)
        self.setTemp.setText(str(self.current_room.set_temp))

    def update_to_room(self, room):
        self.current_room = room
        self.update_room()

    def update_room(self):
        self.roomLabel.setText(self.current_room.name)
        if (self.current_room.current_temp != None):
            self.actTemp.setText(str(int(self.current_room.current_temp)))
        if (self.current_room.set_temp != None):
            self.setTemp.setText(str(int(self.current_room.set_temp)))


class ScheduleWindow(Window):
    def setup(self):
        return


if (onPi):
    xPos = 0
    yPos = -1
else:
    xPos = 100  # 0
    yPos = 100  # -1

width = 480
height = 320
# colorPal = QtGui.QPalette()
# colorPal.setColor(QtGui.QPalette.Background, "#333F50")

app = QtGui.QApplication(sys.argv)

BackW = Window("background")
MainW = MainWindow("main")
SettingsW = SettingsWindow("settings")
AddRoomW = AddRoomWindow("addRoom")
RoomW = RoomWindow("room")
ScheduleW = ScheduleWindow("schedule")

rooms = []
print("Starting")
room1 = Room(72, 72, "Room 1", "192.168.43.206") # "192.168.1.6")
print("Created room 1")
room1.add_vent(35, "192.168.43.44") # "192.168.1.5")
print("Added vent to room 1")
rooms.append(room1)
print("Added room to rooms")
room2 = Room(72, 72, "Room 2", "192.168.43.88")
room2.add_vent(37, "192.168.43.217")
rooms.append(room2)

for room in rooms:
    print("Room " + room.name)
    for vent in room.vents:
        print("  vent " + str(vent.pin))

home_ssid = ""
home_pass = ""
home_scheme = None
therm_ip = ""

def connectToHome(ssid = None, passkey = None):
    global home_ssid, pass_key

    if (ssid == None):
        with open('home_wifi.json', 'r') as wf:
            data = json.loads(wf)
    else:
        with open('home_wifi.json', 'w') as wf:
            data = json.dump(wf, {'ssid':ssid, 'passkey':passkey})
    home_ssid = ssid
    home_pass = passkey
    home_scheme = wifi.connect(ssid,passkey)
    #ni.ifaddresses('wlan0')
    #ip = ni.ifaddresses('wlan0')[2][0]['addr']

def connectProbe():

    wifi.connect('CC Temp Probe')
    response = urllib2.urlopen('http://192.168.4.1/wifisave?s={}&p={}'.format(ssid, pass_key))
    home_scheme.save()
    home_scheme.activate()

def makeServerRequest(script, payload = None):
    url = 'https://797862787b.dataplicity.io/'+script+'.php'
    print(url)
    default_timeout = 1.0
    if payload != None:
        r = requests.get(url, params=payload, timeout=default_timeout)
    else:
        r = requests.get(url, timeout=default_timeout)
    return r.json()

def UpdateRoomTemp(realTemp, roomName):
    payload = {'room_name':roomName,
               'room_temp':realTemp}
    response = makeServerRequest('update_room_temp', payload)
    return response['success']

def UpdateSetTemp(setTemp, roomName):
    payload = {'room_name':roomName,
               'set_temp':setTemp}
    response = makeServerRequest('update_set_temp', payload)
    return response['success']

def UpdateRoomList():
    global rooms
    updated_rooms = rooms
    current_room_names = [r.name for r in rooms]
    response = makeServerRequest('get_room')
    if (not response['success']):
        return
    db_rooms = response['data']
    for db_room in db_rooms:
        if db_room['roomName'] not in current_room_names:
            print ("Adding room :" + db_room['roomName'])
            rooms.append(Room(None, int(db_room['setTemp']), db_room['roomName']))
    #return updated_rooms

def GetSetTemp(roomName):
    payload = {'room_name':roomName}
    response = makeServerRequest('get_room', payload)
    if (response['success']):
        return int(response['data'][0]['setTemp'])
    return 70


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
            # rooms.append(room)


def save_rooms():
    room_data = []
    for room in rooms:
        room_data.append(room.to_json())
    with open('rooms.json', 'w') as fp:
        json.dump(room_data, fp)


def get_tempF_from_probe(probe_ip):
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.connect((probe_ip, 80))
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
    global rooms
    ac_mode = "cool"
    fanOn = False

    if (onPi):
        relay = Relay()

    while (True):
        rooms_to_condition = []
        too_high = []
        too_low = []
        try:
            UpdateRoomList()
        except:
            print("Failed to update rooms from database")
        MainW.update_rooms()

        for room in rooms:
            try:
                print("Getting temp for room: " + room.name)
                room.current_temp = get_tempF_from_probe(room.probe_ip)
            except:
                print("Failed to get temp from room: " + room.name)

            try:
                room.set_temp = GetSetTemp(room.name)
                UpdateRoomTemp(room.current_temp, room.name)
            except:
                print("Failed to update room/set temp for room: " + room.name)
                pass

            if (room.current_temp > room.set_temp + 2):
                too_high.append(room)
            elif (room.current_temp < room.set_temp - 2):
                too_low.append(room)
            else:
                room.close_vents()


        RoomW.update_room()
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
            print("Rooms need conditioning")
            if (onPi):
                for room in rooms:
                    if (room in rooms_to_condition):
                        print("Opening vents for " + room.name)
                        room.open_vents()
                    else:
                        print("Closing vents for " + room.name)
                        room.close_vents()

                if (ac_mode == "cool"):
                    relay.turnCoolOn()
                    # relay.turnFanOn()
                    print("COOLING")
                elif (ac_mode == "heat"):
                    relay.turnHeatOn()
                    # relay.turnFanOn()
                    print("HEATING")

                if (fanOn):
                    relay.turnFanOn()
        else:
            if (onPi):
                relay.turnAllOff()
            else:
                pass

        if (len(rooms_to_condition) > 0):
            print("Rooms to " + ac_mode)
            for room in rooms_to_condition:
                print room.name,
            print

        time.sleep(1)


# motor pin 7

# save_rooms()
# load_rooms()

MainW.setup()
AddRoomW.setup()
RoomW.setup()
SettingsW.setup()
ScheduleW.setup()
BackW.show()
MainW.show()

monitor_thread = threading.Thread(target=monitor_system)
monitor_thread.daemon = True
monitor_thread.start()

sys.exit(app.exec_())


