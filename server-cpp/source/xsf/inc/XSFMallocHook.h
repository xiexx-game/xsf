//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/inc/XSFMallocHook.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：jemalloc内存分配相关
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _XSF_MALLOC_HOOK_H_
#define _XSF_MALLOC_HOOK_H_

#include "jemalloc.h"
#include "XSFDef.h"
#include <unordered_map>
#include <vector>
#include <list>
#include <queue>

/*
    对于需要new和delete的class/struct，可以在申明中添加如下宏，示例如下：

    struct Data
	{
		int nNum;
        char szStr[128];
		
		JEMALLOC_HOOK
	};
*/

namespace xsf
{
    #ifdef _XSF_DEBUG_
        #define JEMALLOC_HOOK
        #define xsf_malloc      malloc
        #define xsf_calloc      calloc
        #define xsf_free        free
    #else
        #define JEMALLOC_HOOK                                                       \
            static void* operator new(size_t size)                                  \
            {                                                                       \
                return je_malloc(size);                                             \
            }		                                                                \
            static void operator delete(void * ptr)	                                \
            {                                                                       \
                je_free(ptr);                                                       \
            }                                                                       \
            static void* operator new[](size_t size)	                            \
            {                                                                       \
                return je_malloc(size);                                             \
            }                                                                       \
            static void operator delete[](void *ptr)	                            \
            {                                                                       \
                je_free(ptr);                                                       \
            }		                                                                \

        
        #define xsf_malloc      je_malloc
        #define xsf_calloc      je_calloc
        #define xsf_free        je_free
    #endif   
}



#endif      // end of _XSF_MALLOC_HOOK_H_