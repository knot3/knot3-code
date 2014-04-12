#Knot3 [![Build Status](https://travis-ci.org/knot3/knot3-code.png?branch=master)](https://travis-ci.org/knot3/knot3-code)

  Bei Knot&sup3; handelt es sich um ein innovatives Spiel, bei dem man Knoten im dreidimensionalem Raum entweder frei modifizieren
oder nach Vorgabe auf Zeit ineinander überführen kann. 

  Das Spiel ist ein Studentenprojekt des Karlsruher Instituts für Technologie (KIT),
das im Rahmen des Kurses [Praxis der Softwareentwicklung](http://cg.ivd.kit.edu/lehre/ws2013/pse/index.php)
im Wintersemester 2013/14 von sechs Studenten des KIT umgesetzt wurde.
Die Idee zu diesem Spiel entstand am [Lehrstuhl für Computergrafik](http://cg.ivd.kit.edu/lehre/ws2013/pse/index.php)
in Zusammenarbeit mit der
[Hochschule für Gestaltung](http://postdigital.hfg-karlsruhe.de/users/greta-luise-hoffmann) in Karlsruhe.

  Das Spiel basiert auf dem [XNA-Framework](http://msdn.microsoft.com/en-us/aa937791.aspx) von Microsoft bzw. auf der freien Implementierung
[MonoGame-SDL2](https://github.com/flibitijibibo/MonoGame).
Es ist für Windows, Linux und Mac OS X verfügbar und wird als nicht-kommerzielle Software unter einer [freien Lizenz](http://knot3.github.io/license.html) zur Verfügung gestellt.

##Installation

This project uses git submodules, so you need to clone it with the --recursive parameter to clone all the dependencies too (otherwise it won't build):

    git clone --recursive https://github.com/knot3/knot3-code.git

Run the following command to update all submodules:

    git submodule update --init --recursive

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

Knot3 is the work of students at [Karlsruhe Institute of Technology](http://www.informatik.kit.edu/)
in collaborative work with [M. Retzlaff](https://cg.ivd.kit.edu/retzlaff/) (<retzlaff@kit.edu>),
[G. Hoffmann](http://postdigital.hfg-karlsruhe.de/users/greta-luise-hoffmann)
and [T. Schmidt](https://cg.ivd.kit.edu/schmidt/index.php) (<thorsten.schmidt@kit.edu>).
In the winter term of 2013 the following students were involved in the development:

| Name              | Assignment                 | GitHub                                                      | Website                        |
| ----------------- | -------------------------- | ----------------------------------------------------------- | ------------------------------ |
| Gerd Augsburg     | Projektleitung             | [@Balduro](https://github.com/Balduro)                      | <http://balduro.de.gg>         |
| Maximilian Reuter | Pflichtenheft              | [@Maximilian-Reuter](https://github.com/Maximilian-Reuter)  |                                |
| Christina Erler   | Entwurfsdokument           | [@Sakurachan4](https://github.com/Sakurachan4)              |                                |
| Tobias Schulz     | Implementierung            | [@tobiasschulz](https://github.com/tobiasschulz)            | <http://www.tobias-schulz.eu>  |
| Pascal Knodel     | Qualitätssicherung         | [@pse](https://github.com/pse)                              |                                |
| Daniel Warzel     | Abschlusspräsentation      | [@wudi0910](https://github.com/wudi0910)                    |                                |

The game is free software; you can redistribute it and/or modify it under the terms of [these licenses](http://knot3.github.io/license.html).

More information about the "Praxis der Softwareentwicklung" course is available at the [Lehrstuhl für Computergrafik](http://cg.ivd.kit.edu/lehre/ws2013/pse/index.php)
of Professor [Dachsbacher](http://cg.ivd.kit.edu/dachsbacher/index.php) and the [Lehrstuhl Programmierparadigmen](http://pp.info.uni-karlsruhe.de/lehre/WS201314/pse/)
of Professor [Snelting](http://pp.info.uni-karlsruhe.de/personhp/gregor_snelting.php).
