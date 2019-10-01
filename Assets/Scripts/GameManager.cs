using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// グリッド
    /// </summary>
    public GameObject grid;

    /// <summary>
    /// キャンバス
    /// </summary>
    public GameObject canvas;

    /// <summary>
    /// フェード管理
    /// </summary>
    private FadeManager fadeManager;

    /// <summary>
    /// 現在のシーン
    /// </summary>
    private SceneBase currentScene;

    // Start is called before the first frame update
    void Start()
    {
        fadeManager = new FadeManager(canvas);

        currentScene = gameObject.AddComponent<GameScene>();
        currentScene.GridGameObject = grid;
        currentScene.CanvasGameObject = canvas;
        currentScene.FadeManager = fadeManager;
    }

    // Update is called once per frame
    void Update()
    {
        fadeManager.Update();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="param"></param>
    public void ButtonClick(GameObject param)
    {
        Debug.Log("ButtonClick::Update");
    }
}
