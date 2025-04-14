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
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;

namespace TP.ConcurrentProgramming.BusinessLogic {
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI {
        private bool Disposed = false;
        private readonly UnderneathLayerAPI layerBellow;
        public BusinessLogicImplementation() : this(null) { }
        internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer) {
            layerBellow ??= UnderneathLayerAPI.GetDataLayer();
        }
        public override void Dispose() {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            layerBellow.Dispose();
            Disposed = true;
        }
        public override void Start(
          int numberOfBalls,
          Action<IPosition, IBall> upperLayerHandler
        ) {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));
            layerBellow.Start(
              numberOfBalls,
              (startingPosition, databall) => upperLayerHandler(
                new Position(startingPosition.x, startingPosition.x),
                new Ball(databall)
              )
            );
        }

        public override void CreateBall(IPosition position, IPosition velocity) {
          layerBellow.CreateBall(position.x, position.y, velocity.x, velocity.y);
        }

        public override IEnumerable<IBall> GetBallsList() {
            throw new NotImplementedException();
        }

        public override void MoveBalls() {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed) {
            returnInstanceDisposed(Disposed);
        }
    }
}
