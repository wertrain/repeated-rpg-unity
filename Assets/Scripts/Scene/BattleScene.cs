using Assets.Scripts.PixelObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Scene
{
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
        /// 遷移 ID
        /// </summary>
        private enum StateEventId : int
        {
            Wait,
            Start,
            TurnChange,
            Battle,
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

            stateMachine = new IceMilkTea.Core.ImtStateMachine<BattleScene>(this);

            FadeManager.Blackout();
            FadeManager.FadeIn();

            stateMachine = new IceMilkTea.Core.ImtStateMachine<BattleScene>(this);
            stateMachine.AddTransition<WaitState, BattleState>((int)StateEventId.Wait);
            stateMachine.AddTransition<WaitState, BattleState>((int)StateEventId.Start);
            stateMachine.AddTransition<BattleState, TurnChangeState>((int)StateEventId.TurnChange);
            stateMachine.AddTransition<TurnChangeState, BattleState>((int)StateEventId.Battle);
            stateMachine.SetStartState<WaitState>();

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
                    Context.popupDamageManager.Popup("30", pos);
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
            private float time;

            protected internal override void Enter()
            {
                time = 0;

                var text = "ターン変更";
                Context.popupDamageManager.Popup(text, new Vector3(0 - (text.Length * 10.0f / 2), 0, 0));
            }

            protected internal override void Update()
            {
                if (time < 1.0f)
                {
                    time += Time.deltaTime;
                    return;
                }

                stateMachine.SendEvent((int)StateEventId.Battle);
            }
        }
    }
}
