using UnityEngine;
using System.Collections;

public class ParticleEffectAutoDestroy : MonoBehaviour
{
    [SerializeField] PooledObjectType type;

    AudioSource audioSource;

    [SerializeField] bool OnlyDeactivate;

    public PooledObjectType Type { get => type; set => type = value; }
	
	void OnEnable()
	{
        audioSource = GetComponent<AudioSource>();
		StartCoroutine(CheckIfAlive());
	}
	
	IEnumerator CheckIfAlive()
	{
		while(true)
		{
			yield return new WaitForSeconds(0.5f);
			if(!GetComponent<ParticleSystem>().IsAlive(true))
			{
                if (audioSource != null)
                {
                    audioSource.Stop();
                }
                // Try returning the object to the object pool, if it's not in there destroy it
                if (ObjectPool.ReturnPooledObject(this.type, gameObject))
                {
                    break;
                }
                else
                {
                    Destroy(gameObject);
                    break;
                }
			}
		}
	}
}
