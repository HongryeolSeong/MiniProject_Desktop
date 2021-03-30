using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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
using System.Windows.Shapes;

namespace WpfSMSApp.View
{
    /// <summary>
    /// LoginView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginView : MetroWindow
    {
        public LoginView()
        {
            InitializeComponent();
            Commons.LOGGER.Info("LoginView 초기화!");
        }

        private async void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            // mahapp 샘플 메서드 사용위해 async 사용
            var result = await this.ShowMessageAsync("종료", "프로그램 종료할까요?", 
                                                        MessageDialogStyle.AffirmativeAndNegative, null); // mahapp 샘플 메서드

            if (result == MessageDialogResult.Affirmative)
            {
                Commons.LOGGER.Info("프로그램 종료");
                Application.Current.Shutdown();
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TxtUserEamil.Focus();

            LblResult.Visibility = Visibility.Hidden;
        }

        private void TxtUserEamil_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                TxtPassword.Focus();
        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                BtnLogin_Click(sender, e);
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            LblResult.Visibility = Visibility.Hidden;

            if (string.IsNullOrEmpty(TxtUserEamil.Text) || string.IsNullOrEmpty(TxtPassword.Password))
            {
                LblResult.Visibility = Visibility.Visible;
                LblResult.Content = "아이디나 패스워드를 입력하세요.";
                Commons.LOGGER.Warn("아이디/패스워드 미입력 접속 시도.");
                return;
            }

            try
            {
                var email = TxtUserEamil.Text;
                var password = TxtPassword.Password;

                var mdHash = MD5.Create();
                password = Commons.GetMD5Hash(mdHash, password);

                var isOurUser = Logic.DataAccess.GetUsers()
                    .Where(u => u.UserEmail.Equals(email) && u.UserPassword.Equals(password) 
                                && u.UserActivated == true).Count();

                if (isOurUser == 0)
                {
                    LblResult.Visibility = Visibility.Visible;
                    LblResult.Content =" 아이디나 패스워드가 일치하지 않습니다.";
                    Commons.LOGGER.Warn("아이디/패스워드 불일치.");
                    return;
                }
                else
                {
                    Commons.LOGINED_USER = Logic.DataAccess.GetUsers().Where(u => u.UserEmail.Equals(email)).FirstOrDefault();
                    Commons.LOGGER.Info($"{email} 접속 성공");
                    this.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                Commons.LOGGER.Error($"예외발생 : {ex}");
                await this.ShowMessageAsync("예외", $"예외발생 {ex}");
            }
        }
    }
}
