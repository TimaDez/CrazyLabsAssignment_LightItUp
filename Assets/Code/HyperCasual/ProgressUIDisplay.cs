using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HyperCasual.Skins
{
    public class ProgressUIDisplay : MonoBehaviour
    {
        public enum FormatType
        {
            AmountFromTotal, Percentage
        }
        
        //public Text uiText;
        public TextMeshProUGUI uiText;
        public Image fillImage;
        public FormatType formatType;

        public string format;
        
        int _total;
        int _current;
        
        public void Set(int current, int total)
        {
            _total = total;
            _current = current;
            
            if (formatType == FormatType.Percentage)
            {
                var percentage = Mathf.CeilToInt((float) current / total * 100);
                if (string.IsNullOrEmpty(format))
                    uiText.text = string.Format("{0}%", percentage);
                else
                {
                    uiText.text = string.Format(format, percentage);
                }
            }
            else
            {
                uiText.text = string.Format("{0}/{1}", current, total);
            }
            

            var ratio = current * 1.0f / total;
            fillImage.fillAmount = ratio;
        }
		public void SetTextOnly(string progressBarText)
		{
			uiText.text = progressBarText;
			fillImage.enabled = false;
		}
    }
}