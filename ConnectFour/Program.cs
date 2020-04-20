using System;
using System.Collections.Generic;

namespace ConnectFour
{
    public class Program
    {
        public static bool player = true;

        public static Random pcPlayer = new Random();

        public static int boardCol = 6;

        public static int boardRow = 5;

        public static int[,] Board = new int[boardCol, boardRow];

        public static Stack<UndoMove> Undo = new Stack<UndoMove>();

        public static void Main(string[] args)
        {
            //Initialises game board.
            for (int x = 1; x < boardCol; x++)
            {
                for (int y = 1; y < boardRow; y++)
                {
                    Board[x, y] = 0;
                }
            }

            Console.WriteLine("Do you wish to play against the AI or a human counterpart?\nEnter c for computer or h for human:");

            string pcOrPeeps = "";

            while (pcOrPeeps != "h" && pcOrPeeps != "c")
            {
                pcOrPeeps = Console.ReadLine();

                if (pcOrPeeps != "h" && pcOrPeeps != "c")
                {
                    Console.WriteLine("That is neither a human or an AI option, try again.");

                    continue;
                }
            }

            Console.WriteLine("Game rules: enter a column number between 1-" + (boardCol - 1) + " to play.\nIf you wish to undo press u, otherwise press enter to continue.");

            PrintBoard();

            if (pcOrPeeps == "h")
            {
                while (true)
                {
                    if (player)
                    {
                        Console.WriteLine("Enter player 1 column: ");
                    }
                    else
                    {
                        Console.WriteLine("Enter player 2 column: ");
                    }

                    int move;

                    if (player)
                    {
                        move = 1;
                    }
                    else
                    {
                        move = 2;
                    }

                    string column = Console.ReadLine();

                    int clmn;

                    try
                    {
                        clmn = int.Parse(column);
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Not a column, please try again.");

                        continue;
                    }

                    int rowNum = GetPosition(clmn);

                    if (rowNum < 1)
                    {
                        Console.WriteLine("Out of bounds, please try again.");

                        continue;
                    }
                    else
                    {
                        Board[clmn, rowNum] = move;

                        Undo.Push(new UndoMove(clmn, rowNum));
                    }

                    MoveUndo();

                    player = !player;

                    PrintBoard();

                    if (GameWon(move))
                    {
                        Console.WriteLine("Player " + move + " won!");

                        GameExit();

                        return;
                    }
                    else if (GameTied() && !GameWon(move))
                    {
                        Console.WriteLine("The game has been tied, neither player wins.");

                        GameExit();

                        return;
                    }
                }
            }
            else if (pcOrPeeps == "c")
            {
                while (true)
                {
                    int ai = 0;

                    int clmn = 0;

                    if (player)
                    {
                        Console.WriteLine("Enter player 1 column: ");

                        string column = Console.ReadLine();

                        try
                        {
                            clmn = int.Parse(column);
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Not a column, please try again.");

                            continue;
                        }
                    }
                    else
                    {
                        ai = PCPlayer(pcPlayer);

                        Console.WriteLine("AI's turn: " + ai);
                    }

                    int move;

                    if (player)
                    {
                        move = 1;
                    }
                    else
                    {
                        move = 2;
                    }

                    int rowNum = GetPosition(clmn);

                    int aiRow = GetPosition(ai);

                    if (move == 1)
                    {
                        if (rowNum < 1)
                        {
                            Console.WriteLine("Out of bounds, please try again.");

                            continue;
                        }
                        else
                        {
                            Board[clmn, rowNum] = move;

                            Undo.Push(new UndoMove(clmn, rowNum));

                            MoveUndo();
                        }
                    }
                    else
                    {
                        Board[ai, aiRow] = move;
                    }

                    player = !player;

                    PrintBoard();

                    if (GameWon(move))
                    {
                        if (move == 1)
                        {
                            Console.WriteLine("The human won!");

                            GameExit();

                            return;
                        }
                        else
                        {
                            Console.WriteLine("AI won, better luck next time.");

                            GameExit();

                            return;
                        }
                    }
                    else if (GameTied() && !GameWon(move))
                    {
                        Console.WriteLine("The game has been tied, neither player wins.");

                        GameExit();

                        return;
                    }
                }
            }
        }

