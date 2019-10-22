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

        // プレイヤーの初期ステータスを作成
        var playerId = Assets.Scripts.Database.PlayerDatabase.Id.Warrior;
        var playerStatus = Assets.Scripts.Database.PlayerDatabase.GetStatus(playerId);

        //var firstScene = gameObject.AddComponent<GameScene>();
        var param = new BattleSceneParam();
        param.PlayerStatus = playerStatus;
        param.EncountId = Assets.Scripts.Database.EnemyDatabase.Id.Slime;
        var firstScene = CreateScene<BattleScene>(param);
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

    /// <summary>
    /// シーン作成
    /// </summary>
    /// <typeparam name="Type"></typeparam>
    /// <returns></returns>
    public Type CreateScene<Type>() where Type : SceneBase
    {
        return CreateScene<Type>(null);
    }

    /// <summary>
    /// シーン作成
    /// </summary>
    /// <typeparam name="Type"></typeparam>
    /// <returns></returns>
    public Type CreateScene<Type>(SceneParam param) where Type : SceneBase
    {
        Type newScene = gameObject.AddComponent<Type>();
        newScene.SceneParam = param;
        newScene.GameManager = this;
        newScene.GridGameObject = grid;
        newScene.CanvasGameObject = canvas;
        newScene.FadeManager = fadeManager;
        return newScene;
    }
}
