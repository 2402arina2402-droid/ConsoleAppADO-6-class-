using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace ConsoleAppADO_6_class_
{
    internal class Program
    {
        private static string connectionString = @"Server=localhost;Database=TeaShop;Integrated Security=True;TrustServerCertificate=True;";

        private static DataSet dataSet = new DataSet();

        static void Main(string[] args)
        {
            if (ConnectAndLoadData() == false)
            {
                Console.WriteLine("Press Enter to exit...");
                Console.ReadLine();
                return;
            }

            bool working = true;
            while (working)
            {
                Console.WriteLine("\n--- TEA SHOP MENU ---");
                Console.WriteLine("1. Show all tea info");
                Console.WriteLine("2. Show all tea names");
                Console.WriteLine("3. Show green tea names");
                Console.WriteLine("4. Show black tea names");
                Console.WriteLine("5. Show tea names except green and black");
                Console.WriteLine("6. Show tea with weight <= N grams");
                Console.WriteLine("7. Show min cost price");
                Console.WriteLine("8. Show max cost price");
                Console.WriteLine("9. Show average cost price");
                Console.WriteLine("10. Show count of teas with min cost price");
                Console.WriteLine("11. Show count of teas with max cost price");
                Console.WriteLine("12. Show count of teas with cost price > average");
                Console.WriteLine("13. Show count of teas for each type");
                Console.WriteLine("0. Exit");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                if (choice == "1") ShowAllTeaInfo();
                else if (choice == "2") ShowAllTeaNames();
                else if (choice == "3") ShowGreenTeaNames();
                else if (choice == "4") ShowBlackTeaNames();
                else if (choice == "5") ShowOtherTeaNames();
                else if (choice == "6") ShowTeaByMaxWeight();
                else if (choice == "7") ShowMinCostPrice();
                else if (choice == "8") ShowMaxCostPrice();
                else if (choice == "9") ShowAvgCostPrice();
                else if (choice == "10") ShowCountMinCostPrice();
                else if (choice == "11") ShowCountMaxCostPrice();
                else if (choice == "12") ShowCountAboveAvgCostPrice();
                else if (choice == "13") ShowCountByEachType();
                else if (choice == "0") working = false;
                else Console.WriteLine("Wrong choice!");

                if (working)
                {
                    Console.WriteLine("\nPress Enter to continue...");
                    Console.ReadLine();
                }
            }
        }

        private static bool ConnectAndLoadData()
        {
            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                Console.WriteLine("Connected to TeaShop database successfully!");
                conn.Close();


                SqlDataAdapter adapterTeas = new SqlDataAdapter("SELECT * FROM Teass", connectionString);
                adapterTeas.Fill(dataSet, "Teass");

                SqlDataAdapter adapterCountries = new SqlDataAdapter("SELECT * FROM Countriess", connectionString);
                adapterCountries.Fill(dataSet, "Countriess");

                SqlDataAdapter adapterTypes = new SqlDataAdapter("SELECT * FROM TeaTypess", connectionString);
                adapterTypes.Fill(dataSet, "TeaTypess");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection error: " + ex.Message);
                return false;
            }
        }


        private static void ShowAllTeaInfo()
        {
            Console.WriteLine("--- ALL TEA INFORMATION ---");
            DataTable teasTable = dataSet.Tables["Teass"];

            foreach (DataRow row in teasTable.Rows)
            {
                string country = GetCountryName(Convert.ToInt32(row["CountryID"]));
                string type = GetTypeName(Convert.ToInt32(row["TypeID"]));

                Console.WriteLine("ID: " + row["ID"] +
                                  " | Name: " + row["Name"] +
                                  " | Country: " + country +
                                  " | Type: " + type +
                                  " | Weight: " + row["WeightInGrams"] + "g" +
                                  " | Price: " + row["CostPrice"] +
                                  " | Description: " + row["Description"]);
            }
        }

        private static void ShowAllTeaNames()
        {
            Console.WriteLine("--- ALL TEA NAMES ---");
            DataTable teasTable = dataSet.Tables["Teass"];

            foreach (DataRow row in teasTable.Rows)
            {
                Console.WriteLine("- " + row["Name"]);
            }
        }

        private static void ShowGreenTeaNames()
        {
            Console.WriteLine("--- GREEN TEA NAMES ---");
            int greenTypeId = GetTypeIdByName("Green");

            DataRow[] rows = dataSet.Tables["Teass"].Select("TypeID = " + greenTypeId);
            foreach (DataRow row in rows)
            {
                Console.WriteLine("- " + row["Name"]);
            }
        }

        private static void ShowBlackTeaNames()
        {
            Console.WriteLine("--- BLACK TEA NAMES ---");
            int blackTypeId = GetTypeIdByName("Black");

            DataRow[] rows = dataSet.Tables["Teass"].Select("TypeID = " + blackTypeId);
            foreach (DataRow row in rows)
            {
                Console.WriteLine("- " + row["Name"]);
            }
        }

        private static void ShowOtherTeaNames()
        {
            Console.WriteLine("--- TEA NAMES (EXCEPT GREEN AND BLACK) ---");
            int greenTypeId = GetTypeIdByName("Green");
            int blackTypeId = GetTypeIdByName("Black");

            DataRow[] rows = dataSet.Tables["Teass"].Select("TypeID <> " + greenTypeId + " AND TypeID <> " + blackTypeId);
            foreach (DataRow row in rows)
            {
                string typeName = GetTypeName(Convert.ToInt32(row["TypeID"]));
                Console.WriteLine("- " + row["Name"] + " (" + typeName + ")");
            }
        }

        private static void ShowTeaByMaxWeight()
        {
            Console.Write("Enter max weight in grams: ");
            int maxWeight = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("\n--- TEA WITH WEIGHT <= " + maxWeight + " GRAMS ---");
            DataRow[] rows = dataSet.Tables["Teass"].Select("WeightInGrams <= " + maxWeight);

            if (rows.Length == 0)
            {
                Console.WriteLine("No teas found.");
            }
            else
            {
                foreach (DataRow row in rows)
                {
                    Console.WriteLine("- " + row["Name"] + " (Weight: " + row["WeightInGrams"] + "g)");
                }
            }
        }

        private static void ShowMinCostPrice()
        {
            object min = dataSet.Tables["Teass"].Compute("MIN(CostPrice)", "");
            Console.WriteLine("Minimum cost price: " + min);
        }


        private static void ShowMaxCostPrice()
        {
            object max = dataSet.Tables["Teass"].Compute("MAX(CostPrice)", "");
            Console.WriteLine("Maximum cost price: " + max);
        }


        private static void ShowAvgCostPrice()
        {
            object avg = dataSet.Tables["Teass"].Compute("AVG(CostPrice)", "");
            Console.WriteLine("Average cost price: " + avg);
        }


        private static void ShowCountMinCostPrice()
        {
            object min = dataSet.Tables["Teass"].Compute("MIN(CostPrice)", "");
            DataRow[] rows = dataSet.Tables["Teass"].Select("CostPrice = " + min);
            Console.WriteLine("Count of teas with min cost price (" + min + "): " + rows.Length);
        }


        private static void ShowCountMaxCostPrice()
        {
            object max = dataSet.Tables["Teass"].Compute("MAX(CostPrice)", "");
            DataRow[] rows = dataSet.Tables["Teass"].Select("CostPrice = " + max);
            Console.WriteLine("Count of teas with max cost price (" + max + "): " + rows.Length);
        }


        private static void ShowCountAboveAvgCostPrice()
        {
            object avg = dataSet.Tables["Teass"].Compute("AVG(CostPrice)", "");
            double avgValue = Convert.ToDouble(avg);

            DataRow[] rows = dataSet.Tables["Teass"].Select("CostPrice > " + avgValue.ToString().Replace(',', '.'));
            Console.WriteLine("Average cost price is: " + avgValue);
            Console.WriteLine("Count of teas with cost price > average: " + rows.Length);
        }


        private static void ShowCountByEachType()
        {
            Console.WriteLine("--- COUNT OF TEAS BY TYPE ---");
            DataTable typesTable = dataSet.Tables["TeaTypess"];

            foreach (DataRow typeRow in typesTable.Rows)
            {
                int typeId = Convert.ToInt32(typeRow["ID"]);
                string typeName = typeRow["Name"].ToString();

                DataRow[] teasOfType = dataSet.Tables["Teass"].Select("TypeID = " + typeId);
                Console.WriteLine("Type: " + typeName + " | Count: " + teasOfType.Length);
            }
        }



        private static string GetCountryName(int countryId)
        {
            DataRow[] rows = dataSet.Tables["Countriess"].Select("ID = " + countryId);
            if (rows.Length > 0)
                return rows[0]["Name"].ToString();
            return "Unknown";
        }

        private static string GetTypeName(int typeId)
        {
            DataRow[] rows = dataSet.Tables["TeaTypess"].Select("ID = " + typeId);
            if (rows.Length > 0)
                return rows[0]["Name"].ToString();
            return "Unknown";
        }

        private static int GetTypeIdByName(string name)
        {
            DataRow[] rows = dataSet.Tables["TeaTypess"].Select("Name = '" + name + "'");
            if (rows.Length > 0)
                return Convert.ToInt32(rows[0]["ID"]);
            return -1;
        }
    }
}
