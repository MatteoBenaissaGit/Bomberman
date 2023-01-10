using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField, Range(0,4)] private float _moveCooldownInSeconds;
    [SerializeField, Range(0,2)] private float _moveSpeedInSeconds;
    [SerializeField] private ParticleSystem _explosionParticle;

    private float _moveCooldownTimer;
    private Vector2 _mapSize;
    private List<Vector2> _directions;
    private Vector2 _lastDirection = Vector2.zero;
    private SpriteView _spriteView;

    private void Start()
    {
        _mapSize = MapManager.Instance.MapSize;
        
        _spriteView = GetComponent<SpriteView>();
        
        _moveCooldownTimer = _moveCooldownInSeconds;
        
        _directions = new List<Vector2>
        {
            new Vector2(0, 1),
            new Vector2(0, -1),
            new Vector2(1,0),
            new Vector2(-1,0)
        };
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        //cooldown
        _moveCooldownTimer -= Time.deltaTime;
        if (_moveCooldownTimer > 0)
        {
            return;
        }
        _moveCooldownTimer = _moveCooldownInSeconds;
        
        //check if enemy is blocked
        if (IsEnemyBlocked())
        {
            return;
        }
        
        //get move direction
        System.Random rnd = new System.Random();
        int randomNumber = rnd.Next(0,4);
        Vector2 direction = _directions[randomNumber];
        Vector2 intNextPosition = new Vector2((int)transform.position.x + direction.x, (int)transform.position.y + direction.y);
        
        //if necessary change move direction
        for (int i = 0; i < 10; i++)
        {
            if (direction == _lastDirection)
            {
                int newRandomNumber = rnd.Next(0,4);
                direction = _directions[newRandomNumber];
                intNextPosition = new Vector2((int)transform.position.x + direction.x, (int)transform.position.y + direction.y);
            }
        }
        while (IsPositionAvailable(intNextPosition) == false)
        {
            int newRandomNumber = rnd.Next(0,4);
            direction = _directions[newRandomNumber];
            intNextPosition = new Vector2((int)transform.position.x + direction.x, (int)transform.position.y + direction.y);
        }
        _lastDirection = -direction;

        //move
        transform.DOComplete();
        Vector3 newPosition = transform.position + new Vector3(direction.x, direction.y, 0);
        transform.DOMove(newPosition , _moveSpeedInSeconds).SetEase(Ease.Linear);
        
        //animation
        _spriteView.PlayState(direction.x != 0 ? "WalkSide" : direction.y > 0 ? "WalkUp" : "WalkDown");
        transform.localScale = new Vector3(1 * Math.Sign(direction.x), 1, 1);
        if (transform.localScale.x == 0)
        {
            transform.localScale = Vector3.one;
        }
    }

    public void TakeDamage()
    {
        Die();
    }

    public void Die()
    {
        GameManager.Instance.ChangeScore(150);
        Instantiate(_explosionParticle, transform.position, Quaternion.identity);
        MapManager.Instance.EnemyList.Remove(this);
        Destroy(gameObject);
        GameManager.Instance.CheckExit();
    }

    private bool IsPositionAvailable(Vector2 position)
    {
        if (position.x >= _mapSize.x || position.x < 0 || Mathf.Abs(position.y) >= _mapSize.y || Mathf.Abs(position.y) < 0)
        {
            return false;
        }

        Tile tileAtPosition = MapManager.Instance.MapTileList.Find(x => x.Position == position);
        
        if (tileAtPosition == null || tileAtPosition.Position != position)
        {
            return true;
        }
        
        return tileAtPosition.IsWall == false;
    }

    private bool IsEnemyBlocked()
    {
        Vector2 upPosition = new Vector2((int)transform.position.x + 0, (int)transform.position.y + 1);
        Vector2 downPosition = new Vector2((int)transform.position.x + 0, (int)transform.position.y + -1);
        Vector2 leftPosition = new Vector2((int)transform.position.x + -1, (int)transform.position.y + 0);
        Vector2 rightPosition = new Vector2((int)transform.position.x + 1, (int)transform.position.y + 0);

        if (IsPositionAvailable(upPosition) == false &&
            IsPositionAvailable(downPosition) == false &&
            IsPositionAvailable(leftPosition) == false &&
            IsPositionAvailable(rightPosition) == false)
        {
            return true;
        }

        return false;
    }
}