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
        protected async void FindEmissions(string csvSourseFilePath, string newPath, Action<string, RefactorType, int, bool, ParserError> onFinish)
        {
            string logFile = LogParserProgress.GetNewLogFilePath(Path.GetFileNameWithoutExtension(csvSourseFilePath) + "_FindEmissions");
            string csvFilePath = MoveInTemp(csvSourseFilePath);
            ParserError error = null;
            IsProcessing = true;
            int foundEmissionsCount = 0;
            var t = Task.Run(async () =>
            {
                FileInfo csvFileInfo = new FileInfo(csvFilePath);

                // Find repeatCount
                string tempPath = Path.GetTempFileName();

                List<double[]> dataLineList = new List<double[]>();

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
                            string errValue = "";
                            try
                            {
                                List<double> valueFields = new List<double>();
                                for (int i = DATA_OFFSET; i < fields.Length; i++)
                                {
                                    errValue = fields[i];
                                    double value = Double.Parse(fields[i], System.Globalization.CultureInfo.InvariantCulture);
                                    valueFields.Add(value);
                                }

                                dataLineList.Add(valueFields.ToArray());
                            }
                            catch (Exception e)
                            {
                                string message = e.Message + " " + errValue;
                                MessageBox.Show(message);
                                error = new ParserError(message);
                                refactoringSoureseDispose?.Dispose();
                                refactoringTempDispose?.Dispose();
                                break;
                            }

                        }
                        Debug("ProcessedLine: " + csvParser.LineNumber, append: false);
                        await Task.Yield();
                    }
                }
                if (error == null)
                {
                    InvokeProgressBarMaxValue((int)csvFileInfo.Length);

                    double[] upLine = new double[dataLineList[0].Length];
                    double[] downLine = new double[dataLineList[0].Length];

                    for (int column = 0; column < dataLineList[0].Length; column++)
                    {
                        List<double> dataRange = new List<double>();
        
                        for (int row = 0; row < dataLineList.Count; row++)
                        {
                            dataRange.Add(dataLineList[row][column]);
                        }
                        dataRange.Sort();
                        double quartile_1 = dataRange[dataRange.Count / 4];
                        double quartile_3 = dataRange[dataRange.Count * 3 / 4];
                        double mid_quartile = quartile_3 - quartile_1;
                        upLine[column] = quartile_3 + mid_quartile * 1.5f;
                        downLine[column] = quartile_1 - mid_quartile * 1.5f;

                        Debug("CalculatedColumn: " + column, append: false);

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
                                if (i >= DATA_OFFSET)
                                {
                                    if (lineCount == 0)
                                    {
                                        myExport[csvHeader[i]] = fields[i];
                                    }
                                    else
                                    {
                                        if (dataLineList[lineCount - 1][i - DATA_OFFSET] >= downLine[i - DATA_OFFSET] && dataLineList[lineCount - 1][i - DATA_OFFSET] <= upLine[i - DATA_OFFSET])
                                        {
                                            myExport[csvHeader[i]] = dataLineList[lineCount - 1][i - DATA_OFFSET].ToString().Replace(",", ".");
                                        }
                                        else
                                        {
                                            myExport[csvHeader[i]] = "";
                                            LogParserProgress.Log("LineNumber " + lineCount
                                                   + " FieldNumber " + i
                                                   + " upValue=" + upLine[i - DATA_OFFSET]
                                                   + " downValue=" + downLine[i - DATA_OFFSET]
                                                   + " Value=" + dataLineList[lineCount - 1][i - DATA_OFFSET], logFile);

                                            foundEmissionsCount++;
                                        }
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
                }

                if (!IsCancelled && error == null)
                    File.Copy(tempPath, newPath, true);
                File.Delete(tempPath);
                File.Delete(csvFilePath);
            });
            await t;

            onFinish?.Invoke(newPath, RefactorType.findEmissionsAnomaly, foundEmissionsCount, IsCancelled, error);
            IsCancelled = false;
            IsProcessing = false;
        }
    }
}
