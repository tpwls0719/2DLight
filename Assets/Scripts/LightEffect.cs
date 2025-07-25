using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightEffect : MonoBehaviour
{
    Light2D targetLight2D;
    [Header("깜박임 속도")]
    public float flickerSpeed = 5f;
    [Header("최소 밝기")]
    public float minIntensity = 0.3f;
    [Header("최대 밝기")]
    public float maxIntensity = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetLight2D = GetComponent<Light2D>();
        if (targetLight2D == null)
        {
            Debug.LogError("Light2D 컴포넌트가 없습니다!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (targetLight2D != null)
        {
            float t = (Mathf.Sin(Time.time * flickerSpeed) + 1f) * 0.5f;
            targetLight2D.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
        }
    }
}
