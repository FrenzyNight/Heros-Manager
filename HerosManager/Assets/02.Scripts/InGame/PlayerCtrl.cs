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
    private bool gamepadMoving;
    private float previousSpeed;
    private NavMeshPath gamepadPath;
    public bool IsGamepadMoving => gamepadMoving;
    private static readonly int MovingParameter = Animator.StringToHash("isMove");

    private void Awake()
    {
        gamepadPath = new NavMeshPath();
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
        ProcessGamepadMovement();
        if (Time.timeScale <= 0 || InGameMgr.Instance.state != State.Camp || !Input.GetMouseButtonDown(1))
            return;
        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
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
        ReleaseGamepadOwnership();
        moving = true;
        if (movePosMgr != null) movePosMgr.MovePosEff(hit.position);
        if (audioSource != null) audioSource.Play();
        if (anim != null) anim.SetBool(MovingParameter, true);
    }

    private void StopMovement()
    {
        moving = false;
        ReleaseGamepadOwnership();
        if (navAgent != null && navAgent.enabled && navAgent.isOnNavMesh)
        {
            navAgent.ResetPath();
            navAgent.velocity = Vector3.zero;
        }
        if (audioSource != null) audioSource.Stop();
        if (anim != null) anim.SetBool(MovingParameter, false);
        if (movePosMgr != null) movePosMgr.gameObject.SetActive(false);
    }

    // Right stick steers on the navigation surface; left stick remains available to camp menus.
    private void ProcessGamepadMovement()
    {
        if (!GamepadInput.Connected || !GamepadUiController.GameplayInputAllowed ||
            !InGameMgr.IsCreateInstance() || InGameMgr.Instance == null || InGameMgr.Instance.state != State.Camp ||
            GamepadInput.Aim == Vector2.zero || navAgent == null || !navAgent.enabled || !navAgent.isOnNavMesh)
        {
            if (gamepadMoving) StopMovement();
            return;
        }
        if (sceneCamera == null) sceneCamera = Camera.main;
        if (sceneCamera == null) { if (gamepadMoving) StopMovement(); return; }
        Vector3 forward = Vector3.ProjectOnPlane(sceneCamera.transform.forward, Vector3.up);
        if (forward.sqrMagnitude < 0.001f)
            forward = Vector3.ProjectOnPlane(sceneCamera.transform.up, Vector3.up);
        forward.Normalize();
        Vector3 right = Vector3.Cross(Vector3.up, forward);
        Vector2 stick = GamepadInput.Aim;
        Vector3 direction = right * stick.x + forward * stick.y;
        float speed = gamepadMoving ? previousSpeed : navAgent.speed;
        Vector3 origin = navAgent.nextPosition;
        Vector3 target = origin + direction.normalized * Mathf.Max(navAgent.stoppingDistance + 0.5f, speed * 0.25f);
        // Clamp to the first edge instead of routing around an obstacle the stick points into.
        if (navAgent.Raycast(target, out var edge)) target = edge.position;
        if ((target - origin).sqrMagnitude <= Mathf.Pow(navAgent.stoppingDistance + 0.05f, 2f) ||
            !navAgent.CalculatePath(target, gamepadPath) || gamepadPath.status != NavMeshPathStatus.PathComplete ||
            !navAgent.SetPath(gamepadPath))
        {
            if (gamepadMoving) StopMovement();
            return;
        }
        if (!gamepadMoving)
        {
            previousSpeed = navAgent.speed;
            gamepadMoving = true;
            if (movePosMgr != null) movePosMgr.gameObject.SetActive(false);
            if (audioSource != null && !audioSource.isPlaying) audioSource.Play();
            if (anim != null) anim.SetBool(MovingParameter, true);
        }
        navAgent.speed = previousSpeed * stick.magnitude;
        moving = true;
    }

    private void ReleaseGamepadOwnership()
    {
        if (gamepadMoving && navAgent != null) navAgent.speed = previousSpeed;
        gamepadMoving = false;
    }

    public void ProcessGamepadMovementForTests() => ProcessGamepadMovement();

    private void LateUpdate()
    {
        // A camp interaction can change state after Update in the same frame.
        if (gamepadMoving && (!GamepadUiController.GameplayInputAllowed ||
            !InGameMgr.IsCreateInstance() || InGameMgr.Instance == null || InGameMgr.Instance.state != State.Camp))
            StopMovement();
    }

    private void OnApplicationFocus(bool focused) { if (!focused && gamepadMoving) StopMovement(); }
    private void OnApplicationPause(bool paused) { if (paused && gamepadMoving) StopMovement(); }
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
