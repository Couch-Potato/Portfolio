import pigpio
from time import sleep
import numpy as np
import depthai as dai
import math
import random

INVERT = False

# Optional. If set (True), the ColorCamera is downscaled from 1080p to 720p.
# Otherwise (False), the aligned depth is automatically upscaled to 1080p
downscaleColor = True
fps = 30
# The disparity is computed at this resolution, then upscaled to RGB resolution
monoResolution = dai.MonoCameraProperties.SensorResolution.THE_720_P

# Create pipeline
pipeline = dai.Pipeline()
queueNames = []

canvas = np.zeros((800, 1280, 3), dtype="uint8")


# Define sources and outputs
# camRgb = pipeline.create(dai.node.ColorCamera)
left = pipeline.create(dai.node.MonoCamera)
right = pipeline.create(dai.node.MonoCamera)
stereo = pipeline.create(dai.node.StereoDepth)

dc = math.cos(0.785398)

# rgbOut = pipeline.create(dai.node.XLinkOut)
depthOut = pipeline.create(dai.node.XLinkOut)

def takeAvgOfArea(arr, x1, y1, width, height):
    total = 0
    for y in range(height):
        zy = y + y1
        if (zy >= len(arr)):
            break
        for x in range(width):
            zx = x + x1
            if (zx >= len(arr[zy])):
                break
            total+=arr[zy][zx]
    return total / (width * height)


# rgbOut.setStreamName("rgb")
# queueNames.append("rgb")
depthOut.setStreamName("depth")
queueNames.append("depth")

# #Properties
# camRgb.setBoardSocket(dai.CameraBoardSocket.RGB)
# camRgb.setResolution(dai.ColorCameraProperties.SensorResolution.THE_1080_P)
# camRgb.setFps(fps)
# if downscaleColor: camRgb.setIspScale(2, 3)
# # For now, RGB needs fixed focus to properly align with depth.
# # This value was used during calibration
# camRgb.initialControl.setManualFocus(130)

left.setResolution(monoResolution)
left.setBoardSocket(dai.CameraBoardSocket.LEFT)
left.setFps(fps)
right.setResolution(monoResolution)
right.setBoardSocket(dai.CameraBoardSocket.RIGHT)
right.setFps(fps)

stereo.initialConfig.setConfidenceThreshold(230)
stereo.initialConfig.setMedianFilter(dai.MedianFilter.KERNEL_7x7)
# LR-check is required for depth alignment
stereo.setLeftRightCheck(True)
stereo.setDepthAlign(dai.CameraBoardSocket.RGB)

# Linking
# camRgb.isp.link(rgbOut.input)
left.out.link(stereo.left)
right.out.link(stereo.right)
stereo.disparity.link(depthOut.input)

def createPiSet(hostName):
    pi = pigpio.pi(hostName)
    device1 = pi.i2c_open(1, 32)
    device2 = pi.i2c_open(3, 32)
    device3 = pi.i2c_open(5, 32)
    if (hostName == "gpiocli7.local"):
        device4 = pi.i2c_open(6, 33)
    else:
        device4 = pi.i2c_open(6, 32)
    device5 = pi.i2c_open(7, 32)
    device6 = pi.i2c_open(9, 32)
    device7 = pi.i2c_open(10, 32)
    deviceSet = [
        device1,
        device2,
        device3,
        device4,
        device5,
        device6,
        device7
    ]
    return deviceSet

# _pis = [
#     createPiSet("gpiocli1.local"),
#     createPiSet("gpiocli2.local"),
#     createPiSet("gpiocli3.local"),
#     createPiSet("gpiocli4.local")
# ]

_pis = [
    createPiSet("gpiocli1.local"),
    createPiSet("gpiocli2.local"),
    createPiSet("gpiocli3.local"),
    createPiSet("gpiocli4.local"),
    createPiSet("gpiocli5.local"),
    createPiSet("gpiocli6.local"),
    createPiSet("gpiocli7.local"),
    createPiSet("gpiocli8.local")
]

_pnx = [
    pigpio.pi("gpiocli1.local"),
    pigpio.pi("gpiocli2.local"),
    pigpio.pi("gpiocli3.local"),
    pigpio.pi("gpiocli4.local"),
    pigpio.pi("gpiocli5.local"),
    pigpio.pi("gpiocli6.local"),
    pigpio.pi("gpiocli7.local"),
    pigpio.pi("gpiocli8.local")
]

