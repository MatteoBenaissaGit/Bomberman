using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSide : MonoBehaviour
{
    private SpriteView _spriteView;
    public bool IsEnd;

    private void Start()
    {
        _spriteView = GetComponent<SpriteView>();
        _spriteView.OnStateEnd.AddListener(DestroyItself);
        
        //anim choice
        if (IsEnd == false)
        {
            _spriteView.PlayState("Intersection");
        }
    }

    private void OnDestroy()
    {
        _spriteView.OnStateEnd.RemoveListener(DestroyItself);
    }

    private void DestroyItself()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        print(col.name);
        IDamageable damageable = col.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage();
        }
    }
}
