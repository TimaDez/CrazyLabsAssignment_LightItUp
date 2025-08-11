using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LightItUp;

[RequireComponent(typeof(Button))]
public class ToggleCrashButton : MonoBehaviour
{
	private Button btn;
	private DebugCrashApp _crashButton;

	void Awake()
	{
		#if !UNITY_EDITOR
		gameObject.SetActive(false);
		#endif
	}

	void Start ()
	{
		GetComponent<Button>().onClick.AddListener(OnClick);
		_crashButton = FindObjectOfType<CanvasController>().GetComponentInChildren<DebugCrashApp>(true);
	}

	private void OnClick()
	{
		if (_crashButton == null)
			return;
		
		_crashButton.gameObject.SetActive(!_crashButton.gameObject.activeInHierarchy);		
	}

}
