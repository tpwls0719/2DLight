using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class DialogRun : MonoBehaviour
{
    [SerializeField]
    private GameObject dialguePanel;
    [SerializeField]
    private DialogSystem[] dialogSystems;
    [SerializeField]
    private TextMeshProUGUI textCountdown;
    [SerializeField]
    private string EndText;
    [SerializeField]
    private PlayableDirector playableDirector;
    public void StartDialogFromTimeline()
    {
        if (playableDirector != null)
            playableDirector.Pause();

        dialguePanel.SetActive(true);
        StartDialogCore();
    }
    public void StartDialogFromButtom()
    {
        StartDialogCore();
    }

    private void StartDialogCore()
    {
        textCountdown.gameObject.SetActive(false);
        StartCoroutine(RunDialogSystems());
    }

    private IEnumerator RunDialogSystems()
    {
        textCountdown.gameObject.SetActive(false);
        for (int i = 0; i < dialogSystems.Length; i++)
        {
            yield return new WaitUntil(() => dialogSystems[i].UpdateDialog());
        }
        
        //엔딩 메세지 출력
        textCountdown.gameObject.SetActive(true);
        textCountdown.text = EndText;
        yield return new WaitForSeconds(2);
        dialguePanel.SetActive(false);

        //타임라인에서 시작한 경우에만 Resume
        if (playableDirector != null && playableDirector.state == PlayState.Paused)
            playableDirector.Resume();
    }
   
}
