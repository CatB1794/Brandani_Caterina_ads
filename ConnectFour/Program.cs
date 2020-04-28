using System;
using System.Collections.Generic;

namespace ConnectFour
{
    public class Program
    {
        public static bool game = true;

        public static bool player = true;

        public static Random pcPlayer = new Random();

        public static int boardCol = 6;

        public static int boardRow = 5;

        public static int[,] Board = new int[boardCol, boardRow];

        public static Stack<GameMoves> Undo = new Stack<GameMoves>();

        public static Queue<GameMoves> Replay = new Queue<GameMoves>();

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

            Console.WriteLine("Welcome to Connect 4!\nThis is a console version of the classic game where each player tries to connect 4 tiles in a row.\nThis can be done horizontally, vertically and in a diagonal.");

            Console.WriteLine("Do you wish to play against the AI or a human counterpart?\nEnter c for computer or h for human:\n(Otherwise if you wish to watch the AI's dish it out enter a)");

            string pcOrPeeps = "";

            while (pcOrPeeps != "h" && pcOrPeeps != "c" && pcOrPeeps != "a" && pcOrPeeps != "H" && pcOrPeeps != "C" && pcOrPeeps != "A")
            {
                pcOrPeeps = Console.ReadLine();

                if (pcOrPeeps != "h" && pcOrPeeps != "c" && pcOrPeeps != "a" && pcOrPeeps != "H" && pcOrPeeps != "C" && pcOrPeeps != "A")
                {
                    Console.WriteLine("That is neither a human or an AI option, try again.");

                    continue;
                }
            }

            Console.WriteLine("Game rules: enter a column number between 1-" + (boardCol - 1) + " to play.\n");

            PrintBoard();

            if (pcOrPeeps == "h" || pcOrPeeps == "H")
            {
                PvP();
            }
            else if (pcOrPeeps == "c" || pcOrPeeps == "C")
            {
                PvPC();
            }
            else if (pcOrPeeps == "a" || pcOrPeeps == "A")
            {
                PCvPC();
            }
        }

