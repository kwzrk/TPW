//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Diagnostics;
using System.Numerics;

namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall, IDisposable
    {
        private readonly object _velocity_lock = new object();
        private readonly object _position_lock = new object();
        private Vector2 _position;
        private Vector2 _velocity;
        private readonly Vector2 Dimension;
        private readonly int _delay = 16;
        private CancellationTokenSource? _moveCancellation;
        private Task? _moveTask;

        private static int _nextBallId = 0;
        private readonly int _ballId;
        private readonly ILogger _logger;

        public event EventHandler<Vector2>? NewPositionNotification;
        public event EventHandler<Vector2>? MovingEndedNotification;

        public Vector2 Velocity
        {
            get { lock (_velocity_lock) return _velocity; }
            set { lock (_velocity_lock) _velocity = (Vector2)value; }
        }

        public Vector2 Position { get { lock (_position_lock) return _position; } }

        private readonly double _radius;
        public double Radius => _radius;

        internal Ball(Vector2 initialPosition, Vector2 initialVelocity, Vector2 Dimension, int delay = 16)
        {
            _ballId = Interlocked.Increment(ref _nextBallId);

            _position = initialPosition;
            _velocity = initialVelocity;
            _radius = 20;
            this.Dimension = Dimension;
            _delay = delay;
        }


        internal Ball(Vector2 initialPosition, Vector2 initialVelocity, Vector2 Dimension, ILogger logger, int delay = 16)
        {
            _ballId = Interlocked.Increment(ref _nextBallId);

            _position = initialPosition;
            _velocity = initialVelocity;
            _radius = 20;
            this.Dimension = Dimension;
            _delay = delay;

            _logger = logger;
            _logger.Log($"Ball {_ballId} created at Pos: ({_position.X:F2},{_position.Y:F2}), Vel: ({_velocity.X:F2},{_velocity.Y:F2})");
        }

        public async Task StartMovement()
        {
            await Task.Run(() =>
             {
                 StopMovement();
                 _moveCancellation = new CancellationTokenSource();
                 _moveTask = MoveContinuouslyAsync(_moveCancellation.Token);
                 _logger.Log($"Ball {_ballId} movement started.");
             });
        }

        public void StopMovement()
        {
            _moveCancellation?.Cancel();
            _logger.Log($"Ball {_ballId} movement stopped.");
            _moveTask = null;
        }

        private async Task MoveContinuouslyAsync(CancellationToken ct)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            long lastUpdate = stopwatch.ElapsedMilliseconds;

            while (!ct.IsCancellationRequested)
            {
                long now = stopwatch.ElapsedMilliseconds;
                long deltaTime = (now - lastUpdate);
                lastUpdate = now;

                lock (_position_lock) _position = Vector2.Add(_position, Vector2.Normalize(_velocity) * deltaTime / 60);
                NewPositionNotification?.Invoke(this, _position);
                await Task.Delay(16, ct);
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
                  _logger.LogBallState(_ballId, _position, _velocity);
              }
            );
        }
    }
}
