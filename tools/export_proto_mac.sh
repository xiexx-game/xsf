#!/bin/bash


current=$(pwd)
root=$(dirname $current)
echo "root path is $root"

cd bin/Debug/net6.0
dotnet Tools.dll -p mac -root $root

