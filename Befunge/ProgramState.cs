using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Befunge
{
    public class ProgramState
    {
        string sourceLocation;
        char[,] program;
        Stack<int> stack;
        bool stringMode = false;
        int instructionPointerX = 0;
        int instructionPointerY = 0;
        PointerDirection pointerDirection  = PointerDirection.right;
        static System.Text.Encoding ascii = System.Text.Encoding.ASCII;

        public ProgramState(string source)
        {
            this.sourceLocation = source;
            program = new char[80,50];
            for (int x = 0; x < 80; x++) { for (int y = 0; y < 50; y++) { program[x, y] = ' '; } }
            this.stack = new Stack<int>();
            StreamReader reader = new StreamReader(source);
            string line = "";
            int j = 0;
            while (line != null)
            {
                if (j >= 50) { break; }
                line = reader.ReadLine();
                if (line != null)
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        if (i >= 80) { break; }
                        program[i, j] = line.ToCharArray()[i];
                    }
                }
                j++;
            }
            reader.Close();
        }

        public void Run()
        {
            while (program[instructionPointerX, instructionPointerY] != '@')
            {
                Execute(program[instructionPointerX, instructionPointerY]);
                MoveInstructionPointer();
            }
        }

        private void MoveInstructionPointer()
        {
            switch (pointerDirection)
            {
                case PointerDirection.up:
                    instructionPointerY = (instructionPointerY <= 0) ? 50 : instructionPointerY - 1;
                    break;
                case PointerDirection.down:
                    instructionPointerY = (instructionPointerY >= 50) ? 0 : instructionPointerY + 1;
                    break;
                case PointerDirection.right:
                    instructionPointerX = (instructionPointerX >= 80) ? 0 : instructionPointerX + 1;
                    break;
                case PointerDirection.left:
                    instructionPointerX = (instructionPointerX <= 0) ? 80 : instructionPointerX - 1;
                    break;
            }
        }

        public void Execute(char c)
        {
            if (c == ' ')
            {
                if (stringMode) { stack.Push(Convert.ToInt32(c)); }
                return;
            }
            if (stringMode && c != '"') { stack.Push(Convert.ToInt32(c)); return; }
            if (Char.IsNumber(c)) { stack.Push(int.Parse(c.ToString())); return; }

            int a, b, x;

            switch (c)
            {
                case '+':
                    a = stack.Pop();
                    b = stack.Pop();
                    stack.Push(a + b);
                    break;
                case '-':
                    a = stack.Pop();
                    b = stack.Pop();
                    stack.Push(b - a);
                    break;
                case '*':
                    a = stack.Pop();
                    b = stack.Pop();
                    stack.Push(a * b);
                    break;
                case '/':
                    a = stack.Pop();
                    b = stack.Pop();
                    if (a == 0)
                    {
                        Console.Write("Division by zero found. What result do you want?");
                        stack.Push(GetNumber());
                    }
                    else
                    {
                        stack.Push(b / a);
                    }
                    break;
                case '%':
                    a = stack.Pop();
                    b = stack.Pop();
                    if (a == 0)
                    {
                        Console.Write("Division by zero found. What result do you want?");
                        stack.Push(GetNumber());
                    }
                    else
                    {
                        stack.Push(b % a);
                    }
                    break;
                case '!':
                    a = stack.Pop();
                    stack.Push((a == 0) ? 1 : 0);
                    break;
                case '`':
                    a = stack.Pop();
                    b = stack.Pop();
                    stack.Push((b > a) ? 1 : 0);
                    break;
                case '>':
                    pointerDirection = PointerDirection.right;
                    break;
                case '<':
                    pointerDirection = PointerDirection.left;
                    break;
                case '^':
                    pointerDirection = PointerDirection.up;
                    break;
                case 'v':
                    pointerDirection = PointerDirection.down;
                    break;
                case '?':
                    Random r = new Random(DateTime.Now.Millisecond);
                    pointerDirection = (PointerDirection)r.Next(4);
                    break;
                case '_':
                    a = stack.Pop();
                    pointerDirection = ( a == 0 ) ? PointerDirection.right : PointerDirection.left;
                    break;
                case '|':
                    a = stack.Pop();
                    pointerDirection = ( a == 0 ) ? PointerDirection.down : PointerDirection.up;;
                    break;
                case '"':
                    stringMode = !stringMode;
                    break;
                case ':':
                    stack.Push((stack.Count > 0) ? stack.Peek() : 0);
                    break;
                case '\\':
                    a = stack.Pop();
                    b = stack.Pop();
                    stack.Push(a);
                    stack.Push(b);
                    break;
                case '$':
                    if (stack.Count > 0) { stack.Pop(); }
                    break;
                case '.':
                    Console.Write(stack.Pop());
                    break;
                case ',':
                    Console.Write(Convert.ToChar(stack.Pop()));
                    break;
                case '#':
                    MoveInstructionPointer();
                    break;
                case 'p':
                    a = stack.Pop();
                    b = stack.Pop();
                    x = stack.Pop();
                    program[b, a] = Convert.ToChar(x);
                    break;
                case 'g':
                    a = stack.Pop();
                    b = stack.Pop();
                    stack.Push(program[b, a]);
                    break;
                case '&':
                    Console.Write("Enter number:");
                    stack.Push(GetNumber());
                    break;
                case '~':
                    Console.Write("Enter character:");
                    stack.Push(GetCharacter());
                    break;
            }
        }

        private static char GetCharacter()
        {
            ConsoleKeyInfo i = Console.ReadKey(true);
            return (Char.IsLetterOrDigit(i.KeyChar) || Char.IsPunctuation(i.KeyChar)) ? i.KeyChar : GetCharacter();
        }

        private static int GetNumber()
        {
            ConsoleKeyInfo i = Console.ReadKey(true);
            return (Char.IsNumber(i.KeyChar)) ? int.Parse(Convert.ToString(i.KeyChar)) : GetNumber();
        }

        private enum PointerDirection
        {
            up,
            down,
            left,
            right
        }
    }
}
