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
        protected async void FindEmptyField(string csvSourseFilePath, string newPath, Action<string, RefactorType, int, bool, ParserError> onFinish)
        {
            string logFile = LogParserProgress.GetNewLogFilePath(Path.GetFileNameWithoutExtension(csvSourseFilePath) + "_FindEmptyField");
            string csvFilePath = MoveInTemp(csvSourseFilePath);
            ParserError error = null;
            IsProcessing = true;
            int foundEmptysCount = 0;
            var t = Task.Run(async () =>
            {
                FileInfo csvFileInfo = new FileInfo(csvFilePath);

                // Find repeatCount
                string tempPath = Path.GetTempFileName();

                List<double?[]> dataLineList = new List<double?[]>();

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
                                List<double?> valueFields = new List<double?>();
                                for (int i = DATA_OFFSET; i < fields.Length; i++)
                                {
                                    if (!string.IsNullOrEmpty(fields[i]))
                                    {
                                        double value = Double.Parse(fields[i], System.Globalization.CultureInfo.InvariantCulture);
                                        valueFields.Add(value);
                                    }
                                    else
                                    {
                                        valueFields.Add(null);
                                    }
                                }

                                dataLineList.Add(valueFields.ToArray());
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
                InvokeProgressBarMaxValue(dataLineList[0].Length*100000 + (int)csvFileInfo.Length);

                for (int column = 0; column < dataLineList[0].Length; column++)
                {
                    for (int row = 0; row < dataLineList.Count; row++)
                    {
                        if (dataLineList[row][column] == null)
                        {
                            double? upValue = null;
                            double? downValue = null;
                            for (int i = row; i >= 0; i--)
                            {
                                if (dataLineList[i][column].HasValue)
                                {
                                    upValue = dataLineList[i][column].Value;
                                    break;
                                }
                            }
                            for (int i = row; i < dataLineList.Count; i++)
                            {
                                if (dataLineList[i][column].HasValue)
                                {
                                    downValue = dataLineList[i][column].Value;
                                    break;
                                }
                            }
                            if (!upValue.HasValue && downValue.HasValue)
                            {
                                for (int i = row; i < dataLineList.Count; i++)
                                {
                                    if (dataLineList[i][column].HasValue && dataLineList[i][column].Value != downValue.Value)
                                    {
                                        upValue = dataLineList[i][column].Value;
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
                                    if (dataLineList[i][column].HasValue && dataLineList[i][column].Value != upValue.Value)
                                    {
                                        downValue = dataLineList[i][column].Value;
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
                                throw new Exception($"No value in column {column}");
                            }

                            dataLineList[row][column] = (upValue.Value + downValue.Value) / 2;
                            LogParserProgress.Log("LineNumber " + row
                                                + " FieldNumber " + (DATA_OFFSET + column)
                                                + " upValue=" + upValue.Value
                                                + " downValue=" + downValue.Value
                                                + " newValue=" + dataLineList[row][column], logFile);

                            foundEmptysCount++;

                        }

                        await Task.Yield();
                    }
                    Debug("CalculatedColumn: " + column, append: false);
                    InvokeProgressBarUpdateValue(column * 100000);
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
                            if (i >= DATA_OFFSET)
                            {
                                if (lineCount == 0)
                                {
                                    myExport[csvHeader[i]] = fields[i];
                                }
                                else
                                {
                                    myExport[csvHeader[i]] = dataLineList[lineCount - 1][i - DATA_OFFSET].ToString().Replace(",", ".");
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
                            InvokeProgressBarUpdateValue(dataLineList[0].Length * 100000 + (int)length);
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

            onFinish?.Invoke(newPath, RefactorType.findEmpty, foundEmptysCount, IsCancelled, error);
            IsCancelled = false;
            IsProcessing = false;
        }
    }
}
