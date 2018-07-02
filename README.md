# PopCom

[![Build status](https://ci.appveyor.com/api/projects/status/vxdxfvkm636k0n5o?svg=true)](https://ci.appveyor.com/project/avishorp/popcom)

## Description
PopCom is a COM port plug-in/plug-out notifier. Whenever a USB device that emulates a COM port is connected to the computer, a pop-up will be displayed, describing the device that has been plugged in and the COM number assigned to it. This pop-up helps determinig the COM number assigned to each device, a number that is required for communicating with it.

![Alt text](images/popup.jpg?raw=true "PopCom pop-up")

## Installation
An installer is supplied. The program uses .NET Framework 3.5, which is compatible with Windows 7 and above. On Windows 8 & 10 machines, it may be
required to enable the appropriate .NET feature (automatically offered by Windows upon first launch).

## Building
The program is built using Microsoft Visual Studio 2017. There are no other required dependencies. The installer requires [Wix Toolset](http://wixtoolset.org/)  3.11 to be installed.
