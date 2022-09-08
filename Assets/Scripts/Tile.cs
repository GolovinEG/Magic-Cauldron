using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int fallFrames;

    public int collumn, row;
    private GameManager game;
    // Start is called before the first frame update
    private void Start()
    {
        game = GameObject.Find("GameManager").GetComponent<GameManager>();
        Fall();
    }

    private void OnMouseDown()
    {
        if (game.isSelectionActive) game.UndoSelection();
        else
        {
            game.isSelectionActive = true;
            GameObject selectionBox = Instantiate<GameObject>(game.selectionBoxSample, transform.position, game.selectionBoxSample.transform.rotation);
            game.selectedTiles.Add(gameObject);
        }
    }

    private void OnMouseEnter()
    {
        if (Input.GetMouseButton(0) && game.isSelectionActive && CheckSameRow() && CheckAdjacent() && !game.isMoving)
        {
            game.GetSelectionBox().transform.Translate(Vector3.right * CheckToTheRight() * 0.5f);
            game.selectedTiles.Add(gameObject);
            game.GetSelectionBox().transform.localScale = new Vector3(0.1f + game.selectedTiles.Count, 1.1f, 1.1f);
        }
    }

    public virtual void Fall()
    {
        int fallTiles = Mathf.RoundToInt(transform.position.y + 1.5f - row);
        StartCoroutine(ProcessFall(fallTiles));
    }

    public void SetPositionData(int inCollumn, int inRow)
    {
        collumn = inCollumn;
        row = inRow;
    }

    public int GetCollumn()
    {
        return collumn;
    }

    public int GetRow()
    {
        return row;
    }

    public void MoveCollumn(int rightShift)
    {
        collumn += rightShift;
        transform.Translate(Vector3.right * rightShift);
    }

    public void LowerRow()
    {
        row--;
    }

    private bool CheckSameRow()
    {
        bool isSameRow = Mathf.Approximately(gameObject.transform.position.y, game.GetSelectionBox().transform.position.y);
        return isSameRow;
    }

    private int CheckToTheRight()
    {
        if (transform.position.x > game.GetSelectionBox().transform.position.x) return 1;
        else return -1;
    }

    private bool CheckAdjacent()
    {
        bool isAdjacent = Mathf.Approximately(Mathf.Abs(transform.position.x - game.GetSelectionBox().transform.position.x) - 0.5f * (game.selectedTiles.Count + 1), 0);
        return isAdjacent;
    }

    public GameObject GetPhantomTile()
    {
        GameObject phantomTile = gameObject;
        phantomTile.AddComponent<PhantomTile>();
        phantomTile.GetComponent<Tile>().enabled = false;
        return phantomTile;
    }

    IEnumerator ProcessFall(int fallTiles)
    {
        for (int i = 0; i < fallTiles * fallFrames; i++) {
            yield return new WaitForEndOfFrame();
            transform.Translate(Vector3.down / fallFrames);
        }
    }
}
