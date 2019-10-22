using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.PixelObject
{
    class Battler : BattlerBase
    {
        /// <summary>
        /// ステータス
        /// </summary>
        public Database.BattlerStatus Status { get; set; }

        /// <summary>
        /// キャラ画像スプライトリスト
        /// </summary>
        public List<Sprite> sprites = new List<Sprite>();

        /// <summary>
        /// アニメーションパターン数
        /// </summary>
        protected const int PATTERN_NUM_BY_DIRECTION = 3;

        /// <summary>
        /// 
        /// </summary>
        private enum StateEventId : int
        {
            Idle,
            Attack,
            Damage,
            Win,
            Lose,
            Leave,
            Max
        }

        /// <summary>
        /// ステートマシン
        /// </summary>
        private IceMilkTea.Core.ImtStateMachine<Battler> stateMachine;

        /// <summary>
        /// 足踏み用のタイマー
        /// </summary>
        protected float stepTime;

        /// <summary>
        /// 足踏み用のインデックス
        /// </summary>
        protected int stepSpriteIndex;

        /// <summary>
        /// プレイヤー移動速度
        /// </summary>
        protected float speed;

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        void Start()
        {
            stateMachine = new IceMilkTea.Core.ImtStateMachine<Battler>(this);
            stateMachine.AddAnyTransition<IdleState>((int)StateEventId.Idle);
            stateMachine.AddTransition<IdleState, AttackState>((int)StateEventId.Attack);
            stateMachine.AddTransition<IdleState, DamageState>((int)StateEventId.Damage);
            stateMachine.AddTransition<IdleState, WinState>((int)StateEventId.Win);
            stateMachine.AddTransition<IdleState, LoseState>((int)StateEventId.Lose);
            stateMachine.AddTransition<WinState, LeaveState>((int)StateEventId.Leave);
            stateMachine.SetStartState<EnterState>();

            stepTime = 0.0f;
            stepSpriteIndex = 1;
            speed = 3.0f;
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        void Update()
        {
            if ((stepTime += Time.deltaTime) > 0.3f)
            {
                stepSpriteIndex = stepSpriteIndex * -1;
                stepTime = 0;
            }

            int spriteIndex = ((int)Direction * PATTERN_NUM_BY_DIRECTION) + 1 + stepSpriteIndex;
            var sr = GetComponent<SpriteRenderer>();
            sr.sprite = sprites[spriteIndex];

            stateMachine.Update();
        }

        /// <summary>
        /// 入場させる
        /// </summary>
        /// <param name="dir"></param>
        public void Enter(Directions dir)
        {
            Direction = dir;

            var sr = GetComponent<SpriteRenderer>();
            sr.sprite = sprites[((int)Direction * PATTERN_NUM_BY_DIRECTION) + 1];
        }

        /// <summary>
        /// アイドル状態か判定する
        /// </summary>
        /// <returns></returns>
        public bool IsIdle()
        {
            return stateMachine.Running && stateMachine.IsCurrentState<IdleState>();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ActionAttack()
        {
            return stateMachine.SendEvent((int)StateEventId.Attack);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ActionDamage()
        {
            return stateMachine.SendEvent((int)StateEventId.Damage);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ActionWin()
        {
            return stateMachine.SendEvent((int)StateEventId.Win);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ActionLose()
        {
            return stateMachine.SendEvent((int)StateEventId.Lose);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ActionLeave()
        {
            return stateMachine.SendEvent((int)StateEventId.Leave);
        }

        /// <summary>
        /// Enter ステート
        /// </summary>
        private class EnterState : IceMilkTea.Core.ImtStateMachine<Battler>.State
        {
            protected internal override void Enter()
            {
                var half = Screen.width / 2;

                var pos = Context.transform.position;
                if (Context.Direction == Directions.DIR_RIGHT)
                {
                    pos.x = -half;
                }
                else if (Context.Direction == Directions.DIR_LEFT)
                {
                    pos.x = half;
                }
                Context.transform.position = pos;
            }

            protected internal override void Update()
            {
                var pos = Context.transform.position;
                if (Context.Direction == Directions.DIR_RIGHT)
                {
                    pos.x += Context.speed;

                    var offset = Context.GetComponent<SpriteRenderer>().sprite.rect.width;
                    if (pos.x > 0 - offset)
                    {
                        // Idle ステートへ
                        Context.stateMachine.SendEvent((int)StateEventId.Idle);
                    }
                }
                else if (Context.Direction == Directions.DIR_LEFT)
                {
                    pos.x -= Context.speed;

                    var offset = Context.GetComponent<SpriteRenderer>().sprite.rect.width;
                    if (pos.x < 0 + offset)
                    {
                        // Idle ステートへ
                        Context.stateMachine.SendEvent((int)StateEventId.Idle);
                    }
                }
                Context.transform.position = pos;
            }
        }

        /// <summary>
        /// Idle ステート
        /// </summary>
        private class IdleState : IceMilkTea.Core.ImtStateMachine<Battler>.State
        {
            protected internal override void Update()
            {
                // Attack ステートへ
                //Context.stateMachine.SendEvent((int)StateEventId.Damage);
            }
        }

        /// <summary>
        /// Attack ステート
        /// </summary>
        private class AttackState : IceMilkTea.Core.ImtStateMachine<Battler>.State
        {
            /// <summary>
            /// 
            /// </summary>
            private int tableIndex;

            /// <summary>
            /// 
            /// </summary>
            List<float> postionTable;

            /// <summary>
            /// 
            /// </summary>
            private float startPosX;

            protected internal override void Enter()
            {
                tableIndex = 0;
                postionTable = new List<float>()
                {
                    4, 5, 9, 11, 11, 14, 18, 14, 12, 9,
                    7, 4, 3
                };

                var pos = Context.transform.position;
                startPosX = pos.x;
            }

            protected internal override void Update()
            {
                var pos = Context.transform.position;
                if (Context.Direction == Directions.DIR_RIGHT)
                {
                    pos.x = startPosX + postionTable[tableIndex] * 2;
                    if (++tableIndex > postionTable.Count - 1)
                    {
                        tableIndex = 0;

                        // Idle ステートへ
                        Context.stateMachine.SendEvent((int)StateEventId.Idle);
                    }
                }
                else if (Context.Direction == Directions.DIR_LEFT)
                {
                    pos.x = startPosX - postionTable[tableIndex] * 2;
                    if (++tableIndex > postionTable.Count - 1)
                    {
                        tableIndex = 0;

                        // Idle ステートへ
                        Context.stateMachine.SendEvent((int)StateEventId.Idle);
                    }
                }
                Context.transform.position = pos;
            }

            protected internal override void Exit()
            {
                var pos = Context.transform.position;
                pos.x = startPosX;
                Context.transform.position = pos;
            }
        }

        /// <summary>
        /// Damage ステート
        /// </summary>
        private class DamageState : IceMilkTea.Core.ImtStateMachine<Battler>.State
        {
            /// <summary>
            /// 
            /// </summary>
            private int tableIndex;

            /// <summary>
            /// 
            /// </summary>
            List<float> postionTable;

            /// <summary>
            /// 
            /// </summary>
            private float startPosX;

            protected internal override void Enter()
            {
                tableIndex = 0;
                postionTable = new List<float>()
                {
                    00, 00, 00, 00, 00, 00, 00, 00, 04, 05,
                    09, 11, 11, 14, 18, 14, 12, 09, 07, 04,
                    03, 02, 01
                };

                var pos = Context.transform.position;
                startPosX = pos.x;
            }

            protected internal override void Update()
            {
                var pos = Context.transform.position;
                if (Context.Direction == Directions.DIR_RIGHT)
                {
                    pos.x = startPosX - postionTable[tableIndex];
                    if (++tableIndex > postionTable.Count - 1)
                    {
                        tableIndex = 0;

                        // Idle ステートへ
                        Context.stateMachine.SendEvent((int)StateEventId.Idle);
                    }
                }
                else if (Context.Direction == Directions.DIR_LEFT)
                {
                    pos.x = startPosX + postionTable[tableIndex];
                    if (++tableIndex > postionTable.Count - 1)
                    {
                        tableIndex = 0;

                        // Idle ステートへ
                        Context.stateMachine.SendEvent((int)StateEventId.Idle);
                    }
                }
                Context.transform.position = pos;
            }

            protected internal override void Exit()
            {
                var pos = Context.transform.position;
                pos.x = startPosX;
                Context.transform.position = pos;
            }
        }

        /// <summary>
        /// Win ステート
        /// </summary>
        private class WinState : IceMilkTea.Core.ImtStateMachine<Battler>.State
        {
            /// <summary>
            /// 
            /// </summary>
            private int tableIndex;

            /// <summary>
            /// 
            /// </summary>
            List<float> postionTable;

            /// <summary>
            /// 
            /// </summary>
            private float startPosY;

            protected internal override void Enter()
            {
                tableIndex = 0;
                postionTable = new List<float>()
                {
                    00, 00, 00, 00, 00, 00, 00, 00, 04, 05,
                    09, 11, 11, 14, 18, 14, 12, 09, 07, 04,
                    00, 00, 00, 00, 00, 00, 00, 00, 04, 05,
                    09, 11, 11, 14, 18, 14, 12, 09, 07, 04,
                    03, 02, 01
                };

                var pos = Context.transform.position;
                startPosY = pos.y;
            }

            protected internal override void Update()
            {
                var pos = Context.transform.position;
                pos.y = startPosY + postionTable[tableIndex];
                if (++tableIndex > postionTable.Count - 1)
                {
                    tableIndex = 0;

                    // Idle ステートへ
                    Context.stateMachine.SendEvent((int)StateEventId.Leave);
                }
                Context.transform.position = pos;
            }

            protected internal override void Exit()
            {
                var pos = Context.transform.position;
                pos.y = startPosY;
                Context.transform.position = pos;
            }
        }

        /// <summary>
        /// Lose ステート
        /// </summary>
        private class LoseState : IceMilkTea.Core.ImtStateMachine<Battler>.State
        {
            protected internal override void Update()
            {
                var spriteRenderer = Context.GetComponent<SpriteRenderer>();
                var color = spriteRenderer.color;
                color.a = color.a - 0.05f;
                spriteRenderer.color = color;

                if (color.a < 0)
                {
                    // Idle ステートへ
                    Context.stateMachine.SendEvent((int)StateEventId.Idle);
                }
            }
        }

        /// <summary>
        /// Leave ステート
        /// </summary>
        private class LeaveState : IceMilkTea.Core.ImtStateMachine<Battler>.State
        {
            float targetX = 0; 

            protected internal override void Enter()
            {
                var offset = Context.GetComponent<SpriteRenderer>().sprite.rect.width;

                if (Context.Direction == Directions.DIR_RIGHT)
                {
                    targetX = Screen.width / 2 + offset;
                }
                else if (Context.Direction == Directions.DIR_LEFT)
                {
                    targetX = 0 - offset;
                }
            }

            protected internal override void Update()
            {
                var pos = Context.transform.position;
                if (Context.Direction == Directions.DIR_RIGHT)
                {
                    pos.x += Context.speed;

                    if (pos.x > targetX)
                    {
                        // Idle ステートへ
                        Context.stateMachine.SendEvent((int)StateEventId.Idle);
                    }
                }
                else if (Context.Direction == Directions.DIR_LEFT)
                {
                    pos.x -= Context.speed;

                    if (pos.x < targetX)
                    {
                        // Idle ステートへ
                        Context.stateMachine.SendEvent((int)StateEventId.Idle);
                    }
                }
                Context.transform.position = pos;
            }
        }
    }
}
