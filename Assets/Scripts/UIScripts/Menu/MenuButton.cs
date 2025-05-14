using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private MenuButtonController menuButtonController;
    [SerializeField] private Animator anim;
    [SerializeField] private AnimatorTriggers animatorTriggers;
    [SerializeField] private int thisIndex;

    public GameObject optionUI;

    private void Awake()
    {

    }

    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            optionUI.SetActive(false);
        }

        if (menuButtonController.index == thisIndex)
        {
            if (Input.GetAxis("Submit") == 1)
            {
                if (thisIndex == 0)
                {
                    StartCoroutine(GameManager.Instance.LoadScene(GameManager.Instance.LoadDelay, "MainScene"));

                }
                if (thisIndex == 1)
                {
                    //SaveGameManager.Instance.DeleteSavedData();
                    StartCoroutine(GameManager.Instance.LoadScene(GameManager.Instance.LoadDelay, "MainScene"));
                }
                if (thisIndex == 2)
                {
                    optionUI.SetActive(true);
                }
                if (thisIndex == 3)
                {
                    if (Application.isPlaying & !Application.isEditor)
                        Application.Quit();
#if false
	else
		UnityEditor.EditorApplication.isPlaying = false;    // Handle being in the editor, but set #if to true to use it
#endif
                }
            }

            anim.SetBool("isSelected", true);
            if (Input.GetAxis("Submit") == 1)
            {
                anim.SetBool("isPressed", true);
            }
            else if (anim.GetBool("isPressed"))
            {
                anim.SetBool("isPressed", false);
                animatorTriggers.disableOnce = true;
            }
        }
        else
        {
            anim.SetBool("isSelected", false);
        }
    }

}
