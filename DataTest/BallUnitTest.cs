//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data.Test
{
  [TestClass]
  public class BallUnitTest
  {
    [TestMethod]
    public void ConstructorTestMethod()
    {
      Vector testinVector = new Vector(0.0, 0.0);
      Ball newInstance = new(testinVector, testinVector, 20);
    }

    [TestMethod]
    public async void MoveTestMethod()
    {
      Vector initialPosition = new(10.0, 10.0);
      Ball newInstance = new(initialPosition, new Vector(0.0, 0.0), 20);
      IVector curentPosition = new Vector(0.0, 0.0);
      int numberOfCallBackCalled = 0;
      newInstance.NewPositionNotification += (sender, position) =>
        {
          Assert.IsNotNull(sender); curentPosition = position; numberOfCallBackCalled++;
        };

      await newInstance.Move();
      Assert.AreEqual<int>(1, numberOfCallBackCalled);
      Assert.AreEqual<IVector>(initialPosition, curentPosition);
    }

    [TestMethod]
    public void BallVelocityAsync()
    {
      var ball = new Ball(new Vector(0, 0), new Vector(1, 1), 5);
      var tasks = new List<Task>();

      for (int i = 0; i < 1000; i++)
      {
        int j = i;
        tasks.Add(Task.Run(() =>
        {
          ball.Velocity = new Vector(j, j);
        }));
      }

      Task.WaitAll(tasks.ToArray());

      var velocity = ball.Velocity;
      Assert.IsTrue(velocity != null);
    }
  }
}
