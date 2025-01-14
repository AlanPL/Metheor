using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CharactersList : UpdatableData
{
    public CharacterData [] charactersData;
}

[System.Serializable]
public struct CharacterData
{

      public GameObject  meteoritePrefab;
      public GameObject  meteoriteModelFract;      
      public Color [] meteorParticlesColor;
      public bool overwriteMaterial;
      public Material material;
      public int price;
      public Sprite img;

}
