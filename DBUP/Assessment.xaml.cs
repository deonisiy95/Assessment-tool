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
    /// Логика взаимодействия для Assessment.xaml
    /// </summary>
    public partial class Assessment : Page
    {
        public static List<Question> questions = new List<Question>();  //Список вопросов
        public static List<string> description = new List<string>();    //Текстовое описание трех глобальных направлений оценок
        public static int[] answeredGroup = new int[34];                //Метки отвечена ли группа полностью; 0-не отвечена, 1-отвечена
        public static List<string> descriptionGroup = new List<string>();      //Список текстового описания каждой группы М1-М34
        static List<string> miniDescripton = new List<string>();        //Названия трех глобальных направлений
        static Brush colorAnswered = Brushes.LightGreen;
        static Brush colorNotAnswered = Brushes.Orange;
            //new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6DB2A5"));
        public Assessment(List<Question> newQuestions)
        {
            InitializeComponent();
            questions = newQuestions;
            InitList();                       //Инициализируем ListBox
            LoadDiscription();                //Загружаем описания групп
        }

        //Метод добавления вопроса в нашу панель вопросов, параметры question-сам вопрос, i-номер вопроса в списке вопросов
        DockPanel AddOuestion(Question question, int i)
        {  
            //Создаем отдельную StackPanel для кнопок
            StackPanel stackPanel_Buttons = new StackPanel();
            stackPanel_Buttons.Width = 100;
            
            DockPanel dockPanel = new DockPanel();
            dockPanel.Margin = new Thickness(0, 20, 0, 0);
            //Кнопка, показывающая номер вопроса
            Button button = new Button()
            {
                Content = "M" + question.group + "." + question.number,
                Width = 60,
                Height = 30,
                Margin = new Thickness(5),
                Name = "btn_" + i
            };
            if (question.answered)
                button.Background = colorAnswered;
            else
                button.Background = colorNotAnswered;
            //button.Style = (Style)this.Resources["ButtonTemplate"];
            //Регистрируем имя кнопки, чтобы потом её можно было найти по имени. И добавляем её в панель кнопок
            RegName(button.Name, button);
            stackPanel_Buttons.Children.Add(button);

            StackPanel stackPanelGroup1 = new StackPanel();       //Содержит RadioButton`ы степень документированности
            StackPanel stackPanelGroup2 = new StackPanel();       //Содержит RadioButton`ы степень выполнения

            Grid gridForCheckBox = new Grid();                    //Содержит CheckBox, оценненый вопрос или нет
            RegName("gridForCheckBox" + i, gridForCheckBox);

            //Создаем Grid где будет храниться stackPanelGroup1 и stackPanelGroup2, для этого разбиваем его на две колонки
            ColumnDefinition column1 = new ColumnDefinition();
            ColumnDefinition column2 = new ColumnDefinition();
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(column1);
            grid.ColumnDefinitions.Add(column2);
            //Регистрируем имя grid`а
            RegName("grid" + i, grid);

            //Если мы добавляем вопрос из группы М1-М6, то добаляем еще кнопки для каждого направления БПТП, БИТП и БТППДн
            if (question.group >= 1 && question.group <= 6)
            {
                //Вызываем метод добавления RadioButton`ов, передаем куда будем добавлять их, номер вопроса и метку направления(БПТП-0; БИТП-1; БТППДн-2)
                AddRadioButtons(ref stackPanelGroup1, ref stackPanelGroup2, i, 0);
                //Вызываем метод добавления CheckBox`а, передаем куда будем добавлять, номер вопроса и метку направления
                AddCheckBox(ref gridForCheckBox, i, 0);

                //Создаем и кнопки по каждому направлению, настраиваем св-ва, 
                //если вопрос отвечен красим её в зеленый, вешаем обработчик событий, регистрируем имя и добавляем в панель кнопок
                Button buttonBPTP = new Button()
                {
                    Content = "БПТП",
                    Width = 60,
                    Height = 30,
                    Margin = new Thickness(5),
                    Name = "btn_" + i + "_" + 0,
                    Tag = new int[] {i, 0 },
                    Background = colorNotAnswered
                };
                buttonBPTP.ToolTip = "Банковский платежный технологический процесс";
                if (question.secondaryOuestions[0].answered || question.secondaryOuestions[0].overlook)
                    buttonBPTP.Background = colorAnswered;
                buttonBPTP.Click += new RoutedEventHandler(SecondButtonClick);
                RegName(buttonBPTP.Name, buttonBPTP);
                stackPanel_Buttons.Children.Add(buttonBPTP);


                Button buttonBITP = new Button()
                {
                    Content = "БИТП",
                    Width = 60,
                    Height = 30,
                    Margin = new Thickness(5),
                    Name = "btn_" + i + "_" + 1,
                    Tag = new int[] { i, 1 },
                    Background = colorNotAnswered
                };
                buttonBITP.ToolTip = "Банковский информационный технологический процесс";
                if (question.secondaryOuestions[1].answered || question.secondaryOuestions[1].overlook)
                    buttonBITP.Background = colorAnswered;
                buttonBITP.Click += new RoutedEventHandler(SecondButtonClick);
                RegName(buttonBITP.Name, buttonBITP);
                stackPanel_Buttons.Children.Add(buttonBITP);


                Button buttonBTPPD = new Button()
                {
                    Content = "БТППДн",
                    Width = 60,
                    Height = 30,
                    Margin = new Thickness(5),
                    Name = "btn_" + i + "_" + 2,
                    Tag = new int[] { i, 2 },
                    Background = colorNotAnswered
                };
                buttonBTPPD.ToolTip = "Банковский технологический процесс, в рамках которого обрабатываются персональные данные";
                if (question.secondaryOuestions[2].answered || question.secondaryOuestions[2].overlook)
                    buttonBTPPD.Background = colorAnswered;
                buttonBTPPD.Click += new RoutedEventHandler(SecondButtonClick);
                RegName(buttonBTPPD.Name, buttonBTPPD);
                stackPanel_Buttons.Children.Add(buttonBTPPD);
            }
            else
            {
                //Иначе вызываем методы добавления RadioButton`ов и CheckBox, с пометкой, что это вопрос не содержит поднаправления
                AddRadioButtons(ref stackPanelGroup1, ref stackPanelGroup2, i, -1);
                AddCheckBox(ref gridForCheckBox, i, -1);
            }
            //Создаем StackPanel для вопроса 
            StackPanel stackPanel_Question = new StackPanel();
            stackPanel_Question.Margin = new Thickness(10,0,10,10);
            stackPanel_Question.Orientation = Orientation.Vertical;

            //В TextBlock будет текст вопроса
            TextBlock textBlockQuestion = new TextBlock()
            {
                TextWrapping = TextWrapping.Wrap,
                Text = question.question,
            };

            //Компануем элементы и складываем их в dockPanel
            grid.Children.Add(stackPanelGroup1);
            grid.Children.Add(stackPanelGroup2);

            stackPanel_Question.Children.Add(textBlockQuestion);
            stackPanel_Question.Children.Add(gridForCheckBox);
            stackPanel_Question.Children.Add(grid);
            dockPanel.Children.Add(stackPanel_Buttons);
            dockPanel.Children.Add(stackPanel_Question);

            return dockPanel;
        }

        void AddDescription(int i)
        {
            int indexDescription = 0;
            if (i >= 1 && i <= 10)
                indexDescription = 0;
            if (i > 10 && i <= 27)
                indexDescription = 1;
            if (i > 27)
                indexDescription = 2;

            TextBlock textBlockDescription = new TextBlock()
            {
                TextWrapping = TextWrapping.Wrap,
                Text = miniDescripton[indexDescription],
                FontSize = 20,
                Margin = new Thickness(10)
            };
            stackPanel.Children.Add(textBlockDescription);

            TextBlock textBlockDescription_2 = new TextBlock()
            {
                TextWrapping = TextWrapping.Wrap,
                Text = descriptionGroup[i - 1],
                FontSize = 14,
                FontWeight = FontWeights.Medium,
                Margin = new Thickness(10, 0, 0, 0)
            };
            stackPanel.Children.Add(textBlockDescription_2);
        }
        void RegName(string name, object obj)
        {
            if (FindName(name) != null)
            {
                UnregisterName(name);
            }
            RegisterName(name, obj);
            
                
        }

        void AddCheckBox(ref Grid grid,int i, int indexButton)
        {
            CheckBox checkBox = new CheckBox()
            {
                IsChecked = false,
                Content = "Требования частного показателя не актуальны для организации",
                Margin = new Thickness(10),
                Tag = new int[] { i, indexButton }
            };
            checkBox.Checked += new RoutedEventHandler(checkboxCheckedChange);
            checkBox.Unchecked += new RoutedEventHandler(checkboxCheckedChange);
            grid.Children.Add(checkBox);

            if (indexButton >= 0)
            {
                if (questions[i].secondaryOuestions[indexButton].overlook)
                {
                    CheckBox g = (CheckBox)grid.Children[0];
                    questions[i].secondaryOuestions[indexButton].overlook = false;
                    g.IsChecked = true;
                }
            }
            else
            {
                if (questions[i].overlook)
                {
                    CheckBox g = (CheckBox)grid.Children[0];
                    questions[i].overlook = false;
                    g.IsChecked = true;
                }
            }
        }
        void AddRadioButtons(ref StackPanel stackPanelGroup1, ref StackPanel stackPanelGroup2, int i, int indexButton)
        {
            if (questions[i].mandatory)
            {
                if (questions[i].category == 1)
                {
                    TextBlock textBlockGroup1 = new TextBlock();
                    textBlockGroup1.Text = "Степень документированности";
                    textBlockGroup1.FontWeight = FontWeights.Medium;

                    RadioButton radio1 = new RadioButton();
                    radio1.Content = "Документирован";
                    radio1.Tag = new int[] { i, 1, indexButton };
                    radio1.Checked += new RoutedEventHandler(RadioChekedDocumentationCategory1);
                    RadioButton radio2 = new RadioButton();
                    radio2.Content = "Не документирован";
                    radio2.Checked += new RoutedEventHandler(RadioChekedDocumentationCategory1);
                    radio2.Tag = new int[] { i, 2, indexButton };

                    TextBlock textBlockGroup2 = new TextBlock();
                    textBlockGroup2.Text = "Степень выполнения";
                    textBlockGroup2.FontWeight = FontWeights.Medium;

                    RadioButton r1 = new RadioButton();
                    r1.Content = "Полностью";
                    r1.Tag = new int[] { i, 1, indexButton };
                    r1.Checked += new RoutedEventHandler(RadioChekedExecutionCategory1);
                    RadioButton r2 = new RadioButton();
                    r2.Content = "Почти полностью";
                    r2.Tag = new int[] { i, 2, indexButton };
                    r2.Checked += new RoutedEventHandler(RadioChekedExecutionCategory1);
                    RadioButton r3 = new RadioButton();
                    r3.Content = "Частично";
                    r3.Tag = new int[] { i, 3, indexButton };
                    r3.Checked += new RoutedEventHandler(RadioChekedExecutionCategory1);
                    RadioButton r4 = new RadioButton();
                    r4.Content = "Не выполняется";
                    r4.Tag = new int[] { i, 4, indexButton };
                    r4.Checked += new RoutedEventHandler(RadioChekedExecutionCategory1);


                    stackPanelGroup1.Children.Add(textBlockGroup1);
                    stackPanelGroup1.Children.Add(radio1);
                    stackPanelGroup1.Children.Add(radio2);




                    stackPanelGroup2.Children.Add(textBlockGroup2);
                    stackPanelGroup2.Children.Add(r1);
                    stackPanelGroup2.Children.Add(r2);
                    stackPanelGroup2.Children.Add(r3);
                    stackPanelGroup2.Children.Add(r4);

                    if (indexButton >= 0)
                    {
                        if (questions[i].secondaryOuestions[indexButton].documentation > 0)
                        {
                            RadioButton r = (RadioButton)stackPanelGroup1.Children[questions[i].secondaryOuestions[indexButton].documentation];
                            r.IsChecked = true;
                        }
                        if (questions[i].secondaryOuestions[indexButton].execution > 0)
                        {
                            RadioButton r = (RadioButton)stackPanelGroup2.Children[questions[i].secondaryOuestions[indexButton].execution];
                            r.IsChecked = true;
                        }
                    }
                    else
                    {
                        if (questions[i].documentation > 0)
                        {
                            RadioButton r = (RadioButton)stackPanelGroup1.Children[questions[i].documentation];
                            r.IsChecked = true;
                        }
                        if (questions[i].execution > 0)
                        {
                            RadioButton r = (RadioButton)stackPanelGroup2.Children[questions[i].execution];
                            r.IsChecked = true;
                        }
                    }
                }
                else if (questions[i].category == 2)
                {
                    TextBlock textBlockGroup1 = new TextBlock();
                    textBlockGroup1.Text = "Степень документированности";
                    textBlockGroup1.FontWeight = FontWeights.Medium;

                    RadioButton radio1 = new RadioButton();
                    radio1.Content = "Документирован";
                    radio1.Tag = new int[] { i, 1, indexButton };
                    radio1.Checked += new RoutedEventHandler(RadioChekedDocumentationCategory2);
                    RadioButton radio2 = new RadioButton();
                    radio2.Content = "Не документирован";
                    radio2.Checked += new RoutedEventHandler(RadioChekedDocumentationCategory2);
                    radio2.Tag = new int[] { i, 2, indexButton };

                    stackPanelGroup1.Children.Add(textBlockGroup1);
                    stackPanelGroup1.Children.Add(radio1);
                    stackPanelGroup1.Children.Add(radio2);

                    if (indexButton >= 0)
                    {
                        if (questions[i].secondaryOuestions[indexButton].documentation > 0)
                        {
                            RadioButton r = (RadioButton)stackPanelGroup1.Children[questions[i].secondaryOuestions[indexButton].documentation];
                            r.IsChecked = true;
                        }
                    }
                    else
                    {
                        if (questions[i].documentation > 0)
                        {
                            RadioButton r = (RadioButton)stackPanelGroup1.Children[questions[i].documentation];
                            r.IsChecked = true;
                        }
                    }

                }
                else if (questions[i].category == 3)
                {
                    TextBlock textBlockGroup2 = new TextBlock();
                    textBlockGroup2.Text = "Степень выполнения";
                    textBlockGroup2.FontWeight = FontWeights.Medium;

                    RadioButton r1 = new RadioButton();
                    r1.Content = "Полностью";
                    r1.Tag = new int[] { i, 1, indexButton };
                    r1.Checked += new RoutedEventHandler(RadioChekedExecutionCategory3);
                    RadioButton r2 = new RadioButton();
                    r2.Content = "В неполном объеме";
                    r2.Tag = new int[] { i, 2, indexButton };
                    r2.Checked += new RoutedEventHandler(RadioChekedExecutionCategory3);
                    RadioButton r3 = new RadioButton();
                    r3.Content = "Не выполняется";
                    r3.Tag = new int[] { i, 3, indexButton };
                    r3.Checked += new RoutedEventHandler(RadioChekedExecutionCategory3);

                    stackPanelGroup2.Children.Add(textBlockGroup2);
                    stackPanelGroup2.Children.Add(r1);
                    stackPanelGroup2.Children.Add(r2);
                    stackPanelGroup2.Children.Add(r3);

                    if (indexButton >= 0)
                    {
                        if (questions[i].secondaryOuestions[indexButton].execution > 0)
                        {
                            RadioButton r = (RadioButton)stackPanelGroup2.Children[questions[i].secondaryOuestions[indexButton].execution];
                            r.IsChecked = true;
                        }
                    }
                    else
                    {
                        if (questions[i].execution > 0)
                        {
                            RadioButton r = (RadioButton)stackPanelGroup2.Children[questions[i].execution];
                            r.IsChecked = true;
                        }
                    }

                }
            }
            else
            {
                RadioButton r1 = new RadioButton();

                r1.Content = "Да";
                r1.Tag = new int[] { i, 0, indexButton };
                r1.Margin = new Thickness(10, 0, 0, 0);
                r1.Checked += new RoutedEventHandler(RadioChekedDocumentationCategory2);
                stackPanelGroup1.Children.Add(r1);

                if (indexButton >= 0)
                {
                    if (questions[i].secondaryOuestions[indexButton].documentation >= 0)
                    {
                        RadioButton r = (RadioButton)stackPanelGroup1.Children[questions[i].secondaryOuestions[indexButton].documentation];
                        r.IsChecked = true;
                    }
                }
                else
                {
                    if (questions[i].documentation >= 0)
                    {
                        RadioButton r = (RadioButton)stackPanelGroup1.Children[questions[i].documentation];
                        r.IsChecked = true;
                    }
                }
            }

            Grid.SetColumn(stackPanelGroup1, 0);
            Grid.SetColumn(stackPanelGroup2, 1);
        }
        private void SecondButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int[] tag = (int[])button.Tag;
            int i = tag[0];
            int indexButton = tag[1];
            StackPanel stackPanelGroup1 = new StackPanel();
            StackPanel stackPanelGroup2 = new StackPanel();

            AddRadioButtons(ref stackPanelGroup1, ref stackPanelGroup2, i, indexButton);

            Grid grid = (Grid)FindName("grid" + i);
            grid.IsEnabled = true;
            Grid gridForChecBox = (Grid)FindName("gridForCheckBox" + i);

            gridForChecBox.Children.Clear();
            AddCheckBox(ref gridForChecBox, i, indexButton);

            grid.Children.Clear();
            grid.Children.Add(stackPanelGroup1);
            grid.Children.Add(stackPanelGroup2);
            
        }

        //Метод проверки отвечена ли полностью группа
        bool CheckAnsweredQuestions(int indexGroupQuestion)
        {
            for (int i = 0; i < questions.Count; i++)
            {
                if (questions[i].group == indexGroupQuestion)
                {
                    if (questions[i].overlook)
                        continue;
                    if (!questions[i].answered)
                        return false;
                }
            }
            return true;
        }

        //Метод проверки отвечен ли полностью вопрос из группы М1-М6
        bool CheckAnsweredSecondaryQuestions(int indexQuestion, int indexGroupQuestion)
        {
            //Просматриваем отвечено ли по каждому направлению. Если нет, сразу возвращаем false
            for (int i = 0; i < questions[indexQuestion].secondaryOuestions.Length; i++)
            {
                if (!questions[indexQuestion].secondaryOuestions[i].answered && !questions[indexQuestion].secondaryOuestions[i].overlook)
                    return false;
            }

            //Иначе указываем вопрос как отвечен полностью
            questions[indexQuestion].answered = true;


            //Далее проверяем отвечены ли вопросы из этой же группы, чтобы можно было пометить группу 
            //как полностью заполненую и изменить иконку в ListBox`е
            if(CheckAnsweredQuestions(indexGroupQuestion))
            {
                ChangeImageListBoxItem("check", 1, indexGroupQuestion);
            }
            else
            {
                ChangeImageListBoxItem("attention", 0, indexGroupQuestion);
            }
            return true;
        }
        private void RadioChekedDocumentationCategory1(object sender, RoutedEventArgs e)
        {
            RadioButton r = (RadioButton)sender;
            int[] tag = (int[]) r.Tag;
            if(tag[2] >= 0)
            {
                questions[tag[0]].secondaryOuestions[tag[2]].documentation = tag[1];
                Button button = (Button)FindName("btn_" + tag[0] + "_" + tag[2]);

                if (questions[tag[0]].secondaryOuestions[tag[2]].execution != -1.0 && button != null)
                {
                    button.Background = colorAnswered;
                    questions[tag[0]].secondaryOuestions[tag[2]].answered = true;

                    if (CheckAnsweredSecondaryQuestions(tag[0], questions[tag[0]].group))
                    {
                        Button btn = (Button)FindName("btn_" + tag[0]);
                        btn.Background = colorAnswered;
                    }
                }
            }
            else
            {
                questions[tag[0]].documentation = tag[1];
                Button button = (Button)FindName("btn_" + tag[0]);

                if (questions[tag[0]].execution != -1.0 && button != null)
                {
                    button.Background = colorAnswered;
                    questions[tag[0]].answered = true;
                    if (CheckAnsweredQuestions(questions[tag[0]].group))
                    {
                        ChangeImageListBoxItem("check", 1, questions[tag[0]].group);
                    }
                }
            }
            
            

        }
        private void RadioChekedExecutionCategory1(object sender, RoutedEventArgs e)
        {
            RadioButton r = (RadioButton)sender;
            int[] tag = (int[])r.Tag;
            

            if(tag[2] >= 0)
            {
                questions[tag[0]].secondaryOuestions[tag[2]].execution = tag[1];

                Button button = (Button)FindName("btn_" + tag[0] + "_" + tag[2]);

                if (questions[tag[0]].secondaryOuestions[tag[2]].documentation != -1 && button != null)
                {
                    button.Background = colorAnswered;
                    questions[tag[0]].secondaryOuestions[tag[2]].answered = true;

                    if (CheckAnsweredSecondaryQuestions(tag[0], questions[tag[0]].group))
                    {
                        Button btn = (Button)FindName("btn_" + tag[0]);
                        btn.Background = colorAnswered;
                    }
                }
            }
            else
            {
                questions[tag[0]].execution = tag[1];

                Button button = (Button)FindName("btn_" + tag[0]);

                if (questions[tag[0]].documentation != -1 && button != null)
                {
                    button.Background = colorAnswered;
                    questions[tag[0]].answered = true;
                    if (CheckAnsweredQuestions(questions[tag[0]].group))
                    {
                        ChangeImageListBoxItem("check", 1, questions[tag[0]].group);
                    }
                }
            }

        }
        private void RadioChekedDocumentationCategory2(object sender, RoutedEventArgs e)
        {
            RadioButton r = (RadioButton)sender;
            int[] tag = (int[])r.Tag;
            if (tag[2] >= 0)
            {
                questions[tag[0]].secondaryOuestions[tag[2]].documentation = tag[1];
                Button button = (Button)FindName("btn_" + tag[0] + "_" + tag[2]);
                if(button != null)
                    button.Background = colorAnswered;
                questions[tag[0]].secondaryOuestions[tag[2]].answered = true;


                if (CheckAnsweredSecondaryQuestions(tag[0], questions[tag[0]].group))
                {
                    Button btn = (Button)FindName("btn_" + tag[0]);
                    btn.Background = colorAnswered;
                }

            }
            else
            {
                questions[tag[0]].documentation = tag[1];
                Button button = (Button)FindName("btn_" + tag[0]);
                button.Background = colorAnswered;
                questions[tag[0]].answered = true;
                if (CheckAnsweredQuestions(questions[tag[0]].group))
                {
                    ChangeImageListBoxItem("check", 1, questions[tag[0]].group);
                }
            }


        }
        private void RadioChekedExecutionCategory3(object sender, RoutedEventArgs e)
        {
            RadioButton r = (RadioButton)sender;
            int[] tag = (int[])r.Tag;


            if (tag[2] >= 0)
            {
                questions[tag[0]].secondaryOuestions[tag[2]].execution = tag[1];

                Button button = (Button)FindName("btn_" + tag[0] + "_" + tag[2]);
                if(button != null)
                    button.Background = colorAnswered;
                questions[tag[0]].secondaryOuestions[tag[2]].answered = true;

                if (CheckAnsweredSecondaryQuestions(tag[0], questions[tag[0]].group))
                {
                    Button btn = (Button)FindName("btn_" + tag[0]);
                    btn.Background = colorAnswered;
                }

            }
            else
            {
                questions[tag[0]].execution = tag[1];
                Button button = (Button)FindName("btn_" + tag[0]);
                button.Background = colorAnswered;
                questions[tag[0]].answered = true;
                if (CheckAnsweredQuestions(questions[tag[0]].group))
                {
                    ChangeImageListBoxItem("check", 1, questions[tag[0]].group);
                }

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

            if(indexButton >= 0)
            {
                if (questions[i].secondaryOuestions[indexButton].overlook)
                {
                    g.IsEnabled = true;
                    questions[i].secondaryOuestions[indexButton].overlook = false;

                    if(secondaryButton != null)
                    {
                        if (!questions[i].secondaryOuestions[indexButton].answered)
                            secondaryButton.Background = colorNotAnswered;
                    }

                    if (!CheckAnsweredSecondaryQuestions(i, questions[i].group))
                    {
                        button.Background = colorNotAnswered;
                        ChangeImageListBoxItem("attention", 0, questions[i].group);
                    }
                }
                else
                {
                    g.IsEnabled = false;

                    if (secondaryButton != null)
                    {
                        secondaryButton.Background = colorAnswered;
                    }
                        
                    questions[i].secondaryOuestions[indexButton].overlook = true;

                    if (CheckAnsweredSecondaryQuestions(i, questions[i].group))
                    {
                        button.Background = colorAnswered;
                    }
                }
            }
            else
            {
                if (questions[i].overlook)
                {
                    g.IsEnabled = true;
                    questions[i].overlook = false;

                    if (button != null)
                    {
                        if(!questions[i].answered)
                            button.Background = colorNotAnswered;
                    }
                        
                    if (!CheckAnsweredQuestions(questions[i].group))
                    {
                        ChangeImageListBoxItem("attention", 0, questions[i].group);
                    }
                }
                else
                {
                    g.IsEnabled = false;
                    questions[i].overlook = true;

                    if (button != null)
                        button.Background = colorAnswered;

                    if (CheckAnsweredQuestions(questions[i].group))
                    {
                        ChangeImageListBoxItem("check", 1, questions[i].group);
                    }
                }
            }

            
            
        }

        void ChangeImageListBoxItem(string nameImage,int answered, int indexGroup)
        {
            StackPanel stackPanel = (StackPanel)listbox.SelectedItem;
            Image img = (Image)stackPanel.Children[0];
            img.Source = new BitmapImage(new Uri(String.Format("pack://application:,,,/Image/{0}.png",nameImage)));
            answeredGroup[indexGroup - 1] = answered;
        }
        

        void LoadDiscription()
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("description.xml");
            XmlElement xRoot = xDoc.DocumentElement;

            XmlNodeList childnodes = xRoot.SelectNodes("description");
            foreach (XmlNode n in childnodes)
            {
                description.Add(n.InnerText);
            }

            childnodes = xRoot.SelectNodes("secondary_description");
            foreach (XmlNode n in childnodes)
            {
                miniDescripton.Add(n.InnerText);
            }
            childnodes = xRoot.SelectNodes("description_group");
            foreach (XmlNode n in childnodes)
            {
                descriptionGroup.Add(n.InnerText);
            }

        }
        void InitList()
        {
            listbox.SelectionChanged += new SelectionChangedEventHandler(SelectionChangedListbox);
            AddToListBox("Текущий уровень ИБ организации", 40,-1);
            for (int i = 1; i <= 34; i++)
            {
                if (i == 11)
                    AddToListBox("Менеджмент ИБ организации", 50,-1);
                if (i == 28)
                    AddToListBox("Уровень осознания ИБ организации", 60,-1);
                AddToListBox("Группа \"M" + i + "\"", i, answeredGroup[i-1], true);
            }
        }

        void SelectionChangedListbox(object sender, SelectionChangedEventArgs e)
        {
            StackPanel stack = (StackPanel)listbox.SelectedItem;
            int[] tag = (int [])stack.Tag;
            int indexGroup = tag[0];
            stackPanel.Children.RemoveRange(0,stackPanel.Children.Count);

            if(indexGroup > 34)
            {
                AddDescriptionsPages(indexGroup);
                return;
            }
            else
            {
                AddDescription(indexGroup);
            }

            for (int i = 0; i < questions.Count; i++)
            {
                if(questions[i].group == indexGroup)
                    stackPanel.Children.Add(AddOuestion(questions[i], i));
            }

            
        }

        void AddDescriptionsPages(int indexGroup)
        {
            string text;
            switch (indexGroup)
            {
                case 40:
                    {
                        text = description[0];
                        break;
                    }
                case 50:
                    {
                        text = description[1];
                        break;
                    }
                case 60:
                    {
                        text = description[2];
                        break;
                    }
                default:
                    text = "";
                    break;
            }

            TextBlock textBlockDescription = new TextBlock()
            {
                TextWrapping = TextWrapping.Wrap,
                Text = text,
                FontSize = 15
            };
            stackPanel.Children.Add(textBlockDescription);
        }
        void AddToListBox(string content, int tag, int answered, bool indentation=false)
        {
            BitmapImage bitmap;
            switch (answered)
            {
                case 0:
                    bitmap = new BitmapImage(new Uri("pack://application:,,,/Image/attention.png"));
                    break;
                case 1:
                    bitmap = new BitmapImage(new Uri("pack://application:,,,/Image/check.png"));
                    break;
                case -1:
                    bitmap = new BitmapImage(new Uri("pack://application:,,,/Image/ball.png"));
                    break;
                default:
                    bitmap = new BitmapImage(new Uri("pack://application:,,,/Image/attention.png"));
                    break;
            }
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
                Tag = new int[] { tag, answered }
            };
            if (indentation)
                stp.Margin = new Thickness(10, 0, 0, 0);
            stp.Children.Add(img);
            stp.Children.Add(lbl);
            listbox.Items.Add(stp);
        }
    }
}
