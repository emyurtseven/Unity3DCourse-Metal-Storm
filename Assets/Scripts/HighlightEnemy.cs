using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// This script is placed on a canvas for highlighting and drawing reticles around enemies.
/// </summary>
public class HighlightEnemy : MonoBehaviour
{
    [SerializeField] GameObject reticlePrefab;      // reticle prefab to be drawn on enemies
    [SerializeField] float refreshInterval = 1f;
    [SerializeField] float reticleMaxRange = 200f;

    Transform target;

    List<GameObject> enemiesInGame = new List<GameObject>();
    Dictionary<GameObject, GameObject> reticleEnemyPair = new Dictionary<GameObject, GameObject>();

    int reticlesCount = 0;

    bool enemyInRangeX;
    bool enemyInRangeY;

    void Start()
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in allEnemies)
        {
            enemiesInGame.Add(enemy);
        }

        EventManager.AddEnemyDeadListener(RemovePair);

        StartCoroutine(DetectEnemiesOnScreen(refreshInterval));
    }

    void Update()
    {
        DrawReticles();
    }

    /// <summary>
    ///  Coroutine that periodically checks if enemies are on screen or not. Updates the lists accordingly.
    /// </summary>
    /// <param name="interval"> Update interval in seconds </param>
    IEnumerator DetectEnemiesOnScreen(float interval)
    {
        while (enemiesInGame.Count > 0)
        {
            for (int i = 0; i < enemiesInGame.Count; i++)
            {
                Vector3 enemyPos = Camera.main.WorldToViewportPoint(enemiesInGame[i].transform.position);

                enemyInRangeX = enemyPos.x > 0 && enemyPos.x < 1;
                enemyInRangeY = enemyPos.y > 0 && enemyPos.y < 1;

                if (enemyPos.z > 0 && enemyPos.z < reticleMaxRange)
                {
                    if (enemyInRangeX && enemyInRangeY)
                    {
                        if (!reticleEnemyPair.ContainsKey(enemiesInGame[i]))
                        {
                            GameObject newReticle = Instantiate(reticlePrefab, enemyPos, Quaternion.identity, transform);
                            reticleEnemyPair.Add(enemiesInGame[i], newReticle);
                            reticlesCount++;
                        }
                    }
                    else
                    {
                        RemovePair(enemiesInGame[i]);
                    }
                }
            }

            yield return new WaitForSeconds(interval);
        }
    }

    /// <summary>
    /// Update reticles' positions to draw on enemies.
    /// </summary>
    private void DrawReticles()
    {
        if (reticlesCount > 0)
        {
            foreach (KeyValuePair<GameObject, GameObject> pair in reticleEnemyPair)
            {
                if (pair.Key != null && pair.Value != null)
                {
                    target = pair.Key.transform;
                    pair.Value.transform.position = Camera.main.WorldToScreenPoint(target.position);
                }
            }
        }
    }

    /// <summary>
    /// Removes enemy-reticle pair from the dictionary.
    /// </summary>
    /// <param name="enemy"></param>
    private void RemovePair(GameObject enemy)
    {
        if (reticleEnemyPair.ContainsKey(enemy))
        {
            Destroy(reticleEnemyPair[enemy]);
            reticleEnemyPair.Remove(enemy);
            enemiesInGame.Remove(enemy);
            reticlesCount--;
        }
    }
}
