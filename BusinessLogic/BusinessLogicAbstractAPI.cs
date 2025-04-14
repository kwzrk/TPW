﻿//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.BusinessLogic {
    public abstract class BusinessLogicAbstractAPI : IDisposable
    {
      private static Lazy<BusinessLogicAbstractAPI> modelInstance =
                       new Lazy<BusinessLogicAbstractAPI>(
                         () => new BusinessLogicImplementation()
                       );

      public static
        BusinessLogicAbstractAPI GetBusinessLogicLayer()
        {
          return modelInstance.Value;
        }

      public static readonly
        Dimensions? GetDimensions;

      public abstract void Start
      (
        int numberOfBalls,
        Action<IPosition, IBall> upperLayerHandler
      );

      public abstract void Dispose();

      public abstract void CreateBall
      (
        IPosition position,
        IPosition velocity
      );

      public abstract IEnumerable<IBall> GetBallsList();
      public abstract void MoveBalls();
    }
    /// <summary>
    /// Immutable type representing table dimensions
    /// </summary>
    /// <param name="BallDimension"></param>
    /// <param name="TableHeight"></param>
    /// <param name="TableWidth"></param>
    /// <remarks>
    /// Must be abstract
    /// </remarks>
    public record Dimensions(
      double BallDimension,
      double TableHeight,
      double TableWidth
    );

    public interface IPosition
    {
        double x { get; init; }
        double y { get; init; }
    }
    public interface IBall
    {
        event EventHandler<IPosition> NewPositionNotification;
    }
}
