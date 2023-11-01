using CitizenFX.Core;
using Dapper;
using MySqlConnector;
using System;
using System.Linq;

namespace Server.Database {
	class Test {

		public Test() {




		try {
		using (var connection = Connector.GetConnection()) {
		//connection.Open();

		Debug.WriteLine(connection.ConnectionString);
		Debug.WriteLine(connection.ToString());
		Debug.WriteLine(connection.Ping() ? "Pinged True" : "Pinged False");
		connection.Open();
		Debug.WriteLine(connection.Ping() ? "Pinged True" : "Pinged False");
		connection.ExecuteScalar("SELECT 1");

		connection.Close();
		}
		} catch (MySqlException ex) {
		// Handle MySQL-specific exceptions
		Console.WriteLine($"MySQL Exception: {ex.Message}");
		} catch (Exception ex) {
		// Handle general exceptions
		Console.WriteLine($"General Exception: {ex.Message}");
		Console.WriteLine(ex.ToString());
		}

		testConnection();
		}


		class Account {

			public int id { get; set; }
			public string license { get; set; }
			public string nick { get; set; }

			public Account(int id, string license, string nick) {
			this.id = id;
			this.license = license;
			this.nick = nick;
			}
		}


		public async void testConnection() {
		try {
		using (var connection = Connector.GetConnection()) {
		connection.Open();
		var result = await connection.QueryAsync<Account>("SELECT * FROM fivem_vg5.accounts;");
		if (result != null) {
		Account acc = result.FirstOrDefault(); // Get the first result if it exists
		if (acc != null) {
		Debug.WriteLine($"Id of loaded: {acc.id} Nick: {acc.nick}, License: {acc.license}");
		
		} else
			Debug.WriteLine("No records found.");
		} else
			Debug.WriteLine("Query result is null.");
		}

		} catch (Exception ex) {
		Debug.WriteLine($"Error: {ex.Message}");
		Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
		}
		}
		
}

	}

