//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data {
    internal class Ball : IBall {

        private readonly object _lock = new object();
        private Vector _position;
        private Vector _velocity;
        public IVector Velocity {
            get {
                lock (_lock) return _velocity;
            }
            set {
                lock (_lock) _velocity = (Vector)value;
            }
        }

        internal Ball(Vector initialPosition, Vector initialVelocity) {
            _position = initialPosition;
            _velocity = initialVelocity;
        }

        public event EventHandler<IVector>? NewPositionNotification;

        private void RaiseNewPositionChangeNotification() {
            NewPositionNotification?.Invoke(this, _position);
        }

        internal void MoveTowards(Vector delta) {
            _position = _position.add(delta);
            RaiseNewPositionChangeNotification();
        }
    }
}
