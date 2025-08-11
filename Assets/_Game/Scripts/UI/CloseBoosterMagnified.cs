using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseBoosterMagnified : MonoBehaviour
{

	public List<GameObject> magnifyBoosters;

	private void Update()
	{
		if (Input.GetKey(KeyCode.Escape))
		{
			DisableMagnifiedPopup();
		}
	}

	private void OnEnable()
	{
		this.GetComponent<Button>().onClick.AddListener(DisableMagnifiedPopup);
	}

	private void OnDisable()
	{
		foreach (GameObject booster in magnifyBoosters)
		{
			booster.SetActive(false);
		}
	}

	private void DisableMagnifiedPopup()
	{
		this.gameObject.SetActive(false);
	}

	public void MagnifyBooster(int boosterID)
	{
		magnifyBoosters[boosterID].SetActive(true);
	}
	
}
