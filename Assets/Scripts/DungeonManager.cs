using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager
{
    /// <summary>
    /// ダンジョンのオブジェクト
    /// </summary>
    private List<GameObject> dungeonList;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public DungeonManager()
    {
        dungeonList = new List<GameObject>
        {
            (GameObject)Resources.Load("Prefabs/Dungeon/Dungeon_1"),
            (GameObject)Resources.Load("Prefabs/Dungeon/Dungeon_2"),
            (GameObject)Resources.Load("Prefabs/Dungeon/Dungeon_3")
        };
    }

    public GameObject GetDungeon(int index)
    {
        return dungeonList[index];
    }
}
