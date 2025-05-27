//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Numerics;

namespace TP.ConcurrentProgramming.Data
{
    public abstract class DataAbstractAPI : IDisposable
    {
        private static Lazy<DataAbstractAPI> modelInstance = new Lazy<DataAbstractAPI>(() => new DataImplementation());

        public static DataAbstractAPI GetDataLayer()
        {
            return modelInstance.Value;
        }

        public abstract void Dispose();
        public abstract void Start(int numberOfBalls, Action<Vector2, IBall> upperLayerHandler);
        public abstract Vector2 CreateVector(float x, float y);
        public abstract Vector2 GetDimensions();
        public abstract List<IBall> GetBalls();
        public abstract void BeginMovement();
    }

    public interface IBall
    {
        event EventHandler<Vector2> NewPositionNotification;
        Vector2 Velocity { get; set; }
        Vector2 Position { get; }
        double Radius { get; }
        public Task StartMovement();
        public void StopMovement();
    }
}
