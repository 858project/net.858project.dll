using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Data;

namespace Project858.Globalization
{
    /// <summary>
    /// Kalendar min.
    /// Data: http://www.kalendar.sk/medzinarodne/
    /// Data: http://www.juko56.dobrosoft.sk/historia4.htm
    /// </summary>
    public static class CalendarNames
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        static CalendarNames()
        {
            //inicializujeme kolekciu
            _itemCollection = InitializeItemCollection();
        }
        #endregion

        #region - Class -
        /// <summary>
        /// Item definujuci datum a den v mesiaci
        /// </summary>
        public class ItemDate
        {
            #region - Constructor -
            /// <summary>
            /// Initialize this class
            /// </summary>
            /// <param name="month">Mesiac</param>
            /// <param name="day">Dan</param>
            public ItemDate(Int32 month, Int32 day)
            {
                //osetrime vstup
                if (month > 12 || month < 1)
                    throw new ArgumentNullException("month");
                if (day > 31 || day < 1)
                    throw new ArgumentNullException("day");

                //uchovame si hodnoty
                this._month = month;
                this._day = day;
            }
            #endregion

            #region - Properties -
            /// <summary>
            /// Mesiac
            /// </summary>
            public Int32 Month
            {
                get { return _month; }
            }
            /// <summary>
            /// Rok
            /// </summary>
            public Int32 Day
            {
                get { return _day; }
            }
            #endregion

            #region - Variable -
            /// <summary>
            /// Mesiac
            /// </summary>
            private Int32 _month = 0;
            /// <summary>
            /// Rok
            /// </summary>
            private Int32 _day = 0;
            #endregion

            #region - Public Method -
            /// <summary>
            /// Overi aktualny objekt so vstupnym objektom
            /// </summary>
            /// <param name="obj">Objekt ktory chceme porovnat</param>
            /// <returns>True = objekty su zhodne</returns>
            public override bool Equals(object obj)
            {
                //ak nie su zhodne typy
                if (obj.GetType() != typeof(ItemDate))
                    return false;

                //ziskame pristup
                ItemDate item = obj as ItemDate;

                //ak nie su zhodne data
                if (item.Day != this.Day || item.Month != this.Month)
                    return false;

                //objekty su zhodne
                return true;
            }
            /// <summary>
            /// Vrati hesovaci kod triedy
            /// </summary>
            /// <returns>Hesovaci kod</returns>
            public override int GetHashCode()
            {
                return this._month * 100 + this._day;
            }
            #endregion
        }
        /// <summary>
        /// Data prisluchajuce konkretnemu casovemu itemu
        /// </summary>
        public class ItemData
        {
            #region - Constructor -
            /// <summary>
            /// Initialize this class
            /// </summary>
            /// <exception cref="ArgumentNullException">
            /// Neinicializovany vstupny argument
            /// </exception>
            /// <param name="name">Meno alebo udalost prisluchajuca danemu dnu</param>
            public ItemData(String name)
            {
                if (String.IsNullOrEmpty(name.Trim()))
                    throw new ArgumentNullException("name");

                //uchovame si data
                this._name = name;
                this._internationalDayInfo = new List<String>();
            }
            #endregion

            #region - Properties -
            /// <summary>
            /// Meno alebo udalost prisluchajuca danemu dnu 
            /// </summary>
            public String Name
            {
                get { return _name; }
            }
            /// <summary>
            /// Kolekcia udalosti patriacich medzi medzinarodne dni
            /// </summary>
            public List<String> InternationalDayInfo
            {
                get { return _internationalDayInfo; }
            }
            #endregion

            #region - Variable -
            /// <summary>
            /// Kolekcia udalosti patriacich medzi medzinarodne dni
            /// </summary>
            private List<String> _internationalDayInfo = null;
            /// <summary>
            /// Meno alebo udalost prisluchajuca danemu dnu 
            /// </summary>
            private String _name = String.Empty;
            #endregion

            #region - Public Method -
            /// <summary>
            /// Prida info o medzinarodnom dni
            /// </summary>
            /// <param name="info">Info ktore chceme pridat</param>
            public void AddInternationalDayInfo(String info)
            {
                //overime vstup
                if (String.IsNullOrEmpty(info))
                    throw new ArgumentNullException("info");

                this._internationalDayInfo.Add(info);
            }
            #endregion
        }
        #endregion

        #region - Variable -
        /// <summary>
        /// Kolekcia dostupnych dat
        /// </summary>
        private static Dictionary<ItemDate, ItemData> _itemCollection = null;
        #endregion

        #region - Public Method -
        /// <summary>
        /// Vrati data ku konrketnemu casovemu itemu
        /// </summary>
        /// <param name="date">Casovy item definujuci den a mesiac</param>
        /// <returns>Data z pozadovaneho dna a mesiaa alebo null</returns>
        public static ItemData GetItemData(ItemDate date)
        {
            if (_itemCollection.ContainsKey(date))
            {
                //vratime polozku dat
                return _itemCollection[date];
            }

            //data nie su dostupne
            return null;
        }
        #endregion

