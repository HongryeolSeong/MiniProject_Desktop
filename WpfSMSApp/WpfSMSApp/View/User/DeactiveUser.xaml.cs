using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using MahApps.Metro.Controls.Dialogs;


namespace WpfSMSApp.View.User
{
    /// <summary>
    /// MyAccount.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DeactiveUser : Page
    {
        public DeactiveUser()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // 그리드 바인딩
                List<Model.User> users = Logic.DataAccess.GetUsers();
                this.DataContext = users;
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 EditAccount Loaded : {ex}");
                throw ex;
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack(); //NavigationService.Navigate(new MyAccount());
        }

        private async void BtnDeactive_Click(object sender, RoutedEventArgs e)
        {
            if (GrdData.SelectedItem == null)
            {
                await Commons.ShowMessageAsync("오류", "비활성화할 사용자를 선택하세요.");
            }

            try
            {
                var user = GrdData.SelectedItem as Model.User;
                user.UserActivated = false;
                var result = Logic.DataAccess.SetUser(user);
                if (result == 0)
                {
                    await Commons.ShowMessageAsync("오류", "사용자 비활성화에 실패했습니다.");
                    return;
                }
                else
                {
                    NavigationService.Navigate(new DeactiveUser());
                }
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 : {ex}");
            }
        }

        private void GrdData_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 GrdData_SelectedCellsChanged : {ex}");
            }
        }
    }
}
