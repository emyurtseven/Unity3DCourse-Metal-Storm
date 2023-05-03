using UnityEngine;
using System.Collections;

public class ParticleEffectAutoDestroy : MonoBehaviour
{
    [SerializeField] PooledObjectType type;
    public bool OnlyDeactivate;

    public PooledObjectType Type { get => type; set => type = value; }
	
	void OnEnable()
	{
		StartCoroutine(CheckIfAlive());
	}
	
	IEnumerator CheckIfAlive()
	{
		while(true)
		{
			yield return new WaitForSeconds(0.5f);
			if(!GetComponent<ParticleSystem>().IsAlive(true))
			{
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
