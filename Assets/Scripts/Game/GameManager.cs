using Assets.Scripts.Scene;
using System.Collections.Generic;
using System.Linq;
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
    /// シーンのスタック
    /// </summary>
    private Stack<SceneBase> sceneStack;

    // Start is called before the first frame update
    void Start()
    {
        fadeManager = new FadeManager(canvas);
        sceneStack = new Stack<SceneBase>();

        //var firstScene = gameObject.AddComponent<GameScene>();
        var firstScene = gameObject.AddComponent<BattleScene>();
        firstScene.GridGameObject = grid;
        firstScene.CanvasGameObject = canvas;
        firstScene.FadeManager = fadeManager;

        PushScene(firstScene);
    }

    // Update is called once per frame
    void Update()
    {
        fadeManager.Update();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sceneBase"></param>
    private void PushScene(SceneBase sceneBase)
    {
        // 現在のシーンを一時停止する
        if (sceneStack.Count > 0)
        {
            var current = sceneStack.Last();
            current.enabled = false;
        }
        sceneStack.Push(sceneBase);
    }

    /// <summary>
    /// 
    /// </summary>
    private SceneBase PopScene()
    {
        if (sceneStack.Count == 0)
        {
            return null;
        }
        var current = sceneStack.Pop();

        // 次のシーンを再開する
        var next = sceneStack.Last();
        next.enabled = true;

        return current;
    }
}
