using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision other) 
    {
        // Debug.Log(other.GetContact(0).thisCollider);
        // Debug.Log(other.gameObject.name);

        transform.parent.GetComponent<Animator>().enabled = false;
        Destroy(gameObject);
    }
}
