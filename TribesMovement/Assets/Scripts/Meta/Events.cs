using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Events {
    public GameEvent<float> PlayerFuelChangedEvent = new GameEvent<float>();
    public GameEvent<bool> PlayerIsSkiingStateChangedEvent = new GameEvent<bool>();
}
