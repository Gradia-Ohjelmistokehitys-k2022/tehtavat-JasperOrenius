using System.Windows;
using System.Windows.Input;

namespace Rising_Star_Pre_assignment.Utils
{
    public class MouseEvents
    {
        public static readonly DependencyProperty MouseMoveCommandProperty = DependencyProperty.RegisterAttached("MouseMoveCommand", typeof(ICommand), typeof(MouseEvents), new PropertyMetadata(null, OnMouseMoveCommandChanged));
        public static readonly DependencyProperty MouseEnterCommandProperty = DependencyProperty.RegisterAttached("MouseEnterCommand", typeof(ICommand), typeof(MouseEvents), new PropertyMetadata(null, OnMouseEnterCommandChanged));
        public static readonly DependencyProperty MouseLeaveCommandProperty = DependencyProperty.RegisterAttached("MouseLeaveCommand", typeof(ICommand), typeof(MouseEvents), new PropertyMetadata(null, OnMouseLeaveCommandChanged));

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

        private static void OnMouseEnterCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                element.MouseEnter -= ElementMouseEnter;
                if (e.NewValue != null)
                {
                    element.MouseEnter += ElementMouseEnter;
                }
            }
        }

        private static void OnMouseLeaveCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                element.MouseLeave -= ElementMouseLeave;
                if (e.NewValue != null)
                {
                    element.MouseLeave += ElementMouseLeave;
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
        
        private static void ElementMouseEnter(object sender, MouseEventArgs e)
        {
            var element = sender as UIElement;
            var command = GetMouseEnterCommand(element);
            if(command != null && command.CanExecute(e))
            {
                command.Execute(e);
            }
        }

        private static void ElementMouseLeave(object sender, MouseEventArgs e)
        {
            var element = sender as UIElement;
            var command = GetMouseLeaveCommand(element);
            if (command != null && command.CanExecute(e))
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

        public static ICommand GetMouseEnterCommand(UIElement element)
        {
            return (ICommand)element.GetValue(MouseEnterCommandProperty);
        }

        public static void SetMouseEnterCommand(UIElement element, ICommand command)
        {
            element.SetValue(MouseEnterCommandProperty, command);
        }

        public static ICommand GetMouseLeaveCommand(UIElement element)
        {
            return (ICommand)element.GetValue(MouseLeaveCommandProperty);
        }

        public static void SetMouseLeaveCommand(UIElement element, ICommand command)
        {
            element.SetValue(MouseLeaveCommandProperty, command);
        }
    }
}
