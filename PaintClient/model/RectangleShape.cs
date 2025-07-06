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
    class RectangleShape : ShapeData
    {
        private Rectangle rectangle;
            
        public RectangleShape()
        {
            rectangle = new Rectangle { Stroke = Brushes.Black, StrokeThickness = 3 };
        }
        public override Shape GetShape()
        {
            return rectangle;
        }
        public override void ConstShape()
        {
            double x = Math.Min(x1, x2);
            double y = Math.Min(y1, y2);
            double height = Math.Abs(y2 - y1);
            double width = Math.Abs(x2 - x1);

            Canvas.SetLeft(rectangle, x);
            Canvas.SetTop(rectangle, y);
            rectangle.Height = height;
            rectangle.Width = width;
        }
    }
}
