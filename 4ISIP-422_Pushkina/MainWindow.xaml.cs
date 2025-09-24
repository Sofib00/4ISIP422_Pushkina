using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using System.Xml.Linq;

namespace _4ISIP_422_Pushkina
{
    public class Product
    {
        private static int _counter = 1;

        public int Code { get; private set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }
        public bool InStock => Quantity > 0;

        public Product(string name, decimal price, int quantity, string category)
        {
            Code = _counter++;
            Name = name;
            Price = price;
            Quantity = quantity;
            Category = category;
        }

        public void Sell(int amount)
        {
            if (Quantity >= amount)
                Quantity -= amount;
            else
                throw new Exception("Недостаточно товара на складе!");
        }

        public void Restock(int amount)
        {
            Quantity += amount;
        }

        public override string ToString()
        {
            return $"[{Code}] {Name} | {Category} | Цена: {Price} | Кол-во: {Quantity} | {(InStock ? "В наличии" : "Нет на складе")}";
        }
    }

    public partial class MainWindow : Window
    {
        private ObservableCollection<Product> products;

        public MainWindow()
        {
            InitializeComponent();

            products = new ObservableCollection<Product>
            {
                new Product("Хлеб", 40, 10, "Продукты"),
                new Product("Молоко", 60, 5, "Продукты"),
                new Product("Телефон", 25000, 2, "Электроника"),
                new Product("Куртка", 3500, 7, "Одежда"),
                new Product("Чай", 120, 0, "Продукты")
            };

            ProductsList.ItemsSource = products;
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var newProduct = new Product("Новый товар", 100, 1, "Продукты");
            products.Add(newProduct);
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsList.SelectedItem is Product product)
                products.Remove(product);
        }

        private void RestockProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsList.SelectedItem is Product product)
            {
                product.Restock(5);
                ProductsList.Items.Refresh();
            }
        }

        private void SellProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsList.SelectedItem is Product product)
            {
                try
                {
                    product.Sell(1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                ProductsList.Items.Refresh();
            }
        }

        private void SearchProduct_Click(object sender, RoutedEventArgs e)
        {
            string input = ShowInputDialog("Введите название/код/категорию:", "Поиск товара");
            if (string.IsNullOrWhiteSpace(input))
                return;

            input = input.Trim();

            var found = products.FirstOrDefault(p =>
                (!string.IsNullOrEmpty(p.Name) && p.Name.IndexOf(input, StringComparison.OrdinalIgnoreCase) >= 0) ||
                p.Code.ToString() == input ||
                (!string.IsNullOrEmpty(p.Category) && p.Category.IndexOf(input, StringComparison.OrdinalIgnoreCase) >= 0));

            if (found != null)
                MessageBox.Show(found.ToString(), "Результат поиска");
            else
                MessageBox.Show("Товар не найден!");
        }

        private string ShowInputDialog(string text, string title)
        {
            var wnd = new Window
            {
                Title = title,
                Width = 360,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Owner = this
            };
            var panel = new StackPanel { Margin = new Thickness(10) };
            panel.Children.Add(new TextBlock { Text = text });
            var tb = new TextBox { Margin = new Thickness(0, 5, 0, 10) };
            panel.Children.Add(tb);

            var btnPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            var ok = new Button { Content = "OK", Width = 75, IsDefault = true, Margin = new Thickness(0, 0, 5, 0) };
            var cancel = new Button { Content = "Отмена", Width = 75, IsCancel = true };
            btnPanel.Children.Add(ok); btnPanel.Children.Add(cancel);
            panel.Children.Add(btnPanel);

            wnd.Content = panel;

            string result = null;
            ok.Click += (s, e) => { result = tb.Text; wnd.DialogResult = true; wnd.Close(); };

            bool? dr = wnd.ShowDialog();
            return dr == true ? result : null;
        }
    }
}
