using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfSMSApp.Model;

namespace WpfSMSApp.View.User
{
    /// <summary>
    /// MyAccount.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UserList : Page
    {
        public UserList()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                RboAll.IsChecked = true;
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 UserList Loaded : {ex}");
                throw ex;
            }
        }

        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddUser());
            try
            {
                NavigationService.Navigate(new AddUser());
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 BtnAddUser_Loaded : {ex}");
                throw ex;
            }
        }

        private void BtnEditUser_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EditUser());
            try
            {
                NavigationService.Navigate(new EditUser());
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 BtnEditUser_Loaded : {ex}");
                throw ex;
            }
        }

        private void BtnDeactivatedUser_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DeactiveUser());
            try
            {
                NavigationService.Navigate(new DeactiveUser());
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 BtnDeactivatedUser_Click : {ex}");
                throw ex;
            }
        }

        private void BtnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "PDF File (*.pdf)|*.pdf";
            saveDialog.FileName = "";
            if (saveDialog.ShowDialog() == true)
            {
                // PDF 변환
                try
                {
                    iTextSharp.text.Font font = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12);
                    string pdfFilePath = saveDialog.FileName;

                    iTextSharp.text.Document pdfDoc = new Document(PageSize.A4);

                    // 1. PDF 객체 생성
                    PdfPTable pdfTable = new PdfPTable(GrdData.Columns.Count);

                    // 2. PDF 내용 생성
                    // 한글 호환을 위한
                    string nanumttf = Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), @"Fonts\NanumGothic.ttf");
                    BaseFont nanumBase = BaseFont.CreateFont(nanumttf, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    var nanumFont = new iTextSharp.text.Font(nanumBase, 16f);

                    Paragraph title = new Paragraph($@"부경대 PKNU Stock Management System : {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}", nanumFont);


                    // 3. PDF 파일 생성
                    using (FileStream stream = new FileStream(pdfFilePath, FileMode.OpenOrCreate))
                    {
                        PdfWriter.GetInstance(pdfDoc, stream);
                        pdfDoc.Open();
                        // 2번에서 생성할 내용 추가

                        pdfDoc.Close();
                        stream.Close(); // 필수 아님
                    }
                }
                catch (Exception ex)
                {
                    Commons.LOGGER.Error($"예외발생 BtnExportPdf_Click : {ex}");
                }
            }
        }

        private void RboAll_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                List<WpfSMSApp.Model.User> users = new List<Model.User>(); // 폴더 User와 충돌하기 때문에 이렇게 코딩

                if (RboAll.IsChecked == true)
                {
                    users = Logic.DataAccess.GetUsers();
                }
                this.DataContext = users; // 페이지 전체에 users 값 삽입
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 : {ex}");
            }
        }

        private void RboActive_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                List<WpfSMSApp.Model.User> users = new List<Model.User>(); // 폴더 User와 충돌하기 때문에 이렇게 코딩

                if (RboActive.IsChecked == true)
                {
                    users = Logic.DataAccess.GetUsers().Where(u => u.UserActivated == true).ToList();
                }
                this.DataContext = users; // 페이지 전체에 users 값 삽입
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 : {ex}");
            }
        }

        private void RboDeactive_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                List<WpfSMSApp.Model.User> users = new List<Model.User>(); // 폴더 User와 충돌하기 때문에 이렇게 코딩

                if (RboDeactive.IsChecked == true)
                {
                    users = Logic.DataAccess.GetUsers().Where(u => u.UserActivated == false).ToList();
                }
                this.DataContext = users; // 페이지 전체에 users 값 삽입
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 : {ex}");
            }
        }
    }
}
