using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;
using System.Collections;
using SharpSvn;

namespace NCRQuality_Inspection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// 
    /// First Release by Francisco Javier Marquez 02 Marzo 2021 1.0.0.1
    /// 
    /// 03 Marzo 2021
    /// Se agrega usuario Administrador "User: Admin   Password: Admin" para no actualizar repositorio mientras se debugea
    /// 1.0.0.2
    /// 
    /// 
    /// 03 Marzo 2021
    /// Se agrega SharpSVN para el checkout de repositorio y la consulta de info de repositorio, revision, ultimo autor, ultimo fecha de cambio.
    /// 1.0.0.3
    /// 
    /// 
    /// 03 Marzo 2021
    /// Se agrega funcion de doble click para visualizar media element e pixture box en full screen 
    /// 1.0.0.4
    /// 
    /// 
    /// 03 Marzo 2021
    /// Se agrega funcion para abortar unidades en inspeccion dando doble click al dataGrid se habilita un boton de abort
    /// 1.0.0.5
    /// 
    /// 
    /// 04 Marzo 2021
    /// Se corrige bug doble enter, se corrige bug en unidades abortadas inicen desde el principio.
    /// 1.0.0.6
    /// 
    /// 
    /// 04 Marzo 2021
    /// Se agrega funcio UICriticalFeature para resaltar un feature a inspeccion critico
    /// 1.0.0.7
    /// 
    /// 
    /// 04 Marzo 2021
    /// FIx label imagen de referencia 1 cortado
    /// 1.0.0.8
    /// 
    /// 
    /// 04 Marzo 2021
    /// Funcion para animar el feature critico
    /// 1.0.0.9
    /// 
    /// 
    /// 05 Marzo 2021
    /// Se agrega en forma Login Control+A para logearte directamente como administrador
    /// Se agerga WORK_SATATION dentro del XML
    /// Se agrega HotKeys F1 SI, F2 NO, Control+U UpdateSVN, ESC Abortar inspeccion
    /// 1.0.0.10
    /// 
    /// 
    /// 22 Marzo 2021
    /// Se agrega funcion para visualizar en doble pantalla las imagenes de refernecia SECOND MONITOR
    /// 1.0.0.11
    /// 
    /// 22 Marzo 2021
    /// Se agrega funcion para visualizar en doble pantalla las imagenes de refernecia SECOND MONITOR
    /// 1.0.0.12 validation
    /// 1.0.0.13 validation
    /// 1.0.0.14 validation
    /// 1.0.0.15 validation
    /// 1.0.0.16 validation
    /// 1.0.0.17 validation
    /// 
    /// 13 ABRIL 2021
    /// Release a produccion
    /// 1.0.0.18
    /// 
    /// 13 ABRIL 2021
    /// Added a return missing
    /// 1.0.0.19
    /// 
    /// 14 ABRIL 2021
    /// Fix error any CPU compile
    /// 1.0.0.20
    /// 1.0.0.21
    /// 
    /// 29 ABRIL 2021
    /// Fix error during test 6688
    /// 1.0.0.22
    /// 1.0.0.23
    /// 
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(string[] InputArgs)
        {         
            InitializeComponent();
            AddHotKeys();


            try
            {
                lblVersion.Content = "NCR Quality Inspection V1.0.0.23"; ///Change version before to publish

                USER = InputArgs[0];
                MODE = InputArgs[1];

                _SvnApi.SVN_GET_REVISION();
                AHUTOR = _SvnApi.AHUTOR;
                REVISION_TIME = _SvnApi.REVISION_TIME;
                REVISION = _SvnApi.REVISION;

                lblMode.Content = "APP" + "\n" +
                                  "USER: " + USER + "\n" +
                                  "STAGE: " + MODE + "\n" + "\n" +
                                  "SVN" + "\n" +
                                  "AHUTOR: " + AHUTOR + "\n" +
                                  "REV TIME: " + REVISION_TIME + "\n" +
                                  "REV: " + REVISION;

                //Asignar repositorio 
                if (MODE == "FINAL INSPECTION")
                {
                    _pathRepo = _pathRepoFinalinspection;
                    _dataBaseRepo = _dataBaseRepoFinalInspection;
                }

                if (MODE == "CQA")
                {
                    _pathRepo = _pathRepoCQA;
                    _dataBaseRepo = _dataBaseCQA;
                }

                //Initialize timer for auto log off
                _timerAutoLogOff.Interval = new TimeSpan(0, 0, 1);
                _timerAutoLogOff.Tick += _timerAutoLogOff_Tick;
                _timerAutoLogOff.Start();

                SVN_API.SVN_CHECKOUT();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }         
        }

        private void _timerAutoLogOff_Tick(object sender, EventArgs e)
        {
            TimeSpan _tim = TimeSpan.FromSeconds(_time);
            lblInactiveTime.Content = String.Format(@"Inactive time: {0:hh\:mm\:ss}", _tim);
            _time++;
            if (_time == 1800)
            {
                Login_Application _log = new Login_Application();
                _log.Show();
                this.Close();
            }

            MediaElement.InvalidateVisual();
        }

        //Variables para escalar imagenes en contenedores
        double scale = 1.0;
        double minScale = 1.0;
        double maxScale = 20.0;
        double scalling = 0.4;


        //Variables para informacion de unidad escaneada
        string SerialNumber;
        string WipNumber;
        string TracerNumber;
        string ClassMc;
        int xNode = 0;
        int _CountTestInQueue = 0;

        string[] items;
        List<string> ListFeatures;

        //Variables para identificar si se escaneo tracer o wip
        string WipOrTracer;


        //Variables para lectura de features.
        string _pathFeats = @"\\mxchim0pangea01\diskbld\feats\Feat";

        //Repositorios CQA
        string _pathRepoCQA = @"C:\Trunk\NCR_Quality_Inspection\CQA\";
        string _dataBaseCQA = @"CQA_QualityInspection.xml";

        //Repositorios FINAL INSPECTION
        string _pathRepoFinalinspection = @"C:\Trunk\NCR_Quality_Inspection\FINAL_INSPECTION\";
        string _dataBaseRepoFinalInspection= @"FINAL_INSPECTION_QualityInspection.xml";


        //Variables para asignar repositorio
        string _pathRepo;
        string _dataBaseRepo;
        string _dirCosmeticIssues = @"C:\Cosmetic_Issues\";


        //Win NT Credentials
        string USER;
        string MODE;


        //AUTO loggoff inactivity 20 minutos
        DispatcherTimer _timerAutoLogOff = new DispatcherTimer();
        DispatcherTimer _timerCriticalFeatAnimation = new DispatcherTimer();
        int _BorderBrushAnimation = 13;
        float _time =0;

        //SVN CHECKOUT
        SVN_API _SvnApi = new SVN_API();
        string AHUTOR;
        string REVISION_TIME;
        string REVISION;


        //XML VARIABLES
        string QUESTION;
        string URIMAIN;
        string MEDIA;
        bool CRITICAL;
        string WORK_STATION;


        FullScreenInstruction _SecondMonitor;

        System.Drawing.Color _Color = System.Drawing.Color.LightGreen;

        #region HotKeys

        void AddHotKeys()
        {
            RoutedCommand _UpdateSVNCommand = new RoutedCommand();
            _UpdateSVNCommand.InputGestures.Add(new KeyGesture(Key.U, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(_UpdateSVNCommand, CTR_U_UpdateSVNCommand));

            RoutedCommand _SiCommand = new RoutedCommand();
            _SiCommand.InputGestures.Add(new KeyGesture(Key.F1, ModifierKeys.None));
            CommandBindings.Add(new CommandBinding(_SiCommand, F1_SiCommand));

            RoutedCommand _NoCommand = new RoutedCommand();
            _NoCommand.InputGestures.Add(new KeyGesture(Key.F2, ModifierKeys.None));
            CommandBindings.Add(new CommandBinding(_NoCommand, F2_NoCommand));

            RoutedCommand _AbortCommand = new RoutedCommand();
            _AbortCommand.InputGestures.Add(new KeyGesture(Key.Escape, ModifierKeys.None));
            CommandBindings.Add(new CommandBinding(_AbortCommand, ESC_AbortCommand));
        }

        private void CTR_U_UpdateSVNCommand(object sender, ExecutedRoutedEventArgs e)
        {
            SVN_API.SVN_CHECKOUT();
        }

        private void F1_SiCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (AnswerInspectYes.IsEnabled) AnswerInspectYes_Click(sender, e);

        }

        private void F2_NoCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (AnswerInspectNo.IsEnabled) AnswerInspectNo_Click(sender, e);
        }

        private void ESC_AbortCommand(object sender, ExecutedRoutedEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            AnswerInspectNo.Background = (Brush)bc.ConvertFrom("#3F51B5");
            AnswerInspectNo.Content = "ABORT (ESC)";
            AnswerInspectNo_Click(sender, e);
        }

        #endregion

        //Objeto para escritura en datagrid
        public struct MyDataInQueue
        {
            public string Feature { set; get; }
            public string Status { set; get; }
            public string Count { set; get; }
        }

        string _RANDOMQUESTION = "QUESTION_POSITIVE";
        Random _random = new Random();

        //Reporting
        DataTable _dt = new DataTable();
        DataTable _dtFinalTars = new DataTable();
        DataSet _ds = new DataSet();

        string EXECUTE_ORDER;
        string FEATURE_UNDER_TEST;
        string FEATURE_DESCRIPTION;
        string DEFECT_TYPE;
        string FAILURE_MODE;
        string QUESTION_UNDER_TEST;
        string TRACER;
        string WIP;
        string CLASS;
        bool UNIT_FAIL = false;

        //Dll externas
        iFactoryInfo.iFactoryInfo _ifactoryFunctions = new iFactoryInfo.iFactoryInfo();
       
        #region Images Viewer
        private void ImgMain_MouseWheel(object sender, MouseWheelEventArgs e)
        {   
            // back to normal (maybe this isn't needed since we're making a new one below anyway)
            ImgMain.RenderTransform = null;
            
            foreach (var item in myCanvas.Children)
            {
                if ((item as UIElement).Uid == "ImgMain")
                {
                    UIElement tmp = item as UIElement;
                    Grid1.Children.Remove(item as UIElement);
                    Grid1.Children.Add(tmp);
                    break;
                }
            }

            var position = e.MouseDevice.GetPosition(ImgMain);

            if (e.Delta > 0)
                scale += scalling;
            else
                scale -= scalling;

            if (scale > maxScale)
                scale = maxScale;
            if (scale < minScale)
                scale = minScale;

            ImgMain.RenderTransform = new ScaleTransform(scale, scale, position.X, position.Y);
        }

        private void ImgMain_MouseLeave(object sender, MouseEventArgs e)
        {
            scale = 1.0;
            minScale = 1.0;
            maxScale = 20.0;
            var position = e.MouseDevice.GetPosition(ImgMain);
            ImgMain.RenderTransform = new ScaleTransform(1, 1, position.X, position.Y);
            MediaElement.RenderTransform = new ScaleTransform(1, 1, position.X, position.Y);
        }


        private void MediaElement_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            MediaElement.RenderTransform = null;
            var position = e.MouseDevice.GetPosition(MediaElement);

            if (e.Delta > 0)
                scale += scalling;
            else
                scale -= scalling;

            if (scale > maxScale)
                scale = maxScale;
            if (scale < minScale)
                scale = minScale;

            MediaElement.RenderTransform = new ScaleTransform(scale, scale, position.X, position.Y);
        }
     
        private void MediaElement_MouseLeave(object sender, MouseEventArgs e)
        {
            scale = 1.0;
            minScale = 1.0;
            maxScale = 20.0;
            var position = e.MouseDevice.GetPosition(ImgMain);
            ImgMain.RenderTransform = new ScaleTransform(1, 1, position.X, position.Y);
            MediaElement.RenderTransform = new ScaleTransform(1, 1, position.X, position.Y);
        }

        #endregion

        #region Eventos de control de forma

        //Reacomodar controles cuando la forma cambie de tamano
        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
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

        //Mover la forma sin border con click sostenido
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (e.ChangedButton == MouseButton.Left)
            //    this.DragMove();
        }

        //Cerrar aplicacion con boton exit
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Login_Application _log = new Login_Application();
            _log.Show();
            try { _SecondMonitor.Close(); } catch (Exception) { };
            this.Close();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        #endregion

        #region Eventos de aplicacion
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!StaticFunctions.CheckSynXML(_pathRepo, _dataBaseRepo)) this.Close();

            InicializarDataGrid();
            AnswerInspectNo.IsEnabled = false;
            AnswerInspectYes.IsEnabled = false;
        }

        //Main program
        private void TxtBoxScan_KeyDown(object sender, KeyEventArgs e)
        {
            _time = 0;
            UNIT_FAIL = false;

            _RANDOMQUESTION = StaticFunctions.RandomizeQuestion();

            if(e.Key == Key.Enter)
            {
                bool Continue = ScanningUnit();
                if (!Continue) return;
                txtBoxScan.IsEnabled = false;

                _dtFinalTars = StaticFunctions.CreateTarsToFinalInspection();

                GetFeatureString(ClassMc);
                AnswerInspectNo.IsEnabled = true;
                AnswerInspectYes.IsEnabled = true;

                _dt = StaticFunctions.CreateTars();

                NextFeatInspect();
            }
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            MediaElement.Position = new TimeSpan(0, 0, 1);
            MediaElement.Play();
        }

        #endregion

        #region Funciones de aplicacion

        //Setup de la inspeccion en base al feature images viewers
        void SetupImgView(string EXECUTEORDER, string DESCRIPTION, string QUESTION,  string URIMAIN, string MEDIA, string FEATCLASS, bool CRITIAL_FLAG)
        {
            System.Threading.Thread.Sleep(1000);

            this.Dispatcher.BeginInvoke(new Action(() =>
            {               
                if (DESCRIPTION == "" && URIMAIN == "" && MEDIA == "") goto Clear;

                lblOnInspection.Content = "EN INSPECCION: TEST-" + EXECUTEORDER;
                lblFeatureUnderTest.Content = "FEATURE UNDER TEST: " + FEATCLASS.ToUpper();
                lblDescription.Content = "FEATURE DESCRIPTION: " + DESCRIPTION.ToUpper();
                lblInstruction.Content = "INSTRUCTION: " + QUESTION;


                //lblDescription.Content = "FEATURE EN INSPECCION: " + "\n" + FEATCLASS + "\n" +
                //                         "INSTRUCCION: " + "\n" + QUESTION.ToUpper();

                try { ImgMain.Source = new BitmapImage(new Uri(_pathRepo + FEATCLASS + @"\" + URIMAIN)); }
                catch (Exception) { ImgMain.Source = new BitmapImage(new Uri(_pathRepo + "WOREF.JPG")); }

                try { MediaElement.Source = new Uri(_pathRepo + FEATCLASS + @"\" + MEDIA); }
                catch (Exception) { MediaElement.Source = new Uri(_pathRepo + "WOREF.JPG"); }


                //SecondMonitor(_pathRepo + FEATCLASS + @"\" + URIMAIN, _pathRepo + FEATCLASS + @"\" + MEDIA, _pathRepo, QUESTION, CRITIAL_FLAG); //TEMPORAL FOR DEBUG

                UICriticalFeat(CRITIAL_FLAG);

                if (MEDIA != "")
                {
                    MediaElement.Play();
                }
                Clear: { };

            }));    
        }

        void SecondMonitor(string Uri, string Media, string PathRepo, string Instruction, bool Critical)
        {
            var secondaryScreen = System.Windows.Forms.Screen.AllScreens.Where(s => !s.Primary).FirstOrDefault();

            if (secondaryScreen != null)
            {
                try { _SecondMonitor.Close(); } catch (Exception ex) { }
                _SecondMonitor = new FullScreenInstruction(Uri, Media, PathRepo, Instruction, Critical);
                _SecondMonitor.Show();
            }
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

        //Function para lectura de features en base a la clase
        void GetFeatureString(string Class)
        {
            if (string.IsNullOrEmpty(Class)) return;

            string[] _Lines = { "" };
            string[] _split = { "" };
            string[] _splitFeatures = { "" };
            ListFeatures = new List<string>();

            Class = ClassMc.Split('-')[0];

            int i = 0;
            int _space = 15;

            try
            {
                _Lines = File.ReadAllLines(_pathFeats + Class);

                foreach (string _line in _Lines)
                {
                    if (!_line.Contains("#"))
                    {
                        _split = _line.Split(' ');
                        i = _space - _split[0].Length;

                        if (_split[0].Contains(ClassMc))
                        {
                            _splitFeatures = _split[i].Split(new string[] { Class }, StringSplitOptions.None);

                            foreach(string Feat in _splitFeatures)
                            {
                                if (Feat == "") goto Skip;
                                if (Feat.Length > 5) goto Skip;

                                string Clase = ClassMc.Split('-')[0];
                                XmlDocument xmlInspection = new XmlDocument();
                                xmlInspection.Load(_pathRepo + _dataBaseRepo);
                                XmlNodeList xnList = xmlInspection.GetElementsByTagName(Feat + "-" + Clase);
                                if (xnList.Count == 0) goto Skip;

                                ListFeatures.Add(Feat);
                                _CountTestInQueue = xnList.Count;

                                //for (i=0; i<xnList.Count; i++)
                                //{
                                //    ListFeatures.Add(Feat);                                 
                                //}

                                Skip: { };
                            }
                        }                          
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Funcion para rellenar el dataGrid con todos los features por inspeccionar para la unidad.
        void FillDataGrid(List<string> _list)
        {
            dgInQueue.Items.Clear();
            int flag = 1;

            foreach (string Feat in _list)
            {
                if (flag == 1)
                {
                    WriteDG(Feat, "In Process", _CountTestInQueue.ToString());
                } 
                else WriteDG(Feat, "In Queue", "");
                flag++;
            }
        }

        void MainProgram(List<string> _list)
        {
            string Clase = ClassMc.Split('-')[0];

            try
            {
                if (_list.ToArray().Length == 0)
                {
                    EndInspection();

                    StaticFunctions.SendTars(_dt, TracerNumber);
                    StaticFunctions.SendFinalInspectionTars(_dtFinalTars, TracerNumber);

                    if(UNIT_FAIL) StaticFunctions.CreateToast("UNIDAD FALLADA EN INSPECCION:",TracerNumber, DeskNotify.NotifyType.OnError);
                    else          StaticFunctions.CreateToast("UNIDAD INSPECCIONADA:", TracerNumber, DeskNotify.NotifyType.OnSuccess);

                    return;
                }

                XmlDocument xmlInspection = new XmlDocument();
                xmlInspection.Load(_pathRepo + _dataBaseRepo);
                FEATURE_UNDER_TEST = _list[0];
                XmlNodeList xnList = xmlInspection.GetElementsByTagName(_list[0] + "-" + Clase);
                if (!Directory.Exists(_pathRepo + @"\" + _list[0] + "-" + Clase))
                {
                    MessageBox.Show("Repositorio inexistente:" + "\n" + _pathRepo + @"\" + _list[0] + "-" + Clase, "Error Repositorio", MessageBoxButton.OK, MessageBoxImage.Error);
                    MessageBox.Show("La aplicacion se cerrara... \n Contacte al Administrador", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();  
                }

                EXECUTE_ORDER = xnList[xNode]["EXECUTE_ORDER"].InnerText;
                FEATURE_DESCRIPTION = xnList[xNode]["FEATURE_DESCRIPTION"].InnerText;
                
                _RANDOMQUESTION = StaticFunctions.RandomizeQuestion();

                QUESTION = xnList[xNode][_RANDOMQUESTION].InnerText;
                QUESTION_UNDER_TEST = QUESTION;
                DEFECT_TYPE = xnList[xNode]["DEFECT_TYPE"].InnerText;
                FAILURE_MODE = xnList[xNode]["FAILURE_MODE"].InnerText;
                URIMAIN = xnList[xNode]["URIMAIN"].InnerText;
                MEDIA = xnList[xNode]["MEDIA"].InnerText;
                CRITICAL = Convert.ToBoolean(xnList[xNode]["CRITICAL"].InnerText);
                WORK_STATION = xnList[xNode]["WORK_STATION"].InnerText;


                void SetupAsync()
                {                    
                    SetupImgView(EXECUTE_ORDER, FEATURE_DESCRIPTION, QUESTION, URIMAIN, MEDIA, _list[0] + "-" + Clase, CRITICAL);
                }

                using (LoadingWin _LoadingWin = new LoadingWin(SetupAsync, "CARGANDO SIGUIENTE INSPECCION", _Color)) _LoadingWin.ShowDialog();
               
                if (xnList.Count == 1)
                {
                    if (ListFeatures.ToArray().Length > 0) ListFeatures.RemoveAt(0);
                    xNode = 0;
                }

                if (xnList.Count != xNode && xnList.Count != 1)
                {
                    xNode++;
                    _CountTestInQueue = xnList.Count - xNode;
                    if (xnList.Count == xNode)
                    {
                        ListFeatures.RemoveAt(0);
                        xNode = 0;
                    }                 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        bool ScanningUnit()
        {              
            SerialNumber = txtBoxScan.Text;

            if (SerialNumber.Length == 11) WipOrTracer = "WIP";
            if (SerialNumber.Length == 8) WipOrTracer = "TRACER";

            if (SerialNumber.Length <= 7)
            {
                txtBoxScan.Clear();
                txtBoxScan.Focus();
                return false;
            }
            
            switch (WipOrTracer)
            {
                case "WIP":

                    WipNumber = SerialNumber;
                    items = _ifactoryFunctions.GetWIPInfo(WipNumber);

                    if (items[0] == "There is no row at position 0.")
                    {
                        txtBoxScan.Clear();
                        txtBoxScan.Focus();
                        return false;
                    }

                    WipNumber = items[0];
                    TracerNumber = items[1];
                    ClassMc = items[3];                   

                    string[] split = ClassMc.Split(new string[] { "MC" }, StringSplitOptions.None);

                    ClassMc = split[0] + "-MC" + split[1];
                    break;

                case "TRACER":

                    TracerNumber = SerialNumber;
                    items = _ifactoryFunctions.GetSCMC(TracerNumber);

                    if(items[0] == "Serial Not Found")
                    {
                        txtBoxScan.Clear();
                        txtBoxScan.Focus();
                        return false;
                    }

                    WipNumber = items[0];
                    TracerNumber = SerialNumber;
                    ClassMc = items[1] + "-MC" + items[2];
                    break;

                default:
                    break;        
            }

            lblTracer.Content = TracerNumber;
            lblWip.Content = WipNumber;
            lblClass.Content = ClassMc;
            TRACER = TracerNumber;
            WIP = WipNumber;
            CLASS = ClassMc.Substring(0,4);
            return true;
        }

        #endregion

        #region Funciones para DataGrid

        void InicializarDataGrid()
        {     
            DataGridTextColumn InQueueFeat = new DataGridTextColumn();
            InQueueFeat.Header = "Feature";
            InQueueFeat.Binding = new Binding("Feature");
            InQueueFeat.Width = 50;
            InQueueFeat.IsReadOnly = true;

            dgInQueue.Columns.Add(InQueueFeat);

            DataGridTextColumn InQueueStatus = new DataGridTextColumn();
            InQueueStatus.Header = "Status";
            InQueueStatus.Binding = new Binding("Status");
            InQueueStatus.Width = 60;
            InQueueStatus.IsReadOnly = true;

            dgInQueue.Columns.Add(InQueueStatus);

            DataGridTextColumn InQueueNum = new DataGridTextColumn();
            InQueueNum.Header = "Count";
            InQueueNum.Binding = new Binding("Count");
            InQueueNum.Width = 45;
            InQueueNum.IsReadOnly = true;

            dgInQueue.Columns.Add(InQueueNum);
        }

        void WriteDG(string FEATURE, string STATUS, string COUNT_INQUEUE)
        {
            dgInQueue.Items.Add(new MyDataInQueue { Feature = FEATURE, Status = STATUS, Count = COUNT_INQUEUE });

            if (dgInQueue.Items.Count > 0)
            {
                var border = VisualTreeHelper.GetChild(dgInQueue, 0) as Decorator;
                if (border != null)
                {
                    var scroll = border.Child as ScrollViewer;
                    //if (scroll != null) scroll.ScrollToEnd();
                }
            }
        }

        private void DgInQueue_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var row = e.Row;
            MyDataInQueue _mydata = new MyDataInQueue();
            _mydata = (MyDataInQueue)row.DataContext;

            if (_mydata.Status.Contains("In Process")) row.Background = new SolidColorBrush(Colors.Yellow);
        }
        #endregion

        private void AnswerInspectYes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Evaluation("QUESTION_POSITIVE");
                NextFeatInspect();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
         
        }

        private void AnswerInspectNo_Click(object sender, RoutedEventArgs e)
        {
            if (AnswerInspectNo.Content.ToString() == "ABORT (ESC)")
            {
                _timerCriticalFeatAnimation.Stop();
                BorderRefImages.BorderThickness = new Thickness(.5, .5, .5, .5);
                BorderRefImages.BorderBrush = Brushes.Black;
                BrushConverter bc = new BrushConverter();
                AnswerInspectNo.Background = (Brush)bc.ConvertFrom("#F44336");
                AnswerInspectNo.Content = "NO (F2)";
                dgInQueue.Items.Clear();
                EndInspection();
                goto End;
            }
        
            Evaluation("QUESTION_NEGATIVE");
            NextFeatInspect();

            End: { };
        }

        void NextFeatInspect()
        {
            FillDataGrid(ListFeatures);
            MainProgram(ListFeatures);            
        }

        void Evaluation(string AnswerByOperator)
        {                     
            if (_RANDOMQUESTION != AnswerByOperator) 
            {
                _Color = System.Drawing.Color.Red;
                AddLineTars(DateTime.Now, TRACER, WIP, CLASS, USER, MODE, FEATURE_UNDER_TEST, FEATURE_DESCRIPTION, QUESTION, DEFECT_TYPE, FAILURE_MODE, WORK_STATION, "FAIL");
                StaticFunctions.SendTars(_dt, TracerNumber);
                _dt = StaticFunctions.CreateTars();
                UNIT_FAIL = true;
            }
            else
            {
                _Color = System.Drawing.Color.LightGreen;
                AddLineTars(DateTime.Now, TRACER, WIP, CLASS, USER, MODE, FEATURE_UNDER_TEST, FEATURE_DESCRIPTION, QUESTION_UNDER_TEST, DEFECT_TYPE, FAILURE_MODE, WORK_STATION, "PASS");
                StaticFunctions.SendTars(_dt, TracerNumber);
                _dt = StaticFunctions.CreateTars();
            }
        }

        void EndInspection()
        {
            try { _SecondMonitor.Close(); }
            catch (Exception ex) { }
            SerialNumber = null;
            xNode = 0;
            txtBoxScan.IsEnabled = true;
            ImgMain.Source = null;
            MediaElement.Source = null;
            lblDescription.Content = null;
            lblFeatureUnderTest.Content = null;
            lblInstruction.Content = null;               
            AnswerInspectNo.IsEnabled = false;
            AnswerInspectYes.IsEnabled = false;
            SerialNumber = string.Empty;
            txtBoxScan.Clear();
            txtBoxScan.Focus();
        }


        void AddLineTars(DateTime Date, string Tracer, string Wip, string Class, string User, string Location, string FeatureUnderTest,  string FeatureDescription, string Question, string DefectType, string FailureMode, string WorkStation, string Status)
        {
            _dt.Rows.Add(Date.ToString().ToUpper(), Tracer.ToUpper(), Wip.ToUpper(), Class.ToUpper(), User.ToUpper(), Location.ToUpper(), FeatureUnderTest.ToUpper(), FeatureDescription.ToUpper(), Question.ToUpper(), DefectType.ToUpper(), FailureMode.ToUpper(), WorkStation.ToUpper(), Status.ToUpper());
            _dtFinalTars.Rows.Add(Date.ToString().ToUpper(), Tracer.ToUpper(), Wip.ToUpper(), Class.ToUpper(), User.ToUpper(), Location.ToUpper(), FeatureUnderTest.ToUpper(), FeatureDescription.ToUpper(), Question.ToUpper(), DefectType.ToUpper(), FailureMode.ToUpper(), WorkStation.ToUpper(), Status.ToUpper());
        }

        //DRAG AND DROP IMAGES
        private void Img1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Image img = e.Source as Image;
            //DataObject data = new DataObject(DataFormats.Text, img.Source);
            //DragDrop.DoDragDrop((DependencyObject)e.Source, data, DragDropEffects.Copy);
        }

        private void DockPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        //Drag and drop image from computer
        private void MediaElement_Drop(object sender, DragEventArgs e)
        {
            //if (e.Data.GetDataPresent(DataFormats.FileDrop))
            //{
            //    MediaElement.Source = null;
            //    string[] Dropfile = (string[])e.Data.GetData(DataFormats.FileDrop);
            //    MediaElement.Source = new Uri(Dropfile[0]);
            //    MediaElement.Play();
            //}
        }

        bool fullScreenME = false;
        int _CountForFullScreenME = 0;
        bool fullScreenImg = false;
        int _CountForFullScreenImg = 0;

        private void MediaElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (!fullScreenME && _CountForFullScreenME == 2)
                {
                    CanvasImages.Children.Remove(MediaElement);
                    this.Content = MediaElement;
                    this.WindowStyle = WindowStyle.None;
                    this.WindowState = WindowState.Maximized;
                    _CountForFullScreenME = 0;
                    fullScreenME = true;
                }

                if (fullScreenME && _CountForFullScreenME == 2)
                {
                    this.Content = Grid1;
                    CanvasImages.Children.Add(MediaElement);
                    this.WindowState = WindowState.Normal;
                    _CountForFullScreenME = 0;
                    fullScreenME = false;
                }
                _CountForFullScreenME++;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }     
        }

        private void ImgMain_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (!fullScreenImg && _CountForFullScreenImg == 2)
                {
                    CanvasImages.Children.Remove(MediaElement);
                    this.Content = MediaElement;
                    this.WindowStyle = WindowStyle.None;
                    this.WindowState = WindowState.Maximized;
                    _CountForFullScreenImg = 0;
                    fullScreenImg = true;
                }

                if (fullScreenImg && _CountForFullScreenImg == 2)
                {
                    this.Content = Grid1;
                    CanvasImages.Children.Add(MediaElement);
                    this.WindowState = WindowState.Normal;
                    _CountForFullScreenImg = 0;
                    fullScreenImg = false;
                }

                _CountForFullScreenImg++;
            }
            catch(Exception ex)
            {

            }         
        }

        private void DgInQueue_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            AnswerInspectNo.Background = (Brush)bc.ConvertFrom("#3F51B5");
            AnswerInspectNo.Content = "ABORT (ESC)";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Closing -= Window_Closing;
            e.Cancel = true;
            var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromMilliseconds(250));
            anim.Completed += (s, _) => this.Close();
            this.BeginAnimation(UIElement.OpacityProperty, anim);
        }
    }
}
