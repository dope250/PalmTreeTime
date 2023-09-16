#!/bin/bash

if [ -z "$1" ]; then
    echo "Usage: $0 static/logo.svg"
    exit 1
fi
FNAME=$1
if [ ! -f "$FNAME" ]; then
    echo "Error: file $FNAME doesn't exist"
    exit 1
fi

DESTDIR="$(dirname "$FNAME")"
magick -background transparent -define 'icon:auto-resize=16,24,32,64' "$FNAME" "$DESTDIR/icon.ico"
