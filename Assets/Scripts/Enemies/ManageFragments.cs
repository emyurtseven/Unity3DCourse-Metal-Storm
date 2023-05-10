using UnityEngine;
using System.Collections;
using UnityEngine.Events;

/// <summary>
/// Adds an explosion like scattering to prefractured meshes. 
/// This script is attached to the parent container.
/// </summary>
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

        StartCoroutine(FadeDebris());
    }

    /// <summary>
    /// Gradually decreases the alpha of mesh renderers to gently fade out the pieces, then destroys the game object.
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeDebris()
    {
        yield return new WaitForSeconds(debrisPersistTime);

        float alpha = 1;
        while (alpha > 0)
        {
            alpha -= 0.02f;
            Color color = new Color(1, 1, 1, alpha);

            foreach (Transform fragment in transform)
            {
                fragment.gameObject.GetComponent<MeshRenderer>().materials[0].color = color;
                fragment.gameObject.GetComponent<MeshRenderer>().materials[1].color = color;
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }

        Destroy(gameObject);
    }
}
