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
			lifetime = gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
		}
		else
		{
			Debug.LogWarning("No Animator Attached! Attach an Animator Component!");
		}
		Destroy(gameObject, lifetime);
	}
}
