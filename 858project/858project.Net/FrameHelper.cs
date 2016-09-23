using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project858.Reflection;

namespace Project858.Net
{
    /// <summary>
    /// Frame helper for serializing or deserializing frame
    /// </summary>
    public static class FrameHelper
    {
        #region - Public Static Methods -
        /// <summary>
        /// Deserializes the Frame to a .NET object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="frame">The Frame to deserialize.</param>
        /// <returns>The deserialized object from the Frame.</returns>
        public static T Deserialize<T>(Frame frame)
        {
            return FrameHelper.InternalDeserialize<T>(frame);
        }
        #endregion

        #region - Private Static Methods -
        /// <summary>
        /// Deserializes the Frame to a .NET object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="frame">The Frame to deserialize.</param>
        /// <returns>The deserialized object from the Frame.</returns>
        private static T InternalDeserialize<T>(Frame frame)
        {
            //get the object reglection
            ReflectionType reflection = ReflectionHelper.GetType(typeof(T));
            if (reflection != null)
            {
                return FrameHelper.InternalDeserialize<T>(frame, reflection);
            }
            return default(T);
        }
        /// <summary>
        /// Deserializes the Frame to a .NET object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="frame">The Frame to deserialize.</param>
        /// <param name="reflection">Reflection information for the Object to deserialize</param>
        /// <returns>The deserialized object from the Frame.</returns>
        private static T InternalDeserialize<T>(Frame frame, ReflectionType reflection)
        {
            //intiailize object
            T result = (T)Activator.CreateInstance(typeof(T));

            //set property
            foreach (ReflectionProperty item in reflection.PropertyCollection.Values)
            {
                //check property type
                if (item.Property.CanWrite)
                {
                    //get current attribute
                    FrameTagAttribute attribute = item.GetCustomAttribute<FrameTagAttribute>();
                    if (attribute != null)
                    {
                        Object value = null;
                        try
                        {
                            value = frame.GetValue<Object>(attribute.Address);
                            item.Property.SetValue(result, value, null);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(String.Format("Error type: {0} -> {1}", (value == null ? "NULL" : value.ToString()), item.Property.Name), ex);
                        }
                    }
                }
            }

            //return result
            return result;
        }
        #endregion
    }
}
