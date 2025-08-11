using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
using UnityEngine;
using LightItUp.Data;
using LightItUp.Game;

namespace LightItUp
{
    [ExecuteInEditMode]
    public class ExplodePartsController : MonoBehaviour
    {
        public class Edge
        {
            public int side;
            public Vector2 start;
            public Vector2 end;
        }

        public Transform partsPivot;
        public List<BlockController> parts;
        [HideInInspector]
        public List<Transform> points;
        public Vector2 size;
        List<List<Vector3>> allPolys;

        private void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && points.Contains(Selection.activeTransform))
            {
                var bc = transform.parent.GetComponent<BlockController>();
                if (bc != null)
                {
                    bc.ValidateShape();
                    bc.ValidatePolyDots();
                }
            }
#endif
        }
        private void OnDrawGizmos()
        {
            GetPoints();
        }

        public void SetSize(Vector2 v, int colorIdx)
        {
            points.RemoveAll(x => x == null);
            size = v;
            GetPoints();
            //Remove null parts
            // add parts not in list
            if (allPolys.Count != parts.Count)
            {
                while (parts.Count > allPolys.Count)
                {
                    EditorActions.DelayedEditorDestroy(parts[parts.Count - 1].gameObject);
                    parts.RemoveAt(parts.Count - 1);
                }
                while (parts.Count < allPolys.Count)
                {
                    parts.Add(Instantiate(PrefabAssets.GetBlock(BlockController.ShapeType.Box), partsPivot));
                }
                // ensure part count
            }
            for (int i = 0; i < allPolys.Count; i++)
            {
                Vector2 center = new Vector2();
                var l = allPolys[i];
                foreach (var a in l) {
                    center += (Vector2)a;
                }
                center /= l.Count;
                for (int u = 0; u < l.Count; u++) {
                    l[u] -= (Vector3)center;
                }
                Debug.DrawLine(center, center +Vector2.up*0.1f);
                parts[i].transform.localPosition = center;
                parts[i].transform.localRotation = Quaternion.identity;
                parts[i].shape = BlockController.ShapeType.Polygon;
                parts[i].colorIdx = colorIdx;
                parts[i].SetPolyPoints(l);
                parts[i].OnValidate();
                // setup parts to fit poly
            }
        }

        void GetPoints()
        {
            allPolys = new List<List<Vector3>>();
            Vector2 tp = transform.position;

            List<Edge> edges = GetEdges();

            for (int i = 0; i < edges.Count - 1; i++)
            {
                AddEdge(edges[i], edges[i + 1]);
            }

            if (edges.Count > 1)
            {
                AddEdge(edges[edges.Count - 1], edges[0]);

                if (edges.Count > 2)
                {
                    List<Vector3> pl2 = new List<Vector3>();
                    foreach (var p in points)
                    {
                        pl2.Add(p.localPosition);
                    }
                    allPolys.Add(pl2);
                }          
            }
/*
        foreach (var v in allPolys)
        {
            for (int i = 0; i < v.Count-1; i++)
            {
                Debug.DrawLine(tp+(Vector2)v[i],tp+(Vector2)v[i + 1]);
            }
            Debug.DrawLine(tp+(Vector2)v[v.Count-1], tp+(Vector2)v[0]);
        }*/
        }
        public List<Edge> GetEdges()
        {
            List<Edge> edges = new List<Edge>();
            for (int i = 0; i < points.Count; i++)
            {
                Vector2 v = points[i].localPosition;
                int side = 0;
                Vector2 end = GetIntersectionPoint(v, out side);
                edges.Add(new Edge
                {
                    side = side,
                    start = v,
                    end = v + end
                });
            }
            return edges;
        }
        void AddEdge(Edge edge1, Edge edge2)
        {
            List<Vector3> pl = new List<Vector3>();
            AddEdges(pl, edge1, edge2);
            allPolys.Add(pl);
        }
        void AddEdges(List<Vector3> pl, Edge edge1, Edge edge2) {
            pl.Add(edge2.end);
            pl.Add(edge2.start);
            pl.Add(edge1.start);
            pl.Add(edge1.end);
            if (edge1.side != edge2.side)
            {
                int side = edge1.side;
                int iterations = 0;
                while (side != edge2.side)
                {
                    pl.Add(GetCornerPoint(side));
                    side = (side + 1) % 4;
                    iterations++;
                    if (iterations > 6)
                    {
                        Debug.LogError("!!");
                        break;
                    }
                    
                }   
            }
        }
        Vector2 GetCornerPoint(int corner)
        {
            var ext = size / 2;
            switch (corner)
            {
                case 0:
                    return new Vector2(ext.x, ext.y);
                case 1:
                    return new Vector2(ext.x, -ext.y);
                case 2:
                    return new Vector2(-ext.x, -ext.y);
                case 3:
                    return new Vector2(-ext.x, ext.y);

                default:
                    Debug.LogError("Invalid corner idx");
                    return Vector2.zero;
            }


        }

        public void SetPoints(List<Vector2> explodePoints)
        {
            while (points.Count > explodePoints.Count)
            {
                EditorActions.DelayedEditorDestroy(points[points.Count - 1].gameObject);
                points.RemoveAt(points.Count - 1);
            }
            while (points.Count < explodePoints.Count)
            {
                CreatePoint();
            }
            for (int i = 0; i < points.Count; i++)
            {
                points[i].localPosition = explodePoints[i];
            }
        }
        public void SetExplosionParts() {
            foreach (var b in parts) b.SetIsExplosionPart();
        }

        Vector2 GetIntersectionPoint(Vector2 v, out int side) {
            side = 0;
            float vm = v.magnitude;
            Vector2 vn = v.normalized;
            var ext = size / 2;
            float minL = float.MaxValue;
            var l = new float[] {
                (ext.y - v.y) / vn.y,
                (ext.x - v.x) / vn.x,
                (-ext.y - v.y) / vn.y,
                (-ext.x - v.x) / vn.x,
            };
            for (int u = 0; u < l.Length; u++)
            {
                if (l[u] > 0 && l[u] < minL)
                {
                    side = u;
                    minL = Mathf.Min(minL, l[u]);
                }
            }
            return vn * minL;
        }
        public void OnValidate()
        {
			#if UNITY_EDITOR
			if (!EditorApplication.isPlayingOrWillChangePlaymode)
			{
            parts.RemoveAll(x => x == null);
            points.RemoveAll(x => x == null);
            List<Transform> t = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++) {
                if (transform.GetChild(i) == partsPivot) continue;
                t.Add(transform.GetChild(i));
                transform.GetChild(i).name = "Point";
            }
            t.RemoveAll(x => x == null);
            t.RemoveAll(x => points.Contains(x));
            points.AddRange(t);
            foreach (var v in points) v.SetAsLastSibling();

            if (transform.parent != null) {
                var bc = transform.parent.GetComponent<BlockController>();
                bc.OnValidate();
            }
				SetExplosionParts();
			}
			#endif
        }
        public Transform CreatePoint() {
            var go = new GameObject("Point").transform;
            go.gameObject.AddComponent<DrawGizmoSphere>();
            go.SetParent(transform);
            go.localPosition = Vector3.zero;
            go.localRotation = Quaternion.identity;
            points.Add(go);
            return go;
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(ExplodePartsController))]
    public class ExplodePartsControllerEditor : Editor
    {
        ReorderableList list;
        private void OnEnable()
        {
            list = GetList("points", "Points", OnRemove);
        }
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        
            serializedObject.Update();
            list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            ExplodePartsController myTarget = (ExplodePartsController)target;
            if (GUILayout.Button("Validate"))
            {
                myTarget.OnValidate();
            }

        }

        void OnRemove(ReorderableList a)
        {
            var mytarget = target as ExplodePartsController;
            var b = mytarget.points[a.index];
            mytarget.points.RemoveAt(a.index);
            if (b != null) DestroyImmediate(b.gameObject);
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

                EditorGUI.PropertyField(rect, element, GUIContent.none);
                GUI.enabled = gui;

            };
            l.onCanAddCallback = (a) => {
                return true;
            };
            l.onAddCallback = (a) =>
            {
                var myTarget = (target as ExplodePartsController);
                var go = myTarget.CreatePoint();
                myTarget.OnValidate();
                Selection.activeGameObject = go.gameObject;
            };
            l.onRemoveCallback = onRemove;
            return l;
        }
    }
#endif
}