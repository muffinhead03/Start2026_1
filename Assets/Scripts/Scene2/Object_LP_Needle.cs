using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Object_LP_Needle : MonoBehaviour, IMouseInteractable
{
    MeshRenderer mesh;
    Collider col;

    Material mat_original;

    [SerializeField] private Material mat_trans;

    [SerializeField] private Transform turntable_plane;

    [SerializeField] private Object_LP_track track;

    [SerializeField] private APwd pwd_LP;

    private bool isDragging;

    private float max_angle = -150f;
    private float min_angle = -200f;
    private float[] angle_range = { -192f, -180f, -171f };
    private int track_id;

    private Vector3 initialDirection;
    private Quaternion initialRotation;

    private Quaternion originalRotation;

    private void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();

        isDragging = false;

        initialDirection = new Vector3(0f, 0f, 1f);
        initialRotation = Quaternion.identity;

        originalRotation = transform.rotation;
        SetActive(false);
    }

    private void Update()
    {
        if (isDragging)
        {
            MoveNeedle();
        }
    }

    public void SetActive(bool active)
    {
        col.enabled = active;

        if(!active) transform.rotation = originalRotation;
    }

    public void OnMouseClick() { isDragging = true; }

    public void OnMouseReleased() 
    { 
        isDragging = false;

        track.HideTrack();

        if (track_id >= 0) pwd_LP.PressButton(track_id.ToString());
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
        if (turntable_plane == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        Plane plane = new Plane(turntable_plane.up,turntable_plane.position);

        if (!plane.Raycast(ray, out float distance)) return;

        Vector3 hitPoint = ray.GetPoint(distance);

        Vector3 pivotPosition = transform.position;

        Vector3 direction = hitPoint - pivotPosition;

        // 마우스의 방향
        direction = Vector3.ProjectOnPlane(direction, turntable_plane.up);

        if (direction.sqrMagnitude < 0.001f) return;

        // 현재 바늘의 방향과 마우스 방향 사이의 각도
        float angle = Vector3.SignedAngle(initialDirection, direction, turntable_plane.up) - 30.0f;

        if (angle < min_angle || (5 < angle && angle <= 150f)) angle = min_angle;
        else if (angle > max_angle) angle = max_angle;

        transform.rotation = Quaternion.AngleAxis(angle, turntable_plane.up) * initialRotation;

        FindTrackID(angle);
    }

    void FindTrackID(float angle)
    {
        int i;

        for(i = 0; i < angle_range.Length; i++)
        {
            if (angle <= angle_range[i]) break;
        }

        track_id = 2 - i;

        track.ShowTrack(track_id);
    }
}
