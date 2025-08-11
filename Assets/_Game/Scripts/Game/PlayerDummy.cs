using UnityEngine;

namespace LightItUp.Game
{
    public class PlayerDummy : MonoBehaviour
    {
        public PlayerController player;
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider != null && collision.contacts.Length > 0 && !WinConditionChecker.GameOver)
            {
                if (player.CheckCollisionForSpikes(collision))
                {
                    player.GameOver(FailureTypes.Spikes);
                }
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var collectible = collision.GetComponent<CollectibleController>();
            if (collectible != null)
            {
                player.ActivateCollectible(collectible.effect, collectible.transform.position);
                collectible.MoveToTransform(player.GetComponent<Rigidbody2D>());
                // Destroy(collectible.gameObject);
            }
        }
    }
}
