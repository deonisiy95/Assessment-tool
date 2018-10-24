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
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Pkcs;
using System.Text.RegularExpressions;

namespace DBUP
{
    /// <summary>
    /// Логика взаимодействия для ReportPage.xaml
    /// </summary>
    public partial class ReportPage : Page
    {
        BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        string lavel;
        Logging log = new Logging();
        public ReportPage()
        {
            InitializeComponent();
            FillData();
            LoadCertificates();
        }

        void CreateSignature(string path)
        {
            try
            {
                
                X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certificates = store.Certificates;
                X509Certificate2 certificate = certificates[cbxCertificates.SelectedIndex - 1];
                byte[] bytesFile = File.ReadAllBytes(path);
                File.WriteAllBytes(path + ".sign", Sign(certificate, bytesFile, true));
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                MessageBox.Show("Сертификат не содержит приватный ключ");
            }
            catch(Exception)
            {
                MessageBox.Show("Ошибка при подписании файла");
            }
        }
        public byte[] Sign(X509Certificate2 certificate, byte[] data, bool detached)
        {
            var contentInfo = new ContentInfo(data);
            var signedCms = new SignedCms(contentInfo, detached);

            var cmsSigner = new CmsSigner(SubjectIdentifierType.IssuerAndSerialNumber, certificate);
            signedCms.ComputeSignature(cmsSigner, true);
            return signedCms.Encode();
        }
        void LoadCertificates()
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "Нет";
            cbxCertificates.Items.Add(textBlock);
            cbxCertificates.SelectedIndex = 0;

            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certificates = store.Certificates;
            foreach (X509Certificate2 item in certificates)
            {
                
                textBlock = new TextBlock();
                textBlock.Text = Regex.Match(item.Subject, @"(?<=CN=)[\w\s]+(?![ ])").Value;
                cbxCertificates.Items.Add(textBlock);
            }
        }
        void FillData()
        {
            tbEV1.Text += " = " + MainWindow.EV_1;
            tbEV2.Text += " = " + MainWindow.EV_2;
            tbEV3.Text += " = " + MainWindow.EV_3;
            tbEV_OOPD.Text += " = " + MainWindow.EV_OOPD;
            tbEV1_OZPD.Text += " = " + MainWindow.EV_OZPD_1;
            tbEV2_OZPD.Text += " = " + MainWindow.EV_OZPD_2;
            tbEV_BITP.Text += " = " + MainWindow.EV_BITP;
            tbEV_BPTP.Text += " = " + MainWindow.EV_BPTP;
            double R = MainWindow.EV_R;
            lavel = "";
            if (R >= 0 && R < 0.25)
                lavel = "нулевой";
            else if (R >= 0.25 && R < 0.5)
                lavel = "первый";
            else if (R >= 0.5 && R < 0.7)
                lavel = "второй";
            else if (R >= 0.7 && R < 0.85)
                lavel = "третий";
            else if (R >= 0.85 && R < 0.95)
                lavel = "четвертый";
            else if (R >= 0.95 && R <= 1)
                lavel = "пятый";
            tbEV_R.Text += String.Format(" = {0} ({1} уровень соответствия ИБ требованиям СТО БР ИББС-1.0)", R, lavel);
        }

        private void btnSaveReport_Click(object sender, RoutedEventArgs e)
        {
            SaveReport();
            log.WriteLog(5);
        }

