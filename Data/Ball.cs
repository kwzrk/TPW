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
      get => _velocity;
      set
      {
        lock (_lock) _velocity = (Vector)value;
      }
    }

    public IVector Position
    {
      get => _position;
    }

    public double Radius => _radius;
    private readonly double _radius;

    // public bool IsColliding(IBall withOther)
    // {
    //   double dist = Math.Sqrt(Math.Pow(withOther.Position.x - Position.x, 2) +
    //                          Math.Pow(withOther.Position.y - Position.y, 2));
    //   if (dist <= Radius + withOther.Radius) return true;
    //   return false;
    // }

    internal Ball(Vector initialPosition, Vector initialVelocity, double radius)
    {
      _position = initialPosition;
      _velocity = initialVelocity;
      _radius = radius;
    }

    private void RaisePositionChangeNotification()
    {
      NewPositionNotification?.Invoke(this, _position);
    }

    internal void Move()
    {
      _position = _position.add(_velocity);
      RaisePositionChangeNotification();
    }
  }
}
