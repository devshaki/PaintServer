using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
namespace PaintClient.model
{
    class CircleShape : ShapeData
    {
        private Ellipse circle;

        public CircleShape()
        {
            circle = new Ellipse { Stroke = Brushes.Black, StrokeThickness = 3 };
        }
        public override Shape GetShape()
        {
            return circle;
        }
        public override void ConstShape()
        {
            double x = Math.Min(x1, x2);
            double y = Math.Min(y1, y2);
            double height = Math.Abs(y2 - y1);
            double width = Math.Abs(x2 - x1);

            Canvas.SetLeft(circle, x);
            Canvas.SetTop(circle, y);
            circle.Height = height;
            circle.Width = width;
        }
    }
}
