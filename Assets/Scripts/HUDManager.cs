using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [SerializeField] Image healthBarImage;
    
    [SerializeField] Image temperatureBar;
    [SerializeField] GameObject overheatWarning;
    [SerializeField] GameObject heatWarningText;

    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI healthAmountText;
    [SerializeField] TextMeshProUGUI missileCountText;

    Color healthBarFullColor;
    Color healthBarCurrentColor;
    GameObject player;
    PlayerShooter playerShooter;

    int currentScore = 0;
    float playerMaxHealth;
    float playerCurrentHealth;
    float temperature;
    float redIncrement, greenIncrement, blueIncrement;

    private void Awake() 
    {
        EventManager.AddIntArgumentListener(UpdateMissileCount, EventType.MissileFired);
        EventManager.AddGameObjectArgumentListener(UpdateScore, EventType.EnemyDestroyed);
        EventManager.AddFloatArgumentListener(UpdatePlayerHealth, EventType.HealthChanged);
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerShooter = player.GetComponent<PlayerShooter>();

        Health playerHealthScript = player.GetComponent<Health>();
        playerMaxHealth = playerHealthScript.MaxHealth;
        playerCurrentHealth = playerMaxHealth;
        healthBarFullColor = healthBarImage.color;

        redIncrement = (1 - healthBarFullColor.r) / 100;
        greenIncrement = (1 - healthBarFullColor.g) / 100;
        blueIncrement = (1 - healthBarFullColor.b) / 100;

        healthAmountText.text = (PlayerHealthRatio() * 100).ToString() + "%";

        scoreText.text = $"Targets Dispatched {currentScore}";
    }

    private void Update()
    {
        AdjustTemperatureBarColor();
    }

    private void AdjustTemperatureBarColor()
    {
        temperature = playerShooter.Temperature;

        if (temperature >= 100)
        {
            StartCoroutine(DisplayOverheatWarning());
        }
        temperatureBar.fillAmount = temperature / 100;
        temperatureBar.color = new Color(temperatureBar.color.r,
                                            2f - 2 * temperatureBar.fillAmount,
                                            1f - 2 * temperatureBar.fillAmount,
                                            temperatureBar.color.a);
    }

    private IEnumerator DisplayOverheatWarning()
    {
        overheatWarning.SetActive(true);
        heatWarningText.SetActive(true);
        yield return new WaitUntil(() => temperature < 100);
        overheatWarning.SetActive(false);
        heatWarningText.SetActive(false);
    }

    private void UpdateScore(GameObject obj)
    {
        currentScore += 1;
        scoreText.text = $"Targets Dispatched {currentScore}";
    }

    private void UpdatePlayerHealth(float updatedValue)
    {
        playerCurrentHealth = updatedValue;
        if (playerCurrentHealth < 0)
        {
            playerCurrentHealth = 0;
        }
        
        float ratio = PlayerHealthRatio();

        healthAmountText.text = ((int)(ratio * 100)).ToString() + "%";

        healthBarImage.fillAmount = ratio;
        float missingHealth = playerMaxHealth - playerCurrentHealth;

        healthBarCurrentColor = new Color(healthBarImage.color.r + redIncrement * missingHealth,
                                                healthBarImage.color.g - greenIncrement * missingHealth,
                                                healthBarImage.color.b - blueIncrement * missingHealth / 2);

        healthBarImage.color = healthBarCurrentColor;
    }

    private void UpdateMissileCount(int count)
    {
        missileCountText.text = count.ToString();
    }

    float PlayerHealthRatio()
    {
        return playerCurrentHealth / playerMaxHealth;
    }


}
