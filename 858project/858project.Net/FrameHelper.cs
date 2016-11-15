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
        /// Serializes the specified object to a Frame.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize to.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="commandAddress">Frame command address</param>
        /// <returns>Frame | null</returns>
        public static Frame Serialize<T>(T obj, UInt16 commandAddress)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            return InternalSerialize<T>(obj, commandAddress);
        }
        /// <summary>
        /// Deserializes the Frame to a .NET object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="frame">The Frame to deserialize.</param>
        /// <returns>The deserialized object from the Frame.</returns>
        public static T Deserialize<T>(Frame frame)
        {
            if (frame == null)
                throw new ArgumentNullException("frame");

            return FrameHelper.InternalDeserialize<T>(frame);
        }
        #endregion

        #region - Private Static Methods -
        /// <summary>
        /// Serializes the specified Object to Frame
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize to.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="commandAddress">Frame command address</param>
        /// <returns>Frame | null</returns>
        private static Frame InternalSerialize<T>(T obj, UInt16 commandAddress)
        {
            //get the object reglection
            ReflectionType reflection = ReflectionHelper.GetType(typeof(T));
            if (reflection != null)
            {
                return FrameHelper.InternalSerialize<T>(obj, commandAddress, reflection);
            }
            return null;
        }
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
        /// Serializes the specified Object to Frame
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize to.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="commandAddress">Frame command address</param>
        /// <param name="reflection">Reflection information for the Object to deserialize</param>
        /// <returns>Frame | null</returns>
        private static Frame InternalSerialize<T>(T obj, UInt16 commandAddress, ReflectionType reflection)
        {
            //intiailize object
            Frame result = new Frame(commandAddress);

            //set property
            foreach (ReflectionProperty item in reflection.PropertyCollection.Values)
            {
                //check property type
                if (item.Property.CanRead)
                {
                    //get current attribute
                    FrameTagAttribute attribute = item.GetCustomAttribute<FrameTagAttribute>();
                    if (attribute != null)
                    {
                        Object value = null;
                        try
                        {
                            value = item.Property.GetValue(obj, null);
                            if (value != null)
                            {
                                IFrameItem frameItem = Frame.CreateFrameItem(attribute.Type, attribute.Address, value);
                                if (frameItem != null)
                                {
                                    result.AddItem(frameItem);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(String.Format("Error type: {0} -> {1} [{2}]", attribute.Type, item.Property.Name, value), ex);
                        }
                    }
                }
            }

            //return result
            return result;
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
