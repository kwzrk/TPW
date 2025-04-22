//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TP.ConcurrentProgramming.Data;
using TP.ConcurrentProgramming.Presentation.Model;
using TP.ConcurrentProgramming.Presentation.ViewModel.MVVMLight;
using ModelIBall = TP.ConcurrentProgramming.Presentation.Model.IBall;

namespace TP.ConcurrentProgramming.Presentation.ViewModel
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {

        public MainWindowViewModel() : this(null) { }

        internal MainWindowViewModel(ModelAbstractApi modelLayerAPI)
        {
            ModelLayer = modelLayerAPI ?? ModelAbstractApi.CreateModel();
            Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));
            //Dimensions = ModelLayer.GetDimensions();
            WindowSizeChangedEvent += ModelLayer.ChangingWindowSize;
            StartCommand = new RelayCommand(StartSimulation);
        }

        private double _windowWidth;
        public double WindowWidth
        {
            get => _windowWidth;
            set
            {
                if (_windowWidth != value)
                {
                    _windowWidth = value;
                    NotifyWindowSizeChanged();
                }
            }
        }

        private double _windowHeight;
        public double WindowHeight
        {
            get => _windowHeight;
            set
            {
                if (_windowHeight != value)
                {
                    _windowHeight = value;
                    NotifyWindowSizeChanged();
                }
            }
        }

        private void NotifyWindowSizeChanged()
        {
            RaisePropertyChanged(nameof(BorderWidth));
            RaisePropertyChanged(nameof(BorderHeight));
            WindowSizeChangedEvent?.Invoke(WindowWidth, WindowHeight);
        }

        public double BorderWidth => ModelLayer.BoardWidth;
        public double BorderHeight => ModelLayer.BoardHeight;

        public event Action<double, double> WindowSizeChangedEvent;

        public void TriggerWindowSizeChangedEvent(double width, double height)
        {
            WindowSizeChangedEvent?.Invoke(width, height);
        }

        public void Start(int numberOfBalls)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            ModelLayer.Start(numberOfBalls);
            Observer.Dispose();
        }

        public ObservableCollection<ModelIBall> Balls
        {
            get;
        } = new ObservableCollection<ModelIBall>();

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    Balls.Clear();
                    Observer.Dispose();
                    ModelLayer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                Disposed = true;
            }
        }

        public void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private IDisposable Observer = null;
        private ModelAbstractApi ModelLayer;
        private bool Disposed = false;

        /// <summary>
        /// Adding number of balls 
        /// </summary>
        private int _numberOfBalls;
        public int NumberOfBalls
        {
            get => _numberOfBalls;
            set
            {
                _numberOfBalls = value;
                RaisePropertyChanged();
            }
        }
        public ICommand StartCommand { get; }
        private void StartSimulation()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            if (NumberOfBalls < 1)
                throw new ArgumentOutOfRangeException(nameof(NumberOfBalls), "Number of balls must be greater than 0");
            ModelLayer.Start(NumberOfBalls);
        }
    }
}
