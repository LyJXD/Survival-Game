using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour, ISaveManager
{
    public static bool GameIsPaused = false;
    private bool optionIsOpened = false;

    public GameObject pauseMenuUI;
    public GameObject optionUI;

    [SerializeField] private UI_VolumeSlider[] volumeSettings;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused && !optionIsOpened)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        ExitOption();
    }

    public void LoadOption()
    {
        pauseMenuUI.SetActive(false);
        optionUI.SetActive(true);
        optionIsOpened = true;
    }

    public void ExitOption()
    {
        optionUI.SetActive(false);
        optionIsOpened = false;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void LoadData(GameData _data)
    {
        foreach (KeyValuePair<string, float> pair in _data.volumeSetting)
        {
            foreach (UI_VolumeSlider item in volumeSettings)
            {
                if (item.parameter == pair.Key)
                {
                    item.LoadSlider(pair.Value);
                }
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.volumeSetting.Clear();

        foreach (UI_VolumeSlider item in volumeSettings)
        {
            _data.volumeSetting.Add(item.parameter, item.slider.value);
        }
    }
}
