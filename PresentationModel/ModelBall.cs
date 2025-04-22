//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2023, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//  by introducing yourself and telling us what you do with this community.
//_____________________________________________________________________________________________________________________________________

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TP.ConcurrentProgramming.BusinessLogic;
using LogicAPI = TP.ConcurrentProgramming.BusinessLogic.BusinessLogicAbstractAPI;
using LogicIBall = TP.ConcurrentProgramming.BusinessLogic.IBall;

namespace TP.ConcurrentProgramming.Presentation.Model
{
  internal class ModelBall : IBall
  {
    public ModelBall(double top, double left, LogicIBall underneathBall)
    {
      TopBackingField = top;
      LeftBackingField = left;
      DiameterBackingField = LogicAPI.GetBusinessLogicLayer().GetDimensions().Diameter;
      underneathBall.NewPositionNotification += NewPositionNotification;
    }

    //public double Top => TopBackingField * ScaleHeight;
    //public double Left => LeftBackingField * ScaleWidth;
    public double Diameter => DiameterBackingField * ScaleWidth;

    private double TopBackingField;
    private double LeftBackingField;
    private double DiameterBackingField;
    private double ScaleWidth = 1;
    private double ScaleHeight = 1;

    public event PropertyChangedEventHandler PropertyChanged;

    public double Top
    {
      get => TopBackingField * ScaleHeight;

      private set
      {
        if (TopBackingField == value)
          return;
        TopBackingField = value;
        RaisePropertyChanged();
        RaisePropertyChanged(nameof(Top));
      }
    }
    public double Left
    {
      get => LeftBackingField * ScaleWidth;

      private set
      {
        if (LeftBackingField == value)
          return;
        LeftBackingField = value;
        RaisePropertyChanged();
        RaisePropertyChanged(nameof(Left));
      }
    }

    private void NewPositionNotification(object sender, IPosition e)
    {
      TopBackingField = e.y;
      LeftBackingField = e.x;
      RaisePropertyChanged(nameof(Top));
      RaisePropertyChanged(nameof(Left));
    }

    public void NewScaleNotification(double scaleWidth, double scaleHeight)
    {
      ScaleWidth = scaleWidth;
      ScaleHeight = scaleHeight;
      RaisePropertyChanged(nameof(Top));
      RaisePropertyChanged(nameof(Left));
      RaisePropertyChanged(nameof(Diameter));
    }

    private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    [Conditional("DEBUG")]
    internal void SetLeft(double x)
    {
      if (LeftBackingField != x)
      {
        LeftBackingField = x;
        RaisePropertyChanged(nameof(Left));
      }
    }

    [Conditional("DEBUG")]
    internal void SettTop(double x)
    {
      if (TopBackingField != x)
      {
        TopBackingField = x;
        RaisePropertyChanged(nameof(Top));
      }
    }

  }
}
