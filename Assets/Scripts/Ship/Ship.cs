using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public List<ShipChunk> Chunks = new List<ShipChunk>();
    
    public Vector3 Position
    {
        get
        {
            Vector3 pos = new Vector3();
            foreach (var item in Chunks)
            {
                pos += item.transform.position;                
            }

            return pos / Chunks.Count;
        }
    }
}
