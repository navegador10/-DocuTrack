using System;
using DocuTrack.Controller;

namespace DocuTrack
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Punto de entrada del programa.
            // Imprime un mensaje inicial y prepara el controlador que orquesta el flujo.
            Console.WriteLine("DocuTrack iniciado.");
            var controller = new DocuTrackController();
            // Ejecuta el flujo completo: construcción del árbol, búsquedas, actualizaciones,
            // eliminaciones, recorridos y métricas finales.
            controller.Ejecutar();
            
            // En algunos entornos (IDE/terminales integrados) puede no haber consola interactiva.
            // Solo pedimos una tecla para salir si la entrada NO está redirigida.
            if (!Console.IsInputRedirected)
            {
                Console.WriteLine("\nPresione cualquier tecla para salir...");
                // Use intercept=true to avoid echoing the key
                Console.ReadKey(intercept: true);
            }
        }
    }
}