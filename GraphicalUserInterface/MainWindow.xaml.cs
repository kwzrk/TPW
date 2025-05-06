//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using System;
using System.Windows;
using TP.ConcurrentProgramming.Presentation.ViewModel;

namespace TP.ConcurrentProgramming.PresentationView {
    /// <summary>
    /// View implementation
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {            
            InitializeComponent();
            MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;

            this.Width = viewModel.WindowWidth;
            this.Height = viewModel.WindowHeight;

            viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(MainWindowViewModel.WindowWidth))
                {
                    this.Width = viewModel.WindowWidth;
                }
                if (e.PropertyName == nameof(MainWindowViewModel.WindowHeight))
                {
                    this.Height = viewModel.WindowHeight;
                }
            };

            viewModel.TriggerWindowSizeChangedEvent(this.Width, this.Height);
            this.SizeChanged += (s, e) =>
            {
                viewModel.WindowWidth = this.ActualWidth;
                viewModel.WindowHeight = this.ActualHeight;
            };

      this.StateChanged += (s, e) =>
      {
        viewModel.WindowWidth = this.ActualWidth;
        viewModel.WindowHeight = this.ActualHeight;
      };
    }

    /// <summary>
    /// Raises the <seealso cref="System.Windows.Window.Closed"/> event.
    /// </summary>
    /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
    protected override void OnClosed(EventArgs e)
    {
      if (DataContext is MainWindowViewModel viewModel)
        viewModel.Dispose();
      base.OnClosed(e);
    }
  }
}
