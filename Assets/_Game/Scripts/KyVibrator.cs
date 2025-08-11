using UnityEngine;

namespace LightItUp
{
    public static class KyVibrator
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    private static readonly AndroidJavaObject Vibrator =
        new AndroidJavaClass("com.unity3d.player.UnityPlayer")// Get the Unity Player.
        .GetStatic<AndroidJavaObject>("currentActivity")// Get the Current Activity from the Unity Player.
        .Call<AndroidJavaObject>("getSystemService", "vibrator");// Then get the Vibration Service from the Current Activity.
#endif
        static KyVibrator()
        {
            // Trick Unity into giving the App vibration permission when it builds.
            // This check will always be false, but the compiler doesn't know that.
            if (Application.isEditor)
                KyVibrator.Vibrate(100);
        }

        public static void Vibrate(long milliseconds)
        {
#if UNITY_EDITOR
#elif UNITY_ANDROID
        Vibrator.Call("vibrate", milliseconds);
#elif UNITY_IOS
        Handheld.Vibrate();
#endif
        }

        public static void Vibrate(long[] pattern, int repeat)
        {
#if UNITY_EDITOR
#elif UNITY_ANDROID
        Vibrator.Call("vibrate", pattern, repeat);
#elif UNITY_IOS
        Handheld.Vibrate();
#endif
        }
    }
}
