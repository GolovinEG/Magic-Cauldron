using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimBoxFire : AimBox
{
    protected override void CastSpell()
    {
        base.CastSpell();
        for (int i = 0; i < 3; i++)
        {
            if (game.collumns[collumn, row] != null)
                if (game.collumns[collumn, row].tag != "Rare")
                    game.detonatingTiles.Add(game.collumns[collumn, row]);
            row++;
            if (row > 7)
                break;
        }
        foreach (GameObject tile in game.detonatingTiles)
        {
            if (game.collumns[tile.GetComponent<CommonTile>().collumn, tile.GetComponent<CommonTile>().row + 1] != null)
                if (!game.detonatingTiles.Contains(game.collumns[tile.GetComponent<CommonTile>().collumn, tile.GetComponent<CommonTile>().row + 1]))
                    tile.GetComponent<CommonTile>().FallAfterEvent();
        }
        game.score += game.detonatingTiles.Count;
        game.scoreText.text = "Score: " + game.score;
        if (game.detonatingTiles.Count > 0)
        {
            game.fireMana = 0;
            game.fireButton.interactable = false;
            game.fireButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Fire: 0/100";
        }
        foreach (GameObject tile in game.detonatingTiles)
            tile.GetComponent<CommonTile>().Detonate();
        foreach (GameObject tile in game.fallingTiles)
            tile.GetComponent<CommonTile>().Fall();
        game.detonatingTiles = new List<GameObject>();
        game.fallingTiles = new List<GameObject>();
    }
}
