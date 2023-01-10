using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [ReadOnly] public int BombSize;
    [SerializeField] private float _timeToExplode;
    [SerializeField] private ExplosionSide _explosionSidePrefab;

    private bool _isExplosion;
    private List<Vector2> _directions;
    private SpriteView _spriteView;

    private void Start()
    {
        transform.localScale = Vector3.one * 0.5f;
        transform.DOScale(Vector3.one, _timeToExplode);
        
        _directions = new List<Vector2>
        {
            new Vector2(0, 1),
            new Vector2(0, -1),
            new Vector2(1,0),
            new Vector2(-1,0)
        };

        _spriteView = GetComponent<SpriteView>();
        _spriteView.OnActionEnd.AddListener(DestroyItself);
    }

    private void OnDestroy()
    {
        _spriteView.OnActionEnd.RemoveListener(DestroyItself);
    }

    private void Update()
    {
        CountdownExplosion();
    }

    private void CountdownExplosion()
    {
        _timeToExplode -= Time.deltaTime;
        if (_timeToExplode <= 0)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (_isExplosion)
        {
            return;
        }
        _isExplosion = true;
        
        //detect tiles to damage
        foreach (Vector2 direction in _directions)
        {
            bool canExplode = true;
            for (int i = 1; i <= BombSize; i++)
            {
                if (canExplode == false)
                {
                    continue;
                }
                //destruction
                Tile tile = MapManager.Instance.MapTileList
                    .Find(x => x.Position == new Vector2((int)transform.position.x + direction.x*i, (int)transform.position.y + direction.y*i));
                if (tile != null)
                {
                    IDamageable damageable = tile.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage();
                        MapManager.Instance.DeleteTileFromList(tile);
                        ExplosionSideAnimation(direction, i);
                        GameManager.Instance.ChangeScore(50);
                    }
                    else
                    {
                        canExplode = false;
                    }
                }
                else //if no tile
                {
                    ExplosionSideAnimation(direction, i);
                }
            }
        }

        //animation
        _spriteView.PlayAction("Explode");
           
        //TODO put explosion on other tiles
    }

    private void ExplosionSideAnimation(Vector2 direction, int i)
    {
        //animation
        Vector3 position = new Vector3(transform.position.x + direction.x * i, transform.position.y + direction.y * i, 0);
        ExplosionSide explosionSide = Instantiate(_explosionSidePrefab, position, quaternion.identity);
        if (i == BombSize)
        {
            explosionSide.IsEnd = true;
        }
        
        //rotation
        int rotation = direction.x != 0 ? 
            direction.x < 0 ? 90 : 270 :
            direction.y < 0 ? 180 : 0;
        Vector3 rotationVector = new Vector3(0, 0, rotation);
        explosionSide.transform.rotation = Quaternion.Euler(rotationVector);
    }

    private void DestroyItself()
    {
        MapManager.Instance.DeleteTileFromList(gameObject.GetComponent<Tile>());
        Destroy(gameObject);
    }
}
