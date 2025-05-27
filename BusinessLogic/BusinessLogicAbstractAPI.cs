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

namespace TP.ConcurrentProgramming.BusinessLogic
{
    public abstract class BusinessLogicAbstractAPI : IDisposable
    {
        private static
        Lazy<BusinessLogicAbstractAPI> modelInstance = new Lazy<BusinessLogicAbstractAPI>(
          () => new BusinessLogicImplementation()
        );
        public static BusinessLogicAbstractAPI GetBusinessLogicLayer() { return modelInstance.Value; }

        public abstract void Start(int numberOfBalls, Action<Vector2, IBall> upperLayerHandler);
        public abstract void Dispose();
        public abstract void SpawnBall(Action<Vector2, IBall> upperLayerHandler);
        public abstract IBusinessDimensions GetDimensions();

    }

    public interface IBall
    {
        event EventHandler<Vector2> NewPositionNotification;
    }

    public interface IBusinessDimensions
    {
        double Width { get; init; }
        double Height { get; init; }
        double Diameter { get; init; }
    }
}