# _pnx = [
#     pigpio.pi("gpiocli1.local"),
#     pigpio.pi("gpiocli2.local"),
#     pigpio.pi("gpiocli3.local"),
#     pigpio.pi("gpiocli4.local")
# ]

currentFrameBuffer = [True] * 484


def GetDecimalFromSocket(in1, in2, in3, in4, in5, in6, in7, in8):
    decimal = 0
    decimal = decimal * 2 + (int(in1 == True))
    decimal = decimal * 2 + (int(in2 == True))
    decimal = decimal * 2 + (int(in3 == True))
    decimal = decimal * 2 + (int(in4 == True))
    decimal = decimal * 2 + (int(in5 == True))
    decimal = decimal * 2 + (int(in6 == True))
    decimal = decimal * 2 + (int(in7 == True))
    decimal = decimal * 2 + (int(in8 == True))
    return decimal

def decimalToBinary(n):
    return bin(n).replace("0b", "").zfill(8)

def GetSocketSetFromDecimal(input):
    binary = decimalToBinary(input)
    datum = []
    # if()
    for i in range(8):
        # print(binary)
        # print(i)
        datum.append(binary[i] == "1")
    return datum

def Clear():
    for x in range(len(_pis)):
        if _pnx[x] == None:
            continue
        pi = _pnx[x]
        deviceSet = _pis[x]
        for device in range(len(deviceSet)):
            try:
                pi.i2c_write_byte(deviceSet[device], 255)
                # sleep(.1)
            except:
                print("Write failed. DEVICE::" + str(x) + " I2C::" + str(device))

def Clear2():
    for x in range(224):
        WriteBulb(x, False)
        sleep(.01)

def Flash():
    for x in range(len(_pis)):
        pi = _pnx[x]
        deviceSet = _pis[x]
        for device in range(len(deviceSet)):
            try:
                pi.i2c_write_byte(deviceSet[device], 255)
            except:
                print("Write failed. DEVICE::" + str(x) + " I2C::" + str(device))


def WriteXY(x,y, val):
    # print("(" + str(x) + "," +str(y) +")")
    if x < 11 and x >=0 and y >=0 and y < 22:
        newx = 0
        if y%2 != 0:
            newx = x
        else:
            newx = 10-x
        area_below = (22-y-1)*11
        if (area_below + newx < 224):
            currentFrameBuffer[int(area_below + newx)] = False
    if x >= 10 and y >= 0 and y< 22:
        relX = x - 10
        relY = y
        newx = 0
        absx = 0
        if y%2 != 0:
            newx = 10-relX
        else:
            newx = relX
            absx = 0
        area_below = ((22-relY-1)*11)+226 + absx
        if (area_below > 10*11 + 226):
            area_below = area_below + 2
        if (area_below + newx < 448):
            currentFrameBuffer[int(area_below + newx)] = False
            # WriteBulb(int(area_below + newx), val)

lastFrameBuffer_s = [True] * 484

def SwapBuffers():
    for i in range(len(currentFrameBuffer)):
        if lastFrameBuffer_s[i] != currentFrameBuffer[i]:
            WriteBulb(i, currentFrameBuffer[i])
            lastFrameBuffer_s[i] = currentFrameBuffer[i]
        currentFrameBuffer[i] = True
    # lastFrameBuffer_s = currentFrameBuffer

def WriteBulb(_id, val):
    id = _id % 56
    pi_id = int((_id - id)/56)
    cadre_id = id % 8
    set_id = int((id - cadre_id) / 8)
    pi = _pnx[pi_id]
    if (pi == None):
        return
    deviceSet = _pis[pi_id]
    try:
        current_set = GetSocketSetFromDecimal(pi.i2c_read_byte(deviceSet[set_id]))
        current_set[cadre_id] = val
        pi.i2c_write_byte(deviceSet[set_id], GetDecimalFromSocket(
        current_set[0],
        current_set[1],
        current_set[2],
        current_set[3],
        current_set[4],
        current_set[5],
        current_set[6],
        current_set[7],
        ))
    except:
        print("Write failed :(")



def TestPattern9():
    while True:
        Flash()
        Clear()

# TestPattern9()


# WriteXY(0,21, True)

doTestPattern = False

