using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    [SerializeField] private Light torchLight;
    [SerializeField] private float minIntensity = 0.5f;
    [SerializeField] private float maxIntensity = 2f;
    [SerializeField] private float flickingSpeed = 0.01f;
    [SerializeField] private float lerpSpeed = 5f;

    private float randomIntensity;

    void Start()
    {
        StartCoroutine(Flicker());
    }

    private IEnumerator Flicker()
    {
        while (true)
        {
            yield return new WaitForSeconds(flickingSpeed);

            randomIntensity = Random.Range(minIntensity, maxIntensity);
            
        }
    }

    private void Update()
    {
        torchLight.intensity = Mathf.Lerp(torchLight.intensity, randomIntensity, lerpSpeed * Time.deltaTime);
    }
}
