using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.PowerUp;
using HyperCasual.Skins;
using UnityEngine;
using LightItUp.Data;
using LightItUp.Sound;
using LightItUp.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LightItUp.Game
{
	[ExecuteInEditMode]
	public class GameLevel : MonoBehaviour
	{
		public event Action OnLevelLoaded;
		
		public static string Base64Encode(string plainText) {
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return Convert.ToBase64String(plainTextBytes);
		}
		
		[System.Serializable]
		public class GameLevelData
		{
			public string uid;
			public int version;
			public Vector2 player;
			public int jumps1Star = 5;
			public int jumps2Star = 3;
			public List<BlockController.BlockData> blockData;
			public List<Wall.WallData> wallData;
			public List<Vector2> starData;
			public List<ReviveZone.ReviveZoneData> noReviveZonesData;

			public bool IsEqual(GameLevelData gld)
			{
				var a = JsonUtility.ToJson(this);
				var b = JsonUtility.ToJson(gld);

				return string.Compare(a,b) == 0;
			}
		}

		public int LitBlockCount
		{
			get
			{
				return blocks.FindAll(x => x.IsLit).Count;
			}
		}

		public float CompletionPercentage
		{
			get { return (float)LitBlockCount / blocks.Count; }
		}
		
		public Rect LevelRect
		{
			get
			{
				Rect r = new Rect(blocks[0].transform.position.x, blocks[0].transform.position.y, 0, 0);
				foreach (var b in blocks)
				{
					if (b.IsLit && b.useMove) continue;
					var max = b.col.bounds.max;
					var min = b.col.bounds.min;
					r.xMax = Mathf.Max(r.xMax, max.x);
					r.xMin = Mathf.Min(r.xMin, min.x);
					r.yMax = Mathf.Max(r.yMax, max.y);
					r.yMin = Mathf.Min(r.yMin, min.y);
				}

				Vector2 pMin = (Vector2)playerStart.transform.position - Vector2.one;
				Vector2 pMax = (Vector2)playerStart.transform.position + Vector2.one;
				r.xMax = Mathf.Max(r.xMax, pMax.x);
				r.xMin = Mathf.Min(r.xMin, pMin.x);
				r.yMax = Mathf.Max(r.yMax, pMax.y);
				r.yMin = Mathf.Min(r.yMin, pMin.y);
				return r;
			}
		}

		public string uid;
		public int version;
		public int jumps1Star = 5;
		public int jumps2Star = 3;
		public Transform playerStart;
		[HideInInspector]
		public List<BlockController> blocks;
		[HideInInspector]
		public List<Wall> walls;
		public PlayerController player;
		public int levelIdx;
		[HideInInspector]
		public float lowestY;
		[HideInInspector]		
		//[HideInInspector]
		public List<BlockController> explodeParts;
		[HideInInspector]
		public List<Transform> stars;
		[HideInInspector]
		public List<ReviveZone> reviveZones;
		
		private PlayerDummy playerDummy;
		private bool hasStarted = false;		
		private List<CollectibleController> spawnedStars;
		private bool reviveOnTap = false;
		private bool isLoadFinalized = false;
		private GameLevelData loadedLevelData;
		private SeekingMissilesController _seekingMissilesController;

		#region lifecycle
		
		protected void Awake()
		{
			GameData.PlayerData.starsCollectedInLevel = 0;

			GameData.PlayerData.jumpCount = 0;
			explodeParts = new List<BlockController>();
			foreach (var b in blocks)
			{
				if (b.useExplode)
				{
					AddExplodeParts(b.explodeController.parts);
				}
			}
			WinConditionChecker.OnLitChanged += OnLitChanged;

			ZoomOutCamera();
			var t = transform.Find("CameraSetup");
			if (t != null) EditorActions.DelayedEditorDestroy(t.gameObject);
			tutorialTexts = new Dictionary<BlockController, TutorialTextData>();
			GameData.PlayerData.jumpCount = 0;
		}
		
		private void Start()
		{
			GameData.PlayerData.ingamePoints = 0;
			OnLitChanged();
			SetLowestY();
			CinemachineController.Instance.InitController (this);
			CinemachineController.Instance.AnnounceGameState (GameState.WaitingInput);
		}
		
		private void Update()
		{
			CheckForMissingObjects();
			CheckForMissingBlocks();
			CheckForMissingWalls();
			SetLowestY();
			//Debug.DrawLine();
		}
		
		private void LateUpdate()
		{
			if (!Application.isPlaying) return;
			if (Time.timeSinceLevelLoad < 0.1f) return;

#if UNITY_EDITOR
			isLoadFinalized = true;
			bool overUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
#else
        bool overUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(0);
#endif
			if (!isLoadFinalized) {
				return;
			}
			if (Input.GetMouseButtonDown(0) && !overUI)
			{
				if (!hasStarted)
				{
					CanvasController.GetPanel<UI_Game>().ShowUI();
					CanvasController.GetPanel<UI_Game>().HideHand();
					Invoke("PlayerAnimateIn", .4f);
					InvokePlaySpawnSound(0.0f);
					player.portal.GetComponent<Animator>().SetTrigger("StartPortal");
					player.camFocus.zoomOut = false;
					hasStarted = true;
					CinemachineController.Instance.AnnounceGameState (GameState.FollowingPlayer);
					
				}else if (reviveOnTap)
				{
					CanvasController.GetPanel<UI_Game>().HideHand();
					reviveOnTap = false;
					player.DelayPortalAnim(0.1f);
				}				
			}
			PlaceTutorialText();

		}

		private void OnDestroy()
		{
			if (player != null)
			{
				Destroy(player.gameObject);
			}
			if (playerDummy != null)
			{
				Destroy(playerDummy.gameObject);
			}
		}

		#endregion
		
		public void SetLowestY()
		{
			lowestY = float.MaxValue; // playerStart.transform.localPosition.y - 0.5f;
			blocks.RemoveAll(x => x == null);

			foreach (var b in blocks)
			{
				var y = b.transform.localPosition.y - b.col.bounds.extents.y;
				lowestY = Mathf.Min(y, lowestY);
			}
			foreach (var b in walls)
			{
				var y = b.transform.localPosition.y - b.boxCol.bounds.extents.y;
				lowestY = Mathf.Min(y, lowestY);
			}
			lowestY -= GameSettings.Player.killPlayerY;
		}

		public void RevivePlayer()
		{
			if (ReviveConfig.reviveType == ReviveConfig.ReviveType.ClosestBlock)
			{
				CanvasController.GetPanel<UI_Game>().ShowHand();
				player.RevivePlayer(false);
				reviveOnTap = true;				
			}
			else
			{				
				player.RevivePlayer(true);
			}
		}
		
		public List<CollectibleController> GetSpawnedStars() {
			if (spawnedStars == null)
				spawnedStars = new List<CollectibleController>();
			spawnedStars.RemoveAll(x => x == null);
			return spawnedStars;
		}
		
		public void ZoomOutCamera()
		{
			if (player != null && player.camFocus)
			{
				player.camFocus.zoomOut = true;
			}
		}

		private void OnValidate()
		{
			#if UNITY_EDITOR
			if (!EditorApplication.isPlayingOrWillChangePlaymode)
			{
				OnLitChanged();
				CheckForMissingBlocks();
				CheckForMissingWalls();
				ValidateStars();
				levelIdx = Mathf.Clamp(levelIdx, 1, GameLevelAssets.Instance.allLevels.Count + 1);
			}
			#endif
		}
		
		void ValidateStars() {
			if (stars == null)
				stars = new List<Transform>();
			stars.RemoveAll(x => x == null);
			if (stars.Count < 3)
			{
				Debug.LogWarning("Level does not have enough stars!!");
			}
		}
		
		public void AddStarDummy() {
			if (stars.Count < 3)
			{
				stars.Add(Instantiate(PrefabAssets.Instance.starDummy, transform));
			}
		}
		
		void CheckForMissingBlocks()
		{
			if (!Application.isPlaying) {
				List<BlockController> bcl = new List<BlockController> ();
				bcl.AddRange (GetComponentsInChildren<BlockController> ());
				bcl.RemoveAll (x => blocks.Contains (x));
				if (bcl.Count > 0) {
					blocks.AddRange (bcl);
				}
			}
		}
		
		void CheckForMissingWalls()
		{
			if (!Application.isPlaying) {
				List<Wall> bcl = new List<Wall> ();
				bcl.AddRange (GetComponentsInChildren<Wall> ());
				bcl.RemoveAll (x => walls.Contains (x));
				if (bcl.Count > 0) {
					walls.AddRange (bcl);
				}
			}
		}
		
		void AddExplodeParts(List<BlockController> parts)
		{
			foreach (var p in parts)
			{
				if (!explodeParts.Contains(p))
				{
					WinConditionChecker.Add(p);
					explodeParts.Add(p);
				}
			}
		}
		
		
		void OnLitChanged()
		{
			blocks.RemoveAll(x => x == null);
		}
		
		public void InvokePlaySpawnSound(float delay) {
			LeanTween.delayedCall(delay, PlaySpawnSound).setIgnoreTimeScale(true);	
		}
		
		void PlaySpawnSound() {
			SoundManager.PlaySound("Spawn");
		}
		
		void PlayerAnimateIn()
		{
			player.PlayPortalAnimation();

		}

		public void LoadPlayer()
		{
			var path = SkinsData.Instance.GetCurrentSkinResourcePath();
			
			path = "SkinModels/" + path;

			var prefab = Resources.Load<GameObject>(path);
			
			player = Instantiate(prefab, playerStart.position, Quaternion.identity, transform).GetComponent<PlayerController>();
			player.canInteract = false;
			playerDummy = Instantiate(PrefabAssets.Instance.playerDummy, playerStart.position, Quaternion.identity, transform);
			player.dummyPlayer = playerDummy.transform;
			playerDummy.player = player;
			playerDummy.gameObject.SetActive(false);
		}

		void CheckForMissingObjects()
		{
			if (!Application.isPlaying)
			{
				if (blocks != null && blocks.Contains(null))
				{
					blocks.RemoveAll(x => x == null);
				}
				if (walls != null && walls.Contains(null))
				{
					walls.RemoveAll(x => x == null);
				}
			}
		}
		public void ConfirmGameLoadFinalized()
		{
			isLoadFinalized = true;
			player.ActivateBoosters (BoosterService.Instance.GetCurrentActiveBoosters());
			BoosterService.Instance.ConsumeCurrentActiveBoostres ();
			
			OnLevelLoaded?.Invoke();

		}
		public void Load(int level)
		{
			isLoadFinalized = false;

			stars.RemoveAll(x => x == null);
			explodeParts = new List<BlockController>();
			reviveZones.RemoveAll(x => x == null);
			levelIdx = level;
			foreach (var v in blocks)
			{
				EditorActions.DelayedEditorDestroy(v.gameObject);
			}
			foreach (var v in walls)
			{
				EditorActions.DelayedEditorDestroy(v.gameObject);
			}
			foreach (var v in stars)
			{
				EditorActions.DelayedEditorDestroy(v.gameObject);
			}
			foreach (var v in reviveZones)
			{
				EditorActions.DelayedEditorDestroy(v.gameObject);
			}
			
			blocks.Clear();
			walls.Clear();
			stars.Clear();
			reviveZones.Clear();
			
			var t = GameLevelAssets.Instance.allLevels[levelIdx - 1];
			if (t != null)
			{
			    loadedLevelData = Newtonsoft.Json.JsonConvert.DeserializeObject<GameLevelData>(t.text);
				playerStart.localPosition = loadedLevelData.player;
				jumps1Star = loadedLevelData.jumps1Star;
				jumps2Star = loadedLevelData.jumps2Star;
				uid = loadedLevelData.uid;
				version = loadedLevelData.version;               

				if (Application.isPlaying)
				{
					LoadPlayer();
				}

				Debug.Log($"[GameLevel] Load() blockData.Count: {loadedLevelData.blockData.Count}");
				foreach (var b in loadedLevelData.blockData)
				{
					var block = Instantiate(PrefabAssets.Instance.blocks[0], transform);
					blocks.Add(block);
					block.Load(b);
					if (block.useExplode)
					{
						AddExplodeParts(block.explodeController.parts);
					}
				}
				foreach (var w in loadedLevelData.wallData)
				{
					var wall = Instantiate(PrefabAssets.Instance.wall, transform);
					walls.Add(wall);
					wall.Load(w);
				}
				
				if (loadedLevelData.starData == null)
					loadedLevelData.starData = new List<Vector2>();
				for (int i = 0; i < loadedLevelData.starData.Count; i++)
				{
					AddStarDummy();
					stars[i].localPosition = loadedLevelData.starData[i];
				}

				if (loadedLevelData.noReviveZonesData == null)
				{
					loadedLevelData.noReviveZonesData = new List<ReviveZone.ReviveZoneData>();
				}
				foreach (var zoneData in loadedLevelData.noReviveZonesData)
				{
					var zone = Instantiate(PrefabAssets.Instance.reviveZone, transform);
					reviveZones.Add(zone);
					zone.Load(zoneData);
				}

				name = t.name;
			}
			else
			{
				Debug.LogError("No level with idx: " + levelIdx);
			}
			FinalizeStartLevel();
		}

		public void FinalizeStartLevel() {
			SetLowestY();
			if (Application.isPlaying)
			{
				spawnedStars = new List<CollectibleController>();
				foreach (var s in stars)
				{
					var c = PrefabAssets.GetCollectible(CollectibleEffect.Star);
					spawnedStars.Add(Instantiate(c, s.position, Quaternion.identity, transform));
				}
				player.camFocus.Init();
			}
		}
		
		public void UseSeekingMissiles()
		{
			//TODO: check transaction between class and controller
			// if(!_isPowerUpFeatureActive)
			// 	return;
			//No need if because of null check in SeekingMissilesController
			if (_seekingMissilesController != null)
			{
				_seekingMissilesController.UseSeekingMissiles().Forget();
			}
			else
			{
				Debug.LogWarning("[GameLevel] UseSeekingMissiles() SeekingMissilesController is not initialized.");
			}
		}
		
		public List<Color> GetBlocksColorList()
		{
			List<Color> uniqueColors = new List<Color> ();
			foreach (var bc in blocks) {
				if (!uniqueColors.Exists(x => x == bc.color)) {
					uniqueColors.Add (bc.color);
				}
			}
			return uniqueColors;
		}


		public void OnBoosterPopupClosed()
		{
			player.ActivateBoosters (BoosterService.Instance.GetCurrentActiveBoosters());
			BoosterService.Instance.ConsumeCurrentActiveBoostres ();
		}

		public void PlayCelebration()
		{
			StartCoroutine(PlayBlockCelebration());
		}

		System.Collections.IEnumerator PlayBlockCelebration()
		{
			float t = 0;
			float t2 = 0;

			while (t < 60)
			{
				t2 += Time.unscaledDeltaTime;
				t += Time.unscaledDeltaTime;

				if (t2 > .1f)
				{

					foreach (BlockController bc in blocks)
					{
						if (bc.useExplode)
						{
							foreach (BlockController ebc in bc.explodeController.parts)
							{
								ebc.PlayLitEffect();
							}
							continue;
						}

						bc.PlayLitEffect();

					}

					t2 = 0;
				}
				yield return null;
			}
		}
		
		#region Tutorial

		public class TutorialTextData
		{
			public TutorialText textObj;
			public Vector2 offset;
		}
		
		Dictionary<BlockController, TutorialTextData> tutorialTexts;


		public void ShowTutorial(string tutorialText, float fontSize, Vector2 tutorialTextOffset, Vector2 pivot, BlockController.TutorialImages img, BlockController block)
		{
			var game = CanvasController.GetPanel<UI_Game>();
			if (img == BlockController.TutorialImages.LeftTap)
			{
				game.ShowFinger(0);
			}
			if (img == BlockController.TutorialImages.RightTap)
			{
				game.ShowFinger(1);
			}
			if (tutorialTexts.ContainsKey(block))
			{
				tutorialTexts[block].textObj.tutorialText.rectTransform.pivot = pivot;
				tutorialTexts[block].offset = tutorialTextOffset;
				tutorialTexts[block].textObj.tutorialText.text = tutorialText;
				tutorialTexts[block].textObj.tutorialText.fontSize = fontSize;

			}
			else {
				var tmp = ObjectPool.GetTutorialText();
				//var tmp = Instantiate(PrefabAssets.Instance.tutorialText, transform);
				tmp.tutorialText.text = tutorialText;
				tmp.tutorialText.fontSize = fontSize;
				tmp.tutorialText.rectTransform.pivot = pivot;
				tutorialTexts.Add(block, new TutorialTextData
				{
					textObj = tmp,
					offset = tutorialTextOffset,
				});
			}
			PlaceTutorialText();
		}
		
		void PlaceTutorialText()
		{
			foreach (var kp in tutorialTexts)
			{
				kp.Value.textObj.transform.position = (Vector2)kp.Key.transform.position + kp.Value.offset;
			}
		}

		public void HideTutorial(BlockController bc)
		{
			ObjectPool.ReturnTutorialText(tutorialTexts[bc].textObj);
			//Destroy(tutorialTexts[bc].textObj.gameObject);
			tutorialTexts.Remove(bc);
			var game = CanvasController.GetPanel<UI_Game>();
			game.HideFingers();
		}
		
		public void CleanTutorials() {
			foreach (var kp in tutorialTexts)
			{
				ObjectPool.ReturnTutorialText(kp.Value.textObj);
			}
			tutorialTexts.Clear();
		}
		
		#endregion
	
		public void Save(bool insert = false)
		{
#if UNITY_EDITOR
			levelIdx = Mathf.Clamp(levelIdx, 1, GameLevelAssets.Instance.allLevels.Count + 1);
			if (insert)
			{
				if (GameLevelAssets.Instance.allLevels.Find(x => x.name == gameObject.name) != null)
				{

					UnityEditor.EditorUtility.DisplayDialog("Cannot save!!",
						"Level with name " + gameObject.name + " already exists!\nLevel was not saved",
						"Ok");

					Debug.LogError("Level with name " + gameObject.name + " already exists!");
					return;
				}
			}
			List<BlockController.BlockData> blockData = new List<BlockController.BlockData>();
			List<Wall.WallData> wallData = new List<Wall.WallData>();
			List<Vector2> starData = new List<Vector2>();
			List<ReviveZone.ReviveZoneData> noReviveZonesData = new List<ReviveZone.ReviveZoneData>();
			foreach (var v in blocks)
			{
				blockData.Add(v.SaveBlock());
			}
			foreach (var v in walls)
			{
				wallData.Add(v.GetWallData());
			}
			foreach (var s in stars)
			{
				starData.Add(s.localPosition);
			}
			
			foreach (var z in reviveZones)
			{
				noReviveZonesData.Add(z.GetData());
			}

            bool isNewLevel = false;

			if (insert || loadedLevelData == null || string.IsNullOrEmpty(uid))
            {
                isNewLevel = true;
                GenerateUID();
            }

            var gameLevelData = new GameLevelData
			{
				version = version,
				uid = uid,					
				player = playerStart.localPosition,
				jumps1Star = jumps1Star,
				jumps2Star = jumps2Star,
				blockData = blockData,
				wallData = wallData,
				starData = starData,
				noReviveZonesData = noReviveZonesData
			};

			if (!isNewLevel)
			{			
				version++;
				gameLevelData.version = version;									
			}
			
			loadedLevelData = gameLevelData;
			
			string name = gameObject.name;
			if (!insert)
			{
				string pathDelete = Application.dataPath + "/_Game/Resources/GameLevels/" + GameLevelAssets.Instance.allLevels[levelIdx - 1].name;
				FileManager.DeleteFile(pathDelete + ".txt");
			}

			string path = Application.dataPath + "/_Game/Resources/GameLevels/" + name;
			FileManager.SaveFile(path + ".txt", gameLevelData);

			var txt = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/_Game/Resources/GameLevels/" + name + ".txt");
			if (insert)
			{
				GameLevelAssets.Instance.InsertLevel(levelIdx - 1, txt);
			}
			else
			{
				if (levelIdx - 1 >= GameLevelAssets.Instance.allLevels.Count)
				{
					GameLevelAssets.Instance.allLevels.Add(txt);
				}
				else
				{
					GameLevelAssets.Instance.allLevels[levelIdx - 1] = txt;
				}
			}
			
			GameLevelAssets.Instance.UpdateAllLevels();

			UnityEditor.AssetDatabase.Refresh();
#endif
		}

        private void GenerateUID()
        {
            var hash = DateTime.Now.GetHashCode();
			uid = hash.ToString("x");
	         
            version = 1;
        }

#if UNITY_EDITOR
        public bool saveLevelOnPlay {
			get {
				return UnityEditor.EditorPrefs.GetBool("GameLevel.saveLevelOnPlay", false);
			}
			set {
				UnityEditor.EditorPrefs.SetBool("GameLevel.saveLevelOnPlay", value);
			}
		}
#endif
	}
}