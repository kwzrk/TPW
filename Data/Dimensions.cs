namespace TP.ConcurrentProgramming.Data
{
  internal record Dimensions : IDimensions
  {

    public Dimensions(double Radius, double Height, double Width)
    {
      this.Radius = Radius;
      this.Height = Height;
      this.Width = Width;
    }

    public double Radius { get; init; }
    public double Height { get; init; }
    public double Width { get; init; }
  }
}
