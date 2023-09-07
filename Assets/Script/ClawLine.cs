using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawLine : MonoBehaviour
{
    public Transform offset;
    public Transform claw;

    LineRenderer line;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    private void LateUpdate()
    {
        line.SetPosition(0, offset.position);
        line.SetPosition(1, claw.position);
    }
}
