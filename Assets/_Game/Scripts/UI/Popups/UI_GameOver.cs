using HyperCasual.PsdkSupport;
using UnityEngine;
using UnityEngine.UI;
using LightItUp.Data;
using System;
namespace LightItUp.UI
{
    public class UI_GameOver : UI_Popup
    {
	    [SerializeField] private bool enableTimer = false;
	    
	    [SerializeField] private Image countDown;
	    [SerializeField] private Button reviveButton;
	    [SerializeField] private Button retryButton;

        private float openedWindowTime;
        private int maxOpenedTime = 5;
		private int maxRevivePerDay = 4;

        private State state;
        private int revivedToday = 0;
        
        public enum State
        {
            Init, Countdown, Revive, Retry, Timeout
        }
        
		private bool IsReviveAvailable()
		{
			string date = DateTime.Now.DayOfYear + "-" + DateTime.Now.Year;
			revivedToday = PlayerPrefs.GetInt ("revivedTodayCount" + date, 0);

			return revivedToday < maxRevivePerDay;
		}

        protected override void OnEnable()
        {
            base.OnEnable();
            state = State.Init;
            Time.timeScale = 0;
            
            countDown.fillAmount = 1;
        }
        
        private void OnDisable()
        {
	        CanvasController.TogglePresenter (false);
        }
        public override void OpenAnimationComplete()
        {
            base.OpenAnimationComplete();
            state = State.Countdown;
            openedWindowTime = 0;
            countDown.fillAmount = 1;
        }

        protected override void Update() 
        {
            base.Update();
			
            if (state == State.Countdown && enableTimer) 
            {
                if (Time.unscaledDeltaTime < 1f)
                {
	                openedWindowTime += Time.unscaledDeltaTime;

	                if (openedWindowTime > maxOpenedTime)
	                {
		                countDown.fillAmount = 0;
		                TimeOut();
		                return;
	                }

	                float t = openedWindowTime;
	                float tNorm = openedWindowTime / maxOpenedTime;

	                for (int i = 0; i < 13; i++)
	                {
		                float fI = (float)i / 13f;
		                if(fI > tNorm)
		                {

			                countDown.fillAmount = 1-(((float)i)/13);
			                break;
		                }
	                }
                }
                
            }
        }

		public void GemsReviveSuccess()
		{
			state = State.Revive;
			ClosePopup(() => GameManager.Instance.RevivePlayer());
		}

        public void Revive()
        {
            if (state != State.Countdown) return;
            
            state = State.Revive;
            
            if (IsReviveAvailable())
            {         
	            PlayerPrefs.SetInt ("revivedTodayCount" + DateTime.Now.DayOfYear.ToString() + "-" + (DateTime.Now.Year).ToString(), revivedToday+1);

	            ClosePopup(() => 
	            {                         
		            GameManager.Instance.RevivePlayer();
	            });
            }
            else
            {
	            TimeOut();
            }         
        }
    
        void TimeOut()
        {
            if (state == State.Timeout)
                return;
            
            state = State.Timeout;
            
            ClosePopup(()=> 
            {
                Time.timeScale = 1;
                var game = CanvasController.GetPanel<UI_Game>();
                game.Hide();
                GameManager.Instance.OpenGameEnded(false);
            });
        
        }

        protected override bool HandleBackPressed()
        {                       
            if (state == State.Countdown)
            {
                TimeOut();
                return true;
            }
            return true;
        }
        
    }
}
