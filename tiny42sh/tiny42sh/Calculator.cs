using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd_Linux
{
    public static class Calculator
    {
        public static int execute_calculator(string[] cmd, ref long last_result)
        {
            //5 + 3 * 2 -> 5 + 6 -> 11
            string new_expression = "";
            int i;
            int j;

            for (i = 0; i < cmd.Length; i++)
            {
                new_expression = new_expression + cmd[i];
            }

            if (next_operation_to_do(ref new_expression, last_result))
            {
                try
                {
                    last_result = Convert.ToInt64(new_expression); //juste pour vérifier que le calcul est OK et sauvegarder le dernier calc
                    if(new_expression[0] == '-')
                    {
                        i = 1;
                    }
                    else
                    {
                        i = 0;

                    }
                    j = new_expression.Length - 1; 
                    while(j - 3 >= i)
                    {
                        new_expression = new_expression.Substring(0, j - 2) + '.' + new_expression.Substring(j - 2, new_expression.Length - j + 2);
                        j = j - 3;
                    }
                    Console.WriteLine("> " + new_expression);
                    return (0);
                }
                catch(Exception)
                {
                    if(new_expression != "")
                    {
                        Console.WriteLine("> calc: calcul error");
                    }
                }
            }

            return (1);
        }

        public static int get_result(string[] cmd, ref long last_result)
        {
            //5 + 3 * 2 -> 5 + 6 -> 11
            string new_expression = "";

            for (int i = 0; i < cmd.Length; i++)
            {
                new_expression = new_expression + cmd[i];
            }

            if (next_operation_to_do(ref new_expression, last_result, false))
            {
                try
                {
                    last_result = Convert.ToInt64(new_expression);
                    return (0);
                }
                catch (Exception)
                {

                }
            }

            return (1);
        }

        public static bool is_math_expression(string[] cmd)
        {
            int i = 0;
            int j;
            int nbrpar = 0;

            while(i < cmd.Length)
            {
                j = 0;
                while (j < cmd[i].Length && (Char.IsNumber(cmd[i][j]) || cmd[i][j] == '(' || cmd[i][j] == ')' || cmd[i][j] == '*' || cmd[i][j] == '/' || cmd[i][j] == '+' || cmd[i][j] == '-' || cmd[i][j] == '^' || cmd[i][j] == '%'))
                {
                    if (cmd[i][j] == '(')
                    {
                        nbrpar++;
                    }
                    if (cmd[i][j] == ')')
                    {
                        nbrpar--;
                    }
                    j++;
                }
                if(j == cmd[i].Length)
                {
                    i++;
                }
                else
                {
                    i = cmd.Length + 1;
                }
            }

            return (i == cmd.Length && nbrpar == 0);
        } //on défini un symbole comme mathématique

        private static bool process_calculator(ref string calc, int operation_index, long last_result, bool show_error)
        {
            long nbr1 = 0;
            long nbr2 = 0;

            if(operation_index < calc.Length - 1 || calc[operation_index] == '²')
            {
                if(operation_index > 0)
                {
                    try
                    {
                        if(calc[0] == '-')
                        {
                            nbr1 = -1 * Convert.ToInt64(calc.Substring(1, operation_index - 1));
                        }
                        else if(calc[0] == '+')
                        {
                            nbr1 = Convert.ToInt64(calc.Substring(1, operation_index - 1));
                        }
                        else
                        {
                            nbr1 = Convert.ToInt64(calc.Substring(0, operation_index));
                        }

                    }
                    catch (Exception e)
                    {
                        if(show_error)
                        {
                            if(e.HResult == -2146233066)
                            {
                                Console.WriteLine("> calc: " + calc.Substring(0, operation_index) + ": overtake calcul capacity");
                            }
                            else
                            {
                                Console.WriteLine("> calc: " + calc.Substring(0, operation_index) + ": invalid argument");
                            }
                        }
                        return (false);
                    }
                }
                else
                {
                    nbr1 = last_result;
                }

                if(calc[operation_index] != '²')
                {
                    try
                    {
                        if (calc[operation_index + 1] == '-')
                        {
                            nbr2 = -1 * Convert.ToInt64(calc.Substring(operation_index + 2, calc.Length - 2 - operation_index));
                        }
                        else if (calc[operation_index + 1] == '-')
                        {
                            nbr2 = Convert.ToInt64(calc.Substring(operation_index + 2, calc.Length - 2 - operation_index));
                        }
                        else
                        {
                            nbr2 = Convert.ToInt64(calc.Substring(operation_index + 1, calc.Length - 1 - operation_index));
                        }
                    }
                    catch (Exception e)
                    {
                        if (show_error)
                        {
                            if (e.HResult == -2146233066)
                            {
                                Console.WriteLine("> calc: " + calc.Substring(0, operation_index) + ": overtake calcul capacity");
                            }
                            else
                            {
                                Console.WriteLine("> calc: " + calc.Substring(operation_index + 1, calc.Length - 1 - operation_index) + ": invalid argument");
                            }
                        }
                        return (false);
                    }
                }
                else 
                {
                    nbr2 = nbr1;
                }
                
                switch(calc[operation_index]) //on defini l'action de l'opérateur sur les nombres nrb1 et nrb2 qui entourent l'opérateur
                {
                    case ('%'):
                        try
                        {
                            calc = (checked(nbr1 % nbr2)).ToString();
                        }
                        catch (OverflowException)
                        {
                            if(show_error)
                            {
                                Console.WriteLine("> calc: overtake calcul capacity");
                            }
                            return (false);
                        }
                        break;
                    case ('^'):
                        try
                        {
                            if(pwd(ref nbr1,nbr2))
                            {
                                calc = (checked(nbr1)).ToString();
                            }
                            else
                            {
                                if (show_error)
                                {
                                    Console.WriteLine("> calc: overtake calcul capacity");
                                }
                                return (false);
                            }
                        }
                        catch (OverflowException)
                        {
                            if (show_error)
                            {
                                Console.WriteLine("> calc: overtake calcul capacity");
                            }
                            return (false);
                        }
                        break;
                    case ('²'):
                        try
                        {
                            calc = (checked(nbr1 * nbr2)).ToString();
                        }
                        catch (OverflowException)
                        {
                            if (show_error)
                            {
                                Console.WriteLine("> calc: overtake calcul capacity");
                            }
                            return (false);
                        }
                        break;
                    case('*'):
                        try
                        {
                            calc = (checked(nbr1 * nbr2)).ToString();
                        }
                        catch(OverflowException)
                        {
                            if (show_error)
                            {
                                Console.WriteLine("> calc: overtake calcul capacity");
                            }
                            return (false);
                        }
                        break;
                    case ('/'):
                        if(nbr2 != 0)
                        {
                            try
                            {
                                calc = (checked(nbr1 / nbr2)).ToString();
                            }
                            catch (OverflowException)
                            {
                                if (show_error)
                                {
                                    Console.WriteLine("> calc: overtake calcul capacity");
                                }
                                return (false);
                            }
                        }
                        else
                        {
                            if (show_error)
                            {
                                Console.WriteLine("> calc: can't divise by 0");
                            }
                            return (false);
                        }
                        break;
                    case ('+'):
                        try
                        {
                            if(result_is_valid(nbr1, nbr2, (nbr1+nbr2), '+'))
                            {
                                calc = (checked(nbr1 + nbr2)).ToString();
                            }
                            else
                            {
                                if (show_error)
                                {
                                    Console.WriteLine("> calc: overtake calcul capacity");
                                }
                                return (false);
                            }
                        }
                        catch (OverflowException)
                        {
                            if (show_error)
                            {
                                Console.WriteLine("> calc: overtake calcul capacity");
                            }
                            return (false);
                        }
                        break;
                    case ('-'):
                        try
                        {
                            if (result_is_valid(nbr1, nbr2, (nbr1 - nbr2), '-'))
                            {
                                calc = (checked(nbr1 - nbr2)).ToString();
                            }
                            else
                            {
                                if (show_error)
                                {
                                    Console.WriteLine("> calc: overtake calcul capacity");
                                }
                                return (false);
                            }
                        }
                        catch (OverflowException)
                        {
                            if (show_error)
                            {
                                Console.WriteLine("> calc: overtake calcul capacity");
                            }
                            return (false);
                        }
                        break;
                }
            }
            else
            {
                if (show_error)
                {
                    Console.WriteLine("> calc: argument not found");
                }
                return (false);
            }

            return(true);
        } //on défini l'action des operateurs

        private static bool next_operation_to_do(ref string expression, long last_result, bool show_error = true)
        {
            List<char> operations = new List<char>();
            List<int> operations_index = new List<int>();
            extract_operations(ref expression, ref operations, ref operations_index, last_result);
            bool is_ok = true;
            string calc = "";
            int index_start = 0;
            int index_stop = expression.Length - 1;
            int index_operation = 0;

            while(is_ok && operations.Count > 0)
            {
                index_operation = most_priority_operation(operations);
                set_start_stop(ref index_start, ref index_stop, operations_index, index_operation, expression);
                calc = expression.Substring(index_start, index_stop - index_start + 1);
                if(process_calculator(ref calc, operations_index[index_operation] - index_start, last_result, show_error))
                {
                    if (index_start == 0)
                    {
                        expression = calc + expression.Substring(index_stop + 1, expression.Length - 1- index_stop);
                    }
                    else if(index_stop == expression.Length - 1)
                    {
                        expression = expression.Substring(0, index_start) + calc;
                    }
                    else
                    {
                        expression = expression.Substring(0, index_start) + calc + expression.Substring(index_stop + 1, expression.Length - 1 - index_stop);
                    }
                }
                else
                {
                    is_ok = false;
                }
                operations = new List<char>();
                operations_index = new List<int>();
                extract_operations(ref expression, ref operations, ref operations_index, last_result);
            }

            return (is_ok);
        }

        private static void extract_operations(ref string expression, ref List<char> operations, ref List<int> index, long last_result)
        {
            int i = 0;
            bool modification = false;

            while(i < expression.Length && !modification)
            {
                if (expression[i] == '²' || expression[i] == '%' || expression[i] == '^' || expression[i] == '*' || expression[i] == '/' || (expression[i] == '+' && i > 0 && expression[i - 1] != '*' && expression[i - 1] != '/' && expression[i - 1] != '+' && expression[i - 1] != '-') || (expression[i] == '-' && i > 0 && expression[i - 1] != '*' && expression[i - 1] != '/' && expression[i - 1] != '+' && expression[i - 1] != '-') || (expression[i] == '+' && i == 0))
                {
                    operations.Add(expression[i]);
                    index.Add(i);
                }
                else if (expression[i] == '(')
                {
                    int parclosure = 0;
                    string parexpression = parenthesis_expression(expression, i, ref parclosure);

                    if(next_operation_to_do(ref parexpression, last_result))
                    {
                        if (i == 0)
                        {
                            expression = parexpression + expression.Substring(parclosure + 1, expression.Length - 1 - parclosure);
                        }
                        else if (parclosure == expression.Length - 1)
                        {
                            expression = expression.Substring(0, i) + parexpression;
                        }
                        else
                        {
                            expression = expression.Substring(0, i) + parexpression + expression.Substring(parclosure + 1, expression.Length - 1 - parclosure);
                        }
                        i = expression.Length;
                        modification = true;
                    }
                    else
                    {
                        expression = "";
                        break;
                    }
                }
                i++;
            }
            if(modification)
            {
                operations = new List<char>();
                index = new List<int>();
                extract_operations(ref expression, ref operations, ref index, last_result);
            }
        } //on défini un symbole comme un operateur

        private static int most_priority_operation(List<char> operations)
        {
            int index = 0;
            int priority = -1; //priority: 0 = + ou -; 1 = * ou /; 2 = ^ ou % ou ²;

            for(int i = 0; i < operations.Count; i++)
            {
                if ((operations[i] == '²' || operations[i] == '^' || operations[i] == '%') && priority < 2)
                {
                    index = i;
                    priority = 2;
                }
                if((operations[i] == '*' || operations[i] == '/') && priority < 1)
                {
                    index = i;
                    priority = 1;
                }
                if((operations[i] == '-' || operations[i] == '+') && priority < 0)
                {
                    index = i;
                    priority = 0;
                }
            }

            return (index);
        } //on défini la priorité de l'opérateur

        private static void set_start_stop(ref int start, ref int stop, List<int> operations_index, int index, string expression)
        {
            if(index == 0)
            {
                start = 0;
                if(operations_index.Count > 1)
                {
                    stop = operations_index[index + 1] - 1;
                }
                else
                {
                    stop = expression.Length - 1;
                }
            }
            else if(index == operations_index.Count - 1)
            {
                stop = expression.Length - 1;
                start = operations_index[index - 1] + 1;
            }
            else
            {
                start = operations_index[index - 1] + 1;
                stop = operations_index[index + 1] - 1;
            }
        }

        private static string parenthesis_expression(string expression, int paropen, ref int parclosure)
        {
            int i = paropen + 1;
            int nbrpar = 1;

            while(i < expression.Length && nbrpar != 0)
            {
                if(expression[i] == ')')
                {
                    nbrpar--;
                }
                else if (expression[i] == '(')
                {
                    nbrpar++;
                }
                if(nbrpar != 0)
                {
                    i++;
                }
            }

            parclosure = i;
            return (expression.Substring(paropen + 1,parclosure - paropen - 1));
        }

        private static bool pwd(ref long a, long b) //a^b
        {
            long result = 1;

            for(int i = 0; i < b; i++)
            {
                if(result_is_valid(a, a, a*a, '*'))
                {
                    result = result * a;
                }
                else
                {
                    return (false);
                }
            }

            a = result;
            return (true);
        }

        public static bool result_is_valid(long nb1, long nb2, long result, char operation)
        {
            switch(operation)
            {
                case ('^'):
                    return ((nb1 >= 0 && result >= 0) || (nb1 <= 0 && result >= 0 && nb2 % 2 == 0) || (nb1 <= 0 && result <= 0 && nb2 % 2 != 0));
                case ('²'):
                    return (result >= 0);
                case ('*'):
                    return ((nb1 >= 0 && nb2 >= 0 && (result >= nb1 || result >= nb2)) || (nb1 <= 0 && nb2 <= 0 && (result <= nb1 || result <= nb2)) || (nb1 >= 0 && nb2 <= 0 && (result <= (-1) * nb1 || result <= nb2)) || (nb1 <= 0 && nb2 >= 0 && (result <= nb1 || result <= (-1) * nb2)));
                case ('/'):
                    return ((((nb1 >= 0 && nb2 >= 0) || (nb1 <= 0 && nb2 <= 0)) && result >= 0) || (((nb1 >= 0 && nb2 <= 0) || (nb1 <= 0 && nb2 >= 0)) && result <= 0));
                case ('+'):
                    return ((nb1 >= 0 && nb2 >= 0 && result >= 0) || (nb1 <= 0 && nb2 <= 0 && result <= 0) || (nb1 >= 0 && nb2 <= 0) || (nb1 <= 0 && nb2 >= 0));
                case ('-'):
                    return ((nb1 >= 0 && nb2 >= 0) || (nb1 <= 0 && nb2 <= 0) || (nb1 >= 0 && nb2 <= 0 && result >= 0) || (nb1 <= 0 && nb2 >= 0 && result <= 0));
                default:
                    return (true);
            }
        }
    }
}
