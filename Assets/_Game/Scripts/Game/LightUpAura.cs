using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LightItUp.Game
{
	public class LightUpAura : MonoBehaviour {

		private bool isActivated;
		private PlayerController player;
		private Sprite orbSprite;
		private Sprite auraSprite;
		private List<GameObject> orbs;
		private int numberOfOrbs = 3;
		private float auraSpeed = 6;
		private float orbSpeed = 6;
		private float currentAngle;
		private List<Color> colors;
		private List<Color> currentActiveColors;
		private GameObject auraParticles;

		public void InjectController(PlayerController player)
		{
			this.player = player;
		}

		public void DeactivateAura()
		{
			isActivated = false;
			//HideOrbs ();
			HideAura ();
		}

		void HideAura()
		{
			GetComponent<SpriteRenderer> ().sprite = null;
			auraParticles.SetActive (false);
		}	
		void ShowAura()
		{
			GetComponent<SpriteRenderer> ().sprite = auraSprite;
			auraParticles.SetActive (true);
		}
		void HideOrbs()
		{
			for (int i = 0; i < orbs.Count; i++) {
				orbs [i].transform.localScale = Vector3.zero;
			}
		}
		void ShowOrbs()
		{
			List<Color> currentActiveColors = new List<Color>(colors);

			for (int i = 0; i < orbs.Count; i++) {
				orbs [i].transform.localScale = Vector3.one;
				int randomColorIndex = Random.Range (0, currentActiveColors.Count - 1);
				orbs [i].GetComponent<SpriteRenderer> ().color = currentActiveColors[randomColorIndex];
				currentActiveColors.RemoveAt (randomColorIndex);
			}

		}

		public void CreateAura(Sprite orbSprite, Sprite auraSprite, GameObject auraParticles)
		{
			this.orbSprite = orbSprite;
			this.auraSprite = auraSprite;
			this.auraParticles = auraParticles;
			//ActivateOrbs ();
		}
		public void ActivateAura(List<Color> colors)
		{
			this.colors = colors;
			if (this.colors == null) {
				this.colors = new List<Color> ();
				this.colors.Add (Color.white);
			}

			while (this.colors.Count < numberOfOrbs) {
				this.colors.Add (this.colors [Random.Range (0, this.colors.Count - 1)]);
			}

			//ShowOrbs ();
			isActivated = true;
			CircleCollider2D _lightUpAuraCollider = GetComponent<CircleCollider2D> ();
			_lightUpAuraCollider.enabled = false;
			_lightUpAuraCollider.enabled = true;
			ShowAura ();
		}

		void OnTriggerEnter2D(Collider2D collider)
		{
			if (isActivated) {
				if (player!=null) {
					player.OnAuraCollision (this, collider);
				}
			}
		}

		void Update()
		{
			if (isActivated) {

				transform.position = player.transform.position;
//				currentAngle += auraSpeed+Time.deltaTime;
//				this.transform.rotation = Quaternion.AngleAxis (currentAngle, Vector3.back);
				//this.transform.Rotate = (new Vector3(0,0,currentAngle));
//				for (int i = 0; i < orbs.Count; i++) {
//					orbs[i].transform.rotation = Quaternion.AngleAxis (currentAngle*2, Vector3.back);
//				}
			}	
		}

		void ActivateOrbs ()
		{
			currentAngle = 0f;
			orbs = new List<GameObject> ();
			float orbInterval = 360 / numberOfOrbs;
			float radius = GetComponent<CircleCollider2D> ().radius;
			for (int i = 0; i < numberOfOrbs; i++) {
				
				orbs.Add(new GameObject ());
				orbs [orbs.Count - 1].transform.localScale = Vector3.zero;
				orbs [orbs.Count-1].AddComponent<SpriteRenderer> ().sprite = orbSprite;
				orbs [orbs.Count - 1].transform.parent = this.transform;
				orbs [orbs.Count-1].transform.position = new Vector3 (transform.position.x + radius - orbSprite.bounds.extents.x,  transform.position.y, transform.position.z - 1);
				this.transform.Rotate(new Vector3(0,0,orbInterval));
				orbs [orbs.Count - 1].name = "AuraOrb_" + i.ToString ();
			}
			currentAngle = this.transform.eulerAngles.z;
		}
	}
}