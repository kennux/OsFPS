using UnityEngine;
using System;
using System.Collections.Generic;

namespace OsFPS
{
    /// <summary>
    /// Contains unity engine helper functions.
    /// </summary>
    public static class UnityUtils
    {
        /// <summary>
        /// Gets the first component in parents.
        /// Returns default(T) if there was no component found.
        /// </summary>
        /// <returns>The first component in parents.</returns>
        /// <param name="transform">Transform.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T GetFirstComponentInParents<T>(Transform transform)
        {
            if (transform.GetComponent<T>() != null)
                return transform.GetComponent<T>();
            if (transform.parent != null)
                return GetFirstComponentInParents<T>(transform.parent);
            return default(T);
        }

        /// <summary>
        /// Calculates direction vector (unnormalized) from me to other.
        /// </summary>
        /// <returns>The to.</returns>
        /// <param name="me">Me.</param>
        /// <param name="other">Other.</param>
        public static Vector3 DirectionTo(this Vector3 me, Vector3 other)
        {
            return other - me;
        }

        public static Vector2 ToVector2XZ(this Vector3 vec3)
        {
            return new Vector2(vec3.x, vec3.z);
        }


        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }

        /// <summary>
        /// Gets the recursive bounds.
        /// This iterates through all gameobject childs, gets their renderers and composes one bounds box which includes all.
        /// </summary>
        /// <returns>The recursive bounds.</returns>
        /// <param name="go">Go.</param>
        public static Bounds GetRecursiveBounds(GameObject go)
        {
            // TODO: This whole method yields very unstable results
            // Most likely due to no real transformations used to determine points
            // Research why this sometimes yields completely incorrect results

            MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer>();

            Bounds bounds = new Bounds(go.transform.position, Vector3.zero);
            foreach (MeshRenderer renderer in renderers)
            {
                var p = renderer.transform.position;
                p = (go.transform.position - p);

                var c = renderer.GetComponent<Collider>();
                if (c != null)
                    bounds.Encapsulate(c.bounds);
                else
                    bounds.Encapsulate(renderer.bounds);
            }

            return bounds;
        }

        public static Bounds TransformBounds(this Transform _transform, Bounds _localBounds)
        {
            var center = _transform.TransformPoint(_localBounds.center);

            // transform the local extents' axes
            var extents = _localBounds.extents;
            var axisX = _transform.TransformVector(extents.x, 0, 0);
            var axisY = _transform.TransformVector(0, extents.y, 0);
            var axisZ = _transform.TransformVector(0, 0, extents.z);

            // sum their absolute value to get the world extents
            extents.x = Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x) + Mathf.Abs(axisZ.x);
            extents.y = Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y) + Mathf.Abs(axisZ.y);
            extents.z = Mathf.Abs(axisX.z) + Mathf.Abs(axisY.z) + Mathf.Abs(axisZ.z);

            return new Bounds { center = center, extents = extents };
        }

        /// <summary>
        /// Sets the layer of the given gameobject recursively (to all its children aswell).
        /// </summary>
        /// <param name="go">Go.</param>
        /// <param name="layerNumber">Layer number.</param>
        public static void SetLayerRecursively(GameObject go, int layerNumber)
        {
            foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
            {
                trans.gameObject.layer = layerNumber;
            }
        }

        public static bool HasParameter(this Animator animator, string paramName)
        {
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == paramName)
                    return true;
            }
            return false;
        }
    }
}