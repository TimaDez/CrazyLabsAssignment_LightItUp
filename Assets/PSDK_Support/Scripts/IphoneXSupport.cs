using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IphoneXSupport : MonoBehaviour
{
	public Camera cam;
	public bool autoRun = true;
	public bool debugForceApply = false;
	public bool debugPreventApply = false;

	public List<RectTransform> hudParents = new List<RectTransform>();
	
	public List<SqueezeForIphoneX> squeezeItems = new List<SqueezeForIphoneX>();
	
	private Vector2 defaultMin = new Vector2(0f, 0f), defaultMax = new Vector2(1f, 1f);

	public static bool IsTablet(Camera cam)
	{
		return cam.aspect >= 3f / 4f;
	}

	public static bool IsPhoneX(Camera cam = null)
	{
		
#if UNITY_EDITOR
		if (cam == null)
		{
			cam = Camera.main;

			if (cam == null)
			{
				cam = FindObjectOfType<Camera>();
			}

			if (cam == null)
			{
				Debug.LogWarning("IsPhoneX: Can't find camera");
				return false;
			}

		}
		return cam.aspect <= 1125f / 2436f;
#elif UNITY_IOS
			return UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneX;
#else 
			return false;
#endif
	}
	
	void OnEnable () 
	{
		#if UNITY_EDITOR

		if (cam == null)
		{
			cam = Camera.main;

			if (cam == null)
			{
				cam = FindObjectOfType<Camera>();
			}
		}

		if (IsPhoneX(cam))
		{
			debugForceApply = true;
		}
		#endif
		
		if (autoRun)
			ApplySafeArea();
	}
	
	public void ApplySafeArea()
	{
		if (debugPreventApply && !debugForceApply)
			return;
		
		#if UNITY_2017
		
		Rect area = Screen.safeArea;
		//Debug.Log("Set safe area anchors before: anchorMin: x="+mainScreen.anchorMin.x.ToString("F2")+" y="+ mainScreen.anchorMin.y.ToString("F2"));
		//Debug.Log("Set safe area anchors before: anchorMin: x="+mainScreen.anchorMax.x.ToString("F2")+" y="+ mainScreen.anchorMax.y.ToString("F2"));
		//Debug.Log("Set safe area screen size : Screen.width="+Screen.width+" Screen.height="+Screen.height);
			
		var anchorMin = area.position;
		var anchorMax = area.position + area.size;
		//Debug.Log("Set safe area size : area.position="+area.position+" area.size="+area.size);
		anchorMin.x /= Screen.width;
		anchorMin.y /= Screen.height;
		anchorMax.x /= Screen.width;
		anchorMax.y /= Screen.height;
		if (anchorMin.x <= 0f)
			anchorMin.x = 0f;
		if (anchorMin.y <= 0f)
			anchorMin.y = 0f;
		if (anchorMax.x >= 1f)
			anchorMax.x = 1f;
		if (anchorMax.y >= 1f)
			anchorMax.y = 1f;

		if (debugForceApply)
		{
			anchorMax.y = 1 - 0.054187f;
			anchorMin.y = 0.04187192f;
		}

		foreach (RectTransform item in hudParents)
		{
			item.anchorMin = anchorMin;
			item.anchorMax = anchorMax;
		}

		if (anchorMax != defaultMax || anchorMin != defaultMin)
		{
			foreach (SqueezeForIphoneX item in squeezeItems)
			{
				item.Squeeze();
			}
		}
			
		Debug.Log("Set safe area anchorMin: x="+anchorMin.x.ToString("F2")+" y="+ anchorMin.y.ToString("F2"));
		Debug.Log("Set safe area anchorMax: x="+anchorMax.x.ToString("F2")+" y="+ anchorMax.y.ToString("F2"));
	
		#endif
	}
}
