using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Farkle
{
    class Program
    {
        static string[] die = new string[]
        {
            " _______ ",
            "|       |",
            "|   0   |",
            "|_______|"
        };

        static void Main(string[] args)
        {
            var rand = new Random();
            List<int> diceRolls = new List<int>();
            int pointsTotal = 0,
                pointsTurn = 0,
                compPoints = 0;
            string userInput = "";
            bool throwAwayDice = false;
            Console.WriteLine("FARKLE");
            Console.WriteLine();

            /*
            diceRolls = new List<int>() { rand.Next(1, 7), rand.Next(1, 7), rand.Next(1, 7), rand.Next(1, 7), rand.Next(1, 7), rand.Next(1, 7) };
            Console.WriteLine($"Your numbers: {diceRolls[0]}, {diceRolls[1]}, {diceRolls[2]}, {diceRolls[3]}, {diceRolls[4]}, {diceRolls[5]}");
            Console.Write("Please pick your numbers: ");
            userInput = Console.ReadLine();
            Console.WriteLine($"Validation: {Validate(userInput,diceRolls)}");
            Console.WriteLine();
            Console.WriteLine();
            */

            while(pointsTotal < 10000 && compPoints < 10000)
            {
                diceRolls.Clear();
                pointsTurn = 0;
                throwAwayDice = false;

                while (diceRolls.Count <= 5)
                {

                    if (diceRolls.Count == 0)
                    {
                        diceRolls = new List<int>() { rand.Next(1, 7), rand.Next(1, 7), rand.Next(1, 7), rand.Next(1, 7), rand.Next(1, 7), rand.Next(1, 7) };
                        Console.WriteLine($"Your numbers: {diceRolls[0]}, {diceRolls[1]}, {diceRolls[2]}, {diceRolls[3]}, {diceRolls[4]}, {diceRolls[5]}");
                        
                        do
                        {
                            Console.Write("Please enter the numbers you'd like to keep: ");
                            userInput = Console.ReadLine();

                        } while (!Validate(userInput,diceRolls));

                        diceRolls.Clear();

                        MatchCollection matches = Regex.Matches(userInput, @"\d");

                        // score the dice
                        pointsTurn += ScoreNumbers(matches);

                        // keep the dice
                        foreach (Match match in matches)
                        {
                            diceRolls.Add(Int32.Parse(match.Value));
                        }

                    }
                    else
                    {
                        // if last die left, no point in asking user whether to keep it
                        if (diceRolls.Count == 5)
                        {
                            var lastRoll = rand.Next(1, 7);
                            diceRolls.Add(lastRoll);
                            MatchCollection match = Regex.Matches(lastRoll.ToString(), @"\d");
                            pointsTurn += ScoreNumbers(match);
                            Console.Write("Your final numbers: ");
                            foreach (var die in diceRolls)
                            {
                                Console.Write(die + ", ");
                            }
                            Console.WriteLine();
                            continue;
                        }

                        var diceRoll = 0;
                        Console.Write("The dice you've kept: ");
                        foreach (var die in diceRolls)
                        {
                            Console.Write(die + ", ");
                        }
                        Console.WriteLine();

                        List<int> temp = new List<int>();
                        Console.Write("Your numbers: ");
                        for (int j = 0; j < 6 - diceRolls.Count; j++)
                        {
                            diceRoll = rand.Next(1, 7);
                            temp.Add(diceRoll);
                            Console.Write(diceRoll + ", ");
                        }
                        Console.WriteLine();

                        do
                        {
                            Console.Write("Please enter the numbers you'd like to keep (to throw away your dice, type q): ");
                            userInput = Console.ReadLine();
                            if (userInput == "q")
                            {
                                throwAwayDice = true;
                                break;
                            }

                        } while (!Validate(userInput, temp));

                        if (throwAwayDice)
                        {
                            break;
                        }
                        
                        MatchCollection matches = Regex.Matches(userInput, @"\d");

                        // score the dice
                        pointsTurn += ScoreNumbers(matches);

                        // keep the dice
                        foreach (Match match in matches)
                        {
                            diceRolls.Add(Int32.Parse(match.Value));
                        }
                    }

                    if (throwAwayDice)
                    {
                        break;
                    }
                }

                pointsTotal += pointsTurn;
                Console.WriteLine($"Your points this turn: {pointsTurn}");
                Console.WriteLine($"Your total points so far: {pointsTotal}");
                Console.WriteLine();
                Console.WriteLine();

                compPoints += ComputerTurn(diceRolls);
                Console.WriteLine($"The computer has a total of {compPoints} points so far");
                Console.WriteLine();
                Console.WriteLine();
            }

            if (pointsTotal == compPoints)
            {
                Console.WriteLine("You guys tied.");
            }
            else
            {
                if (pointsTotal >= 10000)
                {
                    Console.WriteLine("Congrats, you won!");
                }
                else
                {
                    Console.WriteLine("Ooh sorry, you lost!");
                }
                
            }
        }
        
        static bool Validate(string input, List<int> diceRolls)
        {
            // make sure values entered by user are only numbers 1-6, and that there is at least one digit and no more than 6, the number of dice rolled
            MatchCollection matches = Regex.Matches(input, @"[1-6]");
            if (matches.Count == 0 || matches.Count > 6 || matches.Count < input.Length)
            {
                return false;
            }
            else
            {
                // make sure all user values actually correspond to the numbers rolled
                List<int> userValues = new List<int>();
                bool noMatch = false;
                bool matchFound = false;
                int seekCounter = 0;
                foreach (Match match in matches)
                {
                    userValues.Add(Int32.Parse(match.Value));
                }
                userValues.Sort();
                diceRolls.Sort();

                // compare the two sorted lists
                for (int i = 0; i < userValues.Count; i++)
                {
                    matchFound = false;

                    for (int j = seekCounter; j < diceRolls.Count; j++)
                    {
                        if (userValues[i] == diceRolls[j])
                        {
                            matchFound = true;
                            seekCounter = j + 1;
                            break;
                        }
                    }

                    if (matchFound)
                    {
                        continue;
                    }

                    noMatch = true;

                }

                if (noMatch)
                {
                    return false;
                }

                return true;
            }
        }

        static int ScoreNumbers(MatchCollection matches)
        {
            int points = 0;

            // store the number of times each number occurs
            int[] numberCount = new int[6];

            foreach (Match match in matches)
            {
                switch (Int32.Parse(match.Value))
                {
                    case 1:
                        numberCount[0]++;
                        break;
                    case 2:
                        numberCount[1]++;
                        break;
                    case 3:
                        numberCount[2]++;
                        break;
                    case 4:
                        numberCount[3]++;
                        break;
                    case 5:
                        numberCount[4]++;
                        break;
                    case 6:
                        numberCount[5]++;
                        break;
                    default:
                        break;
                }
            }

            // score the numbers
            for (int i = 0; i < numberCount.Length; i++)
            {
                if (numberCount[i] == 3)
                {
                    switch (i + 1)
                    {
                        case 1:
                            points += 1000;
                            break;
                        case 2:
                            points += 200;
                            break;
                        case 3:
                            points += 300;
                            break;
                        case 4:
                            points += 400;
                            break;
                        case 5:
                            points += 500;
                            break;
                        case 6:
                            points += 600;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    if (i == 0)
                    {
                        if (numberCount[i] < 3)
                        {
                            // if there are fewer than three 1's give 100 each
                            points += numberCount[i] * 100;
                        }
                        else
                        {
                            // there must be more than 3, give 1000 for first 3, and 100 there after
                            points += 1000;
                            numberCount[i] -= 3;
                            points += numberCount[i] * 100;
                        }

                    }

                    if (i == 4)
                    {
                        if (numberCount[i] < 3)
                        {
                            // if there are fewer than three 5's give 50 each
                            points += numberCount[i] * 50;
                        }
                        else
                        {
                            // there must be more than 3, give 500 for first 3, and 50 there after
                            points += 500;
                            numberCount[i] -= 3;
                            points += numberCount[i] * 50;
                        }

                    }
                    
                }

            }

            return points;

        }

        static int ScoreNumbers(List<int> numbers)
        {
            int points = 0;

            // store the number of times each number occurs
            int[] numberCount = new int[6];

            foreach (var number in numbers)
            {
                switch (number)
                {
                    case 1:
                        numberCount[0]++;
                        break;
                    case 2:
                        numberCount[1]++;
                        break;
                    case 3:
                        numberCount[2]++;
                        break;
                    case 4:
                        numberCount[3]++;
                        break;
                    case 5:
                        numberCount[4]++;
                        break;
                    case 6:
                        numberCount[5]++;
                        break;
                    default:
                        break;
                }
            }

            // score the numbers
            for (int i = 0; i < numberCount.Length; i++)
            {
                if (numberCount[i] == 3)
                {
                    switch (i + 1)
                    {
                        case 1:
                            points += 1000;
                            break;
                        case 2:
                            points += 200;
                            break;
                        case 3:
                            points += 300;
                            break;
                        case 4:
                            points += 400;
                            break;
                        case 5:
                            points += 500;
                            break;
                        case 6:
                            points += 600;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    if (i == 0)
                    {
                        if (numberCount[i] < 3)
                        {
                            // if there are fewer than three 1's give 100 each
                            points += numberCount[i] * 100;
                        }
                        else
                        {
                            // there must be more than 3, give 1000 for first 3, and 100 there after
                            points += 1000;
                            numberCount[i] -= 3;
                            points += numberCount[i] * 100;
                        }

                    }

                    if (i == 4)
                    {
                        if (numberCount[i] < 3)
                        {
                            // if there are fewer than three 5's give 50 each
                            points += numberCount[i] * 50;
                        }
                        else
                        {
                            // there must be more than 3, give 500 for first 3, and 50 there after
                            points += 500;
                            numberCount[i] -= 3;
                            points += numberCount[i] * 50;
                        }

                    }

                }

            }

            return points;

        }

        static int ComputerTurn(List<int> diceRolls)
        {
            diceRolls.Clear();
            int PointsTurn = 0,
                Points = 0,
                DiceRoll = 0;
            List<int> TempList;
            var rand = new Random();

            while (diceRolls.Count < 6)
            {
                if (diceRolls.Count == 0)
                {
                    diceRolls = new List<int>() { rand.Next(1, 7), rand.Next(1, 7), rand.Next(1, 7), rand.Next(1, 7), rand.Next(1, 7), rand.Next(1, 7) };
                    Console.WriteLine($"The computer rolled: {diceRolls[0]}, {diceRolls[1]}, {diceRolls[2]}, {diceRolls[3]}, {diceRolls[4]}, {diceRolls[5]}");
                    TempList = PickNumbers(diceRolls);
                    Points = ScoreNumbers(TempList);
                    diceRolls.Clear();

                    foreach (var number in TempList)
                    {
                        diceRolls.Add(number);
                    }

                    TempList.Clear();
                    Console.Write("The computer kept numbers ");
                    foreach (var number in diceRolls)
                    {
                        Console.Write(number + ", ");
                    }
                    Console.Write($"for a total of {Points} points.");
                    Console.WriteLine();

                    PointsTurn = Points;
                }
                else
                {
                    TempList = new List<int>();
                    Points = 0;
                    Console.Write("The computer rolled: ");
                    for (int i = 0; i < 6 - diceRolls.Count; i++)
                    {
                        DiceRoll = rand.Next(1, 7);
                        TempList.Add(DiceRoll);
                        Console.Write(DiceRoll + ", ");
                    }

                    TempList = PickNumbers(TempList);
                    Points += ScoreNumbers(TempList);

                    Console.Write("The computer kept numbers ");
                    foreach (var number in TempList)
                    {
                        Console.Write(number + ", ");
                        diceRolls.Add(number);
                    }
                    Console.Write($"for a total of {Points} points.");
                    Console.WriteLine();

                    PointsTurn += Points;
                }
            }

            Console.WriteLine($"The computer scored a total of {PointsTurn} points this turn");

            return PointsTurn;
        }

        static List<int> PickNumbers(List<int> numbers)
        {
            numbers.Sort();
            List<int> TempList = new List<int>();
            if (numbers.Count <= 2)
            {
                return numbers;
            }
            else
            {
                // create a counter to determine if a number occurs 3x
                int NumCount = 0;
                // pick the numbers
                for (int i = 0; i < numbers.Count; i++)
                {
                    // always keep a roll of 1
                    if (numbers[i] == 1)
                    {
                        TempList.Add(numbers[i]);
                    }
                    else
                    {
                        // look for triples by comparing the current number with the one before it in a sorted list
                        if (i >= 1 && numbers[i] == numbers[i-1])
                        {
                            
                            NumCount++;

                            // if NumCount = 2, then a triple has been found, keep all three numbers
                            if (NumCount >= 2 && numbers[i] == numbers[i-2])
                            {
                                TempList.Add(numbers[i]);
                                TempList.Add(numbers[i]);
                                TempList.Add(numbers[i]);
                                NumCount = 0;
                            }
                        }

                        // if we've gotten to the end of the list of numbers without keeping anything, keep at least one number
                        // and make it a 5 if there is one, because that's at least 50 points
                        if (i == numbers.Count - 1 && TempList.Count == 0)
                        {
                            if (numbers.Contains(5))
                            {
                                TempList.Add(5);
                            }
                            else
                            {
                                TempList.Add(numbers[0]);
                            }
                        }
                    }
                }
            }

            return TempList;
        }


        static void DisplayDice(int[] diceRolls)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < diceRolls.Length; j++)
                {
                    if (i == 2)
                    {
                        die[2] = $"|   {diceRolls[j]}   |";
                    }

                    Console.Write(die[i] + "    ");
                }

                Console.WriteLine();
            }
        }
    }
}
