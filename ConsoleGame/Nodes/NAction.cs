﻿using ConsoleGame.Classes;
using ConsoleGame.Models;
using System;
using System.Collections.Generic;

namespace ConsoleGame.Nodes
{
    public class NAction : SNode
    {
        Models.Action act = null;
        readonly List<ConsoleKeyInfo> keysPressed = new List<ConsoleKeyInfo>();
        string BottomMessage = string.Empty;
        ConsoleColor BottomMessageColor = ConsoleColor.DarkCyan;

        public NAction(NodeBase nb) : base(nb)
        {
            PrepareForAction(true);
        }
        void PrepareForAction(bool isFirst)
        {
            ///go to bottom line and prepare prompt
            Console.CursorTop = Console.WindowTop + Console.WindowHeight - 2;
            Console.CursorLeft = Console.WindowLeft + Console.WindowWidth - 1;

            Console.ForegroundColor = ConsoleColor.Gray;
            if (!isFirst)
            {
                Console.CursorTop -= 1;
                Console.WriteLine(" You can't or won't do that. Try again.");
            }

            Console.Write(" \\> ");
            
            //if redrawing after backspacing, rewrite stack
            if (keysPressed.Count > 0)
                for (int i = 0; i < keysPressed.Count; i++)
                    Console.Write(keysPressed[i].KeyChar.ToString());

            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey();
                if (key.Key.Equals(ConsoleKey.Tab)) //if player presses tabs looking for help
                {
                    TabPressed();
                }
                else if (key.Key.Equals(ConsoleKey.Backspace))  //to erase
                {
                    BackSpacePressed(keysPressed);
                }
                else
                    if (!key.Key.Equals(ConsoleKey.Enter))  //normal keys are registered
                        keysPressed.Add(key);

                if (key.Key == ConsoleKey.Enter)
                {
                    EnterPressed(keysPressed);
                }

            } while (true);
        }

        #region Special keys pressed
        void TabPressed() 
        {
            RedrawNode();

            Console.CursorTop = Console.WindowHeight - 4;
            Console.CursorLeft = Console.WindowLeft;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Possible actions here: ");

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            foreach (var action in Actions)
                foreach (var verb in action.Verbs)
                    Console.Write(verb.Term + " ");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.CursorTop += 1;
            Console.CursorLeft = 0;

            if (ID == "1_02") //first action node. this if clause is to mock player just the first time they use help
            {
                Console.CursorTop += 1;
                Console.WriteLine("\\> you pressed tab for help. noob.");
            }
            else
                Console.CursorTop += 2;

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\\>");
            Console.CursorLeft += 1;
        }
        void BackSpacePressed(List<ConsoleKeyInfo> keysPressed)
        {
            if (keysPressed.Count > 0)
            {
                keysPressed.RemoveAt(keysPressed.Count - 1);

                RedrawNode(true);
                PrepareForAction(true);
            }
        }
        void EnterPressed(List<ConsoleKeyInfo> keysPressed)
        {
            if (keysPressed.Count > 0)
            {
                act = null;
                //reconstruct
                string typed = string.Empty;

                for (int i = 0; i < keysPressed.Count; i++)
                    typed += keysPressed[i].KeyChar.ToString().ToLower();

                keysPressed.Clear();                                            //clear the stack after giving command

                char[] delimiterChars = { ' ', ',', '.', ':', '\t', '!', '\r' };

                string[] words = typed.Split(delimiterChars);

                string matchingVerb = string.Empty;

                foreach (string word in words)                                  //is there one word matching one action?
                {
                    foreach (var action in Actions)
                    {
                        var verb = action.Verbs.Find(v => v.Term.Equals(word));
                        if (verb != null)
                        {
                            act = action;
                            matchingVerb = word;                                //store the typed everb which triggered the action
                            break;
                        }
                    }
                }

                if (act != null)                                                //if there's an action available...
                {
                    if (act.Objects.Count == 0)                                 //...and is objectless...
                    {
                        if (!act.EvaluateSimple())                              //if for some reason Kriss can't do it, say it...
                            CustomRefusal(act.Condition.Refusal);
                        else
                        {
                            if (act.Effect != null)                             //in case the action has an Effect (inventory)
                                act.StoreItem(act.Effect);

                            DisplaySuccess(act.Answer, act.ChildId);            //...just do it
                        }
                    }
                    else
                    {                                                           //...otherwise, examine Objects 
                        for (int i = 0; i < act.Objects.Count; i++)             //this is not O^2. only iterates over <10 x <10 items 
                        {
                            Models.Object o = act.Objects[i];

                            foreach (string word in words)                      //is there a matching object available? just hand me the first you find please
                            {
                                if (o.Obj.Contains(word))                       //the action is right, and there is a acceptable object specified
                                {
                                    if (!act.EvaluateCombination(o))            //if for some reason Kriss can't do it, say it...
                                        CustomRefusal(o.Condition.Refusal);
                                    else                                        //...otherwise, do it
                                    {
                                        if (o.Effect != null)                   //in case the obj has an Effect (inventory)
                                            act.StoreItem(o.Effect);

                                        DisplaySuccess(o.Answer, o.ChildId);
                                    }
                                }
                            }
                        }
                        CustomRefusal(act.GetOpinion(matchingVerb));            //the action is right, but no required object is specified
                    }
                }
                else                                                            //if there's no action available, redraw node and display standard refuse
                {
                    RedrawNode();
                    BottomMessage = string.Empty;
                    PrepareForAction(false);
                }
            }
            else
            {
                RedrawNode();
                BottomMessage = string.Empty;
                PrepareForAction(true);
            }
        }
        #endregion

