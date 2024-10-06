using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ikhode_Net
{
    public class SceneLoaderManager : MonoBehaviour
    {
        [Header("AutoSceneLoader")]
        [SerializeField] private bool isAutoLoad;
        [SerializeField] string sceneName;
        // Start is called before the first frame update
        void Start()
        {
            if (isAutoLoad)
            {
                SceneManager.LoadScene(sceneName);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
