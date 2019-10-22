using Assets.Scripts.PixelObject;
using UnityEngine;

namespace Assets.Scripts.Scene
{
    class BattleSceneParam : SceneParam
    {
        /// <summary>
        /// プレイヤーステータス
        /// </summary>
        public Database.BattlerStatus PlayerStatus { get; set; }

        /// <summary>
        /// 遭遇した敵の ID
        /// </summary>
        public Database.EnemyDatabase.Id EncountId { get; set; }
    }

    class BattleScene : SceneBase
    {
        /// <summary>
        /// バトル用キャラクター
        /// </summary>
        private GameObject battlePlayer;

        /// <summary>
        /// バトル用キャラクター
        /// </summary>
        private GameObject battleEnemy;

        /// <summary>
        /// プレイヤーステータス
        /// </summary>
        Database.BattlerStatus playerStatus;

        /// <summary>
        /// 敵のステータス
        /// </summary>
        Database.BattlerStatus enemyStatus;

        /// <summary>
        /// 遷移 ID
        /// </summary>
        private enum StateEventId : int
        {
            Start,
            TurnChange,
            Battle,
            Win,
            Lose,
            Leave,
            Max
        }

        /// <summary>
        /// ステートマシン
        /// </summary>
        private IceMilkTea.Core.ImtStateMachine<BattleScene> stateMachine;

        /// <summary>
        /// ポップアップテキスト管理
        /// </summary>
        PopupTextManager popupDamageManager;

        /// <summary>
        /// ターンタイプ
        /// </summary>
        enum TurnTypes
        {
            /// <summary>
            /// プレイヤーのターン
            /// </summary>
            Player,

            /// <summary>
            /// 敵のターン
            /// </summary>
            Enemy
        };

        /// <summary>
        /// ターン
        /// </summary>
        TurnTypes turnTypes;

        /// <summary>
        /// バトルシーン用のユーティリティ
        /// </summary>
        class Util
        {
            private static float DAMAGE_REVISED = 0.5f;

            public static int GetDamage(Database.BattlerStatus offensive, Database.BattlerStatus defensive)
            {
                return Mathf.CeilToInt(
                    // 攻撃力 ^ 2 /（攻撃力 + 防御力） * 補正
                    (offensive.ATK * offensive.ATK) / (offensive.ATK + defensive.DEF) * DAMAGE_REVISED
                );
            }
        }

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        void Start()
        {
            popupDamageManager = new PopupTextManager(CanvasGameObject);

            // プレハブから Player を生成
            var playerPrefab = (GameObject)Resources.Load("Prefabs/Player/BattlePlayer");
            battlePlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            battlePlayer.SetActive(false);
            var enemyPrefab = (GameObject)Resources.Load("Prefabs/Enemy/BattleEnemy");
            battleEnemy = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity);
            battleEnemy.SetActive(false);

            FadeManager.Blackout();
            FadeManager.FadeIn();

            stateMachine = new IceMilkTea.Core.ImtStateMachine<BattleScene>(this);
            stateMachine.AddTransition<WaitState, BattleState>((int)StateEventId.Start);
            stateMachine.AddTransition<BattleState, TurnChangeState>((int)StateEventId.TurnChange);
            stateMachine.AddTransition<TurnChangeState, BattleState>((int)StateEventId.Battle);
            stateMachine.AddTransition<TurnChangeState, WinState>((int)StateEventId.Win);
            stateMachine.AddTransition<TurnChangeState, LoseState>((int)StateEventId.Lose);
            stateMachine.AddTransition<WinState, LeaveState>((int)StateEventId.Leave);

            stateMachine.SetStartState<WaitState>();

            var playerBattlerScript = battlePlayer.GetComponent<Battler>();
            var enemyBattlerScript = battleEnemy.GetComponent<Battler>();

            var param = (BattleSceneParam)SceneParam;
            playerBattlerScript.Status = param.PlayerStatus;
            enemyBattlerScript.Status = Database.EnemyDatabase.GetStatus(param.EncountId);

            turnTypes = TurnTypes.Player;
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        void Update()
        {
            stateMachine.Update();
        }

        /// <summary>
        /// Wait ステート
        /// Battler の入場待ち
        /// </summary>
        private class WaitState : IceMilkTea.Core.ImtStateMachine<BattleScene>.State
        {
            /// <summary>
            /// 
            /// </summary>
            Battler playerBattlerScript;

            /// <summary>
            /// 
            /// </summary>
            Battler enemyBattlerScript;

            protected internal override void Enter()
            {
                playerBattlerScript = Context.battlePlayer.GetComponent<Battler>();
                enemyBattlerScript = Context.battleEnemy.GetComponent<Battler>();

                playerBattlerScript.Enter(BattlerBase.Directions.DIR_RIGHT);
                enemyBattlerScript.Enter(BattlerBase.Directions.DIR_LEFT);

                Context.battlePlayer.SetActive(true);
                Context.battleEnemy.SetActive(true);
            }

            protected internal override void Update()
            {
                if (playerBattlerScript.IsIdle() && enemyBattlerScript.IsIdle())
                {
                    stateMachine.SendEvent((int)StateEventId.Start);
                }
            }
        }

        /// <summary>
        /// Battle ステート
        /// </summary>
        private class BattleState : IceMilkTea.Core.ImtStateMachine<BattleScene>.State
        {
            /// <summary>
            /// プレイヤースクリプト
            /// </summary>
            Battler playerBattlerScript;

            /// <summary>
            /// 敵スクリプト
            /// </summary>
            Battler enemyBattlerScript;

