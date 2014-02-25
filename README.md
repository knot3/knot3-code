#Knot3 [![Build Status](https://travis-ci.org/pse-knot/knot3-code.png?branch=master)](https://travis-ci.org/pse-knot/knot3-code) [![Stories in Ready](https://badge.waffle.io/pse-knot/knot3-code.png?label=ready)](https://waffle.io/pse-knot/knot3-code)

Bei Knot3 handelt es sich um ein innovatives Spiel bei dem man Knoten im dreidimensionalem Raum entweder frei modifizieren oder nach Vorgabe auf Zeit ineinander überführen kann.

##Installation

###Debian 8.0+ / Ubuntu 13.10+ / SteamOS

Recent versions of Debian and its derivates contain packages for SDL 2.0. If you are using one of the following distributions, you'll probably have SDL2:

  * Debian 8.0 (jessie) or later
  * Ubuntu 13.10 (saucy) or later
  * Linux Mint 16 (petra) or later
  * SteamOS
  * or any other debian derivate that has packages for SDL2 (`libsdl2-2.0-0`, `libsdl2-mixer-2.0-0`, `libsdl2-image-2.0-0`),

####Binary packages

You need to include it in your sources.list file to install Knot3:

    echo deb http://www.knot3.de debian/ | sudo tee /etc/apt/sources.list.d/knot3
    sudo apt-get update
    sudo apt-get install knot3

####Build from source

Run the following command to install all build and runtime dependencies:

    make dep-ubuntu

To build and install the game, run:

    make
    sudo make install

###Debian 7.0 / Ubuntu 12.04

If your distribution doesn't have recent packages of SDL2, for example if you are using Ubuntu 12.04 LTS (precise), run this to install the backported SDL2 packages:

    make dep-ubuntu-precise

Then build and install the game:

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

Install the latest version of [Xamarin Studio](http://monodevelop.com/download). You don't need the MonoGame plugin.
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
