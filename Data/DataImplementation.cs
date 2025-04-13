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

namespace TP.ConcurrentProgramming.Data {
    internal class DataImplementation : DataAbstractAPI {
        //private bool disposedValue;
        private bool Disposed = false;
        private readonly Timer MoveTimer;
        private Random RandomGenerator = new();
        private List<Ball> BallsList = [];

        public DataImplementation()
        {
            MoveTimer = new Timer(
              Move,
              null,
              TimeSpan.Zero,
              TimeSpan.FromMilliseconds(100)
            );
        }

        public override void Start(
          int numberOfBalls,
          Action<IVector, IBall> upperLayerHandler
        ) {
            if (Disposed)
              throw new ObjectDisposedException(nameof(DataImplementation));
            if (upperLayerHandler == null)
              throw new ArgumentNullException(nameof(upperLayerHandler));

            Random random = new Random();

            foreach (var _ in Enumerable.Range(0, numberOfBalls))
            {
                Vector startingPosition = new(
                  random.Next(100, 400 - 100),
                  random.Next(100, 400 - 100)
                );
                Ball newBall = new(startingPosition, startingPosition);
                upperLayerHandler(startingPosition, newBall);
                BallsList.Add(newBall);
            };
        }

        public override void AddBall(IVector lol, IVector xd) {

        }

        protected virtual void Dispose(bool disposing) {
            if (!Disposed) {
                if (disposing) {
                    MoveTimer.Dispose();
                    BallsList.Clear();
                }
                Disposed = true;
            } else
                throw new ObjectDisposedException(nameof(DataImplementation));
        }

        public override void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Move(object? x)
        {
            foreach (Ball item in BallsList)
            {
              item.MoveTowards(new Vector(
                    (RandomGenerator.NextDouble() - 0.5) * 10,
                    (RandomGenerator.NextDouble() - 0.5) * 10
              ));
            }
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
}
