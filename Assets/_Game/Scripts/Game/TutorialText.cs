using LightItUp.Data;

namespace LightItUp.Game
{
    public class TutorialText : PooledObject {
        public TMPro.TextMeshPro tutorialText;
        public void Return() {
            ObjectPool.ReturnTutorialText(this);
        }
    }
}