def TestPattern():
    doTestPattern = True
    while doTestPattern:
        # do x itrx
        for x in range(22):
            Clear()
            for (y) in range(22):
                WriteXY(x,y, True)
            SwapBuffers()
            sleep(.1)
        for y in range(22):
            Clear()
            for x in range(22):
                WriteXY(x,y, True)
            SwapBuffers()
            sleep(.1)

def Random():
    while True:
        Clear()
        for x in range(6):
            idx = random.randrange(0, 20)
            idy = random.randrange(0, 20)
            WriteXY(idx, idy, True)
        SwapBuffers()
        sleep(.35)

Random()
# TestPattern()

def TestPattern2():
    while True:
        for x in range(224):
            # for v in range(25):
                
                # sleep(.1)
            WriteBulb(x, False)
            sleep(.1)
            Flash()
        Flash()
        sleep(1)

                

def TestPattern3():
    while True:
        for x in range(21):
            for z in range(50):
                Clear()
                sleep(.01)
                pi = _pnx[int((x - (x % 7)) / 7)]
                deviceSet = _pis[int((x - (x % 7)) / 7)]
                
                pi.i2c_write_byte(deviceSet[int(x % 7)],255)
                # WriteBulb(x, True)
                sleep(.01)

def TestPattern4():
    while True:
        Clear()
        sleep(.10)
        _pnx[0].i2c_write_byte(_pis[0][0], 255)
        _pnx[0].i2c_write_byte(_pis[0][1], 255)
        sleep(.1)

def TestPatternStrobe():
    while True:
        Clear()
        sleep(.30)
        Flash()
        # sleep(.1)

def TestPattern77():
    
    while True:
        for x in range(448):
            sleep(.01)
            Clear()
            # sleep(.5)
            WriteBulb(x, False)
    
            # sleep(.1)
            

# Clear()
# exit()

# TestPattern()
Clear()


#TestPatternStrobe()

# Connect to device and start pipeline
with dai.Device(pipeline, usb2Mode=True) as device:

    # device.getOutputQueue(name="rgb",   maxSize=4, blocking=False)
    device.getOutputQueue(name="depth", maxSize=4, blocking=False)

    frameRgb = None
    frameDepth = None

    while True:
        latestPacket = {}
        latestPacket["rgb"] = None
        latestPacket["depth"] = None

        queueEvents = device.getQueueEvents(("depth"))
        for queueName in queueEvents:
            packets = device.getOutputQueue(queueName).tryGetAll()
            if len(packets) > 0:
                latestPacket[queueName] = packets[-1]

        if latestPacket["rgb"] is not None:
            frameRgb = latestPacket["rgb"].getCvFrame()
            # cv2.imshow("rgb", frameRgb)

        if latestPacket["depth"] is not None:
            frameDepth = latestPacket["depth"].getFrame()
            maxDisparity = 95.0
            # Optional, extend range 0..95 -> 0..255, for a better visualisation
            frameDepth = (frameDepth * 255. / maxDisparity).astype(np.uint8)
            # Optional, apply false colorization
            # if 1: frameDepth = cv2.applyColorMap(frameDepth, cv2.COLORMAP_HOT)
            frameDepth = np.ascontiguousarray(frameDepth)
            canvas = np.zeros((800, 1280, 3), dtype="uint8")
            # print(len(frameDepth[50][5]))
            # Clear()
            otherYWritten = False
            for y in range(len(frameDepth)):
                if (y % 32 == 0):
                    for x in range(len(frameDepth[y])):
                        if (x % 58 == 0):
                            # print(frameDepth[x][y])
                            # if (frameDepth[y][x] > 30):
                            avgx = frameDepth[y][x]
                            newy = y
                            if (avgx > 120):
                                try:
                                    # print(y)
                                    if (y == 608 and otherYWritten):
                                        WriteXY(22-(x / 58), y / 32, False)
                                    elif (y != 608 and y != 0):
                                        WriteXY(22-(x / 58), y / 32, False)
                                        otherYWritten = True
                                except:
                                    break
                                # WriteXY(0,21, True)
                                # cv2.rectangle(canvas, (x, int(newy)), (x+59,(int(newy))+33), (255,255,255), -1)
            # Clear()
            SwapBuffers()
            # cv2.imshow("Canvas", canvas)

# WriteXY(10,21, True)
# WriteXY(0,20, True)
