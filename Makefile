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
CODE_DIR = src
FRAMEWORK_DIR = framework
TEST_DIR = tests
SOLUTION=Knot3-MG.sln

.PHONY: project_code

project_code: all

clean: distclean

all: build

build: clean
	$(MKDIR) $(CODE_DIR)/bin/Debug/ || true
	$(UNZIP) -o -d $(CODE_DIR)/bin/Debug/ lib/MonoGame-Linux-3.1.2.zip
	$(MKDIR) $(FRAMEWORK_DIR)/bin/Debug/ || true
	$(UNZIP) -o -d $(FRAMEWORK_DIR)/bin/Debug/ lib/MonoGame-Linux-3.1.2.zip
	xbuild $(SOLUTION)

install: build
	$(MKDIR) $(BINDIR)
	install --mode=755 knot3.sh $(BINDIR)/$(NAME)
	$(MKDIR) $(GAMEDIR)
	$(CPR) $(CODE_DIR)/bin/Debug/* $(GAMEDIR)/
	$(CPR) $(CODE_DIR)/Standard_Knots/ $(GAMEDIR)/
	$(CP) LICENSE $(GAMEDIR)/
	$(CP) $(CODE_DIR)/debian/changelog $(GAMEDIR)/CHANGELOG

uninstall:
	$(RM) $(BINDIR)/$(NAME)
	$(RMR) $(GAMEDIR)/

distclean:
	$(RMR) $(CODE_DIR)/bin
	$(RMR) $(FRAMEWORK_DIR)/bin
	git clean -xdf || true

test:
	$(MAKE) -C $(TEST_DIR) test
