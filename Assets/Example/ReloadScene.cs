using UnityEngine;
using UnityEngine.SceneManagement;

namespace MtC.Tools.ObjectPool
{
    public class ReloadScene : MonoBehaviour
    {
        public void Reload()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}