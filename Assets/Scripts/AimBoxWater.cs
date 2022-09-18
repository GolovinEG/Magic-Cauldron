using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimBoxWater : AimBox
{
    protected override void CastSpell()
    {
        base.CastSpell();
        Flood(collumn, row, 1);
        if ((collumn > 0) && (row > 0))
            Flood(collumn - 1, row - 1, -1);
        foreach (GameObject tile in game.blastedTiles)
        {
            GameObject spawnedTile = Instantiate<GameObject>(game.commonTiles[3]);
            spawnedTile.transform.position = tile.transform.position;
            spawnedTile.GetComponent<CommonTile>().row = tile.GetComponent<CommonTile>().row;
            spawnedTile.GetComponent<CommonTile>().collumn = tile.GetComponent<CommonTile>().collumn;
            Debug.Log(spawnedTile.GetComponent<CommonTile>().collumn);
            game.collumns[tile.GetComponent<CommonTile>().collumn, tile.GetComponent<CommonTile>().row] = spawnedTile;
            Destroy(tile);
        }
        if (game.blastedTiles.Count > 0)
        {
            game.waterMana = 0;
            game.waterButton.interactable = false;
            game.waterButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Water: 0/100";
        }
        game.blastedTiles = new List<GameObject>();
    }

    private void Flood(int collumn, int row, int goingRight)
    {
        if (game.collumns[collumn, row] != null)
            game.blastedTiles.Add(game.collumns[collumn, row]);
        if (((collumn + goingRight) >= 0) && ((row - 1) >= 0))
            Flood(collumn + goingRight, row - 1, goingRight);
    }
}
