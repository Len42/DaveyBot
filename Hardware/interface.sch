EESchema Schematic File Version 2  date 09/05/2009 21:31:12
LIBS:power,./ILQ1,./Boarduino,device,transistors,conn,linear,regul,74xx,cmos4000,adc-dac,memory,xilinx,special,microcontrollers,dsp,microchip,analog_switches,motorola,texas,intel,audio,interface,digital-audio,philips,display,cypress,siliconi,opto,atmel,contrib,valves,.\interface.cache
EELAYER 24  0
EELAYER END
$Descr A4 11700 8267
Sheet 1 1
Title "DaveyBot Guitar Interface Board"
Date "11 apr 2009"
Rev "4"
Comp "Len Popp"
Comment1 ""
Comment2 ""
Comment3 ""
Comment4 ""
$EndDescr
Wire Wire Line
	4000 3150 4000 2700
Wire Wire Line
	7350 1800 8400 1800
Wire Wire Line
	7350 5550 8350 5550
Wire Wire Line
	7350 5150 8350 5150
Wire Wire Line
	7350 1900 8400 1900
Wire Wire Line
	7350 5750 8350 5750
Wire Wire Line
	7350 5350 8350 5350
Wire Wire Line
	7350 2000 8400 2000
Wire Wire Line
	7350 1600 8400 1600
Wire Wire Line
	5850 5650 5500 5650
Wire Wire Line
	5500 5650 5500 6100
Wire Wire Line
	5850 5250 5000 5250
Wire Wire Line
	5000 5250 5000 6100
Connection ~ 5400 2950
Wire Wire Line
	5400 3150 5400 2950
Wire Wire Line
	5850 1600 5000 1600
Wire Wire Line
	5000 1600 5000 2450
Wire Wire Line
	5850 2000 5500 2000
Wire Wire Line
	5500 2000 5500 2450
Wire Wire Line
	5850 5850 3300 5850
Wire Wire Line
	3300 5850 3300 4650
Wire Wire Line
	5850 5450 3100 5450
Wire Wire Line
	3100 5450 3100 4650
Wire Wire Line
	5850 2200 3200 2200
Wire Wire Line
	3200 2200 3200 3150
Wire Wire Line
	5850 1800 3400 1800
Wire Wire Line
	3400 1800 3400 3150
Wire Wire Line
	3500 3150 3500 1500
Wire Wire Line
	3500 1500 5850 1500
Wire Wire Line
	3300 3150 3300 1900
Wire Wire Line
	3300 1900 5850 1900
Wire Wire Line
	3000 4650 3000 5150
Wire Wire Line
	3000 5150 5850 5150
Wire Wire Line
	3200 4650 3200 5550
Wire Wire Line
	3200 5550 5850 5550
Wire Wire Line
	5750 2450 5750 2100
Wire Wire Line
	5750 2100 5850 2100
Wire Wire Line
	5250 2450 5250 1700
Wire Wire Line
	5250 1700 5850 1700
Wire Wire Line
	5000 2950 5750 2950
Wire Wire Line
	3500 4650 3500 4850
Connection ~ 5400 6600
Wire Wire Line
	5400 6800 5400 6600
Wire Wire Line
	5000 6600 5750 6600
Wire Wire Line
	5250 6100 5250 5350
Wire Wire Line
	5250 5350 5850 5350
Wire Wire Line
	5750 6100 5750 5750
Wire Wire Line
	5750 5750 5850 5750
Wire Wire Line
	7350 1700 8400 1700
Wire Wire Line
	7350 2100 8400 2100
Wire Wire Line
	7350 5250 8350 5250
Wire Wire Line
	7350 5650 8350 5650
Wire Wire Line
	7350 1500 8400 1500
Wire Wire Line
	7350 2200 8400 2200
Wire Wire Line
	7350 5450 8350 5450
Wire Wire Line
	7350 5850 8350 5850
Wire Wire Line
	4200 3150 4200 2900
$Comp
L CONN_1 BLUE-~(C2)
U 1 1 49E62AEE
P 8550 2200
F 0 "BLUE- (C2)" H 8630 2200 40  0000 L CNN
F 1 "CONN_1" H 8550 2255 30  0001 C CNN
	1    8550 2200
	1    0    0    -1  
$EndComp
$Comp
L CONN_1 YELLOW-~(C2)
U 1 1 49E62AEA
P 8550 1900
F 0 "YELLOW- (C2)" H 8630 1900 40  0000 L CNN
F 1 "CONN_1" H 8550 1955 30  0001 C CNN
	1    8550 1900
	1    0    0    -1  
