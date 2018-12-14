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
using System.Windows.Shapes;
using System.Xml;
using System.Net;
using Newtonsoft.Json;

namespace DBUP
{
    /// <summary>
    /// Логика взаимодействия для EnterWindow.xaml
    /// </summary>
    public partial class EnterWindow : Window
    {
        Logging log = new Logging();
        List<User> users = new List<User>();
        byte[] currentKey;
        public EnterWindow()
        {
            InitializeComponent();
            API.initCookie();
            //API.call("auth/tryLogin", "username=den&password=123");
            
          //  API.call("auth/tryLogin", new Dictionary<string, string> {{"username", "den"}, {"password", "123"}});

          //  API.call("global/doStart");
            // API.call("global/doStart");
            //fileinfo info = new fileinfo("users.xml");
            //if (info.exists)
            //{
            //    loadusers();
            //}
            //else
            //{
            //    createfile();
            //}
        }

        private void Register()
        {
            HashAlgorithm hashAlgorithm = HashAlgorithm.Create("SHA512");
            byte[] passwordBytes = Encoding.UTF8.GetBytes(pbxPassword.Password + tbxLogin.Text);
            currentKey = hashAlgorithm.ComputeHash(passwordBytes);
            string keyString = BitConverter.ToString(currentKey).Replace("-", "").ToLower();

            User newUser = new User(tbxLogin.Text, keyString);
            if (!Contains(newUser))
            {
                if(pbxPassword.Password.Length >= 6)
                {
                    try
                    {
                        users.Add(newUser);

                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.Load("users.xml");
                        XmlNode root = xmlDocument.DocumentElement;

                        XmlElement user = xmlDocument.CreateElement("user");
                        XmlElement login = xmlDocument.CreateElement("login");
                        login.InnerText = tbxLogin.Text;
                        XmlElement hash = xmlDocument.CreateElement("hash");
                        hash.InnerText = keyString;
                        user.AppendChild(login);
                        user.AppendChild(hash);

                        root.AppendChild(user);
                        xmlDocument.Save("users.xml");
                        if (MessageBox.Show("Войти в систему?", "Готово", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            log.WriteLog(1, tbxLogin.Text);
                            MainWindow mainWindow = new MainWindow();
                            MainWindow.password = pbxPassword.Password;
                            MainWindow.login = tbxLogin.Text;
                            mainWindow.Show();
                            this.Close();
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Ошибка при чтении файла", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Пароль должен быть больше 6 символов", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void CreateFile()
        {
            try
            {
                StringBuilder text = new StringBuilder();
                XmlTextWriter writer = new XmlTextWriter("users.xml", System.Text.Encoding.UTF8);
                writer.WriteStartDocument(true);
                writer.Formatting = System.Xml.Formatting.Indented;
                writer.Indentation = 2;
                writer.WriteStartElement("Table");

                writer.WriteEndDocument();
                writer.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Что-то пошло не так", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LoadUsers()
        {
            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load("users.xml");
                XmlElement xRoot = xDoc.DocumentElement;
                string login = "";
                string hash = "";

                XmlNodeList childnodes = xRoot.SelectNodes("user");
                foreach (XmlNode n in childnodes)
                {
                    foreach (XmlNode i in n.ChildNodes)
                    {

                        switch (i.Name)
                        {
                            case "login":
                                login = i.InnerText;
                                break;
                            case "hash":
                                hash = i.InnerText;
                                break;
                        }

                    }
                    users.Add(new User(login,hash));
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Нарушена структура файлов", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void btnReg_Click(object sender, RoutedEventArgs e)
        {
            // Register();
            RegistrationWindow registrationWindow = new RegistrationWindow();
            registrationWindow.Owner = this;
            if(registrationWindow.ShowDialog() == true)
            {
                this.Close();
            }
            
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                // введеный логин 
                string login = tbxLogin.Text;

                // веденный пароль
                string password = pbxPassword.Password;

                // отправляем запрос на сервер
                string response_str = API.call("auth/tryLogin", new Dictionary<string, string> { { "username", login }, { "password", password } });

                // преобразуем в объект
                Response<Message> response = JsonConvert.DeserializeObject<Response<Message>>(response_str);

                // если статус ок
                if (response.status == "ok")
                {
                    // открываем главное окно
                    MainWindow mainWindow = new MainWindow(true);
                    mainWindow.Show();

                    // закрываем окно входа
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин/пароль или пользователь не существует", "Внимание", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
            }
            catch(Exception ex)
            {
                log.WriteLog(1,ex.Message);
            }
            
           

        }

        bool Contains(User user)
        {
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].login == user.login && users[i].hash == user.hash)
                    return true;
            }
            return false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }
    }
}
