using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraTurnTile : CommonTile
{
    public override void Detonate()
    {
        game.isTurnFree = true;
        base.Detonate();
    }
}
