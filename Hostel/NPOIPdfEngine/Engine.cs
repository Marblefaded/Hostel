using NPOI.XWPF.UserModel;
using NPOIPdfEngine.Models;
using Spire.Doc;
using Document = Spire.Doc.Document;


namespace NPOIPdfEngine
{
    public class Engine
    {
        public byte[] CreateReportPdf(ReportModel rModel)
        {
            try
            {
                XWPFDocument doc = new XWPFDocument();
                var probel = doc.CreateParagraph();

                var Rank = doc.CreateParagraph();
                Rank.Alignment = ParagraphAlignment.RIGHT;
                XWPFRun ranc = Rank.CreateRun();
                ranc.FontFamily = "times new roman";
                ranc.FontSize = 12;
                /*r.SetText("Коменданту общежития №1\n" + "Жамьянову И.Ю");*/
                ranc.SetText(rModel.RankOfManagement);

                var p = doc.CreateParagraph();
                p.Alignment = ParagraphAlignment.RIGHT;
                XWPFRun r = p.CreateRun();
                r.FontFamily = "times new roman";
                r.FontSize = 12;
                /*r.SetText("Коменданту общежития №1\n" + "Жамьянову И.Ю");*/
                r.SetText(rModel.NameOfManager);

                var p01 = doc.CreateParagraph();
                p01.Alignment = ParagraphAlignment.RIGHT;
                XWPFRun r01 = p01.CreateRun();
                r01.FontFamily = "times new roman";
                r01.FontSize = 12;
                r01.SetText($"От студента {rModel.Facultet}");
                //r0.SetText($"Студента {rModel.Student}");

                var p02 = doc.CreateParagraph();
                p02.Alignment = ParagraphAlignment.RIGHT;
                XWPFRun r02 = p02.CreateRun();
                r02.FontFamily = "times new roman";
                r02.FontSize = 12;
                r02.SetText($"Группы {rModel.Group}");
                //r0.SetText($"Студента {rModel.Student}");

                var p03 = doc.CreateParagraph();
                p03.Alignment = ParagraphAlignment.RIGHT;
                XWPFRun r03 = p03.CreateRun();
                r03.FontFamily = "times new roman";
                r03.FontSize = 12;
                r03.SetText($"Комнаты№ {rModel.Room}");
                //r0.SetText($"Студента {rModel.Student}");

                var p0 = doc.CreateParagraph();
                p0.Alignment = ParagraphAlignment.RIGHT;
                XWPFRun r0 = p0.CreateRun();
                r0.FontFamily = "times new roman";
                r0.FontSize = 12;
                r0.SetText($"{rModel.Lastname} {rModel.Firstname} {rModel.Middlename}");
                //r0.SetText($"Студента {rModel.Student}");

                var p022 = doc.CreateParagraph();
                p022.Alignment = ParagraphAlignment.RIGHT;
                XWPFRun r022 = p022.CreateRun();
                r022.FontFamily = "times new roman";
                r022.FontSize = 12;
                r022.SetText($"тел. {rModel.Phonenumber}");
                //r0.SetText($"Студента {rModel.Student}");


                var probel1 = doc.CreateParagraph();
                var probel2 = doc.CreateParagraph();
                var probel3 = doc.CreateParagraph();
                var probel4 = doc.CreateParagraph();
                var probel5 = doc.CreateParagraph();
                var probel6 = doc.CreateParagraph();
                var probel7 = doc.CreateParagraph();


                var p1 = doc.CreateParagraph();
                p1.Alignment = ParagraphAlignment.CENTER;
                XWPFRun r1 = p1.CreateRun();
                r1.FontFamily = "times new roman";
                r1.FontSize = 16;
                r1.IsBold = true;
                r1.SetText("Заявление");

                var probel12 = doc.CreateParagraph();

                var p2 = doc.CreateParagraph();
                p2.Alignment = ParagraphAlignment.LEFT;
                p2.IndentationFirstLine = 500;
                XWPFRun r2 = p2.CreateRun();
                r2.FontFamily = "times new roman";
                r2.FontSize = 12;

                rModel.Text = rModel.Text.Replace("[reason]", rModel.Reason)
                    .Replace("[timetogo]", rModel.TimeToGo)
                    .Replace("[timetostart]", rModel.TimeToStart)
                    .Replace("[timetoend]", rModel.TimeToEnd)
                    .Replace("[firstname]", rModel.Firstname)
                    .Replace("[middlename]", rModel.Middlename)
                    .Replace("[lastname]", rModel.Lastname);
                DateTime outResult = new DateTime();
                if (DateTime.TryParse(rModel.Dateofapplication, out outResult))
                {
                    rModel.Text = rModel.Text.Replace("[dateofapplication]",
                            outResult.ToString("dd.MM.yyyy"))
                        .Replace("[dateofapplication+1]",
                            outResult.AddDays(1).ToString("dd.MM.yyyy"));
                }

                r2.SetText($"{rModel.Text}");

                //rModel.Text = rModel.Text.Replace("[reason]", rModel.Reason);


                //r2.SetText($"{rModel.Text}");

                //XWPFRun r26 = p2.CreateRun();
                //r26.FontFamily = "times new roman";
                //r26.FontSize = 12;
                //r26.SetText(" !!!!!");

                //XWPFRun r25 = p2.CreateRun();
                //r25.FontFamily = "times new roman";
                //r25.FontSize = 12;
                //r25.SetText();

                //var probel13 = doc.CreateParagraph();//Так надо?
                //var probel14 = doc.CreateParagraph();
                //var probel15 = doc.CreateParagraph();

                var table = doc.CreateTable(1, 2);
                var tableRow = table.GetRow(0);

                table.SetTopBorder(XWPFTable.XWPFBorderType.NONE, 0, 0, "");
                table.SetBottomBorder(XWPFTable.XWPFBorderType.NONE, 0, 0, "");
                table.SetLeftBorder(XWPFTable.XWPFBorderType.NONE, 0, 0, "");
                table.SetRightBorder(XWPFTable.XWPFBorderType.NONE, 0, 0, "");
                table.SetInsideVBorder(XWPFTable.XWPFBorderType.NONE, 0, 0, "");

                table.Width = 5000;

                var cell1 = tableRow.GetCell(0);
                var pfordate = cell1.AddParagraph();
                pfordate.Alignment = ParagraphAlignment.LEFT;
                var rfordate = pfordate.CreateRun();
                rfordate.FontFamily = "times new roman";
                rfordate.FontSize = 12;
                rfordate.SetText(rModel.Dateofapplication);

                var cell2 = tableRow.GetCell(1);
                var pforfio = cell2.AddParagraph();
                pforfio.Alignment = ParagraphAlignment.RIGHT;
                var rforfio = pforfio.CreateRun();
                rforfio.FontFamily = "times new roman";
                rforfio.FontSize = 12;
                rforfio.SetText($"{rModel.Lastname} {rModel.Firstname} {rModel.Middlename}");

                var probel16 = doc.CreateParagraph();
                var probel17 = doc.CreateParagraph();
                var probel18 = doc.CreateParagraph();
                var probel19 = doc.CreateParagraph();
                var probel20 = doc.CreateParagraph();
                var probel21 = doc.CreateParagraph();
                var probel22 = doc.CreateParagraph();
                var probel23 = doc.CreateParagraph();

                //var p3 = doc.CreateParagraph();
                //p3.Alignment = ParagraphAlignment.LEFT;
                //p3.IndentationFirstLine = 500;
                //XWPFRun r3 = p3.CreateRun();
                //r3.FontFamily = "times new roman";
                //r3.FontSize = 12;
                //rModel.CreateDate = DateTime.Now.Date;
                ////r3.SetText(rModel.CreateDate.ToString("d"));
                //r3.SetText("Дата подачи ФИО Подающего");                



                var probel24 = doc.CreateParagraph();
                var probel25 = doc.CreateParagraph();
                var probel26 = doc.CreateParagraph();
                var probel27 = doc.CreateParagraph();
                var probel28 = doc.CreateParagraph();
                var probel29 = doc.CreateParagraph();
                var probel30 = doc.CreateParagraph();
                var probel31 = doc.CreateParagraph();
                var probel32 = doc.CreateParagraph();
                var probel33 = doc.CreateParagraph();
                var probel34 = doc.CreateParagraph();
                var probel35 = doc.CreateParagraph();
                var probel36 = doc.CreateParagraph();
                var probel37 = doc.CreateParagraph();
                var probel38 = doc.CreateParagraph();
                //var probel39 = doc.CreateParagraph();
                //var probel40 = doc.CreateParagraph();
                //var probel41 = doc.CreateParagraph();
                //var probel42 = doc.CreateParagraph();
                //var probel43 = doc.CreateParagraph();
                //var probel44 = doc.CreateParagraph();
                //var probel45 = doc.CreateParagraph();
                //var probel46 = doc.CreateParagraph();
                //var probel47 = doc.CreateParagraph();
                //var probel48 = doc.CreateParagraph();
                //var probel49 = doc.CreateParagraph();

                //var p3 = doc.CreateParagraph();
                //p3.Alignment = ParagraphAlignment.LEFT;
                //p3.IndentationFirstLine = 500;
                //XWPFRun r3 = p3.CreateRun();
                //r3.FontFamily = "times new roman";
                //r3.FontSize = 12;
                //rModel.CreateDate = DateTime.Now.Date;
                ////r3.SetText(rModel.CreateDate.ToString("d"));
                //r3.SetText("Дата подачи");
                //p3.Alignment = ParagraphAlignment.RIGHT;
                //r3.SetText("ФИО Подающего");


                //doc.Write(fs);
                MemoryStream memory = new MemoryStream();

                doc.Write(memory);
                doc.Close();
                //memory.Position = 0;
                MemoryStream memTemp = new MemoryStream(memory.ToArray());

                Document document = new Document();
                document.LoadFromStream(memTemp, FileFormat.Docx);
                memory.Close();
                memTemp.Close();
                MemoryStream memoryPdf = new MemoryStream();

                document.SaveToStream(memoryPdf, FileFormat.PDF);
                return memoryPdf.ToArray();
            }
            catch (TaskCanceledException e)
            {
                throw e;
            }
            catch (OperationCanceledException e)
            {
                throw e;
            }
            //}
        }

