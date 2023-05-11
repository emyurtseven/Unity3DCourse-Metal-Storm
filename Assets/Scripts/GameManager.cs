using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [SerializeField] float levelRestartDelay = 3f;
    [SerializeField] float playerLiftOffDelay = 3f;
    [Range(0, 1f)]
    [SerializeField] float musicVolume = 1f;

    GameObject player;
    PathFinder playerPathFinder;

    private void Awake()
    {
        SingletonPattern();
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerPathFinder = player.transform.parent.GetComponent<PathFinder>();

        AudioManager.PlayMusic(AudioClipName.CombatMusicLoop, musicVolume);

        StartCoroutine(StartLevel());
    }

    public IEnumerator StartLevel()
    {
        yield return new WaitForSeconds(playerLiftOffDelay);

        if (playerPathFinder.enabled)
        {
            StartCoroutine(playerPathFinder.IterateOverCurves());
        }
    }

    public void RestartLevel(GameObject player)
    {
        StartCoroutine(RestartCoroutine(player));
    }

    private IEnumerator RestartCoroutine(GameObject player)
    {
        Destroy(player);

        yield return new WaitForSeconds(levelRestartDelay);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ObjectPool.Initialize();
    }

    private void SingletonPattern()
    {
        int numGameSessions = FindObjectsOfType<GameManager>().Length;

        if (numGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

}
