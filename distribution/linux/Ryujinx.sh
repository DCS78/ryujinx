#!/bin/sh

SCRIPT_DIR=$(dirname "$(realpath "$0")")

if [ -f "$SCRIPT_DIR/Ryujinx.Headless.SDL2" ]; then
    RYUJINX_BIN="Ryujinx.Headless.SDL2"
fi

if [ -f "$SCRIPT_DIR/Ryujinx" ]; then
    RYUJINX_BIN="Ryujinx"
fi

if [ -z "$RYUJINX_BIN" ]; then
    exit 1
fi

COMMAND="env LANG=C.UTF-8 DOTNET_EnableAlternateStackCheck=1"

XFT_DPI=$(xrdb -get Xft.dpi 2> /dev/null)

if [ -z "$XFT_DPI" ]; then
    XFT_DPI=96
fi

AVALONIA_GLOBAL_SCALE_FACTOR=$(echo "scale=2; $XFT_DPI/96" | bc)

if [ -n "$AVALONIA_GLOBAL_SCALE_FACTOR" ]; then
    COMMAND="$COMMAND AVALONIA_GLOBAL_SCALE_FACTOR=$AVALONIA_GLOBAL_SCALE_FACTOR"
fi

if command -v gamemoderun > /dev/null 2>&1; then
    COMMAND="$COMMAND gamemoderun"
fi

exec $COMMAND "$SCRIPT_DIR/$RYUJINX_BIN" "$@"
