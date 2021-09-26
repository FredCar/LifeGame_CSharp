using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LifeGame
{
    public partial class MainWindow : Window
    {
        private int shape = 25;
        private bool run = false;  
        private int delay = 125;
        private bool infiniteLoop = true;
        private SolidColorBrush colorAlive = Brushes.Gray;
        private SolidColorBrush colorDead = Brushes.AntiqueWhite;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void playgroundLoaded(object sender, RoutedEventArgs e)
        {
            MakeGrid();
        }

        private void MakeGrid()
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

            // Define the buttons
            foreach (int j in Enumerable.Range(0, shape))
            {
                foreach (int k in Enumerable.Range(0, shape))
                {
                    Button btn = new Button();
                    btn.Background = colorDead;
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
            if (clickedButton.Background == colorDead)
            {
                clickedButton.Background = colorAlive;
            }
            else
            {
                clickedButton.Background = colorDead;
            }
        }

        private void ClickGridRefresh(object sender, RoutedEventArgs e)
        {
            // Retrieve the shape field
            bool isNumericShape = int.TryParse(tbShape.Text, out shape);
            if (!isNumericShape)
            {
                tbShape.Text = "25";
                shape = 25;
            }

            // Retrieve the colors
            Color colorDeadBrush = (Color)ColorConverter.ConvertFromString(comboBoxDead.Text);
            SolidColorBrush brushDead = new SolidColorBrush(colorDeadBrush);
            colorDead = brushDead;

            Color colorAliveBrush = (Color)ColorConverter.ConvertFromString(comboBoxAlive.Text);
            SolidColorBrush brushAlive = new SolidColorBrush(colorAliveBrush);
            colorAlive = brushAlive;

            // Clear the old gird content
            playground.Children.Clear();
            playground.RowDefinitions.Clear();
            playground.ColumnDefinitions.Clear();

            // Generate a new grid
            MakeGrid();
        }

        private void startClick(object sender, RoutedEventArgs e)
        {
            // Retrieve the speed field
            bool isNumericSpeed = int.TryParse(tbSpeed.Text, out delay);
            if (!isNumericSpeed)
            {
                tbSpeed.Text = "125";
                delay = 125;
            }
            if (delay <= 0)
            {
                tbSpeed.Text = "1";
                delay = 1;
            }

            // Retrieve ifinite loop checkbox value
            infiniteLoop = (bool)checkBoxInfinite.IsChecked;

            if (run)
            {
                run = false;
                btnStart.Content = "Start";
                btnStart.Background = Brushes.Green;
            }
            else
            {
                run = true;
                btnStart.Content = "Stop";
                btnStart.Background = Brushes.Red;
            }

            Play();
        }

        private async void Play()
        {
            while (run)
            {
                ControlGrid();
                await Task.Delay(delay);
                if (!ThereIsLife())
                {
                    run = false;
                    btnStart.Content = "Start";
                    btnStart.Background = Brushes.Green;
                }
            }
        }

        private bool ThereIsLife()
        {
            // Check if there are any living cells
            for (int i = 0; i < playground.Children.Count; i++)
            {
                Button child = (Button)VisualTreeHelper.GetChild(playground, i);
                if (child.Background == colorAlive)
                {
                    return true;
                }
            }
            return false;
        }

        private void ControlGrid()
        {
            // Temporary list to store the new grid values
            List<int> temp = new List<int>();

            for (int i = 0; i < playground.Children.Count; i++)
            {
                // Calculate the total of neighbors alive
                List<int> neighbors = new List<int>();
                if (infiniteLoop)
                {
                    neighbors = FindNeighborsInfinite(i);
                }
                else
                {
                    neighbors = FindNeighborsFinished(i);
                }
                int total = 0;
                foreach (int n in neighbors)
                {
                    if (n == -1)
                    {
                        continue;
                    }
                    Button neighbor = (Button)VisualTreeHelper.GetChild(playground, n);
                    if (neighbor.Background == colorAlive)
                    {
                        total++;
                    }
                }

                // Apply the base rules
                Button child = (Button) VisualTreeHelper.GetChild(playground, i);
                if (child.Background == colorAlive)
                {
                    if (total == 2 || total == 3)
                    {
                        temp.Add(1);
                    }
                    else
                    {
                        temp.Add(0);
                    }
                }
                else
                {
                    if (total == 3)
                    {
                        temp.Add(1);
                    }
                    else
                    {
                        temp.Add(0);
                    }
                }
            }

            // Recompose a new grid from the temporary list
            foreach (int i in Enumerable.Range(0, playground.Children.Count))
            {
                Button child = (Button)VisualTreeHelper.GetChild(playground, i);
                if (temp[i] == 1)
                {
                    child.Background = colorAlive;
                }
                else
                {
                    child.Background = colorDead;
                }
            }
        }

        private List<int> FindNeighborsFinished(int boxNumber)
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

            // If return to the last line, then shift 1 to the right
            if (temp["top"] % shape == shape - 1 || temp["top-left"] % shape == shape - 1 || temp["top-right"] % shape == shape - 1)
            {
                temp["top-left"] = -1;
                temp["top"] = -1;
                temp["top-right"] = -1;
            }
            // If return to the first line, then shift 1 to the left
            if (temp["bottom"] % shape == 0 || temp["bottom-left"] % shape == 0 || temp["bottom-right"] % shape == 0)
            {
                temp["bottom-left"] = -1;
                temp["bottom"] = -1;
                temp["bottom-right"] = -1;
            }

            List<int> neighbors = new List<int>();
            foreach (int n in temp.Values)
            {
                if (n < 0 || n > (shape * shape - 1))
                {
                    neighbors.Add(-1);
                }
                else
                {
                    neighbors.Add(n);
                }
            }

            return neighbors;
        }

        private List<int> FindNeighborsInfinite(int boxNumber)
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

            // If boxNumber is on the first column
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
            // If boxNumber is on the last column
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

            // If return to the last line, then shift 1 to the right
            if (temp["top"] % shape == shape - 1 || temp["top-left"] % shape == shape - 1 || temp["top-right"] % shape == shape - 1)
            {
                temp["top-left"] += shape;
                temp["top"] += shape;
                temp["top-right"] += shape;
            }
            // If return to the first line, then shift 1 to the left
            if (temp["bottom"] % shape == 0 || temp["bottom-left"] % shape == 0 || temp["bottom-right"] % shape == 0)
            {
                temp["bottom-left"] -= shape;
                temp["bottom"] -= shape;
                temp["bottom-right"] -= shape;
            }

            // Adjust corner returns
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
