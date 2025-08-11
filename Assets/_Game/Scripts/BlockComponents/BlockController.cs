using System;
using System.Collections;
using System.Collections.Generic;
using HyperCasual;
using UnityEngine;
using LightItUp.Data;
using LightItUp.UI;

namespace LightItUp.Game
{
	public class BlockController : MonoBehaviorBase
    {
        #region Variables

        public event System.Action OnLitStateChanged;

        [Header("General")]
        public SpriteRenderer sr;
        public int colorIdx;
        public Color color;
        public bool mustBeListToWin = true;
        public bool doNotRevive = false;
        
        [Header("Rigidbody2D")]
        public bool isKinematic;
        [Header("Shape")]
        public ShapeType shape;
        public float circleRadius;
        public Vector2 boxSize;


        [Header("Effector")]
        public bool useEffector;
        public float effectorRadius;
        public float effectorPower;
        public float damper = 5f;

        [Header("Move")]
        public bool useMove;
        public Vector2 direction;
        public bool localDirection;

        [Header("Rotate")]
        public bool useRotation;
        public float rotateSpeed;
        
        [Header("Pivot Rotate")]
        public bool usePivotRotation;
        public float pivotRotationSpeed;

        [Header("Spikes")]
        public SpikeSetup topSpikes;
        public SpikeSetup botSpikes;
        public SpikeSetup leftSpikes;
        public SpikeSetup rightSpikes;

        [Header("Explode")]
        public bool useExplode;

        [Header("Tutorial")]
        public bool showTutorialText;
        [TextArea]
        public string tutorialText;
        public float tutorialFontSize = 5;
        public Vector2 tutorialTextOffset;
        public Vector2 tutorialTextPivot = new Vector2(0, 0);
        public TutorialImages tutorialImage;
        public TutorialEffect tutorialEffect;

        int tutorialIdx;

        [HideInInspector]
        public Transform arrow;

        [HideInInspector] 
        public ArrowType arrowType;

        [HideInInspector]
        public ExplodePartsController explodeController;
        
        [HideInInspector]
        public PivotRotator pivotRotator;

        [HideInInspector]
        public ShapeMesh shapeMesh;

        [HideInInspector]
        public List<SpriteRenderer> polyDots;

        [HideInInspector]
        public EffectorController effector2D;
        protected Rigidbody2D _rb2d;
        [HideInInspector]
        public Collider2D col;
        Joint2D[] joints;
        List<BlockController> connectedBlocks;
        bool isMoving = false;
        bool isExplosionPart = false;

        [HideInInspector]
        [SerializeField]
        LineMesher lineMesher;

        [HideInInspector]
        [SerializeField]
        List<SpikeController> polySpikes;

        [HideInInspector]
        [SerializeField]
        SpikeRoundController spikesRound;

        bool hasGivenPointsToPlayer;

        public enum ShapeType {
            Box,
            Circle,
            Polygon
        }
        public enum TutorialImages {
            None,
            CenterTap,
            LeftTap,
            RightTap
        }
        public enum TutorialEffect
        {
            None,
            DoubleJump
        }
        [Serializable]
        public class BlockData
        {
            public string name;

            public Vector2 position;
            public Quaternion rotation;

            public int colorIdx;
            public bool mustBeListToWin = true;

            public bool isKinematic;

            public ShapeType shape;
            public float circleRadius;
            public Vector2 boxSize;
            public List<Vector2> polygonPoints;

            public bool useEffector;
            public float effectorRadius;
            public float effectorPower;

            public bool useMove;
            public Vector2 direction;
            public bool localDirection;

            public bool useRotation;
            public float rotateSpeed;

            public bool usePivotRotation;
            public float pivotRotateSpeed;
            public Vector3 pivotPos;

            public SpikeSetup topSpikes;
            public SpikeSetup botSpikes;
            public SpikeSetup leftSpikes;
            public SpikeSetup rightSpikes;

            public bool useExplode;
            public List<Vector2> explodePoints;

            public bool showTutorialtext;
            public string tutorialText;
            public float tutorialFontSize;
            public Vector2 tutorialTextOffset;
            public Vector2 tutorialTextPivot = new Vector2(0, 0);
            public TutorialImages tutorialImage;
            public TutorialEffect tutorialEffect;

            public bool doNotRevive;
        }

        [System.Serializable]
        public class SpikeSetup
        {
            public bool useSpikes;
            [Range(0f, 1f)]
            public float maxRange;
            [Range(0f, 1f)]
            public float minRange;
            public SpikeSetup()
            {
                minRange = 0;
                maxRange = 1;
            }
            [Newtonsoft.Json.JsonIgnore]
            [HideInInspector]
            public SpikeController spikes;
            public SpikeSetup GetCopy()
            {
                return new SpikeSetup
                {
                    useSpikes = useSpikes,
                    minRange = minRange,
                    maxRange = maxRange
                };
            }
        }

        public Rigidbody2D rb2d
        {
            get
            {
                if (_rb2d == null)
                    _rb2d = GetComponent<Rigidbody2D>();
                return _rb2d;
            }
        }
        bool _isLit = false;
        protected bool isLit
        {
            get
            {
                return _isLit;
            }
            set
            {
                var prevLit = _isLit;
                _isLit = value;
                if (prevLit != _isLit)
                {
                    if (OnLitStateChanged != null)
                    {
                        OnLitStateChanged();
                    }
                }
            }

        }

