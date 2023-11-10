
include Common.mk

all : pre-build server

.PHONY : xsf schema msg center gate gatea game

server : xsf schema msg center cc gate gatea game

clean : cxsf cschema cmsg ccc ccenter cgate cgatea
	rm -rf ../bin/lib

pre-build:
	mkdir -p ../bin/log
	mkdir -p ../$(BIN_CENTER)
	mkdir -p ../$(BIN_GATE)
	mkdir -p ../bin/lib
	cp -rf ./$(JEMALLOC_DIR)/$(JEMALLOC_SO) ../bin/lib/$(JEMALLOC_SO_LOCAL)
	cp -rf ./$(PB_DIR)/$(PB_SO) ../bin/lib/$(PB_SO_LOCAL)

	
MAKE = make -j$(shell grep -c ^processor /proc/cpuinfo)
	

#########################################################
# xsf 库编译
xsf :
	$(MAKE) -C ./xsf pre-build
	$(MAKE) -C ./xsf

cxsf :
	$(MAKE) -C ./xsf clean

#########################################################
# schema 库编译
schema :
	$(MAKE) -C ./schema pre-build
	$(MAKE) -C ./schema

cschema :
	$(MAKE) -C ./schema clean

#########################################################
# message 库编译
msg :
	$(MAKE) -C ./message pre-build
	$(MAKE) -C ./message

cmsg :
	$(MAKE) -C ./message clean

#########################################################
# center connector
cc :
	$(MAKE) -C ./center/connector pre-build
	$(MAKE) -C ./center/connector

ccc :
	$(MAKE) -C ./center/connector clean


# center 服务器
center :
	$(MAKE) -C ./center/server pre-build
	$(MAKE) -C ./center/server

ccenter :
	$(MAKE) -C ./center/server clean


#########################################################
# gate 服务器
gate :
	$(MAKE) -C ./gate/server pre-build
	$(MAKE) -C ./gate/server

cgate :
	$(MAKE) -C ./gate/server clean

# gate acceptor
gatea :
	$(MAKE) -C ./gate/acceptor pre-build
	$(MAKE) -C ./gate/acceptor

cgatea :
	$(MAKE) -C ./gate/acceptor clean

#########################################################
###### game
game :
	$(MAKE) -C ./game/server pre-build
	$(MAKE) -C ./game/server

cgame :
	$(MAKE) -C ./game/server clean


#########################################################
copy :
	mkdir -p ~/XSFServer
	rm -rf ~/XSFServer/*
	cp -rf ../bin/* ~/XSFServer/