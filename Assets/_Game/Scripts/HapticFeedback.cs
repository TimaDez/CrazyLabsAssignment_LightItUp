using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using LightItUp.Data;
using TapticPlugin;

namespace LightItUp
{
    public class HapticFeedback
    {
        public static void Generate(UIFeedbackType type)
        {
            if (!GameData.PlayerData.vibrateEnabled || type == UIFeedbackType.None)
                return;

            if (Application.platform == RuntimePlatform.Android)
            {
                Handheld.Vibrate();
                return;
            }
        
            switch (type) {
                case UIFeedbackType.ImpactLight:
                    TapticManager.Impact(ImpactFeedback.Light);
                    break;
                case UIFeedbackType.ImpactMedium:
                    TapticManager.Impact(ImpactFeedback.Medium);
                    break;
                case UIFeedbackType.ImpactHeavy:
                    TapticManager.Impact(ImpactFeedback.Heavy);
                    break;
                

                case UIFeedbackType.Success:
                    TapticManager.Notification(NotificationFeedback.Success);
                    break;

                case UIFeedbackType.Warning:
                    TapticManager.Notification(NotificationFeedback.Warning);
                    break;

                case UIFeedbackType.Error:
                    TapticManager.Notification(NotificationFeedback.Error);
                    break;

                case UIFeedbackType.Selection:
                default:
                    TapticManager.Selection();
                    break;
            }
        }
    }
    public enum UIFeedbackType
    {
        None = -1,
        Selection = 0,
        ImpactLight,
        ImpactMedium,
        ImpactHeavy,
        Success,
        Warning,
        Error
    }
/*
    public enum UIFeedbackType{
    None = -1,
    Selection = 0,
    ImpactLight,
    ImpactMedium,
    ImpactHeavy,
    Success,
    Warning,
    Error
}
public class HapticFeedback 
{
	public static void Generate(UIFeedbackType type)
    {
        if (!GameData.PlayerData.vibrateEnabled ||type == UIFeedbackType.None)
            return;
#if UNITY_IPHONE && !UNITY_EDITOR
		GenerateFeedback((int)type);
#elif UNITY_ANDROID
        switch (type)
        {

            case UIFeedbackType.Selection:
                KyVibrator.Vibrate(150);
                break;
            case UIFeedbackType.ImpactLight:
                KyVibrator.Vibrate(25);
                break;

            case UIFeedbackType.ImpactMedium:
                KyVibrator.Vibrate(50);
                break;
            case UIFeedbackType.ImpactHeavy:
                KyVibrator.Vibrate(100);
                break;

            case UIFeedbackType.Success:

                KyVibrator.Vibrate(300);
                break;

            case UIFeedbackType.Warning:
                KyVibrator.Vibrate(200);
                break;

            case UIFeedbackType.Error:
                KyVibrator.Vibrate(400);
                break;

            default:
                KyVibrator.Vibrate(100);
                break;
        }
#endif
    }
    /*
    static Coroutine vibrationAction;
    static void StartAndroidVibrationAction(List<int> vibrateTimes) {
        if (vibrationAction != null)
        {
            ActionRunner.StopSpecificCoroutine(vibrationAction);
            vibrationAction = null;
        }
        vibrationAction = ActionRunner.Run(AndroidVibrateAction(vibrateTimes));
    }
    static IEnumerator AndroidVibrateAction(List<int> vibrateTimes) {
        while (vibrateTimes.Count > 0)
        {
            KyVibrator.Vibrate(vibrateTimes[0]);
            float f = vibrateTimes[0];
            vibrateTimes.RemoveAt(0);
            if (vibrateTimes.Count > 0)
            {
                f += vibrateTimes[0];
                vibrateTimes.RemoveAt(0);
                yield return new WaitForSeconds(f / 1000);
            }            
        }
        vibrationAction = null;
    }*/
/*
#if UNITY_IPHONE && !UNITY_EDITOR
[DllImport("__Internal")]
private static extern void GenerateFeedback(int type);
#endif
}
*/
}