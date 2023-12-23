using UnityEngine;

public class Lion : MonoBehaviour {

    private Animator anim;
    private bool timerStarted = false;
    
    void Start() {
        anim = GetComponent<Animator>();
        InvokeRepeating("startAttackAnimation", 0f, 3f);
    }

    void Update() {
        if (GameController.beginTimer && !timerStarted) {
            timerStarted = true;
            anim.SetBool("sit", true);
        }
    }

    private void startAttackAnimation() {
        if (!GameController.gamePaused) {
            System.Random random = new System.Random();
            int randNum = random.Next(1, 4);   //Gerando um número aleatório entre 1 e 3
            if (randNum == 1) {
                if (!GameController.beginTimer) {     //Se o tempo não tiver começado a correr
                    SoundController.GetInstance().PlaySound("leao_ataque", gameObject);
                    anim.SetBool("attack", true);
                }
            }
        }
    }
    private void endAttackAnimation() {
        anim.SetBool("attack", false);
    }
}
