using System;
using UnityEngine;

namespace Assets.Scripts.Database
{
    class PlayerDatabase
    {
        /// <summary>
        /// プレイヤーの 種別
        /// </summary>
        public enum Id
        {
            Warrior = 0,
            Num
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        static public BattlerStatus GetStatus(Id id)
        {
            BattlerStatus statusResource = (Resources.Load<BattlerStatus>("Data/Player/" + Enum.GetName(typeof(Id), id)));
            if (statusResource == null)
            {
                return null;
            }
            return GameObject.Instantiate(statusResource);
        }

    }
}
