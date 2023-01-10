using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBombManager : MonoBehaviour
{
    [SerializeField] private int _bombSize = 1;
    [SerializeField] private int _numberOfBomb = 1;
    [SerializeField, Range(0,5)] private float _bombReloadCooldown = 2;
    [SerializeField] private Bomb _bombPrefab;

    private int _currentNumberOfBomb = 1;
    private float _bombReloadCooldownTimer;

    private void Start()
    {
        _bombReloadCooldownTimer = _bombReloadCooldown;
    }

    private void Update()
    {
        BombReload();
        UseBomb();
    }

    private void BombReload()
    {
        //UI
        GameManager.Instance.BombUIReload(_currentNumberOfBomb >= _numberOfBomb ? 1 : _bombReloadCooldownTimer/_bombReloadCooldown);
        
        //cooldown
        _bombReloadCooldownTimer -= Time.deltaTime;
        if (_bombReloadCooldownTimer > 0)
        {
            return;
        }

        //reload
        _bombReloadCooldownTimer = _bombReloadCooldown;
        if (_currentNumberOfBomb < _numberOfBomb)
        {
            _currentNumberOfBomb++;
            GameManager.Instance.ChangeBombNumber(_currentNumberOfBomb);
        }
    }

    private void UseBomb()
    {
        if (_currentNumberOfBomb <= 0)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 position = new Vector2((int)transform.position.x + 0.5f, (int)transform.position.y - 0.5f);
            Bomb bomb = Instantiate(_bombPrefab, position, Quaternion.identity);
            bomb.BombSize = _bombSize;
            _currentNumberOfBomb--;
            
            //place bomb on the map tile for the enemy
            Tile bombTile = bomb.gameObject.GetComponent<Tile>();
            bombTile.Position = new Vector2((int)transform.position.x, (int)transform.position.y);
            MapManager.Instance.MapTileList.Add(bombTile);
            _bombReloadCooldownTimer = _bombReloadCooldown;
            GameManager.Instance.ChangeBombNumber(_currentNumberOfBomb);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Bonus bonus = col.gameObject.GetComponent<Bonus>();
        if (bonus != null)
        {
            switch (bonus.Type)
            {
                case Bonus.BonusType.AddBomb:
                    _numberOfBomb++;
                    break;
                case Bonus.BonusType.BombRange:
                    _bombSize++;
                    break;
            }
            Destroy(bonus.gameObject);
        }
    }
}
