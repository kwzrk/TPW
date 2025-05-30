﻿//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Numerics;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public void MoveTestMethod()
        {
            DataBallFixture dataBallFixture = new DataBallFixture();
            Ball newInstance = new(dataBallFixture);
            int numberOfCallBackCalled = 0;
            newInstance.NewPositionNotification += (sender, position) =>
            {
                Assert.IsNotNull(sender);
                Assert.IsNotNull(position);
                numberOfCallBackCalled++;
            };
            dataBallFixture.Move();
            Assert.AreEqual<int>(1, numberOfCallBackCalled);
        }


        private class DataBallFixture : Data.IBall
        {
            public Vector2 Velocity { get; set; } = new Vector2(0, 0);
            public Vector2 Position { get; set; } = new Vector2(0, 0);

            public double Radius { get; set; } = 10;

            public event EventHandler<Vector2>? NewPositionNotification;

            public Task Move()
            {
                NewPositionNotification?.Invoke(this, new Vector2(1, 1));
                return Task.CompletedTask;
            }

            public Task StartMovement()
            {
                return Task.CompletedTask;
            }

            public void StopMovement()
            {
            }

            public override string? ToString()
            {
                return base.ToString();
            }
        }
    }
}

