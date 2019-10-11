using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace Snake_v2
{
    class Program
    {
        public static char charApple = '*'; //Apple char
        public static char charSnake = '█'; //Snakechar
        public static int width = Console.WindowWidth;
        public static int height = Console.WindowHeight;

        public static int difficulty;
        public static SnakeBody snake;
        public static bool isDead;
        public static Timer ticker;
        public static direction currentDirection;
        public static direction lastMove;
        public static int point;
        public static PossibleAppleLoc appleLoc;
        public static Apple apple;
        public static string code = "";
        public static bool godlike;
        public static bool goThroughWalls;
        public static int HighScore = 0;

        static void Main(string[] args)
        {
        START:
            Console.ResetColor();
            Console.Clear();
            godlike = false;
            goThroughWalls = false;
            currentDirection = direction.Down;
            lastMove = direction.Down;
            point = 0;
            isDead = false;
            snake = new SnakeBody();
            appleLoc = new PossibleAppleLoc();
            apple = new Apple();
            ticker = new Timer();

            Console.Write("Difficulty: <1-5>: ");

            while (!int.TryParse(Console.ReadKey().KeyChar.ToString(), out difficulty) || difficulty < 1 || difficulty > 5)
            {
                Console.Clear();
                Console.WriteLine("-Incorrect format-");
                Console.Write("Difficulty <1-5>: ");
            }

            Console.Clear();
            Console.OutputEncoding = Encoding.Unicode;
            for (int i = 0; i < width; i++)
            {
                Console.SetCursorPosition(i, 0);
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.Write('\0');

                Console.SetCursorPosition(i, height - 1);
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.Write('\0');
            }

            for (int i = 1; i < height; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.Write('\0');

                Console.SetCursorPosition(width - 1, i);
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.Write('\0');
            }

            Console.SetCursorPosition(8, 0);
            Console.Write("<P> to Pause\t<ESC> to Quit\tDifficulty: {0}\tP: 0\tHS: {1}", difficulty, HighScore);
            Console.SetCursorPosition(26, height - 1);
            Console.Write("Made by Stiig \"Neophear\" Gade");
            Console.BackgroundColor = ConsoleColor.Black;

            DrawGame();
            Console.Title = "Snake";
            apple.CreateApple();
            ticker.Interval = 200 / difficulty;
            ticker.Elapsed += ticker_Elapsed;
            ticker.Start();

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if (keyInfo.KeyChar != '\0')
                    code += keyInfo.KeyChar;

                if (Strings.Right(code, "idspispopd".Length) == "idspispopd")
                {
                    goThroughWalls = !goThroughWalls;
                    Console.Title = "Ghosting now, are we?";
                    code = String.Empty;
                }
                if (Strings.Right(code, "iddqd".Length) == "iddqd")
                {
                    godlike = !godlike;
                    Console.Title = "Welcome Marine!";
                    code = String.Empty;
                }
                if (Strings.Right(code, "stiig er awesome".Length) == "stiig er awesome")
                {
                    ticker.Interval = 200;
                    code = String.Empty;
                }

                if (Strings.Right(code, "Henriette".Length) == "henriette")
                {
                    Console.SetCursorPosition(26, height - 1);
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(" <3 Jeg elsker dig Henriette <3 ");
                    Console.ResetColor();
                    code = String.Empty;
                }

                //cuts down the code string when it gets too long
                if (code.Length > 30)
                    code = code.Substring(20);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.Escape:
                        return;
                    case ConsoleKey.UpArrow:
                        if (lastMove != direction.Down)
                            currentDirection = direction.Up;
                        break;
                    case ConsoleKey.DownArrow:
                        if (lastMove != direction.Up)
                            currentDirection = direction.Down;
                        break;
                    case ConsoleKey.LeftArrow:
                        if (lastMove != direction.Right)
                            currentDirection = direction.Left;
                        break;
                    case ConsoleKey.RightArrow:
                        if (lastMove != direction.Left)
                            currentDirection = direction.Right;
                        break;
                    case ConsoleKey.Y:
                        if (isDead)
                            goto START;
                        break;
                    case ConsoleKey.N:
                        if (isDead)
                            return;
                        else
                            break;
                    case ConsoleKey.P:
                        ticker.Enabled = !ticker.Enabled;
                        break;
                    case ConsoleKey.PageDown:
                        if (difficulty > 1)
                            difficulty--;
                        ChangeDifficulty();
                        break;
                    case ConsoleKey.PageUp:
                        if (difficulty < 5)
                            difficulty++;
                        ChangeDifficulty();
                        break;
                }
            }
        }

        static void ChangeDifficulty()
        {
            ticker.Interval = 200 / difficulty;
            Console.SetCursorPosition(52, 0);
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(difficulty);
            Console.ResetColor();
        }

        static void DrawGame()
        {
            //Draw snake
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(snake.Head.X, snake.Head.Y);
            Console.Write(charSnake);

            Point bodyStart = snake.BodyLocations[snake.BodyLocations.Count - 2];
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.SetCursorPosition(bodyStart.X, bodyStart.Y);
            Console.Write(charSnake);

            Console.SetCursorPosition(snake.LastTailPosition.X, snake.LastTailPosition.Y);
            Console.Write('\0');
        }
        static void ticker_Elapsed(object sender, ElapsedEventArgs e)
        {
            snake.Move();

            DrawGame();

            if (isDead)
            {
                ticker.Stop();
                if (point > HighScore)
                {
                    HighScore = point;
                    WriteOnScreen(30, 11, ConsoleColor.Red, String.Format("New highscore: {0}", HighScore));
                }
                WriteOnScreen(25, 12, ConsoleColor.Red, "You died! Try again? <y/n>");
            }
        }

        private static void WriteOnScreen(int l, int r, ConsoleColor c, string t)
        {
            Console.SetCursorPosition(l, r);
            Console.ForegroundColor = c;
            Console.WriteLine(t);
            Console.ResetColor();
        }
        
        public class SnakeBody
        {
            List<Point> bodyLocations = new List<Point>();
            Point lastTailPosition = new Point(1, 1);

            public Point LastTailPosition
            {
                get { return lastTailPosition; }
                set { lastTailPosition = value; }
            }

            public List<Point> BodyLocations
            {
                get { return bodyLocations; }
                set { bodyLocations = value; }
            }

            public SnakeBody()
            {
                bodyLocations.Add(new Point(1, 1));
                bodyLocations.Add(new Point(1, 2));
            }

            public void Move()
            {
                Point nextLocation = new Point();

                switch (currentDirection)
                {
                    case direction.Up:
                        nextLocation = new Point(Head.X, Head.Y - 1);
                        break;
                    case direction.Down:
                        nextLocation = new Point(Head.X, Head.Y + 1);
                        break;
                    case direction.Left:
                        nextLocation = new Point(Head.X - 1, Head.Y);
                        break;
                    case direction.Right:
                        nextLocation = new Point(Head.X + 1, Head.Y);
                        break;
                    default:
                        break;
                }

                lastMove = currentDirection;

                if (!bodyLocations.Contains(nextLocation) && !isDead) // || bodyLocations[0] == nextLocation
                {
                    if ((nextLocation.X < 1 || nextLocation.X > width - 2 || nextLocation.Y < 1 || nextLocation.Y > height - 2) && !goThroughWalls)
                        isDead = !godlike;
                    else if (goThroughWalls)
                    {
                        if (nextLocation.X < 1)
                            nextLocation.X = width - 2;
                        if (nextLocation.X > width - 2)
                            nextLocation.X = 1;
                        if (nextLocation.Y < 1)
                            nextLocation.Y = height - 2;
                        if (nextLocation.Y > height - 2)
                            nextLocation.Y = 1;

                        nextLocation = CheckIfApple(nextLocation);

                        bodyLocations.Add(nextLocation);
                    }
                    else
                    {
                        nextLocation = CheckIfApple(nextLocation);

                        bodyLocations.Add(nextLocation);
                    }
                }
                else
                    isDead = !godlike;
            }

            private Point CheckIfApple(Point nextLocation)
            {
                if (nextLocation != apple.Location)
                {
                    lastTailPosition = bodyLocations[0];
                    bodyLocations.RemoveAt(0);
                    appleLoc.AvailablePositions.Remove(nextLocation);
                    appleLoc.AvailablePositions.Add(lastTailPosition);
                }
                else
                {
                    appleLoc.AvailablePositions.Remove(nextLocation);
                    apple.CreateApple();
                    point += difficulty;
                    Console.SetCursorPosition(59, 0);
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.Write(point);
                    Console.ResetColor();
                }
                return nextLocation;
            }

            public Point Head
            {
                get { return bodyLocations.Last<Point>(); }
            }
        }

        public class Apple
        {
            Point location = new Point();

            public Point Location
            {
                get { return location; }
                set { location = value; }
            }

            public Apple()
            {
                location = appleLoc.GetNextPosition();
            }

            public void CreateApple()
            {
                location = appleLoc.GetNextPosition();

                Console.SetCursorPosition(apple.Location.X, apple.Location.Y);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(charApple);
                Console.ResetColor();
            }
        }

        public class PossibleAppleLoc
        {
            List<Point> availablePositions = new List<Point>();

            public List<Point> AvailablePositions
            {
                get { return availablePositions; }
                set { availablePositions = value; }
            }

            public PossibleAppleLoc()
            {
                for (int x = 1; x < width - 2; x++)
                    for (int y = 1; y < height - 2; y++)
                        availablePositions.Add(new Point(x, y));

                availablePositions.RemoveAt(0);
            }

            public Point GetNextPosition()
            {
                Random rand = new Random();

                Point nextPosition = availablePositions[rand.Next(0, availablePositions.Count)];

                return nextPosition;
            }
        }

        public enum direction
        {
            Up,
            Down,
            Left,
            Right
        }
    }
}