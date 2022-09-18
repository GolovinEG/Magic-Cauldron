using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloomTile : CommonTile
{
    public override void Detonate()
    {
        game.airMana += 20;
        if (game.airMana >= 100)
        {
            game.airMana = 100;
            game.airButton.interactable = true;
        }
        game.airButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Air " + game.airMana + "/100";
        game.earthMana += 20;
        if (game.earthMana >= 100)
        {
            game.earthMana = 100;
            game.earthButton.interactable = true;
        }
        game.earthButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Earth " + game.earthMana + "/100";
        game.fireMana += 20;
        if (game.fireMana >= 100)
        {
            game.fireMana = 100;
            game.fireButton.interactable = true;
        }
        game.fireButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Fire " + game.fireMana + "/100";
        game.waterMana += 20;
        if (game.waterMana >= 100)
        {
            game.waterMana = 100;
            game.waterButton.interactable = true;
        }
        game.waterButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Water " + game.waterMana + "/100";
        base.Detonate();
    }
}
