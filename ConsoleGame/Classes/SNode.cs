﻿using ConsoleGame.Models;
using ConsoleGame.Nodes;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleGame.Classes
{
    /// <summary>
    /// The main class. It represents every step of the story, every screen which will or won't have several types of player interactions
    /// </summary>
    public class SNode
    {
        #region Properties
        internal string ID { get; set; }
        internal string Text { get; set; } = string.Empty;
        internal string ChildId { get; set; }
        internal List<Choice> Choices { get; set; }
        internal List<Models.Action> Actions { get; set; }
        internal List<Dialogue> Dialogues { get; set; }

        readonly NodeBase node;
        #endregion

        #region CTOR & "SaveStatusOnExit"
        /// <summary>
        /// At its creation, an instantiated node should clear the screen, print its text and prepare to receive player's input.
        /// This root node loads text resources for everybody
        public SNode(NodeBase nb)
        {
            Console.Clear();
            node = nb;

            ID = node.Id;
            Text = node.Text;
            ChildId = node.ChildId;
            Choices = node.Choices;
            Actions = node.Actions;
            Dialogues = node.Dialogues;

            if (!node.IsVisited)     
                TextFlow(true);
            else
                TextFlow(false);
        }
        internal void SaveStatusOnExit()  //this to be called on exit from node to mark it as not new
        {
            string[] number = node.Id.Split("_");
            var updatingNode = DataLayer.DB.Chapters[Convert.ToInt32(number[0])].Find(n => n.Id == node.Id);
            updatingNode.IsVisited = true;
            DataLayer.SaveProgress(0);
        }
        #endregion

        #region TextFlow
        /// <summary>
        /// Mimics the flow of text of old console games. 
        /// </summary>

        internal int FlowDelay { get; set; } = 30; // fine-tunes the speed of TextFlow
        internal int ParagraphBreak { get; set; } = 1000; // fine-tunes the pause between blocks

        internal void TextFlow(bool isFlowing, string text = "default")
        {
            //debug disable effect
            //isFlowing = false;
            //debug

            if (text == "default")
                text = Text;

            if (text != null)
            {
                int flow = FlowDelay;
                int paragraph = ParagraphBreak;

                if (this is NStory)
                    Console.ForegroundColor = ConsoleColor.DarkCyan; //narrator, default color

                if (!isFlowing)
                {
                    flow = 0;
                    paragraph = 0;
                }
                char prevChar = new Char();

                foreach (char c in text)
                {
                    if (prevChar.ToString().Equals("$"))
                        switch (c.ToString())
                        {
                            case "R":
                                Console.ForegroundColor = ConsoleColor.Red; //Corolla
                                break;
                            case "r":
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                break;
                            case "G":
                                Console.ForegroundColor = ConsoleColor.Green;
                                break;
                            case "B":
                                Console.ForegroundColor = ConsoleColor.Blue; //Theo
                                break;
                            case "C":
                                Console.ForegroundColor = ConsoleColor.DarkCyan; //narrator
                                break;
                            case "c":
                                Console.ForegroundColor = ConsoleColor.Cyan; //yourself
                                break;
                            case "M":
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                break;
                            case "Y":
                                Console.ForegroundColor = ConsoleColor.Yellow; //Smiurl
                                break;
                            case "K":
                                Console.ForegroundColor = ConsoleColor.Black;
                                break;
                            case "W":
                                Console.ForegroundColor = ConsoleColor.White;
                                break;
                            case "S":
                                Console.BackgroundColor = ConsoleColor.White;
                                break;
                            case "D":
                                Console.ForegroundColor = ConsoleColor.DarkGray; //menus, help
                                break;
                            case "d":
                                Console.ForegroundColor = ConsoleColor.Gray; //menus, help
                                break;
                            default:
                                break;
                        }
                    else
                    {
                        if (!c.ToString().Equals("#") && !c.ToString().Equals("$"))
                        {
                            Console.Write(c);
                            Thread.Sleep(flow);
                        }
                        else if (c.ToString().Equals("#"))
                        {
                            Thread.Sleep(paragraph);
                        }
                    }
                    //if (prevChar.ToString().Equals("@"))
                    //{
                    //    Random rnd = new Random();
                    //    Random rnd2 = new Random();
                    //    for (int i = 0; i < 10000; i++)
                    //        Console.Beep(rnd2.Next(500, 2000), rnd.Next(50, 500));
                    //} 
                        
                    prevChar = c;
                }
            }
        }
        #endregion
    }
}