            /// <summary>
            /// 攻撃側
            /// </summary>
            GameObject offensiveBattler;

            /// <summary>
            /// 守備側
            /// </summary>
            GameObject defensiveBattler;

            protected internal override void Enter()
            {
                playerBattlerScript = Context.battlePlayer.GetComponent<Battler>();
                enemyBattlerScript = Context.battleEnemy.GetComponent<Battler>();

                if (Context.turnTypes == TurnTypes.Player)
                {
                    offensiveBattler = Context.battlePlayer;
                    defensiveBattler = Context.battleEnemy;
                }
                else
                {
                    offensiveBattler = Context.battleEnemy;
                    defensiveBattler = Context.battlePlayer;
                }

                offensiveBattler.GetComponent<Battler>().ActionAttack();
                defensiveBattler.GetComponent<Battler>().ActionDamage();
            }

            protected internal override void Update()
            {
                if (playerBattlerScript.IsIdle() && enemyBattlerScript.IsIdle())
                {
                    var pos = defensiveBattler.transform.position;
                    pos.y = pos.y - 32.0f;

                    var defensiveBattlerStatus = defensiveBattler.GetComponent<Battler>().Status;

                    var damage = Util.GetDamage(
                        offensiveBattler.GetComponent<Battler>().Status,
                        defensiveBattlerStatus);

                    defensiveBattlerStatus.LP = defensiveBattlerStatus.LP - damage;

                    Context.popupDamageManager.Popup(damage.ToString(), pos);
                    stateMachine.SendEvent((int)StateEventId.TurnChange);
                }
            }

            protected internal override void Exit()
            {
                // 攻撃・守備側の切り替え
                if (Context.turnTypes == TurnTypes.Player)
                {
                    Context.turnTypes = TurnTypes.Enemy;
                }
                else
                {
                    Context.turnTypes = TurnTypes.Player;
                }
            }
        }

        /// <summary>
        /// TurnChange ステート
        /// </summary>
        private class TurnChangeState : IceMilkTea.Core.ImtStateMachine<BattleScene>.State
        {
            /// <summary>
            /// 経過時間
            /// </summary>
            private float time;

            /// <summary>
            /// 次のステート
            /// </summary>
            private int nextState;

            protected internal override void Enter()
            {
                time = 0;

                var text = "ターン変更";
                Context.popupDamageManager.Popup(text, new Vector3(0 - (text.Length * 10.0f / 2), 0, 0));

                var playerBattlerScript = Context.battlePlayer.GetComponent<Battler>();
                var enemyBattlerScript = Context.battleEnemy.GetComponent<Battler>();
                var playerStatus = playerBattlerScript.Status;
                var enemyStatus = enemyBattlerScript.Status;

                // 次のステートを決定
                var nextEventId = StateEventId.Battle;
                if (playerStatus.LP <= 0)
                {
                    nextEventId = StateEventId.Lose;
                }
                else if (enemyStatus.LP <= 0)
                {
                    nextEventId = StateEventId.Win;
                }
                nextState = (int)nextEventId;
            }

            protected internal override void Update()
            {
                if (time < 1.0f)
                {
                    time += Time.deltaTime;
                    return;
                }

                stateMachine.SendEvent(nextState);
            }
        }

        /// <summary>
        /// Win ステート
        /// </summary>
        private class WinState : IceMilkTea.Core.ImtStateMachine<BattleScene>.State
        {
            /// <summary>
            /// プレイヤースクリプト
            /// </summary>
            Battler playerBattlerScript;

            /// <summary>
            /// 敵スクリプト
            /// </summary>
            Battler enemyBattlerScript;

            protected internal override void Enter()
            {
                playerBattlerScript = Context.battlePlayer.GetComponent<Battler>();
                enemyBattlerScript = Context.battleEnemy.GetComponent<Battler>();

                playerBattlerScript.ActionWin();
                enemyBattlerScript.ActionLose();
            }

            protected internal override void Update()
            {
                if (playerBattlerScript.IsIdle() && enemyBattlerScript.IsIdle())
                {
                    stateMachine.SendEvent((int)StateEventId.Leave);
                }
            }
        }

        /// <summary>
        /// Lose ステート
        /// </summary>
        private class LoseState : IceMilkTea.Core.ImtStateMachine<BattleScene>.State
        {
            /// <summary>
            /// プレイヤースクリプト
            /// </summary>
            Battler playerBattlerScript;

            /// <summary>
            /// 敵スクリプト
            /// </summary>
            Battler enemyBattlerScript;

            protected internal override void Enter()
            {
                playerBattlerScript = Context.battlePlayer.GetComponent<Battler>();
                enemyBattlerScript = Context.battleEnemy.GetComponent<Battler>();

                playerBattlerScript.ActionWin();
            }

            protected internal override void Update()
            {

            }
        }

        /// <summary>
        /// Leave ステート
        /// </summary>
        private class LeaveState : IceMilkTea.Core.ImtStateMachine<BattleScene>.State
        {
            /// <summary>
            /// プレイヤースクリプト
            /// </summary>
            Battler playerBattlerScript;

            /// <summary>
            /// 敵スクリプト
            /// </summary>
            Battler enemyBattlerScript;

            protected internal override void Enter()
            {
                playerBattlerScript = Context.battlePlayer.GetComponent<Battler>();
                enemyBattlerScript = Context.battleEnemy.GetComponent<Battler>();

                playerBattlerScript.ActionLeave();
            }

            protected internal override void Update()
            {

            }
        }
    }
}
