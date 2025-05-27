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

namespace TP.ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            Vector2 Dimensions = new(100, 100);
            Vector2 testinVector2 = new Vector2(0.0f, 0.0f);
            Ball newInstance = new(testinVector2, testinVector2, Dimensions);
        }

        [TestMethod]
        public async Task MoveTestMethod()
        {
            Vector2 Dimensions = new(100, 100);
            Vector2 initialPosition = new(10.0f, 10.0f);
            Ball newInstance = new(initialPosition, new Vector2(0.0f, 0.0f), Dimensions, 20);
            Vector2 curentPosition = new Vector2(0.0f, 0.0f);
            int numberOfCallBackCalled = 0;
            newInstance.NewPositionNotification += (sender, position) =>
              {
                  Assert.IsNotNull(sender); curentPosition = position; numberOfCallBackCalled++;
              };

            await newInstance.Move();
            Assert.AreEqual<int>(1, numberOfCallBackCalled);
            Assert.AreEqual<Vector2>(initialPosition, curentPosition);
        }

        [TestMethod]
        public void BallVelocityAsync()
        {
            Vector2 Dimensions = new(100, 100);
            var ball = new Ball(new Vector2(0, 0), new Vector2(1, 1), Dimensions, 5);
            var tasks = new List<Task>();

            for (int i = 0; i < 10.0f; i++)
            {
                int j = i;
                tasks.Add(Task.Run(() =>
                {
                    ball.Velocity = new Vector2(j, j);
                }));
            }

            Task.WaitAll(tasks.ToArray());

            var velocity = ball.Velocity;
            Assert.IsTrue(velocity != null);
        }
    }
}
