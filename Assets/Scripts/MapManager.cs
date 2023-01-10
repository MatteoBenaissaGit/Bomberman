using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    //singleton
    public static MapManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
            Instance = this;
        }
    }


    [SerializeField, Header("Map Generation")] private List<TilePrefab> _tilePrefabList = new List<TilePrefab>();
    [SerializeField, TextArea] private string _mapText;
    [SerializeField] private char _voidIndex;
    [SerializeField] private TileExit _tileExit;
    [SerializeField] private char _exitIndex;
    [SerializeField, Space(10), Header("Enemies")] private Enemy _enemyPrefab;
    [SerializeField] private List<Vector2> _enemySpawnedList;
    [SerializeField, Space(10), Header("Player")] private TopDownCharacterController _playerPrefab;
    [SerializeField] private Vector2 _playerSpawnPosition;

    private string[] _mapArray;
    private Dictionary<char, Tile> _tileDictionary = new Dictionary<char, Tile>();
    [HideInInspector] public Vector2 MapSize;
    [HideInInspector] public List<Tile> MapTileList = new List<Tile>();
    [HideInInspector] public List<Tile> GrassTileList = new List<Tile>();
    [ReadOnly] public List<Enemy> EnemyList = new List<Enemy>();

    private void Start()
    {
        DataCreation();
        GenerateMap();
        EnemyAndPlayerPlacement();
        CameraPlacement();
    }

    private void DataCreation()
    {
        //text array
        _mapArray = _mapText.Split('\n');
        
        //tile dictionary
        foreach (var tilePrefab in _tilePrefabList)
        {
            _tileDictionary.Add(tilePrefab.ID, tilePrefab.Prefab);
        }
        
        //mapSize
        MapSize = new Vector2(_mapArray[0].Length, _mapArray.Length);
    }

    private void GenerateMap()
    {
        
        for (int y = 0; y < MapSize.y; y++)
        {
            for (int x = 0; x < MapSize.x; x++)
            {
                //grass tile
                Tile grassTile = Instantiate(_tilePrefabList.FirstOrDefault().Prefab, new Vector3(x, -y, 0), Quaternion.identity);
                grassTile.transform.SetParent(transform);
                grassTile.Init(new Vector2(x, -y));
                GrassTileList.Add(grassTile);
                
                //other tiles
                char index = _mapArray[y][x];
                
                //exit tile
                if (index == _exitIndex)
                {
                    TileExit exitTile = Instantiate(_tileExit, new Vector3(x, -y, 0), Quaternion.identity);
                    GameManager.Instance.TileExit = exitTile;
                }
                
                //walls
                if (index == null || index == _voidIndex || _tileDictionary.ContainsKey(index) == false)
                {
                    continue;
                }
                
                Tile tile = Instantiate(_tileDictionary[index], new Vector3(x, -y, 0), Quaternion.identity);
                tile.Init(new Vector2(x, -y));
                tile.transform.SetParent(transform);
                MapTileList.Add(tile);
            }
        }
    }

    private void EnemyAndPlayerPlacement()
    {
        //enemies
        foreach (Vector2 position in _enemySpawnedList)
        {
            if (position.x <= MapSize.x && position.y <= MapSize.y && position.x >= 0 && position.y >= 0)
            {
                Enemy enemy = Instantiate(_enemyPrefab, new Vector3(position.x + 0.5f, -position.y - 0.5f, 0), Quaternion.identity);
                EnemyList.Add(enemy);
            }
        }
        
        //player
        if (_playerSpawnPosition.x <= MapSize.x && _playerSpawnPosition.y <= MapSize.y && _playerSpawnPosition.x >= 0 && _playerSpawnPosition.y >= 0)
        {
            Instantiate(_playerPrefab, new Vector3(_playerSpawnPosition.x+0.5f, -_playerSpawnPosition.y-0.5f, 0), Quaternion.identity);
        }
    }

    private void CameraPlacement()
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            return;
        }

        camera.transform.position = new Vector3(MapSize.x/2, -MapSize.y/2,-10);
    }

    public void DeleteTileFromList(Tile tile)
    {
        MapTileList.Remove(tile);
    }
}

[Serializable]
public struct TilePrefab
{
    public char ID;
    public Tile Prefab;
}
