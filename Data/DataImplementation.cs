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

namespace TP.ConcurrentProgramming.Data {
    internal class DataImplementation : DataAbstractAPI {
        //private bool disposedValue;
        private bool Disposed = false;
        private readonly Timer MoveTimer;
        private Random RandomGenerator = new();
        private List<Ball> BallsList = [];
        private readonly object _lock = new object();

        public DataImplementation() {
            MoveTimer = new Timer(
              Move,
              null,
              TimeSpan.Zero,
              TimeSpan.FromMilliseconds(20)
            );
        }

        public override void Start(
          int numberOfBalls,
          Action<IVector, IBall> upperLayerHandler
        ) {
            ObjectDisposedException.ThrowIf(Disposed, nameof(DataImplementation));
            ArgumentNullException.ThrowIfNull(upperLayerHandler);

            foreach (var _ in Enumerable.Range(0, numberOfBalls)) {
                Vector startingPosition = new(
                  RandomGenerator.Next(100, 400 - 100),
                  RandomGenerator.Next(100, 400 - 100)
                );

                Vector initialVelocity = new(
                  RandomGenerator.NextDouble() - 0.5 * 5,
                  RandomGenerator.NextDouble() - 0.5 * 5
                );

                double radius = 20;

                Ball newBall = new(startingPosition, initialVelocity, radius);
                upperLayerHandler(startingPosition, newBall);
                BallsList.Add(newBall);
            }
            ;
        }

        public override Dimensions GetDimensions() {
            return DataAbstractAPI.Dimensions;
        }

        public override void SpawnBall(Action<IVector, IBall> upperLayerHandler) {
            ObjectDisposedException.ThrowIf(Disposed, nameof(DataImplementation));
            ArgumentNullException.ThrowIfNull(upperLayerHandler);

            Vector startingPosition = new(
              RandomGenerator.Next(100, 400 - 100),
              RandomGenerator.Next(100, 400 - 100)
            );

            double radius = RandomGenerator.Next(5, 30);

            Ball newBall = new(startingPosition, startingPosition, radius);
            upperLayerHandler(startingPosition, newBall);
            BallsList.Add(newBall);
        }

        protected virtual void Dispose(bool disposing) {
            ObjectDisposedException.ThrowIf(Disposed, nameof(DataImplementation));

            if (disposing) {
                MoveTimer.Dispose();
                BallsList.Clear();
            }

            Disposed = true;
        }

        public override IVector CreateVector(double x, double y) {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            return new Vector(x, y);
        }


        public override void Dispose() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Move(object? x) {
            lock (_lock) {
                BallsList.ForEach(x => x.Move());
            }
        }


        public override List<IBall> GetBalls() {
            return BallsList.Select(n => (IBall)n).ToList();
        }


        [Conditional("DEBUG")]
        internal void CheckBallsList(Action<IEnumerable<IBall>> returnBallsList) {
            returnBallsList(BallsList);
        }

        [Conditional("DEBUG")]
        internal void CheckNumberOfBalls(Action<int> returnNumberOfBalls) {
            returnNumberOfBalls(BallsList.Count);
        }

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed) {
            returnInstanceDisposed(Disposed);
        }
    }

    public class CollisionEventArgs : EventArgs {
        public CollisionEventArgs(IBall b1, IBall? b2) {

        }

        public IBall Ball1 { get; }
        public IBall Ball2 { get; }
    }
}
