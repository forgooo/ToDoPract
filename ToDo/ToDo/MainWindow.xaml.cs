using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ToDoList
{
    public partial class MainWindow : Window
    {
        // Create an ObservableCollection to hold the tasks in the ToDo list.
        ObservableCollection<Task> tasks = new ObservableCollection<Task>();

        public MainWindow()
        {
            InitializeComponent();

            // Set the DataContext of the ItemsControl to the ObservableCollection.
            itemsControl.DataContext = tasks;
        }

        // Event handler for the "Add Item" button click event.
        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            string newItemName = txtNewItem.Text;
            if (!string.IsNullOrEmpty(newItemName))
            {
                Task newTask = new Task(newItemName);

                itemsControl.Items.Add(newTask);
                txtNewItem.Clear();
            }
        }

        private void btnDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Task task = button.DataContext as Task;
            tasks.Remove(task);
        }


        private void CreateNotification_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            StackPanel stackPanel = btn.Parent as StackPanel;
            Task task = stackPanel.DataContext as Task;

            ComboBox hoursComboBox = new ComboBox();
            for (int i = 0; i < 24; i++)
            {
                hoursComboBox.Items.Add(i.ToString("00"));
            }
            hoursComboBox.SelectedValue = task.Time.Hour.ToString("00");

            ComboBox minutesComboBox = new ComboBox();
            for (int i = 0; i < 60; i++)
            {
                minutesComboBox.Items.Add(i.ToString("00"));
            }
            minutesComboBox.SelectedValue = task.Time.Minute.ToString("00");

            Button goButton = new Button() { Content = "Go", Margin = new Thickness(5, 0, 5, 0) };

            TextBlock colon = new TextBlock() { Text = ":", Margin = new Thickness(5, 0, 5, 0) };

            stackPanel.Children.Remove(btn);
            stackPanel.Children.Add(hoursComboBox);
            stackPanel.Children.Add(colon);
            stackPanel.Children.Add(minutesComboBox);
            stackPanel.Children.Add(goButton);

            goButton.Click += (s, args) =>
            {
                if (hoursComboBox.SelectedValue != null && minutesComboBox.SelectedValue != null)
                {
                    task.Time = new DateTime(task.Time.Year, task.Time.Month, task.Time.Day, int.Parse(hoursComboBox.SelectedValue.ToString()), int.Parse(minutesComboBox.SelectedValue.ToString()), 0);

                    string selectedHours = hoursComboBox.SelectedValue.ToString();
                    string selectedMinutes = minutesComboBox.SelectedValue.ToString();
                    string TIME = selectedHours + ":" + selectedMinutes;

                    // Get the task text
                    TextBlock taskText = (TextBlock)stackPanel.FindName("TaskText");
                    string taskName = taskText.Text;

                    SendNotification(taskName, GetSecondsDifference(TIME));

                    

                    // Get the task text
                    

                    // Refresh the binding to update the ItemsControl
                    BindingExpression bindingExpr = itemsControl.GetBindingExpression(ItemsControl.ItemsSourceProperty);
                    if (bindingExpr != null)
                    {
                        bindingExpr.UpdateTarget();
                    }

                    // Hide the ComboBoxes and Go button
                    stackPanel.Children.Remove(hoursComboBox);
                    stackPanel.Children.Remove(colon);
                    stackPanel.Children.Remove(minutesComboBox);
                    stackPanel.Children.Remove(goButton);

                    // Show the Create Notification button again
                    stackPanel.Children.Add(btn);
                }
                else
                {
                    MessageBox.Show("Please select a valid time.", "Invalid Time", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            };
        }


        public static int GetSecondsDifference(string timeStr)
        {
            // Parse the time string into hours and minutes
            int hours = int.Parse(timeStr.Split(':')[0]);
            int minutes = int.Parse(timeStr.Split(':')[1]);

            // Get the current time and the desired time
            DateTime currentTime = DateTime.Now;
            DateTime targetTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, hours, minutes, 0);

            // Calculate the difference in seconds
            int secondsDifference = (int)(targetTime - currentTime).TotalSeconds;

            return secondsDifference;
        }



        public static void SendNotification(string message, int delayInSeconds)
        {
            // Convert delay to milliseconds
            int delayInMilliseconds = delayInSeconds * 1000;

            // Create a new timer with the specified delay
            var timer = new Timer(
                state =>
                {
                    // This code executes when the timer elapses
                    MessageBox.Show("I remind you to " + message);
                },
                null, // No state object needed in this case
                delayInMilliseconds, // Delay before the first timer event
                Timeout.Infinite // Don't repeat the timer
            );
        }

        private void invisibleButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Rectangle check = (Rectangle)btn.FindName("check");

            if (check.Opacity == 0)
            {
                // Add the image
                Image img = new Image();
                img.Source = new BitmapImage(new Uri("image.png", UriKind.Relative));
                img.Margin = new Thickness(10);
                ((StackPanel)check.Parent).Children.Add(img);
                check.Opacity = 1;
            }
            else
            {

                // Remove the image
                ((StackPanel)check.Parent).Children.Remove(((StackPanel)check.Parent).Children.OfType<Image>().FirstOrDefault());
                check.Opacity = 0;
            }
        }

        private void btnToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            // If the current theme is dark, switch to light mode
            if (this.Background == Brushes.Black)
            {
                this.Background = Brushes.White;
                this.Foreground = Brushes.Black;
            }
            // Otherwise, switch to dark mode
            else
            {
                this.Background = Brushes.Black;
                this.Foreground = Brushes.White;
            }
            this.NavigationService.Refresh();
        }


    }



    // Custom class to represent a task in the ToDo list.
    public class Task
    {
        public string Name { get; set; }
        public DateTime Time { get; set; }

        public Task(string name)
        {
            Name = name;
            Time = DateTime.Now;
        }
    }

}
