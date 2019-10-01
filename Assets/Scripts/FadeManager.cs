using UnityEngine;
using UnityEngine.UI;

public class FadeManager
{ 
    /// <summary>
    /// フェード状態
    /// </summary>
    private enum FadeState
    {
        Ready,
        In,
        Out,
        End
    };

    /// <summary>
    /// フェード速度
    /// </summary>
    public float fadeSpeed = 0.05f;

    /// <summary>
    /// フェード進行状態
    /// </summary>
    private FadeState fadeState;

    /// <summary>
    /// フェード用のパネル
    /// </summary>
    private GameObject fadePanel;

    /// <summary>
    /// 
    /// </summary>
    public FadeManager(GameObject canvas)
    {
        fadeState = FadeState.Ready;

        // フェード用のパネル生成
        var prefab = (GameObject)Resources.Load("Prefabs/System/FadePanel");
        fadePanel = Object.Instantiate(prefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        fadePanel.transform.SetParent(canvas.transform, false); // false でないと表示されない
        //fadePanel.transform.SetAsFirstSibling(); // ボタンを押させるために順序を変更
    }

    /// <summary>
    /// 
    /// </summary>
    public void Update()
    {
        var color = fadePanel.GetComponent<Image>().color;
        switch (fadeState)
        {
            case FadeState.Ready:
                return;
            case FadeState.In:
                if ((color.a -= fadeSpeed) < 0.0f)
                {
                    color.a = 0.0f;
                    fadeState = FadeState.End;
                    fadePanel.SetActive(false);
                }
                break;
            case FadeState.Out:
                if ((color.a += fadeSpeed) > 1.0f)
                {
                    color.a = 1.0f;
                    fadeState = FadeState.End;
                    fadePanel.SetActive(false);
                }
                break;
            case FadeState.End:
                return;
        }
        fadePanel.GetComponent<Image>().color = color;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Blackout()
    {
        fadePanel.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        fadePanel.SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    public void FadeIn()
    {
        fadePanel.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        fadeState = FadeState.In;
        fadePanel.SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    public void FadeOut()
    {
        fadePanel.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        fadeState = FadeState.Out;
        fadePanel.SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsFade()
    {
        return fadeState == FadeState.In || fadeState == FadeState.Out;
    }
}
