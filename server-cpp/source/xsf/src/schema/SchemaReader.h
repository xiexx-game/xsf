//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/src/schema/SchemaReader.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：配置读取器
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _CSV_READER_H_
#define _CSV_READER_H_

#include "IXSFSchema.h"
#include "XSFMallocHook.h"
#include "tinyxml2.h"

namespace xsf
{
	class CSVReader : public ISchemaReader
	{
		struct LineData
		{
			vector<string> datas;

			JEMALLOC_HOOK
		};

		using VctLine = vector<LineData *>;

	public:
		CSVReader(void) {}
		~CSVReader(void) 
		{
			XSF_FREE(m_pContent);

			for( LineData * l : m_Lines)
			{
				delete l;
			}
		}

		bool LoadFile( const char * sFilename );

		// 行总数
		uint32 RowCount(void) override { return m_nRowCount; }

		ICSVData * GetData(byte nType, uint32 row, uint32 col) override;

		tinyxml2::XMLElement * GetXmlRoot(void) override { return nullptr; }

	private:
		bool Parse(const char * sFilename);

	private:
		uint32 m_nRowCount = 0;
		uint32 m_nColCount = 0;

		char * m_pContent = nullptr;

		VctLine m_Lines;
	};

	class XMLReader : public ISchemaReader
	{
	public:
		bool LoadFile( const char * sFilename );

		// 行总数
		uint32 RowCount(void) override { return 0; }

		ICSVData * GetData(byte nType, uint32 row, uint32 col) override { return nullptr; }

		tinyxml2::XMLElement * GetXmlRoot(void) override;
	private:
		tinyxml2::XMLDocument m_Doc;
	};
}


#endif      // end of _CSV_READER_H_