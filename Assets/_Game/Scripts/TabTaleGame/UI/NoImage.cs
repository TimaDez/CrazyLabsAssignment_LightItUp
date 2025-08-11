using UnityEngine.UI;

namespace LightItUp.UI
{
    public class NoImage : Image {

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            toFill.Clear();
            return;
        }
    }
}
