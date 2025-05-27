using System.Collections.Concurrent;
using System.Numerics;

namespace TP.ConcurrentProgramming.Data
{
    internal class Logger : ILogger, IDisposable
    {

        private readonly ConcurrentQueue<string> _logQueue = new ConcurrentQueue<string>();
        private readonly string _logFilePath;
        private Task? _loggingTask;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly object _disposeLock = new object();
        private bool _disposed = false;

        public Logger()
        {
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            Directory.CreateDirectory(logDirectory);
            _logFilePath = Path.Combine(logDirectory, $"BallSimulationLog_{DateTime.Now:yyyyMMdd_HHmmss}.log");
            _loggingTask = Task.Run(() => ProcessLogQueue(_cancellationTokenSource.Token));
        }

        public void Log(string message)
        {
            if (_disposed) return;
            _logQueue.Enqueue($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}] {message}");
        }

        public void LogBallState(int ballId, Vector2 position, Vector2 velocity)
        {
            if (_disposed) return;
            Log($"BallID: {ballId}, Pos: ({position.X:F2},{position.Y:F2}), Vel: ({velocity.X:F2},{velocity.Y:F2})");
        }

        private async Task ProcessLogQueue(CancellationToken cancellationToken)
        {
            using (StreamWriter writer = new StreamWriter(_logFilePath, append: true))
            {
                while (!cancellationToken.IsCancellationRequested || !_logQueue.IsEmpty)
                {
                    if (_logQueue.TryDequeue(out string? message))
                    {
                        await writer.WriteLineAsync(message);
                        await writer.FlushAsync();
                    }
                    else
                    {
                        await Task.Delay(10, cancellationToken);
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            lock (_disposeLock)
            {
                if (_disposed) return;

                if (disposing)
                {
                    _cancellationTokenSource.Cancel();
                    _loggingTask?.Wait(1000);
                    _loggingTask = null;

                    using (StreamWriter writer = new StreamWriter(_logFilePath, append: true))
                    {
                        while (_logQueue.TryDequeue(out string? message))
                        {
                            writer.WriteLine(message);
                        }
                    }
                    _cancellationTokenSource.Dispose();
                }
                _disposed = true;
            }
        }

        ~Logger()
        {
            Dispose(false);
        }

    }
}