        //Prints the game board with the rows and colums numbered.
        public static void PrintBoard()
        {
            for (int x = 1; x < boardCol; x++)
            {
                Console.Write("   " + x);
            }

            Console.Write("\n");

            for (int x = 1; x < boardCol; x++)
            {
                Console.Write("   -");
            }

            Console.Write("\n");

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
                        Console.Write("| " + y);
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

        //Player vs player method to be called when pcOrPeeps = "h" is entered.
        public static void PvP()
        {
            while (game)
            {
                int move;

                if (player)
                {
                    move = 1;

                    Console.WriteLine("Enter player 1 column: ");
                }
                else
                {
                    move = 2;

                    Console.WriteLine("Enter player 2 column: ");
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

                    Undo.Push(new GameMoves(clmn, rowNum, 0));
                }

                PrintBoard();

                MoveUndo();

                player = !player;

                Replay.Enqueue(new GameMoves(clmn, rowNum, move));

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

        //Player vs AI method to be called when pcOrPeeps = "c" is entered.
        public static void PvPC()
        {
            while (game)
            {
                int ai = 0;

                int clmn = 0;

                int move;

                if (player)
                {
                    move = 1;

                    Console.WriteLine("Enter player column: ");

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
                    move = 2;

                    ai = PCPlayer(pcPlayer);

                    Console.WriteLine("AI's turn: " + ai);
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

                        Undo.Push(new GameMoves(clmn, rowNum, 0));

                        PrintBoard();

                        MoveUndo();

                        Replay.Enqueue(new GameMoves(clmn, rowNum, move));
                    }
                }
                else
                {
                    Board[ai, aiRow] = move;

                    PrintBoard();

                    Replay.Enqueue(new GameMoves(ai, aiRow, move));
                }

                player = !player;

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

        //AI vs AI method to be called when pcOrPeeps = "a" is entered.
        public static void PCvPC()
        {
            while (game)
            {
                int move;

                if (player)
                {
                    move = 1;

                    int ai1 = PCPlayer(pcPlayer);

                    Console.WriteLine("AI 1's turn: " + ai1);

                    int ai1Row = GetPosition(ai1);

                    Board[ai1, ai1Row] = move;

                    Replay.Enqueue(new GameMoves(ai1, ai1Row, move));
                }
                else
                {
                    move = 2;

                    int ai2 = PCPlayer(pcPlayer);

                    Console.WriteLine("AI 2's turn: " + ai2);

                    int ai2Row = GetPosition(ai2);

                    Board[ai2, ai2Row] = move;

                    Replay.Enqueue(new GameMoves(ai2, ai2Row, move));
                }

                player = !player;

                PrintBoard();

                if (GameWon(move))
                {
                    Console.WriteLine("AI " + move + " won.");

                    GameExit();

                    return;
                }
                else if (GameTied() && !GameWon(move))
                {
                    Console.WriteLine("The game has been tied, neither AI wins.");

                    GameExit();

                    return;
                }
            }
        }

        //Checks whether the board is full.
        public static bool GameTied()
        {
            if (Replay.Count < ((boardCol - 1) * (boardRow - 1)))
            {
                return false;
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
            for (x = 1; x < (boardCol - 3); x++)
            {
                for (y = 4; y < boardRow; y++)
                {
                    if (Board[x, y] == move && Board[x + 1, y - 1] == move && Board[x + 2, y - 2] == move && Board[x + 3, y - 3] == move)
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

            while (GetPosition(move) < 1)
            {
                move = rnd.Next(1, boardCol);
            }

            return move;
        }

        //Removes the last data input and reprints the game board with the previous board state.
        public static void MoveUndo()
        {
            string undoMove = "_";

            while (undoMove != "u" && undoMove != "U" && undoMove != "")
            {
                Console.WriteLine("If you wish to undo press u, otherwise press enter to continue.");

                undoMove = Console.ReadLine();

                if (undoMove != "u" && undoMove != "U" && undoMove != "")
                {
                    Console.WriteLine("That is not valid input, try again.");

                    continue;
                }
                else if (undoMove == "u" || undoMove == "U")
                {
                    GameMoves undo = Undo.Pop();

                    Board[undo.X, undo.Y] = 0;

                    player = !player;

                    PrintBoard();
                }
            }
        }

        //Once the game has finished it allows the user to review the game from the starting move, through the saved board states in the Replay Queue.
        public static void GameReplay()
        {
            string replayGame = "_";

            while (replayGame != "Y" && replayGame != "y" && replayGame != "")
            {
                Console.WriteLine("If you wish to review the game enter y, otherwise press enter to proceed to game exit.");

                replayGame = Console.ReadLine();

                if (replayGame == "Y" || replayGame == "y")
                {
                    for (int x = 1; x < boardCol; x++)
                    {
                        for (int y = 1; y < boardRow; y++)
                        {
                            Board[x, y] = 0;
                        }
                    }

                    for (int c = Replay.Count; c > 0; c--)
                    {
                        GameMoves replay = Replay.Dequeue();

                        Board[replay.X, replay.Y] = replay.Player;

                        PrintBoard();

                        Console.WriteLine("Press any key to continue.");

                        Console.ReadKey();
                    }
                }
                else if (replayGame != "Y" && replayGame != "y" && replayGame != "")
                {
                    Console.WriteLine("That is not valid input, try again.");

                    continue;
                }
            }
        }

        //Waits for a user input before exiting the game, allowing the user to be able to see who won, instead of immediately closing the console.
        public static void GameExit()
        {
            GameReplay();

            Console.WriteLine("Press any key to exit.");

            Console.ReadKey();
        }
    }

    //Gets and sets the values needed to be able to do the MoveUndo and GameReplay methods.
    public class GameMoves
    {
        public GameMoves(int x, int y, int player)
        {
            this.X = x;

            this.Y = y;

            this.Player = player;
        }

        public int X { get; set; }

        public int Y { get; set; }

        public int Player { get; set; }
    }
}