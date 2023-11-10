//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/inc/IXSFSchema.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：配置相关定义
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _I_XSF_SCHEMA_H_
#define _I_XSF_SCHEMA_H_

#include "XSFDef.h"

namespace tinyxml2
{
	class XMLDocument;
	class XMLElement;
}

namespace xsf
{
	// 配置类型
    enum EMSchemaType
    {
        SchemaType_CSV = 1,
        SchemaType_XML = 2,
    };

	#define XML_FIRST_CHILD(_ELE, _NAME)		_ELE->FirstChildElement(_NAME);
	#define XML_NEXT_SIBLING(_ELE)				_ELE->NextSiblingElement();
	#define XML_NEXT_SIBLING_WITH_NAME(_ELE, _NAME)			_ELE->NextSiblingElement(_NAME);

	#define XML_INT_ATTR( _ELE, _NAME, _OUT )	_ELE->QueryIntAttribute( _NAME, &(_OUT) )
	#define XML_UINT_ATTR( _ELE, _NAME, _OUT )	_ELE->QueryUnsignedAttribute( _NAME, &(_OUT) )
	#define XML_UINT64_ATTR( _ELE, _NAME, _OUT )	_ELE->QueryUnsigned64Attribute( _NAME, &(_OUT) )
	#define XML_FLOAT_ATTR( _ELE, _NAME, _OUT ) _ELE->QueryFloatAttribute( _NAME, &(_OUT) )

	#define XML_STRING_ATTR( _ELE, _NAME, _OUT )			\
	{														\
		const char * pStr = _ELE->Attribute(_NAME);			\
		if( pStr != nullptr )								\
			strcpy(_OUT, pStr );							\
        else                                                \
            _OUT[0] = '\0';                                 \
	}                                                       \
    

	#define XML_BOOL_ATTR( _ELE, _NAME, _OUT )	_ELE->QueryBoolAttribute( _NAME, &(_OUT) )

	#define XML_INT( _ELE, _NAME, _OUT)								\
	{																\
		XMLElement* pElement = XML_FIRST_CHILD( _ELE, _NAME );		\
		if(pElement != nullptr)										\
		{															\
			_OUT = pElement->IntText();								\
		}															\
		else														\
		{															\
			XSF_ERROR("element %s not found", _NAME);				\
		}															\
	}																\


	#define XML_UINT( _ELE, _NAME, _OUT)							\
	{																\
		XMLElement* pElement = XML_FIRST_CHILD( _ELE, _NAME );		\
		if(pElement != nullptr)										\
		{															\
			_OUT = pElement->UnsignedText();						\
		}															\
		else														\
		{															\
			XSF_ERROR("element %s not found", _NAME);				\
		}															\
	}																\


	#define XML_BOOL( _ELE, _NAME, _OUT)							\
	{																\
		XMLElement* pElement = XML_FIRST_CHILD( _ELE, _NAME );		\
		if(pElement != nullptr)										\
		{															\
			_OUT = pElement->BoolText();							\
		}															\
		else														\
		{															\
			XSF_ERROR("element %s not found", _NAME);				\
		}															\
	}																\


	#define XML_TEXT( _ELE, _NAME, _OUT)							\
	{																\
		XMLElement* pElement = XML_FIRST_CHILD( _ELE, _NAME );		\
		if(pElement != nullptr)										\
		{															\
			const char * pTxt = pElement->GetText();				\
			strcpy(_OUT, pTxt);										\
		}															\
		else														\
		{															\
			XSF_ERROR("element %s not found", _NAME);				\
		}															\
	}																\


	

	//////////////////////////////////////////////////////////////////////////
	// CSV读取器
	struct ICSVData
	{
		virtual bool Read(uint32 row, uint32 col, const char * sData) = 0;
	};

	struct ISchemaReader
	{
		virtual uint32 RowCount(void) = 0;

		virtual ICSVData * GetData(byte nType, uint32 row, uint32 col) = 0;

		virtual tinyxml2::XMLElement * GetXmlRoot(void) = 0;
	};

	struct ISchema
	{
		virtual ~ISchema() {}

		virtual bool OnSchemaLoad( int32 nSchemaID, ISchemaReader * pReader ) = 0;

		virtual void * Get(uint32 nID1 = 0, uint32 nID2 = 0) = 0;

		virtual void Release(void) { delete this; }
	};

	struct ISchemaHelper
    {
        virtual ICSVData * GetData(byte type) = 0;

        virtual ISchema * GetSchema(uint32 nID) = 0;
    };
}



#endif      // end of _I_XSF_SCHEMA_H_