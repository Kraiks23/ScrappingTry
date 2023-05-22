using HtmlAgilityPack;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CsharpScraper
{
    class Book
    {
        public string Title { get; set; }  // Título del libro
        public string Price { get; set; }  // Precio del libro
    }

    class Program
    {
        static void Main(string[] args)
        {
            var bookLinks = GetBookLinks("https://books.toscrape.com/catalogue/category/books/mystery_3/index.html");  // Obtener enlaces de libros
            Console.WriteLine("Found {0} links", bookLinks.Count);  // Mostrar cantidad de enlaces encontrados
            var books = GetBookDetails(bookLinks);  // Obtener detalles de los libros
            ExportToCSV(books);
        }

       
       static void ExportToCSV(List<Book> books)
        {
            using (var writer = new StreamWriter("./books.csv")) // Abrir un StreamWriter para escribir en el archivo "books.csv"
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))// Crear una instancia de CsvWriter para escribir registros CSV en el archivo
            {
                csv.WriteRecords(books);// Escribir los registros de libros en el archivo CSV
            }// La disposición de CsvWriter automáticamente cierra el StreamWriter
        }
        
        
        static HtmlDocument GetDocument(string url)
        {
            var web = new HtmlWeb();  // Instancia de HtmlWeb
            var doc = web.Load(url);  // Cargar el documento HTML
            return doc;
        }

        static List<string> GetBookLinks(string url)
        {
            var bookLinks = new List<string>();  // Lista de enlaces de libros
            var doc = GetDocument(url);  // Obtener el documento HTML
            var linkNodes = doc.DocumentNode.SelectNodes("//h3/a");  // Obtener nodos de enlaces
            var baseUri = new Uri(url);  // URI base
            foreach (var link in linkNodes)
            {
                var href = link.Attributes["href"].Value;  // Obtener el atributo href del enlace
                bookLinks.Add(new Uri(baseUri, href).AbsoluteUri);  // Agregar enlace absoluto a la lista
            }

            return bookLinks;  // Devolver lista de enlaces
        }

        static List<Book> GetBookDetails(List<string> urls)
        {
            var books = new List<Book>();  // Lista de libros
            foreach (var url in urls)
            {
                var document = GetDocument(url);  // Obtener el documento HTML
                var titleXpath = "//h1";  // XPath para el título
                var priceXpath = "//div[contains(@class, 'product_main')]/p[@class='price_color']";  // XPath para el precio
                var book = new Book();  // Instancia de Book
                book.Title = document.DocumentNode.SelectSingleNode(titleXpath)?.InnerText;  // Obtener el título del libro
                book.Price = document.DocumentNode.SelectSingleNode(priceXpath)?.InnerText;  // Obtener el precio del libro
                books.Add(book);  // Agregar el libro a la lista
            }
            return books;  // Devolver lista de libros
        }
    }
}
