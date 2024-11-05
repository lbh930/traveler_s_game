using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyHorseAutoDestroy : MonoBehaviour
{
    public HumanAi ai;
    public float destroyTimer = 15;
    public GameObject smoke;
    // Start is called before the first frame update
    void Start()
    {
        ai = GetComponent<HumanAi>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ai != null)
        {
            if (ai.rider == null && ai.state == HumanAi.CreatureState.Escape)
            { //∆Ô ÷À¿Õˆ
                destroyTimer -= Time.deltaTime;
                if (destroyTimer < 0)
                {
                    Instantiate(smoke, transform.position + Vector3.up * 0.1f, transform.rotation);
                    Destroy(gameObject);
                }
            }
        }
    }
}
