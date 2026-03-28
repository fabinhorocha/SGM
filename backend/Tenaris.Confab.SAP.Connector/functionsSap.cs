using SAP.Middleware.Connector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tenaris.Confab.SAP.Connector
{
    public class FunctionsSap
    {

        public string Name { get; set; }
        public string AppServerHost { get; set; }
        public string SystemNumber { get; set; }
        public string Client { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Language { get; set; }
        public string PoolSize { get; set; }


        public FunctionsSap(string _Name, string _AppServerHost, string _SystemNumber, string _Client, string _User, string _Password, string _Language, string _PoolSize)
        {
            Name = _Name;
            AppServerHost = _AppServerHost;
            SystemNumber = _SystemNumber;
            Client = _Client;
            User = _User;
            Password = _Password;
            Language = _Language;
            PoolSize = _PoolSize;
        }

        public bool TestConnect(ref string Message)
        {
            RfcDestination rfc = null;
            var connectSAP = new ConnectSap();

            return connectSAP.Connect(Name, AppServerHost, SystemNumber, Client, User, Password, Language, PoolSize, ref rfc, ref Message);
        }

        /// <summary>
        /// READ TABLE AND RETURN DATA
        /// </summary>
        /// <param name="table">J_1BNFDOC</param>
        /// <param name="fields">MODEL,etc</param>
        /// <param name="filter">DOCNUM EQ 20</param>
        /// <param name="delimiter">|</param>
        /// <param name="message"></param>
        /// <returns>DataSet</returns>
        public DataSet ReadTable(string table, List<RFC_DB_FLD> fields, List<string> filters, char delimiter, ref string message)
        {
            RfcDestination rfc = null;
            DataSet dsData = null;
            var connectSAP = new ConnectSap();


            bool connect = connectSAP.Connect(Name, AppServerHost, SystemNumber, Client, User, Password, Language, PoolSize, ref rfc, ref message);

            if (connect)
            {
                try
                {
                    message = "";
                    RfcRepository rfcRepo = rfc.Repository;

                    IRfcFunction rfcFunc = rfcRepo.CreateFunction("BBP_RFC_READ_TABLE");
                    rfcFunc.SetValue("query_table", table);
                    rfcFunc.SetValue("delimiter", delimiter);


                    IRfcTable rfcTableFields = rfcFunc["FIELDS"].GetTable();
                    IRfcTable rfcTableOptions = rfcFunc["OPTIONS"].GetTable();

                    IRfcStructure rfcStructureFields = null;

                    foreach (var field in fields)
                    {
                        rfcStructureFields = rfcRepo.GetStructureMetadata("RFC_DB_FLD").CreateStructure();
                        rfcStructureFields.SetValue("FIELDNAME", field.FIELDNAME);
                        rfcStructureFields.SetValue("LENGTH", field.LENGTH);
                        rfcStructureFields.SetValue("TYPE", (char)field.TYPE);
                        rfcStructureFields.SetValue("FIELDTEXT", field.FIELDTEXT.ToString());
                        rfcTableFields.Insert(rfcStructureFields);
                    }


                    IRfcStructure rfcStructureOption = null;

                    for (var i = filters.Count - 1; i >= 0; i--)
                    {
                        rfcStructureOption = rfcRepo.GetStructureMetadata("RFC_DB_OPT").CreateStructure();
                        rfcStructureOption.SetValue("TEXT", filters[i]);
                        rfcTableOptions.Insert(rfcStructureOption);
                    }

                    //foreach (var filter in filters)
                    //{
                    //    rfcStructureOption = rfcRepo.GetStructureMetadata("RFC_DB_OPT").CreateStructure();
                    //    rfcStructureOption.SetValue("TEXT", filter);
                    //    rfcTableOptions.Insert(rfcStructureOption);
                    //}


                    rfcFunc.Invoke(rfc);

                    IRfcTable rfcTableData = rfcFunc["DATA"].GetTable();

                    //ORDER COLUMNS 
                    var columns = new List<string>();
                    foreach (var field in rfcTableFields)
                    {
                        RfcElementMetadata metadata = field.GetElementMetadata("FIELDNAME");
                        columns.Add(field.GetString(metadata.Name));
                    }


                    dsData = new DataSet();
                    dsData.Tables.Add(ConvertToTable(rfcTableData, columns, delimiter));

                }

                catch (Exception ex)
                {
                    message = "Error BBP_RFC_READ_TABLE: " + ex.Message;

                }


            }


            return dsData;
        }

        public DataSet GetData(string nameFunction, string nameTable, IDictionary<string, string> Values, ref string Message)
        {
            try
            {
                var dsData = new DataSet();
                RfcDestination rfc = null;
                var connectSAP = new ConnectSap();


                bool connect = connectSAP.Connect(Name, AppServerHost, SystemNumber, Client, User, Password, Language, PoolSize, ref rfc, ref Message);

                if (connect)
                {

                    RfcRepository rfcRepo = rfc.Repository;
                    IRfcFunction rfcFunc = rfcRepo.CreateFunction(nameFunction);

                    foreach (var value in Values)
                    {
                        rfcFunc.SetValue(value.Key, value.Value);
                    }

                    rfcFunc.Invoke(rfc);

                    dsData.Tables.Add(ConvertToTable(rfcFunc.GetTable(nameTable)));

                    return dsData;


                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Message = "Erro GetData: " + ex.Message;
                return null;
            }

        }

        public DataSet GetData(string nameFunction, string nameTable, string nameTableMetadata, IDictionary<string, string> Values, ref string Message)
        {
            try
            {
                var dsData = new DataSet();
                RfcDestination rfc = null;
                var connectSAP = new ConnectSap();


                bool connect = connectSAP.Connect(Name, AppServerHost, SystemNumber, Client, User, Password, Language, PoolSize, ref rfc, ref Message);

                if (connect)
                {

                    RfcRepository rfcRepo = rfc.Repository;
                    IRfcFunction rfcFunc = rfcRepo.CreateFunction(nameFunction);

                    foreach (var value in Values)
                    {
                        rfcFunc.SetValue(value.Key, value.Value);
                    }

                    rfcFunc.Invoke(rfc);
                    IRfcStructure structureData = rfcFunc.GetStructure(nameTable);
                    IRfcTable tableData = rfc.Repository.GetTableMetadata(nameTableMetadata).CreateTable();
                    tableData = structureData.GetTable(nameTable);


                    dsData.Tables.Add(ConvertToTable(tableData));

                    return dsData;


                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Message = "Erro GetData: " + ex.Message;
                return null;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameFunction">BAPI_PO_CREATE</param>
        /// <param name="nameTransaction">BAPI_TRANSACTION_COMMIT</param>
        /// <param name="nameHeader">PO_HEADER</param>
        /// <param name="nameItemsTable">PO_ITEMS</param>
        /// <param name="nameSchedulesTable">PO_ITEM_SCHEDULES</param>
        /// <param name="valuesTransaction">[{"WAIT","X"}]</param>
        /// <param name="valuesHeader">[{"DOC_TYPE", "NB"},{"PURCH_ORG", "0001"},{"PURCH_ORG", "0001"}]</param>        
        /// <param name="valuesItemsTable">[{"PO_ITEM", "1"},{"PUR_MAT", "TEST_MAT"},{"PLANT", "0001"}]</param>
        /// <param name="valuesSchedulesTable">[{"PO_ITEM", "1"},{"PUR_MAT", "DELIV_DATE"},{"QUANTITY", 10}]</param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public DataSet CreateData(string nameFunction, string nameTransaction, string nameHeader, string nameItemsTable, string nameSchedulesTable, IDictionary<string, string> valuesTransaction, IDictionary<string, string> valuesHeader, IDictionary<string, string> valuesItemsTable, IDictionary<string, string> valuesSchedulesTable, string fieldReturn, ref string Message)
        {
            try
            {
                var dsData = new DataSet();
                RfcDestination rfc = null;
                var connectSAP = new ConnectSap();

                bool connect = connectSAP.Connect(Name, AppServerHost, SystemNumber, Client, User, Password, Language, PoolSize, ref rfc, ref Message);

                if (connect)
                {

                    //RfcSessionManager.BeginContext(rfc);

                    //RfcSessionManager.SessionChangeHandler x = new RfcSessionManager.SessionChangeHandler((z)=> {  });

                    RfcRepository rfcRepo = rfc.Repository;
                    IRfcFunction rfcFunc = rfcRepo.CreateFunction(nameFunction);
                    IRfcFunction rfcTransaction = rfcRepo.CreateFunction(nameTransaction);

                    foreach (var value in valuesTransaction)
                    {
                        rfcTransaction.SetValue(value.Key, value.Value);
                    }

                    IRfcStructure rfcHeader = rfcFunc[nameHeader].GetStructure();
                    foreach (var value in valuesHeader)
                    {
                        rfcHeader.SetValue(value.Key, value.Value);
                    }

                    IRfcTable rfcItems = rfcFunc[nameItemsTable].GetTable();
                    IRfcStructure rfcItem = rfcItems.Metadata.LineType.CreateStructure();

                    foreach (var value in valuesItemsTable)
                    {
                        rfcItem.SetValue(value.Key, value.Value);
                    }

                    rfcItems.Insert(rfcItem);


                    IRfcTable rfcSchedules = rfcFunc[nameSchedulesTable].GetTable();
                    IRfcStructure rfcSchedule = rfcItems.Metadata.LineType.CreateStructure();

                    foreach (var value in valuesSchedulesTable)
                    {
                        rfcSchedule.SetValue(value.Key, value.Value);
                    }

                    rfcSchedules.Insert(rfcSchedule);

                    rfcFunc.Invoke(rfc);

                    rfcTransaction.Invoke(rfc);

                    IRfcTable rfcReturns = rfcFunc[fieldReturn].GetTable();

                    dsData.Tables.Add(ConvertToTable(rfcReturns));

                    //RfcSessionManager.EndContext(rfc);


                    return dsData;


                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Message = "Error GetData: " + ex.Message;
                return null;
            }

        }

        /// <summary>
        /// CREATE WARNING WITH CUSTOM BAPI ZPM_CREAR_AVISO_GAM2
        /// </summary>        
        /// <param name="idReport"></param>  
        /// <param name="valuesFunc"></param>        
        /// <param name="Message"></param>
        /// <returns></returns>
        public int? ZPM_CREAR_AVISO_GAM2(int idReport, IDictionary<string, string> valuesFunc, ref string Message)
        {
            try
            {
                RfcDestination rfc = null;
                var connectSAP = new ConnectSap();

                bool connect = connectSAP.Connect(Name, AppServerHost, SystemNumber, Client, User, Password, Language, PoolSize, ref rfc, ref Message);

                if (connect)
                {

                    //RfcSessionManager.BeginContext(rfc);

                    //RfcSessionManager.SessionChangeHandler x = new RfcSessionManager.SessionChangeHandler((z) => { });

                    RfcRepository rfcRepo = rfc.Repository;
                    IRfcFunction rfcFunc = rfcRepo.CreateFunction("ZPM_CREAR_AVISO_GAM2");



                    foreach (var value in valuesFunc)
                    {
                        rfcFunc.SetValue(value.Key, value.Value);
                    }



                    rfcFunc.Invoke(rfc);


                    object rfcReturn = rfcFunc["NROAVISO"].GetValue();

                    //RfcSessionManager.EndContext(rfc);

                    Message = rfcReturn.ToString() == "" ? "Não foi possível criar aviso para o laudo " + idReport.ToString() + " !" : "Aviso criado com sucesso para o laudo " + idReport.ToString() + " !";

                    return rfcReturn.ToString() == "" ? null : (int?)Convert.ToInt32(rfcReturn);


                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Message = "Error ZPM_CREAR_AVISO_GAM2: " + ex.Message;
                return null;
            }

        }

        /// <summary>
        /// CREATE WARNING WITH CUSTOM BAPI ZPM_CREAR_DOCUMENTO
        /// </summary>        
        /// <param name="valuesFunc"></param>        
        /// <param name="Message"></param>
        /// <returns></returns>
        public bool ZPM_CREAR_DOCUMENTO(IDictionary<string, string> valuesFunc, ref string Message)
        {
            try
            {
                RfcDestination rfc = null;
                var connectSAP = new ConnectSap();

                bool connect = connectSAP.Connect(Name, AppServerHost, SystemNumber, Client, User, Password, Language, PoolSize, ref rfc, ref Message);

                if (connect)
                {

                    // RfcSessionManager.BeginContext(rfc);

                    // RfcSessionManager.SessionChangeHandler x = new RfcSessionManager.SessionChangeHandler((z) => { });

                    RfcRepository rfcRepo = rfc.Repository;
                    IRfcFunction rfcFunc = rfcRepo.CreateFunction("ZPM_CREAR_DOCUMENTO");


                    foreach (var value in valuesFunc)
                    {
                        rfcFunc.SetValue(value.Key, value.Value);
                    }


                    rfcFunc.Invoke(rfc);


                    object rfcReturn = rfcFunc["TRANS_OK"].GetValue();

                    // RfcSessionManager.EndContext(rfc);

                    Message = rfcReturn.ToString() == "SI" ? "Documento anexado com sucesso !" : "Não foi possível anexar o documento !";

                    return rfcReturn.ToString() == "SI" ? true : false;


                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Message = "Error ZPM_CREAR_DOCUMENTO: " + ex.Message;
                return false;
            }

        }


        /// <summary>
        /// CREATE NOTE
        /// </summary>        
        /// <param name="idReport"></param>  
        /// <param name="valuesFunc"></param>        
        /// <param name="Message"></param>
        /// <returns></returns>
        public DataSet BAPI_ALM_NOTIF_CREATE(string typeNotif, IDictionary<string, string> valuesHeader, IDictionary<int, Dictionary<string, string>> itemsLines, ref string Message)
        {
            try
            {
                var dsData = new DataSet();

                RfcDestination rfc = null;
                var connectSAP = new ConnectSap();

                bool connect = connectSAP.Connect(Name, AppServerHost, SystemNumber, Client, User, Password, Language, PoolSize, ref rfc, ref Message);

                if (connect)
                {

                    RfcSessionManager.BeginContext(rfc);


                    RfcSessionManager.SessionChangeHandler x = new RfcSessionManager.SessionChangeHandler((z) => { });

                    RfcRepository rfcRepo = rfc.Repository;


                    //IRfcFunction rfcFuncq = rfcRepo.CreateFunction("IW21"); 
                    IRfcFunction rfcFunc = rfcRepo.CreateFunction("BAPI_ALM_NOTIF_CREATE");


                    rfcFunc.SetValue("NOTIF_TYPE", typeNotif);

                    IRfcStructure rfcHeader = rfcFunc["NOTIFHEADER"].GetStructure();

                    foreach (var value in valuesHeader)
                    {
                        rfcHeader.SetValue(value.Key, value.Value);
                    }


                    //IRfcTable rfcTexts = rfcFunc["NOTITEM"].GetTable();
                    //IRfcStructure rfcItem = rfcTexts.Metadata.LineType.CreateStructure();


                    //foreach (var line in itemsLines)
                    //{
                    //    foreach (var content in line.Value)
                    //    {
                    //        rfcItem.SetValue(content.Key, content.Value);

                    //    }

                    //    rfcTexts.Insert(rfcItem);
                    //}

                    rfcFunc.Invoke(rfc);


                    //object rfcReturn = rfcFunc["NROAVISO"].GetValue();

                    ////RfcSessionManager.EndContext(rfc);

                    //Message = rfcReturn.ToString() == "" ? "Não foi possível criar aviso para o laudo " + idReport.ToString() + " !" : "Aviso criado com sucesso para o laudo " + idReport.ToString() + " !";

                    //return rfcReturn.ToString() == "" ? null : (int?)Convert.ToInt32(rfcReturn);
                    IRfcTable rfcReturns = rfcFunc["RETURN"].GetTable();

                    dsData.Tables.Add(ConvertToTable(rfcReturns));

                    if (dsData.Tables[0].Rows.Count == 0)
                    {

                        IRfcFunction rfcFuncSave = rfcRepo.CreateFunction("BAPI_ALM_NOTIF_SAVE");
                        var id = rfcFunc["NOTIFHEADER_EXPORT"].GetStructure()[0].GetValue();

                        rfcFuncSave.SetValue("NOTIFHEADER", rfcFunc["NOTIFHEADER_EXPORT"].GetStructure());
                        rfcFuncSave.SetValue("NUMBER", id);


                        IRfcFunction rfcFuncCommit = rfcRepo.CreateFunction("BAPI_TRANSACTION_COMMIT");

                        rfcFuncSave.Invoke(rfc);
                        rfcFuncCommit.Invoke(rfc);


                        //RfcSessionManager.EndContext(rfc);

                        dsData = new DataSet();
                        dsData.Tables.Add(ConvertToTable(rfcFuncSave["RETURN"].GetTable()));

                    }


                    RfcSessionManager.EndContext(rfc);

                    return dsData;

                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Message = "Error ZPM_CREAR_AVISO_GAM2: " + ex.Message;
                return null;
            }

        }

        public DataSet BAPI_ALM_ORDERHEADER_GETLIST_OT(List<string> planPlants, List<string> typeDocs, List<string> plantGroups, DateTime dateStart, DateTime dateEnd, bool complete, ref string message)
        {
            RfcDestination rfc = null;
            var dsData = new DataSet();
            var connectSAP = new ConnectSap();

            var Message = "";

            bool connect = connectSAP.Connect(Name, AppServerHost, SystemNumber, Client, User, Password, Language, PoolSize, ref rfc, ref Message);

            if (connect)
            {

                try
                {

                    //RfcSessionManager.BeginContext(rfc);

                    // RfcSessionManager.SessionChangeHandler x = new RfcSessionManager.SessionChangeHandler((z) => { });

                    RfcRepository rfcRepo = rfc.Repository;

                    IRfcFunction rfcFunc = rfcRepo.CreateFunction("BAPI_ALM_ORDERHEAD_GET_LIST");

                    IRfcStructure rfcStructureParams = rfcFunc["DISPLAY_PARAMETERS"].GetStructure();
                    rfcStructureParams.SetValue("PAGELENGTH", "1000000");


                    IRfcTable rfcTableRange = rfcFunc["IT_RANGES"].GetTable();

                    IRfcStructure rfcStructureRange = null;

                    foreach (string doc in typeDocs)
                    {
                        rfcStructureRange = rfcRepo.GetStructureMetadata("BAPI_ALM_ORDER_LISTHEAD_RANGES").CreateStructure();
                        rfcStructureRange.SetValue("FIELD_NAME", "OPTIONS_FOR_DOC_TYPE");
                        rfcStructureRange.SetValue("SIGN", "");
                        rfcStructureRange.SetValue("OPTION", "");
                        rfcStructureRange.SetValue("LOW_VALUE", doc);
                        rfcStructureRange.SetValue("HIGH_VALUE", "");
                        rfcTableRange.Insert(rfcStructureRange);
                    }

                    foreach (string planPlant in planPlants)
                    {
                        rfcStructureRange = rfcRepo.GetStructureMetadata("BAPI_ALM_ORDER_LISTHEAD_RANGES").CreateStructure();
                        rfcStructureRange.SetValue("FIELD_NAME", "OPTIONS_FOR_PLANPLANT");
                        rfcStructureRange.SetValue("SIGN", "");
                        rfcStructureRange.SetValue("OPTION", "");
                        rfcStructureRange.SetValue("LOW_VALUE", planPlant);
                        rfcStructureRange.SetValue("HIGH_VALUE", "");
                        rfcTableRange.Insert(rfcStructureRange);
                    }

                    foreach (string planGroup in plantGroups)
                    {
                        rfcStructureRange = rfcRepo.GetStructureMetadata("BAPI_ALM_ORDER_LISTHEAD_RANGES").CreateStructure();
                        rfcStructureRange.SetValue("FIELD_NAME", "OPTIONS_FOR_PLANGROUP");
                        rfcStructureRange.SetValue("SIGN", "");
                        rfcStructureRange.SetValue("OPTION", "");
                        rfcStructureRange.SetValue("LOW_VALUE", planGroup);
                        rfcStructureRange.SetValue("HIGH_VALUE", "");
                        rfcTableRange.Insert(rfcStructureRange);
                    }


                    if (!complete)
                    {
                        rfcStructureRange = rfcRepo.GetStructureMetadata("BAPI_ALM_ORDER_LISTHEAD_RANGES").CreateStructure();
                        rfcStructureRange.SetValue("FIELD_NAME", "OPTIONS_FOR_ENTER_DATE");
                        rfcStructureRange.SetValue("SIGN", "I");
                        rfcStructureRange.SetValue("OPTION", "BT");
                        rfcStructureRange.SetValue("LOW_VALUE", "");
                        rfcStructureRange.SetValue("HIGH_VALUE", dateEnd.ToString("yyyyMMdd"));
                        rfcTableRange.Insert(rfcStructureRange);
                    }


                    rfcStructureRange = rfcRepo.GetStructureMetadata("BAPI_ALM_ORDER_LISTHEAD_RANGES").CreateStructure();
                    rfcStructureRange.SetValue("FIELD_NAME", "SHOW_DOCS_WITH_FROM_DATE");
                    rfcStructureRange.SetValue("SIGN", complete ? "I" : "");
                    rfcStructureRange.SetValue("OPTION", complete ? "EQ" : "");
                    rfcStructureRange.SetValue("LOW_VALUE", complete ? dateStart.ToString("yyyyMMdd") : "");
                    rfcStructureRange.SetValue("HIGH_VALUE", "");
                    rfcTableRange.Insert(rfcStructureRange);


                    rfcStructureRange = rfcRepo.GetStructureMetadata("BAPI_ALM_ORDER_LISTHEAD_RANGES").CreateStructure();
                    rfcStructureRange.SetValue("FIELD_NAME", "SHOW_DOCS_WITH_TO_DATE");
                    rfcStructureRange.SetValue("SIGN", complete ? "I" : "");
                    rfcStructureRange.SetValue("OPTION", complete ? "EQ" : "");
                    rfcStructureRange.SetValue("LOW_VALUE", complete ? dateEnd.ToString("yyyyMMdd") : "99991231");
                    rfcStructureRange.SetValue("HIGH_VALUE", "");
                    rfcTableRange.Append(rfcStructureRange);


                    if (complete)
                    {
                        rfcStructureRange = rfcRepo.GetStructureMetadata("BAPI_ALM_ORDER_LISTHEAD_RANGES").CreateStructure();
                        rfcStructureRange.SetValue("FIELD_NAME", "SHOW_COMPLETED_DOCUMENTS");
                        rfcStructureRange.SetValue("SIGN", "I");
                        rfcStructureRange.SetValue("OPTION", "EQ");
                        rfcStructureRange.SetValue("LOW_VALUE", "X");
                        rfcStructureRange.SetValue("HIGH_VALUE", "");
                        rfcTableRange.Append(rfcStructureRange);


                        //rfcStructureRange = rfcRepo.GetStructureMetadata("BAPI_ALM_ORDER_LISTHEAD_RANGES").CreateStructure();
                        //rfcStructureRange.SetValue("FIELD_NAME", "SHOW_HISTORICAL_DOCUMENTS");
                        //rfcStructureRange.SetValue("SIGN", "I");
                        //rfcStructureRange.SetValue("OPTION", "EQ");
                        //rfcStructureRange.SetValue("LOW_VALUE", "X");
                        //rfcStructureRange.SetValue("HIGH_VALUE", "");
                        //rfcTableRange.Append(rfcStructureRange);
                    }
                    else
                    {
                        rfcStructureRange = rfcRepo.GetStructureMetadata("BAPI_ALM_ORDER_LISTHEAD_RANGES").CreateStructure();
                        rfcStructureRange.SetValue("FIELD_NAME", "SHOW_OPEN_DOCUMENTS");
                        rfcStructureRange.SetValue("SIGN", "I");
                        rfcStructureRange.SetValue("OPTION", "EQ");
                        rfcStructureRange.SetValue("LOW_VALUE", "X");
                        rfcStructureRange.SetValue("HIGH_VALUE", "");
                        rfcTableRange.Append(rfcStructureRange);

                        rfcStructureRange = rfcRepo.GetStructureMetadata("BAPI_ALM_ORDER_LISTHEAD_RANGES").CreateStructure();
                        rfcStructureRange.SetValue("FIELD_NAME", "SHOW_DOCUMENTS_IN_PROCESS");
                        rfcStructureRange.SetValue("SIGN", "I");
                        rfcStructureRange.SetValue("OPTION", "EQ");
                        rfcStructureRange.SetValue("LOW_VALUE", "X");
                        rfcStructureRange.SetValue("HIGH_VALUE", "");
                        rfcTableRange.Append(rfcStructureRange);
                    }




                    rfcFunc.SetValue("IT_RANGES", rfcTableRange);

                    rfcFunc.Invoke(rfc);

                    IRfcTable rfcReturns = rfcFunc["ET_RESULT"].GetTable();

                    dsData.Tables.Add(ConvertToTable(rfcReturns));

                    message = "Dados processados com sucesso !";

                    //RfcSessionManager.EndContext(rfc);
                }

                catch (Exception ex)
                {
                    message = ex.Message;
                    //RfcSessionManager.EndContext(rfc);
                }


            }

            return dsData;
        }

        public DataSet BAPI_ALM_ORDERHEADER_GETLIST(List<string> planPlants, List<string> typeDocs, List<string> plantGroups, DateTime dateStart, DateTime dateEnd, ref string message)
        {
            RfcDestination rfc = null;
            var dsData = new DataSet();
            var connectSAP = new ConnectSap();

            var Message = "";

            bool connect = connectSAP.Connect(Name, AppServerHost, SystemNumber, Client, User, Password, Language, PoolSize, ref rfc, ref Message);

            if (connect)
            {


                try
                {


                    //RfcSessionManager.BeginContext(rfc);

                    //RfcSessionManager.SessionChangeHandler x = new RfcSessionManager.SessionChangeHandler((z) => { });

                    RfcRepository rfcRepo = rfc.Repository;

                    IRfcFunction rfcFunc = rfcRepo.CreateFunction("BAPI_ALM_ORDERHEAD_GET_LIST");

                    IRfcStructure rfcStructureParams = rfcFunc["DISPLAY_PARAMETERS"].GetStructure();
                    rfcStructureParams.SetValue("PAGELENGTH", "1000000");


                    IRfcTable rfcTableRange = rfcFunc["IT_RANGES"].GetTable();

                    IRfcStructure rfcStructureRange = null;

                    foreach (string doc in typeDocs)
                    {
                        rfcStructureRange = rfcRepo.GetStructureMetadata("BAPI_ALM_ORDER_LISTHEAD_RANGES").CreateStructure();
                        rfcStructureRange.SetValue("FIELD_NAME", "OPTIONS_FOR_DOC_TYPE");
                        rfcStructureRange.SetValue("SIGN", "");
                        rfcStructureRange.SetValue("OPTION", "");
                        rfcStructureRange.SetValue("LOW_VALUE", doc);
                        rfcStructureRange.SetValue("HIGH_VALUE", "");
                        rfcTableRange.Insert(rfcStructureRange);
                    }

                    foreach (string planPlant in planPlants)
                    {
                        rfcStructureRange = rfcRepo.GetStructureMetadata("BAPI_ALM_ORDER_LISTHEAD_RANGES").CreateStructure();
                        rfcStructureRange.SetValue("FIELD_NAME", "OPTIONS_FOR_PLANPLANT");
                        rfcStructureRange.SetValue("SIGN", "");
                        rfcStructureRange.SetValue("OPTION", "");
                        rfcStructureRange.SetValue("LOW_VALUE", planPlant);
                        rfcStructureRange.SetValue("HIGH_VALUE", "");
                        rfcTableRange.Insert(rfcStructureRange);
                    }

                    foreach (string planGroup in plantGroups)
                    {
                        rfcStructureRange = rfcRepo.GetStructureMetadata("BAPI_ALM_ORDER_LISTHEAD_RANGES").CreateStructure();
                        rfcStructureRange.SetValue("FIELD_NAME", "OPTIONS_FOR_PLANGROUP");
                        rfcStructureRange.SetValue("SIGN", "");
                        rfcStructureRange.SetValue("OPTION", "");
                        rfcStructureRange.SetValue("LOW_VALUE", planGroup);
                        rfcStructureRange.SetValue("HIGH_VALUE", "");
                        rfcTableRange.Insert(rfcStructureRange);
                    }


                    rfcStructureRange = rfcRepo.GetStructureMetadata("BAPI_ALM_ORDER_LISTHEAD_RANGES").CreateStructure();
                    rfcStructureRange.SetValue("FIELD_NAME", "OPTIONS_FOR_START_DATE");
                    rfcStructureRange.SetValue("SIGN", "I");
                    rfcStructureRange.SetValue("OPTION", "BT");
                    rfcStructureRange.SetValue("LOW_VALUE", dateStart.ToString("yyyyMMdd"));
                    rfcStructureRange.SetValue("HIGH_VALUE", dateEnd.ToString("yyyyMMdd"));
                    rfcTableRange.Insert(rfcStructureRange);


                    rfcStructureRange = rfcRepo.GetStructureMetadata("BAPI_ALM_ORDER_LISTHEAD_RANGES").CreateStructure();
                    rfcStructureRange.SetValue("FIELD_NAME", "SHOW_DOCS_WITH_FROM_DATE");
                    rfcStructureRange.SetValue("SIGN", "");
                    rfcStructureRange.SetValue("OPTION", "");
                    rfcStructureRange.SetValue("LOW_VALUE", "");
                    rfcStructureRange.SetValue("HIGH_VALUE", "");
                    rfcTableRange.Insert(rfcStructureRange);


                    rfcStructureRange = rfcRepo.GetStructureMetadata("BAPI_ALM_ORDER_LISTHEAD_RANGES").CreateStructure();
                    rfcStructureRange.SetValue("FIELD_NAME", "SHOW_DOCS_WITH_TO_DATE");
                    rfcStructureRange.SetValue("SIGN", "");
                    rfcStructureRange.SetValue("OPTION", "");
                    rfcStructureRange.SetValue("LOW_VALUE", "99991231");
                    rfcStructureRange.SetValue("HIGH_VALUE", "");
                    rfcTableRange.Append(rfcStructureRange);


                    rfcStructureRange = rfcRepo.GetStructureMetadata("BAPI_ALM_ORDER_LISTHEAD_RANGES").CreateStructure();
                    rfcStructureRange.SetValue("FIELD_NAME", "SHOW_OPEN_DOCUMENTS");
                    rfcStructureRange.SetValue("SIGN", "I");
                    rfcStructureRange.SetValue("OPTION", "EQ");
                    rfcStructureRange.SetValue("LOW_VALUE", "X");
                    rfcStructureRange.SetValue("HIGH_VALUE", "");
                    rfcTableRange.Append(rfcStructureRange);



                    rfcStructureRange = rfcRepo.GetStructureMetadata("BAPI_ALM_ORDER_LISTHEAD_RANGES").CreateStructure();
                    rfcStructureRange.SetValue("FIELD_NAME", "SHOW_DOCUMENTS_IN_PROCESS");
                    rfcStructureRange.SetValue("SIGN", "I");
                    rfcStructureRange.SetValue("OPTION", "EQ");
                    rfcStructureRange.SetValue("LOW_VALUE", "X");
                    rfcStructureRange.SetValue("HIGH_VALUE", "");
                    rfcTableRange.Append(rfcStructureRange);


                    rfcStructureRange = rfcRepo.GetStructureMetadata("BAPI_ALM_ORDER_LISTHEAD_RANGES").CreateStructure();
                    rfcStructureRange.SetValue("FIELD_NAME", "SHOW_COMPLETED_DOCUMENTS");
                    rfcStructureRange.SetValue("SIGN", "I");
                    rfcStructureRange.SetValue("OPTION", "EQ");
                    rfcStructureRange.SetValue("LOW_VALUE", "X");
                    rfcStructureRange.SetValue("HIGH_VALUE", "");
                    rfcTableRange.Append(rfcStructureRange);


                    rfcStructureRange = rfcRepo.GetStructureMetadata("BAPI_ALM_ORDER_LISTHEAD_RANGES").CreateStructure();
                    rfcStructureRange.SetValue("FIELD_NAME", "SHOW_HISTORICAL_DOCUMENTS");
                    rfcStructureRange.SetValue("SIGN", "I");
                    rfcStructureRange.SetValue("OPTION", "EQ");
                    rfcStructureRange.SetValue("LOW_VALUE", "X");
                    rfcStructureRange.SetValue("HIGH_VALUE", "");
                    rfcTableRange.Append(rfcStructureRange);



                    rfcFunc.SetValue("IT_RANGES", rfcTableRange);

                    rfcFunc.Invoke(rfc);

                    IRfcTable rfcReturns = rfcFunc["ET_RESULT"].GetTable();

                    dsData.Tables.Add(ConvertToTable(rfcReturns));

                    message = "Dados processados com sucesso !";

                    //RfcSessionManager.EndContext(rfc);
                }

                catch (Exception ex)
                {
                    message = ex.Message;
                    //RfcSessionManager.EndContext(rfc);
                }


            }

            return dsData;
        }

        public DataSet BAPI_ALM_ORDEROPER_GETLIST(List<string> orders, ref string message)
        {
            RfcDestination rfc = null;
            var dsData = new DataSet();
            var connectSAP = new ConnectSap();

            var Message = "";

            bool connect = connectSAP.Connect(Name, AppServerHost, SystemNumber, Client, User, Password, Language, PoolSize, ref rfc, ref Message);

            if (connect)
            {



                try
                {
                    //RfcSessionManager.BeginContext(rfc);

                    //RfcSessionManager.SessionChangeHandler x = new RfcSessionManager.SessionChangeHandler((z) => { });


                    RfcRepository rfcRepo = rfc.Repository;

                    IRfcFunction rfcFunc = rfcRepo.CreateFunction("BAPI_ALM_ORDEROPER_GET_LIST");

                    IRfcStructure rfcStructureParams = rfcFunc["DISPLAY_PARAMETERS"].GetStructure();
                    rfcStructureParams.SetValue("PAGELENGTH", "1000000");


                    IRfcTable rfcTableRange = rfcFunc["IT_RANGES"].GetTable();

                    IRfcStructure rfcStructureRange = null;

                    foreach (string order in orders)
                    {
                        rfcStructureRange = rfcRepo.GetStructureMetadata("BAPI_ALM_ORDER_LISTOPER_RANGES").CreateStructure();
                        rfcStructureRange.SetValue("FIELD_NAME", "OPTIONS_FOR_ORDERID");
                        rfcStructureRange.SetValue("SIGN", "");
                        rfcStructureRange.SetValue("OPTION", "");
                        rfcStructureRange.SetValue("LOW_VALUE", order);
                        rfcStructureRange.SetValue("HIGH_VALUE", "");
                        rfcTableRange.Insert(rfcStructureRange);
                    }



                    rfcFunc.SetValue("IT_RANGES", rfcTableRange);

                    rfcFunc.Invoke(rfc);

                    IRfcTable rfcReturns = rfcFunc["ET_RESULT"].GetTable();

                    dsData.Tables.Add(ConvertToTable(rfcReturns));

                    message = "Dados processados com sucesso !";


                    //RfcSessionManager.EndContext(rfc);
                }

                catch (Exception ex)
                {
                    message = ex.Message;
                    //RfcSessionManager.EndContext(rfc);
                }


            }

            return dsData;
        }

        public DataSet BAPI_ALM_NOTIF_GET_DETAIL(string noteNumber, ref string message)
        {
            RfcDestination rfc = null;
            var dsData = new DataSet();
            var connectSAP = new ConnectSap();

            var Message = "";

            bool connect = connectSAP.Connect(Name, AppServerHost, SystemNumber, Client, User, Password, Language, PoolSize, ref rfc, ref Message);

            if (connect)
            {



                try
                {
                    //RfcSessionManager.BeginContext(rfc);

                    //RfcSessionManager.SessionChangeHandler x = new RfcSessionManager.SessionChangeHandler((z) => { });

                    RfcRepository rfcRepo = rfc.Repository;

                    IRfcFunction rfcFunc = rfcRepo.CreateFunction("BAPI_ALM_NOTIF_GET_DETAIL");

                    rfcFunc.SetValue("NUMBER", noteNumber.PadLeft(12, '0'));


                    rfcFunc.Invoke(rfc);


                    IRfcStructure rfcStructureHeader = rfcFunc["NOTIFHEADER_EXPORT"].GetStructure();
                    IRfcStructure rfcStructureHDTEXT = rfcFunc["NOTIFHDTEXT"].GetStructure();

                    IRfcTable rfcItem = rfcFunc["NOTITEM"].GetTable();
                    IRfcTable rfcCaus = rfcFunc["NOTIFCAUS"].GetTable();
                    IRfcTable rfcActv = rfcFunc["NOTIFACTV"].GetTable();
                    IRfcTable rfcTask = rfcFunc["NOTIFTASK"].GetTable();
                    IRfcTable rfcPartnr = rfcFunc["NOTIFPARTNR"].GetTable();


                    dsData.Tables.Add(ConvertStructureToTable(rfcStructureHeader));

                    dsData.Tables.Add(ConvertStructureToTable(rfcStructureHDTEXT));

                    dsData.Tables.Add(ConvertToTable(rfcItem));
                    dsData.Tables.Add(ConvertToTable(rfcCaus));
                    dsData.Tables.Add(ConvertToTable(rfcActv));
                    dsData.Tables.Add(ConvertToTable(rfcTask));
                    dsData.Tables.Add(ConvertToTable(rfcPartnr));

                    message = "Dados processados com sucesso !";

                    //RfcSessionManager.EndContext(rfc);
                }

                catch (Exception ex)
                {
                    message = ex.Message;
                    //RfcSessionManager.EndContext(rfc);
                }


            }

            return dsData;
        }

        public DataSet BAPI_ALM_NOTIF_LIST_PLANGROUP(string plant, string plantGroup, DateTime dateNotification, bool complete, List<string> columns, ref string message)
        {
            RfcDestination rfc = null;
            var dsData = new DataSet();
            var connectSAP = new ConnectSap();

            var Message = "";

            bool connect = connectSAP.Connect(Name, AppServerHost, SystemNumber, Client, User, Password, Language, PoolSize, ref rfc, ref Message);

            if (connect)
            {

                try
                {
                    //RfcSessionManager.BeginContext(rfc);

                    //RfcSessionManager.SessionChangeHandler x = new RfcSessionManager.SessionChangeHandler((z) => { });

                    RfcRepository rfcRepo = rfc.Repository;

                    IRfcFunction rfcFunc = rfcRepo.CreateFunction("BAPI_ALM_NOTIF_LIST_PLANGROUP");

                    rfcFunc.SetValue("PLANPLANT", plant);

                    rfcFunc.SetValue("PLANGROUP", plantGroup);

                    rfcFunc.SetValue("NOTIFICATION_DATE", dateNotification.ToString("yyyyMMdd"));

                    rfcFunc.SetValue("COMPLETE", complete ? "X" : "");


                    rfcFunc.Invoke(rfc);


                    IRfcTable rfcReturns = rfcFunc["NOTIFICATION"].GetTable();

                    dsData.Tables.Add(ConvertToTable(rfcReturns, columns));


                    message = "Dados processados com sucesso !";

                    //RfcSessionManager.EndContext(rfc);
                }

                catch (Exception ex)
                {
                    message = ex.Message;
                    //RfcSessionManager.EndContext(rfc);
                }


            }

            return dsData;
        }

        public DataSet BAPI_ALM_ORDER_GETDETAIL(string orderNumber, ref string message)
        {
            RfcDestination rfc = null;
            var dsData = new DataSet();
            var connectSAP = new ConnectSap();

            var Message = "";

            bool connect = connectSAP.Connect(Name, AppServerHost, SystemNumber, Client, User, Password, Language, PoolSize, ref rfc, ref Message);

            if (connect)
            {
                try
                {

                    //RfcSessionManager.BeginContext(rfc);

                    //RfcSessionManager.SessionChangeHandler x = new RfcSessionManager.SessionChangeHandler((z) => { });

                    RfcRepository rfcRepo = rfc.Repository;
                    IRfcFunction rfcFunc = rfcRepo.CreateFunction("BAPI_ALM_ORDER_GET_DETAIL");

                    rfcFunc.SetValue("NUMBER", orderNumber);

                    rfcFunc.Invoke(rfc);

                    IRfcStructure rfcStructure = rfcFunc["ES_HEADER"].GetStructure();

                    dsData.Tables.Add(ConvertStructureToTable(rfcStructure));


                    //RfcSessionManager.EndContext(rfc);
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    // RfcSessionManager.EndContext(rfc);
                }
            }


            return dsData;
        }

        public DataSet BAPI_EQMT_DETAIL(string idEquipment, ref string message)
        {
            RfcDestination rfc = null;
            var dsData = new DataSet();
            var connectSAP = new ConnectSap();

            try
            {

                var Message = "";

                bool connect = connectSAP.Connect(Name, AppServerHost, SystemNumber, Client, User, Password, Language, PoolSize, ref rfc, ref Message);

                if (connect)
                {

                    //RfcSessionManager.BeginContext(rfc);

                    //RfcSessionManager.SessionChangeHandler x = new RfcSessionManager.SessionChangeHandler((z) => { });

                    RfcRepository rfcRepo = rfc.Repository;

                    IRfcFunction rfcFunc = rfcRepo.CreateFunction("BAPI_EQMT_DETAIL");

                    rfcFunc.SetValue("EQUIPMENT", idEquipment);

                    rfcFunc.Invoke(rfc);

                    IRfcStructure rfcReturns = rfcFunc["EQUIMASTER"].GetStructure();


                    dsData.Tables.Add(ConvertStructureToTable(rfcReturns));

                    //RfcSessionManager.EndContext(rfc);


                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                //RfcSessionManager.EndContext(rfc);
            }

            return dsData;
        }

        public DataSet BAPI_FUNCLOC_GETLIST(string plant, string plantGroup, ref string message)
        {
            RfcDestination rfc = null;
            var dsData = new DataSet();
            var connectSAP = new ConnectSap();

            var Message = "";

            bool connect = connectSAP.Connect(Name, AppServerHost, SystemNumber, Client, User, Password, Language, PoolSize, ref rfc, ref Message);

            if (connect)
            {


                try
                {


                    RfcRepository rfcRepo = rfc.Repository;

                    IRfcFunction rfcFunc = rfcRepo.CreateFunction("BAPI_FUNCLOC_GETLIST");


                    IRfcTable rfcTablePlant = rfcFunc["PLANPLANT_RA"].GetTable();

                    IRfcStructure rfcStructurePlant = null;

                    rfcStructurePlant = rfcRepo.GetStructureMetadata("BAPI_ITOB_SEL_PLANPLANT").CreateStructure();
                    rfcStructurePlant.SetValue("SIGN", "");
                    rfcStructurePlant.SetValue("OPTION", "");
                    rfcStructurePlant.SetValue("LOW", plant);
                    rfcStructurePlant.SetValue("HIGH", "");
                    rfcTablePlant.Insert(rfcStructurePlant);



                    IRfcTable rfcTablePlantGroup = rfcFunc["PLANGROUP_RA"].GetTable();

                    IRfcStructure rfcStructurePlantGroup = null;


                    rfcStructurePlantGroup = rfcRepo.GetStructureMetadata("BAPI_ITOB_SEL_PLANGROUP").CreateStructure();
                    rfcStructurePlantGroup.SetValue("SIGN", "");
                    rfcStructurePlantGroup.SetValue("OPTION", "");
                    rfcStructurePlantGroup.SetValue("LOW", plantGroup);
                    rfcStructurePlantGroup.SetValue("HIGH", "");
                    rfcTablePlantGroup.Insert(rfcStructurePlantGroup);



                    rfcFunc.Invoke(rfc);

                    IRfcTable rfcReturns = rfcFunc["FUNCLOC_LIST"].GetTable();

                    dsData.Tables.Add(ConvertToTable(rfcReturns));

                    message = "Dados processados com sucesso !";

                }

                catch (Exception ex)
                {
                    message = ex.Message;

                }

            }

            return dsData;
        }


        private DataTable ConvertToTable(IRfcTable _rfcTable)
        {

            var dtTable = new DataTable();

            for (int i = 0; i < _rfcTable.ElementCount; i++)
            {
                RfcElementMetadata metadata = _rfcTable.GetElementMetadata(i);
                dtTable.Columns.Add(metadata.Name);
            }

            foreach (IRfcStructure r in _rfcTable)
            {

                var dr = dtTable.NewRow();

                for (int i = 0; i < _rfcTable.ElementCount; i++)
                {
                    RfcElementMetadata metadata = _rfcTable.GetElementMetadata(i);

                    if (metadata.DataType == RfcDataType.BCD && metadata.Name == "ABC")
                        dr[i] = r.GetInt(metadata.Name);
                    else
                        dr[i] = r.GetString(metadata.Name);

                }

                dtTable.Rows.Add(dr);

            }

            return dtTable;
        }

        private DataTable ConvertToTable(IRfcTable _rfcTable, List<string> columns)
        {

            var dtTable = new DataTable();


            foreach (var col in columns)
            {
                dtTable.Columns.Add(col);
            }

            foreach (IRfcStructure r in _rfcTable)
            {
                var dr = dtTable.NewRow();

                foreach (var col in columns)
                {
                    RfcElementMetadata metadata = _rfcTable.GetElementMetadata(col);

                    if (metadata.DataType == RfcDataType.BCD && metadata.Name == "ABC")
                        dr[col] = r.GetInt(metadata.Name);
                    else
                        dr[col] = r.GetString(metadata.Name);
                }



                dtTable.Rows.Add(dr);

            }

            return dtTable;
        }

        private DataTable ConvertStructureToTable(IRfcStructure _rfcStructure)
        {

            var dtTable = new DataTable();

            for (int i = 0; i < _rfcStructure.ElementCount; i++)
            {
                RfcElementMetadata metadata = _rfcStructure.GetElementMetadata(i);
                dtTable.Columns.Add(metadata.Name);
            }



            var dr = dtTable.NewRow();

            for (int i = 0; i < _rfcStructure.ElementCount; i++)
            {
                RfcElementMetadata metadata = _rfcStructure.GetElementMetadata(i);

                if (metadata.DataType == RfcDataType.BCD && metadata.Name == "ABC")
                    dr[i] = _rfcStructure.GetInt(metadata.Name);
                else
                    dr[i] = _rfcStructure.GetString(metadata.Name);

            }

            dtTable.Rows.Add(dr);



            return dtTable;
        }

        private DataTable ConvertToTable(IRfcTable _rfcTable, List<string> columns, char delimiter)
        {

            var dtTable = new DataTable();


            foreach (var col in columns)
            {
                dtTable.Columns.Add(col);
            }

            foreach (IRfcStructure r in _rfcTable)
            {
                var dr = dtTable.NewRow();

                RfcElementMetadata metadata = _rfcTable.GetElementMetadata("WA");

                string[] result = r.GetString(metadata.Name).Split(delimiter);

                int index = 0;
                foreach (var col in columns)
                {
                    dr[col] = result[index];

                    index++;
                }

                dtTable.Rows.Add(dr);

            }

            return dtTable;
        }
    }
}
