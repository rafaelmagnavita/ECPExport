using Genetec.Sdk;
using Genetec.Sdk.Entities;
using Genetec.Sdk.Queries;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Genetec.Sdk.Events;
using Genetec.Sdk.Workspace;
using System.Reflection;
using System.Windows.Controls;
using System.Windows;
using ECPExport.Class;
using StartProject;
using System.Threading;

namespace ECPExport
{
    public partial class UserControl1 : UserControl
    {

        private List<ObjetosSC> listaObj = new List<ObjetosSC>();
        private List<Parametros> listaPar = new List<Parametros>();
        private List<ObjetosSC> listaReg = new List<ObjetosSC>();
        private List<ObjetosSC> listaLado = new List<ObjetosSC>();
        private List<ObjetosSC> listaFluxos = new List<ObjetosSC>();

        public UserControl1()
        {
            InitializeComponent();
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(delegate ()
            {
                RetornarAccessManager();
                RetornarUnit();
                REGRAS();
                LADOS();
                RetornarDoor();
                RetornarArchiver();
                RetornarVideoUnit();
                FLUXOS();
                RetornarCamera();
                //Licenses();
                iModZones();
                MessageBox.Show("Sincronizado com Sucesso!");               
            });
            thread.Start();
        }
        /// <summary>
        /// Função para obter dados do role Access Manager a partir de alguma controladora existente no projeto.
        /// </summary>
        /// <returns></returns>
        public List<Unit> RetornarAccessManager()
        {
            listaObj.Clear();
            EntityConfigurationQuery query;
            QueryCompletedEventArgs result;
            List<Unit> accessmanagers = new List<Unit>();

            query = Main.engine.ReportManager.CreateReportQuery(ReportType.EntityConfiguration) as EntityConfigurationQuery;
            query.EntityTypeFilter.Add(EntityType.Unit);
            query.NameSearchMode = StringSearchMode.StartsWith;
            result = query.Query();
            SystemConfiguration systemConfiguration = Main.engine.GetEntity(SdkGuids.SystemConfiguration) as SystemConfiguration;
            var service = systemConfiguration.CustomFieldService;
            if (result.Success)
            {
                try
                {
                    foreach (DataRow dr in result.Data.Rows)
                    {
                        Unit objetos = Main.engine.GetEntity((Guid)dr[0]) as Unit;
                        ObjetosSC ob = new ObjetosSC();
                        if (!string.IsNullOrEmpty(objetos.Guid.ToString()) && !objetos.OwnerRoleType.ToString().Contains("SecurityCenter")
                            && !objetos.OwnerRole.ToString().Contains("00000000-0000-0000-0000-000000000000"))
                        {
                            var z = listaObj.Where(a => a.ObGuid == objetos.AccessManagerRole.Guid.ToString());
                            if (z.Count() == 0)
                            {
                                var a = Main.engine.LicenseManager;
                                List<Guid> ads = new List<Guid>();
                                ads.Add(objetos.Guid);
                                var e = a.RequestLicenseUsageAsync(ads);
                                ob.Nome = objetos.AccessManagerRole.Name.ToString();
                                ob.Type = objetos.AccessManagerRole.Type.ToString();
                                ob.ObGuid = objetos.AccessManagerRole.Guid.ToString();
                                ob.BancoDados = objetos.AccessManagerRole.DatabaseName.ToString();
                                ob.ServidorBD = objetos.AccessManagerRole.DatabaseServer.ToString();
                                AccessManagerRole Sss = (AccessManagerRole)Main.engine.GetEntity(objetos.AccessManagerRole.Guid);
                                ObjetosSC LogRetention = new ObjetosSC();
                                Utils.XMLRole(Sss, "XmlAccessManager/LogRetention", LogRetention);
                                ObjetosSC KeepHistory = new ObjetosSC();
                                Utils.XMLRole(Sss, "XmlAccessManager/KeepHistory", KeepHistory);
                                if (KeepHistory.Nome == "true")
                                {
                                    ob.ManterEvento = "true";
                                }
                                else
                                {
                                    ob.ManterEvento = "false";
                                    ob.Frequencia = LogRetention.Nome;
                                }
                                var acoes = "";
                                foreach (var acao in objetos.AccessManagerRole.EventToActions)
                                {
                                    if (acoes != "")
                                    {
                                        acoes = acoes + ", " + "Evento:" + acao.EventType.ToString() + "---> Ação:" + acao.Id.ToString();

                                    }
                                    else
                                    {
                                        acoes = "Evento:" + acao.EventType.ToString() + "-> Ação:" + acao.Id.ToString();
                                    }
                                }
                                ob.Acoes = acoes;
                            }
                        }
                        if (ob.ObGuid != null)
                            listaObj.Add(ob);
                    }
                    CreateXML(listaObj, @"C:\Logs\AccessManagers.xml");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro carregar Access Manager.: " + ex.Message);
                }

            }
            return accessmanagers;
        }
        /// <summary>
        /// Função para retornar controladoras de acesso.
        /// </summary>
        /// <returns></returns>
        public List<Unit> RetornarUnit()
        {
            listaObj.Clear();
            listaPar.Clear();
            EntityConfigurationQuery query;
            QueryCompletedEventArgs result;
            List<Unit> units = new List<Unit>();
            query = Main.engine.ReportManager.CreateReportQuery(ReportType.EntityConfiguration) as EntityConfigurationQuery;
            query.EntityTypeFilter.Add(EntityType.Unit);
            query.NameSearchMode = StringSearchMode.StartsWith;
            result = query.Query();
            SystemConfiguration systemConfiguration = Main.engine.GetEntity(SdkGuids.SystemConfiguration) as SystemConfiguration;
            var service = systemConfiguration.CustomFieldService;
            if (result.Success)
            {
                try
                {
                    foreach (DataRow dr in result.Data.Rows)    //sempre remove todas as regras de um CardHolder
                    {
                        Unit objetos = Main.engine.GetEntity((Guid)dr[0]) as Unit;
                        ObjetosSC ob = new ObjetosSC();
                        Parametros parametros = new Parametros();
                        if (!string.IsNullOrEmpty(objetos.Guid.ToString()) && !objetos.OwnerRoleType.ToString().Contains("SecurityCenter")
                            && !objetos.OwnerRole.ToString().Contains("00000000-0000-0000-0000-000000000000"))
                        {
                            ob.ObGuid = objetos.Guid.ToString();
                            ob.Type = objetos.EntityType.ToString();
                            ob.BelongTo = objetos.OwnerRole.ToString();
                            ob.Nome = objetos.Name.ToString();
                            ob.MAC = objetos.MacAddress.ToString();
                            ob.IP = objetos.IPAddress.ToString();
                            ObjetosSC Fabricante = new ObjetosSC();
                            ObjetosSC Modelo = new ObjetosSC();
                            ObjetosSC Username = new ObjetosSC();
                            ObjetosSC Password = new ObjetosSC();
                            Utils.NonPublic(objetos, "InternalEntity", "Manufacturer", null, Fabricante);
                            ob.Fabricante = Fabricante.Nome;
                            Utils.NonPublic(objetos, "InternalEntity", "Model", null, Modelo);
                            ob.Modelo = Modelo.Nome;
                            Utils.NonPublic(objetos, "InternalEntity", "Username", null, Username);
                            ob.Login = Username.Nome != null ? Username.Nome : "";
                            ObjetosSC Gateway = new ObjetosSC();
                            Utils.XMLEntity(objetos, "Gateway", Gateway);
                            ob.Gateway = Gateway.Nome;
                            ObjetosSC Mask = new ObjetosSC();
                            Utils.XMLEntity(objetos, "NetMask", Mask);
                            ob.Mask = Mask.Nome;

                            var acoes = "";
                            foreach (var acao in objetos.EventToActions)
                            {
                                if (acoes != "")
                                {
                                    acoes = acoes + ", " + "Evento:" + acao.EventType.ToString() + "---> Ação:" + acao.Id.ToString();

                                }
                                else
                                {
                                    acoes = "Evento:" + acao.EventType.ToString() + "-> Ação:" + acao.Id.ToString();
                                }
                            }
                            ob.Acoes = acoes;
                        }
                        else
                        {
                            ob.BelongTo = objetos.OwnerRole.ToString();
                            if (listaPar.Where(a => a.ObGuid == ob.BelongTo).Count() == 0 && objetos.Guid.ToString() != null)
                            {
                                parametros.ObGuid = objetos.Guid.ToString();
                                listaPar.Add(parametros);
                            }
                        }
                        if (ob.ObGuid != null)
                        {
                            listaObj.Add(ob);
                        }
                    }
                    CreateXML(listaObj, @"C:\Logs\Controllers.xml");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro carregar Access Unit.: " + ex.Message);
                }

            }
            return units;
        }

