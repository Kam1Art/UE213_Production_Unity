using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    public GameObject FinishLineMenu, pauseButton;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            FinishLineMenu.SetActive(true);
            pauseButton.SetActive(false);
            Time.timeScale = 0;
            AudioSource[] audios = FindObjectsOfType<AudioSource>();

            foreach (AudioSource a in audios)
            {
                a.Pause();
            }
        }
    }
    public void Home()
    {
        SceneManager.LoadScene("Main Menu");
        Time.timeScale = 1;
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }
}
