using System;
using System.Collections.Generic;
using System.Linq;
using Unity;
using UnityEngine;
namespace TheNorth 
{
    public class DialoguesTest: MonoBehaviour
    {
        public List<Story> stories;
        public Dictionary<string, Story> storiesByTag;
        private void Awake()
        {
            CreateStories();
            storiesByTag = stories.ToDictionary(keySelector: key => key.tag, elementSelector: element => element);
        }
        private void CreateStories() 
        {
            stories = new List<Story>() 
            {
                new Story() 
                {
                    tag = "d1",
                    text = "Йоу, менчик, привет",
                    answers = new Answer[] 
                    {
                        new Answer() 
                        {
                            text = "Привет, родной, как сам?",
                            tagOfNextDialogue = "d2",

                        },
                        new Answer() 
                        {
                            text = "Брат, сорян, некогда",
                            tagOfNextDialogue = "d3",
                        }
                    }
                },
                new Story() 
                {
                    tag = "d2",
                    text = "Да вот на работу устроился, а еще...",
                    answers = new Answer[] 
                    {
                        new Answer() 
                        {
                            text = "Ну ладно, давай, я погнал",
                            tagOfNextDialogue = "endOfDialogue",
                        }
                    }
                },
                new Story () 
                {
                    tag = "d3",
                    text = "Жаль, ну ладно, давай",
                    answers = new Answer[] 
                    {
                        new Answer() 
                        {
                            text = "ББ",
                            tagOfNextDialogue = "endOfDialogue"
                        }
                    }
                },
            };
            DialogueEventManager.Instance.Subscribe("d2", GoodAnswer);
            DialogueEventManager.Instance.Subscribe("d3", BadAnswer);
        }
        public void GoodAnswer() 
        {
            Debug.Log("Вы выбрали хороший ответ");
        }
        public void BadAnswer() 
        {
            Debug.Log("Вы выбрали плохой ответ");
        }
    }
}