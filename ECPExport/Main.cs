using Genetec.Sdk;
using Genetec.Sdk.Entities;
using Genetec.Sdk.Workspace;
using Genetec.Sdk.Workspace.Modules;
using Genetec.Sdk.Workspace.Tasks;
using Genetec.Sdk.Workspace.Components.MapObjectViewBuilder;
using Genetec.Sdk.Workspace.Monitors;
using Genetec.Sdk.Workspace.Services;
using Genetec.Sdk.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using System.IO;
using System.Reflection;
using Module = Genetec.Sdk.Workspace.Modules.Module;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.Xml;
using ECPExport.Pages;



namespace StartProject
{
    public class Main : Module
    {
        #region Constants


        public static readonly Guid appID = new Guid("{23454CD0-7E9C-FAFA-B8A6-24FD71D6DD60}");
        public static IEngine engine;
        public Engine sdk = new Engine();
        private static readonly BitmapImage IconeProjeto;

        private readonly List<Task> m_tasks = new List<Task>();
        #endregion

        #region Constructors
        static Main()
        {
            try
            {
                IconeProjeto = new BitmapImage(new Uri(@"pack://application:,,/ECPExport;Component/Resources/Category.png", UriKind.RelativeOrAbsolute));
            }
            catch (Exception ex)
            {               
            }
        }
        #endregion

        #region Public Methods
        public override void Load()
        {

            engine = Workspace.Sdk;
            //engine.ClientCertificate = "y+BiIiYO5VxBax6/HNi7/ZcXWuvlnEemfaMhoQS1RMkfOGvEBWdUV7zQN272yHVG";
            SubscribeToSdkEvents(engine);
            SubscribeToWorkspaceEvents();
            RegisterTaskExtensions();
        }

        public override void Unload()
        {
            if (Workspace != null)
            {
                UnregisterTaskExtensions();
                UnsubscribeFromWorkspaceEvents();
                UnsubscribeFromSdkEvents(Workspace.Sdk);
            }
        }
        #endregion

        #region Private Methods - Modulo
        private void SubscribeToSdkEvents(IEngine engine)
        {
            if (sdk != null)
            {
                //engine.ClientCertificate = "y+BiIiYO5VxBax6/HNi7/ZcXWuvlnEemfaMhoQS1RMkfOGvEBWdUV7zQN272yHVG";
                sdk.LoggedOn += OnLoggedOn;
                engine.LogonFailed += OnLogonFailed;

            }
        }
       
        private void OnLoggedOn(object sender, LoggedOnEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnLogonFailed(object sender, LogonFailedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void SubscribeToWorkspaceEvents()
        {
            if (Workspace != null)
            {
                //Workspace.Initialized += OnWorkspaceInitialized;
            }
        }

        private void UnregisterTaskExtensions()
        {
            // Register them to the workspace
            foreach (Genetec.Sdk.Workspace.Tasks.Task task in m_tasks)
            {
                Workspace.Tasks.Unregister(task);
            }

            m_tasks.Clear();
        }

        private void UnsubscribeFromSdkEvents(IEngine engine)
        {
            if (engine != null)
            {
                //engine.LoggedOn -= OnLoggedOn;
            }
        }

        private void UnsubscribeFromWorkspaceEvents()
        {
            if (Workspace != null)
            {
                //Workspace.Initialized -= OnWorkspaceInitialized;
            }
        }

        private void RegisterTaskExtensions()
        {
            TaskGroup taskGroup = new TaskGroup(appID, Guid.Empty, "ECPExport", IconeProjeto, 1);
            taskGroup.Initialize(Workspace);
            m_tasks.Add(taskGroup);

            Task task = new CreatePageTask<Home>();
            task.Initialize(Workspace);
            m_tasks.Add(task);

            foreach (Task pageExtension in m_tasks)
            {
                Workspace.Tasks.Register(pageExtension);
            }
        }
    }
    #endregion
}

