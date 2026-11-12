using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class RandomOffsetTriggerOnStart : MonoBehaviour
{
    [Tooltip("The SHORTEST amount of time in seconds the game can run this trigger when it starts.")]
    public float minimumStartTime = 0;
    [Tooltip("The LONGEST amount of time in seconds the game can run this trigger when it starts.")]
    public float maximumStartTime = 1;
    [HideInInspector]
    public float timeToWait;
    [Tooltip("The event that triggers when 'Time To Wait' reaches zero.")]
    public UnityEvent timedEvent;
    [Tooltip("The number of times this timer will repeat after reaching zero. The event 'Timed Event' will trigger each time the timer reaches zero, then the timer will reset to the value in 'Time To Wait'.")]
    public TMP_Text displayText;
    [TextAreaAttribute]
    [Tooltip("The format for how 'Display Text' will show the time left. \"{0:0}\" is a placeholder for the number left, but you can add any text you want before or after it. You can change the decimal point displayed by adding decimal zeros to the right number, e.g. \"{0:0.0}\" will show one decimal point and \"{0:0.00}\" will show two.")]
    public string displayFormat = "{0:0}";
    [TextAreaAttribute]
    [Tooltip("The text that will display on 'Display Text' when the timer reaches zero.")]
    public string completeText = "";
    private bool timerActive = false;
    private float currentTimeLeft = 0;
    private int currentRepeats = 0;
    [Space]
    [Header("Debug")]
    [Tooltip("Whether or not this script prints information to the debug console.")]
    public bool consoleLogTimer = false;
    public void Start()
    {
        timerActive = true;
        timeToWait = Random.Range(minimumStartTime, maximumStartTime);
        currentTimeLeft = timeToWait;
        if (consoleLogTimer) Debug.Log("Timer Started (" + timeToWait + "s): " + gameObject.name);
    }
    public void CancelTimer()
    {
        timerActive = false;
        if (consoleLogTimer) Debug.Log("Timer Cancelled: " + gameObject.name);
    }
    public void Update()
    {
        if (timerActive)
        {
            currentTimeLeft -= Time.deltaTime;
            if (currentTimeLeft <= 0)
            {
                timedEvent.Invoke();

                timerActive = false;
                if (displayText != null)
                {
                    displayText.text = completeText;
                }
                if (consoleLogTimer) Debug.Log("Timer Triggered: " + gameObject.name);
                
            }
            if (displayText != null && timerActive)
            {
                displayText.text = string.Format(displayFormat, currentTimeLeft);
            }
        }
    }
}
