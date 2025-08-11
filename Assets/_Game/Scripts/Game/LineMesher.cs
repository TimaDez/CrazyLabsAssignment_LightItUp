using System.Collections.Generic;
using UnityEngine;
using LightItUp.Data;

namespace LightItUp.Game
{
    public class LineMesher : MonoBehaviour
    {
        public MeshFilter mf;
        public MeshRenderer mr;
        public Material mat {
            get {
                if (mr == null) mr = GetComponent<MeshRenderer>();
                if (Application.isPlaying)
                {
                    return mr.material;
                }
                else {
                    return mr.sharedMaterial;
                }
            
            }
        }
        public void SetColor(Color color)
        {
           
            if (!Application.isPlaying)
            {
                Debug.LogError("Cannot set custom color while not playing!!");
            }
            else
            {
                Debug.Log("set Color");
                mat.color = color;
            }
        }
        public void SetColor(int color)
        {
            var c = SpriteAssets.Instance.GetColorFromScheme(0, color);
            if (!Application.isPlaying)
            {
                mr.sharedMaterial = SpriteAssets.Instance.GetColorOutlineMatFromScheme(0, color);
            }
            else
            {
                SetColor(c);
            }
        }
        public void MakeCircle(float radius, float min, float max, int color)
        {
            int partitions = GameSettings.Block.circlePartitions;
            var circ = 2 * Mathf.PI * radius;
            List<Vector3> verts = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> tris = new List<int>();
            float pr = 1f / partitions;
            bool lastWasSpike = false;

            float minIdx = 0;
            float maxIdx = 0;
            for (int u = 0; u < partitions; u++)
            {
                float au = u * pr;
                if (minIdx == 0 && au >= min && min > 0)
                {
                    minIdx = u;
                }
                if (maxIdx == 0 && au >= max)
                {
                    maxIdx = u;
                }
            }
            if (maxIdx == 0) maxIdx = partitions - 1;

            float distIdx = maxIdx - minIdx;
            float spikedDist = Mathf.FloorToInt(circ * pr * distIdx * GameSettings.Block.uvScale);


            for (int i = 0; i < partitions; i++)
            {
                float a = (i * pr);
                Vector2 v = new Vector2(Mathf.Sin(a * 360 * Mathf.Deg2Rad), Mathf.Cos(a * 360 * Mathf.Deg2Rad));
                float aNext = (i + 1) * pr;
                Vector2 vNext = new Vector2(Mathf.Sin(aNext * 360 * Mathf.Deg2Rad), Mathf.Cos(aNext * 360 * Mathf.Deg2Rad));

                bool isSpikes = a >= min && a < max;

                float uvX = spikedDist * (minIdx - i) / distIdx;
                float uvXNext = spikedDist * (minIdx - i+1) / distIdx;

                float uvOffset = 0;
                if (i > 0 && lastWasSpike != isSpikes)
                {
                    if (lastWasSpike)
                        uvOffset = 0.5f;
                    verts.Add(v * (radius - GameSettings.Block.lineThickness * 0.5f));
                    verts.Add(v * (radius + GameSettings.Block.lineThickness * 0.5f));
                    uvs.Add(new Vector2(uvX, uvOffset));
                    uvs.Add(new Vector2(uvX, uvOffset + 0.5f));
                }


                if (isSpikes)
                    uvOffset = 0.5f;
                else
                {
                    uvOffset = 0;
                }
                verts.Add(v * (radius - GameSettings.Block.lineThickness * 0.5f));
                verts.Add(v * (radius + GameSettings.Block.lineThickness * 0.5f));
                uvs.Add(new Vector2(uvX, uvOffset));
                uvs.Add(new Vector2(uvX, uvOffset + 0.5f));
                if (i < partitions) {
                    int u = verts.Count - 2;
                    tris.Add(u);
                    tris.Add(u + 1);
                    tris.Add(u + 2);
                    tris.Add(u + 2);
                    tris.Add(u + 1);
                    tris.Add(u + 3);
                }
            
                if (i == partitions-1)
                {
                    verts.Add(vNext * (radius - GameSettings.Block.lineThickness * 0.5f));
                    verts.Add(vNext * (radius + GameSettings.Block.lineThickness * 0.5f));
                    uvs.Add(new Vector2(uvXNext, uvOffset));
                    uvs.Add(new Vector2(uvXNext, uvOffset + 0.5f));
                }
                lastWasSpike = isSpikes;
            }
            SetMesh(verts, uvs, tris, color);
        }

