using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResettable
{
    public void resetGameObject();
    public bool isDestructible();
}