$EndComp
$Comp
L CONN_1 RED-~(C2)
U 1 1 49E62AC9
P 8550 1800
F 0 "RED- (C2)" H 8630 1800 40  0000 L CNN
F 1 "CONN_1" H 8550 1855 30  0001 C CNN
	1    8550 1800
	1    0    0    -1  
$EndComp
$Comp
L CONN_1 GREEN-~(C4)
U 1 1 49E62AC3
P 8550 1500
F 0 "GREEN- (C4)" H 8630 1500 40  0000 L CNN
F 1 "CONN_1" H 8550 1555 30  0001 C CNN
	1    8550 1500
	1    0    0    -1  
$EndComp
$Comp
L CONN_1 WHAMMY-
U 1 1 49E62A25
P 8500 5150
F 0 "WHAMMY-" H 8580 5150 40  0000 L CNN
F 1 "CONN_1" H 8500 5205 30  0001 C CNN
	1    8500 5150
	1    0    0    -1  
$EndComp
$Comp
L CONN_1 ODRV-~(BACK-)
U 1 1 49E629DC
P 8500 5450
F 0 "ODRV- (BACK-)" H 8580 5450 40  0000 L CNN
F 1 "CONN_1" H 8500 5505 30  0001 C CNN
	1    8500 5450
	1    0    0    -1  
$EndComp
$Comp
L CONN_1 STRUM-~(C4)
U 1 1 49E629CD
P 8500 5550
F 0 "STRUM- (C4)" H 8580 5550 40  0000 L CNN
F 1 "CONN_1" H 8500 5605 30  0001 C CNN
	1    8500 5550
	1    0    0    -1  
$EndComp
$Comp
L CONN_1 ORANGE-~(C3)
U 1 1 49E629C3
P 8500 5850
F 0 "ORANGE- (C3)" H 8580 5850 40  0000 L CNN
F 1 "CONN_1" H 8500 5905 30  0001 C CNN
	1    8500 5850
	1    0    0    -1  
$EndComp
Text Notes 550  7700 0    60   ~ 0
Copyright 2009 Len Popp CC by-nc-sa
$Comp
L CONN_1 ORANGE+~(LB)
U 1 1 49E0C134
P 8500 5750
F 0 "ORANGE+ (LB)" H 8580 5750 40  0000 L CNN
F 1 "CONN_1" H 8500 5805 30  0001 C CNN
	1    8500 5750
	1    0    0    -1  
$EndComp
$Comp
L CONN_1 STRUM+
U 1 1 49E0C130
P 8500 5650
F 0 "STRUM+" H 8580 5650 40  0000 L CNN
F 1 "CONN_1" H 8500 5705 30  0001 C CNN
	1    8500 5650
	1    0    0    -1  
$EndComp
$Comp
L CONN_1 ODRV+~(BACK+)
U 1 1 49E0C126
P 8500 5350
F 0 "ODRV+ (BACK+)" H 8580 5350 40  0000 L CNN
F 1 "CONN_1" H 8500 5405 30  0001 C CNN
	1    8500 5350
	1    0    0    -1  
$EndComp
$Comp
L CONN_1 WHAMMY+
U 1 1 49E0C122
P 8500 5250
F 0 "WHAMMY+" H 8580 5250 40  0000 L CNN
F 1 "CONN_1" H 8500 5305 30  0001 C CNN
	1    8500 5250
	1    0    0    -1  
$EndComp
$Comp
L CONN_1 BLUE+~(X)
U 1 1 49E0BF16
P 8550 2100
F 0 "BLUE+ (X)" H 8630 2100 40  0000 L CNN
F 1 "CONN_1" H 8550 2155 30  0001 C CNN
	1    8550 2100
	1    0    0    -1  
$EndComp
$Comp
L CONN_1 YELLOW+~(Y)
U 1 1 49E0BF12
P 8550 2000
F 0 "YELLOW+ (Y)" H 8630 2000 40  0000 L CNN
F 1 "CONN_1" H 8550 2055 30  0001 C CNN
	1    8550 2000
	1    0    0    -1  
$EndComp
$Comp
L CONN_1 RED+~(B)
U 1 1 49E0BF07
P 8550 1700
F 0 "RED+ (B)" H 8630 1700 40  0000 L CNN
F 1 "CONN_1" H 8550 1755 30  0001 C CNN
	1    8550 1700
	1    0    0    -1  
