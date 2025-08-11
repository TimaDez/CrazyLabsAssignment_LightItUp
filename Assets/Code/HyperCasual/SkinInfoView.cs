using LightItUp.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HyperCasual.Skins
{
    public class SkinInfoView : UI_Popup
    {
        public Image icon;
        public TextMeshProUGUI description;
        public TextMeshProUGUI condition;
        public ProgressUIDisplay uiProgress;
        
        public void Show(SkinConfig data)
        {
            gameObject.SetActive(true);
            icon.sprite = data.iconHighlighted;
            description.text = data.description;
            condition.text = data.condition.description;

            var progress = ConditionsManager.Instance.GetConditionProgress(data.condition);

            uiProgress.Set((int)progress.x, (int)progress.y);            
        }                
    }
}