        /*public byte[] ExportToPdf()
        {
            MemoryStream stream = new MemoryStream();
            Document document = new Document();
            document.LoadFromFile(@"D:\Obshaga-update.docx");
            document.SaveToFile(@"D:\Obshaga-pdf.pdf", Spire.Doc.FileFormat.PDF);
            return stream.ToArray();
        }*/

        public void CreateReportWord()
        {
            ReportModel rModel = new ReportModel();
            var newFile = @"D:\Obshaga-update.docx";
            using (var fs = new FileStream(newFile, FileMode.Create, FileAccess.Write))
            {
                XWPFDocument doc = new XWPFDocument();
                var p0 = doc.CreateParagraph();
                p0.Alignment = ParagraphAlignment.RIGHT;
                XWPFRun r0 = p0.CreateRun();
                r0.FontFamily = "times new roman";
                r0.FontSize = 12;
                r0.IsBold = true;
                r0.SetText("Студента");

                var p1 = doc.CreateParagraph();
                p1.Alignment = ParagraphAlignment.CENTER;
                XWPFRun r1 = p1.CreateRun();
                r1.FontFamily = "times new roman";
                r1.FontSize = 16;
                r1.IsBold = true;
                r1.SetText("Заявление");

                var p2 = doc.CreateParagraph();
                p2.Alignment = ParagraphAlignment.LEFT;
                p2.IndentationFirstLine = 500;
                XWPFRun r2 = p2.CreateRun();
                r2.FontFamily = "times new roman";
                r2.FontSize = 12;
                r2.IsBold = true;
                r2.SetText(rModel.Text);
                doc.Write(fs);


            }
        }

    }
}