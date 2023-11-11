#!/bin/bash

TAG=$1
ID=$3
NAME=$2

workpath=`pwd`

path=config/${TAG}/log_console
content=$(cat $path)

cd ./$NAME
pwd
ProcName="./xsf-$NAME"

if [[ $content -eq 1 ]]; then
    $ProcName -tag $TAG -i $ID -c -rd $workpath &
else
    $ProcName -tag $TAG -i $ID -rd $workpath &
fi

echo "start ok, tag:${TAG}, name:${NAME}"