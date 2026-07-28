using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace ZenUI.Wpf.Gallery.Views
{
    public partial class DataGridView : UserControl, INotifyPropertyChanged
    {
        private bool _isUpdatingSelection;

        public DataGridView()
        {
            InitializeComponent();
            Users = new ObservableCollection<UserRow>
            {
                new UserRow(1, 1001, "林知夏", "zhixia.lin@example.com", "产品设计", 136.5, "在线"),
                new UserRow(2, 1002, "周景明", "jingming.zhou@example.com", "前端开发", 152, "在线"),
                new UserRow(3, 1003, "陈雨青", "yuqing.chen@example.com", "测试工程", 143.5, "忙碌"),
                new UserRow(4, 1004, "宋予安", "yuan.song@example.com", "项目管理", 128, "离线"),
                new UserRow(5, 1005, "顾言川", "yanchuan.gu@example.com", "后端开发", 149, "在线")
            };

            foreach (var user in Users)
            {
                user.PropertyChanged += OnUserPropertyChanged;
            }

            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<UserRow> Users { get; }

        public bool? AreAllUsersSelected
        {
            get
            {
                if (Users.All(user => user.IsSelected))
                {
                    return true;
                }

                return Users.Any(user => user.IsSelected) ? (bool?)null : false;
            }
            set
            {
                if (!value.HasValue)
                {
                    return;
                }

                _isUpdatingSelection = true;
                foreach (var user in Users)
                {
                    user.IsSelected = value.Value;
                }

                _isUpdatingSelection = false;
                OnPropertyChanged();
            }
        }

        private void OnUserPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!_isUpdatingSelection && e.PropertyName == nameof(UserRow.IsSelected))
            {
                OnPropertyChanged(nameof(AreAllUsersSelected));
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public sealed class UserRow : INotifyPropertyChanged
        {
            private bool _isSelected;

            public UserRow(
                int sequence,
                int id,
                string name,
                string email,
                string role,
                double monthlyHours,
                string status)
            {
                Sequence = sequence;
                Id = id;
                Name = name;
                Email = email;
                Role = role;
                MonthlyHours = monthlyHours;
                Status = status;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public int Sequence { get; }
            public int Id { get; }
            public string Name { get; }
            public string Email { get; }
            public string Role { get; }
            public double MonthlyHours { get; }
            public string Status { get; }

            public bool IsSelected
            {
                get => _isSelected;
                set
                {
                    if (_isSelected == value)
                    {
                        return;
                    }

                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }
    }
}
