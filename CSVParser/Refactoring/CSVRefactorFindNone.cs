using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using Jitbit.Utils;
using System.Windows.Forms;

namespace CSVParser
{
    partial class CSVRefactor
    {
        protected async void FindNoneField(string csvSourseFilePath, string newPath, int columnIndex, Action<string, RefactorType, int, bool, ParserError> onFinish)
        {
            string logFile = LogParserProgress.GetNewLogFilePath(Path.GetFileNameWithoutExtension(csvSourseFilePath) + "_FindNoneField");
            string csvFilePath = MoveInTemp(csvSourseFilePath);
            ParserError error = null;
            IsProcessing = true;
            int foundEmptysCount = 0;
            var t = Task.Run(async () =>
            {
                FileInfo csvFileInfo = new FileInfo(csvFilePath);

                // Find repeatCount
                string tempPath = Path.GetTempFileName();

                List<double?> dataLineList = new List<double?>();

                using (TextFieldParser csvParser = new TextFieldParser(csvFilePath))
                {

                    refactoringSoureseDispose = csvParser;
                    csvParser.TextFieldType = FieldType.Delimited;
                    csvParser.SetDelimiters(",");
                    bool isNotFirst = false;

                    while (!csvParser.EndOfData)
                    {
                        string[] fields = csvParser.ReadFields();
                        if (!isNotFirst)
                        {
                            isNotFirst = true;
                        }
                        else
                        {
                            try
                            {
                                double? value = null;
                                if (!string.IsNullOrEmpty(fields[columnIndex]) && fields[columnIndex].ToLower() != "none")
                                {
                                    value = Double.Parse(fields[columnIndex], System.Globalization.CultureInfo.InvariantCulture);
                                }

                                dataLineList.Add(value);
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.Message);
                                error = new ParserError(e.Message);
                                refactoringSoureseDispose?.Dispose();
                                refactoringTempDispose?.Dispose();
                                break;
                            }

                        }
                        Debug("ProcessedLine: " + csvParser.LineNumber, append: false);
                        await Task.Yield();
                    }
                }
                InvokeProgressBarMaxValue((int)csvFileInfo.Length);

                for (int row = 0; row < dataLineList.Count; row++)
                {
                    if (dataLineList[row]== null)
                    {
                        double? upValue = null;
                        double? downValue = null;
                        for (int i = row; i >= 0; i--)
                        {
                            if (dataLineList[i].HasValue)
                            {
                                upValue = dataLineList[i].Value;
                                break;
                            }
                        }
                        for (int i = row; i < dataLineList.Count; i++)
                        {
                            if (dataLineList[i].HasValue)
                            {
                                downValue = dataLineList[i].Value;
                                break;
                            }
                        }
                        if (!upValue.HasValue && downValue.HasValue)
                        {
                            for (int i = row; i < dataLineList.Count; i++)
                            {
                                if (dataLineList[i].HasValue && dataLineList[i].Value != downValue.Value)
                                {
                                    upValue = dataLineList[i].Value;
                                    break;
                                }
                            }
                            if (!upValue.HasValue)
                            {
                                upValue = downValue;
                            }
                        }
                        else if (!downValue.HasValue && upValue.HasValue)
                        {
                            for (int i = row; i >= 0; i--)
                            {
                                if (dataLineList[i].HasValue && dataLineList[i].Value != upValue.Value)
                                {
                                    downValue = dataLineList[i].Value;
                                    break;
                                }
                            }
                            if (!downValue.HasValue)
                            {
                                downValue = upValue;
                            }
                        }
                        else if (!downValue.HasValue && !upValue.HasValue)
                        {
                            throw new Exception($"No value in column {row}");
                        }

                        dataLineList[row] = (upValue.Value + downValue.Value) / 2;
                        LogParserProgress.Log("LineNumber " + row
                                            + " FieldNumber " + columnIndex
                                            + " upValue=" + upValue.Value
                                            + " downValue=" + downValue.Value
                                            + " newValue=" + dataLineList[row], logFile);

                        foundEmptysCount++;

                    }

                    await Task.Yield();
                }

                //Запись в файл
                using (TextFieldParser csvParser = new TextFieldParser(csvFilePath))
                {
                    csvParser.TextFieldType = FieldType.Delimited;
                    csvParser.SetDelimiters(",");
                    int lineCount = 0;
                    string[] csvHeader = null;

                    while (!csvParser.EndOfData)
                    {
                        string[] fields = csvParser.ReadFields();
                        if (lineCount == 0)
                        {
                            csvHeader = fields;
                        }
                        var myExport = new CsvExport(",", false, false);
                        myExport.AddRow();
                        for (int i = 0; i < fields.Length; i++)
                        {
                            if (i == columnIndex)
                            {
                                if (lineCount == 0)
                                {
                                    myExport[csvHeader[i]] = fields[i];
                                }
                                else
                                {
                                    myExport[csvHeader[i]] = dataLineList[lineCount - 1].ToString().Replace(",", ".");
                                }
                            }
                            else
                            {
                                myExport[csvHeader[i]] = fields[i];
                            }
                        }
                        lineCount++;
                        using (StreamWriter sw = new StreamWriter(tempPath, true))
                        {
                            sw.Write(myExport.Export());
                            long length = sw.BaseStream.Length;
                            InvokeProgressBarUpdateValue((int)length);
                        }
                        await Task.Yield();
                    }
                }

                if (!IsCancelled && error == null)
                    File.Copy(tempPath, newPath, true);
                File.Delete(tempPath);
                File.Delete(csvFilePath);
            });
            await t;

            onFinish?.Invoke(newPath, RefactorType.findNone, foundEmptysCount, IsCancelled, error);
            IsCancelled = false;
            IsProcessing = false;
        }
    }
}
