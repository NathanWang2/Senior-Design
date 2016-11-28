from __future__ import print_function
import mysql.connector
from mysql.connector import errorcode
from models import *

# Establishes a connection with database
try:
    conn  =  mysql.connector.connect(
        user='sql7146138',
        password='GFUUmSnIH5',
        port = '3306',
        host='sql7.freemysqlhosting.net',
        database='sql7146138');
    conn.connect()
    cursor = conn.cursor()
    print ("Table Connected")
except mysql.connector.Error as err:
  if err.errno == errorcode.ER_ACCESS_DENIED_ERROR:
    print("Something is wrong with your user name or password")
  elif err.errno == errorcode.ER_BAD_DB_ERROR:
    print("Database does not exist")
  else:
    print(err)

#Check and creates table
# try:
#     TABLES = {}
#     TABLES['homeinfo'] = (
#     "CREATE TABLE `homeinfo` ("
#     "  `roomName` varchar(150) NOT NULL,"
#     "  `setTemp` int(3) NOT NULL,"
#     "  `roomTemp` int(3) NOT NULL,"
#     "  `ventStatus` varchar(6) NOT NULL,"
#     "  `heatCoolOff` varchar(4) NOT NULL,"
#     "  PRIMARY KEY (`roomName`)"
#     ") ENGINE=InnoDB")
#     cursor.execute(TABLES['homeinfo'])
# except mysql.connector.Error as err:
#     print("Table already Created")


# Creates a new Room Entry
def NewRoom(insert_RoomName):
    global cursor
    try:
        conn.connect()
        add_RoomName = ("INSERT INTO homeinfo (roomName) VALUES ('"+insert_RoomName+"')")
        cursor.execute(add_RoomName)
        conn.commit()
        print ('CORRECTLY UPDATED TABLE ENTRY')
    except mysql.connector.Error as err:
        print('Did not write to table')

# Writes to Room Temp by which Room Name is Selected
def UpdateRoomTemp(realTemp,current_roomName):
    try:
        add_RoomTemp = ("UPDATE homeinfo SET roomTemp = %s WHERE roomName = \"%s\"")
        cursor.execute(add_RoomTemp,(realTemp,current_roomName))
        conn.commit()
        print ("Updated Room Temp")
    except:
        print ('Did not update roomtemp')


# Reads all room names and prints them out
def RoomNameList():
    db_rooms = []
    try:
        conn.connect()
        cursor = conn.cursor()
        read_RoomName = ("SELECT roomName FROM homeinfo" )
        cursor.execute(read_RoomName)
        for roomName in cursor:
            for fixTuple in roomName:
                db_rooms.append(fixTuple)
                #print (fixTuple)
        #cursor.close()
    except:
        print ("No Room Name List")
    return db_rooms

def GetSetTemp(roomName):
    try:
        get_setTemp = ("SELECT setTemp FROM homeinfo WHERE roomName = '"+roomName+"'")
        cursor.execute(get_setTemp,(roomName))
        set_temp = cursor.fetchone()[0]
        #cursor.close()
        return set_temp
    except mysql.connector.Error as err:
        print ("Couldn't get set temp for room", roomName)
    except Exception as we:
        print (we)

# Write current Room Temp to Database
def UpdateSetTemp(setTemp,current_roomName):
    try:
        update_SetTemp = ("UPDATE homeinfo SET setTemp = %s WHERE roomName = \"%s\"")
        cursor.execute(update_SetTemp,(setTemp,current_roomName))
        conn.commit()
        print('Updated Set Temp')
    except:
        print ('Did not update Room Set Temp')

# Closes Connection
#conn.close()

def UpdateRoomList(current_rooms):
    updated_rooms = current_rooms
    current_room_names = [r.name for r in current_rooms]
    db_rooms = RoomNameList()
    for db_room in db_rooms:
        if db_room not in current_room_names:
            print ("Adding room :", db_room)
            room_set_temp = GetSetTemp(db_room)
            if (room_set_temp != None):
                updated_rooms.append(Room(None, room_set_temp, db_room))
            else:
                updated_rooms.append(Room(None, None, db_room))


    for current_room in current_rooms:
        if current_room.name not in db_rooms:
            print ("Adding room to db :", current_room.name)
            NewRoom(current_room.name)
            UpdateRoomTemp(current_room.current_temp, current_room.name)
            UpdateSetTemp(current_room.set_temp, current_room.name)
        else:
            current_room.set_temp = GetSetTemp(current_room.name)

    return updated_rooms
