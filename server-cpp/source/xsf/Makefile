
include ../Common.mk

# 编译临时文件目录
LOCAL_OBJ_DIR = $(TEMP_DIR)/xsf

# 头文件目录
INC = -I./inc	\
	  -I./src	\
	  -I./src/logger	\
	  -I./src/net	\
	  -I./src/schema	\
	  -I./src/timer	\
	  -I./src/server	\
	  -I../$(JEMALLOC_INC)
	  

LOCAL_DEFINE = 

# 源文件所在目录所有C++或C文件
SRC = $(wildcard ./src/*.cpp)
SRC += $(wildcard ./src/logger/*.cpp)
SRC += $(wildcard ./src/net/*.cpp)
SRC += $(wildcard ./src/net_point/*.cpp)
SRC += $(wildcard ./src/schema/*.cpp)
SRC += $(wildcard ./src/timer/*.cpp)
SRC += $(wildcard ./src/server/*.cpp)

CPPS = $(notdir $(SRC))

# 每一个C++对应的.o文件
OBJS = $(patsubst %.cpp, $(LOCAL_OBJ_DIR)/%.o, $(CPPS))

# 输出文件
OUTPUT = $(LIB_DIR)/$(XSF_LIB)

all : $(OUTPUT)

$(foreach v, $(SRC), $(eval $(call SRC_COMPLIE,$(v), $(notdir $(v)))) )


# 编译最终输出
$(OUTPUT) : $(OBJS)
	$(GCC) $(GCC_FLAGS) $(GCC_SHARED) $(OBJS) -o $@

pre-build :
	@echo "========================= [start build xsf] ========================="
	@mkdir -p $(LOCAL_OBJ_DIR)
	@mkdir -p $(LIB_DIR)

clean :
	rm -rf $(LOCAL_OBJ_DIR)
	rm -rf $(OUTPUT)