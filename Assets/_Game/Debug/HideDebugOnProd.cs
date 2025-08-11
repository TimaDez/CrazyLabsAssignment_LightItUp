using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideDebugOnProd : MonoBehaviour {

	void Awake()
	{
		if (!Debug.isDebugBuild)
		{
			gameObject.SetActive(false);
		}
	}
	
}
