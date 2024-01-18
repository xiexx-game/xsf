//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/PacMan/AI/AI_Pinky.cs
// 作者：Xoen Xie
// 时间：2023/06/21
// 描述：鬼AI - 小粉
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

public class AI_Pinky : AI_Ghost
{
    public override void OnBorn()
    {

    }

    public override PacManMapBlock GetTarget()
    {
        var current = LevelGamePackMan.Instance.Character.Current;
        var dir = LevelGamePackMan.Instance.Character.MoveDir;

        int nCount = 0;
        while(nCount < 4)
        {
            if(current.ConnectIndex[(int)dir] > 0)
            {
                current = LevelGamePackMan.Instance.Map.GetBlockByIndex(current.ConnectIndex[(int)dir]);
            }
            else
            {
                int nextDir = (int)dir +1;
                if(nextDir >= (int)PacManMoveDir.Max)
                    nextDir = (int)PacManMoveDir.Up;

                if(current.ConnectIndex[(int)nextDir] > 0)
                {
                    current = LevelGamePackMan.Instance.Map.GetBlockByIndex(current.ConnectIndex[(int)dir]);
                }
                else
                {
                    nextDir = (int)dir - 1;
                    if(nextDir < 0)
                        nextDir = (int)PacManMoveDir.Left;


                    if(current.ConnectIndex[(int)nextDir] > 0)
                    {
                        current = LevelGamePackMan.Instance.Map.GetBlockByIndex(current.ConnectIndex[(int)dir]);
                    }
                    else
                    {
                        Debug.LogError("AI_Pinky GetTarget target not found, character current=" + current.Index);
                        return null;
                    }
                }
            }

            nCount ++;
        }

        return current;
    }
}