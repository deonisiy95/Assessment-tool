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

namespace DBUP
{
    /// <summary>
    /// Логика взаимодействия для DocumentBlankPage.xaml
    /// </summary>
    public partial class DocumentBlankPage : Page
    {
        public static List<DocQuestion> document_poll = new List<DocQuestion>();
        public DocumentBlankPage()
        {
            InitializeComponent();

            LoadDocumentQuestion();

            renderBlank();
        }

        public void renderBlank()
        {

            for (int i = 0; i < document_poll.Count; i ++)
            {
                DockPanel dockPanel = new DockPanel();
                StackPanel stackPanel = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 5, 0 ,5),
                };

                CheckBox checkBox = new CheckBox()
                {
                    IsChecked = false,
                    //Content = document_poll[i].value
                    // Tag = new int[] { i, indexButton }
                };

                TextBlock textBlockSymbol = new TextBlock()
                {
                    Text = document_poll[i].symbol,
                    TextAlignment = TextAlignment.Center,
                    Width = 30
                };

                //В TextBlock будет текст вопроса
                TextBlock textBlockQuestion = new TextBlock()
                {
                    TextWrapping = TextWrapping.Wrap,
                    Text = document_poll[i].value,
                    Width = 800
                };

                if (document_poll[i].id == -1)
                {
                    textBlockQuestion.FontWeight = FontWeight.FromOpenTypeWeight(600);
                    textBlockSymbol.FontWeight = FontWeight.FromOpenTypeWeight(600);
                    textBlockQuestion.FontSize = 14;
                    textBlockSymbol.FontSize = 14;
                    stackPanel.Children.Add(textBlockSymbol);
                    stackPanel.Children.Add(textBlockQuestion);
                } else
                {
                    stackPanel.Children.Add(checkBox);
                    stackPanel.Children.Add(textBlockSymbol);
                    stackPanel.Children.Add(textBlockQuestion);
                }
                
                checkBox.Checked += new RoutedEventHandler(checkboxCheckedChange);
                checkBox.Unchecked += new RoutedEventHandler(checkboxCheckedChange);

                dockPanel.Children.Add(stackPanel);
                stackPanelBlank.Children.Add(dockPanel);
            }
          
    
        }

        public void LoadDocumentQuestion()
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("documents.xml");
            XmlElement xRoot = xDoc.DocumentElement;

            int id = 0;
            string symbol = "";
            string value = "";

            XmlNodeList childnodes = xRoot.SelectNodes("question");
            foreach (XmlNode n in childnodes)
            {
                foreach (XmlNode i in n.ChildNodes)
                {
                    switch (i.Name)
                    {
                        case "id":
                            id = int.Parse(i.InnerText);
                            break;
                        case "symbol":
                            symbol = i.InnerText.ToString();
                            break;
                        case "value":
                            value = i.InnerText.ToString();
                            break;
                    }
                }

                document_poll.Add(new DocQuestion(id, symbol, value));
            }
        }



            void checkboxCheckedChange(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            int[] tag = (int[])checkBox.Tag;
            int i = tag[0];
            int indexButton = tag[1];
            Button secondaryButton = (Button)FindName("btn_" + i + "_" + indexButton);
            Button button = (Button)FindName("btn_" + i);
            Grid g = (Grid)FindName("grid" + i);



        }
    }
}
