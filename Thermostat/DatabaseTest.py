from __future__ import print_function
import mysql.connector
from mysql.connector import errorcode

# Establishes a connection with database
try:
    conn  =  mysql.connector.connect(
        user='sql9144471',
        password='gAj17T1zmr',
        port = '3306',
        host='sql9.freemysqlhosting.net',
        database='sql9144471');
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

# Check and creates table
try:
    TABLES = {}
    TABLES['homeinfo'] = (
    "CREATE TABLE `homeinfo` ("
    "  `roomName` varchar(150) NOT NULL,"
    "  `SetTemp` int(3) NOT NULL,"
    "  `RoomTemp` int(3) NOT NULL,"
    "  `VentStatus` varchar(6) NOT NULL,"
    "  `HeatCoolOff` varchar(4) NOT NULL,"
    "  PRIMARY KEY (`roomName`)"
    ") ENGINE=InnoDB")
    cursor.execute(TABLES['homeinfo'])
except mysql.connector.Error as err:
    print("Table already Created")


# Creates a new Room Entry
def NewRoom(cursor,insert_RoomName):
    
    try:
        add_RoomName = ("INSERT INTO homeinfo (roomName) VALUES ('"+insert_RoomName+"')")
        cursor.execute(add_RoomName)
        conn.commit()
        print ('CORRECTLY UPDATED TABLE ENTRY')
    except:
        print('Did not write to table')

# Writes to Room Temp by which Room Name is Selected
def UpdateRoomTemp(cursor,realTemp,current_roomName):
    try:
        add_RoomTemp = ("UPDATE homeinfo SET RoomTemp = %s WHERE roomName = %s")
        cursor.execute(add_RoomTemp,(realTemp,current_roomName))
        conn.commit()
        print ("Updated Room Temp")
    except:
        print ('Did not update roomtemp')


# Reads all room names and prints them out
def RoomNameList(cursor):
    try:
        read_RoomName = ("SELECT roomName FROM homeinfo" )
        cursor.execute(read_RoomName)
        for roomName in cursor:
            for fixTuple in roomName:
                print (fixTuple)
        cursor.close()
    except:
        print ("No Room Name List")

# Write current Room Temp to Database
def UpdateSetTemp(cursor, setTemp,current_roomName):
    try:
        update_SetTemp = ("UPDATE homeinfo SET SetTemp = %s WHERE roomName = %s")
        cursor.execute(update_SetTemp,(setTemp,current_roomName))
        conn.commit()
        print('Updated Set Temp')
    except:
        print ('Did not update Room Set Temp')
UpdateSetTemp(cursor, 72, 'function')
# Closes Connection
conn.close()
