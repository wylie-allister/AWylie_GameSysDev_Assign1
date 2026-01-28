using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    public TetrisManager tM;

    public GameObject endGamePanel;
    public void UpdateScore()
    {
        scoreText.text = $"SCORE: {tM.score:n0}";
    }

    public void UpdateGameOver()
    {
        endGamePanel.SetActive(tM.gameOver);
    }

    public void PlayAgain()
    {
        tM.SetGameOver(false);
    }
}
