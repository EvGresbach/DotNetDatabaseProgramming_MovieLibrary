using System;
using System.IO; 
using System.Collections.Generic;

namespace MovieLibrary
{
    class Program
    {
        static void Main(string[] args)
        {   
            int answer; 
            //set up logger
            string path = Directory.GetCurrentDirectory() + "\\nlog.config";
            var logger = NLog.Web.NLogBuilder.ConfigureNLog(path).GetCurrentClassLogger();

            //ask to list movies or add movie
            do{
                Console.Write("1. Read movies from file\n2. Write movie to file\n3. Exit\n>");
                while (!Int32.TryParse(Console.ReadLine(), out answer)){
                    Console.Write("Invalid input - input must be an integer. Please try again\n>");
                } 

                switch(answer){
                    case 1: 
                        //catch and log if the file can't be read
                        try{
                            //read and print movies from movies.csv
                            StreamReader sr = new StreamReader("movies.csv"); 
                            
                            while(!sr.EndOfStream){
                                string line = sr.ReadLine(); 
                                string[] movie = line.Split(","); 
                                string[] genres = movie[2].Split("|"); 
                                Console.WriteLine($"{movie[0]}: {movie[1]} - {String.Join(", ", genres)}");
                            }

                            sr.Close(); 

                        } catch(Exception e) {
                            Console.WriteLine("File could not be read"); 
                            logger.Error(e.Message);
                        }
                        
                        break; 

                    case 2:
                        //catch and log if the file can't be written to
                        try{
                            // read from the file to compare names and ids
                            StreamReader sr1 = new StreamReader("movies.csv"); 
                            List<int> movieID = new List<int>(); 
                            List<string> movieName = new List<string>(); 
                            
                            while(!sr1.EndOfStream){
                                string line = sr1.ReadLine(); 
                                string[] movie = line.Split(","); 
                                string[] genres = movie[2].Split("|"); 
                                int tempID; 
                                if(!Int32.TryParse(movie[0], out tempID)){
                                    tempID = 0; 
                                }
                                movieID.Add(tempID); 
                                movieName.Add(movie[1]); 
                            }
                            sr1.Close(); 

                            // open stream writer that appends to the file
                            StreamWriter sw = File.AppendText("movies.csv"); 

                            //get id, name, and genres for movie to add
                            Console.Write("Enter movie id >"); 
                            int id; 
                            while(!Int32.TryParse(Console.ReadLine(), out id)){
                                Console.Write("Invlid id - id must be an integer. Please try again\n>"); 
                            }

                            Console.Write("Enter movie name >"); 
                            string name = Console.ReadLine(); 

                            List<string> genre = new List<string>(); 
                            bool moreGenres = false; 
                            do{
                                Console.Write("Enter genre >"); 
                                genre.Add(Console.ReadLine()); 
                                Console.Write("Do you wish to enter another genre? (Y/N)>"); 
                                moreGenres = Console.ReadLine().ToUpper() == "Y" ? true : false; 
                            } while(moreGenres);

                            //check if id is already in list
                            bool idExists = false; 
                            bool nameExists = false; 
                            for(int i = 0; i < movieID.Count; i++)
                            {
                                if(movieID[i] == id){
                                    Console.Write("Id already exists\n");
                                    idExists = true; 
                                }
                            }
                            // check if name is already in the list
                            for(int i = 0; i < movieName.Count; i++)
                            {
                                if (movieName[i] == name){
                                    Console.Write("Movie already exists\n");
                                    nameExists = true; 
                                }
                            }
                            //if the id and name are free, tell user and add to movies.csv
                            if(!idExists && !nameExists) 
                            {
                                Console.WriteLine($"{id}, {name}, {String.Join("|", genre)} entered");
                                sw.WriteLine($"{id},{name},{String.Join("|", genre)}");
                            }
                            
                            sw.Close(); 
                        } catch (Exception e){
                            Console.WriteLine("File could not be written to");
                            logger.Error(e.Message); 
                        }

                        break; 

                    case 3: 
                        Console.WriteLine("Exiting..."); 
                        break; 
                }

            }while(answer != 3);
            
        }
    }
}
