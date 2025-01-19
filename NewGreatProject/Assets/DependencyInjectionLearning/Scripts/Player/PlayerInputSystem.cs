using System;
using UnityEngine;
using Zenject;

namespace Player
{
    class PlayerInputSystem : IInitializable, IDisposable, IFixedTickable
    {
        internal event Action<Vector2> OnMovementInputFixedUpdate;
        internal event Action OnMovementInputStart;

        bool _isMoving;
        Vector2 _lastMoveDir;
        MainController _controller;

        public void Dispose() => _controller.Dispose();

        public void Initialize()
        {
            _controller = new();
            _controller.Enable();
            _controller.Main.Movement.performed += ctx =>
            {
                _isMoving = true;
                _lastMoveDir = ctx.ReadValue<Vector2>();
                OnMovementInputStart?.Invoke();
            };
            _controller.Main.Movement.canceled += ctx =>
            {
                _isMoving = false;
                _lastMoveDir = ctx.ReadValue<Vector2>();
            };
        }

        public void FixedTick()
        {
            if (_isMoving)
                OnMovementInputFixedUpdate?.Invoke(_lastMoveDir);
        }
    }
}
