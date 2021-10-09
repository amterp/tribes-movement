using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct LodInfo {
    public int LevelOfUndetail;
    public float VisibleDistanceEnd;

    public LodInfo(int levelOfUndetail, float visibleDistanceEnd) {
        LevelOfUndetail = levelOfUndetail;
        VisibleDistanceEnd = visibleDistanceEnd;
    }
}
