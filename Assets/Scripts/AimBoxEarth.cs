using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimBoxEarth : AimBox
{
    protected override void CastSpell()
    {
        base.CastSpell();
        for (int i = 0; i < 2; i++)
        {
            if (game.collumns[collumn, row] != null)
                game.blastedTiles.Add(game.collumns[collumn, row]);
            collumn += Mathf.RoundToInt(2 * xShift);
        }
        foreach (GameObject tile in game.blastedTiles)
        {
            if (tile.GetComponent<CommonTile>().row != 0)
            {
                ShiftUp(game.collumns[tile.GetComponent<CommonTile>().collumn, tile.GetComponent<CommonTile>().row - 1]);
                tile.GetComponent<CommonTile>().row = 0;
                game.collumns[tile.GetComponent<CommonTile>().collumn, 0] = tile;
                tile.GetComponent<CommonTile>().Fall();
            }
        }
        if (row != 0)
        {
            game.earthMana = 0;
            game.earthButton.interactable = false;
            game.earthButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Earth: 0/100";
        }
        game.blastedTiles = new List<GameObject>();
    }

    private void ShiftUp(GameObject tile)
    {
        CommonTile tileMover = tile.GetComponent<CommonTile>();
        game.collumns[tileMover.collumn, tileMover.row + 1] = tile;
        if (tileMover.row != 0)
            ShiftUp(game.collumns[tileMover.collumn, tileMover.row - 1]);
        tileMover.row++;
        tile.transform.Translate(Vector3.up);
    }
}
