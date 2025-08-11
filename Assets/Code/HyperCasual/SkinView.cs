using System;
using UnityEngine;
using UnityEngine.UI;

namespace HyperCasual.Skins
{
    public class SkinView : MonoBehaviour
    {
        //[HideInInspector]        
        public SkinConfig Data;

        public Image bg;
        public Image icon;
        public GameObject tapToPlay, lockIcon;

        public event Action<SkinView> Click = s => { }; 

        public void SetData(SkinConfig data)
        {
            Data = data;
            Selected = false;
            
            RefreshView();
        }

        void Start()
        {
            if (Data != null)
            {
                RefreshView();
            }            
        }

        public void RefreshView()
        {                        
            SetBGView();
            SetIconView();
            
            tapToPlay.SetActive(Selected);
            lockIcon.SetActive(!Data.Unlocked);            
        }

        private void SetIconView()
        {
            icon.sprite = Selected?Data.iconHighlighted:Data.icon;
            
            var alpha = Data.Unlocked ? 1 : 0.36f;

            var c = icon.color;
            c.a = alpha;
            icon.color = c;
        }
        
        private void SetBGView()
        {
            float alpha;
            
            if (Selected)
            {
                alpha = 0.3f;
            }
            else
            {
               alpha = 0.1f;                        
            }
            
            var c = bg.color;
            c.a = alpha;
            bg.color = c;
        }

        public void ToggleSelected(bool isSelected)
        {
            Selected = isSelected;

            RefreshView();
        }

        public void OnClick()
        {            
            Click(this);
        }
	
        public bool Selected { get; private set; }
    }
}