//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/src/schema/SchemaReader.cpp
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：CSV读取器
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#include "SchemaReader.h"

#include "tinyxml2.h"
#include "XSF.h"

namespace xsf
{
	bool CSVReader::LoadFile( const char * sFilename )
	{
		FILE* fp = 0;
		fp = fopen( sFilename, "rb" );
		if ( !fp) 
		{
			XSF_ERROR("CSVReader::LoadFile, cant't open file, filename=%s", sFilename);
			return false;
		}

		fseek( fp, 0, SEEK_END );
		size_t size = ftell( fp );
		fseek( fp, 0, SEEK_SET );

		if ( size == 0 ) {
			fclose( fp );
			XSF_ERROR("CSVReader::LoadFile file size is empty, filename=%s", sFilename);
			return false;
		}

		m_pContent = (char*)xsf_malloc(size+1);
		size_t read = fread( m_pContent, 1, size, fp );
		if ( read != size ) {
			fclose( fp );
			XSF_ERROR("CSVReader::LoadFile read error, read=%d, size=%d, filename=%s", read, size, sFilename);
			return false;
		}

		fclose( fp );
		m_pContent[size] = '\0';

		return Parse(sFilename);
	}

	ICSVData * CSVReader::GetData(byte nType, uint32 row, uint32 col)
	{
		if( row >= m_nRowCount || col >= m_nColCount )
		{
			XSF_ERROR("CSVReader Read error, nRow[%u] >= m_nRowCount[%u] || nCol[%u] >= m_nColCount[%u]", row, m_nRowCount, col, m_nColCount);
			return nullptr;
		}

		ICSVData * d = XSFCore::mSchemaHelper->GetData(nType);
		if(d == nullptr)
		{
			XSF_ERROR("CSVReader Read error ICSVData is null, type=%d", nType);
			return nullptr;
		}

		if(!d->Read(row, col, m_Lines[row]->datas[col].c_str()))
		{
			return nullptr;
		}

		return d;
	}


	bool CSVReader::Parse(const char * sFilename)
	{
	#define START_INDEX	3
	#define CTABLE_MAX_COL 4
	// csv 头的行数为4
	// 0 中文说明
	// 1 数据类型
	// 2 数据名称

		int32 nCurRow = 0;
		vector<string> lines;
		XSFCore::Split(m_pContent, '\n', lines);

		if(m_bIsColTable)
		{
			m_nColCount = CTABLE_MAX_COL;
		}

		do
		{
			if(nCurRow >= (int32)lines.size())
				break;

			int32 nCurrent = nCurRow ++;

			auto pStr = lines[nCurrent].c_str();

			//XSF_INFO("2 nCurrent=%u, str=%s", nCurrent, pStr);
			
			if(!m_bIsColTable)
			{
				if( nCurrent == 1 )	// 读取第一行说明，确定最大列数
				{	
					vector<string> heads;
					XSFCore::Split(pStr, ',', heads);
					m_nColCount = heads.size();
					continue;
				}
				else if( nCurrent < START_INDEX )	// 跳过头
					continue;
			}
			
			//XSF_INFO("2 m_nColCount=%u, str=%s", m_nColCount, pStr);
			LineData * pNewLine = new LineData();
			XSFCore::Split(pStr, ',', pNewLine->datas);
			m_Lines.push_back(pNewLine);

			if( pNewLine->datas.size() != m_nColCount )
			{
				XSF_ERROR("CSVReader::Parse column size error, row=%u, pNewLine->datas.size()[%u] != m_nColCount[%u], pStr=%s", m_nRowCount, (uint32)pNewLine->datas.size(), m_nColCount, pStr);
				return false;
			}

			m_nRowCount ++;

		} while (true);
		
		return true;
	}



	bool XMLReader::LoadFile( const char * sFilename )
	{
		tinyxml2::XMLError errID = m_Doc.LoadFile(sFilename);
		if( tinyxml2::XML_SUCCESS != errID )
		{
			XSF_ERROR("XMLReader::LoadFile load xml[%s] error[%d]", sFilename, errID );
			return false;
		}

		return true;
	}

	tinyxml2::XMLElement * XMLReader::GetXmlRoot(void)
	{
		auto root = m_Doc.FirstChildElement("root");
		if(root == nullptr)
		{
			XSF_ERROR("XMLReader::GetXmlRoot error, no root element");
			return nullptr;
		}

		return root;
	}
}

