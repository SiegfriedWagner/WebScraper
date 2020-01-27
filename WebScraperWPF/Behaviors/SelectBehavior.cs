using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;

namespace WebScraperWPF.Behaviors
{
    public class Selection
    {
        public Selection(double top, double left, double width, double height)
        {
            Top = top;
            Left = left;
            Width = width;
            Height = height;
        }
        public double Top { private set;  get; }
        public double Left { private set;  get; }
        public double Width { private set; get; }
        public double Height { private set; get; }
    }
    public class SelectBehavior : Behavior<Canvas>
    {
        public static readonly DependencyProperty SelectionProperty = DependencyProperty.Register(
            "Selection", 
            typeof(Selection), 
            typeof(SelectBehavior),
            new FrameworkPropertyMetadata(null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        //https://stackoverflow.com/questions/24479754/wpf-binding-to-properties-from-class-with-custom-behavior
        Point? startPoint = null;
        public Selection Selection
        {
            get { return (Selection)GetValue(SelectionProperty); }
            set { SetValue(SelectionProperty, value); }
        }
        private Rectangle CanvasRectanangle
        {
            get; set;
        }
        public SelectBehavior() : base()
        {
            Selection = new Selection(-1, -1, -1, -1);
        }
        protected override void OnAttached()
        {

            AssociatedObject.MouseDown += MouseDown;
            AssociatedObject.MouseMove += MouseMove;
            AssociatedObject.MouseUp += MouseUp;
            base.OnAttached();
        }

        private void MouseUp(object sender, MouseButtonEventArgs e)
        {
            var a = Selection;
            startPoint = null;           
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (startPoint == null)
                return;
            Canvas obj = AssociatedObject as Canvas;
            Point mousePosition = e.GetPosition(obj);
            CanvasRectanangle.Width = Math.Max(mousePosition.X - startPoint.Value.X, 0);
            CanvasRectanangle.Height = Math.Max(mousePosition.Y -  startPoint.Value.Y, 0);
            Selection = new Selection(left: startPoint.Value.X, top: startPoint.Value.Y, width: CanvasRectanangle.Width, height: CanvasRectanangle.Height);
        }

        private void MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CanvasRectanangle != null && startPoint == null)
                RemoveSelection();
            Canvas obj = AssociatedObject as Canvas;
            startPoint = e.GetPosition(obj);
            CanvasRectanangle = new Rectangle();
            CanvasRectanangle.Width = 5;
            CanvasRectanangle.Height = 5;
            CanvasRectanangle.StrokeThickness = 2;
            CanvasRectanangle.Stroke = new SolidColorBrush(Colors.DarkBlue);
            CanvasRectanangle.Fill = new SolidColorBrush(Colors.Blue);
            CanvasRectanangle.Opacity = 0.2;
            Canvas.SetLeft(CanvasRectanangle, startPoint.Value.X);
            Canvas.SetTop(CanvasRectanangle, startPoint.Value.Y);
            obj.Children.Add(CanvasRectanangle);
        }

        private void RemoveSelection()
        {
            if (CanvasRectanangle == null)
                return;
            Canvas obj = AssociatedObject as Canvas;
            obj.Children.Remove(CanvasRectanangle);
            CanvasRectanangle = null;
            startPoint = null;
        }
}
}
