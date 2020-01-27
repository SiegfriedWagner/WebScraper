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
    public class SelectBehavior : Behavior<Canvas>
    {
        public static readonly DependencyProperty SelectionProperty = DependencyProperty.Register("Selection", typeof(Rectangle), typeof(SelectBehavior), new PropertyMetadata(null));
        //https://stackoverflow.com/questions/24479754/wpf-binding-to-properties-from-class-with-custom-behavior
        Point? startPoint = null;
        public Rectangle Selection
        {
            get; set;
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
            startPoint = null;
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (startPoint == null)
                return;
            Canvas obj = AssociatedObject as Canvas;
            Point mousePosition = e.GetPosition(obj);
            Selection.Width = Math.Max(mousePosition.X - startPoint.Value.X, 0);
            Selection.Height = Math.Max(mousePosition.Y -  startPoint.Value.Y, 0);
        }

        private void MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Selection != null && startPoint == null)
                RemoveSelection();
            Canvas obj = AssociatedObject as Canvas;
            startPoint = e.GetPosition(obj);
            Selection = new Rectangle();
            Selection.Width = 5;
            Selection.Height = 5;
            Selection.StrokeThickness = 2;
            Selection.Stroke = new SolidColorBrush(Colors.DarkBlue);
            Selection.Fill = new SolidColorBrush(Colors.Blue);
            Selection.Opacity = 0.2;
            Canvas.SetLeft(Selection, startPoint.Value.X);
            Canvas.SetTop(Selection, startPoint.Value.Y);
            obj.Children.Add(Selection);
        }

        private void RemoveSelection()
        {
            if (Selection == null)
                return;
            Canvas obj = AssociatedObject as Canvas;
            obj.Children.Remove(Selection);
            Selection = null;
            startPoint = null;
        }
}
}
