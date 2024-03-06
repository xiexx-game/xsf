//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/PacMan/PacManMapBlock.cs
// 作者：Xoen Xie
// 时间：2023/06/21
// 描述：吃豆人 地图块
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections.Generic;

public class PacManMapBlock
{
    public const int CONNECT_MAX = 4;
    public int Index;
    public GameObject go;
    public ScpPacManMap scp;
    public Vector3 pos;
    public uint TypeValue;

    public int ConnectCount;
    public int[] ConnectIndex = new int[CONNECT_MAX];

    public bool IsRoad {
        get {
            return HasType(BlockType.Road);
        }
    }

    public bool IsReadArea {
        get {
            return HasType(BlockType.RedArea);
        }
    }

    public bool IsTunnel {
        get {
            return HasType(BlockType.Tunnel);
        }
    }

    public bool HasGhost {
        get
        {
            return HasType(BlockType.GhostBlinky) || HasType(BlockType.GhostClyde) || HasType(BlockType.GhostInky) || HasType(BlockType.GhostPinky);
        }
    }

    public bool HasType(BlockType nType)
    {
        uint nTypeCheck = (uint)nType;
        //Debug.Log($"TypeValue={TypeValue}, nType={nTypeCheck}， v1={(TypeValue & nTypeCheck)}, v2={nTypeCheck}");
        return ((TypeValue & nTypeCheck) == nTypeCheck);
    }

    public void ClearType(BlockType nType)
    {
        uint nTypeClear = (uint)nType;
        nTypeClear = ~nTypeClear;
        TypeValue = TypeValue & nTypeClear;
    }

    public void SetType(BlockType nType)
    {
        TypeValue = TypeValue | (uint)nType;
    }
}

public class PacManPathNode 
{
    public PacManMapBlock block;

    public PacManPathNode pre;

    public PacManMoveDir enterDir;
}

public struct PacManPathResult
{
    public PacManMapBlock block;
    public Vector3 pos;
    public bool Teleport;
}

public enum BlockType
{
    None = 0,
    Road        = 0b1,         // 可行走区域
    Bean        = 0b10,        // 普通豆
    EnergyBean  = 0b100,      // 能量豆
    RedArea     = 0b1000,       // 红色区域，ghost不能往上走的区域
    Tunnel      = 0b10000,      // 隧道区域，ghost要减速
    PacMan           = 0b100000,
    GhostBlinky      = 0b1000000,     // ghost
    GhostClyde       = 0b10000000,     // ghost
    GhostInky        = 0b100000000,     // ghost
    GhostPinky       = 0b1000000000,     // ghost

    Max,
}

public class PacManMap
{
    public List<PacManMapBlock> m_Blocks;
    public const float SINGLE_BLOCK_SIZE = 0.38f;
    public const int MAX_COL = 28;
    public const int MAX_ROW = 31;

    public const float BLOCK_Z = -0.1f;

    public GameObject[] m_Beans;

    public const float XOffset = (SINGLE_BLOCK_SIZE * MAX_COL) / 2;
    public const float YOffset = (SINGLE_BLOCK_SIZE * MAX_ROW) / 2;

    public PacManMapBlock []FleeTargets { get; private set; }

    public readonly int TunnelLeftIndex = 392;
    public readonly int TunnelRightIndex = 419;

    public readonly float TunnelLeft = -5.318f;
    public readonly float TunnelRight = 5.318f;

    public int BeanCount { get; private set;}

    public PacManMap()
    {
        m_Blocks = new List<PacManMapBlock>();
        m_Beans = new GameObject[MAX_COL * MAX_ROW];
    }

