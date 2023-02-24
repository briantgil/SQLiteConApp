using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace HelloWorldSample
{
    class Program
    {
        static void Main()
        {
            using (var connection = new SqliteConnection("Data Source=hello.db"))
            {
                connection.Open();

                //create table and insert initial data
                var command = connection.CreateCommand();
                command.CommandText =
                @"
                    DROP TABLE IF EXISTS USER;
                    CREATE TABLE if not exists user (
                        id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        name TEXT NOT NULL
                    );
                    INSERT INTO user
                    VALUES (1, 'Brice'),
                           (2, 'Alexander'),
                           (3, 'Nate');
                ";
                command.ExecuteNonQuery();  //C:\Users\...\Projects\SQLiteConApp\SQLiteConApp\bin\Debug\net6.0

                //get a name from user to put into db
                Console.Write("Name: ");
                var name = Console.ReadLine();
                command.CommandText =
                @"
                    INSERT INTO user (name)
                    VALUES ($name)
                ";
                command.Parameters.AddWithValue("$name", name); //parameterized
                command.ExecuteNonQuery();

                //get and print last record
                command.CommandText =
                @"
                    SELECT last_insert_rowid()
                ";
                var newId = (long)command.ExecuteScalar();
                Console.WriteLine($"Your new user ID is {newId}.");
            }


            using (var connection = new SqliteConnection("Data Source=hello.db"))
            {
                //get query result
                Console.Write("User ID: ");
                var id1 = int.Parse(Console.ReadLine());

                Console.Write("User ID: ");
                var id2 = int.Parse(Console.ReadLine());

                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                @"
                    SELECT id, name
                    FROM user
                    WHERE id in ($id1, $id2)
                ";
                command.Parameters.AddWithValue("$id1", id1);  //parameterized
                command.Parameters.AddWithValue("$id2", id2);  //parameterized

                //loop through query result
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader.GetString(1);
                        Console.WriteLine($"Hello, {name}!");
                    }
                }
            }

            // Clean up
            //File.Delete("hello.db");
        }
    }
}

