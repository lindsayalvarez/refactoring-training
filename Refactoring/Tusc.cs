using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactoring
{
    public class Tusc
    {
        private static User currentUser;
        const string CURRENCY = "C";
        private static bool continueLoginPrompt = true;

        public static void Start(List<User> users, List<Product> products)
        {
            WriteWelcomeMessage();

            while (!LogIn(users) && continueLoginPrompt)
            { }

            if (continueLoginPrompt)
            {
                PrintLoginSuccessfulMessage();
                PrintRemainingBalance();

                bool cancelled = false;
                while (!cancelled)
                {
                    PrintProductList(products);
                    int productNumber = GetProductNumberBasedOnUserInput();

                    if (productNumber == products.Count)
                    {
                        WritePurchaseDetails(users, products);
                        PrintClosingMessage();
                        return;
                    }
                    else
                    {
                        PrintPurchaseSummaryBeforePurchase(products, productNumber);

                        int quantityToPurchase = GetPurchaseQuantity();

                        if (quantityToPurchase <= 0)
                        {
                            PrintPurchaseCancelled();
                            cancelled = true;
                        }
                        else if (CheckHasEnoughMoney(products, productNumber, quantityToPurchase) &&
                            CheckHasEnoughStock(products, productNumber, quantityToPurchase))
                        {
                            MakePurchase(products, productNumber, quantityToPurchase);
                        }
                    }
                }
            }
        }

        private static void PrintPurchaseSummaryBeforePurchase(List<Product> products, int productNumber)
        {
            Console.WriteLine();
            Console.WriteLine("You want to buy: " + products[productNumber].Name);
            Console.WriteLine("Your balance is " + currentUser.Bal.ToString(CURRENCY));
        }

        private static void WritePurchaseDetails(List<User> users, List<Product> products)
        {
            string json = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(@"Data\Users.json", json);

            string json2 = JsonConvert.SerializeObject(products, Formatting.Indented);
            File.WriteAllText(@"Data\Products.json", json2);
        }

        private static void PrintClosingMessage()
        {
            Console.WriteLine();
            Console.WriteLine("Press Enter key to exit");
            Console.ReadLine();
        }

        private static void PrintPurchaseCancelled()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine("Purchase cancelled");
            Console.ResetColor();
        }

        private static void MakePurchase(List<Product> products, int productNumber, int quantityToPurchase)
        {
            currentUser.Bal = currentUser.Bal - products[productNumber].Price * quantityToPurchase;
            products[productNumber].Qty = products[productNumber].Qty - quantityToPurchase;
            PrintPurchaseDetails(products, productNumber, quantityToPurchase);
        }

        private static void PrintPurchaseDetails(List<Product> products, int productNumber, int quantityToPurchase)
        {

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("You bought " + quantityToPurchase + " " + products[productNumber].Name);
            Console.WriteLine("Your new balance is " + currentUser.Bal.ToString(CURRENCY));
            Console.ResetColor();
        }

        private static bool CheckHasEnoughStock(List<Product> products, int productNumber, int quantityToPurchase)
        {
            bool HasEnoughStock = true;

            if (products[productNumber].Qty < quantityToPurchase)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine("Sorry, " + products[productNumber].Name + " is out of stock");
                Console.ResetColor();
                HasEnoughStock = false;
            }

            return HasEnoughStock;
        }

        private static bool CheckHasEnoughMoney(List<Product> products, int productNumber, int quantityToPurchase)
        {
            bool HasEnoughMoney = true;
            double purchaseCost = products[productNumber].Price * quantityToPurchase;

            if (currentUser.Bal - purchaseCost < 0)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine("You do not have enough money to buy that.");
                Console.ResetColor();
                HasEnoughMoney = false;
            }

            return HasEnoughMoney;
        }

        private static int GetPurchaseQuantity()
        {
            Console.WriteLine("Enter amount to purchase:");
            string userInput = Console.ReadLine();
            int quantityToPurchase = Convert.ToInt32(userInput);

            return quantityToPurchase;
        }

        private static void PrintProductList(List<Product> products)
        {
            Console.WriteLine();
            Console.WriteLine("What would you like to buy?");

            for (int i = 0; i < products.Count; i++)
            {
                int productNumber = i + 1;
                Console.WriteLine(productNumber + ": " + products[i].Name + " (" + products[i].Price.ToString(CURRENCY) + ")");
            }

            Console.WriteLine(products.Count + 1 + ": Exit");
        }

        private static int GetProductNumberBasedOnUserInput()
        {
            Console.WriteLine("Enter a number:");
            string userInput = Console.ReadLine();
            int itemNumber = Convert.ToInt32(userInput) - 1;

            return itemNumber;
        }

        private static double PrintRemainingBalance()
        {
            double balance = currentUser.Bal;
            Console.WriteLine();
            Console.WriteLine("Your balance is " + balance.ToString(CURRENCY));
            return balance;
        }

        private static void PrintLoginSuccessfulMessage()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Login successful! Welcome " + currentUser.Name + "!");
            Console.ResetColor();
        }

        private static void PrintInvalidPasswordMessage()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("You entered an invalid password.");
            Console.ResetColor();
        }

        private static void PrintInvalidUserMessage()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("You entered an invalid user.");
            Console.ResetColor();
        }

        private static void WriteWelcomeMessage()
        {
            Console.WriteLine("Welcome to TUSC");
            Console.WriteLine("---------------");
        }

        private static string PromptForUsername()
        {
            Console.WriteLine();
            Console.WriteLine("Enter Username:");
            return Console.ReadLine();
        }

        private static string PromptForPassword()
        {
            Console.WriteLine("Enter Password:");
            return Console.ReadLine();
        }

        private static bool SetCurrentUser(List<User> users, string username)
        {
            bool isValidUser = false;

            if (!string.IsNullOrEmpty(username))
            {
                for (int i = 0; i < users.Count; i++)
                {
                    if (users[i].Name == username)
                    {
                        currentUser = users[i];
                        isValidUser = true;
                    }
                }
            }

            return isValidUser;
        }

        private static bool IsValidPassword(string password)
        {
            bool isValidPassword = false;

            if (currentUser.Pwd == password)
            {
                isValidPassword = true;
            }

            return isValidPassword;
        }

        private static bool LogIn(List<User> users)
        {
            bool IsLoggedOn = false;

            string username = PromptForUsername();

            if (username != null)
            {
                if (SetCurrentUser(users, username))
                {
                    string password = PromptForPassword();

                    if (IsValidPassword(password))
                    {
                        IsLoggedOn = true;
                    }
                    else
                    {
                        PrintInvalidPasswordMessage();
                    }
                }
                else
                {
                    PrintInvalidUserMessage();
                }
            }
            else
                continueLoginPrompt = false;

            return IsLoggedOn;
        }
    }
}
