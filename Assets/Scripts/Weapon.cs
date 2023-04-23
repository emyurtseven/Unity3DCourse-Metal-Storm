using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] float damagePerShot;

    public float DamagePerShot { get => damagePerShot; }
}
