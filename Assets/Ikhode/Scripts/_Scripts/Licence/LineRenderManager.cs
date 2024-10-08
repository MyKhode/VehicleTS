using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Licence
{
    public class LineRenderManager : MonoBehaviour
    {
        public Collider _collider;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        private void OnTriggerEnter(Collider other)
        {
            // Debug.Log(other.gameObject.layer);
            if ( other.gameObject.layer == 28)
            {
                gameObject.SetActive(false);
            }

        }

    }
}