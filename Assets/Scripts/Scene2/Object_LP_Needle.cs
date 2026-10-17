using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Object_LP_Needle : MonoBehaviour, IMouseInteractable
{
    MeshRenderer mesh;

    Material mat_original;

    [SerializeField] private Object_Turntable turntable;

    [SerializeField] private Material mat_trans;

    [SerializeField] private Transform turntable_plane;

    private bool isDragging;

    private void Start()
    {
        mesh = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (isDragging)
        {
            MoveNeedle();
        }
    }

    public void OnMouseClick() { isDragging = true; }

    public void OnMouseReleased() 
    { 
        isDragging = false;
    }

    public void OnMouseEnter()
    {
        mat_original = mesh.material;

        mesh.material = mat_trans;
    }

    public void OnMouseExit()
    {
        mesh.material = mat_original;
    }

    private void MoveNeedle()
    {
        if (turntable_plane == null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        Plane plane = new Plane(turntable_plane.up,turntable_plane.position);

        if (!plane.Raycast(ray, out float distance)) return;

        Vector3 hitPoint = ray.GetPoint(distance);

        Vector3 pivotPosition = transform.position;

        Vector3 direction = hitPoint - pivotPosition;

        direction = Vector3.ProjectOnPlane(direction, turntable_plane.up);

        if (direction.sqrMagnitude < 0.0001f) return;

        // 바늘의 회전축 기준 방향 계산
        Vector3 initialDirection = Vector3.ProjectOnPlane(transform.forward, turntable_plane.up);

        // 초기 방향과 마우스 방향 사이의 각도 계산
        float angle = Vector3.SignedAngle(initialDirection, direction, turntable_plane.up) - 23.0f;

        // 회전축을 기준으로 회전
        transform.rotation =Quaternion.AngleAxis(angle,turntable_plane.up) * Quaternion.LookRotation(initialDirection,turntable_plane.up);

    }
}
