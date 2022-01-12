using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace Inogic.Click2Undo.Plugins.Helper
{
    public class PluginConfig
    {
        private ITracingService _tracing;

        private IPluginExecutionContext _pluginContext;

        private IOrganizationServiceFactory _serviceFactory;

        private IOrganizationService _service;

        private OrganizationServiceContext _context;

        private readonly IServiceProvider _serviceProvider;

        public ITracingService Tracing
        {
            get
            {
                try
                {
                    _tracing ??= (ITracingService)GetServices(typeof(ITracingService));
                }
                catch (InvalidPluginExecutionException ex)
                {
                    throw new InvalidPluginExecutionException(ex.Message);
                }
                catch (Exception ex2)
                {
                    throw new InvalidPluginExecutionException(ex2.Message);
                }

                return _tracing;
            }
        }

        public IPluginExecutionContext PluginContext
        {
            get
            {
                try
                {
                    _pluginContext ??= (IPluginExecutionContext)GetServices(typeof(IPluginExecutionContext));
                }
                catch (InvalidPluginExecutionException ex)
                {
                    throw new InvalidPluginExecutionException(ex.Message);
                }
                catch (Exception ex2)
                {
                    throw new InvalidPluginExecutionException(ex2.Message);
                }

                return _pluginContext;
            }
        }

        public IOrganizationServiceFactory ServiceFactory
        {
            get
            {
                try
                {
                    _serviceFactory ??= (IOrganizationServiceFactory)GetServices(typeof(IOrganizationServiceFactory));
                }
                catch (InvalidPluginExecutionException ex)
                {
                    throw new InvalidPluginExecutionException(ex.Message);
                }
                catch (Exception ex2)
                {
                    throw new InvalidPluginExecutionException(ex2.Message);
                }

                return _serviceFactory;
            }
        }

        public IOrganizationService Service
        {
            get
            {
                try
                {
                    _service ??= ServiceFactory.CreateOrganizationService(PluginContext.UserId);
                }
                catch (InvalidPluginExecutionException ex)
                {
                    throw new InvalidPluginExecutionException(ex.Message);
                }
                catch (Exception ex2)
                {
                    throw new InvalidPluginExecutionException(ex2.Message);
                }

                return _service;
            }
        }

        public OrganizationServiceContext Context
        {
            get
            {
                try
                {
                    _context ??= new OrganizationServiceContext(Service);
                }
                catch (InvalidPluginExecutionException ex)
                {
                    throw new InvalidPluginExecutionException(ex.Message);
                }
                catch (Exception ex2)
                {
                    throw new InvalidPluginExecutionException(ex2.Message);
                }

                return _context;
            }
        }

        public PluginConfig(IServiceProvider pServiceProvider)
        {
            _serviceProvider = pServiceProvider;
        }

        private object GetServices(Type pType)
        {
            return _serviceProvider?.GetService(pType);
        }
    }
}