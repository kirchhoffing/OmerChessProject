using UnityEngine;
using System.Collections;

public class Board
{
    private static Board setBoard = null;
    public static Board Instance
    {
        get
        {
            if (setBoard == null)
            {
                setBoard = new Board();
            }
            return setBoard;
        }
    }

    private Tile[,] board = new Tile[8, 8];

    public void SetupBoard()
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                board[x, y] = new Tile(x, y);
            }
        }
    }

    public Tile GetTile(Vector2 tile)
    {
        return board[(int)tile.x, (int)tile.y];
    }
}
