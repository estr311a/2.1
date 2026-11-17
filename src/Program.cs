using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Globalization;

namespace KalkulatorDesktop
{
    public class MainForm : Form
    {
        private TextBox txtDisplay;
        private Label lblExpression;
        private ListBox lstHistory;
        private Label lblHistoryTitle;
        private Button btnClearHistory;
        private Panel pnlHistory;
        private Label lblMemoryStatus;
        
        private double memoryValue = 0.0;
        private bool hasMemory = false;
        
        private double lastOperand = 0.0;
        private string pendingOperator = "";
        private bool isNewEntry = true;
        private bool hasError = false;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Kalkulator Desktopowy - Julia Kluga";
            this.Size = new Size(680, 560);
            this.MinimumSize = new Size(640, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(28, 28, 36);
            this.ForeColor = Color.White;
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(MainForm_KeyDown);

            // Główny kontener lewy - Kalkulator
            Panel pnlCalc = new Panel();
            pnlCalc.Dock = DockStyle.Left;
            pnlCalc.Width = 420;
            pnlCalc.Padding = new Padding(12);
            pnlCalc.BackColor = Color.FromArgb(28, 28, 36);
            this.Controls.Add(pnlCalc);

            // Prawy panel - Historia
            pnlHistory = new Panel();
            pnlHistory.Dock = DockStyle.Fill;
            pnlHistory.Padding = new Padding(12);
            pnlHistory.BackColor = Color.FromArgb(34, 34, 44);
            this.Controls.Add(pnlHistory);

            // Elementy historii
            lblHistoryTitle = new Label();
            lblHistoryTitle.Text = "Historia obliczen";
            lblHistoryTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblHistoryTitle.ForeColor = Color.FromArgb(220, 220, 230);
            lblHistoryTitle.Dock = DockStyle.Top;
            lblHistoryTitle.Height = 32;
            pnlHistory.Controls.Add(lblHistoryTitle);

            lstHistory = new ListBox();
            lstHistory.Dock = DockStyle.Fill;
            lstHistory.BackColor = Color.FromArgb(42, 42, 54);
            lstHistory.ForeColor = Color.FromArgb(220, 220, 240);
            lstHistory.BorderStyle = BorderStyle.None;
            lstHistory.Font = new Font("Segoe UI", 9.5f);
            lstHistory.ItemHeight = 22;
            pnlHistory.Controls.Add(lstHistory);

            btnClearHistory = new Button();
            btnClearHistory.Text = "Wyczysc historie";
            btnClearHistory.Dock = DockStyle.Bottom;
            btnClearHistory.Height = 36;
            btnClearHistory.BackColor = Color.FromArgb(52, 52, 68);
            btnClearHistory.ForeColor = Color.White;
            btnClearHistory.FlatStyle = FlatStyle.Flat;
            btnClearHistory.FlatAppearance.BorderSize = 0;
            btnClearHistory.Font = new Font("Segoe UI", 9);
            btnClearHistory.Cursor = Cursors.Hand;
            btnClearHistory.Click += new EventHandler(delegate(object s, EventArgs e) {
                lstHistory.Items.Clear();
            });
            pnlHistory.Controls.Add(btnClearHistory);

            // Wyświetlacz kalkulatora
            Panel pnlDisplay = new Panel();
            pnlDisplay.Dock = DockStyle.Top;
            pnlDisplay.Height = 100;
            pnlDisplay.BackColor = Color.FromArgb(38, 38, 50);
            pnlDisplay.Padding = new Padding(10, 8, 10, 8);
            pnlCalc.Controls.Add(pnlDisplay);

            lblMemoryStatus = new Label();
            lblMemoryStatus.Text = "";
            lblMemoryStatus.ForeColor = Color.FromArgb(129, 140, 248);
            lblMemoryStatus.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblMemoryStatus.Location = new Point(10, 6);
            lblMemoryStatus.AutoSize = true;
            pnlDisplay.Controls.Add(lblMemoryStatus);

            lblExpression = new Label();
            lblExpression.Text = "";
            lblExpression.ForeColor = Color.FromArgb(160, 160, 175);
            lblExpression.Font = new Font("Segoe UI", 10);
            lblExpression.TextAlign = ContentAlignment.MiddleRight;
            lblExpression.Dock = DockStyle.Top;
            lblExpression.Height = 26;
            pnlDisplay.Controls.Add(lblExpression);

            txtDisplay = new TextBox();
            txtDisplay.Text = "0";
            txtDisplay.ReadOnly = true;
            txtDisplay.BackColor = Color.FromArgb(38, 38, 50);
            txtDisplay.ForeColor = Color.White;
            txtDisplay.BorderStyle = BorderStyle.None;
            txtDisplay.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            txtDisplay.TextAlign = HorizontalAlignment.Right;
            txtDisplay.Dock = DockStyle.Bottom;
            pnlDisplay.Controls.Add(txtDisplay);

            // Klawiatura kalkulatora
            TableLayoutPanel tblGrid = new TableLayoutPanel();
            tblGrid.Dock = DockStyle.Fill;
            tblGrid.Padding = new Padding(0, 10, 0, 0);
            tblGrid.ColumnCount = 5;
            tblGrid.RowCount = 6;

            for (int c = 0; c < 5; c++)
                tblGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20f));
            for (int r = 0; r < 6; r++)
                tblGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66f));

            string[,] buttons = new string[,]
            {
                { "MC", "MR", "M+", "M-", "C" },
                { "CE", "<-", "+/-", "sqrt", "/" },
                { "7", "8", "9", "x^2", "*" },
                { "4", "5", "6", "1/x", "-" },
                { "1", "2", "3", "%", "+" },
                { "Hist", "0", ".", "=", "=" }
            };

            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    if (row == 5 && col == 4) continue; // Przycisk '=' połączony lub normalny

                    string text = buttons[row, col];
                    Button btn = new Button();
                    btn.Text = text;
                    btn.Dock = DockStyle.Fill;
                    btn.Margin = new Padding(3);
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
                    btn.Cursor = Cursors.Hand;

                    // Stylowanie przycisków w zależności od typu
                    if (text == "=")
                    {
                        btn.BackColor = Color.FromArgb(79, 70, 229);
                        btn.ForeColor = Color.White;
                        if (row == 5 && col == 3)
                        {
                            tblGrid.SetColumnSpan(btn, 2);
                        }
                    }
                    else if (text == "+" || text == "-" || text == "*" || text == "/" || text == "%")
                    {
                        btn.BackColor = Color.FromArgb(63, 63, 85);
                        btn.ForeColor = Color.FromArgb(165, 180, 252);
                    }
                    else if (text.StartsWith("M") || text == "C" || text == "CE" || text == "<-" || text == "+/-" || text == "sqrt" || text == "x^2" || text == "1/x" || text == "Hist")
                    {
                        btn.BackColor = Color.FromArgb(48, 48, 62);
                        btn.ForeColor = Color.FromArgb(200, 205, 220);
                        btn.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
                    }
                    else
                    {
                        btn.BackColor = Color.FromArgb(58, 60, 76);
                        btn.ForeColor = Color.White;
                    }

                    string capturedText = text;
                    btn.Click += new EventHandler(delegate(object s, EventArgs e) {
                        HandleButton(capturedText);
                    });

                    tblGrid.Controls.Add(btn, col, row);
                }
            }

            pnlCalc.Controls.Add(tblGrid);
            pnlCalc.Controls.SetChildIndex(pnlDisplay, 0);
            pnlCalc.Controls.SetChildIndex(tblGrid, 1);
        }

        private void HandleButton(string text)
        {
            if (hasError && text != "C" && text != "CE")
            {
                ClearAll();
            }

            switch (text)
            {
                case "0": case "1": case "2": case "3": case "4":
                case "5": case "6": case "7": case "8": case "9":
                    InputDigit(text);
                    break;
                case ".":
                    InputDecimal();
                    break;
                case "+": case "-": case "*": case "/": case "%":
                    SetOperator(text);
                    break;
                case "=":
                    CalculateResult();
                    break;
                case "C":
                    ClearAll();
                    break;
                case "CE":
                    ClearEntry();
                    break;
                case "<-":
                    Backspace();
                    break;
                case "+/-":
                    Negate();
                    break;
                case "sqrt":
                    ApplyUnary("sqrt");
                    break;
                case "x^2":
                    ApplyUnary("sqr");
                    break;
                case "1/x":
                    ApplyUnary("recip");
                    break;
                case "MC":
                    memoryValue = 0.0;
                    hasMemory = false;
                    lblMemoryStatus.Text = "";
                    break;
                case "MR":
                    if (hasMemory)
                    {
                        txtDisplay.Text = memoryValue.ToString(CultureInfo.InvariantCulture);
                        isNewEntry = true;
                    }
                    break;
                case "M+":
                    try {
                        double val = double.Parse(txtDisplay.Text, CultureInfo.InvariantCulture);
                        memoryValue += val;
                        hasMemory = true;
                        lblMemoryStatus.Text = "M (" + memoryValue.ToString("G6", CultureInfo.InvariantCulture) + ")";
                    } catch {}
                    break;
                case "M-":
                    try {
                        double val = double.Parse(txtDisplay.Text, CultureInfo.InvariantCulture);
                        memoryValue -= val;
                        hasMemory = true;
                        lblMemoryStatus.Text = "M (" + memoryValue.ToString("G6", CultureInfo.InvariantCulture) + ")";
                    } catch {}
                    break;
                case "Hist":
                    pnlHistory.Visible = !pnlHistory.Visible;
                    break;
            }
        }

        private void InputDigit(string digit)
        {
            if (isNewEntry || txtDisplay.Text == "0")
            {
                txtDisplay.Text = digit;
                isNewEntry = false;
            }
            else
            {
                if (txtDisplay.Text.Length < 16)
                    txtDisplay.Text += digit;
            }
        }

        private void InputDecimal()
        {
            if (isNewEntry)
            {
                txtDisplay.Text = "0.";
                isNewEntry = false;
            }
            else if (!txtDisplay.Text.Contains("."))
            {
                txtDisplay.Text += ".";
            }
        }

        private void SetOperator(string op)
        {
            if (!string.IsNullOrEmpty(pendingOperator) && !isNewEntry)
            {
                CalculateResult();
            }

            try
            {
                lastOperand = double.Parse(txtDisplay.Text, CultureInfo.InvariantCulture);
                pendingOperator = op;
                lblExpression.Text = lastOperand.ToString("G8", CultureInfo.InvariantCulture) + " " + op;
                isNewEntry = true;
            }
            catch
            {
                ShowError("Blad danych");
            }
        }

        private void CalculateResult()
        {
            if (string.IsNullOrEmpty(pendingOperator)) return;

            try
            {
                double current = double.Parse(txtDisplay.Text, CultureInfo.InvariantCulture);
                double result = 0.0;
                string expr = lastOperand.ToString("G8", CultureInfo.InvariantCulture) + " " + pendingOperator + " " + current.ToString("G8", CultureInfo.InvariantCulture);

                switch (pendingOperator)
                {
                    case "+": result = lastOperand + current; break;
                    case "-": result = lastOperand - current; break;
                    case "*": result = lastOperand * current; break;
                    case "/":
                        if (Math.Abs(current) < 0.0000000001)
                        {
                            ShowError("Nie mozna dzielic przez 0");
                            return;
                        }
                        result = lastOperand / current;
                        break;
                    case "%":
                        result = (lastOperand * current) / 100.0;
                        break;
                }

                string resStr = result.ToString("G10", CultureInfo.InvariantCulture);
                txtDisplay.Text = resStr;
                lblExpression.Text = expr + " =";
                
                // Zapis do historii
                lstHistory.Items.Insert(0, expr + " = " + resStr);
                
                pendingOperator = "";
                isNewEntry = true;
            }
            catch (Exception)
            {
                ShowError("Blad");
            }
        }

        private void ApplyUnary(string func)
        {
            try
            {
                double val = double.Parse(txtDisplay.Text, CultureInfo.InvariantCulture);
                double result = 0.0;
                string desc = "";

                if (func == "sqrt")
                {
                    if (val < 0) { ShowError("Zla wartosc"); return; }
                    result = Math.Sqrt(val);
                    desc = "sqrt(" + val.ToString(CultureInfo.InvariantCulture) + ")";
                }
                else if (func == "sqr")
                {
                    result = val * val;
                    desc = "sqr(" + val.ToString(CultureInfo.InvariantCulture) + ")";
                }
                else if (func == "recip")
                {
                    if (Math.Abs(val) < 0.0000000001) { ShowError("Nie dziel przez 0"); return; }
                    result = 1.0 / val;
                    desc = "1/(" + val.ToString(CultureInfo.InvariantCulture) + ")";
                }

                string resStr = result.ToString("G10", CultureInfo.InvariantCulture);
                txtDisplay.Text = resStr;
                lblExpression.Text = desc + " =";
                lstHistory.Items.Insert(0, desc + " = " + resStr);
                isNewEntry = true;
            }
            catch
            {
                ShowError("Blad");
            }
        }

        private void Negate()
        {
            try
            {
                double val = double.Parse(txtDisplay.Text, CultureInfo.InvariantCulture);
                val = -val;
                txtDisplay.Text = val.ToString(CultureInfo.InvariantCulture);
            }
            catch {}
        }

        private void Backspace()
        {
            if (isNewEntry || hasError) return;
            if (txtDisplay.Text.Length > 1)
            {
                txtDisplay.Text = txtDisplay.Text.Substring(0, txtDisplay.Text.Length - 1);
            }
            else
            {
                txtDisplay.Text = "0";
                isNewEntry = true;
            }
        }

        private void ClearAll()
        {
            txtDisplay.Text = "0";
            lblExpression.Text = "";
            pendingOperator = "";
            lastOperand = 0.0;
            isNewEntry = true;
            hasError = false;
        }

        private void ClearEntry()
        {
            txtDisplay.Text = "0";
            isNewEntry = true;
            hasError = false;
        }

        private void ShowError(string msg)
        {
            txtDisplay.Text = msg;
            hasError = true;
            isNewEntry = true;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9 && !e.Shift)
            {
                InputDigit(((int)e.KeyCode - (int)Keys.D0).ToString());
                e.Handled = true;
            }
            else if (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9)
            {
                InputDigit(((int)e.KeyCode - (int)Keys.NumPad0).ToString());
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.OemPeriod || e.KeyCode == Keys.Decimal || e.KeyCode == Keys.Oemcomma)
            {
                InputDecimal();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Add || (e.KeyCode == Keys.Oemplus && e.Shift))
            {
                SetOperator("+");
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus)
            {
                SetOperator("-");
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Multiply)
            {
                SetOperator("*");
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Divide || e.KeyCode == Keys.OemQuestion)
            {
                SetOperator("/");
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Oemplus)
            {
                CalculateResult();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Back)
            {
                Backspace();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                ClearAll();
                e.Handled = true;
            }
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
