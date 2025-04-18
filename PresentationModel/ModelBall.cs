﻿//____________________________________________________________________________________________________________________________________
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
using LogicIBall = TP.ConcurrentProgramming.BusinessLogic.IBall;

namespace TP.ConcurrentProgramming.Presentation.Model
{
  internal class ModelBall : IBall
  {
    public ModelBall(double top, double left, LogicIBall underneathBall)
    {
      TopBackingField = top;
      LeftBackingField = left;
      underneathBall.NewPositionNotification += NewPositionNotification;
    }

    private double TopBackingField;
    private double LeftBackingField;

    public event PropertyChangedEventHandler PropertyChanged;

    public double Diameter { get; init; } = 0;
    public double Top
    {
      get => TopBackingField;

      private set
      {
        if (TopBackingField == value)
          return;
        TopBackingField = value;
        RaisePropertyChanged();
      }
    }
    public double Left
    {
      get => LeftBackingField;

      private set
      {
        if (LeftBackingField == value)
          return;
        LeftBackingField = value;
        RaisePropertyChanged();
      }
    }

    private void NewPositionNotification(object sender, IPosition e)
    {
      Top = e.y;
      Left = e.x;
    }

    private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    [Conditional("DEBUG")]
    internal void SetLeft(double x) { Left = x; }

    [Conditional("DEBUG")]
    internal void SettTop(double x) { Top = x; }

  }
}
