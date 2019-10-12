using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : CharacterBase
{
    /// <summary>
    /// キャラ画像スプライトリスト
    /// </summary>
    public List<Sprite> sprites;

    /// <summary>
    /// アニメーションパターン数
    /// </summary>
    private const int PATTERN_NUM_BY_DIRECTION = 3;

    /// <summary>
    /// 足踏み用のタイマー
    /// </summary>
    private float stepTime;

    /// <summary>
    /// 足踏み用のインデックス
    /// </summary>
    private int stepSpriteIndex;

    /// <summary>
    /// プレイヤー移動速度
    /// </summary>
    private float speed;

    /// <summary>
    /// 
    /// </summary>
    public enum AutoMoveMode
    {
        Stop,
        Frontward,
        Left,
        Right,
        Backward
    };

    /// <summary>
    /// 自動移動中
    /// </summary>
    private AutoMoveMode autoMove;

    /// <summary>
    /// 自動移動時間
    /// </summary>
    private float autoMoveTime;

    // Start is called before the first frame update
    void Start()
    {
        Direction = Directions.DIR_BACKWARD;

        var sr = GetComponent<SpriteRenderer>();
        sr.sprite = sprites[((int)Direction * PATTERN_NUM_BY_DIRECTION) + 1];

        stepTime = 0.0f;
        stepSpriteIndex = 1;
        speed = 3.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (autoMove == AutoMoveMode.Stop)
        {
            stepSpriteIndex = 0;
        }
        else
        {
            if ((stepTime += Time.deltaTime) > autoMoveTime)
            {
                stepSpriteIndex = stepSpriteIndex * -1;
                stepTime = 0;
            }

            var pos = transform.position;
            switch (autoMove)
            {
                case AutoMoveMode.Frontward:
                    pos.y += speed;
                    break;
                case AutoMoveMode.Left:
                    pos.x -= speed;
                    break;
                case AutoMoveMode.Right:
                    pos.x += speed;
                    break;
                case AutoMoveMode.Backward:
                    pos.y -= speed;
                    break;
            }            
            transform.position = pos;
        }

        int spriteIndex = ((int)Direction * PATTERN_NUM_BY_DIRECTION) + 1 + stepSpriteIndex;
        var sr = GetComponent<SpriteRenderer>();
        sr.sprite = sprites[spriteIndex];
    }

    /// <summary>
    /// 
    /// </summary>
    public void StartAutoMove(AutoMoveMode mode, float time)
    {
        autoMove = mode;
        switch (autoMove)
        {
            case AutoMoveMode.Frontward:
                Direction = Directions.DIR_BACKWARD;
                break;
            case AutoMoveMode.Left:
                Direction = Directions.DIR_LEFT;
                break;
            case AutoMoveMode.Right:
                Direction = Directions.DIR_RIGHT;
                break;
            case AutoMoveMode.Backward:
                Direction = Directions.DIR_FRONT;
                break;
        }
        autoMoveTime = time;
        stepSpriteIndex = 1;
    }

    public void StopAutoMove()
    {
        autoMove = AutoMoveMode.Stop;
        stepSpriteIndex = 0;
    }
}
