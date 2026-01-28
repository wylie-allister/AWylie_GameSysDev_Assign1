using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    public TetrisManager tM;

    public void UpdateScore()
    {
        scoreText.text = $"SCORE: {tM.score:n0}";
    }
}
