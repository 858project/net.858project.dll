using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace Project858.Net
{
	/// <summary>
	/// EventArgs ako sucast eventu oznamujceho prijatie klienta
	/// </summary>
	public class TcpClientEventArgs : EventArgs
	{
		#region - Constructor -
		/// <summary>
		/// Initialize this class
		/// </summary>
		/// <param name="client">Client ktory bol prijaty serverom</param>
		public TcpClientEventArgs(TcpClient client)
		{
			//osetrenie vstupu
			if (client == null)
				throw new ArgumentNullException("client");

			//uchovame si klienta
			this._client = client;
		}
		#endregion

		#region - Properties -
		/// <summary>
		/// Klient ktory bol serverom akceptovany
		/// </summary>
		public TcpClient Client
		{
			get { return _client; }
		}
		#endregion

		#region - Variable -
		/// <summary>
		/// Klient ktory bol serverom akceptovany
		/// </summary>
		private TcpClient _client = null;
		#endregion
	}
}
