using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCrashApp : MonoBehaviour {

	public void TryCrash()
	{
		Application.ForceCrash(0);
	}
}
