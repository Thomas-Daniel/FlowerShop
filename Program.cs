using MySql.Data.MySqlClient;
using System;

namespace Fleuriste
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            new UI();

            /*

            // Chaîne de connexion à votre base de données
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=fleuriste;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string[] header = new string[] { "Identifiant Client", "Nom", "Prénom", "Téléphone", "Mail", "Adresse" };
            // Requête SQL pour sélectionner tous les tuples de votre table
            string query = "SELECT * FROM client";

            MySqlCommand command = new MySqlCommand(query, connection);

            MySqlDataReader reader = command.ExecuteReader();

            // Tableau pour stocker les données
            string[,] table = new string[reader.FieldCount, reader.VisibleFieldCount];

            // Récupération des en-têtes de colonnes
            for (int i = 0; i < reader.VisibleFieldCount; i++)
            {
                table[i, 0] = reader.GetName(i);
                Console.WriteLine(reader.GetName(i));
            }

            int row = 1;

            // Parcours des tuples de la requête
            while (reader.Read())
            {
                // Ajout des valeurs dans le tableau
                for (int i = 0; i < reader.VisibleFieldCount; i++)
                {
                    table[i, row] = reader.GetValue(i).ToString();
                    Console.WriteLine(reader.GetValue(i).ToString());
                }

                row++;
            }

            reader.Close();

            // Affichage du tableau dans la console
            for (int i = 0; i < reader.VisibleFieldCount; i++)
            {
                Console.Write($"{table[i, 0],-20}");
            }

            Console.WriteLine();

            for (int i = 0; i < reader.VisibleFieldCount; i++)
            {
                Console.Write($"{new string('-', 20),-20}");
            }

            Console.WriteLine();

            for (int j = 1; j < row; j++)
            {
                for (int i = 0; i < reader.VisibleFieldCount; i++)
                {
                    Console.Write($"{table[i, j],-20}");
                }

                Console.WriteLine();
            }

            Console.ReadLine();
            */
        }
    }
}
