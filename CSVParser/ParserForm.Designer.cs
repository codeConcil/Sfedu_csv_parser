namespace CSVParser
{
    partial class ParserForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.pathTextBox = new System.Windows.Forms.TextBox();
            this.startParsingButton = new System.Windows.Forms.Button();
            this.lineProgressBar = new System.Windows.Forms.ProgressBar();
            this.parsingProgressBar = new System.Windows.Forms.ProgressBar();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.csvTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.columnNumUpDown = new System.Windows.Forms.NumericUpDown();
            this.noneRadioButton = new System.Windows.Forms.RadioButton();
            this.normanizationRadioButton = new System.Windows.Forms.RadioButton();
            this.logButton = new System.Windows.Forms.Button();
            this.emissionsRadioButton = new System.Windows.Forms.RadioButton();
            this.emptyFieldRadioButton = new System.Windows.Forms.RadioButton();
            this.repeatLineRadioButton = new System.Windows.Forms.RadioButton();
            this.refactoringProgressBar = new System.Windows.Forms.ProgressBar();
            this.startRefactoringButton = new System.Windows.Forms.Button();
            this.debugConsole = new System.Windows.Forms.TextBox();
            this.classificationRadioButton = new System.Windows.Forms.RadioButton();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.columnNumUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // pathTextBox
            // 
            this.pathTextBox.AllowDrop = true;
            this.pathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pathTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.pathTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.pathTextBox.Location = new System.Drawing.Point(12, 28);
            this.pathTextBox.Multiline = true;
            this.pathTextBox.Name = "pathTextBox";
            this.pathTextBox.ReadOnly = true;
            this.pathTextBox.Size = new System.Drawing.Size(510, 334);
            this.pathTextBox.TabIndex = 0;
            this.pathTextBox.Text = "Перетащите файл";
            this.pathTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.PathTextBoxDragDrop);
            this.pathTextBox.DragOver += new System.Windows.Forms.DragEventHandler(this.PathTextBoxDragOver);
            // 
            // startParsingButton
            // 
            this.startParsingButton.Location = new System.Drawing.Point(12, 368);
            this.startParsingButton.Name = "startParsingButton";
            this.startParsingButton.Size = new System.Drawing.Size(98, 70);
            this.startParsingButton.TabIndex = 1;
            this.startParsingButton.Text = "Начать";
            this.startParsingButton.UseVisualStyleBackColor = true;
            this.startParsingButton.Click += new System.EventHandler(this.startParsingButton_Click);
            // 
            // lineProgressBar
            // 
            this.lineProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lineProgressBar.Location = new System.Drawing.Point(116, 368);
            this.lineProgressBar.Name = "lineProgressBar";
            this.lineProgressBar.Size = new System.Drawing.Size(405, 30);
            this.lineProgressBar.TabIndex = 2;
            // 
            // parsingProgressBar
            // 
            this.parsingProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.parsingProgressBar.Location = new System.Drawing.Point(117, 405);
            this.parsingProgressBar.Name = "parsingProgressBar";
            this.parsingProgressBar.Size = new System.Drawing.Size(404, 33);
            this.parsingProgressBar.TabIndex = 3;
            // 
            // csvTextBox
            // 
            this.csvTextBox.AllowDrop = true;
            this.csvTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.csvTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.csvTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.csvTextBox.Location = new System.Drawing.Point(12, 467);
            this.csvTextBox.Multiline = true;
            this.csvTextBox.Name = "csvTextBox";
            this.csvTextBox.ReadOnly = true;
            this.csvTextBox.Size = new System.Drawing.Size(510, 87);
            this.csvTextBox.TabIndex = 4;
            this.csvTextBox.Text = "CSV:";
            this.csvTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.CSVTextBoxDragDrop);
            this.csvTextBox.DragOver += new System.Windows.Forms.DragEventHandler(this.CSVTextBoxDragOver);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 451);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "CSV refactoring";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "CSV creator";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.classificationRadioButton);
            this.panel1.Controls.Add(this.columnNumUpDown);
            this.panel1.Controls.Add(this.noneRadioButton);
            this.panel1.Controls.Add(this.normanizationRadioButton);
            this.panel1.Controls.Add(this.logButton);
            this.panel1.Controls.Add(this.emissionsRadioButton);
            this.panel1.Controls.Add(this.emptyFieldRadioButton);
            this.panel1.Controls.Add(this.repeatLineRadioButton);
            this.panel1.Location = new System.Drawing.Point(12, 561);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(510, 48);
            this.panel1.TabIndex = 7;
            // 
            // columnNumUpDown
            // 
            this.columnNumUpDown.Location = new System.Drawing.Point(278, 23);
            this.columnNumUpDown.Name = "columnNumUpDown";
            this.columnNumUpDown.Size = new System.Drawing.Size(61, 20);
            this.columnNumUpDown.TabIndex = 14;
            // 
            // noneRadioButton
            // 
            this.noneRadioButton.AutoSize = true;
            this.noneRadioButton.Location = new System.Drawing.Point(278, 4);
            this.noneRadioButton.Name = "noneRadioButton";
            this.noneRadioButton.Size = new System.Drawing.Size(61, 17);
            this.noneRadioButton.TabIndex = 13;
            this.noneRadioButton.TabStop = true;
            this.noneRadioButton.Text = "\"None\"";
            this.noneRadioButton.UseVisualStyleBackColor = true;
            this.noneRadioButton.CheckedChanged += new System.EventHandler(this.noneRadioButton_CheckedChanged);
            // 
            // normanizationRadioButton
            // 
            this.normanizationRadioButton.AutoSize = true;
            this.normanizationRadioButton.Location = new System.Drawing.Point(156, 26);
            this.normanizationRadioButton.Name = "normanizationRadioButton";
            this.normanizationRadioButton.Size = new System.Drawing.Size(101, 17);
            this.normanizationRadioButton.TabIndex = 12;
            this.normanizationRadioButton.TabStop = true;
            this.normanizationRadioButton.Text = "Нормализация";
            this.normanizationRadioButton.UseVisualStyleBackColor = true;
            this.normanizationRadioButton.CheckedChanged += new System.EventHandler(this.normanizationRadioButton_CheckedChanged);
            // 
            // logButton
            // 
            this.logButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.logButton.Location = new System.Drawing.Point(465, 3);
            this.logButton.Name = "logButton";
            this.logButton.Size = new System.Drawing.Size(45, 40);
            this.logButton.TabIndex = 11;
            this.logButton.Text = "Log";
            this.logButton.UseVisualStyleBackColor = true;
            this.logButton.Click += new System.EventHandler(this.logButton_Click);
            // 
            // emissionsRadioButton
            // 
            this.emissionsRadioButton.AutoSize = true;
            this.emissionsRadioButton.Location = new System.Drawing.Point(156, 3);
            this.emissionsRadioButton.Name = "emissionsRadioButton";
            this.emissionsRadioButton.Size = new System.Drawing.Size(72, 17);
            this.emissionsRadioButton.TabIndex = 2;
            this.emissionsRadioButton.TabStop = true;
            this.emissionsRadioButton.Text = "Выбросы";
            this.emissionsRadioButton.UseVisualStyleBackColor = true;
            this.emissionsRadioButton.CheckedChanged += new System.EventHandler(this.emissionsRadioButton_CheckedChanged);
            // 
            // emptyFieldRadioButton
            // 
            this.emptyFieldRadioButton.AutoSize = true;
            this.emptyFieldRadioButton.Location = new System.Drawing.Point(3, 26);
            this.emptyFieldRadioButton.Name = "emptyFieldRadioButton";
            this.emptyFieldRadioButton.Size = new System.Drawing.Size(90, 17);
            this.emptyFieldRadioButton.TabIndex = 1;
            this.emptyFieldRadioButton.TabStop = true;
            this.emptyFieldRadioButton.Text = "Пустые поля";
            this.emptyFieldRadioButton.UseVisualStyleBackColor = true;
            this.emptyFieldRadioButton.CheckedChanged += new System.EventHandler(this.emptyFieldRadioButton_CheckedChanged);
            // 
            // repeatLineRadioButton
            // 
            this.repeatLineRadioButton.AutoSize = true;
            this.repeatLineRadioButton.Checked = true;
            this.repeatLineRadioButton.Location = new System.Drawing.Point(3, 3);
            this.repeatLineRadioButton.Name = "repeatLineRadioButton";
            this.repeatLineRadioButton.Size = new System.Drawing.Size(147, 17);
            this.repeatLineRadioButton.TabIndex = 0;
            this.repeatLineRadioButton.TabStop = true;
            this.repeatLineRadioButton.Text = "Повторяющиеся строки";
            this.repeatLineRadioButton.UseVisualStyleBackColor = true;
            this.repeatLineRadioButton.CheckedChanged += new System.EventHandler(this.repeatLineRadioButton_CheckedChanged);
            // 
            // refactoringProgressBar
            // 
            this.refactoringProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.refactoringProgressBar.Location = new System.Drawing.Point(116, 615);
            this.refactoringProgressBar.Name = "refactoringProgressBar";
            this.refactoringProgressBar.Size = new System.Drawing.Size(405, 30);
            this.refactoringProgressBar.TabIndex = 8;
            // 
            // startRefactoringButton
            // 
            this.startRefactoringButton.Location = new System.Drawing.Point(12, 615);
            this.startRefactoringButton.Name = "startRefactoringButton";
            this.startRefactoringButton.Size = new System.Drawing.Size(98, 30);
            this.startRefactoringButton.TabIndex = 9;
            this.startRefactoringButton.Text = "Начать";
            this.startRefactoringButton.UseVisualStyleBackColor = true;
            this.startRefactoringButton.Click += new System.EventHandler(this.startRefactoringButton_Click);
            // 
            // debugConsole
            // 
            this.debugConsole.AllowDrop = true;
            this.debugConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.debugConsole.Cursor = System.Windows.Forms.Cursors.Default;
            this.debugConsole.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.debugConsole.Location = new System.Drawing.Point(12, 651);
            this.debugConsole.Multiline = true;
            this.debugConsole.Name = "debugConsole";
            this.debugConsole.ReadOnly = true;
            this.debugConsole.Size = new System.Drawing.Size(510, 84);
            this.debugConsole.TabIndex = 10;
            this.debugConsole.Text = "Debug";
            // 
            // classificationRadioButton
            // 
            this.classificationRadioButton.AutoSize = true;
            this.classificationRadioButton.Location = new System.Drawing.Point(349, 15);
            this.classificationRadioButton.Name = "classificationRadioButton";
            this.classificationRadioButton.Size = new System.Drawing.Size(106, 17);
            this.classificationRadioButton.TabIndex = 15;
            this.classificationRadioButton.TabStop = true;
            this.classificationRadioButton.Text = "Классификация";
            this.classificationRadioButton.UseVisualStyleBackColor = true;
            this.classificationRadioButton.CheckedChanged += new System.EventHandler(this.classificationRadioButton_CheckedChanged);
            // 
            // ParserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 747);
            this.Controls.Add(this.debugConsole);
            this.Controls.Add(this.startRefactoringButton);
            this.Controls.Add(this.refactoringProgressBar);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.csvTextBox);
            this.Controls.Add(this.parsingProgressBar);
            this.Controls.Add(this.lineProgressBar);
            this.Controls.Add(this.startParsingButton);
            this.Controls.Add(this.pathTextBox);
            this.MinimumSize = new System.Drawing.Size(550, 750);
            this.Name = "ParserForm";
            this.Text = "CSV parser";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.columnNumUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox pathTextBox;
        private System.Windows.Forms.Button startParsingButton;
        private System.Windows.Forms.ProgressBar lineProgressBar;
        private System.Windows.Forms.ProgressBar parsingProgressBar;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.TextBox csvTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ProgressBar refactoringProgressBar;
        private System.Windows.Forms.Button startRefactoringButton;
        private System.Windows.Forms.RadioButton emissionsRadioButton;
        private System.Windows.Forms.RadioButton emptyFieldRadioButton;
        private System.Windows.Forms.RadioButton repeatLineRadioButton;
        private System.Windows.Forms.TextBox debugConsole;
        private System.Windows.Forms.Button logButton;
        private System.Windows.Forms.RadioButton normanizationRadioButton;
        private System.Windows.Forms.NumericUpDown columnNumUpDown;
        private System.Windows.Forms.RadioButton noneRadioButton;
        private System.Windows.Forms.RadioButton classificationRadioButton;
    }
}

