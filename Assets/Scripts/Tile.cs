using UnityEngine;
using System.Collections;

public class Tile
{
    private Vector2 position = Vector2.zero;
    public Vector2 Position
    {
        get { return position; }
    }

    private Piece currentPiece = null;
    public Piece CurrentPiece
    {
        get { return currentPiece; }
        set { currentPiece = value; }
    }

    public Tile(int x, int y)
    {
        position.x = x;
        position.y = y;

        if (y == 0 || y == 1 || y == 6 || y == 7)
        {
            currentPiece = GameObject.Find(x.ToString() + " " + y.ToString()).GetComponent<Piece>();
        }
    }

    public void SwapFakePieces(Piece newPiece)
    {
        currentPiece = newPiece;
    }
}
