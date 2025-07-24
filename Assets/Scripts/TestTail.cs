using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestTail : MonoBehaviour
{
    public Tilemap tilemap;
    public RuleTile wallTile;
    public RuleTile floorTile;
    // Start is called before the first frame update
    void Start()
    {
        tilemap.ClearAllTiles();
        tilemap.SetTile(new Vector3Int(0, 0,1), wallTile);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
