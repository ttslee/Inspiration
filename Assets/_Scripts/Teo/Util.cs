using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Engarde_Teo
{

    /// <summary>
    /// Provides general quality of life helper functions.
    /// </summary>
    public static class Util
    {

        /// <summary>
        /// Calculate integer direction from raw value.
        /// </summary>
        public static int CalculateDirection(float raw, float deadzone = 0.01f)
        {
            if (raw < -deadzone)
            {
                return -1;
            }
            else if (raw > deadzone)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Destroy all child gameobjects.
        /// <para><i>Anakin no</i></para>
        /// </summary>
        public static void DestroyChildGameObjects(Transform parent, bool immediate = false)
        {
            if (parent == null) throw new ArgumentNullException("Failed to destroy child game objects, parent transform was null.");
            GameObject[] children = new GameObject[parent.childCount];
            for (int i = 0; i < parent.childCount; i++)
            {
                children[i] = parent.GetChild(i).gameObject;
            }
            foreach (var item in children)
            {
                if (immediate)
                {
                    UnityEngine.Object.DestroyImmediate(item);
                }
                else
                {
                    UnityEngine.Object.Destroy(item);
                }
            }
        }

        /// <summary>
        /// Get a Vector2 containing only the X and Y components of a Vector3.
        /// </summary>
        public static Vector2 GetXY(this Vector3 vector3) => new Vector2(vector3.x, vector3.y);

        /// <summary>
        /// Loop an int between [min, max)
        /// </summary>
        public static int Loop(int value, int min, int max)
        {
            if (min == max) throw new ArgumentException("Min and max cannot be equal");
            while (value < min) value += max - min;
            while (value >= max) value -= max - min;
            return value;
        }

        /// <summary>
        /// Loop a float between [min, max]
        /// </summary>
        public static float Loop(float value, float min, float max)
        {
            if (min == max) throw new ArgumentException("Min and max cannot be equal");
            while (value < min) value += max - min;
            while (value >= max) value -= max - min;
            return value;
        }

        /// <summary>
        /// Loop a Vector2 between [min, max]
        /// </summary>
        public static Vector2 Loop(Vector2 value, Vector2 min, Vector2 max)
        {
            return new Vector2(Loop(value.x, min.x, max.x), Loop(value.y, min.y, max.y));
        }

        /// <summary>
        /// Get a color from a hex string in the format #RRGGBB
        /// </summary>
        public static Color FromHex(string hex)
        {
            hex = hex.TrimStart('#');
            return new Color(
                int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber) / 255f,
                int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber) / 255f,
                int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber) / 255f);
        }

        /// <summary>
        /// Set the alpha value of a color.
        /// </summary>
        public static Color SetAlpha(Color color, float amount)
        {
            color.a = amount;
            return color;
        }

        /// <summary>
        /// Set the alpha value of a color.
        /// </summary>
        public static void SetAlpha(ref Color color, float amount)
        {
            color.a = amount;
        }

    }

}