        /// <summary>
        /// Função para retornar dados de porta e , juntando com a função dos lados e regras, salva no mesmo XML a porta com seus
        /// respetivos lados, regras e dispositivos.
        /// </summary>
        /// <returns></returns>
        public List<Door> RetornarDoor()
        {
            listaObj.Clear();
            EntityConfigurationQuery query;
            QueryCompletedEventArgs result;
            List<Door> portas = new List<Door>();

            query = Main.engine.ReportManager.CreateReportQuery(ReportType.EntityConfiguration) as EntityConfigurationQuery;
            query.EntityTypeFilter.Add(EntityType.Door);
            query.NameSearchMode = StringSearchMode.StartsWith;
            result = query.Query();
            SystemConfiguration systemConfiguration = Main.engine.GetEntity(SdkGuids.SystemConfiguration) as SystemConfiguration;
            var service = systemConfiguration.CustomFieldService;
            if (result.Success)
            {
                try
                {
                    foreach (DataRow dr in result.Data.Rows)    //sempre remove todas as regras de um CardHolder
                    {
                        Door objetos = Main.engine.GetEntity((Guid)dr[0]) as Door;
                        ObjetosSC ob = new ObjetosSC();
                        //ob.Nome = objetos.Name;
                        if (!string.IsNullOrEmpty(objetos.Guid.ToString()) && objetos.Units.FirstOrDefault().ToString() != "00000000-0000-0000-0000-000000000000"
                            && listaPar.Where(a => a.ObGuid == objetos.Units.FirstOrDefault().ToString()).Count() == 0)
                        {
                            ob.ObGuid = objetos.Guid.ToString();
                            ob.Type = objetos.EntityType.ToString();
                            ob.Nome = objetos.Name.ToString();
                            ob.BelongTo = objetos.Units.FirstOrDefault().ToString();
                            ob.TempoConcessao = objetos.StandardGrantTimeInSeconds.ToString();
                            ob.ConcessaoAmp = objetos.ExtendedGrantTimeInSeconds.ToString();
                            ob.Retrancamento = objetos.RelockDelayInSeconds.ToString();
                            ob.InterfacePreferida = Main.engine.GetEntity(objetos.PreferredInterface) != null ? Main.engine.GetEntity(objetos.PreferredInterface).Name : "";
                            ob.Trancamento = Main.engine.GetEntity(objetos.DoorLockDevice) != null ? Main.engine.GetEntity(objetos.DoorLockDevice).Name : "";
                            ObjetosSC DoorSensor = new ObjetosSC();
                            Utils.NonPublic(objetos, "InternalEntity", "DoorSensor", "Device", DoorSensor);
                            Guid ds = new Guid(DoorSensor.Nome);
                            ob.SensorPorta = Main.engine.GetEntity(ds) != null ? Main.engine.GetEntity(ds).Name : "";
                            var acoes = "";
                            foreach (var acao in objetos.EventToActions)
                            {
                                if (acoes != "")
                                {
                                    acoes = acoes + ", " + "Evento:" + acao.EventType.ToString() + "---> Ação:" + acao.Id.ToString();

                                }
                                else
                                {
                                    acoes = "Evento:" + acao.EventType.ToString() + "-> Ação:" + acao.Id.ToString();
                                }
                            }
                            ob.Acoes = acoes;
                            bool lado1ein = false;
                            List<string> lados = new List<string>();
                            var numlados = listaLado.Where(a => a.BelongTo == ob.ObGuid);
                            int i = 1;
                            while (i <= 2)
                            {
                                if (i == 1)
                                {
                                    ob.Lado1Nome = numlados.FirstOrDefault().Nome;
                                    ob.Lado1ObGuid = numlados.FirstOrDefault().ObGuid;
                                    ob.Lado1Camera = numlados.FirstOrDefault().Lado1Camera;
                                    if (ob.Lado1ObGuid == objetos.DoorSideIn.Reader.Guid.ToString())
                                    {
                                        lado1ein = true;
                                        List<string> ladoRegra = new List<string>();
                                        foreach (var regra in objetos.DoorSideIn.Reader.AccessRules)
                                        {
                                            var nomeRegra = listaReg.Where(a => a.ObGuid == regra.ToString()).FirstOrDefault();
                                            ladoRegra.Add(nomeRegra.Nome);
                                        }
                                        ob.Lado1Regras = ladoRegra;
                                    }
                                    else
                                    {
                                        List<string> ladoRegra = new List<string>();
                                        foreach (var regra in objetos.DoorSideOut.Reader.AccessRules)
                                        {
                                            var nomeRegra = listaReg.Where(a => a.ObGuid == regra.ToString()).FirstOrDefault();
                                            ladoRegra.Add(nomeRegra.Nome);
                                        }
                                        ob.Lado1Regras = ladoRegra;
                                    }
                                    i = i + 1;
                                }
                                if (i == 2)
                                {
                                    var numlados2 = numlados.Where(o => o.ObGuid != ob.Lado1ObGuid).FirstOrDefault();
                                    ob.Lado2Nome = numlados2.Nome;
                                    ob.Lado2ObGuid = numlados2.ObGuid;
                                    ob.Lado2Camera = numlados2.Lado1Camera;
                                    if (ob.Lado2ObGuid == objetos.DoorSideIn.Reader.Guid.ToString())
                                    {
                                        List<string> ladoRegra = new List<string>();
                                        foreach (var regra in objetos.DoorSideIn.Reader.AccessRules)
                                        {
                                            var nomeRegra = listaReg.Where(a => a.ObGuid == regra.ToString()).FirstOrDefault();
                                            ladoRegra.Add(nomeRegra.Nome);
                                        }
                                        ob.Lado2Regras = ladoRegra;
                                    }
                                    else
                                    {
                                        List<string> ladoRegra = new List<string>();
                                        foreach (var regra in objetos.DoorSideOut.Reader.AccessRules)
                                        {
                                            var nomeRegra = listaReg.Where(a => a.ObGuid == regra.ToString()).FirstOrDefault();
                                            ladoRegra.Add(nomeRegra.Nome);
                                        }
                                        ob.Lado2Regras = ladoRegra;
                                    }
                                    i = i + 1;
                                }
                            }
                            Devices Devices = new Devices();
                            Utils.NonPublicList(objetos, "InternalEntity", "Devices", null, Devices);
                            foreach (var guid in Devices.ObGuids)
                            {
                                Device atest = (Device)Main.engine.GetEntity(guid);
                                if (atest.DeviceType.ToString() == "Reader")
                                {
                                    if (atest.AccessPoint.FirstOrDefault().ToString() == ob.Lado1ObGuid)
                                    {
                                        ob.Lado1Leitor = atest.Name.ToString();
                                    }
                                    if (atest.AccessPoint.FirstOrDefault().ToString() == ob.Lado2ObGuid)
                                    {
                                        ob.Lado2Leitor = atest.Name.ToString();
                                    }
                                }
                                if (atest.DeviceType.ToString() == "Input")
                                {
                                    if (lado1ein == true)
                                    {
                                        if (atest.AccessPoint.FirstOrDefault() == objetos.DoorSideIn.Rex.Guid)
                                        {
                                            ob.Lado1REX = atest.Name.ToString();
                                        }
                                        if (atest.AccessPoint.FirstOrDefault() == objetos.DoorSideIn.EntrySensor.Guid)
                                        {
                                            ob.Lado1SensorEntrada = atest.Name.ToString();
                                        }
                                        if (atest.AccessPoint.FirstOrDefault() == objetos.DoorSideOut.Rex.Guid)
                                        {
                                            ob.Lado2REX = atest.Name.ToString();
                                        }
                                        if (atest.AccessPoint.FirstOrDefault() == objetos.DoorSideOut.EntrySensor.Guid)
                                        {
                                            ob.Lado2SensorEntrada = atest.Name.ToString();
                                        }
                                    }
                                    if (lado1ein == false)
                                    {
                                        if (atest.AccessPoint.FirstOrDefault() == objetos.DoorSideIn.Rex.Guid)
                                        {
                                            ob.Lado2REX = atest.Name.ToString();
                                        }
                                        if (atest.AccessPoint.FirstOrDefault() == objetos.DoorSideIn.EntrySensor.Guid)
                                        {
                                            ob.Lado2SensorEntrada = atest.Name.ToString();
                                        }
                                        if (atest.AccessPoint.FirstOrDefault() == objetos.DoorSideOut.Rex.Guid)
                                        {
                                            ob.Lado1REX = atest.Name.ToString();
                                        }
                                        if (atest.AccessPoint.FirstOrDefault() == objetos.DoorSideOut.EntrySensor.Guid)
                                        {
                                            ob.Lado1SensorEntrada = atest.Name.ToString();
                                        }
                                    }
                                }
                            }
                        }
                        if (ob.ObGuid != null)
                        {
                            listaObj.Add(ob);
                        }
                    }
                    CreateXML(listaObj, @"C:\Logs\Portas.xml");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro carregar Porta.: " + ex.Message);

                }

            }
            return portas;
        }
        /// <summary>
        /// Função para retornar archivers a partir de video units existentes
        /// </summary>
        /// <returns></returns>
        public List<VideoUnit> RetornarArchiver()
        {
            listaObj.Clear();
            EntityConfigurationQuery query;
            QueryCompletedEventArgs result;
            List<VideoUnit> archivers = new List<VideoUnit>();

            query = Main.engine.ReportManager.CreateReportQuery(ReportType.EntityConfiguration) as EntityConfigurationQuery;
            query.EntityTypeFilter.Add(EntityType.VideoUnit);
            query.NameSearchMode = StringSearchMode.StartsWith;
            result = query.Query();
            SystemConfiguration systemConfiguration = Main.engine.GetEntity(SdkGuids.SystemConfiguration) as SystemConfiguration;
            var service = systemConfiguration.CustomFieldService;
            if (result.Success)
            {
                try
                {
                    foreach (DataRow dr in result.Data.Rows)    //sempre remove todas as regras de um CardHolder
                    {
                        VideoUnit objetos = Main.engine.GetEntity((Guid)dr[0]) as VideoUnit;
                        ObjetosSC ob = new ObjetosSC();
                        if (!objetos.Guid.ToString().Contains("fafa") && !objetos.OwnerRoleType.ToString().Contains("SecurityCenter"))
                        {
                            if (!string.IsNullOrEmpty(objetos.Guid.ToString()))
                            {
                                var z = listaObj.Where(a => a.ObGuid == objetos.ArchiverRole.Guid.ToString());
                                if (z.Count() == 0)
                                {
                                    ob.Nome = objetos.ArchiverRole.Name.ToString();
                                    ob.Type = objetos.ArchiverRole.Type.ToString();
                                    ob.ObGuid = objetos.ArchiverRole.Guid.ToString();
                                    ob.BancoDados = objetos.ArchiverRole.DatabaseName.ToString();
                                    ob.ServidorBD = objetos.ArchiverRole.DatabaseServer.ToString();
                                    ArchiverRole Sss = (ArchiverRole)Main.engine.GetEntity(objetos.ArchiverRole.Guid);
                                    ob.LimpezaAutomatica = Sss.RecordingConfiguration.AutomaticCleanup;
                                    ob.Frequencia = Sss.RecordingConfiguration.RetentionPeriod.Days.ToString();

                                    ObjetosSC LimpezaAutomatica = new ObjetosSC();
                                    ObjetosSC Frequencia = new ObjetosSC();
                                    Server serv = (Server)Main.engine.GetEntity(Sss.CurrentServer);
                                    ob.Servidor = serv.Name.ToString();
                                    var modos = "";
                                    foreach (var modo in Sss.RecordingConfiguration.ScheduledRecordingModes)
                                    {
                                        if (modos != "")
                                        {
                                            modos = modos + ", " + modo.Mode.ToString();

                                        }
                                        else
                                        {
                                            modos = modo.Mode.ToString();
                                        }
                                    }
                                    ob.ModoGravacao = modos;
                                    var acoes = "";
                                    foreach (var acao in objetos.EventToActions)
                                    {
                                        if (acoes != "")
                                        {
                                            acoes = acoes + ", " + "Evento:" + acao.EventType.ToString() + "---> Ação:" + acao.Id.ToString();

                                        }
                                        else
                                        {
                                            acoes = "Evento:" + acao.EventType.ToString() + "-> Ação:" + acao.Id.ToString();
                                        }
                                    }
                                    ob.Acoes = acoes;
                                }
                            }
                        }
                        if (ob.ObGuid != null)
                            listaObj.Add(ob);
                    }
                    CreateXML(listaObj, @"C:\Logs\Archivers.xml");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro carregar Archiver.: " + ex.Message);
                }
            }
            return archivers;
        }
        /// <summary>
        /// Função para retornar uma video unit
        /// </summary>
        /// <returns></returns>
        public List<VideoUnit> RetornarVideoUnit()
        {
            listaObj.Clear();
            listaPar.Clear();
            EntityConfigurationQuery query;
            QueryCompletedEventArgs result;
            List<VideoUnit> videounits = new List<VideoUnit>();

            query = Main.engine.ReportManager.CreateReportQuery(ReportType.EntityConfiguration) as EntityConfigurationQuery;
            query.EntityTypeFilter.Add(EntityType.VideoUnit);
            query.NameSearchMode = StringSearchMode.StartsWith;
            result = query.Query();
            SystemConfiguration systemConfiguration = Main.engine.GetEntity(SdkGuids.SystemConfiguration) as SystemConfiguration;
            var service = systemConfiguration.CustomFieldService;
            if (result.Success)
            {
                try
                {
                    foreach (DataRow dr in result.Data.Rows)    //sempre remove todas as regras de um CardHolder
                    {
                        VideoUnit objetos = Main.engine.GetEntity((Guid)dr[0]) as VideoUnit;
                        ObjetosSC ob = new ObjetosSC();
                        Parametros parametros = new Parametros();
                        if (!string.IsNullOrEmpty(objetos.Guid.ToString()) && !objetos.OwnerRoleType.ToString().Contains("SecurityCenter")
                            && !objetos.Guid.ToString().Contains("fafa"))
                        {
                            ob.ObGuid = objetos.Guid.ToString();
                            ob.Type = objetos.EntityType.ToString();
                            ob.Nome = objetos.Name.ToString();
                            ob.BelongTo = objetos.ArchiverRoleGuid.ToString();
                            ob.TipoIP = objetos.Dhcp.ToString();
                            ob.IP = objetos.IPAddress.ToString();
                            ob.MAC = objetos.MacAddress.ToString();
                            ob.Mask = objetos.SubnetMask.ToString();
                            ob.Gateway = objetos.Gateway.ToString();
                            ob.Login = objetos.Username.ToString();
                            ob.Latitude = objetos.Latitude.ToString();
                            ob.Longitude = objetos.Longitude.ToString();
                            ob.Fuso = objetos.TimeZone.DisplayName.ToString();
                            ob.Fabricante = objetos.Manufacturer.ToString();
                            ob.Modelo = objetos.Model.ToString();
                            var acoes = "";
                            foreach (var acao in objetos.EventToActions)
                            {
                                if (acoes != "")
                                {
                                    acoes = acoes + ", " + "Evento:" + acao.EventType.ToString() + "---> Ação:" + acao.Id.ToString();

                                }
                                else
                                {
                                    acoes = "Evento:" + acao.EventType.ToString() + "-> Ação:" + acao.Id.ToString();
                                }
                            }
                            ob.Acoes = acoes;
                        }
                        else
                        {
                            if (listaPar.Where(a => a.ObGuid == objetos.OwnerRole.ToString()).Count() == 0 && objetos.Guid.ToString() != null)
                            {
                                ob.BelongTo = objetos.OwnerRole.ToString();
                                parametros.ObGuid = objetos.Guid.ToString();
                                listaPar.Add(parametros);
                            }
                        }
                        if (ob.ObGuid != null)
                            listaObj.Add(ob);

                    }
                    CreateXML(listaObj, @"C:\Logs\VideoUnits.xml");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro carregar Video Unit.: " + ex.Message);
                }

            }
            return videounits;
        }
        /// <summary>
        /// Função para retornar dados das câmeras e junto com a função de fluxos, une na XML as câmeras com seus
        /// respectivos fluxos
        /// </summary>
        /// <returns></returns>
        public List<Camera> RetornarCamera()
        {
            listaObj.Clear();

            EntityConfigurationQuery query;
            QueryCompletedEventArgs result;
            List<Camera> cameras = new List<Camera>();

            query = Main.engine.ReportManager.CreateReportQuery(ReportType.EntityConfiguration) as EntityConfigurationQuery;
            query.EntityTypeFilter.Add(EntityType.Camera);
            query.NameSearchMode = StringSearchMode.StartsWith;
            result = query.Query();
            SystemConfiguration systemConfiguration = Main.engine.GetEntity(SdkGuids.SystemConfiguration) as SystemConfiguration;
            var service = systemConfiguration.CustomFieldService;
            if (result.Success)
            {
                try
                {
                    foreach (DataRow dr in result.Data.Rows)    //sempre remove todas as regras de um CardHolder
                    {
                        Camera objetos = Main.engine.GetEntity((Guid)dr[0]) as Camera;
                        ObjetosSC ob = new ObjetosSC();
                        if (!string.IsNullOrEmpty(objetos.Guid.ToString()) && objetos.Unit.ToString() != "00000000-0000-0000-0000-000000000000"
                            && listaPar.Where(a => a.ObGuid == objetos.Unit.ToString()).Count() == 0)
                        {
                            ob.ObGuid = objetos.Guid.ToString();
                            ob.Type = objetos.EntityType.ToString();
                            ob.Nome = objetos.Name.ToString();
                            ob.BelongTo = objetos.Unit.ToString();
                            ob.LimpezaAutomatica = objetos.RecordingConfiguration.AutomaticCleanup;
                            ob.Frequencia = objetos.RecordingConfiguration.RetentionPeriod.Days.ToString();
                            var modos = "";
                            foreach (var modo in objetos.RecordingConfiguration.ScheduledRecordingModes)
                            {
                                if (modos != "")
                                {
                                    modos = modos + ", " + modo.Mode.ToString();

                                }
                                else
                                {
                                    modos = modo.Mode.ToString();
                                }
                            }
                            ob.ModoGravacao = modos;
                            var acoes = "";
                            foreach (var acao in objetos.EventToActions)
                            {
                                if (acoes != "")
                                {
                                    acoes = acoes + ", " + "Evento:" + acao.EventType.ToString() + "---> Ação:" + acao.Id.ToString();

                                }
                                else
                                {
                                    acoes = "Evento:" + acao.EventType.ToString() + "-> Ação:" + acao.Id.ToString();
                                }
                            }
                            ob.Acoes = acoes;
                            var fluxos = listaFluxos.Where(a => a.BelongTo == ob.ObGuid);
                            int i = 1;
                            if (fluxos.Count() != 0)
                            {
                                while (i <= 6)
                                {
                                    if (i == 1)
                                    {
                                        ob.Fluxo1Nome = fluxos.FirstOrDefault().Nome;
                                        ob.Fluxo1Guid = fluxos.FirstOrDefault().ObGuid;
                                        ob.Fluxo1Resolucao = fluxos.FirstOrDefault().Resolucao;
                                        ob.Fluxo1TipoConexao = fluxos.FirstOrDefault().Fluxo1TipoConexao;
                                        ob.Fluxo1EndMulticast = fluxos.FirstOrDefault().Fluxo1EndMulticast;
                                        ob.Fluxo1TaxaBits = fluxos.FirstOrDefault().Fluxo1TaxaBits;
                                        ob.Fluxo1VelocidadeQuadro = fluxos.FirstOrDefault().Fluxo1VelocidadeQuadro;

                                        string Uso = "";
                                        foreach (var usodefx in objetos.StreamUsages)
                                        {
                                            if (usodefx.Stream.ToString() == ob.Fluxo1Guid)
                                            {
                                                Uso = Uso == "" ? usodefx.Usage.ToString() : Uso + ", " + usodefx.Usage.ToString();
                                            }
                                        }
                                        ob.Fluxo1Uso = Uso;
                                        i = i + 1;
                                    }
                                    if (i == 2)
                                    {
                                        var fluxos2 = fluxos.Where(a => a.ObGuid != ob.Fluxo1Guid);
                                        if (fluxos2.Count() != 0)
                                        {
                                            ob.Fluxo2Nome = fluxos2.FirstOrDefault().Nome;
                                            ob.Fluxo2Guid = fluxos2.FirstOrDefault().ObGuid;
                                            ob.Fluxo2Resolucao = fluxos2.FirstOrDefault().Resolucao;
                                            ob.Fluxo2TipoConexao = fluxos2.FirstOrDefault().Fluxo1TipoConexao;
                                            ob.Fluxo2EndMulticast = fluxos2.FirstOrDefault().Fluxo1EndMulticast;
                                            ob.Fluxo2TaxaBits = fluxos2.FirstOrDefault().Fluxo1TaxaBits;
                                            ob.Fluxo2VelocidadeQuadro = fluxos2.FirstOrDefault().Fluxo1VelocidadeQuadro;
                                            string Uso = "";
                                            foreach (var usodefx in objetos.StreamUsages)
                                            {
                                                if (usodefx.Stream.ToString() == ob.Fluxo2Guid)
                                                {
                                                    Uso = Uso == "" ? usodefx.Usage.ToString() : Uso + ", " + usodefx.Usage.ToString();
                                                }
                                            }
                                            ob.Fluxo2Uso = Uso;
                                            i = i + 1;
                                        }
                                        else
                                            i = 7;
                                    }
                                    if (i == 3)
                                    {
                                        var fluxos3 = fluxos.Where(a => a.ObGuid != ob.Fluxo1Guid && a.ObGuid != ob.Fluxo2Guid);
                                        if (fluxos3.Count() != 0)
                                        {
                                            ob.Fluxo3Nome = fluxos3.FirstOrDefault().Nome;
                                            ob.Fluxo3Guid = fluxos3.FirstOrDefault().ObGuid;
                                            ob.Fluxo3Resolucao = fluxos3.FirstOrDefault().Resolucao;
                                            ob.Fluxo3TipoConexao = fluxos3.FirstOrDefault().Fluxo1TipoConexao;
                                            ob.Fluxo3EndMulticast = fluxos3.FirstOrDefault().Fluxo1EndMulticast;
                                            ob.Fluxo3TaxaBits = fluxos3.FirstOrDefault().Fluxo1TaxaBits;
                                            ob.Fluxo3VelocidadeQuadro = fluxos3.FirstOrDefault().Fluxo1VelocidadeQuadro;
                                            string Uso = "";
                                            foreach (var usodefx in objetos.StreamUsages)
                                            {
                                                if (usodefx.Stream.ToString() == ob.Fluxo3Guid)
                                                {
                                                    Uso = Uso == "" ? usodefx.Usage.ToString() : Uso + ", " + usodefx.Usage.ToString();
                                                }
                                            }
                                            ob.Fluxo3Uso = Uso;
                                            i = i + 1;
                                        }
                                        else
                                            i = 7;
                                    }
                                    if (i == 4)
                                    {
                                        var fluxos4 = fluxos.Where(a => a.ObGuid != ob.Fluxo1Guid && a.ObGuid != ob.Fluxo2Guid && a.ObGuid != ob.Fluxo3Guid);
                                        if (fluxos4.Count() != 0)
                                        {
                                            ob.Fluxo4Nome = fluxos4.FirstOrDefault().Nome;
                                            ob.Fluxo4Guid = fluxos4.FirstOrDefault().ObGuid;
                                            ob.Fluxo4Resolucao = fluxos4.FirstOrDefault().Resolucao;
                                            ob.Fluxo4TipoConexao = fluxos4.FirstOrDefault().Fluxo1TipoConexao;
                                            ob.Fluxo4EndMulticast = fluxos4.FirstOrDefault().Fluxo1EndMulticast;
                                            ob.Fluxo4TaxaBits = fluxos4.FirstOrDefault().Fluxo1TaxaBits;
                                            ob.Fluxo4VelocidadeQuadro = fluxos4.FirstOrDefault().Fluxo1VelocidadeQuadro;
                                            string Uso = "";
                                            foreach (var usodefx in objetos.StreamUsages)
                                            {
                                                if (usodefx.Stream.ToString() == ob.Fluxo4Guid)
                                                {
                                                    Uso = Uso == "" ? usodefx.Usage.ToString() : Uso + ", " + usodefx.Usage.ToString();
                                                }
                                            }
                                            ob.Fluxo4Uso = Uso;
                                            i = i + 1;
                                        }
                                        else
                                            i = 7;
                                    }
                                    if (i == 5)
                                    {
                                        var fluxos5 = fluxos.Where(a => a.ObGuid != ob.Fluxo1Guid && a.ObGuid != ob.Fluxo2Guid && a.ObGuid != ob.Fluxo3Guid && a.ObGuid != ob.Fluxo4Guid);
                                        if (fluxos5.Count() != 0)
                                        {
                                            ob.Fluxo5Nome = fluxos5.FirstOrDefault().Nome;
                                            ob.Fluxo5Guid = fluxos5.FirstOrDefault().ObGuid;
                                            ob.Fluxo5Resolucao = fluxos5.FirstOrDefault().Resolucao;
                                            ob.Fluxo5TipoConexao = fluxos5.FirstOrDefault().Fluxo1TipoConexao;
                                            ob.Fluxo5EndMulticast = fluxos5.FirstOrDefault().Fluxo1EndMulticast;
                                            ob.Fluxo5TaxaBits = fluxos5.FirstOrDefault().Fluxo1TaxaBits;
                                            ob.Fluxo5VelocidadeQuadro = fluxos5.FirstOrDefault().Fluxo1VelocidadeQuadro;
                                            string Uso = "";
                                            foreach (var usodefx in objetos.StreamUsages)
                                            {
                                                if (usodefx.Stream.ToString() == ob.Fluxo5Guid)
                                                {
                                                    Uso = Uso == "" ? usodefx.Usage.ToString() : Uso + ", " + usodefx.Usage.ToString();
                                                }
                                            }
                                            ob.Fluxo5Uso = Uso;
                                            i = i + 1;
                                        }
                                        else
                                            i = 7;
                                    }
                                    if (i == 6)
                                    {
                                        var fluxos6 = fluxos.Where(a => a.ObGuid != ob.Fluxo1Guid && a.ObGuid != ob.Fluxo2Guid && a.ObGuid != ob.Fluxo3Guid && a.ObGuid != ob.Fluxo4Guid
                                         && a.ObGuid != ob.Fluxo5Guid);
                                        if (fluxos6.Count() != 0)
                                        {
                                            ob.Fluxo6Nome = fluxos6.FirstOrDefault().Nome;
                                            ob.Fluxo6Guid = fluxos6.FirstOrDefault().ObGuid;
                                            ob.Fluxo6Resolucao = fluxos6.FirstOrDefault().Resolucao;
                                            ob.Fluxo6TipoConexao = fluxos6.FirstOrDefault().Fluxo1TipoConexao;
                                            ob.Fluxo6EndMulticast = fluxos6.FirstOrDefault().Fluxo1EndMulticast;
                                            ob.Fluxo6TaxaBits = fluxos6.FirstOrDefault().Fluxo1TaxaBits;
                                            ob.Fluxo6VelocidadeQuadro = fluxos6.FirstOrDefault().Fluxo1VelocidadeQuadro;
                                            string Uso = "";
                                            foreach (var usodefx in objetos.StreamUsages)
                                            {
                                                if (usodefx.Stream.ToString() == ob.Fluxo6Guid)
                                                {
                                                    Uso = Uso == "" ? usodefx.Usage.ToString() : Uso + ", " + usodefx.Usage.ToString();
                                                }
                                            }
                                            ob.Fluxo6Uso = Uso;
                                            i = i + 1;
                                        }
                                        else
                                            i = 7;
                                    }
                                }
                            }
                        }
                        if (ob.ObGuid != null)
                            listaObj.Add(ob);

                    }
                    CreateXML(listaObj, @"C:\Logs\Cameras.xml");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro carregar Câmera.: " + ex.Message);
                }

            }
            return cameras;
        }

