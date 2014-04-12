#!/bin/bash

#### Deploy

sudo apt-get -qq -y install jq

export TAG=$(git describe --abbrev=0 --tags )
export URL=$(curl "https://api.github.com/repos/knot3/knot3-code/releases" | jq '.[] | select(.tag_name == "'"$TAG"'") | .upload_url' | sed 's%"\([^"{?]*\).*"%\1%')

cd ../deploy/

if [ "$TRAVIS_PULL_REQUEST" = "false" ]; then
	for base in *.{zip,tar.gz,deb,rpm}
	do
	        echo $base | egrep '[*]' || (
		        echo Upload $base
		        export DOWN_URL=https://github.com/knot3/knot3-code/releases/download/$TAG/$base
		        wget -O /dev/null $DOWN_URL \
		        || (
		                echo "$URL?name=$base"
		                curl -u "$GITHUB_USER:$GITHUB_PASSWORD" -H "name: $base" -H "Content-Type: application/gzip" --data-binary "@$base" "$URL?name=$base"
		                sleep 3
		        )
		        wget -O /dev/null $DOWN_URL \
		        && echo success: $base
		        sleep 2
        	)
	done
fi

#echo $GITHUB_USER | wc -c
#echo $GITHUB_PASSWORD | wc -c

