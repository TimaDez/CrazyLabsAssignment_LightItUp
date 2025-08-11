using System.Collections;
using UnityEngine;

public class SqueezeForIphoneX : MonoBehaviour
{

	public Vector2 scaleValue = Vector2.one;

	public void Squeeze()
	{
		Vector3 updatedScale = transform.localScale;
		updatedScale.x *= scaleValue.x;
		updatedScale.y *= scaleValue.y;
		transform.localScale = updatedScale;
	}
	
}
