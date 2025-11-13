// Jaden Olvera, 11-11-25, Lab 10: ATM machine 
using System.Text;

Console.Clear();
Console.WriteLine("The Bank\u2122's ATM");
Console.WriteLine("~ Where your money is safer ~");

var customerDataBase = loadBankCustomers("bank.txt");

// Login
int userIndex = validateUser(customerDataBase);
if (userIndex == -1)
{
    return;
}
else
{
    int requestedMenu = 0;
    while (requestedMenu != 7)
    {
        requestedMenu = mainMenu(customerDataBase, userIndex);
        switch (requestedMenu)
        {
            case 1:
                requestedMenu = checkBalance(customerDataBase, userIndex);
                break;
            case 2:
                (requestedMenu, decimal fundsToWithdraw) = withdrawFunds(customerDataBase, userIndex);
                if (fundsToWithdraw > 0)
                {
                    customerDataBase[userIndex][2] = Convert.ToString(decimal.Parse(customerDataBase[userIndex][2]) - fundsToWithdraw);
                    saveBankCustomers("bank.txt", customerDataBase);
                }
                //A little theater to make it look nice
                Console.Clear();
                Console.WriteLine("Dispensing funds...");
                Thread.Sleep(2000);
                Console.Clear();
                Console.WriteLine($"Funds dispensed. Your new balance is: $ {customerDataBase[userIndex][2]}");
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey(true);
                break;
            case 3:
                (requestedMenu, decimal fundsToDeposit) = depositFunds(customerDataBase, userIndex);
                if (fundsToDeposit > 0)
                {
                    customerDataBase[userIndex][2] = Convert.ToString(decimal.Parse(customerDataBase[userIndex][2]) + fundsToDeposit);
                    saveBankCustomers("bank.txt", customerDataBase);
                }
                Console.Clear();
                Console.WriteLine("Depositing funds...");
                Thread.Sleep(2000);
                Console.Clear();
                Console.WriteLine($"Funds deposited. Your new balance is: $ {customerDataBase[userIndex][2]}");
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey(true);
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            default:
                break;

        }
    }
    Console.WriteLine(requestedMenu);
}

static List<List<string>> loadBankCustomers(string filepath)
{
    var listOfLists = File.ReadAllLines(filepath).Select(line => line.Split(',').ToList()).ToList();
    return listOfLists;
}

static void saveBankCustomers(string filepath, List<List<string>> listToSave)
{
    List<string> writeList = [];
    foreach (List<string> line in listToSave)
    {
        StringBuilder lineBuilder = new();
        foreach (string field in line)
            lineBuilder.Append(field + ",");
        writeList.Add(lineBuilder.ToString().TrimEnd(','));
    }
    string[] dataArray = writeList.ToArray();
    File.WriteAllLines(filepath, dataArray);
}

static int findUser(List<List<string>> dataList)
{
    while (true)
    {
        drawHeader();
        Console.Write("Username:  ");
        string? userNameAttempt = Console.ReadLine();
        if (string.IsNullOrEmpty(userNameAttempt) == false)
        {
            userNameAttempt = userNameAttempt.Trim().ToLowerInvariant();
            for (int index = 0; index < dataList.Count; index++)
            {
                if (userNameAttempt.Equals(dataList[index][0]) == true)
                {
                    return index;
                }
            }
            // If the current user index wasn't returned, we didn't find the supplied username
            Console.WriteLine("User not found!");
            Thread.Sleep(1300);
        }
    }
}

static int validateUser(List<List<string>> dataList)
{
    // Won't proceed until we have a valid user index to check the PIN for
    int currentUserIndex = findUser(dataList);

    // We don't want to let them brute force the PIN
    int attemptCounter = 0;
    int attemptLimit = 3;
    while (attemptCounter <= attemptLimit)
    {
        drawHeader();
        Console.Write("PIN:  ");

        StringBuilder pwAttempt = new();
        ConsoleKeyInfo userInput;

        //We want to collect characters as long as they don't press Enter or Escape
        do
        {
            // Catch userInput as ConsoleKeyInfo to use both KeyChar and Key later
            userInput = Console.ReadKey(true);

            // Only append characters to the list if they're numerical digits that would be in a PIN
            if (userInput.KeyChar >= '0' && userInput.KeyChar <= '9')
            {
                pwAttempt.Append(userInput.KeyChar);
                Console.Write('*');
            }

            // Allow users to backspace if they need to
            else if (userInput.Key == ConsoleKey.Backspace)
            {
                if (pwAttempt.Length > 0)
                {
                    pwAttempt.Remove(pwAttempt.Length - 1, 1);
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    Console.Write(' ');
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                }
            }
        } while (userInput.Key != ConsoleKey.Enter && userInput.Key != ConsoleKey.Escape);

        // Collapse char list into a string
        string pinEntered = pwAttempt.ToString();

        // Check pin attempt against the current user index's password field, should always be [i][1]
        if (pinEntered == dataList[currentUserIndex][1])
        {
            Console.WriteLine();
            Console.WriteLine("PIN correct! Loading account...");
            Thread.Sleep(1300);
            return currentUserIndex;
        }
        else
        {
            Console.WriteLine();
            if (attemptCounter == 3)
                Console.WriteLine("PIN incorrect. \nDue to repeated incorrect entries, you must try again later.\nGoodbye!");
            else
                Console.WriteLine("PIN incorrect. Please try again.");
            attemptCounter++;
            Thread.Sleep(1300);
        }
    }
    return -1;
}

