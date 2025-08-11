using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HyperCasual;
using LightItUp.Currency;
using UnityEngine;
using UnityEngine.EventSystems;
using LightItUp.Data;
using LightItUp.Sound;
using LightItUp.UI;

namespace LightItUp.Game
{
    public static class FailureTypes
    {
        public const string Falling = "falling";
        public const string Spikes = "Spikes";        
    }

    public class PlayerController : MonoBehaviour
    {
        public static event Action<State> StateChangedEvent = (s) => { };
        
        public CameraFocus camFocus;
        public AnimationController animC;
        public Transform dummyPlayer;
        public PlayerLitArea litArea;
		public ParticleSystem jumpFX;
		public ParticleSystem jumpShoesFX;
        public ParticleSystem metalBootsFX;
        public ParticleSystem deathFX;
        public GameObject portal;
        public ParticleSystem portalEffect;
        public ParticleSystem portalEffect2;
        public TMPro.TextMeshPro jumpMaxText;
        public float extraY;
        public LayerMask reviveLayerMask;
        public bool canInteract = false;
        public bool inifinteJumps = false;
        public bool hasUsedRevive;

		public List<GameObject> visualStyleObjects1;
		public List<GameObject> visualStyleObjects2;
		public List<GameObject> visualStyleObjects12;
        public State state;

        public enum State { 
            Normal, Fail, Win
        }

        public bool PlayerDead
        {
            get { return state == State.Fail; }
        }

        public bool PlayerWon
        {
            get { return state == State.Win; }
        }

        bool inputOnRightSide
        {
            get
            {
                if (GameData.PlayerData.usePlayerCenterJumping)
                {
                    var r = Camera.main.ScreenPointToRay(Input.mousePosition);
                    return r.origin.x > transform.position.x;
                }

                return Input.mousePosition.x > Screen.width / 2;
            }
        }

        public Rect worldRect
        {
            get
            {
                return new Rect((Vector2) transform.position - Vector2.one * _collider.radius, Vector2.one * _collider.radius * 2);
            }
        }
        
        Rigidbody2D _rigidBody;
		CircleCollider2D _collider;
		LightUpAura _lightUpAura;
        BlockController _lastTouchedBlock = null;
        bool _jump;
		int _jumpDirection;
		int jumpsAmmount = 2;
		int _jumpsRemaining = 2;
        
        Vector2 _jumpClickPosition;
       
        BlockController _parentBC;
        Wall _lastWallTouched;
        List<Collider2D> _ignoredColliders;
        
        GameLevel _gameLevelRef;
        float _lastJumpFrame = 0;
        BlockController.TutorialEffect _tutorialEffect = BlockController.TutorialEffect.None;
        TutorialText _tutorialText;
        float _colorFader;
        int _blockedJumpDirection = 0;
		bool isInvulnerable = false;
		bool currentBlinkDirection = false;
		Color lastKnownColor = Color.white;
		int currentAlphaFrame = 1;
		int visibleFrameCount = 7;
		int invisibleFrameCount = 4;
		private float currentInvisiblityTime = 0f;
		private bool isSpikeImmune = false;
		private bool magicHatActive = false;
		private List<GameObject> boosterVisualObjects;
		private List<SpriteRenderer> boosterVisualSprites;
		List<BoosterType> currentActiveBoosters;

        void Awake() {
            _ignoredColliders = new List<Collider2D>();
            _collider = GetComponent<CircleCollider2D>();
            _rigidBody = GetComponent<Rigidbody2D>();
            _rigidBody.bodyType = RigidbodyType2D.Kinematic;
			_jumpsRemaining = jumpsAmmount;
			isInvulnerable = false;
            camFocus.transform.SetParent(null);
            animC.gameObject.SetActive(false);
            portal.SetActive(true);      
			boosterVisualObjects = new List<GameObject> ();
			boosterVisualSprites = new List<SpriteRenderer> ();
        }
        
        private void Start()
        {
            _gameLevelRef = GameManager.Instance.currentLevel;
            ChangeState(State.Normal);
            Time.timeScale = 1;
        }

        public void ChangeState(State newState)
        {
            if (state != newState)
            {            
                state = newState;
            }
        
            StateChangedEvent(newState);
        }

        void Update() {
            if (Time.timeSinceLevelLoad < 0.1f) 
                return;
        
            if (PlayerDead)
            {
                return;
            }
        
            if (WinConditionChecker.GameOver)
            {
                return;
            }
        
            if (!canInteract)
            {
                return;
            }
        
            HandleInput();
        }
        