    public void Create(MonoPacMan mono)
    {
        int index = 0;
        float XOrigin = (SINGLE_BLOCK_SIZE * MAX_COL) / -2;
        float YOrigin = (SINGLE_BLOCK_SIZE * MAX_ROW) / 2;
        float XStart = XOrigin + SINGLE_BLOCK_SIZE / 2;
        float YStart = YOrigin - SINGLE_BLOCK_SIZE / 2;

        var datas = XSFSchema.Instance.Get<SchemaPacManMap>((int)SchemaID.PacManMap).Datas;

        for (int r = 0; r < MAX_ROW; r++)
        {
            float y = YStart - r * SINGLE_BLOCK_SIZE;

            for (int c = 0; c < MAX_COL; c++)
            {
                float x = XStart + c * SINGLE_BLOCK_SIZE;
                PacManMapBlock block = new PacManMapBlock();
                block.Index = index;
                block.scp = datas[index++];
                block.TypeValue = block.scp.uBlockType;

                //Debug.Log($"index={index}, c={c}, row={r}, block.scp.iCol={block.scp.iCol}, block.scp.iRow={block.scp.iRow}");

                if (block.scp.iCol != c || block.scp.iRow != r)
                {
                    Debug.LogError($"PacManMap Create block.scp.iCol:{block.scp.iCol} != c:{c} || block.scp.iRow:{block.scp.iRow} != r:{r}");
                    return;
                }

                switch (block.scp.sSprite)
                {
                    case "double-angle-90":
                        block.go = GameObject.Instantiate(mono.MapObjs[(int)MapObjID.DoubleAngle90]);
                        break;

                    case "double-angle-small":
                        block.go = GameObject.Instantiate(mono.MapObjs[(int)MapObjID.DoubleAngleSmall]);
                        break;

                    case "double-angle":
                        block.go = GameObject.Instantiate(mono.MapObjs[(int)MapObjID.DoubleAngle]);
                        break;

                    case "double-line":
                        block.go = GameObject.Instantiate(mono.MapObjs[(int)MapObjID.DoubleLine]);
                        break;

                    case "single-angle-line":
                        block.go = GameObject.Instantiate(mono.MapObjs[(int)MapObjID.SingleAngleLine]);
                        break;

                    case "single-angle":
                        block.go = GameObject.Instantiate(mono.MapObjs[(int)MapObjID.SingleAngle]);
                        break;

                    case "single-line":
                        block.go = GameObject.Instantiate(mono.MapObjs[(int)MapObjID.SingleLine]);
                        break;

                    default:
                        if(block.IsReadArea)
                        {
                            block.go = GameObject.Instantiate(mono.MapObjs[(int)MapObjID.RedArea]);
                        }
                        else if(block.IsTunnel)
                        {
                            block.go = GameObject.Instantiate(mono.MapObjs[(int)MapObjID.Tunnel]);
                        }
                        else if(block.IsRoad)
                        {
                            block.go = GameObject.Instantiate(mono.MapObjs[(int)MapObjID.Road]);
                        }

                        if (block.HasType(BlockType.EnergyBean))
                        {
                            m_Beans[block.Index] = GameObject.Instantiate(mono.MapObjs[(int)MapObjID.EnergyBean]);
                            m_Beans[block.Index].transform.SetParent(mono.Frame.transform);
                            m_Beans[block.Index].transform.localPosition = new Vector3(x, y, BLOCK_Z);
                            m_Beans[block.Index].SetActive(true);
                            BeanCount ++;
                        }
                        
                        if(block.HasType(BlockType.Bean))
                        {
                            m_Beans[block.Index] = GameObject.Instantiate(mono.MapObjs[(int)MapObjID.Bean]);
                            m_Beans[block.Index].transform.SetParent(mono.Frame.transform);
                            m_Beans[block.Index].transform.localPosition = new Vector3(x, y, BLOCK_Z);
                            m_Beans[block.Index].SetActive(true);
                            BeanCount ++;
                        }
                        break;
                }

                block.pos = new Vector3(x, y, BLOCK_Z+0.001f);

                if (block.go != null)
                {
                    block.go.SetActive(true);
                    block.go.name = $"{block.scp.iRow}_{block.scp.iCol}_{block.Index}";
                    block.go.transform.SetParent(mono.Frame.transform);
                    block.go.transform.localPosition = block.pos;
                    block.go.transform.localRotation = Quaternion.Euler(block.scp.fXRota, block.scp.fYRota, block.scp.fZRota);
                }

                m_Blocks.Add(block);
            }
        }

        for (int i = 0; i < m_Blocks.Count; i++)
        {
            if (m_Blocks[i].IsRoad)
            {
                int nCol = m_Blocks[i].scp.iCol;
                int nRow = m_Blocks[i].scp.iRow;

                // 上
                int upRow = nRow - 1;
                if (upRow >= 0)
                {
                    var blockUp = GetBlock(upRow, nCol);
                    if (blockUp != null)
                    {
                        if (blockUp.IsRoad)
                        {
                            m_Blocks[i].go.name += $".U{blockUp.Index}";
                            m_Blocks[i].ConnectIndex[0] = blockUp.Index;
                            m_Blocks[i].ConnectCount ++;
                        }
                    }
                }


                // right
                int rightCol = nCol + 1;
                if (rightCol < MAX_COL)
                {
                    var blockRight = GetBlock(nRow, rightCol);
                    if (blockRight != null)
                    {
                        if (blockRight.IsRoad)
                        {
                            m_Blocks[i].go.name += $".R{blockRight.Index}";
                            m_Blocks[i].ConnectIndex[1] = blockRight.Index;
                            m_Blocks[i].ConnectCount ++;
                        }
                    }
                }

                // down
                int downRow = nRow + 1;
                if (downRow < MAX_ROW)
                {
                    var blockDown = GetBlock(downRow, nCol);
                    if (blockDown != null)
                    {
                        if (blockDown.IsRoad)
                        {
                            m_Blocks[i].go.name += $".D{blockDown.Index}";
                            m_Blocks[i].ConnectIndex[2] = blockDown.Index;
                            m_Blocks[i].ConnectCount ++;
                        }
                    }
                }

                // left
                int leftCol = nCol - 1;
                if (leftCol >= 0)
                {
                    var blockLeft = GetBlock(nRow, leftCol);
                    if (blockLeft != null)
                    {
                        if (blockLeft.IsRoad)
                        {
                            m_Blocks[i].go.name += $".L{blockLeft.Index}";
                            m_Blocks[i].ConnectIndex[3] = blockLeft.Index;
                            m_Blocks[i].ConnectCount ++;
                        }
                    }
                }

                // 构建隧道
                if (i == TunnelLeftIndex)
                {
                    m_Blocks[i].go.name += $".L{TunnelRightIndex}";
                    m_Blocks[i].ConnectIndex[(int)PacManMoveDir.Left] = TunnelRightIndex;
                    m_Blocks[i].ConnectCount ++;
                }
                else if (i == TunnelRightIndex)
                {
                    m_Blocks[i].go.name += $".R{TunnelLeftIndex}";
                    m_Blocks[i].ConnectIndex[(int)PacManMoveDir.Right] = TunnelLeftIndex;
                    m_Blocks[i].ConnectCount ++;
                }
            }
        }

        FleeTargets = new PacManMapBlock[(int)GhostType.Max];
        FleeTargets[(int)GhostType.Blinky] = GetBlockByIndex(54);
        FleeTargets[(int)GhostType.Pinky] = GetBlockByIndex(29);
        FleeTargets[(int)GhostType.Inky] = GetBlockByIndex(838);
        FleeTargets[(int)GhostType.Clyde] = GetBlockByIndex(813);

        LevelGamePackMan.Instance.IsMapReady = true;
    }