$EndComp
$Comp
L CONN_1 GREEN+~(A)
U 1 1 49E0BEE8
P 8550 1600
F 0 "GREEN+ (A)" H 8630 1600 40  0000 L CNN
F 1 "CONN_1" H 8550 1655 30  0001 C CNN
	1    8550 1600
	1    0    0    -1  
$EndComp
$Comp
L SW_PUSH_SMALL SW1
U 1 1 49E0BCCE
P 4100 2800
F 0 "SW1" H 4250 2910 30  0000 C CNN
F 1 "RESET" H 4100 2721 30  0000 C CNN
	1    4100 2800
	1    0    0    -1  
$EndComp
$Comp
L GND #PWR01
U 1 1 49E0BC44
P 5400 6800
F 0 "#PWR01" H 5400 6800 30  0001 C CNN
F 1 "GND" H 5400 6730 30  0001 C CNN
	1    5400 6800
	1    0    0    -1  
$EndComp
$Comp
L R R8
U 1 1 49E0BC43
P 5750 6350
F 0 "R8" V 5830 6350 50  0000 C CNN
F 1 "1k" V 5750 6350 50  0000 C CNN
	1    5750 6350
	1    0    0    -1  
$EndComp
$Comp
L R R7
U 1 1 49E0BC42
P 5500 6350
F 0 "R7" V 5580 6350 50  0000 C CNN
F 1 "1k" V 5500 6350 50  0000 C CNN
	1    5500 6350
	1    0    0    -1  
$EndComp
$Comp
L R R6
U 1 1 49E0BC41
P 5250 6350
F 0 "R6" V 5330 6350 50  0000 C CNN
F 1 "1k" V 5250 6350 50  0000 C CNN
	1    5250 6350
	1    0    0    -1  
$EndComp
$Comp
L R R5
U 1 1 49E0BC40
P 5000 6350
F 0 "R5" V 5080 6350 50  0000 C CNN
F 1 "1k" V 5000 6350 50  0000 C CNN
	1    5000 6350
	1    0    0    -1  
$EndComp
$Comp
L GND #PWR02
U 1 1 49E0B930
P 3500 4850
F 0 "#PWR02" H 3500 4850 30  0001 C CNN
F 1 "GND" H 3500 4780 30  0001 C CNN
	1    3500 4850
	1    0    0    -1  
$EndComp
$Comp
L GND #PWR03
U 1 1 49E0B634
P 5400 3150
F 0 "#PWR03" H 5400 3150 30  0001 C CNN
F 1 "GND" H 5400 3080 30  0001 C CNN
	1    5400 3150
	1    0    0    -1  
$EndComp
$Comp
L R R4
U 1 1 49E0B5FC
P 5750 2700
F 0 "R4" V 5830 2700 50  0000 C CNN
F 1 "1k" V 5750 2700 50  0000 C CNN
	1    5750 2700
	1    0    0    -1  
$EndComp
$Comp
L R R3
U 1 1 49E0B5F9
P 5500 2700
F 0 "R3" V 5580 2700 50  0000 C CNN
F 1 "1k" V 5500 2700 50  0000 C CNN
	1    5500 2700
	1    0    0    -1  
$EndComp
$Comp
L R R2
U 1 1 49E0B5F4
P 5250 2700
F 0 "R2" V 5330 2700 50  0000 C CNN
F 1 "1k" V 5250 2700 50  0000 C CNN
	1    5250 2700
	1    0    0    -1  
$EndComp
$Comp
L R R1
U 1 1 49E0B5CC
P 5000 2700
F 0 "R1" V 5080 2700 50  0000 C CNN
F 1 "1k" V 5000 2700 50  0000 C CNN
	1    5000 2700
	1    0    0    -1  
$EndComp
$Comp
L ILQ1 IC2
U 1 1 49E0AFFC
P 6600 5500
F 0 "IC2" H 6600 5400 50  0000 C CNN
F 1 "ILQ1" H 6600 5600 50  0000 C CNN
	1    6600 5500
	1    0    0    -1  
$EndComp
$Comp
L ILQ1 IC1
U 1 1 49E0AFF5
P 6600 1850
F 0 "IC1" H 6600 1750 50  0000 C CNN
F 1 "ILQ1" H 6600 1950 50  0000 C CNN
	1    6600 1850
	1    0    0    -1  
$EndComp
$Comp
L Boarduino U1
U 1 1 49E0AFD5
P 3550 3900
F 0 "U1" H 3550 3800 50  0000 C CNN
F 1 "Boarduino" H 3550 4000 50  0000 C CNN
	1    3550 3900
	0    1    1    0   
$EndComp
$EndSCHEMATC
