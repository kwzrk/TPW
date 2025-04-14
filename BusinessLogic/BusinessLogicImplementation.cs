//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System;
using System.Diagnostics;
using System.Numerics;
using TP.ConcurrentProgramming.Data;
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;

namespace TP.ConcurrentProgramming.BusinessLogic {
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI {
        private bool Disposed = false;
        //private readonly object _collisionlock = new object();
        private readonly UnderneathLayerAPI layerBellow;
        public BusinessLogicImplementation() : this(null) { }
        internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer) {
            layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetDataLayer() : underneathLayer;
            // zasubkrybowanie kolizji
            layerBellow.PositionChanged += (sender,e) => HandleCollisions();
        }

        private void LayerBellow_PositionChanged(object? sender, EventArgs e) {
            throw new NotImplementedException();
        }

        public override void Dispose() {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            // odsubskrybowanie kolizji
            layerBellow.PositionChanged -= (sender, e) => HandleCollisions();
            layerBellow.Dispose();
            Disposed = true;
        }
        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler) {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));
            layerBellow.Start(numberOfBalls, (startingPosition, databall) => upperLayerHandler(new Position(startingPosition.x, startingPosition.y), new Ball(databall)));
        }

        public override void HandleCollisions() {
            List<Data.IBall> balls = layerBellow.GetBalls().ToList();
            foreach (Data.IBall ball in balls) {

                if (ball.Position.x < 0 && ball.Velocity.x < 0 || ball.Position.x + GetDimensions.BallDimension > GetDimensions.TableWidth && ball.Velocity.x > 0) {
                    ball.Velocity = layerBellow.CreateVector(-ball.Velocity.x, ball.Velocity.y);
                }
                if (ball.Position.y < 0 && ball.Velocity.y < 0 || ball.Position.y + GetDimensions.BallDimension > GetDimensions.TableWidth && ball.Velocity.y > 0) {
                    ball.Velocity = layerBellow.CreateVector(ball.Velocity.x, -ball.Velocity.y);
                }

                foreach (Data.IBall otherBall in balls) {
                    if (ball != otherBall) {
                        double futureDistance = Math.Sqrt(Math.Pow((ball.Position.x + ball.Velocity.x) - (otherBall.Position.x + otherBall.Velocity.x), 2) +
                                 Math.Pow((ball.Position.y + ball.Velocity.y) - (otherBall.Position.y + otherBall.Velocity.y), 2));
                        if (futureDistance <= GetDimensions.BallDimension) {
                            Data.IVector temp = ball.Velocity;
                            ball.Velocity = otherBall.Velocity;
                            otherBall.Velocity = temp;
                        }
                    }
                }
            }
        }

        //public override IBall CreateBall() {
        //    if (Disposed)
        //        throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
        //    Random random = new Random();
            
        //    IVector startingPosition = layerBellow.CreateVector(random.Next(100, 400 - 100), random.Next(100, 400 - 100));
        //    IVector velocity = layerBellow.CreateVector((random.NextDouble() - 0.5) * 1, (random.NextDouble() - 0.5) * 1);
            
        //    layerBellow.AddBall(startingPosition,velocity);
            
        //    Data.IBall dataBall = layerBellow.GetBalls().Last();
            
        //    return new Ball(dataBall);
        //}

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed) {
            returnInstanceDisposed(Disposed);
        }
    }
}
