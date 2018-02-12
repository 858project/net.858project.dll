using System;
using System.Collections.Generic;
using Project858.Reflection;

namespace Project858.Net
{
    /// <summary>
    /// Frame helper for serializing or deserializing frame
    /// </summary>
    public static class FrameHelper
    {
        #region - Public Static Methods V2 -
        /// <summary>
        /// This function finds frame in array
        /// </summary>
        /// <param name="array">Input array data</param>
        /// <param name="action">Callback for parsing frame items</param>
        /// <returns>Frame | null</returns>
        public static FrameV2 FindFrameV2(List<Byte> array, Func<UInt16, UInt16, UInt32, FrameItemTypes> action)
        {
            //variables
            int count = array.Count;

            //find start byte
            for (int index = 0; index < count; index++)
            {
                //check start byte and length
                if (array[index] == 0x70 && (count - (index + 2)) >= 2)
                {
                    //get length from array
                    UInt16 length = (UInt16)(array[index + 2] << 8 | array[index + 1]);

                    //get command from array
                    UInt16 address = (UInt16)(array[index + 4] << 8 | array[index + 3]);

                    //get state
                    Byte state = array[index + 5];

                    //overime ci je dostatok dat na vytvorenie package
                    if (count >= (length - 1))
                    {
                        FrameV2 frame = FrameHelper.ConstructFrameV2(array, index + 6, length - 6, address, state, action);
                        if (frame != null)
                        {
                            //return package
                            return frame;
                        }
                    }
                    else
                    {
                        //remove first data
                        if (index > 0)
                        {
                            array.RemoveRange(0, index + 1);
                        }

                        //nedostatok dat
                        return null;
                    }
                }
            }

            //clear all data from buffer
            array.Clear();

            //any package
            return null;
        }
        /// <summary>
        /// This function constructs frame from data array
        /// </summary>
        /// <param name="array">Data array</param>
        /// <param name="index">Start frame index</param>
        /// <param name="length">Frame length</param>
        /// <param name="address">Command address from frame</param>
        /// <param name="state">State value</param>]
        /// <param name="action">Callback for parsing frame items</param>
        /// <returns>Frame | null</returns>
        private static FrameV2 ConstructFrameV2(List<Byte> array, int index, int length, UInt16 address, Byte state, Func<UInt16, UInt16, UInt32, FrameItemTypes> action)
        {
            //check data length available
            if ((array.Count - index) >= length)
            {
                //get checksum
                Byte checkSum = FrameHelper.GetFrameDataCheckSum(array, index - 5, length + 5);
                Byte currentCheckSum = array[index + length + 0];
                if (checkSum != currentCheckSum)
                {
                    return null;
                }

                //copy data block
                List<Byte> temp = array.GetRange(index, length);

                //initialize package
                FrameV2 frame = new FrameV2(address, state, temp, action);

                //remove data
                array.RemoveRange(0, length + index + 1);

                //return package
                return frame;
            }
            return null;
        }
        /// <summary>
        /// This function serializes object to frame group
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data object</param>
        /// <returns>Group collection</returns>
        public static List<FrameGroupItem> SerializeV2ToGroup<T>(List<T> data)
        {
            return FrameHelper.InternalSerializeV2ToGroup<T>(data);
        }
        /// <summary>
        /// This function serializes object to frame group
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data object</param>
        /// <returns>Group collection</returns>
        public static FrameGroupItem SerializeV2ToGroup<T>(T data)
        {
            return FrameHelper.InternalSerializeV2ToGroup<T>(data);
        }
        /// <summary>
        /// Serializes the specified object to a Frame.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize to.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="address">Frame command address</param>
        /// <returns>Frame | null</returns>
        public static FrameV2 SerializeV2<T>(T obj, UInt16 address)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            //create collection
            List<T> collection = new List<T>();
            collection.Add(obj);
 
            //serialize
            return SerializeV2<T>(collection, address);
        }
        /// <summary>
        /// Serializes the specified object to a Frame.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize to.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="commandAddress">Frame command address</param>
        /// <returns>Frame | null</returns>
        public static FrameV2 SerializeV2<T>(List<T> obj, UInt16 commandAddress)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            //intiailize object
            FrameV2 result = new FrameV2(commandAddress, 0x00);