        public void MakePolygon(List<ShapeMesh.PolyPoint> points, int color)
        {
            Debug.Log("Make Polygon Color");
            List<Vector3> verts = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> tris = new List<int>();

            for (int i = 0; i < points.Count - 1; i++)
            {
                if (points[i].addSpikes && points[i + 1].addSpikes)
                {
                    float mag = GameSettings.Block.polySpikesGraceArea / (points[i].point.localPosition - points[i + 1].point.localPosition).magnitude;
                    AddLineSpiked(points[i].point.localPosition, points[i + 1].point.localPosition, ref verts, ref uvs, ref tris, new BlockController.SpikeSetup
                    {
                        useSpikes = true,
                        minRange = mag,
                        maxRange = 1f - mag
                    });
                }
                else
                {
                    AddLine(points[i].point.localPosition, points[i + 1].point.localPosition,
                        ref verts, ref uvs, ref tris,
                        points[i].addSpikes && points[i + 1].addSpikes);
                }
            }
            if (points.Count > 2) {
                if (points[points.Count - 1].addSpikes && points[0].addSpikes)
                {
                    float mag = GameSettings.Block.polySpikesGraceArea / (points[points.Count - 1].point.localPosition - points[0].point.localPosition).magnitude;
                    AddLineSpiked(points[points.Count - 1].point.localPosition, points[0].point.localPosition, ref verts, ref uvs, ref tris, new BlockController.SpikeSetup
                    {
                        useSpikes = true,
                        minRange = mag,
                        maxRange = 1f - mag
                    });
                }
                else {
                    AddLine(points[points.Count - 1].point.localPosition, points[0].point.localPosition,
                        ref verts, ref uvs, ref tris,
                        points[points.Count - 1].addSpikes && points[0].addSpikes);
                }



            }
        
        

            SetMesh(verts, uvs, tris, color);
        }
        public void MakeQuad(Vector2 size, BlockController.SpikeSetup[] spikes, int color)
        {
            List<Vector3> verts = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> tris = new List<int>();
            Vector2 extents = size / 2;
            Vector2 ul = new Vector2(-extents.x, extents.y);
            Vector2 ur = new Vector2(extents.x, extents.y);
            Vector2 dl = new Vector2(-extents.x, -extents.y);
            Vector2 dr = new Vector2(extents.x, -extents.y);

            MakeQuadLine(ul, ur, ref verts, ref uvs, ref tris, spikes[0]);
            MakeQuadLine(dr, dl, ref verts, ref uvs, ref tris, FlipSpikes(spikes[1]));
            MakeQuadLine(dl, ul, ref verts, ref uvs, ref tris, spikes[2]);
            MakeQuadLine(ur, dr, ref verts, ref uvs, ref tris, FlipSpikes(spikes[3]));

            SetMesh(verts, uvs, tris, color);
        }
        void MakeQuadLine(Vector2 a, Vector2 b, ref List<Vector3> verts, ref List<Vector2> uvs, ref List<int> tris, BlockController.SpikeSetup spikes) {
            float mag = (b - a).magnitude;
            float m = GameSettings.Block.polySpikesGraceArea / mag;
            Vector2 a1 = Vector2.Lerp(a, b, m);
            Vector2 b1 = Vector2.Lerp(a, b, 1-m);
            AddLine(a, a1, ref verts, ref uvs, ref tris, false);
            AddLineSpiked(a1, b1, ref verts, ref uvs, ref tris, spikes);
            AddLine(b1, b, ref verts, ref uvs, ref tris, false);
        }

