using UnityEngine;
using UnityEngine.UI;

public class Blink : MonoBehaviour
{
    public float blinkSpeed;
    float timeToNextBlink = 0;
    void Update()
    {
        timeToNextBlink -= Time.deltaTime;
        if(timeToNextBlink <= 0)
        {
            timeToNextBlink += blinkSpeed;
            GetComponent<Image>().color = new Color(1, 1, 1, 1 - GetComponent<Image>().color.a);
        }
    }
}
