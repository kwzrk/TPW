//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using TP.ConcurrentProgramming.Presentation.Model;
using ModelIBall = TP.ConcurrentProgramming.Presentation.Model.IBall;

namespace TP.ConcurrentProgramming.Presentation.ViewModel.Test
{
  [TestClass]
  public class MainWindowViewModelUnitTest
  {
    [TestMethod]
    public void ConstructorTest()
    {
      ModelNullFixture nullModelFixture = new();
      Assert.AreEqual<int>(0, nullModelFixture.Disposed);
      Assert.AreEqual<int>(0, nullModelFixture.Started);
      Assert.AreEqual<int>(0, nullModelFixture.Subscribed);
      using (MainWindowViewModel viewModel = new(nullModelFixture))
      {
        Random random = new Random();
        int numberOfBalls = random.Next(1, 10);
        viewModel.Start(numberOfBalls);
        Assert.IsNotNull(viewModel.Balls);
        Assert.AreEqual<int>(0, nullModelFixture.Disposed);
        Assert.AreEqual<int>(numberOfBalls, nullModelFixture.Started);
        Assert.AreEqual<int>(1, nullModelFixture.Subscribed);
      }
      Assert.AreEqual<int>(1, nullModelFixture.Disposed);
    }

    [TestMethod]
    public void BehaviorTestMethod()
    {
      ModelSimulatorFixture modelSimulator = new();
      MainWindowViewModel viewModel = new(modelSimulator);
      Assert.IsNotNull(viewModel.Balls);
      Assert.AreEqual<int>(0, viewModel.Balls.Count);
      Random random = new Random();
      int numberOfBalls = random.Next(1, 10);
      viewModel.Start(numberOfBalls);
      Assert.AreEqual<int>(numberOfBalls, viewModel.Balls.Count);
      viewModel.Dispose();
      Assert.IsTrue(modelSimulator.Disposed);
      Assert.AreEqual<int>(0, viewModel.Balls.Count);
    }

    private class ModelNullFixture : ModelAbstractApi
    {
      internal int Disposed = 0;
      internal int Started = 0;
      internal int Subscribed = 0;

      public override double ScaleWidth { get; protected set; } = 1.0;
      public override double ScaleHeight { get; protected set; } = 1.0;

      public override double BoardWidth => 500;

      public override double BoardHeight => 300;

      public override void ChangingWindowSize(double width, double height)
      {
        ScaleWidth = width / BoardWidth;
        ScaleHeight = height / BoardHeight;
      }

      public override void Dispose()
      {
        Disposed++;
      }
      public override void Start(int numberOfBalls)
      {
        Started = numberOfBalls;
      }
      public override IDisposable Subscribe(IObserver<ModelIBall> observer)
      {
        Subscribed++;
        return new NullDisposable();
      }
      private class NullDisposable : IDisposable
      {
        public void Dispose()
        { }
      }
    }

    private class ModelSimulatorFixture : ModelAbstractApi
    {
      internal bool Disposed = false;

      public ModelSimulatorFixture()
      {
        eventObservable = Observable.FromEventPattern<BallChangeEventArgs>(this, "BallChanged");
      }

      public override IDisposable? Subscribe(IObserver<ModelIBall> observer)
      {
        return eventObservable?.Subscribe(x => observer.OnNext(x.EventArgs.Ball), ex => observer.OnError(ex), () => observer.OnCompleted());
      }

      public override void Start(int numberOfBalls)
      {
        for (int i = 0; i < numberOfBalls; i++)
        {
          ModelBall newBall = new ModelBall(0, 0) { };
          BallChanged?.Invoke(this, new BallChangeEventArgs() { Ball = newBall });
        }
      }

      public override void Dispose()
      {
        Disposed = true;
      }

      public override void ChangingWindowSize(double width, double height)
      {
        ScaleWidth = width / BoardWidth;
        ScaleHeight = height / BoardHeight;
      }

      public event EventHandler<BallChangeEventArgs> BallChanged;
      private IObservable<EventPattern<BallChangeEventArgs>>? eventObservable = null;

      public override double ScaleWidth { get; protected set; } = 1.0;
      public override double ScaleHeight { get; protected set; } = 1.0;

      public override double BoardWidth => 420;

      public override double BoardHeight => 400;

      private class ModelBall : ModelIBall
      {
        public ModelBall(double top, double left)
        { }
        public double Diameter => throw new NotImplementedException();

        public double Top => throw new NotImplementedException();

        public double Left => throw new NotImplementedException();

        public event PropertyChangedEventHandler? PropertyChanged;
      }
    }
  }
}
