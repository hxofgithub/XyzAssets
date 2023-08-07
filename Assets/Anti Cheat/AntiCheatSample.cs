using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiCheatSample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {


        uint orgKey = 1000;
        XyzAniCheat.Runtime.ObscuredTypes.ObscuredUInt key = 1000;
        orgKey++;
        key++;

        orgKey += 100;
        key += 100;

        byte a = 100;
        key += a;
        orgKey += a;

        Debug.Log("-org------ " + orgKey);
        Debug.Log("-org------ " + (true));
        Debug.Log("-org------ " + (false));
        Debug.Log("-org------ " + (orgKey % orgKey));
        Debug.Log("-org------ " + (orgKey / orgKey));

        Debug.Log("-org------ " + (100 * orgKey > orgKey));
        Debug.Log("-org------ " + (100 * orgKey % orgKey));
        Debug.Log("-org------ " + (100 * orgKey / orgKey));



        Debug.Log("-key------ " + key);
        Debug.Log("-key------ " + (key == orgKey));
        Debug.Log("-key------ " + (key > orgKey));
        Debug.Log("-key------ " + (key % orgKey));
        Debug.Log("-key------ " + (key / orgKey));

        Debug.Log("-key------ " + (100 * key > orgKey));
        Debug.Log("-key------ " + (100 * key % orgKey));
        Debug.Log("-key------ " + (100 * key / orgKey));


    }

    // Update is called once per frame
    void Update()
    {

    }
}
