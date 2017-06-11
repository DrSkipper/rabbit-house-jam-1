using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCreator : MonoBehaviour
{
    public string SceneName = "TestScene";
    private int timer;

    void Start()
    {
        timer = 1;
        SceneManager.LoadScene(this.SceneName, LoadSceneMode.Additive);
    }

    void Update()
    {
        if (timer == 0)
        {
            timer = -1;

            Scene scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

            GameObject[] rootObjects = scene.GetRootGameObjects();
            for (int i = 0; i < rootObjects.Length; ++i)
            {
                rootObjects[i].transform.SetParent(this.transform, false);
            }

            SceneManager.UnloadSceneAsync(this.SceneName);
        }
        else if (timer > 0)
        {
            --timer;
        }
    }
}
