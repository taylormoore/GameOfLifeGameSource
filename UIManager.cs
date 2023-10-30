using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Michsky.UI.ModernUIPack;


public class UIManager : MonoBehaviour
{
    [SerializeField]
    AudioClip highlightSound;
    [SerializeField]
    AudioClip clickSound;
    [SerializeField]
    AudioClip errorSound;
    [SerializeField]
    ProgressBar loadingProgressBar;
    [SerializeField]
    GameObject progressBarGameObject;
    [SerializeField]
    Texture2D cursorTexture;


    [SerializeField]
    TextMeshProUGUI cameraModeText;

    [SerializeField]
    GameObject loadingIcon;

    [SerializeField]
    GameObject loadingTextGameObject;

    [SerializeField]
    TextMeshProUGUI loadingText;

    [SerializeField]
    Image simulateIcon;

    [SerializeField]
    Sprite[] simulateIconSprites;

    [SerializeField]
    private NotificationManager notificationManager;

    [SerializeField]
    private SliderManager stepSpeedSlider;

    [SerializeField]
    private GameManager gameManager;

    [SerializeField]
    private TextMeshProUGUI sliderValue;

    private SoundManager soundManager;
    private Animator animator;
    private Image blackFadeImage;

    void Start()
    {
        if (notificationManager)
            notificationManager.CloseNotification();

        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        blackFadeImage = transform.Find("Black Fade").GetComponent<Image>();
        animator = GetComponent<Animator>();
        soundManager = GameObject.Find("Sound Manager").GetComponent<SoundManager>();
    }

    public void PlayHighlightSound()
    {
        soundManager.PlaySound(highlightSound, new Vector2(.9f, 1.1f), new Vector2(.9f, 1.1f));
    }

    public void PlayClickSound()
    {
        soundManager.PlaySound(clickSound, new Vector2(.9f, 1.1f), new Vector2(.9f, 1.1f));
    }

    public void PlayErrorSound()
    {
        soundManager.PlaySound(errorSound, new Vector2(.9f, 1.1f), new Vector2(.9f, 1.1f));
    }

    public void ChangeCameraModeText(string text)
    {
        cameraModeText.text = text;
    }

    public void FadeToBlack()
    {
        ClearAnimatorTriggers();
        animator.SetTrigger("fadeToBlack");
    }

    public void FadeFromBlack()
    {
        ClearAnimatorTriggers();
        animator.SetTrigger("fadeFromBlack");
    }

    public void ClearAnimatorTriggers()
    {
        animator.ResetTrigger("fadeToBlack");
        animator.ResetTrigger("fadeFromBlack");
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(ChangeSceneCo(sceneName));
    }

    public IEnumerator FadeFromBlackCo()
    {
        SetLoadingPercent(1f);
        yield return new WaitForSeconds(1f);
        FadeFromBlack();
    }


    public IEnumerator ChangeSceneCo(string sceneName)
    {
        while (blackFadeImage.color.a < 1f)
        {
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }

    public void Anim_EnableLoadIconAndText()
    {
        loadingIcon.SetActive(true);
        loadingTextGameObject.SetActive(true);
        progressBarGameObject.SetActive(true);
    }

    public void Anim_DisableLoadIconAndText()
    {
        loadingIcon.SetActive(false);
        loadingTextGameObject.SetActive(false);
        progressBarGameObject.SetActive(false);
    }

    public void Anim_EnableFadeClickable()
    {
        blackFadeImage.raycastTarget = true;
    }

    public void Anim_DisableFadeClickable()
    {
        blackFadeImage.raycastTarget = false;
    }

    public void SetLoadingPercent(float value)
    {
        loadingProgressBar.currentPercent = value;
    }

    public void UpdateLoadingText(string text)
    {
        loadingText.text = text;
    }

    public void ChangeSimButtonIcon(GameManager.GameState state)
    {
        if (state == GameManager.GameState.Building)
            simulateIcon.sprite = simulateIconSprites[0];
        else
            simulateIcon.sprite = simulateIconSprites[1];
    }

    public void ShowNotification(string title, string description)
    {
        notificationManager.title = title;
        notificationManager.description = description;
        notificationManager.UpdateUI();
        notificationManager.OpenNotification();
    }

    public void UpdateGameSpeed()
    {
        gameManager.gameSpeed = stepSpeedSlider.mainSlider.value;
        sliderValue.text = stepSpeedSlider.mainSlider.value.ToString("#.#");
    }

    public void Anim_ShowCameraTooltip()
    {
        ShowNotification("Press [Tab]", "to toggle camera mode");
    }
}
