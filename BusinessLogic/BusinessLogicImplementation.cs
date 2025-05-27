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
using TP.ConcurrentProgramming.Data;
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
        private bool Disposed = false;
        private Object _lock = new();
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
          Action<Vector2, IBall> upperLayerHandler
        )
        {
            ObjectDisposedException.ThrowIf(Disposed, nameof(BusinessLogicImplementation));
            ArgumentNullException.ThrowIfNull(upperLayerHandler);

            layerBellow.Start(
              numberOfBalls,
              (startingPosition, databall) =>
              {
                  databall.NewPositionNotification += (s, pos) =>
                  {
                      CheckBallCollision(s, pos);
                      CheckWallCollision(s, pos);
                  };
                  var ball = new Ball(databall);
                  upperLayerHandler(new Vector2(startingPosition.X, startingPosition.Y), new Ball(databall));
              }
            );

            layerBellow.BeginMovement();
        }

        private void CheckWallCollision(object? s, Vector2 pos)
        {
            if (s == null) return;
            Data.IBall src = (Data.IBall)s;
            Vector2 dim = layerBellow.GetDimensions();

            if (
                (src.Position.X <= 0 && src.Velocity.X <= 0) ||
                (src.Position.X + src.Radius + 10 >= dim.X && src.Velocity.X >= 0))
            {
                InvokeWallCollision(src, isHorizontal: true);
                return;
            }
            if (
                (src.Position.Y <= 0 && src.Velocity.Y <= 0) ||
                (src.Position.Y + src.Radius + 10 >= dim.Y && src.Velocity.Y >= 0))
            {
                InvokeWallCollision(src, isHorizontal: false);
                return;
            }
        }

        private void CheckBallCollision(object? s, Vector2 pos)
        {
            if (s == null) return;
            Data.IBall src = (Data.IBall)s;
            Vector2 dim = layerBellow.GetDimensions();

            lock (_lock)
            {
                foreach (Data.IBall otherBall in layerBellow.GetBalls())
                {
                    if (src.Equals(otherBall)) continue;
                    if (IsColliding(src, otherBall)) InvokeBallCollision(src, otherBall);
                }
            }
        }

        private bool IsColliding(Data.IBall first, Data.IBall second)
        {
            double dist = Math.Sqrt(Math.Pow(second.Position.X - first.Position.X, 2) +
                                   Math.Pow(second.Position.Y - first.Position.Y, 2));
            return dist < (first.Radius + second.Radius) / 2;
        }


        private void InvokeBallCollision(Data.IBall ball, Data.IBall otherBall)
        {
            double delta = Math.Sqrt(
              Math.Pow(
              (ball.Position.X + ball.Velocity.X) -
              (otherBall.Position.X + otherBall.Velocity.X), 2) +
              Math.Pow(
                (ball.Position.Y + ball.Velocity.Y) -
                (otherBall.Position.Y + otherBall.Velocity.Y), 2)
              );

            if (delta > (ball.Radius + otherBall.Radius) / 2) return;

            Vector2 temp = ball.Velocity;
            ball.Velocity = otherBall.Velocity;
            otherBall.Velocity = temp;
        }

        private void InvokeWallCollision(Data.IBall ball, bool isHorizontal)
        {
            if (isHorizontal)
            {
                ball.Velocity = layerBellow.CreateVector(
                  -ball.Velocity.X,
                  ball.Velocity.Y
                );
            }
            else
            {
                ball.Velocity = layerBellow.CreateVector(ball.Velocity.X, -ball.Velocity.Y);
            }
        }

        public override Vector2 GetDimensions()
        {
            ObjectDisposedException.ThrowIf(Disposed, nameof(BusinessLogicImplementation));
            return layerBellow.GetDimensions();
        }

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }
    }
}
