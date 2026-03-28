using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain;
using Tenaris.Confab.SGM.Domain.Entities;
using Dapper;
using System.Data.SqlClient;
using Tenaris.Confab.SAP.Connector;
using System.Data;

namespace Tenaris.Confab.SGM.Repositories
{
    public class EquipmentPlantGroupRepositorie : IEquipmentPlantGroupRepositorie
    {

        public EquipmentPlantGroup GetEquipmentPlantGroup(string sheet)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {


                    return sqlConn.Query<EquipmentPlantGroup>(@"
                        select TOP 1 e.* 
                        from [Plant].[EquipmentPlantGroup] e                                        
                        where e.Sheet = @Sheet", new { Sheet = sheet }).SingleOrDefault();


                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }

        public object InsertEquipmentPlantGroup(List<string> planPlants, List<string> plantGroups)
        {

            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {


                    var listEquipmentsPlantGroup = GetEquipmentsPlantGroupSAP(planPlants, plantGroups);

                    foreach (var equipment in listEquipmentsPlantGroup)
                    {

                        if (equipment.idEquipmentPlantGroup == null)
                        {

                            sqlConn.Execute(@"
                                Insert into [Plant].[EquipmentPlantGroup] values 
                                (@Sheet, @Name, @cdPlantGroup, @Active, @InsDateTime, @UpdDateTime, @cdUser, @Like)",
                            new
                            {

                                Sheet = equipment.Sheet,
                                Name = equipment.Name,
                                cdPlantGroup = equipment.cdPlantGroup,
                                Active = true,
                                InsDateTime = DateTime.Now,
                                UpdDateTime = DateTime.Now,
                                cdUser = equipment.cdUser,
                                Like = 0,

                            });
                        }
                        else
                        {
                            sqlConn.Execute(@"
                                        update [Plant].[EquipmentPlantGroup] set  
                                        Sheet = @Sheet, Name = @Name, cdPlantGroup = @cdPlantGroup, Active = @Active, InsDateTime = @InsDateTime, UpdDateTime = @UpdDateTime, cdUser = @cdUser, [Like] = @Like where idEquipmentPlantGroup = @idEquipmentPlantGroup",
                                           new
                                           {
                                               idEquipmentPlantGroup = equipment.idEquipmentPlantGroup,
                                               Sheet = equipment.Sheet,
                                               Name = equipment.Name,
                                               cdPlantGroup = equipment.cdPlantGroup,
                                               Active = true,
                                               InsDateTime = equipment.InsDateTime,
                                               UpdDateTime = DateTime.Now,
                                               cdUser = equipment.cdUser,
                                               Like = 0,

                                           });
                        }
                    }

                    return new { status = true, message = "Dados inseridos com sucesso !" };

                }
            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }


        }


