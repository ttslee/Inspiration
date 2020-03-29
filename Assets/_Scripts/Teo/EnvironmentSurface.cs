using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engarde_Teo
{

    public enum SurfaceTypes
    {
        Generic
    }

    public class EnvironmentSurface : MonoBehaviour
    {

        // TODO: Optimize away .GetComponent call every FixedUpdate by using a Dictionary<GameObject hash, SurfaceType>

        public SurfaceTypes surfaceType = SurfaceTypes.Generic;

    }

}