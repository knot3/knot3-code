X = xbuild
CP = cp -v
CPR = cp -rvf
RM = rm -v
RMR = rm -rvf
MKDIR = mkdir -p
UNZIP = unzip
BINDIR = $(DESTDIR)/usr/games
GAMEDIR = $(DESTDIR)/usr/share/knot3
NAME = knot3
SOLUTION=Knot3-MG.sln

CODE_DIR = Game
FRAMEWORK_DIR = Framework
TEST_DIR = UnitTests
TOOL_MODELEDITOR_DIR = Tools/ModelEditor
TOOL_CONFIGRESET_DIR = Tools/ConfigReset

LIB_DIR = lib
LIB_MG_LINUX = $(LIB_DIR)/MonoGame-Linux-3.1.2.zip
LIB_MG_WINDOWS = $(LIB_DIR)/MonoGame-Windows-3.1.2.zip

.PHONY: project_code

project_code: all

clean: distclean

all: build

build: clean
	$(MKDIR) $(CODE_DIR)/bin/Debug/ || true
	$(UNZIP) -o -d $(CODE_DIR)/bin/Debug/ $(LIB_MG_LINUX)
	$(MKDIR) $(FRAMEWORK_DIR)/bin/Debug/ || true
	$(UNZIP) -o -d $(FRAMEWORK_DIR)/bin/Debug/ $(LIB_MG_LINUX)
	$(MKDIR) $(TEST_DIR)/bin/Debug/ || true
	$(UNZIP) -o -d $(TEST_DIR)/bin/Debug/ $(LIB_MG_LINUX)
	$(MKDIR) $(TOOL_MODELEDITOR_DIR)/bin/Debug/ || true
	$(UNZIP) -o -d $(TOOL_MODELEDITOR_DIR)/bin/Debug/ $(LIB_MG_LINUX)
	xbuild $(SOLUTION)

install: build
	$(MKDIR) $(BINDIR)
	install --mode=755 knot3.sh $(BINDIR)/$(NAME)
	$(MKDIR) $(GAMEDIR)
	$(CPR) $(FRAMEWORK_DIR)/bin/Debug/* $(GAMEDIR)/
	$(CPR) $(TOOL_MODELEDITOR_DIR)/bin/Debug/* $(GAMEDIR)/
	$(CPR) $(CODE_DIR)/bin/Debug/* $(GAMEDIR)/
	$(CPR) $(CODE_DIR)/Standard_Knots/ $(GAMEDIR)/
	$(CP) LICENSE $(GAMEDIR)/
	$(CP) debian/changelog $(GAMEDIR)/CHANGELOG

uninstall:
	$(RM) $(BINDIR)/$(NAME)
	$(RMR) $(GAMEDIR)/

distclean:
	$(RMR) $(CODE_DIR)/bin
	$(RMR) $(CODE_DIR)/obj
	$(RMR) $(FRAMEWORK_DIR)/bin
	$(RMR) $(FRAMEWORK_DIR)/obj
	$(RMR) $(TEST_DIR)/bin
	$(RMR) $(TEST_DIR)/obj
	$(RMR) $(TOOL_MODELEDITOR_DIR)/bin
	$(RMR) $(TOOL_MODELEDITOR_DIR)/obj
	git clean -xdf || true

test: build
	cd $(TEST_DIR)
	nunit-console $(TEST_DIR)/bin/Debug/Knot3.UnitTests.dll

build-windows: clean
	$(MKDIR) $(CODE_DIR)/bin/Release/ || true
	$(UNZIP) -o -d $(CODE_DIR)/bin/Release/ $(LIB_MG_WINDOWS)
	$(MKDIR) $(FRAMEWORK_DIR)/bin/Release/ || true
	$(UNZIP) -o -d $(FRAMEWORK_DIR)/bin/Release/ $(LIB_MG_WINDOWS)
	$(MKDIR) $(TEST_DIR)/bin/Release/ || true
	$(UNZIP) -o -d $(TEST_DIR)/bin/Release/ $(LIB_MG_WINDOWS)
	$(MKDIR) $(TOOL_MODELEDITOR_DIR)/bin/Release/ || true
	$(UNZIP) -o -d $(TOOL_MODELEDITOR_DIR)/bin/Release/ $(LIB_MG_WINDOWS)
	xbuild /p:Configuration=Release $(SOLUTION)

package-windows: build-windows
	$(MKDIR) $(DESTDIR)
	$(CPR) $(TOOL_CONFIGRESET_DIR)/bin/Release/ConfigReset* $(DESTDIR)
	$(CPR) $(CODE_DIR)/Standard_Knots/ $(CODE_DIR)/Content/ $(DESTDIR)
	$(CPR) $(FRAMEWORK_DIR)/bin/Release/* $(DESTDIR)/
	$(CPR) $(TOOL_MODELEDITOR_DIR)/bin/Release/* $(DESTDIR)/
	$(CPR) $(CODE_DIR)/bin/Release/* $(DESTDIR)/
	$(CP) LICENSE $(DESTDIR)/
	$(CP) debian/changelog $(DESTDIR)/CHANGELOG
	$(CP) README.md $(DESTDIR)/README
