using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScene : SceneBase
{
    /// <summary>
    /// ダンジョン管理
    /// </summary>
    private DungeonManager dungeon;

    /// <summary>
    /// ダンジョンのインデックス番号
    /// </summary>
    private int dungeonIndex;

    /// <summary>
    /// 
    /// </summary>
    private enum StateEventId
    {
        In,
        InToIdle,
        IdleToOut,
        OutToIn,
    }

    /// <summary>
    /// ステートマシン
    /// </summary>
    private IceMilkTea.Core.ImtStateMachine<GameScene> stateMachine;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        dungeon = new DungeonManager();

        // ステートマシンの遷移テーブルを構築
        stateMachine = new IceMilkTea.Core.ImtStateMachine<GameScene>(this);
        stateMachine.AddTransition<InState, IdleState>((int)StateEventId.In);
        stateMachine.AddTransition<InState, IdleState>((int)StateEventId.InToIdle);
        stateMachine.AddTransition<IdleState, OutState>((int)StateEventId.IdleToOut);
        stateMachine.AddTransition<OutState, InState>((int)StateEventId.OutToIn);
        stateMachine.SetStartState<InState>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        stateMachine.Update();
    }

    /// <summary>
    /// IN ステート
    /// </summary>
    private class InState : IceMilkTea.Core.ImtStateMachine<GameScene>.State
    {
        protected internal override void Enter()
        {
            // 最初のダンジョンを生成
            Context.dungeonIndex = Random.Range(0, 3);
            var prefab = Context.dungeon.GetDungeon(Context.dungeonIndex);
            var gameObject = Instantiate(prefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            gameObject.transform.SetParent(Context.GridGameObject.transform, false);

            Context.FadeManager.Blackout();
            Context.FadeManager.FadeIn();
        }

        protected internal override void Update()
        {
            if (!Context.FadeManager.IsFade())
            {
                // Idle ステートへ
                Context.stateMachine.SendEvent((int)StateEventId.InToIdle);
            }
        }

        protected internal override void Exit()
        {

        }
    }

    /// <summary>
    /// IDLE ステート
    /// </summary>
    private class IdleState : IceMilkTea.Core.ImtStateMachine<GameScene>.State
    {
        protected internal override void Enter()
        {
            var arrowsList = new List<GameObject>
            {
                (GameObject)Resources.Load("Prefabs/UI/Arrows_1"),
                (GameObject)Resources.Load("Prefabs/UI/Arrows_2"),
                (GameObject)Resources.Load("Prefabs/UI/Arrows_3")
            };

            var prefab = arrowsList[Context.dungeonIndex];
            var gameObject = Instantiate(prefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            gameObject.transform.SetParent(Context.CanvasGameObject.transform, false);

            foreach (Transform child in gameObject.transform)
            {
                var button = child.GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    // Out ステートへ
                    Context.stateMachine.SendEvent((int)StateEventId.IdleToOut);
                });
            }
        }

        protected internal override void Update()
        {

        }

        protected internal override void Exit()
        {

        }
    }

    /// <summary>
    /// OUT ステート
    /// </summary>
    private class OutState : IceMilkTea.Core.ImtStateMachine<GameScene>.State
    {
        protected internal override void Enter()
        {
            Context.FadeManager.FadeOut();
        }

        protected internal override void Update()
        {
            if (!Context.FadeManager.IsFade())
            {
                foreach (Transform child in Context.CanvasGameObject.transform)
                {
                    if (child.tag == "Arrows")
                    {
                        Destroy(child.gameObject);
                    }
                }
                foreach (Transform child in Context.GridGameObject.transform)
                {
                    if (child.tag == "Dungeon")
                    {
                        Destroy(child.gameObject);
                    }
                }
                
                // In ステートへ
                Context.stateMachine.SendEvent((int)StateEventId.OutToIn);
            }
        }

        protected internal override void Exit()
        {

        }
    }
}