    public int XPos2Col(float x)
    {
        return (int)((x+XOffset)/SINGLE_BLOCK_SIZE);
    }

    public int YPos2Row(float y)
    {
        return Mathf.Abs((int)((y-YOffset)/SINGLE_BLOCK_SIZE));
    }

    public void Release()
    {
        for (int i = 0; i < m_Blocks.Count; i++)
        {
            GameObject.Destroy(m_Blocks[i].go);
        }

        LevelGamePackMan.Instance.IsMapReady = false;
    }

    public PacManMapBlock GetBlock(int row, int col)
    {
        var index = LevelDef.GetBlockIndex(row, col, MAX_COL);
        //Debug.Log($"GetBlock index={index}");
        return GetBlockByIndex(index);
    }

    public PacManMapBlock GetBlockByIndex(int index)
    {
        //Debug.Log($"GetBlockByIndex index={index} m_Blocks.Count={m_Blocks.Count}");
        if (index < 0 || index >= m_Blocks.Count)
            return null;

        //Debug.Log($"GetBlockByIndex m_Blocks[index]={m_Blocks[index]}");
        return m_Blocks[index];
    }

    private HashSet<int> CloseList = new HashSet<int>();
    List<PacManPathNode> OpenList = new List<PacManPathNode>();

    public PacManMapBlock Pos2Block(Vector3 pos)
    {
        int col = XPos2Col(pos.x);
        int row = YPos2Row(pos.y);
        //Debug.Log($"Pos2Block col={col}, row={row}");
        if(col < 0 || col >= MAX_COL) {
            return null;
        }

        if(row < 0 || row >= MAX_ROW) {
            return null;
        }

        return GetBlock(row, col);
    }

