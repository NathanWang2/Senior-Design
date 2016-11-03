import socket
import time


TCP_IP = '192.168.1.22'
TCP_PORT = 80
BUFFER_SIZE = 1024
MESSAGE = "temp"

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((TCP_IP, TCP_PORT))

#s.connect((TCP_IP, TCP_PORT))
s.send(MESSAGE)
temp = ''
while 1:
    data = s.recv(BUFFER_SIZE)
    if not data: break
    temp += data
    print "received data:", data

print("Temperature", temp)
time.sleep(2)

s.close()