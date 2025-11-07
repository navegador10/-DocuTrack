using System;
using DocuTrack.Model;

namespace DocuTrack.View
{
    public class DocuTrackView
    {
        public void MostrarEncabezado(string titulo)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"=== {titulo} ===");
            Console.ResetColor();
            ImprimirRegla();
            Console.WriteLine();
        }

        public void MostrarMensaje(string mensaje)
        {
            Console.WriteLine(mensaje);
        }

        public void MostrarResultadoInsercion(string nombre, bool insertado, int comparaciones, string motivo)
        {
            Console.Write($"• Insertar '{nombre}': ");
            if (insertado)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("OK");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"RECHAZADO ({motivo})");
            }
            Console.ResetColor();
            Console.WriteLine($" (comparaciones: {comparaciones})");
        }

        public void MostrarResultadoBusqueda(string nombre, bool encontrado, int comparaciones)
        {
            Console.Write($"• Buscar '{nombre}': ");
            if (encontrado)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("ENCONTRADO");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("NO ENCONTRADO");
            }
            Console.ResetColor();
            Console.WriteLine($" (comparaciones: {comparaciones})");
        }

        public void MostrarResultadoEliminacion(string nombre, bool eliminado, string caso)
        {
            Console.Write($"• Eliminar '{nombre}': ");
            if (eliminado)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("OK");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("NO ENCONTRADO");
            }
            Console.ResetColor();
            if (!string.IsNullOrWhiteSpace(caso)) Console.Write($" {caso}");
            Console.WriteLine();
        }

        public void MostrarRecorrido(System.Collections.Generic.IEnumerable<string> recorrido, string nombre)
        {
            Console.Write($"\n");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{nombre}: ");
            Console.ResetColor();
            Console.WriteLine(string.Join(", ", recorrido));
        }

        public void MostrarArbol(Nodo? raiz)
        {
            if (raiz == null)
            {
                Console.WriteLine("[Árbol vacío]");
                return;
            }
            var q = new System.Collections.Generic.Queue<(Nodo node,int level)>();
            q.Enqueue((raiz, 0));
            int current = -1;
            while (q.Count > 0)
            {
                var (n, lvl) = q.Dequeue();
                if (lvl != current)
                {
                    current = lvl;
                    Console.Write("\n");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"Nivel {lvl}: ");
                    Console.ResetColor();
                }
                Console.Write($"[{n.Nombre}] ");
                if (n.Izquierdo != null) q.Enqueue((n.Izquierdo, lvl+1));
                if (n.Derecho != null) q.Enqueue((n.Derecho, lvl+1));
            }
            Console.WriteLine();
        }

        public void MostrarAltura(int altura)
        {
            Console.Write("\n");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Altura: ");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"{altura}");
            Console.ResetColor();
        }

        public void MostrarArbolAscii(Nodo? raiz)
        {
            if (raiz == null)
            {
                Console.WriteLine("[Árbol vacío]");
                return;
            }
            Console.WriteLine();
            ImprimirNodoAscii(raiz, prefix: string.Empty, esUltimo: true);
        }

        private void ImprimirNodoAscii(Nodo nodo, string prefix, bool esUltimo)
        {
            var conector = esUltimo ? "└── " : "├── ";
            Console.WriteLine($"{prefix}{conector}{nodo.Nombre}");

            var hijos = new System.Collections.Generic.List<Nodo>();
            if (nodo.Izquierdo != null) hijos.Add(nodo.Izquierdo);
            if (nodo.Derecho != null) hijos.Add(nodo.Derecho);

            for (int i = 0; i < hijos.Count; i++)
            {
                bool ultimoHijo = i == hijos.Count - 1;
                var nuevoPrefijo = prefix + (esUltimo ? "    " : "│   ");
                ImprimirNodoAscii(hijos[i], nuevoPrefijo, ultimoHijo);
            }
        }

        private void ImprimirRegla()
        {
            int ancho;
            try { ancho = Console.WindowWidth; if (ancho <= 0) ancho = 50; }
            catch { ancho = 50; }
            var largo = Math.Min(100, Math.Max(20, ancho - 1));
            Console.WriteLine(new string('─', largo));
        }
    }
}