        //Prints the game board with the rows and colums numbered.
        public static void PrintBoard()
        {
            for (int y = 1; y < boardRow; y++)
            {
                Console.Write(y);

                Console.Write(" |");

                for (int x = 1; x < boardCol; x++)
                {
                    Console.Write(Board[x, y]);

                    if (x < (boardCol - 1))
                    {
                        Console.Write(" - ");
                    }
                    else
                    {
                        Console.Write("|");
                    }
                }

                Console.Write("\n");
            }

            for (int x = 1; x < boardCol; x++)
            {
                Console.Write("   -");
            }

            Console.Write("\n");

            for (int x = 1; x < boardCol; x++)
            {
                Console.Write("   " + x);
            }

            Console.Write("\n");
        }

        //Gets the corresponding row to the column input and checks whether the move is valid.
        public static int GetPosition(int x)
        {
            int move = 0;

            if (x > 0 && x < boardCol)
            {
                for (int y = 1; y < boardRow; y++)
                {
                    if (Board[x, y] == 0)
                    {
                        move = y;
                    }
                }
            }
            else
            {
                return move;
            }

            return move;
        }

        //Checks whether the board is full.
        public static bool GameTied()
        {
            for (int x = 1; x < boardCol; x++)
            {
                for (int y = 1; y < boardRow; y++)
                {
                    if (Board[x, y] == 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        //Checks for 4 in a row, returns true if the winning conditional has been met.
        public static bool GameWon(int move)
        {
            int x, y;

            //Horizontal.
            for (x = 4; x < boardCol; x++)
            {
                for (y = 1; y < boardRow; y++)
                {
                    if (Board[x - 3, y] == move && Board[x - 2, y] == move && Board[x - 1, y] == move && Board[x, y] == move)
                    {
                        return true;
                    }
                }
            }

            //Vertical.
            for (x = 1; x < boardCol; x++)
            {
                for (y = 4; y < boardRow; y++)
                {
                    if (Board[x, y] == move && Board[x, y - 1] == move && Board[x, y - 2] == move && Board[x, y - 3] == move)
                    {
                        return true;
                    }
                }
            }

            //Ascending.
            for (x = 4; x < boardCol; x++)
            {
                for (y = 1; y < (boardRow - 3); y++)
                {
                    if (Board[x - 3, y + 3] == move && Board[x - 2, y + 2] == move && Board[x - 1, y + 1] == move && Board[x, y] == move)
                    {
                        return true;
                    }
                }
            }

            //Descending.
            for (x = 1; x < (boardCol - 3); x++)
            {
                for (y = 4; y < boardRow; y++)
                {
                    if (Board[x + 3, y] == move && Board[x + 2, y - 1] == move && Board[x + 1, y - 2] == move && Board[x, y - 3] == move)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        //Generates a random int and ensures that it is within the board bounds.
        public static int PCPlayer(Random rnd)
        {
            int move = rnd.Next(1, boardCol);

            if (GetPosition(move) < 1)
            {
                move = rnd.Next(1, boardCol);
            }

            return move;
        }

        //Removes the last data input and reprints the game board with the previous board state.
        public static void MoveUndo()
        {
            string undoMove = "_";

            while (undoMove != "u" && undoMove != "")
            {
                undoMove = Console.ReadLine();

                if (undoMove != "u" && undoMove != "")
                {
                    Console.WriteLine("That is not valid input, try again.");

                    continue;
                }
                else if (undoMove == "u")
                {
                    UndoMove undo = Undo.Pop();

                    Board[undo.X, undo.Y] = 0;

                    player = !player;
                }
            }
        }

        //Waits for a user input before exiting the game, allows user to be ablt to see who won, instead of immediately closing the console.
        public static void GameExit()
        {
            Console.WriteLine("Press any key to exit.");

            Console.ReadKey();
        }
    }

    //Gets and sets the values needed to be able to do the MoveUndo method.
    public class UndoMove
    {
        public UndoMove(int x, int y)
        {
            this.X = x;

            this.Y = y;
        }

        public int X { get; set; }

        public int Y { get; set; }
    }
}