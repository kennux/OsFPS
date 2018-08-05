using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OsFPS
{
    /// <summary>
    /// This class implements player core gameplay mechanics as entity component.
    /// Those mechanics are (as of now):
    /// - Uber suit (a suit that has several available modes <see cref="UberSuitMode"/> it can be in).
    /// </summary>
    public class PlayerMechanics : GenericEntityComponent<FirstPersonEntity>
    {
    }
}