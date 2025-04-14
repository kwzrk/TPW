//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic {
    internal class Ball : IBall {
        
        //private readonly Data.IBall dataBall;
        
        public event EventHandler<IPosition>? NewPositionNotification;
        public Ball(Data.IBall ball) {
            //this.dataBall = ball;
            ball.NewPositionNotification += RaisePositionChangeEvent;
        }

        //public IVector Position {
        //    get => dataBall.Position;
        //    set => dataBall.Position = value;
        //}

        private void RaisePositionChangeEvent(object? sender, Data.IVector e) {
            NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
        }

    }
}
