//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//_____________________________________________________________________________________________________________________________________

using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
///
using TP.ConcurrentProgramming.BusinessLogic;
using TP.ConcurrentProgramming.Data;

/// 
using UnderneathLayerAPI = TP.ConcurrentProgramming.BusinessLogic.BusinessLogicAbstractAPI;

namespace TP.ConcurrentProgramming.Presentation.Model {
    /// <summary>
    /// Class Model - implements the <see cref="ModelAbstractApi" />
    /// </summary>
    internal class ModelImplementation : ModelAbstractApi {
        public event EventHandler<BallChaneEventArgs> BallChanged;
        private bool Disposed = false;
        private readonly IObservable<EventPattern<BallChaneEventArgs>> eventObservable = null;
        private readonly UnderneathLayerAPI layerBellow = null;

        internal ModelImplementation() : this(null) { }

        internal ModelImplementation(UnderneathLayerAPI underneathLayer) {
            layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetBusinessLogicLayer() : underneathLayer;
            eventObservable = Observable.FromEventPattern<BallChaneEventArgs>(this, "BallChanged");
        }

        public override void Dispose() {
            if (Disposed)
                throw new ObjectDisposedException(nameof(Model));
            layerBellow.Dispose();
            Disposed = true;
        }

        public override double getHeight() {
            return (double)UnderneathLayerAPI.GetDimensions.TableHeight;
        }
        public override double getWidth() {
            return (double)UnderneathLayerAPI.GetDimensions.TableWidth;
        }

        public override double getDiameter() {
            return (double)UnderneathLayerAPI.GetDimensions.BallDimension;
        }

        public override IDisposable Subscribe(IObserver<IBall> observer) {
            return eventObservable.Subscribe(x => observer.OnNext(x.EventArgs.Ball), ex => observer.OnError(ex), () => observer.OnCompleted());
        }

        public override void Start(int numberOfBalls) {
            layerBellow.Start(numberOfBalls, StartHandler);
        }

        private void StartHandler(BusinessLogic.IPosition position, BusinessLogic.IBall ball) {
            ModelBall newBall = new ModelBall(position.x, position.y, ball) { Diameter = getDiameter() };
            BallChanged.Invoke(this, new BallChaneEventArgs() { Ball = newBall });
            /// Notify the observers about the new ball
            //ball.NewPositionNotification += (sender, e) => {
            //    BallChaneEventArgs args = new BallChaneEventArgs() { Ball = newBall };
            //    BallChanged?.Invoke(this, args);
            //};
        }

        /// <summary>
        /// Creates a new ball and notifies the observers about the change.
        /// </summary>
        //public override void AddNewBall() {
        //    BusinessLogic.IBall createdBall = layerBellow.CreateBall();
        //    ModelBall newBall = new ModelBall(createdBall.Position.x, createdBall.Position.y, createdBall) {
        //        Diameter = getDiameter()
        //    };
        //    BallChanged?.Invoke(this, new BallChaneEventArgs { Ball = newBall });
        //}

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed) {
            returnInstanceDisposed(Disposed);
        }

        [Conditional("DEBUG")]
        internal void CheckUnderneathLayerAPI(Action<UnderneathLayerAPI> returnNumberOfBalls) {
            returnNumberOfBalls(layerBellow);
        }

        [Conditional("DEBUG")]
        internal void CheckBallChangedEvent(Action<bool> returnBallChangedIsNull) {
            returnBallChangedIsNull(BallChanged == null);
        }

    }

    public class BallChaneEventArgs : EventArgs {
        public IBall Ball { get; init; }
    }
}
