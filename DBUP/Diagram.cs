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

namespace DBUP
{
    class Diagram
    {
        private Canvas canvas;
        //Координаты центра диаграммы
        static double centerX; //280
        static double centerY; //280
                               //Размер диаграммы (Диаметр окружности уровня 1)
        static double sizeDiagramm; //500
                                    //Словарь координат секторов
        static Dictionary<int, double[,]> dictionaryCoordinateSectors;
        //Дополнительные точки для диаграммы Ev
        static double[,] additionalPoints;
        public Diagram(double _centerX, double _centerY, double _sizeDiagramm, Canvas _canvas)
        {
            centerX = _centerX;
            centerY = _centerY;
            sizeDiagramm = _sizeDiagramm;
            dictionaryCoordinateSectors = new Dictionary<int, double[,]>();
            canvas = _canvas;
            additionalPoints = new double[,] { { -300.0, canvas.Height }, { 0.0, 0.0 }, { canvas.Width + 300.0, 0.0 }, { canvas.Width + 300, canvas.Height + 300 } };
        }

        //Диаграмма групповых показателей
        public void DrawGroupDiagram(double[] args)
        {
            DrawEmptyDiagram();
            DrawSectors(args);
        }

        public void DrawEvDiagram(double[] args)
        {
            DrawEmptyDiagram();
            DictionaryCorrection();
            DrawSectors(args);
        }

        public void DrawResultDiagram(double R)
        {
            DrawEmptyDiagram();
            string red = "#F44236";
            string yellow = "#FEC107";
            string green = "#4CAF52";
            double radius = (sizeDiagramm / 2) * R;

            Path path = new Path();
            path.Stroke = System.Windows.Media.Brushes.Black;
            path.StrokeThickness = 1;

            EllipseGeometry ellipse = new EllipseGeometry();
            ellipse.Center = new Point(centerX, centerY);
            ellipse.RadiusX = radius;
            ellipse.RadiusY = radius;

            Brush color = Brushes.Yellow;
            if (R >= 0 && R < 0.5)
            {
                color = (SolidColorBrush)(new BrushConverter().ConvertFrom(red));
            }
            else if (R >= 0.5 && R < 0.85)
            {
                color = (SolidColorBrush)(new BrushConverter().ConvertFrom(yellow));
            }
            else if (R >= 0.85 && R <= 1)
            {
                color = (SolidColorBrush)(new BrushConverter().ConvertFrom(green));
            }
            path.Fill = color;
            path.MouseEnter += new MouseEventHandler(MouseEnter);
            path.MouseLeave += new MouseEventHandler(MouseLeave);
            ToolTip toolTip = new ToolTip();
            toolTip.Content = "R = " + R.ToString();
            path.ToolTip = toolTip;
            path.Data = ellipse;
            Canvas.SetZIndex(path, 1);
            canvas.Children.Add(path);
        }

        void DictionaryCorrection()
        {
            Dictionary<int, double[,]> newDictionary = new Dictionary<int, double[,]>();
            newDictionary.Add(0, new double[,] { { dictionaryCoordinateSectors[33][0, 0], dictionaryCoordinateSectors[33][0, 1] }, { dictionaryCoordinateSectors[10][1, 0], dictionaryCoordinateSectors[10][1, 1] } });
            newDictionary.Add(1, new double[,] { { dictionaryCoordinateSectors[10][1, 0], dictionaryCoordinateSectors[10][1, 1] }, { dictionaryCoordinateSectors[27][1, 0], dictionaryCoordinateSectors[27][1, 1] } });
            newDictionary.Add(2, new double[,] { { dictionaryCoordinateSectors[27][1, 0], dictionaryCoordinateSectors[27][1, 1] }, { dictionaryCoordinateSectors[33][0, 0], dictionaryCoordinateSectors[33][0, 1] } });
            dictionaryCoordinateSectors = newDictionary;
        }

        public void DrawEmptyDiagram()
        {
            DrawEllipse(1);
            DrawEllipse(0.95);
            DrawEllipse(0.85);
            DrawEllipse(0.7);
            DrawEllipse(0.5);
            DrawEllipse(0.25);
            DrawLines();
            DrawNumbers();
        }


