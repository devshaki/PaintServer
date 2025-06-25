using System.ComponentModel;
namespace PaintClient.model
{
    enum ShapeType
    {
        [Description("Line")]
        Line,
        [Description("Rectangle")]
        Rectangle,
        [Description("Circle")]
        Circle,
    }
}
