using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    public int nextSceneIndex = 2;
    public void SelectCharacter(int charcterIndex)
    {
        PlayerPrefs.SetInt("SelectedCharacter", charcterIndex);
        PlayerPrefs.Save();
        SceneTransitionManager.Instance.FadeOutAndChangeScene(nextSceneIndex);
    }
}
