using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class EnableAndDisable : MonoBehaviour
{

    public VuforiaBehaviour vuforiaBehaviour;

    public void EnableAndDisableARCamera(bool enabled)
    {
        vuforiaBehaviour.enabled = enabled;
    }

}
