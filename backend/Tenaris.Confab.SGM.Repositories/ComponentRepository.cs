using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Tenaris.Confab.SGM.Domain.Entities;
using System.Data.SqlClient;
using System.Transactions;
using Tenaris.Confab.SGM.Domain;

namespace Tenaris.Confab.SGM.Repositories
{
    public class ComponentRepository : IComponentRepository
    {
        public List<Component> GetComponents()
        {
            try
            {
                var lookup = new Dictionary<int, Component>();

                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    sqlConn.Query<Component, OilType, OilManagementEquipment, OilManagementArea, OilManagementFactory, Component>(@"
                    SELECT c.*, ot.*, e.*, a.*, f.* 
                    FROM [OilManagement].[Component] c WITH(NOLOCK)
                    JOIN [OilManagement].[Equipment] e WITH(NOLOCK) ON e.idEquipment = c.idEquipment
                    JOIN [OilManagement].[Area] a WITH(NOLOCK) ON a.idArea = e.idArea
                    JOIN [OilManagement].[Factory] f WITH(NOLOCK) ON f.idFactory = a.idFactory
                    LEFT JOIN [OilManagement].[OilTypeByComponent] otc WITH(NOLOCK) ON otc.idComponent = c.idComponent
                    LEFT JOIN [OilManagement].[OilType] ot WITH(NOLOCK) ON otc.idOilType = ot.idOilType
                    ORDER BY c.Name
                    ",
                    (c, ot, e, a, f) =>
                    {
                        Component component;
                        OilType oilType;

                        if (!lookup.TryGetValue(c.idComponent, out component))
                        {
                            component = c;
                            component.Equipment = e;
                            component.Equipment.Area = a;
                            component.Equipment.Area.Factory = f;
                            component.EnabledOilTypes = new List<OilType>();

                            lookup.Add(c.idComponent, component);
                        }

                        oilType = lookup[c.idComponent].EnabledOilTypes.Where(x => x.idOilType == ot.idOilType).FirstOrDefault();
                        if (oilType == null && ot != null)
                        {
                            oilType = ot;
                            lookup[c.idComponent].EnabledOilTypes.Add(oilType);
                        }

                        return component;
                    },
                    splitOn: "idComponent,idOilType,idEquipment,idArea,idFactory"
                    ).AsQueryable();
                }

                return lookup.Values.Cast<Component>().ToList();
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }
        public Component GetComponent(int idComponent)
        {
            try
            {
                var lookup = new Dictionary<int, Component>();

                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    sqlConn.Query<Component, OilType, OilManagementEquipment, Component>(@"
                    SELECT c.*, ot.*, e.* 
                    FROM [OilManagement].[Component] c WITH(NOLOCK)
                    JOIN [OilManagement].[Equipment] e WITH(NOLOCK) ON e.idEquipment = c.idEquipment
                    LEFT JOIN [OilManagement].[OilTypeByComponent] otc WITH(NOLOCK) ON otc.idComponent = c.idComponent
                    LEFT JOIN [OilManagement].[OilType] ot WITH(NOLOCK) ON otc.idOilType = ot.idOilType
                    WHERE c.idComponent = @idComponent",
                    (c, ot, e) =>
                    {
                        Component component;
                        OilType oilType;

                        if (!lookup.TryGetValue(c.idComponent, out component))
                        {
                            component = c;
                            component.Equipment = e;
                            component.EnabledOilTypes = new List<OilType>();

                            lookup.Add(c.idComponent, component);
                        }

                        oilType = lookup[c.idComponent].EnabledOilTypes.Where(x => x.idOilType == ot.idOilType).FirstOrDefault();
                        if (oilType == null && ot != null)
                        {
                            oilType = ot;
                            lookup[c.idComponent].EnabledOilTypes.Add(oilType);
                        }

                        return component;
                    },
                    new { idComponent = idComponent },
                    splitOn: "idComponent,idOilType,idEquipment"
                    ).AsQueryable();
                }

                return lookup.Values.Cast<Component>().ToList().FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }
        public List<Component> GetComponents(int? idFactory, int? idArea, int? idEquipment, int? idComponent)
        {
            try
            {
                var lookup = new Dictionary<int, Component>();

                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    sqlConn.Query<Component, OilType, OilManagementEquipment, OilManagementArea, OilManagementFactory, Component>(@"
                    SELECT c.*, ot.*, e.*, a.*, f.*
                    FROM [OilManagement].[Component] c WITH(NOLOCK)
                    JOIN [OilManagement].[Equipment] e WITH(NOLOCK) ON e.idEquipment = c.idEquipment
                    JOIN [OilManagement].[Area] a WITH(NOLOCK) ON a.idArea = e.idArea
                    JOIN [OilManagement].[Factory] f WITH(NOLOCK) ON f.idFactory = a.idFactory
                    LEFT JOIN [OilManagement].[OilTypeByComponent] otc WITH(NOLOCK) ON otc.idComponent = c.idComponent
                    LEFT JOIN [OilManagement].[OilType] ot WITH(NOLOCK) ON otc.idOilType = ot.idOilType
                    WHERE (@idFactory IS NULL OR (NOT @idFactory IS NULL AND f.idFactory = @idFactory))
	                    AND (@idArea IS NULL OR (NOT @idArea IS NULL AND a.idArea = @idArea))
	                    AND (@idEquipment IS NULL OR (NOT @idEquipment IS NULL AND e.idEquipment = @idEquipment))
                        AND (@idComponent IS NULL OR (NOT @idComponent IS NULL AND c.idComponent = @idComponent))
                    ORDER BY c.Name ",
                    (c, ot, e, a, f) =>
                    {
                        Component component;
                        OilType oilType;

                        if (!lookup.TryGetValue(c.idComponent, out component))
                        {
                            component = c;
                            component.Equipment = e;
                            component.Equipment.Area = a;
                            component.Equipment.Area.Factory = f;
                            component.EnabledOilTypes = new List<OilType>();

                            lookup.Add(c.idComponent, component);
                        }

                        oilType = lookup[c.idComponent].EnabledOilTypes.Where(x => x.idOilType == ot.idOilType).FirstOrDefault();
                        if (oilType == null && ot != null)
                        {
                            oilType = ot;
                            lookup[c.idComponent].EnabledOilTypes.Add(oilType);
                        }

                        return component;
                    },
                    new
                    {
                        idFactory = idFactory,
                        idArea = idArea,
                        idEquipment = idEquipment,
                        idComponent = idComponent
                    },
                    splitOn: "idComponent,idOilType,idEquipment,idArea,idFactory"
                    ).AsQueryable();
                }

                return lookup.Values.Cast<Component>().ToList();
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }
        public List<OilManagementFactory> GetAllFactories()
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    return sqlConn.Query<OilManagementFactory>(@"
                        SELECT f.* FROM [OilManagement].[Factory] f").ToList();
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }
        public List<OilManagementArea> GetAllAreas()
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    return sqlConn.Query<OilManagementArea>(@"
                        SELECT a.* FROM [OilManagement].[Area] a").ToList();
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }
        public List<OilManagementEquipment> GetAllEquipments()
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    return sqlConn.Query<OilManagementEquipment>(@"
                        SELECT e.* FROM [OilManagement].[Equipment] e").ToList();
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }
        public object Insert(Component newComponent)
        {
            try
            {
                using (var transactionScope = new TransactionScope())
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    var newComponentCreated = sqlConn.Query(@"
                        INSERT INTO [OilManagement].[Component] (
                            [idEquipment],
	                        [Name],
	                        [Capacity],
                            [OilGradeISO], 
                            [ISOLimitCode],
	                        [CriticalComponent],
                            [MachinesServed],
                            [FlowRate],
                            [Preassure],
	                        [Active],
	                        [InsDateTime],
	                        [UpdDateTime],
	                        [cdIntegrate],
	                        [CreatedBy] ) 
                        VALUES (
                            @idEquipment, 
                            @Name, 
                            @Capacity, 
                            @OilGradeISO,
                            @ISOLimitCode,
                            @CriticalComponent,
                            @MachinesServed,
                            @FlowRate,
                            @Preassure,
                            1,
                            GETDATE(),
                            GETDATE(),
                            0,
                            @CreatedBy);
                        SELECT CAST(SCOPE_IDENTITY() AS INT) idComponent, GETDATE() InsDateTime;",
                    new
                    {
                        idEquipment = newComponent.Equipment.idEquipment,
                        Name = newComponent.Name,
                        Capacity = newComponent.Capacity,
                        OilGradeISO = newComponent.OilGradeISO,
                        ISOLimitCode = newComponent.ISOLimitCode,
                        CriticalComponent = newComponent.CriticalComponent,
                        MachinesServed = newComponent.MachinesServed,
                        FlowRate = newComponent.FlowRate,
                        Preassure = newComponent.Preassure,
                        CreatedBy = newComponent.CreatedBy
                    }).Single();

                    foreach (var oilType in newComponent.EnabledOilTypes)
                    {
                        sqlConn.Query(@"
                        INSERT INTO [OilManagement].[OilTypeByComponent] (
                            [idComponent],
                            [idOilType]
	                        ) 
                        VALUES (
                            @idComponent,
                            @idOilType
                            );",
                            new
                            {
                                idComponent = newComponentCreated.idComponent,
                                idOilType = oilType.idOilType

                            });
                    }

                    transactionScope.Complete();

                    return new
                    {
                        result = new
                        {
                            id = newComponentCreated.idComponent,
                            date = newComponentCreated.InsDateTime,
                        },
                        status = true,
                        message = "Unidade Hidráulica criada com sucesso !"
                    };
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }
        public object Update(Component currentComponent, string cdUser)
        {
            try
            {
                using (var transactionScope = new TransactionScope())
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    var editComponent = sqlConn.Query(@"
                        UPDATE [OilManagement].[Component]
                        SET [Name]  = @Name,
	                        [Capacity] = @Capacity,
	                        [OilGradeISO] = @OilGradeISO,
                            [ISOLimitCode] = @ISOLimitCode,
                            [CriticalComponent] = @CriticalComponent,
                    	    [MachinesServed] = @MachinesServed,
	                        [FlowRate] = @FlowRate,
	                        [Preassure] = @Preassure,
                            [UpdDateTime] = GETDATE(),
                            [ModificatedBy] = @ModificatedBy,
                            [Active] = @Active
                        WHERE idComponent = @idComponent;
                        SELECT [UpdDateTime] FROM [OilManagement].[Component] WHERE idComponent = @idComponent;",
                    new
                    {
                        idComponent = currentComponent.idComponent,
                        Name = currentComponent.Name,
                        Capacity = currentComponent.Capacity,
                        OilGradeISO = currentComponent.OilGradeISO,
                        ISOLimitCode = currentComponent.ISOLimitCode,
                        CriticalComponent = currentComponent.CriticalComponent,
                        MachinesServed = currentComponent.MachinesServed,
                        FlowRate = currentComponent.FlowRate,
                        Preassure = currentComponent.Preassure,
                        ModificatedBy = cdUser,
                        Active = currentComponent.Active
                    }).Single();

                    var sql = @"
                        DELETE FROM [OilManagement].[OilTypeByComponent]
                        WHERE [idComponent] = " + currentComponent.idComponent.ToString() + "; ";

                    foreach (var oilType in currentComponent.EnabledOilTypes)
                    {
                        sql += @"
                        INSERT INTO [OilManagement].[OilTypeByComponent] ([idComponent],[idOilType])
                        VALUES (" + currentComponent.idComponent.ToString() + "," + oilType.idOilType.ToString() + ");";
                    }

                    sqlConn.Query(sql);
                    
                    transactionScope.Complete();

                    return new
                    {
                        result = new
                        {
                            id = currentComponent.idComponent,
                            date = editComponent.UpdDateTime
                        },
                        status = true,
                        message = "Unidade Hidráulica alterada com sucesso !"
                    };
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }
    }
}
