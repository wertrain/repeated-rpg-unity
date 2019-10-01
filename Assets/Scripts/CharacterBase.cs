using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : PixelObjectBase
{
    protected enum Directions
    {
        DIR_FRONT = 0,
        DIR_LEFT,
        DIR_RIGHT,
        DIR_BACKWARD,
        DIR_MAX
    };

    protected Directions Direction { get; set; }
}
