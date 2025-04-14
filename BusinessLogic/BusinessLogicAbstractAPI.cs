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
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic {
    public abstract class BusinessLogicAbstractAPI : IDisposable {

        private static Lazy<BusinessLogicAbstractAPI> modelInstance = new Lazy<BusinessLogicAbstractAPI>(() => new BusinessLogicImplementation());
        
        public static BusinessLogicAbstractAPI GetBusinessLogicLayer() {
            return modelInstance.Value;
        }

        public static readonly Dimensions GetDimensions = new(10.0, 420.0, 400.0);
        public abstract void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler);
        public abstract void Dispose();
        public abstract void HandleCollisions();
        public abstract IBall CreateBall();

    }
    public record Dimensions(double BallDimension, double TableHeight, double TableWidth);
    public interface IPosition {
        double x { get; init; }
        double y { get; init; }
    }
    public interface IBall {
        IVector Position { get; set; }
        event EventHandler<IPosition> NewPositionNotification;
    }
}
