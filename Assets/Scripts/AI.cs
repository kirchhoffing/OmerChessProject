using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI
{
    int maxDepth = 4;

    List<Move> moves = new List<Move>();
    List<Tile> tilesWithPieces = new List<Tile>();
    List<Tile> blackPieces = new List<Tile>();
    List<Tile> whitePieces = new List<Tile>();
    Stack<Move> moveStack = new Stack<Move>();
    Weights weight = new Weights();
    Tile[,] localBoard = new Tile[8, 8];
    int whiteScore = 0;
    int blackScore = 0;
    Move bestMove;

    Board board;

    float infinity = Mathf.Infinity;

    public Move GetMove()
    {
        board = Board.Instance;
        bestMove = CreateMove(board.GetTile(new Vector2(0, 0)), board.GetTile(new Vector2(0, 0)));
        AlphaBeta(maxDepth, -infinity, infinity, true);
        return bestMove;
    }

    float AlphaBeta(int depth, float alpha, float beta, bool maximizingPlayer)
    {
        GetBoardState();

        if (depth == 0)
        {
            return Evaluate();
        }
        if (maximizingPlayer)
        {
            float score = -infinity;
            List<Move> allMoves = GetMoves(Piece.playerColor.BLACK);
            foreach (Move move in allMoves)
            {
                moveStack.Push(move);

                DoFakeMove(move.firstPosition, move.secondPosition);

                score = AlphaBeta(depth - 1, alpha, beta, false);

                UndoFakeMove();

                if (score > alpha)
                {
                    move.score = score;
                    if (move.score > bestMove.score && depth == maxDepth)
                    {
                        bestMove = move;
                    }
                    alpha = score;
                }
                if (score >= beta)
                {
                    break;
                }
            }
            return alpha;
        }
        else
        {
            float score = infinity;
            List<Move> allMoves = GetMoves(Piece.playerColor.WHITE);
            foreach (Move move in allMoves)
            {
                moveStack.Push(move);

                DoFakeMove(move.firstPosition, move.secondPosition);

                score = AlphaBeta(depth - 1, alpha, beta, true);

                UndoFakeMove();

                if (score < beta)
                {
                    move.score = score;
                    beta = score;
                }
                if (score <= alpha)
                {
                    break;
                }
            }
            return beta;
        }
    }

    int Evaluate()
    {
        float pieceDiff = 0;
        float whiteWeight = 0;
        float blackWeight = 0;

        foreach (Tile tile in whitePieces)
        {
            whiteWeight += weight.GetBoardWeight(tile.CurrentPiece.Type, tile.CurrentPiece.position, Piece.playerColor.WHITE);
        }
        foreach (Tile tile in blackPieces)
        {
            blackWeight += weight.GetBoardWeight(tile.CurrentPiece.Type, tile.CurrentPiece.position, Piece.playerColor.BLACK);
        }
        pieceDiff = (blackScore + (blackWeight / 100)) - (whiteScore + (whiteWeight / 100));
        return Mathf.RoundToInt(pieceDiff * 100);
    }

    void UndoFakeMove()
    {
        Move tempMove = moveStack.Pop();
        Tile movedTo = tempMove.secondPosition;
        Tile movedFrom = tempMove.firstPosition;
        Piece pieceCaptured = tempMove.pieceCaptured;
        Piece pieceMoved = tempMove.pieceMoved;

        movedFrom.CurrentPiece = movedTo.CurrentPiece;

        if (pieceCaptured != null)
        {
            movedTo.CurrentPiece = pieceCaptured;
        }
        else
        {
            movedTo.CurrentPiece = null;
        }
    }

    void DoFakeMove(Tile currentTile, Tile targetTile)
    {
        targetTile.SwapFakePieces(currentTile.CurrentPiece);
        currentTile.CurrentPiece = null;
    }

    List<Move> GetMoves(Piece.playerColor color)
    {
        List<Move> turnMove = new List<Move>();
        List<Tile> pieces = new List<Tile>();

        if (color == Piece.playerColor.BLACK)
            pieces = blackPieces;
        else pieces = whitePieces;

        foreach (Tile tile in pieces)
        {
            MoveManager manager = new MoveManager(board);
            List<Move> pieceMoves = manager.GetMoves(tile.CurrentPiece, tile.Position);

            foreach (Move move in pieceMoves)
            {
                Move newMove = CreateMove(move.firstPosition, move.secondPosition);
                turnMove.Add(newMove);
            }
        }
        return turnMove;
    }

    

    void GetBoardState()
    {
        blackPieces.Clear();
        whitePieces.Clear();
        blackScore = 0;
        whiteScore = 0;
        tilesWithPieces.Clear();

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                localBoard[x, y] = board.GetTile(new Vector2(x, y));
                if (localBoard[x, y].CurrentPiece != null && localBoard[x, y].CurrentPiece.Type != Piece.pieceType.UNKNOWN)
                {
                    tilesWithPieces.Add(localBoard[x, y]);
                }
            }
        }
        foreach (Tile tile in tilesWithPieces)
        {
            if (tile.CurrentPiece.Player == Piece.playerColor.BLACK)
            {
                blackScore += weight.GetPieceWeight(tile.CurrentPiece.Type);
                blackPieces.Add(tile);
            }
            else
            {
                whiteScore += weight.GetPieceWeight(tile.CurrentPiece.Type);
                whitePieces.Add(tile);
            }
        }
    }

    Move CreateMove(Tile tile, Tile move)
    {
        Move tempMove = new Move();
        tempMove.firstPosition = tile;
        tempMove.pieceMoved = tile.CurrentPiece;
        tempMove.secondPosition = move;

        if (move.CurrentPiece != null)
        {
            tempMove.pieceCaptured = move.CurrentPiece;
        }

        return tempMove;
    }
}