        public bool IsLit
        {
            get
            {
                return isLit;
            }
        }

        public Rect worldRect {
            get {
                return new Rect(transform.position-sr.bounds.extents, sr.bounds.size);
            }
        }
    
        #endregion

        #region Validation
        public virtual void OnValidate()
        {
            if (!Application.isPlaying && !string.IsNullOrEmpty(gameObject.scene.name))
            {
                ValidateEntireBlock();
            }
        }
        
        void ValidateEntireBlock()
        {
            ValidateColor();
            ValidateShapeMesh();
            ValidateShape();
            ValidateEffector();
            ValidateOverlayArrows();
            ValidateSpikes();
            ValidateExplode();
            ValidatePivotRotate();
            LightToggle(false);
            ToggleEffector(false);

        }
        void ValidateColor() {
            colorIdx = Mathf.Clamp(colorIdx, 0, SpriteAssets.Instance.colorSchemes[0].colors.Count-1);
            color = SpriteAssets.Instance.GetColorFromScheme(0, colorIdx);
            if (shape != ShapeType.Box) useExplode = false;
            if (useExplode) {
                topSpikes.useSpikes = false;
                botSpikes.useSpikes = false;
                leftSpikes.useSpikes = false;
                rightSpikes.useSpikes = false;
                useMove = false;
                useRotation = false;
                useEffector = false;
            }
            rb2d.bodyType = isKinematic ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
            rb2d.useFullKinematicContacts = true;
            rb2d.interpolation = RigidbodyInterpolation2D.Interpolate;
    
#if UNITY_EDITOR
            var b = UnityEditorInternal.ComponentUtility.MoveComponentUp(this);
            int i = 0;
            while (b && i < 10)
            {
                i++;
                UnityEditorInternal.ComponentUtility.MoveComponentUp(this);
            }
#endif
    
        }
        void ValidateShapeMesh() {
            var sm = GetComponentsInChildren<ShapeMesh>();
            if (sm.Length > 1) {
                shapeMesh = null;
                foreach (var v in sm) {
                    EditorActions.DelayedEditorDestroy(v.gameObject);
                }
            }
            switch (shape)
            {
                case ShapeType.Circle:
                case ShapeType.Polygon:
                    sr.enabled = false;
                    if (shapeMesh == null)
                    {
                        shapeMesh = Instantiate(PrefabAssets.Instance.shapeMesh, transform);
                        shapeMesh.transform.localPosition = Vector3.zero;
                        shapeMesh.transform.localRotation = Quaternion.identity;
                        shapeMesh.name = PrefabAssets.Instance.shapeMesh.name;
                    }
                    shapeMesh.gameObject.SetActive(true);
                    if (shape == ShapeType.Circle)
                    {
                        shapeMesh.SetupCircle(circleRadius);
                    }
                    else
                    {
                        shapeMesh.SetupPolygon();
                    }
                    break;
                default:
                    sr.enabled = true;
                    if (shapeMesh != null)
                    {
                        EditorActions.DelayedEditorDestroy(shapeMesh.gameObject);
                        shapeMesh = null;
                    }

                    break;
            }
            if (shapeMesh != null)
            {
                shapeMesh.SetColor(colorIdx);
            }
            ValidatePolyDots();        
        }
        
        public void SetPolyPoints(List<Vector3> p)
        {
            ValidateEntireBlock();
            EditorActions.DelayedEditorAction(() => {
                if (shapeMesh != null)
                {
                    shapeMesh.SetPoints(p);
                }
            
            });        
        }
        
        public void ValidatePolyDots() {
            if (polyDots == null)
                polyDots = new List<SpriteRenderer>();
            polyDots.RemoveAll(x => x == null);

            var srs = new List<SpriteRenderer>();            
            srs.AddRange(GetComponentsInChildren<SpriteRenderer>());
            srs.RemoveAll(x => polyDots.Contains(x) || !x.CompareTag("PolyDot"));

            polyDots.AddRange(srs);
            List<Vector3> ps = new List<Vector3>();
            if (shape == ShapeType.Polygon)
            {
                foreach (var v in shapeMesh.points)
                {
                    ps.Add(v.point.localPosition);
                }
            }
            else if (shape == ShapeType.Box)
            {
                var b = boxSize / 2;
                ps.Add(new Vector2(b.x, b.y));
                ps.Add(new Vector2(-b.x, b.y));
                ps.Add(new Vector2(-b.x, -b.y));
                ps.Add(new Vector2(b.x, -b.y));
                if (useExplode && explodeController != null) {
                    var edges = explodeController.GetEdges();
                    foreach (var v in edges) {
                        ps.Add(v.start);
                        ps.Add(v.end);
                    }
                }
            }

            if (ps.Count > 0) {
                while (polyDots.Count < ps.Count)
                {
                    var g = new GameObject("PolyDot");
                    g.transform.SetParent(transform);
                    g.transform.localPosition = Vector3.zero;
                    g.transform.localRotation = Quaternion.identity;
                    var dotSr = g.AddComponent<SpriteRenderer>();
                    dotSr.sprite = SpriteAssets.Instance.polyDot;
                    polyDots.Add(dotSr);
                }
                while (polyDots.Count > ps.Count)
                {
                    EditorActions.DelayedEditorDestroy(polyDots[polyDots.Count - 1].gameObject);
                    polyDots.RemoveAt(polyDots.Count - 1);
                }
                for (int i = 0; i < ps.Count; i++)
                {
                    polyDots[i].tag = "PolyDot";
                    polyDots[i].transform.localPosition = ps[i];
                    polyDots[i].transform.SetAsLastSibling();
                    polyDots[i].color = color;
                    polyDots[i].gameObject.hideFlags = HideFlags.HideAndDontSave;
                }
            }
            else
            {
                if (polyDots.Count > 0)
                {
                    for (int i = 0; i < polyDots.Count; i++)
                    {
                        EditorActions.DelayedEditorDestroy(polyDots[i].gameObject);
                    }
                    polyDots.Clear();
                }
            }
        }
        
