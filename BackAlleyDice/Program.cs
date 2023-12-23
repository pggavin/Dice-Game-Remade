using System;
using static System.Console;
using static System.Threading.Thread;

GameStart:

    #region Initial Setup

    var userBalance = 200;
    // Initial balance

    var random = new Random();
    // For generating random numbers

    var odds = new int[]
    {
        -1, 1, 1, 1, 1, 180, 180, 180, 180, 180, 180, 10, 10, 10, 10, 10, 10, 30, 60, 30, 18, 12, 8, 7, 6
    };
    // Odds for each bet, index is per-bet and there is no bet at 0

    var diceFaces = new string[][]
    {
        null,                     // You can't roll 0   

        new string[]{ "████████", // 1
                      "███  ███",
                      "████████", },

        new string[]{ "█  █████", // 2
                      "████████",
                      "█████  █", },

        new string[]{ "██████  ", // 3
                      "███  ███",
                      "  ██████", },

        new string[]{ "█  ██  █", // 4
                      "████████",
                      "█  ██  █", },

        new string[]{ "█  ██  █", // 5
                      "███  ███",
                      "█  ██  █", },

        new string[]{ "  █  █  ", // 6
                      "████████",
                      "  █  █  ", },
    };
    // Used for printing dice rolls

    var splash = @"
    ██████████████████████████████████████████████████████
    ██                                                  ██
    ██                ~~~~~~~~~~~~~~~~~~                ██
    ██     RYAN GOSLING'S AMAZING GAMBLING SIMULATOR    ██
    ██                ~~~~~~~~~~~~~~~~~~                ██
    ██                                                  ██
    ██████████████████████████████████████████████████████
    ██                                                  ██
    ██              Press any key to start              ██
    ██                                                  ██
    ██████████████████████████████████████████████████████";
    // Used for the start screen

    #endregion Initial Setup

    #region Game Intro

    WriteLine(splash);

    ReadKey();

    SetCursorPosition(0,0);
    WriteLine(splash.Replace('█', '▓'));
    Sleep(500);

    SetCursorPosition(0,0);
    WriteLine(splash.Replace('█', '▒'));
    Sleep(250);

    SetCursorPosition(0,0);
    WriteLine(splash.Replace('█', '░'));
    Sleep(125);

    Clear();

    #endregion Game Intro

