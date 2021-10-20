using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterMover {
    void UpdateMover();
    void LateUpdateMover();
    void Ski();
    void Accelerate(Vector3 vector);
}