        public void ValidateShape() {
            if (lineMesher == null) {
                lineMesher = GetComponentInChildren<LineMesher>();
                if (lineMesher == null) {
                    lineMesher = Instantiate(PrefabAssets.Instance.lineMesher, transform);
                    lineMesher.name = "LineMesher";
                }
            }

            lineMesher.transform.localPosition = Vector3.zero;
            lineMesher.transform.localRotation = Quaternion.identity;

            switch (shape)
            {
                case ShapeType.Circle:
                    var c = EnsureColliderType<CircleCollider2D>();
                    if (c == null)
                        return;
                    if (circleRadius < 0.1f)
                    {
                        circleRadius = c.radius;
                    }
                    else
                    {
                        c.radius = circleRadius;
                    }
                    float min = topSpikes.minRange;
                    float max = topSpikes.maxRange;
                    if (!topSpikes.useSpikes) {
                        min = 1;
                        max = 1;
                    }
                    lineMesher.MakeCircle(circleRadius, min, max, colorIdx);
                    if (sr != null)
                    {
                        sr.size = Vector2.one * 2 * circleRadius;
                    }
                    break;

                case ShapeType.Polygon:
                    var h = EnsureColliderType<PolygonCollider2D>();
                    var v = shapeMesh.GetPolygonPoints();
                    Vector2[] l = new Vector2[v.Length];
                    for (int i = 0; i < v.Length; i++)
                    {
                        l[i] = v[i];
                    }
                    h.points = l;
                    var s = shapeMesh.GetSize();
                    boxSize = Vector2.one * s *2;
                    lineMesher.MakePolygon(shapeMesh.points, colorIdx);

                    sr.size = boxSize;
                    break;
                case ShapeType.Box:
                default:
                    var b = EnsureColliderType<BoxCollider2D>();
                    if (b == null)
                        return;
                    if (boxSize.sqrMagnitude < 0.1f)
                    {
                        boxSize = b.size;
                    }
                    else
                    {
                        b.size = boxSize;
                    }

                    if (!useExplode)
                    {
                        lineMesher.MakeQuad(boxSize, new SpikeSetup[] {
                            topSpikes,
                            botSpikes,
                            leftSpikes,
                            rightSpikes
                        }, colorIdx);
                    }
                    else if (explodeController != null) {
                        lineMesher.MakeExploding(boxSize, explodeController.GetEdges(), colorIdx);
                    }
                    if (sr != null)
                    {
                        sr.size = boxSize ;
                    }

                    break;
            }
        }
        
        void ValidateEffector() {
            if (effector2D == null) {
                effector2D = GetComponentInChildren<EffectorController>();
            }
            var prevEffector = effector2D;
            if (useEffector && effector2D == null)
            {    
                effector2D = Instantiate(PrefabAssets.Instance.effector, transform);
                effector2D.transform.localPosition = Vector3.zero;
            }
            else {
                if (prevEffector != null && (prevEffector != effector2D || !useEffector))
                {
                    EditorActions.DelayedEditorDestroy(prevEffector.gameObject);
                }
            }
            if (useEffector) {
                effector2D.Setup(effectorRadius, effectorPower, color, damper);
                /*
            var col = effector2D.GetComponent<CircleCollider2D>();
            col.radius = effectorRadius;
            effector2D.forceMagnitude = effectorPower;
            effector2D.name = effectorPower < 0 ? "Attractor" : "Deflector";*/
            }
        }

        void ValidateOverlayArrows()
        {
            if (!useMove && !useRotation && !useEffector)
            {
                if (arrow != null)
                {
                    EditorActions.DelayedEditorDestroy(arrow.gameObject);
                }

                return;
            }

            var newType = GetArrowType();

            bool shouldCreatePrefab = false;
        
            if (arrowType != newType || arrow == null)
            {
                shouldCreatePrefab = true;
                if (arrow != null)
                {
                    EditorActions.DelayedEditorDestroy(arrow.gameObject);
                }
            }
        
            arrowType = newType;

            if (shouldCreatePrefab)
            {
                var prefab = PrefabAssets.Instance.ArrowToPrefab(arrowType);

                if (prefab == null)
                {
                    Debug.LogError("can't find arrow for type: " + arrowType);
                    return;
                }
        
                arrow = Instantiate(prefab, transform.position, Quaternion.identity, transform).transform;
            }
        
            if (useMove)
            {            
                Vector2 d = direction.normalized;
                float angle = Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg + 90;
                if (localDirection)
                {
                    arrow.localRotation = Quaternion.Euler(0, 0, angle);
                }
                else
                {
                    arrow.rotation = Quaternion.Euler(0, 0, angle);
                }            
            }
            else {
                if (arrow != null) {
                    //      EditorActions.DelayedEditorDestroy(arrow.gameObject);
                }
            }
        
            if (useRotation)
            {            
                arrow.localScale = new Vector3(rotateSpeed < 0 ? 1 : -1, 1, 1);
            }
            else if (arrow != null)
            {
                //EditorActions.DelayedEditorDestroy(rotateArrow.gameObject);
            }
            if (useEffector) {            
                arrow.localPosition = Vector3.zero;
            }
        }

