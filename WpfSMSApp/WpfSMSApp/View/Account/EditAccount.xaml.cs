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


namespace WpfSMSApp.View.Account
{
    /// <summary>
    /// MyAccount.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EditAccount : Page
    {
        public EditAccount()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LblUserIdentityNumber.Visibility = LblUserSurName.Visibility =
                    LblUserName.Visibility = LblUserEmail.Visibility =
                    LblUserPassword.Visibility = LblUserAdmin.Visibility =
                    LblUserActivated.Visibility = Visibility.Hidden;

                // 콤보박스 초기화
                List<string> comboValues = new List<string>
                {
                    "False", // 0 인덱스
                    "True" // 1 인덱스
                };
                CboUserAdmin.ItemsSource = comboValues;
                CboUserActivated.ItemsSource = comboValues;

                var user = Commons.LOGINED_USER;
                TxtUserID.Text = user.UserID.ToString();
                TxtUserIdentityNumber.Text = user.UserIdentityNumber;
                TxtUserSurName.Text = user.UserSurname;
                TxtUserName.Text = user.UserName;
                TxtUserEmail.Text = user.UserEmail;
                //TxtUserPassword.Password = user.UserPassword; //암호화 추가로 오류 발생하기 때문에 주석 처리
                CboUserAdmin.SelectedIndex = user.UserAdmin == false ? 0 : 1;
                CboUserActivated.SelectedIndex = user.UserActivated == false ? 0 : 1;
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

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true; // 입력된 값이 모두 만족하는지 판별하는 플래그

            LblUserIdentityNumber.Visibility = LblUserSurName.Visibility =
                    LblUserName.Visibility = LblUserEmail.Visibility =
                    LblUserPassword.Visibility = LblUserAdmin.Visibility =
                    LblUserActivated.Visibility = Visibility.Hidden;

            var user = Commons.LOGINED_USER;

            if (string.IsNullOrEmpty(TxtUserSurName.Text))
            {
                LblUserSurName.Visibility = Visibility.Visible;
                LblUserSurName.Text = "이름(성)을 입력하세요.";
                isValid = false;
            }

            if (string.IsNullOrEmpty(TxtUserName.Text))
            {
                LblUserName.Visibility = Visibility.Visible;
                LblUserName.Text = "이름(성)을 입력하세요.";
                isValid = false;
            }

            if (string.IsNullOrEmpty(TxtUserEmail.Text))
            {
                LblUserEmail.Visibility = Visibility.Visible;
                LblUserEmail.Text = "이메일을 입력하세요.";
                isValid = false;
            }

            if (string.IsNullOrEmpty(TxtUserPassword.Password))
            {
                LblUserPassword.Visibility = Visibility.Visible;
                LblUserPassword.Text = "패스워드를 입력하세요.";
                isValid = false;
            }

            if (isValid)
            {
                MessageBox.Show("DB수정 처리 완료!");
                user.UserSurname = TxtUserSurName.Text;
                user.UserName = TxtUserName.Text;
                user.UserEmail = TxtUserEmail.Text;
                user.UserPassword = TxtUserPassword.Password;
                user.UserAdmin = bool.Parse(CboUserAdmin.SelectedValue.ToString());
                user.UserActivated = bool.Parse(CboUserActivated.SelectedValue.ToString());

                try
                {
                    var mdHash = MD5.Create();
                    user.UserPassword = Commons.GetMD5Hash(mdHash, user.UserPassword);

                    var result = Logic.DataAccess.SetUser(user);
                    if (result == 0)
                    {
                        // 수정 안됨
                        LblResult.Text = "계정 수정에 문제가 발생했습니다.";
                        LblResult.Foreground = Brushes.OrangeRed;
                    }
                    else
                    {
                        // 수정 됨
                        LblResult.Text = "정상적으로 수정했습니다.";
                        LblResult.Foreground = Brushes.DeepSkyBlue;
                    }
                }
                catch (Exception ex)
                {
                    Commons.LOGGER.Error($"예외발생 : {ex}");
                }
            }
        }
    }
}
