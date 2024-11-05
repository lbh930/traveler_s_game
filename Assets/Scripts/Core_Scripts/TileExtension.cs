using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
    public class TileExtension : MonoBehaviour
    {
        // Start is called before the first frame update
        bool initialized = false;
        void Initialize()
        {

            if (initialized) return;
            initialized = true;
        }

        void Start()
        {
            Initialize();

        }

        // Update is called once per frame
        void Update()
        {
            Initialize();

        }
    }
}
