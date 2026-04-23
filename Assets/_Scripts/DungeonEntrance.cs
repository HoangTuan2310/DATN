using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonPortal : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Agent>())
        {
            StartCoroutine(LoadSceneRoutine());
        }
    }

    private IEnumerator LoadSceneRoutine()
    {
        UIFade.Instance.FadeToBlack();

        yield return new WaitUntil(() => UIFade.Instance.IsFinished());

        SceneManager.LoadScene(sceneToLoad);
        UIFade.Instance.FadeToClear();
    }
}