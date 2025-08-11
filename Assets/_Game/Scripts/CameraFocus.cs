using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LightItUp.Data;
using LightItUp.Game;
using System.Collections;

namespace LightItUp
{
    public class CameraFocus : MonoBehaviour
    {
        public class CamTarget
        {
            public BlockController target;
            public float time;
            public Rect rect;
        }
        public class CamTempTarget
        {
            public Collider2D target;
            public float time;
            public Rect rect {
                get
                {
                    return new Rect(target.bounds.center, target.bounds.size);
                }
            }
        }

        [SerializeField]
        PlayerController player;

        bool hasStartPos;
        static Vector3 camStartPosition;
        static float camStartOrthoSize;
        public BlockController GetClosesBlock()
        {
            if (unlitBlocks.Count > 0) {
                return unlitBlocks[0];
            }
            return litBlocks[0];        
        }
        public BlockController[] UnlitBlocks {
            get {
                return unlitBlocks.ToArray();
            }
        }
        public BlockController[] LitBlocks
        {
            get
            {
                return litBlocks.ToArray();
            }
        }

        [HideInInspector]
        public bool zoomOut = true;

        List<BlockController> unlitBlocks;
        List<BlockController> litBlocks;
        List<Wall> walls;
        List<CamTarget> targets;
        List<CamTempTarget> tempTargets;

        Vector3 currentPosition;
        Vector3 wantedPosition;
        float currentOrthoSize;
        float wantedOrthoSize;
    
        float fullZoom = 1;
        float zoomDirection = 1;
		bool isIgnoringTargets = false;

        private void Awake()
        {
        
            fullZoom = 1;
            zoomOut = true;
            targets = new List<CamTarget>();
            tempTargets = new List<CamTempTarget>();
            unlitBlocks = new List<BlockController>();
            litBlocks = new List<BlockController>();
            walls = new List<Wall>();
            if (!hasStartPos)
            {
                if (Camera.main == null)
                    return;
            
                camStartPosition = Camera.main.transform.position;
                camStartOrthoSize = Camera.main.orthographicSize;
            }

        }
        private void OnDestroy()
        {
            SetCamera(camStartPosition, camStartOrthoSize);
        }
		public void IgnoreTargetsByDuration(float ignoreDuration)
		{
			isIgnoringTargets = true;
			StartCoroutine(WaitFor(ignoreDuration, () => {
				isIgnoringTargets = false;
			}));
		}

		IEnumerator WaitFor(float secs, System.Action onComplete)
		{
			yield return new WaitForSeconds(secs);
			onComplete();
		}

        public void AddTempTarget(Collider2D col, float duration) {
            tempTargets.Add(new CamTempTarget {
                target = col,
                time = duration
            });
        }

        public void Init()
        {
            GetSortedBlocks();
            SetWantedValues(GetFullLevelRect());
            ForceSetCamera(wantedPosition, wantedOrthoSize);
        }

        void Update()
        {
            if (GameManager.Instance.currentLevel == null) {
                SetCamera(camStartPosition, camStartOrthoSize);
                return;
            }



            zoomDirection = Mathf.Clamp(zoomDirection + (zoomOut ? GameSettings.CameraFocus.zoomDirectionChangeSpeed : -GameSettings.CameraFocus.zoomDirectionChangeSpeed) * Time.unscaledDeltaTime, -1, 1);
            fullZoom = Mathf.Clamp01(fullZoom + (zoomDirection) * Time.unscaledDeltaTime);

            GetSortedBlocks();
        
            ProgressBlendTime();
            Rect r = fullZoom < 0.0001f ? GetBlendedRect() : LerpRect(GetBlendedRect(), GetFullLevelRect(), GameSettings.CameraFocus.blendCurve.Evaluate(fullZoom));
            SetWantedValues(r);
            MoveTowardsWanted();


			SetCamera (currentPosition, currentOrthoSize);

            DrawDebugRect(r);
        }


