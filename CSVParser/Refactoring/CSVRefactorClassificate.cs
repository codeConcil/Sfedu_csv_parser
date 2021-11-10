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
        protected async void Classificate(string csvSourseFilePath, string newFolder, Action<string, RefactorType, int, bool, ParserError> onFinish)
        {
            string logFile = LogParserProgress.GetNewLogFilePath(Path.GetFileNameWithoutExtension(csvSourseFilePath) + "_Classification");
            string csvFilePath = MoveInTemp(csvSourseFilePath);
            ParserError error = null;
            IsProcessing = true;
            int foundSection = 0;
            string newPath = "";
            var t = Task.Run(async () =>
            {
                FileInfo csvFileInfo = new FileInfo(csvFilePath);
                InvokeProgressBarMaxValue((int)csvFileInfo.Length);

                //string tempPath = Path.GetTempFileName();
                Dictionary<string, string> tempPaths = new Dictionary<string, string>();

                List<string> sections = new List<string>();

                using (TextFieldParser csvParser = new TextFieldParser(csvFilePath))
                {

                    refactoringSoureseDispose = csvParser;
                    csvParser.TextFieldType = FieldType.Delimited;
                    csvParser.SetDelimiters(",");
                    bool isNotFirst = false;
                    string[] csvHeader = null;
                    int startRange = 0;
                    int endRange = 0;
                    while (!csvParser.EndOfData)
                    {
                        string[] fields = csvParser.ReadFields();
                        List<string> newFields = new List<string>();
                        for (int i = 0; i < DATA_OFFSET; i++)
                        {
                            if (fields[i].ToLower() == "none")
                                newFields.Add("");
                            else
                                newFields.Add(fields[i]);
                        }

                        var myExport = new CsvExport(",", false, false);
                        myExport.AddRow();

                        if (!isNotFirst)
                        {
                            endRange = fields.Length - 1;
                            for (int i = DATA_OFFSET; i < fields.Length; i++)
                            {
                                double value = Double.Parse(fields[i], System.Globalization.CultureInfo.InvariantCulture);
                                if (value == 24350)
                                {
                                    startRange = i;
                                }
                                if (value == 24950)
                                {
                                    endRange = i;
                                }
                                if (value >= 24350 && value <= 24950)
                                {
                                    newFields.Add(fields[i]);
                                }
                            }

                            isNotFirst = true;
                            csvHeader = newFields.ToArray();
                        }
                        else
                        {
                            for (int i = startRange; i <= endRange; i++)
                            {
                                newFields.Add(fields[i]);
                            }

                            if (!sections.Contains(newFields[0]))
                            {
                                sections.Add(newFields[0]);
                                tempPaths.Add(newFields[0], Path.GetTempFileName());
                                foundSection++;

                                for (int i = 0; i < newFields.Count; i++)
                                {
                                    myExport[csvHeader[i]] = csvHeader[i];
                                }
                                myExport.AddRow();
                            }

                            for (int i = 0; i < newFields.Count; i++)
                            {
                                myExport[csvHeader[i]] = newFields[i];
                            }

                            string tempPath = tempPaths[newFields[0]];
                            using (StreamWriter sw = new StreamWriter(tempPath, true))
                            {
                                sw.Write(myExport.Export());
                                long length = sw.BaseStream.Length;
                                InvokeProgressBarUpdateValue((int)length);
                            }
                        }


                        Debug("ProcessedLine: " + csvParser.LineNumber, append: false);
                        await Task.Yield();
                    }
                }

                if (!IsCancelled && error == null)
                {
                    foreach (var pair in tempPaths)
                    {
                        newPath = Path.Combine(Path.GetDirectoryName(newFolder),
                            Path.GetFileNameWithoutExtension(newFolder) + "_" + pair.Key + Path.GetExtension(newFolder));
                        File.Copy(pair.Value, newPath, true);
                    }
                }

                foreach (var pair in tempPaths)
                {
                    File.Delete(pair.Value);
                }
                File.Delete(csvFilePath);
            });
            await t;

            onFinish?.Invoke(newPath, RefactorType.findEmissionsAnomaly, foundSection, IsCancelled, error);
            IsCancelled = false;
            IsProcessing = false;
        }
    }
}
