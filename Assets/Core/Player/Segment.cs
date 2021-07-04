using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
    public SegmentType segmentType = SegmentType.Undefined;
    public enum SegmentType
    {
        Undefined,
        Head,
        Body,
        Tail
    }

    public Vector3 previousPosition;
    public Quaternion previousRotation;

    public bool isSpawned;
}
