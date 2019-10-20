using UnityEngine;

namespace Assets.Scripts.System
{
    class AnimationFinishable : MonoBehaviour
    {
        public bool IsFinish { get; set; }

        public void OnAnimationFinish()
        {
            this.IsFinish = true;
        }
    }
}