        private void HandleInput()
        {
        
#if UNITY_EDITOR
            bool overUI = EventSystem.current.IsPointerOverGameObject();
#else
        bool overUI = EventSystem.current.IsPointerOverGameObject(0);
#endif        
        
            if (Input.GetMouseButtonDown(0) && !overUI)
            {
                TryToJump(0);
            }else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                TryToJump(1);            
            }else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                TryToJump(-1);
            }
        
            if (Input.GetMouseButton(0) && _rigidBody.velocity.y > 0)
            {
                if (GameSettings.Player.reduceGravityWhenHoldingJump)
                {
                    _rigidBody.gravityScale = GameSettings.Player.gravityScale * GameSettings.Player.reduceGravityScale;
                }
            }
            else
            {
                _rigidBody.gravityScale = GameSettings.Player.gravityScale * 1.5f;
            }
        }

        private void TryToJump(int overrideJumpDirection)
        {
            if (_jumpsRemaining > 0 || inifinteJumps)
            {
                if (overrideJumpDirection == 0)
                    _jumpDirection = inputOnRightSide ? 1 : -1;
                else
                {
                    _jumpDirection = overrideJumpDirection;
                }
            
                if (_jumpDirection != _blockedJumpDirection)
                {
                    TriggerJump();
                }
                else
                {
                    SoundManager.PlaySound("CantJump");
                }
            }
            else
            {
                DisplayMaxJumpReachedMessage();
            }
        }

        private void DisplayMaxJumpReachedMessage()
        {
            jumpMaxText.transform.position = transform.position + new Vector3(0, 2.5f);
            jumpMaxText.transform.rotation = Quaternion.identity;
            jumpMaxText.gameObject.SetActive(true);
        }

        private void TriggerJump()
        {
            _blockedJumpDirection = 0;
            if (_tutorialText != null)
            {
                ObjectPool.ReturnTutorialText(_tutorialText);
                _tutorialText = null;
            }

			if (Time.timeScale == 0) {
				Time.timeScale = 1;		
			}

            _jump = true;
            CanvasController.GetPanel<UI_Game>().DisplayPressed(_jumpDirection);
            var r = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector2 p = r.origin - transform.position;
            _jumpClickPosition = p.normalized;
            _jumpsRemaining--;
            if (jumpFX != null)
                jumpFX.Emit(10);
        }

		void PlayPortalAnimationVisualsOnly()
		{
			portalEffect.Emit(15);
			portalEffect2.Play();
		}

        public void PlayPortalAnimation()
        {
            portalEffect.Emit(15);
            portalEffect2.Play();
            animC.SetColor(Color.white);
            transform.SetParent(null);
            canInteract = true;
            portal.SetActive(false);
            animC.gameObject.SetActive(true);
            ChangeState(State.Normal);
            camFocus.zoomOut = false;
            
            _rigidBody.bodyType = RigidbodyType2D.Dynamic;
            _rigidBody.simulated = true;
            _rigidBody.velocity = Vector2.zero;
            _collider.enabled = true;            
			_jumpsRemaining = jumpsAmmount;
            _rigidBody.WakeUp();
			if (_lightUpAura!=null) {
				_lightUpAura.ActivateAura (_gameLevelRef.GetBlocksColorList());	
			}
        }
        private void LateUpdate()
        {
            camFocus.transform.position = transform.position;
        }

        #region Revive
        
        void RevivePlayerPortalAnim()
        {
            Time.timeScale = 1;  
            PlayPortalAnimation();
            _rigidBody.simulated = true;
            _rigidBody.velocity = Vector2.zero;
            _collider.enabled = true;
            camFocus.zoomOut = false;
			_jumpsRemaining = jumpsAmmount;
            dummyPlayer.gameObject.SetActive(false);
        
            WinConditionChecker.CheckWinCondition(); 


			ActivateReviveFeatures ();
        }
    
    
		void ActivateReviveFeatures ()
		{
			currentInvisiblityTime = 0f;
			ActivateAfterReviveVisuals ();
			ActivateAfterReviveInvulnerability ();
			ActivateAfterReviveCameraFeature ();
			ActivateAfterReviveTimeSlowdown ();
			
            foreach (var reviveZone in GameManager.Instance.currentLevel.reviveZones)
            {
                if (reviveZone.mode == ReviveZone.Mode.SafeZone)
                {
                    if (reviveZone.collider.OverlapPoint(transform.position))
                    {
                        reviveZone.safeBox.gameObject.SetActive(true);
                    }
                }
            }
		}
		void InvulnerabilityFinished()
		{
			Time.timeScale = 1;
			inifinteJumps = false;
			isInvulnerable = false;
			_jumpsRemaining = jumpsAmmount;
			SetPlayerBlinkToDefault();
			SetBoosterVisualsToDefault();
			PlayPortalAnimationVisualsOnly();
		}

		void ActivateAfterReviveInvulnerability ()
		{
			inifinteJumps = true;
			isInvulnerable = true;
//			StartCoroutine(WaitFor(GameSettings.Player.reviveInvulnerabilityDuration, () => {
//				OnActivateAfterReviveInvulnerabilityFinished();
//			}));

		}

		void ActivateAfterReviveCameraFeature ()
		{
			camFocus.IgnoreTargetsByDuration (GameSettings.Player.reviveInvulnerabilityDuration);

		}
		void ActivateAfterReviveVisuals ()
		{
			currentBlinkDirection = false;
			currentAlphaFrame = 1;
		}

		void ActivateAfterReviveTimeSlowdown ()
		{
			Time.timeScale = GameSettings.Player.slowDownTarget;
			//StartCoroutine(WaitFor(GameSettings.Player.reviveInvulnerabilityDuration, () => {
				//Time.timeScale = 1;
			//}));
		}
        public bool CanRevive(out Vector3 pos, ReviveConfig.ReviveType reviveType)
        {
            pos = Vector3.zero;

            bool reviveGemsAvailable = CurrencyService.Instance.GetCurrentAmount(CurrencyType.Gems) >= 40;
            if (hasUsedRevive || !reviveGemsAvailable) return false;
            
            foreach (var reviveZone in GameManager.Instance.currentLevel.reviveZones)
            {
                if (reviveZone.collider.OverlapPoint(transform.position))
                {
                    if (reviveZone.mode == ReviveZone.Mode.NoRevive)
                        return false;
                    else
                    {
                        pos = reviveZone.safeSpot.position;
                        pos.y += 2;                        
                        return true;
                    }
                }
            }
            
            if (_lastTouchedBlock != null)
            {
                if (reviveType == ReviveConfig.ReviveType.LastBlock)
                {
                    if (GetRevivePosForLastTouchedBlock(ref pos))
                    {
                        return true;
                    }                
                }
                else
                {
                    if (GetRevivePosForClosestBlock(ref pos))
                    {
                        return true;
                    }
                    
                    if (GetRevivePosForLastTouchedBlock(ref pos))
                    {
                        return true;
                    }         
                    
                }            
            }
        
            return false;
        }

        private bool GetRevivePosForClosestBlock(ref Vector3 pos)
        {
            var lastTouchedBlockPos = _lastTouchedBlock.transform.position;

            var unlitBlocks = GameManager.Instance.currentLevel.blocks.Where(
                b => !b.IsLit && !b.doNotRevive
                     );

            float minDistance = Mathf.Infinity;

            BlockController reviveBlock = null;
            
            //unlitBlocks.
        
            foreach (var blockController in unlitBlocks)
            {
                var distance = Vector3.Distance(lastTouchedBlockPos, blockController.transform.position);
                
                foreach (var noReviveZone in GameManager.Instance.currentLevel.reviveZones)
                {
                    if (noReviveZone.collider.OverlapPoint(blockController.transform.position))
                    {
                        return false;
                    }
                }

                if (distance < minDistance)
                {
                    var candidatePos = GetRevivePos(blockController.col);
                    var c = Physics2D.OverlapCircle(candidatePos, _collider.radius, reviveLayerMask);
                    
                    if (c == null || c == blockController.col)
                    {
                        if (blockController.HasSpikes)
                        {
                            var hit = Physics2D.Raycast(candidatePos, Vector2.down, _collider.radius * 4,
                                reviveLayerMask);
                            if (hit != null && hit.collider != null)
                            {
                                if (CheckCollisionForSpikes(hit.collider))
                                {
                                    //Debug.LogError(blockController.name + ", Cannot spawn at revive location, will hit spikes after falling");  
                                    continue;
                                }
                                else
                                {
                                    //Debug.LogError(blockController.name + "has spikes but won't touch them :)");  
                                }                         
                            }
                        }
                        
                        reviveBlock = blockController;
                        pos = candidatePos;
                        minDistance = distance;                   
                    }
                    else
                    {
                        Debug.Log("Cannot spawn at revive location, blocked by something!");                    
                    }
                }
            }

            return reviveBlock != null;        
        }

        private bool GetRevivePosForLastTouchedBlock(ref Vector3 p)
        {
            var list = new List<BlockController>();
            list.AddRange(camFocus.LitBlocks);
            list.OrderBy(x => (x.transform.position - _lastTouchedBlock.transform.position).magnitude);
            for (int i = 0; i < list.Count; i++)
            {
                if (CanSpawnOnBlock(list[i]))
                {
                    p = GetRevivePos(list[i].col);
                    var c = Physics2D.OverlapCircle(p, _collider.radius, reviveLayerMask);
                    if (c == null)
                    {
                        Debug.Log("Cannot spawn at revive location, blocked by something!");
                        return true;
                    }
                }
            }

            Debug.Log("Cannot spawn on lastTouchedBlock");
            return false;
        }

        bool CanSpawnOnBlock(BlockController bc)
        {
            foreach (var noReviveZone in GameManager.Instance.currentLevel.reviveZones)
            {
                if (noReviveZone.collider.OverlapPoint(bc.transform.position))
                {
                    return false;
                }
            }
            
            var blocks = new List<BlockController>();
            blocks.AddRange(camFocus.UnlitBlocks);
            blocks.AddRange(camFocus.LitBlocks);
            blocks.Remove(bc);
            blocks = blocks.OrderBy(x => (bc.transform.position - x.transform.position).magnitude).ToList();
            var d = (bc.transform.position - blocks[0].transform.position).magnitude;
            var b = Mathf.Abs(bc.transform.position.x - blocks[0].transform.position.x) < GameSettings.Player.maxReviveDistance && bc.transform.position.y > blocks[0].transform.position.y;
            return d < GameSettings.Player.maxReviveDistance || b;
        }

        Vector3 GetRevivePos(Collider2D bCol)
        {
            return bCol.transform.position + Vector3.up * (bCol.bounds.extents.y + 2);
        }
    
        public void RevivePlayer(bool startAnimation)
        {                   

            hasUsedRevive = true;
            transform.position = GameData.PlayerData.revivePosition; 
            portal.SetActive(true);
            animC.gameObject.SetActive(false);
            camFocus.zoomOut = false;

			CinemachineController.Instance.AnnounceGameState (GameState.FollowingPlayer);
            if (startAnimation)
                DelayPortalAnim(1);            
        }

        public void DelayPortalAnim(float delay)
        {
            LeanTween.delayedCall(delay, RevivePlayerPortalAnim).setIgnoreTimeScale(true);	
            _gameLevelRef.InvokePlaySpawnSound(delay * 0.6f);            
        }
        
        #endregion
        
        public void ReleaseBlock() {
            
            if (_parentBC != null)
            {
                _parentBC.PlayerRelease();
            }

            var wall = GetComponentInParent<Wall>();

            if (wall != null)
            {
                if (wall.disableOnJump)
                {
                    wall.gameObject.SetActive(false);
                }
            }
            
            transform.SetParent(null);
            GameData.PlayerData.jumpCount++;
            _jump = false;
            _rigidBody.simulated = true;
            _rigidBody.bodyType = RigidbodyType2D.Dynamic;
            _collider.enabled = true;                
        }
        
        public void ResetPlayerColor()
        {
            animC.SetColor(Vector4.Lerp(animC.GetColor(), Color.white, _colorFader));
            _colorFader += Time.unscaledDeltaTime;
        }

		void SetPlayerBlinkToDefault()
		{
			Color _targetColor = new Color (lastKnownColor.r, lastKnownColor.g, lastKnownColor.b, 1);;
			animC.SetColor(_targetColor);
		}
		void SetBoosterVisualsToDefault()
		{
			for (int i = 0; i < boosterVisualSprites.Count; i++) {
				boosterVisualSprites [i].color = Color.white;
			}
		}
		void BlinkPlayerColorWithToggleInvisibility()
		{
			Color _currentColor = animC.GetColor ();
			Color _targetColor;
			currentAlphaFrame += 1;
			if (currentAlphaFrame > visibleFrameCount+invisibleFrameCount) {
				currentAlphaFrame = 1;
			}
			if (currentAlphaFrame > 0 && currentAlphaFrame <= visibleFrameCount) {

				_targetColor = new Color (_currentColor.r, _currentColor.g, _currentColor.b, 1);
				animC.SetColor(_targetColor);
				BoostersVisualsApplyColor (_targetColor);
				return;
			}

			if (currentAlphaFrame > visibleFrameCount && currentAlphaFrame <= visibleFrameCount+invisibleFrameCount) {
				_targetColor = new Color (_currentColor.r, _currentColor.g, _currentColor.b, 0);
				animC.SetColor(_targetColor);
				BoostersVisualsApplyColor (_targetColor);
				return;
				
			}

		}
		void BoostersVisualsApplyColor(Color targetColor)
		{
			for (int i = 0; i < boosterVisualSprites.Count; i++) {
				boosterVisualSprites [i].color = targetColor;
			}
		}

		void BlinkPlayerColor()
		{
			Color _currentColor = animC.GetColor ();
			Color _targetColor;
			float _targetAlpha = 0f;
			if (currentBlinkDirection) {
				_targetAlpha= Mathf.Clamp(_currentColor.a + GameSettings.Player.blinkSpeed, 0, 1);
			}else
			{
				_targetAlpha = Mathf.Clamp(_currentColor.a - GameSettings.Player.blinkSpeed, 0, 1);
			}
			_targetColor = new Color (_currentColor.r, _currentColor.g, _currentColor.b, _targetAlpha);

			if (_targetAlpha == 1 || _targetAlpha == 0) {
				currentBlinkDirection = !currentBlinkDirection;	
			}
			animC.SetColor(_targetColor);
		}

        private void FixedUpdate()
        {
			if (currentInvisiblityTime >= 0) {
				currentInvisiblityTime += Time.deltaTime;
				if (currentInvisiblityTime>=GameSettings.Player.reviveInvulnerabilityDuration) {
					currentInvisiblityTime =-1f;
					InvulnerabilityFinished ();
				}
			}

			if(transform.parent == null && _jumpsRemaining <= 0 && !isInvulnerable)
            {
                ResetPlayerColor();
            }

			if(isInvulnerable)
			{
				BlinkPlayerColorWithToggleInvisibility();
			}

            if (_jump)
            {
                HandleJump();
                _jump = false;
            }
            
            if (transform.position.y < _gameLevelRef.lowestY)
            {
                GameOver(FailureTypes.Falling);
                return;
            }
            
            if (_ignoredColliders.Count > 0)
            {
                List<Collider2D> overlapping = new List<Collider2D>();
                overlapping.AddRange(Physics2D.OverlapCircleAll(transform.position, _collider.radius + 0.2f));
                for (int i = _ignoredColliders.Count - 1; i >= 0; i--)
                {
                    var c = _ignoredColliders[i];
                    if (!overlapping.Contains(c))
                    {
                        Physics2D.IgnoreCollision(_collider, c, false);
                        _ignoredColliders.RemoveAt(i);
                    }
                }
            }
            HandleTutorial();
        }

        private void HandleTutorial()
        {
            if (_tutorialEffect == BlockController.TutorialEffect.DoubleJump)
            {
                if (_rigidBody.velocity.y < 0 && transform.parent == null)
                {
                    _tutorialEffect = BlockController.TutorialEffect.None;
                    Time.timeScale = 0;
                    _tutorialText = ObjectPool.GetTutorialText();
                    _tutorialText.tutorialText.text = GameSettings.InGame.tutorialDoubleJumpText;
                    _tutorialText.tutorialText.fontSize = GameSettings.InGame.tutorialDoubleJumpFontSize;
                    Vector3 v = GameSettings.InGame.tutorialDoubleJumpTextOffset;
                    _tutorialText.tutorialText.rectTransform.pivot = GameSettings.InGame.tutorialDoubleJumpTextPivot;
                    _tutorialText.transform.position = transform.position + v;
                }
            }

            if (_jumpsRemaining <= 0)
            {
                _tutorialEffect = BlockController.TutorialEffect.None;
            }
        }

        private void HandleJump()
        {
//SoundManager.PlaySound("Jump"); //Moved to animation controller
            jumpMaxText.gameObject.SetActive(false);
            _lastJumpFrame = Time.fixedTime;
            bool hadParent = transform.parent == null;

            ReleaseBlock();

            Vector3 m = Vector3.zero;

            if (_parentBC != null && _parentBC.useMove)
            {
                m = _parentBC.transform.TransformDirection(_parentBC.direction);
                m.x = 0;
                m.z = 0;
                m.y = Mathf.Max(0, m.y);
            }

            if (GameData.PlayerData.useStraightJumping)
            {
                _jumpClickPosition.Normalize();
                _jumpClickPosition *= GameSettings.Player.jumpPower.magnitude;

                float dot = Vector2.Dot(Vector2.up, _jumpClickPosition.normalized);
                float f = dot / 2 + 0.5f;

                _rigidBody.velocity = _jumpClickPosition * f + (Vector2) m;
                if (_rigidBody.velocity.y < extraY && _rigidBody.velocity.y > 0)
                {
                    _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, extraY);
                }

                if (_rigidBody.velocity.y < 0)
                {
                    _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, -extraY * .3f);
                }
            }
            else
            {
                _rigidBody.velocity = Vector3.up * GameSettings.Player.jumpPower.y +
                                      Vector3.right * GameSettings.Player.jumpPower.x * _jumpDirection + m;
            }

            _collider.enabled = true;
            _parentBC = null;
            dummyPlayer.gameObject.SetActive(false);
            if (hadParent)
            {
                _colorFader = 0;
                animC.DoubleJump();
            }
            else
            {
                animC.Jump(false);
			}
			PlayJumpSound ();

        }


		void PlayJumpSound()
		{
			if (jumpsAmmount == 2) {
				if (_jumpsRemaining == 1) {
					SoundManager.PlaySound ("Jump");
					return;
				}
				if (_jumpsRemaining == 0) {
					SoundManager.PlaySound ("JumpTwice");
					return;
				}
			}
		
			if (jumpsAmmount == 3) {
				if (_jumpsRemaining == 2) {
					SoundManager.PlaySound ("Jump");
					return;
				}
				if (_jumpsRemaining == 1) {
					SoundManager.PlaySound ("JumpTwice");
					return;
				}
				if (_jumpsRemaining == 0) {
					SoundManager.PlaySound ("SpringShoes");
					jumpShoesFX.Play ();
					return;
				}
			}
		}
        public void ActivateCollectible(CollectibleEffect effect, Vector3 pickupPos)
        {
            //Disable all other effects here?
            if (litArea != null)
            {
                litArea.gameObject.SetActive(false);
            }        
            inifinteJumps = false;
//        homingJumps = false;

            switch (effect)
            {
                case CollectibleEffect.InfiniteJumps:
                    inifinteJumps = true;
                    StartCoroutine(WaitFor(3, () => {
                        inifinteJumps = false;
                    }));
                    break;
                case CollectibleEffect.HomingJumps:
                    /*               homingJumps = true;
                StartCoroutine(WaitFor(3, () => {
                    homingJumps = false;
                }));*/
                    break;
                case CollectibleEffect.LightWave:
                    var lw = ObjectPool.GetLitArea();
                    lw.transform.position = transform.position;
                    lw.transform.SetParent(null);
                    lw.Play();
                    // create lit area at player position, disable after wave animation
                    break;
                case CollectibleEffect.Taser:
                    litArea.gameObject.SetActive(true);
                    StartCoroutine(WaitFor(3, () => {
                        litArea.gameObject.SetActive(false);
                    }));
                    break;
                case CollectibleEffect.Star:
                    //Debug.Log("Picked up star!");
                    GameData.PlayerData.starsCollectedInLevel++;
                    LeanTween.delayedCall(0.2f, () =>
                        GameData.PlayerData.ValueWasUpdated());
                    break;
                default:
                    break;
            }
        }

        IEnumerator WaitFor(float secs, System.Action onComplete)
        {
            yield return new WaitForSeconds(secs);
            onComplete();
        }

        public void GameOver(string failType)
        {
            if (state != State.Normal) 
                return;
            
            if (failType == FailureTypes.Spikes)
                StatisticsService.CountStat(GameStats.die_on_spikes, 1);
        
            ChangeState(State.Fail);
            HapticFeedback.Generate(GameSettings.InGame.hapticFeedbackPlayerDead);
			if (_lightUpAura!=null) {
				_lightUpAura.DeactivateAura ();
			}
            jumpMaxText.gameObject.SetActive(false);
            if (deathFX != null) deathFX.Play();
            SoundManager.PlaySound("Death");
            animC.gameObject.SetActive(false);
            camFocus.zoomOut = true;
        
            _rigidBody.simulated = false;
            _collider.enabled = false;
        
            //Dead!
            Debug.Log("Player dead!");
            _gameLevelRef.ZoomOutCamera();

            GameData.PlayerData.wonLastGame = false;

            Time.timeScale = 0;
			GameData.PlayerData.SetCompletionPercentage (Mathf.CeilToInt(((float)GameManager.Instance.currentLevel.LitBlockCount*100)/(float)GameManager.Instance.currentLevel.blocks.Count),GameManager.Instance.currentLevel.levelIdx );
			CinemachineController.Instance.AnnounceGameState (GameState.EndLevel);
            LeanTween.delayedCall(GameSettings.CameraFocus.zoomOutWinDuration, () =>
            {
                GameManager.Instance.OpenRevive();   
            }).setIgnoreTimeScale(true);           
        }
    
        private void OnDestroy()
        {
            if (camFocus != null && camFocus.gameObject != null)
            {
                Destroy(camFocus.gameObject);
            }
        }
    
        public bool CheckCollisionForSpikes(Collision2D collision) {
			if (isSpikeImmune || isInvulnerable) {
				return false;
			}
            var spikes = collision.collider.GetComponent<SpikeController>();
            var spikesRound = collision.collider.GetComponent<SpikeRoundController>();
            if (spikes != null || spikesRound != null)
            {                
                return true;
            }
            return false;
        }

		public bool CheckCollisionForSpikesNoConditions(Collision2D collision) {
			var spikes = collision.collider.GetComponent<SpikeController>();
			var spikesRound = collision.collider.GetComponent<SpikeRoundController>();
			if (spikes != null || spikesRound != null)
			{                
				return true;
			}
			return false;
		}

        public bool CheckCollisionForSpikes(Collider2D collider) {
            var spikes = collider.GetComponent<SpikeController>();
            var spikesRound = collider.GetComponent<SpikeRoundController>();
            if (spikes != null || spikesRound != null)
            {                
                return true;
            }
            return false;
        }

		private void HandlePlayerCollision(Collision2D collision)
		{

			if (collision.collider != null && collision.contacts.Length > 0 && transform.parent == null)
			{
				jumpMaxText.gameObject.SetActive(false);

				if (CheckCollisionForSpikes (collision) && !isInvulnerable)
				{

					GameOver(FailureTypes.Spikes);
					return;
				}



				var bc = collision.collider.GetComponent<BlockController>();
				//
				//				if (bc == null && isSpikeImmune) {
				//					bc = GetPossibleValidParentForSpikes (collision);
				//				}

				if (bc != null)
				{
					LandOnBlock(collision, bc);
				}

				var wall = collision.collider.GetComponent<Wall>();

				if (wall != null && _lastJumpFrame+0.01f < Time.fixedTime)
				{
					LandOnWall(collision, wall);
				}
			}        
		}

		public void OnAuraCollision(LightUpAura aura,Collider2D collision)
		{
			var bc = collision.gameObject.GetComponent<BlockController> ();
			if (bc!=null && !bc.IsBomb && !bc.isKinematic && !bc.useMove && !bc.useEffector && !bc.IsLit) {
				bc.PlayerHit();
				SoundManager.PlaySound ("MagicAura");
			}
		}

