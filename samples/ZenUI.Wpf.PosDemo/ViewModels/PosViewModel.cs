using System;
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
        private const int CartPageSize = 3;
        private const int ProductPageSize = 15;
        private readonly ProductViewModel[] products;
        private int cartPageIndex;
        private int productPageIndex;
        private ProductViewModel[] filteredProducts = Array.Empty<ProductViewModel>();
        private CategoryViewModel selectedCategory;
        private CategoryViewModel selectedSubcategory;
        private string searchText;
        private string orderNumber;
        private string statusMessage;
        private AlertVariant statusVariant;
        private Visibility statusVisibility = Visibility.Collapsed;

        public PosViewModel()
        {
            StoreName = "湖滨路演示门店";
            CashierName = "演示收银员";
            ShiftLabel = "早班 · 08:00—16:00";

            Categories = new ObservableCollection<CategoryViewModel>
            {
                new CategoryViewModel("全部", ButtonAppearance.Filled),
                new CategoryViewModel("咖啡", ButtonAppearance.Text),
                new CategoryViewModel("茶饮", ButtonAppearance.Text),
                new CategoryViewModel("烘焙", ButtonAppearance.Text),
                new CategoryViewModel("甜点", ButtonAppearance.Text)
            };
            selectedCategory = Categories[0];
            SecondaryCategories = new ObservableCollection<CategoryViewModel>();

            products = new[]
            {
                new ProductViewModel("美式咖啡", "美式", "咖啡", "经典咖啡", 22m),
                new ProductViewModel("拿铁咖啡", "拿铁", "咖啡", "经典咖啡", 28m),
                new ProductViewModel("燕麦澳白", "澳白", "咖啡", "经典咖啡", 32m),
                new ProductViewModel("茉莉绿茶", "茉莉", "茶饮", "纯茶", 18m),
                new ProductViewModel("葡萄冰茶", "葡萄", "茶饮", "果茶", 26m),
                new ProductViewModel("黄油可颂", "可颂", "烘焙", "酥点", 16m),
                new ProductViewModel("海盐贝果", "贝果", "烘焙", "面包", 20m),
                new ProductViewModel("松露蛋糕", "蛋糕", "甜点", "蛋糕", 36m),
                new ProductViewModel("巴斯克芝士", "芝士", "甜点", "蛋糕", 34m),
                new ProductViewModel("抹茶千层", "抹茶", "甜点", "蛋糕", 32m),

                new ProductViewModel("浓缩咖啡", "浓缩", "咖啡", "经典咖啡", 18m),
                new ProductViewModel("焦糖玛奇朵", "焦糖", "咖啡", "风味咖啡", 32m),
                new ProductViewModel("香草拿铁", "香草", "咖啡", "风味咖啡", 30m),
                new ProductViewModel("榛果拿铁", "榛果", "咖啡", "风味咖啡", 30m),
                new ProductViewModel("摩卡咖啡", "摩卡", "咖啡", "风味咖啡", 30m),
                new ProductViewModel("椰青美式", "椰青", "咖啡", "创意咖啡", 26m),
                new ProductViewModel("海盐焦糖拿铁", "海盐", "咖啡", "风味咖啡", 34m),

                new ProductViewModel("白桃乌龙", "白桃", "茶饮", "果茶", 24m),
                new ProductViewModel("桂花龙井", "桂花", "茶饮", "纯茶", 22m),
                new ProductViewModel("柠檬红茶", "柠檬", "茶饮", "果茶", 20m),
                new ProductViewModel("杨枝甘露", "甘露", "茶饮", "特调茶饮", 28m),
                new ProductViewModel("生椰抹茶", "生椰", "茶饮", "特调茶饮", 28m),
                new ProductViewModel("莓果气泡茶", "莓果", "茶饮", "果茶", 26m),

                new ProductViewModel("杏仁牛角包", "杏仁", "烘焙", "酥点", 22m),
                new ProductViewModel("肉桂卷", "肉桂", "烘焙", "酥点", 20m),
                new ProductViewModel("巧克力司康", "司康", "烘焙", "酥点", 18m),
                new ProductViewModel("芝士火腿包", "火腿", "烘焙", "面包", 24m),
                new ProductViewModel("蒜香法棍", "法棍", "烘焙", "面包", 18m),
                new ProductViewModel("红豆面包", "红豆", "烘焙", "面包", 16m),

                new ProductViewModel("提拉米苏", "提拉", "甜点", "杯装甜品", 38m),
                new ProductViewModel("草莓奶油蛋糕", "草莓", "甜点", "蛋糕", 36m),
                new ProductViewModel("芒果慕斯", "芒果", "甜点", "杯装甜品", 34m),
                new ProductViewModel("焦糖布丁", "布丁", "甜点", "杯装甜品", 18m),
                new ProductViewModel("纽约芝士蛋糕", "纽约", "甜点", "蛋糕", 38m),
                new ProductViewModel("巧克力熔岩蛋糕", "熔岩", "甜点", "蛋糕", 40m)
            };

            VisibleProducts = new ObservableCollection<ProductViewModel>();
            OrderItems = new ObservableCollection<OrderItemViewModel>();
            PagedOrderItems = new ObservableCollection<OrderItemViewModel>();

            AddProductCommand = new DelegateCommand<ProductViewModel>(AddProduct, product => product != null);
            SelectCategoryCommand = new DelegateCommand<CategoryViewModel>(SelectCategory, category => category != null);
            SelectSubcategoryCommand = new DelegateCommand<CategoryViewModel>(SelectSubcategory, category => category != null);
            PreviousProductPageCommand = new DelegateCommand(
                PreviousProductPage,
                () => productPageIndex > 0);
            NextProductPageCommand = new DelegateCommand(
                NextProductPage,
                () => productPageIndex < ProductPageCount - 1);
            IncreaseItemCommand = new DelegateCommand<OrderItemViewModel>(IncreaseItem, item => item != null);
            DecreaseItemCommand = new DelegateCommand<OrderItemViewModel>(DecreaseItem, item => item != null);
            RemoveItemCommand = new DelegateCommand<OrderItemViewModel>(RemoveItem, item => item != null);
            PreviousCartPageCommand = new DelegateCommand(
                PreviousCartPage,
                () => cartPageIndex > 0);
            NextCartPageCommand = new DelegateCommand(
                NextCartPage,
                () => cartPageIndex < CartPageCount - 1);
            ClearOrderCommand = new DelegateCommand(ClearOrder);
            NewOrderCommand = new DelegateCommand(NewOrder);
            CheckoutCommand = new DelegateCommand(Checkout);
            ExitCommand = new DelegateCommand(Exit);

            RefreshSecondaryCategories();
            NewOrder();
            ApplyProductFilter();
        }

        public string StoreName { get; }

        public string CashierName { get; }

        public string ShiftLabel { get; }

        public string OrderNumber
        {
            get { return orderNumber; }
            private set { SetProperty(ref orderNumber, value); }
        }

        public string SearchText
        {
            get { return searchText; }
            set
            {
                if (SetProperty(ref searchText, value))
                {
                    productPageIndex = 0;
                    ApplyProductFilter();
                }
            }
        }

        public ObservableCollection<CategoryViewModel> Categories { get; }

        public ObservableCollection<CategoryViewModel> SecondaryCategories { get; }

        public ObservableCollection<ProductViewModel> VisibleProducts { get; }

        public ObservableCollection<OrderItemViewModel> OrderItems { get; }

        public ObservableCollection<OrderItemViewModel> PagedOrderItems { get; }

        public DelegateCommand<ProductViewModel> AddProductCommand { get; }

        public DelegateCommand<CategoryViewModel> SelectCategoryCommand { get; }

        public DelegateCommand<CategoryViewModel> SelectSubcategoryCommand { get; }

        public DelegateCommand PreviousProductPageCommand { get; }

        public DelegateCommand NextProductPageCommand { get; }

        public DelegateCommand<OrderItemViewModel> IncreaseItemCommand { get; }

        public DelegateCommand<OrderItemViewModel> DecreaseItemCommand { get; }

        public DelegateCommand<OrderItemViewModel> RemoveItemCommand { get; }

        public DelegateCommand PreviousCartPageCommand { get; }

        public DelegateCommand NextCartPageCommand { get; }

        public DelegateCommand ClearOrderCommand { get; }

        public DelegateCommand NewOrderCommand { get; }

        public DelegateCommand CheckoutCommand { get; }

        public DelegateCommand ExitCommand { get; }

        public int ItemCount => OrderItems.Sum(item => item.Quantity);

        public decimal OrderTotal => OrderItems.Sum(item => item.LineTotal);

        public int ProductPageNumber => productPageIndex + 1;

        public int ProductPageCount => Math.Max(
            1,
            (int)Math.Ceiling(filteredProducts.Length / (double)ProductPageSize));

        public int CartPageNumber => cartPageIndex + 1;

        public int CartPageCount => Math.Max(
            1,
            (int)Math.Ceiling(OrderItems.Count / (double)CartPageSize));

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

            var itemIndex = OrderItems.IndexOf(existingItem ?? OrderItems[OrderItems.Count - 1]);
            cartPageIndex = itemIndex / CartPageSize;
            RefreshOrderSummary();
            ShowStatus(product.Name + " 已加入购物车。", AlertVariant.Success);
        }

        private void SelectCategory(CategoryViewModel category)
        {
            selectedCategory = category;
            productPageIndex = 0;
            foreach (var item in Categories)
            {
                item.Appearance = ReferenceEquals(item, category)
                    ? ButtonAppearance.Filled
                    : ButtonAppearance.Text;
            }

            RefreshSecondaryCategories();
            ApplyProductFilter();
        }

        private void SelectSubcategory(CategoryViewModel subcategory)
        {
            selectedSubcategory = subcategory;
            productPageIndex = 0;
            foreach (var item in SecondaryCategories)
            {
                item.Appearance = ReferenceEquals(item, subcategory)
                    ? ButtonAppearance.Filled
                    : ButtonAppearance.Text;
            }

            ApplyProductFilter();
        }

        private void RefreshSecondaryCategories()
        {
            SecondaryCategories.Clear();
            SecondaryCategories.Add(new CategoryViewModel("全部", ButtonAppearance.Filled));

            string[] names;
            switch (selectedCategory.Name)
            {
                case "咖啡":
                    names = new[] { "经典咖啡", "风味咖啡", "创意咖啡" };
                    break;
                case "茶饮":
                    names = new[] { "纯茶", "果茶", "特调茶饮" };
                    break;
                case "烘焙":
                    names = new[] { "酥点", "面包" };
                    break;
                case "甜点":
                    names = new[] { "蛋糕", "杯装甜品" };
                    break;
                default:
                    names = Array.Empty<string>();
                    break;
            }

            foreach (var name in names)
            {
                SecondaryCategories.Add(new CategoryViewModel(name, ButtonAppearance.Text));
            }

            selectedSubcategory = SecondaryCategories[0];
        }

        private void ApplyProductFilter()
        {
            if (VisibleProducts == null || selectedCategory == null)
            {
                return;
            }

            var query = products.AsEnumerable();
            if (selectedCategory.Name != "全部")
            {
                query = query.Where(product => product.Category == selectedCategory.Name);
            }

            if (selectedSubcategory != null && selectedSubcategory.Name != "全部")
            {
                query = query.Where(product => product.Subcategory == selectedSubcategory.Name);
            }

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var keyword = SearchText.Trim();
                query = query.Where(product =>
                    product.Name.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                    product.Category.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                    product.Subcategory.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0);
            }

            filteredProducts = query.ToArray();
            productPageIndex = Math.Max(0, Math.Min(productPageIndex, ProductPageCount - 1));
            RefreshProductPage();
        }

        private void PreviousProductPage()
        {
            if (productPageIndex <= 0)
            {
                return;
            }

            productPageIndex--;
            RefreshProductPage();
        }

        private void NextProductPage()
        {
            if (productPageIndex >= ProductPageCount - 1)
            {
                return;
            }

            productPageIndex++;
            RefreshProductPage();
        }

        private void RefreshProductPage()
        {
            VisibleProducts.Clear();
            foreach (var product in filteredProducts
                .Skip(productPageIndex * ProductPageSize)
                .Take(ProductPageSize))
            {
                VisibleProducts.Add(product);
            }

            RaisePropertyChanged(nameof(ProductPageNumber));
            RaisePropertyChanged(nameof(ProductPageCount));
            PreviousProductPageCommand.RaiseCanExecuteChanged();
            NextProductPageCommand.RaiseCanExecuteChanged();
        }

        private void IncreaseItem(OrderItemViewModel item)
        {
            item.Quantity++;
            RefreshOrderSummary();
        }

        private void DecreaseItem(OrderItemViewModel item)
        {
            if (item.Quantity <= 1)
            {
                OrderItems.Remove(item);
            }
            else
            {
                item.Quantity--;
            }

            RefreshOrderSummary();
        }

        private void RemoveItem(OrderItemViewModel item)
        {
            OrderItems.Remove(item);
            RefreshOrderSummary();
        }

        private void PreviousCartPage()
        {
            if (cartPageIndex <= 0)
            {
                return;
            }

            cartPageIndex--;
            RefreshCartPage();
        }

        private void NextCartPage()
        {
            if (cartPageIndex >= CartPageCount - 1)
            {
                return;
            }

            cartPageIndex++;
            RefreshCartPage();
        }

        private void ClearOrder()
        {
            OrderItems.Clear();
            cartPageIndex = 0;
            RefreshOrderSummary();
            ShowStatus("购物车已清空。", AlertVariant.Info);
        }

        private void NewOrder()
        {
            OrderItems.Clear();
            cartPageIndex = 0;
            OrderNumber = "订单号 ZP-" + DateTime.Now.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture);
            StatusVisibility = Visibility.Collapsed;
            RefreshOrderSummary();
        }

        private void Checkout()
        {
            if (OrderItems.Count == 0)
            {
                ShowStatus("请先添加商品。", AlertVariant.Warning);
                return;
            }

            ShowStatus(
                string.Format(CultureInfo.CurrentCulture, "模拟支付成功，共收款 ¥ {0:N2}。", OrderTotal),
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
            RefreshCartPage();
        }

        private void RefreshCartPage()
        {
            cartPageIndex = Math.Max(0, Math.Min(cartPageIndex, CartPageCount - 1));

            PagedOrderItems.Clear();
            foreach (var item in OrderItems
                .Skip(cartPageIndex * CartPageSize)
                .Take(CartPageSize))
            {
                PagedOrderItems.Add(item);
            }

            RaisePropertyChanged(nameof(CartPageNumber));
            RaisePropertyChanged(nameof(CartPageCount));
            PreviousCartPageCommand.RaiseCanExecuteChanged();
            NextCartPageCommand.RaiseCanExecuteChanged();
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
        public ProductViewModel(string name, string shortName, string category, string subcategory, decimal price)
        {
            Name = name;
            ShortName = shortName;
            Category = category;
            Subcategory = subcategory;
            Price = price;
        }

        public string Name { get; }

        public string ShortName { get; }

        public string Category { get; }

        public string Subcategory { get; }

        public decimal Price { get; }

        public string ImagePath
        {
            get
            {
                switch (Category)
                {
                    case "咖啡":
                        return "/ZenUI.Wpf.PosDemo;component/Assets/Products/coffee.jpg";
                    case "茶饮":
                        return "/ZenUI.Wpf.PosDemo;component/Assets/Products/tea.jpg";
                    case "烘焙":
                        return "/ZenUI.Wpf.PosDemo;component/Assets/Products/bakery.jpg";
                    case "甜点":
                        return "/ZenUI.Wpf.PosDemo;component/Assets/Products/dessert.jpg";
                    default:
                        return null;
                }
            }
        }
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

        public string Name => Product.Name;

        public decimal UnitPrice => Product.Price;

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

        public decimal LineTotal => Product.Price * Quantity;
    }
}
