using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class VersionText : MonoBehaviour
{

	public BuildData buildData;
	
	void Start()
	{
		string versionText = "v " + Application.version;

		if (buildData != null)
			versionText += " b" + buildData.buildNumber;


		GetComponent<Text>().text = versionText;
	}	
}