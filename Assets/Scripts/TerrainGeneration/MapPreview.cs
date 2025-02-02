﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapPreview : MonoBehaviour {

	GameObject [][] objetos;
	public GameObject background;
	GameObject displayedObjectsParent;
	public ObjectPlacingList objectPlacingList;
	public enum DrawMode {Map, DeleteChilds};
	public DrawMode drawMode;

	public bool autoUpdate;
	public bool autoGenerate;
	public int chunkSize;
	public int actualScore;

	public void DrawMapInEditor() {
		if (autoGenerate) {
			if (drawMode == DrawMode.Map) {
				DrawMap ();
			} else if (drawMode == DrawMode.DeleteChilds) {
				GameObject.DestroyImmediate(displayedObjectsParent);
			}
		}

	}

	public void DrawMap() {
		ObjectGenerator.DeleteObjectsInPreview(ref displayedObjectsParent);
                    //background.transform.localScale = new Vector3( (float)chunkSize/10, (float)chunkSize/10, (float)chunkSize/10 );

		displayedObjectsParent = new GameObject("Displayed Objects Parent");
		displayedObjectsParent.transform.parent = transform;
		ObjectGenerator.GenerateObjectsInPreview(ref objetos, objectPlacingList, chunkSize,  displayedObjectsParent.transform, actualScore );

		displayedObjectsParent.gameObject.SetActive(true);
	}

	void OnValuesUpdated(){
		if (!Application.isPlaying) {
			DrawMapInEditor();
		}

	}

	public void OnValidate(){
		if (objectPlacingList!=null) {
			objectPlacingList.OnValuesUpdated-=OnValuesUpdated;
			objectPlacingList.OnValuesUpdated+=OnValuesUpdated;
		}
	}

}
