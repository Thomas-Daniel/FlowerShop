using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Fleuriste
{
    class UI
    {
        string userAdmin = "root";
        string passwdAdmin = "root";
        MySqlConnection connection;
        int seuilAlert = 50;
        bool alert;
        string produitRupture;
        int quantiteRestante;
        string welcomeMessage;

        public UI()
        {
            ReadData();
            string connectionString = $"SERVER=localhost;PORT=3306;DATABASE=fleuriste;UID={userAdmin};PASSWORD={passwdAdmin};";
            this.connection = CreateConnection(connectionString);
            MenuPrincipal();
        }

        public static MySqlConnection CreateConnection(string connectionString)
        {
            try
            {
                MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();
                return connection;
            }
            catch (Exception e)
            {
                Console.WriteLine("ERREUR : Impossible de se connecter à la BDD. Assurez-vous que la BDD existe et que cette dernière est accessible");
                Console.ReadKey();
                Environment.Exit(1);
                return null;
            }
        }

        public void ReadData()
        {
            //string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            //this.seuilAlert = int.Parse(File.ReadAllText($"{path}\\data.txt"));

            string fullName = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "\\welcome.txt";
            this.welcomeMessage = File.ReadAllText(fullName);
        }


        /// <summary>
        /// Menu principal avec message de bienvenue et différentes options de manipulation d'image
        /// </summary>
        private void MenuPrincipal()
        {
            // Liste des différentes options sur le menu principal
            string[] mainOptions = { "1 - Consulter les clients", "2 - Consulter les produits", "3 - Consulter les commandes", "4 - Afficher les statistiques", "5 - Administrer la BD", "6 - Quitter" };
            int selectedIndex = 0;

            // Cache le curseur de l'utilisateur
            Console.CursorVisible = false;
            while (true)
            {
                Console.Clear();
                // Affiche le message de bienvenue contenu dans le fichier "welcome.txt"
                Console.WriteLine(this.welcomeMessage);

                // Afficher les options du menu
                for (int i = 0; i < mainOptions.Length; i++)
                {
                    // Surligne en vert l'option sélectionnée
                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    Console.WriteLine(mainOptions[i]);
                    Console.ResetColor();
                }

                // Capturer l'entrée de l'utilisateur
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                // Déplacer le curseur en fonction de l'entrée de l'utilisateur
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = Math.Max(0, selectedIndex - 1);
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = Math.Min(mainOptions.Length - 1, selectedIndex + 1);
                        break;
                    case ConsoleKey.Enter:
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Option sélectionnée : {mainOptions[selectedIndex]}\n");
                        Console.ResetColor();

                        // Action en fonction du choix utilisateur
                        string selection = mainOptions[selectedIndex];
                        switch (selection)
                        {
                            case "1 - Consulter les clients":
                                MenuClients();
                                break;

                            case "2 - Consulter les produits":
                                MenuProduits();
                                break;

                            case "3 - Consulter les commandes":
                                MenuCommandes();
                                break;

                            case "4 - Afficher les statistiques":
                                Stats();
                                break;

                            case "5 - Administrer la BD":
                                MenuAdmin();
                                break;

                            case "6 - Quitter":
                                this.connection.Close();
                                return;
                        }
                        MenuPrincipal();
                        this.connection.Close();
                        return;
                }
            }            
        }



        public void MenuClients()
        {
            // Liste des différentes options sur le menu principal
            string[] mainOptions = { "1 - Afficher tous les clients", "2 - Filtrer les clients", "3 - Retour" };
            int selectedIndex = 0;

            // Cache le curseur de l'utilisateur
            Console.CursorVisible = false;
            while (true)
            {
                Console.Clear();

                // Afficher les options du menu
                for (int i = 0; i < mainOptions.Length; i++)
                {
                    // Surligne en vert l'option sélectionnée
                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    Console.WriteLine(mainOptions[i]);
                    Console.ResetColor();
                }

                // Capturer l'entrée de l'utilisateur
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                // Déplacer le curseur en fonction de l'entrée de l'utilisateur
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = Math.Max(0, selectedIndex - 1);
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = Math.Min(mainOptions.Length - 1, selectedIndex + 1);
                        break;
                    case ConsoleKey.Enter:
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Option sélectionnée : {mainOptions[selectedIndex]}\n");
                        Console.ResetColor();

                        // Action en fonction du choix utilisateur
                        string selection = mainOptions[selectedIndex];
                        switch (selection)
                        {
                            case "1 - Afficher tous les clients":
                                Clients();                                
                                break;

                            case "2 - Filtrer les clients":
                                FiltrerClients();
                                break;

                            case "3 - Retour":
                                return;
                        }
                        Console.WriteLine("\n\nAppuyez sur ENTRER pour continuer");
                        Console.ReadKey();
                        MenuClients();
                        return;
                }
            }
        }


        public void Clients()
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText =
                "select distinct idC, prenom, nom, tel, mail, adresseFact, fidelite from client;";

            MySqlDataReader reader = command.ExecuteReader();

            string[] header = new string[] { "Numéro client", "Prénom", "Nom", "Téléphone", "Mail", "Adresse", "Fidélité" };
            string columnName = "| ";
            for (int i = 0; i < header.Length; i++)
            {
                columnName += $"{header[i]} | ";
            }
            columnName += "\n";
            Console.WriteLine(columnName);

            while (reader.Read())
            {
                string currentRowAsString = "| ";
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string valueAsString;
                    if (!reader.IsDBNull(i))
                    {
                        valueAsString = reader.GetValue(i).ToString();
                    }
                    else
                    {
                        valueAsString = "Null";
                    }
                    currentRowAsString += valueAsString + " | ";
                }
                Console.WriteLine(currentRowAsString);
            }
            reader.Close();
        }

        
        public void FiltrerClients()
        {
            Console.WriteLine("Vous allez être présenté aux différents attributs qui définissent un client.\nSi vous cherchez un client qui correspond à l'un de ces attributs, entrez la valeur correspondante que vous recherchez.");
            string[] attributs = new string[] { "idC", "prenom", "nom", "tel", "mail", "adresseFact", "fidelite" };
            string[] header = new string[] { "numéro client", "prénom", "nom", "téléphone", "mail", "adresse", "fidélité" };            

            MySqlCommand command = connection.CreateCommand();

            // Création de la requête avec des paramètres pour chaque colonne
            command.CommandText = "SELECT idC, prenom, nom, tel, mail, adresseFact, fidelite FROM client WHERE ";
            for (int i = 0; i < attributs.Length; i++)
            {
                Console.Write($"\nSaisissez la valeur pour {header[i]} : ");
                string value = Console.ReadLine();
                bool validInput;
                if (value != "")
                {                    
                    switch (attributs[i])
                    {
                        case "idC":
                            try
                            {
                                if (int.Parse(value) >= 0)
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                }
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }                            

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un entier positif valide");
                                value = Console.ReadLine();

                                try
                                {
                                    if (int.Parse(value) >= 0)
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }                                    
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;

                        case "tel":
                            try
                            {
                                if ((int.Parse(value) > 0) && (value.Length > 7) && (value.Length < 16))
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                }
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un numéro de téléphone valide");
                                value = Console.ReadLine();
                                try
                                {
                                    Console.WriteLine(int.Parse(value));
                                    if ((int.Parse(value) > 0) && (value.Length > 7) && (value.Length < 16))
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;

                        case "mail":
                            while (!value.Contains("@"))
                            {
                                Console.Write("Entrez une adresse email correcte");
                                value = Console.ReadLine();
                            }
                            break;
                    }
                    command.CommandText += $"{attributs[i]} = @{attributs[i]}";
                    command.CommandText += " AND ";
                    command.Parameters.Add($"@{attributs[i]}", MySqlDbType.VarChar);
                    command.Parameters[$"@{attributs[i]}"].Value = value;
                }                
            }            
            command.CommandText = command.CommandText.Remove(command.CommandText.Length - (" AND ".Length), " AND ".Length) + ";";
            Console.Clear();

            MySqlDataReader reader = command.ExecuteReader();

            string[] header2 = new string[] { "Numéro client", "Prénom", "Nom", "Téléphone", "Mail", "Adresse", "Fidélité" };
            string columnName = "| ";
            for (int i = 0; i < header2.Length; i++)
            {
                columnName += $"{header2[i]} | ";
            }
            columnName += "\n";
            Console.WriteLine(columnName);

            while (reader.Read())
            {
                string currentRowAsString = "| ";
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string valueAsString;
                    if (!reader.IsDBNull(i))
                    {
                        valueAsString = reader.GetValue(i).ToString();
                    }
                    else
                    {
                        valueAsString = "Null";
                    }
                    currentRowAsString += valueAsString + " | ";
                }
                Console.WriteLine(currentRowAsString);
            }
            reader.Close();
        }


        public void MenuProduits()
        {
            // Liste des différentes options sur le menu principal
            string[] mainOptions = { "1 - Afficher tous les produits", "2 - Filtrer les produits", "3 - Retour" };
            int selectedIndex = 0;

            // Cache le curseur de l'utilisateur
            Console.CursorVisible = false;
            while (true)
            {
                Console.Clear();

                // Afficher les options du menu
                for (int i = 0; i < mainOptions.Length; i++)
                {
                    // Surligne en vert l'option sélectionnée
                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    Console.WriteLine(mainOptions[i]);
                    Console.ResetColor();
                }

                // Capturer l'entrée de l'utilisateur
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                // Déplacer le curseur en fonction de l'entrée de l'utilisateur
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = Math.Max(0, selectedIndex - 1);
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = Math.Min(mainOptions.Length - 1, selectedIndex + 1);
                        break;
                    case ConsoleKey.Enter:
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Option sélectionnée : {mainOptions[selectedIndex]}\n");
                        Console.ResetColor();

                        // Action en fonction du choix utilisateur
                        string selection = mainOptions[selectedIndex];
                        switch (selection)
                        {
                            case "1 - Afficher tous les produits":
                                Produits();
                                break;

                            case "2 - Filtrer les produits":
                                FiltrerProduits();
                                break;

                            case "3 - Retour":
                                return;
                        }
                        Console.WriteLine("\n\nAppuyez sur ENTRER pour continuer");
                        Console.ReadKey();
                        MenuProduits();
                        return;
                }
            }            
        }


        public void Produits()
        {
            Console.Clear();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText =
                "SELECT P.idP, M.nom, P.nom, P.typeP, P.prix, P.dispo, P.stock FROM Magasin M inner join Produit P on M.idM = P.idM order by P.stock asc; ";

            MySqlDataReader reader = command.ExecuteReader();

            string[] header = new string[] { "Numéro produit", "Nom magasin", "Nom produit", "Type", "Prix", "Disponibilité", "Stock" };
            string columnName = "| ";
            for (int i = 0; i < header.Length; i++)
            {
                columnName += $"{header[i]} | ";
            }
            columnName += "\n";
            Console.WriteLine(columnName);

            while (reader.Read())
            {
                string currentRowAsString = "| ";
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string attribut = reader.GetName(i);
                    string valueAsString;
                    if (!reader.IsDBNull(i))
                    {
                        if ((attribut == "stock") && (reader.GetInt32(i) < this.seuilAlert)) 
                        {
                            this.alert = true;
                            this.produitRupture = reader.GetString(1);
                            this.quantiteRestante = reader.GetInt32(i);
                        }
                        valueAsString = reader.GetValue(i).ToString();
                    }
                    else
                    {
                        valueAsString = "Null";
                    }
                    currentRowAsString += valueAsString + " | ";
                }
                Console.WriteLine(currentRowAsString);
            }
            reader.Close();

            if (this.alert)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nAttention le stock de {produitRupture} est bientôt vide, il n'en reste que {quantiteRestante}");
                Console.ForegroundColor = ConsoleColor.Black;
            }
        }


        public void FiltrerProduits()
        {
            Console.WriteLine("Vous allez être présenté aux différents attributs qui définissent un produit.\nSi vous cherchez un produit qui correspond à l'un de ces attributs, entrez la valeur correspondante que vous recherchez.");
            string[] attributs = new string[] { "idP", "typeP", "nom", "prix", "dispo", "stock", "idM" };
            string[] header = new string[] { "numéro produit", "type produit", "nom", "prix", "dispo", "stock", "numéro magasin" };

            MySqlCommand command = connection.CreateCommand();

            // Création de la requête avec des paramètres pour chaque colonne
            command.CommandText = "SELECT idP, typeP, nom, prix, dispo, stock, idM FROM produit WHERE ";
            for (int i = 0; i < attributs.Length; i++)
            {
                Console.Write($"\nSaisissez la valeur pour {header[i]} : ");
                string value = Console.ReadLine();
                bool validInput;
                if (value != "")
                {
                    switch (attributs[i])
                    {
                        case "idP":
                            try
                            {
                                int.Parse(value);
                                validInput = true;
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un entier positif valide");
                                string userInput = Console.ReadLine();

                                try
                                {
                                    if (int.Parse(value) >= 0)
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;

                        case "prix":
                            try
                            {
                                int.Parse(value);
                                validInput = true;
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un entier positif valide");
                                string userInput = Console.ReadLine();

                                try
                                {
                                    if (int.Parse(value) >= 0)
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;

                        case "stock":
                            try
                            {
                                int.Parse(value);
                                validInput = true;
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un entier positif valide");
                                string userInput = Console.ReadLine();

                                try
                                {
                                    if (int.Parse(value) >= 0)
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;

                        case "idM":
                            try
                            {
                                int.Parse(value);
                                validInput = true;
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un entier positif valide");
                                string userInput = Console.ReadLine();

                                try
                                {
                                    if (int.Parse(value) >= 0)
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;
                    }
                    command.CommandText += $"{attributs[i]} = @{attributs[i]}";
                    command.CommandText += " AND ";
                    command.Parameters.Add($"@{attributs[i]}", MySqlDbType.VarChar);
                    command.Parameters[$"@{attributs[i]}"].Value = value;
                }
            }
            command.CommandText = command.CommandText.Remove(command.CommandText.Length - (" AND ".Length), " AND ".Length) + ";";

            Console.Clear();

            MySqlDataReader reader = command.ExecuteReader();

            string[] header2 = new string[] { "Numéro produit", "Type produit", "Nom", "Prix", "Dispo", "Stock", "Numéro magasin" };
            string columnName = "| ";
            for (int i = 0; i < header2.Length; i++)
            {
                columnName += $"{header2[i]} | ";
            }
            columnName += "\n";
            Console.WriteLine(columnName);

            while (reader.Read())
            {
                string currentRowAsString = "| ";
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string valueAsString;
                    if (!reader.IsDBNull(i))
                    {
                        valueAsString = reader.GetValue(i).ToString();
                    }
                    else
                    {
                        valueAsString = "Null";
                    }
                    currentRowAsString += valueAsString + " | ";
                }
                Console.WriteLine(currentRowAsString);
            }
            reader.Close();
        }


        public void MenuCommandes()
        {
            // Liste des différentes options sur le menu principal
            string[] mainOptions = { "1 - Afficher toutes les commandes", "2 - Filtrer les commandes", "3 - Retour" };
            int selectedIndex = 0;

            // Cache le curseur de l'utilisateur
            Console.CursorVisible = false;
            while (true)
            {
                Console.Clear();

                // Afficher les options du menu
                for (int i = 0; i < mainOptions.Length; i++)
                {
                    // Surligne en vert l'option sélectionnée
                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    Console.WriteLine(mainOptions[i]);
                    Console.ResetColor();
                }

                // Capturer l'entrée de l'utilisateur
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                // Déplacer le curseur en fonction de l'entrée de l'utilisateur
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = Math.Max(0, selectedIndex - 1);
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = Math.Min(mainOptions.Length - 1, selectedIndex + 1);
                        break;
                    case ConsoleKey.Enter:
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Option sélectionnée : {mainOptions[selectedIndex]}\n");
                        Console.ResetColor();

                        // Action en fonction du choix utilisateur
                        string selection = mainOptions[selectedIndex];
                        switch (selection)
                        {
                            case "1 - Afficher toutes les commandes":
                                Commandes();
                                break;

                            case "2 - Filtrer les commandes":
                                FiltrerCommandes();
                                break;

                            case "3 - Retour":
                                return;
                        }
                        Console.WriteLine("\n\nAppuyez sur ENTRER pour continuer");
                        Console.ReadKey();
                        MenuCommandes();
                        return;
                }
            }
        }
        

        public void Commandes()
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText =
                "SELECT dateCom, statut, COUNT(*) FROM Commande GROUP BY dateCom, statut ORDER BY dateCom desc;";

            MySqlDataReader reader = command.ExecuteReader();

            string[] header = new string[] { "Date commande", "Statut", "Nombre" };
            string columnName = "| ";
            for (int i = 0; i < header.Length; i++)
            {
                columnName += $"{header[i]} | ";
            }
            columnName += "\n";
            Console.WriteLine(columnName);

            while (reader.Read())
            {
                string currentRowAsString = "| ";
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string attribut = reader.GetName(i);
                    string valueAsString;
                    if (!reader.IsDBNull(i))
                    {
                        if (i == 0)
                        {
                            string[] temp = reader.GetValue(i).ToString().Split(" ");
                            valueAsString = temp[0];
                        }
                        else
                        {
                            valueAsString = reader.GetValue(i).ToString();
                        }
                    }
                    else
                    {
                        valueAsString = "Null";
                    }
                    currentRowAsString += valueAsString + " | ";
                }
                Console.WriteLine(currentRowAsString);
            }
            reader.Close();

            if (this.alert)
            {
                Console.WriteLine($"\nAttention le stock de {produitRupture} est bientôt vide, il n'en reste que {quantiteRestante} ");
            }
        }


        public void FiltrerCommandes()
        {
            Console.WriteLine("Vous allez être présenté aux différents attributs qui définissent une commande.\nSi vous cherchez une commande qui correspond à l'un de ces attributs, entrez la valeur correspondante que vous recherchez.");
            string[] attributs = new string[] { "idCom", "dateCom", "adresseLiv", "dateLiv", "message", "statut", "idBouqPerso", "idBouqStand", "idC"};
            string[] header = new string[] { "numéro commande", "date de commande", "adresse de livraison", "date de livraison", "message", "statut", "numéro de bouquet personnalisé", "numéro de bouquet standard", "numéro client" };

            MySqlCommand command = connection.CreateCommand();

            // Création de la requête avec des paramètres pour chaque colonne
            command.CommandText = "SELECT idCom, dateCom, adresseLiv, dateLiv, message, statut, idBouqPerso, idBouqStand, idC FROM commande WHERE ";
            for (int i = 0; i < attributs.Length; i++)
            {
                Console.Write($"\nSaisissez la valeur pour {header[i]} : ");
                string value = Console.ReadLine();
                if (value != "")
                {
                    command.CommandText += $"{attributs[i]} = @{attributs[i]}";
                    command.CommandText += " AND ";
                    command.Parameters.Add($"@{attributs[i]}", MySqlDbType.VarChar);
                    command.Parameters[$"@{attributs[i]}"].Value = value;
                }
            }
            command.CommandText = command.CommandText.Remove(command.CommandText.Length - (" AND ".Length), " AND ".Length) + ";";

            Console.Clear();

            MySqlDataReader reader = command.ExecuteReader();

            string[] header2 = new string[] { "Numéro commande", "Date commande", "Adresse livraison", "Date livraison", "Message", "Statut", "Numéro bouquet perso", "Numéro bouquet standard", "Numéro client" };
            string columnName = "| ";
            for (int i = 0; i < header2.Length; i++)
            {
                columnName += $"{header2[i]} | ";
            }
            columnName += "\n";
            Console.WriteLine(columnName);

            while (reader.Read())
            {
                string currentRowAsString = "| ";
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string valueAsString;
                    if (!reader.IsDBNull(i))
                    {
                        if (i == 1 || i == 3)
                        {
                            string[] temp = reader.GetValue(i).ToString().Split(" ");
                            valueAsString = temp[0];
                        }
                        else
                        {
                            valueAsString = reader.GetValue(i).ToString();
                        }
                    }
                    else
                    {
                        valueAsString = "Null";
                    }
                    currentRowAsString += valueAsString + " | ";
                }
                Console.WriteLine(currentRowAsString);
            }
            reader.Close();
        }



        public void Stats()
        {
            // Liste des différentes options sur le menu principal
            string[] mainOptions = { "1 - Statistiques relatives aux clients", "2 - Statistiques relatives aux commandes et ventes", "3 - Statistiques relatives aux produits", "4 - Statistiques relatives aux magasins", "5 - Retour" };
            int selectedIndex = 0;

            // Cache le curseur de l'utilisateur
            Console.CursorVisible = false;
            while (true)
            {
                Console.Clear();

                // Afficher les options du menu
                for (int i = 0; i < mainOptions.Length; i++)
                {
                    // Surligne en vert l'option sélectionnée
                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    Console.WriteLine(mainOptions[i]);
                    Console.ResetColor();
                }

                // Capturer l'entrée de l'utilisateur
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                // Déplacer le curseur en fonction de l'entrée de l'utilisateur
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = Math.Max(0, selectedIndex - 1);
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = Math.Min(mainOptions.Length - 1, selectedIndex + 1);
                        break;
                    case ConsoleKey.Enter:
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Option sélectionnée : {mainOptions[selectedIndex]}\n");
                        Console.ResetColor();

                        // Action en fonction du choix utilisateur
                        string selection = mainOptions[selectedIndex];
                        switch (selection)
                        {
                            case "1 - Statistiques relatives aux clients":
                                NombreClients();
                                MeilleurClientMois();
                                MeilleurClientAnnee();
                                AdresseDiff();
                                break;

                            case "2 - Statistiques relatives aux commandes et ventes":
                                NombreVentes();
                                PrixMoyenBouquet();
                                break;

                            case "3 - Statistiques relatives aux produits":
                                BouquetStandardSucces();
                                FleurMoinsVendue();
                                break;

                            case "4 - Statistiques relatives aux magasins":
                                MagasinCA();
                                break;

                            case "5 - Retour":
                                return;
                        }
                        Console.Write("\n\nVeuillez appuyer sur ENTRER pour continuer");
                        Console.ReadKey();
                        Stats();
                        return;
                }
            }
        }


        /// Clients

        public void NombreClients()
        {            
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(DISTINCT idC) FROM client;";

            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Console.WriteLine($"Vous avez : {reader.GetValue(0)} clients");
            }
            reader.Close();
        }

        public void MeilleurClientMois()
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT c.prenom, c.nom, COUNT(*) AS nb_commandes FROM client c natural join commande cmd WHERE MONTH(cmd.dateCom) = MONTH(NOW()) AND YEAR(cmd.dateCom) = YEAR(NOW()) GROUP BY c.idC ORDER BY nb_commandes DESC LIMIT 1;";

            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Console.WriteLine($"Le meilleur client du mois est : {reader.GetValue(0)} {reader.GetValue(1)} avec {reader.GetValue(2)} commandes");               
            }
            reader.Close();
        }

        // REQUETE SYNCHRONISEE
        public void MeilleurClientAnnee()
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT c.prenom, c.nom, COUNT(*) AS nb_commandes FROM client c NATURAL JOIN commande cmd WHERE YEAR(cmd.dateCom) = YEAR(NOW()) GROUP BY c.idC HAVING COUNT(*) = ( SELECT MAX(commandes_par_client) FROM ( SELECT COUNT(*) AS commandes_par_client FROM commande WHERE YEAR(dateCom) = YEAR(NOW()) GROUP BY idC ) AS t);";
            
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Console.WriteLine($"Le meilleur client de l'année est : {reader.GetValue(0)} {reader.GetValue(1)} avec {reader.GetValue(2)} commandes");
            }
            reader.Close();
        }

        // REQUETE AUTO-JOINTURE
        public void AdresseDiff()
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) AS nombreClients FROM(SELECT DISTINCT c.idC FROM Client c JOIN Commande cmd1 ON c.idC = cmd1.idC JOIN Commande cmd2 ON c.idC = cmd2.idC AND cmd1.adresseLiv <> cmd2.adresseLiv) sous_requete;";

            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Console.WriteLine($"Il y a {reader.GetValue(0)} client(s) qui ont utilisé une adresse de facturation et de livraison différentes");
            }
            reader.Close();            
        }

        /// Commandes et ventes

        public void NombreVentes()
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM commande;";

            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Console.WriteLine($"{reader.GetValue(0)} ventes ont été effectuées depuis le début");
            }
            reader.Close();
        }

        // REQUETE AVEC UNE UNION
        public void PrixMoyenBouquet()
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText =
                "SELECT AVG(prix) FROM(SELECT prixMax as prix FROM bouquet_perso UNION ALL SELECT prix FROM bouquet_standard) AS tous_les_bouquets;";

            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                double value = reader.GetDouble(0);
                Console.WriteLine($"Le prix moyen du bouquet acheté est de {Math.Round(value, 2)} €\n");
            }
            reader.Close();
        }

        public void MagasinCA()
        {
            string query = @"SELECT nom, annee, SUM(chiffre_affaires) as CA
                     FROM (
                            SELECT magasin.nom, YEAR(commande.dateCom) as annee, SUM(bouquet_standard.prix) as chiffre_affaires
                            FROM magasin
                            JOIN client ON magasin.idM = client.idM
                            JOIN commande ON client.idC = commande.idC
                            JOIN bouquet_standard ON commande.idBouqStand = bouquet_standard.idBouqStand
                            GROUP BY magasin.nom, YEAR(commande.dateCom), bouquet_standard.prix

                            UNION

                            SELECT magasin.nom, YEAR(commande.dateCom) as annee, SUM(produit.prix * item_bouquet.quantite) as chiffre_affaires
                            FROM magasin
                            JOIN client ON magasin.idM = client.idM
                            JOIN commande ON client.idC = commande.idC
                            JOIN bouquet_perso ON commande.idBouqPerso = bouquet_perso.idBouqPerso
                            JOIN item_bouquet ON commande.idBouqPerso = item_bouquet.idBouqPerso
                            JOIN produit ON item_bouquet.idP = produit.idP
                            GROUP BY magasin.nom, YEAR(commande.dateCom), bouquet_perso.PrixMax
                         ) as res
                     GROUP BY nom, annee
                     ORDER BY nom, annee;";

            string[] header = new string[] { "Magasin", "Année", "Chiffre d'affaires" };
            string columnName = "| ";
            for (int i = 0; i < header.Length; i++)
            {
                columnName += $"{header[i]} | ";
            }
            columnName += "\n";
            Console.WriteLine(columnName);

            using MySqlCommand cmd = new MySqlCommand(query, connection);
            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string currentRowAsString = "| ";
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string valueAsString = reader.GetValue(i).ToString();
                    valueAsString = char.ToUpper(valueAsString[0]) + valueAsString.Substring(1);                    
                    if (i == 2)
                    {
                        valueAsString += "€";
                    }
                    currentRowAsString += valueAsString + " | ";
                }
                Console.WriteLine(currentRowAsString);
            }
            reader.Close();
        }

        /// Produits
        
        public void BouquetStandardSucces()
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT bs.nom, COUNT(*) AS nb_commandes FROM bouquet_standard bs NATURAL JOIN commande cmd GROUP BY bs.idBouqStand ORDER BY nb_commandes DESC LIMIT 1;";

            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Console.WriteLine($"Le bouquet standard avec le plus succès est : {reader.GetValue(0)} avec {reader.GetValue(1)} ventes");
            }
            reader.Close();
        }

        public void FleurMoinsVendue()
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT P.nom, SUM(ib.quantite) FROM Item_Bouquet as ib natural JOIN Produit P WHERE P.typeP = 'fleur' GROUP BY P.nom ORDER BY SUM(ib.quantite) asc LIMIT 1;";

            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Console.WriteLine($"La fleur la moins vendue est {reader.GetValue(0)} avec {reader.GetValue(1)} ventes");
            }
            reader.Close();
        }





        public void MenuAdmin()
        {
            // Liste des différentes options sur le menu principal
            string[] mainOptions = { "1 - Ajouter/Modifier/Supprimer un client", "2 - Ajouter/Supprimer une commande", "3 - Ajouter/Modifier/Supprimer un produit", "4 - Envoyer une requête en direct", "5 - Réinitialiser la BD", "6 - Retour" };
            int selectedIndex = 0;

            // Cache le curseur de l'utilisateur
            Console.CursorVisible = false;
            while (true)
            {
                Console.Clear();

                // Afficher les options du menu
                for (int i = 0; i < mainOptions.Length; i++)
                {
                    // Surligne en vert l'option sélectionnée
                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    Console.WriteLine(mainOptions[i]);
                    Console.ResetColor();
                }

                // Capturer l'entrée de l'utilisateur
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                // Déplacer le curseur en fonction de l'entrée de l'utilisateur
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = Math.Max(0, selectedIndex - 1);
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = Math.Min(mainOptions.Length - 1, selectedIndex + 1);
                        break;
                    case ConsoleKey.Enter:
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Option sélectionnée : {mainOptions[selectedIndex]}\n");
                        Console.ResetColor();

                        // Action en fonction du choix utilisateur
                        string selection = mainOptions[selectedIndex];
                        switch (selection)
                        {
                            case "1 - Ajouter/Modifier/Supprimer un client":
                                AdminClient();
                                break;

                            case "2 - Ajouter/Supprimer une commande":
                                AdminCommande();
                                break;

                            case "3 - Ajouter/Modifier/Supprimer un produit":
                                AdminProduit();
                                break;

                            case "4 - Envoyer une requête en direct":
                                SendCMD();
                                break;

                            case "5 - Réinitialiser la BD":
                                AdminBDD();
                                break;

                            case "6 - Retour":
                                return;
                        }
                        MenuAdmin();
                        return;
                }
            }
        }


        /*
        MENU ADMINISTRATEUR CLIENT
        */


        public void AdminClient()
        {
            // Liste des différentes options sur le menu principal
            string[] mainOptions = { "1 - Ajouter un client", "2 - Modifier un client", "3 - Supprimer un client", "4 - Retour" };
            int selectedIndex = 0;

            // Cache le curseur de l'utilisateur
            Console.CursorVisible = false;
            while (true)
            {
                Console.Clear();

                // Afficher les options du menu
                for (int i = 0; i < mainOptions.Length; i++)
                {
                    // Surligne en vert l'option sélectionnée
                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    Console.WriteLine(mainOptions[i]);
                    Console.ResetColor();
                }

                // Capturer l'entrée de l'utilisateur
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                // Déplacer le curseur en fonction de l'entrée de l'utilisateur
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = Math.Max(0, selectedIndex - 1);
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = Math.Min(mainOptions.Length - 1, selectedIndex + 1);
                        break;
                    case ConsoleKey.Enter:
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Option sélectionnée : {mainOptions[selectedIndex]}\n");
                        Console.ResetColor();

                        // Action en fonction du choix utilisateur
                        string selection = mainOptions[selectedIndex];
                        switch (selection)
                        {
                            case "1 - Ajouter un client":
                                AddClient();
                                break;

                            case "2 - Modifier un client":
                                ModifClient();
                                break;

                            case "3 - Supprimer un client":
                                DelClient();
                                break;                            

                            case "4 - Retour":
                                return;
                        }
                        Console.WriteLine("\n\nVeuillez appuyer sur ENTRER pour continuer");
                        Console.ReadKey();
                        return;
                }
            }
        }


        public void AddClient()
        {
            Console.WriteLine("Veuillez entrer les différents attributs qui définissent un client\n");
            string[] attributs = new string[] { "nom", "prenom", "tel", "mail", "passwd", "adresseFact", "cb", "idM" };
            string[] header = new string[] { "nom", "prénom", "téléphone", "mail", "mot de passe", "adresse de facturation", "carte bleue", "numéro magasin  de rattachement" };

            MySqlCommand command = connection.CreateCommand();

            // Création de la requête avec des paramètres pour chaque colonne
            command.CommandText = "insert into client(prenom, nom, tel, mail, passwd, adresseFact, cb, idM) values (";

            for (int i = 0; i < attributs.Length; i++)
            {
                Console.Write($"\nSaisissez la valeur pour {header[i]} : ");
                bool validInput;
                string value = Console.ReadLine();
                if (value != "")
                {
                    switch (attributs[i])
                    {
                        case "tel":
                            try
                            {
                                if ((int.Parse(value) > 0) && (value.Length > 7) && (value.Length < 16))
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                }
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un numéro de téléphone valide");
                                value = Console.ReadLine();
                                try
                                {
                                    Console.WriteLine(int.Parse(value));
                                    if ((int.Parse(value) > 0) && (value.Length > 7) && (value.Length < 16))
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;

                        case "mail":
                            while (!value.Contains("@"))
                            {
                                Console.Write("Entrez une adresse email correcte");
                                value = Console.ReadLine();
                            }
                            break;

                        case "passwd":
                            while (value.Length <= 10)
                            {
                                Console.Write("Entrez un mot de passe d'au moins 10 caractères");
                                value = Console.ReadLine();
                            }
                            break;

                        case "idM":
                            try
                            {
                                if (int.Parse(value) >= 0)
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                }
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un entier positif valide");
                                value = Console.ReadLine();

                                try
                                {
                                    if (int.Parse(value) >= 0)
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;
                    }
                    command.CommandText += $"{attributs[i]} = @{attributs[i]}";
                    command.CommandText += ", ";
                    command.Parameters.Add($"@{attributs[i]}", MySqlDbType.VarChar);
                    command.Parameters[$"@{attributs[i]}"].Value = value;                    
                }
            }
            command.CommandText += ");";

            try
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Client ajouté avec succès");
            }
            catch (MySqlException e)
            {
                Console.WriteLine("\nERREUR : Vérifiez que vous avez entré des valeurs correctes");
            }
            command.Dispose();
        }


        public void ModifClient()
        {
            Console.WriteLine("Veuillez entrer le numéro du client que vous souhaitez modifier\n");
            int idCValue = SecureIntInput();

            string[] attributs = new string[] { "nom", "prenom", "tel", "mail", "passwd", "adresseFact", "fidelite", "cb", "idM" };
            string[] header = new string[] { "nom", "prénom", "téléphone", "mail", "mot de passe", "adresse de facturation", "fidélité", "carte bleue", "numéro magasin  de rattachement" };

            MySqlCommand command = connection.CreateCommand();

            // Création de la requête avec des paramètres pour chaque colonne
            command.CommandText = "UPDATE client SET ";

            for (int i = 0; i < attributs.Length; i++)
            {
                Console.Write($"\nSaisissez la nouvelle valeur pour {header[i]} : ");
                bool validInput;
                string value = Console.ReadLine();
                if (value != "")
                {
                    switch (attributs[i])
                    {
                        case "tel":
                            try
                            {
                                if ((int.Parse(value) > 0) && (value.Length > 7) && (value.Length < 16))
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                }
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un numéro de téléphone valide");
                                value = Console.ReadLine();
                                try
                                {
                                    Console.WriteLine(int.Parse(value));
                                    if ((int.Parse(value) > 0) && (value.Length > 7) && (value.Length < 16))
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;

                        case "mail":
                            while (!value.Contains("@"))
                            {
                                Console.Write("Entrez une adresse email correcte");
                                value = Console.ReadLine();
                            }
                            break;

                        case "passwd":
                            while (value.Length <= 10)
                            {
                                Console.Write("Entrez un mot de passe d'au moins 10 caractères");
                                value = Console.ReadLine();
                            }
                            break;

                        case "fidelite":
                            while(value != "Or" || value != "Bronze")
                            {
                                Console.Write("Entrez un statut de fidélité correcte (Or ou Bronze)");
                                value = Console.ReadLine();
                            }
                            break;

                        case "cb":
                            try
                            {
                                if ((int.Parse(value) > 0) && (value.Length == 16))
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                }
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Entrez un numéro de carte bleue valide (16 caractères)");
                                value = Console.ReadLine();
                                try
                                {
                                    Console.WriteLine(int.Parse(value));
                                    if ((int.Parse(value) > 0) && (value.Length == 16))
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;

                        case "idM":
                            try
                            {
                                if (int.Parse(value) >= 0)
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                }
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un entier positif valide");
                                value = Console.ReadLine();

                                try
                                {
                                    if (int.Parse(value) >= 0)
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;
                    }
                    command.CommandText += $"{attributs[i]} = @{attributs[i]}";
                    command.CommandText += ", ";
                    command.Parameters.Add($"@{attributs[i]}", MySqlDbType.VarChar);
                    command.Parameters[$"@{attributs[i]}"].Value = value;
                }
            }
            if (command.CommandText.EndsWith(", "))
            {
                command.CommandText = command.CommandText.Remove(command.CommandText.Length - ", ".Length) + " WHERE idC = @idC;";
            }
            command.Parameters.Add("@idC", MySqlDbType.VarChar);
            command.Parameters["@idC"].Value = idCValue;

            try
            {
                command.ExecuteNonQuery();
                Console.WriteLine("\nClient modifié avec succès");
            }
            catch (MySqlException e)
            {
                Console.WriteLine("ERREUR : Vérifiez que vous avez entré des valeurs correctes");
            }
            command.Dispose();
        }


        public void DelClient()
        {
            // On récupère les idC des clients à supprimer
            Console.WriteLine("Entrez les valeurs des attributs correspondants au(x) client(s) que vous souhaitez supprimer");

            string[] attributs = new string[] { "idC", "nom", "prenom", "tel", "mail", "passwd", "adresseFact", "fidelite", "cb", "idM" };
            string[] header = new string[] { "numéro client", "nom", "prénom", "téléphone", "mail", "mot de passe", "adresse de facturation", "fidélité", "carte bleue", "numéro magasin de rattachement" };

            MySqlCommand command = connection.CreateCommand();

            command.CommandText = "select idC FROM client WHERE ";
            for (int i = 0; i < attributs.Length; i++)
            {
                Console.Write($"\nSaisissez la valeur pour {header[i]} : ");
                bool validInput;
                string value = Console.ReadLine();
                if (value != "")
                {
                    switch (attributs[i])
                    {
                        case "idC":
                            try
                            {
                                if (int.Parse(value) >= 0)
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                }
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un entier positif valide");
                                value = Console.ReadLine();

                                try
                                {
                                    if (int.Parse(value) >= 0)
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;

                        case "tel":
                            try
                            {
                                if ((int.Parse(value) > 0) && (value.Length > 7) && (value.Length < 16))
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                }
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un numéro de téléphone valide");
                                value = Console.ReadLine();
                                try
                                {
                                    Console.WriteLine(int.Parse(value));
                                    if ((int.Parse(value) > 0) && (value.Length > 7) && (value.Length < 16))
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;

                        case "mail":
                            while (!value.Contains("@"))
                            {
                                Console.Write("Entrez une adresse email correcte");
                                value = Console.ReadLine();
                            }
                            break;

                        case "fidelite":
                            while (value != "Or" || value != "Bronze")
                            {
                                Console.Write("Entrez un statut de fidélité correcte (Or ou Bronze)");
                                value = Console.ReadLine();
                            }
                            break;

                        case "cb":
                            try
                            {
                                if ((int.Parse(value) > 0) && (value.Length == 16))
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                }
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Entrez un numéro de carte bleue valide (16 caractères)");
                                value = Console.ReadLine();
                                try
                                {
                                    Console.WriteLine(int.Parse(value));
                                    if ((int.Parse(value) > 0) && (value.Length == 16))
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;

                        case "idM":
                            try
                            {
                                if (int.Parse(value) >= 0)
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                }
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un entier positif valide");
                                value = Console.ReadLine();

                                try
                                {
                                    if (int.Parse(value) >= 0)
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;
                    }
                    command.CommandText += $"{attributs[i]} = @{attributs[i]}";
                    command.CommandText += " AND ";
                    command.Parameters.Add($"@{attributs[i]}", MySqlDbType.VarChar);
                    command.Parameters[$"@{attributs[i]}"].Value = value;
                }
            }
            if (command.CommandText.EndsWith(" AND "))
            {
                command.CommandText = command.CommandText.Remove(command.CommandText.Length - " AND ".Length) + ";";
            }
            command.Dispose();

            MySqlDataReader reader = command.ExecuteReader();

            string idC;
            if (reader.Read())
            {
                // On supprime les commandes du client à supprimer
                idC = reader.GetValue(0).ToString();
                reader.Close();
                command = connection.CreateCommand();
                command.CommandText = "DELETE FROM commande WHERE idC = @idC";
                command.Parameters.Add($"@idC", MySqlDbType.VarChar);
                command.Parameters[$"@idC"].Value = idC;

                try
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("\nCommandes associées au client supprimées");
                }
                catch (MySqlException e)
                {
                    Console.WriteLine("\nERREUR : Impossible de supprimer les commandes associées au client à supprimer");
                    Console.ReadKey();
                }
                command.Dispose();

                // On supprime ensuite les clients grâce aux idC
                command = connection.CreateCommand();
                command.CommandText = "DELETE FROM client WHERE idC = @idC";
                command.Parameters.Add($"@idC", MySqlDbType.VarChar);
                command.Parameters[$"@idC"].Value = idC;

                try
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("\nClient supprimé avec succès");
                }
                catch (MySqlException e)
                {
                    Console.WriteLine("\nERREUR : Impossible de supprimer le client spécifié : Vérifiez que les valeurs entrées sont correctes et réessayez");
                    Console.ReadKey();
                }
                command.Dispose();
            }
            else
            {
                Console.WriteLine("Le client spécifié n'existe pas ou plus");
                reader.Close();
            }
        }



        /*
        MENU ADMINISTRATEUR COMMANDE
        */


        public void AdminCommande()
        {
            // Liste des différentes options sur le menu principal
            string[] mainOptions = { "1 - Ajouter une commande", "2 - Supprimer une commande", "3 - Retour" };
            int selectedIndex = 0;

            // Cache le curseur de l'utilisateur
            Console.CursorVisible = false;
            while (true)
            {
                Console.Clear();

                // Afficher les options du menu
                for (int i = 0; i < mainOptions.Length; i++)
                {
                    // Surligne en vert l'option sélectionnée
                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    Console.WriteLine(mainOptions[i]);
                    Console.ResetColor();
                }

                // Capturer l'entrée de l'utilisateur
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                // Déplacer le curseur en fonction de l'entrée de l'utilisateur
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = Math.Max(0, selectedIndex - 1);
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = Math.Min(mainOptions.Length - 1, selectedIndex + 1);
                        break;
                    case ConsoleKey.Enter:
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Option sélectionnée : {mainOptions[selectedIndex]}\n");
                        Console.ResetColor();

                        // Action en fonction du choix utilisateur
                        string selection = mainOptions[selectedIndex];
                        switch (selection)
                        {
                            case "1 - Ajouter une commande":
                                AddCommande();
                                break;

                            case "2 - Supprimer une commande":
                                DelCommande();
                                break;

                            case "3 - Retour":
                                return;
                        }
                        Console.WriteLine("\n\nVeuillez appuyer sur ENTRER pour continuer");
                        Console.ReadKey();
                        return;
                }
            }
        }


        public void AddCommande()
        {
            Console.WriteLine("Veuillez entrer les différents attributs qui définissent une commande\n");
            string[] attributs = new string[] { "idCom", "dateCom", "adresseLiv", "dateLiv", "message", "idC" };
            string[] header = new string[] { "numéro commande", "date commande", "adresse livraison", "date livraison", "message personnel", "numéro du client passant la commande" };

            MySqlCommand command = connection.CreateCommand();

            // Création de la requête avec des paramètres pour chaque colonne
            command.CommandText = "insert into commande(idCom, dateCom, adresseLiv, dateLiv, message, statut, adresseFact, cb, idM) values (";

            for (int i = 0; i < attributs.Length; i++)
            {
                Console.Write($"\nSaisissez la valeur pour {header[i]} : ");
                bool validInput;
                string value = Console.ReadLine();
                if (value != "")
                {
                    switch (attributs[i])
                    {
                        case "idC":
                            try
                            {
                                if (int.Parse(value) >= 0)
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                }
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un entier positif valide");
                                value = Console.ReadLine();

                                try
                                {
                                    if (int.Parse(value) >= 0)
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;

                        case "tel":
                            try
                            {
                                if ((int.Parse(value) > 0) && (value.Length > 7) && (value.Length < 16))
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                }
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un numéro de téléphone valide");
                                value = Console.ReadLine();
                                try
                                {
                                    Console.WriteLine(int.Parse(value));
                                    if ((int.Parse(value) > 0) && (value.Length > 7) && (value.Length < 16))
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;

                        case "mail":
                            while (!value.Contains("@"))
                            {
                                Console.Write("Entrez une adresse email correcte");
                                value = Console.ReadLine();
                            }
                            break;

                        case "passwd":
                            while (value.Length <= 10)
                            {
                                Console.Write("Entrez un mot de passe d'au moins 10 caractères");
                                value = Console.ReadLine();
                            }
                            break;

                        case "idM":
                            try
                            {
                                if (int.Parse(value) >= 0)
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                }
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un entier positif valide");
                                value = Console.ReadLine();

                                try
                                {
                                    if (int.Parse(value) >= 0)
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;
                    }
                    command.CommandText += $"{attributs[i]} = @{attributs[i]}";
                    command.CommandText += ", ";
                    command.Parameters.Add($"@{attributs[i]}", MySqlDbType.VarChar);
                    command.Parameters[$"@{attributs[i]}"].Value = value;
                }
            }
            command.CommandText += ");";

            try
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Client ajouté avec succès");
            }
            catch (MySqlException e)
            {
                Console.WriteLine("ERREUR : Vérifiez que vous avez entré des valeurs correctes");
            }
            command.Dispose();
        }


        public void DelCommande()
        {
            // On récupère les idC des clients à supprimer
            Console.WriteLine("Entrez les valeurs des attributs correspondants au(x) commande(s) que vous souhaitez supprimer");

            string[] attributs = new string[] { "idCom", "dateCom", "adresseLiv", "dateLiv", "message", "statut", "idC" };
            string[] header = new string[] { "numéro commande", "date commande", "adresse livraison", "date livraison", "message personnel", "statut commande", "numéro client passant la commande" };

            MySqlCommand command = connection.CreateCommand();

            command.CommandText = "delete from commande WHERE ";
            for (int i = 0; i < attributs.Length; i++)
            {
                Console.Write($"\nSaisissez la valeur pour {header[i]} : ");
                DateTime date;
                bool validInput;
                string value = Console.ReadLine();
                if (value != "")
                {
                    switch (attributs[i])
                    {
                        case "idCom":
                            try
                            {
                                if (int.Parse(value) >= 0)
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                }
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un entier positif valide");
                                value = Console.ReadLine();

                                try
                                {
                                    if (int.Parse(value) >= 0)
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;

                        case "dateCom":
                            if (DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                            {
                                validInput = true;
                            }
                            else
                            {
                                validInput = true;
                            }

                            while (!validInput)
                            {
                                value = Console.ReadLine();

                                if (DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                    Console.WriteLine("Format de date invalide, entrez une date au format YYYY-MM-DD");
                                }
                            }
                            break;

                        case "dateLiv":
                            if (DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                            {
                                validInput = true;
                            }
                            else
                            {
                                validInput = true;
                            }

                            while (!validInput)
                            {
                                value = Console.ReadLine();

                                if (DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                    Console.WriteLine("Format de date invalide, entrez une date au format YYYY-MM-DD");
                                }
                            }
                            break;

                        case "statut":
                            while (new string[] { "VINV", "CC", "CPAV" }.Contains(value))
                            {
                                Console.Write("Entrez un statut de commande (VINV, CC ou CPAV : vous ne pouvez pas supprimer les commandes prêtes à être livrées ou déjà livrées)");
                                value = Console.ReadLine();
                            }
                            break;

                        case "idC":
                            try
                            {
                                if (int.Parse(value) >= 0)
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                }
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un entier positif valide");
                                value = Console.ReadLine();

                                try
                                {
                                    if (int.Parse(value) >= 0)
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;
                    }
                    command.CommandText += $"{attributs[i]} = @{attributs[i]}";
                    command.CommandText += " AND ";
                    command.Parameters.Add($"@{attributs[i]}", MySqlDbType.VarChar);
                    command.Parameters[$"@{attributs[i]}"].Value = value;
                }
            }
            if (command.CommandText.EndsWith(" AND "))
            {
                command.CommandText = command.CommandText.Remove(command.CommandText.Length - " AND ".Length) + ";";
            }

            try
            {
                command.ExecuteNonQuery();
                Console.WriteLine("\nCommande supprimée avec succès");
            }
            catch (MySqlException e)
            {
                Console.WriteLine("\nERREUR : Impossible de supprimer la commande spécifiée : Vérifiez que les valeurs entrées sont correctes et réessayez");
            }
            command.Dispose();               
        }


        /*
        MENU ADMINISTRATEUR PRODUIT
        */


        public void AdminProduit()
        {
            // Liste des différentes options sur le menu principal
            string[] mainOptions = { "1 - Ajouter un produit", "2 - Modifier un produit", "3 - Supprimer un produit", "4 - Retour" };
            int selectedIndex = 0;

            // Cache le curseur de l'utilisateur
            Console.CursorVisible = false;
            while (true)
            {
                Console.Clear();

                // Afficher les options du menu
                for (int i = 0; i < mainOptions.Length; i++)
                {
                    // Surligne en vert l'option sélectionnée
                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    Console.WriteLine(mainOptions[i]);
                    Console.ResetColor();
                }

                // Capturer l'entrée de l'utilisateur
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                // Déplacer le curseur en fonction de l'entrée de l'utilisateur
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = Math.Max(0, selectedIndex - 1);
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = Math.Min(mainOptions.Length - 1, selectedIndex + 1);
                        break;
                    case ConsoleKey.Enter:
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Option sélectionnée : {mainOptions[selectedIndex]}\n");
                        Console.ResetColor();

                        // Action en fonction du choix utilisateur
                        string selection = mainOptions[selectedIndex];
                        switch (selection)
                        {
                            case "1 - Ajouter un produit":
                                AddProduit();
                                break;

                            case "2 - Modifier un produit":
                                ModifProduit();
                                break;

                            case "3 - Supprimer un produit":
                                DelProduit();
                                break;

                            case "4 - Retour":
                                return;
                        }
                        Console.WriteLine("\n\nVeuillez appuyer sur ENTRER pour continuer");
                        Console.ReadKey();
                        return;
                }
            }
        }


        public void AddProduit()
        {
            Console.WriteLine("Veuillez entrer les différents attributs qui définissent un client\n");
            string[] attributs = new string[] { "typeP", "nom", "prix", "dispo", "stock", "idM" };
            string[] header = new string[] { "type produit", "nom", "prix", "disponibilité", "stock", "numéro magasin de stockage" };

            MySqlCommand command = connection.CreateCommand();

            // Création de la requête avec des paramètres pour chaque colonne
            command.CommandText = "insert into produit(typeP, nom, prix, dispo, stock, idM) values (";

            for (int i = 0; i < attributs.Length; i++)
            {
                Console.Write($"\nSaisissez la valeur pour {header[i]} : ");
                bool validInput;
                string value = Console.ReadLine();
                if (value != "")
                {
                    switch (attributs[i])
                    {
                        case "typeP":
                            while (new string[] { "Fleur", "Accessoire" }.Contains(value))
                            {
                                Console.Write("Entrez un type de produit de commande (VINV, CC ou CPAV : vous ne pouvez pas supprimer les commandes prêtes à être livrées ou déjà livrées)");
                                value = Console.ReadLine();
                            }
                            break;

                        case "prix":
                            try
                            {
                                if (int.Parse(value) >= 0)
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                }
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un entier positif valide");
                                value = Console.ReadLine();

                                try
                                {
                                    if (int.Parse(value) >= 0)
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;

                        case "stock":
                            try
                            {
                                if (int.Parse(value) >= 0)
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                }
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un entier positif valide");
                                value = Console.ReadLine();

                                try
                                {
                                    if (int.Parse(value) >= 0)
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;

                        case "idM":
                            try
                            {
                                if (int.Parse(value) >= 0)
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                }
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un entier positif valide");
                                value = Console.ReadLine();

                                try
                                {
                                    if (int.Parse(value) >= 0)
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;
                    }
                    command.CommandText += $"{attributs[i]} = @{attributs[i]}";
                    command.CommandText += ", ";
                    command.Parameters.Add($"@{attributs[i]}", MySqlDbType.VarChar);
                    command.Parameters[$"@{attributs[i]}"].Value = value;
                }
            }
            command.CommandText += ");";

            try
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Produit ajouté avec succès");
            }
            catch (MySqlException e)
            {
                Console.WriteLine("\nERREUR : Vérifiez que vous avez entré des valeurs correctes");
                Console.ReadKey();
            }            
            command.Dispose();
        }


        public void ModifProduit()
        {
            Console.WriteLine("Veuillez entrer le numéro du produit que vous souhaitez modifier\n");
            int idPValue = SecureIntInput();

            string[] attributs = new string[] { "typeP", "nom", "prix", "dispo", "stock", "idM" };
            string[] header = new string[] { "type produit", "nom", "prix", "disponibilité", "stock", "numéro magasin de stockage" };

            MySqlCommand command = connection.CreateCommand();

            // Création de la requête avec des paramètres pour chaque colonne
            command.CommandText = "UPDATE client SET ";

            for (int i = 0; i < attributs.Length; i++)
            {
                Console.Write($"\nSaisissez la nouvelle valeur pour {header[i]} : ");
                bool validInput;
                string value = Console.ReadLine();
                if (value != "")
                {
                    switch (attributs[i])
                    {
                        case "typeP":
                            while (new string[] { "Fleur", "Accessoire" }.Contains(value))
                            {
                                Console.Write("Entrez un type de produit de commande (VINV, CC ou CPAV : vous ne pouvez pas supprimer les commandes prêtes à être livrées ou déjà livrées)");
                                value = Console.ReadLine();
                            }
                            break;

                        case "prix":
                            try
                            {
                                if (int.Parse(value) >= 0)
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                }
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un entier positif valide");
                                value = Console.ReadLine();

                                try
                                {
                                    if (int.Parse(value) >= 0)
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;

                        case "stock":
                            try
                            {
                                if (int.Parse(value) >= 0)
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                }
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un entier positif valide");
                                value = Console.ReadLine();

                                try
                                {
                                    if (int.Parse(value) >= 0)
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;

                        case "idM":
                            try
                            {
                                if (int.Parse(value) >= 0)
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                }
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un entier positif valide");
                                value = Console.ReadLine();

                                try
                                {
                                    if (int.Parse(value) >= 0)
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;
                    }
                    command.CommandText += $"{attributs[i]} = @{attributs[i]}";
                    command.CommandText += ", ";
                    command.Parameters.Add($"@{attributs[i]}", MySqlDbType.VarChar);
                    command.Parameters[$"@{attributs[i]}"].Value = value;
                }
            }
            if (command.CommandText.EndsWith(", "))
            {
                command.CommandText = command.CommandText.Remove(command.CommandText.Length - ", ".Length) + " WHERE idP = @idP;";
            }
            command.Parameters.Add("@idP", MySqlDbType.VarChar);
            command.Parameters["@idP"].Value = idPValue;

            try
            {
                command.ExecuteNonQuery();
                Console.WriteLine("\nProduit modifié avec succès");
            }
            catch (MySqlException e)
            {
                Console.WriteLine("\nERREUR : Vérifiez que vous avez entré un numéro de produit valide");
                Console.ReadKey();
            }
            command.Dispose();
        }


        public void DelProduit()
        {
            // On récupère les idC des clients à supprimer
            Console.WriteLine("Entrez les valeurs des attributs correspondants au(x) produits(s) que vous souhaitez supprimer");

            string[] attributs = new string[] { "idP", "typeP", "nom", "prix", "dispo", "stock", "idM" };
            string[] header = new string[] { "numéro produit", "type produit", "nom", "prix", "disponibilité", "stock", "numéro magasin de stockage" };

            MySqlCommand command = connection.CreateCommand();

            command.CommandText = "select idP FROM produit WHERE ";
            for (int i = 0; i < attributs.Length; i++)
            {
                Console.Write($"\nSaisissez la valeur pour {header[i]} : ");
                bool validInput;
                string value = Console.ReadLine();
                if (value != "")
                {
                    switch (attributs[i])
                    {
                        case "idP":
                            try
                            {
                                if (int.Parse(value) >= 0)
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                }
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un entier positif valide");
                                value = Console.ReadLine();

                                try
                                {
                                    if (int.Parse(value) >= 0)
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;

                        case "typeP":
                            while (new string[] { "Fleur", "Accessoire" }.Contains(value))
                            {
                                Console.Write("Entrez un type de produit de commande (VINV, CC ou CPAV : vous ne pouvez pas supprimer les commandes prêtes à être livrées ou déjà livrées)");
                                value = Console.ReadLine();
                            }
                            break;

                        case "prix":
                            try
                            {
                                if (int.Parse(value) >= 0)
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                }
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un entier positif valide");
                                value = Console.ReadLine();

                                try
                                {
                                    if (int.Parse(value) >= 0)
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;

                        case "stock":
                            try
                            {
                                if (int.Parse(value) >= 0)
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                }
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un entier positif valide");
                                value = Console.ReadLine();

                                try
                                {
                                    if (int.Parse(value) >= 0)
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;

                        case "idM":
                            try
                            {
                                if (int.Parse(value) >= 0)
                                {
                                    validInput = true;
                                }
                                else
                                {
                                    validInput = false;
                                }
                            }
                            catch (Exception)
                            {
                                validInput = false;
                            }

                            while (!validInput)
                            {
                                Console.WriteLine("Veuillez entrer un entier positif valide");
                                value = Console.ReadLine();

                                try
                                {
                                    if (int.Parse(value) >= 0)
                                    {
                                        validInput = true;
                                    }
                                    else
                                    {
                                        validInput = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    validInput = false;
                                }
                            }
                            break;
                    }
                    command.CommandText += $"{attributs[i]} = @{attributs[i]}";
                    command.CommandText += " AND ";
                    command.Parameters.Add($"@{attributs[i]}", MySqlDbType.VarChar);
                    command.Parameters[$"@{attributs[i]}"].Value = value;
                }
            }
            if (command.CommandText.EndsWith(" AND "))
            {
                command.CommandText = command.CommandText.Remove(command.CommandText.Length - " AND ".Length) + ";";
            }
            command.Dispose();

            MySqlDataReader reader = command.ExecuteReader();

            string idP;
            if (reader.Read())
            {
                // On supprime les items de bouquets qui font référence à ce produit
                idP = reader.GetValue(0).ToString();
                reader.Close();
                command = connection.CreateCommand();
                command.CommandText = "DELETE FROM item_bouquet WHERE idP = @idP";
                command.Parameters.Add($"@idP", MySqlDbType.VarChar);
                command.Parameters[$"@idP"].Value = idP;

                try
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("\nItems de bouquets associés au produit supprimés");
                }
                catch (MySqlException e)
                {
                    Console.WriteLine("\nERREUR : Impossible de supprimer les items de bouquets associés au produit à supprimer");
                    Console.ReadKey();
                }
                command.Dispose();

                // On supprime ensuite les clients grâce aux idC
                command = connection.CreateCommand();
                command.CommandText = "DELETE FROM produit WHERE idP = @idP";
                command.Parameters.Add($"@idP", MySqlDbType.VarChar);
                command.Parameters[$"@idP"].Value = idP;

                try
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("\nProduit supprimé avec succès");
                }
                catch (MySqlException e)
                {
                    Console.WriteLine("\nERREUR : Impossible de supprimer le produit spécifié : Vérifiez que les valeurs entrées sont correctes et réessayez");
                }
                command.Dispose();
            }
            else
            {
                Console.WriteLine("Le produit spécifié n'existe pas ou plus");
                reader.Close();
            }
        }


        /*
        ENVOI REQUETE
        */


        public void SendCMD()
        {
            Console.WriteLine("Veuillez entrer la commande (en écriture seulement) à envoyer à la BDD");
            MySqlCommand command = connection.CreateCommand();
            Console.CursorVisible = true;
            command.CommandText = Console.ReadLine();
            Console.CursorVisible = false;
            if (command.CommandText != "")
            {
                try
                {
                    Console.WriteLine("\nLa requête a été effectuée avec succès\n\n");
                    MySqlDataReader reader = command.ExecuteReader();

                    string columnName = "| ";
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        columnName += $"{reader.GetName(i)} | ";
                    }
                    columnName += "\n";
                    Console.WriteLine(columnName);

                    while (reader.Read())
                    {
                        string currentRowAsString = "| ";
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string valueAsString;
                            if (!reader.IsDBNull(i))
                            {
                                valueAsString = reader.GetValue(i).ToString();
                            }
                            else
                            {
                                valueAsString = "Null";
                            }
                            currentRowAsString += valueAsString + " | ";
                        }
                        Console.WriteLine(currentRowAsString);
                    }
                    reader.Close();

                }
                catch (MySqlException e)
                {
                    Console.WriteLine("La requête a échoué : " + e.ToString());
                }
            }
            Console.WriteLine("\n\nVeuillez appuyer sur ENTRER pour continuer");
            Console.ReadKey();
        }



        /*
        MENU ADMINISTRATEUR BDD
        */


        public void AdminBDD()
        {
            // Liste des différentes options sur le menu principal
            string[] mainOptions = { "1 - Vider toutes les tables", "2 - Supprimer la BD", "3 - Retour" };
            int selectedIndex = 0;

            // Cache le curseur de l'utilisateur
            Console.CursorVisible = false;
            while (true)
            {
                Console.Clear();

                // Afficher les options du menu
                for (int i = 0; i < mainOptions.Length; i++)
                {
                    // Surligne en vert l'option sélectionnée
                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    Console.WriteLine(mainOptions[i]);
                    Console.ResetColor();
                }

                // Capturer l'entrée de l'utilisateur
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                // Déplacer le curseur en fonction de l'entrée de l'utilisateur
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = Math.Max(0, selectedIndex - 1);
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = Math.Min(mainOptions.Length - 1, selectedIndex + 1);
                        break;
                    case ConsoleKey.Enter:
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Option sélectionnée : {mainOptions[selectedIndex]}\n");
                        Console.ResetColor();

                        Console.CursorVisible = true;

                        bool securityCheck = false;

                        Console.WriteLine("Etes-vous sûr de vouloir supprimer la base de données, cela effacera toutes les données de manière irréversible.\n" +
                                    "Si vous souhaitez procéder, entrez la phrase suivante : \"Je souhaite supprimer la BD et je suis conscient que c'est un acte irréversible\"\n");
                        string choice = Console.ReadLine();
                        if (choice == "Je souhaite supprimer la BD et je suis conscient que c'est un acte irréversible")
                        {
                            Console.WriteLine("\nPar mesure de sécurité : veuillez entrer votre mot de passe\n");
                            string passwd = Console.ReadLine();
                            if (passwd == passwdAdmin)
                            {
                                securityCheck = true;
                            }
                            else
                            {
                                Console.WriteLine("\nMot de passe incorrect : action annulée");
                            }
                        }

                        Console.CursorVisible = false;

                        // Action en fonction du choix utilisateur
                        string selection = mainOptions[selectedIndex];
                        switch (selection)
                        {
                            case "1 - Vider toutes les tables":
                                if (securityCheck)
                                {
                                    ViderTables();
                                }
                                break;

                            case "2 - Supprimer la BD":
                                if (securityCheck)
                                {
                                    DropBDD();
                                }                       
                                break;                            

                            case "3 - Retour":
                                return;
                        }
                        Console.WriteLine("\n\nVeuillez appuyer sur ENTRER pour continuer");
                        Console.ReadKey();
                        return;
                }
            }
        }


        public void ViderTables()
        {
            string[] tables = new string[] { "item_bouquet", "produit", "commande", "bouquet_standard", "bouquet_perso", "client", "depot", "magasin" };
            foreach(string table in tables)
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = $"DELETE FROM {table};";
                try
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine($"\nTable {table} vidée");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"\nERREUR : Impossible de vider la table {table} : {e}");
                }
            }
        }


        public void DropBDD()
        {            
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "DROP DATABASE IF EXISTS fleuriste;";
            try
            {
                command.ExecuteNonQuery();
                Console.WriteLine("\nBase de données \"fleuriste\" supprimée avec succès");
            }
            catch (MySqlException e)
            {
                Console.WriteLine("\nERREUR : Impossible de supprimer la base de données");
            }
        }
        


        /// <summary>
        /// Saisie sécurisée d'entier
        /// </summary>
        /// <returns> Entier entré par l'utilisateur </returns>
        public int SecureIntInput()
        {
            string input;
            int value = 0;

            while (true)
            {
                input = Console.ReadLine();
                try
                {
                    value = int.Parse(input);
                    if (value > 0)
                    {
                        return value;
                    }
                    Console.WriteLine("La valeur entrée n'est pas strictement supérieure à 0");
                }
                catch (Exception)
                {
                    Console.WriteLine("La valeur entrée n'est pas un entier");
                }
            }
        }


    }
}
