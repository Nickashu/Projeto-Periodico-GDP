using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using System.Collections;

public class OfficerAI : MonoBehaviour {
    private Transform target;   //target representa a posição para a qual o guarda está indo
    public Transform transformPlayer;
    private Player scriptPlayer;
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


    private void Start() {
        seekerScript = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        scriptPlayer = transformPlayer.gameObject.GetComponent<Player>();

        patrolPointId = 0;
        updatePatrolPoint();

        //Inicializando um caminho para o guarda seguir e fazendo este caminho ser calculado continuamente por meio do InvokeRepeating:
        InvokeRepeating("UpdatePath", 0f, 0.1f);
    }
    private void Update() {
        float playerDistance = Vector2.Distance(rb.position, transformPlayer.position);
        //Debug.Log("Distância do jogador: " + playerDistance);
        if(playerDistance <= 15f && playerInGrid()) {    //Se a dsistância do guarda para o jogador for de menos de 20 e o jogador estiver na área do grid
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
                        if (!isChasing && !flagMovement) {   //Se não está perseguido o jogador, vou atualizar o ponto de patrulha
                            flagMovement = true;
                            if (patrolPointId == 0)
                                going = true;
                            else if (patrolPointId == patrolPoints.Count - 1)
                                going = false;
                            if (going)
                                patrolPointId++;
                            else
                                patrolPointId--;

                            updatePatrolPoint();   //Atualizando o ponto para o qual o inimigo seguirá
                        }
                    }
                    else {    //Se não chegamos ao fim do caminho, vamos mover o guarda para seu alvo
                        flagMovement = false;
                        movementDirection = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
                        movementForce = movementDirection * speed * Time.deltaTime;
                        //Fazendo o guarda se mover através do RigidBody2D:
                        //rb.MovePosition(rb.position + movementForce);
                        //rb.velocity = movementForce;
                        rb.AddForce(movementForce);

                        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
                        if (distance < nextWaypointDistance)
                            currentWaypoint++;
                    }
                }
            }
            else {   //Se o guarda conseguiu chegar até o gato
                rb.velocity = Vector2.zero;
                StartCoroutine(catchPlayer());
            }
        }
        else
            rb.velocity = Vector2.zero;
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

    private void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.tag.Equals("Player") && !GameController.playerCaught) {
            Debug.Log("Tá tocando!!!");
            GameController.playerCaught = true;
        }
    }
}