        private ArrowType GetArrowType()
        {
            ArrowType arrowType;

            if (useMove)
            {
                if (useRotation)
                {
                    if (useEffector)
                    {
                        arrowType = effectorPower > 0 ? ArrowType.MoveRotateRepel : ArrowType.MoveRotateAttract;
                    }
                    else
                    {
                        arrowType = ArrowType.MoveRotate;
                    }
                }
                else
                {
                    if (useEffector)
                    {
                        arrowType = effectorPower > 0 ? ArrowType.MoveRepel : ArrowType.MoveAttract;
                    }
                    else
                    {
                        arrowType = ArrowType.Move;
                    }                                
                }
            }
            else
            {
                if (useRotation)
                {
                    if (useEffector)
                    {
                        arrowType = effectorPower > 0 ? ArrowType.RotateRepel : ArrowType.RotateAttract;
                    }
                    else
                    {
                        arrowType = ArrowType.Rotate;
                    }
                }
                else
                {
                    arrowType = effectorPower > 0 ? ArrowType.Repel : ArrowType.Attract;
                }
            }

            return arrowType;
        }

        void ValidateExplode()
        {
            if (useExplode)
            {
                if (explodeController == null)
                {
                    explodeController = Instantiate(PrefabAssets.Instance.explodeParts, transform);
                    explodeController.name = "ExplodeParts";
                }
                explodeController.SetSize(boxSize, colorIdx);
                explodeController.SetExplosionParts();
            }
            else
            {
                if (explodeController != null)
                {
                    EditorActions.DelayedEditorDestroy(explodeController.gameObject);
                    explodeController = null;

                }
            }
        }
        
        void ValidatePivotRotate()
        {
            if (usePivotRotation)
            {
                isKinematic = true;
                if (pivotRotator == null)
                {
                    pivotRotator = Instantiate(PrefabAssets.Instance.pivotRotator, transform);
                    pivotRotator.name = "Pivot Rotator";
                }
                              
                
            }
            else
            {
                if (pivotRotator != null)
                {
                    isKinematic = false;
                    EditorActions.DelayedEditorDestroy(pivotRotator.gameObject);
                    pivotRotator = null;
                }
            }
        }
        
        List<Vector3> AddSpikePoses(Vector2 a, Vector2 b)
        {
            List<Vector3> spikePoses = new List<Vector3>();
            var mag = GameSettings.Block.polySpikesGraceArea / (b - a).magnitude;
            spikePoses.Add(Vector2.Lerp(a, b, mag));
            spikePoses.Add(Vector2.Lerp(a, b, 1f - mag));
            return spikePoses;
        }

        void ValidateSpikes()
        {
            if (shape == ShapeType.Box)
            {
                var thickness = GameSettings.Block.spikeThickness / 2 - GameSettings.Block.spikeColliderThickness / 2;
                ValidateSpikes(topSpikes, false, Vector2.up * (boxSize.y / 2 + thickness), "SpikesTop");
                ValidateSpikes(botSpikes, false, -Vector2.up * (boxSize.y / 2 + thickness), "SpikesBot");
                ValidateSpikes(leftSpikes, true, -Vector2.right * (boxSize.x / 2 + thickness), "SpikesLeft");
                ValidateSpikes(rightSpikes, true, Vector2.right * (boxSize.x / 2 + thickness), "SpikesRight");
            }
            else
            {
                ClearSpikes(topSpikes);
                ClearSpikes(botSpikes);
                ClearSpikes(leftSpikes);
                ClearSpikes(rightSpikes);
            }
            if (polySpikes == null) polySpikes = new List<SpikeController>();

            if (shape == ShapeType.Polygon && shapeMesh.points.Count > 0)
            {
                polySpikes.Clear();
                polySpikes.AddRange(GetComponentsInChildren<SpikeController>());
                polySpikes.RemoveAll(x => x == null);
                List<Vector3> spikePoses = new List<Vector3>();
                for (int i = 0; i < shapeMesh.points.Count - 1; i++)
                {
                    if (shapeMesh.points[i].addSpikes && shapeMesh.points[i + 1].addSpikes)
                    {
                        spikePoses.AddRange(AddSpikePoses(shapeMesh.points[i].point.localPosition, shapeMesh.points[i + 1].point.localPosition));

                    }
                }
                if (shapeMesh.points[shapeMesh.points.Count - 1].addSpikes && shapeMesh.points[0].addSpikes)
                {
                    spikePoses.AddRange(AddSpikePoses(shapeMesh.points[shapeMesh.points.Count - 1].point.localPosition, shapeMesh.points[0].point.localPosition));
                }

                int c = spikePoses.Count / 2;
                while (polySpikes.Count < c)
                {
                    var sp = Instantiate(PrefabAssets.Instance.spikes, transform);
                    polySpikes.Add(sp);
                }
                while (polySpikes.Count > c)
                {
                    EditorActions.DelayedEditorDestroy(polySpikes[polySpikes.Count - 1].gameObject);
                    polySpikes.RemoveAt(polySpikes.Count - 1);
                }
                for (int i = 0; i < polySpikes.Count; i++)
                {
                    int u = i * 2;
                    SetupPolySpikes(i, spikePoses[u], spikePoses[u + 1], polySpikes[i]);
                }
            }
            else {
                foreach (var s in polySpikes) {
                    EditorActions.DelayedEditorDestroy(s.gameObject);
                }
                polySpikes.Clear();
            }

            if (shape == ShapeType.Circle && topSpikes.useSpikes)
            {
                if (spikesRound == null) {
                    spikesRound = GetComponent<SpikeRoundController>();
                    if (spikesRound == null)
                    {
                        spikesRound = Instantiate(PrefabAssets.Instance.spikesRound, transform);
                    }                
                }            
                spikesRound.transform.localPosition = Vector3.zero;
                spikesRound.transform.localRotation = Quaternion.identity;
                spikesRound.SetMesh(circleRadius, topSpikes);
            }
            else {
                if (spikesRound != null) {
                    EditorActions.DelayedEditorDestroy(spikesRound.gameObject);
                    spikesRound = null;
                }
            }
        }
        
