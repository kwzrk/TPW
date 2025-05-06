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
  internal class Ball : IBall
  {
    private readonly object _lock = new object();
    private Vector _position;
    private Vector _velocity;

    public event EventHandler<IVector>? NewPositionNotification;
    public IVector Velocity
    {
      get { lock (_lock) return _velocity; }
      set { lock (_lock) _velocity = (Vector)value; }
    }

    public IVector Position { get { lock (_lock) return _position; } }

    public double Radius => _radius;
    private readonly double _radius;

    internal Ball(Vector initialPosition, Vector initialVelocity, double radius)
    {
      _position = initialPosition;
      _velocity = initialVelocity;
      _radius = radius;
    }

    private Task RaisePositionChangeNotification()
    {
      if (NewPositionNotification == null)
        return Task.CompletedTask;

      return Task.Run(() =>
      {
        NewPositionNotification?.Invoke(this, _position);
      });
    }

    internal async void Move()
    {
      _position = _position.add(_velocity);
      await RaisePositionChangeNotification();
    }
  }
}
