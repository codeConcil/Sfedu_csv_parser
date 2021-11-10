using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace CSVParser
{
    public partial class ParserForm : Form
    {
        CSVParser parser;
        CSVRefactor refactor;
        string source_txt_File;
        string source_param_File;
        string source_csv_File;

        CSVRefactor.RefactorType RefactorType = CSVRefactor.RefactorType.findRepeat;

        public ParserForm()
        {
            InitializeComponent();
            parser = new CSVParser();
            startParsingButton.Enabled = false;
            startParsingButton.Text = "Начать";
            lineProgressBar.Value = 0;
            parsingProgressBar.Value = 0;
            pathTextBox.AllowDrop = true;

            refactor = new CSVRefactor();
            startRefactoringButton.Enabled = false;
            startRefactoringButton.Text = "Начать";
            refactoringProgressBar.Value = 0;
            csvTextBox.AllowDrop = true;
            debugConsole.ScrollBars = ScrollBars.Vertical;

            UpdatePathTextBox("none", "none");
            UpdateCSVTextBox("none");

            saveFileDialog1.Filter = "CSV files(*.csv)|*.csv";
            parser.OnFullProgressBarMaxValue += (x) =>
            {
                parsingProgressBar.Invoke(new Action(() => { parsingProgressBar.Value = 0; parsingProgressBar.Maximum = x; }));
            };
            parser.OnLineProgressBarMaxValue += (x) =>
            {
                lineProgressBar.Invoke(new Action(() => { lineProgressBar.Value = 0; lineProgressBar.Maximum = x; }));
            };
            parser.OnFullProgressBarUpdateValue += (x) =>
            {
                parsingProgressBar.Invoke(new Action(() =>
                {
                    if (x <= parsingProgressBar.Maximum)
                    {
                        parsingProgressBar.Value = x;
                    }
                }));
            };
            parser.OnLineProgressBarUpdateValue += (x) =>
            {
                lineProgressBar.Invoke(new Action(() => { lineProgressBar.Value = x; }));
            };

            refactor.OnProgressBarMaxValue += (x) =>
            {
                refactoringProgressBar.Invoke(new Action(() => { refactoringProgressBar.Value = 0; refactoringProgressBar.Maximum = x; }));
            };
            refactor.OnProgressBarUpdateValue += (x) =>
            {
                refactoringProgressBar.Invoke(new Action(() =>
                {
                    if (x <= refactoringProgressBar.Maximum)
                    {
                        refactoringProgressBar.Value = x;
                    }
                }));
            };
            refactor.OnInvokeDebugConsole += (x, f) =>
            {
                debugConsole.Invoke(new Action(() =>
                {
                    if (f)
                        debugConsole.Text += x;
                    else
                        debugConsole.Text = x;
                }));
            };
        }

        private void startParsingButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(source_txt_File) && !string.IsNullOrEmpty(source_param_File))
            {
                if (!parser.IsProcessing)
                {
                    if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                        return;

                    startParsingButton.Text = "Отменить";
                    pathTextBox.AllowDrop = false;
                    lineProgressBar.Value = 0;
                    parsingProgressBar.Value = 0;
                    parser.SCVFromFiles(source_txt_File, source_param_File, saveFileDialog1.FileName, FinishParsing);
                }
                else
                {
                    lineProgressBar.Value = 0;
                    parsingProgressBar.Value = 0;
                    startParsingButton.Text = "Начать";
                    parser.CancelParsing();
                }
            }
        }

        private void FinishParsing(string newPath, bool isCancel, ParserError error)
        {
            if (error != null)
            {
                pathTextBox.BackColor = Color.Red;
                pathTextBox.Text = error.description;
                MessageBox.Show(error.description);
            }
            else if (isCancel)
            {
                pathTextBox.BackColor = Color.Pink;
                pathTextBox.Text = "Отменено!";
                MessageBox.Show("Отменено!");
            }
            else
            {
                parsingProgressBar.Value = parsingProgressBar.Maximum;
                pathTextBox.BackColor = Color.LightGreen;
                pathTextBox.Text = "CSV готов!";

                string argument = "/select, \"" + newPath + "\"";
                Process.Start("explorer.exe", argument);

                source_csv_File = newPath;
                UpdateCSVTextBox(source_csv_File);
                startRefactoringButton.Enabled = true;
                startRefactoringButton.Text = "Начать";
            }
            source_txt_File = null;
            source_param_File = null;
            pathTextBox.AllowDrop = true;
            startParsingButton.Text = "Начать";
        }

        private void PathTextBoxDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void PathTextBoxDragDrop(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];

            if (files != null && files.Any())
            {
                string txt = "none";
                string param = "none";
                CheckDropedFile(files);
                if (!string.IsNullOrEmpty(source_txt_File) && File.Exists(source_txt_File))
                {
                    txt = source_txt_File;
                }
                if (!string.IsNullOrEmpty(source_param_File) && File.Exists(source_param_File))
                {
                    param = source_param_File;
                }

                UpdatePathTextBox(txt, param);

                if (txt != "none" && param != "none")
                {
                    startParsingButton.Enabled = true;
                    startParsingButton.Text = "Начать";
                    lineProgressBar.Value = 0;
                    parsingProgressBar.Value = 0;
                    return;
                }
            }

            startParsingButton.Enabled = false;
        }

        private void UpdatePathTextBox(string txt, string param)
        {
            pathTextBox.BackColor = SystemColors.Control;
            pathTextBox.Text = "Добавленные файлы: \r\n\r\n" +
                     "txt: " + txt + " \r\n\r\n" +
                     "param: " + param;
        }
        private void UpdateCSVTextBox(string csv)
        {
            csvTextBox.BackColor = SystemColors.Control;
            csvTextBox.Text = "Добавленные файлы: \r\n" +
                     "csv: " + csv;
        }

        private bool CheckDropedFile(string[] files)
        {
            foreach (var file in files)
            {
                if (Path.GetExtension(file) == ".txt")
                {
                    source_txt_File = file;
                }
                else if (Path.GetExtension(file) == ".param")
                {
                    source_param_File = file;
                }
            }
            return true;
        }

        private void startRefactoringButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(source_csv_File))
            {
                if (!refactor.IsProcessing)
                {
                    if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                        return;
                    startRefactoringButton.Text = "Отменить";
                    csvTextBox.AllowDrop = false;
                    refactoringProgressBar.Value = 0;
                    refactor.StartRefactoring(source_csv_File, RefactorType, saveFileDialog1.FileName, FinishRefactoring, (int)columnNumUpDown.Value);
                }
                else
                {
                    refactoringProgressBar.Value = 0;
                    startRefactoringButton.Text = "Начать";
                    refactor.CancelRefactoring();
                }
            }
        }

        private void FinishRefactoring(string newPath, CSVRefactor.RefactorType refactorType, int count, bool isCancel, ParserError error)
        {
            if (error != null)
            {
                csvTextBox.BackColor = Color.Red;
                csvTextBox.Text = error.description;
                MessageBox.Show(error.description);
            }
            else if (isCancel)
            {
                csvTextBox.BackColor = Color.Pink;
                csvTextBox.Text = "Отменено!";
                MessageBox.Show("Отменено!");
            }
            else
            {
                refactoringProgressBar.Value = refactoringProgressBar.Maximum;
                csvTextBox.BackColor = Color.LightGreen;
                csvTextBox.Text = "CSV обновлен!\n\r" + refactorType.ToString() + ": " + count;

                string argument = "/select, \"" + newPath + "\"";
                Process.Start("explorer.exe", argument);
            }
            csvTextBox.AllowDrop = true;
            startRefactoringButton.Text = "Начать";
        }

        private void CSVTextBoxDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void CSVTextBoxDragDrop(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];

            if (files != null && files.Any())
            {
                string csv = "none";
                
                foreach (var file in files)
                {
                    if (Path.GetExtension(file) == ".csv")
                    {
                        source_csv_File = file;
                    }
                }
                if (!string.IsNullOrEmpty(source_csv_File) && File.Exists(source_csv_File))
                {
                    csv = source_csv_File;
                }

                UpdateCSVTextBox(csv);

                if (csv != "none")
                {
                    startRefactoringButton.Enabled = true;
                    startRefactoringButton.Text = "Начать";
                    refactoringProgressBar.Value = 0;
                    return;
                }
            }

            startRefactoringButton.Enabled = false;
        }

        private void repeatLineRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb == null) return;

            if (rb.Checked)
            {
                RefactorType = CSVRefactor.RefactorType.findRepeat;
            }
        }

        private void emptyFieldRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb == null) return;

            if (rb.Checked)
            {
                RefactorType = CSVRefactor.RefactorType.findEmpty;
            }
        }

        private void emissionsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb == null) return;

            if (rb.Checked)
            {
                RefactorType = CSVRefactor.RefactorType.findEmissionsAnomaly;
            }
        }
        private void normanizationRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb == null) return;

            if (rb.Checked)
            {
                RefactorType = CSVRefactor.RefactorType.findNormalizationAnomaly;
            }
        }

        private void logButton_Click(object sender, EventArgs e)
        {
            Process.Start(LogParserProgress.GetAppDirectory());
        }

        private void noneRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb == null) return;

            if (rb.Checked)
            {
                RefactorType = CSVRefactor.RefactorType.findNone;
            }
        }

        private void classificationRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb == null) return;

            if (rb.Checked)
            {
                RefactorType = CSVRefactor.RefactorType.classificate;
            }
        }
    }
}
