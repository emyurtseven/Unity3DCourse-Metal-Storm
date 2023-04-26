using UnityEngine;
using System.Collections;

public class ParticleEffectAutoDestroy : MonoBehaviour
{
    PooledObjectType type;
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
                ObjectPool.ReturnPooledObject(this.type, gameObject);
                break;
			}
		}
	}
}
