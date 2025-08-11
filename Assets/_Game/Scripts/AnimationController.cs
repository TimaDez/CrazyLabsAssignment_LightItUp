using UnityEngine;
using LightItUp.Sound;

namespace LightItUp
{
	public class AnimationController : MonoBehaviour
	{
		public LayerMask comeonJeppe;
		public float radius;
		public float rayDist;
		public Animator anim;
		public Rigidbody2D rb2d;
		public float rotateSpeed = 360;
		public Material stickManMaterial;
		int evenCounter;
		float landForce;
		float landForceSide;
		float landForceUnder;
		float currentRotation = 0;
		float wantedRotation = 0;
		float dist = 0;

		void Start ()
		{
			transform.localScale = new Vector3(1, 1, 1);
		}
		public void SetColor(Color color) {
			stickManMaterial.color = color;
		}
		public Color GetColor()
		{
			return stickManMaterial.color;
		}
		void Update ()
		{
			PlayerDirection(Mathf.Atan2(rb2d.velocity.normalized.x, -rb2d.velocity.normalized.y) * Mathf.Rad2Deg);
			if (Physics2D.Raycast(transform.position, rb2d.velocity.normalized, rb2d.velocity.magnitude*rayDist, comeonJeppe))
			{
				dist = Vector2.Distance(transform.position, Physics2D.Raycast(transform.position, rb2d.velocity.normalized, rb2d.velocity.magnitude * rayDist, comeonJeppe).point);
				PrepareLanding(dist);
			}
			else
			{
				PrepareLanding(5);
			}
			var landed = anim.GetBool("Land");
			if (landed)
			{
				landForce += Time.deltaTime * 70;
				landForceSide -= Time.deltaTime * 30;
			}
			else {
				if (rb2d.velocity.x >= 0)
				{
					transform.localScale = new Vector3(-1, 1, 1);
				}
				else
				{
					transform.localScale = new Vector3(1, 1, 1);
				}

				landForce = rb2d.velocity.y;
				landForceSide = Mathf.Abs(rb2d.velocity.x);
			}

			anim.SetFloat("Blend", rb2d.velocity.y - 2);

			anim.SetFloat("LandForce", landForce);
			anim.SetFloat("LandForceSide", landForceSide);
			float rot = Mathf.Abs((Mathf.Atan2(transform.up.y, transform.up.x) * Mathf.Rad2Deg) - 90);
			anim.SetFloat("Rotation", rot);

			currentRotation = Mathf.MoveTowardsAngle(currentRotation, wantedRotation, Time.deltaTime * rotateSpeed);

			transform.localRotation = Quaternion.Euler(0, 0, currentRotation);
		}
		public void Land(Vector3 normal) {
			anim.SetBool("Land", true);
			//SoundManager.PlaySound("Land");
			var localNorm = rb2d.transform.InverseTransformDirection(normal);
			wantedRotation = Mathf.Atan2(localNorm.y, localNorm.x) * Mathf.Rad2Deg - 90;
			currentRotation = wantedRotation;
		}
		public void Jump(bool doubleJump) {
			anim.SetBool("Land", false);
//			if(!doubleJump)
//				SoundManager.PlaySound("Jump");
			var localNorm = rb2d.transform.InverseTransformDirection(Vector3.up);
			wantedRotation = Mathf.Atan2(localNorm.y, localNorm.x) * Mathf.Rad2Deg - 90;
			anim.SetBool("Standing on wall", false);
		}
		public void DoubleJump() {
			Jump(true);
//			SoundManager.PlaySound("JumpTwice");
			anim.SetTrigger("DoubleJump");

		}

		public void LandWall()
		{
			anim.SetBool("Standing on wall", true);
			SetColor(Color.white);
		}

		void PrepareLanding(float dist)
		{
			anim.SetFloat("Brace", dist);
		}
		void PlayerDirection(float dir)
		{
			anim.SetFloat("PlayerDir", dir);
		}
		/*void OnDrawGizmos()
	{
		if(anim != null)
			Gizmos.DrawSphere(transform.parent.position + (Vector3)rb2d.velocity.normalized * (hit.distance+radius), radius);
	}*/
	}
}
