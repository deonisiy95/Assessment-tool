using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

namespace DBUP
{
    /// <summary>
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();

            loadProfileInfo();
        }

        private void loadProfileInfo()
        {
            // получаем информацию о профиле с сервера
            string response_str = API.call("profile/get");

            // преобразуем в объект
            Response<Profile> response = JsonConvert.DeserializeObject<Response<Profile>>(response_str);

            // заполним поля
            tbxName.Content = response.response.full_name;
            tbxNameOrganization.Content = response.response.organisation_name;
            tbxRole.Content = Define.TYPE_ROLE[response.response.role];
            tbxAddress.Content = response.response.address;

            // получим информацию о доступных оценках
            response_str = API.call("assessment/getAll");

            // преобразуем в объект
            Response<List<Assessments>> r = JsonConvert.DeserializeObject<Response<List<Assessments>>>(response_str);

            // заполним поля
            for(int i = 0; i < r.response.Count; i++)
            {
                AddToListBox(r.response[i].audit_object, r.response[i].assessment_link);
            }
        }

        void AddToListBox(string content, string assessment_link)
        {
            BitmapImage bitmap = new BitmapImage(new Uri("pack://application:,,,/Image/ball.png"));
            Image img = new Image
            {
                Source = bitmap,
                Width = 16
            };
            Label lbl = new Label
            {
                VerticalContentAlignment = VerticalAlignment.Center,
                Content = content,
                FontWeight = FontWeight.FromOpenTypeWeight(450),
            };
            StackPanel stp = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Height = 25,
                Tag = assessment_link
            };
            
            stp.Children.Add(img);
            stp.Children.Add(lbl);
            lbxAssessmentList.Items.Add(stp);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            API.call("auth/doLogout");

            // откроем окно входа
            EnterWindow enter_window = new EnterWindow();
            MainWindow mainWindow = (MainWindow)DBUP.App.Current.Windows[0];

            enter_window.Show();
            mainWindow.Close();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {

            MainWindow mainWindow = (MainWindow)DBUP.App.Current.Windows[0];
            mainWindow.btnDiagram.IsEnabled = true;
            mainWindow.btnResult.IsEnabled = true;
            mainWindow.btnAssessment.IsEnabled = true;

            MainPage mainPage = new MainPage();
            mainWindow.frame.NavigationService.Navigate(mainPage);
        }

        private void lbxAssessmentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {

            //StackPanel stack = (StackPanel)lbxAssessmentList.SelectedItem;
            //    string assessment_link = stack.Tag.ToString();

            //WebClient wc = new WebClient();
            //byte[] bytesFile;
            //using (MemoryStream stream = new MemoryStream(wc.DownloadData(assessment_link)))
            //{
            //    bytesFile = stream.ToArray();
            //}

            //MessageBox.Show(bytesFile[0].ToString());

            if (lbxAssessmentList.SelectedIndex != -1)
            {
                StackPanel stack = (StackPanel)lbxAssessmentList.SelectedItem;
                string assessment_link = stack.Tag.ToString();

                List<Question> list = new List<Question>();
                MainPage.assessmentData = new Dictionary<string, string>();
                Assessment.answeredGroup = new int[34];

                if (MainPage.OpenQuestions(ref list, assessment_link, true))
                {
                   
                    MainWindow mainWindow = (MainWindow)DBUP.App.Current.Windows[0];
                    mainWindow.btnDiagram.IsEnabled = true;
                    mainWindow.btnResult.IsEnabled = true;
                    mainWindow.btnAssessment.IsEnabled = true;

                    Assessment assessment = new Assessment(list);
                    mainWindow.frame.NavigationService.Navigate(assessment);
                }
            }
            else
            {
                MessageBox.Show("Выберите оценку", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
