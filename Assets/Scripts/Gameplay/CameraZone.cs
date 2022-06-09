using DialogueQuests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraZoneType
{
    ResumeNormal=0,
    LockCamera=10,
    CameraOffest=20
}

[RequireComponent(typeof(Region))]
public class CameraZone : MonoBehaviour
{
    public CameraZoneType type;
    public GameObject lock_target;
    public Vector3 offset;

    private Region region;

    void Start()
    {
        region = GetComponent<Region>();
        region.onEnterRegion += OnEnterRegion;
    }

    void Update()
    {
        
    }

    void OnEnterRegion(Actor actor)
    {
        if (type == CameraZoneType.ResumeNormal)
        {
            TheCamera.Get().UnlockCamera();
            TheCamera.Get().SetCameraOffset(Vector3.zero);
        }

        if (type == CameraZoneType.LockCamera)
        {
            TheCamera.Get().LockCamera(lock_target);
        }

        if (type == CameraZoneType.CameraOffest)
        {
            TheCamera.Get().UnlockCamera();
            TheCamera.Get().SetCameraOffset(offset);
        }
    }
}
