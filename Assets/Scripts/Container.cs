using UnityEngine;
using System.Collections;

public class Container : MonoBehaviour
{
    public Move move;
    GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && move != null)
        {
            gameManager.SwapPieces(move);
        }
    }
}
