using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class TileExit : Tile
{
    [SerializeField] private Transform _wall;
    [ReadOnly, SerializeField] private bool _canExit;

    private BoxCollider2D _boxCollider2D;

    private void Start()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }

    public void Exit()
    {
        Destroy(_wall.gameObject);
        _canExit = true;
        _boxCollider2D.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        TopDownCharacterController player = col.gameObject.GetComponent<TopDownCharacterController>();
        if (player != null)
        {
            GameManager.Instance.Win();
            print("win");
        }
    }
}
