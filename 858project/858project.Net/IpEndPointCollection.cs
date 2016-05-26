using System;
using System.Net;
using System.Collections.Generic;
using System.Text;

namespace Project858.Net
{
    /// <summary>
    /// Kolekia IpEndPointov ku ktorym sa chceme pripojit
    /// </summary>
    public sealed class IpEndPointCollection : List<IPEndPoint>
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public IpEndPointCollection()
        {

        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="ipEndPoint">IpEndPoint ktory chceme pridat do kolekcie</param>
        public IpEndPointCollection(IPEndPoint ipEndPoint)
        {
            this.Add(ipEndPoint);
        }
        #endregion

        #region - Public Method -
        /// <summary>
        /// Prida element do kolekcie. Ak uz v kolekcii nieco existuje overi ci su typy zhodne. 
        /// Kolekcia moze obsahovat len elementy zhoddeho typu
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// neinicializovany vstupny argument
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Nespravna struktura dat
        /// </exception>
        /// <param name="ipEndPoint">Element ktory chceme pridat</param>
        public new void Add(IPEndPoint ipEndPoint)
        {
            //overime pridavany element
            if (ipEndPoint == null)
                throw new ArgumentNullException("element");

            //prejdeme vsetky polozky
            for (int i = 0; i < this.Count; i++)
            {
                //ziskame pristup
                IPEndPoint iep = this[i];

                //overime element
                if (iep.Address.Equals(ipEndPoint.Address) &&
                    iep.Port == ipEndPoint.Port)
                {
                    //rovnaka polozka uz existuje
                    throw new ArgumentException("IPEndPoint already exist !");
                }
            }

            //pridame element do kolekcie
            base.Add(ipEndPoint);
        }
        /// <summary>
        /// Prida ipEndPoints do kolekcie. Ak uz v kolekcii nieco existuje overi ci su typy zhodne. 
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// neinicializovany vstupny argument
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Nespravna struktura dat
        /// </exception>
        /// <param name="ipEndPoints">Kolekcia ipEndPoints ktore chceme pridat</param>
        public new void AddRange(IEnumerable<IPEndPoint> ipEndPoints)
        {
            if (ipEndPoints == null)
                throw new ArgumentNullException("elements");

            //ziskame kolekciu
            ICollection<IPEndPoint> collection = ipEndPoints as ICollection<IPEndPoint>;
            IPEndPoint[] _ipEndPoints = new IPEndPoint[collection.Count];
            collection.CopyTo(_ipEndPoints, 0);

            //pridame elementy do kolekcie
            for (int i = 0; i < _ipEndPoints.Length; i++)
            {
                //pridame dalsi element
                this.Add(_ipEndPoints[i]);
            }
        }
        #endregion
    }
}
