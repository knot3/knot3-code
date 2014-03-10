X = xbuild
CP = cp -v
CPR = cp -rvf
RM = rm -vf
RMR = rm -rvf
MKDIR = mkdir -p
UNZIP = unzip
BINDIR = $(DESTDIR)/usr/games
GAMEDIR = $(DESTDIR)/usr/share/knot3
DOCDIR = $(DESTDIR)/usr/share/doc/knot3
NAME = knot3
SOLUTION=Knot3-MG.sln

CODE_DIR = Game
FRAMEWORK_DIR = Framework
UNITTESTS_DIR = UnitTests
TOOL_MODELEDITOR_DIR = Tools/ModelEditor
TOOL_CONFIGRESET_DIR = Tools/ConfigReset
VISUALTESTS_DIR = VisualTests
CONTENT_DIR = Content
STD_KNOT_DIR = Game/Standard_Knots

LIB_DIR = lib
#LIB_MG_LINUX = $(LIB_DIR)/MonoGame-Linux-3.1.2.zip
#LIB_MG_LINUX = $(LIB_DIR)/MonoGame-Linux-SDL2.zip
LIB_MG_WINDOWS = $(LIB_DIR)/Dependencies-Windows.zip
DOC_PDF = doc/Spielanleitung.pdf

.PHONY: project_code

project_code: all

clean: distclean

all: build

build: clean
	xbuild $(SOLUTION)
	$(CPR) $(LIB_DIR)/*.dll.config $(CODE_DIR)/bin/Debug/
	$(CPR) $(LIB_DIR)/*.dll.config $(TOOL_MODELEDITOR_DIR)/bin/Debug/
	$(CPR) $(LIB_DIR)/*.dll.config $(UNITTESTS_DIR)/bin/Debug/

install: build
	$(MKDIR) $(BINDIR)
	install --mode=755 knot3.sh $(BINDIR)/$(NAME)
	$(MKDIR) $(GAMEDIR)
	$(CPR) $(FRAMEWORK_DIR)/bin/Debug/* $(GAMEDIR)/
	$(CPR) $(TOOL_MODELEDITOR_DIR)/bin/Debug/* $(GAMEDIR)/
	$(CPR) $(CODE_DIR)/bin/Debug/* $(GAMEDIR)/
	$(CPR) $(STD_KNOT_DIR) $(GAMEDIR)/
	$(CP) LICENSE $(GAMEDIR)/
	$(CP) debian/changelog $(GAMEDIR)/CHANGELOG
	$(MKDIR) $(DOCDIR)
	$(CP) $(DOC_PDF) $(DOCDIR)/

uninstall:
	$(RM) $(BINDIR)/$(NAME)
	$(RMR) $(GAMEDIR)/

distclean:
	$(RMR) $(CODE_DIR)/bin
	$(RMR) $(CODE_DIR)/obj
	$(RMR) $(FRAMEWORK_DIR)/bin
	$(RMR) $(FRAMEWORK_DIR)/obj
	$(RMR) $(UNITTESTS_DIR)/bin
	$(RMR) $(UNITTESTS_DIR)/obj
	$(RMR) $(TOOL_MODELEDITOR_DIR)/bin
	$(RMR) $(TOOL_MODELEDITOR_DIR)/obj
	git clean -xdf || true

test: build
	cd $(UNITTESTS_DIR)
	nunit-console $(UNITTESTS_DIR)/bin/Debug/Knot3.UnitTests.dll

build-windows: clean
	$(MKDIR) $(CODE_DIR)/bin/Release/ || true
	$(UNZIP) -o -d $(CODE_DIR)/bin/Release/ $(LIB_MG_WINDOWS) || true
	$(MKDIR) $(FRAMEWORK_DIR)/bin/Release/ || true
	$(UNZIP) -o -d $(FRAMEWORK_DIR)/bin/Release/ $(LIB_MG_WINDOWS) || true
	$(MKDIR) $(UNITTESTS_DIR)/bin/Release/ || true
	$(UNZIP) -o -d $(UNITTESTS_DIR)/bin/Release/ $(LIB_MG_WINDOWS) || true
	$(MKDIR) $(TOOL_MODELEDITOR_DIR)/bin/Release/ || true
	$(UNZIP) -o -d $(TOOL_MODELEDITOR_DIR)/bin/Release/ $(LIB_MG_WINDOWS) || true
	$(MKDIR) $(VISUALTESTS_DIR)/bin/Release/ || true
	$(UNZIP) -o -d $(VISUALTESTS_DIR)/bin/Release/ $(LIB_MG_WINDOWS) || true
	xbuild /p:Configuration=Release $(SOLUTION)

package-windows: build-windows
	$(MKDIR) $(DESTDIR)
	$(CPR) $(TOOL_CONFIGRESET_DIR)/bin/Release/ConfigReset* $(DESTDIR)
	$(CPR) $(STD_KNOT_DIR) $(DESTDIR)
	$(MKDIR) $(DESTDIR)/Content
	$(CPR) $(CONTENT_DIR)/* $(DESTDIR)/Content/
	$(CPR) $(FRAMEWORK_DIR)/bin/Release/* $(DESTDIR)/
	$(CPR) $(TOOL_MODELEDITOR_DIR)/bin/Release/* $(DESTDIR)/
	$(CPR) $(VISUALTESTS_DIR)/bin/Release/* $(DESTDIR)/
	$(CPR) $(CODE_DIR)/bin/Release/* $(DESTDIR)/
	$(CP) LICENSE $(DESTDIR)/
	$(CP) debian/changelog $(DESTDIR)/CHANGELOG
	$(CP) README.md $(DESTDIR)/README
	$(RM) $(DESTDIR)/*.pdb
	$(CP) $(DOC_PDF) $(DESTDIR)/

dep-ubuntu-saucy:
	sudo apt-get -q -y install mono-devel mono-dmcs nunit-console libopenal1
	sudo apt-get -q -y install libsdl2-2.0-0 libsdl2-mixer-2.0-0 libsdl2-image-2.0-0

dep-ubuntu-precise:
	wget `curl http://de.archive.ubuntu.com/ubuntu/dists/saucy/main/binary-amd64/Packages.gz http://de.archive.ubuntu.com/ubuntu/dists/saucy/universe/binary-amd64/Packages.gz 2>/dev/null | zcat | grep Filename | egrep 'libsdl2|libflac8|libfluidsynth1|libmad0|libmodplug1|libtiff5|libwebp4|libjack-jackd2-0|libsamplerate0|libjbig0|liblzma5' | grep -v -- -dbg | grep -v -- -dev | sed 's@Filename: @http://de.archive.ubuntu.com/ubuntu/@gm'` ; \
	sudo dpkg -i *.deb ; \
	sudo apt-get -q -y -f install #libsdl2-image-2.0-0 libsdl2-mixer-2.0-0
	sudo apt-get -q -y install mono-devel mono-dmcs nunit-console libopenal1

dep-ubuntu: dep-ubuntu-saucy

dep: dep-ubuntu