        #region - Private Method -
        /// <summary>
        /// Inicializuje kolekciu dostupnych dat
        /// </summary>
        /// <returns>Kolekcia dostupnych dat</returns>
        private static Dictionary<ItemDate, ItemData> InitializeItemCollection()
        {
            //inicializujeme kolekciu
            Dictionary<ItemDate, ItemData> itemCollection = new Dictionary<ItemDate, ItemData>();

            //pomocne premenne
            InitializeItemCollectionJanuary(ref itemCollection);
            InitializeItemCollectionFebruary(ref itemCollection);
            InitializeItemCollectionMarch(ref itemCollection);
            InitializeItemCollectionApril(ref itemCollection);
            InitializeItemCollectionMay(ref itemCollection);
            InitializeItemCollectionJune(ref itemCollection);
            InitializeItemCollectionJuly(ref itemCollection);
            InitializeItemCollectionAugust(ref itemCollection);
            InitializeItemCollectionSeptember(ref itemCollection);
            InitializeItemCollectionOctober(ref itemCollection);
            InitializeItemCollectionNovember(ref itemCollection);
            InitializeItemCollectionDecember(ref itemCollection);

            //vratime kolekciu
            return itemCollection;
        }
        /// <summary>
        /// Inicializuje data pre januar
        /// </summary>
        /// <param name="collection">Kolekcia do ktorej chceme data vlozit</param>
        private static void InitializeItemCollectionJanuary(ref Dictionary<ItemDate, ItemData> collection)
        {
            //pomocne premenne
            ItemData itemData = null;

            //inicializujeme data pre januar
            itemData = new ItemData("Nov� rok");
            itemData.AddInternationalDayInfo("De� vzniku Slovenskej republiky");
            itemData.AddInternationalDayInfo("Svetov� de� mieru (p�pe� Pavol VI.)");
            collection.Add(new ItemDate(1, 1), itemData);
            itemData = new ItemData("Alexandra");
            collection.Add(new ItemDate(1, 2), itemData);
            itemData = new ItemData("Daniela");
            collection.Add(new ItemDate(1, 3), itemData);
            itemData = new ItemData("Drahoslav");
            itemData.AddInternationalDayInfo("Svetov� Braillov de� (Svetov� slepeck� �nia WBU)");
            collection.Add(new ItemDate(1, 4), itemData);
            itemData = new ItemData("Andrea");
            collection.Add(new ItemDate(1, 5), itemData);
            itemData = new ItemData("Ant�nia");
            collection.Add(new ItemDate(1, 6), itemData);
            itemData = new ItemData("Bohuslava");
            collection.Add(new ItemDate(1, 7), itemData);
            itemData = new ItemData("Sever�n");
            collection.Add(new ItemDate(1, 8), itemData);
            itemData = new ItemData("Alexej");
            collection.Add(new ItemDate(1, 9), itemData);
            itemData = new ItemData("D�a");
            collection.Add(new ItemDate(1, 10), itemData);
            itemData = new ItemData("Malv�na");
            collection.Add(new ItemDate(1, 11), itemData);
            itemData = new ItemData("Ernest");
            collection.Add(new ItemDate(1, 12), itemData);
            itemData = new ItemData("Rastislav");
            collection.Add(new ItemDate(1, 13), itemData);
            itemData = new ItemData("Radovan");
            collection.Add(new ItemDate(1, 14), itemData);
            itemData = new ItemData("Dobroslav");
            collection.Add(new ItemDate(1, 15), itemData);
            itemData = new ItemData("Krist�na");
            collection.Add(new ItemDate(1, 16), itemData);
            itemData = new ItemData("Nata�a");
            collection.Add(new ItemDate(1, 17), itemData);
            itemData = new ItemData("Bohdana");
            collection.Add(new ItemDate(1, 18), itemData);
            itemData = new ItemData("Drahom�ra");
            collection.Add(new ItemDate(1, 19), itemData);
            itemData = new ItemData("Dalibor");
            collection.Add(new ItemDate(1, 20), itemData);
            itemData = new ItemData("Vincent");
            collection.Add(new ItemDate(1, 21), itemData);
            itemData = new ItemData("Zora");
            collection.Add(new ItemDate(1, 22), itemData);
            itemData = new ItemData("Milo�");
            collection.Add(new ItemDate(1, 23), itemData);
            itemData = new ItemData("Timotej");
            itemData.AddInternationalDayInfo("De� komplimentov");
            collection.Add(new ItemDate(1, 24), itemData);
            itemData = new ItemData("Gejza");
            collection.Add(new ItemDate(1, 25), itemData);
            itemData = new ItemData("Tamara");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� coln�ctva");
            collection.Add(new ItemDate(1, 26), itemData);
            itemData = new ItemData("Bohu�");
            itemData.AddInternationalDayInfo("Pam�tn� de� holokaustu, de� oslobodenia koncentra�n�ho t�bora v Osvien�ime (1945)");
            collection.Add(new ItemDate(1, 27), itemData);
            itemData = new ItemData("Alfonz");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� mobiliz�cie proti jadrovej vojne");
            collection.Add(new ItemDate(1, 28), itemData);
            itemData = new ItemData("Ga�par");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� bez internetu");
            collection.Add(new ItemDate(1, 29), itemData);
            itemData = new ItemData("Ema");
            collection.Add(new ItemDate(1, 30), itemData);
            itemData = new ItemData("Emil");
            collection.Add(new ItemDate(1, 31), itemData);
        }
        /// <summary>
        /// Inicializuje data pre februar
        /// </summary>
        /// <param name="collection">Kolekcia do ktorej chceme data vlozit</param>
        private static void InitializeItemCollectionFebruary(ref Dictionary<ItemDate, ItemData> collection)
        {
            //pomocne premenne
            ItemData itemData = null;

            //inicializujeme data pre februar
            itemData = new ItemData("Tatiana");
            collection.Add(new ItemDate(2, 1), itemData);
            itemData = new ItemData("Erik, Erika");
            itemData.AddInternationalDayInfo("Svetov� de� mokrad�");
            collection.Add(new ItemDate(2, 2), itemData);
            itemData = new ItemData("Bla�ej");
            collection.Add(new ItemDate(2, 3), itemData);
            itemData = new ItemData("Veronika");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� boja proti nezamestnanosti");
            itemData.AddInternationalDayInfo("Svetov� de� boja proti rakovine");
            collection.Add(new ItemDate(2, 4), itemData);
            itemData = new ItemData("Ag�ta");
            collection.Add(new ItemDate(2, 5), itemData);
            itemData = new ItemData("Dorota");
            collection.Add(new ItemDate(2, 6), itemData);
            itemData = new ItemData("Vanda");
            collection.Add(new ItemDate(2, 7), itemData);
            itemData = new ItemData("Zoja");
            collection.Add(new ItemDate(2, 8), itemData);
            itemData = new ItemData("Zdenko");
            collection.Add(new ItemDate(2, 9), itemData);
            itemData = new ItemData("Gabriela");
            collection.Add(new ItemDate(2, 10), itemData);
            itemData = new ItemData("Dezider");
            itemData.AddInternationalDayInfo("Svetov� de� chor�ch a trpiacich");
            collection.Add(new ItemDate(2, 11), itemData);
            itemData = new ItemData("Perla");
            collection.Add(new ItemDate(2, 12), itemData);
            itemData = new ItemData("Arp�d");
            collection.Add(new ItemDate(2, 13), itemData);
            itemData = new ItemData("Valent�n");
            itemData.AddInternationalDayInfo("De� za��ben�ch");
            collection.Add(new ItemDate(2, 14), itemData);
            itemData = new ItemData("Pravoslav");
            collection.Add(new ItemDate(2, 15), itemData);
            itemData = new ItemData("Ida, Liana");
            collection.Add(new ItemDate(2, 16), itemData);
            itemData = new ItemData("Miloslava");
            collection.Add(new ItemDate(2, 17), itemData);
            itemData = new ItemData("Jarom�r");
            collection.Add(new ItemDate(2, 18), itemData);
            itemData = new ItemData("Vlasta");
            collection.Add(new ItemDate(2, 19), itemData);
            itemData = new ItemData("L�via");
            collection.Add(new ItemDate(2, 20), itemData);
            itemData = new ItemData("Eleon�ra");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� matersk�ho jazyka (UNESCO)");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� sprievodcov cestovn�ho ruchu");
            collection.Add(new ItemDate(2, 21), itemData);
            itemData = new ItemData("Etela");
            collection.Add(new ItemDate(2, 22), itemData);
            itemData = new ItemData("Roman, Romana");
            collection.Add(new ItemDate(2, 23), itemData);
            itemData = new ItemData("Matej");
            collection.Add(new ItemDate(2, 24), itemData);
            itemData = new ItemData("Frederik");
            collection.Add(new ItemDate(2, 25), itemData);
            itemData = new ItemData("Viktor");
            collection.Add(new ItemDate(2, 26), itemData);
            itemData = new ItemData("Alexandr");
            collection.Add(new ItemDate(2, 27), itemData);
            itemData = new ItemData("Zlatica");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� kvetinov�ho dizajnu");
            collection.Add(new ItemDate(2, 28), itemData);
            itemData = new ItemData("Radom�r");
            collection.Add(new ItemDate(2, 29), itemData);
        }
        /// <summary>
        /// Inicializuje data pre marec
        /// </summary>
        /// <param name="collection">Kolekcia do ktorej chceme data vlozit</param>
        private static void InitializeItemCollectionMarch(ref Dictionary<ItemDate, ItemData> collection)
        {
            //pomocne premenne
            ItemData itemData = null;

            itemData = new ItemData("Alb�n");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� boja proti at�mov�m zbraniam");
            collection.Add(new ItemDate(3, 1), itemData);
            itemData = new ItemData("Ane�ka");
            collection.Add(new ItemDate(3, 2), itemData);
            itemData = new ItemData("Bohumil, Bohumila");
            collection.Add(new ItemDate(3, 3), itemData);
            itemData = new ItemData("Kazim�r");
            collection.Add(new ItemDate(3, 4), itemData);
            itemData = new ItemData("Fridrich");
            collection.Add(new ItemDate(3, 5), itemData);
            itemData = new ItemData("Radoslav");
            itemData.AddInternationalDayInfo("Svetov� de� modlitieb");
            collection.Add(new ItemDate(3, 6), itemData);
            itemData = new ItemData("Tom�");
            itemData.AddInternationalDayInfo("De� v�skumu rakoviny");
            collection.Add(new ItemDate(3, 7), itemData);
            itemData = new ItemData("Alan");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� �ien");
            collection.Add(new ItemDate(3, 8), itemData);
            itemData = new ItemData("Franti�ka");
            collection.Add(new ItemDate(3, 9), itemData);
            itemData = new ItemData("Branislav");
            collection.Add(new ItemDate(3, 10), itemData);
            itemData = new ItemData("Angela");
            collection.Add(new ItemDate(3, 11), itemData);
            itemData = new ItemData("Gregor");
            itemData.AddInternationalDayInfo("vetov� de� obli�iek");
            collection.Add(new ItemDate(3, 12), itemData);
            itemData = new ItemData("Vlastimil");
            collection.Add(new ItemDate(3, 13), itemData);
            itemData = new ItemData("Matilda");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� boja proti priehrad�m, za rieky, vodu a �ivot");
            collection.Add(new ItemDate(3, 14), itemData);
            itemData = new ItemData("Svetlana");
            itemData.AddInternationalDayInfo("Svetov� de� spotrebite�sk�ch pr�v");
            collection.Add(new ItemDate(3, 15), itemData);
            itemData = new ItemData("Boleslav");
            collection.Add(new ItemDate(3, 16), itemData);
            itemData = new ItemData("�ubica");
            collection.Add(new ItemDate(3, 17), itemData);
            itemData = new ItemData("Eduard");
            collection.Add(new ItemDate(3, 18), itemData);
            itemData = new ItemData("Jozef");
            itemData.AddInternationalDayInfo("Sviatok sv. Jozefa, patr�na robotn�kov");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� astron�mie");
            collection.Add(new ItemDate(3, 19), itemData);
            itemData = new ItemData("V�azoslav");
            itemData.AddInternationalDayInfo("Svetov� de� divadla pre deti a ml�de�");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� invalidov");
            collection.Add(new ItemDate(3, 20), itemData);
            itemData = new ItemData("Blahoslav");
            itemData.AddInternationalDayInfo("Svetov� de� po�zie (UNESCO)");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� boja proti rasovej diskrimin�cii");
            itemData.AddInternationalDayInfo("Svetov� de� lesov");
            collection.Add(new ItemDate(3, 21), itemData);
            itemData = new ItemData("Be�adik");
            itemData.AddInternationalDayInfo("Svetov� de� vody");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� planet�ri�");
            collection.Add(new ItemDate(3, 22), itemData);
            itemData = new ItemData("Adri�n");
            itemData.AddInternationalDayInfo("Svetov� de� meteorol�gie (WMO)");
            collection.Add(new ItemDate(3, 23), itemData);
            itemData = new ItemData("Gabriel");
            itemData.AddInternationalDayInfo("Svetov� de� tuberkul�zy (WHO)");
            collection.Add(new ItemDate(3, 24), itemData);
            itemData = new ItemData("Mari�n");
            itemData.AddInternationalDayInfo("De� z�pasu za �udsk� pr�va");
            collection.Add(new ItemDate(3, 25), itemData);
            itemData = new ItemData("Emanuel");
            collection.Add(new ItemDate(3, 26), itemData);
            itemData = new ItemData("Alena");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� divadla");
            collection.Add(new ItemDate(3, 27), itemData);
            itemData = new ItemData("So�a");
            collection.Add(new ItemDate(3, 28), itemData);
            itemData = new ItemData("Miroslav");
            collection.Add(new ItemDate(3, 29), itemData);
            itemData = new ItemData("Vieroslava");
            collection.Add(new ItemDate(3, 30), itemData);
            itemData = new ItemData("Benjam�n");
            collection.Add(new ItemDate(3, 31), itemData);
        }
        /// <summary>
        /// Inicializuje data pre april
        /// </summary>
        /// <param name="collection">Kolekcia do ktorej chceme data vlozit</param>
        private static void InitializeItemCollectionApril(ref Dictionary<ItemDate, ItemData> collection)
        {
            //pomocne premenne
            ItemData itemData = null;

            itemData = new ItemData("Hugo");
            collection.Add(new ItemDate(4, 1), itemData);
            itemData = new ItemData("Zita");
            collection.Add(new ItemDate(4, 2), itemData);
            itemData = new ItemData("Richard");
            collection.Add(new ItemDate(4, 3), itemData);
            itemData = new ItemData("Izidor");
            collection.Add(new ItemDate(4, 4), itemData);
            itemData = new ItemData("Miroslava");
            collection.Add(new ItemDate(4, 5), itemData);
            itemData = new ItemData("Irena");
            collection.Add(new ItemDate(4, 6), itemData);
            itemData = new ItemData("Zolt�n");
            itemData.AddInternationalDayInfo("Svetov� de� zdravia (WHO)");
            collection.Add(new ItemDate(4, 7), itemData);
            itemData = new ItemData("Albert");
            collection.Add(new ItemDate(4, 8), itemData);
            itemData = new ItemData("Milena");
            collection.Add(new ItemDate(4, 9), itemData);
            itemData = new ItemData("Igor");
            collection.Add(new ItemDate(4, 10), itemData);
            itemData = new ItemData("J�lius");
            collection.Add(new ItemDate(4, 11), itemData);
            itemData = new ItemData("Estera");
            collection.Add(new ItemDate(4, 12), itemData);
            itemData = new ItemData("Ale�");
            collection.Add(new ItemDate(4, 13), itemData);
            itemData = new ItemData("Just�na");
            collection.Add(new ItemDate(4, 14), itemData);
            itemData = new ItemData("Fedor");
            collection.Add(new ItemDate(4, 15), itemData);
            itemData = new ItemData("Dana");
            collection.Add(new ItemDate(4, 16), itemData);
            itemData = new ItemData("Rudolf");
            collection.Add(new ItemDate(4, 17), itemData);
            itemData = new ItemData("Val�r");
            collection.Add(new ItemDate(4, 18), itemData);
            itemData = new ItemData("Jela");
            collection.Add(new ItemDate(4, 19), itemData);
            itemData = new ItemData("Marcel");
            collection.Add(new ItemDate(4, 20), itemData);
            itemData = new ItemData("Erv�n");
            collection.Add(new ItemDate(4, 21), itemData);
            itemData = new ItemData("Slavom�r");
            collection.Add(new ItemDate(4, 22), itemData);
            itemData = new ItemData("Vojtech");
            itemData.AddInternationalDayInfo("Svetov� de� kn�h a autorsk�ch pr�v (UNESCO)");
            collection.Add(new ItemDate(4, 23), itemData);
            itemData = new ItemData("Juraj");
            collection.Add(new ItemDate(4, 24), itemData);
            itemData = new ItemData("Marek");
            collection.Add(new ItemDate(4, 25), itemData);
            itemData = new ItemData("Jaroslava");
            collection.Add(new ItemDate(4, 26), itemData);
            itemData = new ItemData("Jaroslav");
            collection.Add(new ItemDate(4, 27), itemData);
            itemData = new ItemData("Jarmila");
            collection.Add(new ItemDate(4, 28), itemData);
            itemData = new ItemData("Lea");
            collection.Add(new ItemDate(4, 29), itemData);
            itemData = new ItemData("Anast�zia");
            collection.Add(new ItemDate(4, 30), itemData);
        }
        /// <summary>
        /// Inicializuje data pre May
        /// </summary>
        /// <param name="collection">Kolekcia do ktorej chceme data vlozit</param>
        private static void InitializeItemCollectionMay(ref Dictionary<ItemDate, ItemData> collection)
        {
            //pomocne premenne
            ItemData itemData = null;

            itemData = new ItemData("-");
            collection.Add(new ItemDate(5, 1), itemData);
            itemData = new ItemData("�igmund");
            collection.Add(new ItemDate(5, 2), itemData);
            itemData = new ItemData("Galina");
            itemData.AddInternationalDayInfo("Svetov� de� slobody tla�e (UNESCO)");
            collection.Add(new ItemDate(5, 3), itemData);
            itemData = new ItemData("Flori�n");
            collection.Add(new ItemDate(5, 4), itemData);
            itemData = new ItemData("Lesana");
            collection.Add(new ItemDate(5, 5), itemData);
            itemData = new ItemData("Herm�na");
            collection.Add(new ItemDate(5, 6), itemData);
            itemData = new ItemData("Monika");
            collection.Add(new ItemDate(5, 7), itemData);
            itemData = new ItemData("Ingrida");
            itemData.AddInternationalDayInfo("De� matiek");
            collection.Add(new ItemDate(5, 8), itemData);
            itemData = new ItemData("Roland");
            collection.Add(new ItemDate(5, 9), itemData);
            itemData = new ItemData("Vikt�ria");
            collection.Add(new ItemDate(5, 10), itemData);
            itemData = new ItemData("Bla�ena");
            collection.Add(new ItemDate(5, 11), itemData);
            itemData = new ItemData("Pankr�c");
            collection.Add(new ItemDate(5, 12), itemData);
            itemData = new ItemData("Serv�c");
            collection.Add(new ItemDate(5, 13), itemData);
            itemData = new ItemData("Bonif�c");
            collection.Add(new ItemDate(5, 14), itemData);
            itemData = new ItemData("�ofia");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� rodiny");
            collection.Add(new ItemDate(5, 15), itemData);
            itemData = new ItemData("Svetoz�r");
            collection.Add(new ItemDate(5, 16), itemData);
            itemData = new ItemData("Gizela");
            itemData.AddInternationalDayInfo("Svetov� de� telekomunik�ci� (ITU)");
            collection.Add(new ItemDate(5, 17), itemData);
            itemData = new ItemData("Viola");
            collection.Add(new ItemDate(5, 18), itemData);
            itemData = new ItemData("Gertr�da");
            collection.Add(new ItemDate(5, 19), itemData);
            itemData = new ItemData("Bernard");
            collection.Add(new ItemDate(5, 20), itemData);
            itemData = new ItemData("Zina");
            itemData.AddInternationalDayInfo("Svetov� de� kult�rnej diverzity pre dial�g a rozvoj");
            collection.Add(new ItemDate(5, 21), itemData);
            itemData = new ItemData("J�lia, Juliana");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� biologickej diverzity");
            collection.Add(new ItemDate(5, 22), itemData);
            itemData = new ItemData("�elm�ra");
            collection.Add(new ItemDate(5, 23), itemData);
            itemData = new ItemData("Ela");
            collection.Add(new ItemDate(5, 24), itemData);
            itemData = new ItemData("Urban");
            itemData.AddInternationalDayInfo("De� Afriky");
            collection.Add(new ItemDate(5, 25), itemData);
            itemData = new ItemData("Du�an");
            collection.Add(new ItemDate(5, 26), itemData);
            itemData = new ItemData("Iveta");
            collection.Add(new ItemDate(5, 27), itemData);
            itemData = new ItemData("Viliam");
            collection.Add(new ItemDate(5, 28), itemData);
            itemData = new ItemData("Vilma");
            collection.Add(new ItemDate(5, 29), itemData);
            itemData = new ItemData("Ferdinand");
            collection.Add(new ItemDate(5, 30), itemData);
            itemData = new ItemData("Petronela");
            itemData.AddInternationalDayInfo("Svetov� de� bez tabaku (WHO)");
            collection.Add(new ItemDate(5, 31), itemData);
        }
        /// <summary>
        /// Inicializuje data pre June
        /// </summary>
        /// <param name="collection">Kolekcia do ktorej chceme data vlozit</param>
        private static void InitializeItemCollectionJune(ref Dictionary<ItemDate, ItemData> collection)
        {
            //pomocne premenne
            ItemData itemData = null;

            itemData = new ItemData("�aneta");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� det�");
            collection.Add(new ItemDate(6, 1), itemData);
            itemData = new ItemData("X�nia");
            collection.Add(new ItemDate(6, 2), itemData);
            itemData = new ItemData("Karol�na");
            collection.Add(new ItemDate(6, 3), itemData);
            itemData = new ItemData("Lenka");
            collection.Add(new ItemDate(6, 4), itemData);
            itemData = new ItemData("Laura");
            itemData.AddInternationalDayInfo("Svetov� de� �ivotn�ho prostredia (UNEP)");
            collection.Add(new ItemDate(6, 5), itemData);
            itemData = new ItemData("Norbert");
            collection.Add(new ItemDate(6, 6), itemData);
            itemData = new ItemData("R�bert");
            collection.Add(new ItemDate(6, 7), itemData);
            itemData = new ItemData("Medard");
            collection.Add(new ItemDate(6, 8), itemData);
            itemData = new ItemData("Stanislava");
            collection.Add(new ItemDate(6, 9), itemData);
            itemData = new ItemData("Margar�ta");
            collection.Add(new ItemDate(6, 10), itemData);
            itemData = new ItemData("Dobroslava");
            collection.Add(new ItemDate(6, 11), itemData);
            itemData = new ItemData("Zlatko");
            collection.Add(new ItemDate(6, 12), itemData);
            itemData = new ItemData("Anton");
            collection.Add(new ItemDate(6, 13), itemData);
            itemData = new ItemData("Vasil");
            collection.Add(new ItemDate(6, 14), itemData);
            itemData = new ItemData("V�t");
            collection.Add(new ItemDate(6, 15), itemData);
            itemData = new ItemData("Blanka");
            collection.Add(new ItemDate(6, 16), itemData);
            itemData = new ItemData("Adolf");
            itemData.AddInternationalDayInfo("Svetov� de� boja proti roz�irovaniu p��t� a sucha");
            collection.Add(new ItemDate(6, 17), itemData);
            itemData = new ItemData("Vratislav");
            collection.Add(new ItemDate(6, 18), itemData);
            itemData = new ItemData("Alfr�d");
            itemData.AddInternationalDayInfo("De� otcov");
            collection.Add(new ItemDate(6, 19), itemData);
            itemData = new ItemData("Val�ria");
            itemData.AddInternationalDayInfo("Svetov� de� ute�encov");
            collection.Add(new ItemDate(6, 20), itemData);
            itemData = new ItemData("Alojz");
            collection.Add(new ItemDate(6, 21), itemData);
            itemData = new ItemData("Paul�na");
            collection.Add(new ItemDate(6, 22), itemData);
            itemData = new ItemData("Sid�nia");
            collection.Add(new ItemDate(6, 23), itemData);
            itemData = new ItemData("J�n");
            collection.Add(new ItemDate(6, 24), itemData);
            itemData = new ItemData("Tade�");
            collection.Add(new ItemDate(6, 25), itemData);
            itemData = new ItemData("Adri�na");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� OSN za podporu obet� t�rania");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� proti zneu��vaniu drog a nez�konn�mu obchodovaniu");
            collection.Add(new ItemDate(6, 26), itemData);
            itemData = new ItemData("Ladislav");
            collection.Add(new ItemDate(6, 27), itemData);
            itemData = new ItemData("Be�ta");
            collection.Add(new ItemDate(6, 28), itemData);
            itemData = new ItemData("Peter, Pavol, Petra");
            collection.Add(new ItemDate(6, 29), itemData);
            itemData = new ItemData("Mel�nia");
            collection.Add(new ItemDate(6, 30), itemData);
        }
        /// <summary>
        /// Inicializuje data pre July
        /// </summary>
        /// <param name="collection">Kolekcia do ktorej chceme data vlozit</param>
        private static void InitializeItemCollectionJuly(ref Dictionary<ItemDate, ItemData> collection)
        {
            //pomocne premenne
            ItemData itemData = null;

            itemData = new ItemData("Diana");
            collection.Add(new ItemDate(7, 1), itemData);
            itemData = new ItemData("Berta");
            collection.Add(new ItemDate(7, 2), itemData);
            itemData = new ItemData("Miloslav");
            collection.Add(new ItemDate(7, 3), itemData);
            itemData = new ItemData("Prokop");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� kooperat�vov");
            collection.Add(new ItemDate(7, 4), itemData);
            itemData = new ItemData("-");
            collection.Add(new ItemDate(7, 5), itemData);
            itemData = new ItemData("Patrik, Patr�cia");
            collection.Add(new ItemDate(7, 6), itemData);
            itemData = new ItemData("Oliver");
            collection.Add(new ItemDate(7, 7), itemData);
            itemData = new ItemData("Ivan");
            collection.Add(new ItemDate(7, 8), itemData);
            itemData = new ItemData("Lujza");
            collection.Add(new ItemDate(7, 9), itemData);
            itemData = new ItemData("Am�lia");
            collection.Add(new ItemDate(7, 10), itemData);
            itemData = new ItemData("Milota");
            itemData.AddInternationalDayInfo("Svetov� de� popul�cie (UNFPA)");
            collection.Add(new ItemDate(7, 11), itemData);
            itemData = new ItemData("Nina");
            collection.Add(new ItemDate(7, 12), itemData);
            itemData = new ItemData("Margita");
            collection.Add(new ItemDate(7, 13), itemData);
            itemData = new ItemData("Kamil");
            collection.Add(new ItemDate(7, 14), itemData);
            itemData = new ItemData("Henrich");
            collection.Add(new ItemDate(7, 15), itemData);
            itemData = new ItemData("Drahom�r");
            collection.Add(new ItemDate(7, 16), itemData);
            itemData = new ItemData("Bohuslav");
            collection.Add(new ItemDate(7, 17), itemData);
            itemData = new ItemData("Kamila");
            collection.Add(new ItemDate(7, 18), itemData);
            itemData = new ItemData("Du�ana");
            collection.Add(new ItemDate(7, 19), itemData);
            itemData = new ItemData("I�ja");
            collection.Add(new ItemDate(7, 20), itemData);
            itemData = new ItemData("Daniel");
            collection.Add(new ItemDate(7, 21), itemData);
            itemData = new ItemData("Magdal�na");
            collection.Add(new ItemDate(7, 22), itemData);
            itemData = new ItemData("O�ga");
            collection.Add(new ItemDate(7, 23), itemData);
            itemData = new ItemData("Vladim�r");
            collection.Add(new ItemDate(7, 24), itemData);
            itemData = new ItemData("Jakub");
            collection.Add(new ItemDate(7, 25), itemData);
            itemData = new ItemData("Anna");
            collection.Add(new ItemDate(7, 26), itemData);
            itemData = new ItemData("Bo�ena");
            collection.Add(new ItemDate(7, 27), itemData);
            itemData = new ItemData("Kri�tof");
            collection.Add(new ItemDate(7, 28), itemData);
            itemData = new ItemData("Marta");
            collection.Add(new ItemDate(7, 29), itemData);
            itemData = new ItemData("Libu�a");
            collection.Add(new ItemDate(7, 30), itemData);
            itemData = new ItemData("Ign�c");
            collection.Add(new ItemDate(7, 31), itemData);
        }
        /// <summary>
        /// Inicializuje data pre August
        /// </summary>
        /// <param name="collection">Kolekcia do ktorej chceme data vlozit</param>
        private static void InitializeItemCollectionAugust(ref Dictionary<ItemDate, ItemData> collection)
        {
            //pomocne premenne
            ItemData itemData = null;

            itemData = new ItemData("Bo�idara");
            collection.Add(new ItemDate(8, 1), itemData);
            itemData = new ItemData("Gust�v");
            collection.Add(new ItemDate(8, 2), itemData);
            itemData = new ItemData("Jergu�");
            collection.Add(new ItemDate(8, 3), itemData);
            itemData = new ItemData("Dominik");
            collection.Add(new ItemDate(8, 4), itemData);
            itemData = new ItemData("Hortenzia");
            collection.Add(new ItemDate(8, 5), itemData);
            itemData = new ItemData("Jozef�na");
            collection.Add(new ItemDate(8, 6), itemData);
            itemData = new ItemData("�tef�nia");
            collection.Add(new ItemDate(8, 7), itemData);
            itemData = new ItemData("Oskar");
            collection.Add(new ItemDate(8, 8), itemData);
            itemData = new ItemData("�ubom�ra");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� domorod�ch �ud�");
            collection.Add(new ItemDate(8, 9), itemData);
            itemData = new ItemData("Vavrinec");
            collection.Add(new ItemDate(8, 10), itemData);
            itemData = new ItemData("Zuzana");
            collection.Add(new ItemDate(8, 11), itemData);
            itemData = new ItemData("Darina");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� ml�de�e");
            collection.Add(new ItemDate(8, 12), itemData);
            itemData = new ItemData("�ubom�r");
            collection.Add(new ItemDate(8, 13), itemData);
            itemData = new ItemData("Mojm�r");
            collection.Add(new ItemDate(8, 14), itemData);
            itemData = new ItemData("Marcela");
            collection.Add(new ItemDate(8, 15), itemData);
            itemData = new ItemData("Leonard");
            collection.Add(new ItemDate(8, 16), itemData);
            itemData = new ItemData("Milica");
            collection.Add(new ItemDate(8, 17), itemData);
            itemData = new ItemData("Elena");
            collection.Add(new ItemDate(8, 18), itemData);
            itemData = new ItemData("L�dia");
            collection.Add(new ItemDate(8, 19), itemData);
            itemData = new ItemData("Anabela");
            collection.Add(new ItemDate(8, 20), itemData);
            itemData = new ItemData("Jana");
            collection.Add(new ItemDate(8, 21), itemData);
            itemData = new ItemData("Tichom�r");
            collection.Add(new ItemDate(8, 22), itemData);
            itemData = new ItemData("Filip");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� spomienky na obchod s otrokmi a jeho zru�enie (UNESCO)");
            collection.Add(new ItemDate(8, 23), itemData);
            itemData = new ItemData("Bartolomej");
            collection.Add(new ItemDate(8, 24), itemData);
            itemData = new ItemData("�udov�t");
            collection.Add(new ItemDate(8, 25), itemData);
            itemData = new ItemData("Samuel");
            collection.Add(new ItemDate(8, 26), itemData);
            itemData = new ItemData("Silvia");
            collection.Add(new ItemDate(8, 27), itemData);
            itemData = new ItemData("August�n");
            collection.Add(new ItemDate(8, 28), itemData);
            itemData = new ItemData("Nikola");
            collection.Add(new ItemDate(8, 29), itemData);
            itemData = new ItemData("Ru�ena");
            collection.Add(new ItemDate(8, 30), itemData);
            itemData = new ItemData("Nora");
            collection.Add(new ItemDate(8, 31), itemData);
        }
        /// <summary>
        /// Inicializuje data pre September
        /// </summary>
        /// <param name="collection">Kolekcia do ktorej chceme data vlozit</param>
        private static void InitializeItemCollectionSeptember(ref Dictionary<ItemDate, ItemData> collection)
        {
            //pomocne premenne
            ItemData itemData = null;

            itemData = new ItemData("Drahoslava");
            collection.Add(new ItemDate(9, 1), itemData);
            itemData = new ItemData("Linda");
            collection.Add(new ItemDate(9, 2), itemData);
            itemData = new ItemData("Belo");
            collection.Add(new ItemDate(9, 3), itemData);
            itemData = new ItemData("Roz�lia");
            collection.Add(new ItemDate(9, 4), itemData);
            itemData = new ItemData("Reg�na");
            collection.Add(new ItemDate(9, 5), itemData);
            itemData = new ItemData("Alica");
            collection.Add(new ItemDate(9, 6), itemData);
            itemData = new ItemData("Marianna");
            collection.Add(new ItemDate(9, 7), itemData);
            itemData = new ItemData("Miriama");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� gramotnosti (UNESCO)");
            collection.Add(new ItemDate(9, 8), itemData);
            itemData = new ItemData("Martina");
            collection.Add(new ItemDate(9, 9), itemData);
            itemData = new ItemData("Oleg");
            collection.Add(new ItemDate(9, 10), itemData);
            itemData = new ItemData("Bystr�k");
            collection.Add(new ItemDate(9, 11), itemData);
            itemData = new ItemData("M�ria");
            collection.Add(new ItemDate(9, 12), itemData);
            itemData = new ItemData("Ctibor");
            collection.Add(new ItemDate(9, 13), itemData);
            itemData = new ItemData("�udomil");
            collection.Add(new ItemDate(9, 14), itemData);
            itemData = new ItemData("Jolana");
            collection.Add(new ItemDate(9, 15), itemData);
            itemData = new ItemData("�udmila, Ludomila");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� ochrany oz�novej vrstvy");
            collection.Add(new ItemDate(9, 16), itemData);
            itemData = new ItemData("Olympia");
            collection.Add(new ItemDate(9, 17), itemData);
            itemData = new ItemData("Eug�nia");
            collection.Add(new ItemDate(9, 18), itemData);
            itemData = new ItemData("Kon�tant�n");
            collection.Add(new ItemDate(9, 19), itemData);
            itemData = new ItemData("�uboslava, �uboslav");
            collection.Add(new ItemDate(9, 20), itemData);
            itemData = new ItemData("Mat��");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� mieru");
            collection.Add(new ItemDate(9, 21), itemData);
            itemData = new ItemData("M�ric");
            collection.Add(new ItemDate(9, 22), itemData);
            itemData = new ItemData("Zdenka");
            collection.Add(new ItemDate(9, 23), itemData);
            itemData = new ItemData("�ubor, �ubo�");
            collection.Add(new ItemDate(9, 24), itemData);
            itemData = new ItemData("Vladislav");
            collection.Add(new ItemDate(9, 25), itemData);
            itemData = new ItemData("Edita");
            collection.Add(new ItemDate(9, 26), itemData);
            itemData = new ItemData("Cypri�n");
            collection.Add(new ItemDate(9, 27), itemData);
            itemData = new ItemData("V�clav");
            itemData.AddInternationalDayInfo("Medzin�rodn� n�morn� de� (IMO)");
            collection.Add(new ItemDate(9, 28), itemData);
            itemData = new ItemData("Michaela, Michal");
            itemData.AddInternationalDayInfo("Medzin�rodn� n�morn� de� (IMO)");
            collection.Add(new ItemDate(9, 29), itemData);
            itemData = new ItemData("Jarol�m");
            itemData.AddInternationalDayInfo("Medzin�rodn� n�morn� de� (IMO)");
            collection.Add(new ItemDate(9, 30), itemData);
        }
        /// <summary>
        /// Inicializuje data pre October
        /// </summary>
        /// <param name="collection">Kolekcia do ktorej chceme data vlozit</param>
        private static void InitializeItemCollectionOctober(ref Dictionary<ItemDate, ItemData> collection)
        {
            //pomocne premenne
            ItemData itemData = null;

            itemData = new ItemData("Arnold");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� star��ch os�b");
            collection.Add(new ItemDate(10, 1), itemData);
            itemData = new ItemData("Levoslav");
            collection.Add(new ItemDate(10, 2), itemData);
            itemData = new ItemData("Stela");
            collection.Add(new ItemDate(10, 3), itemData);
            itemData = new ItemData("Franti�ek");
            collection.Add(new ItemDate(10, 4), itemData);
            itemData = new ItemData("Viera");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� u�ite�ov (UNESCO)");
            itemData.AddInternationalDayInfo("Svetov� de� �udsk�ch obydl�");
            collection.Add(new ItemDate(10, 5), itemData);
            itemData = new ItemData("Nat�lia");
            collection.Add(new ItemDate(10, 6), itemData);
            itemData = new ItemData("Eli�ka");
            collection.Add(new ItemDate(10, 7), itemData);
            itemData = new ItemData("Brigita");
            collection.Add(new ItemDate(10, 8), itemData);
            itemData = new ItemData("Dion�z");
            itemData.AddInternationalDayInfo("Svetov� de� po�ty (UPU)");
            collection.Add(new ItemDate(10, 9), itemData);
            itemData = new ItemData("Slavom�ra");
            itemData.AddInternationalDayInfo("Svetov� de� du�evn�ho zdravia");
            collection.Add(new ItemDate(10, 10), itemData);
            itemData = new ItemData("Valent�na");
            collection.Add(new ItemDate(10, 11), itemData);
            itemData = new ItemData("Maxmili�n");
            collection.Add(new ItemDate(10, 12), itemData);
            itemData = new ItemData("Koloman");
            collection.Add(new ItemDate(10, 13), itemData);
            itemData = new ItemData("Boris");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� redukcie pr�rodn�ch katastrof");
            collection.Add(new ItemDate(10, 14), itemData);
            itemData = new ItemData("Ter�zia");
            collection.Add(new ItemDate(10, 15), itemData);
            itemData = new ItemData("Vladim�ra");
            itemData.AddInternationalDayInfo("Svetov� de� potravy (FAO)");
            collection.Add(new ItemDate(10, 16), itemData);
            itemData = new ItemData("Hedviga");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� boja proti chudobe");
            collection.Add(new ItemDate(10, 17), itemData);
            itemData = new ItemData("Luk�");
            collection.Add(new ItemDate(10, 18), itemData);
            itemData = new ItemData("Kristi�n");
            collection.Add(new ItemDate(10, 19), itemData);
            itemData = new ItemData("Vendel�n");
            collection.Add(new ItemDate(10, 20), itemData);
            itemData = new ItemData("Ur�u�a");
            collection.Add(new ItemDate(10, 21), itemData);
            itemData = new ItemData("Sergej");
            collection.Add(new ItemDate(10, 22), itemData);
            itemData = new ItemData("Alojza");
            collection.Add(new ItemDate(10, 23), itemData);
            itemData = new ItemData("Kvetoslava");
            itemData.AddInternationalDayInfo("Svetov� de� informovanosti o rozvoji");
            itemData.AddInternationalDayInfo("De� OSN");
            collection.Add(new ItemDate(10, 24), itemData);
            itemData = new ItemData("Aurel");
            collection.Add(new ItemDate(10, 25), itemData);
            itemData = new ItemData("Demeter");
            collection.Add(new ItemDate(10, 26), itemData);
            itemData = new ItemData("Sab�na");
            collection.Add(new ItemDate(10, 27), itemData);
            itemData = new ItemData("Dobromila");
            collection.Add(new ItemDate(10, 28), itemData);
            itemData = new ItemData("Kl�ra");
            collection.Add(new ItemDate(10, 29), itemData);
            itemData = new ItemData("Simona");
            collection.Add(new ItemDate(10, 30), itemData);
            itemData = new ItemData("Aur�lia");
            collection.Add(new ItemDate(10, 31), itemData);
        }
        /// <summary>
        /// Inicializuje data pre November
        /// </summary>
        /// <param name="collection">Kolekcia do ktorej chceme data vlozit</param>
        private static void InitializeItemCollectionNovember(ref Dictionary<ItemDate, ItemData> collection)
        {
            //pomocne premenne
            ItemData itemData = null;

            itemData = new ItemData("Denisa");
            collection.Add(new ItemDate(11, 1), itemData);
            itemData = new ItemData("-");
            collection.Add(new ItemDate(11, 2), itemData);
            itemData = new ItemData("Hubert");
            collection.Add(new ItemDate(11, 3), itemData);
            itemData = new ItemData("Karol");
            collection.Add(new ItemDate(11, 4), itemData);
            itemData = new ItemData("Imrich");
            collection.Add(new ItemDate(11, 5), itemData);
            itemData = new ItemData("Ren�ta");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� ochrany �ivotn�ho prostredia pred ni�en�m po�as vojny a ozbrojen�ho konfliktu");
            collection.Add(new ItemDate(11, 6), itemData);
            itemData = new ItemData("Ren�");
            collection.Add(new ItemDate(11, 7), itemData);
            itemData = new ItemData("Bohum�r");
            collection.Add(new ItemDate(11, 8), itemData);
            itemData = new ItemData("Teodor");
            collection.Add(new ItemDate(11, 9), itemData);
            itemData = new ItemData("Tibor");
            itemData.AddInternationalDayInfo("Svetov� de� vedy pre mier a rozvoj (UNESCO)");
            collection.Add(new ItemDate(11, 10), itemData);
            itemData = new ItemData("Martin");
            collection.Add(new ItemDate(11, 11), itemData);
            itemData = new ItemData("Sv�topluk");
            collection.Add(new ItemDate(11, 12), itemData);
            itemData = new ItemData("Stanislav");
            collection.Add(new ItemDate(11, 13), itemData);
            itemData = new ItemData("Irma");
            collection.Add(new ItemDate(11, 14), itemData);
            itemData = new ItemData("Leopold");
            collection.Add(new ItemDate(11, 15), itemData);
            itemData = new ItemData("Agnesa");
            collection.Add(new ItemDate(11, 16), itemData);
            itemData = new ItemData("Klaudia");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� tolerancie (UNESCO)");
            collection.Add(new ItemDate(11, 17), itemData);
            itemData = new ItemData("Eugen");
            collection.Add(new ItemDate(11, 18), itemData);
            itemData = new ItemData("Al�b�ta");
            collection.Add(new ItemDate(11, 19), itemData);
            itemData = new ItemData("F�lix");
            itemData.AddInternationalDayInfo("Svetov� de� det� (UNICEF)");
            itemData.AddInternationalDayInfo("De� spriemyselnenia Afriky");
            collection.Add(new ItemDate(11, 20), itemData);
            itemData = new ItemData("Elv�ra");
            itemData.AddInternationalDayInfo("Svetov� de� telev�zie");
            itemData.AddInternationalDayInfo("De� filozofie v UNESCO");
            collection.Add(new ItemDate(11, 21), itemData);
            itemData = new ItemData("Cec�lia");
            collection.Add(new ItemDate(11, 22), itemData);
            itemData = new ItemData("Klement");
            collection.Add(new ItemDate(11, 23), itemData);
            itemData = new ItemData("Em�lia");
            collection.Add(new ItemDate(11, 24), itemData);
            itemData = new ItemData("Katar�na");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� elimin�cie n�silia vo�i �en�m");
            collection.Add(new ItemDate(11, 25), itemData);
            itemData = new ItemData("Kornel");
            collection.Add(new ItemDate(11, 26), itemData);
            itemData = new ItemData("Milan");
            collection.Add(new ItemDate(11, 27), itemData);
            itemData = new ItemData("Henrieta");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� solidarity s �u�mi Palest�ny");
            collection.Add(new ItemDate(11, 28), itemData);
            itemData = new ItemData("Vratko");
            collection.Add(new ItemDate(11, 29), itemData);
            itemData = new ItemData("Ondrej, Andrej");
            collection.Add(new ItemDate(11, 30), itemData);
        }
        /// <summary>
        /// Inicializuje data pre December
        /// </summary>
        /// <param name="collection">Kolekcia do ktorej chceme data vlozit</param>
        private static void InitializeItemCollectionDecember(ref Dictionary<ItemDate, ItemData> collection)
        {
            //pomocne premenne
            ItemData itemData = null;

            itemData = new ItemData("Edmund");
            itemData.AddInternationalDayInfo("Svetov� de� boja proti AIDS (WHO)");
            collection.Add(new ItemDate(12, 1), itemData);
            itemData = new ItemData("Bibi�na");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� zru�enia otroctva");
            collection.Add(new ItemDate(12, 2), itemData);
            itemData = new ItemData("Oldrich");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� postihnut�ch");
            collection.Add(new ItemDate(12, 3), itemData);
            itemData = new ItemData("Barbora");
            collection.Add(new ItemDate(12, 4), itemData);
            itemData = new ItemData("Oto");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� dobrovo�n�kov v ekonomickom a soci�lnom rozvoji");
            collection.Add(new ItemDate(12, 5), itemData);
            itemData = new ItemData("Mikul�");
            collection.Add(new ItemDate(12, 6), itemData);
            itemData = new ItemData("Ambr�z");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� civilnej leteckej prepravy (ICAO)");
            collection.Add(new ItemDate(12, 7), itemData);
            itemData = new ItemData("Mar�na");
            collection.Add(new ItemDate(12, 8), itemData);
            itemData = new ItemData("Izabela");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� boja proti korupcii");
            collection.Add(new ItemDate(12, 9), itemData);
            itemData = new ItemData("Rad�z");
            itemData.AddInternationalDayInfo("De� �udsk�ch pr�v");
            collection.Add(new ItemDate(12, 10), itemData);
            itemData = new ItemData("Hilda");
            collection.Add(new ItemDate(12, 11), itemData);
            itemData = new ItemData("Ot�lia");
            collection.Add(new ItemDate(12, 12), itemData);
            itemData = new ItemData("Lucia");
            collection.Add(new ItemDate(12, 13), itemData);
            itemData = new ItemData("Branislava");
            collection.Add(new ItemDate(12, 14), itemData);
            itemData = new ItemData("Ivica");
            collection.Add(new ItemDate(12, 15), itemData);
            itemData = new ItemData("Alb�na");
            collection.Add(new ItemDate(12, 16), itemData);
            itemData = new ItemData("Korn�lia");
            collection.Add(new ItemDate(12, 17), itemData);
            itemData = new ItemData("Sl�va");
            itemData.AddInternationalDayInfo("Medzin�rodn� de� migrantov");
            collection.Add(new ItemDate(12, 18), itemData);
            itemData = new ItemData("Judita");
            collection.Add(new ItemDate(12, 19), itemData);
            itemData = new ItemData("Dagmara");
            collection.Add(new ItemDate(12, 20), itemData);
            itemData = new ItemData("Bohdan");
            collection.Add(new ItemDate(12, 21), itemData);
            itemData = new ItemData("Adela");
            collection.Add(new ItemDate(12, 22), itemData);
            itemData = new ItemData("Nade�da");
            collection.Add(new ItemDate(12, 23), itemData);
            itemData = new ItemData("Adam, Eva");
            collection.Add(new ItemDate(12, 24), itemData);
            itemData = new ItemData("-");
            collection.Add(new ItemDate(12, 25), itemData);
            itemData = new ItemData("�tefan");
            collection.Add(new ItemDate(12, 26), itemData);
            itemData = new ItemData("Filom�na");
            collection.Add(new ItemDate(12, 27), itemData);
            itemData = new ItemData("Ivana");
            collection.Add(new ItemDate(12, 28), itemData);
            itemData = new ItemData("Milada");
            collection.Add(new ItemDate(12, 29), itemData);
            itemData = new ItemData("D�vid");
            collection.Add(new ItemDate(12, 30), itemData);
            itemData = new ItemData("Silvester");
            collection.Add(new ItemDate(12, 31), itemData);
        }
        #endregion
    }
}