    public PacManMoveDir DirReverse(PacManMoveDir dir) 
    {
        switch(dir)
        {
        case PacManMoveDir.Up:  return PacManMoveDir.Down;
        case PacManMoveDir.Right:  return PacManMoveDir.Left;
        case PacManMoveDir.Down:  return PacManMoveDir.Up;
        case PacManMoveDir.Left:  return PacManMoveDir.Right;
        default: return dir;
        }
    }

    private List<PacManMapBlock> m_LastPath;
    public List<PacManPathResult> FindPath(PacManMoveDir enterDir, PacManMapBlock startBlock, PacManMapBlock endBlock, bool showPath)
    {
        if(!endBlock.IsRoad)
        {
            Debug.LogError("FindPath end block  is not road....");
            return null;
        }

        OpenList.Clear();
        CloseList.Clear();

        CloseList.Add(startBlock.Index);

        PacManPathNode startNode = new PacManPathNode();
        startNode.block = startBlock;
        startNode.enterDir = enterDir;

        PacManPathNode endNode = new PacManPathNode();
        endNode.block = endBlock;
        //Debug.LogWarning($"FindPath endNode.block={endNode.block.Index} enterDir={enterDir}");

        List<PacManPathNode> CheckResult = new List<PacManPathNode>();
        CheckResult.Add(startNode);

        bool Working = true;
        while(Working)
        {
            OpenList.Clear();
            OpenList.AddRange(CheckResult);
            CheckResult.Clear();
            for(int i = 0; i < OpenList.Count; i ++)
            {
                if(CheckNode(OpenList[i], CheckResult, endNode))
                {
                    Working = false;
                    break;
                }
            }
        }

        List<PacManMapBlock> result = new List<PacManMapBlock>();
        PacManPathNode pathNode = endNode;
        
        while(pathNode != null)
        {
            result.Add(pathNode.block);

            pathNode = pathNode.pre;
        }

        result.Reverse();

        if(showPath)
        {
            if(m_LastPath != null)
            {
                for(int i = 0; i < m_LastPath.Count; i ++)
                {
                var block = GetBlock(m_LastPath[i].scp.iRow, m_LastPath[i].scp.iCol);
                block.go.GetComponent<SpriteRenderer>().color = Color.white;
                }
            }

            m_LastPath = result;
            for(int i = 0; i < m_LastPath.Count; i ++)
            {
                //Debug.Log($"pos={m_LastPath[i].scp.iRow}, {m_LastPath[i].scp.iCol}");
                var block = GetBlock(m_LastPath[i].scp.iRow, m_LastPath[i].scp.iCol);
                block.go.GetComponent<SpriteRenderer>().color = Color.yellow;
            }
        }

        List<PacManPathResult> finalPath = new List<PacManPathResult>();
        for(int i = 0; i < result.Count; i ++)
        {
            PacManPathResult res;
            res.block = result[i];
            res.pos = result[i].pos;
            res.Teleport = false;
            finalPath.Add(res);

            if(i +1 < result.Count)
            {
                if(result[i].Index == TunnelLeftIndex && result[i+1].Index == TunnelRightIndex)
                {
                    PacManPathResult teleStart;
                    teleStart.pos = result[i].pos;
                    teleStart.pos.x = TunnelLeft;
                    teleStart.Teleport = true;
                    teleStart.block = result[i];
                    finalPath.Add(teleStart);

                    PacManPathResult teleEnd;
                    teleEnd.pos = result[i].pos;
                    teleEnd.pos.x = TunnelRight;
                    teleEnd.block = result[i+1];
                    teleEnd.Teleport = false;
                    finalPath.Add(teleEnd);
                }
                else if(result[i].Index == TunnelRightIndex && result[i+1].Index == TunnelLeftIndex)
                {
                    PacManPathResult teleStart;
                    teleStart.pos = result[i].pos;
                    teleStart.pos.x = TunnelRight;
                    teleStart.Teleport = true;
                    teleStart.block = result[i];
                    finalPath.Add(teleStart);

                    PacManPathResult teleEnd;
                    teleEnd.pos = result[i].pos;
                    teleEnd.pos.x = TunnelLeft;
                    teleEnd.block = result[i+1];
                    teleEnd.Teleport = false;
                    finalPath.Add(teleEnd);
                }
            }
        }

        return finalPath;
    }