        //Рисование окружностей (границ уровней) lenhth - уровень
        void DrawEllipse(double length)
        {
            //Определяем размер окружности как доля от размера диаграммы
            double size = sizeDiagramm * length;
            //Создание и инициализация окружности
            Ellipse ellipse = new Ellipse();
            ellipse.Width = size;
            ellipse.Height = size;
            Canvas.SetTop(ellipse, centerX - size / 2);
            Canvas.SetLeft(ellipse, centerY - size / 2);
            //Окружность уровня соответствия 4 т.е [0.85;0.95) рисуем зеленым
            if (length == 0.85)
            {
                ellipse.StrokeThickness = 3;
                ellipse.Stroke = Brushes.Green;

            }
            else
            {
                ellipse.Stroke = Brushes.Black;
                ellipse.StrokeThickness = 2;
            }
            Canvas.SetZIndex(ellipse, 3);
            canvas.Children.Add(ellipse);
        }

        //Обработчик событий наведения мыши на элемент
        void MouseEnter(object sender, MouseEventArgs e)
        {
            //Увеличиваем гранницу элемента
            Path path = (Path)sender;
            Canvas.SetZIndex(path, 2);
            path.StrokeThickness = 3;
        }
        //Обработчик событий покидания границ элемента
        void MouseLeave(object sender, MouseEventArgs e)
        {
            //Уменьшаем границу элемента
            Path path = (Path)sender;
            Canvas.SetZIndex(path, 1);
            path.StrokeThickness = 1;
        }


        //Рисование делений диаграммы
        void DrawLines()
        {
            dictionaryCoordinateSectors = new Dictionary<int, double[,]>();
            double alpha = 0;       //Угол сектора
            double k = 360.0 / 34.0;
            double x = centerX, y = centerY * 2; //Начальная точка 
            double r = sizeDiagramm / 2;       //Радиус
            Line a = new Line();
            double[] currentCoordinate = new double[] { };
            double[] prevCoordinate = new double[] { x, y };

            //Вычисление точек на окружности и рисование линий из центра в эти точки
            for (int i = 0; i < 34; i++)
            {
                a = new Line();
                a.X1 = centerX;
                a.X2 = x + Math.Sin(Math.PI / 180.0 * alpha) * r;
                a.Y1 = centerX;
                a.Y2 = x + Math.Cos(Math.PI / 180.0 * alpha) * r;
                a.Stroke = Brushes.Black;
                Canvas.SetZIndex(a, 3);
                canvas.Children.Add(a);
                alpha += k;

                currentCoordinate = new double[] { x + Math.Sin(Math.PI / 180.0 * alpha) * r, x + Math.Cos(Math.PI / 180.0 * alpha) * r };
                dictionaryCoordinateSectors.Add(33 - i, new double[,] { { prevCoordinate[0], prevCoordinate[1] }, { currentCoordinate[0], currentCoordinate[1] } });
                prevCoordinate = currentCoordinate;
            }
        }

        //Нумерование делений диаграммы
        void DrawNumbers()
        {
            double alpha = 360.0 / 34.0;
            double k = alpha;
            alpha /= 2;
            double x = centerX - 7, y = (centerY - 5) * 2;
            double r = sizeDiagramm / 2 + 15;

            for (int i = 34; i > 0; i--)
            {
                TextBlock text = new TextBlock();
                text.Text = i.ToString();
                // double[] currentCoordinate = new double[] { x + Math.Sin(Math.PI / 180.0 * alpha) * r, x + Math.Cos(Math.PI / 180.0 * alpha) * r };
                Canvas.SetLeft(text, x + Math.Sin(Math.PI / 180.0 * alpha) * r);
                Canvas.SetTop(text, x + Math.Cos(Math.PI / 180.0 * alpha) * r);
                canvas.Children.Add(text);
                alpha += k;

                //dictionaryCoordinateSectors.Add(i-1, new double[,] { {prevCoordinate[0],prevCoordinate[1] },{currentCoordinate[0],currentCoordinate[1] } });
                //prevCoordinate = currentCoordinate;
            }
        }

