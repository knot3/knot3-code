#!/bin/bash

#### Code Format

. /etc/lsb-release
if [ "x$DISTRIB_RELEASE" = "x12.04" ]
then
	wget `curl http://de.archive.ubuntu.com/ubuntu/dists/saucy/main/binary-amd64/Packages.gz http://de.archive.ubuntu.com/ubuntu/dists/saucy/universe/binary-amd64/Packages.gz 2>/dev/null | zcat | grep Filename | egrep 'astyle' | grep -v -- -dbg | grep -v -- -dev | sed 's@Filename: @http://de.archive.ubuntu.com/ubuntu/@gm'` ; \
	sudo dpkg -i *.deb ; \
	sudo apt-get -qq -y -f install
else
	sudo apt-get -qq -y install astyle
fi

( echo "machine github.com"; echo "login $GITHUB_USER"; echo "password $GITHUB_PASSWORD"; ) > ~/.netrc

export LAST_AUTHOR=$(git log --stat | grep Author:  | cut -d" " -f2,3,4,5,6,7,8,9 | grep -vi "PSE Knot" | head -n 1)
if [ "x$TRAVIS_REPO_SLUG" != "x" ]
then
	true #git remote set-url origin https://github.com/${TRAVIS_REPO_SLUG}.git
fi
git reset --hard HEAD
git clean -dxf
git checkout HEAD --force
git pull
git submodule update --init --recursive

find -name Thumbs.db -exec git rm '{}' \;

git clone --depth=5 https://github.com/knot3/CSharpCodeFormat.git ../CSharpCodeFormat

../CSharpCodeFormat/format #>/dev/null 2>&1
scripts/rewrite-csproj.pl

git diff HEAD | grep "...." && (
	export DOTNETVERSION=$(git describe --tags | perl -n -e 's/[^0-9.]+/./m; s/[-].*/.0/gm; @x = split(/\./, $_); $x[2]++; $_=join(".", @x); print')
	cat Game/Properties/AssemblyInfo.cs.template | sed 's@__VERSION__@'$DOTNETVERSION'@gm' > Game/Properties/AssemblyInfo.cs
        git add Game/Properties/AssemblyInfo.cs
)

git commit -a -m "Code Format" --author="$LAST_AUTHOR"
git push origin $(git rev-parse --abbrev-ref HEAD)