    private bool CheckNode(PacManPathNode current, List<PacManPathNode> cacheList, PacManPathNode endNode)
    {
        var currentBlock = current.block;
        //Debug.Log($"CheckNode currentBlock={currentBlock.scp.iRow}, {currentBlock.scp.iCol}, enterDir={current.enterDir}");

        for(int i = 0; i < currentBlock.ConnectIndex.Length; i ++)
        {
            if(currentBlock.IsReadArea && i == (int)PacManMoveDir.Up && currentBlock.ConnectIndex[i] > 0 )      // 红色区域不能往上走
                continue;

            if(currentBlock.ConnectIndex[i] > 0 && i != (int)current.enterDir)
            {
                var nextIndex = currentBlock.ConnectIndex[i];
                PacManPathNode preNode = current;
                PacManMoveDir currentEnterDir = DirReverse((PacManMoveDir)i);
                
                while(true)
                {
                    var nextBlock = GetBlockByIndex(nextIndex);

                    if(nextIndex == endNode.block.Index)     // 找到终点了
                    {
                        //Debug.Log("Find End node");
                        endNode.pre = preNode;
                        return true;
                    } 
                    else
                    {
                        if(nextBlock.ConnectCount >= 3)     // 说明有分叉路
                        {
                            if(!CloseList.Contains(nextBlock.Index))
                            {
                                CloseList.Add(nextBlock.Index);

                                PacManPathNode node = new PacManPathNode();
                                node.block = nextBlock;
                                node.pre = preNode;
                                node.enterDir = currentEnterDir;
                                cacheList.Add(node);

                                preNode = node;

                                //Debug.Log($"CheckNode find next block 3, nextBlock={nextBlock.scp.iRow}, {nextBlock.scp.iCol}, enterDir={currentEnterDir}");
                            }
                            break;
                        }
                        else
                        {
                            PacManPathNode node = new PacManPathNode();
                            node.block = nextBlock;
                            node.pre = preNode;

                            preNode = node;

                            //Debug.Log($"CheckNode find next block 2, nextBlock={nextBlock.scp.iRow}, {nextBlock.scp.iCol}, enterDir={currentEnterDir}");

                            for(int J = 0; J < nextBlock.ConnectIndex.Length; J ++)
                            {
                                if(J != (int)currentEnterDir && nextBlock.ConnectIndex[J] > 0)
                                {
                                    nextIndex = nextBlock.ConnectIndex[J];
                                    currentEnterDir = DirReverse((PacManMoveDir)J);
                                    break;
                                }
                            }

                            //Debug.Log("nextIndex=" + nextIndex + ", currentEnterDir=" + currentEnterDir);
                        }
                    }
                }
            }
        }

        return false;
    }

