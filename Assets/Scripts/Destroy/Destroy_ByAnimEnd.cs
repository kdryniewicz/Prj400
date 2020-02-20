using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy_ByAnimEnd : MonoBehaviour
{
	private float lifetime;
	
	void Start()
	{
		if(gameObject.GetComponent<Animator>() != null)
		{
			lifetime = gameObject.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length;
		}
		else
		{
			Debug.LogWarning("No Animator Attached! Attach an Animator Component!");
		}
		Debug.Log("Lifetime of Effects object: " + lifetime);
		Destroy(gameObject, lifetime);
	}
}
