
namespace Assets.Scripts.PixelObject
{
    class BattlerBase : PixelObjectBase
    {
        public enum Directions
        {
            DIR_LEFT = 0,
            DIR_RIGHT,
            DIR_MAX
        };

        protected Directions Direction { get; set; }
    }
}
