using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy_ByTime : MonoBehaviour
{
	public float lifetime;

	void Start()
	{
		Destroy(gameObject, lifetime);
	}
	

}
