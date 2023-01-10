
using System.Collections.Generic;
using UnityEngine;

public class DestructibleWall : Tile, IDamageable
{
    [SerializeField] private List<Bonus> _bonusList;
    [SerializeField] private ParticleSystem _explosionParticle;
    
    public void TakeDamage()
    {
        Die();
    }

    public void Die()
    {
        //bonus
        Instantiate(_explosionParticle, transform.position + new Vector3(0.5f, -0.5f, 0), Quaternion.identity);
        System.Random rnd = new System.Random();
        int randomNumber = rnd.Next(0,4);
        if (randomNumber <= _bonusList.Count-1)
        {
            Instantiate(_bonusList[randomNumber], transform.position + new Vector3(0.5f,-0.5f,0), Quaternion.identity);
        }
        
        //destroy
        MapManager.Instance.DeleteTileFromList(this);
        Destroy(gameObject);
    }
}
