using TP.ConcurrentProgramming.BusinessLogic;

namespace TP.ConcurrentProgramming.Data
{
  internal record BusinessDimensions : IBusinessDimensions
  {
    public BusinessDimensions(Data.IDimensions dimensions)
    {
      Height = dimensions.Height;
      Width = dimensions.Width;
      Diameter = dimensions.Radius;
    }

    public double Height { get; init; }
    public double Width { get; init; }
    public double Diameter { get; init; }
  }
}
