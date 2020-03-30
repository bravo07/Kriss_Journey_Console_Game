﻿using ConsoleGame.Classes;
using System.Collections.Generic;

namespace ConsoleGame.Models
{
    public class Action
    {
        public string Verb { get; set; } //verb of the action
        public string ChildId { get; set; } //key for matching next node
        public string DefaultAnswer { get; set; } //answer for incomplete player requests 
        public List<Object> Objects { get; set; } = new List<Object>(); //objects for the verbs
        public Condition Condition { get; set; } //condition for the viability of the action. normally an item
        public Effect Effect { get; set; } //consequence from the base action
        public string GetAnswer()           // to get response message when action requires an object and player does not provide any valid
        {
            if (Verb != null && !string.IsNullOrWhiteSpace(Verb))
                switch (Verb)
                {
                    case "look":
                        return "What shoud I look at? Where?";
                    case "take":
                        return "What shoud I take?";
                    case "go":
                        return "Where should I go?";
                    case "search":
                        return "Where should I search? For what?";
                    case "remove":
                        return "What will I remove? from where?";
                    case "wear":
                        return "What could I wear?";
                    case "rest":
                        return "Where could I lay down?";
                    case "drink":
                        return "What will I drink?";
                    case "eat":
                        return "What will I eat?";
                    case "sleep":
                        return "Where can I sleep?";
                }
            return "Sorry can't do that.";
        }
       

        public bool Evaluate()                      // check according to the condition
        {
            if (Condition != null)
            {
                var storedItem = DataLayer.DB.Inventory.Find(i => i.Name == Condition.Item);
                if (storedItem != null)
                {
                    if (storedItem.Had & Condition.Value)
                        return true;
                }
                return false;
            }
            return true;
        }
        public void StoreItem(Effect effect)       // consequent modify of inventory
        {
            var itemToStore = new Item() { Name = effect.Item, Had = effect.Value };
            DataLayer.DB.Inventory.Add(itemToStore);
        }
    }
    public class Object
    {
        public string Obj { get; set; } //object of the action
        public string Answer { get; set; } //answer for incomplete player requests 
        public string ChildId { get; set; } //key for matching next node
        public Effect Effect { get; set; } //consequence from the object
    }
    public class Condition                      //condition for the viability of the action. normally an item
    {
        public string Item { get; set; }        // name of the resource
        public bool Value { get; set; }         // value of the resource
        public string Refusal { get; set; }     // message for condition not met
    }
    public class Effect                         // now it affect player. normally inventory
    {
        public string Item { get; set; }        // name of the resource 
        public bool Value { get; set; }         // value of the resource
    }
}