//		private void OnTriggerEnter2D(Collider2D collider )
//		{
//			HandleAuraCollision (collider);
//		}
        private void OnCollisionEnter2D(Collision2D collision)
		{
			
			HandlePlayerCollision (collision);

        }

		private BlockController GetPossibleValidParentForSpikes (Collision2D collision)
		{
			BlockController bc = collision.gameObject.GetComponent<BlockController> ();
			if (bc != null) {
				return bc;
			} else {
				if (collision.collider.transform.parent != null) {
					return collision.gameObject.transform.parent.gameObject.GetComponent<BlockController> ();
				}
			}
			return null;
		}
		private void LandOnBlock(Collision2D collision, BlockController bc)
        {
            _lastTouchedBlock = bc;
            if (bc.tutorialEffect != BlockController.TutorialEffect.None)
            {
                _tutorialEffect = bc.tutorialEffect;
            }

            _lastWallTouched = null;
            _parentBC = bc;
            bc.PlayerHit();
            _rigidBody.bodyType = RigidbodyType2D.Kinematic;
            SetParent(collision.gameObject.transform, collision.contacts[0].point + collision.contacts[0].normal * 0.5f);
            bc.rb2d.AddForce(-collision.contacts[0].normal * 100);

            animC.Land(collision.contacts[0].normal);
			PlayLandSound (bc);
			SetPlayerColor (bc.color);
            Physics2D.IgnoreCollision(_collider, collision.collider, true);
            _ignoredColliders.Add(collision.collider);
        }


		void PlayLandSound(BlockController bc)
		{
			if (bc.HasSpikes && isSpikeImmune) {
				SoundManager.PlaySound ("MetalBoots");
				metalBootsFX.Play ();
			} else {
				SoundManager.PlaySound("Land");
			}
		}
		void SetPlayerColor(Color currentColor)
		{
			if (!isInvulnerable) {
				animC.SetColor (currentColor);
			} else {
				lastKnownColor = currentColor;
			}
		}


        private void LandOnWall(Collision2D collision, Wall wall)
        {
            animC.LandWall();

            _lastWallTouched = wall;
            SetParent(collision.gameObject.transform, collision.contacts[0].point + collision.contacts[0].normal * 0.5f);
            animC.Land(collision.contacts[0].normal);
			SoundManager.PlaySound("Land");
            if (GameData.PlayerData.useJumpThroughWalls)
            {
                Physics2D.IgnoreCollision(_collider, collision.collider, true);
                _ignoredColliders.Add(collision.collider);
            }

            var c = Vector2.Dot(collision.contacts[0].normal, Vector3.right);
            if (c > 0.999f)
            {
                _blockedJumpDirection = -1;
            }
            else if (c < -0.999f)
            {
                _blockedJumpDirection = 1;
            }
            else
            {
                _blockedJumpDirection = 0;
            }
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            var collectible = collision.GetComponent<CollectibleController>();
            if (collectible != null)
            {
                ActivateCollectible(collectible.effect, collectible.transform.position);
                collectible.MoveToTransform(_rigidBody);
                // Destroy(collectible.gameObject);
            }


        }
    
        void SetParent(Transform parent, Vector3 pos)
        {
            _rigidBody.bodyType = RigidbodyType2D.Kinematic;
            transform.SetParent(parent);
            transform.position = pos;
            _rigidBody.position = pos;
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.angularVelocity = 0;
			_jumpsRemaining = jumpsAmmount;
            _collider.enabled = false;
            dummyPlayer.SetParent(parent);
            dummyPlayer.position = transform.position;
            dummyPlayer.gameObject.SetActive(true);
            _rigidBody.simulated = false;
        }


		public List<BoosterType> GetCurrentActiveBoosters()
		{
			return currentActiveBoosters;
		}

		public void ActivateBoosters(List<BoosterType> activeBoosters)
		{ 
			//For debug 
			currentActiveBoosters = activeBoosters;
			bool hasMagicAura = currentActiveBoosters.Contains (BoosterType.MagicHat);
			bool hasMetalBoots = currentActiveBoosters.Contains (BoosterType.MetalBoots);
			bool hasSpringShoes = currentActiveBoosters.Contains (BoosterType.SpringShoes);

			if (hasMagicAura) {
				ActivateBoosterMagicHat ();
			} 
			if (hasMetalBoots) {
				ActivateBoosterMetalBoots ();
			} 
			if (hasSpringShoes) {
				ActivateBoosterSpringShoes ();
			} 

			if (hasMetalBoots && hasSpringShoes) {
				ActivateVisualStyle(3);	
			}
			if (hasMetalBoots && !hasSpringShoes) {
				ActivateVisualStyle (1);	
			}
			if (!hasMetalBoots && hasSpringShoes) {
				ActivateVisualStyle (2);	
			}
			//currentActiveBoosters = activeBoosters;


		}

		void ActivateVisualStyle(int visualObjectsStyleIndex)
		{
			switch (visualObjectsStyleIndex) {
			case 1:
				boosterVisualObjects = visualStyleObjects1;
				break;
			case 2:
				boosterVisualObjects = visualStyleObjects2;
				break;
			case 3:
				boosterVisualObjects = visualStyleObjects12;
				break;
			default:
				break;
			}
			boosterVisualSprites = new List<SpriteRenderer> ();
			for (int i = 0; i < boosterVisualObjects.Count; i++) {
				boosterVisualObjects [i].SetActive (true);
				boosterVisualSprites.Add( boosterVisualObjects[i].GetComponent<SpriteRenderer> ());
			}
		}


		void ActivateBoosterMagicHat ()
		{
			magicHatActive = true;
			CreateLightUpAura ();
		}
		void ActivateBoosterMetalBoots ()
		{
			isSpikeImmune = true;
			RemoveAllSpikesInteractions ();
		}
		void ActivateBoosterSpringShoes ()
		{
			jumpsAmmount = 3;
			_jumpsRemaining = jumpsAmmount;
		}

		void RemoveAllSpikesInteractions()
		{
			var spikes = FindObjectsOfType<SpikeController> ();
			foreach (var item in spikes) {
				//item.GetComponent<Collider2D> ().isTrigger = true;
				Physics2D.IgnoreCollision(_collider, item.GetComponent<Collider2D>(), true);
			}
			var roundSpikes = FindObjectsOfType<SpikeRoundController> ();
			foreach (var item in roundSpikes) {
				//item.GetComponent<Collider2D> ().isTrigger = true;
				Physics2D.IgnoreCollision(_collider, item.GetComponent<Collider2D>(), true);
			}
		}

		void CreateLightUpAura()
		{
			GameObject _lightUpAuraObject = new GameObject ();
			_lightUpAura = _lightUpAuraObject.AddComponent <LightUpAura>();
			CircleCollider2D _lightUpAuraCollider = CopyComponent (_collider, _lightUpAuraObject);
			_lightUpAuraObject.transform.localScale = new Vector3 (0.9f, 0.9f, 1);
			_lightUpAuraObject.transform.parent = this.transform.parent;
			_lightUpAuraObject.name = "LightUpAura";
			_lightUpAuraObject.transform.localPosition = transform.localPosition;
			_lightUpAuraObject.AddComponent<SpriteRenderer> ();
			_lightUpAuraCollider.isTrigger = true;
			_lightUpAuraCollider.radius = _lightUpAuraCollider.radius * 8;
			_lightUpAura.InjectController (this);
			GameObject auraParticles = Instantiate (PrefabAssets.Instance.auraParticles);

			auraParticles.transform.parent = _lightUpAura.transform;
			auraParticles.transform.localPosition = Vector3.zero;
			auraParticles.SetActive (false);
			_lightUpAura.CreateAura (SpriteAssets.Instance.auraOrb, SpriteAssets.Instance.aura, auraParticles);
		}


		T CopyComponent<T>(T original, GameObject destination) where T : Component
		{
			System.Type type = original.GetType();
			Component copy = destination.AddComponent(type);
			System.Reflection.FieldInfo[] fields = type.GetFields();
			foreach (System.Reflection.FieldInfo field in fields)
			{
				field.SetValue(copy, field.GetValue(original));
			}
			return copy as T;
		}


    }
}