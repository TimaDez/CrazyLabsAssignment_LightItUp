using UnityEngine;
using UnityEngine.UI;

namespace HyperCasual.Components.DebugComponents
{
    public class FPSCounter
        : MonoBehaviour
    {
        public Text Display;
        public float DeltaTime { get; private set; }
        
        public void Update()
        {
            DeltaTime += (Time.unscaledDeltaTime - DeltaTime)*0.1f;
        }

        public void LateUpdate()
        {
            var ms = DeltaTime*1000.0f;
            var fps = 1.0f/DeltaTime;
            Display.text = string.Format("{1:0.}fps\n[{0:0.0}ms]", ms, fps);
        }
    }
}