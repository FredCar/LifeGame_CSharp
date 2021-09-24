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
        private int shape = 5;

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
                    btn.Click += new RoutedEventHandler(ActivateBtn);
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
            //textBoxInfo.Text = "Hello world !!";
            //controlGrid();

            //DEBUG
            int nb = 24;
            Button test = (Button)VisualTreeHelper.GetChild(playground, nb);
            test.Background = Brushes.Green;

            List<int> neighbors = FindNeighbors(nb);
            foreach (int n in neighbors)
            {
                try
                {
                    Button neighbor = (Button)VisualTreeHelper.GetChild(playground, n);
                    neighbor.Background = Brushes.Red;
                }
                catch
                {
                    textBoxInfo.Text += n.ToString();
                }
            }
        }

        private void controlGrid()
        {
            for (int i = 0; i < playground.Children.Count; i++)
            {
                List<int> neighbors = FindNeighbors(i);
                int total = 0;
                foreach (int n in neighbors)
                {
                    Button neighbor = (Button)VisualTreeHelper.GetChild(playground, n);
                    if (neighbor.Background == Brushes.Gray)
                    {
                        total++;
                    }
                }

                //int total = neighbors.Sum(x => Convert.ToInt32(x));
                //Console.WriteLine(">>> " + total);

                Button child = (Button) VisualTreeHelper.GetChild(playground, i);
                //TODO stocker dans un tableau
                child.Background = Brushes.Gray;
                //Console.WriteLine(VisualTreeHelper.GetChild(playground, i));

            }

            //System.Collections.IEnumerator box = playground.Children.GetEnumerator();
            //while (box.MoveNext())
            //{
            //    Object blok = box.Current;
            //    textBoxInfo.Text = blok.ToString();
            //}
        }

        private List<int> FindNeighbors(int boxNumber)
        {
            IDictionary<string, int> temp = new Dictionary<string, int>();
            temp.Add("top-left", boxNumber - (shape + 1));
            temp.Add("left", boxNumber - shape);
            temp.Add("bottom-left", boxNumber - (shape - 1));
            temp.Add("top", boxNumber - 1);
            temp.Add("bottom", boxNumber + 1);
            temp.Add("top-right", boxNumber + (shape - 1));
            temp.Add("right", boxNumber + shape);
            temp.Add("bottom-right", boxNumber + (shape + 1));

            // Si boxNumber est sur la 1ere colonne
            if (temp["top-left"] < 0)
            {
                temp["top-left"] = shape * shape + temp["top-left"];
            }
            if (temp["left"] < 0)
            {
                temp["left"] = shape * shape + temp["left"];
            }
            if (temp["bottom-left"] < 0)
            {
                temp["bottom-left"] = shape * shape + temp["bottom-left"];
            }
            // Si boxNumber est sur la derniere colonne
            if (temp["top-right"] >= shape * shape)
            {
                temp["top-right"] = temp["top-right"] - shape * shape;
            }
            if (temp["right"] >= shape * shape)
            {
                temp["right"] = temp["right"] - shape * shape;
            }
            if (temp["bottom-right"] >= shape * shape)
            {
                temp["bottom-right"] = temp["bottom-right"] - shape * shape;
            }

            // Si retour a la derniere ligne, alors décaler de 1 vers la doite
            if (temp["top"] % shape == shape - 1 || temp["top-left"] % shape == shape - 1 || temp["top-right"] % shape == shape - 1)
            {
                temp["top-left"] += shape;
                temp["top"] += shape;
                temp["top-right"] += shape;
            }
            // Si retour a la premiere ligne, alors décaler de 1 vers la gauche
            if (temp["bottom"] % shape == 0 || temp["bottom-left"] % shape == 0 || temp["bottom-right"] % shape == 0)
            {
                temp["bottom-left"] -= shape;
                temp["bottom"] -= shape;
                temp["bottom-right"] -= shape;
            }

            // Ajuster les retours de coins
            if (temp["top-left"] > shape * shape)
            {
                temp["top-left"] = temp["top-left"] - shape * shape;
            }
            if (temp["top-right"] > shape * shape)
            {
                temp["top-right"] = temp["top-right"] - shape * shape;
            }
            if (temp["bottom-left"] < 0)
            {
                temp["bottom-left"] = shape * shape - shape;
            }
            if (temp["bottom-right"] < 0)
            {
                temp["bottom-right"] = shape * shape - shape;
            }

            List<int> neighbors = new List<int>();
            foreach (int n in temp.Values)
            {
                neighbors.Add(n);
            }

            return neighbors;
        }
    }
}
