using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimBoxAir : AimBox
{
    protected override void CastSpell()
    {
        base.CastSpell();
        for (int gameCollumn = Mathf.Max(collumn - 1, 0); gameCollumn <= Mathf.Min(collumn + 1, 7); gameCollumn++)
            for (int gameRow = Mathf.Max(row - 1, 0); gameRow <= Mathf.Min(row + 1, 7); gameRow++)
                if (game.collumns[gameCollumn, gameRow] != null)
                    game.blastedTiles.Add(game.collumns[gameCollumn, gameRow]);
        if (game.blastedTiles.Count > 0)
        {
            game.airMana = 0;
            game.airButton.interactable = false;
            game.airButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Air: 0/100";
        }
        foreach (GameObject tile in game.blastedTiles)
        {
            game.collumns[tile.GetComponent<CommonTile>().collumn, tile.GetComponent<CommonTile>().row] = null;
            if (game.collumns[tile.GetComponent<CommonTile>().collumn, tile.GetComponent<CommonTile>().row + 1] != null)
                if (!game.blastedTiles.Contains(game.collumns[tile.GetComponent<CommonTile>().collumn, tile.GetComponent<CommonTile>().row + 1]))
                    tile.GetComponent<CommonTile>().FallAfterEvent();
        }
        foreach (GameObject tile in game.fallingTiles)
            tile.GetComponent<CommonTile>().Fall();
        foreach (GameObject tile in game.blastedTiles)
        {
            int lowestCollumn = 6;
            List<int> viableCollumns = new List<int>();
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (game.collumns[i, j] == null)
                        if (j <= lowestCollumn)
                        {
                            if (j < lowestCollumn)
                            {
                                lowestCollumn = j;
                                viableCollumns = new List<int>();
                            }
                            viableCollumns.Add(i);
                        }
            if (lowestCollumn < 6)
            {
                int spawnCollumn = viableCollumns[Random.Range(0, viableCollumns.Count)];
                Vector3 spawnPoint = new Vector3();
                int row = 0;
                for (int i = 0; i < 8; i++) if (game.collumns[spawnCollumn, i] == null)
                    {
                        spawnPoint = new Vector3(-3.5f + spawnCollumn, game.spawnHeight, 0);
                        row = i;
                        break;
                    }
                GameObject spawnedTile = Instantiate<GameObject>(tile, spawnPoint, tile.transform.rotation);
                spawnedTile.GetComponent<CommonTile>().collumn = spawnCollumn;
                spawnedTile.GetComponent<CommonTile>().row = row;
                game.collumns[spawnCollumn, row] = spawnedTile;
            }
        }
        foreach (GameObject tile in game.blastedTiles)
            Destroy(tile);
        game.blastedTiles = new List<GameObject>();
        game.fallingTiles = new List<GameObject>();
    }
}
