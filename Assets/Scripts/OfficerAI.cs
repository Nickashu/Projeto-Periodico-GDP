using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using System.Collections;

public class OfficerAI : MonoBehaviour {
    private Transform target;   //target representa a posição para a qual o guarda está indo
    public Transform transformPlayer;
    private Player scriptPlayer;
    private Animator anim;
    public List<Transform> patrolPoints;    //Estas serão as posições que o guarda passa ao patrulhar o cenário

    [SerializeField]
    private float speedChasing = 500f;
    [SerializeField]
    private float  speedPatrolling = 300f;
    public float maxDownGridY, maxRightGridX;   //Estas variáveis armazenam o até onde a área de grid do A* se extende para baixo e para a direita 

    //Estas variáveis são usadas para movimentar o guarda pelo grid:
    private float nextWaypointDistance = 1f, speed;
    private int currentWaypoint=0, patrolPointId;
    private bool isChasing = false, going, flagMovement = false;

    private Path path;
    private Seeker seekerScript;
    private Rigidbody2D rb;

    [SerializeField]
    private float minDistPlayer = 13f;

    private void Start() {
        seekerScript = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        anim = transform.GetChild(0).gameObject.GetComponent<Animator>();
        scriptPlayer = transformPlayer.gameObject.GetComponent<Player>();

        patrolPointId = 0;
        updatePatrolPoint();

        InvokeRepeating("UpdatePath", 0f, 0.1f);   //Inicializando um caminho para o guarda seguir e fazendo este caminho ser calculado continuamente
        InvokeRepeating("UpdateAnimation", 1f, 0.3f);   //Atualizando a animação do guarda andando
        InvokeRepeating("PlaySoundOfficer", 0f, 3f);
    }
    private void Update() {
        float playerDistance = Vector2.Distance(rb.position, transformPlayer.position);
        //Debug.Log("Distância do jogador: " + playerDistance);
        if(playerDistance <= minDistPlayer && playerInGrid()) {    //Se a dsistância do guarda para o jogador for de menos de 20 e o jogador estiver na área do grid
            if (!isChasing && !GameController.gameIsPaused()) {
                target = transformPlayer;
                SoundController.GetInstance().PlaySound("guarda_surpreso", gameObject);
                SoundController.GetInstance().PlaySound("OST_tension", null);
            }
            isChasing = true;
            speed = speedChasing;
        }
        else {
            if (isChasing) {
                patrolPointId = 0;
                updatePatrolPoint();
                if (GameController.beginTimer)
                    SoundController.GetInstance().PlaySound("OST_trilha1_timer", null);
                else {
                    if(playerInGrid())   //Se o jogador ainda estiver na área dos guardas
                        SoundController.GetInstance().PlaySound("OST_trilha1", null);
                }
            }
            isChasing = false;
            speed = speedPatrolling;
        }

        if(playerDistance <= 1.55f && !GameController.playerCaught) {
            SoundController.GetInstance().PlaySound("gato_capturado", null);
            GameController.playerCaught = true;
        }
    }

    private void FixedUpdate() {
        if (!GameController.gameIsPaused()) {
            if (!GameController.playerCaught) {
                if (path != null) {   //Se existir um caminho calculado
                    Vector2 movementDirection = new Vector2();
                    Vector2 movementForce = new Vector2();
                    if (currentWaypoint >= path.vectorPath.Count) {   //Se chegamos ao fim do caminho
                        if (!isChasing && !flagMovement) {   //Se não está perseguido o jogador, vou atualizar o ponto de patrulha
                            flagMovement = true;
                            movementForce = Vector2.zero;
                            if (patrolPointId == 0)
                                going = true;
                            else if (patrolPointId == patrolPoints.Count - 1)
                                going = false;
                            if (going)
                                patrolPointId++;
                            else
                                patrolPointId--;

                            updatePatrolPoint();   //Atualizando o ponto para o qual o inimigo seguirá
                            UpdateAnimation();
                        }
                    }
                    else {    //Se não chegamos ao fim do caminho, vamos mover o guarda para seu alvo
                        flagMovement = false;
                        movementDirection = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
                        movementForce = movementDirection * speed * Time.deltaTime;

                        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
                        if (distance < nextWaypointDistance)
                            currentWaypoint++;
                    }
                    rb.AddForce(movementForce);   //Fazendo o guarda se mover através do RigidBody2D
                }
            }
            else {   //Se o guarda conseguiu chegar até o gato
                rb.velocity = Vector2.zero;
                StartCoroutine(catchPlayer());
            }
        }
        else {
            anim.SetFloat("Speed", 0);
            rb.velocity = Vector2.zero;   //Fazendo o guarda parar de se mover
        }
    }

    private void UpdatePath() {
        if (seekerScript.IsDone())   //Se já terminou de calcular o último caminho
            seekerScript.StartPath(rb.position, target.position, onPathComplete);
    }

    private void updatePatrolPoint() {
        target = patrolPoints[patrolPointId];   //Atualizando o alvo do movimento
    }

    private void onPathComplete(Path p) {
        if (!p.error) {
            this.path = p;
            this.currentWaypoint = 0;
        }
    }

    private bool playerInGrid() {
        if (transformPlayer.position.x < maxRightGridX && transformPlayer.position.y > maxDownGridY)
            return true;
        return false;
    }
    private IEnumerator catchPlayer() {
        GameController.gamePaused = true;
        yield return new WaitForSeconds(2.0f);
        StopAllCoroutines();
        Debug.Log("Capturou!");
        scriptPlayer.looseLife();   //Fazendo o jogador perder uma vida e voltar ao início do mapa
    }

    private void PlaySoundOfficer() { 
        if (!isChasing && !GameController.gameIsPaused()) {   //Se o guarda estiver na patrulha
            System.Random random = new System.Random();
            int randNum = random.Next(1, 6);
            if (randNum == 5) {
                SoundController.GetInstance().PlaySound("guarda_falando", gameObject);
                //Debug.Log(gameObject.name + " falou!");
            }
            else {
                SoundController.GetInstance().PlaySound("guarda_assobiando", gameObject);
                //Debug.Log(gameObject.name + " assobiou!");
            }
        }
    }
    

    private void UpdateAnimation() {
        if(currentWaypoint < path.vectorPath.Count) {
            Vector2 teste = (Vector2)path.vectorPath[currentWaypoint] - rb.position;
            //Animações do guarda andando:
            if (Mathf.Abs(teste.x) > 0.0001 || Mathf.Abs(teste.y) > 0.0001)
                anim.SetFloat("Speed", 5);
            else
                anim.SetFloat("Speed", 0);
            if (Mathf.Abs(teste.x) > Mathf.Abs(teste.y)) {
                int x = teste.x < 0 ? -1 : 1;
                anim.SetFloat("Horizontal", x);
                anim.SetFloat("Vertical", 0);
            }
            else {
                int y = teste.y < 0 ? -1 : 1;
                anim.SetFloat("Vertical", y);
                anim.SetFloat("Horizontal", 0);
            }
        }
    }
}
