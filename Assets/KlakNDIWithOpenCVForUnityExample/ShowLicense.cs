using UnityEngine;
using UnityEngine.SceneManagement;

namespace KlakNDIWithOpenCVForUnityExample
{

    public class ShowLicense : MonoBehaviour
    {

        public void OnBackButtonClick()
        {
            SceneManager.LoadScene("KlakNDIWithOpenCVForUnityExample");
        }
    }
}
