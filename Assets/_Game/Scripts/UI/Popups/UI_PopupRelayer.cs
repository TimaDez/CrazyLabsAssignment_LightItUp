using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightItUp.Singletons;


namespace LightItUp.UI
{     
	public class UI_PopupRelayer : SingletonCreate<UI_PopupRelayer>  {

		public LightItUp.CanvasController.Popups popUpOnHold;
		public Stack<LightItUp.CanvasController.Popups> popUpOnHoldStack;
		
//		private bool IsOnHold = false;

		public int GetPopUpsCount()
		{
			if (popUpOnHoldStack != null)
			{
				return popUpOnHoldStack.Count;
			}
			return 0;
		}
		public void PutPopupOnHold (LightItUp.CanvasController.Popups popUpOnHold)
		{
//			this.popUpOnHold = popUpOnHold;
//			IsOnHold = true;
			if (popUpOnHoldStack != null)
			{
				popUpOnHoldStack.Push(popUpOnHold);
				
			}
			else
			{
				popUpOnHoldStack = new Stack<LightItUp.CanvasController.Popups>();
				popUpOnHoldStack.Push(popUpOnHold);
			}
			
		}

		public LightItUp.CanvasController.Popups PullPopupOnHold ()
		{
//			IsOnHold = false;
			return popUpOnHoldStack.Pop();
		}
		public bool IsPopupOnHold ()
		{
//			return IsOnHold;
			return GetPopUpsCount() > 0;
		}

		public void RemovePreviousHeldPopup()
		{
//			IsOnHold = false;
			if (popUpOnHoldStack != null && popUpOnHoldStack.Count > 0)
			{
				popUpOnHoldStack.Pop();
			}
		}
		public void OpenOnHoldPopup()
		{
			if (IsPopupOnHold()) 
			{
				if (popUpOnHoldStack.Peek() == CanvasController.Popups.GameEnded) 
				{
					CanvasController.GetPopup<UI_GameEnded> ().SetFirstTimeOpen(false);
				}
				CanvasController.Open (popUpOnHoldStack.Pop());
//				IsOnHold = false;
			}
		}
        public string GetPopUpOnHoldName()
        {
	        if (popUpOnHoldStack != null && IsPopupOnHold())
	        {
		        return popUpOnHoldStack.Peek().ToString();
	        }
	        
	        return "";
	        
        }
	}
}
