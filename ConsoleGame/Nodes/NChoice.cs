﻿using ConsoleGame.Classes;
using ConsoleGame.Models;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleGame.Nodes
{
    public class NChoice : SNode
    {
        int selectedRow = 0;
        readonly List<Choice> visibleChoices = new List<Choice>();

        public NChoice(NodeBase nb) : base(nb)
        {
            Thread.Sleep(ParagraphBreak);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            visibleChoices = Choices.FindAll(c => c.IsHidden == false);

            WaitForChoice();
        }
        void WaitForChoice()
        {
            ConsoleKeyInfo key;
            do
            {
                for (int i = 0; i < visibleChoices.Count; i++)
                {
                   
                    var foreground = ConsoleColor.DarkCyan;
                    var background = ConsoleColor.Black;
                        
                    if (Choices[i].IsPlayed)
                    {
                        foreground = ConsoleColor.DarkGray;
                        if (i == selectedRow)
                        {
                            background = ConsoleColor.DarkGray;
                            foreground = ConsoleColor.White;
                        }
                    }
                    else
                    {
                        if (i == selectedRow)
                        {
                            background = ConsoleColor.DarkBlue;
                            foreground = ConsoleColor.White;
                        }
                    }
              
                    Console.Write("\t");
                    Console.ForegroundColor = foreground;
                    Console.BackgroundColor = background;
                    Console.Write((i + 1) + ". " + visibleChoices[i].Desc);

                    Console.ResetColor();
                    Console.WriteLine();
                    Console.CursorLeft = Console.WindowLeft;
                }

                while (Console.KeyAvailable)
                    Console.ReadKey(true);
                key = Console.ReadKey(true);

                if ((key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.LeftArrow) && selectedRow > 0)
                    selectedRow--;
                if ((key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.RightArrow) && selectedRow < visibleChoices.Count - 1)
                    selectedRow++;

                Console.Clear();
                TextFlow(false);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();

            } while (key.Key != ConsoleKey.Enter);

            var choice = visibleChoices[selectedRow];

            if (choice.IsPlayed)
            {
                if (choice.IsNotRepeatable)
                {
                    RedrawNode();
                    WaitForChoice();
                }
            }
            if (choice.Evaluate())
            {
                if (choice.Effect != null)
                    choice.StoreItem(choice.Effect);

                if (choice.UnHide.HasValue)                  //if this choice unlocks others
                {
                    int UnHide = (int)choice.UnHide;
                    Choices[UnHide].IsHidden = false;
                }

                choice.IsPlayed = true;

                SaveStatusOnExit();
                NodeFactory.CreateNode(choice.ChildId);
            }
            else
            {
                Console.CursorTop = Console.WindowHeight - 4;
                Console.CursorLeft = Console.WindowLeft;
                TextFlow(true, choice.Refusal, ConsoleColor.DarkYellow);
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("Press any key...");
                Console.ReadKey(true);

                RedrawNode();
                WaitForChoice();
            }
        }
        void RedrawNode() 
        {
            Console.Clear();
            TextFlow(false);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}

