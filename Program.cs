using ALE.ETLBox.ConnectionManager;
using ALE.ETLBox.DataFlow;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace ETLBoxIssue
{
	class Program
	{

		private static SQLiteConnectionManager _sourceConnection;
		private static SQLiteConnectionManager _destinationConnection;

		static void Main(string[] args)
		{

			/* Load Connection Strings from Settings */
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

			IConfigurationRoot configuration = builder.Build();


			var sqlLiteConnection = $"Data Source={Path.Combine(Directory.GetCurrentDirectory(), "testdb.db")}";

	
			/* In my real data, Source and Destination are 2 Seperate Databases within the same SQL 2012 Instance */
			_sourceConnection = new SQLiteConnectionManager(sqlLiteConnection);
			_destinationConnection = new SQLiteConnectionManager(sqlLiteConnection);

			Console.WriteLine($"Source: {_sourceConnection.ConnectionString}");
			Console.WriteLine($"Destination: {_destinationConnection.ConnectionString}");

			var nameSource = new DbSource<Name>(_sourceConnection, "Name");
			var personMerge = new DbMerge<People>(_destinationConnection, "People");


			var transform = new RowTransformation<Name, People>(d =>
			{
				Console.WriteLine(d.FIRST_NAME);
				return new People()
				{
					FirstName = d.FIRST_NAME,
					LastName = d.LAST_NAME,
					ExternalId = d.ID
				};
			});

			nameSource.LinkTo(transform);
			transform.LinkTo(personMerge);

			nameSource.Execute();

			personMerge.Wait();

			Console.WriteLine("Complete");
		}
	}
}