        void GetSortedBlocks() {
            var b = GameManager.Instance.currentLevel.blocks;
            unlitBlocks.Clear();
            litBlocks.Clear();
			walls.Clear();
			unlitBlocks.AddRange(b.FindAll(x => !x.IsLit));
			litBlocks.AddRange(b.FindAll(x => x.IsLit));
			unlitBlocks.RemoveAll(x => x == null);
			litBlocks.RemoveAll(x => x == null);

            if (!GameSettings.CameraFocus.sortBlocksByPriority && player != null)
            {
                unlitBlocks = unlitBlocks.OrderBy((x) => {
                    return ((Vector2)(x.transform.position - player.transform.position)).magnitude;
                }).ToList();
                litBlocks = litBlocks.OrderBy((x) => {
                    return ((Vector2)(x.transform.position - player.transform.position)).magnitude;
                }).ToList();
            }

            if (unlitBlocks.Count > 0 && (targets.Count <= 0 || unlitBlocks[0] != targets[targets.Count-1].target))
            {
                //Set new target
                targets.Add(new CamTarget {
                    target = unlitBlocks[0],
                    time = 0
                });
            }
            walls.AddRange(GameManager.Instance.currentLevel.walls);
        }


        float GetOrthoFromRect(Rect r)
        {
            float wO = Screen.height;
            wO /= Screen.width;
            wO *= r.width / 2;
            return Mathf.Max(GameSettings.CameraFocus.minOrthoSize, wO, r.height / 2);
        }
        void SetWantedValues(Rect r)
        {
            wantedPosition = r.center;
            wantedOrthoSize = GetOrthoFromRect(r);
        }

        Rect GetFullLevelRect() {
            Rect r = new Rect();
            bool rSet = false;
            if (!player.PlayerDead)
            {
                rSet = true;
                r = ExpandRect(player.worldRect, GameSettings.CameraFocus.playerZoomBorder);
            }
            foreach (var v in unlitBlocks)
            {
                if (!rSet)
                {
                    rSet = true;
                    r = ExpandRect(v.worldRect, GameSettings.CameraFocus.blockZoomBorder);
                }
                else
                {
                    r = IncludeRect(r, ExpandRect(v.worldRect, GameSettings.CameraFocus.blockZoomBorder));
                }
            }
            foreach (var v in litBlocks)
            {
                if (v.useMove)
                    continue;
                if (!rSet)
                {
                    rSet = true;
                    r = ExpandRect(v.worldRect, GameSettings.CameraFocus.blockZoomBorder);
                }
                else
                {
                    r = IncludeRect(r, ExpandRect(v.worldRect, GameSettings.CameraFocus.blockZoomBorder));
                }
            }
            foreach (Wall w in walls) {
                if (!rSet)
                {
                    rSet = true;
                    r = ExpandRect(w.worldRect, GameSettings.CameraFocus.wallZoomBorder);
                }
                else
                {
                    r = IncludeRect(r, ExpandRect(w.worldRect, GameSettings.CameraFocus.wallZoomBorder));
                }
            }
            var stars = GameManager.Instance.currentLevel.GetSpawnedStars();
            foreach (CollectibleController w in stars)
            {
                if (!rSet)
                {
                    rSet = true;
                    r = ExpandRect(w.worldRect, GameSettings.CameraFocus.wallZoomBorder);
                }
                else
                {
                    r = IncludeRect(r, ExpandRect(w.worldRect, GameSettings.CameraFocus.wallZoomBorder));
                }
            }

            return r;
        }

        #region Target Rect Blending

