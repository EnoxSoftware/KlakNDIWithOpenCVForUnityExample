using UnityEngine;
using UnityEngine.SceneManagement;

namespace KlacNDIWithOpenCVForUnityExample
{

    public class ShowLicense : MonoBehaviour
    {

        public void OnBackButtonClick()
        {
            SceneManager.LoadScene("KlacNDIWithOpenCVForUnityExample");
        }
    }
}
