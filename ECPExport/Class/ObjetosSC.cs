using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECPExport.Class
{
    public class ObjetosSC
    {
        public string Nome { get; set; }
        public string ObGuid { get; set; }
        public string Type { get; set; }
        public string Projeto { get; set; }
        public string ManterEvento { get; set; }
        public string Intervalo { get; set; }
        public string Servidor { get; set; }
        public string BancoDados { get; set; }
        public string ServidorBD { get; set; }
        public string BelongTo { get; set; }
        public string Resolucao { get; set; }
        public string Fabricante { get; set; }
        public string Modelo { get; set; }
        public string MAC { get; set; }
        public string TipoIP { get; set; }
        public string IP { get; set; }
        public string Mask { get; set; }
        public string Gateway { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Fuso { get; set; }
        public string TempoConcessao { get; set; }
        public string ConcessaoAmp { get; set; }
        public string Retrancamento { get; set; }
        public string InterfacePreferida { get; set; }
        public string SensorPorta { get; set; }
        public string Trancamento { get; set; }
        public bool LimpezaAutomatica { get; set; }
        public string Frequencia { get; set; }
        public string ModoGravacao { get; set; }
        public string Acoes { get; set; }
        public string Lado1Nome { get; set; }
        public string Lado1ObGuid { get; set; }
        public string Lado1Leitor { get; set; }
        public string Lado1REX { get; set; }
        public string Lado1SensorEntrada { get; set; }
        public string Lado1Camera { get; set; }
        public List<string> Lado1Regras { get; set; }
        public string Lado2Nome { get; set; }
        public string Lado2ObGuid { get; set; }
        public string Lado2Leitor { get; set; }
        public string Lado2REX { get; set; }
        public string Lado2SensorEntrada { get; set; }
        public string Lado2Camera { get; set; }
        public List<string> Lado2Regras { get; set; }
        public string Fluxo1Nome { get; set; }
        public string Fluxo1Guid { get; set; }
        public string Fluxo1Resolucao { get; set; }
        public string Fluxo1TipoConexao { get; set; }
        public string Fluxo1EndMulticast { get; set; }
        public string Fluxo1TaxaBits { get; set; }
        public string Fluxo1VelocidadeQuadro { get; set; }
        public string Fluxo1Uso { get; set; }
        public string Fluxo2Nome { get; set; }
        public string Fluxo2Guid { get; set; }
        public string Fluxo2Resolucao { get; set; }
        public string Fluxo2TipoConexao { get; set; }
        public string Fluxo2EndMulticast { get; set; }
        public string Fluxo2TaxaBits { get; set; }
        public string Fluxo2VelocidadeQuadro { get; set; }
        public string Fluxo2Uso { get; set; }
        public string Fluxo3Nome { get; set; }
        public string Fluxo3Guid { get; set; }
        public string Fluxo3Resolucao { get; set; }
        public string Fluxo3TipoConexao { get; set; }
        public string Fluxo3EndMulticast { get; set; }
        public string Fluxo3TaxaBits { get; set; }
        public string Fluxo3VelocidadeQuadro { get; set; }
        public string Fluxo3Uso { get; set; }
        public string Fluxo4Nome { get; set; }
        public string Fluxo4Guid { get; set; }
        public string Fluxo4Resolucao { get; set; }
        public string Fluxo4TipoConexao { get; set; }
        public string Fluxo4EndMulticast { get; set; }
        public string Fluxo4TaxaBits { get; set; }
        public string Fluxo4VelocidadeQuadro { get; set; }
        public string Fluxo4Uso { get; set; }
        public string Fluxo5Nome { get; set; }
        public string Fluxo5Guid { get; set; }
        public string Fluxo5Resolucao { get; set; }
        public string Fluxo5TipoConexao { get; set; }
        public string Fluxo5EndMulticast { get; set; }
        public string Fluxo5TaxaBits { get; set; }
        public string Fluxo5VelocidadeQuadro { get; set; }
        public string Fluxo5Uso { get; set; }
        public string Fluxo6Nome { get; set; }
        public string Fluxo6Guid { get; set; }
        public string Fluxo6Resolucao { get; set; }
        public string Fluxo6TipoConexao { get; set; }
        public string Fluxo6EndMulticast { get; set; }
        public string Fluxo6TaxaBits { get; set; }
        public string Fluxo6VelocidadeQuadro { get; set; }
        public string Fluxo6Uso { get; set; }
        public List<string> Dev { get; set; }
        public List<string> Particoes { get; set; }
        public List<string> EntradasDispositivos { get; set; }
        public List<string> Cameras { get; set; }


    }

    public class Parametros
    {
        public string ObGuid { get; set; }

    }

    public class Devices
    {
        public List<Guid> ObGuids { get; set; }
        public List<Object> Objetos { get; set; }

    }

    public class Logado
    {
        public bool Confirma { get; set; }
    }
}
