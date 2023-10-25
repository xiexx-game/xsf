//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/LevelView.cs
// 作者：Xoen
// 时间：2023/08/25
// 描述：关卡预览脚本
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

using System.IO;

public class LevelView : MonoBehaviour
{
    public int nLevel;
    public GameObject block;
    public GameObject Dot;
    private string[] LevelData;

    private SingleBlock[] m_Blocks;

    public int Row;
    public int Col;


    void Start()
    {
        string[] levelData = File.ReadAllLines($"{Application.dataPath}/Scp/Level.csv");
        string data = levelData[3+nLevel];
        string[] datas = data.Split(",");
        Debug.Log(data);
        LevelData = datas[1].Split(":");
        Row = System.Convert.ToInt32(datas[2]);
        Col = System.Convert.ToInt32(datas[3]);

        m_Blocks = LevelDef.CreateBlocks(Row, Col, transform, block, 0.8f, false);
        for(int i = 0; i < LevelData.Length; i ++)
        {
            if(LevelData[i] == "#") 
            {
                m_Blocks[i].SetColor(BlockColor.Wall);
            }
            else if(LevelData[i] == "-")
            {
                m_Blocks[i].SetColor(BlockColor.Road);
            }
            else if(LevelData[i] == "@")
            {
                m_Blocks[i].SetColor(BlockColor.Road);
            }
            else if(LevelData[i] == ".")
            {
                m_Blocks[i].SetColor(BlockColor.Road);

                var dot = GameObject.Instantiate(Dot);
                dot.SetActive(true);
                dot.transform.SetParent(transform);
                dot.transform.position = m_Blocks[i].go.transform.position;
                dot.transform.localScale = Dot.transform.localScale;
            }
            else if(LevelData[i] == "$")
            {
                m_Blocks[i].SetColor(BlockColor.Box);
            }
            else if(LevelData[i] == "*")
            {
                m_Blocks[i].SetColor(BlockColor.Box);
                
                var dot = GameObject.Instantiate(Dot);
                dot.SetActive(true);
                dot.transform.SetParent(transform);
                dot.transform.position = m_Blocks[i].go.transform.position;
                dot.transform.localScale = Dot.transform.localScale;
            }
        }
    }
}