using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterProperties))]
public class JetpackTank : MonoBehaviour {
    private CharacterProperties _characterProps;
    private float _currentFuel;
    private int _lastFrameUsedFuel;

    void Awake() {
        _characterProps = GetComponent<CharacterProperties>();
    }

    void LateUpdate() {
        RunGradualRefill();
    }

    public float TryUseFuelAndReturnEfficiency() {
        _lastFrameUsedFuel = Time.frameCount;

        float maxAmountToUse = _characterProps.JetpackFuelConsumptionPerSecond * Time.deltaTime;
        float amountUsed = Mathf.Min(maxAmountToUse, _currentFuel);
        AddFuel(-amountUsed);

        float efficiency = amountUsed / maxAmountToUse;
        return efficiency;
    }

    private void RunGradualRefill() {
        if (_currentFuel >= _characterProps.JetpackMaxFuel) return;
        if (Time.frameCount == _lastFrameUsedFuel) return;
        float fuelRefillAmount = _characterProps.JetpackFuelRefillPerSecond * Time.deltaTime;
        AddFuel(fuelRefillAmount);
    }

    private void AddFuel(float fuelToAdd) {
        float newFuel = Mathf.Min(_characterProps.JetpackMaxFuel, _currentFuel + fuelToAdd);
        float actualFuelAdded = newFuel - _currentFuel;
        _currentFuel = newFuel;
        GameManager.Events.PlayerFuelChangedEvent.SafeInvoke(_currentFuel / _characterProps.JetpackMaxFuel);
    }
}
