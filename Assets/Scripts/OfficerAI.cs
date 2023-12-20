using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using System.Collections;

public class OfficerAI : MonoBehaviour {
    private Transform target;   //target representa a posi��o para a qual o guarda est� indo
    public Transform transformPlayer;
    private Player scriptPlayer;
    private Animator anim;
    public List<Transform> patrolPoints;    //Estas ser�o as posi��es que o guarda passa ao patrulhar o cen�rio

    [SerializeField]
    private float speedChasing = 500f;
    [SerializeField]
    private float  speedPatrolling = 300f;
    public float maxDownGridY, maxRightGridX;   //Estas vari�veis armazenam o at� onde a �rea de grid do A* se extende para baixo e para a direita 

    //Estas vari�veis s�o usadas para movimentar o guarda pelo grid:
    private float nextWaypointDistance = 1f, speed;
    private int currentWaypoint=0, patrolPointId;
    private bool isChasing = false, going, flagMovement = false;

    private Path path;
    private Seeker seekerScript;
    private Rigidbody2D rb;


    private void Start() {
        seekerScript = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        anim = transform.GetChild(0).gameObject.GetComponent<Animator>();
        scriptPlayer = transformPlayer.gameObject.GetComponent<Player>();

        patrolPointId = 0;
        updatePatrolPoint();

        //Inicializando um caminho para o guarda seguir e fazendo este caminho ser calculado continuamente por meio do InvokeRepeating:
        InvokeRepeating("UpdatePath", 0f, 0.1f);

        InvokeRepeating("UpdateAnimation", 1f, 1f);

        //InvokeRepeating("PlaySoundOfficer", 0f, 3f);
    }
    private void Update() {
        float playerDistance = Vector2.Distance(rb.position, transformPlayer.position);
        //Debug.Log("Dist�ncia do jogador: " + playerDistance);
        if(playerDistance <= 10f && playerInGrid()) {    //Se a dsist�ncia do guarda para o jogador for de menos de 20 e o jogador estiver na �rea do grid
            if(!isChasing)
                target = transformPlayer;
            isChasing = true;
            speed = speedChasing;
        }
        else {
            if (isChasing) {
                patrolPointId = 0;
                updatePatrolPoint();
            }
            isChasing = false;
            speed = speedPatrolling;
        }
    }

    private void FixedUpdate() {
        if (!GameController.gameIsPaused()) {
            if (!GameController.playerCaught) {
                if (path != null) {   //Se existir um caminho calculado
                    Vector2 movementDirection=new Vector2();
                    Vector2 movementForce=new Vector2();
                    if (currentWaypoint >= path.vectorPath.Count) {   //Se chegamos ao fim do caminho
                        if (!isChasing && !flagMovement) {   //Se n�o est� perseguido o jogador, vou atualizar o ponto de patrulha
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

                            updatePatrolPoint();   //Atualizando o ponto para o qual o inimigo seguir�
                            UpdateAnimation();
                        }
                    }
                    else {    //Se n�o chegamos ao fim do caminho, vamos mover o guarda para seu alvo
                        flagMovement = false;
                        movementDirection = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
                        movementForce = movementDirection * speed * Time.deltaTime;

                        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
                        if (distance < nextWaypointDistance)
                            currentWaypoint++;
                    }
                    //Fazendo o guarda se mover atrav�s do RigidBody2D:
                    //rb.MovePosition(rb.position + movementForce);
                    //rb.velocity = movementForce;
                    rb.AddForce(movementForce);
                }
            }
            else {   //Se o guarda conseguiu chegar at� o gato
                rb.velocity = Vector2.zero;
                StartCoroutine(catchPlayer());
            }
        }
        else
            rb.velocity = Vector2.zero;
    }

    private void UpdatePath() {
        if (seekerScript.IsDone())   //Se j� terminou de calcular o �ltimo caminho
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
        scriptPlayer.looseLife();   //Fazendo o jogador perder uma vida e voltar ao in�cio do mapa
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag.Equals("Player") && !GameController.playerCaught) {
            anim.SetFloat("Speed", 0);
            //GameController.playerCaught = true;
        }
    }
    private void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.tag.Equals("Player") && !GameController.playerCaught)
            Debug.Log("T� tocando!!!");
    }
    /*
    private void PlaySoundOfficer() { 
        if (!isChasing) {   //Se o guarda estiver na patrulha
            System.Random random = new System.Random();
            int randNum = random.Next(1, 4);
            if(randNum != 1) { 
                randNum = random.Next(1, 3);
                if (randNum == 1) {
                    SoundController.GetInstance().PlaySound("guarda_assobio", gameObject);
                    //Debug.Log("assobiou");
                }
                else {
                    SoundController.GetInstance().PlaySound("guarda_falando", gameObject);
                    //Debug.Log("falou");
                }
            }
        }
    }
    */

    private void UpdateAnimation() {
        if(currentWaypoint < path.vectorPath.Count) {
            Vector2 teste = (Vector2)path.vectorPath[currentWaypoint] - rb.position;
            Debug.Log("x: " + teste.x + "   y: " + teste.y);
            //Anima��es do guarda andando:
            if (Mathf.Abs(teste.x) > 0.0001 || Mathf.Abs(teste.y) > 0.0001)
                anim.SetFloat("Speed", 5);
            else
                anim.SetFloat("Speed", 0);
            if (Mathf.Abs(teste.x) > Mathf.Abs(teste.y)) {
                int x = teste.x < 0 ? -1 : 1;
                anim.SetFloat("Horizontal", x);
                anim.SetFloat("Vertical", 0);
                Debug.Log("popopopop");
            }
            else {
                int y = teste.y < 0 ? -1 : 1;
                anim.SetFloat("Vertical", y);
                anim.SetFloat("Horizontal", 0);
                Debug.Log("irewirwjeri");
            }
        }
    }
}
