using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonType : MonoBehaviour
{
    public CalculatorManager.buttons button;

    private void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(() =>
        {
            CalculatorManager.Instance.OnButtonsClick(button);
        });
    }
}
