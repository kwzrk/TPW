﻿//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.BusinessLogic
{
  internal record Position : IPosition
  {
    public double x { get; init; }
    public double y { get; init; }
    /// <summary>
    /// Creates new instance of <see also cref="IPosition"/> and initialize all properties
    /// </summary>
    public Position(double posX, double posY)
    {
      x = posX;
      y = posY;
    }
  }
}
