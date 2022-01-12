using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Workflow;

namespace Inogic.Click2Undo.Workflows.Helper
{
    public class WorkflowConfig
    {
        private ITracingService _tracing;

        private IWorkflowContext _workflowContext;

        private IOrganizationServiceFactory _serviceFactory;

        private IOrganizationService _service;

        private OrganizationServiceContext _context;

        private CodeActivityContext _codeActivityContext;

        public ITracingService Tracing
        {
            get
            {
                try
                {
                    if (_tracing == null)
                    {
                        _tracing = _codeActivityContext.GetExtension<ITracingService>();
                    }
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

        public IWorkflowContext WorkflowContext
        {
            get
            {
                try
                {
                    if (_workflowContext == null)
                    {
                        _workflowContext = _codeActivityContext.GetExtension<IWorkflowContext>();
                    }
                }
                catch (InvalidPluginExecutionException ex)
                {
                    throw new InvalidPluginExecutionException(ex.Message);
                }
                catch (Exception ex2)
                {
                    throw new InvalidPluginExecutionException(ex2.Message);
                }

                return _workflowContext;
            }
        }

        public IOrganizationServiceFactory ServiceFactory
        {
            get
            {
                try
                {
                    if (_serviceFactory == null)
                    {
                        _serviceFactory = _codeActivityContext.GetExtension<IOrganizationServiceFactory>();
                    }
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
                    if (_service == null)
                    {
                        _service = ServiceFactory.CreateOrganizationService(WorkflowContext.InitiatingUserId);
                    }
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
                    if (_context == null)
                    {
                        _context = new OrganizationServiceContext(Service);
                    }
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

        public WorkflowConfig(CodeActivityContext pCodeActivityContext)
        {
            _codeActivityContext = pCodeActivityContext;
        }
    }
}