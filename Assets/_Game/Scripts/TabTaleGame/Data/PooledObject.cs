using UnityEngine;

namespace LightItUp.Data
{
    public abstract class PooledObject : MonoBehaviour {
        public virtual void OnInitPoolObj() {
            gameObject.SetActive(true);
        }
        public virtual void OnReturnedPoolObj()
        {
            gameObject.SetActive(false);
        }
    }
}
