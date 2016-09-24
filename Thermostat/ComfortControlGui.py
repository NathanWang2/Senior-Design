import sys
import time
from PyQt4 import QtGui, QtCore

class Room():
	current_temp = 0
	set_temp = 0
	name = "Test"

	def __init__(self, _current_temp, _set_temp, _name):
		self.current_temp = _current_temp
		self.set_temp = _set_temp
		self.name = _name

	def inc_set_temp(self):
		self.set_temp += 1

	def dec_set_temp(self):
		self.set_temp -= 1

	def change_name(self, newName):
		self.name = newName

class Button(QtGui.QPushButton):
	
	def __init__(self, text, window, _x, _y, _width, _height):
		super(Button, self).__init__(text, window)
		self.resize(_width, _height)
		self.move(_x, _y)		

class Window(QtGui.QMainWindow):
	
	def __init__(self, _title, _nextWindow = None):
		super(Window, self).__init__()
		self.setWindowTitle(_title)
		self.nextWindow = _nextWindow

		if (_title != "home"):
			homeButton = Button("Home", self, 0, 0, 50, 20)
			homeButton.clicked.connect(self.goto_home)

		self.refresh_window(self)

	def close_application(self):
		print("custom close")
		sys.exit()

	def change_windows(self):
		self.hide()
		self.refresh_window(self.nextWindow)
		self.nextWindow.show()

	def refresh_window(self, window):
		window.setGeometry(0, 0, width, height)
		window.setPalette(colorPal)

	def goto_home(self):
		self.nextWindow = Home
		self.change_windows()

class MainWindow(Window):
    
    	def home(self):
		btn = QtGui.QPushButton("Quit", self)
		btn.clicked.connect(self.close_application)
		btn.resize(100, 100)
		btn.move(0, 0)

		btn2 = QtGui.QPushButton("Settings", self)
		btn2.clicked.connect(self.goto_settings)
		btn2.resize(100, 100)
		btn2.move(200, 0)

		btn3 = Button("Room", self, 100, 0, 100, 100)
		btn3.clicked.connect(self.goto_room)

	def show_window(self):
		self.show()

	def goto_settings(self):
		self.nextWindow = Settings
		self.change_windows()

	def goto_room(self):
		self.nextWindow = Room
		self.change_windows()

class SettingsWindow(Window):
	red = False
	maximized = False

	def home(self):
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
		whitePal.setColor(QtGui.QPalette.Background,QtCore.Qt.white)

		redPal = QtGui.QPalette()
		redPal.setColor(QtGui.QPalette.Background,QtCore.Qt.red)

		if (self.red):
			#colorPal = whitePal
			colorPal.setColor(QtGui.QPalette.Background,QtCore.Qt.white)
			self.setPalette(colorPal)
			self.red = False
		else:
			#colorPal = redPal
			colorPal.setColor(QtGui.QPalette.Background,QtCore.Qt.red)
			self.setPalette(colorPal)
			self.red = True
		return

	def change_size(self):
		global width, height
		if (self.maximized):
			width = 480
			height = 320
			self.setGeometry(0, 0, width, height)
			self.maximized = False
		else:
			width = 960
			height = 640
			self.setGeometry(0, 0, width, height)
			self.maximized = True

class RoomWindow(Window):
	name = "Test"
	current_temp = 0
	set_temp = 0

	def home(self):	
		btn = Button("Schedules", self, 100, 0, 100, 100)
		self.nextWindow = Schedule
		btn.clicked.connect(self.change_windows)
		return	

class ScheduleWindow(Window):
	
	def home(self):
		return

width = 480
height = 320
colorPal = QtGui.QPalette()
colorPal.setColor(QtGui.QPalette.Background,QtCore.Qt.white)

app = QtGui.QApplication(sys.argv)

Home = MainWindow("home")
Settings = SettingsWindow("settings")
Room = RoomWindow("room")
Schedule = ScheduleWindow("schedule")

for each in [Home, Settings, Room, Schedule]:
	each.home()

def run():
    	Home.show()
	sys.exit(app.exec_())

run()
