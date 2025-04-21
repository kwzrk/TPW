namespace TP.ConcurrentProgramming.Data
{
  internal record Dimensions : IDimensions
  {

    public Dimensions(double Radius, int Height, int Width)
    {
      this.Radius = Radius;
      this.Height = Height;
      this.Width = Width;
    }

    public double Radius { get; init; }
    public int Height { get; init; }
    public int Width { get; init; }
  }
}
