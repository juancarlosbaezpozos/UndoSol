using System;
using System.Activities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.Text;
using Inogic.Click2Undo.Workflows.Entities;
using Inogic.Click2Undo.Workflows.Helper;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;

namespace Inogic.Click2Undo.Workflows
{
    public class GetAuditDetails : CodeActivity
    {
        [RequiredArgument] [Input("RecordId")] public InArgument<string> INRECORDID { get; set; }

        [RequiredArgument]
        [Input("EntityName")]
        public InArgument<string> INENTITYNAME { get; set; }

        [RequiredArgument]
        [Input("StartDate")]
        public InArgument<string> INSTARTDATE { get; set; }

        [RequiredArgument] [Input("EndDate")] public InArgument<string> INENDDATE { get; set; }

        [RequiredArgument]
        [Input("PagingCookie")]
        public InArgument<string> PagingCookie { get; set; }

        [Input("SelectedAttributes")] public InArgument<string> SelectedAttributes { get; set; }

        [Input("DesdendingOrder")] public InArgument<bool> DescendingOrder { get; set; }

        [Output("AuditDetailsJson")] public OutArgument<string> OUTAUDITDETAILSJSON { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            WorkflowConfig wfConfig = null;
            var dictionary = new Dictionary<string, object>();
            var arg = "Execute: ";
            var recordId = string.Empty;
            var entityName = string.Empty;
            var pagingCookie = string.Empty;
            var traceLog = new StringBuilder();

            try
            {
                wfConfig = new WorkflowConfig(context);
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                if (INRECORDID == null || INRECORDID.Get<string>(context) == null || INENTITYNAME == null ||
                    INENTITYNAME.Get<string>(context) == null)
                {
                    Utility.SetTrace("Input parameters are not valid.", ref traceLog, wfConfig);
                    return;
                }

                ProcessGetAuditDetails(ref recordId, ref entityName, ref pagingCookie, context, ref traceLog, wfConfig);
                Utility.SetTrace("Execution completed.", ref traceLog, wfConfig);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                var arg2 = ((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
                throw new InvalidPluginExecutionException(
                    $"AssemblyErrorMessage: {arg2} AssemblyTracing: {traceLog.ToString()}");
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    wfConfig);
                var arg3 = ((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
                throw new InvalidPluginExecutionException(
                    $"AssemblyErrorMessage: {arg3} AssemblyTracing: {traceLog.ToString()}");
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                var arg4 = ((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
                throw new InvalidPluginExecutionException(
                    $"AssemblyErrorMessage: {arg4} AssemblyTracing: {traceLog.ToString()}");
            }
        }

        private void ProcessGetAuditDetails(ref string recordId, ref string entityName, ref string pagingCookie,
            CodeActivityContext context, ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "ProcessGetAuditDetails: ";
            var empty = string.Empty;
            var startDate = string.Empty;
            var endDate = string.Empty;
            var num = 0;
            var num2 = 0;
            var descendingOrder = true;
            var text = string.Empty;
            var selectedAttributeMasks = string.Empty;

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                GetInputParameters(ref recordId, ref entityName, ref startDate, ref endDate, ref pagingCookie,
                    ref descendingOrder, ref selectedAttributeMasks, context, ref traceLog, wfConfig);
                num2 = Utility.GetRecordsPerPage(ref traceLog, wfConfig);
                var entityCollection = Utility.RetrieveAuditDetails(num2, recordId, startDate, endDate, pagingCookie,
                    selectedAttributeMasks, descendingOrder, getCount: false, getFields: false, "2", string.Empty, "2",
                    isUndoActionTriggered: false, ref traceLog, wfConfig);
                if (entityCollection == null || entityCollection.Entities.Count <= 0)
                {
                    Utility.SetTrace("Audit entity collection not found.", ref traceLog, wfConfig);
                    return;
                }

                Utility.SetTrace(
                    $"Total audits count for record with id: {recordId} is : {entityCollection.Entities.Count}",
                    ref traceLog, wfConfig);
                var entityMetadata = Utility.RetrieveEntityMetadata(entityName, ref traceLog, wfConfig);
                if (entityMetadata == null)
                {
                    Utility.SetTrace("Entity metadata not found.", ref traceLog, wfConfig);
                    return;
                }

                var recID = recordId;
                var source = from o in wfConfig.Context.CreateQuery("audit")
                             where o.GetAttributeValue<OptionSetValue>("operation").Value.Equals("2") &&
                                   (o.GetAttributeValue<OptionSetValue>("action").Value.Equals("2") ||
                                    o.GetAttributeValue<OptionSetValue>("action").Value.Equals("13")) &&
                                   ((EntityReference)o["objectid"]).Id.Equals(new Guid(recID)) &&
                                   o.GetAttributeValue<DateTime>("createdon") >= new DateTime(
                                       Convert.ToInt32(startDate.Split('-')[2]), Convert.ToInt32(startDate.Split('-')[0]),
                                       Convert.ToInt32(startDate.Split('-')[1]), 0, 0, 0) &&
                                   o.GetAttributeValue<DateTime>("createdon") <= new DateTime(
                                       Convert.ToInt32(endDate.Split('-')[2]), Convert.ToInt32(endDate.Split('-')[0]),
                                       Convert.ToInt32(endDate.Split('-')[1]), 23, 59, 59)
                             orderby o["createdon"] descending
                             select new
                             {
                                 attributemask =
                                     ((o.Attributes.Contains("attributemask") && o.Attributes["attributemask"] != null)
                                         ? o.Attributes["attributemask"]
                                         : ((object)"not found")),
                                 auditid = o.Attributes["auditid"]
                             };
                var list = (from f in source.ToList()
                            group f by f.attributemask
                    into queryGrp
                            select new
                            {
                                attributemask = queryGrp.Key,
                                auditcount = queryGrp.Count()
                            })?.ToList();
                if (list == null || list.Count <= 0)
                {
                    Utility.SetTrace("countColl not found.", ref traceLog, wfConfig);
                    return;
                }

                foreach (var item in list)
                {
                    var text2 = item.attributemask.ToString();
                    var flag = false;
                    Utility.SetTrace($"currentAttributeMasks:{text2}", ref traceLog, wfConfig);
                    if (text2 != null && text2.Trim() != string.Empty)
                    {
                        var num3 = 0;
                        while (!flag && num3 < text2.Split(',').Length)
                        {
                            if (text2.Split(',')[num3] != string.Empty)
                            {
                                flag = isValidAttribute(entityMetadata, text2.Split(',')[num3], ref traceLog, wfConfig);
                            }

                            num3++;
                        }
                    }

                    if (flag)
                    {
                        Utility.SetTrace($"currentAttributeMasks:{text2} has valid attribute", ref traceLog, wfConfig);
                    }

                    num += (flag ? Convert.ToInt32(item.auditcount) : 0);
                    Utility.SetTrace($"totalRecords:{num}", ref traceLog, wfConfig);
                }

                var source2 = from o in wfConfig.Context.CreateQuery("audit")
                              where o.GetAttributeValue<OptionSetValue>("operation").Value.Equals("2") &&
                                    (o.GetAttributeValue<OptionSetValue>("action").Value.Equals("2") ||
                                     o.GetAttributeValue<OptionSetValue>("action").Value.Equals("13")) &&
                                    ((EntityReference)o["objectid"]).Id.Equals(new Guid(recID)) &&
                                    o.GetAttributeValue<DateTime>("createdon") >= new DateTime(
                                        Convert.ToInt32(startDate.Split('-')[2]), Convert.ToInt32(startDate.Split('-')[0]),
                                        Convert.ToInt32(startDate.Split('-')[1]), 0, 0, 0) &&
                                    o.GetAttributeValue<DateTime>("createdon") <= new DateTime(
                                        Convert.ToInt32(endDate.Split('-')[2]), Convert.ToInt32(endDate.Split('-')[0]),
                                        Convert.ToInt32(endDate.Split('-')[1]), 23, 59, 59)
                              orderby o["createdon"] descending
                              select new
                              {
                                  attributemask =
                                      ((o.Attributes.Contains("attributemask") && o.Attributes["attributemask"] != null)
                                          ? o.Attributes["attributemask"]
                                          : ((object)"not found")),
                                  auditid = o.Attributes["auditid"]
                              };
                var list2 = (from f in source2.ToList()
                             group f by f.attributemask
                    into queryGrp
                             select new
                             {
                                 attributemask = queryGrp.Key,
                                 auditcount = queryGrp.Count()
                             })?.ToList();
                if (list2 == null || list2.Count <= 0)
                {
                    Utility.SetTrace("fieldColl not found.", ref traceLog, wfConfig);
                    return;
                }

                foreach (var item2 in list2)
                {
                    text = text + "," + item2.attributemask.ToString();
                }

                var serializeAuditDetails = new SerializeAuditDetails();
                var list3 = new List<FieldInfo>();
                if (text.Split(',').Length != 0)
                {
                    Utility.SetTrace($"attribute masks: {text}", ref traceLog, wfConfig);
                    var array = text.Split(',').Distinct().ToArray();
                    if (array != null && array.Length != 0)
                    {
                        for (var i = 0; i < array.Length; i++)
                        {
                            Utility.SetTrace($"attribute mask: {i}", ref traceLog, wfConfig);
                            Utility.SetTrace($"attribute mask value: {array[i]}", ref traceLog, wfConfig);
                            if (!string.IsNullOrEmpty(array[i]))
                            {
                                var fieldInfo = new FieldInfo();
                                var logicalName = string.Empty;
                                fieldInfo.FieldDisplayName = GetAttributeInfo(entityMetadata, ref logicalName, array[i],
                                    ref traceLog, wfConfig);
                                fieldInfo.FieldLogicalName = logicalName;
                                Utility.SetTrace($"fieldLogicalName: {logicalName}", ref traceLog, wfConfig);
                                Utility.SetTrace($"fieldInfo.FieldDisplayName: {fieldInfo.FieldDisplayName}",
                                    ref traceLog, wfConfig);
                                fieldInfo.AttributeMask = array[i];
                                if (!string.IsNullOrEmpty(fieldInfo.FieldLogicalName))
                                {
                                    list3.Add(fieldInfo);
                                }
                            }
                        }
                    }
                }

                serializeAuditDetails.AuditRecords = GetAuditRecordsCollection(entityCollection, entityMetadata,
                    selectedAttributeMasks, ref traceLog, wfConfig);
                serializeAuditDetails.TotalAuditRecordscount = num.ToString();
                serializeAuditDetails.PagingCookie = entityCollection.PagingCookie;
                serializeAuditDetails.fieldInfo = ((list3 != null && list3.Count > 0) ? list3.ToArray() : null);
                serializeAuditDetails.RecordsPerPage = num2.ToString();
                empty = GenerateXml(serializeAuditDetails, typeof(SerializeAuditDetails), ref traceLog, wfConfig);
                if (string.IsNullOrEmpty(empty))
                {
                    Utility.SetTrace("No JSON string found.", ref traceLog, wfConfig);
                }

                Utility.SetTrace($"JSON length: {empty.Length}", ref traceLog, wfConfig);
                OUTAUDITDETAILSJSON.Set(context, empty);
                Utility.SetTrace("Output parameter set successfully.", ref traceLog, wfConfig);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }

        private int GetRecordsPerPage(ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "GetRecordsPerPage: ";
            var result = 0;

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                var query = string.Concat(
                    "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>\r\n                   <entity name='usersettings'>\r\n                   <all-attributes\r\n                   <filter type='and'>\r\n                   <condition attribute='systemuserid' operator='eq' value='",
                    wfConfig.WorkflowContext.UserId,
                    "' />\r\n                   </filter>\r\n                   </entity>\r\n                   </fetch>");
                var entityCollection = wfConfig.Service.RetrieveMultiple(new FetchExpression(query));
                if (entityCollection != null && entityCollection.Entities.Count > 0)
                {
                    result = (entityCollection.Entities[0].Attributes.Contains("paginglimit")
                        ? entityCollection.Entities[0].GetAttributeValue<int>("paginglimit")
                        : 0);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return result;
        }

        private void GetInputParameters(ref string recordId, ref string entityName, ref string startDate,
            ref string endDate, ref string pagingCookie, ref bool descendingOrder, ref string selectedAttributeMasks,
            CodeActivityContext context, ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "GetInputParameters: ";

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                recordId = ((INRECORDID != null && INRECORDID.Get<string>(context) != null)
                    ? INRECORDID.Get<string>(context)
                    : string.Empty);
                Utility.SetTrace($"Record ID: {recordId}", ref traceLog, wfConfig);
                entityName = ((INENTITYNAME != null && INENTITYNAME.Get<string>(context) != null)
                    ? INENTITYNAME.Get<string>(context)
                    : string.Empty);
                Utility.SetTrace($"Entity Name: {entityName}", ref traceLog, wfConfig);
                startDate = ((INSTARTDATE != null && INSTARTDATE.Get<string>(context) != null)
                    ? INSTARTDATE.Get<string>(context)
                    : string.Empty);
                Utility.SetTrace($"Start Date: {startDate}", ref traceLog, wfConfig);
                endDate = ((INENDDATE != null && INENDDATE.Get<string>(context) != null)
                    ? INENDDATE.Get<string>(context)
                    : string.Empty);
                Utility.SetTrace($"End Date: {endDate}", ref traceLog, wfConfig);
                pagingCookie = ((PagingCookie != null && PagingCookie.Get<string>(context) != null)
                    ? PagingCookie.Get<string>(context)
                    : string.Empty);
                Utility.SetTrace($"pagingCookie: {pagingCookie}", ref traceLog, wfConfig);
                int num;
                if (DescendingOrder != null)
                {
                    DescendingOrder.Get<bool>(context);
                    num = (DescendingOrder.Get<bool>(context) ? 1 : 0);
                }
                else
                {
                    num = (descendingOrder ? 1 : 0);
                }

                descendingOrder = (byte)num != 0;
                Utility.SetTrace($"descendingOrder: {descendingOrder.ToString()}", ref traceLog, wfConfig);
                selectedAttributeMasks =
                    ((SelectedAttributes != null && SelectedAttributes.Get<string>(context) != null)
                        ? SelectedAttributes.Get<string>(context)
                        : string.Empty);
                Utility.SetTrace($"selectedAttributeMasks: {selectedAttributeMasks}", ref traceLog, wfConfig);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }

        private AuditRecords[] GetAuditRecordsCollection(EntityCollection auditEntityCollection,
            EntityMetadata entityMetadata, string selectedAttributeMasks, ref StringBuilder traceLog,
            WorkflowConfig wfConfig)
        {
            var arg = "GetAuditRecordsCollection: ";
            AuditRecords[] array;

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                array = new AuditRecords[auditEntityCollection.Entities.Count];
                for (var i = 0; i < auditEntityCollection.Entities.Count; i++)
                {
                    Utility.SetTrace($"For audit collection at {i}", ref traceLog, wfConfig);
                    var entity = auditEntityCollection.Entities[i];
                    var auditRecords = new AuditRecords();
                    var auditDetail = ((entity.Id != Guid.Empty)
                        ? Utility.GetAuditDetails(entity.Id, ref traceLog, wfConfig)
                        : null);
                    if (auditDetail == null || auditDetail.AuditRecord == null ||
                        auditDetail.GetType() != typeof(AttributeAuditDetail))
                    {
                        Utility.SetTrace("Audit details not found.", ref traceLog, wfConfig);
                        continue;
                    }

                    Utility.SetTrace("Audit details found.", ref traceLog, wfConfig);
                    var attributeAuditDetail = auditDetail as AttributeAuditDetail;
                    if (attributeAuditDetail == null ||
                        ((attributeAuditDetail.NewValue == null || attributeAuditDetail.NewValue.Attributes == null ||
                          attributeAuditDetail.NewValue.Attributes.Count <= 0) &&
                         (attributeAuditDetail.OldValue == null ||
                          attributeAuditDetail.OldValue.Attributes.Count <= 0)))
                    {
                        Utility.SetTrace("Attribute not found.", ref traceLog, wfConfig);
                        continue;
                    }

                    var array2 = GetAuditRecordAttributesCollection(attributeAuditDetail, entityMetadata, auditDetail,
                        selectedAttributeMasks, ref traceLog, wfConfig);
                    if (array2 == null)
                    {
                        Utility.SetTrace("Audit details attribute collection not found.", ref traceLog, wfConfig);
                        continue;
                    }

                    auditRecords.AuditDetailAttributes = array2;
                    array[i] = auditRecords;
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return array;
        }

        private AuditDetailAttributes[] GetAuditRecordAttributesCollection(AttributeAuditDetail attributeAuditDetail,
            EntityMetadata entityMetadata, AuditDetail auditDetail, string selectedAttributeMasks,
            ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "GetAuditRecordAttributesCollection: ";
            var num = 0;
            string[] attributeMaskColl = null;
            var source = new string[8]
            {
                "salesstage", "stepname", "totallineitemamount", "totalamountlessfreight", "totalamount",
                "totaldiscountamount", "totaltax", "totallineitemdiscountamount"
            };
            AuditDetailAttributes[] array;

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                array = new AuditDetailAttributes[((attributeAuditDetail.NewValue != null)
                    ? attributeAuditDetail.NewValue.Attributes.Count
                    : 0) + ((attributeAuditDetail.OldValue != null)
                    ? attributeAuditDetail.OldValue.Attributes.Count
                    : 0)];
                if (selectedAttributeMasks != null && selectedAttributeMasks.Trim() != string.Empty &&
                    selectedAttributeMasks.Split(',').Length != 0)
                {
                    Utility.SetTrace($"attribute masks: {selectedAttributeMasks}", ref traceLog, wfConfig);
                    attributeMaskColl = selectedAttributeMasks.Split(',').Distinct().ToArray();
                }

                Utility.SetTrace("loop through the new attribute values.", ref traceLog, wfConfig);
                for (var i = 0; i < attributeAuditDetail.NewValue.Attributes.Count; i++)
                {
                    Utility.SetTrace($"For attribute: {attributeAuditDetail.NewValue.Attributes.ToArray()[i].Key}",
                        ref traceLog, wfConfig);
                    if (attributeAuditDetail.NewValue.Attributes.ToArray()[i].Key == "statecode" ||
                        attributeAuditDetail.NewValue.Attributes.ToArray()[i].Key == "statuscode" ||
                        !Utility.IsValidatForUpdateAndAttributeNumber(entityMetadata,
                            attributeAuditDetail.NewValue.Attributes.ToArray()[i].Key, attributeMaskColl, ref traceLog,
                            wfConfig) || source.Contains(attributeAuditDetail.NewValue.Attributes.ToArray()[i].Key))
                    {
                        Utility.SetTrace("Not valid field.", ref traceLog, wfConfig);
                        continue;
                    }

                    array[i] = GetAuditRecordAttributes(attributeAuditDetail.NewValue.Attributes.ToArray()[i],
                        entityMetadata, auditDetail.AuditRecord, attributeAuditDetail, ref traceLog, wfConfig);
                    num++;
                }

                Utility.SetTrace("loop through the old attribute values.", ref traceLog, wfConfig);
                for (var j = 0; j < attributeAuditDetail.OldValue.Attributes.Count; j++)
                {
                    Utility.SetTrace($"For attribute: {attributeAuditDetail.OldValue.Attributes.ToArray()[j].Key}",
                        ref traceLog, wfConfig);
                    if (attributeAuditDetail.OldValue.Attributes.ToArray()[j].Key == "statecode" ||
                        attributeAuditDetail.OldValue.Attributes.ToArray()[j].Key == "statuscode" ||
                        !Utility.IsValidatForUpdateAndAttributeNumber(entityMetadata,
                            attributeAuditDetail.OldValue.Attributes.ToArray()[j].Key, attributeMaskColl, ref traceLog,
                            wfConfig) || source.Contains(attributeAuditDetail.OldValue.Attributes.ToArray()[j].Key))
                    {
                        Utility.SetTrace("Not valid field.", ref traceLog, wfConfig);
                        continue;
                    }

                    if (attributeAuditDetail.NewValue.Attributes.Keys.Contains(
                            attributeAuditDetail.OldValue.Attributes.ToArray()[j].Key))
                    {
                        Utility.SetTrace("Already proccessed for new attribute values.", ref traceLog, wfConfig);
                        continue;
                    }

                    array[num] = GetAuditRecordAttributes(attributeAuditDetail.OldValue.Attributes.ToArray()[j],
                        entityMetadata, auditDetail.AuditRecord, attributeAuditDetail, ref traceLog, wfConfig);
                    num++;
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return array;
        }

        private AuditDetailAttributes GetAuditRecordAttributes(KeyValuePair<string, object> attribute,
            EntityMetadata entityMetadata, Entity auditRecord, AttributeAuditDetail attributeAuditDetail,
            ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "GetAuditRecordAttributes: ";
            var auditDetailAttributes = new AuditDetailAttributes();

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                auditDetailAttributes.AuditDetailId =
                    ((auditRecord.Id != Guid.Empty) ? auditRecord.Id.ToString() : string.Empty);
                Utility.SetTrace($"auditDetailAttributesContract.AuditDetailId: {auditDetailAttributes.AuditDetailId}",
                    ref traceLog, wfConfig);
                auditDetailAttributes.FormattedChangedDate =
                    ((auditRecord.FormattedValues.Contains("createdon") &&
                      auditRecord.FormattedValues["createdon"] != null)
                        ? auditRecord.FormattedValues["createdon"]
                        : "");
                Utility.SetTrace(
                    $"auditDetailAttributesContract.FormattedChangedDate: {auditDetailAttributes.FormattedChangedDate}",
                    ref traceLog, wfConfig);
                auditDetailAttributes.ChangedDate =
                    ((auditRecord.Attributes.Contains("createdon") && auditRecord.Attributes["createdon"] != null)
                        ? Utility.GetConvertedDate(Convert.ToDateTime(auditRecord.Attributes["createdon"]),
                            ref traceLog, wfConfig)
                        : "");
                Utility.SetTrace($"auditDetailAttributesContract.ChangedDate: {auditDetailAttributes.ChangedDate}",
                    ref traceLog, wfConfig);
                auditDetailAttributes.ChangedBy =
                    ((auditRecord.Contains("userid") &&
                      auditRecord.GetAttributeValue<EntityReference>("userid") != null)
                        ? auditRecord.GetAttributeValue<EntityReference>("userid").Name
                        : "");
                Utility.SetTrace($"auditDetailAttributesContract.ChangedBy: {auditDetailAttributes.ChangedBy}",
                    ref traceLog, wfConfig);
                auditDetailAttributes.ChangedFieldType =
                    GetChangedFieldType(attribute, attributeAuditDetail, ref traceLog, wfConfig);
                Utility.SetTrace(
                    $"auditDetailAttributesContract.ChangedFieldType: {auditDetailAttributes.ChangedFieldType}",
                    ref traceLog, wfConfig);
                auditDetailAttributes.ChangedFieldName =
                    GetAttributeDisplayInfo(entityMetadata, attribute.Key, ref traceLog, wfConfig);
                Utility.SetTrace(
                    $"auditDetailAttributesContract.ChangedFieldName: {auditDetailAttributes.ChangedFieldName}",
                    ref traceLog, wfConfig);
                auditDetailAttributes.ChangedFieldLogicalName = attribute.Key;
                Utility.SetTrace(
                    $"auditDetailAttributesContract.ChangedFieldLogicalName: {auditDetailAttributes.ChangedFieldLogicalName}",
                    ref traceLog, wfConfig);
                auditDetailAttributes.OldValue = ValidateAndGetAttributesValue(attribute, attributeAuditDetail,
                    entityMetadata, isForNewValue: false, ref traceLog, wfConfig);
                Utility.SetTrace($"auditDetailAttributes.OldValue: {auditDetailAttributes.OldValue}", ref traceLog,
                    wfConfig);
                auditDetailAttributes.NewValue = ValidateAndGetAttributesValue(attribute, attributeAuditDetail,
                    entityMetadata, isForNewValue: true, ref traceLog, wfConfig);
                Utility.SetTrace($"auditDetailAttributesContract.NewValue: {auditDetailAttributes.NewValue}",
                    ref traceLog, wfConfig);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return auditDetailAttributes;
        }

        private string GetChangedFieldType(KeyValuePair<string, object> attribute,
            AttributeAuditDetail attributeAuditDetail, ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "GetChangedFieldType: ";
            var result = string.Empty;

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                if (attributeAuditDetail.NewValue.Attributes.Contains(attribute.Key) &&
                    attributeAuditDetail.NewValue.Attributes[attribute.Key] != null &&
                    attributeAuditDetail.NewValue.Attributes[attribute.Key].GetType() != null &&
                    !string.IsNullOrEmpty(attributeAuditDetail.NewValue.Attributes[attribute.Key].GetType().FullName))
                {
                    result = attributeAuditDetail.NewValue.Attributes[attribute.Key].GetType().FullName;
                }
                else if (attributeAuditDetail.NewValue.Attributes.Contains(attribute.Key) &&
                         attributeAuditDetail.NewValue.Attributes[attribute.Key] != null &&
                         attributeAuditDetail.NewValue.Attributes[attribute.Key].GetType() != null &&
                         !string.IsNullOrEmpty(attributeAuditDetail.NewValue.Attributes[attribute.Key].GetType()
                             .FullName))
                {
                    result = attributeAuditDetail.NewValue.Attributes[attribute.Key].GetType().FullName;
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return result;
        }

        private string GetAttributeDisplayInfo(EntityMetadata entityMetadata, string attributeName,
            ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "GetAttributeDisplayName: ";
            var empty = string.Empty;

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                Utility.SetTrace($"Entity Metadata: {entityMetadata}", ref traceLog, wfConfig);
                Utility.SetTrace($"Attribute Name: {attributeName}", ref traceLog, wfConfig);
                if (entityMetadata == null && string.IsNullOrEmpty(attributeName))
                {
                    Utility.SetTrace("Entity metadata and entity name not found.", ref traceLog, wfConfig);
                    return "";
                }

                var list = (from attrColl in entityMetadata.Attributes
                            where attrColl.LogicalName == attributeName
                            select new
                            {
                                displayName = attrColl.DisplayName.UserLocalizedLabel.Label
                            }).ToList();
                if (list == null || list.Count <= 0 || list[0] == null)
                {
                    Utility.SetTrace("Display name not found.", ref traceLog, wfConfig);
                    return "";
                }

                empty = list[0].displayName;
                Utility.SetTrace($"Value: {empty}", ref traceLog, wfConfig);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return empty;
        }

        private string GetAttributeInfo(EntityMetadata entityMetadata, ref string logicalName, string attributemask,
            ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "GetAttributeInfo: ";
            var empty = string.Empty;
            var source = new string[8]
            {
                "salesstage", "stepname", "totallineitemamount", "totalamountlessfreight", "totalamount",
                "totaldiscountamount", "totaltax", "totallineitemdiscountamount"
            };

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                var list = (from attrColl in entityMetadata.Attributes
                            where attrColl.ColumnNumber == Convert.ToInt32(attributemask)
                            select new
                            {
                                isValidForUpdate = attrColl.IsValidForUpdate,
                                displayName = attrColl.DisplayName.UserLocalizedLabel.Label,
                                logicalName = attrColl.LogicalName
                            }).ToList();
                if (list == null || list.Count <= 0 || list[0] == null)
                {
                    Utility.SetTrace("logicalName name not found.", ref traceLog, wfConfig);
                    return "";
                }

                Utility.SetTrace($"logicalName :{list[0].logicalName}", ref traceLog, wfConfig);
                if (list[0].logicalName == "statecode" || list[0].logicalName == "statuscode" ||
                    list[0].isValidForUpdate != true || source.Contains(list[0].logicalName))
                {
                    Utility.SetTrace("Not valid field.", ref traceLog, wfConfig);
                    return string.Empty;
                }

                empty = list[0].displayName;
                logicalName = list[0].logicalName;
                Utility.SetTrace($"Value: {empty}", ref traceLog, wfConfig);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return empty;
        }

        private bool isValidAttribute(EntityMetadata entityMetadata, string attributemask, ref StringBuilder traceLog,
            WorkflowConfig wfConfig)
        {
            var arg = "isValidAttribute: ";
            var source = new string[8]
            {
                "salesstage", "stepname", "totallineitemamount", "totalamountlessfreight", "totalamount",
                "totaldiscountamount", "totaltax", "totallineitemdiscountamount"
            };
            var flag = false;
            var fieldDisplayName = string.Empty;
            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                var list = (from attrColl in entityMetadata.Attributes
                            where attrColl.ColumnNumber == Convert.ToInt32(attributemask)
                            select new
                            {
                                displayName = attrColl.DisplayName.UserLocalizedLabel.Label,
                                logicalName = attrColl.LogicalName
                            }).ToList();
                if (list == null || list.Count <= 0 || list[0] == null)
                {
                    Utility.SetTrace("logicalName name not found.", ref traceLog, wfConfig);
                    return false;
                }

                Utility.SetTrace($"logicalName :{list[0].logicalName}", ref traceLog, wfConfig);
                if (list[0].logicalName == "statecode" || list[0].logicalName == "statuscode" ||
                    !Utility.IsValidatForUpdate(entityMetadata, list[0].logicalName, ref fieldDisplayName,
                        getDisplayName: false, ref traceLog, wfConfig) || source.Contains(list[0].logicalName))
                {
                    Utility.SetTrace("Not valid field.", ref traceLog, wfConfig);
                    return false;
                }

                flag = true;
                Utility.SetTrace($"isValidAttribute: {flag.ToString()}", ref traceLog, wfConfig);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return flag;
        }

        private string ValidateAndGetAttributesValue(KeyValuePair<string, object> attribute,
            AttributeAuditDetail attributeAuditDetail, EntityMetadata entityMetadata, bool isForNewValue,
            ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "ValidateAndGetAttributesValue: ";
            var result = string.Empty;
            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                if (isForNewValue && attributeAuditDetail.NewValue.Attributes.Keys != null &&
                    attributeAuditDetail.NewValue.Attributes.Keys.ToArray() != null && attributeAuditDetail.NewValue
                        .Attributes.Keys.ToArray().Contains(attribute.Key))
                {
                    var index = attributeAuditDetail.NewValue.Attributes.Keys.ToList().IndexOf(attribute.Key);
                    Utility.SetTrace($"New value key: {attributeAuditDetail.NewValue.Attributes.ToList()[index]}",
                        ref traceLog, wfConfig);
                    result = GetAttributeValue(attributeAuditDetail.NewValue.Attributes.ToList()[index],
                        attributeAuditDetail.NewValue.Attributes, entityMetadata, ref traceLog, wfConfig);
                }
                else if (!isForNewValue && attributeAuditDetail.OldValue.Attributes.Keys != null &&
                         attributeAuditDetail.OldValue.Attributes.Keys.ToArray() != null && attributeAuditDetail
                             .OldValue.Attributes.Keys.ToArray().Contains(attribute.Key))
                {
                    var index = attributeAuditDetail.OldValue.Attributes.Keys.ToList().IndexOf(attribute.Key);
                    Utility.SetTrace($"Old value key: {attributeAuditDetail.OldValue.Attributes.ToList()[index]}",
                        ref traceLog, wfConfig);
                    result = GetAttributeValue(attributeAuditDetail.OldValue.Attributes.ToList()[index],
                        attributeAuditDetail.OldValue.Attributes, entityMetadata, ref traceLog, wfConfig);
                }
                else
                {
                    Utility.SetTrace("Key not present in old value colection.", ref traceLog, wfConfig);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return result;
        }

        private string GetAttributeValue(KeyValuePair<string, object> attribute,
            AttributeCollection attributeCollection, EntityMetadata entityMetadata, ref StringBuilder traceLog,
            WorkflowConfig wfConfig)
        {
            var arg = "GetAttributeValue: ";
            var empty = string.Empty;
            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                if (!attributeCollection.Contains(attribute.Key) ||
                    attributeCollection[attribute.Key].GetType() == null ||
                    attributeCollection[attribute.Key].GetType().FullName == null)
                {
                    Utility.SetTrace("Attribute type not found.", ref traceLog, wfConfig);
                    return "";
                }

                switch (attributeCollection[attribute.Key].GetType().FullName.ToString().ToLower())
                {
                    case "microsoft.xrm.sdk.optionsetvalue":
                        Utility.SetTrace("Inside optionsetvalue.", ref traceLog, wfConfig);
                        empty = GetOptionLabel(entityMetadata, ((OptionSetValue)attribute.Value).Value, attribute.Key,
                            ref traceLog, wfConfig);
                        break;
                    case "microsoft.xrm.sdk.money":
                        Utility.SetTrace("Inside money.", ref traceLog, wfConfig);
                        empty = ((Money)attribute.Value).Value.ToString();
                        break;
                    case "microsoft.xrm.sdk.entityreference":
                        Utility.SetTrace("Inside entityreference.", ref traceLog, wfConfig);
                        empty = ((EntityReference)attribute.Value).Name;
                        break;
                    case "microsoft.xrm.sdk.optionsetvaluecollection":
                        Utility.SetTrace("Inside optionsetvaluecollection.", ref traceLog, wfConfig);
                        empty = GetMultiOptionSetLabels(entityMetadata, (OptionSetValueCollection)attribute.Value,
                            attribute.Key, ref traceLog, wfConfig);
                        break;
                    case "microsoft.xrm.sdk.entitycollection":
                        Utility.SetTrace("Inside entitycollection.", ref traceLog, wfConfig);
                        empty = GetEntityCollectionValue((EntityCollection)attribute.Value, ref traceLog, wfConfig);
                        break;
                    case "system.boolean":
                        Utility.SetTrace("Inside boolean.", ref traceLog, wfConfig);
                        empty = GetBooleanAttributeLabel(entityMetadata, (bool)attribute.Value, attribute.Key,
                            ref traceLog, wfConfig);
                        break;
                    case "system.string":
                        Utility.SetTrace("Inside string.", ref traceLog, wfConfig);
                        empty = attribute.Value.ToString();
                        break;
                    case "system.decimal":
                        Utility.SetTrace("Inside decimal.", ref traceLog, wfConfig);
                        empty = attribute.Value.ToString();
                        break;
                    case "system.int32":
                        Utility.SetTrace("Inside int32.", ref traceLog, wfConfig);
                        empty = attribute.Value.ToString();
                        break;
                    case "system.double":
                        Utility.SetTrace("Inside double.", ref traceLog, wfConfig);
                        empty = attribute.Value.ToString();
                        break;
                    case "system.datetime":
                        Utility.SetTrace("Inside datetime.", ref traceLog, wfConfig);
                        empty = Utility.GetConvertedDate(Convert.ToDateTime(attribute.Value), ref traceLog, wfConfig);
                        break;
                    default:
                        empty = "";
                        break;
                }

                Utility.SetTrace($"Value: {empty}", ref traceLog, wfConfig);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return empty;
        }

        private string GetOptionLabel(EntityMetadata entityMetadata, int optionSetValue, string attributeName,
            ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "GetOptionLabel: ";
            var empty = string.Empty;
            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                Utility.SetTrace($"Entity Metadata: {entityMetadata}", ref traceLog, wfConfig);
                Utility.SetTrace($"Attribute Name: {attributeName}", ref traceLog, wfConfig);
                Utility.SetTrace($"Option Set Value: {optionSetValue}", ref traceLog, wfConfig);
                if (entityMetadata == null || string.IsNullOrEmpty(attributeName))
                {
                    Utility.SetTrace("Entity metadata, option set value or attribute name not found.", ref traceLog,
                        wfConfig);
                    return "";
                }

                var list = (from attrCol in entityMetadata.Attributes
                            where attrCol.LogicalName == attributeName
                            select new
                            {
                                attribute = attrCol
                            }).ToList();
                if (list == null || list[0] == null)
                {
                    Utility.SetTrace("Attribute metadata not found.", ref traceLog, wfConfig);
                    return "";
                }

                var list2 = (from options in ((PicklistAttributeMetadata)list[0].attribute).OptionSet.Options
                             where options.Value == optionSetValue
                             select new
                             {
                                 optionLabel = options.Label.UserLocalizedLabel.Label
                             }).ToList();
                if (list2 == null || list2[0] == null)
                {
                    Utility.SetTrace("No option set found", ref traceLog, wfConfig);
                    return "";
                }

                empty = list2[0].optionLabel;
                Utility.SetTrace($"optionLabel: {empty}", ref traceLog, wfConfig);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return empty;
        }

        private string GetMultiOptionSetLabels(EntityMetadata entityMetadata,
            OptionSetValueCollection optionSetValueColl, string attributeName, ref StringBuilder traceLog,
            WorkflowConfig wfConfig)
        {
            var arg = "GetMultiOptionSetLabels: ";
            var text = string.Empty;
            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                Utility.SetTrace($"Entity Metadata: {entityMetadata}", ref traceLog, wfConfig);
                Utility.SetTrace($"Attribute Name: {attributeName}", ref traceLog, wfConfig);
                if (entityMetadata == null || string.IsNullOrEmpty(attributeName) || optionSetValueColl == null ||
                    optionSetValueColl.Count <= 0)
                {
                    Utility.SetTrace("Entity metadata, option set value or attribute name not found.", ref traceLog,
                        wfConfig);
                    return "";
                }

                var list = (from attrCol in entityMetadata.Attributes
                            where attrCol.LogicalName == attributeName
                            select new
                            {
                                attribute = attrCol
                            }).ToList();
                if (list == null || list[0] == null)
                {
                    Utility.SetTrace("Attribute metadata not found.", ref traceLog, wfConfig);
                    return "";
                }

                Utility.SetTrace($"Optionset value collection count: {optionSetValueColl.Count}", ref traceLog,
                    wfConfig);
                int i;
                for (i = 0; i < optionSetValueColl.Count; i++)
                {
                    Utility.SetTrace($"For optionset value at: {i}", ref traceLog, wfConfig);
                    int num;
                    if (optionSetValueColl[i] != null)
                    {
                        _ = optionSetValueColl[i].Value;
                        num = 0;
                    }
                    else
                    {
                        num = 1;
                    }

                    if (num != 0)
                    {
                        Utility.SetTrace("Multioptionset value not found.", ref traceLog, wfConfig);
                        return "";
                    }

                    Utility.SetTrace($"MultiOption Set Value: {optionSetValueColl[i].Value}", ref traceLog, wfConfig);
                    var list2 = (from options in ((MultiSelectPicklistAttributeMetadata)list[0].attribute).OptionSet
                            .Options
                                 where options.Value == optionSetValueColl[i].Value
                                 select new
                                 {
                                     optionLabel = options.Label.UserLocalizedLabel.Label
                                 }).ToList();
                    if (list2 == null || list2[0] == null)
                    {
                        Utility.SetTrace("No option set found", ref traceLog, wfConfig);
                        return "";
                    }

                    if (!string.IsNullOrEmpty(text))
                    {
                        text += ", ";
                    }

                    text += list2[0].optionLabel;
                }

                Utility.SetTrace($"optionLabel: {text}", ref traceLog, wfConfig);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return text;
        }

        private string GetEntityCollectionValue(EntityCollection selectedValueColl, ref StringBuilder traceLog,
            WorkflowConfig wfConfig)
        {
            var arg = "GetEntityCollectionValue: ";
            var text = string.Empty;
            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                if (selectedValueColl == null || selectedValueColl.Entities.Count <= 0)
                {
                    Utility.SetTrace("Entity collection or entity metadata not valid.", ref traceLog, wfConfig);
                    return "";
                }

                Utility.SetTrace($"Entity collection entities count: {selectedValueColl.Entities.Count}", ref traceLog,
                    wfConfig);
                for (var i = 0; i < selectedValueColl.Entities.Count; i++)
                {
                    Utility.SetTrace($"Entity at: {i}", ref traceLog, wfConfig);
                    if (selectedValueColl.Entities[i] == null ||
                        !selectedValueColl.Entities[i].Attributes.Contains("partyid"))
                    {
                        Utility.SetTrace("Value not found.", ref traceLog, wfConfig);
                        return "";
                    }

                    Utility.SetTrace(
                        string.Format("Entity Collection Value: {0}",
                            ((EntityReference)selectedValueColl.Entities[i].Attributes["partyid"]).Name), ref traceLog,
                        wfConfig);
                    if (!string.IsNullOrEmpty(text))
                    {
                        text += ", ";
                    }

                    text += ((EntityReference)selectedValueColl.Entities[i].Attributes["partyid"]).Name;
                }

                Utility.SetTrace($"entityCollectionValues: {text}", ref traceLog, wfConfig);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return text;
        }

        private string GetBooleanAttributeLabel(EntityMetadata entityMetadata, bool attributeValue,
            string attributeName, ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "GetBooleanAttributeLabel: ";
            var empty = string.Empty;
            BooleanAttributeMetadata booleanAttributeMetadata = null;
            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                Utility.SetTrace($"Entity Metadata: {entityMetadata}", ref traceLog, wfConfig);
                Utility.SetTrace($"Attribute Name: {attributeName}", ref traceLog, wfConfig);
                Utility.SetTrace($"Option Set Value: {attributeValue}", ref traceLog, wfConfig);
                if (entityMetadata == null || string.IsNullOrEmpty(attributeName))
                {
                    Utility.SetTrace("Entity metadata, attribute value or attribute name not found.", ref traceLog,
                        wfConfig);
                    return "";
                }

                var list = (from attrCol in entityMetadata.Attributes
                            where attrCol.LogicalName == attributeName
                            select new
                            {
                                attribute = attrCol
                            }).ToList();
                if (list == null || list[0] == null)
                {
                    Utility.SetTrace("Attribute metadata not found.", ref traceLog, wfConfig);
                    return "";
                }

                booleanAttributeMetadata = (BooleanAttributeMetadata)list[0].attribute;
                empty = ((!attributeValue)
                    ? ((booleanAttributeMetadata != null && booleanAttributeMetadata.OptionSet != null &&
                        booleanAttributeMetadata.OptionSet.FalseOption != null &&
                        booleanAttributeMetadata.OptionSet.FalseOption.Label != null &&
                        booleanAttributeMetadata.OptionSet.FalseOption.Label.UserLocalizedLabel != null)
                        ? booleanAttributeMetadata.OptionSet.FalseOption.Label.UserLocalizedLabel.Label
                        : string.Empty)
                    : ((booleanAttributeMetadata != null && booleanAttributeMetadata.OptionSet != null &&
                        booleanAttributeMetadata.OptionSet.TrueOption != null &&
                        booleanAttributeMetadata.OptionSet.TrueOption.Label != null &&
                        booleanAttributeMetadata.OptionSet.TrueOption.Label.UserLocalizedLabel != null)
                        ? booleanAttributeMetadata.OptionSet.TrueOption.Label.UserLocalizedLabel.Label
                        : string.Empty));
                Utility.SetTrace($"optionLabel: {empty}", ref traceLog, wfConfig);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return empty;
        }

        private string GenerateXml(SerializeAuditDetails entity, Type objecttype, ref StringBuilder traceLog,
            WorkflowConfig wfConfig)
        {
            var arg = "GenerateXml: ";
            var result = string.Empty;
            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                var assemblyQualifiedName = objecttype.AssemblyQualifiedName;
                var dataContractJsonSerializer = new DataContractJsonSerializer(objecttype);
                using var memoryStream = new MemoryStream();
                dataContractJsonSerializer.WriteObject(memoryStream, entity);
                memoryStream.Position = 0L;
                var streamReader = new StreamReader(memoryStream);
                result = streamReader.ReadToEnd();
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return result;
        }
    }
}