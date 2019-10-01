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

    // Start is called before the first frame update
    void Start()
    {
        Direction = Directions.DIR_FRONT;

        var sr = GetComponent<SpriteRenderer>();
        sr.sprite = sprites[((int)Direction * PATTERN_NUM_BY_DIRECTION) + 1];

        stepTime = 0.0f;
        stepSpriteIndex = 1;
    }

    // Update is called once per frame
    void Update()
    {
        // 右・左
        float x = Input.GetAxisRaw("Horizontal");

        // 上・下
        float y = Input.GetAxisRaw("Vertical");

        if (Input.anyKey)
        {
            if (y < 0.0f)
            {
                Direction = Directions.DIR_FRONT;
            }
            else
            {
                Direction = Directions.DIR_BACKWARD;
            }

            if (x < 0.0f)
            {
                Direction = Directions.DIR_LEFT;
            }
            else
            {
                Direction = Directions.DIR_RIGHT;
            }
        }

        // 移動する向きを求める
        Vector2 direction = new Vector2(x, y).normalized;

        if ((stepTime += Time.deltaTime) > 1.0f)
        {
            stepSpriteIndex = stepSpriteIndex * -1;
            stepTime = 0;
        }

        int spriteIndex = ((int)Direction * PATTERN_NUM_BY_DIRECTION) + 1 + stepSpriteIndex;
        var sr = GetComponent<SpriteRenderer>();
        sr.sprite = sprites[spriteIndex];
    }
}
