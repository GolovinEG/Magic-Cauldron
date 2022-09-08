using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectBox : MonoBehaviour
{
    public float moveSnapDistance;
    public Material[] phantomMaterials;

    private int row, rightShift;
    private int[] selectBorders = new int[2];
    private int[] movementBorders = new int[2];
    private float initialPosition;
    private List<GameObject> phantomTiles = new List<GameObject>();
    private GameManager game;
    // Start is called before the first frame update
    void Start()
    {
        game = GameObject.Find("GameManager").GetComponent<GameManager>();
        row = Mathf.FloorToInt(transform.position.y + 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (game.isMoving)
        {

            if (!Input.GetMouseButton(0))
            {
                HandleMovement();
                game.UndoSelection();
            }
            else if (Mathf.Abs(Input.mousePosition.x - initialPosition) > moveSnapDistance)
            {
                if (Input.mousePosition.x > initialPosition) MoveSelection(true);
                else MoveSelection(false);
                initialPosition = Input.mousePosition.x;
            }
        }

    }

    private void OnMouseDown()
    {
        game.isMoving = true;
        GameObject phantomTile;
        initialPosition = Input.mousePosition.x;
        foreach (GameObject tile in game.selectedTiles)
        {
            phantomTile = Instantiate<GameObject>(tile).GetComponent<Tile>().GetPhantomTile();
            phantomTile.GetComponent<MeshRenderer>().material = GetElement(tile);
            phantomTiles.Add(phantomTile);
            //Instantiate<GameObject>(phantomTile);
        }
        rightShift = 0;
        selectBorders = GetSelectBorders();
        movementBorders = GetMovementBorders(selectBorders);
    }

    private Material GetElement(GameObject tile)
    {
        switch (tile.tag)
        {
            case "Air":
                return phantomMaterials[0];
            case "Earth":
                return phantomMaterials[1];
            case "Fire":
                return phantomMaterials[2];
            case "Water":
                return phantomMaterials[3];
            default:
                Debug.Log("GetElement error");
                return phantomMaterials[0];
        }
    }

    private void MoveSelection(bool isMovingRight)
    {
        int[] shiftedSelectBorders = {selectBorders[0] + rightShift, selectBorders[1] + rightShift};
        if (isMovingRight && (shiftedSelectBorders[1] < movementBorders[1]))
        {
            foreach (GameObject tile in phantomTiles) tile.transform.Translate(Vector3.right);
            rightShift++;
        }
        else if (!isMovingRight && (shiftedSelectBorders[0] > movementBorders[0]))
        {
            foreach (GameObject tile in phantomTiles) tile.transform.Translate(Vector3.left);
            rightShift--;
        }
    }

    private int[] GetSelectBorders()
    {
        int leftBorder = Mathf.FloorToInt(transform.position.x - 0.5f * (game.selectedTiles.Count - 1) + 4);
        int rightBorder = Mathf.FloorToInt(transform.position.x + 0.5f * (game.selectedTiles.Count - 1) + 4);
        int[] selectBorders = {leftBorder, rightBorder };
        return selectBorders;
    }

    private int[] GetMovementBorders(int[] selectBorders)
    {
        int leftBorder = -1;
        int rightBorder = -1;
        for (int i = 0; i < selectBorders[0]; i++)
        {
            if ((leftBorder == -1)&&(game.collumns[i, row] == null)) leftBorder = i;
            else if (game.collumns[i, row] != null) leftBorder = -1;
        };
        if (leftBorder == -1) leftBorder = selectBorders[0];
        for (int i = selectBorders[1] + 1; i < 8; i++)
        {
            if (game.collumns[i, row] == null) rightBorder = i;
            else break;
        };
        if (rightBorder == -1) rightBorder = selectBorders[1];
        int[] movementBorders = { leftBorder, rightBorder };
        return movementBorders;
    }

    private void HandleMovement()
    {
        if (rightShift != 0)
        {
            List<int[]> shiftedTiles = new List<int[]>();
            foreach(GameObject tile in game.selectedTiles)
            {
                int[] shiftedTile = new int[2];
                Tile tileMover = tile.GetComponent<Tile>();
                game.collumns[tileMover.GetCollumn() + rightShift, tileMover.GetRow()] = game.collumns[tileMover.GetCollumn(), tileMover.GetRow()];
                game.collumns[tileMover.GetCollumn(), tileMover.GetRow()] = null;
                shiftedTile[0] = tileMover.GetCollumn();
                shiftedTile[1] = tileMover.GetRow();
                shiftedTiles.Add(shiftedTile);
                tileMover.MoveCollumn(rightShift);
                while (game.collumns[tileMover.GetCollumn(), tileMover.GetRow() - 1] == null)
                {
                    game.collumns[tileMover.GetCollumn(), tileMover.GetRow() - 1] = game.collumns[tileMover.GetCollumn(), tileMover.GetRow()];
                    game.collumns[tileMover.GetCollumn(), tileMover.GetRow()] = null;
                    tileMover.LowerRow();
                }
                tileMover.Fall();
            }
            foreach (GameObject tile in phantomTiles) Destroy(tile);
            foreach (int[] position in shiftedTiles) if (game.collumns[position[0], position[1]] == null) CheckFallingTiles(position);//check falling tiles
        }
    }

    private void CheckFallingTiles(int[] position)
    {
        if (game.collumns[position[0], position[1] + 1] != null)
        {
            game.collumns[position[0], position[1]] = game.collumns[position[0], position[1] + 1];
            game.collumns[position[0], position[1] + 1] = null;
            game.collumns[position[0], position[1]].GetComponent<Tile>().LowerRow();
            game.collumns[position[0], position[1]].GetComponent<Tile>().Fall();
            int[] newPosition = { position[0], position[1] + 1 };
            CheckFallingTiles(newPosition);
        }
    }

    IEnumerator DelayDebug()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("DebugOn");
    }
}
