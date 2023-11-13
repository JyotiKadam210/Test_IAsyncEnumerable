
   // TextWriter textWriter = new StreamWriter(@System.Environment.GetEnvironmentVariable("OUTPUT_PATH"), true);

    string author = Console.ReadLine();

    
List<string> result = await AuthorResult.getAuthorHistory(author);    
    //Result.getAuthorHistory(author);


foreach (String s in result)
{
    Console.WriteLine(s);
}

   
