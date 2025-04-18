﻿//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Diagnostics;
using TP.ConcurrentProgramming.Data;
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;

namespace TP.ConcurrentProgramming.BusinessLogic
{
  internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
  {
    private bool Disposed = false;
    private readonly UnderneathLayerAPI layerBellow;
    public BusinessLogicImplementation() : this(null) { }
    internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer)
    {
      layerBellow = underneathLayer ?? UnderneathLayerAPI.GetDataLayer();
    }
    public override void Dispose()
    {
      ObjectDisposedException.ThrowIf(Disposed, nameof(BusinessLogicImplementation));
      layerBellow.Dispose();
      Disposed = true;
    }
    public override void Start(
      int numberOfBalls,
      Action<IPosition, IBall> upperLayerHandler
    )
    {
      ObjectDisposedException.ThrowIf(Disposed, nameof(BusinessLogicImplementation));
      ArgumentNullException.ThrowIfNull(upperLayerHandler);

      layerBellow.Start(
        numberOfBalls,
        (startingPosition, databall) =>
        {
          databall.NewPositionNotification += CheckCollision;
          var ball = new Ball(databall);
          upperLayerHandler(new Position(startingPosition.x, startingPosition.y), new Ball(databall));
        }
      );
    }

    private void CheckCollision(object? s, IVector pos)
    {
      if (s == null) return;
      Data.IBall src = (Data.IBall)s;
      Dimensions dim = layerBellow.GetDimensions();

      if (
          (pos.x - src.Radius < 0 && src.Velocity.x < 0) ||
          (pos.x + src.Radius > dim.TableWidth && src.Velocity.x > 0))
      {
        InvokeWallCollision(src, isHorizontal: true);
        return;
      }
      if (
        (pos.y - src.Radius < 0 &&
          src.Velocity.y < 0) ||
        (pos.y + src.Radius > dim.TableHeight &&
          src.Velocity.y > 0))
      {
        InvokeWallCollision(src, isHorizontal: false);
        return;
      }

      foreach (Data.IBall otherBall in layerBellow.GetBalls())
      {
        if (src.Equals(otherBall)) continue;
        if (src.IsColliding(otherBall)) InvokeBallCollision(src, otherBall);
      }
    }

    public override void SpawnBall(Action<IPosition, IBall> upperLayerHandler)
    {
      layerBellow.SpawnBall(
        (position, ball) => upperLayerHandler(
          new Position(position.x, position.y),
          new Ball(ball)
        )
      );
    }


    // public override void HandleCollisions()
    // {
    //   List<Data.IBall> balls = layerBellow.GetBalls().ToList();
    //   foreach (Data.IBall ball in balls)
    //   {
    //
    //     if (ball.Position.x < 0 && ball.Velocity.x < 0 || ball.Position.x + GetDimensions.BallDimension > GetDimensions.TableWidth && ball.Velocity.x > 0)
    //     {
    //       ball.Velocity = layerBellow.CreateVector(-ball.Velocity.x, ball.Velocity.y);
    //     }
    //     if (ball.Position.y < 0 && ball.Velocity.y < 0 || ball.Position.y + GetDimensions.BallDimension > GetDimensions.TableWidth && ball.Velocity.y > 0)
    //     {
    //       ball.Velocity = layerBellow.CreateVector(ball.Velocity.x, -ball.Velocity.y);
    //     }
    //
    //     foreach (Data.IBall otherBall in balls)
    //     {
    //       if (ball != otherBall)
    //       {
    //         double futureDistance = Math.Sqrt(Math.Pow((ball.Position.x + ball.Velocity.x) - (otherBall.Position.x + otherBall.Velocity.x), 2) +
    //                  Math.Pow((ball.Position.y + ball.Velocity.y) - (otherBall.Position.y + otherBall.Velocity.y), 2));
    //         if (futureDistance <= GetDimensions.BallDimension)
    //         {
    //           Data.IVector temp = ball.Velocity;
    //           ball.Velocity = otherBall.Velocity;
    //           otherBall.Velocity = temp;
    //         }
    //       }
    //     }
    //   }
    // }

    private void InvokeBallCollision(Data.IBall ball, Data.IBall otherBall)
    {
      double futureDistance = Math.Sqrt(Math.Pow(
        (ball.Position.x + ball.Velocity.x) - (otherBall.Position.x + otherBall.Velocity.x), 2) +
               Math.Pow((ball.Position.y + ball.Velocity.y) - (otherBall.Position.y + otherBall.Velocity.y), 2));
      if (futureDistance <= ball.Radius)
      {
        Data.IVector temp = ball.Velocity;
        ball.Velocity = otherBall.Velocity;
        otherBall.Velocity = temp;
      }
    }

    private void InvokeWallCollision(Data.IBall ball, bool isHorizontal)
    {
      if (isHorizontal)
      {
        ball.Velocity = layerBellow.CreateVector(-ball.Velocity.x, ball.Velocity.y);
      }
      else
      {
        ball.Velocity = layerBellow.CreateVector(ball.Velocity.x, -ball.Velocity.y);
      }
    }


    public override IEnumerable<IBall> GetBallsList()
    {
      throw new NotImplementedException();
    }

    public override void MoveBalls()
    {
      throw new NotImplementedException();
    }


    [Conditional("DEBUG")]
    internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
    {
      returnInstanceDisposed(Disposed);
    }
  }
}
