using UnityEngine;
using LightItUp.Sound;
using LightItUp.Sound;
using System;
using HyperCasual.PsdkSupport;

namespace LightItUp.UI
{
    public class UI_Popup : MonoBehaviour {
        Animator anim;
        System.Action onAnimationComplete;

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }
        protected virtual void OnEnable()
        {
            if (anim == null) anim = GetComponent<Animator>();
            anim.Play("PopupSlideOpen");
            CanvasController.AnimationCount++;
            SoundManager.PlaySound(SoundNames.PopupOpen);

        }
        public virtual void ClosePopup() 
        {
            ClosePopup(null);
        }
        
        public virtual void ClosePopup(System.Action onComplete = null)
        {
            onAnimationComplete = onComplete;
            SoundManager.PlaySound(SoundNames.PopupClose);
            anim.Play("PopupSlideClosed");
            CanvasController.AnimationCount++;

        }
        
        public virtual void OpenAnimationComplete()
        {
            CanvasController.AnimationCount--;

        }
        public virtual void CloseAnimationComplete()
        {
            CanvasController.AnimationCount--;
            gameObject.SetActive(false);
//            CanvasController.Instance.CloseAllPopups();
            if (onAnimationComplete != null)
            {
                var a = onAnimationComplete;
                onAnimationComplete = null;
                a();
            }
        }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (CanvasController.AnimationCount > 0)
                    return;
            
                if (!HandleBackPressed())
                    ClosePopup();
            }
        }

        protected virtual bool HandleBackPressed()
        {
            return false;
        }
    }
}
