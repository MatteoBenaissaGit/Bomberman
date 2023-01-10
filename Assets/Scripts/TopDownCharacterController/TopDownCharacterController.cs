using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

namespace DefaultNamespace
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class TopDownCharacterController : MonoBehaviour, IDamageable
    {
        [SerializeField] private int _life = 3;
        [SerializeField] private float _damageInvicibleTime = 1;
        [SerializeField, Range(0,20)] private float _speed = 5;
        [SerializeField, Range(0,1)] private float _deceleration = 0.5f;
        [SerializeField, Range(0,1)] private float _acceleration = 0.5f;
        [SerializeField] private ParticleSystem _dustParticle;
        [SerializeField] private ParticleSystem _explosionParticle;

        [ReadOnly] public bool CanMove = true;
        
        private Vector2 _inputs;
        private Rigidbody2D _rigidbody;
        private SpriteView _spriteView;
        private float _currentDamageInvicibleTime;

        private void Start()
        {
            _dustParticle.Stop();
            _rigidbody = GetComponent<Rigidbody2D>();
            _spriteView = GetComponent<SpriteView>();
            
            _spriteView.OnActionEnd.AddListener(EndAction);
        }

        private void OnDestroy()
        {
            _spriteView.OnActionEnd.RemoveListener(EndAction);
        }

        private void Update()
        {
            HandleMovementInputs();
            _currentDamageInvicibleTime -= Time.deltaTime;
        }

        private void FixedUpdate()
        {
            ApplyAnimation();
            ApplyMovement();
        }

        private void ApplyMovement()
        {
            float speed = CanMove && _inputs.magnitude > 0.1f ? _speed : 0;
            _rigidbody.velocity = _inputs.normalized * speed;
        }

        private void HandleMovementInputs()
        {
            //get raw inputs
            float rawInputX = Input.GetAxisRaw("Horizontal");
            float rawInputY = Input.GetAxisRaw("Vertical");
            //lerp current value toward raw
            float lerpValue = Mathf.Abs(rawInputX) + Mathf.Abs(rawInputY) < 0.1f ? _deceleration : _acceleration;
            float lerpX = Mathf.Lerp(_inputs.x, rawInputX, lerpValue);
            float lerpY = Mathf.Lerp(_inputs.y, rawInputY, lerpValue);
            //assign input value
            _inputs = new Vector2(lerpX, lerpY);
        }
        
        private void ApplyAnimation()
        {
            transform.localScale = new Vector3(1 * Math.Sign(_inputs.x), 1, 1);
            if (transform.localScale.x == 0)
            {
                transform.localScale = Vector3.one;
            }
            
            string stateToPlay = String.Empty;
            if (_inputs.magnitude > 0.1f)
            {
                stateToPlay = Math.Abs(_rigidbody.velocity.x) > Math.Abs(_rigidbody.velocity.y) ? "WalkSide" :
                    _inputs.y > 0 ? "WalkBack" : "WalkFront";
            }
            else
            {
                stateToPlay = "IdleFront";
            }

            if (stateToPlay != String.Empty)
            {
                _spriteView.PlayState(stateToPlay);
            }
            
            //dust
            if (_inputs.magnitude != 0 && _dustParticle.isPlaying == false)
            {
                _dustParticle.Play();
            }
            else if (_inputs.magnitude == 0)
            {
                _dustParticle.Stop();
            }
        }

        private void EndAction()
        {
            CanMove = true;
        }

        public void TakeDamage()
        {
            //invicible time
            if (_currentDamageInvicibleTime > 0)
            {
                return;
            }
            _currentDamageInvicibleTime = _damageInvicibleTime;
            
            _life--;
            if (_life <= 0)
            {
                Die();
            }
            else
            {
                //animation
                transform.DOPunchScale(Vector3.one * 0.4f, 0.3f);
            }
            
            //ui
            GameManager.Instance.ChangeLife((float)_life/3);
        }

        public void Die()
        {
            Instantiate(_explosionParticle, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            Enemy enemy = col.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                TakeDamage();
            }
        }
    }
}