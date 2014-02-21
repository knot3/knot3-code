#Knot3 [![Build Status](https://travis-ci.org/pse-knot/knot3-code.png?branch=master)](https://travis-ci.org/pse-knot/knot3-code)

Bei Knot3 handelt es sich um ein innovatives Spiel bei dem man Knoten im dreidimensionalem Raum entweder frei modifizieren oder nach Vorgabe auf Zeit ineinander überführen kann.

##Installation

###Debian / Ubuntu / SteamOS (binary)

The official debian repository of Knot3 is compatible with the following distributions:

  * Debian 8.0 (jessie) or later
  * Ubuntu 13.10 (saucy) or later
  * any other debian derivate that has packages for SDL2 (`libsdl2-2.0-0`, `libsdl2-mixer-2.0-0`, `libsdl2-image-2.0-0`),

You need to include it in your sources.list file to install Knot3:

    echo deb http://www.knot3.de debian/ | sudo tee /etc/apt/sources.list.d/knot3
    sudo apt-get update
    sudo apt-get install knot3

###Debian / Ubuntu / SteamOS (source)

If you are using one of the following distributions:

  * Debian 8.0 (jessie) or later
  * Ubuntu 13.10 (saucy) or later
  * any other debian derivate that has packages for SDL2 (`libsdl2-2.0-0`, `libsdl2-mixer-2.0-0`, `libsdl2-image-2.0-0`),

then run the following command to install all build and runtime dependencies:

    make dep-ubuntu

Otherwise, for example if you are using Ubuntu 12.04 LTS, run this to install the backported SDL2 packages:

    make dep-ubuntu-precise

To build and install the game, run:

    make
    sudo make install

###Other Linux

You need to have Mono (3.0+) and SDL (2.0+) and xbuild installed. Once installed,
simply run:

    make
    sudo make install

To run the unit tests, run:

    make test

You can also open the solution file "Knot3-MonoGame.sln" in MonoDevelop.

###Windows (MonoGame)

Install the latest version of [Xamarin Studio](http://monodevelop.com/download) and download the
[MonoGame 3.0.1](http://monogame.codeplex.com/downloads/get/632972) plugin.

Then replace the DLL files in the following directory with their newer versions (MonoGame 3.1.2), which can be found in the `lib/MonoGame-Windows-3.1.2.zip` file in the repository.

    C:\Users\[...]\AppData\Local\XamarinStudio-4.0\LocalInstall\Addins\MonoDevelop.MonoGame.3.0.1\assemblies\WindowsGL

Then, open the solution file "Knot3-MonoGame.sln" in Xamarin Studio to build the game.

##Authors

* [Tobias Schulz](https://github.com/tobiasschulz)
* [Gerd Augsburg](https://github.com/Balduro)
* [Maximilian Reuter](https://github.com/Maximilian-Reuter)
* [Pascal Knodel](https://github.com/pse)
* [Christina Erler](https://github.com/Sakurachan4)
* [Daniel Warzel](https://github.com/wudi0910)

Knot3 is the work of students at [Karlsruhe Institute of Technology](http://www.kit.edu)
in collaborative work with M. Retzlaff, F. Kalka, G. Hoffmann, T. Schmidt.

