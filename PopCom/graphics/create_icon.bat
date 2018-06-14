@echo off

inkscape.exe -e icon_16x16.png -i icon -w 16 icons.svg
inkscape.exe -e icon_32x32.png -i icon -w 32 icons.svg
inkscape.exe -e icon_64x64.png -i icon -w 64 icons.svg
inkscape.exe -e icon_128x128.png -i icon -w 128 icons.svg

magick convert icon_16x16.png icon_32x32.png icon_64x64.png icon_128x128.png ..\assets\popcom.ico
