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
        const int DATA_OFFSET = 37;

        protected async void FindNormalization(string csvSourseFilePath, string newPath, Action<string, RefactorType, int, bool, ParserError> onFinish)
        {
            string logFile = LogParserProgress.GetNewLogFilePath(Path.GetFileNameWithoutExtension(csvSourseFilePath) + "_FindNormalization");
            string csvFilePath = MoveInTemp(csvSourseFilePath);
            ParserError error = null;
            IsProcessing = true;
            int foundAnomalyCount = 0;
            var t = Task.Run(async () =>
            {
                FileInfo csvFileInfo = new FileInfo(csvFilePath);
                InvokeProgressBarMaxValue((int)((csvFileInfo.Length)));
                double[,] normalizedValue;
                string normalizedTempPath = Path.GetTempFileName();
                string tempPath = Path.GetTempFileName();

                // Find repeatCount
                using (TextFieldParser csvParser = new TextFieldParser(csvFilePath))
                {
                    csvParser.TextFieldType = FieldType.Delimited;
                    csvParser.SetDelimiters(",");
                    string[] csvHeader = null;
                    refactoringSoureseDispose = csvParser;
                    bool isFirst = true;
                    List<double[]> tableValues = new List<double[]>();
                    while (!csvParser.EndOfData)
                    {
                        List<double> values = new List<double>();
                        string[] fields = csvParser.ReadFields();
                        if (!isFirst)
                        {
                            try
                            {
                                for (int ii = DATA_OFFSET; ii < fields.Length; ii++)
                                {
                                    double value = Double.Parse(fields[ii], System.Globalization.CultureInfo.InvariantCulture);
                                    values.Add(value);
                                }
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.Message);
                                error = new ParserError(e.Message);
                                refactoringSoureseDispose?.Dispose();
                                refactoringTempDispose?.Dispose();
                                break;
                            }
                            tableValues.Add(values.ToArray());
                        }
                        else
                        {
                            csvHeader = fields;
                            isFirst = false;
                        }
                        Debug("ProcessedLine " + csvParser.LineNumber, append: false);
                        await Task.Yield();
                    }
                    //Расчет нормализации
                    normalizedValue = new double[tableValues.Count, tableValues[0].Length];
                    for (int i = 0; i < tableValues[0].Length; i++)
                    {
                        double[] values = new double[tableValues.Count];
                        for (int ii = 0; ii < tableValues.Count; ii++)
                        {
                            values[ii] = tableValues[ii][i];
                        }

                        double meanSquareDeviation = MeanSquareDeviation(values);
                        for (int ii = 0; ii < values.Length; ii++)
                        {
                            normalizedValue[ii, i] = ZNormalize(values[ii], values.ToArray(), meanSquareDeviation);
                        }
                        Debug("CalculatedZNormalize " + i, append: false);
                    }
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
                        var myExportNormalized = new CsvExport(",", false, false);
                        myExport.AddRow();
                        myExportNormalized.AddRow();
                        int lineAnomalyCount = 0;
                        for (int i = 0; i < fields.Length; i++)
                        {
                            if (i >= DATA_OFFSET)
                            {
                                if (lineCount == 0 || (normalizedValue[lineCount - 1, i - DATA_OFFSET] > -1 && normalizedValue[lineCount - 1, i - DATA_OFFSET] < 1))
                                {
                                    myExport[csvHeader[i]] = fields[i];
                                }
                                else
                                {
                                    myExport[csvHeader[i]] = "";
                                    foundAnomalyCount++;
                                    lineAnomalyCount++;
                                }

                                if (lineCount == 0)
                                {
                                    myExportNormalized[csvHeader[i]] = fields[i];
                                }
                                else
                                {
                                    myExportNormalized[csvHeader[i]] = normalizedValue[lineCount - 1, i - DATA_OFFSET].ToString().Replace(",", ".");
                                }
                            }
                            else
                            {
                                myExport[csvHeader[i]] = fields[i];
                            }
                        }
                        LogParserProgress.Log("LineNumber: " + lineCount + " anomalyCount: " + lineAnomalyCount, logFile);
                        lineCount++;
                        using (StreamWriter sw = new StreamWriter(tempPath, true))
                        {
                            sw.Write(myExport.Export());
                            long length = sw.BaseStream.Length;
                            InvokeProgressBarUpdateValue((int)length);
                        }
                        using (StreamWriter sw = new StreamWriter(normalizedTempPath, true))
                        {
                            sw.Write(myExportNormalized.Export());
                        }

                        await Task.Yield();
                    }
                }


                string normalizeFilePath = Path.Combine(Path.GetDirectoryName(newPath),
                Path.GetFileNameWithoutExtension(newPath) + "_Normalized" + Path.GetExtension(newPath));
                if (!IsCancelled && error == null)
                {
                    File.Copy(tempPath, newPath, true);
                    File.Copy(normalizedTempPath, normalizeFilePath, true);
                }
                File.Delete(tempPath);
                File.Delete(normalizedTempPath);
                File.Delete(csvFilePath);
            });
            await t;

            onFinish?.Invoke(newPath, RefactorType.findNormalizationAnomaly, foundAnomalyCount, IsCancelled, error);
            IsCancelled = false;
            IsProcessing = false;
        }

        private double ZNormalize(double value, double[] valueRange, double meanSquareDeviation)
        {
            return (value - AverageValue(valueRange)) / meanSquareDeviation;
        }

        private double MeanSquareDeviation(double[] valueRange)
        {
            double sum = 0;
            double avg = AverageValue(valueRange);
            for (int i = 0; i < valueRange.Length; i++)
            {
                sum += Math.Pow(valueRange[i] - avg, 2);
            }
            return Math.Sqrt(sum / (valueRange.Length - 1));
        }

        private double AverageValue(double[] valueRange)
        {
            double avg = 0;
            foreach (var v in valueRange)
            {
                avg += v;
            }
            avg /= valueRange.Length;
            return avg;
        }
    }
}
