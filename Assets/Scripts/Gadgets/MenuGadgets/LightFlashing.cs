using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlashing : MonoBehaviour
{
    float alphaBase;
    public float emissionBase = 1;
    public Color emissionColor = Color.white;
    float emissionNow = 1;

    public float alphaShift = 0.1f;
    public float emissionShift = 0.5f;
    public float alphaSpeed = 0.1f;
    float alphaTarget = 0;
    public float emissionSpeed = 1f;
    float emissionTarget = 0;

    MeshRenderer renderer;
    Material mat;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        mat = renderer.material;
        mat.EnableKeyword("_EMISSION");

        alphaTarget = alphaBase + Random.Range(-1 * alphaShift, alphaShift);
        alphaBase = mat.color.a;
        emissionTarget = emissionBase + Random.Range(-1 * emissionShift, emissionShift);
    }

    // Update is called once per frame
    void Update()
    {
        mat.color = new Color(mat.color.r, mat.color.g, mat.color.b,
            Mathf.MoveTowards(mat.color.a, alphaTarget, Time.deltaTime * alphaSpeed));
        if (Mathf.Abs (mat.color.a - alphaTarget) < 0.001f)
        {
            alphaTarget = alphaBase + Random.Range(-1 * alphaShift, alphaShift);
        }

        if (Mathf.Abs(emissionNow - emissionTarget) < 0.001f)
        {
            emissionTarget = emissionBase + Random.Range(-1 * emissionShift, emissionShift);
        }
        emissionNow = Mathf.MoveTowards(emissionNow, emissionTarget, Time.deltaTime * emissionSpeed);
        mat.SetColor("_EmissionColor", emissionColor*emissionNow);
    }
}
