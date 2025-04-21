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

    public static DataAbstractAPI GetDataLayer()
    {
      return modelInstance.Value;
    }

    public abstract void Dispose();
    public abstract void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler);
    public abstract void SpawnBall(Action<IVector, IBall> upperLayerHandler);
    public abstract IVector CreateVector(double x, double y);
    public abstract IDimensions GetDimensions();

    public abstract List<IBall> GetBalls();
  }

  public interface IDimensions
  {
    double Radius { get; init; }

    double Height { get; init; }

    double Width { get; init; }
  }

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
    IVector Position { get; }
    double Radius { get; }
  }
}
