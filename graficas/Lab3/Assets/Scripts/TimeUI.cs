using UnityEngine;
using TMPro;

public class TimeUI : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    // Start is called before the first frame updateprivate void OnEnable()

    private void UpdateTime()
{
    timeText.text = $"{TimeManager.Hour:00}:{TimeManager.Minute:00}";
}
    private void OnEnable()
    {
        TimeManager.OnMinuteChanged += UpdateTime;
        TimeManager.OnHourChanged += UpdateTime;
    }

    private void OnDisable()
    {
        TimeManager.OnMinuteChanged -= UpdateTime;
        TimeManager.OnHourChanged -= UpdateTime;
    }

}
