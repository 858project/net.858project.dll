using System;
using System.Threading;
using System.ComponentModel;

namespace Project858
{
	/// <summary>
	/// The result of an asynchronous operation.
	/// </summary>
	public class AsyncResult : IAsyncResult, IDisposable
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="callback">Calback na asynchronne oznamenie ukoncenia operacie</param>
        /// <param name="state">Objekt obsahujuci informacie o asynchronnej operacii</param>
        public AsyncResult(AsyncCallback callback, Object state)
        {
            this.asyncCallback = callback;
            this.state = state;
            this.manualResentEvent = new ManualResetEvent(false);
        }
        #endregion

        #region - Event -
        /// <summary>
        /// Event oznamujuci Dispose() objektu
        /// </summary>
        private event EventHandler disposed = null;
        /// <summary>
		/// Occurs when this instance is _disposed.
		/// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
		public event EventHandler Disposed
		{
			add {
                //je objekt _disposed ?
                if (this.isDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");
                
                this.disposed += value;
            }
			remove {
                //je objekt _disposed ?
                if (this.isDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");
                
                this.disposed -= value;
            }
		}
        #endregion

        #region - Properties - 
		/// <summary>
		/// (Get) Objekt obsahujuci informacie o asynchronnej operacii
		/// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
		public Object AsyncState
		{
			get {
                //je objekt _disposed ?
                if (this.isDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");
                
                return this.state;
            }
		}
		/// <summary>
		/// (Get) Handle sluziacia na cakanie na ukoncenie operacie
		/// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
		public WaitHandle AsyncWaitHandle
		{
			get {
                //je objekt _disposed ?
                if (this.isDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");
                
                return this.manualResentEvent;
            }
		}
		/// <summary>
		/// (Get) Vrati ci bola operacia dokoncena synchronne
		/// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
		public Boolean CompletedSynchronously
		{
			get {
                //je objekt _disposed ?
                if (this.isDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");
                
                return false;
            }
		}
		/// <summary>
		/// (Get) Indikacia ci bola operacia ukoncena
		/// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
		public Boolean IsCompleted
		{
			get {
                //je objekt _disposed ?
                if (this.isDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");
                
                return this.manualResentEvent.WaitOne(0, false);
            }
		}
        /// <summary>
		/// (Get) Definuje ci bol objektu Disposed()
		/// </summary>
		[Browsable(false)]
		public Boolean IsDisposed
		{
			get { return this.isDisposed; }
		}
		#endregion

        #region - Variable -
        /// <summary>
        /// Pomocny objekt definujuci ci doslo k Dispose() objektu
        /// </summary>
        private Boolean isDisposed = false;
        /// <summary>
        /// Calback na asynchronne oznamenie ukoncenia operacie
        /// </summary>
        private AsyncCallback asyncCallback = null;
        /// <summary>
        /// Objekt obsahujuci informacie o asynchronnej operacii
        /// </summary>
		private Object state = null;
        /// <summary>
        /// Manual Reset Event na cakanie ukoncenia operacie
        /// </summary>
		private ManualResetEvent manualResentEvent = null;
        #endregion

        #region - Method -
        /// <summary>
		/// Ukonci operaciu a vykona event s oznamenim
		/// </summary>
		public virtual void OnCompleted()
		{
            //ukoncime manualReset event
			this.manualResentEvent.Set();

            //ak je dostupny calback tak vykoname event
			if (this.asyncCallback != null)
			{
				this.asyncCallback(this);
			}
		}
        /// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		public void Dispose()
		{
			if (!this.isDisposed)
			{
				this.Dispose(true);
				System.GC.SuppressFinalize(this);
			}
		}
        /// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing">
        /// <c>true</c> 
        /// to release both managed and unmanaged resources; 
        /// <c>false</c> 
        /// to release only unmanaged resources.
        /// </param>
		protected virtual void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
                    //ukoncime manual reset event
					this.manualResentEvent.Close();
					this.manualResentEvent = null;

                    //zmazeme objekt s informaciami
					this.state = null;

                    //zmazeme calback na oznamenie ukoncenia operacie
					this.asyncCallback = null;

                    //event o Disposed()
                    this.OnDisposed(EventArgs.Empty);
				}
			}
			finally
			{
                //objekt bol Disposed()
				this.isDisposed = true;
			}
		}
        /// <summary>
        /// Vygeneruje event oznamujuci Disposed()
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected virtual void OnDisposed(EventArgs e)
        {
            EventHandler handler = disposed;

            if (handler != null)
            {
                //vykoname event
                handler(this, e);
                //zrusime handler
                handler = null;
            }
        }
        #endregion
	}
}
