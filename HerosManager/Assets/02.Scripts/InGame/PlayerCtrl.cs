using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerCtrl : MonoBehaviour
{
    public MovePosMgr movePosMgr;
    private AudioSource audioSource;
    private NavMeshAgent navAgent;
    private Animator anim;
    private Camera sceneCamera;
    private bool moving;
    private static readonly int MovingParameter = Animator.StringToHash("isMove");

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        sceneCamera = Camera.main;
    }

    private void Start()
    {
        if (navAgent == null) { enabled = false; return; }
        navAgent.speed = 7f;
        navAgent.angularSpeed = 720f;
        navAgent.acceleration = 20f;
    }

    private void Update()
    {
        if (moving && !navAgent.pathPending &&
            (!navAgent.isOnNavMesh || navAgent.pathStatus != NavMeshPathStatus.PathComplete ||
             navAgent.remainingDistance <= Mathf.Max(navAgent.stoppingDistance, 0.1f)))
            StopMovement();
        if (Time.timeScale <= 0 || InGameMgr.Instance.state != State.Camp || !Input.GetMouseButtonDown(1))
            return;
        if (sceneCamera == null) sceneCamera = Camera.main;
        if (sceneCamera == null || !navAgent.isOnNavMesh) return;
        var hits = Physics.RaycastAll(sceneCamera.ScreenPointToRay(Input.mousePosition));
        float nearest = float.PositiveInfinity;
        Vector3 destination = default;
        foreach (var hit in hits)
            if (hit.transform.CompareTag("Land") && hit.distance < nearest)
            {
                nearest = hit.distance;
                destination = hit.point;
            }
        if (!float.IsPositiveInfinity(nearest)) MoveTo(destination);
    }

    private void MoveTo(Vector3 position)
    {
        if (!NavMesh.SamplePosition(position, out var hit, 2f, navAgent.areaMask) ||
            !navAgent.SetDestination(hit.position)) return;
        moving = true;
        if (movePosMgr != null) movePosMgr.MovePosEff(hit.position);
        if (audioSource != null) audioSource.Play();
        if (anim != null) anim.SetBool(MovingParameter, true);
    }

    private void StopMovement()
    {
        moving = false;
        if (navAgent != null && navAgent.enabled && navAgent.isOnNavMesh) navAgent.ResetPath();
        if (audioSource != null) audioSource.Stop();
        if (anim != null) anim.SetBool(MovingParameter, false);
        if (movePosMgr != null) movePosMgr.gameObject.SetActive(false);
    }

    private void OnDisable() => StopMovement();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("CampObject")) return;
        var camp = other.GetComponentInParent<CampObjectMgr>();
        if (camp != null) camp.OpenUI();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("CampObject")) return;
        var camp = other.GetComponentInParent<CampObjectMgr>();
        if (camp != null) camp.CloseUI();
    }
}