        public void MakeExploding(Vector2 size, List<ExplodePartsController.Edge> edges, int color) {
            List<Vector3> verts = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> tris = new List<int>();
            Vector2 extents = size / 2;
            Vector2 ul = new Vector2(-extents.x, extents.y);
            Vector2 ur = new Vector2(extents.x, extents.y);
            Vector2 dl = new Vector2(-extents.x, -extents.y);
            Vector2 dr = new Vector2(extents.x, -extents.y);

            AddLineSpiked(ul, ur, ref verts, ref uvs, ref tris, null);
            AddLineSpiked(dr, dl, ref verts, ref uvs, ref tris, null);
            AddLineSpiked(dl, ul, ref verts, ref uvs, ref tris, null);
            AddLineSpiked(ur, dr, ref verts, ref uvs, ref tris, null);

            for (int i = 0; i < edges.Count; i++)
            {
                AddLineSpiked(edges[i].start, edges[i].end, ref verts, ref uvs, ref tris, null);
            }

            if (edges.Count > 1)
            {
                for (int i = 0; i < edges.Count - 1; i++)
                {
                    AddLineSpiked(edges[i].start, edges[i + 1].start, ref verts, ref uvs, ref tris, null);
                }
                if (edges.Count > 1) {
                    AddLineSpiked(edges[edges.Count - 1].start, edges[0].start, ref verts, ref uvs, ref tris, null);
                }
            
            }
        

            SetMesh(verts, uvs, tris, color);
        }


        BlockController.SpikeSetup FlipSpikes(BlockController.SpikeSetup ss) {
            return new BlockController.SpikeSetup
            {
                useSpikes = ss.useSpikes,
                minRange = 1 - ss.maxRange,
                maxRange = 1 - ss.minRange,
            };
        }

        void SetMesh(List<Vector3> verts, List<Vector2> uvs, List<int> tris, int color)
        {
            Mesh m = new Mesh();
            SetColor(color);
            /*List<Color> vertColor = new List<Color>();
        for (int i = 0; i < verts.Count; i++)
        {
            vertColor.Add(color);
        }*/
            m.vertices = verts.ToArray();
            m.triangles = tris.ToArray();
            m.uv = uvs.ToArray();
            // m.colors = vertColor.ToArray();
            mf.mesh = m;
        }


        void AddLineSpiked(Vector3 p1, Vector3 p2, ref List<Vector3> verts, ref List<Vector2> uvs, ref List<int> tris, BlockController.SpikeSetup spikeSetup)
        {
            if (spikeSetup == null) spikeSetup = new BlockController.SpikeSetup();
            if (spikeSetup.useSpikes)
            {
                AddLineSpiked(p1, p2, ref verts, ref uvs, ref tris, spikeSetup.minRange, spikeSetup.maxRange);
            }
            else
            {
                AddLine(p1, p2, ref verts, ref uvs, ref tris, false);
            }
        
        }

        void AddLineSpiked(Vector3 p1, Vector3 p2, ref List<Vector3> verts, ref List<Vector2> uvs, ref List<int> tris, float spikeMin, float spikeMax) {
            Vector3 pMin = Vector3.Lerp(p1, p2, spikeMin);
            Vector3 pMax = Vector3.Lerp(p1, p2, spikeMax);

            if (spikeMin > 0)
            {
                AddLine(p1, pMin, ref verts, ref uvs, ref tris, false);
            }

            AddLine(pMin, pMax, ref verts, ref uvs, ref tris, true);

            if (spikeMax < 1)
            {
                AddLine(pMax, p2, ref verts, ref uvs, ref tris, false);
            }
        }
        void AddLine(Vector3 p1, Vector3 p2, ref List<Vector3> verts, ref List<Vector2> uvs, ref List<int> tris, bool isSpikes) {
            Vector2 o = p2 - p1;
            float mag = o.magnitude * GameSettings.Block.uvScale;
            mag =  Mathf.Max(Mathf.FloorToInt(mag), 1);
            Vector3 v = o.normalized;
            v = new Vector3(-v.y, v.x) * (GameSettings.Block.lineThickness / 2);

            float uvOffset = 0;
            if (isSpikes)
                uvOffset = 0.5f;

            int i = verts.Count;
            verts.Add(p1 + v);
            verts.Add(p1 - v);
            verts.Add(p2 + v);
            verts.Add(p2 - v);

            uvs.Add(new Vector2(0, uvOffset + 0.5f));
            uvs.Add(new Vector2(0, uvOffset));
            uvs.Add(new Vector2(mag, uvOffset + 0.5f));
            uvs.Add(new Vector2(mag, uvOffset));

            tris.Add(i);
            tris.Add(i + 2);
            tris.Add(i + 1);
            tris.Add(i + 1);
            tris.Add(i + 2);
            tris.Add(i + 3);
        }

    }
}
