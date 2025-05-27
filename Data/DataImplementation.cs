using System.Diagnostics;
using System.Numerics;

namespace TP.ConcurrentProgramming.Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        public static readonly Vector2 Dimensions = new Vector2(400, 400);
        private readonly object _lock = new object();
        private bool Disposed = false;
        private Random RandomGenerator = new();
        private List<Ball> BallsList = [];
        private readonly ILogger _logger;

        public DataImplementation()
        {
            _logger = new Logger(); // Instantiate the logger
            _logger.Log("Data Layer: Initialized.");
        }

        public override void Start(
          int numberOfBalls,
          Action<Vector2, IBall> upperLayerHandler
        )
        {
            ObjectDisposedException.ThrowIf(Disposed, nameof(DataImplementation));
            ArgumentNullException.ThrowIfNull(upperLayerHandler);
            _logger.Log($"Data Layer: Starting with {numberOfBalls} balls.");
            foreach (var _ in Enumerable.Range(0, numberOfBalls))
            {
                Vector2 startingPosition = new(
                    RandomGenerator.Next(50, (int)(Dimensions.X) - 50),
                    RandomGenerator.Next(50, (int)(Dimensions.Y) - 50)
                );

                Vector2 initialVelocity = new(
                  (float)(RandomGenerator.NextDouble() - 0.5),
                  (float)(RandomGenerator.NextDouble() - 0.5)
                );

                Ball newBall = new(startingPosition, initialVelocity, Dimensions, _logger);
                upperLayerHandler(startingPosition, newBall);
                BallsList.Add(newBall);
                _logger.Log($"Data Layer: Created ball at {startingPosition} with velocity {initialVelocity}");
            }
        }
        public override Vector2 GetDimensions()
        {
            return Dimensions;
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
