using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain;
using Tenaris.Confab.SGM.Domain.Entities;
using Dapper;
using System.Data.SqlClient;
using System.Dynamic;
using System.Transactions;
using Tenaris.Confab.SAP.Connector;
using System.IO;

namespace Tenaris.Confab.SGM.Repositories
{
    public class PlantRepositorie : IPlantRepositorie
    {

      
        public List<Plant> GetPlants()
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    return sqlConn.Query<Plant>(@"
                       select p.*
                        from [Plant].[Plant] p                                                       
                        where p.Active = 1"                                            
                        ).ToList();                 
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }

        public List<PlantGroup> GetPlantsGroups()
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    var lookup = new Dictionary<int, PlantGroup>();

                    sqlConn.Query<PlantGroup, Plant, EquipmentPlantGroup, PlantGroup>(@"
                      Select pg.*, p.*, eq.*
                          from  [Plant].[PlantGroup] pg 
                          inner join[Plant].[PlantPlantGroup] ppg  on ppg.cdPlantGroup = pg.idPlantGroup 
                          inner join [Plant].[Plant] p on p.idPlant = ppg.cdPlant
                          left join [Plant].[EquipmentPlantGroup] eq on eq.cdPlantGroup = pg.idPlantGroup
                        where ppg.Active = 1 and pg.Active = 1 and p.Active = 1",
                        (pg, p, eq) =>
                        {
                            PlantGroup plantGroup;

                            if (!lookup.TryGetValue(pg.idPlantGroup, out plantGroup))
                            {

                                lookup.Add(pg.idPlantGroup, plantGroup = pg);

                            }



                            if (p != null)
                            {
                                if (plantGroup.Plants == null)
                                    plantGroup.Plants = new List<Plant>();

                                plantGroup.Plants.Add(p);
                            }

                            if (eq != null)
                            {
                                if (plantGroup.EquipmentsPlantGroup == null)
                                    plantGroup.EquipmentsPlantGroup = new List<EquipmentPlantGroup>();

                                plantGroup.EquipmentsPlantGroup.Add(eq);
                            }



                            return plantGroup;

                        },                        
                        splitOn: "idPlantGroup,idPlant,idEquipmentPlantGroup"
                        ).AsQueryable();


                    return lookup.Values.Cast<PlantGroup>().ToList();
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }

        public PlantGroup GetPlantGroup(int idPlantGroup)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    var lookup = new Dictionary<int, PlantGroup>();

                    sqlConn.Query<PlantGroup, Plant, EquipmentPlantGroup, PlantGroup>(@"
                      Select pg.*, p.*, eq.*
                          from  [Plant].[PlantGroup] pg 
                          inner join[Plant].[PlantPlantGroup] ppg  on ppg.cdPlantGroup = pg.idPlantGroup 
                          inner join [Plant].[Plant] p on p.idPlant = ppg.cdPlant
                          left join [Plant].[EquipmentPlantGroup] eq on eq.cdPlantGroup = pg.idPlantGroup
                        where ppg.Active = 1 and pg.Active = 1 and p.Active = 1 and pg.idPlantGroup = @idPlantGroup",
                        (pg, p, eq) =>
                        {
                            PlantGroup plantGroup;

                            if (!lookup.TryGetValue(pg.idPlantGroup, out plantGroup))
                            {

                                lookup.Add(pg.idPlantGroup, plantGroup = pg);

                            }



                            if (p != null)
                            {
                                if (plantGroup.Plants == null)
                                    plantGroup.Plants = new List<Plant>();

                                plantGroup.Plants.Add(p);
                            }

                            if (eq != null)
                            {
                                if (plantGroup.EquipmentsPlantGroup == null)
                                    plantGroup.EquipmentsPlantGroup = new List<EquipmentPlantGroup>();

                                plantGroup.EquipmentsPlantGroup.Add(eq);
                            }



                            return plantGroup;

                        },
                        new { idPlantGroup = idPlantGroup },
                        splitOn: "idPlantGroup,idPlant,idEquipmentPlantGroup"
                        ).AsQueryable();


                    return lookup.Values.Cast<PlantGroup>().SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }


        public PlantGroup GetPlantGroupBySheet(string plant)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    var lookup = new Dictionary<int, PlantGroup>();

                    sqlConn.Query<PlantGroup, Plant, EquipmentPlantGroup, PlantGroup>(@"
                      Select pg.*, p.*, eq.*
                          from  [Plant].[PlantGroup] pg 
                          inner join[Plant].[PlantPlantGroup] ppg  on ppg.cdPlantGroup = pg.idPlantGroup 
                          inner join [Plant].[Plant] p on p.idPlant = ppg.cdPlant
                          left join [Plant].[EquipmentPlantGroup] eq on eq.cdPlantGroup = pg.idPlantGroup
                        where ppg.Active = 1 and pg.Active = 1 and p.Active = 1 and pg.Sheet = @plant",
                        (pg, p, eq) =>
                        {
                            PlantGroup plantGroup;

                            if (!lookup.TryGetValue(pg.idPlantGroup, out plantGroup))
                            {

                                lookup.Add(pg.idPlantGroup, plantGroup = pg);

                            }



                            if (p != null)
                            {
                                if (plantGroup.Plants == null)
                                    plantGroup.Plants = new List<Plant>();

                                plantGroup.Plants.Add(p);
                            }

                            if (eq != null)
                            {
                                if (plantGroup.EquipmentsPlantGroup == null)
                                    plantGroup.EquipmentsPlantGroup = new List<EquipmentPlantGroup>();

                                plantGroup.EquipmentsPlantGroup.Add(eq);
                            }



                            return plantGroup;

                        },
                        new { plant = plant },
                        splitOn: "idPlantGroup,idPlant,idEquipmentPlantGroup"
                        ).AsQueryable();


                    return lookup.Values.Cast<PlantGroup>().SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }
        }
    }
}