        void SetupPolySpikes(int i, Vector3 a, Vector3 b, SpikeController spike) {
            Vector3 o = a - b;
            var m = o.magnitude;
            o.Normalize();
            var od = new Vector2(-o.y, o.x);
            float angle = Mathf.Atan2(o.y, o.x);
            polySpikes[i].Setup(new Vector2(m, GameSettings.Block.spikeColliderThickness));
            polySpikes[i].transform.localPosition = Vector2.Lerp(a, b, 0.5f) - od * (GameSettings.Block.spikeThickness/2 - GameSettings.Block.spikeColliderThickness/2);
            polySpikes[i].transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        }
        
        void ValidateSpikes(SpikeSetup spikeSetup, bool vertical, Vector2 pos, string goName)
        {
            if (spikeSetup.useSpikes)
            {
                Vector2 s = boxSize;
                s.x -= GameSettings.Block.polySpikesGraceArea * 2;
                s.y -= GameSettings.Block.polySpikesGraceArea * 2;
                if (spikeSetup.spikes == null)
                {
                    spikeSetup.spikes = Instantiate(PrefabAssets.Instance.spikes, transform);
                }
                ValidateSpikeRanges(spikeSetup);
                Vector2 v = Vector2.zero;
                if (!vertical)
                {
                    var min = s.x * spikeSetup.minRange;
                    var max = s.x * (1 - spikeSetup.maxRange);
                    v = new Vector2(GameSettings.Block.spikeColliderThickness, s.x - min - max);
                    pos.x += min / 2 - max / 2;
                }
                else
                {
                    var min = s.y * spikeSetup.minRange;
                    var max = s.y * (1 - spikeSetup.maxRange);
                    v = new Vector2(s.y - min - max, GameSettings.Block.spikeColliderThickness);
                    pos.y += min / 2 - max / 2;
                }
                spikeSetup.spikes.Setup(v);
                spikeSetup.spikes.transform.localPosition = pos;
                spikeSetup.spikes.name = goName;
            }
            else
            {
                ClearSpikes(spikeSetup);
            }
        }
        
        void ValidateSpikeRanges(SpikeSetup spikeSetup) {
            spikeSetup.maxRange = Mathf.Clamp01(spikeSetup.maxRange);
            spikeSetup.minRange = Mathf.Clamp01(spikeSetup.minRange);
            if (spikeSetup.minRange > spikeSetup.maxRange)
            {
                spikeSetup.maxRange = spikeSetup.minRange;
            }
        }
        
        void ClearSpikes(SpikeSetup spikeSetup) {
            if (spikeSetup.spikes != null)
            {
                EditorActions.DelayedEditorDestroy(spikeSetup.spikes.gameObject);
                spikeSetup.spikes = null;
            }
            ValidateSpikeRanges(spikeSetup);
        }

        T EnsureColliderType<T>() where T : Collider2D
        {
            if (col == null)
            {
                col = GetComponent<Collider2D>();
            }
            var b = col as T;
            if (b == null)
            {
                List<Collider2D> allColliders = new List<Collider2D>();
                allColliders.AddRange(GetComponents<Collider2D>());
                if (allColliders.Count > 0)
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.delayCall += () =>
                    {
                        foreach (var v in allColliders)
                        {
                            DestroyImmediate(v);
                        }                    
                    };
#else
                foreach (var v in allColliders)
                {
                    Destroy(v);
                }
#endif
                }
                b = gameObject.AddComponent<T>();
                col = b;
            }
            return b;
        }
        #endregion

