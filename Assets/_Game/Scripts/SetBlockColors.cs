using System.Collections;
using System.Collections.Generic;
using LightItUp.Game;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// [ExecuteInEditMode]
public class SetBlockColors : MonoBehaviour {

	private Vector3[] _initPositions;

	public void SetColours(){
		int index = 0;
		var t = GetComponentsInChildren<BlockController>();
		foreach (BlockController b in t){
			b.colorIdx = index;
			index++;
			index = index % 5;
			b.ValidateShape();
		}
	}

	public void SetRandomSizes(){
		var maxSize = 3f;
		var minRadius = 1f;
		var maxRadius = 2f;

		var t = GetComponentsInChildren<BlockController>();
		foreach (BlockController b in t){
			if (b.useMove || b.useExplode || b.useRotation || b.useEffector)
				continue;
			var shape = Random.Range(0f,1f);
			if (shape > 0.66f){
				b.shape = BlockController.ShapeType.Circle;
				b.circleRadius = Random.Range(minRadius,maxRadius);
			} else if (shape > 0.33f) {
				b.shape = BlockController.ShapeType.Box;
				var x = Random.Range(3f,maxSize);
				var y = 0f;
				if (x > 3)
					y = Random.Range(1.5f, maxSize*0.5f);
				else
					y = Random.Range(maxSize*0.5f,maxSize);
				b.boxSize = new Vector2(x,y);
			} else {
				b.shape = BlockController.ShapeType.Polygon;	
			}
			if(b.shape != BlockController.ShapeType.Polygon){
				b.ValidateShape();
				b.ValidatePolyDots();
			}
		}
	}

	public void SetRandomOffsets(){
		var minRange = 7f;
		var maxRange = 15f;
		var t = GetComponentsInChildren<BlockController>();
		int i = 0;
		if (_initPositions == null){
			_initPositions = new Vector3[t.Length];
			
			foreach (BlockController b in t){
				_initPositions[i] = b.transform.position;
				i++;
			} 
		}
		i = 0;
		foreach (BlockController b in t){
			if (b.useMove || b.useExplode || b.useRotation || b.useEffector)
				continue;
				b.transform.position = _initPositions[i] + new Vector3(Random.Range(minRange, maxRange),Random.Range(minRange, maxRange),0f);
				i++;
		}
	}

	// private IEnumerator ScalePoly(BlockController b){
	// 	yield return null;
	// 	yield return null;
	// 	var parent = b.transform.Find("ShapeMesh");
	// 	var points = parent.GetComponentInChildren<DrawGizmoSphere>().transform;
	// 	foreach (Transform p in points){
	// 		p.Translate(new Vector3(Random.Range(0,2f),Random.Range(0,2f),0));
	// 	}
	// 	// b.ValidateShape();
	// 	// b.ValidatePolyDots();
	
	// }

	public void SetRandomRotation(){
		var t = GetComponentsInChildren<BlockController>();
		foreach (BlockController b in t){
			if (b.useMove || b.useExplode || b.useRotation || b.useEffector)
				continue;
			b.transform.localRotation = Quaternion.Euler(0,0,Random.Range(0,360));
		}
	}
	
}

#if UNITY_EDITOR
[CustomEditor(typeof(SetBlockColors))]
public class SetBlockColorsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SetBlockColors myTarget = (SetBlockColors)target;
        if (GUILayout.Button("Set Colours"))
        {
            myTarget.SetColours();
        }
 		if (GUILayout.Button("Random Boxes"))
        {
            myTarget.SetRandomSizes();
        }
		if (GUILayout.Button("Random Rotation"))
        {
            myTarget.SetRandomRotation();
        }
		if (GUILayout.Button("Random Offsets"))
        {
            myTarget.SetRandomOffsets();
        }

    }

	void OnInspectorUpdate()
    {
        Repaint();
    }
}
#endif
