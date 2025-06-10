using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using PaintClient.networking;
using PaintClient.model;
namespace PaintClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary> 
    public partial class MainWindow : Window
    {
        private enum ShapeType { Line, Rectangle, Circle}
        private ShapeType activeShape = ShapeType.Line;
        private Shape tempShape;
        private Point firstPoint;
        private FileManager fileManager;
        private NetworkClient networkClient;
        public MainWindow()
        {
            InitializeComponent();
            fileManager = FileManager.GetFileManager();
            networkClient = new NetworkClient("127.0.0.1", 3333);
            NetworkClient.RecivedFile = LoadShapes;

        }

        private void SelectShape(object sender,RoutedEventArgs arg)
        {
            string shape = (string)((System.Windows.Controls.RadioButton)sender).Content;
            switch (shape)
            {
                case "Line":
                    activeShape = ShapeType.Line;
                    break;
                case "Circle":
                    activeShape = ShapeType.Circle;
                    break;
                case "Rectangle":
                    activeShape = ShapeType.Rectangle;
                    break;
            }

        }
        private Shape CreateShape(ShapeType shapeType)
        {
            switch (shapeType)
            {
                case ShapeType.Line:
                    return new Line { Stroke = Brushes.Black, StrokeThickness = 3 };
                case ShapeType.Circle:
                    return new Ellipse { Stroke = Brushes.Black, StrokeThickness = 3 };
                case ShapeType.Rectangle:
                    return new Rectangle { Stroke = Brushes.Black, StrokeThickness = 3 };
            }
            return null;
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            firstPoint = e.GetPosition(Paint);
            tempShape = CreateShape(activeShape);
            Paint.Children.Add(tempShape);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (tempShape == null || e.LeftButton != MouseButtonState.Pressed)
                return;

            Point endPoint = e.GetPosition(Paint);
            FixShapePoints(tempShape, firstPoint, endPoint);
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            tempShape = null;

        }
        public void FixShapePoints(Shape shape,Point firstPoint, Point endPoint)
        {
            if (shape is Line line)
            {
                line.X1 = firstPoint.X;
                line.Y1 = firstPoint.Y;
                line.X2 = endPoint.X;
                line.Y2 = endPoint.Y;
            }
            else if (shape is Rectangle || shape is Ellipse){
                double x = Math.Min(firstPoint.X, endPoint.X);
                double y = Math.Min(firstPoint.Y, endPoint.Y);
                double height = Math.Abs(endPoint.Y - firstPoint.Y);
                double width = Math.Abs(endPoint.X - firstPoint.X);

                Canvas.SetLeft(shape, x);
                Canvas.SetTop(shape, y);
                shape.Height = height;
                shape.Width = width;
                
            }
        }

        private void LoadShapes(string json)
        {
            List<ShapeData> shapes = fileManager.Json2List(json);
            foreach (ShapeData shape in shapes)
            {
                switch (shape.Type)
                {
                    case "Line":
                        Line line = new Line { Stroke = Brushes.Black, StrokeThickness = 3, X1 = shape.X1, X2 = shape.X2, Y1 = shape.Y1, Y2 = shape.Y2 };
                        Paint.Children.Add(line);
                        break;
                    case "Circle":
                        Ellipse circle = new Ellipse { Stroke = Brushes.Black, StrokeThickness = 3, Width = shape.Width, Height = shape.Height };
                        Canvas.SetLeft(circle,shape.Left);
                        Canvas.SetTop(circle, shape.Top);
                        Paint.Children.Add(circle);
                        break;
                    case "Rectangle":
                        Rectangle rectangle = new Rectangle { Stroke = Brushes.Black, StrokeThickness = 3, Width = shape.Width, Height = shape.Height };
                        Canvas.SetLeft(rectangle, shape.Left);
                        Canvas.SetTop(rectangle, shape.Top);
                        Paint.Children.Add(rectangle);
                        break;
                }
            }
        }
        private void SaveShapes(string filename)
        {
            List<ShapeData> shapes = new List<ShapeData>();

            foreach (UIElement element in Paint.Children)
            {
                if (element is Shape shape)
                {
                    ShapeData shapeData = new ShapeData{};
                    if (shape is Line line)
                    {
                        shapeData.X1 = line.X1;
                        shapeData.Y1 = line.Y1;
                        shapeData.X2 = line.X2;
                        shapeData.Y2 = line.Y2;
                    }
                    else if (shape is Rectangle)
                    {
                        shapeData.Type = "Rectangle";
                        shapeData.Width = shape.Width;
                        shapeData.Height = shape.Height;
                        shapeData.Left = Canvas.GetLeft(shape);
                        shapeData.Top = Canvas.GetTop(shape);

                    }
                    else if (shape is Ellipse)
                    {
                        shapeData.Type = "Circle";
                        shapeData.Width = shape.Width;
                        shapeData.Height = shape.Height;
                        shapeData.Left = Canvas.GetLeft(shape);
                        shapeData.Top = Canvas.GetTop(shape);
                    }
                    shapes.Add(shapeData);
                }
            }
            string json = fileManager.List2Json(shapes);
            int jsonSize = Encoding.UTF8.GetByteCount(json);
            networkClient.SendHeader("upload", FileName.Text.ToString(), jsonSize, json);
        }
    }
}
