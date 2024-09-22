using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Piece : MonoBehaviour
{
    public enum pieceType { KING, QUEEN, BISHOP, ROOK, KNIGHT, PAWN, UNKNOWN = -1};
    public enum playerColor { BLACK, WHITE, UNKNOWN = -1};

    [SerializeField] private pieceType type = pieceType.UNKNOWN;
    [SerializeField] private playerColor player = playerColor.UNKNOWN;
    public pieceType Type
    {
        get { return type; }
    }
    public playerColor Player
    {
        get { return player; }
    }

    public Sprite pieceSprite = null;
    public Vector2 position;
    private Vector3 move;
    private GameManager gameManager;

    private MoveManager factory = new MoveManager(Board.Instance);
    private List<Move> moves = new List<Move>();

    private bool isMoved = false;
    public bool IsMoved
    {
        get { return isMoved; }
        set { isMoved = value; }
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && player == playerColor.WHITE && gameManager.playerTurn)
        {
            moves.Clear();
            GameObject[] objects = GameObject.FindGameObjectsWithTag("Highlight");
            foreach (GameObject o in objects)
            {
                Destroy(o);
            }

            moves = factory.GetMoves(this, position);
            foreach (Move move in moves)
            {
                if (move.pieceCaptured == null)
                {
                    GameObject instance = Instantiate(Resources.Load("MoveTile")) as GameObject;
                    instance.transform.position = new Vector3(-move.secondPosition.Position.x, 0, move.secondPosition.Position.y);
                    instance.GetComponent<Container>().move = move;
                }
                else if (move.pieceCaptured != null)
                {
                    GameObject instance = Instantiate(Resources.Load("CaptureTile")) as GameObject;
                    instance.transform.position = new Vector3(-move.secondPosition.Position.x, 0, move.secondPosition.Position.y);
                    instance.GetComponent<Container>().move = move;
                }
            }
            GameObject i = Instantiate(Resources.Load("CurrentPiece")) as GameObject;
            i.transform.position = this.transform.position;
        }
    }

    public void MovePiece(Vector3 position)
    {
        move = position;
    }

    void Start()
    {
        move = this.transform.position;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    void Update()
    {
        transform.position = Vector3.Lerp(this.transform.position, move, 9 * Time.deltaTime);
    }
}