        /// <summary>
        /// Retorna todas as regras com seu guid
        /// </summary>
        /// <returns></returns>
        public List<AccessRule> REGRAS()
        {
            listaReg.Clear();

            EntityConfigurationQuery query;
            QueryCompletedEventArgs result;
            List<AccessRule> rules = new List<AccessRule>();

            query = Main.engine.ReportManager.CreateReportQuery(ReportType.EntityConfiguration) as EntityConfigurationQuery;
            query.EntityTypeFilter.Add(EntityType.AccessRule);
            query.NameSearchMode = StringSearchMode.StartsWith;
            result = query.Query();
            SystemConfiguration systemConfiguration = Main.engine.GetEntity(SdkGuids.SystemConfiguration) as SystemConfiguration;
            var service = systemConfiguration.CustomFieldService;
            if (result.Success)
            {
                try
                {
                    foreach (DataRow dr in result.Data.Rows)
                    {
                        AccessRule regra = Main.engine.GetEntity((Guid)dr[0]) as AccessRule;
                        ObjetosSC rg = new ObjetosSC();
                        if (!string.IsNullOrEmpty(regra.Guid.ToString()))
                        {
                            rg.ObGuid = regra.Guid.ToString();
                            rg.Nome = regra.Name.ToString();
                        }
                        if (rg.ObGuid != null)
                            listaReg.Add(rg);

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro carregar Regra.: " + ex.Message);
                }

            }
            return rules;

        }
        /// <summary>
        /// Retorna todos os lados com algumas propriedades
        /// </summary>
        /// <returns></returns>
        public List<DoorSide> LADOS()
        {
            listaLado.Clear();
            EntityConfigurationQuery query;
            QueryCompletedEventArgs result;
            List<DoorSide> doorSides = new List<DoorSide>();

            query = Main.engine.ReportManager.CreateReportQuery(ReportType.EntityConfiguration) as EntityConfigurationQuery;
            query.EntityTypeFilter.Add(EntityType.AccessPoint);
            query.NameSearchMode = StringSearchMode.StartsWith;
            result = query.Query();
            SystemConfiguration systemConfiguration = Main.engine.GetEntity(SdkGuids.SystemConfiguration) as SystemConfiguration;
            var service = systemConfiguration.CustomFieldService;
            if (result.Success)
            {
                try
                {
                    foreach (DataRow dr in result.Data.Rows)
                    {
                        DoorSide lado = Main.engine.GetEntity((Guid)dr[0]) as DoorSide;
                        ObjetosSC ld = new ObjetosSC();
                        if (lado != null)
                        {
                            ld.ObGuid = lado.Guid.ToString();
                            ld.Nome = lado.Name.ToString();
                            ld.BelongTo = lado.Door.ToString();
                            string nomecamera = "";
                            if (lado.Cameras.Count != 0)
                            {
                                foreach (var camera in lado.Cameras)
                                {
                                    var NomeCamera = Main.engine.GetEntity(camera).Name.ToString();
                                    if (nomecamera == "")
                                    {
                                        nomecamera = NomeCamera;
                                    }
                                    else
                                    {
                                        nomecamera = nomecamera + ", " + NomeCamera;
                                    }
                                }
                            }
                            ld.Lado1Camera = nomecamera;
                        }
                        if (ld.ObGuid != null)
                        {
                            listaLado.Add(ld);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro carregar Lado.: " + ex.Message);
                }

            }
            return doorSides;

        }
        /// <summary>
        /// Retorna todos os fluxos e propriedades
        /// </summary>
        /// <returns></returns>
        public List<VideoStream> FLUXOS()
        {
            listaFluxos.Clear();
            EntityConfigurationQuery query;
            QueryCompletedEventArgs result;
            List<VideoStream> videoStreams = new List<VideoStream>();

            query = Main.engine.ReportManager.CreateReportQuery(ReportType.EntityConfiguration) as EntityConfigurationQuery;
            query.EntityTypeFilter.Add(EntityType.Stream);
            query.NameSearchMode = StringSearchMode.StartsWith;
            result = query.Query();
            SystemConfiguration systemConfiguration = Main.engine.GetEntity(SdkGuids.SystemConfiguration) as SystemConfiguration;
            var service = systemConfiguration.CustomFieldService;
            if (result.Success)
            {
                try
                {
                    foreach (DataRow dr in result.Data.Rows)
                    {
                        VideoStream fluxo = Main.engine.GetEntity((Guid)dr[0]) as VideoStream;
                        ObjetosSC fx = new ObjetosSC();
                        if (fluxo != null && fluxo.RunningState == State.Running)
                        {
                            fx.ObGuid = fluxo.Guid.ToString();
                            fx.Nome = fluxo.Name.ToString();
                            fx.BelongTo = fluxo.Camera.ToString();
                            fx.Fluxo1TipoConexao = fluxo.PreferredConnectionType.ToString();
                            fx.Fluxo1EndMulticast = fluxo.MulticastAddress.ToString();
                            fx.Fluxo1TaxaBits = fluxo.VideoCompressions.FirstOrDefault().BitRate.ToString();
                            fx.Fluxo1VelocidadeQuadro = fluxo.VideoCompressions.FirstOrDefault().FrameRate.ToString();
                            string resx = fluxo.VideoCompressions.FirstOrDefault().ResolutionX.ToString();
                            string resy = fluxo.VideoCompressions.FirstOrDefault().ResolutionY.ToString();
                            fx.Resolucao = resx + " x " + resy;
                        }
                        if (fx.ObGuid != null)
                        {
                            listaFluxos.Add(fx);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro carregar Fluxo.: " + ex.Message);
                }

            }
            return videoStreams;
        }


        public List<Zone> iModZones()
        {
            listaObj.Clear();
            EntityConfigurationQuery query;
            QueryCompletedEventArgs result;
            List<Zone> zones = new List<Zone>();

            query = Main.engine.ReportManager.CreateReportQuery(ReportType.EntityConfiguration) as EntityConfigurationQuery;
            query.EntityTypeFilter.Add(EntityType.Zone);
            query.NameSearchMode = StringSearchMode.StartsWith;
            result = query.Query();
            SystemConfiguration systemConfiguration = Main.engine.GetEntity(SdkGuids.SystemConfiguration) as SystemConfiguration;
            var service = systemConfiguration.CustomFieldService;
            if (result.Success)
            {
                try
                {
                    foreach (DataRow dr in result.Data.Rows)
                    {
                        Zone zone = Main.engine.GetEntity((Guid)dr[0]) as Zone;
                        ObjetosSC ac = new ObjetosSC();
                        if (zone != null)
                        {
                            if (zone.Devices.FirstOrDefault().ToString().Contains("fafa") && zone.RunningState == State.Running)
                            {
                                ac.ObGuid = zone.Guid.ToString();
                                ac.Nome = zone.Name.ToString();
                                List<string> EntradasDispositivos = new List<string>();
                                foreach (var guid in zone.Devices)
                                {
                                    Device device = (Device)Main.engine.GetEntity(guid);
                                    var input = device.Name.ToString();
                                    var Unidade = Main.engine.GetEntity(device.Unit).Name.ToString();
                                    EntradasDispositivos.Add(input + " // " + Unidade);
                                }
                                ac.EntradasDispositivos = EntradasDispositivos;
                                List<string> Cameras = new List<string>();
                                foreach (var camera in zone.Cameras)
                                {
                                    var input = Main.engine.GetEntity(camera).Name.ToString();
                                    Cameras.Add(input);
                                }
                                ac.Cameras = Cameras;
                                List<string> Particoes = new List<string>();
                                foreach (var particao in zone.GetPartitions())
                                {
                                    var input = Main.engine.GetEntity(particao).Name.ToString();
                                    Particoes.Add(input);
                                }
                                ac.Particoes = Particoes;
                            }

                        }
                        if (ac.ObGuid != null)
                        {
                            listaObj.Add(ac);
                        }
                    }
                    CreateXML(listaObj, @"C:\Logs\Zonas.xml");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro carregar Zona iMod.: " + ex.Message);
                }

            }
            return zones;

        }
        //public List<Object> Licenses()
        //{
        //    listaObj.Clear();
        //    List<object> licenses = new List<object>();
        //    Guid guid = new Guid("80a25b7b-ae69-44d6-9542-0efde14ce6cb");
        //    var license = Main.engine.LicenseManager.GetDynamicLicenseItemsInfo(guid);
        //    var ttt = Main.engine.GetEntity(guid);
        //    return licenses;

        //}

        /// <summary>
        /// Cria um XML da entidade
        /// </summary>
        /// <param Entidade="file"></param>
        /// <param CaminhoXML="path"></param>
        public void CreateXML(Object file, String path)
        {
            XmlSerializer serializer = new XmlSerializer(file.GetType());
            TextWriter writer = new StreamWriter(path);
            serializer.Serialize(writer, file);
            writer.Close();
        }

    }
}