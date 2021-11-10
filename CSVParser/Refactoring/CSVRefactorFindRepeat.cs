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
        protected async void FindRepeatLine(string csvSourseFilePath, string newPath, Action<string, RefactorType, int, bool, ParserError> onFinish)
        {
            string logFile = LogParserProgress.GetNewLogFilePath(Path.GetFileNameWithoutExtension(csvSourseFilePath) + "_FindRepeatLine");
            string csvFilePath = MoveInTemp(csvSourseFilePath);
            ParserError error = null;
            IsProcessing = true;
            int foundRepeatsCount = 0;
            var t = Task.Run(async () =>
            {
                FileInfo csvFileInfo = new FileInfo(csvFilePath);
                InvokeProgressBarMaxValue((int)((csvFileInfo.Length)));

                // Find repeatCount
                string tempPath = Path.GetTempFileName();
                HashSet<string> withoutRepeat = new HashSet<string>();
                using (TextFieldParser csvParser = new TextFieldParser(csvFilePath))
                {
                    using (StreamWriter sw = new StreamWriter(tempPath, true))
                    {
                        refactoringSoureseDispose = csvParser;
                        while (!csvParser.EndOfData)
                        {
                            string line = csvParser.ReadLine();
                            if (withoutRepeat.Add(line))
                            {
                                sw.WriteLine(line);
                                long length = sw.BaseStream.Length;
                                InvokeProgressBarUpdateValue((int)length);
                            }
                            else
                            {
                                LogParserProgress.Log(" EqualsLine " + csvParser.LineNumber, logFile);
                                foundRepeatsCount++;
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

            onFinish?.Invoke(newPath, RefactorType.findRepeat, foundRepeatsCount, IsCancelled, error);
            IsCancelled = false;
            IsProcessing = false;
        }
    }
}
