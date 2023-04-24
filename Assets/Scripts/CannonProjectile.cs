using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonProjectile : MonoBehaviour
{
    [SerializeField] float damagePerShot = 5f;

    public float DamagePerShot { get => damagePerShot; }
}
