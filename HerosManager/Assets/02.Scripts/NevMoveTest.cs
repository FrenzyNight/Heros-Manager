using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class NevMoveTest : MonoBehaviour
{
    private NavMeshAgent _agent;

    public GameObject target;

    public GameObject[] char3DAni;

    public GameObject charProduct;
   // [HideInInspector] public Sprite3DAnimator[] spriteControllers;
    // Start is called before the first frame update
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.SetDestination(target.transform.position);
    }

    void Update()
    {
        if (_agent.velocity.sqrMagnitude >= 0.2f * 0.2f && _agent.remainingDistance <= 0.5f)
        {
            //Debug.Log("도착");
            //this.transform.position = _agent.destination;

            _agent.ResetPath();
            foreach (var obj in char3DAni)
            {
                obj.GetComponent<Sprite3DAnimator>().SetIdle();
            }
            
            charProduct.GetComponent<CharProduct>().FadeIn();
        }
    }

}
