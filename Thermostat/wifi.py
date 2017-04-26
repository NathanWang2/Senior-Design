from wifi import Cell, Scheme

def connect(ssid, passkey):
    wifi_list = Cell.all('wlan0')
    wifi_to_connect = None
    for cell in wifi_list:
        if (cell.ssid == ssid):
            wifi_to_connect = cell

    if (wifi_to_connect == None):
        return False

    scheme = Scheme.for_cell('wlan', 'home', wifi_to_connect, passkey)
    scheme.save()
    scheme.activate()
    return True

def setup_probe():

from wifi import Cell, Scheme
import urllib2
from subprocess import call


''' to connect to wifi '''
def connecct(ssid, passkey = None):
    for cell in Cell.all('wlan0'):
        if cell.ssid == ssid:
            print('found {}'.format(ssid))
            connect_cell = cell

    if (passkey == None):
        s = Scheme.for_cell('wlan0', 'home', connect_cell)
    else:
        s = Scheme.for_cell('wlan0', 'home', connect_cell, passkey)
    s.save()
    s.activate()
    s.delete()
    return s

''' to disconnect wifi '''
def disconnect():
    call(['sudo', 'ifdown', 'wlan0'])
    call(['sudo', 'ifup', 'wlan0'])

'''
    need to pass thermostat ip to probe
    (reconnect to home wifi)
    need to set probe up as client, thermostat as server
    probe send its ip to thermostat
    thermostat save ip, return to being client
    probe turns to server

    need seperate versions for vent and temp probe
'''


