using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace OpenCLforNetSample.TemplateMatching
{
    class MainWindowModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName]string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        #region MVVM Property


        private bool _Calculating;
        public bool Calculating
        {
            get => _Calculating;
            set
            {
                if (_Calculating == value) return;
                _Calculating = value;
                RaisePropertyChanged();
            }
        }


        private Bitmap _Source;
        public Bitmap Source
        {
            get => _Source;
            set
            {
                if (_Source == value) return;
                _Source = value;
                RaisePropertyChanged();
                TryTemplateMatching();
            }
        }

        private Bitmap _Template;
        public Bitmap Template
        {
            get => _Template;
            set
            {
                if (_Template == value) return;
                _Template = value;
                RaisePropertyChanged();
                TryTemplateMatching();
            }
        }


        private double _Threashold;
        public double Threashold
        {
            get => _Threashold;
            set
            {
                if (_Threashold == value) return;
                _Threashold = value;
                RaisePropertyChanged();
                TryTemplateMatching();
            }
        }


        private CanvasSizeChanger _SourcePreview;
        public CanvasSizeChanger SourcePreview
        {
            get => _SourcePreview;
            set
            {
                if (_SourcePreview == value) return;
                _SourcePreview = value;
                RaisePropertyChanged();
            }
        }

        private CanvasSizeChanger _TemplatePreview;
        public CanvasSizeChanger TemplatePreview
        {
            get => _TemplatePreview;
            set
            {
                if (_TemplatePreview == value) return;
                _TemplatePreview = value;
                RaisePropertyChanged();
            }
        }

        private CanvasSizeChanger _ResultView;
        public CanvasSizeChanger ResultView
        {
            get => _ResultView;
            set
            {
                if (_ResultView == value) return;
                _ResultView = value;
                RaisePropertyChanged();
            }
        }


        private ObservableCollection<Rect> _Rects;
        public ObservableCollection<Rect> Rects
        {
            get => _Rects;
            set
            {
                if (_Rects == value) return;
                _Rects = value;
                RaisePropertyChanged();
            }
        }


        private double _ExecutionTime;
        public double ExecutionTime
        {
            get => _ExecutionTime;
            set
            {
                if (_ExecutionTime == value) return;
                _ExecutionTime = value;
                RaisePropertyChanged();
            }
        }


        private string _WhereSourceFromTxt;
        public string WhereSourceFromTxt
        {
            get => _WhereSourceFromTxt;
            set
            {
                if (_WhereSourceFromTxt == value) return;
                _WhereSourceFromTxt = value;
                RaisePropertyChanged();
            }
        }


        private string _WhereTemplateFromTxt;
        public string WhereTemplateFromTxt
        {
            get => _WhereTemplateFromTxt;
            set
            {
                if (_WhereTemplateFromTxt == value) return;
                _WhereTemplateFromTxt = value;
                RaisePropertyChanged();
            }
        }



        #endregion

        #region MVVM Command Prooperty

        public ICommand LoadSourceFromFileAction { get; }
        public ICommand LoadSourceFromDesktopAction { get; }
        public ICommand LoadTemplateFromFileAction { get; }
        public ICommand LoadTemplateFromDesktopAction { get; }


        #endregion

        public MainWindowModel()
        {
            LoadSourceFromFileAction = new Command(LoadSourceFromFile);
            LoadSourceFromDesktopAction = new Command(LoadSourceFromDesktop);
            LoadTemplateFromFileAction = new Command(LoadTemplateFromFile);
            LoadTemplateFromDesktopAction = new Command(LoadTemplateFromDesktop);

            Threashold = 0.8;

            SourcePreview = new CanvasSizeChanger(this, nameof(Source));
            TemplatePreview = new CanvasSizeChanger(this, nameof(Template));
            ResultView = new CanvasSizeChanger(this, nameof(Source));

            Rects = new ObservableCollection<Rect>();
        }


        public void LoadSourceFromFile()
        {
            var savingFilePath = DoOnUiThread(() =>
            {

                using (var openDialog = new System.Windows.Forms.OpenFileDialog())
                {
                    openDialog.Filter = "Image file|*.bmp;*.png;*.jpg;*.jpeg";
                    openDialog.DefaultExt = "piksav";
                    openDialog.AddExtension = true;
                    return (openDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) ?
                        openDialog.FileName : null;
                }
            });

            if (!String.IsNullOrEmpty(savingFilePath))
            {
                Source = new Bitmap(savingFilePath);
                WhereSourceFromTxt = $"from file '{Path.GetFileName(savingFilePath)}'";
            }
        }
        public void LoadSourceFromDesktop()
        {
            Source = DesktopCaptureUtil.TakeScreensoht();
            WhereSourceFromTxt = "from desktop";
        }

        public void LoadTemplateFromFile()
        {
            var savingFilePath = DoOnUiThread(() =>
            {

                using (var openDialog = new System.Windows.Forms.OpenFileDialog())
                {
                    openDialog.Filter = "Image file|*.bmp;*.png;*.jpg;*.jpeg";
                    openDialog.DefaultExt = "piksav";
                    openDialog.AddExtension = true;
                    return (openDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) ?
                        openDialog.FileName : null;
                }
            });

            if (!String.IsNullOrEmpty(savingFilePath))
            {
                Template = new Bitmap(savingFilePath);
                WhereTemplateFromTxt = $"from file '{Path.GetFileName(savingFilePath)}'";
            }
        }
        public void LoadTemplateFromDesktop()
        {
            Template = DesktopCaptureUtil.ClipImageFromDesktop();
            WhereTemplateFromTxt = "from desktop";
        }

        public void TryTemplateMatching()
        {
            if (Source is null) return;
            if (Template is null) return;
            Calculating = true;

            Task.Run(() =>
            {
                try
                {
                    var timer = Stopwatch.StartNew();

                    using (var matcher = new TemplateMatching())
                    {
                        matcher.Source = Source;
                        matcher.Template = Template;
                        matcher.Threashold = (float)Threashold;

                        var ans = matcher.Find();

                        DoOnUiThread(() =>
                        {
                            Rects.Clear();
                            foreach (var rect in ans.Select(pnt => new Rect(pnt.J, pnt.I, Template.Width, Template.Height)))
                            {
                                Rects.Add(rect);
                            }
                        });
                    }
                    timer.Stop();

                    ExecutionTime = (timer.ElapsedMilliseconds / 10) / 100d;
                }
                finally
                {
                    Calculating = false;
                }
            });
        }


        private static void DoOnUiThread(Action callback) => DoOnUiThread<object>(() => { callback(); return null; });

        private static V DoOnUiThread<V>(Func<V> callback)
        {
            var dispatcher = Application.Current.Dispatcher;
            if (Thread.CurrentThread == dispatcher.Thread)
            {
                // UIスレッド上ならそのまま実行
                return callback();
            }
            else
            {
                return dispatcher.Invoke(callback);
            }
        }
    }

    class CanvasSizeChanger : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName]string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ArgCommand<SizeChangedEventArgs> SizeChangedAction { get; }

        private int imgWid;
        private int imgHei;


        private double _Scale;
        public double Scale
        {
            get => _Scale;
            set
            {
                if (_Scale == value) return;
                _Scale = value;
                RaisePropertyChanged();
            }
        }

        private double canvasWid;
        public double CanvasWidth
        {
            get => canvasWid;
            set
            {
                if (canvasWid == value) return;
                canvasWid = value;
                RaisePropertyChanged();
                ChangeScale();
            }
        }


        private double canvasHei;
        public double CanvasHeight
        {
            get => canvasHei;
            set
            {
                if (canvasHei == value) return;
                canvasHei = value;
                RaisePropertyChanged();
                ChangeScale();
            }
        }


        public CanvasSizeChanger(MainWindowModel owner, string path)
        {
            owner.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == path)
                {
                    var prop = owner.GetType().GetProperty(path);
                    ImageChanged((Bitmap)prop.GetValue(owner));
                }
            };
        }

        private void ImageChanged(Bitmap image)
        {
            if (image is null)
            {
                imgWid = 0;
                imgHei = 0;
            }
            else
            {
                imgWid = image.Width;
                imgHei = image.Height;
            }
            ChangeScale();
        }


        private void ChangeScale()
        {
            if (canvasWid == 0 || canvasHei == 0)
            {
                Scale = 1;
            }
            else
            {
                Scale = Math.Min(canvasWid / imgWid, canvasHei / imgHei);
            }
        }
    }

    class ArgCommand<T> : ICommand
    {
        private readonly Action<T> execute;
        public ArgCommand(Action<T> execute)
        {
            this.execute = execute;
        }
        public void Execute(object parameter) => execute((T)parameter);
        public bool CanExecute(object parameter) => true;

        public event EventHandler CanExecuteChanged;
    }

    class Command : ICommand
    {
        private readonly Action execute;
        public Command(Action execute)
        {
            this.execute = execute;
        }
        public void Execute(object parameter) => execute();
        public bool CanExecute(object parameter) => true;

        public event EventHandler CanExecuteChanged;
    }
}
