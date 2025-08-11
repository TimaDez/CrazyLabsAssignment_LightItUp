using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using mattatz.Triangulation2DSystem;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
using UnityEngine;
using LightItUp.Data;

namespace LightItUp.Game
{
    [ExecuteInEditMode]
    public class ShapeMesh : MonoBehaviour {
        [Serializable]
        public class PolyPoint {
            public Transform point;
            public bool addSpikes;
        }
        [HideInInspector]
        public List<PolyPoint> points;
        BlockController.ShapeType shape;
        //[SerializeField]Vector2 centerOffset;
        float radius = 0.5f;
        //LineRenderer lr;
        void Awake() {
            if (points.Count == 0)
            {
                points.Clear();
                for (int i = 0; i < transform.childCount; i++)
                {
                    points.Add(new PolyPoint
                    {
                        point = transform.GetChild(i)
                    });
                }
            }        
        }

        public Vector3[] GetPolygonPoints()
        {
            Awake();
            List<Vector3> pos2D = new List<Vector3>();
            for (int i = 0; i < points.Count; i++)
            {
                Vector2 v = points[i].point.localPosition;
                pos2D.Add(v);
            }
            return pos2D.ToArray();
        }
        Vector3[] GetCirclePoints(float radius) {
            List<Vector3> l = new List<Vector3>();
            int partitions = GameSettings.Block.circlePartitions;
            for (int i = 0; i < partitions+1; i++)
            {
                float a = i;
                a /= partitions;
                a *= 360;
                l.Add(new Vector2(Mathf.Sin(a * Mathf.Deg2Rad), Mathf.Cos(a * Mathf.Deg2Rad)) * radius);
            }
            return l.ToArray();
        }
        public Mesh GetMesh() {
            var mf = GetComponent<MeshFilter>();
            return mf.mesh;
        }

        public Mesh GetMesh(Vector3[] pos2D) {
            var l = new Vector2[pos2D.Length];
            for (int i = 0; i < pos2D.Length; i++)
                l[i] = pos2D[i];

            if (shape != BlockController.ShapeType.Circle)
            {
                var polygon = Polygon2D.Contour(l);
                var vertices = polygon.Vertices;
                if (vertices.Length < 3)
                {
                    Debug.LogError("Not enough verts in poly!!!");
                    return new Mesh(); // error
                }
                var triangulation = new Triangulation2D(polygon, 360, 100);
                return triangulation.Build();
            }
            else
            {
                List<int> tris = new List<int>();
                for (int i = 1; i < pos2D.Length-1; i++)
                {
                    tris.Add(0);
                    tris.Add(i);
                    tris.Add(i+1);
                }
                Mesh m = new Mesh();
                m.vertices = pos2D;
                m.triangles = tris.ToArray();
                return m;
            }
        
        }

        public float GetSize()
        {
            float d = 0;
            if (shape == BlockController.ShapeType.Polygon)
            {
                foreach (var p in points)
                {
                    d = Mathf.Max(d, Mathf.Abs(p.point.localPosition.x), Mathf.Abs(p.point.localPosition.y));
                }
            }
            else
                d = radius;
            return d;
        }

        public void SetupCircle(float newRadius) {
            shape = BlockController.ShapeType.Circle;
            radius = newRadius;
            UpdateMesh();
        }
        public void SetupPolygon()
        {
            shape = BlockController.ShapeType.Polygon;
            UpdateMesh();
        }
        void UpdateMesh()
        {
            var mf = GetComponent<MeshFilter>();
            //lr = GetComponent<LineRenderer>();
            if (shape == BlockController.ShapeType.Polygon)
            {
                var p = GetPolygonPoints();
                mf.mesh = GetMesh(p);
                //lr.positionCount = p.Length;
                //lr.SetPositions(p);
            }
            else
            {
                var p = GetCirclePoints(radius);
                mf.mesh = GetMesh(p);
                //lr.positionCount = p.Length;
                //lr.SetPositions(p);
            }
            var bc = GetComponentInParent<BlockController>();
            if (bc != null)
            {
                bc.ValidatePolyDots();
                bc.ValidateShape();
            }
        
        }
        public void SetSprite(Sprite texture)
        {
            if (Application.isPlaying)
            {
                var mr = GetComponent<MeshRenderer>();
                mr.material.mainTexture = texture.texture;
            }

        }
        public void SetColor(int color)
        {
            //lr.startColor = color;
            //lr.endColor = color;
            var mr = GetComponent<MeshRenderer>();
            if (Application.isPlaying)
            {
                mr.material.color = SpriteAssets.Instance.GetColorFromScheme(0, color);
            }
            else
            {
                mr.sharedMaterial = SpriteAssets.Instance.GetColorFillMatFromScheme(0, color);
            }

        }
        public void SetColor(Color color)
        {
            //lr.startColor = color;
            //lr.endColor = color;
            var mr = GetComponent<MeshRenderer>();
            if (Application.isPlaying)
            {
                mr.material.color = color;
            }
            else
            {
                Debug.LogError("Cant set color while not in playmode");
                mr.sharedMaterial = SpriteAssets.Instance.GetColorFillMatFromScheme(0, 0);
            }

        }
        public void SetPoints(List<Vector3> p) {
            points.RemoveAll(x => x.point == null);
            while (p.Count < points.Count)
            {
                EditorActions.DelayedEditorDestroy(points[points.Count - 1].point.gameObject);
                points.RemoveAt(points.Count - 1);
            }
            while (p.Count > points.Count)
            {
                var t = CreatePoint();
                points.Add(new PolyPoint
                {
                    point = t
                });
            }
            for (int i = 0; i < p.Count; i++) {
                if (points[i].point == null) continue;
                points[i].point.localPosition = p[i];
            }
        }
        Transform CreatePoint()
        {
            var t = new GameObject("Point");
            if (t != null)
            {
                t.transform.SetParent(transform);
                PlacePoint(t.transform);
                return t.transform;
            }
            return null;
        }
        void PlacePoint(Transform t)
        {
            t.SetAsLastSibling();
            var p = t.localPosition;
            p.z = 0;
            t.localPosition = p;
            t.name = "Point";
        }
#if UNITY_EDITOR
        public void OnValidate()
        {
            switch (shape)
            {
                case BlockController.ShapeType.Box:
                    shape = BlockController.ShapeType.Circle;
                    break;

            }
            points.RemoveAll(x => x.point == null);
            for (int i = 0; i < points.Count; i++)
            {
                PlacePoint(points[i].point);
            }
        }




