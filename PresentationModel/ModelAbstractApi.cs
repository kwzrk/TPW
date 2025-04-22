//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using System;
using System.ComponentModel;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.Presentation.Model
{
  public interface IBall : INotifyPropertyChanged
  {
    double Top { get; }
    double Left { get; }
    double Diameter { get; }
  }

  public abstract class ModelAbstractApi : IObservable<IBall>, IDisposable
  {
    private static Lazy<ModelAbstractApi> modelInstance = new Lazy<ModelAbstractApi>(() => new ModelImplementation());

    public static ModelAbstractApi CreateModel()
    {
      return modelInstance.Value;
    }
    public abstract void Start(int numberOfBalls);
    public abstract IDisposable Subscribe(IObserver<IBall> observer);
    public abstract void Dispose();
    public abstract IDimensions GetDimensions();
  }
}
