namespace ENTITY
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Unidad { get; set; }
        public string Emoji { get; set; }
        public int IdCategoria { get; set; }
        public string Comando { get; set; }

        public void GenerarComando()
        {
            Comando = "/" + Nombre
                          .ToLower()
                          .Replace(" ", "")
                          .Replace("á", "a").Replace("é", "e")
                          .Replace("í", "i").Replace("ó", "o")
                          .Replace("ú", "u");
        }
    }
}
