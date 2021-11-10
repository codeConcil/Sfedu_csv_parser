using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using Jitbit.Utils;

namespace CSVParser
{
    public class ParserError
    {
        public readonly string description;
        public ParserError(string err)
        {
            description = err;
        }
    }
    class CSVParser
    {
        public bool IsProcessing { get; private set; } = false;
        public bool IsCancelled { get; private set; } = false;

        public event Action<int> OnLineProgressBarMaxValue;
        public event Action<int> OnLineProgressBarUpdateValue;
        public event Action<int> OnFullProgressBarMaxValue;
        public event Action<int> OnFullProgressBarUpdateValue;

        IDisposable txtParseDispose;
        IDisposable paramParseDispose;

        public async void SCVFromFiles(string txtSourseFilePath, string paramSourseFilePath, string newPath, Action<string, bool, ParserError> onFinish)
        {
            string txtFilePath = MoveInTemp(txtSourseFilePath);
            string paramFilePath = MoveInTemp(paramSourseFilePath);
            ParserError error = null;
            IsProcessing = true;
            var t = Task.Run(async () =>
            {
                FileInfo txtFileInfo = new FileInfo(txtFilePath);
                FileInfo paramFileInfo = new FileInfo(paramFilePath);
                OnFullProgressBarMaxValue?.Invoke((int)((txtFileInfo.Length + paramFileInfo.Length) * 1.1));

                using (TextFieldParser txtParser = new TextFieldParser(txtFilePath))
                {
                    using (TextFieldParser paramParser = new TextFieldParser(paramFilePath))
                    {
                        string mergeTempPath = Path.GetTempFileName();
                        using (StreamWriter sw = new StreamWriter(mergeTempPath))
                        {

                            string[] paramHeader = null;
                            string[] txtHeader = null;
                            txtParseDispose = txtParser;
                            paramParseDispose = paramParser;

                            txtParser.TextFieldType = FieldType.Delimited;
                            txtParser.SetDelimiters(" ", "\t");
                            paramParser.TextFieldType = FieldType.Delimited;
                            paramParser.SetDelimiters("\t");
                            bool isNotFirst = false;
                            while (!txtParser.EndOfData || !paramParser.EndOfData)
                            {
                                if (txtParser.EndOfData != paramParser.EndOfData)
                                {
                                    error = new ParserError("Data length not equals!");
                                    txtParseDispose?.Dispose();
                                    paramParseDispose?.Dispose();
                                    break;
                                }
                                var myExport = new CsvExport(",", false, false);
                                myExport.AddRow();

                                string[] txtFields = txtParser.ReadFields();
                                string[] paramFields = paramParser.ReadFields();

                                // Исключить все None 
                                for (int i = 0; i < paramFields.Length; i++)
                                {
                                    if (paramFields[i].ToLower() == "none")
                                    {
                                        paramFields[i] = "";
                                    }
                                }

                                if (!isNotFirst)
                                {
                                    paramHeader = paramFields;
                                    txtHeader = txtFields;
                                }
                                isNotFirst = true;
                                if (paramHeader == null || txtHeader == null)
                                {
                                    error = new ParserError("Headers is null!");
                                    txtParseDispose?.Dispose();
                                    paramParseDispose?.Dispose();
                                    break;
                                }
                                if (paramHeader.Length != paramFields.Length)
                                {
                                    error = new ParserError("param header & fields not equals!" + paramHeader.Length + " " + paramFields.Length);
                                    txtParseDispose?.Dispose();
                                    paramParseDispose?.Dispose();
                                    break;
                                }
                                if (txtHeader.Length != txtFields.Length)
                                {
                                    error = new ParserError("txt header & fields not equals!");
                                    txtParseDispose?.Dispose();
                                    paramParseDispose?.Dispose();
                                    break;
                                }


                                OnLineProgressBarMaxValue?.Invoke(txtFields.Length + paramFields.Length);


                                for (int i = 0; i < paramFields.Length; i++)
                                {
                                    myExport[paramHeader[i]] = paramFields[i];
                                    OnLineProgressBarUpdateValue?.Invoke(i);
                                }

                                for (int i = 0; i < txtFields.Length; i++)
                                {
                                    myExport[txtHeader[i]] = txtFields[i];
                                    OnLineProgressBarUpdateValue?.Invoke(paramFields.Length + i);
                                }

                                long length = sw.BaseStream.Length;
                                sw.Write(myExport.Export());

                                OnFullProgressBarUpdateValue?.Invoke((int)length);
                                await Task.Yield();
                            }
                        }
                        if (!IsCancelled)
                            File.Copy(mergeTempPath, newPath, true);
                        File.Delete(mergeTempPath);
                    }
                    File.Delete(paramFilePath);
                }
                File.Delete(txtFilePath);
            });
            await t;

            onFinish?.Invoke(newPath, IsCancelled, error);
            IsCancelled = false;
            IsProcessing = false;
        }

        private string MoveInTemp(string file)
        {
            string temp = Path.GetTempFileName();
            File.Copy(file, temp, true);
            return temp;
        }

        public void CancelParsing()
        {
            IsCancelled = true;
            txtParseDispose?.Dispose();
        }
    }
}
