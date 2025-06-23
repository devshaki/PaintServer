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
using System.Windows.Shapes;
using PaintClient.networking;
using PaintClient.model;
using PaintClient.utils;
namespace PaintClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary> 
    public partial class MainWindow : Window
    {
        public FileBrowserWindow fileBrowserWindow;
        private enum ShapeType { Line, Rectangle, Circle}
        private ShapeType activeShape = ShapeType.Line;
        private ShapeData tempShape;
        private Point firstPoint;
        private NetworkClient networkClient;
        private readonly string ip = "127.0.0.1";
        private readonly int port = 3333;
        public MainWindow()
        {
            InitializeComponent();
            networkClient = NetworkClient.GetNetworkClient(ip, port);
            NetworkClient.RecivedFile = LoadShapes;
            _ = networkClient.Connect();
            fileBrowserWindow = new FileBrowserWindow();
            fileBrowserWindow.mainWindow = this;

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
        private ShapeData CreateShape(ShapeType shapeType)
        {
            switch (shapeType)
            {
                case ShapeType.Line:
                    return new LineShape();
                case ShapeType.Circle:
                    return new CircleShape();
                case ShapeType.Rectangle:
                    return new RectangleShape();
            }
            return null;
        }

        private void CanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            firstPoint = e.GetPosition(Paint);
            tempShape = CreateShape(activeShape);
            Paint.Children.Add(tempShape.GetShape());
        }

        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (tempShape == null || e.LeftButton != MouseButtonState.Pressed)
                return;

            Point endPoint = e.GetPosition(Paint);
            tempShape.Set1thPoint(firstPoint.X, firstPoint.Y);
            tempShape.Set2ndPoint(endPoint.X, endPoint.Y);
            tempShape.ConstShape();

        }

        private void CanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            tempShape = null;

        }

        private void LoadButton(Object sender, RoutedEventArgs e)
        {
            fileBrowserWindow.Show();
            fileBrowserWindow.loadNames();
            this.Hide();
        }
        private void RequestFile(Object sender, RoutedEventArgs e)
        {
            RequestFile(FileName.Text);
        }
        public void RequestFile(string fileName)
        {
            _ = networkClient.SendHeader("get", fileName);
        }

        private void LoadShapes(string json)
        {

            ClearShapes();
            if (json == "locked")
            {
                TextBlock lockedErrorBox = new TextBlock
                {
                    Text = "File is locked",
                    FontSize = 30,
                    Foreground = Brushes.Red,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Canvas.SetLeft(lockedErrorBox, Paint.ActualWidth / 2);
                Canvas.SetTop(lockedErrorBox, Paint.ActualHeight / 2);

                Paint.Children.Add(lockedErrorBox);
            }
            List<ShapeData> shapes = JsonUtils.Json2List<ShapeData>(json);
            Console.WriteLine(shapes.ToString());
            foreach (ShapeData shape in shapes)
            {
                shape.ConstShape();
                Paint.Children.Add(shape.GetShape());
            }
        }

        public void ClearShapes()
        {
            Paint.Children.Clear();
        }
        private void SaveShapes(object sender, RoutedEventArgs e)
        {
            string filename = FileName.Text;
            List<ShapeData> shapes = new List<ShapeData>();

            foreach (UIElement element in Paint.Children)
            {
                if (element is Shape shape)
                {
                    ShapeData newShape = null;

                    if (shape is Line line)
                    {
                        newShape = new LineShape();
                        newShape.x1 = line.X1;
                        newShape.x2 = line.X2;
                        newShape.y1 = line.Y1;
                        newShape.y2 = line.Y2;
                    }
                    else if (shape is Rectangle || shape is Ellipse)
                    {
                        if (shape is Rectangle)
                        {
                            newShape = new RectangleShape();

                        }
                        else if (shape is Ellipse)
                        {
                            newShape = new CircleShape();
                        }
                        newShape.Set1thPoint(shape.Width, shape.Height);
                        newShape.Set2ndPoint(Canvas.GetLeft(shape), Canvas.GetTop(shape));
                    }
                    newShape.ConstShape();
                    shapes.Add(newShape);
                }
            }
            string json = JsonUtils.List2Json(shapes);
            int jsonSize = Encoding.UTF8.GetByteCount(json);
            networkClient.SendHeader("upload", filename, jsonSize, json);
            ClearShapes();
        }

    }
}
