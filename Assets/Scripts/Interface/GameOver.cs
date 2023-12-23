using UnityEngine;

public class GameOver : MonoBehaviour {

    //Métodos que serão ativados depois de algum botão da tela de game over ser pressionado
    public void ReturnToMenu() {
        SoundController.GetInstance().PlaySound("btn_click", null);
        TransitionsController.GetInstance().LoadNextScene();
    }
    public void RestartGame() {
        SoundController.GetInstance().PlaySound("btn_click", null);
        TransitionsController.GetInstance().LoadMainScene();
    }
    public void QuitGame() {
        Application.Quit();
    }
}
