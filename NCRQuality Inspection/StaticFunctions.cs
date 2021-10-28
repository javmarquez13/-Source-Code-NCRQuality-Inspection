using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace NCRQuality_Inspection
{
    class StaticFunctions
    {
        public static string RandomizeQuestion()
        {
            Random _random = new Random();
            string _RANDOMQUESTION = string.Empty;
            int randomQuestion = _random.Next(0, 5);
            
            switch (randomQuestion)
            {
                //POSITIVE
                case 0:
                    _RANDOMQUESTION = "QUESTION_POSITIVE";
                    break;

                case 1:
                    _RANDOMQUESTION = "QUESTION_NEGATIVE";
                    break;
                case 2:
                    _RANDOMQUESTION = "QUESTION_POSITIVE";
                    break;
                //NEGATIVE
                case 3:
                    _RANDOMQUESTION = "QUESTION_NEGATIVE";
                    break;
                case 4:
                    _RANDOMQUESTION = "QUESTION_POSITIVE";
                    break;
                case 5:
                    _RANDOMQUESTION = "QUESTION_NEGATIVE";
                    break;
            }
            return _RANDOMQUESTION;
        }

        public static DataTable CreateTarsToFinalInspection()
        {
            DataTable _dtFinalTars = new DataTable();
            _dtFinalTars.TableName = "TEST";
            _dtFinalTars.Columns.Add("DATE");
            _dtFinalTars.Columns.Add("TRACER");
            _dtFinalTars.Columns.Add("WIP");
            _dtFinalTars.Columns.Add("CLASS");
            _dtFinalTars.Columns.Add("USER");
            _dtFinalTars.Columns.Add("LOCATION");
            _dtFinalTars.Columns.Add("FEAT_UNDER_TEST");
            _dtFinalTars.Columns.Add("FEATURE_DESCRIPTION");
            _dtFinalTars.Columns.Add("QUESTION");
            _dtFinalTars.Columns.Add("DEFECT_TYPE");
            _dtFinalTars.Columns.Add("FAILURE_MODE");
            _dtFinalTars.Columns.Add("WORK_STATION");
            _dtFinalTars.Columns.Add("STATUS");

            return _dtFinalTars;
        }

        public static DataTable CreateTars()
        {
            DataTable _dt = new DataTable();
            _dt.TableName = "TEST";
            _dt.Columns.Add("DATE");
            _dt.Columns.Add("TRACER");
            _dt.Columns.Add("WIP");
            _dt.Columns.Add("CLASS");
            _dt.Columns.Add("USER");
            _dt.Columns.Add("LOCATION");
            _dt.Columns.Add("FEAT_UNDER_TEST");
            _dt.Columns.Add("FEATURE_DESCRIPTION");
            _dt.Columns.Add("QUESTION");
            _dt.Columns.Add("DEFECT_TYPE");
            _dt.Columns.Add("FAILURE_MODE");
            _dt.Columns.Add("WORK_STATION");
            _dt.Columns.Add("STATUS");
            return _dt;
        }

        public static void SendTars(DataTable _dt, string TracerNumber)
        {
            string _dirCosmeticIssues = @"C:\Cosmetic_Issues\";
            DataSet _ds = new DataSet();
            _ds.DataSetName = "COSMETIC_SCRIPT";
            _ds.Namespace = "RECURSO=PENDIENTE/STEPNAME=PENDIENTE";
            _ds.Tables.Add(_dt.Copy());
            string xmlResult = _ds.GetXml();

            if (!Directory.Exists(_dirCosmeticIssues)) Directory.CreateDirectory(_dirCosmeticIssues);

            File.WriteAllText(_dirCosmeticIssues + TracerNumber + "_" + DateTime.Now.Millisecond + ".xml", xmlResult);
        }

        public static void SendFinalInspectionTars(DataTable _dtFinalTars, string TracerNumber)
        {
            DataSet _ds = new DataSet();
            _ds.DataSetName = "COSMETIC_SCRIPT";
            _ds.Namespace = "RECURSO=PENDIENTE/STEPNAME=PENDIENTE";
            _ds.Tables.Add(_dtFinalTars.Copy());
            string xmlResult = _ds.GetXml();

            if (!Directory.Exists(@"C:\xml\")) Directory.CreateDirectory(@"C:\xml\");

            File.WriteAllText(@"C:\xml\" + TracerNumber + "_" + DateTime.Now.Millisecond + ".xml", xmlResult);
        }

        public static bool CheckSynXML(string _pathRepo, string _dataBaseRepo)
        {
            bool result = false;
            try
            {
                XmlDocument xmlInspection = new XmlDocument();
                xmlInspection.Load(_pathRepo + _dataBaseRepo);
                result = true;
            }
            catch (Exception ex)
            {
                //if (ex.Message.Contains("Line"))
                //{
                //    string _lineNumber = ex.Message
                //}

                MessageBox.Show("Error de syntaxis en repositorio: " + _dataBaseRepo + "\n" + ex.Message, "Error on XML", MessageBoxButton.OK, MessageBoxImage.Error);
                MessageBox.Show("La aplicacion se cerrara... \n Contacte al Administrador", "Error on XML", MessageBoxButton.OK, MessageBoxImage.Error);
                //Process proc;
                //proc.Start("C:\\Program Files (x86)\\Notepad++\\Notepad++", QStringList() << "file.xml" << "-n 2000");)
                result = false;
            }

            return result;               
        }

        public static void CreateToast(string TextInterface1, string TextInterface2, DeskNotify.NotifyType type)
        {
            DeskNotify deskNotifyWin = new DeskNotify(TextInterface1, TextInterface2, type);
            deskNotifyWin.Show();
        }
    }
}