static void drawHeader()
{
    Console.Clear();
    Console.WriteLine("The Bank\u2122's ATM");
    Console.WriteLine("~ Where your money is safer ~");
    Console.WriteLine();
}

static int mainMenu(List<List<string>> dataList, int userIndex)
{
    string[] mainMenuArray = [  $"Welcome to your The Bank\u2122 account, {dataList[userIndex][0]}!",
                                $"What can we help you with today?",
                                "1. Check Balance", 
                                "2. Withdraw", 
                                "3. Deposit", 
                                "4. Display last 5 transactions", 
                                "5. Quick Withdraw $40", 
                                "6. Quick Withdraw $100",
                                "7. End current session"
                            ];
    int firstOptionIndex = 2;
    // Return selected option, offset by 1 to account for index
    return optionSelector(mainMenuArray, firstOptionIndex) - 1;
}

static int optionSelector(string[] menuArray, int firstOptionIndex)
{
    int optionHighlighted = firstOptionIndex;
    while (true)
    {
        drawHeader();
        for (int index = 0; index < menuArray.Length; index++)
        {
            if (optionHighlighted == index)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
            }

            Console.WriteLine(menuArray[index]);
            
            if (optionHighlighted == index)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        ConsoleKey userInput = Console.ReadKey(true).Key;
        if (userInput == ConsoleKey.UpArrow && optionHighlighted > firstOptionIndex)
            optionHighlighted--;
        else if (userInput == ConsoleKey.DownArrow && optionHighlighted <= menuArray.Length - firstOptionIndex)
            optionHighlighted++;
        else if (userInput == ConsoleKey.Enter)
            return optionHighlighted;
        else
            Console.Beep();
    }
}

// Just makes sure we get a proper value, actual value checking is in the methods calling this
// Returns a bool and a value, if they enter 0, it will be a success
static (bool parseSuccess, decimal fundsInput) handleMoneyInput()
{
    string stringInput = Console.ReadLine();
    if (String.IsNullOrEmpty(stringInput) == false)
    {
        bool parseSuccess = decimal.TryParse(stringInput, out decimal parseOut);
        if (parseSuccess == true)
            return (true, parseOut);
    }
    return (false, 0);
}

// Main Menu Function Methods

// Just shows the balance and kicks us back to the main menu
static int checkBalance(List<List<string>> dataList, int userIndex)
{
    drawHeader();
    Console.WriteLine($"{dataList[userIndex][0]}, your current balance is: $ {dataList[userIndex][2]}\n Please press any key to return to the main menu.");
    Console.ReadKey(true);
    return 0;
}

// Collect an amount from the user and return 0 for main menu and the amount
static (int requestedMenu, decimal fundsToWithdraw) withdrawFunds(List<List<string>>dataList, int userIndex)
{
    bool parseSuccess = false;
    decimal fundsToWithdraw = 0;
    while (parseSuccess == false)
    {
        drawHeader();
        Console.WriteLine($"{dataList[userIndex][0]}, your current balance is: $ {dataList[userIndex][2]}\nPlease enter an amount to withdraw that is less than or equal to your current balance.");
        (parseSuccess, fundsToWithdraw) = handleMoneyInput();
        // Value can't be less than 0
        if (parseSuccess == true && fundsToWithdraw < 0)
        {
            parseSuccess = false;
            Console.WriteLine("Please enter a positive amount.");
            Thread.Sleep(1300);
        }
        // Value can't be less than what they have in the account, no overdrafts
        else if (parseSuccess == true && fundsToWithdraw > decimal.Parse(dataList[userIndex][2]))
        {
            parseSuccess = false;
            Console.WriteLine("That amount is more than your current balance.\nPlease enter an amount that is less than or equal to your current balance.");
            Thread.Sleep(1300);
        }
        // Only returns to the main menu with a value if it is correctly parsed, not negative, and less than or equal to their balance
        else if (parseSuccess == true && fundsToWithdraw <= decimal.Parse(dataList[userIndex][2]))
            return (0, fundsToWithdraw);
        // Catch for something that's not in the above cases
        else
        {
            parseSuccess = false;
            Console.WriteLine("That input wasn't properly recognized,\nplease enter an amount to withdraw.");
            Thread.Sleep(1300);
        }
    }
    //Default path
    return (0, fundsToWithdraw);
}

static (int requestedMenu, decimal fundsToDeposit) depositFunds(List<List<string>>dataList, int userIndex)
{
    bool parseSuccess = false;
    decimal fundsToDeposit = 0;
    while (parseSuccess == false)
    {
        drawHeader();
        Console.WriteLine($"{dataList[userIndex][0]}, your current balance is: $ {dataList[userIndex][2]}\nPlease enter an amount to withdraw that is less than or equal to your current balance.");
        (parseSuccess, fundsToDeposit) = handleMoneyInput();
        if (parseSuccess == true && fundsToDeposit < 0)
        {
            parseSuccess = false;
            Console.WriteLine("Please enter a positive amount.");
            Thread.Sleep(1300);
        }
        else if (parseSuccess == true)
            return (0, fundsToDeposit);
        else
        {
            parseSuccess = false;
            Console.WriteLine("That input wasn't properly recognized,\nplease enter an amount to withdraw.");
            Thread.Sleep(1300);
        }
    }
    //Default path
    return (0, fundsToDeposit);
}