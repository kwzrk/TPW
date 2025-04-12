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

        internal Ball(Vector initialPosition, Vector initialVelocity) {
            Position = initialPosition;
            Velocity = initialVelocity;
        }


        public event EventHandler<IVector>? NewPositionNotification;
        public IVector Velocity { get; set; }
        private Vector Position;

        private void RaiseNewPositionChangeNotification() {
            NewPositionNotification?.Invoke(this, Position);
        }

        internal void Move(Vector delta) {
            Position = new Vector(Position.x + delta.x, Position.y + delta.y);
            RaiseNewPositionChangeNotification();
        }

    }
}
