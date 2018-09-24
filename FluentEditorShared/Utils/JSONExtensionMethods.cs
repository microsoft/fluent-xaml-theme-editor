// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Numerics;
using Windows.Data.Json;
using Windows.UI;

namespace FluentEditorShared.Utils
{
    public static class JsonExtensionMethods
    {
        public static string GetOptionalString(this JsonObject data, string key)
        {
            if (data == null || string.IsNullOrEmpty(key))
            {
                return null;
            }
            if (data.ContainsKey(key))
            {
                return data[key].GetOptionalString();
            }
            else
            {
                return null;
            }
        }

        public static string GetOptionalString(this IJsonValue data)
        {
            if (data == null)
            {
                return null;
            }
            if (data.ValueType == JsonValueType.String)
            {
                return data.GetString();
            }
            return data.ToString();
        }

        public static int GetInt(this IJsonValue data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.ValueType == JsonValueType.String)
            {
                int retVal;
                if (!int.TryParse(data.GetString(), out retVal))
                {
                    throw new Exception("JsonObject is type string but cannot be parsed to int");
                }
                return retVal;
            }
            else if (data.ValueType == JsonValueType.Number)
            {
                return (int)Math.Round(data.GetNumber());
            }
            else
            {
                throw new Exception(string.Format("JsonObject is type {0}", data.ValueType.ToString()));
            }
        }

        public static bool TryGetInt(this IJsonValue data, out int retVal)
        {
            if (data == null)
            {
                retVal = default(int);
                return false;
            }
            if (data.ValueType == JsonValueType.String)
            {
                if (!int.TryParse(data.GetString(), out retVal))
                {
                    retVal = default(int);
                    return false;
                }
                return true;
            }
            else if (data.ValueType == JsonValueType.Number)
            {
                retVal = (int)Math.Round(data.GetNumber());
                return true;
            }
            else
            {
                retVal = default(int);
                return false;
            }
        }

        public static float GetFloat(this IJsonValue data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.ValueType == JsonValueType.String)
            {
                float retVal;
                if (!float.TryParse(data.GetString(), out retVal))
                {
                    throw new Exception("JsonObject is type string but cannot be parsed to float");
                }
                return retVal;
            }
            else if (data.ValueType == JsonValueType.Number)
            {
                return (float)data.GetNumber();
            }
            else
            {
                throw new Exception(string.Format("JsonObject is type {0}", data.ValueType.ToString()));
            }
        }

        public static bool TryGetFloat(this IJsonValue data, out float retVal)
        {
            if (data == null)
            {
                retVal = default(float);
                return false;
            }
            if (data.ValueType == JsonValueType.String)
            {
                if (!float.TryParse(data.GetString(), out retVal))
                {
                    retVal = default(float);
                    return false;
                }
                return true;
            }
            else if (data.ValueType == JsonValueType.Number)
            {
                retVal = (float)data.GetNumber();
                return true;
            }
            else
            {
                retVal = default(float);
                return false;
            }
        }

        public static Vector2 GetVector2(this JsonObject data)
        {
            if (data == null)
            {
                return default(Vector2);
            }
            float x = 0f, y = 0f;

            IJsonValue xnode = null;
            if (data.ContainsKey("X"))
            {
                xnode = data["X"];
            }
            else if (data.ContainsKey("x"))
            {
                xnode = data["x"];
            }
            if (xnode != null)
            {
                if (!xnode.TryGetFloat(out x))
                {
                    x = 0f;
                }
            }

            IJsonValue ynode = null;
            if (data.ContainsKey("Y"))
            {
                ynode = data["Y"];
            }
            else if (data.ContainsKey("y"))
            {
                ynode = data["y"];
            }
            if (ynode != null)
            {
                if (!ynode.TryGetFloat(out y))
                {
                    y = 0f;
                }
            }

            return new Vector2(x, y);
        }

        public static Vector3 GetVector3(this JsonObject data)
        {
            if (data == null)
            {
                return default(Vector3);
            }
            float x = 0f, y = 0f, z = 0f;

            IJsonValue xnode = null;
            if (data.ContainsKey("X"))
            {
                xnode = data["X"];
            }
            else if (data.ContainsKey("x"))
            {
                xnode = data["x"];
            }
            if (xnode != null)
            {
                if (!xnode.TryGetFloat(out x))
                {
                    x = 0f;
                }
            }

            IJsonValue ynode = null;
            if (data.ContainsKey("Y"))
            {
                ynode = data["Y"];
            }
            else if (data.ContainsKey("y"))
            {
                ynode = data["y"];
            }
            if (ynode != null)
            {
                if (!ynode.TryGetFloat(out y))
                {
                    y = 0f;
                }
            }

            IJsonValue znode = null;
            if (data.ContainsKey("Z"))
            {
                znode = data["Z"];
            }
            else if (data.ContainsKey("z"))
            {
                znode = data["z"];
            }
            if (znode != null)
            {
                if (!znode.TryGetFloat(out z))
                {
                    z = 0f;
                }
            }

            return new Vector3(x, y, z);
        }

        public static Color GetColor(this IJsonValue data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.ValueType == JsonValueType.String)
            {
                Color retVal;
                if (!FluentEditorShared.Utils.ColorUtils.TryParseColorString(data.GetString(), out retVal))
                {
                    throw new Exception("JsonObject is type string but cannot be parsed to Color");
                }
                return retVal;
            }
            else
            {
                throw new Exception(string.Format("JsonObject is type {0}", data.ValueType.ToString()));
            }
        }

        public static T GetEnum<T>(this IJsonValue data) where T : struct
        {
            string dataString = data.GetString();
            if (Enum.TryParse<T>(dataString, out T retVal))
            {
                return retVal;
            }
            else
            {
                throw new Exception(string.Format("Unable to parse {0} into enum of type {1}", dataString, typeof(T).ToString()));
            }
        }
    }
}
