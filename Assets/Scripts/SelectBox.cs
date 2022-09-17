using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectBox : MonoBehaviour
{
    public float moveSnapDistance;
    public GameObject phantomTileSample;
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
                if (Input.mousePosition.x > initialPosition) PreviewMovement(true);
                else PreviewMovement(false);
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
            phantomTile = Instantiate<GameObject>(phantomTileSample, tile.transform.position, tile.transform.rotation);
            phantomTile.GetComponent<MeshRenderer>().material = tile.GetComponent<CommonTile>().pahntomMaterial;
            phantomTiles.Add(phantomTile);
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

    private void PreviewMovement(bool isMovingRight)
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
            GameObject[,] newCollumns = new GameObject[8,8];
            CopyCollumns(game.collumns, newCollumns);
            foreach (GameObject tile in game.selectedTiles)
                newCollumns[tile.GetComponent<CommonTile>().collumn, tile.GetComponent<CommonTile>().row] = null;
            foreach (GameObject tile in game.selectedTiles)
            {
                int[] shiftedTile = new int[2];
                CommonTile tileMover = tile.GetComponent<CommonTile>();
                newCollumns[tileMover.collumn + rightShift, tileMover.row] = game.collumns[tileMover.collumn, tileMover.row];
                shiftedTile[0] = tileMover.collumn;
                shiftedTile[1] = tileMover.row;
                shiftedTiles.Add(shiftedTile);
                tileMover.MoveCollumn(rightShift);
                while ((game.collumns[tileMover.collumn, Mathf.Max(tileMover.row - 1, 0)] == null)&&(tileMover.row - 1 >= 0))
                {
                    newCollumns[tileMover.collumn, tileMover.row - 1] = newCollumns[tileMover.collumn, tileMover.row];
                    newCollumns[tileMover.collumn, tileMover.row] = null;
                    tileMover.row--;
                }
                tileMover.Fall();
            }
            CopyCollumns(newCollumns, game.collumns);
            foreach (int[] position in shiftedTiles) if (game.collumns[position[0], position[1]] == null) CheckFallingTiles(position);//check falling tiles
            game.HandleTurn();
        }
        foreach (GameObject tile in phantomTiles) Destroy(tile);
    }

    private void CheckFallingTiles(int[] position)
    {
        if (position[1] != 7)
            if (game.collumns[position[0], position[1] + 1] != null)
            {
                game.collumns[position[0], position[1]] = game.collumns[position[0], position[1] + 1];
                game.collumns[position[0], position[1] + 1] = null;
                game.collumns[position[0], position[1]].GetComponent<CommonTile>().row--;
                game.collumns[position[0], position[1]].GetComponent<CommonTile>().Fall();
                int[] newPosition = { position[0], position[1] + 1 };
                CheckFallingTiles(newPosition);
            }
    }

    private void CopyCollumns(GameObject[,] inCollumns, GameObject[,] outCollumns)
    {
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                outCollumns[i, j] = inCollumns[i, j];
    }
}
