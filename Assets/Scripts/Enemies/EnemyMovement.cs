using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;

    Rigidbody rb;

    private void Start() 
    {
        rb = GetComponent<Rigidbody>();

        rb.velocity = Vector3.forward * moveSpeed;
    }

    


}
