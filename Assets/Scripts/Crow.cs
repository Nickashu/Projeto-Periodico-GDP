using System;
using System.Collections;
using UnityEngine;

public class Crow : MonoBehaviour {

    private Animator anim;
    private SpriteRenderer spriteRenderer;
    public GameObject player, wallBlocking;
    private bool isJumping = false, checkFly=false;
    private float offSetPlayer = 1.5f;
    public int crowVersion;    //Esta variável vai representar qual é o ponto do jogo em que este corvo foi carregado


    void Start() {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        if (!checkFly) {    //Se o corvo não estiver prestes a voar
            if (!isJumping) {
                isJumping = true;
                StartCoroutine(jump());
            }
            //Fazendo o corvo ficar virado para onde o jogador estiver:
            if (transform.position.x > player.transform.position.x + offSetPlayer)
                spriteRenderer.flipX = true;
            else if (transform.position.x < player.transform.position.x - offSetPlayer)
                spriteRenderer.flipX = false;


            //Verificando se o player já deu a comida para o corvo:
            if ((GameController.terminouDeFalarCorvo1 && crowVersion == 1)) {
                checkFly = true;
                StartCoroutine(flyDelay());
            }
        }
    }

    private IEnumerator jump() {
        System.Random random = new System.Random();
        int randNum = random.Next(1, 4);   //Gerando um número aleatório entre 1 e 3
        if(randNum == 1)
            anim.SetBool("jump", true);
        yield return new WaitForSeconds(3.0f);   //Esperando 3 segundos
        isJumping = false;
    }

    private IEnumerator flyDelay() {
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("fly", true);
    }

    private void endJumpAnimation() {     //Evento ao acabar a animação do corvo pulando
        anim.SetBool("jump", false);
    }

    private void startFlyAnimation() {     //Evento ao acabar a animação do corvo voando
        spriteRenderer.flipX = false;
    }
    private void endFlyAnimation() {     //Evento ao acabar a animação do corvo voando
        Destroy(transform.gameObject);
        GameController.acabouTutorial = true;
        wallBlocking.SetActive(false);    //Desbloqueando a entrada para o jogador
    }

    private void playSoundJump() {
        SoundController.GetInstance().PlaySound("corvo_pulo", gameObject);
    }
}
