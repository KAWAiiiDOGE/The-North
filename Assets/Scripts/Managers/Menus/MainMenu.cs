using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheNorth
{
    public class MainMenu : MonoBehaviour
    {
        [Scene] public int PlaygroundScene = 1;

        public void Play_Button()
        {
            SceneManager.LoadScene(PlaygroundScene);
        }
        public void Load_Button()
        {

        }
        public void Settings_Button()
        {

        }
        public void Exit_Button()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }
    }
}