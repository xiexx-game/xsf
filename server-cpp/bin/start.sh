#!/bin/bash

TAG=$1
SERVER=$2         # 需要开启的服务器
P=$3
KT=$4

ulimit -c unlimited

SoCheck()
{
    srcso=../lib/$1
    destso=./$1

    if [ ! -h "$destso" ]; then
        ln -s $srcso $destso
    fi
}

dirs=(
    center
    gate
    game
)

for i in ${dirs[*]}
do
    cd ./$i
    SoCheck libxsf.so
    SoCheck libjemalloc.so.2
    SoCheck libmessage.so
    SoCheck libprotobuf.so.21
    SoCheck libschema.so

    if [ "$i" = "gate" ]
    then
        SoCheck libcenter_c.so
    elif [ "$i" = "game" ]
    then
        SoCheck libcenter_c.so
        SoCheck libgate_a.so
    fi

    cd ../
done

echo "check done, start server ....."


TAG=$1

sh ./single_start.sh $TAG center 1000

exit 0
