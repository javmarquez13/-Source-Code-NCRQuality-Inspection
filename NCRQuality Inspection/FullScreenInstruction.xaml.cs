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
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace NCRQuality_Inspection
{
    /// <summary>
    /// Interaction logic for FullScreenInstruction.xaml
    /// </summary>
    public partial class FullScreenInstruction : Window
    {
        public FullScreenInstruction(string URIMAIN,string MEDIA, string PATHREPO, string INSTRUCTION, bool CRITICAL)
        {
            InitializeComponent();

            _URIMain     = URIMAIN;
            _Media       = MEDIA;
            _PathRepo    = PATHREPO;
            _Instruction = INSTRUCTION;
            _Critical    = CRITICAL;

            lblInstruction.Content = "INSTRUCTION: " + _Instruction;
        }

        string _URIMain;
        string _Media;
        string _PathRepo;
        string _Instruction;
        bool   _Critical;

        DispatcherTimer _timerCriticalFeatAnimation = new DispatcherTimer();
        int _BorderBrushAnimation = 13;
        float _time = 0;



        public void MaximizeToSecondaryMonitor()
        {
            var secondaryScreen = Screen.AllScreens.Where(s => !s.Primary).FirstOrDefault();

            if (secondaryScreen != null)
            {
                var workingArea = secondaryScreen.WorkingArea;
                this.Left = workingArea.Left;
                this.Top = workingArea.Top;
                //this.Width = workingArea.Width;
                //this.Height = workingArea.Height;                

                if (this.IsLoaded)
                {
                    this.WindowState = WindowState.Maximized;
                }
            }

            try { ImgMain.Source = new BitmapImage(new Uri(_URIMain)); }
            catch (Exception) { ImgMain.Source = new BitmapImage(new Uri(_PathRepo + "WOREF.JPG")); }

            try
            {
                MediaElement.Source = new Uri(_Media);
                MediaElement.Play();
            }
            catch (Exception) { MediaElement.Source = new Uri(_PathRepo + "WOREF.JPG"); }
        }

        private void GridMain_SizeChanged(object sender, SizeChangedEventArgs e)
        {   
            myCanvas.Width = e.NewSize.Width;
            myCanvas.Height = e.NewSize.Height;

            double xChange = 1, yChange = 1;

            if (e.PreviousSize.Width != 0)
                xChange = (e.NewSize.Width / e.PreviousSize.Width);

            if (e.PreviousSize.Height != 0)
                yChange = (e.NewSize.Height / e.PreviousSize.Height);

            foreach (FrameworkElement fe in myCanvas.Children)
            {
                /*because I didn't want to resize the grid I'm having inside the canvas in this particular instance. (doing that from xaml) */
                if (fe is Grid == false)
                {
                    fe.Height = fe.ActualHeight * yChange;
                    fe.Width = fe.ActualWidth * xChange;

                    Canvas.SetTop(fe, Canvas.GetTop(fe) * yChange);
                    Canvas.SetLeft(fe, Canvas.GetLeft(fe) * xChange);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {         
            //DispatcherTimer _timer = new DispatcherTimer();
            //_timer.Interval = new TimeSpan(0, 0, 0, 2);
            //_timer.Tick += _timer_Tick;
            //_timer.Start();

            MaximizeToSecondaryMonitor();
            UICriticalFeat(_Critical);
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
           
        }

        void UICriticalFeat(bool Critical_flag)
        {
            if (Critical_flag)
            {
                _timerCriticalFeatAnimation.Interval = new TimeSpan(0, 0, 0, 1);
                _timerCriticalFeatAnimation.Tick += _timerCriticalFeatAnimation_Tick;

                BrushConverter bc = new BrushConverter();
                //BorderRefImages.BorderThickness = new Thickness(_BorderBrushAnimation, _BorderBrushAnimation, _BorderBrushAnimation, _BorderBrushAnimation);
                BorderRefImages.BorderBrush = (Brush)bc.ConvertFrom("#F44336");

                _timerCriticalFeatAnimation.Start();
            }
            else
            {
                _timerCriticalFeatAnimation.Stop();
                BorderRefImages.BorderThickness = new Thickness(.5, .5, .5, .5);
                BorderRefImages.BorderBrush = Brushes.Black;
            }
        }

        private void _timerCriticalFeatAnimation_Tick(object sender, EventArgs e)
        {
            //_BorderBrushAnimation++;
            //BrushConverter bc = new BrushConverter();
            //BorderRefImages.BorderThickness = new Thickness(_BorderBrushAnimation, _BorderBrushAnimation, _BorderBrushAnimation, _BorderBrushAnimation);
            //BorderRefImages.BorderBrush = (Brush)bc.ConvertFrom("#F44336");
            //if (_BorderBrushAnimation == 15) _BorderBrushAnimation = 13;



            ThicknessAnimation myThicknessAnimation = new ThicknessAnimation();
            myThicknessAnimation.Duration = TimeSpan.FromSeconds(.5);
            myThicknessAnimation.FillBehavior = FillBehavior.Stop;
            myThicknessAnimation.From = new Thickness(.5, .5, .5, .5);
            myThicknessAnimation.To = new Thickness(3, 3, 3, 3);

            Storyboard.SetTargetName(myThicknessAnimation, BorderRefImages.Name);
            Storyboard.SetTargetProperty(myThicknessAnimation, new PropertyPath(Border.BorderThicknessProperty));
            Storyboard ellipseStoryboard = new Storyboard();
            ellipseStoryboard.Children.Add(myThicknessAnimation);
            ellipseStoryboard.Begin(this);


            //Storyboard sb = new Storyboard();
            //ObjectAnimationUsingKeyFrames anim = new ObjectAnimationUsingKeyFrames() { Duration = TimeSpan.FromMilliseconds(200) };
            //anim.KeyFrames.Add(new DiscreteObjectKeyFrame
            //{
            //    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(200)),
            //    Value = new Thickness(10)
            //});
            //Storyboard.SetTarget(anim, BorderRefImages);
            //Storyboard.SetTargetProperty(anim, new PropertyPath(Border.CornerRadiusProperty));
            //sb.Children.Add(anim);
            //sb.Begin();
        }
    }
}
