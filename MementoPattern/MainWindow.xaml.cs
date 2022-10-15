using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Image = System.Windows.Controls.Image;

namespace MementoPattern
{

    public class Originator
    {
        private string _state;

        public Originator(string state)
        {
            _state = state;
        }

        public void DoSomething(string filename)
        {
            this._state = filename;
        }
        public IMemento Save()
        {
            return new TextMemento(this._state);
        }

        public void Restore(IMemento memento)
        {
            if (!(memento is TextMemento))
            {
                throw new Exception("Unknown memento class " + memento.ToString());
            }
            this._state = memento.GetState();
        }
    }
    public interface IMemento
    {
        string GetState();
    }
    public class TextMemento : IMemento
    {
        private string _state;
        private DateTime _date;
        public TextMemento(string state)
        {
            this._state = state;
            this._date = DateTime.Now;
        }

        public string GetState()
        {
            return _state;
        }
    }
    public class CareTaker
    {
        public List<IMemento> _mementos = new List<IMemento>();
        private Originator _originator = null;

        public CareTaker(Originator originator)
        {
            _originator = originator;
        }

        public void Undo()
        {
            if (_mementos.Count == 0)
            {
                return;
            }

            //var memento = _mementos.Last();
            //this._mementos.Remove(memento);
            var memento = _mementos[Index];
            Index--;
            try
            {
                _originator.Restore(memento);
            }
            catch (Exception)
            {
                this.Undo();
            }

        }
        public void Redo()
        {
            var memento = _mementos[Index];
            Index++;
        }
        public int Index { get; set; } = -1;
        public void BackUp()
        {
            this._mementos.Add(_originator.Save());
            Index++;
        }

    }


    public partial class MainWindow : Window
    {
        public Originator originator { get; set; }
        public CareTaker careTaker { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            originator = new Originator(string.Empty);
            careTaker = new CareTaker(originator);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String filename = "ScreenCapture-" + DateTime.Now.ToString("ddMMyyyy-hhmmss") + ".png";
            Bitmap bm = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics g = Graphics.FromImage(bm);
            g.CopyFromScreen(0, 0, 0, 0, bm.Size);
            filename = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + filename;
            bm.Save(filename);

            Image finalImage = new Image();
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri(filename);
            logo.EndInit();
            finalImage.Source = logo;
            ScreenImage.Source = logo;
            originator.DoSomething(filename);
            careTaker.BackUp();
        }

        private void RedoBtn_Click(object sender, RoutedEventArgs e)
        {
            careTaker.Redo();
            try
            {
                Image finalImage = new Image();
                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                logo.UriSource = new Uri(careTaker._mementos[careTaker.Index].GetState());
                logo.EndInit();
                finalImage.Source = logo;
                ScreenImage.Source = logo;
            }
            catch (Exception)
            {

            }
        }

        private void UndoBtn_Click(object sender, RoutedEventArgs e)
        {
            careTaker.Undo();

            try
            {
                Image finalImage = new Image();
                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                logo.UriSource = new Uri(careTaker._mementos[careTaker.Index].GetState());
                logo.EndInit();
                finalImage.Source = logo;
                ScreenImage.Source = logo;
            }
            catch (Exception)
            {

            }

        }
    }
}
