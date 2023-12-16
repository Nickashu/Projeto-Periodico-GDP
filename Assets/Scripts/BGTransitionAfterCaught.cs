using UnityEngine;

public class BGTransitionAfterCaught : MonoBehaviour {
    public GameObject player;

    private void repositionPlayer() {
        player.gameObject.GetComponent<Player>().returnToInitialPosition();   //Fazendo o player retornar para o início do mapa durante a animação da tela de transição
        GameController.gamePaused = false;
    }
}
