using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct LodInfo {
    public int LevelOfUndetail;
    public float VisibleDistanceEnd;
    public bool UseForCollision;
    [HideInInspector] public int? Index;
}