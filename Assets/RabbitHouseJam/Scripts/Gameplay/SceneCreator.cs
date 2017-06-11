using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCreator : MonoBehaviour
{
    public string SceneName = "TestScene";

    void Start()
    {
        SceneManager.LoadScene(this.SceneName, LoadSceneMode.Additive);
        Scene scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        GameObject[] rootObjects = scene.GetRootGameObjects();
        for (int i = 0; i < rootObjects.Length; ++i)
        {
            rootObjects[i].transform.parent = this.transform;
        }

        SceneManager.UnloadSceneAsync(this.SceneName);
    }
}
