using UnityEngine;

public class Amigo : MonoBehaviour {

    private Animator anim;
    private bool timerStarted = false;

    void Start() {
        anim = GetComponent<Animator>();
    }

    void Update() {
        if(GameController.beginTimer && !timerStarted) {
            timerStarted = true;
            anim.SetBool("idle", true);
        }
    }
}
