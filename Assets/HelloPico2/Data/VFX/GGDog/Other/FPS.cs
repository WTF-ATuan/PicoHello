using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour
{
    [SerializeField] private float updateInterval = 1f;

    private float accum = 0f;
    private int frames = 0;
    private float timeLeft;
    private Text fpsText;


    private void Start()
    {
        timeLeft = updateInterval;
        fpsText = GetComponent<Text>();
    }


    private void Update()
    {
        timeLeft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        frames++;

        if (timeLeft <= 0)
        {
            float fps = accum / frames;
            fpsText.text = $"FPS: {fps:F0}";
            timeLeft = updateInterval;
            accum = 0f;
            frames = 0;
        }
    }
}