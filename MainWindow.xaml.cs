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
        private int oldShape = 25; // Used to know if we can redraw the cells in the new grid
        private bool run = false;
        private int delay = 125;
        private bool infiniteLoop = true;
        private string colorStartDead = "AntiqueWhite";
        private string colorStartAlive = "Gray";
        private SolidColorBrush colorAlive;
        private SolidColorBrush colorDead;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void playgroundLoaded(object sender, RoutedEventArgs e)
        {
            // Assign colors
            Color colorDeadBrush = (Color)ColorConverter.ConvertFromString(colorStartDead);
            colorDead = new SolidColorBrush(colorDeadBrush);
            Color colorAliveBrush = (Color)ColorConverter.ConvertFromString(colorStartAlive);
            colorAlive = new SolidColorBrush(colorAliveBrush);

            MakeGrid();

            //// Retrieve colors list and bind it with comboBoxes
            comboBoxDead.ItemsSource = typeof(Brushes).GetProperties();
            comboBoxDead.SelectedItem = typeof(Brushes).GetProperty(colorStartDead);

            comboBoxAlive.ItemsSource = typeof(Brushes).GetProperties();
            comboBoxAlive.SelectedItem = typeof(Brushes).GetProperty(colorStartAlive);
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
                    btn.BorderThickness = new Thickness(0.2);
                    btn.Click += new RoutedEventHandler(ActivateBtn);
                    Grid.SetColumn(btn, j);
                    Grid.SetRow(btn, k);
                    playground.Children.Add(btn);
                }
            }
        }

        private void ActivateBtn(Object sender, RoutedEventArgs e)
        {
            // Activate or deactivate cells on click
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
            // Save the old grid
            List<int> savedGrid = SaveGrid();

            // Retrieve the shape field
            int newShape = 0;
            bool isNumericShape = int.TryParse(tbShape.Text, out newShape);
            if (!isNumericShape)
            {
                tbShape.Text = oldShape.ToString();
                newShape = oldShape;
            }

            // Check if the shape has changed
            if (newShape != oldShape)
            {
                MessageBoxResult result = MessageBox.Show(
                    "If the shape has changed, \nthe cells can't not be redraw.",
                    "My App",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning,
                    MessageBoxResult.Cancel
                );

                switch (result)
                {
                    case MessageBoxResult.OK:
                        shape = newShape;
                        break;
                    case MessageBoxResult.Cancel:
                        shape = oldShape;
                        tbShape.Text = shape.ToString();
                        return;
                }
            }

            // Retrieve the colors
            Color colorDeadBrush = (Color)ColorConverter.ConvertFromString(comboBoxDead.Text.Substring(37));
            colorDead = new SolidColorBrush(colorDeadBrush);

            Color colorAliveBrush = (Color)ColorConverter.ConvertFromString(comboBoxAlive.Text.Substring(37));
            colorAlive = new SolidColorBrush(colorAliveBrush);

            // Clear the old gird content
            playground.Children.Clear();
            playground.RowDefinitions.Clear();
            playground.ColumnDefinitions.Clear();

            // Generate a new grid
            MakeGrid();
            if (newShape == oldShape)
            {
                RedrawGrid(savedGrid);
            }

            oldShape = newShape;
        }

        private List<int> SaveGrid()
        {
            List<int> savedGrid = new List<int>();
            for (int i = 0; i < playground.Children.Count; i++)
            {
                Button child = (Button)VisualTreeHelper.GetChild(playground, i);
                if (child.Background == colorAlive)
                {
                    savedGrid.Add(1);
                }
                else
                {
                    savedGrid.Add(0);
                }
            }

            return savedGrid;
        }

        private void RedrawGrid(List<int> savedGrid)
        {
            // Redraw odld cells
            for (int i = 0; i < playground.Children.Count; i++)
            {
                Button child = (Button)VisualTreeHelper.GetChild(playground, i);
                if (savedGrid[i] == 1)
                {
                    child.Background = colorAlive;
                }
                else
                {
                    child.Background = colorDead;
                }
            }
        }

        private void ClickGridClear(object sender, RoutedEventArgs e)
        {
            // Clear the old gird content
            playground.Children.Clear();
            playground.RowDefinitions.Clear();
            playground.ColumnDefinitions.Clear();

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

                // Stop if there are no more cells alive on the grid
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
            // Temporary list to store the next grid values
            List<int> temp = new List<int>();

            for (int i = 0; i < playground.Children.Count; i++)
            {
                // Calculate the total of neighbors alive
                List<int> neighbors = new List<int>();
                if (infiniteLoop)
                {
                    neighbors = FindNeighborsLoop(i);
                }
                else
                {
                    neighbors = FindNeighborsFinished(i);
                }
                int total = 0;
                foreach (int n in neighbors)
                {
                    if (n == -1 || n >= (oldShape * oldShape))
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

        /// <summary>
        /// Find the active cell's neighbors in a finished grid
        /// </summary>
        private List<int> FindNeighborsFinished(int boxNumber)
        {
            // TODO Improve it or remove it
            IDictionary<string, int> temp = new Dictionary<string, int>();
            temp.Add("top-left", boxNumber - (shape + 1));
            temp.Add("left", boxNumber - shape);
            temp.Add("bottom-left", boxNumber - (shape - 1));
            temp.Add("top", boxNumber - 1);
            temp.Add("bottom", boxNumber + 1);
            temp.Add("top-right", boxNumber + (shape - 1));
            temp.Add("right", boxNumber + shape);
            temp.Add("bottom-right", boxNumber + (shape + 1));

            // If return to the last line
            if (temp["top"] % shape == shape - 1 || temp["top-left"] % shape == shape - 1 || temp["top-right"] % shape == shape - 1)
            {
                temp["top-left"] = -1;
                temp["top"] = -1;
                temp["top-right"] = -1;
            }
            // If return to the first line
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

        /// <summary>
        /// Find the active cell's neighbors in an infinte loop
        /// </summary>
        private List<int> FindNeighborsLoop(int boxNumber)
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
