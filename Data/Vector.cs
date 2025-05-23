﻿//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//  by introducing yourself and telling us what you do with this community.
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data
{
  /// <summary>
  ///  Two dimensions immutable vector
  /// </summary>
  internal record Vector : IVector
  {

    /// <summary>
    /// The X component of the vector.
    /// </summary>
    public double x { get; init; }
    /// <summary>
    /// The Y component of the vector.
    /// </summary>
    public double y { get; init; }


    /// <summary>
    /// Creates new instance of <seealso cref="Vector"/> and initialize all properties
    /// </summary>
    public Vector(double XComponent, double YComponent)
    {
      x = XComponent;
      y = YComponent;
    }

    public Vector add(IVector delta)
    {
      return new Vector(this.x + delta.x, this.y + delta.y);
    }

    public IVector Normalize()
    {
      double length = Math.Sqrt(x * x + y * y);
      if (length == 0.0) Vector.zero();
      return new Vector(x / length, y / length);
    }

    public float Length()
    {
      return (float)Math.Sqrt(x * x + y * y);
    }

    public float Dot(IVector vector)
    {
      return (float)(this.x * vector.x + this.y * vector.y);
    }

    static public Vector zero()
    {
      return new Vector(0, 0);
    }
  }
}
