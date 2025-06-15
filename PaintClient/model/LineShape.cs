using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media;
using System.Windows.Shapes;
namespace PaintClient.model
{
    class LineShape : ShapeData
    {
        private Line line;
        
        public LineShape()
        {
            line = new Line { Stroke = Brushes.Black, StrokeThickness = 3 };
        }
        public override Shape GetShape()
        {
            return line;
        }
        public override void ConstShape()
        {
            line.X1 = x1;
            line.X2 = x2;
            line.Y1 = y1;
            line.Y2 = y2;
        }
    }
}
