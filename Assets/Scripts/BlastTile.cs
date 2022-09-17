using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastTile : CommonTile
{

    protected override void OnMouseOver()
    {
        
    }
    public override void Detonate()
    {
        int maxCollumn = Mathf.Min(collumn + 1, 7);
        int maxRow = Mathf.Min(row + 1, 7);
        for (int gameCollumn = Mathf.Max(collumn - 1, 0); gameCollumn <= maxCollumn; gameCollumn++)
            for (int gameRow = Mathf.Max(row - 1, 0); gameRow <= maxRow; gameRow++)
            {
                if (game.collumns[gameCollumn, gameRow] != null)
                {
                    GameObject tile = game.collumns[gameCollumn, gameRow];
                    if ((!game.blastedTiles.Contains(tile)) && (!game.detonatingTiles.Contains(tile)))
                    {
                        game.blastedTiles.Add(tile);
                        tile.GetComponent<CommonTile>().Detonate();
                    }
                }
            }
        base.Detonate();
    }
}
