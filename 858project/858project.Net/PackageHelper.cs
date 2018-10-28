using System;
using System.Collections.Generic;
using System.Reflection;
using Project858.Reflection;

namespace Project858.Net
{
    /// <summary>
    /// Frame helper for serializing or deserializing frame
    /// </summary>
    public static class PackageHelper
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        static PackageHelper()
        {
            PackageHelper.m_reflectionPropertyCollection = new ReflectionPackageModelCollection();
        }
        #endregion

        #region - Private Static Class -
        /// <summary>
        /// Item property
        /// </summary>
        private sealed class PackageModelPropertyItem
        {
            #region - Constructor -
            /// <summary>
            /// Initialize this class
            /// </summary>
            /// <param name="property">Property ktoru objekt reprezentuje</param>
            public PackageModelPropertyItem(PropertyInfo property)
            {
                if (property == null)
                    throw new ArgumentNullException("property");

                this.m_property = property;
            }
            #endregion

            #region - Properties -
            /// <summary>
            /// Vrati atributy prisluchajucej property
            /// </summary>
            public PackageItemAttribute ItemAttribute
            {
                get
                {
                    if (this.m_columnAttributes == null)
                    {
                        Object[] attributes = this.Property.GetCustomAttributes(typeof(PackageItemAttribute), true);
                        if (attributes != null && attributes.Length == 1)
                        {
                            this.m_columnAttributes = attributes[0] as PackageItemAttribute;
                        }
                    }
                    return this.m_columnAttributes;
                }
            }
            /// <summary>
            /// Property ktoru objekt reprezentuje
            /// </summary>
            public PropertyInfo Property
            {
                get { return this.m_property; }
            }
            #endregion

            #region - Variable -
            /// <summary>
            /// Atributy prisluchajucej property
            /// </summary>
            private PackageItemAttribute m_columnAttributes = null;
            /// <summary>
            /// Property
            /// </summary>
            private PropertyInfo m_property = null;
            #endregion
        }
        /// <summary>
        /// Package item
        /// </summary>
        private sealed class ReflectionPackageModelItem : Dictionary<UInt32, PackageModelPropertyItem>
        {
            #region - Constructor -
            /// <summary>
            /// Initialize this class
            /// </summary>
            /// <param name="type">Typ objektu ktoreho property objekt reprezentuje</param>
            public ReflectionPackageModelItem(Type type)
            {
                if (type == null)
                    throw new ArgumentNullException("type");

                this.Type = type;
                this.InternalInitializeProperty(type);
            }
            #endregion

            #region - Properties -
            /// <summary>
            /// Typ objektu
            /// </summary>
            public Type Type { get; set; }
            /// <summary>
            /// Atribut definujuci informacie o view prisluchajucemu k objektu
            /// </summary>
            public PackageGroupAttribute GroupAttribute
            {
                get
                {
                    if (this.m_viewAttribute == null)
                    {
                        Object[] attributes = this.Type.GetCustomAttributes(typeof(PackageGroupAttribute), true);
                        if (attributes != null && attributes.Length == 1)
                        {
                            this.m_viewAttribute = attributes[0] as PackageGroupAttribute;
                        }
                    }
                    return this.m_viewAttribute;
                }
            }
            #endregion

            #region - Variable -
            /// <summary>
            /// Atribut definujuci informacie o view prisluchajucemu k objektu
            /// </summary>
            private PackageGroupAttribute m_viewAttribute = null;
            #endregion

            #region - Public Methods -
            /// <summary>
            /// Vrati pozadovanu property a jej informacie na zaklade mena
            /// </summary>
            /// <param name="address">Meno property ktoru ziadame</param>
            /// <returns>ReflectionPropertyItem alebo null</returns>
            public PackageModelPropertyItem FindProperty(UInt32 address)
            {
                if (this.ContainsKey(address))
                {
                    return this[address];
                }
                return null;
            }
            #endregion

