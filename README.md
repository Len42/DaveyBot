# DaveyBot

_A software/hardware system that plays "Rock Band", by Len Popp_

## Overview

See [http://www.lmp.dyndns.org/daveybot/daveybot.html](http://www.lmp.dyndns.org/daveybot/daveybot.html) for general information about DaveyBot.

**Please note** that this project is many years old and is not compatible with current versions of Xbox, Windows, Arduino, etc.

## DaveyBot Software

The DaveyBot program is written in C#. Its source code is in the "DaveyBotApp" directory.

### Software Requirements

The DaveyBot software uses the .NET Framework version 2.0 and DirectX 9.0 or later. It also uses [the DirectShowNET library](http://directshownet.sourceforge.net/). It requires a video input device that is compatible with DirectX (DirectShow).

### Running DaveyBot

*   Connect the Xbox 360‘s composite video output to the computer‘s composite video input. Make sure the Xbox 360‘s video output is switched to "TV" (not "HDTV").
*   Connect the hacked Guitar Hero controller to the computer with a USB cable. (Note: You can‘t do this unless you hacked your Guitar Hero controller like I did!)
*   Run DaveyBot.exe.
*   If the DaveyBot window isn‘t showing the Xbox 360 video, click "Setup" and select the correct video device and video input.
*   Run "Rock Band" or "Rock Band 2" on the Xbox 360.
*   Start playing a song with the guitar or bass. (Currently only a single player is supported.)
*   Just before the song starts, click "Play Game" in the DaveyBot window.
*   As guitar notes are detected and played, coloured blocks will flash in the DaveyBot window. (The notes will be played, if you have your hacked guitar controller hooked up.)

Other features:

*   The "Grab Frames" button will capture about 10 seconds of video images and save them in separate bitmap files. The files will be saved in a directory called "DaveyBot" in your "Pictures" or "My Pictures" directory.
*   The "Check Frame" button will read and analyze one of these saved bitmaps and display the results in a dialog.
*   The "Analyze" button will read and analyze all of the saved images in a selected directory, and save the results in a text file.

### Video Capture

The DaveyBot software requires a video input device that can receive the composite video from an Xbox 360 and works with DirectShow (which is part of DirectX). DirectShow devices can be very complicated, so DaveyBot may not work with all types of video capture hardware. It has been tested with a couple of devices: a simple video input and a TV tuner.

DaveyBot is currently designed to work with video images that are 720 by 480 pixels in size. Different video inputs may have different resolutions, which will probably not work unless the software is modified.

C# class: Eyes

### Note Detection

Each frame of the video stream is analyzed to find any notes coming down the screen and decide when they should be played. Different note detection algorithms may be selected in the Setup dialog. These algorithms are implemented in subclasses of the abstract class NoteFinder.

Notes are detected a short time before they are to be played. There are two reasons for this: It allows flexibility for the software to avoid graphic elements that obscure the notes, and it allows the software to allow for time lag introduced in the video capture stack, which can be substantial.

Currently, the note detection algorithms are based on a fixed image bitmap size (720 by 480 pixels, with the actual image inset slightly in the bitmap). The image size will probably be different if different video input hardware is used, which will cause note detection to fail.

C# classes: Brain, NoteFinder and subclasses

### Playing Notes

After notes are detected, commands are sent via USB to the hacked guitar controller. The controller interface hardware appears as a serial comm port in Windows. Each command is a single byte whose bits indicate which buttons are to be pressed on the guitar.

Since notes are detected some time before they are played, the notes are queued up until it‘s time to play them. A different thread de-queues the notes and plays them.

C# class: Fingers

## Microcontroller Software

The DaveyBot software on the computer interfaces with an Arduino-compatible microcontroller embedded in the Xbox 360 Guitar Hero controller. The microcontroller receives commands from the computer and simulates button presses on the guitar.

The source code for the microcontroller software is in the "Arduino" directory. It was written using the Arduino programming environment.

## Hardware Interface

The schematic design for the hardware interface in the Guitar Hero controller is in the "Schematic" directory. It is in Kicad format, as well as an image file.