            //serialize
            return SerializeV2<T>(obj, result);
        }
        /// <summary>
        /// Serializes the specified object to a Frame.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize to.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="frame">Frame</param>
        /// <returns>Frame | null</returns>
        public static FrameV2 SerializeV2<T>(List<T> obj, FrameV2 frame)
        {
            //intiailize object
            return InternalSerializeV2<T>(obj, frame);
        }
        /// <summary>
        /// Deserializes the Frame to a .NET object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="frame">The Frame to deserialize.</param>
        /// <returns>The deserialized object from the Frame.</returns>
        public static List<T> DeserializeV2<T>(FrameV2 frame)
        {
            if (frame == null)
                throw new ArgumentNullException("frame");

            return FrameHelper.InternalDeserializeV2<T>(frame);
        }
        /// <summary>
        /// Deserializes the Group to a .NET object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="group">The Group to deserialize.</param>
        /// <returns>The deserialized object from the Group.</returns>
        public static T DeserializeV2<T>(FrameGroupItem group)
        {
            if (group == null)
                throw new ArgumentNullException("group");

            return FrameHelper.InternalDeserializeV2<T>(group);
        }
        #endregion

        #region - Public Static Methods -
        /// <summary>
        /// This function calculate check sum from frame
        /// </summary>
        /// <param name="array">Data array</param>
        /// <param name="index">Start frame index</param>
        /// <param name="length">Frame length</param>
        /// <returns>Check sum</returns>
        public static Byte GetFrameDataCheckSum(List<Byte> array, int index, int length)
        {
            int sum = 0;
            for (int currentIndex = index; currentIndex < (length + index); currentIndex++)
            {
                sum += (int)array[currentIndex];
            }
            sum += 0xA5;
            sum = sum & 0xFF;
            return (byte)(256 - sum);
        }
        /// <summary>
        /// This function finds frame in array
        /// </summary>
        /// <param name="array">Input array data</param>
        /// <param name="action">Callback for parsing frame items</param>
        /// <returns>Frame | null</returns>
        public static Frame FindFrame(List<Byte> array, Func<UInt16, UInt32, FrameItemTypes> action)
        {
            //variables
            int count = array.Count;

            //find start byte
            for (int index = 0; index < count; index++)
            {
                //check start byte and length
                if (array[index] == 0x68 && (count - (index + 2)) >= 2)
                {
                    //get length from array
                    UInt16 length = (UInt16)(array[index + 2] << 8 | array[index + 1]);

                    //get command from array
                    UInt16 address = (UInt16)(array[index + 4] << 8 | array[index + 3]);

                    //overime ci je dostatok dat na vytvorenie package
                    if (count >= (length - 1))
                    {
                        Frame frame = FrameHelper.ConstructFrame(array, index + 5, length - 5, address, action);
                        if (frame != null)
                        {
                            //return package
                            return frame;
                        }
                    }
                    else
                    {
                        //remove first data
                        if (index > 0)
                        {
                            array.RemoveRange(0, index + 1);
                        }

                        //nedostatok dat
                        return null;
                    }
                }
            }

            //clear all data from buffer
            array.Clear();

            //any package
            return null;
        }
        /// <summary>
        /// This function constructs frame from data array
        /// </summary>
        /// <param name="array">Data array</param>
        /// <param name="index">Start frame index</param>
        /// <param name="length">Frame length</param>
        /// <param name="address">Command address from frame</param>
        /// <returns>Frame | null</returns>
        private static Frame ConstructFrame(List<Byte> array, int index, int length, UInt16 address, Func<UInt16, UInt32, FrameItemTypes> action)
        {
            //check data length available
            if ((array.Count - index) >= length)
            {
                //get checksum
                Byte checkSum = FrameHelper.GetFrameDataCheckSum(array, index - 4, length + 4);
                Byte currentCheckSum = array[index + length + 0];
                if (checkSum != currentCheckSum)
                {
                    return null;
                }

                //copy data block
                List<Byte> temp = array.GetRange(index, length);

                //initialize package
                Frame frame = new Frame(address, temp, action);

                //remove data
                array.RemoveRange(0, length + index + 1);

                //return package
                return frame;
            }
            return null;
        }
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

            //intiailize object
            Frame result = new Frame(commandAddress);

            //serialize
            return Serialize<T>(obj, result);
        }
        /// <summary>
        /// Serializes the specified object to a Frame.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize to.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="frame">Frame</param>
        /// <returns>Frame | null</returns>
        public static Frame Serialize<T>(T obj, Frame frame)
        {
            //intiailize object
            return InternalSerialize<T>(obj, frame);
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

        #region - Private Static Methods V2 -
        /// <summary>
        /// Serializes the specified Object to Frame
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize to.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="frame">Frame</param>
        /// <returns>Frame | null</returns>
        private static FrameV2 InternalSerializeV2<T>(List<T> obj, FrameV2 frame)
        {
            //get the object reglection
            ReflectionType reflection = ReflectionHelper.GetType(typeof(T));
            if (reflection != null)
            {
                return FrameHelper.InternalSerializeV2<T>(obj, frame, reflection);
            }
            return null;
        }
        /// <summary>
        /// This function serializes object to frame group
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data object</param>
        /// <returns>Group collection</returns>
        private static FrameGroupItem InternalSerializeV2ToGroup<T>(T data)
        {
            //get the object reglection
            ReflectionType reflection = ReflectionHelper.GetType(typeof(T));
            if (reflection != null)
            {
                return FrameHelper.InternalSerializeV2ToGroup<T>(data, reflection);
            }
            return null;
        }
        /// <summary>
        /// This function serializes object to frame group
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data object</param>
        /// <returns>Group collection</returns>
        private static List<FrameGroupItem> InternalSerializeV2ToGroup<T>(List<T> data)
        {
            //get the object reglection
            ReflectionType reflection = ReflectionHelper.GetType(typeof(T));
            if (reflection != null)
            {
                return FrameHelper.InternalSerializeV2ToGroup<T>(data, reflection);
            }
            return null;
        }
        /// <summary>
        /// This function serializes object to frame group
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data object</param>
        /// <param name="reflection">reflection info for this data type</param>
        /// <returns>Group collection</returns>
        private static List<FrameGroupItem> InternalSerializeV2ToGroup<T>(List<T> data, ReflectionType reflection)
        {
            //get group attribute
            FrameGroupAttribute frameGroupAttribute = reflection.GetCustomAttribute<FrameGroupAttribute>();

            //check group attribute
            if (frameGroupAttribute != null)
            {
                //get collection
                List<FrameGroupItem> collection = new List<FrameGroupItem>();

                //loop all object
                foreach (T obj in data)
                {
                    //serialize object
                    FrameGroupItem group = InternalSerializeV2ToGroup<T>(obj, reflection, frameGroupAttribute);
                    if (group != null)
                    {
                        collection.Add(group);
                    }
                }

                //return groups
                return collection;
            }

            //no data
            return null;
        }
        /// <summary>
        /// This function serializes object to frame group
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data object</param>
        /// <param name="reflection">reflection info for this data type</param>
        /// <returns>Group | null</returns>
        private static FrameGroupItem InternalSerializeV2ToGroup<T>(T data, ReflectionType reflection)
        {
            //get group attribute
            FrameGroupAttribute frameGroupAttribute = reflection.GetCustomAttribute<FrameGroupAttribute>();

            //check group attribute
            if (frameGroupAttribute != null)
            {
                //serialize data
                return InternalSerializeV2ToGroup<T>(data, reflection, frameGroupAttribute);
            }

            //no data
            return null;
        }
        /// <summary>
        /// This function serializes object to frame group
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data object</param>
        /// <param name="reflection">reflection info for this data type</param>
        /// <param name="frameGroupAttribute">Current group attribute</param>
        /// <returns>Group | null</returns>
        private static FrameGroupItem InternalSerializeV2ToGroup<T>(T data, ReflectionType reflection, FrameGroupAttribute frameGroupAttribute)
        {
            //variables
            Object value = null;

            //create group
            FrameGroupItem group = new FrameGroupItem(frameGroupAttribute.Address);

            //set property
            foreach (ReflectionProperty item in reflection.PropertyCollection.Values)
            {
                //check property type
                if (item.Property.CanRead)
                {
                    //get current attribute
                    FrameItemAttribute attribute = item.GetCustomAttribute<FrameItemAttribute>();
                    if (attribute != null)
                    {
                        try
                        {
                            value = item.Property.GetValue(data, null);
                            if (value != null)
                            {
                                IFrameItem frameItem = Frame.CreateFrameItem(attribute.Type, attribute.Address, value);
                                if (frameItem != null)
                                {
                                    group.Add(frameItem);
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

            //return current group
            return group;
        }
        /// <summary>
        /// Serializes the specified Object to Frame
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize to.</typeparam>
        /// <param name="data">The object to serialize.</param>
        /// <param name="frame">Frame</param>
        /// <param name="reflection">Reflection information for the Object to deserialize</param>
        /// <returns>Frame | null</returns>
        private static FrameV2 InternalSerializeV2<T>(List<T> data, FrameV2 frame, ReflectionType reflection)
        {
            //get group attribute
            FrameGroupAttribute frameGroupAttribute = reflection.GetCustomAttribute<FrameGroupAttribute>();

            //each object
            foreach (T obj in data)
            {
                //check attribute
                if (frameGroupAttribute != null)
                {
                    //serialize group
                    FrameGroupItem group = InternalSerializeV2ToGroup<T>(obj, reflection, frameGroupAttribute);

                    //create group to frame
                    frame.Add(group);
                }
            }

            //return result
            return frame;
        }
        /// <summary>
        /// Deserializes the Frame to a .NET object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="frame">The Frame to deserialize.</param>
        /// <returns>The deserialized object from the Frame.</returns>
        private static List<T> InternalDeserializeV2<T>(FrameV2 frame)
        {
            //get the object reglection
            ReflectionType reflection = ReflectionHelper.GetType(typeof(T));
            if (reflection != null)
            {
                return FrameHelper.InternalDeserializeV2<T>(frame, reflection);
            }
            return default(List<T>);
        }
        /// <summary>
        /// Deserializes the Group to a .NET object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="group">The Group to deserialize.</param>
        /// <returns>The deserialized object from the Group.</returns>
        private static T InternalDeserializeV2<T>(FrameGroupItem group)
        {
            //get the object reglection
            ReflectionType reflection = ReflectionHelper.GetType(typeof(T));
            if (reflection != null)
            {
                return FrameHelper.InternalDeserializeV2<T>(group, reflection);
            }
            return default(T);
        }
        /// <summary>
        /// Deserializes the Group to a .NET object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="group">The Group to deserialize.</param>
        /// <param name="reflection">Reflection information for the Object to deserialize</param>
        /// <returns>The deserialized object from the Group.</returns>
        private static T InternalDeserializeV2<T>(FrameGroupItem group, ReflectionType reflection)
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
                    FrameItemAttribute attribute = item.GetCustomAttribute<FrameItemAttribute>();
                    if (attribute != null)
                    {
                        Object value = null;
                        try
                        {
                            //get vale from frame
                            value = group.GetValue<Object>(attribute.Address);

                            //update value
                            value = FrameHelper.InternalUpdateValue(attribute.Type, item.Property.PropertyType, value);

                            //set value to property
                            item.Property.SetValue(result, value, null);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(String.Format("Error type: {0} -> {1}", (value == null ? "NULL" : value.ToString()), item.Property.Name), ex);
                        }
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Deserializes the Frame to a .NET object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="frame">The Frame to deserialize.</param>
        /// <param name="reflection">Reflection information for the Object to deserialize</param>
        /// <returns>The deserialized object from the Frame.</returns>
        private static List<T> InternalDeserializeV2<T>(FrameV2 frame, ReflectionType reflection)
        {
            //get group attribute
            FrameGroupAttribute frameGroupAttribute = reflection.GetCustomAttribute<FrameGroupAttribute>();
            if (frameGroupAttribute == null)
            {
                throw new Exception(String.Format("Type {0} does not contain FrameGroup attribute!", reflection.Type.Name));
            }

            //create collection
            List<T> collection = new List<T>();

            //each objects
            foreach (FrameGroupItem group in frame.Groups)
            {
                //check group address for this type
                if (group.Address == frameGroupAttribute.Address)
                {
                    //intiailize object
                    T result = FrameHelper.InternalDeserializeV2<T>(group, reflection);

                    //create objet to collection
                    collection.Add(result);
                }
            }

            //return result
            return collection;
        }
        #endregion

        #region - Private Static Methods V1 -
        /// <summary>
        /// Serializes the specified Object to Frame
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize to.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="frame">Frame</param>
        /// <returns>Frame | null</returns>
        private static Frame InternalSerialize<T>(T obj, Frame frame)
        {
            //get the object reglection
            ReflectionType reflection = ReflectionHelper.GetType(typeof(T));
            if (reflection != null)
            {
                return FrameHelper.InternalSerialize<T>(obj, frame, reflection);
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
        /// <param name="frame">Frame</param>
        /// <param name="reflection">Reflection information for the Object to deserialize</param>
        /// <returns>Frame | null</returns>
        private static Frame InternalSerialize<T>(T obj, Frame frame, ReflectionType reflection)
        {
            //set property
            foreach (ReflectionProperty item in reflection.PropertyCollection.Values)
            {
                //check property type
                if (item.Property.CanRead)
                {
                    //get current attribute
                    FrameItemAttribute attribute = item.GetCustomAttribute<FrameItemAttribute>();
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
                                    frame.Add(frameItem);
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
            return frame;
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
                    FrameItemAttribute attribute = item.GetCustomAttribute<FrameItemAttribute>();
                    if (attribute != null)
                    {
                        Object value = null;
                        try
                        {
                            //get vale from frame
                            value = frame.GetValue<Object>(attribute.Address);

                            //update value
                            value = FrameHelper.InternalUpdateValue(attribute.Type, item.Property.PropertyType, value);

                            //set value to property
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
        /// <summary>
        /// This method updates or converts value to target type
        /// </summary>
        /// <param name="type">Frame item type</param>
        /// <param name="targetType">Target property type</param>
        /// <param name="value">Value to convert</param>
        /// <returns></returns>
        private static Object InternalUpdateValue(FrameItemTypes type, Type targetType, Object value)
        {
            //check value
            if (value != null)
            {
                //check type
                if (type == FrameItemTypes.String && targetType == typeof(Guid) && value is String)
                {
                    Nullable<Guid> guidValue = (value as String).ToGuidWithoutDash();
                    return guidValue.Value;
                }
            }
            return value;
        }
        #endregion
    }
}