        #region Awake & Update
        protected virtual void Awake()
        {
            col = GetComponent<Collider2D>();
            joints = GetComponents<Joint2D>();
            connectedBlocks = new List<BlockController>();
            if (joints == null) joints = new Joint2D[0];
        
            isLit = false;
            ValidateEntireBlock();
            ToggleEffector(false);
            rb2d.interpolation = RigidbodyInterpolation2D.Interpolate;


        }
        private void Start()
        {
            foreach (var j in joints)
            {
                var bl = j.connectedBody.GetComponent<BlockController>();
                if (bl != null)
                {
                    connectedBlocks.Add(bl);
                    bl.OnLitStateChanged += ConnectedBlockLitStateChanged;
                }
            }
            WinConditionChecker.Add(this);
            ToggleEffector(false);
        }
        void LateUpdate() {
            if (effector2D != null)
            {
                effector2D.transform.position = transform.position;
            }
        }
        void FixedUpdate()
        {
            if (isMoving)
            {
                if (useMove)
                {
                    Vector2 move = direction;
                    if (localDirection)
                    {
                        move = transform.up * move.y + transform.right * move.x;
                    }
                    rb2d.MovePosition(rb2d.position + move * Time.fixedDeltaTime);
                }
                if (useRotation)
                {
                    rb2d.MoveRotation(rb2d.rotation + rotateSpeed * Time.fixedDeltaTime);
                }

               
            }
        }

        void Update()
        {
            if (isMoving)
            {
                if (usePivotRotation)
                {
                    transform.RotateAround(pivotRotator.pivot.transform.position, Vector3.forward,
                        pivotRotationSpeed * Time.deltaTime);
                }
            }
        }

        public void DebugSetLit()
        {
            isLit = true;
        }
    
        #endregion

        #region State Changes

        public void SetIsExplosionPart() {
            isExplosionPart = true;
        }
        protected void ToggleEffector(bool isActive)
        {
            if (effector2D != null)
            {
                if (!Application.isPlaying)
                {
                    effector2D.gameObject.SetActive(true);
                }
                else
                {
                    Physics2D.IgnoreCollision(effector2D.GetComponent<Collider2D>(), col, true);
                    if (isActive && Application.isPlaying)
                        effector2D.transform.SetParent(GameManager.Instance.currentLevel.transform);
                    effector2D.gameObject.SetActive(isActive);
                }
            }
        }

        public virtual void PlayerHit()
        {
            Debug.Log("TT_DEBUG:: Player HIT");
            if (showTutorialText) {
                //Debug.Log("Show tutorial text");
                //Debug.Log(tutorialText);
                GameManager.Instance.currentLevel.ShowTutorial(tutorialText, tutorialFontSize, tutorialTextOffset, tutorialTextPivot, tutorialImage, this);
            }
            GivePointsToPlayer(GameSettings.Block.pointsPlayerLightsUp);
            //isLit = true;
            bool litStateChanged = Collide(false);

            if (litStateChanged)
            {
                StatisticsService.CountStat(GameStats.light_blocks, 1);
                if (IsBomb)
                {
                    StatisticsService.CountStat(GameStats.light_exploding_blocks, 1);
                }
                else
                {
                    switch (colorIdx)
                    {
                        case GameStatsParams.yellowColorIndex:
                            StatisticsService.CountStat(GameStats.light_yellow_blocks, 1);
                            break;
                        case GameStatsParams.redColorIndex:
                            StatisticsService.CountStat(GameStats.light_red_blocks, 1);
                            break;
                    }          
                }
            }           
        }
        
        void GivePointsToPlayer(int points)
        {
            if (isExplosionPart) return;
            if (hasGivenPointsToPlayer || points == 0) return;
            hasGivenPointsToPlayer = true;
            GameData.PlayerData.ingamePoints += points;
            CanvasController.GetPanel<UI_Game>().UpdateFeedbackPoints(points);

        }
        IEnumerator DelayedExplosion() {
            Debug.Log("Play delayed explosion animation");
            yield return new WaitForSeconds(GameSettings.Block.blockExplodeDelay);
            var player = GetComponentInChildren<PlayerController>();
            if (player != null) {
                player.ReleaseBlock();
            }

            gameObject.SetActive(false);
            foreach (var exp in explodeController.parts)
            {
                // GameManager.Instance.currentLevel.player.camFocus.AddTempTarget(exp.col, GameSettings.CameraFocus.blockExplodePartsFocusDuration);
                var dir = exp.transform.localPosition;
                exp.transform.SetParent(transform.parent);
                exp.Collide();
                exp.rb2d.velocity = dir * 10;
            }
        }

        public bool IsBomb
        {
            get { return explodeController != null && explodeController.parts.Count > 0; }
        }

