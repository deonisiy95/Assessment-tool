using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml;

namespace DBUP
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public static Dictionary<string, string> assessmentData;
        public static string path;
        public MainPage()
        {
            InitializeComponent();
            assessmentData = new Dictionary<string, string>();
            path = "";
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (tbxNameAssessment.Text != "" && tbxNameObject.Text != "" && tbxAddress.Text != "" && tbxAuditor.Text != "")
            {
                MainWindow mainWindow = (MainWindow)DBUP.App.Current.Windows[0];
                mainWindow.btnDiagram.IsEnabled = true;
                mainWindow.btnResult.IsEnabled = true;
                mainWindow.btnAssessment.IsEnabled = true;

                

                assessmentData.Add("NameAssessment", tbxNameAssessment.Text);
                assessmentData.Add("NameObject", tbxNameObject.Text);
                assessmentData.Add("Address", tbxAddress.Text);
                assessmentData.Add("Auditor", tbxAuditor.Text);
                Assessment.answeredGroup = new int[34];

                List<Question> list = new List<Question>();
                LoadQuestions(ref list);
                Assessment assessment = new Assessment(list);
                mainWindow.frame.NavigationService.Navigate(assessment);
            }
            else
            {
                MessageBox.Show("Введите данные", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        
        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            
            if(path != "")
            {
                List<Question> list = new List<Question>();
                if(OpenQuestions(ref list, path))
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
                MessageBox.Show("Выберите файл", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            
        }

        bool OpenQuestions(ref List<Question> questions, string path)
        {
            try
            {
                byte[] bytesFile = File.ReadAllBytes(path);
                MemoryStream ms = new MemoryStream(Decrypt(bytesFile, MainWindow.password+MainWindow.login));
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(ms);
                XmlElement xRoot = xDoc.DocumentElement;
                string value = "";
                int group = 0;
                int number = 0;
                int category = 0;
                bool mandatory = false;
                bool answered = false;
                bool overlook = false;
                string question = "";
                int documentation = -1;
                int execution = -1;
                bool secondaryAnswered = false;
                bool secondaryOverlook = false;
                int secondaryDocumentation = -1;
                int secondaryExecution = -1;
                SecondaryOuestion secondaryOuestion_1 = new SecondaryOuestion();
                SecondaryOuestion secondaryOuestion_2 = new SecondaryOuestion();
                SecondaryOuestion secondaryOuestion_3 = new SecondaryOuestion();

                XmlNodeList childnodes = xRoot.SelectNodes("question");
                foreach (XmlNode n in childnodes)
                {
                    foreach (XmlNode i in n.ChildNodes)
                    {

                        switch (i.Name)
                        {
                            case "group":
                                group = int.Parse(i.InnerText);
                                break;
                            case "number":
                                number = int.Parse(i.InnerText);
                                break;
                            case "category":
                                category = int.Parse(i.InnerText);
                                break;
                            case "mandatory":
                                mandatory = bool.Parse(i.InnerText);
                                break;
                            case "answered":
                                answered = bool.Parse(i.InnerText);
                                break;
                            case "overlook":
                                overlook = bool.Parse(i.InnerText);
                                break;
                            case "question_value":
                                question = i.InnerText;
                                break;
                            case "documentation":
                                documentation = int.Parse(i.InnerText);
                                break;
                            case "execution":
                                execution = int.Parse(i.InnerText);
                                break;
                            case "secondaryOuestion1":
                                {
                                    foreach (XmlNode j in i.ChildNodes)
                                    {
                                        switch (j.Name)
                                        {
                                            case "answered":
                                                secondaryAnswered = bool.Parse(j.InnerText);
                                                break;
                                            case "overlook":
                                                secondaryOverlook = bool.Parse(j.InnerText);
                                                break;
                                            case "documentation":
                                                secondaryDocumentation = int.Parse(j.InnerText);
                                                break;
                                            case "execution":
                                                secondaryExecution = int.Parse(j.InnerText);
                                                break;
                                        }
                                    }
                                    secondaryOuestion_1 = new SecondaryOuestion(secondaryDocumentation, secondaryExecution, secondaryAnswered, secondaryOverlook);
                                    break;
                                }
                            case "secondaryOuestion2":
                                {
                                    foreach (XmlNode j in i.ChildNodes)
                                    {
                                        switch (j.Name)
                                        {
                                            case "answered":
                                                secondaryAnswered = bool.Parse(j.InnerText);
                                                break;
                                            case "overlook":
                                                secondaryOverlook = bool.Parse(j.InnerText);
                                                break;
                                            case "documentation":
                                                secondaryDocumentation = int.Parse(j.InnerText);
                                                break;
                                            case "execution":
                                                secondaryExecution = int.Parse(j.InnerText);
                                                break;
                                        }
                                    }
                                    secondaryOuestion_2 = new SecondaryOuestion(secondaryDocumentation, secondaryExecution, secondaryAnswered, secondaryOverlook);
                                    break;
                                }
                            case "secondaryOuestion3":
                                {
                                    foreach (XmlNode j in i.ChildNodes)
                                    {
                                        switch (j.Name)
                                        {
                                            case "answered":
                                                secondaryAnswered = bool.Parse(j.InnerText);
                                                break;
                                            case "overlook":
                                                secondaryOverlook = bool.Parse(j.InnerText);
                                                break;
                                            case "documentation":
                                                secondaryDocumentation = int.Parse(j.InnerText);
                                                break;
                                            case "execution":
                                                secondaryExecution = int.Parse(j.InnerText);
                                                break;
                                        }
                                    }
                                    secondaryOuestion_3 = new SecondaryOuestion(secondaryDocumentation, secondaryExecution, secondaryAnswered,secondaryOverlook);
                                    break;
                                }

                        }

                    }
                    Question newQuestion = new Question(group, number, question, mandatory, category, overlook, documentation, execution, answered);
                    newQuestion.secondaryOuestions = new SecondaryOuestion[] { secondaryOuestion_1, secondaryOuestion_2, secondaryOuestion_3 };
                    questions.Add(newQuestion);
                }

                XmlNodeList assessmentDataXml = xRoot.SelectNodes("assessmentData");
                foreach (XmlNode node in assessmentDataXml)
                {
                    foreach (XmlNode i in node.ChildNodes)
                        assessmentData.Add(i.Name, i.InnerText);
                }
                XmlNodeList answeredGroup = xRoot.SelectNodes("answeredGroup");
                foreach (XmlNode node in answeredGroup)
                {
                    foreach (XmlNode i in node.ChildNodes)
                        Assessment.answeredGroup[int.Parse(i.Name.Replace("item",""))] = int.Parse(i.InnerText);
                }
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("Не удается прочитать файл. Возможные причины: \n -Файл не является файлом сесии \n -Вы не владелец \n -Файл поврежден", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
        }

        void LoadQuestions(ref List<Question> questions)
        {
            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load("question.xml");
                XmlElement xRoot = xDoc.DocumentElement;
                string value = "";
                int group = 0;
                int number = 0;
                int category = 0;
                bool mandatory = false;

                XmlNodeList childnodes = xRoot.SelectNodes("question");
                foreach (XmlNode n in childnodes)
                {
                    foreach (XmlNode i in n.ChildNodes)
                    {

                        switch (i.Name)
                        {
                            case "name":
                                number = int.Parse(i.InnerText.Split('.')[1]);
                                group = int.Parse(i.InnerText.Split('.')[0].Substring(1));
                                break;
                            case "value":
                                value = i.InnerText;
                                break;
                            case "category":
                                category = int.Parse(i.InnerText);
                                break;
                            case "mandatory":
                                if (i.InnerText == "true")
                                    mandatory = true;
                                else
                                    mandatory = false;
                                break;
                        }

                    }
                    questions.Add(new Question(group, number, value, mandatory, category));
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Нарушена структура файлов", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                DBUP.App.Current.MainWindow.Close();
            }
        }

        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            if (open.ShowDialog() == true)
            {
                txbOpenFile.Text = open.FileName;
                path = open.FileName;
            }
        }

        static byte[] Decrypt(byte[] cyphertext, string key)
        {
            SymmetricAlgorithm algorithm = SymmetricAlgorithm.Create("AES");
            HashAlgorithm hashAlgorithm = HashAlgorithm.Create("MD5");
            algorithm.Key = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(key));
            algorithm.IV = new byte[16];
            algorithm.Mode = CipherMode.CBC;
            algorithm.Padding = PaddingMode.ANSIX923;
            MemoryStream ms = new MemoryStream(cyphertext);
            CryptoStream cs = new CryptoStream(ms, algorithm.CreateDecryptor(), CryptoStreamMode.Read);
            byte[] buffer = new byte[cyphertext.Length];
            int readCount = cs.Read(buffer, 0, cyphertext.Length);
            byte[] result = new byte[readCount];
            Array.Copy(buffer, result, readCount);
            cs.Close();
            ms.Close();

            return result;
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
    }
}
