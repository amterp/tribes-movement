using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterProperties : MonoBehaviour {
    public float AccelerationAmount = 100;
    public float MaxSpeed = 32;
    public float MidAirAccelerationFactor = 0.5f;
    public float JetpackAcceleration = 40f;
    public float JetpackMaxFuel = 100f;
    public float JetpackFuelConsumptionPerSecond = 10f;
    public float JetpackFuelRefillPerSecond = 15f;
    [Range(0, 5f)]
    public float WalkFrictionStatic = 2f;
    [Range(0, 5f)]
    public float WalkFrictionDynamic = 1.5f;
    [Range(0, 2f)]
    public float SkiFriction = 0.15f;
}
