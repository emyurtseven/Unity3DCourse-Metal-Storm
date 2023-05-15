using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightEnemy : MonoBehaviour
{
    [SerializeField] GameObject reticlePrefab;
    [SerializeField] float refreshInterval = 1f;
    [SerializeField] float reticleMaxRange = 200f;

    Transform target;

    List<GameObject> reticles = new List<GameObject>();
    List<GameObject> enemiesInGame = new List<GameObject>();
    List<GameObject> enemiesOnScreen = new List<GameObject>();

    bool enemyInRangeX;
    bool enemyInRangeY;

    void Start()
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in allEnemies)
        {
            enemiesInGame.Add(enemy);
        }

        EventManager.AddEnemyDeadListener(RemoveEnemyFromList);

        StartCoroutine(HighlightEnemies());
    }

    void Update()
    {
        DrawReticles();
    }

    private void DrawReticles()
    {
        if (enemiesInGame.Count > 0)
        {
            for (int i = 0; i < enemiesOnScreen.Count; i++)
            {
                target = enemiesOnScreen[i].transform;

                reticles[i].transform.position = Camera.main.WorldToScreenPoint(target.position);
            }
        }
    }

    IEnumerator HighlightEnemies()
    {
        while (enemiesInGame.Count > 0)
        {
            for (int i = enemiesInGame.Count - 1; i >= 0; i--)
            {
                Vector3 enemyPos = Camera.main.WorldToViewportPoint(enemiesInGame[i].transform.position);

                enemyInRangeX = enemyPos.x > 0 && enemyPos.x < 1;
                enemyInRangeY = enemyPos.y > 0 && enemyPos.y < 1;

                if (enemyPos.z > 0 && enemyPos.z < reticleMaxRange)
                {
                    if (enemyInRangeX && enemyInRangeY)
                    {
                        if (!enemiesOnScreen.Contains(enemiesInGame[i]))
                        {
                            enemiesOnScreen.Add(enemiesInGame[i]);
                            GameObject newCrosshair = Instantiate(reticlePrefab, enemyPos, Quaternion.identity, transform);
                            reticles.Add(newCrosshair);
                        }
                    }
                }
                else if(enemiesOnScreen.Contains(enemiesInGame[i]))
                {
                    enemiesOnScreen.RemoveAt(i);
                    reticles.RemoveAt(i);
                }
            }

            yield return new WaitForSeconds(refreshInterval);
        }
    }

    private void RemoveEnemyFromList(GameObject enemy)
    {
        enemiesInGame.Remove(enemy);
        enemiesOnScreen.Remove(enemy);
        Destroy(reticles[reticles.Count - 1]);
        reticles.RemoveAt(reticles.Count - 1);
    }
}
