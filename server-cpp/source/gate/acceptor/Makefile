
include ../../Common.mk

# 编译临时文件目录
LOCAL_OBJ_DIR = ../$(TEMP_DIR)/gate_a

# 头文件目录
INC = -I./inc	\
	  -I./src	\
	  -I../../$(XSF_INC)	\
	  -I../../$(JEMALLOC_INC)	\
	  -I../../$(PB_INC)			\
	  -I../../$(MESSAGE_INC)	\
	  							\



# 源文件所在目录所有C++或C文件
SRC = $(wildcard ./src/*.cpp)

CPPS = $(notdir $(SRC))

# 每一个C++对应的.o文件
OBJS = $(patsubst %.cpp, $(LOCAL_OBJ_DIR)/%.o, $(CPPS))

# 输出文件
OUTPUT = ../$(LIB_DIR)/libgate_a.so

all : pre-build $(OUTPUT)

$(foreach v, $(SRC), $(eval $(call SRC_COMPLIE,$(v), $(notdir $(v)))) )

# 编译最终输出
$(OUTPUT) : $(OBJS)
	$(GCC) $(GCC_FLAGS) $(GCC_SHARED) $(OBJS) -o $@

pre-build :
	@echo "========================= [start build gate acceptor] ========================="
	@mkdir -p $(LOCAL_OBJ_DIR)
	@mkdir -p ../$(LIB_DIR)

clean :
	rm -rf $(LOCAL_OBJ_DIR)
	rm -rf $(OUTPUT)
	rm -rf $(PCH_OUT)