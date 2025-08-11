using System.Collections.Generic;
using System.Linq;
using HyperCasual.PsdkSupport;
using LightItUp;
using LightItUp.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HyperCasual.Skins
{
    public class SkinsManager : MonoBehaviour
    {                
		public SkinsData skinsData;

        public GameObject skinPrefab;
        public GameObject promotedSkinPrefab;

        public RectTransform viewParent;

        public SkinInfoView skinInfoView;

        public ProgressUIDisplay progressDisplay;

        public int promotedSkinIndex = 10;

        private bool _seenNewSkinsNotification;

        List<SkinView> _skinViews2;


		List<SkinView> GetSkinViews()
		{
			if (_skinViews2 == null) {
				//UnloadSkins ();
				LoadSkinViews (1);
			}
			return  _skinViews2;
		}

        [ContextMenu("LoadSkins")]
        public void LoadToGrid()
        {            
			for (int i = 0; i < skinsData.GetCurrentSkin().Count; i++)
            {
                var prefab = i == promotedSkinIndex ? promotedSkinPrefab : skinPrefab;
                
                var go = Instantiate(prefab, viewParent);

                var skinView = go.GetComponent<SkinView>();

				var data = skinsData.GetCurrentSkin()[i];

                skinView.SetData(data);

                if (_skinViews2 == null)
                {
	                _skinViews2 = new List<SkinView>();
                }
                _skinViews2.Add(skinView);
                
                go.name = data.id;
            }
            
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
            }            
#endif
        }

        void AddListeners()
        {
			var views = GetSkinViews();
            
            foreach (var skinView in views)
            {                
                skinView.Click += SkinView_OnClick;                
            }                                    
		}
		void RemoveListeners()
		{
			var views = GetSkinViews();

			foreach (var skinView in views)
			{                
				skinView.Click -= SkinView_OnClick;                
			}                                    
		}

		void LoadSkinViews(int skinsIndex)
		{
			if (_skinViews2 ==null ||_skinViews2.Count == 0)
			{
				skinsData.SelectSkinList (skinsIndex);
				LoadToGrid();
			}
		}
		void UnloadSkins()
		{
			RemoveListeners ();

			if (_skinViews2 == null) {
				_skinViews2 = new List<SkinView> ();
				return;
			}
			for (int i = _skinViews2.Count-1; i >= 0; i--) 
			{
				if (_skinViews2[i]!=null && _skinViews2[i].gameObject!=null) {

					Destroy(_skinViews2[i].gameObject);
					_skinViews2.RemoveAt(i);
				}
			}
			
			_skinViews2 = new List<SkinView> ();
		}
		public void LoadSkins ()
		{

			//skinsData.SelectSkinList (skinIndex);
			//LoadToGrid ();

			foreach (var skinView in _skinViews2)
			{                
				skinView.RefreshView();               
			}

			SelectIndex(SkinsData.Instance.CurrentSkinIndex);

			progressDisplay.Set(skinsData.UnlockedCount, skinsData.Count);

			_seenNewSkinsNotification = SkinsState.HasNewSkins;

			SkinsState.HasNewSkins = false;                
		}

		private void OnEnable()
		{
			UnloadSkins ();
			LoadSkinViews (1);
			LoadSkins ();
			AddListeners ();
		}
       
        private void SkinView_OnClick(SkinView skinView)
        {
			int currentIndex = _skinViews2.IndexOf(skinView);

            if (skinView.Data.Unlocked)
            {
                if (skinView.Selected)
                {
                    GameManager.Instance.isReplay = false;

					GameManager.Instance.StartGame();
                }
                else
                {
                    SkinsData.CurrentSkinId = skinView.Data.id;
                    SelectIndex(currentIndex);
                }
            }
            else
            {
                ShowConditionView(skinView.Data);
            }
        }

        private void ShowConditionView(SkinConfig skinData)
        {
            skinInfoView.Show(skinData);
        }

        void SelectIndex(int index)
        {                      
			for (var i = 0; i < _skinViews2.Count; i++)
            {
				_skinViews2[i].ToggleSelected(i == index);                
            }
        }

        private void Update()
        {
            HandleBackButton();
        }

        private void HandleBackButton()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (CanvasController.AnimationCount > 0)
                    return;
                
                if (skinInfoView.gameObject.activeInHierarchy)
                {
	                skinInfoView.ClosePopup();
                }
                else
                {
	                CanvasController.Open(CanvasController.Panels.Menu);
                }			    
            }
        }
    }
}