//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.ComponentModel;

namespace TP.ConcurrentProgramming.Data {
    internal class Ball : IBall {

        public event EventHandler<IVector>? NewPositionNotification;
        
        public IVector Velocity {
            get;
            set;
        }
        public IVector Position {
            get;
            set;
        }

        //public event PropertyChangedEventHandler? PropertyChanged;
        //private IVector _velocity;
        //public IVector Velocity {
        //    get => _velocity;
        //    set {
        //        _velocity = value;
        //        OnPropertyChanged(nameof(Velocity));
        //    }
        //}

        //private IVector _position;
        //public IVector Position {
        //    get => _position;
        //    set {
        //        _position = value;
        //        OnPropertyChanged(nameof(Position));
        //    }
        //}

        internal Ball(Vector initialPosition, Vector initialVelocity) {
            //_position = initialPosition;
            //_velocity = initialVelocity;
            Position = initialPosition;
            Velocity = initialVelocity;
            RaiseNewPositionChangeNotification();
        }

        private void RaiseNewPositionChangeNotification() {
            NewPositionNotification?.Invoke(this, Position);
        }
        //protected virtual void OnPropertyChanged(string propertyName) {
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        internal void Move(Vector delta) {
            Position = new Vector(Position.x + delta.x, Position.y + delta.y);
            RaiseNewPositionChangeNotification();
        }

        internal void Move() {
            Position = new Vector(Position.x + Velocity.x, Position.y + Velocity.y);
            RaiseNewPositionChangeNotification();
        }
    }
}