        public bool HasSpikes
        {
            get
            {
                var spikes = GetComponentInChildren<SpikeController>();
                var roundSpikes = GetComponentInChildren<SpikeRoundController>();

                return spikes != null || roundSpikes != null;            
            }
        }
    
    
        public virtual bool Collide(bool focusLitBlock = true)
        {
            bool litStateChanged = false;
            
            if (explodeController != null && explodeController.parts.Count > 0 && !IsLit)
            {
                StartCoroutine(DelayedExplosion());
            }
            if (!isLit)
            {
                litStateChanged = true;
                isLit = true;
                foreach (var c in collisionStayBlocks)
                {
                    if (c == null || !c.gameObject.activeSelf || c.IsLit) continue;
                    c.Collide();
                }
                HapticFeedback.Generate(GameSettings.InGame.hapticFeedbackLightUpBlock);
                bool lastBlock = GameManager.Instance.IsLastBlock();
                //Debug.Log("Last block "+lastBlock, gameObject);
                GivePointsToPlayer(GameSettings.Block.pointsBlockLightsUp);
                var lfx = ObjectPool.GetLightUpFX();
                //SoundManager.PlaySound("LightUpBlock_"+ UnityEngine.Random.Range(1,4).ToString());
                ArpeggioPlayer.Instance.PlayKey();

				CinemachineController.Instance.AnnounceBlockLit (this);

                if (focusLitBlock) {
                    GameManager.Instance.currentLevel.player.camFocus.AddTempTarget(col, GameSettings.CameraFocus.blockExplodePartsFocusDuration);
                }
                switch (shape)
                {

                    case ShapeType.Box:
                        lfx.PlayBox(transform, boxSize);
                        if(lastBlock)
                        {
                            var p = ObjectPool.GetCelebrationFX();
                            p.PlayBox(transform, boxSize);
                            p.transform.SetParent(null);
                        }
                        break;

                    case ShapeType.Circle:
                        lfx.PlayCircle(transform, circleRadius);
                        if (lastBlock)
                        {
                            var p = ObjectPool.GetCelebrationFX();
                            p.PlayCircle(transform, circleRadius);
                            p.transform.SetParent(null);
                        }
                        break;

                    case ShapeType.Polygon:
                        lfx.PlayPolygon(transform, shapeMesh.GetMesh());
                        if (lastBlock)
                        {
                            var p = ObjectPool.GetCelebrationFX();
                            p.PlayPolygon(transform, shapeMesh.GetMesh());
                            p.transform.SetParent(null);
                        }
                        break;

                }
                gameObject.layer = LayerMask.NameToLayer("SolidLit");
                LightToggle();
            }
            isMoving = true;
            ToggleEffector(true);

            return litStateChanged;
        }
        public void PlayLitEffect()
        {
            if (colorIdx < 4)
                colorIdx++;
            else
            {
                colorIdx = 0;
            }
            if(sr != null)
            {
                sr.color = SpriteAssets.Instance.GetColorFromScheme(0, colorIdx);
                if (lineMesher != null) lineMesher.SetColor(sr.color);
                if (shapeMesh != null) shapeMesh.SetColor(sr.color);
                polyDots.RemoveAll(x => x == null);

                foreach (var p in polyDots)
                {
                    p.color = sr.color;
                }
			
            }
            //lineMesher.GetComponent<Renderer>().color = SpriteAssets.Instance.GetColorFromScheme(0, colorIdx);
        }
        protected virtual void ConnectedBlockLitStateChanged() {
            bool lit = false;
            foreach (var bl in connectedBlocks)
            {
                if (bl.IsLit)
                {
                    lit = true;
                    break;
                }
            }
            if (lit)
            {
                foreach (var bl in connectedBlocks)
                {
                    if (!bl.IsLit)
                    {
                        bl.Collide();
                    }
                }
                Collide();
            }
        }
        protected virtual void LightToggle(bool triggerOnLitChanged = true)
        {
            if (triggerOnLitChanged && OnLitStateChanged != null)
            {
                OnLitStateChanged();
            }
            lineMesher.mat.mainTexture = isLit ? SpriteAssets.Instance.outlineLit : SpriteAssets.Instance.outlineUnlit;
            sr.color = color;
            var b = SpriteAssets.Instance.GetBlockSprite(shape);
            if (isLit)
            {
                sr.sprite = b.lit;
                if (shapeMesh != null)
                {
                    shapeMesh.SetSprite(b.lit);
                }
            }
            else
            {
                sr.sprite = b.unlit;
                if (shapeMesh != null)
                {
                    shapeMesh.SetSprite(b.unlit);
                }
            }
        }
        public virtual void PlayerRelease()
        {
            //Debug.Log("Player release");
            if (showTutorialText) {
                GameManager.Instance.currentLevel.HideTutorial(this);
            }
        

        }
        protected virtual void SetSolid()
        {

        }
        #endregion

