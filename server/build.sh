#!/bin/bash

workpath=`pwd`

BuildServer() {
    echo "start build server $1"
    rm -rf ./bin/$1
    cd $1
    dotnet build
    mv bin/Debug/net6.0/$1 bin/Debug/net6.0/xsf-$1
    mv bin/Debug/net6.0  $workpath/bin/$1
    rm -rf bin
    cd $workpath
}

BuildServer Center
BuildServer Gate
BuildServer DB
BuildServer Hub
BuildServer Game

mkdir -p ~/XSFServer
rm -rf ~/XSFServer/*
cp -rf $workpath/bin/* ~/XSFServer/
