using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CalculatorManager : MonoBehaviour
{
    public static CalculatorManager Instance;
    public string calcalationText;
    public TextMeshProUGUI displayText;
    public AudioSource clickSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        calcalationText = "";
        /*UpdateDisplayText(calcalationText);*/
        StartFlicker();
    }

    public enum buttons
    {
        Num0,
        Num1,
        Num2,
        Num3,
        Num4,
        Num5,
        Num6,
        Num7,
        Num8,
        Num9,
        Add,
        Subtract,
        Multiply,
        Divide,
        Delete,
        Equal,
        Clear,
        Decimal
    }
    private Coroutine flickerCoroutine;

    private void StartFlicker()
    {
        if (flickerCoroutine != null)
            StopCoroutine(flickerCoroutine);

        flickerCoroutine = StartCoroutine(FlickerCursor());
    }

    private IEnumerator FlickerCursor()
    {
        while (string.IsNullOrEmpty(calcalationText))
        {
            displayText.text = "|";
            yield return new WaitForSeconds(0.5f);
            displayText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void UpdateDisplayText(string text)
    {
        calcalationText = text;

        if (string.IsNullOrEmpty(text))
        {
            StartFlicker();
        }
        else
        {
            if (flickerCoroutine != null)
                StopCoroutine(flickerCoroutine);

            displayText.text = text;
        }
    }

    public void OnButtonsClick(buttons button)
    {
        clickSound.Play();
        switch (button)
        {
            case buttons.Num0:
                if (string.IsNullOrEmpty(calcalationText) ||
                    IsOperator(calcalationText[calcalationText.Length - 1]))
                {
                    calcalationText += "0";
                }
                else
                {
                    if (!(calcalationText.EndsWith("0") &&
                         (calcalationText.Length == 1 ||
                          IsOperator(calcalationText[calcalationText.Length - 2]))))
                    {
                        calcalationText += "0";
                    }
                }
                break;

            case buttons.Num1:
                calcalationText += "1";
                break;

            case buttons.Num2:
                calcalationText += "2";
                break;

            case buttons.Num3:
                calcalationText += "3";
                break;

            case buttons.Num4:
                calcalationText += "4";
                break;

            case buttons.Num5:
                calcalationText += "5";
                break;

            case buttons.Num6:
                calcalationText += "6";
                break;

            case buttons.Num7:
                calcalationText += "7";
                break;

            case buttons.Num8:
                calcalationText += "8";
                break;

            case buttons.Num9:
                calcalationText += "9";
                break;

            case buttons.Add:
                if (!string.IsNullOrEmpty(calcalationText) && !IsOperator(calcalationText[^1]))
                    calcalationText += "+";
                break;

            case buttons.Subtract:
                if (string.IsNullOrEmpty(calcalationText) || !IsOperator(calcalationText[^1]) ||calcalationText[^1] == 'x' || calcalationText[^1] == '÷')
                {
                    calcalationText += "-";
                }
                break;

            case buttons.Multiply:
                if (!string.IsNullOrEmpty(calcalationText) && !IsOperator(calcalationText[^1]))
                    calcalationText += "x";
                break;

            case buttons.Divide:
                if (!string.IsNullOrEmpty(calcalationText) && !IsOperator(calcalationText[^1]))
                    calcalationText += "÷";
                break;

            case buttons.Decimal:
                if (string.IsNullOrEmpty(calcalationText) || IsOperator(calcalationText[^1]))
                {
                    calcalationText += "0.";
                }
                else if (CanAddDecimal())
                {
                    calcalationText += ".";
                }
                break;

            case buttons.Delete:
                if (calcalationText.Length > 0)
                {
                    calcalationText = calcalationText.Substring(0, calcalationText.Length - 1);
                }
                break;

            case buttons.Clear:
                calcalationText = "";
                break;

            case buttons.Equal:
                if (!string.IsNullOrEmpty(calcalationText) && !IsOperator(calcalationText[^1]))
                {
                    float result = EvaluateExpression(calcalationText);
                    calcalationText = result.ToString();
                }
                break;
        }

        UpdateDisplayText(calcalationText);
    }

    private float EvaluateExpression(string expr)
    {
        expr = expr.Replace("x", "*").Replace("÷", "/");

        float result = 0;
        float lastNumber = 0;
        char lastOperator = '+';
        int i = 0;

        while (i < expr.Length)
        {
            bool isNegative = false;
            //checking its a negative number
            if (expr[i] == '-' && (i == 0 || IsOperator(expr[i - 1])))
            {
                isNegative = true;
                i++;
            }


            string numStr = "";
            //checking if its a decimal number
            while (i < expr.Length && (char.IsDigit(expr[i]) || expr[i] == '.'))
            {
                numStr += expr[i]; //add the number to the string
                i++;
            }

            if (string.IsNullOrEmpty(numStr)) //Ensures an empty number string is treated as 0
                numStr = "0";

            float current = float.Parse(numStr); //converting to float
            if (isNegative)
            {
                current = -current; //if negative , then make the number negative as a whole
            }

            
            if (lastOperator == '*') 
            {
                lastNumber *= current;
            }
            else if (lastOperator == '/')
            {
                lastNumber /= current;     //first priority
            }
            else
            {
                
                result += lastNumber;     
                lastNumber = current; 


                if (lastOperator == '-')
                {
                    lastNumber = -lastNumber; //again if multiplying or dividing by a negative number
                }
            }


            if (i < expr.Length)
                lastOperator = expr[i++];
        }

        result += lastNumber;
        return result;
    }

    private bool IsOperator(char c)
    {
        return c == '+' || c == '-' || c == '*' || c == '/' || c == 'x' || c == '÷';
    }

    private bool CanAddDecimal()
    {
        string expr = calcalationText;

        if (string.IsNullOrEmpty(expr))
            return true;

        int lastOpIndex = expr.LastIndexOfAny(new char[] { '+', '-', 'x', '÷' });

        string currentNumber = expr.Substring(lastOpIndex + 1);

        return !currentNumber.Contains(".");
    }

}
