using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

public class OfficerAI : MonoBehaviour {
    private Transform target;   //target representa a posi��o para a qual o guarda est� indo
    public Transform transformPlayer;
    public List<Transform> patrolPoints;    //Estas ser�o as posi��es que o guarda passa ao patrulhar o cen�rio
    private float speedChasing = 400f, speedPatrolling = 500f, speed;

    //Estas vari�veis s�o usadas para movimentar o guarda pelo grid:
    private float nextWaypointDistance = 3f;
    private int currentWaypoint=0, patrolPointId;
    private bool isChasing = false, going, flagMovement = false;

    private Path path;
    private Seeker seekerScript;
    private Rigidbody2D rb;


    private void Start() {
        seekerScript = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        patrolPointId = 0;
        updatePatrolPoint();

        //Inicializando um caminho para o guarda seguir e fazendo este caminho ser calculado continuamente por meio do InvokeRepeating:
        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }
    private void Update() {
        float playerDistance = Vector2.Distance(rb.position, transformPlayer.position);
        Debug.Log("Dist�ncia do jogador: " + playerDistance);
        if(playerDistance <= 20f) {
            if (!isChasing) {
                target = transformPlayer;
            }
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
        if (path != null) {   //Se existir um caminho calculado
            if (currentWaypoint >= path.vectorPath.Count) {   //Se chegamos ao fim do caminho
                if (!isChasing && !flagMovement) {   //Se n�o est� perseguido o jogador, vou atualizar o ponto de patrulha
                    flagMovement = true;
                    if (patrolPointId == 0)
                        going = true;
                    else if (patrolPointId == patrolPoints.Count - 1)
                        going = false;
                    if(going)
                        patrolPointId++;
                    else
                        patrolPointId--;

                    updatePatrolPoint();   //Atualizando o ponto para o qual o inimigo seguir�
                }
                else {
                    return;
                }
            }
            else {    //Se n�o chegamos ao fim do caminho, vamos mover o guarda para seu alvo
                flagMovement = false;
                Vector2 movementDirection = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
                Vector2 movementForce = movementDirection * speed * Time.deltaTime;
                rb.AddForce(movementForce);    //Fazendo o guarda se mover atrav�s do RigidBody2D

                float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
                if (distance < nextWaypointDistance)
                    currentWaypoint++;
            }
        }
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
}
