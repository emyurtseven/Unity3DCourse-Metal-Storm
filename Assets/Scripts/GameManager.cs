using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [SerializeField] float levelRestartDelay = 3f;
    // Start is called before the first frame update
    void Start()
    {
        ObjectPool.Initialize();
    }

    public void RestartLevel(GameObject player)
    {
        StartCoroutine(RestartCoroutine(player));
    }

    private IEnumerator RestartCoroutine(GameObject player)
    {
        player.transform.parent.GetComponent<Animator>().enabled = false;
        Destroy(player);

        yield return new WaitForSeconds(levelRestartDelay);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