            #region - Private Methods -
            /// <summary>
            /// Inicializuje property objektu
            /// </summary>
            /// <param name="type">Typ objektu ktoreho property objekt reprezentuje</param>
            private void InternalInitializeProperty(Type type)
            {
                PropertyInfo[] properties = type.GetProperties();
                int length = properties.Length;
                for (int i = 0; i < length; i++)
                {
                    PropertyInfo property = properties[i];
                    PackageModelPropertyItem item = new PackageModelPropertyItem(property);
                    if (item.ItemAttribute != null)
                    {
                        this.Add(item.ItemAttribute.Address, item);
                    }
                }
            }
            #endregion
        }
        /// <summary>
        /// Cache data
        /// </summary>
        private sealed class ReflectionPackageModelCollection : Dictionary<UInt16, ReflectionPackageModelItem>
        {
            #region - Public Methods -
            /// <summary>
            /// Vrati alebo vytvori a vrati kolekciu property pre pozadovany typ objektu
            /// </summary>
            /// <param name="type">Model type</param>
            /// <returns>ReflectionPropertyItemCollection</returns>
            public ReflectionPackageModelItem FindPropertyCollection(Type type)
            {
                //get group attribude
                PackageGroupAttribute attribute = InternalGetGroupAttribute(type);
                if (attribute != null)
                {
                    if (!this.ContainsKey(attribute.Address))
                    {
                        return this.InternalCreateType(type);
                    }
                    return this[attribute.Address];
                }
                return null;
            }
            #endregion

            #region - Private Methods -
            /// <summary>
            /// Prida objekt pre pozadovany type
            /// </summary>
            /// <param name="type">Type objektu pre ktory chceme property nacitat</param>
            private ReflectionPackageModelItem InternalCreateType(Type type)
            {
                ReflectionPackageModelItem item = new ReflectionPackageModelItem(type);
                if (item.GroupAttribute != null)
                {
                    this.Add(item.GroupAttribute.Address, item);
                    return item;
                }
                return null;
            }
            /// <summary>
            /// This method returns group attribude from type
            /// </summary>
            /// <param name="type">Type</param>
            /// <returns>Group attribude</returns>
            private PackageGroupAttribute InternalGetGroupAttribute(Type type)
            {
                Object[] attributes = type.GetCustomAttributes(typeof(PackageGroupAttribute), true);
                if (attributes != null && attributes.Length == 1)
                {
                    return attributes[0] as PackageGroupAttribute;
                }
                return null;
            }
            #endregion
        }
        #endregion

        #region - Private Static Variable -
        /// <summary>
        /// Static variable for this item
        /// </summary>
        private static ReflectionPackageModelCollection m_reflectionPropertyCollection;
        #endregion

