using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerControler : MonoBehaviour
{
    public UnityEngine.AI.NavMeshAgent _playerAgent;
    InputAction _mouseAction;
    InputAction _clickAction;
    Vector2 _mousePosition;

    void Awake()
    {
        _playerAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _clickAction = InputSystem.actions["Attack"];
        _mouseAction = InputSystem.actions["Look"];
    }
    
    void Update()
    {
        _mousePosition = _mouseAction.ReadValue<Vector2>();
        if(_clickAction.WasPressedThisFrame())
        {
            SetDestination();
        }
    }

    void SetDestination()
    {
        Ray ray = Camera.main.ScreenPointToRay(_mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            _playerAgent.SetDestination(hit.point);
        }
    }
}
