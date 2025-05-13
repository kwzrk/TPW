//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data
{
  internal class Ball : IBall, IDisposable
  {
    private readonly object _lock = new object();
    private Vector _position;
    private Vector _velocity;
    private readonly int _delay = 16;
    private CancellationTokenSource? _moveCancellation;
    private Task? _moveTask;

    public event EventHandler<IVector>? NewPositionNotification;
    public event EventHandler<IVector>? MovingEndedNotification;

    public IVector Velocity
    {
      get { lock (_lock) return _velocity; }
      set { lock (_lock) _velocity = (Vector)value; }
    }

    public IVector Position { get { lock (_lock) return _position; } }

    private readonly double _radius;
    public double Radius => _radius;

    internal Ball(Vector initialPosition, Vector initialVelocity, double radius, int delay = 16)
    {
      _position = initialPosition;
      _velocity = initialVelocity;
      _radius = radius;
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
        lock (_lock) { _position = _position.add(_velocity); }
        NewPositionNotification?.Invoke(this, _position);

        await Task.Delay(_delay, ct);
      }

      MovingEndedNotification?.Invoke(this, _position);
    }

    public void Dispose() => StopMovement();

    internal async Task Move()
    {
      await Task.Run(() =>
       {
         _position = _position.add(_velocity);
         NewPositionNotification?.Invoke(this, _position);
       }
      );
    }
  }
}
