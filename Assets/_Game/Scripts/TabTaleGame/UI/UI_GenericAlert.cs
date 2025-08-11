using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using LightItUp.Data;
using LightItUp.Sound;
using LightItUp.Data;
using LightItUp.Sound;

namespace LightItUp.UI
{
    public class UI_GenericAlert : MonoBehaviour
    {
  
        public TMPro.TextMeshProUGUI title;
        public TMPro.TextMeshProUGUI body;
        public Button btn1;
        public Button btn2;
        public Button btn3;
        public Button closeBtn;
        public Button fadeBtn;
        public TMPro.TextMeshProUGUI btn1Text;
        public TMPro.TextMeshProUGUI btn2Text;
        public TMPro.TextMeshProUGUI btn3Text;


        Animator anim;

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        protected virtual void OnEnable()
        {
            if (anim == null)
                anim = GetComponent<Animator>();
            anim.Play("PopupSlideOpen");
            CanvasController.AnimationCount++;
            SoundManager.PlaySound(SoundNames.PopupOpen);

        }
        public virtual void CloseAlert()
        {
            anim.Play("PopupSlideClosed");
            CanvasController.AnimationCount++;
            SoundManager.PlaySound(SoundNames.PopupClose);
        }
        public virtual void OpenAnimationComplete()
        {
            CanvasController.AnimationCount--;
        }
        public virtual void CloseAnimationComplete()
        {
            CanvasController.AnimationCount--;
            gameObject.SetActive(false);
        }


        public void Open(AlertTypes alertType, UnityAction btn1Action, UnityAction cancelAction, bool btn1AutoClose = true, bool cancelAutoClose = true,
            string[] titleParams = null, string[] bodyParams = null,
            string[] btn1Params = null)
        {
            Open(alertType, btn1Action, null, null, cancelAction, btn1AutoClose, true, true, cancelAutoClose, titleParams, bodyParams, btn1Params, null, null);
        }
        public void Open(AlertTypes alertType, UnityAction btn1Action, UnityAction btn2Action, UnityAction cancelAction, bool btn1AutoClose = true, bool btn2AutoClose = true, bool cancelAutoClose = true,
            string[] titleParams = null, string[] bodyParams = null,
            string[] btn1Params = null, string[] btn2Params = null)
        {
            Open(alertType, btn1Action, btn2Action, null, cancelAction, btn1AutoClose, btn2AutoClose, true, cancelAutoClose, titleParams, bodyParams, btn1Params, btn2Params, null);
        }
        public void Open(AlertTypes alertType, UnityAction btn1Action, UnityAction btn2Action, UnityAction btn3Action, UnityAction cancelAction, bool btn1AutoClose = true, bool btn2AutoClose = true, bool btn3AutoClose = true, bool cancelAutoClose = true,
            string[] titleParams = null, string[] bodyParams = null,
            string[] btn1Params = null, string[] btn2Params = null, string[] btn3Params = null)
        {
        
            var a = Localization.GetAlert(alertType);
            title.text = (titleParams != null) ? string.Format(a.header, titleParams) : a.header;
            body.text = (bodyParams != null) ? string.Format(a.body, bodyParams)  : a.body;
            btn1Text.text = "";
            btn2Text.text = "";
            btn3Text.text = "";
            if (a.buttons.Length > 0)
            {
                btn1Text.text = (btn1Params != null) ? string.Format(a.buttons[0], btn1Params) : a.buttons[0];
            }
            if (a.buttons.Length > 1)
            {
                btn2Text.text = (btn2Params != null) ? string.Format(a.buttons[1], btn1Params) : a.buttons[1];
            }
            if (a.buttons.Length > 2)
            {
                btn3Text.text = (btn3Params != null) ? string.Format(a.buttons[2], btn1Params) : a.buttons[2];
            }

            btn2.gameObject.SetActive(!string.IsNullOrEmpty(btn2Text.text));
            btn3.gameObject.SetActive(!string.IsNullOrEmpty(btn3Text.text));
            closeBtn.gameObject.SetActive(cancelAction != null);

            btn1.onClick = new Button.ButtonClickedEvent();
            btn2.onClick = new Button.ButtonClickedEvent();
            btn3.onClick = new Button.ButtonClickedEvent();
            closeBtn.onClick = new Button.ButtonClickedEvent();
            fadeBtn.onClick = new Button.ButtonClickedEvent();
            btn1.onClick.AddListener(()=> {
                if (btn1AutoClose)
                    CloseAlert();
                if (btn1Action != null)
                    btn1Action();
            });
            btn2.onClick.AddListener(() => {
                if (btn2AutoClose)
                    CloseAlert();
                if (btn2Action != null)
                    btn2Action();
            });
            btn3.onClick.AddListener(() => {
                if (btn3AutoClose)
                    CloseAlert();
                if (btn3Action != null)
                    btn3Action();
            });

            UnityAction ca = () =>
            {
                if (cancelAutoClose)
                    CloseAlert();
                if (cancelAction != null)
                    cancelAction();
            };
            fadeBtn.onClick.AddListener(ca);
            closeBtn.onClick.AddListener(ca);

            if ((btn2.gameObject.activeSelf && btn2Action == null) || (btn3.gameObject.activeSelf && btn3Action == null))
            {
                Debug.LogError("action count does not match btn text count??");
            }
            
            gameObject.SetActive(true);
        }



        [System.Serializable]
        public class AlertData
        {
            public string title;
            public string body;
            public string btn1;
            public string btn2;
            public string btn3;
            public AlertData(string title, string body,
                string btn1, string btn2 = "", string btn3 = "")
            {
                this.title = title;
                this.body = body;
                this.btn1 = btn1;
                this.btn2 = btn2;
                this.btn3 = btn3;
            }
        }
    }
}
