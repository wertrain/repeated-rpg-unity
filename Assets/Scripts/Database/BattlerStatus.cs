using UnityEngine;

namespace Assets.Scripts.Database
{
    [CreateAssetMenu(menuName = "データ作成/BattlerStatus 作成")]
    class BattlerStatus : ScriptableObject
    {
        /// <summary>
        /// 体力値
        /// </summary>
        public int LP = 0;

        /// <summary>
        /// スキル値
        /// </summary>
        public int SP = 0;

        /// <summary>
        /// 攻撃力
        /// </summary>
        public int ATK = 0;

        /// <summary>
        /// 防御力
        /// </summary>
        public int DEF = 0;

        /// <summary>
        /// 知力
        /// </summary>
        public int INT = 0;

        /// <summary>
        /// 素早さ
        /// </summary>
        public int AGI = 0;

        /// <summary>
        /// 運
        /// </summary>
        public int LUK = 0;

        /// <summary>
        /// レベル
        /// </summary>
        public int LV = 0;

        /// <summary>
        /// 経験値
        /// </summary>
        public int EXP = 0;
    }
}
