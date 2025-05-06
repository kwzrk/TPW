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

        private const double InitialWindowWidth = 600;
        private const double InitialWindowHeight = 800;
        private double _windowScale = 1.0;

        internal MainWindowViewModel(ModelAbstractApi modelLayerAPI)
        {
            ModelLayer = modelLayerAPI ?? ModelAbstractApi.CreateModel();
            Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));
            WindowSizeChangedEvent += ModelLayer.ChangingWindowSize;
            StartCommand = new RelayCommand(StartSimulation);
            IncreaseSizeCommand = new RelayCommand(IncreaseSize);
            DecreaseSizeCommand = new RelayCommand(DecreaseSize);
            WindowWidth = InitialWindowWidth;
            WindowHeight = InitialWindowHeight;
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
            if (WindowWidth < BorderWidth + 100)
            {
                _windowWidth = BorderWidth + 100;
            }

            if (WindowHeight < BorderHeight + 100)
            {
                _windowHeight = BorderHeight + 100;
            }
            
            RaisePropertyChanged(nameof(WindowWidth));
            RaisePropertyChanged(nameof(WindowHeight));
            
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
        public ICommand IncreaseSizeCommand { get; }
        public ICommand DecreaseSizeCommand { get; }
        private void StartSimulation()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            if (NumberOfBalls < 1)
                throw new ArgumentOutOfRangeException(nameof(NumberOfBalls), "Number of balls must be greater than 0");
            ModelLayer.Start(NumberOfBalls);
        }
        private void IncreaseSize()
        {
            if (_windowScale * 1.1 <= 2.0)
            {
                _windowScale *= 1.1;
                WindowWidth = InitialWindowWidth * _windowScale;
                WindowHeight = InitialWindowHeight * _windowScale;
                NotifyWindowSizeChanged();
            }
        }

        private void DecreaseSize()
        {
            if (_windowScale * 0.9 >= 0.5)
            {
                _windowScale *= 0.9;
                WindowWidth = InitialWindowWidth * _windowScale;
                WindowHeight = InitialWindowHeight * _windowScale;
                NotifyWindowSizeChanged();
            }
        }
    }
}
