using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveManager
{
    Board board;
    List<Move> moves = new List<Move>();
    Dictionary<Piece.pieceType, System.Action> pieceFunction = new Dictionary<Piece.pieceType, System.Action>();

    private Piece piece;
    private Piece.pieceType type;
    private Piece.playerColor player;
    private Vector2 position;

    public MoveManager(Board _board)
    {
        board = _board;
        pieceFunction.Add(Piece.pieceType.PAWN, PawnMoves);
        pieceFunction.Add(Piece.pieceType.ROOK, RookMoves);
        pieceFunction.Add(Piece.pieceType.KNIGHT, KnightMoves);
        pieceFunction.Add(Piece.pieceType.BISHOP, BishopMoves);
        pieceFunction.Add(Piece.pieceType.QUEEN, QueenMoves);
        pieceFunction.Add(Piece.pieceType.KING, KingMoves);
    }

    public List<Move> GetMoves(Piece _piece, Vector2 _position)
    {
        piece = _piece;
        type = piece.Type;
        player = piece.Player;
        position = _position;

        foreach (KeyValuePair<Piece.pieceType, System.Action> p in pieceFunction)
        {
            if (type == p.Key)
            {
                p.Value.Invoke();
            }
        }

        return moves;
    }

    void PawnMoves()
    {
        if (piece.Player == Piece.playerColor.BLACK)
        {
            int limit = piece.IsMoved ? 1 : 2;
            MoveGenerate(limit, new Vector2(0, 1));

            Vector2 leftDiagonal = new Vector2(position.x - 1, position.y + 1);
            Vector2 rightDiagonal = new Vector2(position.x + 1, position.y + 1);
            Tile ldTile = null;
            Tile rdTile = null;
            if (OnBoard(leftDiagonal))
            {
                ldTile = board.GetTile(leftDiagonal);
            }
            if (OnBoard(rightDiagonal))
            {
                rdTile = board.GetTile(rightDiagonal);
            }

            if (ldTile != null && PieceCheck(ldTile) && EnemyCheck(ldTile))
            {
                MoveCheck(leftDiagonal);
            }
            if (rdTile != null && PieceCheck(rdTile) && EnemyCheck(rdTile))
            {
                MoveCheck(rightDiagonal);
            }
        }
        else
        {
            int limit = piece.IsMoved ? 1 : 2;
            MoveGenerate(limit, new Vector2(0, -1));

            Vector2 leftDiagonal = new Vector2(position.x - 1, position.y - 1);
            Vector2 rightDiagonal = new Vector2(position.x + 1, position.y - 1);
            Tile ldTile = null;
            Tile rdTile = null;
            if (OnBoard(leftDiagonal))
            {
                ldTile = board.GetTile(leftDiagonal);
            }
            if (OnBoard(rightDiagonal))
            {
                rdTile = board.GetTile(rightDiagonal);
            }

            if (ldTile != null && PieceCheck(ldTile) && EnemyCheck(ldTile))
            {
                MoveCheck(leftDiagonal);
            }
            if (rdTile != null && PieceCheck(rdTile) && EnemyCheck(rdTile))
            {
                MoveCheck(rightDiagonal);
            }
        }
    }

    void KnightMoves()
    {
        Vector2 move;
        move = new Vector2(position.x + 2, position.y + 1);
        MoveCheck(move);
        move = new Vector2(position.x + 2, position.y - 1);
        MoveCheck(move);
        move = new Vector2(position.x - 2, position.y + 1);
        MoveCheck(move);
        move = new Vector2(position.x - 2, position.y - 1);
        MoveCheck(move);

        move = new Vector2(position.x + 1, position.y - 2);
        MoveCheck(move);
        move = new Vector2(position.x + 1, position.y + 2);
        MoveCheck(move);
        move = new Vector2(position.x - 1, position.y + 2);
        MoveCheck(move);
        move = new Vector2(position.x - 1, position.y - 2);
        MoveCheck(move);
    }

    void BishopMoves()
    {
        MoveGenerate(8, new Vector2(1, 1));
        MoveGenerate(8, new Vector2(-1, -1));
        MoveGenerate(8, new Vector2(1, -1));
        MoveGenerate(8, new Vector2(-1, 1));
    }

    void RookMoves()
    {
        MoveGenerate(8, new Vector2(0, 1));
        MoveGenerate(8, new Vector2(0, -1));
        MoveGenerate(8, new Vector2(1, 0));
        MoveGenerate(8, new Vector2(-1, 0));
    }

    void QueenMoves()
    {
        BishopMoves();
        RookMoves();
    }

    void KingMoves()
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                MoveCheck(new Vector2(position.x + x, position.y + y));
            }
        }
    }


    void MoveGenerate(int limit, Vector2 direction)
    {
        for (int i = 1; i <= limit; i++)
        {
            Vector2 move = position + direction * i;
            if (OnBoard(move) && PieceCheck(board.GetTile(move)))
            {
                if (EnemyCheck(board.GetTile(move)) && type != Piece.pieceType.PAWN)
                {
                    MoveCheck(move);
                }
                break;
            }
            MoveCheck(move);
        }
    }

    void MoveCheck(Vector2 move)
    {
        if (OnBoard(move) && (!PieceCheck(board.GetTile(move)) || EnemyCheck(board.GetTile(move))))
        {
            Move m = new Move();
            m.firstPosition = board.GetTile(position);
            m.pieceMoved = piece;
            m.secondPosition = board.GetTile(move);

            if (m.secondPosition != null)
                m.pieceCaptured = m.secondPosition.CurrentPiece;

            moves.Add(m);
        }
    }
    bool PieceCheck(Tile tile)
    {
        if (!OnBoard(tile.Position))
            return false;

        if (tile.CurrentPiece != null)
            return true;
        else
            return false;
    }

    bool EnemyCheck(Tile tile)
    {
        if (player != tile.CurrentPiece.Player)
            return true;
        else
            return false;
    } 

    bool OnBoard(Vector2 point)
    {
        if (point.x >= 0 && point.y >= 0 && point.x < 8 && point.y < 8)
            return true;
        else
            return false;
    }
}