        #region Physics Events
        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject != null && collision.contacts.Length > 0)
            {
                CheckCollision(collision);
            }
        }
        protected virtual void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject != null)
            {
                CheckCollisionRelease(collision);
            }
        }
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            CheckTrigger(collision);
        }
        protected virtual void OnTriggerStay2D(Collider2D collision)
        {
            CheckTrigger(collision);
        }
        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            CheckTrigger(collision);
        }

        protected void CheckTrigger(Collider2D col)
        {
            CheckForBlackHole(col.gameObject);
            CheckForPlayerLitArea(col.gameObject);
        }

        protected void CheckCollision(Collision2D collision)
        {
            CheckForBlocks(collision.gameObject);
            CheckForPlayerDummy(collision.gameObject);
        }
        protected void CheckCollisionRelease(Collision2D collision)
        {
            CheckForBlocksRelease(collision.gameObject);
        }
        protected bool CheckForBlackHole(GameObject go)
        {
            var blackHole = go.GetComponent<BlackHoleController>();
            if (blackHole != null)
            {
                isLit = false;
                LightToggle();
                return true;
            }
            return false;
        }

        protected bool CheckForPlayerLitArea(GameObject go)
        {
            var bl = go.GetComponent<PlayerLitArea>();
            if (bl != null)
            {
                Collide();
                return true;
            }
            return false;
        }

        protected bool CheckForBlocks(GameObject go)
        {
            var bl = go.GetComponent<BlockController>();
            if (bl != null)
            {
                if (!collisionStayBlocks.Contains(bl)) {
                    Debug.Log("Add");
                    collisionStayBlocks.Add(bl);
                    collisionStayBlocks.RemoveAll(x => x == null || !x.gameObject.activeSelf);
                }
                if (IsLit || bl.IsLit)
                {
                    Collide();
                    return true;
                }
            }
            return false;
        }
        List<BlockController> collisionStayBlocks = new List<BlockController>();
        protected bool CheckForBlocksRelease(GameObject go)
        {
            var bl = go.GetComponent<BlockController>();
            if (bl != null)
            {
                if (collisionStayBlocks.Contains(bl)) {
                    Debug.Log("Remove");
                    collisionStayBlocks.Remove(bl);
                    collisionStayBlocks.RemoveAll(x => x == null || !x.gameObject.activeSelf);
                }
                return true;
            }
            return false;
        }
        protected bool CheckForPlayerDummy(GameObject go)
        {
            var bl = go.GetComponent<PlayerDummy>();
            if (bl != null)
            {
                PlayerHit();
                return true;
            }
            return false;
        }

        #endregion

        #region Save/Load

        public BlockData SaveBlock()
        {
            List<Vector2> polyPoints = new List<Vector2>();
            List<Vector2> explodePoints = new List<Vector2>();

            if (shape == ShapeType.Polygon)
            {
                var l = shapeMesh.GetPolygonPoints();
                foreach (var a in l)
                {
                    polyPoints.Add(a);
                }
            }
            if (useExplode)
            {
                foreach (var v in explodeController.points)
                {
                    explodePoints.Add(v.localPosition);
                }

            }

            BlockData d = new BlockData
            {
                name = name,
                position = transform.localPosition,
                rotation = transform.rotation,
                colorIdx = colorIdx,
                mustBeListToWin = mustBeListToWin,
                isKinematic = isKinematic,
                shape = shape,
                circleRadius = circleRadius,
                boxSize = boxSize,
                polygonPoints = polyPoints,
                useEffector = useEffector,
                effectorRadius = effectorRadius,
                effectorPower = effectorPower,
                useMove = useMove,
                direction = direction,
                localDirection = localDirection,
                useRotation = useRotation,
                rotateSpeed = rotateSpeed,
                usePivotRotation = usePivotRotation,
                pivotRotateSpeed = pivotRotationSpeed,                
                topSpikes = topSpikes.GetCopy(),
                botSpikes = botSpikes.GetCopy(),
                leftSpikes = leftSpikes.GetCopy(),
                rightSpikes = rightSpikes.GetCopy(),
                useExplode = useExplode,
                explodePoints = explodePoints,
                showTutorialtext = showTutorialText,
                tutorialText = tutorialText,
                tutorialFontSize = tutorialFontSize,
                tutorialTextOffset = tutorialTextOffset,
                tutorialTextPivot = tutorialTextPivot,
                tutorialImage = tutorialImage,
                tutorialEffect = tutorialEffect,
                doNotRevive = doNotRevive
            };

            if (usePivotRotation)
            {
                d.pivotPos = pivotRotator.pivotPosition;
            }
            return d;
        }

        public void Load(BlockData b)
        {
            if (string.IsNullOrEmpty(b.name)) name = "Block";
            else name = b.name;
            transform.localPosition = b.position;
            transform.localRotation = b.rotation;
            colorIdx = b.colorIdx;
            mustBeListToWin = b.mustBeListToWin;
            isKinematic = b.isKinematic;
            shape = b.shape;
            circleRadius = b.circleRadius;
            boxSize = b.boxSize;
            useEffector = b.useEffector;
            effectorRadius = b.effectorRadius;
            effectorPower = b.effectorPower;
            useMove = b.useMove;
            direction = b.direction;
            localDirection = b.localDirection;
            useRotation = b.useRotation;
            rotateSpeed = b.rotateSpeed;
            usePivotRotation = b.usePivotRotation;
            pivotRotationSpeed = b.pivotRotateSpeed;
            topSpikes = b.topSpikes.GetCopy();
            botSpikes = b.botSpikes.GetCopy();
            leftSpikes = b.leftSpikes.GetCopy();
            rightSpikes = b.rightSpikes.GetCopy();
            useExplode = b.useExplode;
            showTutorialText = b.showTutorialtext;
            tutorialText = b.tutorialText;
            tutorialFontSize = b.tutorialFontSize;
            tutorialTextOffset = b.tutorialTextOffset;
            tutorialTextPivot = b.tutorialTextPivot;
            tutorialImage = b.tutorialImage;
            tutorialEffect = b.tutorialEffect;
            doNotRevive = b.doNotRevive;            
            
            ValidateEntireBlock();
            if (shape == ShapeType.Polygon)
            {
                List<Vector3> v = new List<Vector3>();
                foreach (var g in b.polygonPoints)
                    v.Add(g);
                shapeMesh.SetPoints(v);
            }
            if (useExplode)
            {
                explodeController.SetPoints(b.explodePoints);
            }
            ToggleEffector(false);
            ValidateEntireBlock();
            
            if (usePivotRotation)
            {
                pivotRotator.pivot.transform.localPosition = b.pivotPos;
                pivotRotator.Refresh();
            }
        }
        #endregion
    }
}