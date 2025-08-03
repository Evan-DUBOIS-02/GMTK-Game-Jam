using System;
using Game;
using UnityEngine;

namespace Player
{
    public class PlayerMovement: MonoBehaviour
    {
        [SerializeField] private float _moveSpeed;
        private Rigidbody2D _rb;
        private Vector2 _movement;
        private Vector3 _startPosition;
        private Vector2 _lastPosition;
        private bool _isFacingRight = true;
        Animator _animator;
        [SerializeField]
        GameObject _rendererGO;
        

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _startPosition = transform.position;
            _animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            _movement.x = Input.GetAxisRaw("Horizontal");
            _movement.y = Input.GetAxisRaw("Vertical");
            _movement.Normalize();

            FlipSprite();
        }

        private void FixedUpdate()
        {
            if (GameManager.Instance.IsEndLevel)
                return;
            
            Vector2 newPosition = _rb.position + _movement * _moveSpeed * Time.fixedDeltaTime;
            _rb.MovePosition(newPosition);

            Vector2 velocity = (newPosition - _lastPosition) / Time.fixedDeltaTime;
            _lastPosition = newPosition;

            _animator.SetFloat("xVelocity", (Mathf.Abs(velocity.x) + Mathf.Abs(velocity.y)));
            //_animator.SetFloat("xVelocity", Mathf.Abs(velocity.y));
        }

        public void InitializePosition()
        {
            transform.position = _startPosition;
        }

        public bool IsMoving()
        {
            return _movement != Vector2.zero;
        }

        private void FlipSprite()
        {
            if (_isFacingRight && _movement.x < 0f || !_isFacingRight && _movement.x > 0f) 
            {
                _isFacingRight = !_isFacingRight;
                GetComponent<PlayerManager>()._currentState.IsFacingRight = _isFacingRight;
                Vector3 ls = _rendererGO.transform.localScale;
                ls.x *= -1f;
                _rendererGO.transform.localScale = ls;
            }
        }
    }
}