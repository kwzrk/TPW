//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data
{
  public abstract class DataAbstractAPI : IDisposable
  {
    private static Lazy<DataAbstractAPI> modelInstance =
      new Lazy<DataAbstractAPI>(() => new DataImplementation());

    public static readonly Dimensions Dimensions = new(20.0, 420.0, 400.0);
    public abstract Dimensions GetDimensions();

    public static DataAbstractAPI GetDataLayer()
    {
      return modelInstance.Value;
    }

    public abstract void Dispose();
    public abstract void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler);
    public abstract void SpawnBall(Action<IVector, IBall> upperLayerHandler);
    public abstract IVector CreateVector(double x, double y);

    public abstract List<IBall> GetBalls();

    public event EventHandler? PositionChanged;

    protected void OnPositionChanged()
    {
      PositionChanged?.Invoke(this, EventArgs.Empty);
    }
  }

  public record Dimensions(
    double BallDimension,
    double TableHeight,
    double TableWidth
  );

  public interface IVector
  {
    /// The X component of the vector.
    double x { get; init; }

    /// The Y component of the vector.
    double y { get; init; }
  }

  public interface IBall
  {
    event EventHandler<IVector> NewPositionNotification;
    IVector Velocity { get; set; }
    IVector Position { get; set; }
    double Radius { get; }
    public bool IsColliding(IBall ball2);
  }
}
