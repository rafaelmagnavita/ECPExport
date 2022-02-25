using Genetec.Sdk.Workspace.Pages;
using StartProject;
using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

// ==========================================================================
// Copyright (C) 2016 by Genetec, Inc.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// ==========================================================================
namespace ECPExport.Pages
{
    #region Classes

    [Page(typeof(PagePersistenceSampleDescriptor))]
    public class Home : Page
    {
        #region Constants

        private readonly UserControl1 m_view = new UserControl1();

        #endregion

        #region Nested Classes and Structures

        /// <summary>
        /// Class that contains the data that needs to be persisted for the page.
        /// </summary>
        [Serializable]
        public class PageData
        {
            #region Fields

            public string Message;

            #endregion

            #region Public Methods

            /// <summary>
            /// Converts the byte array to the PageData.
            /// </summary>
            /// <param name="serializedData"></param>
            /// <returns></returns>
            public static PageData Deserialize(byte[] serializedData)
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        var serializer = new XmlSerializer(typeof(PageData));
                        ms.Write(serializedData, 0, serializedData.Length);
                        ms.Seek(0, SeekOrigin.Begin);
                        PageData obj = (PageData)serializer.Deserialize(ms);

                        return obj;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                return null;
            }

            /// <summary>
            /// Convert the data to a byte array.
            /// </summary>
            public byte[] Serialize()
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        var serializer = new XmlSerializer(typeof(PageData));
                        serializer.Serialize(ms, this);

                        return ms.ToArray();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                return null;
            }

            #endregion
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets if the page can be saved as a Public/Private task.
        /// </summary>
        /// <returns>True if the page can be saved; Otherwise, false.</returns>
        //protected override bool CanSave()
        //{
        //    //if (m_view == null)
        //    //    return false;

        //    //return m_view.CanSaveAs;
        //}

        /// <summary>
        /// Deserializes the data contained by the specified byte array.
        /// </summary>
        /// <param name="data">A byte array that contains the data.</param>
        protected override void Deserialize(byte[] data)
        {
            if (data == null)
                return;

            PageData pageData = PageData.Deserialize(data);
            if (pageData != null)
            {
                //m_view.Message = pageData.Message;
            }
        }

        /// <summary>
        /// Initialize the page.
        /// </summary>
        /// <remarks>At this step, the <see cref="Genetec.Sdk.Workspace.Workspace"/> is available.</remarks>
        protected override void Initialize()
        {
            if (Main.engine.IsConnected)
            {
                //Main.engine.LogOff();
            }
            View = m_view;
        }

        /// <summary>
        /// Serializes the data to a byte array.
        /// </summary>
        /// <returns>A byte array that contains the data.</returns>
        protected override byte[] Serialize()
        {
            if (m_view != null)
            {
                PageData pageData = new PageData();
                //pageData.Message = m_view.Message;

                return pageData.Serialize();
            }

            return null;
        }

        #endregion
    }

    /// <summary>
    /// Describes the attributes of PagePersistenceSample.
    /// </summary>
    public class PagePersistenceSampleDescriptor : PageDescriptor
    {
        #region Constants

        /// <summary>
        /// The privilege that needs to be allowed in order to execute the task, as specified in ModuleSample.privileges.xml.
        /// </summary>
        public const string Privilege = "{D1EE90DF-88CC-4ABF-A92E-1B0F57F8CF80}";

        private readonly ImageSource m_icon;

        private readonly ImageSource m_thumbnail;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the page's task group to which it is associated.
        /// </summary>
        public override Guid CategoryId
        {
            get { return Main.appID; }
        }

        public override string Description
        {
            get { return "Pagina de Exportações Gerais - ECP"; }
        }
        /// <summary>
        /// Gets the icon representing the page.
        /// </summary>
        /// <remarks>Optimal resolution is 16x16.</remarks>
        public override ImageSource Icon
        {
            get { return m_icon; }
        }
        /// <summary>
        /// Gets the page's default name.
        /// </summary>
        public override string Name
        {
            get
            {
                // This name is the one that will appear in the Home menu. It will also be the default name on creation.
                return "Exportações de Projetos - ECP";
            }
        }
        /// <summary>
        /// Gets the thumbnail representing the page.
        /// </summary>
        /// <remarks>Optimal resolution is 256x256.</remarks>
        public override ImageSource Thumbnail
        {
            get { return m_thumbnail; }
        }
        /// <summary>
        /// Gets the page's unique ID.
        /// </summary>
        public override Guid Type
        {
            get
            {
                // The Guid has to be unique among all your tasks. It should never be changed afterwards.
                return new Guid("{F8615F5D-5BB7-47B8-ADE1-9A2996B331BB}");
            }
        }

        #endregion

        #region Constructors

        public PagePersistenceSampleDescriptor()
        {

            try
            {

                BitmapImage img;
                m_icon = new BitmapImage(new Uri(@"pack://application:,,/ECPExport;Component/Resources/Category.png", UriKind.RelativeOrAbsolute));
                m_thumbnail = new BitmapImage(new Uri(@"pack://application:,,/ECPExport;Component/Resources/Category.png", UriKind.RelativeOrAbsolute));
                img = new BitmapImage(new Uri(@"pack://application:,,/ECPExport;Component/Resources/Category.png", UriKind.RelativeOrAbsolute));
            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.Message);

            }
        }

        #endregion
        #region Public Methods

        /// <summary>
        /// Gets if the current user has the privilege to see the page.
        /// </summary>
        /// <returns>True if allowed; Otherwise, false.</returns>
        public override bool HasPrivilege()
        {
            //m_sdk.ClientCertificate = "y+BiIiYO5VxBax6/HNi7/ZcXWuvlnEemfaMhoQS1RMkfOGvEBWdUV7zQN272yHVG";
            if (m_sdk.IsConnected)
            {
                return m_sdk.SecurityManager.IsPrivilegeGranted(new Guid(Privilege));
            }

            return false;
        }

        #endregion
    }

    #endregion
}

