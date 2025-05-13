﻿//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
  [TestClass]
  public class BusinessLogicImplementationUnitTest
  {
    [TestMethod]
    public void ConstructorTestMethod()
    {
      using (BusinessLogicImplementation newInstance = new(new DataLayerConstructorFixcure()))
      {
        bool newInstanceDisposed = true;
        newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
        Assert.IsFalse(newInstanceDisposed);
      }
    }

    [TestMethod]
    public void DisposeTestMethod()
    {
      DataLayerDisposeFixcure dataLayerFixcure = new DataLayerDisposeFixcure();
      BusinessLogicImplementation newInstance = new(dataLayerFixcure);
      Assert.IsFalse(dataLayerFixcure.Disposed);
      bool newInstanceDisposed = true;
      newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
      Assert.IsFalse(newInstanceDisposed);
      newInstance.Dispose();
      newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
      Assert.IsTrue(newInstanceDisposed);
      Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Dispose());
      Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Start(0, (position, ball) => { }));
      Assert.IsTrue(dataLayerFixcure.Disposed);
    }

    [TestMethod]
    public void StartTestMethod()
    {
      DataLayerStartFixcure dataLayerFixcure = new();
      using (BusinessLogicImplementation newInstance = new(dataLayerFixcure))
      {
        int called = 0;
        int numberOfBalls2Create = 10;
        newInstance.Start(
          numberOfBalls2Create,
          (startingPosition, ball) => { called++; Assert.IsNotNull(startingPosition); Assert.IsNotNull(ball); });
        Assert.AreEqual<int>(1, called);
        Assert.IsTrue(dataLayerFixcure.StartCalled);
        Assert.AreEqual<int>(numberOfBalls2Create, dataLayerFixcure.NumberOfBallseCreated);
      }
    }


    private class DataLayerConstructorFixcure : Data.DataAbstractAPI
    {
      public override void SpawnBall(Action<IVector, Data.IBall> upperLayerHandler)
      {
        throw new NotImplementedException();
      }


      public override void Dispose()
      { }

      public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
      {
        throw new NotImplementedException();
      }


      public override IVector CreateVector(double x, double y)
      {
        throw new NotImplementedException();
      }

      public override List<Data.IBall> GetBalls()
      {
        throw new NotImplementedException();
      }

      public override IDimensions GetDimensions()
      {
        throw new NotImplementedException();
      }

      public override bool Equals(object? obj)
      {
        return base.Equals(obj);
      }

      public override int GetHashCode()
      {
        return base.GetHashCode();
      }

      public override string? ToString()
      {
        return base.ToString();
      }

      public override void BeginMovement()
      {
        return;
      }
    }

    private class DataLayerDisposeFixcure : Data.DataAbstractAPI
    {
      internal bool Disposed = false;

      public override void SpawnBall(Action<IVector, Data.IBall> upperLayerHandler)
      {
        throw new NotImplementedException();
      }

      public override void Dispose()
      {
        Disposed = true;
      }

      public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
      {
        throw new NotImplementedException();
      }

      public override IVector CreateVector(double x, double y)
      {
        throw new NotImplementedException();
      }

      public override List<Data.IBall> GetBalls()
      {
        throw new NotImplementedException();
      }
      public override IDimensions GetDimensions()
      {
        throw new NotImplementedException();
      }

      public override bool Equals(object? obj)
      {
        return base.Equals(obj);
      }

      public override int GetHashCode()
      {
        return base.GetHashCode();
      }

      public override string? ToString()
      {
        return base.ToString();
      }

      public override void BeginMovement()
      {
        throw new NotImplementedException();
      }
    }

    private class DataLayerStartFixcure : Data.DataAbstractAPI
    {
      internal bool StartCalled = false;
      internal int NumberOfBallseCreated = -1;

      public override void Dispose()
      { }

      public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
      {
        StartCalled = true;
        NumberOfBallseCreated = numberOfBalls;
        upperLayerHandler(new DataVectorFixture(0, 0), new DataBallFixture());
      }

      public override void SpawnBall(Action<IVector, Data.IBall> upperLayerHandler)
      {
        throw new NotImplementedException();
      }

      public override IVector CreateVector(double x, double y)
      {
        throw new NotImplementedException();
      }

      public override List<Data.IBall> GetBalls()
      {
        throw new NotImplementedException();
      }

      public override IDimensions GetDimensions()
      {
        throw new NotImplementedException();
      }

      public override void BeginMovement()
      {
        return;
      }

      private record DataVectorFixture : Data.IVector
      {
        public DataVectorFixture(double x, double y)
        {
          this.x = x;
          this.y = y;
        }

        public double x { get; init; }
        public double y { get; init; }

        public float Dot(IVector vec)
        {
          throw new NotImplementedException();
        }

        public float Length()
        {
          throw new NotImplementedException();
        }

        public IVector Normalize()
        {
          throw new NotImplementedException();
        }
      }

      private class DataBallFixture : Data.IBall
      {
        public IVector Velocity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IVector Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public double Radius => throw new NotImplementedException();

        public event EventHandler<IVector>? NewPositionNotification = null;

        public Task Move()
        {
          NewPositionNotification?.Invoke(this, new DataVectorFixture(0.0, 0.0));
          return Task.CompletedTask;
        }

        public Task StartMovement()
        {
          throw new NotImplementedException();
        }

        public void StopMovement()
        {
          throw new NotImplementedException();
        }
      }
    }
  }
}
