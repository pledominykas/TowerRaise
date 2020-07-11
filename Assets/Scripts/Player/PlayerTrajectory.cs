using System.Collections.Generic;
using UnityEngine;

public class PlayerTrajectory : MonoBehaviour {

    private int LinePositionCount = 30;
    private float LinePositionDistance = 0.05f;
    [SerializeField] LayerMask GroundMask;
    private Vector3 force;
    private LineRenderer lr;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = LinePositionCount;
    }

    public void DrawTrajectory(Vector3 _force)
    {
        force = _force;
        for (int i = 0; i < LinePositionCount; i++)
        {
            float t = i * LinePositionDistance;
            Vector3 pos = Physics.gravity * t * t * 0.5f + force * t;
            lr.SetPosition(i, transform.TransformPoint(pos));
        }
    }

    public void DisableTrajcetory() { lr.enabled = false; }
    public void EnableTrajcetory() { lr.enabled = true; }
}