        void Update() {
            if (!Application.isPlaying)
            {
                points.RemoveAll(x => x.point == null);

                if (transform.childCount != points.Count)
                {
                    List<Transform> children = new List<Transform>();
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        children.Add(transform.GetChild(i));
                    }
                    for (int u = children.Count - 1; u >= 0; u--) {
                        if (points.FirstOrDefault(x => x.point == children[u]) != null) {
                            children.RemoveAt(u);
                        }
                    }
                    foreach (var c in children) {
                        points.Add(new PolyPoint {
                            point = c,
                            addSpikes = false
                        });
                    }
                    for (int i = 0; i < points.Count; i++)
                    {
                        points[i].point.SetAsLastSibling();
                        points[i].point.name = "Point " + i;
                    }
                }

                UpdateMesh();
            }
        }


#endif
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(ShapeMesh))]
    public class ShapeMeshEditor : Editor
    {
        ReorderableList list;

        private void OnEnable()
        {
            list = GetList("points", "Points", OnRemove);
        }
        public override void OnInspectorGUI()
        {
            ShapeMesh myTarget = (ShapeMesh)target;
            DrawDefaultInspector();

            serializedObject.Update();
            list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
            if (GUILayout.Button("Validate"))
            {
                var bc = myTarget.transform.parent.GetComponent<BlockController>();
                bc.OnValidate();

            }
        }
        void OnRemove(ReorderableList a) {
            var mytarget = target as ShapeMesh;
            var b = mytarget.points[a.index];
            mytarget.points.RemoveAt(a.index);
            if (b != null) DestroyImmediate(b.point.gameObject);
        }
        public ReorderableList GetList(string propertyName, string header, ReorderableList.RemoveCallbackDelegate onRemove)
        {
            var l = new ReorderableList(serializedObject,
                serializedObject.FindProperty(propertyName),
                true, true, true, true);
            l.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, header);
            };
            l.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                var gui = GUI.enabled;
                GUI.enabled = false;
                var element = l.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;
                Rect a = rect;
                a.width -= 100;
                var b = rect;
                b.x += a.width;
                b.width = 100;
                var c = b;
                c.x += 50;
                c.width -= 50;
                EditorGUI.PropertyField(a, element.FindPropertyRelative("point"), GUIContent.none);
                GUI.enabled = gui;
                EditorGUI.LabelField(b, "Spikes");
                EditorGUI.PropertyField(c, element.FindPropertyRelative("addSpikes"), GUIContent.none);

            };
            l.onCanRemoveCallback = (a) => {
                return a.count > 3;
            };
            l.onCanAddCallback = (a) => {
                return true;
            };
            l.onAddCallback = (a) =>
            {
                var myTarget = (target as ShapeMesh);
                var go = new GameObject("Point").transform;
                go.gameObject.AddComponent<DrawGizmoSphere>();
                go.SetParent(myTarget.transform);
                go.localPosition = Vector3.zero;
                go.localRotation = Quaternion.identity;
                myTarget.points.Add(new ShapeMesh.PolyPoint
                {
                    point = go
                });
                myTarget.OnValidate();
            };
            l.onRemoveCallback = onRemove;
            return l;
        }

    }

#endif
}