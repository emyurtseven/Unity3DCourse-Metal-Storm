using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreKeeper : MonoBehaviour
{
    int currentScore = 0;

    TextMeshProUGUI scoreText;

    // Start is called before the first frame update
    void Start()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
        EventManager.AddPointAddedListener(UpdateScore);
        scoreText.text = $"Targets Dispatched {currentScore}";
    }

    private void UpdateScore(int score)
    {
        currentScore += score;

        scoreText.text = $"Targets Dispatched {currentScore}";
    }


}
