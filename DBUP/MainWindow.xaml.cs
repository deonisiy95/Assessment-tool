using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Xml;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace DBUP
{
    /// <summary>
    /// Главное окно приложения
    /// </summary>
    public partial class MainWindow : Window
    {
        Logging log = new Logging();
        public static string password;
        public static string login;
        public static List<Mi> valueGroup = new List<Mi>();
        static bool isCalculate = false;
        public static double EV_BPTP;
        public static double EV_BITP;
        public static double EV_OZPD_1;
        public static double EV_OZPD_2;
        public static double EV_OOPD;
        public static double EV_1;
        public static double EV_2;
        public static double EV_3;
        public static double EV_R;

        public MainWindow()
        {
            // инициализируем cookie
            API.initCookie();

            // получаем информацию залогинен ли пользователь
            string response_str = API.call("global/doStart");

            MessageBox.Show(response_str);

            // преобразуем в объект
            Response<doStart> response = JsonConvert.DeserializeObject<Response<doStart>>(response_str);

            // если ошибка или не залогинен
            if (response.status != "ok" || response.response.is_logged == 0)
            {
                // откроем окно входа
                EnterWindow enter_window = new EnterWindow();
                enter_window.Show();

                this.Close();
            }

            InitializeComponent();
            frame.NavigationService.Navigate(new Uri("MainPage.xaml", UriKind.Relative));
            btnAssessment.IsEnabled = false;
            btnDiagram.IsEnabled = false;
            btnResult.IsEnabled = false;
        }
        public MainWindow(bool from_enter_window = false)
        {
            // инициализируем cookie
            API.initCookie();

            InitializeComponent();
            frame.NavigationService.Navigate(new Uri("MainPage.xaml", UriKind.Relative));
            btnAssessment.IsEnabled = false;
            btnDiagram.IsEnabled = false;
            btnResult.IsEnabled = false;
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow window = new AboutWindow();
            window.Owner = this;
            window.ShowDialog();

        }


        private void menuItemSaveAssessment_Click(object sender, RoutedEventArgs e)
        {
            if (frame.NavigationService.Content.ToString() != "DBUP.MainPage")
            {
                SaveFileAssessment();
                log.WriteLog(5);
            }
        }

        private void menuItemNewAssessment_Click(object sender, RoutedEventArgs e)
        {
            if(frame.NavigationService.Content.ToString() != "DBUP.MainPage")
            {
                MessageBoxResult resultQuery = MessageBox.Show("Сохранить файл?", "Создать новую оценку", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (resultQuery == MessageBoxResult.No)
                {
                    frame.NavigationService.Navigate(new Uri("MainPage.xaml", UriKind.Relative));
                    btnAssessment.IsEnabled = false;
                    btnDiagram.IsEnabled = false;
                    btnResult.IsEnabled = false;
                }
                else if (resultQuery == MessageBoxResult.Yes)
                {
                    if (SaveFileAssessment())
                    {
                        log.WriteLog(5);
                        frame.NavigationService.Navigate(new Uri("MainPage.xaml", UriKind.Relative));
                        btnAssessment.IsEnabled = false;
                        btnDiagram.IsEnabled = false;
                        btnResult.IsEnabled = false;
                    }
                }
            }
        }


        bool SaveFileAssessment()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "DBUP файл | * .dbup";
            string currentPath = "";
            if(MainPage.path != "")
            {
                currentPath = MainPage.path;
            }
            else
            {
                if (saveFileDialog.ShowDialog() == true)
                {
                    currentPath = saveFileDialog.FileName;
                }
            }
            if (currentPath != "")
            {
                try
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    StringWriter stringWriter = new StringWriter(stringBuilder);
                    //XmlTextWriter writer = new XmlTextWriter(currentPath, System.Text.Encoding.UTF8);
                    XmlTextWriter writer = new XmlTextWriter(stringWriter);
                    writer.WriteStartDocument(true);
                    writer.Formatting = System.Xml.Formatting.Indented;
                    writer.Indentation = 2;
                    writer.WriteStartElement("Table");

                    for (int i = 0; i < Assessment.questions.Count; i++)
                    {
                        writer.WriteStartElement("question");
                        writer.WriteStartElement("group");
                        writer.WriteString(Assessment.questions[i].group.ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("number");
                        writer.WriteString(Assessment.questions[i].number.ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("category");
                        writer.WriteString(Assessment.questions[i].category.ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("value");
                        writer.WriteString(Assessment.questions[i].value.ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("mandatory");
                        writer.WriteString(Assessment.questions[i].mandatory.ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("answered");
                        writer.WriteString(Assessment.questions[i].answered.ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("overlook");
                        writer.WriteString(Assessment.questions[i].overlook.ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("question_value");
                        writer.WriteString(Assessment.questions[i].question.ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("documentation");
                        writer.WriteString(Assessment.questions[i].documentation.ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("execution");
                        writer.WriteString(Assessment.questions[i].execution.ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("secondaryOuestion1");
                        writer.WriteStartElement("answered");
                        writer.WriteString(Assessment.questions[i].secondaryOuestions[0].answered.ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("overlook");
                        writer.WriteString(Assessment.questions[i].secondaryOuestions[0].overlook.ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("documentation");
                        writer.WriteString(Assessment.questions[i].secondaryOuestions[0].documentation.ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("execution");
                        writer.WriteString(Assessment.questions[i].secondaryOuestions[0].execution.ToString());
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteStartElement("secondaryOuestion2");
                        writer.WriteStartElement("answered");
                        writer.WriteString(Assessment.questions[i].secondaryOuestions[1].answered.ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("overlook");
                        writer.WriteString(Assessment.questions[i].secondaryOuestions[1].overlook.ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("documentation");
                        writer.WriteString(Assessment.questions[i].secondaryOuestions[1].documentation.ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("execution");
                        writer.WriteString(Assessment.questions[i].secondaryOuestions[1].execution.ToString());
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteStartElement("secondaryOuestion3");
                        writer.WriteStartElement("answered");
                        writer.WriteString(Assessment.questions[i].secondaryOuestions[2].answered.ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("overlook");
                        writer.WriteString(Assessment.questions[i].secondaryOuestions[2].overlook.ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("documentation");
                        writer.WriteString(Assessment.questions[i].secondaryOuestions[2].documentation.ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("execution");
                        writer.WriteString(Assessment.questions[i].secondaryOuestions[2].execution.ToString());
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                    }

                    writer.WriteStartElement("assessmentData");
                    writer.WriteStartElement("NameAssessment");
                    writer.WriteString(MainPage.assessmentData["NameAssessment"]);
                    writer.WriteEndElement();
                    writer.WriteStartElement("NameObject");
                    writer.WriteString(MainPage.assessmentData["NameObject"]);
                    writer.WriteEndElement();
                    writer.WriteStartElement("Address");
                    writer.WriteString(MainPage.assessmentData["Address"]);
                    writer.WriteEndElement();
                    writer.WriteStartElement("Auditor");
                    writer.WriteString(MainPage.assessmentData["Auditor"]);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteStartElement("answeredGroup");
                    for (int i = 0; i < Assessment.answeredGroup.Length; i++)
                    {
                        writer.WriteStartElement("item" + i.ToString());
                        writer.WriteString(Assessment.answeredGroup[i].ToString());
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Close();

                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(stringBuilder.ToString());

                    MemoryStream memoryStream = new MemoryStream();
                    xml.Save(memoryStream);
                    byte[] bytesFile = Encrypt(memoryStream.ToArray(), password+login);
                    File.WriteAllBytes(currentPath, bytesFile);
                    return true;
                }
                catch (Exception)
                {
                    System.Windows.MessageBox.Show("Что-то пошло не так", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        byte[] Encrypt(byte[] fileBytes, string key)
        {
            SymmetricAlgorithm algorithm = SymmetricAlgorithm.Create("AES");
            HashAlgorithm hashAlgorithm = HashAlgorithm.Create("MD5");
            algorithm.Key = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(key));
            algorithm.IV = new byte[16];
            algorithm.Mode = CipherMode.CBC;
            algorithm.Padding = PaddingMode.ANSIX923;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, algorithm.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(fileBytes, 0, fileBytes.Length);
            cs.Close();
            ms.Close();
            return ms.ToArray();
        }
        private void btnDiagram_Click(object sender, RoutedEventArgs e)
        {
            if (allQuestionsAnswered())
            {
                if (!isCalculate)
                    Calculate();
                frame.NavigationService.Navigate(new Uri("DiagramPage.xaml", UriKind.Relative));
            }else
            {
                MessageBox.Show("Оценка не закончена");
            }
        }

        private void btnAssessment_Click(object sender, RoutedEventArgs e)
        {
            if (frame.NavigationService.Content.ToString() != "DBUP.Assessment")
            {
                Assessment assessment = new Assessment(Assessment.questions);
                frame.NavigationService.Navigate(assessment);
            }
        }

        private void btnResult_Click(object sender, RoutedEventArgs e)
        {
            if (allQuestionsAnswered())
            {
                if (!isCalculate)
                    Calculate();
                frame.NavigationService.Navigate(new Uri("ReportPage.xaml", UriKind.Relative));
            }
            else
            {
                MessageBox.Show("Оценка не закончена");
            }

        }

        private void btnSendMail_Click(object sender, RoutedEventArgs e)
        {
            Process process = new Process();
            process.StartInfo.FileName = "mailto:deonisiy9572@gmail.com";
            process.Start();
        }

        bool allQuestionsAnswered()
        {
            for (int i = 0; i < Assessment.answeredGroup.Length; i++)
            {
                if (Assessment.answeredGroup[i] != 1)
                   return false;
            }
            return true;
        }

        void Calculate()
        {
            CalculateValueGroup();

            EV_BPTP = CalculateEv(new int[] { 1, 2, 3, 4, 5, 6, 7 }, 20, 0);
            EV_BITP = CalculateEv(new int[] { 1, 2, 3, 4, 5, 6, 8 }, 20, 1);
            EV_OZPD_1 = CalculateEv(new int[] { 1, 2, 3, 4, 5, 8, 10 }, 20, 2);
            EV_OZPD_2 = CalculateEv(new int[] { 1, 2, 3, 4, 5, 6, 8, 10 }, 20, 2);
            EV_OOPD = CalculateEv(new int[] { 9 }, 8);
            EV_1 = Math.Min(EV_OZPD_2, Math.Min(EV_OOPD, Math.Min(EV_BITP, EV_BPTP)));
            EV_2 = CalculateEv(new int[] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27 }, 25);
            EV_3 = CalculateEv(new int[] { 28, 29, 30, 31, 32, 33, 34}, 10);
            EV_R = Math.Min(EV_1, Math.Min(EV_2, EV_3));

            isCalculate = true;
        }

        double CalculateEv(int[] indices, int rule_k, int indexSecondary = -1)
        {
            double sum = 0;
            int amountZero = 0;
            int amountOverlook = 0;
            double k = 1;
            int[] control = {1,2,3,4,5,6};
            
            for (int i = 0; i < indices.Length; i++)
            {
                if (control.Contains(indices[i]))
                {
                    sum += valueGroup[indices[i] - 1].secondaryMi[indexSecondary].value;
                    amountZero += valueGroup[indices[i] - 1].secondaryMi[indexSecondary].countZero;
                    if (valueGroup[indices[i] - 1].secondaryMi[indexSecondary].isOverlook())
                        amountOverlook++;
                }
                else
                {
                    sum += valueGroup[indices[i] - 1].value;
                    amountZero += valueGroup[indices[i] - 1].countZero;
                    if (valueGroup[indices[i] - 1].isOverlook())
                        amountOverlook++;
                }
            }

            if (amountZero > 0 && amountZero <= rule_k)
                k = 0.85;
            else if (amountZero > rule_k)
                k = 0.7;

            return (sum / (indices.Length - amountOverlook))*k;
        }

        void CalculateValueGroup()
        {
            int index = 0;

            for (int i = 1; i <= 6; i++)
            {
                Mi group = new Mi(i);
                Mi groupBPTP = new Mi(i);
                Mi groupBITP = new Mi(i);
                Mi groupBTPPDn = new Mi(i);
                int amount = 0;

                while (i == Assessment.questions[index].group)
                {
                    if (Assessment.questions[index].mandatory)
                    {
                        if (!Assessment.questions[index].secondaryOuestions[0].overlook)
                        {
                            double value = GetValue(Assessment.questions[index], 0);
                            groupBPTP.sum += value;
                            if (value == 0)
                                groupBPTP.countZero++;
                        }
                        else
                        {
                            groupBPTP.amountOverlookQuestions++;
                        }


                        if (!Assessment.questions[index].secondaryOuestions[1].overlook)
                        {
                            double value = GetValue(Assessment.questions[index], 1);
                            groupBITP.sum += value;
                            if (value == 0)
                                groupBITP.countZero++;
                        }
                        else
                        {
                            groupBITP.amountOverlookQuestions++;
                        }

                        if (!Assessment.questions[index].secondaryOuestions[2].overlook)
                        {
                            double value = GetValue(Assessment.questions[index], 2);
                            groupBTPPDn.sum += value;
                            if (value == 0)
                                groupBTPPDn.countZero++;
                        }
                        else
                        {
                            groupBTPPDn.amountOverlookQuestions++;
                        }
                    }
                    else
                    {
                        if (!Assessment.questions[index].secondaryOuestions[0].overlook)
                        {
                            groupBPTP.sum += 1;
                        }
                        else
                        {
                            groupBPTP.amountOverlookQuestions++;
                        }


                        if (!Assessment.questions[index].secondaryOuestions[1].overlook)
                        {
                            groupBITP.sum += 1;
                        }
                        else
                        {
                            groupBITP.amountOverlookQuestions++;
                        }

                        if (!Assessment.questions[index].secondaryOuestions[2].overlook)
                        {
                            groupBTPPDn.sum += 1;
                        }
                        else
                        {
                            groupBTPPDn.amountOverlookQuestions++;
                        }
                    }
                    index++;
                    amount++;
                }
                groupBPTP.amountQuestions = amount;
                groupBITP.amountQuestions = amount;
                groupBTPPDn.amountQuestions = amount;
                group.secondaryMi[0] = groupBPTP;
                group.secondaryMi[1] = groupBITP;
                group.secondaryMi[2] = groupBTPPDn;
                group.amountQuestions = amount;
                valueGroup.Add(group);
            }

            for (int i = 7; i <= 34; i++)
            {
                Mi group = new Mi(i);
                int amount = 0;

                while (index < Assessment.questions.Count && i == Assessment.questions[index].group)
                {
                    if (Assessment.questions[index].mandatory)
                    {
                        if (Assessment.questions[index].overlook)
                            group.amountOverlookQuestions++;
                        else
                            group.sum += GetValue(Assessment.questions[index]);
                    }
                    else
                    {
                        if (Assessment.questions[index].overlook)
                            group.amountOverlookQuestions++;
                        else
                            group.sum += 1;
                    }
                    index++;
                    amount++;
                }
                group.amountQuestions = amount;
                valueGroup.Add(group);
            }

            for (int i = 1; i <= valueGroup.Count; i++)
            {
                if (i >= 1 && i <= 6)
                {
                    for (int j = 0; j < valueGroup[i - 1].secondaryMi.Length; j++)
                    {
                        valueGroup[i - 1].secondaryMi[j].CalculateValue();
                    }
                }
                else
                {
                    valueGroup[i - 1].CalculateValue();
                }
            }
        }

        double GetValue(Question question, int indexSecondaryQuestion = -1)
        {
            double result = -1;
            if (indexSecondaryQuestion == -1)
            {
                switch (question.category)
                {
                    case 1:
                        if(question.documentation == 2)
                        {
                            result = 0;
                        }
                        else
                        {
                            switch (question.execution)
                            {
                                case 1:
                                    result = 1;
                                    break;
                                case 2:
                                    result = 0.75;
                                    break;
                                case 3:
                                    result = 0.5;
                                    break;
                                case 4:
                                    result = 0.25;
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case 2:
                        if (question.documentation == 1)
                            result = 1;
                        else if (question.documentation == 2)
                            result = 0;
                        break;
                    case 3:
                        switch (question.execution)
                        {
                            case 1:
                                result = 1;
                                break;
                            case 2:
                                result = 0.5;
                                break;
                            case 3:
                                result = 0;
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (question.category)
                {
                    case 1:
                        if (question.secondaryOuestions[indexSecondaryQuestion].documentation == 2)
                        {
                            result = 0;
                        }
                        else
                        {
                            switch (question.secondaryOuestions[indexSecondaryQuestion].execution)
                            {
                                case 1:
                                    result = 1;
                                    break;
                                case 2:
                                    result = 0.75;
                                    break;
                                case 3:
                                    result = 0.5;
                                    break;
                                case 4:
                                    result = 0.25;
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case 2:
                        if (question.secondaryOuestions[indexSecondaryQuestion].documentation == 1)
                            result = 1;
                        else if (question.secondaryOuestions[indexSecondaryQuestion].documentation == 2)
                            result = 0;
                        break;
                    case 3:
                        switch (question.secondaryOuestions[indexSecondaryQuestion].execution)
                        {
                            case 1:
                                result = 1;
                                break;
                            case 2:
                                result = 0.5;
                                break;
                            case 3:
                                result = 0;
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            log.WriteLog(6);
        }
    }
}
