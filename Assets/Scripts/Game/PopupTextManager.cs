using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ポップアップテキストを管理
/// </summary>
public class PopupTextManager
{
    /// <summary>
    /// 一文字当たりの幅
    /// </summary>
    public static float CharWidth = 12.0f;

    /// <summary>
    /// キャンバスまでの参照
    /// </summary>
    private GameObject canvasGameObject;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="canvas"></param>
    public PopupTextManager(GameObject canvas)
    {
        canvasGameObject = canvas;
    }

    /// <summary>
    /// 指定位置に文字列をポップアップさせる
    /// </summary>
    /// <param name="text"></param>
    /// <param name="postion"></param>
    public void Popup(string text, Vector3 postion)
    {
        PopupText p = canvasGameObject.AddComponent<PopupText>();
        p.Popup(text, postion, canvasGameObject);
    }

    /// <summary>
    /// 一つのポップアップテキスト
    /// </summary>
    class PopupText : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        private GameObject canvasGameObject;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="postion"></param>
        /// <param name="canvas"></param>
        public void Popup(string text, Vector3 postion, GameObject canvas)
        {
            canvasGameObject = canvas;
            StartCoroutine(Execute(text, postion));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="postion"></param>
        /// <returns></returns>
        private IEnumerator Execute(string text, Vector3 postion)
        {
            var root = new GameObject();
            var canvasGroup = root.AddComponent<CanvasGroup>();
            root.name = "PopupText(\"" + text + "\")";
            root.transform.SetParent(canvasGameObject.transform);
            root.transform.SetAsFirstSibling();

            var texts = new List<Assets.Scripts.System.AnimationFinishable>();

            foreach (var c in text)
            {
                var obj = new GameObject();
                obj.transform.position = postion;
                obj.transform.SetParent(root.transform);

                // 1文字ずつ生成
                var popupTextPrefab = (GameObject)Resources.Load("Prefabs/System/PopupText");
                var valueText = Instantiate(popupTextPrefab, Vector3.zero, Quaternion.identity);
                var textComponent = valueText.GetComponent<Text>();
                textComponent.text = c.ToString();
                valueText.transform.SetParent(obj.transform);
                texts.Add(valueText.GetComponent<Assets.Scripts.System.AnimationFinishable>());

                // 0.03秒待つ(適当)
                yield return new WaitForSeconds(0.03f);

                // 次の位置
                postion.x += CharWidth;
            }

            // 適当に待ち
            while (!texts.TrueForAll(t => t.IsFinish))
            {
                yield return new WaitForSeconds(0.1f);
            }
            // フェードアウト
            for (int n = 9; n >= 0; n--)
            {
                canvasGroup.alpha = n / 10.0f;
                yield return new WaitForSeconds(0.01f);
            }

            // 破棄
            Destroy(root);
        }
    }
}
