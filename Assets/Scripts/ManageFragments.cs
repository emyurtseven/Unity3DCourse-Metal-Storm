using UnityEngine;
using UnityEngine.Events;

public class ManageFragments : MonoBehaviour
{
    [Header("Explosion parameters")]
    [Range(0, 100)]
    [SerializeField] float minExplosionForce;
    [Range(0, 100)]
    [SerializeField] float maxExplosionForce;
    [SerializeField] float debrisPersistTime = 2f;

    Vector3 explosionPosition;
    float explosionRadius;

    private void Start() 
    {
        explosionPosition = transform.position;
        explosionRadius = GetComponent<SphereCollider>().radius;

        ScatterFragments();
    }

    /// <summary>
    /// Adds force to every child fragment.
    /// </summary>
    private void ScatterFragments()
    {
        foreach(Transform fragment in transform)
        {
            Rigidbody rb = fragment.gameObject.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.None;
            float force = UnityEngine.Random.Range(minExplosionForce, maxExplosionForce);
            rb.AddExplosionForce(force, explosionPosition, explosionRadius);
        }

        Invoke("DestroyDebris", debrisPersistTime);
    }

    private void DestroyDebris()
    {
        Destroy(gameObject);
    }
}
