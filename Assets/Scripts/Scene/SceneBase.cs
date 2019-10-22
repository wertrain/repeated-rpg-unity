using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Scene
{
    /// <summary>
    /// シーンのパラメータ
    /// </summary>
    public class SceneParam
    {

    }

    /// <summary>
    /// シーンの基底クラス
    /// </summary>
    public class SceneBase : MonoBehaviour
    {
        /// <summary>
        /// GameManager インスタンス
        /// </summary>
        public GameManager GameManager { get; set; }

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

        /// <summary>
        /// シーンパラメータの設定
        /// </summary>
        /// <param name="param"></param>
        public SceneParam SceneParam { get; set; }
}
}