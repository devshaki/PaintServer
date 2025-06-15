using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Shapes;
namespace PaintClient.model
{
    abstract class ShapeData
    {
        public double x1, x2, y1, y2;

        public void Set1thPoint(double x1, double y1)
        {
            this.x1 = x1;
            this.y1 = y1;
        }

        public void Set2ndPoint(double x1, double y1)
        {
            x2 = x1;
            y2 = y1;
        }

        public abstract void ConstShape();
        public abstract Shape GetShape();


    }
}