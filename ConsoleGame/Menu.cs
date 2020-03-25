﻿using ConsoleGame.Classes;
using System;
using System.IO;

namespace ConsoleGame
{
    public class Menu
    {
        public Menu()
        {
            TextResource.Init();
           
            ShowMenu();
        }

        void ShowMenu()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine("         />_________________________________");
            Console.WriteLine("[########[]_________________________________>");
            Console.WriteLine("         \\>");
            Console.WriteLine();
            Console.WriteLine("                 KRISS' JOURNEY");
            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();


            //debug: start from
            NodeFactory.CreateNode("1_11");
            //debug


            if (TextResource.DB.lastchapter.iscomplete && TextResource.DB.lastchapter.number > 0)
            {
                Console.WriteLine("Welcome back, traveler.");
                Console.WriteLine("So far, you completed chapter no. " + TextResource.DB.lastchapter.number.ToString());
                Console.WriteLine();
                Console.WriteLine("Press any key to start the next one.");
                Console.ReadLine();

                NodeFactory.CreateChapter(TextResource.DB.lastchapter.number + 1); 
            }
            else
            {
                Console.WriteLine("Welcome traveler.");
                Console.WriteLine("Your journey is yet to be started.");
                Console.WriteLine();
                Console.WriteLine("This game features autosave. You just won't know when.");
                Console.WriteLine();
                Console.WriteLine("Press any key.");
                Console.ReadKey(true);

                NodeFactory.CreateChapter(1);
            }
        }
    }
}
