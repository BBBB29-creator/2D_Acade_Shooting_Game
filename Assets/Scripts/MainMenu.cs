using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환을 위해 무조건 필수인 네임스페이스

public class MainMenu : MonoBehaviour
{
    // [START 버튼용] 클릭하면 1번 인덱스(인게임 씬)를 불러옵니다.
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    // [EXIT 버튼용] 클릭하면 게임을 완전히 종료합니다. (빌드 후 실행 파일에서만 작동)
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("게임 종료 완료!");
    }
}
