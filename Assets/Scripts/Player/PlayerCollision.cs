using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] GameObject airExplosionPrefab;
    [SerializeField] GameObject prefracturedPrefab;

    bool crashed;
    GameManager gameManager;
    private void Start() 
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    private void OnCollisionEnter(Collision other)
    {
        Instantiate(airExplosionPrefab, transform.position, Quaternion.identity);
        Instantiate(prefracturedPrefab, transform.position, Quaternion.identity);
        gameManager.RestartLevel(gameObject);
    }

}
