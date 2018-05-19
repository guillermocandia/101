using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerController : MonoBehaviour {

    public Camera cam;
    public NavMeshAgent agent;
    public ThirdPersonCharacter character;
    public Animator anim;

    public GameObject startPoint;
    public GameObject endPoint;
    public GameObject marker;

    private Queue<Vector3> queue = new Queue<Vector3>();
    private bool reached = false;
    private bool moving = false;
    private bool dead = false;
    private bool win = false;

    // aux
    private GameObject newMarker;


    void Start()
    {
        transform.position = startPoint.transform.position;
        agent.updateRotation = false;    
    }


    void Update () {
		if (Input.GetMouseButtonDown(0) && !moving)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                queue.Enqueue(hit.point); // NO tengo explicación para esto(pero funciona)
                queue.Enqueue(hit.point); // el Dequeue esta sacando 2 registros de la cola!!!
                newMarker = Instantiate(marker);
                newMarker.transform.position = hit.point;
            }
        }

        if (win)
        {
            return;
        }

        if (dead)
        {
            return;
        }

        if (moving)
        {
            if (Vector3.Distance(transform.position, endPoint.transform.position) < 1)
            {
                character.Move(Vector3.zero, false, false);
                anim.SetBool("Win", true);
                win = true;
            }


            if (queue.Count > 0 && reached)
            {
                agent.SetDestination(queue.Dequeue());
                reached = false;
            }

            if (agent.remainingDistance > agent.stoppingDistance && !reached)
            {
                character.Move(agent.desiredVelocity, false, false);
            }
            else
            {
                character.Move(Vector3.zero, false, false);
                reached = true;
            }
        }

        
        
	}

    void OnTriggerEnter(Collider other)
    {
        character.Move(Vector3.zero, false, false);
        agent.isStopped = true;
        anim.SetBool("DeathTrigger", true);
        
        dead = true;
    }

    public void StartMoving ()
    {
        moving = true;
    }

    public void Reset()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
}
