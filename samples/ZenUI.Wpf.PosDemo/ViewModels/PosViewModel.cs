using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;

using Prism.Commands;
using Prism.Mvvm;

using ZenUI.Wpf.Controls;

namespace ZenUI.Wpf.PosDemo.ViewModels
{
    public sealed class PosViewModel : BindableBase
    {
        private readonly ProductViewModel[] products;
        private string statusMessage;
        private AlertVariant statusVariant;
        private Visibility statusVisibility = Visibility.Collapsed;

        public PosViewModel()
        {
            StoreName = "湖滨路演示门店";
            CashierName = "演示收银员";
            ShiftLabel = "早班 · 08:00—16:00";
            WelcomeText = "欢迎回来，今天也要保持好心情。";
            OrderNumber = "订单号 ZP-20260723-0088";

            Categories = new ObservableCollection<CategoryViewModel>
            {
                new CategoryViewModel("全部", ButtonAppearance.Filled),
                new CategoryViewModel("咖啡", ButtonAppearance.Text),
                new CategoryViewModel("茶饮", ButtonAppearance.Text),
                new CategoryViewModel("烘焙", ButtonAppearance.Text),
                new CategoryViewModel("甜点", ButtonAppearance.Text)
            };

            products = new[]
            {
                new ProductViewModel("美式咖啡", "美", "咖啡", 22m),
                new ProductViewModel("拿铁咖啡", "拿", "咖啡", 28m),
                new ProductViewModel("燕麦澳白", "澳", "咖啡", 32m),
                new ProductViewModel("茉莉绿茶", "茉", "茶饮", 18m),
                new ProductViewModel("葡萄冰茶", "葡", "茶饮", 26m),
                new ProductViewModel("黄油可颂", "颂", "烘焙", 16m),
                new ProductViewModel("海盐贝果", "贝", "烘焙", 20m),
                new ProductViewModel("松露蛋糕", "糕", "甜点", 36m)
            };

            VisibleProducts = new ObservableCollection<ProductViewModel>(products);
            OrderItems = new ObservableCollection<OrderItemViewModel>
            {
                new OrderItemViewModel(products[1], 1),
                new OrderItemViewModel(products[7], 1)
            };

            AddProductCommand = new DelegateCommand<ProductViewModel>(AddProduct, product => product != null);
            SelectCategoryCommand = new DelegateCommand<CategoryViewModel>(SelectCategory, category => category != null);
            ClearOrderCommand = new DelegateCommand(ClearOrder);
            CheckoutCommand = new DelegateCommand(Checkout);
            ExitCommand = new DelegateCommand(Exit);
        }

        public string StoreName { get; }

        public string CashierName { get; }

        public string ShiftLabel { get; }

        public string WelcomeText { get; }

        public string OrderNumber { get; }

        public ObservableCollection<CategoryViewModel> Categories { get; }

        public ObservableCollection<ProductViewModel> VisibleProducts { get; }

        public ObservableCollection<OrderItemViewModel> OrderItems { get; }

        public DelegateCommand<ProductViewModel> AddProductCommand { get; }

        public DelegateCommand<CategoryViewModel> SelectCategoryCommand { get; }

        public DelegateCommand ClearOrderCommand { get; }

        public DelegateCommand CheckoutCommand { get; }

        public DelegateCommand ExitCommand { get; }

        public int ItemCount
        {
            get { return OrderItems.Sum(item => item.Quantity); }
        }

        public decimal OrderTotal
        {
            get { return OrderItems.Sum(item => item.LineTotal); }
        }

        public string StatusMessage
        {
            get { return statusMessage; }
            private set { SetProperty(ref statusMessage, value); }
        }

        public AlertVariant StatusVariant
        {
            get { return statusVariant; }
            private set { SetProperty(ref statusVariant, value); }
        }

        public Visibility StatusVisibility
        {
            get { return statusVisibility; }
            private set { SetProperty(ref statusVisibility, value); }
        }

        private void AddProduct(ProductViewModel product)
        {
            var existingItem = OrderItems.FirstOrDefault(item => ReferenceEquals(item.Product, product));
            if (existingItem == null)
            {
                OrderItems.Add(new OrderItemViewModel(product, 1));
            }
            else
            {
                existingItem.Quantity++;
            }

            RefreshOrderSummary();
            ShowStatus(product.Name + " 已加入订单。", AlertVariant.Success);
        }

        private void SelectCategory(CategoryViewModel selectedCategory)
        {
            foreach (var category in Categories)
            {
                category.Appearance = ReferenceEquals(category, selectedCategory)
                    ? ButtonAppearance.Filled
                    : ButtonAppearance.Text;
            }

            VisibleProducts.Clear();
            var filteredProducts = selectedCategory.Name == "全部"
                ? products
                : products.Where(product => product.Category == selectedCategory.Name).ToArray();

            foreach (var product in filteredProducts)
            {
                VisibleProducts.Add(product);
            }
        }

        private void ClearOrder()
        {
            OrderItems.Clear();
            RefreshOrderSummary();
            ShowStatus("订单已清空（模拟操作）。", AlertVariant.Info);
        }

        private void Checkout()
        {
            if (OrderItems.Count == 0)
            {
                ShowStatus("请先添加商品。", AlertVariant.Warning);
                return;
            }

            ShowStatus(
                string.Format(
                    CultureInfo.CurrentCulture,
                    "模拟支付成功，共收款 ¥ {0:N2}。",
                    OrderTotal),
                AlertVariant.Success);
        }

        private static void Exit()
        {
            Application.Current.Shutdown();
        }

        private void RefreshOrderSummary()
        {
            RaisePropertyChanged(nameof(ItemCount));
            RaisePropertyChanged(nameof(OrderTotal));
        }

        private void ShowStatus(string message, AlertVariant variant)
        {
            StatusMessage = message;
            StatusVariant = variant;
            StatusVisibility = Visibility.Visible;
        }
    }

    public sealed class CategoryViewModel : BindableBase
    {
        private ButtonAppearance appearance;

        public CategoryViewModel(string name, ButtonAppearance appearance)
        {
            Name = name;
            this.appearance = appearance;
        }

        public string Name { get; }

        public ButtonAppearance Appearance
        {
            get { return appearance; }
            set { SetProperty(ref appearance, value); }
        }
    }

    public sealed class ProductViewModel
    {
        public ProductViewModel(string name, string shortName, string category, decimal price)
        {
            Name = name;
            ShortName = shortName;
            Category = category;
            Price = price;
        }

        public string Name { get; }

        public string ShortName { get; }

        public string Category { get; }

        public decimal Price { get; }
    }

    public sealed class OrderItemViewModel : BindableBase
    {
        private int quantity;

        public OrderItemViewModel(ProductViewModel product, int quantity)
        {
            Product = product;
            this.quantity = quantity;
        }

        public ProductViewModel Product { get; }

        public string Name
        {
            get { return Product.Name; }
        }

        public int Quantity
        {
            get { return quantity; }
            set
            {
                if (SetProperty(ref quantity, value))
                {
                    RaisePropertyChanged(nameof(LineTotal));
                }
            }
        }

        public decimal LineTotal
        {
            get { return Product.Price * Quantity; }
        }
    }
}
