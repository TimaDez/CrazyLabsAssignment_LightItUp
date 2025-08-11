using System.Collections;
using System.Collections.Generic;
using LightItUp.Data;
using UnityEngine;

public class LevelTrigger : MonoBehaviour
{
	[SerializeField]private GameObject level;
	[SerializeField] private GameObject dots;
	[SerializeField] private GameObject image;
	[SerializeField] private GameObject stars;

	
	private void OnEnable()
	{
		DeactivateLEvel();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log("--------------------------- OnTriggerStay2D");
		ActivateLevel();
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		Debug.Log("--------------------------- OnTriggerExit2D");
		DeactivateLEvel();
	}

	private void ActivateLevel()
	{
		dots.SetActive(true);
		image.SetActive(true);
		stars.SetActive(true);
	}
	private void DeactivateLEvel()
	{
		dots.SetActive(false);
		image.SetActive(false);
		stars.SetActive(false);
	}
}
