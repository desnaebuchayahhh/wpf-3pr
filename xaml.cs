
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

namespace CalculatorWPF
{
    public partial class MainWindow : Window
    {
        private double? firstOperand = null;
        private string currentOperator = null;
        private string currentExpression = "";
        private bool lastInputWasPiOrE = false;
        private bool lastInputWasInvert = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnNumber_Click(object sender, RoutedEventArgs e)
        {
            string number = (sender as Button).Content.ToString();
            AppendToExpression(number);
            lastInputWasPiOrE = false;
        }

        private void BtnOperator_Click(object sender, RoutedEventArgs e)
        {
            string op = (sender as Button).Content.ToString();

            if (string.IsNullOrEmpty(txtExpression.Text) && op != "-")
            {
                return;
            }

            if (firstOperand == null)
            {
                if (double.TryParse(txtExpression.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double num))
                {
                    firstOperand = num;
                    currentOperator = op;
                    txtExpressionDisplay.Text = txtExpression.Text + " " + op;
                    txtExpression.Text = "";
                    lastInputWasPiOrE = false;
                }
                else
                {
                    txtResult.Text = "Ошибка";
                    return;
                }
            }
            else
            {
              
                CalculateResult(); 
                if (txtResult.Text == "Ошибка") return; 

                firstOperand = double.Parse(txtResult.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture);
                currentOperator = op;
                txtExpressionDisplay.Text = txtResult.Text + " " + op;
                txtExpression.Text = "";
                lastInputWasPiOrE = false;

            }

        }

        private void BtnDecimal_Click(object sender, RoutedEventArgs e)
        {
            if (!txtExpression.Text.Contains(","))
            {
                AppendToExpression(",");
            }
            lastInputWasPiOrE = false;
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
        }

        private void BtnClearEntry_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
        }

        private void BtnBackspace_Click(object sender, RoutedEventArgs e)
        {
            Backspace();
        }

        private void BtnEquals_Click(object sender, RoutedEventArgs e)
        {
            CalculateResult();
        }

        private void UpdateExpressionDisplay()
        {
            // txtExpressionDisplay.Text = currentExpression;
        }

        private void AppendToExpression(string str)
        {
            txtExpression.Text += str;
            currentExpression += str;
            UpdateExpressionDisplay();
        }

        private void ClearAll()
        {
            txtExpression.Text = "";
            txtResult.Text = "";
            firstOperand = null;
            currentOperator = null;
            currentExpression = "";
            txtExpressionDisplay.Text = "";
            UpdateExpressionDisplay();
            lastInputWasPiOrE = false;
            lastInputWasInvert = false;
        }

        private void Backspace()
        {
            if (txtExpression.Text.Length > 0)
            {
                txtExpression.Text = txtExpression.Text.Remove(txtExpression.Text.Length - 1);
                if (currentExpression.Length > 0)
                {
                    currentExpression = currentExpression.Remove(currentExpression.Length - 1);
                    UpdateExpressionDisplay();
                }
            }
        }

        private void CalculateResult()
        {
            try
            {
                if (firstOperand == null || string.IsNullOrEmpty(currentOperator))
                {
                    if (string.IsNullOrEmpty(txtExpression.Text)) return; 
                    if (double.TryParse(txtExpression.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                    {
                        txtResult.Text = result.ToString("N2", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        txtResult.Text = "Ошибка";
                    }
                    return; 
                }

                if (string.IsNullOrEmpty(txtExpression.Text))
                {
                    txtResult.Text = "Ошибка";
                    return; 
                }

                if (double.TryParse(txtExpression.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double secondOperand))
                {
                    double result = 0;

                    switch (currentOperator)
                    {
                        case "+":
                            result = (double)firstOperand + secondOperand;
                            break;
                        case "-":
                            result = (double)firstOperand - secondOperand;
                            break;
                        case "×":
                            result = (double)firstOperand * secondOperand;
                            break;
                        case "÷":
                            if (secondOperand == 0)
                            {
                                txtResult.Text = "Деление на ноль";
                                return;
                            }
                            result = (double)firstOperand / secondOperand;
                            break;
                        case "^":
                            result = Math.Pow((double)firstOperand, secondOperand);
                            break;
                        default:
                            txtResult.Text = "Ошибка: Неизвестный оператор";
                            return;
                    }

                    txtResult.Text = result.ToString("N2", CultureInfo.InvariantCulture);
                    txtExpressionDisplay.Text = "";
                    txtExpression.Text = "";
                    firstOperand = null;
                    currentOperator = null;
                    lastInputWasPiOrE = false;
                    lastInputWasInvert = false;
                }
                else
                {
                    txtResult.Text = "Ошибка: Некорректный ввод";
                }
            }
            catch (Exception ex)
            {
                txtResult.Text = "Ошибка";
                MessageBox.Show(ex.Message, "Ошибка вычисления");
            }
        }

        private void BtnPi_Click(object sender, RoutedEventArgs e)
        {
            if (!lastInputWasPiOrE && txtExpression.Text.Length == 0)
            {
                AppendToExpression(Math.PI.ToString("F15", CultureInfo.InvariantCulture).Substring(0, 17));
                lastInputWasPiOrE = true;
                lastInputWasInvert = false;
            }
        }

        private void BtnE_Click(object sender, RoutedEventArgs e)
        {
            if (!lastInputWasPiOrE && txtExpression.Text.Length == 0)
            {
                AppendToExpression(Math.E.ToString("F15", CultureInfo.InvariantCulture).Substring(0, 17));
                lastInputWasPiOrE = true;
                lastInputWasInvert = false;
            }
        }

        private void BtnOpenParen_Click(object sender, RoutedEventArgs e)
        {
            AppendToExpression("(");
            lastInputWasPiOrE = false;
            lastInputWasInvert = false;
        }

        private void BtnCloseParen_Click(object sender, RoutedEventArgs e)
        {
            AppendToExpression(")");
            lastInputWasPiOrE = false;
            lastInputWasInvert = false;
        }
        private void BtnInvert_Click(object sender, RoutedEventArgs e)
        {
            if (!lastInputWasInvert)
            {
                AppendToExpression("-");
                lastInputWasInvert = true;
            }
        }

        private void CalculateUnaryOperation(Func<double, double> operation)
        {
            try
            {
                if (double.TryParse(txtExpression.Text.Replace(",", "."), out double number))
                {
                    double result = operation(number);
                    txtExpression.Text = result.ToString(CultureInfo.InvariantCulture);
                    txtResult.Text = txtExpression.Text;
                    currentExpression = "";
                    UpdateExpressionDisplay();
                }
                else
                {
                    txtResult.Text = "Ошибка";
                }
            }
            catch (Exception ex)
            {
                txtResult.Text = "Ошибка";
                MessageBox.Show(ex.Message, "Ошибка вычисления");
            }
        }
    }
}