        #region - Public Static Methods V2 -
        /// <summary>
        /// This function registers model to cache
        /// </summary>
        /// <param name="type">Type</param>
        public static void RegisterModel(Type type)
        {
            PackageHelper.m_reflectionPropertyCollection.FindPropertyCollection(type);
        }
        /// <summary>
        /// This function finds frame in array
        /// </summary>
        /// <param name="array">Input array data</param>
        /// <param name="action">Callback for parsing frame items</param>
        /// <returns>Frame | null</returns>
        public static PackageV2 FindPackageV2(List<Byte> array, Func<UInt16, UInt16, UInt32, PackageItemTypes> action)
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
                        PackageV2 frame = PackageHelper.ConstructPackageV2(array, index + 6, length - 6, address, state, action);
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
        private static PackageV2 ConstructPackageV2(List<Byte> array, int index, int length, UInt16 address, Byte state, Func<UInt16, UInt16, UInt32, PackageItemTypes> action)
        {
            //check data length available
            if ((array.Count - index) >= length)
            {
                //get checksum
                Byte checkSum = PackageHelper.GetFrameDataCheckSum(array, index - 5, length + 5);
                Byte currentCheckSum = array[index + length + 0];
                if (checkSum != currentCheckSum)
                {
                    return null;
                }

                //copy data block
                List<Byte> temp = array.GetRange(index, length);

                //get current action
                Func<UInt16, UInt16, UInt32, PackageItemTypes> currentAction = action == null ? PackageHelper.InternalGetPackageItemType : action;

                //initialize package
                PackageV2 frame = new PackageV2(address, state, temp, currentAction);

                //remove data
                array.RemoveRange(0, length + index + 1);

                //return package
                return frame;
            }
            return null;
        }
        /// <summary>
        /// This methods gets item type for current package and group
        /// </summary>
        /// <param name="frameAddress">Package address</param>
        /// <param name="groupAddress">Grop address</param>
        /// <param name="itemAddress">Item address</param>
        /// <returns></returns>
        private static PackageItemTypes InternalGetPackageItemType(UInt16 frameAddress, UInt16 groupAddress, UInt32 itemAddress)
        {
            if (PackageHelper.m_reflectionPropertyCollection != null)
            {
                if (PackageHelper.m_reflectionPropertyCollection.ContainsKey(groupAddress))
                {
                    ReflectionPackageModelItem modelItem = PackageHelper.m_reflectionPropertyCollection[groupAddress];
                    if (modelItem != null)
                    {
                        if (modelItem.ContainsKey(itemAddress))
                        {
                            PackageModelPropertyItem propertyItem = modelItem[itemAddress];
                            return propertyItem.ItemAttribute.Type;
                        }
                    }
                }
            }
            return PackageItemTypes.Unkown;
        }
        /// <summary>
        /// This function serializes object to frame group
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data object</param>
        /// <returns>Group collection</returns>
        public static List<PackageGroupItem> SerializeV2ToGroup<T>(List<T> data)
        {
            return PackageHelper.InternalSerializeV2ToGroup<T>(data);
        }
        /// <summary>
        /// This function serializes object to frame group
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data object</param>
        /// <returns>Group collection</returns>
        public static PackageGroupItem SerializeV2ToGroup<T>(T data)
        {
            return PackageHelper.InternalSerializeV2ToGroup<T>(data);
        }
        /// <summary>
        /// Serializes the specified object to a Frame.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize to.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="address">Frame command address</param>
        /// <returns>Frame | null</returns>
        public static PackageV2 SerializeV2<T>(T obj, UInt16 address)
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
        public static PackageV2 SerializeV2<T>(List<T> obj, UInt16 commandAddress)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            //intiailize object
            PackageV2 result = new PackageV2(commandAddress, 0x00);

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
        public static PackageV2 SerializeV2<T>(List<T> obj, PackageV2 frame)
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
        public static List<T> DeserializeV2<T>(PackageV2 frame)
        {
            if (frame == null)
                throw new ArgumentNullException("frame");

            return PackageHelper.InternalDeserializeV2<T>(frame);
        }
        /// <summary>
        /// Deserializes the Group to a .NET object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="group">The Group to deserialize.</param>
        /// <returns>The deserialized object from the Group.</returns>
        public static T DeserializeV2<T>(PackageGroupItem group)
        {
            if (group == null)
                throw new ArgumentNullException("group");

            return PackageHelper.InternalDeserializeV2<T>(group);
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
        public static Package FindFrame(List<Byte> array, Func<UInt16, UInt32, PackageItemTypes> action)
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
                        Package frame = PackageHelper.ConstructFrame(array, index + 5, length - 5, address, action);
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
        private static Package ConstructFrame(List<Byte> array, int index, int length, UInt16 address, Func<UInt16, UInt32, PackageItemTypes> action)
        {
            //check data length available
            if ((array.Count - index) >= length)
            {
                //get checksum
                Byte checkSum = PackageHelper.GetFrameDataCheckSum(array, index - 4, length + 4);
                Byte currentCheckSum = array[index + length + 0];
                if (checkSum != currentCheckSum)
                {
                    return null;
                }

                //copy data block
                List<Byte> temp = array.GetRange(index, length);

                //initialize package
                Package frame = new Package(address, temp, action);

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
        public static Package Serialize<T>(T obj, UInt16 commandAddress)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            //intiailize object
            Package result = new Package(commandAddress);

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
        public static Package Serialize<T>(T obj, Package frame)
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
        public static T Deserialize<T>(Package frame)
        {
            if (frame == null)
                throw new ArgumentNullException("frame");

            return PackageHelper.InternalDeserialize<T>(frame);
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
        private static PackageV2 InternalSerializeV2<T>(List<T> obj, PackageV2 frame)
        {
            //get the object reglection
            ReflectionType reflection = ReflectionHelper.GetType(typeof(T));
            if (reflection != null)
            {
                return PackageHelper.InternalSerializeV2<T>(obj, frame, reflection);
            }
            return null;
        }
        /// <summary>
        /// This function serializes object to frame group
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data object</param>
        /// <returns>Group collection</returns>
        private static PackageGroupItem InternalSerializeV2ToGroup<T>(T data)
        {
            //get the object reglection
            ReflectionType reflection = ReflectionHelper.GetType(typeof(T));
            if (reflection != null)
            {
                return PackageHelper.InternalSerializeV2ToGroup<T>(data, reflection);
            }
            return null;
        }
        /// <summary>
        /// This function serializes object to frame group
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data object</param>
        /// <returns>Group collection</returns>
        private static List<PackageGroupItem> InternalSerializeV2ToGroup<T>(List<T> data)
        {
            //get the object reglection
            ReflectionType reflection = ReflectionHelper.GetType(typeof(T));
            if (reflection != null)
            {
                return PackageHelper.InternalSerializeV2ToGroup<T>(data, reflection);
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
        private static List<PackageGroupItem> InternalSerializeV2ToGroup<T>(List<T> data, ReflectionType reflection)
        {
            //get group attribute
            PackageGroupAttribute PackageGroupAttribute = reflection.GetCustomAttribute<PackageGroupAttribute>();

            //check group attribute
            if (PackageGroupAttribute != null)
            {
                //get collection
                List<PackageGroupItem> collection = new List<PackageGroupItem>();

                //loop all object
                foreach (T obj in data)
                {
                    //serialize object
                    PackageGroupItem group = InternalSerializeV2ToGroup<T>(obj, reflection, PackageGroupAttribute);
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
        private static PackageGroupItem InternalSerializeV2ToGroup<T>(T data, ReflectionType reflection)
        {
            //get group attribute
            PackageGroupAttribute PackageGroupAttribute = reflection.GetCustomAttribute<PackageGroupAttribute>();

            //check group attribute
            if (PackageGroupAttribute != null)
            {
                //serialize data
                return InternalSerializeV2ToGroup<T>(data, reflection, PackageGroupAttribute);
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
        /// <param name="PackageGroupAttribute">Current group attribute</param>
        /// <returns>Group | null</returns>
        private static PackageGroupItem InternalSerializeV2ToGroup<T>(T data, ReflectionType reflection, PackageGroupAttribute PackageGroupAttribute)
        {
            //variables
            Object value = null;

            //create group
            PackageGroupItem group = new PackageGroupItem(PackageGroupAttribute.Address);

            //set property
            foreach (ReflectionProperty item in reflection.PropertyCollection.Values)
            {
                //check property type
                if (item.Property.CanRead)
                {
                    //get current attribute
                    PackageItemAttribute attribute = item.GetCustomAttribute<PackageItemAttribute>();
                    if (attribute != null)
                    {
                        try
                        {
                            value = item.Property.GetValue(data, null);
                            if (value != null)
                            {
                                IPackageItem frameItem = Package.CreatePackageItem(attribute.Type, attribute.Address, value);
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
        private static PackageV2 InternalSerializeV2<T>(List<T> data, PackageV2 frame, ReflectionType reflection)
        {
            //get group attribute
            PackageGroupAttribute PackageGroupAttribute = reflection.GetCustomAttribute<PackageGroupAttribute>();

            //each object
            foreach (T obj in data)
            {
                //check attribute
                if (PackageGroupAttribute != null)
                {
                    //serialize group
                    PackageGroupItem group = InternalSerializeV2ToGroup<T>(obj, reflection, PackageGroupAttribute);

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
        private static List<T> InternalDeserializeV2<T>(PackageV2 frame)
        {
            //get the object reglection
            ReflectionType reflection = ReflectionHelper.GetType(typeof(T));
            if (reflection != null)
            {
                return PackageHelper.InternalDeserializeV2<T>(frame, reflection);
            }
            return default(List<T>);
        }
        /// <summary>
        /// Deserializes the Group to a .NET object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="group">The Group to deserialize.</param>
        /// <returns>The deserialized object from the Group.</returns>
        private static T InternalDeserializeV2<T>(PackageGroupItem group)
        {
            //get the object reglection
            ReflectionType reflection = ReflectionHelper.GetType(typeof(T));
            if (reflection != null)
            {
                return PackageHelper.InternalDeserializeV2<T>(group, reflection);
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
        private static T InternalDeserializeV2<T>(PackageGroupItem group, ReflectionType reflection)
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
                    PackageItemAttribute attribute = item.GetCustomAttribute<PackageItemAttribute>();
                    if (attribute != null)
                    {
                        Object value = null;
                        try
                        {
                            //get vale from frame
                            value = group.GetValue<Object>(attribute.Address);

                            //update value
                            value = PackageHelper.InternalUpdateValue(attribute.Type, item.Property.PropertyType, value);

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
        private static List<T> InternalDeserializeV2<T>(PackageV2 frame, ReflectionType reflection)
        {
            //get group attribute
            PackageGroupAttribute PackageGroupAttribute = reflection.GetCustomAttribute<PackageGroupAttribute>();
            if (PackageGroupAttribute == null)
            {
                throw new Exception(String.Format("Type {0} does not contain FrameGroup attribute!", reflection.Type.Name));
            }

            //create collection
            List<T> collection = new List<T>();

            //each objects
            foreach (PackageGroupItem group in frame.Groups)
            {
                //check group address for this type
                if (group.Address == PackageGroupAttribute.Address)
                {
                    //intiailize object
                    T result = PackageHelper.InternalDeserializeV2<T>(group, reflection);

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
        private static Package InternalSerialize<T>(T obj, Package frame)
        {
            //get the object reglection
            ReflectionType reflection = ReflectionHelper.GetType(typeof(T));
            if (reflection != null)
            {
                return PackageHelper.InternalSerialize<T>(obj, frame, reflection);
            }
            return null;
        }
        /// <summary>
        /// Deserializes the Frame to a .NET object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="frame">The Frame to deserialize.</param>
        /// <returns>The deserialized object from the Frame.</returns>
        private static T InternalDeserialize<T>(Package frame)
        {
            //get the object reglection
            ReflectionType reflection = ReflectionHelper.GetType(typeof(T));
            if (reflection != null)
            {
                return PackageHelper.InternalDeserialize<T>(frame, reflection);
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
        private static Package InternalSerialize<T>(T obj, Package frame, ReflectionType reflection)
        {
            //set property
            foreach (ReflectionProperty item in reflection.PropertyCollection.Values)
            {
                //check property type
                if (item.Property.CanRead)
                {
                    //get current attribute
                    PackageItemAttribute attribute = item.GetCustomAttribute<PackageItemAttribute>();
                    if (attribute != null)
                    {
                        Object value = null;
                        try
                        {
                            value = item.Property.GetValue(obj, null);
                            if (value != null)
                            {
                                IPackageItem frameItem = Package.CreatePackageItem(attribute.Type, attribute.Address, value);
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
        private static T InternalDeserialize<T>(Package frame, ReflectionType reflection)
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
                    PackageItemAttribute attribute = item.GetCustomAttribute<PackageItemAttribute>();
                    if (attribute != null)
                    {
                        Object value = null;
                        try
                        {
                            //get vale from frame
                            value = frame.GetValue<Object>(attribute.Address);

                            //update value
                            value = PackageHelper.InternalUpdateValue(attribute.Type, item.Property.PropertyType, value);

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
        private static Object InternalUpdateValue(PackageItemTypes type, Type targetType, Object value)
        {
            //check value
            if (value != null)
            {
                //check type
                if (type == PackageItemTypes.String && targetType == typeof(Guid) && value is String)
                {
                    Nullable<Guid> guidValue = (value as String).ToGuidWithoutDash();
                    return guidValue.Value;
                }
                else if (type == PackageItemTypes.Enum)
                {
                    if (Utility.IsNullableType(targetType))
                    {
                        targetType = Nullable.GetUnderlyingType(targetType);
                    }
                    if (!targetType.IsEnum)
                    {
                        throw new Exception(String.Format("TargetType is not enum type. [{0}]", targetType));
                    }
                    return Enum.ToObject(targetType, value);
                }
            }
            return value;
        }
        #endregion
    }
}