        void CustomRefusal(string refusal)
        {
            RedrawNode();

            Console.CursorTop = Console.WindowHeight - 4;
            Console.CursorLeft = Console.WindowLeft;

            refusal = "<<" + refusal + ">>";

            BottomMessage = refusal;
            BottomMessageColor = ConsoleColor.Cyan;

            TextFlow(true, refusal, ConsoleColor.Cyan);
            Console.WriteLine();
            Console.WriteLine();

            PrepareForAction(true); //display prompt without standard refuse
        }
        void DisplaySuccess(string answer, string childId = null) 
        {
            if (answer != null)
            {
                RedrawNode();

                BottomMessage = answer;
                BottomMessageColor = ConsoleColor.DarkYellow;

                Console.CursorTop = MeasureMessage(answer);
                Console.CursorLeft = Console.WindowLeft;

                TextFlow(true, answer, ConsoleColor.DarkYellow);
                Console.WriteLine();
                Console.WriteLine();

                if (childId != null)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("Press any key...");
                    Console.ReadKey(true);

                    SaveStatusOnExit();
                    NodeFactory.CreateNode(childId);
                }
            }
            if (childId != null)
            {
                SaveStatusOnExit();
                NodeFactory.CreateNode(childId);
            }

            //if everything fails:
            PrepareForAction(true); //display prompt without standard refuse
        }
        void RedrawNode(bool isDeleting = false)
        {
            Console.Clear();
            TextFlow(false);

            if (isDeleting)
                if (!string.IsNullOrWhiteSpace(BottomMessage))                      //decide if there is a Bottom Message and of which type
                    {
                    switch (BottomMessageColor)
                    {
                        case ConsoleColor.DarkYellow:
                            Console.CursorTop = MeasureMessage(BottomMessage);
                            Console.CursorLeft = Console.WindowLeft;
                            break;

                        case ConsoleColor.Cyan:
                            Console.CursorTop = Console.WindowHeight - 4;
                            Console.CursorLeft = Console.WindowLeft;
                            break;
                    }

                    TextFlow(false, BottomMessage, BottomMessageColor);
                }
        }

        int MeasureMessage(string answer)
        {
            //measure the lenght and the newlines in the answer to determine how up to go to start writing
            var newLines = System.Text.RegularExpressions.Regex.Matches(answer, "\\n").Count;
            var rows = answer.Length / Console.WindowWidth;

            return Math.Min(Console.WindowHeight - (rows + newLines), Console.WindowHeight - 5) - 1;
        }
    }
}
