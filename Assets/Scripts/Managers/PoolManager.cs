using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoSingleton<PoolManager>
{
    public TreeFactory treeFactory;
    public GrassFactory grassFactory;
    public BuildingFactory buildingFactory;
}
