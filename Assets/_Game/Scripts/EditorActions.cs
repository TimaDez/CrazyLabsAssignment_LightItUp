using UnityEngine;

namespace LightItUp
{
    public static class EditorActions {
        public static void DelayedEditorDestroy(GameObject go)
        {
#if UNITY_EDITOR
            DelayedEditorAction(() =>
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(go);
                }
                else
                {
                    GameObject.DestroyImmediate(go);
                }
            });
#else
        GameObject.Destroy(go);
#endif
        }
        public static void DelayedEditorAction(System.Action onDelayed)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () =>
            {
                onDelayed();
            };
#else
        onDelayed();
#endif
        }
    }
}
