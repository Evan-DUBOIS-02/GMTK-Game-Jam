using System;
using UnityEngine;

namespace Player
{
    public class PlayerMovement: MonoBehaviour
    {
        [SerializeField] private float _moveSpeed;
        private Rigidbody2D _rb;
        private Vector2 _movement;
        private Vector3 _startPosition;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _startPosition = transform.position;
        }

        private void Update()
        {
            _movement.x = Input.GetAxisRaw("Horizontal");
            _movement.y = Input.GetAxisRaw("Vertical");
            _movement.Normalize();
        }

        private void FixedUpdate()
        {
            Vector2 newPosition = _rb.position + _movement * _moveSpeed * Time.fixedDeltaTime;
            _rb.MovePosition(newPosition);
        }

        public void InitializePosition()
        {
            transform.position = _startPosition;
        }

        public bool IsMoving()
        {
            return _movement != Vector2.zero;
        }
    }
}