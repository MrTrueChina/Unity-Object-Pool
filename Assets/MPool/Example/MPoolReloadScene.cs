using UnityEngine;
using UnityEngine.SceneManagement;

public class MPoolReloadScene : MonoBehaviour
{
    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}