        void SaveReport()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Pdf файл | * .pdf";
            if (saveFileDialog.ShowDialog() == true)
            {
                Canvas mainCanvas = new Canvas();
                mainCanvas.Background = Brushes.White;
                mainCanvas.Width = 550;
                mainCanvas.Height = 550;
                Diagram diagram = new Diagram(280, 280, 500, mainCanvas);
                DiagramPage.PrepareDataForDiagram();
                

                var doc = new Document();
                
                PdfWriter.GetInstance(doc, new FileStream(saveFileDialog.FileName, FileMode.Create));
                doc.Open();

                Phrase phrase = new Phrase("РЕЗУЛЬТАТЫ ОЦЕНКИ СООТВЕТСТВИЯ ИНФОРМАЦИОННОЙ БЕЗОПАСНОСТИ " + MainPage.assessmentData["NameAssessment"] + " TРЕБОВАНИЯМ СТО БР ИББС-1.0-2014\n\n",
                new iTextSharp.text.Font(baseFont, 8, iTextSharp.text.Font.BOLD));

                doc.Add(phrase);
                phrase = new Phrase("1. ОБЛАСТЬ ОЦЕНКИ",
                  new iTextSharp.text.Font(baseFont, 8, iTextSharp.text.Font.BOLD));
                doc.Add(phrase);

                iTextSharp.text.Paragraph p = new iTextSharp.text.Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, new BaseColor(System.Drawing.Color.Black), Element.ALIGN_LEFT, 1)));
                doc.Add(p);

                phrase = new Phrase(String.Format("       ● {0} {1}\n\n", MainPage.assessmentData["NameObject"], MainPage.assessmentData["Address"]),
                  new iTextSharp.text.Font(baseFont, 8, iTextSharp.text.Font.NORMAL));
                doc.Add(phrase);

                phrase = new Phrase("2. РЕЗУЛЬТАТЫ ОЦЕНКИ",
                  new iTextSharp.text.Font(baseFont, 8, iTextSharp.text.Font.BOLD));
                doc.Add(phrase);

                p = new iTextSharp.text.Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, new BaseColor(System.Drawing.Color.Black), Element.ALIGN_LEFT, 1)));
                doc.Add(p);

                phrase = new Phrase(String.Format("Итоговый уровень соответствия ИБ организации БС РФ требованиям СТО БР ИББС - 1.0: {0} \n\n",lavel), new iTextSharp.text.Font(baseFont, 8, iTextSharp.text.Font.NORMAL));
                doc.Add(phrase);

                if (chDirection.IsChecked == true)
                {
                    phrase = new Phrase("Оценки по направлениям обеспечения ИБ:\n", new iTextSharp.text.Font(baseFont, 8, iTextSharp.text.Font.BOLD));
                    doc.Add(phrase);
                    string param;

                    PdfPTable table = new PdfPTable(3);
                    table.TotalWidth = 525f;
                    table.LockedWidth = true;
                    table.SetWidths(new float[] { 12f, 80f, 10f });

                    table.AddCell(GetTitleCell("Обозначение\nоценки"));
                    table.AddCell(GetTitleCell("Наименование оценки"));
                    table.AddCell(GetTitleCell("Значение\nоценки"));

                    table.AddCell(GetCellAlign("EVбитп"));
                    table.AddCell(GetCell("Оценка степени выполнения требований СТО БР ИББС-1.0, регламентирующих банковский информационный технологический процесс"));
                    param = MainWindow.EV_BITP.ToString();
                    if (param.Length > 5)
                        param = param.Substring(0, 5);
                    table.AddCell(GetCellAlign(param));

                    table.AddCell(GetCellAlign("EVбптп"));
                    table.AddCell(GetCell("Оценка степени выполнения требований СТО БР ИББС-1.0, регламентирующих банковский платежный технологический процесс"));
                    param = MainWindow.EV_BPTP.ToString();
                    if (param.Length > 5)
                        param = param.Substring(0, 5);
                    table.AddCell(GetCellAlign(param));

                    table.AddCell(GetCellAlign("EV1опзд"));
                    table.AddCell(GetCell("Оценка степени выполнения требований СТО БР ИББС-1.0, регламентирующих защиту ПД, без учета оценки степени выполнения требований СТО БР ИББС - 1.0 по обеспечению ИБ при использовании СКЗИ"));
                    param = MainWindow.EV_OZPD_1.ToString();
                    if (param.Length > 5)
                        param = param.Substring(0, 5);
                    table.AddCell(GetCellAlign(param));

                    table.AddCell(GetCellAlign("EV2опзд"));
                    table.AddCell(GetCell("Оценка степени выполнения требований СТО БР ИББС-1.0, регламентирующих защиту ПД, с учетом оценки степени выполнения требований СТО БР ИББС - 1.0 по обеспечению ИБ при использовании СКЗИ"));
                    param = MainWindow.EV_OZPD_2.ToString();
                    if (param.Length > 5)
                        param = param.Substring(0, 5);
                    table.AddCell(GetCellAlign(param));

                    table.AddCell(GetCellAlign("EVоопд"));
                    table.AddCell(GetCell("Оценка степени выполнения требований СТО БР ИББС-1.0, регламентирующих обработку персональных данных"));
                    param = MainWindow.EV_OOPD.ToString();
                    if (param.Length > 5)
                        param = param.Substring(0, 5);
                    table.AddCell(GetCellAlign(param));

                    table.AddCell(GetCellAlign("EV1"));
                    table.AddCell(GetCell("Оценка степени выполнения требований СТО БР ИББС-1.0 по направлению “текущий уровень ИБ организации””"));
                    param = MainWindow.EV_1.ToString();
                    if (param.Length > 5)
                        param = param.Substring(0, 5);
                    table.AddCell(GetCellAlign(param));

                    table.AddCell(GetCellAlign("EV2"));
                    table.AddCell(GetCell("Оценка степени выполнения требований СТО БР ИББС-1.0 по направлению “менеджмент ИБ организации”"));
                    param = MainWindow.EV_2.ToString();
                    if (param.Length > 5)
                        param = param.Substring(0, 5);
                    table.AddCell(GetCellAlign(param));

                    table.AddCell(GetCellAlign("EV3"));
                    table.AddCell(GetCell("Ооценка степени выполнения требований СТО БР ИББС-1.0 по направлению “уровень осознания ИБ организации””"));
                    param = MainWindow.EV_3.ToString();
                    if (param.Length > 5)
                        param = param.Substring(0, 5);
                    table.AddCell(GetCellAlign(param));

                    table.AddCell(GetCellAlign("R"));
                    table.AddCell(GetCell("Итоговый уровень соответствия ИБ организации БС РФ требованиям СТО БР ИББС - 1.0"));
                    param = MainWindow.EV_R.ToString();
                    if (param.Length > 5)
                        param = param.Substring(0, 5);
                    table.AddCell(GetCellAlign(param));

                    doc.Add(table);
                }

                if (chbGroup.IsChecked == true)
                {
                    string param = "";
                    phrase = new Phrase("\nОценки групповых показателей ИБ:\n", new iTextSharp.text.Font(baseFont, 8, iTextSharp.text.Font.BOLD));
                    doc.Add(phrase);

                    PdfPTable table = new PdfPTable(5);
                    table.TotalWidth = 525f;
                    table.LockedWidth = true;
                    table.SetWidths(new float[] { 12f, 60f, 10f, 10f, 10f });

                    table.AddCell(GetTitleCell("Код"));
                    table.AddCell(GetTitleCell("Наименование показателя"));
                    table.AddCell(GetTitleCell("Оценка\nБПТП"));
                    table.AddCell(GetTitleCell("Оценка\nБИТП"));
                    table.AddCell(GetTitleCell("Оценка\nБТППДн"));

                    for (int i = 0; i < 6; i++)
                    {
                        table.AddCell(GetCellAlign(String.Format("M{0}",i+1)));
                        table.AddCell(GetCell(Assessment.descriptionGroup[i].Substring(13, Assessment.descriptionGroup[i].Length-14)));
                        param = DiagramPage.paramsBPTP_Diagram[i].ToString();
                        if (param.Length > 5)
                            param = param.Substring(0, 5);
                        table.AddCell(GetCellAlign(param));
                        param = DiagramPage.paramsBITP_Diagram[i].ToString();
                        if (param.Length > 5)
                            param = param.Substring(0, 5);
                        table.AddCell(GetCellAlign(param));
                        param = DiagramPage.paramsBTPPDn_Diagram[i].ToString();
                        if (param.Length > 5)
                            param = param.Substring(0, 5);
                        table.AddCell(GetCellAlign(param));
                    }

                    doc.Add(table);

                    table = new PdfPTable(3);
                    table.TotalWidth = 525f;
                    table.LockedWidth = true;
                    table.SetWidths(new float[] { 12f, 80f, 10f });

                    table.AddCell(GetTitleCell("Код"));
                    table.AddCell(GetTitleCell("Наименование показателя"));
                    table.AddCell(GetTitleCell("Оценка"));

                    for (int i = 6; i < 34; i++)
                    {
                        table.AddCell(GetCellAlign(String.Format("M{0}", i + 1)));
                        table.AddCell(GetCell(Assessment.descriptionGroup[i].Substring(14, Assessment.descriptionGroup[i].Length - 15)));
                        param = DiagramPage.paramsBTPPDn_Diagram[i].ToString();
                        if (param.Length > 5)
                            param = param.Substring(0, 5);
                        table.AddCell(GetCellAlign(param));
                    }

                    doc.Add(table);
                }

                if (chDiagramBPTP.IsChecked == true)
                {
                    InsertDiagram(doc, mainCanvas, diagram, "Диаграмма соответствия ИБ групповых показателей  (БПТБ) :", DiagramPage.paramsBPTP_Diagram);
                }

                if (chDiagramBITP.IsChecked == true)
                {
                    InsertDiagram(doc, mainCanvas, diagram, "Диаграмма соответствия ИБ групповых показателей (БИТБ) :", DiagramPage.paramsBITP_Diagram);
                }

                if (chDiagramBTPPDn.IsChecked == true)
                {
                    InsertDiagram(doc, mainCanvas, diagram, "Диаграмма соответствия ИБ групповых показателей (БТППДн) :", DiagramPage.paramsBTPPDn_Diagram);
                }

                if (chDiagramDirection.IsChecked == true)
                {
                    doc.Add(new Phrase("\n"));

                    PdfPTable table = new PdfPTable(1);
                    table.TotalWidth = 200f;
                    table.LockedWidth = true;
                    table.SetWidths(new float[] { 200f });

                    PdfPCell cell = new PdfPCell(new Phrase("Диаграмма соответствия ИБ по направлениям :", new iTextSharp.text.Font(baseFont, 8, iTextSharp.text.Font.BOLD)));
                    cell.Padding = 5;
                    cell.Colspan = 3;
                    cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);

                    mainCanvas.Children.Clear();
                    diagram.DrawEvDiagram(new double[] { MainWindow.EV_1, MainWindow.EV_2, MainWindow.EV_3 });
                    cell = new PdfPCell(SaveCanvasToPDF(mainCanvas));
                    cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    table.AddCell(cell);

                    doc.Add(table);

                }

                if (chDiagramResult.IsChecked == true)
                {
                    doc.Add(new Phrase("\n\n"));

                    PdfPTable table = new PdfPTable(1);
                    table.TotalWidth = 200f;
                    table.LockedWidth = true;
                    table.SetWidths(new float[] { 200f });

                    PdfPCell cell = new PdfPCell(new Phrase("Итоговая диаграмма :", new iTextSharp.text.Font(baseFont, 8, iTextSharp.text.Font.BOLD)));
                    cell.Padding = 5;
                    cell.Colspan = 3;
                    cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);

                    mainCanvas.Children.Clear();
                    diagram.DrawResultDiagram(MainWindow.EV_R);
                    cell = new PdfPCell(SaveCanvasToPDF(mainCanvas));
                    cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    table.AddCell(cell);

                    doc.Add(table);
                }

                doc.Close();

                if(cbxCertificates.SelectedIndex > 0)
                {
                    CreateSignature(saveFileDialog.FileName);
                }
            }
        }

        void InsertDiagram(iTextSharp.text.Document doc, Canvas mainCanvas, Diagram diagram, string text, double[] paramsDiagram)
        {
            doc.Add(new Phrase("\n"));

            PdfPTable table = new PdfPTable(1);
            table.TotalWidth = 200f;
            table.LockedWidth = true;
            table.SetWidths(new float[] { 200f });
            
            PdfPCell cell = new PdfPCell(new Phrase(text, new iTextSharp.text.Font(baseFont, 8, iTextSharp.text.Font.BOLD)));
            cell.Padding = 5;
            cell.Colspan = 3;
            cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);

            mainCanvas.Children.Clear();
            diagram.DrawGroupDiagram(paramsDiagram);
            cell = new PdfPCell(SaveCanvasToPDF(mainCanvas));
            cell.Border = iTextSharp.text.Rectangle.NO_BORDER; 
            table.AddCell(cell);

            doc.Add(table);
        }

        PdfPCell GetTitleCell(string text)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, new iTextSharp.text.Font(baseFont, 8, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black))));
            cell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            return cell;
        }

        PdfPCell GetCellAlign(string text)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, new iTextSharp.text.Font(baseFont, 8, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black))));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Padding = 5;
            return cell;
        }
        PdfPCell GetCell(string text)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, new iTextSharp.text.Font(baseFont, 8, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black))));
            cell.Padding = 5;
            return cell;
        }
        public static iTextSharp.text.Image SaveCanvasToPDF(Canvas surface)
        {
            surface.LayoutTransform = null;
            Size size = new Size(surface.Width, surface.Height);
            surface.Measure(size);
            surface.Arrange(new Rect(size));
            surface.UpdateLayout();

            RenderTargetBitmap renderBitmap =
              new RenderTargetBitmap(
                (int)size.Width,
                (int)size.Height,
                96d,
                96d,
                PixelFormats.Pbgra32);
            renderBitmap.Render(surface);

            using (FileStream outStream = new FileStream("temp.png", FileMode.Create))
            {
                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                encoder.Save(outStream);
                
            }
            
            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance("temp.png");
            image.ScaleAbsolute(200f, 200f);
            image.Alignment = Element.ALIGN_CENTER;
            return image;
        }
    }
}
