using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;

namespace DeliveryListTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please write the name of the file with delivery, sent by the supplier: ");
            string inputDeliveryFileName = Console.ReadLine();
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), inputDeliveryFileName)))
            {
                Console.WriteLine("The file does not exist.");
                return;
            }
            if (Path.GetExtension(inputDeliveryFileName) != ".csv")
            {
                Console.WriteLine("The file is not in csv format.");
                return;
            }
            Console.WriteLine("");

            Console.WriteLine("Please write the name of the file with orders from clients: ");
            string inputOrderFromClientsFileName = Console.ReadLine();
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), inputOrderFromClientsFileName)))
            {
                Console.WriteLine("The file does not exist.");
                return;
            }
            if (Path.GetExtension(inputOrderFromClientsFileName) != ".csv")
            {

                Console.WriteLine("The file is not in csv format.");
                return;
            }
            Console.WriteLine("");

            Console.WriteLine("Please input the name of the import: ");
            string deliveryName = Console.ReadLine();
            Console.WriteLine("");

            string generatedFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ImportFile.csv");
            Console.WriteLine("CSV file is generating...");
            Console.WriteLine("");
            GenerateDeliveryListImportFile(inputDeliveryFileName, inputOrderFromClientsFileName, generatedFilePath, deliveryName);
            Console.WriteLine("");
            Console.WriteLine("Generated file with name: " + generatedFilePath);
            Console.WriteLine("");
            // GenerateDeliveryListImportFile("1.csv", "2.csv", "HM127");

            Console.WriteLine("Press Enter to close...");
            Console.ReadLine();
        }

        private static void GenerateDeliveryListImportFile(string inputDeliveryFileName, string inputOrderFromClientsFileName, string generatedFilePath, string deliveryName)
        {
            string inputDeliveryFileNamePath = Path.Combine(Directory.GetCurrentDirectory(), inputDeliveryFileName);
            var deliveryItems = GetDeliveryItems(inputDeliveryFileNamePath, deliveryName);

            string inputOrderFromClientsFilePath = Path.Combine(Directory.GetCurrentDirectory(), inputOrderFromClientsFileName);
            var allOrderItemRows = File.ReadAllLines(inputOrderFromClientsFilePath);
            
            MarkReservedDeliveryItems(deliveryItems, allOrderItemRows);

            using (StreamWriter file = new StreamWriter(generatedFilePath, false))
            {
                foreach (var deliveryItem in deliveryItems)
                {
                    if (!deliveryItem.IsReservedForClient)
                    {
                        file.WriteLine(deliveryItem.ToString());
                    }
                }
            }
        }

        private static List<DeliveryItem> GetDeliveryItems(string inputDeliveryFileNamePath, string deliveryName)
        {
            var result = new List<DeliveryItem>();
            var allDeliveryItemRows = File.ReadAllLines(inputDeliveryFileNamePath);

            for (int i = 0; i < allDeliveryItemRows.Length; ++i)
            {
                string row = allDeliveryItemRows[i];
                try
                {
                    string[] dataRow = ParseRow(row);
                    string itemCode = dataRow[0];
                    int quantity = Convert.ToInt32(dataRow[1]);
                    string batchNumber = dataRow[2];
                    DateTime expirationDate = Convert.ToDateTime(dataRow[3]);

                    var deliveryItem =
                        new DeliveryItem(itemCode, quantity, batchNumber, expirationDate, deliveryName, "EUR");

                    result.Add(deliveryItem);
                }
                catch
                {
                    Console.WriteLine("Wrong format for row " + i + ": " + row);
                }
            }

            return result;
        }
        
        private static void MarkReservedDeliveryItems(List<DeliveryItem> deliveryItems, string[] allOrderItemRows)
        {
            for (int i = 0; i < allOrderItemRows.Length; ++i)
            {
                string row = allOrderItemRows[i];
                try
                {
                    string[] dataRow = ParseRow(row);
                    string itemCode = dataRow[0];
                    int quantity = Convert.ToInt32(Convert.ToDouble(dataRow[1], CultureInfo.CreateSpecificCulture("bg-BG")));

                    foreach (var deliveryItem in deliveryItems)
                    {
                        deliveryItem.CheckForReservation(itemCode, quantity);
                    }
                }
                catch
                {
                    Console.WriteLine("Wrong format for row " + i + ": " + row);
                }
            }
        }

        private static string[] ParseRow(string row)
        {
            TextFieldParser parser = new TextFieldParser(new StringReader(row));

            parser.HasFieldsEnclosedInQuotes = true;
            parser.SetDelimiters(",");

            return parser.ReadFields();
        }

    }
}
