using System.Diagnostics;
using System.Numerics;

namespace TP.ConcurrentProgramming.Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        private readonly object _lock = new object();
        private bool Disposed = false;
        private Random RandomGenerator = new();
        private List<Ball> BallsList = [];

        public readonly Dimensions _dimensions = new Dimensions(20, 400, 420);

        public override void Start(
          int numberOfBalls,
          Action<Vector2, IBall> upperLayerHandler
        )
        {
            ObjectDisposedException.ThrowIf(Disposed, nameof(DataImplementation));
            ArgumentNullException.ThrowIfNull(upperLayerHandler);
            foreach (var _ in Enumerable.Range(0, numberOfBalls))
            {
                Vector2 startingPosition = new(
                  RandomGenerator.Next(10, _dimensions.Width - 50),
                  RandomGenerator.Next(10, _dimensions.Height - 50)
                );

                Vector2 initialVelocity = new(
                  (float)(RandomGenerator.NextDouble() - 0.5) * 10,
                  (float)(RandomGenerator.NextDouble() - 0.5) * 10
                );

                double radius = _dimensions.Radius;

                Ball newBall = new(startingPosition, initialVelocity);
                upperLayerHandler(startingPosition, newBall);
                BallsList.Add(newBall);
            }
        }

        public override void SpawnBall(Action<Vector2, IBall> upperLayerHandler)
        {
            ObjectDisposedException.ThrowIf(Disposed, nameof(DataImplementation));
            ArgumentNullException.ThrowIfNull(upperLayerHandler);

            Vector2 startingPosition = new(
              (float)RandomGenerator.Next(100, 400 - 100),
              (float)RandomGenerator.Next(100, 400 - 100)
            );

            Vector2 initialVelocity = new(
                (float)(RandomGenerator.NextDouble() - 0.5),
                (float)(RandomGenerator.NextDouble() - 0.5)
            );

            Ball newBall = new(startingPosition, initialVelocity);
            upperLayerHandler(startingPosition, newBall);
            BallsList.Add(newBall);
        }

        protected virtual void Dispose(bool disposing)
        {
            ObjectDisposedException.ThrowIf(Disposed, nameof(DataImplementation));

            if (disposing)
            {
                BallsList.Clear();
            }

            Disposed = true;
        }

        public override Vector2 CreateVector(float x, float y)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            return new Vector2(x, y);
        }


        public override void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public override void BeginMovement()
        {
            BallsList.ForEach(async item => await item.StartMovement());
        }

        public override List<IBall> GetBalls()
        {
            lock (_lock)
            {
                return BallsList.Select(ball => (IBall)ball).ToList();
            }
        }

        public override IDimensions GetDimensions() => _dimensions;

        [Conditional("DEBUG")]
        internal void CheckBallsList(Action<IEnumerable<IBall>> returnBallsList)
        {
            returnBallsList(BallsList);
        }

        [Conditional("DEBUG")]
        internal void CheckNumberOfBalls(Action<int> returnNumberOfBalls)
        {
            returnNumberOfBalls(BallsList.Count);
        }

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }
    }
}
