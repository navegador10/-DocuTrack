namespace DocuTrack.Model
{
    // Representa un elemento del BST (carpeta o archivo)
    public class Nodo
    {
        // Nombre del elemento (clave de comparación en el BST)
        public string Nombre { get; set; }
        // Indica si el nodo representa una carpeta (true) o un archivo (false)
        public bool EsCarpeta { get; set; }
        // Enlaces al hijo izquierdo y derecho (pueden ser nulos)
        public Nodo? Izquierdo { get; set; }
        public Nodo? Derecho { get; set; }

        // Constructor: inicializa nombre/tipo y deja hijos como nulos
        public Nodo(string nombre, bool esCarpeta)
        {
            Nombre = nombre;
            EsCarpeta = esCarpeta;
            Izquierdo = null;
            Derecho = null;
        }
    }
}
