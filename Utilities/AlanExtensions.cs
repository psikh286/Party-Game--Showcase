using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Party.Utilities
{
    public static class AlanExtensions
    {
        #region LerpTo
        // ReSharper disable Unity.PerformanceAnalysis
        public static IEnumerator LerpTo(Action<float> result, float from, float to, float delta, bool unscaledTime = false, AnimationCurve curve = null)
        {
            var current = 0f;
        
            curve ??= AnimationCurve.Linear(0, 0, 1, 1);

            while (current < 1)
            {
                current = Mathf.MoveTowards(current, 1, unscaledTime ? delta * Time.unscaledDeltaTime : delta * Time.deltaTime);
                var newValue = Mathf.Lerp(from, to, curve.Evaluate(current));
                result(newValue);

                yield return null;
            }
        }
        public static IEnumerator LerpTo(Action<Vector3> result, Vector3 from, Vector3 to, float delta, bool unscaledTime = false, AnimationCurve curve = null)
        {
            var current = 0f;

            curve ??= AnimationCurve.Linear(0, 0, 1, 1);

            while (current < 1)
            {
                current = Mathf.MoveTowards(current, 1, unscaledTime ? delta * Time.unscaledDeltaTime : delta * Time.deltaTime);
                var newValue = Vector3.Lerp(from, to, curve.Evaluate(current));
                result(newValue);
            
                yield return null;
            }
        }
        public static IEnumerator LerpTo(Action<Quaternion> result, Quaternion from, Quaternion to, float delta, bool unscaledTime = false, AnimationCurve curve = null)
        {
            var current = 0f;

            curve ??= AnimationCurve.Linear(0, 0, 1, 1);

            while (current < 1)
            {
                current = Mathf.MoveTowards(current, 1, unscaledTime ? delta * Time.unscaledDeltaTime : delta * Time.deltaTime);
                var newValue = Quaternion.Lerp(from, to, curve.Evaluate(current));
                result(newValue);
            
                yield return null;
            }
        }
        public static IEnumerator LerpTo(Action<Quaternion> result, Vector3 from, Vector3 to, float delta, bool unscaledTime = false, AnimationCurve curve = null)
        {
            var current = 0f;
            var qFrom = Quaternion.Euler(from);
            var qTo = Quaternion.Euler(to);

            curve ??= AnimationCurve.Linear(0, 0, 1, 1);

            while (current < 1)
            {
                current = Mathf.MoveTowards(current, 1, unscaledTime ? delta * Time.unscaledDeltaTime : delta * Time.deltaTime);
                var newValue = Quaternion.Lerp(qFrom, qTo, curve.Evaluate(current));
                result(newValue);
            
                yield return null;
            }
        }
        #endregion

        #region Shuffle
        public static void Shuffle<T>(this IList<T> list)  
        {
            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = Utilities.Utility.RandomInt(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }  
        }
        #endregion

        #region Find And Get
    
        public static T FindOfType<T>() where T : class
        {
            return Object.FindObjectsOfType<MonoBehaviour>().OfType<T>().Select(monoBehaviour => monoBehaviour as T).FirstOrDefault();
        }

        public static List<Transform> GetAllChildren(this Transform parent)
        {
            List<Transform> result = new();
            for (int i = 0; i < parent.childCount; i++)
            {
                result.Add(parent.GetChild(i));
            }
            return result;
        }

        public static T GetComponentInSibling<T>(this Transform self)
        {
            return self.parent.GetComponentInChildren<T>();
        }
    
        public static T[] GetComponentsInSibling<T>(this Transform self)
        {
            return self.parent.GetComponentsInChildren<T>();
        }

        public static int GetKeyIndex<T, T1>(this Dictionary<T, T1> dic, T key)
        {
            return dic.Keys.ToList().IndexOf(key);
        }
    
        #endregion

        #region Vectors

        public static Vector3 V22V3(this Vector2 v2)
        {
            return new Vector3(v2.x, 0, v2.y);
        }

        public static Vector2 V32V2(this Vector3 v3)
        {
            return new Vector2(v3.x , v3.z);
        }

        public static Vector2 ToVector2(this float radian)
        {
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }

        public static float Vector2ToRadian(this Vector2 v2)
        {
            v2.Normalize();

            return (Mathf.Acos(v2.x) + Mathf.Asin(v2.y)) / 2f;
        }

        #endregion

        public static T GetCopyOf<T>(this Component comp, T other) where T : Component
        {
            var type = comp.GetType();
            if (type != other.GetType()) return null; // type mis-match
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            var pinfos = type.GetProperties(flags);
            foreach (var pinfo in pinfos)
            {
                if (!pinfo.CanWrite) continue;
            
                try
                {
                    pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                }
                catch { } // In case of NotImplementedException being thrown.
            }
            var finfos = type.GetFields(flags);
            foreach (var finfo in finfos)
            {
                finfo.SetValue(comp, finfo.GetValue(other));
            }
            return comp as T;
        }
        public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
        {
            return go.AddComponent<T>().GetCopyOf(toAdd) as T;
        }
        
        public static Vector3 AlignToCamera(this Vector3 input, Transform camera)
        {
            return Quaternion.Euler(0f, camera.eulerAngles.y, 0f) * input;
        }
    }
}