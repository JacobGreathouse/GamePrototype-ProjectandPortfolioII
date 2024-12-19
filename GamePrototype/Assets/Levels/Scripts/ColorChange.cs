using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
    [SerializeField] Light rave;
    [SerializeField] Renderer target;
    private float lerp = 20f;

    void Start()
    {
        StartCoroutine(ColorChangeRoutine());
    }

    private IEnumerator ColorChangeRoutine()
    {
        while (true)
        {
            var startColor = rave.color;
            var endColor = new Color32(System.Convert.ToByte(Random.Range(0, 255)), System.Convert.ToByte(Random.Range(0, 255)), System.Convert.ToByte(Random.Range(0, 255)), 255);

            float t = 0;
            while (t < 1)
            {
                t += lerp + Time.deltaTime; // Multiply Time.deltaTime by some constant to speed/slow the transition.
                rave.color = Color.Lerp(startColor, endColor, t);
                yield return null;
            }

            yield return new WaitForSeconds(1); ;
        }
    }
}
