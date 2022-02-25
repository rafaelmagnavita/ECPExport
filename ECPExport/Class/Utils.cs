using Genetec.Sdk;
using Genetec.Sdk.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using ECPExport.Class;
using System.Windows;

namespace ECPExport
{
    public static class Utils
    {
        public static void NonPublic(object objetos, string prop1, string prop2, string prop3, ObjetosSC Valor)
        {
            object strProperty = objetos.GetType().GetProperty(prop1, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).GetValue(objetos);
            PropertyInfo Propriedade2 = strProperty.GetType().GetProperty(prop2);
            try
            {
                if (prop3 == null)
                {
                    Valor.Nome = (string)Propriedade2.GetValue(strProperty);
                    var a = Propriedade2.GetValue(strProperty);
                }
                else
                {
                    PropertyInfo Propriedade3 = Propriedade2.GetValue(strProperty).GetType().GetProperty(prop3);
                    Valor.Nome = Propriedade3.GetValue(Propriedade2.GetValue(strProperty)).ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro carregar NonPublic.: " + ex.Message);
                Valor = null;
            }

        }
        public static void NonPublicList(object objetos, string prop1, string prop2, string prop3, Devices Valor)
        {
            object strProperty = objetos.GetType().GetProperty(prop1, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).GetValue(objetos);
            PropertyInfo Propriedade2 = strProperty.GetType().GetProperty(prop2);
            try
            {
                List<Guid> ab = new List<Guid>();
                if (prop3 == null)
                {
                    Valor.ObGuids = (List<Guid>)Propriedade2.GetValue(strProperty);
                }
                else
                {
                    PropertyInfo Propriedade3 = Propriedade2.GetValue(strProperty).GetType().GetProperty(prop3);
                    Valor.ObGuids = (List<Guid>)Propriedade3.GetValue(Propriedade2.GetValue(strProperty));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro carregar NonPublicList.: " + ex.Message);
                Valor = null;
            }

        }

        public static void XMLRole(Entity role, string property, ObjetosSC valor)
        {
            try
            {
                ObjetosSC obj = new ObjetosSC();
                Utils.NonPublic(role, "InternalEntity", "XmlInfo", null, obj);
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(obj.Nome);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("XmlRole/XmlInfo");
                XmlDocument xmlDocument2 = new XmlDocument();
                xmlDocument2.LoadXml(xmlNode.InnerText);
                XmlNode xmlNode2 = xmlDocument2.SelectSingleNode(property);
                var atributo = xmlNode2.InnerText;
                valor.Nome = atributo;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro carregar XMLRole.: " + ex.Message);
                throw;
            }


        }

        public static void XMLEntity(Entity role, string property, ObjetosSC valor)
        {
            try
            {
                ObjetosSC obj = new ObjetosSC();
                object strProperty = role.GetType().GetProperty("InternalEntity", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).GetValue(role);
                var Propriedade2 = strProperty.GetType().GetField("m_unitRow", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                var propww = Propriedade2.GetValue(strProperty);
                var abc = propww.GetType().GetProperty("Data").GetValue(Propriedade2.GetValue(strProperty));
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(abc.ToString());
                XmlNode xmlNode = xmlDocument.SelectSingleNode("Unit/" + property);
                if (xmlNode != null)
                {
                    valor.Nome = xmlNode.InnerText;
                }
                else
                    valor.Nome = "";

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro carregar XMLEntity.: " + ex.Message);
                throw;
            }


        }

    }
}
