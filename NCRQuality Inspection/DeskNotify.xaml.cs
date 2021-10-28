using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NCRQuality_Inspection
{
    /// <summary>
    /// Interaction logic for DeskNotify.xaml
    /// </summary>
    public partial class DeskNotify : Window
    {
        string _TextInterface1;    
        string _TextInterface2;
        NotifyType _type;
        SolidColorBrush _ColorOnError = new SolidColorBrush(Color.FromRgb(244, 67, 54));
        SolidColorBrush _ColorOnAlert = new SolidColorBrush(Color.FromRgb(255, 235, 59));
        SolidColorBrush _ColorOnSuccess = new SolidColorBrush(Color.FromRgb(102, 187, 106));


        public enum NotifyType
        {
            OnError,
            OnAlert,
            OnSuccess,
            OnSVNSucces,
            OnSVNError
        }
     
        public DeskNotify(string textInterface1, string textInterface2, NotifyType notifyType)
        {
            InitializeComponent();
            //_x = x;
            //_y = y;
            _TextInterface1 = textInterface1;
            _TextInterface2 = textInterface2;

            _type = notifyType;
            if (notifyType == NotifyType.OnError) Grid.Background = _ColorOnError;
            if (notifyType == NotifyType.OnAlert) Grid.Background = _ColorOnAlert;
            if (notifyType == NotifyType.OnSuccess) Grid.Background = _ColorOnSuccess;
        }

  
        DispatcherTimer _timer = new DispatcherTimer();

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            //var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            //this.Left = desktopWorkingArea.Right - this.Width;
            //this.Top = _y;
                       
            lblNotify.Content = _TextInterface1;
            lblNotify2.Content = "TRACER: " + _TextInterface2;
            if (_type == NotifyType.OnSuccess) lblSendingTar.Content = "SENDING REPORT PASS...";
            if (_type == NotifyType.OnError) lblSendingTar.Content = "SENDING REPORT FAIL...";

            if (_type == NotifyType.OnSVNSucces || _type == NotifyType.OnSVNError)
            {
                lblSendingTar.Content = " ";
                lblNotify2.Content = _TextInterface2;
            }
      
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Tick += DispatcherTimer_Tick;
            _timer.Start();
            //InvalidateVisual();



            //ANIMATIONS TO VALIDATE
            //var sb = new Storyboard();
            //var anim = new ObjectAnimationUsingKeyFrames() { Duration = TimeSpan.FromMilliseconds(200) };
            //anim.KeyFrames.Add(new DiscreteObjectKeyFrame
            //{
            //    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(200)),
            //    Value = new Thickness(10)
            //});
            //Storyboard.SetTarget(anim, BorderBrushMain);
            //Storyboard.SetTargetProperty(anim, new PropertyPath(Border.CornerRadiusProperty));
            //sb.Children.Add(anim);
            //sb.Begin();

            //Storyboard _storyBoard = new Storyboard();
            //DoubleAnimation _doubleAnimation = new DoubleAnimation();
            //_doubleAnimation.From = 0;
            //_doubleAnimation.To = 30;
            //_doubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(.5));
            //_storyBoard.Children.Add(_doubleAnimation);
            //Storyboard.SetTargetProperty(_doubleAnimation, new PropertyPath(Rectangle.RadiusXProperty));
            //Storyboard.SetTargetName(_doubleAnimation, BorderBrushMain.Name);
            //_storyBoard.Begin(this);


            //Storyboard _storyBoard1 = new Storyboard();
            //_storyBoard1.Children.Add(_doubleAnimation);
            //Storyboard.SetTargetProperty(_doubleAnimation, new PropertyPath(Rectangle.RadiusYProperty));
            //Storyboard.SetTargetName(_doubleAnimation, BorderBrushMain.Name);
            //_storyBoard1.Begin(this);
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        { 
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Closing -= Window_Closing;
            e.Cancel = true;
            var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(1));
            anim.Completed += (s, _) => this.Close();
            this.BeginAnimation(UIElement.OpacityProperty, anim);
        }
    }
}
