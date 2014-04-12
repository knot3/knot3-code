#!/bin/bash

#### Package

sudo apt-get -qq -y install build-essential debhelper dh-make quilt fakeroot lintian devscripts alien
make dep-ubuntu-precise

export PATH=/home/pse-knot/perl5/bin:/home/pse-knot/bin:/usr/local/bin:/usr/bin:/bin:/usr/local/games:/usr/games:$PATH
export R=$(pwd)
export DEPLOY_DIR=$R/../deploy

mkdir -p $DEPLOY_DIR

(
	cd $R
	git reset --hard HEAD
	git clean -dxf
	git checkout HEAD --force
	git pull
	git submodule update --init --recursive

	export SUFFIX=""
	if [ "x$TRAVIS_BRANCH" != "x" -a "x$TRAVIS_BRANCH" != "xmaster" ]
	then
		export SUFFIX=${SUFFIX}-$TRAVIS_BRANCH
	fi
	export NAME=knot3${SUFFIX}
	export LONGVERSION=$(git describe --tags | sed 's@git@@gm')
	export VERSION=$(git describe --tags | cut -d- -f1,2 | sed 's@git@@gm')
	export DOTNETVERSION=$(git describe --tags | perl -n -e 's/[^0-9.]+/./m; s/[-].*/.0/gm; @x = split(/\./, $_); $x[2]++; $_=join(".", @x); print')

	cat /tmp/built | grep $VERSION && echo "Already built." && exit 0
	echo $VERSION >> /tmp/built

	cd $R

        cat Game/Properties/AssemblyInfo.cs.template | sed 's@__VERSION__@'$DOTNETVERSION'@gm' > Game/Properties/AssemblyInfo.cs

	cd $R

	sed -i 's@Source: knot3@Source: '$NAME'@gm' debian/control
	sed -i 's@Package: knot3@Package: '$NAME'@gm' debian/control
	scripts/gitlog-to-deblog.rb | sed 's@pse-knot@'$NAME'@gm' | sed 's@knot3-code@'$NAME'@gm' | perl -n -e 's/-g[0-9a-f]+\)/)/gm; print' > debian/changelog
	debuild -b -us -uc
	alien -r ../*.deb
	#export V=$(basename ../*.deb | perl -n -e 'print $1 if (/_(.*?)_/);')

	mv -f ../*.deb *.rpm $DEPLOY_DIR/

	#cd $R/..
	#zip -r $DEPLOY_DIR/debian.zip *.dsc *.tar.gz *.changes
	#rm *.build *.changes *.dsc

	# export V=$(git rev-list HEAD --count)-$(git log --oneline | head -n 1 | cut -d' ' -f1)

	###cd $R
	###make clean
	###make
	###schroot -c precise -- gendarme --html gendarme.html Game/bin/Debug/Knot3.exe ; mv gendarme.html /var/www/knot3.de/development/gendarme.html

	# Windows ZIP
	cd $R
	make clean
	make package-windows DESTDIR=/tmp/${NAME}-git$VERSION
	cd /tmp
	zip -r ${NAME}-git$VERSION.zip ${NAME}-git$VERSION
	mv ${NAME}-*.zip $DEPLOY_DIR/
	rm -rf ${NAME}-git$VERSION

	# Source tarballs
	cd $R
	make clean
	cd /tmp
	rm -rf ${NAME}_$VERSION
	cp -rf $R ${NAME}_$VERSION
	rm -rf ${NAME}_$VERSION/.git
	tar cplSzf ${NAME}_$VERSION.tar.gz ${NAME}_$VERSION
	rm -rf ${NAME}_$VERSION
	mv -f ${NAME}_$VERSION.tar.gz $DEPLOY_DIR/
)

#(
#	cd $DEBIAN_REPO/..
#	dpkg-scanpackages debian /dev/null | gzip -9c > debian/Packages.gz
#	zcat debian/Packages.gz > debian/Packages
#)

rm -f $DEPLOY_DIR/*{_.tar.gz,-git.zip}


