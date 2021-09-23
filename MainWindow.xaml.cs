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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LifeGame
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int shape = 25;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void playgroundLoaded(object sender, RoutedEventArgs e)
        {
            foreach (int i in Enumerable.Range(0, shape))
            {
                // Define the Columns
                ColumnDefinition colDef = new ColumnDefinition();
                playground.ColumnDefinitions.Add(colDef);

                // Define the Rows
                RowDefinition rowDef = new RowDefinition();
                playground.RowDefinitions.Add(rowDef);
            }

            foreach (int j in Enumerable.Range(0, shape))
            {
                foreach (int k in Enumerable.Range(0, shape))
                {
                    // Define the buttons
                    Button btn = new Button();
                    btn.Background = Brushes.AntiqueWhite;
                    btn.Click += new RoutedEventHandler(this.ActivateBtn);
                    Grid.SetColumn(btn, j);
                    Grid.SetRow(btn, k);
                    playground.Children.Add(btn);
                }
            }
        }

        private void ActivateBtn(Object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            clickedButton.Background = Brushes.Gray;
        }

        private void startClick(object sender, RoutedEventArgs e)
        {
            textBoxInfo.Text = "Hello world !!";
            controlGrid();
        }

        private void controlGrid()
        {
            for (int i=0; i < playground.Children.Count; i++)
            {
                Button child = (Button) VisualTreeHelper.GetChild(playground, i);
                //TODO stocker dans un tableau
                child.Background = Brushes.Gray;
                Console.WriteLine(VisualTreeHelper.GetChild(playground, i));

            }
            System.Collections.IEnumerator box = playground.Children.GetEnumerator();
            while (box.MoveNext())
            {
                Object blok = box.Current;
                textBoxInfo.Text = blok.ToString();
            }
        }
    }
}
