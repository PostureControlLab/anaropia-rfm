@echo off
mkdir Build
mklink /d /j .\Build\stimuli .\stimuli
mklink /d /j ".\Build\Screen Textures" ".\Screen Textures"
mklink /d /j ".\Build\img" ".\img"
mklink /h .\Build\README.md .\README.md