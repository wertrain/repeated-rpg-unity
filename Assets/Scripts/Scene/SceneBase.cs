using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Scene
{
    public class SceneBase : MonoBehaviour
    {
        /// <summary>
        /// グリッド
        /// </summary>
        public GameObject GridGameObject { get; set; }

        /// <summary>
        /// キャンバス
        /// </summary>
        public GameObject CanvasGameObject { get; set; }

        /// <summary>
        /// フェード管理
        /// </summary>
        public FadeManager FadeManager { get; set; }
    }
}