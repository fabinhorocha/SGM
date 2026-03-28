using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tenaris.Confab.SAP.Connector;
using System.Data;
using System.Collections.Generic;
using Tenaris.Confab.SGM.Services;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethodAviso()
        {
            var functionSAP = new FunctionsSap("TEP", "saptep52.tenaris.ot","52","800", "CFBPMM2", "Cfbauto800", "PT","10");
            //var functionSAP = new FunctionsSap("TIQ", "saptiq96.tenaris.ot", "96", "800", "TERCRD", "crd020", "PT", "10");
            //var functionSAP = new FunctionsSap("TIQ", "saptiq96.tenaris.ot", "96", "800", "Z_CFBPMM2", "Inicio12", "PT", "10");
            var message = "";
            var number = "'000013968402'";
            //var number = "'000014132484'";


            // var data = functionSAP.BAPI_ALM_NOTIF_GET_DETAIL(number, ref message);

            var delimiters = '|';

            //Search Document
            var fields = new List<RFC_DB_FLD>();
            fields.Add(new RFC_DB_FLD { FIELDNAME = "MANDT", FIELDTEXT = "MANDT", LENGTH = 3, TYPE = INTTYPE.C });
            fields.Add(new RFC_DB_FLD { FIELDNAME = "/TENR/AVISOFA", FIELDTEXT = "/TENR/AVISOFA", LENGTH = 12, TYPE = INTTYPE.C });
            fields.Add(new RFC_DB_FLD { FIELDNAME = "/TENR/AVISOM2", FIELDTEXT = "/TENR/AVISOM2", LENGTH = 12, TYPE = INTTYPE.C });
            fields.Add(new RFC_DB_FLD { FIELDNAME = "/TENR/REF", FIELDTEXT = "/TENR/REF", LENGTH = 1, TYPE = INTTYPE.C });
            fields.Add(new RFC_DB_FLD { FIELDNAME = "OBJNR", FIELDTEXT = "OBJNR", LENGTH = 22, TYPE = INTTYPE.C });

            var filters = new List<string>();
            filters.Add("/TENR/AVISOFA EQ " + number);

            var dataDoc = functionSAP.ReadTable("/TENR/T_PM_FA_M2", fields, filters, delimiters, ref message);

            var notesRef = new List<string>();
            if (dataDoc.Tables.Count > 0)
            {
                foreach (DataRow row in dataDoc.Tables[0].Rows)
                {
                    var value = row["/TENR/AVISOM2"].ToString();
                    if (!String.IsNullOrEmpty(value))
                        notesRef.Add(value);
                }
            }

        }

        [TestMethod]
        public void TestMethodNota()
        {
            var functionSAP = new FunctionsSap("TEP", "saptep52.tenaris.ot","52","800", "CFBPMM2", "Cfbauto800", "PT","10");
            //var functionSAP = new FunctionsSap("TIQ", "saptiq96.tenaris.ot", "96", "800", "TERCRD", "crd020", "PT", "10");
            //var functionSAP = new FunctionsSap("TIQ", "saptiq96.tenaris.ot", "96", "800", "Z_CFBPMM2", "Inicio12", "PT", "10");
            var message = "";
            var locationCenter = "CT10";
            var dtStart = "20181030";
            var dtEnd = "20181030";
            var typeNote = "M2";
            
            var delimiters = '|';

            //Search Notes
            var fields = new List<RFC_DB_FLD>();
            fields.Add(new RFC_DB_FLD { FIELDNAME = "IWERK", FIELDTEXT = "IWERK", LENGTH = 4, TYPE = INTTYPE.C }); //Plant
            fields.Add(new RFC_DB_FLD { FIELDNAME = "QMNUM", FIELDTEXT = "QMNUM", LENGTH = 12, TYPE = INTTYPE.C }); //idNote
            fields.Add(new RFC_DB_FLD { FIELDNAME = "QMTXT", FIELDTEXT = "QMTXT", LENGTH = 40, TYPE = INTTYPE.C }); //Description
            fields.Add(new RFC_DB_FLD { FIELDNAME = "TPLNR", FIELDTEXT = "TPLNR", LENGTH = 30, TYPE = INTTYPE.C }); //Location
            fields.Add(new RFC_DB_FLD { FIELDNAME = "AUSVN", FIELDTEXT = "AUSVN", LENGTH = 8, TYPE = INTTYPE.D }); //dateStart
            fields.Add(new RFC_DB_FLD { FIELDNAME = "AUSBS", FIELDTEXT = "AUSBS", LENGTH = 8, TYPE = INTTYPE.D }); //dateEnd
            fields.Add(new RFC_DB_FLD { FIELDNAME = "BEZDT", FIELDTEXT = "BEZDT", LENGTH = 8, TYPE = INTTYPE.D }); //dateRef
            fields.Add(new RFC_DB_FLD { FIELDNAME = "INGRP", FIELDTEXT = "INGRP", LENGTH = 3, TYPE = INTTYPE.C }); //PlantGroup
            fields.Add(new RFC_DB_FLD { FIELDNAME = "QMART", FIELDTEXT = "QMART", LENGTH = 2, TYPE = INTTYPE.C }); //Type            
            fields.Add(new RFC_DB_FLD { FIELDNAME = "ERNAM", FIELDTEXT = "ERNAM", LENGTH = 12, TYPE = INTTYPE.C }); //cdUser Create
            fields.Add(new RFC_DB_FLD { FIELDNAME = "AENAM", FIELDTEXT = "AENAM", LENGTH = 12, TYPE = INTTYPE.C }); //cdUser Update
            fields.Add(new RFC_DB_FLD { FIELDNAME = "ERDAT", FIELDTEXT = "ERDAT", LENGTH = 8, TYPE = INTTYPE.D }); //InsDateTime 
            fields.Add(new RFC_DB_FLD { FIELDNAME = "AEDAT", FIELDTEXT = "AEDAT", LENGTH = 8, TYPE = INTTYPE.D }); //UpdDatetime

            fields.Add(new RFC_DB_FLD { FIELDNAME = "AUSWK", FIELDTEXT = "AUSWK", LENGTH = 1, TYPE = INTTYPE.C }); //Effect Operation

            fields.Add(new RFC_DB_FLD { FIELDNAME = "ARBPL", FIELDTEXT = "ARBPL", LENGTH = 8, TYPE = INTTYPE.C }); //Obter CenterCost

            fields.Add(new RFC_DB_FLD { FIELDNAME = "OBJNR", FIELDTEXT = "OBJNR", LENGTH = 22, TYPE = INTTYPE.C }); //Obter Status

            var filters = new List<string>();
            filters.Add("IWERK EQ '" + locationCenter + "' AND ");
            filters.Add("QMART EQ '" + typeNote + "' AND ");
            filters.Add("ERDAT GE '" + dtStart + "' AND ERDAT LE '" + dtEnd+"'");

            var dataDoc = functionSAP.ReadTable("VIQMEL", fields, filters, delimiters, ref message);

            var notesRef = new List<string>();
            if (dataDoc.Tables.Count > 0)
            {
                foreach (DataRow row in dataDoc.Tables[0].Rows)
                {

                    if(row["QMNUM"].ToString() == "000014093533")
                    {
                        var test = "";
                    }
                }
            }

        }


        /// <summary>
        /// Retorna Centro de Trabalho
        /// </summary>
        [TestMethod]
        public void TestMethodWorkCenter()
        {
            var functionSAP = new FunctionsSap("TEP", "saptep52.tenaris.ot","52","800", "CFBPMM2", "Cfbauto800", "PT","10");
            //var functionSAP = new FunctionsSap("TIQ", "saptiq96.tenaris.ot", "96", "800", "TERCRD", "crd020", "PT", "10");
           // var functionSAP = new FunctionsSap("TIQ", "saptiq96.tenaris.ot", "96", "800", "Z_CFBPMM2", "Inicio12", "PT", "10");
            var message = "";
            var objNumber = "10002409";

            var delimiters = '|';

            var fields = new List<RFC_DB_FLD>();
            fields.Add(new RFC_DB_FLD { FIELDNAME = "OBJID", FIELDTEXT = "OBJID", LENGTH = 8, TYPE = INTTYPE.C });
            fields.Add(new RFC_DB_FLD { FIELDNAME = "ARBPL", FIELDTEXT = "ARBPL", LENGTH = 8, TYPE = INTTYPE.C });
           

            var filters = new List<string>();
            filters.Add("OBJID EQ '" + objNumber+ "'");

            var data = functionSAP.ReadTable("CRHD", fields, filters, delimiters, ref message);

            var workCenter = "";
            if (data.Tables.Count > 0)
            {
                foreach (DataRow row in data.Tables[0].Rows)
                {
                    workCenter = row["ARBPL"].ToString();
                }

            }
        }


        /// <summary>
        /// Retorna status
        /// </summary>
        [TestMethod]
        public void TestMethodStatusNota()
        {
            var functionSAP = new FunctionsSap("TEP", "saptep52.tenaris.ot","52","800", "CFBPMM2", "Cfbauto800", "PT","10");
            //var functionSAP = new FunctionsSap("TIQ", "saptiq96.tenaris.ot", "96", "800", "TERCRD", "crd020", "PT", "10");
            //var functionSAP = new FunctionsSap("TIQ", "saptiq96.tenaris.ot", "96", "800", "Z_CFBPMM2", "Inicio12", "PT", "10");
            var message = "";
            var objNumber = "QM000014093533";

            var delimiters = '|';

            var fields = new List<RFC_DB_FLD>();
            fields.Add(new RFC_DB_FLD { FIELDNAME = "MANDT", FIELDTEXT = "MANDT", LENGTH = 3, TYPE = INTTYPE.C });
            fields.Add(new RFC_DB_FLD { FIELDNAME = "OBJNR", FIELDTEXT = "OBJNR", LENGTH = 22, TYPE = INTTYPE.C });
            fields.Add(new RFC_DB_FLD { FIELDNAME = "STAT", FIELDTEXT = "STAT", LENGTH = 5, TYPE = INTTYPE.C });
            fields.Add(new RFC_DB_FLD { FIELDNAME = "INACT", FIELDTEXT = "INACT", LENGTH = 1, TYPE = INTTYPE.C });
            fields.Add(new RFC_DB_FLD { FIELDNAME = "CHGNR", FIELDTEXT = "CHGNR", LENGTH = 3, TYPE = INTTYPE.C });

            var filters = new List<string>();
            filters.Add("OBJNR EQ '" + objNumber + "' AND INACT EQ '' ");

            var dataStatus = functionSAP.ReadTable("JEST", fields, filters, delimiters, ref message);

            var status = "";
            if (dataStatus.Tables.Count > 0)
            {
                foreach (DataRow row in dataStatus.Tables[0].Rows)
                {
                    fields = new List<RFC_DB_FLD>();
                    fields.Add(new RFC_DB_FLD { FIELDNAME = "ISTAT", FIELDTEXT = "ISTAT", LENGTH = 5, TYPE = INTTYPE.C });
                    fields.Add(new RFC_DB_FLD { FIELDNAME = "SPRAS", FIELDTEXT = "SPRAS", LENGTH = 1, TYPE = INTTYPE.C });
                    fields.Add(new RFC_DB_FLD { FIELDNAME = "TXT04", FIELDTEXT = "TXT04", LENGTH = 5, TYPE = INTTYPE.C });
                    fields.Add(new RFC_DB_FLD { FIELDNAME = "TXT30", FIELDTEXT = "TXT30", LENGTH = 30, TYPE = INTTYPE.C });


                    filters = new List<string>();
                    filters.Add("ISTAT EQ '" + row["STAT"].ToString() + "' AND SPRAS EQ 'P' ");

                    var dataDescStatus = functionSAP.ReadTable("TJ02T", fields, filters, delimiters, ref message);

                    var listStatus = new List<string>();
                   

                    foreach (DataRow row2 in dataDescStatus.Tables[0].Rows)
                    {
                        listStatus.Add(row2["TXT04"].ToString());
                        //status = dataDescStatus.Tables[0].Rows.Count > 1 ? status + " " + row2["TXT04"].ToString() : row2["TXT04"].ToString();
                    }

                    var result = String.Join(" ", listStatus).ToString();
                }

            }
        }


        /// <summary>
        /// Retorna status
        /// </summary>
        [TestMethod]
        public void TestMethodGetLocation()
        {
            //var functionSAP = new FunctionsSap("TEP", "saptep52.tenaris.ot","52","800", "CFBPMM2", "Cfbauto800", "PT","10");
            //var functionSAP = new FunctionsSap("TIQ", "saptiq96.tenaris.ot", "96", "800", "TERCRD", "crd020", "PT", "10");
            //var message = "";

            var listPlants = new List<string>();
            var listGroups = new List<string>();

            listPlants.Add("CT10");
            listGroups.Add("CFV");
            listGroups.Add("CF6");

            //var data = functionSAP.BAPI_FUNCLOC_GETLIST(listPlants, listGroups, ref message);


            //if (data.Tables.Count > 0)
            //{
            //    foreach (DataRow row in data.Tables[0].Rows)
            //    {

            //    }

            //}

            var equipmentPlantGroupService = new EquipmentPlantGroupService();
            equipmentPlantGroupService.InsertEquipmentsPlantGroup(listPlants, listGroups);

        }

        [TestMethod]
        public void TestJoinString()
        {
            var listStatus = new List<string>();
            listStatus.Add("abc");
            listStatus.Add("fdfdffdfd");
            listStatus.Add("eeeeee");

           

            var result = String.Join(" ", listStatus);
        }


        [TestMethod]
        public void TestMethodGetPlantDataNote()
        {
            

            var plantDataService = new PlantDataService();
            var result  = plantDataService.GetPlantData(2,12, true);

        }

        [TestMethod]
        public void CreateDocument()
        {
            // var functionSAP = new FunctionsSap("TEQ", "sapteq51.tenaris.ot", "51", "800", "Z_GAM_TBAY", "Inicio01", "EN", "10");
            // var functionSAP = new FunctionsSap("TEP", "saptep52.tenaris.ot", "52", "800", "GAM_TBAY", "k.s1Q2lM", "EN", "10");
            var functionSAP = new FunctionsSap("TIQ", "saptiq96.tenaris.ot", "96", "800", "CFBVCO", "Cfbauto803", "EN", "10");
            //var functionSAP = new FunctionsSap("TEP", "saptep52.tenaris.ot","52","800", "CFBPMM2", "Cfbauto800", "PT","10");
            //var functionSAP = new FunctionsSap("TIQ", "saptiq96.tenaris.ot", "96", "800", "TERCRD", "crd020", "PT", "10");

            //var functionSAP = new FunctionsSap("TEP", "saptep52.tenaris.ot", "52", "800", "GAM-IT09", "T3nar1s23", "EN", "10");
            var message = "";

            var values = new Dictionary<string, string>();
            //GAM_CON_AVI000008286
            values.Add("P_IDSAP", "GAM_CON_GMP_12820");
            values.Add("P_IDDOC", "AV-F4-MSEC-BOMBA.pdf");
            values.Add("P_CLASE", "GA8");
            values.Add("P_DESCR", "PRED.AV - UH4-050-D03 - BOHI");
            values.Add("P_APLICA", "PDF");
            values.Add("P_PATH", @"\\CFBPIAP2\Sisdata\MANUTENCAO_PREDITIVA\");



            var data = functionSAP.ZPM_CREAR_DOCUMENTO(values, ref message);




            //var equipmentPlantGroupService = new EquipmentPlantGroupService();
            // equipmentPlantGroupService.InsertEquipmentsPlantGroup(listPlants, listGroups);

        }

        [TestMethod]
        public void CreateDocument2()
        {
            // var functionSAP = new FunctionsSap("TEQ", "sapteq51.tenaris.ot", "51", "800", "Z_GAM_TBAY", "Inicio01", "EN", "10");
            var functionSAP = new FunctionsSap("TEP", "saptep52.tenaris.ot", "52", "800", "GAM_TBAY", "k.s1Q2lM", "EN", "10");
            //var functionSAP = new FunctionsSap("TIQ", "saptiq96.tenaris.ot", "96", "800", "CFBVCO", "Cfbauto803", "EN", "10");
            //var functionSAP = new FunctionsSap("TEP", "saptep52.tenaris.ot","52","800", "CFBPMM2", "Cfbauto800", "PT","10");
            //var functionSAP = new FunctionsSap("TIQ", "saptiq96.tenaris.ot", "96", "800", "TERCRD", "crd020", "PT", "10");

            //var functionSAP = new FunctionsSap("TEP", "saptep52.tenaris.ot", "52", "800", "GAM-IT09", "T3nar1s23", "EN", "10");
            var message = "";

            var values = new Dictionary<string, string>();
            //GAM_CON_AVI000008286
            values.Add("P_IDSAP", "AV-F4-FB-VENTIL.DIR.MT");
            values.Add("P_IDDOC", "AV-F4-FB-VENTIL.DIR.MT.pdf");
            values.Add("P_CLASE", "GA8");
            values.Add("P_DESCR", "PRED.AV - CABEÇOTE DIREITO - MOEL VENTILADOR");
            values.Add("P_APLICA", "PDF");
            values.Add("P_PATH", @"\\CFBPIAP2\Sisdata\MANUTENCAO_PREDITIVA\");



            var data = functionSAP.ZPM_CREAR_DOCUMENTO(values, ref message);




            //var equipmentPlantGroupService = new EquipmentPlantGroupService();
            // equipmentPlantGroupService.InsertEquipmentsPlantGroup(listPlants, listGroups);

        }

        [TestMethod]
        public void CreateAviso()
        {
            //var functionSAP = new FunctionsSap("TEQ", "sapteq51.tenaris.ot", "51", "800", "Z_GAM_TBAY", "Inicio01", "EN", "10");
            //var functionSAP = new FunctionsSap("TEP", "saptep52.tenaris.ot", "52", "800", "GAM_TBAY", "k.s1Q2lM", "EN", "10");
            var functionSAP = new FunctionsSap("TIQ", "saptiq96.tenaris.ot", "96", "800", "CFBVCO", "Cfbauto803", "EN", "10");
            //var functionSAP = new FunctionsSap("TEP", "saptep52.tenaris.ot","52","800", "CFBPMM2", "Cfbauto800", "PT","10");
            //var functionSAP = new FunctionsSap("TIQ", "saptiq96.tenaris.ot", "96", "800", "TERCRD", "crd020", "PT", "10");
            var message = "";

            var values = new Dictionary<string, string>();


            values.Add("P_CLASE", "M8");
            values.Add("P_DESCR", "PRED.AV - UH4-050-D03 - BOHI");
            values.Add("P_EQUIPO", "000000000006990186");
            values.Add("P_FECHAINICIO", "2022-12-28");
            values.Add("P_HORAINICIO", "20:52:00");
            values.Add("P_PRIORIDAD", "1");
            values.Add("P_DOCUMENTO", "GAM_CON_GMP_12820");
            values.Add("P_OBSERVACION", "Created Doc Test");

            var data = functionSAP.ZPM_CREAR_AVISO_GAM2(12820, values, ref message);




            //var equipmentPlantGroupService = new EquipmentPlantGroupService();
            // equipmentPlantGroupService.InsertEquipmentsPlantGroup(listPlants, listGroups);

        }
    }
}
