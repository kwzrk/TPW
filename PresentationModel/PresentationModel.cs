//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//_____________________________________________________________________________________________________________________________________

using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using TP.ConcurrentProgramming.BusinessLogic;
using TP.ConcurrentProgramming.Data;
using UnderneathLayerAPI = TP.ConcurrentProgramming.BusinessLogic.BusinessLogicAbstractAPI;

namespace TP.ConcurrentProgramming.Presentation.Model
{
  /// <summary>
  /// Class Model - implements the <see cref="ModelAbstractApi" />
  /// </summary>
  internal class ModelImplementation : ModelAbstractApi
  {
    internal ModelImplementation() : this(null) { }

    private bool Disposed = false;
    private readonly
      IObservable<EventPattern<BallChangeEventArgs>>
                            eventObservable = null;
    private readonly UnderneathLayerAPI layerBellow = null;


    internal ModelImplementation(UnderneathLayerAPI underneathLayer)
    {
      layerBellow = underneathLayer ?? UnderneathLayerAPI.GetBusinessLogicLayer();

      eventObservable = Observable
        .FromEventPattern<BallChangeEventArgs>(this, "BallChanged");
    }


    public override void Dispose()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(Model));
      layerBellow.Dispose();
      Disposed = true;
    }

    public override IDisposable Subscribe(IObserver<IBall> observer)
    {
      return eventObservable.Subscribe(
        x => observer.OnNext(x.EventArgs.Ball),
        ex => observer.OnError(ex),
        () => observer.OnCompleted()
      );
    }

    public override void Start(int numberOfBalls)
    {
      layerBellow.Start(numberOfBalls, StartHandler);
      Scale?.Invoke(ScaleWidth, ScaleHeight);
    }

    public event EventHandler<BallChangeEventArgs> BallChanged;

    private void StartHandler(BusinessLogic.IPosition position, BusinessLogic.IBall ball)
    {
      ModelBall newBall = new ModelBall(position.x, position.y, ball);
      Scale += newBall.NewScaleNotification;
      BallChanged.Invoke(this, new BallChangeEventArgs() { Ball = newBall });
    }

    public override void ChangingWindowSize(double width, double height)
    {
      var dim = UnderneathLayerAPI.GetBusinessLogicLayer().GetDimensions();
      Debug.WriteLine($"Board Dimensions - Raw: W={dim.Width}, H={dim.Height} | Scaled: W={BoardWidth}, H={BoardHeight}");
      double minWidth = UnderneathLayerAPI.GetBusinessLogicLayer().GetDimensions().Width;
      double minHeight = UnderneathLayerAPI.GetBusinessLogicLayer().GetDimensions().Height;

      if (width < minWidth || height < minHeight)
      {
        ScaleWidth = Math.Clamp(ScaleWidth, 0.5, 2.0);
        ScaleHeight = Math.Clamp(ScaleHeight, 0.5, 2.0);
      }
      else if (height < width)
      {
        ScaleHeight = (height - 200) / UnderneathLayerAPI.GetBusinessLogicLayer().GetDimensions().Height;
        ScaleWidth = ScaleHeight;
      }
      else
      {
        ScaleWidth = (width - 200) / UnderneathLayerAPI.GetBusinessLogicLayer().GetDimensions().Width;
        ScaleHeight = ScaleWidth;
      }
      Scale?.Invoke(ScaleWidth, ScaleHeight);
    }

    public override double ScaleWidth { get; protected set; } = 1.0;
    public override double ScaleHeight { get; protected set; } = 1.0;
    public override double BoardHeight => ScaleHeight * UnderneathLayerAPI.GetBusinessLogicLayer().GetDimensions().Height;
    public override double BoardWidth => ScaleWidth * UnderneathLayerAPI.GetBusinessLogicLayer().GetDimensions().Width;

    public event Action<double, double> Scale;


    [Conditional("DEBUG")]
    internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
    {
      returnInstanceDisposed(Disposed);
    }

    [Conditional("DEBUG")]
    internal void CheckUnderneathLayerAPI(Action<UnderneathLayerAPI> returnNumberOfBalls)
    {
      returnNumberOfBalls(layerBellow);
    }

    [Conditional("DEBUG")]
    internal void CheckBallChangedEvent(Action<bool> returnBallChangedIsNull)
    {
      returnBallChangedIsNull(BallChanged == null);
    }

  }

  public class BallChangeEventArgs : EventArgs
  {
    public IBall Ball { get; init; }
  }
}
