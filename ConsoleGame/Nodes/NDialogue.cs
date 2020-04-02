﻿using ConsoleGame.Classes;
using ConsoleGame.Models;
using System;
using System.Threading;

namespace ConsoleGame.Nodes
{
    public class NDialogue : SNode
    {
        public NDialogue(NodeBase nb) : base(nb)
        {
            RecursiveDialogues();
        }

        ConsoleKeyInfo key;
        int selectedRow = 0;

        void RecursiveDialogues(int lineId = 0, bool isLineFlowing = true)       //lineid iterates over elements of dialogues[] 
        {
            TextFlow(false);

            var currentLine = Dialogues[lineId];                                //cureent object selected in the iteration

        #region Drawing base element of the Dialog object (speech part)

            if(currentLine.PreComment != null)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;

                TextFlow(isLineFlowing, currentLine.PreComment);
                if (isLineFlowing)
                    Thread.Sleep(ParagraphBreak);

                Console.WriteLine();
                Console.WriteLine();
            }
            if(currentLine.Line != null)
            {
                Console.ForegroundColor = DataLayer.ActorsColors[currentLine.Actor]; 

                TextFlow(isLineFlowing, "\"" + currentLine.Line + "\" ");
            }
            if(currentLine.Comment != null)      
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;

                TextFlow(isLineFlowing, currentLine.Comment);
                if (isLineFlowing)
                    Thread.Sleep(ParagraphBreak);
            }
            Console.WriteLine();
            Console.WriteLine();
            
            if (currentLine.IsBreakNeeded)
            {
                HoldScreen();
                Console.Clear();
            }
        #endregion

            if (currentLine.ChildId != null)                                        //if it encounters a link, jump to the node
            {      
                HoldScreen();
                NodeFactory.CreateNode(currentLine.ChildId);
            }

            if (currentLine.Replies != null && currentLine.Replies.Count > 0)       //if there are replies inside, display choice
            {
                do
                {
                    for (int i = 0; i < Dialogues[lineId].Replies.Count; i++)       //draw the replies, select them
                    {
                        if (i == selectedRow)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkCyan;
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        Console.Write("\t");
                        Console.Write((i + 1) + ". \"" + Dialogues[lineId].Replies[i].Line + "\"");

                        Console.ResetColor();
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine();
                        Console.CursorLeft = Console.WindowLeft;
                    }

                    while (Console.KeyAvailable)
                        Console.ReadKey(true);
                    key = Console.ReadKey(true);

                    if ((key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.LeftArrow) && selectedRow > 0)
                    {
                        selectedRow--;
                        Console.Clear();
                        RecursiveDialogues(lineId, false);                                //redraw the node to allow the selection effect
                    }
                    if ((key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.RightArrow) && selectedRow < Dialogues[lineId].Replies.Count - 1)
                    {
                        selectedRow++;
                        Console.Clear();
                        RecursiveDialogues(lineId, false);                                //redraw the node to allow the selection effect
                    }

                } while (key.Key != ConsoleKey.Enter);

                Console.Clear();

                if (currentLine.Replies[selectedRow].ChildId != null)                 //on selecion, either 
                {
                    Console.ReadKey(true);
                    NodeFactory.CreateNode(currentLine.Replies[selectedRow].ChildId); //navigate to node specified in selected reply
                }
                else
                {
                    var nextLineId = Dialogues.FindIndex(l => l.LineName == currentLine.Replies[selectedRow].NextLine);
                    RecursiveDialogues(nextLineId);                                   //step to the next line
                }
            }
            else
            {
                if (Dialogues.Count > lineId + 1)                       
                    RecursiveDialogues(lineId + 1);  
            }
        }

        void HoldScreen()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Press any key...");
            Console.ReadKey(true);
        }
    }
}
