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
        static List<Assessments> assessment_info_list = new List<Assessments>();
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

            assessment_info_list = r.response;

            // заполним поля
            for (int i = 0; i < r.response.Count; i++)
            {
                AddToListBox(r.response[i].audit_object, r.response[i].assessment_link, r.response[i].assessment_id);
            }
        }

        void AddToListBox(string content, string assessment_link, int assessment_id)
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
                Tag = new string[] { assessment_link, assessment_id.ToString() }
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

            try
            {
                if (lbxAssessmentList.SelectedIndex != -1)
                {
                    StackPanel stack = (StackPanel)lbxAssessmentList.SelectedItem;
                    string[] tag = (string[])stack.Tag;
                    string assessment_link = tag[0];
                    int assessment_id = int.Parse(tag[1]);
                    Assessments assessment_info = null;

                    for (int i = 0; i < assessment_info_list.Count(); i++)
                    {
                        if (assessment_info_list[i].assessment_id == assessment_id)
                        {
                            assessment_info = assessment_info_list[i];
                            break;
                        }
                    }

                    if (assessment_info != null)
                    {
                        MainWindow mainWindow = (MainWindow)DBUP.App.Current.Windows[0];
                        mainWindow.btnDiagram.IsEnabled = true;
                        mainWindow.btnResult.IsEnabled = true;
                        mainWindow.btnAssessment.IsEnabled = true;
                        MainWindow.active_assessment_id = Int32.Parse(tag[1]);

                        DocumentBlankPage documentBlankPage = new DocumentBlankPage(assessment_info);
                        mainWindow.frame.NavigationService.Navigate(documentBlankPage);

                        return;
                    }

                    List<Question> list = new List<Question>();
                    MainPage.assessmentData = new Dictionary<string, string>();
                    Assessment.answeredGroup = new int[34];

                    if (MainPage.OpenQuestions(ref list, assessment_link, true))
                    {

                        MainWindow mainWindow = (MainWindow)DBUP.App.Current.Windows[0];
                        mainWindow.btnDiagram.IsEnabled = true;
                        mainWindow.btnResult.IsEnabled = true;
                        mainWindow.btnAssessment.IsEnabled = true;
                        MainWindow.active_assessment_id = Int32.Parse(tag[1]);

                        Assessment assessment = new Assessment(list);
                        mainWindow.frame.NavigationService.Navigate(assessment);
                    }
                }
                else
                {
                    MessageBox.Show("Выберите оценку", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
