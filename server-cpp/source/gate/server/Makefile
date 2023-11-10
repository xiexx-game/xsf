
include ../../Common.mk

# 链接库所在的目录
LIBS_LINK_DIR = -L../$(LIB_DIR)		\
				-L../../$(JEMALLOC_DIR)		\
				-L../../$(LUA_DIR)			\
				-L../../$(PB_DIR)			\



# 需要链接的库
LIBS = $(JEMALLOC_LINK) 	\
	   $(PB_LINK)		\
	   $(XSF_LINK) 		\
	   $(SCHEMA_LINK) 	\
	   $(MESSAGE_LINK)	\
	   $(CENTER_C_LINK)	\
	   					\

				

# 编译临时文件目录
LOCAL_OBJ_DIR = ../$(TEMP_DIR)/gate
LOCAL_BIN_DIR = $(BIN_DIR)/gate

# 头文件目录
INC = -I./inc	\
	  -I./src	\
	  -I./src/client	\
	  -I./src/connector	\
	  -I../../$(XSF_INC)	\
	  -I../../$(JEMALLOC_INC)	\
	  -I../../$(PB_INC)			\
	  -I../../$(SCHEMA_INC)		\
	  -I../../$(MESSAGE_INC)	\
	  -I../../$(CENTER_C_INC)	\
	  							\
	  


# 源文件所在目录所有C++或C文件
SRC = $(wildcard ./src/*.cpp)
SRC += $(wildcard ./src/client/*.cpp)
SRC += $(wildcard ./src/connector/*.cpp)

CPPS = $(notdir $(SRC))

# 每一个C++对应的.o文件
OBJS = $(patsubst %.cpp, $(LOCAL_OBJ_DIR)/%.o, $(CPPS))

# 输出文件
OUTPUT = $(LOCAL_BIN_DIR)/xsf-gate

all : pre-build $(OUTPUT)

$(foreach v, $(SRC), $(eval $(call SRC_COMPLIE,$(v), $(notdir $(v)))) )


# 编译最终输出
$(OUTPUT) : $(OBJS)
	$(GCC) $(GCC_FLAGS) $(OBJS) $(WL_RPATH) $(LIBS_LINK_DIR) $(LIBS) -o $@

pre-build :
	@echo "========================= [start build gate server] ========================="
	@mkdir -p $(LOCAL_OBJ_DIR)
	@mkdir -p $(LOCAL_BIN_DIR)

clean :
	rm -rf $(LOCAL_OBJ_DIR)
	rm -rf $(LOCAL_BIN_DIR)/*
	rm -rf $(PCH_OUT)