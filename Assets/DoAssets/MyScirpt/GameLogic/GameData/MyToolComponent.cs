using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyToolComponent :MonoBehaviour {
    public float strength; // [0..1] or 0% to 100%

    void Start()
    {
        strength = 1; // full strength
    }

    /**
	 * 每次使用工具都会给工具带来一些损伤
	 */
    public void use(float damage)
    {
        strength -= damage;
    }

    public bool destroyed()
    {
        return strength <= 0f;
    }
}
