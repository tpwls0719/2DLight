using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] 
    private GameObject[] characterPrefabs; // 캐릭터 프리팹들
    
    [SerializeField]
    private Transform spawnPoint; // 캐릭터 스폰 위치 (옵션)
    
    void Start()
    {
        SpawnSelectedCharacter();
    }
    
    void SpawnSelectedCharacter()
    {
        // 저장된 캐릭터 인덱스 로드 (기본값 0)
        int selectedCharacter = PlayerPrefs.GetInt("SelectedCharacter", 0);
        
        // 배열 범위 체크
        if (characterPrefabs != null && selectedCharacter >= 0 && selectedCharacter < characterPrefabs.Length)
        {
            // 선택한 캐릭터 스폰
            if (spawnPoint != null)
            {
                Instantiate(characterPrefabs[selectedCharacter], spawnPoint.position, spawnPoint.rotation);
            }
            else
            {
                Instantiate(characterPrefabs[selectedCharacter]);
            }
        }
        else
        {            
            // 기본 캐릭터 스폰 (첫 번째)
            if (characterPrefabs != null && characterPrefabs.Length > 0)
            {
                Instantiate(characterPrefabs[0]);
            }
        }
    }
}