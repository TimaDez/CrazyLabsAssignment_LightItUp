using LightItUp.Game;
using UnityEngine;
using UnityEngine.UI;
using LightItUp;

[RequireComponent(typeof(Button))]
public class DebugAddBoosters : MonoBehaviour
{
    private Text text;

    void Awake () {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
		BoosterService.Instance.AddRegularBoosterCharges (BoosterType.MagicHat, 10);
		BoosterService.Instance.AddRegularBoosterCharges (BoosterType.MetalBoots, 10);
		BoosterService.Instance.AddRegularBoosterCharges (BoosterType.SpringShoes, 10);
      
    }

}