        void ProgressBlendTime() {
            targets.RemoveAll(x => x.target == null);
            foreach (var t in targets)
            {
                t.rect = IncludeRect(ExpandRect(player.worldRect, GameSettings.CameraFocus.playerBorder), ExpandRect(t.target.worldRect, GameSettings.CameraFocus.blockBorder));
                t.time += Time.unscaledDeltaTime / GameSettings.CameraFocus.blendTime;
            }

            while (targets.Count > 1 && targets[1].time > 1)
            {
                targets.RemoveAt(0);
            }
            for (int i = tempTargets.Count - 1; i >= 0; i--)
            {
                tempTargets[i].time -= Time.unscaledDeltaTime;
                if (tempTargets[i].time <= 0)
                    tempTargets.RemoveAt(i); 
            }
        }

        Rect GetBlendedRect()
        {
            Rect r = new Rect();
            if (targets.Count <= 0)
            {
                r = GetFullLevelRect();
            }
            else {
                r = targets[0].rect;
            }
            for (int i = 1; i < targets.Count; i++)
            {
                r = LerpRect(r, targets[i].rect, GameSettings.CameraFocus.blendCurve.Evaluate(targets[i].time));
            }
            foreach (var v in tempTargets)
            {
                Rect r2 = ExpandRect(v.rect, GameSettings.CameraFocus.blockExplodePartsBorder);
                float fadedDuration = GameSettings.CameraFocus.blockExplodePartsFocusFadeDuration;
                if (v.time < fadedDuration)
                    r2 = LerpRect(r, r2, GameSettings.CameraFocus.blendCurve.Evaluate(v.time / fadedDuration));
                r = IncludeRect(r, r2);
            }
            // Add explosions
            // Add indirect lit blocks
            return r;
        }

        #endregion

        #region Camera Position

        void MoveTowardsWanted() {
            currentOrthoSize = Mathf.Lerp(currentOrthoSize, wantedOrthoSize, Mathf.Clamp01(1- GameSettings.CameraFocus.dampeningOrtho));
            currentPosition = new Vector3(
                Mathf.Lerp(currentPosition.x, wantedPosition.x, Mathf.Clamp01(1 - GameSettings.CameraFocus.dampeningX)),
                Mathf.Lerp(currentPosition.y, wantedPosition.y, Mathf.Clamp01(1 - GameSettings.CameraFocus.dampeningY))
            );
        }

        void ForceSetCamera(Vector3 position, float size)
        {
            wantedOrthoSize = Mathf.Max(GameSettings.CameraFocus.minOrthoSize, size);
            currentOrthoSize = wantedOrthoSize;
            wantedPosition = position;
            currentPosition = position;
            SetCamera(currentPosition, currentOrthoSize);
        }

        void SetCamera(Vector3 position, float orthoSize)
        {
            position.z = GameSettings.CameraFocus.zPosition;
            if (Camera.main != null)
            {
                Camera.main.transform.position = position;
                Camera.main.orthographicSize = orthoSize;
            }
        
        }

        #endregion

        #region Rect Helpers

        void DrawDebugRect(Rect r)
        {
            Debug.DrawLine(r.min, new Vector2(r.xMin, r.yMax));
            Debug.DrawLine(r.min, new Vector2(r.xMax, r.yMin));
            Debug.DrawLine(r.max, new Vector2(r.xMin, r.yMax));
            Debug.DrawLine(r.max, new Vector2(r.xMax, r.yMin));
        }


        Rect LerpRect(Rect a, Rect b, float t)
        {
            Rect rect = new Rect(Vector2.Lerp(a.min, b.min, t), Vector2.zero);
            rect.max = Vector2.Lerp(a.max, b.max, t);
            return rect;
        }

        Rect IncludeRect(Rect r1, Rect r2)
        {
            r1.min = Vector2.Min(r1.min, r2.min);
            r1.max = Vector2.Max(r1.max, r2.max);
            return r1;
        }
        Rect ExpandRect(Rect r, float expand)
        {
            return ExpandRect(r, Vector2.one * expand);
        }
        Rect ExpandRect(Rect r, Vector2 expand)
        {
            r.min -= expand;
            r.max += expand;
            return r;
        }

        #endregion
    }
}

