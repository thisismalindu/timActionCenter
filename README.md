# timActionCenter

A customizable alternative Action Center for Windows.

![timActionCenter](https://i.imgur.com/Idvlvep.png)

## About

timActionCenter started from a simple problem I had while using Windows.

I sometimes need to turn my proxy settings on and off. To make this easier, I created a small utility called **ProxyEnabler**, which stayed on top of the screen and allowed me to toggle the proxy with a single click.

That gave me the idea that there are probably many other Windows settings that people would like to access just as quickly, similar to turning Wi-Fi or Bluetooth on and off from the Windows Action Center.

So I decided to build **timActionCenter**: a customizable Action Center where useful Windows settings and actions can be accessed from one place.

The application runs from the **system tray**, similar to tools such as EarTrumpet.

timActionCenter was originally built using **Windows Forms**, but the project was later migrated to **WPF** to provide more control over the interface and make the UI easier to customize.

![timActionCenter Demo](https://i.imgur.com/hsJkG56.gif)

The interface also automatically detects and uses your **Windows system accent color**.

## Features

Currently, timActionCenter can:

* Enable or disable Windows proxy settings
* Switch between Windows Dark and Light themes
* Detect and use the current system accent color
* Run directly from the Windows system tray

### Compatibility

Tested on:

* Windows 10
* Windows 11

## Download

Download the latest version from the [Releases page](https://github.com/thisismalindu/timActionCenter/releases).

## Planned Improvements

* Add more Windows settings and quick actions
* Automatically update the interface when a setting is changed outside timActionCenter
* Add Acrylic or Mica effects to the interface
* Continue improving UI customization

## Status

timActionCenter is still a work in progress.

The project started as a small utility for my own use, and I am continuing to expand it with features that can make commonly used Windows settings faster and easier to access.
