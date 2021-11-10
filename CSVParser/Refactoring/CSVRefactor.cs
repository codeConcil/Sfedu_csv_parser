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
    public struct Pair<T1, T2>
    {
        public T1 Value1;
        public T2 Value2;

        public Pair(T1 value1, T2 value2)
        {
            Value1 = value1;
            Value2 = value2;
        }
    }

    public struct Buffer
    {
        public string[] upFields;
        public string[] currentFields;
        public int[] emptyIndexList;
    }
    partial class CSVRefactor
    {
        public enum RefactorType { findRepeat, findEmpty, findEmissionsAnomaly, findNormalizationAnomaly, findNone, classificate }
        public bool IsProcessing { get; protected set; } = false;
        public bool IsCancelled { get; protected set; } = false;

        public event Action<int> OnProgressBarMaxValue;
        public event Action<int> OnProgressBarUpdateValue;
        public event Action<string, bool> OnInvokeDebugConsole;

        protected void InvokeProgressBarMaxValue(int value) { OnProgressBarMaxValue?.Invoke(value); }
        protected void InvokeProgressBarUpdateValue(int value) { OnProgressBarUpdateValue?.Invoke(value); }

        protected IDisposable refactoringSoureseDispose;
        protected IDisposable refactoringTempDispose;

        public void StartRefactoring(string csvSourseFilePath, RefactorType refactorType, string newPath, Action<string, RefactorType, int, bool, ParserError> onFinish, object param = null)
        {
            Debug("", "", false);
            switch (refactorType)
            {
                case RefactorType.findRepeat:
                    FindRepeatLine(csvSourseFilePath, newPath, onFinish);
                    break;
                case RefactorType.findEmpty:
                    FindEmptyField(csvSourseFilePath, newPath, onFinish);
                    break;
                case RefactorType.findNormalizationAnomaly:
                    FindNormalization(csvSourseFilePath, newPath, onFinish);
                    break;
                case RefactorType.findEmissionsAnomaly:
                    FindEmissions(csvSourseFilePath, newPath, onFinish);
                    break;
                case RefactorType.findNone:
                    FindNoneField(csvSourseFilePath, newPath, (int)param, onFinish);
                    break;
                case RefactorType.classificate:
                    Classificate(csvSourseFilePath, newPath, onFinish);
                    break;
            }
        }

        protected string MoveInTemp(string file)
        {
            string temp = Path.GetTempFileName();
            File.Copy(file, temp, true);
            return temp;
        }

        public void CancelRefactoring()
        {
            IsCancelled = true;
            refactoringSoureseDispose?.Dispose();
            refactoringTempDispose?.Dispose();
        }

        protected async void Debug(string value, string delimiters = "\r\n", bool append = true)
        {
            var t = Task.Run(() =>
            {
                OnInvokeDebugConsole?.Invoke(value + delimiters, append);
            });
            await t;
        }
    }
}
