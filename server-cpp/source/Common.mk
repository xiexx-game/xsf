
# 编译参数
debug ?= 1
netdebug ?= 0
memdebug ?= 0
coverage ?= 0

# 执行程序输出目录
BIN_DIR = ../../../bin

# 库输出目录
LIB_DIR = ../../bin/lib

# 临时目录
TEMP_DIR = ../../xsf-temp

WL_RPATH = -Wl,--rpath=./
# gcc
ifeq (,$(shell which ccache))
else
	GCC = ccache
endif

GCC += g++ -std=c++11
GCC_SHARED = -fPIC --shared

# 附加编译选项
GCC_FLAGS = -Wall -rdynamic
GLOBAL_DEFINE = 


ifeq ($(debug), 1)
	# 附加编译选项
	GCC_FLAGS += -g
	# 全局宏定义，其他地方需要扩展，请使用 += 扩展
	GLOBAL_DEFINE = -D_XSF_DEBUG_

	ifeq ($(coverage), 1)
		GCC_FLAGS += -fprofile-arcs -ftest-coverage
	endif
else
	GCC_FLAGS += -g

	GLOBAL_DEFINE += -D_XSF_RELEASE_
endif

ifeq ($(netdebug), 1)
	GLOBAL_DEFINE += -D_XSF_NET_DEBUG_
endif

ifeq ($(memdebug), 1)
	GLOBAL_DEFINE += -DMALLOC_DEBUG
endif


# jemalloc
JEMALLOC_INC = 3rd/jemalloc/include
JEMALLOC_DIR = 3rd/jemalloc/lib
JEMALLOC_LINK = -ljemalloc.5.2.1
JEMALLOC_SO = libjemalloc.5.2.1.so
JEMALLOC_SO_LOCAL = libjemalloc.so.2

# xsf
XSF_INC = xsf/inc
XSF_LINK = -lxsf
XSF_LIB = libxsf.so

# schema
SCHEMA_INC = schema/inc
SCHEMA_LINK = -lschema
SCHEMA_LIB = libschema.so

# protobuf
PB_INC = 3rd/protobuf
PB_DIR = 3rd/protobuf/lib
PB_LINK = -lprotobuf3.10.1
PB_SO = libprotobuf3.10.1.so
PB_SO_LOCAL = libprotobuf.so.21

# message
MESSAGE_INC = message/inc
MESSAGE_LINK = -lmessage
MESSAGE_LIB = libmessage.so

# center connector
CENTER_C_INC = center/connector/inc
CENTER_C_LINK = -lcenter_c
CENTER_C_LIB = libcenter_c.so

# gate acceptor
GATE_A_INC = gate/acceptor/inc
GATE_A_LINK = -lgate_a
GATE_A_LIB = libgate_a.so

# bin dir
BIN_CENTER = bin/center
BIN_DB = bin/db
BIN_GATE = bin/gate
BIN_HUB = bin/hub


# 编译过程定义
define SRC_COMPLIE
 $(patsubst %.cpp, $(LOCAL_OBJ_DIR)/%.o, $(2)) : $(1)
	$$(GCC) $$(GLOBAL_DEFINE) $$(LOCAL_DEFINE) $$(GCC_FLAGS) $$(INC) $$(GCC_SHARED) $$(PCH_INC) -c -o $$@ $$<
endef