        public List<EquipmentPlantGroup> GetEquipmentsPlantGroupSAP(List<string> planPlants, List<string> plantGroups)
        {

            try
            {

                var listEquipmentsPlantGroup = new List<EquipmentPlantGroup>();
                var plantGroupRepo = new PlantGroupRepositorie();

                var funcSAP =
                   new FunctionsSap(
                       Properties.Settings.Default.Name,
                       Properties.Settings.Default.Sap_Host,
                       Properties.Settings.Default.Sap_System,
                       Properties.Settings.Default.Sap_Client,
                       Properties.Settings.Default.Sap_User,
                       Properties.Settings.Default.Sap_Password,
                       Properties.Settings.Default.Sap_Language,
                       Properties.Settings.Default.Sap_PoolSize);
                

                foreach (var plant in planPlants)
                {

                    var message = "";

                    foreach (var plantGroup in plantGroups)
                    {
                        var plantGroupObj = plantGroupRepo.GetPlantGroup(plantGroup);

                        if (plantGroupObj != null)
                        {
                            var dsEquipmentsPlantGroup = funcSAP.BAPI_FUNCLOC_GETLIST(plant, plantGroup, ref message);

                            if (dsEquipmentsPlantGroup.Tables.Count > 0)
                            {
                                

                                foreach (DataRow row in dsEquipmentsPlantGroup.Tables[0].Rows)
                                {
                                  
                                       
                                        var equip = GetEquipmentPlantGroup(row["FUNCTLOCATION"].ToString());

                                        if (equip == null)
                                            listEquipmentsPlantGroup.Add(new EquipmentPlantGroup
                                            {
                                                Sheet = row["FUNCTLOCATION"].ToString(),
                                                Name = row["DESCRIPT"].ToString(),
                                                Active = true,
                                                cdUser = "AUTMPIN",
                                                InsDateTime = DateTime.Now,
                                                UpdDateTime = DateTime.Now,
                                                cdPlantGroup = plantGroupObj.idPlantGroup,

                                            });
                                        else
                                            listEquipmentsPlantGroup.Add(new EquipmentPlantGroup
                                            {
                                                Sheet = row["FUNCTLOCATION"].ToString(),
                                                Name = row["DESCRIPT"].ToString(),
                                                Active = true,
                                                cdUser = "AUTMPIN",
                                                InsDateTime = equip.InsDateTime,
                                                UpdDateTime = DateTime.Now,
                                                cdPlantGroup = plantGroupObj.idPlantGroup,
                                                idEquipmentPlantGroup = equip.idEquipmentPlantGroup

                                            });
                                    
                                }

                            }
                        }
                    }

                }

                return listEquipmentsPlantGroup;

            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }




        }


        /// <summary>
        /// index = 2 - Planta
        /// index = 3 - Area
        /// index >= 4 - Equipamentos
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<EquipmentPlantGroup> GetEquipmentsPlantGroup(int index, string sheedMan)
        {
            try
            {

                var sql = "";
                var andWhere = !String.IsNullOrEmpty(sheedMan) ? @" and e.Sheet like @sheedMan+'%' " : "";
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    //if (index >= 4)
                    //    sql = @"select e.*, p.*  
                    //                    from [Plant].[VwEquipmentPlantGroup] e 
                    //                    inner join [Plant].[PlantGroup] p on p.idPlantGroup = e.cdPlantGroup                                       
                    //                where  (select count(*) from [Common].[FTSplit]('-', e.Sheet)) >= @index"
                    //          + andWhere + " order by e.Name"; 
                    //else
                    sql = @"select e.*, p.* 
                                    from [Plant].[VwEquipmentPlantGroup] e         
                                    inner join [Plant].[PlantGroup] p on p.idPlantGroup = e.cdPlantGroup                               
                                where  (select count(*) from [Common].[FTSplit]('-', e.Sheet)) = @index"
                          + andWhere + " order by e.Name";


                    return !String.IsNullOrEmpty(sheedMan) ?
                        sqlConn.Query<EquipmentPlantGroup, PlantGroup, EquipmentPlantGroup>(sql,
                         (e, p) =>
                         {
                             EquipmentPlantGroup equipmentPlantGroup;
                             equipmentPlantGroup = e;
                             equipmentPlantGroup.PlantGroup = p;

                             return equipmentPlantGroup;
                         },
                         new { index = index, sheedMan = sheedMan },
                         splitOn: "idEquipmentPlantGroup,idPlantGroup"

                        ).ToList() :
                        sqlConn.Query<EquipmentPlantGroup, PlantGroup, EquipmentPlantGroup>(sql,
                        (e, p) =>
                        {
                            EquipmentPlantGroup equipmentPlantGroup;
                            equipmentPlantGroup = e;
                            equipmentPlantGroup.PlantGroup = p;

                            return equipmentPlantGroup;
                        },
                         new { index = index },
                         splitOn: "idEquipmentPlantGroup,idPlantGroup"
                        ).ToList();

                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }

        }

    }
}
