using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd_Linux
{
    public static class Breakout
    {
        static public int execute_breakout()
        {
            /*
             * 0 = vide
             * 1 = joueur
             * 2 = obstacle
             * 3 = balle
            */
            init_window();
            int[][] board = init_board();
            print_board(board);
            Console.Write("WORK IN PROGRESS");
            Console.ReadKey(true);
            end_game_window();
            return (0);
        }

        static private void init_window()
        {
            Console.Clear();
            if (Console.WindowHeight < 5 || Console.WindowWidth < 5)
            {
                Console.SetWindowSize(5, 5);
            }
            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
        }

        static private void end_game_window()
        {
            Console.Clear();
            Console.SetBufferSize(Console.WindowWidth, 300);
        }

        static private int[][] init_board()
        {
            int[][] board = new int[Console.WindowWidth][];
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                board[i] = new int[Console.WindowHeight];
                for (int j = 0; j < Console.WindowHeight - 4; j++)
                {
                    board[i][j] = 2;
                }
            }

            board[0][Console.WindowHeight - 1] = 1;
            board[1][Console.WindowHeight - 1] = 1;
            board[2][Console.WindowHeight - 1] = 1;
            board[1][Console.WindowHeight - 2] = 3;

            return (board);
        }

        static private void print_board(int[][] board)
        {
            for(int i = 0; i < board[0].GetLength(0); i++)
            {
                for(int j = 0; j < board.GetLength(0); j++)
                {
                    Console.Write(get_skin(board[j][i]));
                }
            }
        }

        static private char get_skin(int id)
        {
            switch(id)
            {
                case (0):
                    return (' ');
                case (1):
                    return ('_');
                case (2):
                    return ('X');
                case (3):
                    return ('*');
                default:
                    return ('\0');
            }
        }

        static private void apply_player_action(ConsoleKeyInfo key_pressed)
        {
            switch(key_pressed.Key)
            {
                case (ConsoleKey.RightArrow):
                    //move right if possible
                    break;
                case (ConsoleKey.LeftArrow):
                    //move left if possible
                    break;
                case (ConsoleKey.P):
                    //pause
                    break;
                case (ConsoleKey.Escape):
                    //leave
                    break;
            }
        }
    }
}
