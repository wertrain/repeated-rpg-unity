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
    /// プレイヤー
    /// </summary>
    private GameObject player;

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
        InToPlayerMove,
        PlayerMoveToWaitSelect,
        IdleToOut,
        OutToIn,
    }

    /// <summary>
    /// ステートマシン
    /// </summary>
    private IceMilkTea.Core.ImtStateMachine<GameScene> stateMachine;

    /// <summary>
    /// プレイヤー初期位置
    /// </summary>
    Vector3 playerInitPos = new Vector3(110.0f, -500.0f, 0.0f);

    /// <summary>
    /// プレイヤー移動目標位置
    /// </summary>
    Vector3 playerTargetPos = new Vector3(110.0f, -320.0f, 0.0f);

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        dungeon = new DungeonManager();

        // ステートマシンの遷移テーブルを構築
        stateMachine = new IceMilkTea.Core.ImtStateMachine<GameScene>(this);
        stateMachine.AddTransition<InState, PlayerMoveState>((int)StateEventId.In);
        stateMachine.AddTransition<InState, PlayerMoveState>((int)StateEventId.InToPlayerMove);
        stateMachine.AddTransition<PlayerMoveState, WaitSelectState>((int)StateEventId.PlayerMoveToWaitSelect);
        stateMachine.AddTransition<WaitSelectState, OutState>((int)StateEventId.IdleToOut);
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
    /// In ステート
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

            // プレハブから Player を生成
            var playerPrefab = (GameObject)Resources.Load("Prefabs/Player/Player");
            Context.player = Instantiate(playerPrefab, Context.playerInitPos, Quaternion.identity);
            // 自動移動の開始
            var script = Context.player.GetComponent<Player>();
            script.StartAutoMove(Player.AutoMoveMode.Frontward, 0.3f);

            Context.FadeManager.Blackout();
            Context.FadeManager.FadeIn();
        }

        protected internal override void Update()
        {
            if (!Context.FadeManager.IsFade())
            {
                // Idle ステートへ
                Context.stateMachine.SendEvent((int)StateEventId.InToPlayerMove);
            }
        }

        protected internal override void Exit()
        {

        }
    }

    /// <summary>
    /// PlayerMove ステート
    /// </summary>
    private class PlayerMoveState : IceMilkTea.Core.ImtStateMachine<GameScene>.State
    {
        protected internal override void Update()
        {
            if (Context.player.transform.position.y >= Context.playerTargetPos.y)
            {
                var script = Context.player.GetComponent<Player>();
                script.StopAutoMove();

                // Idle ステートへ
                Context.stateMachine.SendEvent((int)StateEventId.PlayerMoveToWaitSelect);
            }
        }
    }

    /// <summary>
    /// WaitSelect ステート
    /// </summary>
    private class WaitSelectState : IceMilkTea.Core.ImtStateMachine<GameScene>.State
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
            gameObject.transform.SetAsFirstSibling(); // フェードパネルよりも手前にしたい

            foreach (Transform child in gameObject.transform)
            {
                var button = child.GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    // Out ステートへ
                    Context.stateMachine.SendEvent((int)StateEventId.IdleToOut);

                    // 自動移動の開始
                    /*
                    var script = Context.player.GetComponent<Player>();
                    var autoMoveDir = Player.AutoMoveMode.Frontward;
                    switch (Context.dungeonIndex)
                    {
                        case 0:
                            autoMoveDir = Player.AutoMoveMode.Frontward;
                            break;

                        case 1:
                            if (button.tag == "0") autoMoveDir = Player.AutoMoveMode.Left;
                            else autoMoveDir = Player.AutoMoveMode.Right;
                            break;

                        case 2:
                            if (button.tag == "0") autoMoveDir = Player.AutoMoveMode.Left;
                            else if(button.tag == "1") autoMoveDir = Player.AutoMoveMode.Frontward;
                            else autoMoveDir = Player.AutoMoveMode.Right;
                            break;
                    }
                    script.StartAutoMove(autoMoveDir, 0.3f);
                    */
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
    /// Out ステート
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
                Destroy(Context.player);

                // In ステートへ
                Context.stateMachine.SendEvent((int)StateEventId.OutToIn);
            }
        }

        protected internal override void Exit()
        {

        }
    }
}
