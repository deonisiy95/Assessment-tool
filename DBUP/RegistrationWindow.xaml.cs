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
using System.Windows.Shapes;
using System.Xml;
using System.Security.Cryptography;

namespace DBUP
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        Logging log = new Logging();
        List<User> users = new List<User>();
        byte[] currentKey;
        public RegistrationWindow()
        {
            InitializeComponent();
            LoadUsers();
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
                    users.Add(new User(login, hash));
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Нарушена структура файлов", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
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
                if (pbxPassword.Password.Length >= 6)
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
                        if (MessageBox.Show("Успешно. Войти в систему?", "Готово", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            log.WriteLog(1, tbxLogin.Text);
                            MainWindow mainWindow = new MainWindow();
                            MainWindow.password = pbxPassword.Password;
                            MainWindow.login = tbxLogin.Text;
                            mainWindow.Show();
                            this.DialogResult = true;
                            this.Close();
                        }
                        else
                        {
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
            else
            {
                MessageBox.Show("Пользователь с таким логином уже зарегистрирован", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void btnReg_Click(object sender, RoutedEventArgs e)
        {
            Register();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
