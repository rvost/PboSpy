using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PboSpy.Modules.PreviewImage.Views.Components;

public class ZoomBorder : Border
{
    private UIElement _child = null;
    private Point _origin;
    private Point _start;

    private static TranslateTransform GetTranslateTransform(UIElement element)
    {
        return (TranslateTransform)((TransformGroup)element.RenderTransform)
          .Children.First(tr => tr is TranslateTransform);
    }

    private static ScaleTransform GetScaleTransform(UIElement element)
    {
        return (ScaleTransform)((TransformGroup)element.RenderTransform)
          .Children.First(tr => tr is ScaleTransform);
    }

    public override UIElement Child
    {
        get { return base.Child; }
        set
        {
            if (value != null && value != Child)
                Initialize(value);
            base.Child = value;
        }
    }

    public void Initialize(UIElement element)
    {
        _child = element;
        if (_child != null)
        {
            var group = new TransformGroup();
            var st = new ScaleTransform();
            group.Children.Add(st);
            var tt = new TranslateTransform();
            group.Children.Add(tt);
            _child.RenderTransform = group;
            _child.RenderTransformOrigin = new Point(0.0, 0.0);
            MouseWheel += OnChildMouseWheel;
            MouseLeftButtonDown += OnChildMouseLeftButtonDown;
            MouseLeftButtonUp += OnChildMouseLeftButtonUp;
            MouseMove += OnChildMouseMove;
            PreviewMouseRightButtonDown += new MouseButtonEventHandler(
              OnChildPreviewMouseRightButtonDown);
        }
    }

    public void Reset()
    {
        if (_child != null)
        {
            // reset zoom
            var st = GetScaleTransform(_child);
            st.ScaleX = 1.0;
            st.ScaleY = 1.0;

            // reset pan
            var tt = GetTranslateTransform(_child);
            tt.X = 0.0;
            tt.Y = 0.0;
        }
    }

    #region Child Events

    private void OnChildMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (_child != null)
        {
            var st = GetScaleTransform(_child);
            var tt = GetTranslateTransform(_child);

            double zoom = e.Delta > 0 ? .2 : -.2;
            if (!(e.Delta > 0) && (st.ScaleX < .4 || st.ScaleY < .4))
                return;

            Point relative = e.GetPosition(_child);
            double abosuluteX;
            double abosuluteY;

            abosuluteX = relative.X * st.ScaleX + tt.X;
            abosuluteY = relative.Y * st.ScaleY + tt.Y;

            st.ScaleX += zoom;
            st.ScaleY += zoom;

            tt.X = abosuluteX - relative.X * st.ScaleX;
            tt.Y = abosuluteY - relative.Y * st.ScaleY;
        }
    }

    private void OnChildMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_child != null)
        {
            var tt = GetTranslateTransform(_child);
            _start = e.GetPosition(this);
            _origin = new Point(tt.X, tt.Y);
            Cursor = Cursors.Hand;
            _child.CaptureMouse();
        }
    }

    private void OnChildMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_child != null)
        {
            _child.ReleaseMouseCapture();
            Cursor = Cursors.Arrow;
        }
    }

    void OnChildPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e) => Reset();

    private void OnChildMouseMove(object sender, MouseEventArgs e)
    {
        if (_child != null)
        {
            if (_child.IsMouseCaptured)
            {
                var tt = GetTranslateTransform(_child);
                Vector v = _start - e.GetPosition(this);
                tt.X = _origin.X - v.X;
                tt.Y = _origin.Y - v.Y;
            }
        }
    }

    #endregion
}
