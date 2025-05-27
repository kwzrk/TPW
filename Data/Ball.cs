//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Numerics;

namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall, IDisposable
    {
        private readonly object _lock = new object();
        private Vector2 _position;
        private Vector2 _velocity;
        private readonly int _delay = 16;
        private CancellationTokenSource? _moveCancellation;
        private Task? _moveTask;

        public event EventHandler<Vector2>? NewPositionNotification;
        public event EventHandler<Vector2>? MovingEndedNotification;

        public Vector2 Velocity
        {
            get { lock (_lock) return _velocity; }
            set { lock (_lock) _velocity = (Vector2)value; }
        }

        public Vector2 Position { get { lock (_lock) return _position; } }

        private readonly double _radius;
        public double Radius => _radius;

        internal Ball(Vector2 initialPosition, Vector2 initialVelocity, int delay = 16)
        {
            _position = initialPosition;
            _velocity = initialVelocity;
            _radius = 20;
            _delay = delay;
        }

        public async Task StartMovement()
        {
            await Task.Run(() =>
             {
                 StopMovement();
                 _moveCancellation = new CancellationTokenSource();
                 _moveTask = MoveContinuouslyAsync(_moveCancellation.Token);
             });
        }

        public void StopMovement()
        {
            _moveCancellation?.Cancel();
            _moveTask = null;
        }

        private async Task MoveContinuouslyAsync(CancellationToken ct)
        {

            while (!ct.IsCancellationRequested)
            {
                int actualDelay = (int)(_delay / _velocity.Length());
                actualDelay = Math.Max(actualDelay, 1);

                lock (_lock) _position = Vector2.Add(_position, _velocity);
                NewPositionNotification?.Invoke(this, _position);

                await Task.Delay(actualDelay, ct);
            }

            MovingEndedNotification?.Invoke(this, _position);
        }

        public void Dispose() => StopMovement();

        internal async Task Move()
        {
            await Task.Run(() =>
             {
                 _position = Vector2.Add(_position, _velocity);
                 NewPositionNotification?.Invoke(this, _position);
             }
            );
        }
    }
}
