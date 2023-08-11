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
    [SerializeField] bool debugMode = false;
    
    GameObject player;
    FollowPath playerPathFinder;
    bool sceneLoadedFromStart = false;

    public static GameManager Instance { get; private set; }
    public bool DebugMode { get => debugMode; }
    public bool SceneLoadedFromStart { get => sceneLoadedFromStart; set => sceneLoadedFromStart = value; }

    private void Awake()
    {
        Application.targetFrameRate = 30;
        SingletonThisObject();
        AudioManager.Initialize();
    }

    /// <summary>
    /// The game is initialized here instead of Start() to ensure they're called when scene is reloaded.
    /// </summary>
    /// <param name="scene"> Scene which is loaded </param>
    /// <param name="mode"> Unused, required for delegate </param>
    private void InitializeScene(Scene scene, LoadSceneMode mode)
    {
        // play start menu music and return, if we're in that scene
        if(scene.buildIndex == 0)
        {
            AudioManager.PlayMusicFadeIn(0, AudioClipName.MenuBackground, 1, 3f, 1f);
            return;
        }

        EventManager.Initialize();
        ObjectPool.Initialize();
        player = GameObject.FindGameObjectWithTag("Player");
        playerPathFinder = player.transform.parent.GetComponent<FollowPath>();

        if (sceneLoadedFromStart)
        {
            AudioManager.PlayMusicFadeIn(0, AudioClipName.CombatMusicLoop, musicVolume, 3f, 1f);
        }
    }

    /// <summary>
    /// Sets the player in motion after a set delay.
    /// </summary>
    public IEnumerator PlayerLiftOff()
    {
        yield return new WaitForSeconds(playerLiftOffDelay);

        if (playerPathFinder.enabled)
        {
            StartCoroutine(playerPathFinder.IterateOverCurves());
        }
    }

    public void RestartLevel(GameObject player)
    {
        sceneLoadedFromStart = false;
        StartCoroutine(RestartLevelCoroutine(player));
    }

    private IEnumerator RestartLevelCoroutine(GameObject player)
    {
        Destroy(player);

        yield return new WaitForSeconds(levelRestartDelay);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Add InitializeScene() method as a listener for scene changes
    void OnEnable()
    {
        SceneManager.sceneLoaded += InitializeScene;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= InitializeScene;
    }


    private void SingletonThisObject()
    {
        int numGameSessions = FindObjectsOfType<GameManager>().Length;

        if (numGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
