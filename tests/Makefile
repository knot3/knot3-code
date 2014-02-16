X = xbuild
CP = cp -v
CPR = cp -rvf
RM = rm -v
RMR = rm -rvf
MKDIR = mkdir -p
UNZIP = unzip
SOLUTION = ../Knot3-MonoGame.sln

all: build

build: clean
	ls /tmp/monogame.zip || wget https://github.com/pse-knot/MonoGame/releases/download/v3.1.2/MonoGame-Linux-20130126.zip -O /tmp/monogame.zip
	mkdir -p bin/Debug/ || true
	unzip -d bin/Debug/ /tmp/monogame.zip || true
	xbuild $(SOLUTION)

test: build
	nunit-console bin/Debug/Knot3.UnitTests.dll

clean: distclean

distclean:
	$(RMR) bin

uninstall:
	$(RM) $(BINDIR)/$(NAME)
	$(RMR) $(GAMEDIR)/

.PHONY: clean distclean uninstall build install all

