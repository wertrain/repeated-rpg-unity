using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Database
{
    /// <summary>
    /// 敵データベース
    /// </summary>
    class EnemyDatabase
    {
        /// <summary>
        /// 敵の ID
        /// </summary>
        public enum Id
        {
            Slime = 0,
            Num
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        static public BattlerStatus GetStatus(Id id)
        {
            BattlerStatus statusResource = (Resources.Load<BattlerStatus>("Data/Enemy/" + Enum.GetName(typeof(Id), id)));
            if (statusResource == null)
            {
                return null;
            }
            return GameObject.Instantiate(statusResource);
        }
    }
}
