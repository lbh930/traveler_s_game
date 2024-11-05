using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;

public class MapScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 poss;
    public int layerr;
    bool initialized = false;

    public Tilemap[] terrainLayers;
    public Tilemap[] buildingLayers;
    public Transform traveler;
    public int currentLayer = 0;
    public Vector2 screenSize = new Vector2(20, 12);
    public Vector2 stepSize = new Vector2(2, 2);
    public Vector3 shifting = new Vector3(1, 0, 1);
    public float blockHeight = 2.5f;
    public float darkenEachLayer = 0.03f;

    IdentityList idList;
    NavMeshSurface navmesh;
    void Initialize()
    {
        
        if (initialized) return;
        idList = GameObject.FindObjectOfType<IdentityList>();
        navmesh = GetComponent<NavMeshSurface>();
        initialized = true;
    }

    void Start()
    {
        Initialize();
        CreateNavMesh();
    }

    void RefreshScreen(int layer)
    {
        Tilemap map = terrainLayers[layer];
        Vector3 center = traveler.position;

        for (int i = Mathf.RoundToInt(screenSize.x/-2); i <= Mathf.RoundToInt(screenSize.x / 2); i++)
        {
            for(int j = Mathf.RoundToInt(screenSize.y / -2); j <= Mathf.RoundToInt(screenSize.y / 2); j++)
            {
                Vector3 cellWorldPos = new Vector3(Mathf.RoundToInt(center.x + i * stepSize.x), map.transform.position.y - blockHeight,
                    Mathf.RoundToInt(center.z + j * stepSize.y));
                Vector3Int cellPos = map.WorldToCell(new Vector3(cellWorldPos.x, map.transform.position.y + 0.01f, cellWorldPos.z));
                TileBase tileBase = map.GetTile(cellPos);

                if (tileBase != null) {
   
                    if (tileBase == idList.tilelist[0]) continue; //如果该格子已经生成，跳过
                    
                    int index = 0;
                    for (index = 0; index < idList.tilelist.Length; index++) //找到在idlist的对应
                    {
                        if (idList.tilelist[index] == tileBase)
                        {
                            Vector3 spawnPos = map.CellToWorld(cellPos);
                            BlockScript bs = Instantiate(idList.blockList[index], 
                                new Vector3 (spawnPos.x + stepSize.x/2, cellWorldPos.y, spawnPos.z + stepSize.y/2), transform.rotation).GetComponent<BlockScript>();
                            SetOccupation(bs, map, cellPos);
                            bs.SetBlock();
                            bs.transform.SetParent(map.transform);
                            bs.mapScript = this;
                            bs.layer = layer;
                            map.SetTile(cellPos, idList.tilelist[0]);
                            break;
                        }
                    }
                }
            }
        }
    }

    public void RefreshBlock(Vector3 pos, int layer)
    {
        pos += new Vector3(0, 0.01f, 0);
        Vector3Int c = terrainLayers[layer].WorldToCell(pos);
        for (int i = 0; i <= 3; i++)
        {
            BlockScript bs = GetBlock(Algori.GetNearbyPos(pos, i), layer);            
            if (bs != null)
            {               
                Vector3Int v =
                    terrainLayers[layer].WorldToCell(new Vector3(bs.transform.position.x, terrainLayers[layer].transform.position.y + 0.01f, bs.transform.position.z));
                SetOccupation(bs, terrainLayers[layer], v);
                bs.SetBlock();
            }
        }
        if (layer - 1 >= 0)
        {
            BlockScript bs = GetBlock(pos, layer - 1);
           
            if (bs != null)
            {
                Vector3Int v =
                    terrainLayers[layer].WorldToCell(new Vector3(bs.transform.position.x, terrainLayers[layer].transform.position.y + 0.01f, bs.transform.position.z));
                SetOccupation(bs, terrainLayers[layer - 1], v);
                bs.SetBlock();
            }
        }
    }

    public void SetTilemap (Vector3 pos, int layer, TileBase t)
    {
        pos += new Vector3(0, 0.01f, 0);
        Vector3Int c = terrainLayers[layer].WorldToCell(pos);
        terrainLayers[layer].SetTile(c, t);
    }

    public BlockScript GetBlock(Vector3 pos, int layer)
    {
        RaycastHit[] hit = Physics.RaycastAll(pos + Vector3.up * 1, Vector3.up * -1, 3.0f);

        if (hit.Length > 0)
        {
            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].transform.parent == terrainLayers[layer].transform)
                {
                    BlockScript block = hit[i].transform.GetComponent<BlockScript>();
                    if (block != null) return block;
                }
            }
        }
        return null;

    }

    public void SetOccupation (BlockScript bs, Tilemap map, Vector3Int origin)
    {
        if (bs == null) return;

        for (int i = 0; i <= 3; i++)
        {
            if (map.GetTile(Algori.GetNearby (origin, i)) != null)
            {
                bs.aroundOccupation[i] = true;
            }
            else
            {
                bs.aroundOccupation[i] = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Initialize();
        for (int i = 0; i < terrainLayers.Length; i++)
        {
            RefreshScreen(i);
            terrainLayers[i].color = new Color(1, 1, 1, 0);
            if (Mathf.Abs (terrainLayers[i].transform.position.y - traveler.position.y) < 0.01f)
            {
                currentLayer = i;
            }
        }

        if (Input.GetKey (KeyCode.R))
        {
            navmesh.BuildNavMesh();
        }
    }

    void CreateNavMesh()
    {
        navmesh.BuildNavMesh();
    }
}