GameLoop:

    #region User Betting

    WriteLine(@"
    ███████████████████████████████████████████████████████████
    ██ Pick a bet to place!                                  ██
    ███████████████████████████████████████████████████████████
    ██                                                       ██
    ██  1. Big             2. Small           3. Odd         ██
    ██  4. Even            5. All 1s          6. All 2s      ██
    ██  7. All 3s          8. All 4s          9. All 5s      ██
    ██  10. All 6s         11. Double 1s      12. Double 2s  ██
    ██  13. Double 3s      14. Double 4s      15. Double 5s  ██
    ██  16. Double 6s      17. Any triples    18. 4 or 17    ██
    ██  19. 5 or 16        20. 6 or 15        21. 7 or 14    ██
    ██  22. 8 or 13        23. 9 or 12        24. 10 or 11   ██
    ██                                                       ██
    ███████████████████████████████████████████████████████████
       Current Balance : ${0}

       Hit enter to input a number for a bet
       or to view bet descriptions (no input)", userBalance);
    // Gives user the necessary info for betting
    // They also have the option to read descriptions in a separate screen

    int userBetType;
    string userInput;
    // We have a special response for null user inputs

    while (!(int.TryParse(userInput = ReadLine(), out userBetType) && userBetType > 0 && userBetType < 25))
    // If bet type is an int and in range
    {
        if (string.IsNullOrEmpty(userInput))
        {
            Clear();
            goto GameInfo;
        }
        // If user has a blank input they can see more info
    }

    Clear();
    WriteLine(@"
    ██████████████████████████████████████████████████████
    ██                                                  ██
    ██          How much would you like to bet?         ██
    ██                   ( Min : $ 1 )                  ██
    ██                                                  ██
    ██████████████████████████████████████████████████████
       Current Balance : ${0}

       Hit enter to input your bet amount
       or to go back to the bet menu (no input)", userBalance);

    int userBetAmount;
    while (!(int.TryParse(userInput = ReadLine(), out userBetAmount) && userBetAmount > 0 && userBetAmount <= userBalance))
    // If bet amount is an int and in range
    {
        if (string.IsNullOrEmpty(userInput))
        {
            Clear();
            goto GameLoop;
        }
        // If user has a blank input they can go back
    }

    Clear();
    // Clear so if a user spams inputs it wont appear under the dice
    
    #endregion User Betting
    
    #region Dice Rolling
    
    int[] userRoll = null;
    // The initial value is always overriden but the compiler is forcing me to use one anyway lol

    for (int r = 0; r < 14; r++)
    {
        Sleep(75);
        SetCursorPosition(0, 0);
        // Time delay & resetting cursor position for flair

        userRoll = new int[] { random.Next(1, 7), random.Next(1, 7), random.Next(1, 7) };
        // Generates 3 randomized dice rolls

        WriteLine(@"
    ██████████████████████████████████████████████████████
    ██                                                  ██
    ██     ██████████     ██████████     ██████████     ██");
        for (int p = 0; p < 3; p++)
        {
            Write("    ██");
            for (int i = 0; i < 3; i++)
            {
                Write("     █{0}█", diceFaces[userRoll[i]][p]);
            }
            Write("     ██\n");
        }
        WriteLine(
@"    ██     ██████████     ██████████     ██████████     ██
    ██                                                  ██
    ██████████████████████████████████████████████████████");
        // Ascii art of the dice roll results
        // Very messy code but it basically just prints 3 dice
    }

    #endregion Dice Rolling

    #region Determining Outcome

    var betWon       = false;
    var sum          = userRoll[0] +  userRoll[1] +  userRoll[2]; // sum cached for ease of access
    var isTriple     = userRoll[0] == userRoll[1] && userRoll[1] == userRoll[2];
    
    var chosenNumber = -1;               // Used for all triples/doubles
    var betRange     = userBetType - 17; // Used as an offset for the two result bets (IE: 6 or 7)

    if ((userBetType >= 11) && userBetType < 17)
    {
        chosenNumber = userBetType - 10; // Specific doubles
    }
    else if ((userBetType >= 5) && userBetType < 11)
    {
        chosenNumber = userBetType - 4; // Specific triples
    }
    
    switch (userBetType)
    {
        case 1 when sum >= 11 && sum <= 17:                                 // BIG
        case 2 when sum >= 4  && sum <= 10:                                 // SMALL
        case 3 when sum % 2 == 1:                                           // ODD
        case 4 when sum % 2 == 0:                                           // EVEN
        case >= 5  and < 11 when isTriple && userRoll[0] == chosenNumber:   // Specific triple
        case >= 11 and < 17 when !isTriple && (                             
              (userRoll[0] == userRoll[1] || userRoll[1] == userRoll[2]     
            || userRoll[0] == userRoll[2]) && userRoll[0] == chosenNumber): // Specific double
        case 17 when isTriple:                                              // Any triple
        case >= 18 when sum == 3 + betRange || sum == 18 - betRange:        // Bets for specific sum ranges like 4 or 17, 5 or 16, etc.
        betWon = true;
        break;
    }
    
    #endregion Determining Outcome

    #region End Results

    WriteLine(@"
    ██████████████████████████████████████████████████████
    ██                                                  ██
    ██                     You {0}                     ██
    ██                                                  ██
    ██████████████████████████████████████████████████████",
    betWon ? "Won!" : "Lost");

    if (betWon)
    {
        WriteLine(@"
       Gained      : ${0}
       Old Balance : ${1}
       New Balance : ${2}",
     /*Gained     */ userBetAmount * odds[userBetType],
     /*Old Balance*/ userBalance,
     /*New Balance*/ userBalance + userBetAmount * odds[userBetType]);

        userBalance += userBetAmount * odds[userBetType];
    }
    else
    {
        WriteLine(@"
       Lost        : ${0}
       Old Balance : ${1}
       New Balance : ${2}",
     /*Gained     */ userBetAmount,
     /*Old Balance*/ userBalance,
     /*New Balance*/ userBalance - userBetAmount);

       userBalance -= userBetAmount;
    }

    WriteLine("\n Press any key to continue");

    ReadKey();

    Clear();

    if (userBalance > 0 && userBalance < 100_000)
    {
        goto GameLoop;
    }

    #endregion End Results

    #region Game Win/Loss

    if (userBalance > 100_000)
    {
        WriteLine("You have drained the casino of all its money! Good job!\nFinal Balance : ${0}", userBalance);
    }
    else if (userBalance == 0)
    {
        WriteLine("You lost all your money! Go get some more!");
    }
    WriteLine("\nPress Enter to try again, or Escape to quit");
    // Win/Lose text

    ConsoleKey userKey;
    while ((userKey = ReadKey().Key) is not ConsoleKey.Escape)
    {
        if (userKey is ConsoleKey.Enter)
        {
            Clear();
            goto GameStart;
        }
    }
    // Wait for a valid user input, Escape quits & Enter continues the loop

    return;
    // Game is done!

#endregion Game Win/Loss

GameInfo:

    #region Info Text

    SetCursorPosition(0, 0);
    WriteLine(@"
    ███████████████████████████████████████████████████████████████████████████████████████████
    ██ Available bets to choose from!                                                        ██
    ██████████████████████████████████████████████████████████████████████████████| ODDS |█████
    ██                                                                                       ██
    ██  1. Big __________________________________________________________________ (1 to 1)   ██
    ██ The sum of the dice is between 11 and 17 (inclusive) with the exception of a triple   ██
    ██                                                                                       ██
    ██  2. Small ________________________________________________________________ (1 to 1)   ██
    ██ The sum of the dice is between 4 and 10 (inclusive) with the exception of a triple    ██
    ██                                                                                       ██
    ██  3. Odd __________________________________________________________________ (1 to 1)   ██
    ██ The sum of the dice is an ODD number with the exception of a triple                   ██
    ██                                                                                       ██
    ██  4. Even _________________________________________________________________ (1 to 1)   ██
    ██ The sum of the dice is an EVEN number with the exception of a triple                  ██
    ██                                                                                       ██
    ██  5-10. Specific Triples ________________________________________________ (180 to 1)   ██
    ██ Specific triples are rolled                                                           ██
    ██                                                                                       ██
    ██  11-16. Specific Doubles ________________________________________________ (10 to 1)   ██
    ██ Specific doubles are rolled with the exception of a triple                            ██
    ██                                                                                       ██
    ██  17. Any Triples ________________________________________________________ (30 to 1)   ██
    ██ Any triples are rolled                                                                ██
    ██                                                                                       ██
    ██████████████████████████████████████████████████████████████████████████████|(1) ->|█████");
    // Bet descriptions page 1
    goto UserInfoResponse;

ExtendedInfo:

    SetCursorPosition(0, 0);
    WriteLine(@"
    ███████████████████████████████████████████████████████████████████████████████████████████
    ██ Available bets to choose from!                                                        ██
    ██████████████████████████████████████████████████████████████████████████████| ODDS |█████
    ██                                                                                       ██
    ██  18. 4 or 17 ____________________________________________________________ (60 to 1)   ██
    ██ The sum of the dice is 4 or 17                                                        ██
    ██                                                                                       ██
    ██  19. 5 or 16 ____________________________________________________________ (30 to 1)   ██
    ██ The sum of the dice is 5 or 16                                                        ██
    ██                                                                                       ██
    ██  20. 6 or 15 ____________________________________________________________ (18 to 1)   ██
    ██ The sum of the dice is 6 or 15                                                        ██
    ██                                                                                       ██
    ██  21. 7 or 14 ____________________________________________________________ (12 to 1)   ██
    ██ The sum of the dice is 7 or 14                                                        ██
    ██                                                                                       ██
    ██  22. 8 or 13 _____________________________________________________________ (8 to 1)   ██
    ██ The sum of the dice is 8 or 13                                                        ██
    ██                                                                                       ██
    ██  23. 9 or 12 _____________________________________________________________ (7 to 1)   ██
    ██ The sum of the dice is 9 or 12                                                        ██
    ██                                                                                       ██
    ██  24. 10 or 11 ____________________________________________________________ (6 to 1)   ██
    ██ The sum of the dice is 10 or 11                                                       ██
    ██                                                                                       ██
    ██████████████████████████████████████████████████████████████████████████████|<- (2)|█████");
    // Bet descriptions page 2
    goto UserInfoResponse;

UserInfoResponse:

    Write(@"
       Winning a bet nets you your bet amount multiplied by the odds against it
       Press Enter to return to betting screen, arrow keys to navigate pages");

    ConsoleKey userInfoResponse;
    while ((userInfoResponse = ReadKey().Key) is not ConsoleKey.Enter)
    {
        if (userInfoResponse is ConsoleKey.LeftArrow)
        {
            goto GameInfo;
        }

        if (userInfoResponse is ConsoleKey.RightArrow)
        {
            goto ExtendedInfo;
        }
    }
    // Go to desired info screen if its an arrow key input

    Clear();
    goto GameLoop;
    // Otherwise we return to the betting screen

    #endregion Info Text
