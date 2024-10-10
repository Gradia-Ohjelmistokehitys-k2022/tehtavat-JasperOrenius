using System.Windows;
using System.Windows.Input;

namespace Rising_Star_Pre_assignment.Utils
{
    public class MouseEvents
    {
        public static readonly DependencyProperty MouseMoveCommandProperty = DependencyProperty.RegisterAttached("MouseMoveCommand", typeof(ICommand), typeof(MouseEvents), new PropertyMetadata(null, OnMouseMoveCommandChanged));

        private static void OnMouseMoveCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                element.MouseMove -= ElementMouseMove;
                if (e.NewValue != null)
                {
                    element.MouseMove += ElementMouseMove;
                }
            }
        }

        private static void ElementMouseMove(object sender, MouseEventArgs e)
        {
            var element = sender as UIElement;
            var command = GetMouseMoveCommand(element);
            if(command != null && command.CanExecute(e))
            {
                command.Execute(e);
            }
        }

        public static ICommand GetMouseMoveCommand(UIElement element)
        {
            return(ICommand)element.GetValue(MouseMoveCommandProperty);
        }

        public static void SetMouseMoveCommand(UIElement element, ICommand command)
        {
            element.SetValue(MouseMoveCommandProperty, command);
        }
    }
}
