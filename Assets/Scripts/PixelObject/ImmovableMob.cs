using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// 配置モブ
    /// </summary>
    class ImmovableMob : CharacterBase
    {
         /// <summary>
         /// キャラ画像スプライトリスト
         /// </summary>
        public List<Sprite> sprites = new List<Sprite>();

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
            sr.sprite = sprites[1];

            stepTime = 0.0f;
            stepSpriteIndex = 1;
        }

        // Update is called once per frame
        void Update()
        {
            if ((stepTime += Time.deltaTime) > 0.5f)
            {
                stepSpriteIndex = stepSpriteIndex * -1;
                stepTime = 0;
            }

            int spriteIndex = 1 + stepSpriteIndex;
            var sr = GetComponent<SpriteRenderer>();
            sr.sprite = sprites[spriteIndex];
        }
    }
}
