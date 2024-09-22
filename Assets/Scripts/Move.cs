using UnityEngine;
using System.Collections;

public class Move
{
    public Tile firstPosition = null;
    public Tile secondPosition = null;
    public Piece pieceMoved = null;
    public Piece pieceCaptured = null;
    public float score = -Mathf.Infinity;
}
