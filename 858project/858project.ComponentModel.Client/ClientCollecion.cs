using System;
using System.Collections.Generic;
using System.Text;

namespace Project858.ComponentModel.Client
{
    /// <summary>
    /// Kolekcia klientov
    /// </summary>
    public class ClientCollecion : List<IClient>
    {
        #region - Public Method -
        /// <summary>
        /// Spusti vsetkych klientov v kolekcii
        /// </summary>
        /// <returns>True = klienti boli uspesne inicializovany</returns>
        public Boolean StartAll()
        {
            //spustime vsetkych klientov
            for (int i = 0; i < this.Count; i++)
                if (this[i].ClientState != ClientStates.Start)
                    if (!this[i].Start())
                        return false;

            //klienti boli uspesne inicializovany
            return true;
        }
        /// <summary>
        /// Pozastavi vsetkych klientov v kolekcii
        /// </summary>
        public void PauseAll()
        {
            //pozastavime vsetkych klientov
            for (int i = 0; i < this.Count; i++)
                if (this[i].ClientState != ClientStates.Pause)
                    this[i].Pause();
        }
        /// <summary>
        /// Ukonci vsetkych klientov v kolekcii
        /// </summary>
        public void StopAll()
        {
            //ukoncime vsetkych klientov
            for (int i = this.Count - 1; i > -1; i--)
                if (this[i].ClientState != ClientStates.Stop)
                    this[i].Stop();
        }
        /// <summary>
        /// Overi ci sa rovnaky typ klienta uz nenacahdza v zozname
        /// </summary>
        /// <param name="type">Typ klienta</param>
        /// <returns>True = klient rovnakeho typu sa v zozname uz nachadza</returns>
        public Boolean TypeContains(Type type)
        {
            //prejdeme vsetkych klientov
            foreach (IClient client in this)
            {
                //ziskame typ
                Type clientType = client.GetType();

                //overime ci typy maju zhodne mena
                if (clientType.Name == type.Name &&
                    clientType.FullName == type.FullName)
                    return true;
            }

            //zadany tym sa v zozname este nenachadza
            return false;
        }
        #endregion
    }
}
