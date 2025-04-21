using System.Diagnostics;

namespace TP.ConcurrentProgramming.Data
{
  internal class DataImplementation : DataAbstractAPI
  {
    //private bool disposedValue;
    private readonly object _lock = new object();
    private bool Disposed = false;
    private readonly Timer MoveTimer;
    private Random RandomGenerator = new();
    private List<Ball> BallsList = [];

    public readonly Dimensions _dimensions = new Dimensions(20, 600, 100);

    public DataImplementation()
    {
      MoveTimer = new Timer(
        Move,
        null,
        TimeSpan.Zero,
        TimeSpan.FromMilliseconds(50)
      );
    }

    public override void Start(
      int numberOfBalls,
      Action<IVector, IBall> upperLayerHandler
    )
    {
      ObjectDisposedException.ThrowIf(Disposed, nameof(DataImplementation));
      ArgumentNullException.ThrowIfNull(upperLayerHandler);
      foreach (var _ in Enumerable.Range(0, numberOfBalls))
      {
        Vector startingPosition = new(
          RandomGenerator.Next(10, 400),
          RandomGenerator.Next(10, 400)
        );

        Vector initialVelocity = new(
          RandomGenerator.NextDouble() - 0.5 * 10,
          RandomGenerator.NextDouble() - 0.5 * 10
        );

        double radius = 20;

        Ball newBall = new(startingPosition, initialVelocity, radius);
        upperLayerHandler(startingPosition, newBall);
        BallsList.Add(newBall);
      }
    }

    public override void SpawnBall(Action<IVector, IBall> upperLayerHandler)
    {
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

    protected virtual void Dispose(bool disposing)
    {
      ObjectDisposedException.ThrowIf(Disposed, nameof(DataImplementation));

      if (disposing)
      {
        MoveTimer.Dispose();
        BallsList.Clear();
      }

      Disposed = true;
    }

    public override IVector CreateVector(double x, double y)
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(DataImplementation));
      return new Vector(x, y);
    }


    public override void Dispose()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    private void Move(object? x)
    {
      //BallsList.ForEach(x => x.Move());
      foreach (var ball in BallsList)
      {
        ball.Move();
      }
    }

    public override List<IBall> GetBalls() => BallsList.Select(ball => (IBall)ball).ToList();
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