        //Рисование сектора
        void DrawSectors(double[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                double X1 = dictionaryCoordinateSectors[i][0, 0];
                double Y1 = dictionaryCoordinateSectors[i][0, 1];
                double X2 = dictionaryCoordinateSectors[i][1, 0];
                double Y2 = dictionaryCoordinateSectors[i][1, 1];
                double size = args[i];
                double radius = (sizeDiagramm / 2) * size;
                string red = "#F44236";
                string yellow = "#FEC107";
                string green = "#4CAF52";


                Path path = new Path();
                path.Stroke = System.Windows.Media.Brushes.Black;
                path.StrokeThickness = 1;
                Brush color = Brushes.Aquamarine;
                if (size >= 0 && size < 0.5)
                {
                    color = (SolidColorBrush)(new BrushConverter().ConvertFrom(red));//#BB0707
                }
                else if (size >= 0.5 && size < 0.85)
                {
                    color = (SolidColorBrush)(new BrushConverter().ConvertFrom(yellow));//#FF9308
                }
                else if (size >= 0.85 && size <= 1)
                {
                    color = (SolidColorBrush)(new BrushConverter().ConvertFrom(green));//#3FCA3F
                }
                path.Fill = color;



                CombinedGeometry combinedGeometry = new CombinedGeometry();
                combinedGeometry.GeometryCombineMode = GeometryCombineMode.Intersect;

                GeometryGroup geometryGroup_0 = new GeometryGroup();
                //geometryGroup_0.FillRule = FillRule.EvenOdd;


                EllipseGeometry ellipse = new EllipseGeometry();
                ellipse.Center = new Point(centerX, centerY);
                ellipse.RadiusX = radius;
                ellipse.RadiusY = radius;

                geometryGroup_0.Children.Add(ellipse);

                GeometryGroup geometryGroup = new GeometryGroup();
                //geometryGroup.FillRule = FillRule.EvenOdd;

                PathGeometry pathGeometry = new PathGeometry();

                PathFigure pathFigure = new PathFigure();
                pathFigure.IsClosed = true;
                pathFigure.IsFilled = true;
                pathFigure.StartPoint = new Point(centerX, centerY);

                LineSegment lineSegment1 = new LineSegment();
                lineSegment1.Point = new Point(X1, Y1);
                pathFigure.Segments.Add(lineSegment1);
                if (args.Length == 3)
                {
                    if (i == 0)
                    {
                        LineSegment additionalLineSegment = new LineSegment();
                        additionalLineSegment.Point = new Point(additionalPoints[0, 0], additionalPoints[0, 1]);
                        pathFigure.Segments.Add(additionalLineSegment);
                    }
                    else if (i == 1)
                    {
                        LineSegment additionalLineSegment = new LineSegment();
                        additionalLineSegment.Point = new Point(additionalPoints[1, 0], additionalPoints[1, 1]);
                        LineSegment additionalLineSegment2 = new LineSegment();
                        additionalLineSegment2.Point = new Point(additionalPoints[2, 0], additionalPoints[2, 1]);
                        pathFigure.Segments.Add(additionalLineSegment);
                        pathFigure.Segments.Add(additionalLineSegment2);
                    }
                    else if (i == 2)
                    {
                        LineSegment additionalLineSegment = new LineSegment();
                        additionalLineSegment.Point = new Point(additionalPoints[3, 0], additionalPoints[3, 1]);
                        pathFigure.Segments.Add(additionalLineSegment);
                    }
                }

                LineSegment lineSegment2 = new LineSegment();
                lineSegment2.Point = new Point(X2, Y2);

                pathFigure.Segments.Add(lineSegment2);

                PathFigureCollection figureCollection = new PathFigureCollection();
                figureCollection.Add(pathFigure);

                pathGeometry.Figures = figureCollection;

                geometryGroup.Children.Add(pathGeometry);

                combinedGeometry.Geometry1 = geometryGroup_0;
                combinedGeometry.Geometry2 = geometryGroup;

                path.Data = combinedGeometry;
                path.MouseEnter += new MouseEventHandler(MouseEnter);
                path.MouseLeave += new MouseEventHandler(MouseLeave);

                ToolTip toolTip = new ToolTip();
                //string str = dictionaryCoordinateSectors.Count == 3 ? "EV" + (i + 1).ToString() + " = " : "";
                toolTip.Content = size.ToString();
                path.ToolTip = toolTip;

                Canvas.SetZIndex(path, 1);

                canvas.Children.Add(path);
            }

            dictionaryCoordinateSectors = new Dictionary<int, double[,]>();
        }
    }
}
