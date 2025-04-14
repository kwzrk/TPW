//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using Newtonsoft.Json.Bson;

namespace TP.ConcurrentProgramming.Data {
    public abstract class DataAbstractAPI : IDisposable {

        private static Lazy<DataAbstractAPI> modelInstance = new Lazy<DataAbstractAPI>(() => new DataImplementation());
        public event EventHandler? PositionChanged;

        public static DataAbstractAPI GetDataLayer() {
            return modelInstance.Value;
        }
        
        // wykorzystywane podczas wyliczania kolizji
        protected void OnPositionChanged() {
            PositionChanged?.Invoke(this, EventArgs.Empty);
        }

        public abstract void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler);
        public abstract void Dispose();

        public abstract IEnumerable<IBall> GetBalls();
        public abstract IVector CreateVector(double x, double y);
        //public abstract void AddBall(IVector position, IVector velocity);
    }

    public interface IVector {
        
        /// The X component of the vector.
        double x { get; init; }

        /// The Y component of the vector.
        double y { get; init; }
    }

    public interface IBall {
        event EventHandler<IVector> NewPositionNotification;

        IVector Velocity { get; set; }
        IVector Position { get; set; }
    }
}
