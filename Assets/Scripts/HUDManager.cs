using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    int currentScore = 0;
    float playerMaxHealth;
    float playerCurrentHealth;

    [SerializeField] Image healthBarImage;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI healthAmountText;

    void Start()
    {
        Health playerHealthScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        playerMaxHealth = playerHealthScript.MaxHealth;
        playerCurrentHealth = playerMaxHealth;

        healthAmountText.text = (PlayerHealthRatio() * 100).ToString() + "%";

        EventManager.AddGameObjectArgumentListener(UpdateScore, EventType.EnemyDestroyed);
        EventManager.AddFloatArgumentListener(UpdatePlayerHealth, EventType.HealthChanged);
        scoreText.text = $"Targets Dispatched {currentScore}";
    }

    private void UpdateScore(GameObject obj)
    {
        currentScore += 1;
        scoreText.text = $"Targets Dispatched {currentScore}";
    }

    private void UpdatePlayerHealth(float updatedValue)
    {
        playerCurrentHealth = updatedValue;
        float ratio = PlayerHealthRatio();

        healthAmountText.text = ((int)(ratio * 100)).ToString() + "%";

        healthBarImage.fillAmount = ratio;
    }

    float PlayerHealthRatio()
    {
        return playerCurrentHealth / playerMaxHealth;
    }


}
