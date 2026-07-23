using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ZenUI.Wpf.Demo.Views
{
    public partial class DataGridView : UserControl
    {
        public DataGridView()
        {
            InitializeComponent();
            Users = new ObservableCollection<UserRow>
            {
                new UserRow(1001, "林知夏", "zhixia.lin@example.com", "产品设计", "在线"),
                new UserRow(1002, "周景明", "jingming.zhou@example.com", "前端开发", "在线"),
                new UserRow(1003, "陈雨青", "yuqing.chen@example.com", "测试工程", "忙碌"),
                new UserRow(1004, "宋予安", "yuan.song@example.com", "项目管理", "离线"),
                new UserRow(1005, "顾言川", "yanchuan.gu@example.com", "后端开发", "在线")
            };
            DataContext = this;
        }

        public ObservableCollection<UserRow> Users { get; }

        public sealed class UserRow
        {
            public UserRow(int id, string name, string email, string role, string status)
            {
                Id = id;
                Name = name;
                Email = email;
                Role = role;
                Status = status;
            }

            public int Id { get; }
            public string Name { get; }
            public string Email { get; }
            public string Role { get; }
            public string Status { get; }
        }
    }
}
