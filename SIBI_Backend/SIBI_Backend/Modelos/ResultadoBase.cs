namespace Web.Api.Softijs.Results
{
    public class ResultadoBase
    {
        public string Mensaje { set; get; } = null!;
        public bool Ok { set; get; }
        public string Error { get; set; }
        public int CodigoEstado { set; get; }
        public dynamic Resultado { get; set; }
    }
}