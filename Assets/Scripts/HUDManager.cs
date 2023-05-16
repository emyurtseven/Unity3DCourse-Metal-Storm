using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    int currentScore = 0;

    [SerializeField] Image healthBarImage;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI healthAmountText;

    void Start()
    {
        EventManager.AddPointAddedListener(UpdateScore);
        scoreText.text = $"Targets Dispatched {currentScore}";
    }

    private void UpdateScore(int score)
    {
        currentScore += score;
        scoreText.text = $"Targets Dispatched {currentScore}";
    }


}
