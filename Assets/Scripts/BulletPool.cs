using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : Pool<Bullet>
{
    private void Awake()
    {
        Init();
    }
}
