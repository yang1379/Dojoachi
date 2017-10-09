using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
 
namespace Dojoachi.Controllers
{
    public class DojoachiController : Controller
    {

        private static Random random = new Random();
        // private int fullness = 20;

        // private int happiness = 20;
        // private int meals = 3;

        // private int energy = 50;


        [HttpGet]
        [Route("")]
        public IActionResult DisplayDefault()
        {
            int? fullness = HttpContext.Session.GetInt32("Fullness");
            if(fullness == null) {
                fullness = 20;
                HttpContext.Session.SetInt32("Fullness", (int)fullness);
            }

            int? happiness = HttpContext.Session.GetInt32("Happiness");
            if(happiness == null) {
                happiness = 20;
                HttpContext.Session.SetInt32("Happiness", (int)happiness);
            }

            int? meals = HttpContext.Session.GetInt32("Meals");
            if(meals == null) {
                meals = 3;
                HttpContext.Session.SetInt32("Meals", (int)meals);
            }

            int? energy = HttpContext.Session.GetInt32("Energy");
            if(energy == null) {
                energy = 50;
                HttpContext.Session.SetInt32("Energy", (int)energy);
            }

            string gameStatus = HttpContext.Session.GetString("GameStatus");
            if(gameStatus == null) {
                gameStatus = "Continue";
                HttpContext.Session.SetString("GameStatus", gameStatus);
            }

            string message = HttpContext.Session.GetString("Message");

            ViewBag.Fullness = fullness;
            ViewBag.Happiness = happiness;
            ViewBag.Meals = meals;
            ViewBag.Energy = energy;
            ViewBag.Message = message;
            ViewBag.GameStatus = gameStatus;

            return View("Home");
        }

        [HttpPost]
        [Route("restart")]
        public IActionResult Restart(string activity) {
            HttpContext.Session.SetInt32("Meals", 3);
            HttpContext.Session.SetInt32("Energy", 50);
            HttpContext.Session.SetInt32("Fullness", 20);
            HttpContext.Session.SetInt32("Happiness", 20);

            HttpContext.Session.SetString("Message", "");
            HttpContext.Session.SetString("GameStatus", "Continue");

            return RedirectToAction("DisplayDefault");
        }

        [HttpPost]
        [Route("process")]
        public IActionResult Process(string activity) {
            Console.WriteLine("Activity: {0}", activity);
            int? meals = HttpContext.Session.GetInt32("Meals");
            int? energy = HttpContext.Session.GetInt32("Energy");
            int? fullness = HttpContext.Session.GetInt32("Fullness");
            int? happiness = HttpContext.Session.GetInt32("Happiness");

            int chance = random.Next(1, 5);
            if (String.Equals("Feed", activity, StringComparison.OrdinalIgnoreCase) && meals > 0) {
                processFeed((int)meals, (int)fullness, chance);
            }
            else if (String.Equals("Play", activity, StringComparison.OrdinalIgnoreCase) && energy > 4) {
                processPlay((int) energy, (int) happiness, chance);
            }
            else if (String.Equals("Work", activity, StringComparison.OrdinalIgnoreCase) && energy > 4) {
                processWork((int) energy, (int) meals);
            }
            else if (String.Equals("Sleep", activity, StringComparison.OrdinalIgnoreCase) ) {
                processSleep((int) energy, (int) fullness, (int) happiness);
            }

            energy = HttpContext.Session.GetInt32("Energy");
            fullness = HttpContext.Session.GetInt32("Fullness");
            happiness = HttpContext.Session.GetInt32("Happiness");

            if(energy > 100 && fullness > 100 && happiness > 100) {
                string message = String.Format("Congratulations! You Won!");
                HttpContext.Session.SetString("Message", message);
                HttpContext.Session.SetString("GameStatus", "Over");
                // ViewBag.GameStatus = "Over";
            }
            else if(fullness < 1 || happiness < 1) {
                string message = String.Format("Your Dojoachi has passed away.");
                HttpContext.Session.SetString("Message", message);
                HttpContext.Session.SetString("GameStatus", "Over");
                // ViewBag.GameStatus = "Over";
            }

            return RedirectToAction("DisplayDefault");
        }

        private void processFeed(int meals, int fullness, int chance) {
            meals--;
            HttpContext.Session.SetInt32("Meals", (int)meals);
            if(chance != 1) {
                int randNum = random.Next(5, 11);        
                fullness += randNum;

                string message = String.Format("You fed your Dojoachi: Fullness +{0}", randNum);
                HttpContext.Session.SetString("Message", message);
                
                HttpContext.Session.SetInt32("Fullness", (int)fullness);
            } else {
                string message = String.Format("Dojoachi did not like what you fed it!");
                HttpContext.Session.SetString("Message", message);
            }
        }

        private void processPlay(int energy, int happiness, int chance) {
            energy -= 5;
            HttpContext.Session.SetInt32("Energy", (int)energy);
            if(chance != 1) {
                int randNum = random.Next(5, 11);
                happiness += randNum;

                string message = String.Format("You played with your Dojoachi: Happiness +{0}", randNum);
                HttpContext.Session.SetString("Message", message);
                
                HttpContext.Session.SetInt32("Happiness", (int)happiness);
            } else {
                string message = String.Format("Dojoachi did not like playing with you!");
                HttpContext.Session.SetString("Message", message);
            }
        }

        private void processWork(int energy, int meals) {
            energy -= 5;
            HttpContext.Session.SetInt32("Energy", (int)energy);
            int randNum = random.Next(1, 4);
            meals += randNum;

            string message = String.Format("Your Dojoachi worked and gained: Meals +{0}", randNum);
            HttpContext.Session.SetString("Message", message);
            HttpContext.Session.SetInt32("Meals", (int)meals);
        }

        private void processSleep(int energy, int fullness, int happiness) {
            energy += 15;
            fullness -= 5;
            if(fullness <0) fullness = 0;
            
            happiness -= 5;
            if(happiness <0) happiness = 0;

            string message = String.Format("You put your Dojoachi to sleep: Energy +15");
            HttpContext.Session.SetString("Message", message);
            HttpContext.Session.SetInt32("Energy", (int)energy);
            HttpContext.Session.SetInt32("Fullness", (int)fullness);
            HttpContext.Session.SetInt32("Happiness", (int)happiness);
        }

    }

}