    public void OnPacManEnterBlock(PacManMapBlock block)
    {
        //Debug.Log("OnPacManEnterBlock index=" + block.Index);
        block.SetType(BlockType.PacMan);
        var character = LevelGamePackMan.Instance.Character;

        bool HasBlinky = block.HasType(BlockType.GhostBlinky);
        bool HasClyde = block.HasType(BlockType.GhostClyde);
        bool HasInky = block.HasType(BlockType.GhostInky);
        bool HasPinky = block.HasType(BlockType.GhostPinky);
        if(HasBlinky || HasClyde || HasInky || HasPinky)
        {
            if(character.Speed.HasEnergy)
            {
                // 鬼死了
                
            }
            else
            {
                LevelGamePackMan.Instance.GameOver();
                return;
            }
        }
        
        //Debug.Log("OnPacManEnterBlock 1");
        if(block.HasType(BlockType.Bean))
        {
            //Debug.Log("OnPacManEnterBlock HasType(BlockType.Bean)");
            m_Beans[block.Index].SetActive(false);
            character.Speed.EatBean();
            block.ClearType(BlockType.Bean);
            Level.Instance.Current.GameSocre += 1;
            if(BeanCount > 0)
            {
                BeanCount --;
            }
        }
        else if(block.HasType(BlockType.EnergyBean))
        {
            //Debug.Log("OnPacManEnterBlock HasType(BlockType.EnergyBean)");
            block.ClearType(BlockType.EnergyBean);

            m_Beans[block.Index].SetActive(false);

            character.Speed.OnEnergy();

            for(int i = 0; i < LevelGamePackMan.Instance.Ghosts.Length; i ++)
            {
                LevelGamePackMan.Instance.Ghosts[i].OnPacManEatEnergy();
            }

            Level.Instance.Current.GameSocre += 10;

            if(BeanCount > 0)
            {
                BeanCount --;
            }
        }
        
    }

    public void OnPacManExitBlock(PacManMapBlock block)
    {
        //Debug.Log("OnPacManExitBlock index=" + block.Index);
        block.ClearType(BlockType.PacMan);
    }

    public void OnGhostEnterBlock(MonoGhost ghost, PacManMapBlock block)
    {
        block.SetType(ghost.m_AI.SetType);
        Debug.Log($"OnGhostEnterBlock block index={block.Index}, type={block.TypeValue}, gost set type={ghost.m_AI.SetType}");
        if(block.HasType(BlockType.PacMan))
        {
            var character = LevelGamePackMan.Instance.Character;
            if(character.Speed.HasEnergy)
            {
                // 鬼死了
                block.ClearType(ghost.m_AI.SetType);
                ghost.Die();
            }
            else
            {
                LevelGamePackMan.Instance.GameOver();
            }
        }
        else if(block.HasType(BlockType.Tunnel))
        {
            ghost.Speed.EnterTunnel();
        }
    }

    public void OnGhostExitBlock(MonoGhost ghost, PacManMapBlock block)
    {
        block.ClearType(ghost.m_AI.SetType);

        Debug.Log($"OnGhostExitBlock block index={block.Index}, type={block.TypeValue}, gost set type={ghost.m_AI.SetType}");

        if(block.HasType(BlockType.Tunnel))
        {
            ghost.Speed.ExitTunnel();
        }
    }
}