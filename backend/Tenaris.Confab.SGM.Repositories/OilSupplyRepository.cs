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
    public class OilSupplyRepository : IOilSupplyRepository
    {
        public OilSupply GetOilSupply(int idOilSupply)
        {
            try
            {
                var lookup = new Dictionary<int, OilSupply>();

                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    sqlConn.Query<OilSupply, Component, OilManagementEquipment, OilType, OilSupplyType, StoppageType, OilSupply>(@"
                    SELECT s.*, c.*, e.*, ot.*, ost.*, st.*
                    FROM [OilManagement].[OilSupply] s WITH(NOLOCK)
                    JOIN [OilManagement].[Component] c WITH(NOLOCK) ON s.idComponent = c.idComponent
                    JOIN [OilManagement].[Equipment] e WITH(NOLOCK) ON c.idEquipment = e.idEquipment
                    JOIN [OilManagement].[OilType] ot WITH(NOLOCK) ON s.idOilType = ot.idOilType
                    JOIN [OilManagement].[OilSupplyType] ost WITH(NOLOCK) ON ost.idOilSupplyType = s.idOilSupplyType
                    LEFT JOIN [Common].[StoppageType] st WITH(NOLOCK) ON s.idStoppageType = st.idStoppageType
                    WHERE s.idOilSupply = @idOilSupply
                    ",
                    (o, c, e, ot, ost, st) =>
                    {
                        OilSupply oilSupply;

                        oilSupply = o;
                        oilSupply.Component = c;
                        oilSupply.Component.Equipment = e;
                        oilSupply.OilType = ot;
                        oilSupply.OilSupplyType = ost;
                        oilSupply.StoppageType = st;

                        lookup.Add(o.idOilSupply, oilSupply);

                        return oilSupply;
                    },
                    new
                    {
                        idOilSupply = idOilSupply
                    },
                    splitOn: "idOilSupply,idComponent, idEquipment, idOilType, idOilSupplyType, idStoppageType"
                    ).AsQueryable();
                }

                return lookup.Values.Cast<OilSupply>().ToList().FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }
        public List<OilSupply> GetOilSupplyHistory(int idEquipment, int idComponent)
        {
            try
            {
                var lookup = new Dictionary<int, OilSupply>();

                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    sqlConn.Query<OilSupply, Component, OilManagementEquipment, OilType, OilSupplyType, StoppageType, OilSupply>(@"
                    SELECT TOP 10 s.*, c.*, e.*, ot.*, ost.*, st.*
                    FROM [OilManagement].[OilSupply] s WITH(NOLOCK)
                    JOIN [OilManagement].[Component] c WITH(NOLOCK) ON s.idComponent = c.idComponent
                    JOIN [OilManagement].[Equipment] e WITH(NOLOCK) ON c.idEquipment = e.idEquipment
                    JOIN [OilManagement].[OilType] ot WITH(NOLOCK) ON s.idOilType = ot.idOilType
                    JOIN [OilManagement].[OilSupplyType] ost WITH(NOLOCK) ON ost.idOilSupplyType = s.idOilSupplyType
                    LEFT JOIN [Common].[StoppageType] st WITH(NOLOCK) ON s.idStoppageType = st.idStoppageType
                    WHERE e.idEquipment = @idEquipment
                        AND c.idComponent = @idComponent
                    ORDER BY s.InsDateTime DESC",
                    (o, c, e, ot,ost, st) =>
                    {
                        OilSupply oilSupply;

                        oilSupply = o;
                        c.Equipment = e;
                        oilSupply.Component = c;
                        oilSupply.OilType = ot;
                        oilSupply.StoppageType = st;
                        oilSupply.OilSupplyType = ost;

                        lookup.Add(o.idOilSupply, oilSupply);

                        return oilSupply;
                    },
                    new
                    {
                        idEquipment = idEquipment,
                        idComponent = idComponent
                    },
                    splitOn: "idOilSupply,idComponent, idEquipment, idOilType, idOilSupplyType, idStoppageType"
                    ).AsQueryable();
                }

                return lookup.Values.Cast<OilSupply>().ToList();
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }
        public List<OilSupply> GetOilSupplyHistory(int? idFactory, int? idArea, int? idEquipment, int? idComponent, DateTime startDate, DateTime endDate, bool considerInsDateTime)
        {
            try
            {
                var lookup = new Dictionary<int, OilSupply>();

                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    sqlConn.Query<OilSupply>(@"
                        SELECT s.*, c.*, e.*, a.*, f.*, ot.*, ost.*, st.*
                        FROM [OilManagement].[OilSupply] s WITH(NOLOCK)
                        JOIN [OilManagement].[Component] c WITH(NOLOCK) ON s.idComponent = c.idComponent
                        JOIN [OilManagement].[Equipment] e WITH(NOLOCK) ON c.idEquipment = e.idEquipment
                        JOIN [OilManagement].[Area] a WITH(NOLOCK) ON a.idArea = e.idArea
                        JOIN [OilManagement].[Factory] f WITH(NOLOCK) ON f.idFactory = a.idFactory
                        JOIN [OilManagement].[OilType] ot WITH(NOLOCK) ON s.idOilType = ot.idOilType
                        JOIN [OilManagement].[OilSupplyType] ost WITH(NOLOCK) ON ost.idOilSupplyType = s.idOilSupplyType
                        LEFT JOIN [Common].[StoppageType] st WITH(NOLOCK) ON s.idStoppageType = st.idStoppageType
                        WHERE (@idFactory IS NULL OR (NOT @idFactory IS NULL AND f.idFactory = @idFactory))
	                        AND (@idArea IS NULL OR (NOT @idArea IS NULL AND a.idArea = @idArea))
	                        AND (@idEquipment IS NULL OR (NOT @idEquipment IS NULL AND e.idEquipment = @idEquipment))
	                        AND (@idComponent IS NULL OR (NOT @idComponent IS NULL AND c.idComponent = @idComponent))
                            AND ((@considerInsDateTime = 1 AND s.InsDateTime BETWEEN @startDate AND @endDate) 
                                OR (@considerInsDateTime = 0 AND s.SupplyDateTime BETWEEN @startDate AND @endDate))
                        ORDER BY s.InsDateTime DESC",
                    new[]
                        {
                            typeof(OilSupply),
                            typeof(Component),
                            typeof(OilManagementEquipment),
                            typeof(OilManagementArea),
                            typeof(OilManagementFactory),
                            typeof(OilType),
                            typeof(OilSupplyType),
                            typeof(StoppageType)
                        },
                    obj =>
                    {
                        OilSupply o = obj[0] as OilSupply;
                        Component c = obj[1] as Component;
                        OilManagementEquipment e = obj[2] as OilManagementEquipment;
                        OilManagementArea a = obj[3] as OilManagementArea;
                        OilManagementFactory f = obj[4] as OilManagementFactory;
                        OilType ot = obj[5] as OilType;
                        OilSupplyType ost = obj[6] as OilSupplyType;
                        StoppageType st = obj[7] as StoppageType;

                        OilSupply oilSupply;

                        oilSupply = o;
                        oilSupply.Component = c;
                        oilSupply.Component.Equipment = e;
                        oilSupply.Component.Equipment.Area = a;
                        oilSupply.Component.Equipment.Area.Factory = f;
                        oilSupply.OilType = ot;
                        oilSupply.StoppageType = st;
                        oilSupply.OilSupplyType = ost;

                        lookup.Add(o.idOilSupply, oilSupply);

                        return oilSupply;

                    }
                   ,
                    new {
                        idFactory = idFactory,
                        idArea = idArea,
                        idEquipment = idEquipment,
                        idComponent = idComponent,
                        startDate = startDate,
                        endDate = endDate,
                        considerInsDateTime = considerInsDateTime
                    },
                    splitOn: "idOilSupply,idComponent,idEquipment,idArea,idFactory,idOilType,idOilSupplyType,idStoppageType"
                    ).AsQueryable();
                }

                return lookup.Values.Cast<OilSupply>().ToList();
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }
        public object Insert(OilSupply newOilSupply)
        {
            try
            {
                using (var transactionScope = new TransactionScope())
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    var newOilSupplyCreated = sqlConn.Query(@"
                        INSERT INTO [OilManagement].[OilSupply] (
                            [idComponent],
	                        [Quantity],
	                        [idOilType],
                            [idOilSupplyType],
	                        [SupplyDateTime],
                            [IsReuseOil],
                            [Shift],
                            [Comment],
                            [idStoppageType],
                            [StoppageTime], 
	                        [CreatedBy],
	                        [InsDateTime],
	                        [UpdDateTime])
                        VALUES (
                            @idComponent, 
                            @Quantity, 
                            @idOilType,
                            @idOilSupplyType,
                            @supplyDateTime,
                            @IsReuseOil,
                            @Shift,
                            @Comment,
                            @idStoppageType,
                            @StoppageTime,
                            @CreatedBy,
                            GETDATE(),
                            GETDATE());
                        SELECT CAST(SCOPE_IDENTITY() AS INT) idOilSupply, @supplyDateTime [SupplyDateTime];",
                    new
                    {
                        idComponent = newOilSupply.Component.idComponent,
                        Quantity = newOilSupply.Quantity,
                        idOilType = newOilSupply.OilType.idOilType,
                        idOilSupplyType = newOilSupply.OilSupplyType.idOilSupplyType,
                        supplyDateTime = newOilSupply.SupplyDateTime,
                        IsReuseOil = newOilSupply.IsReuseOil,
                        Shift = newOilSupply.Shift,
                        Comment = newOilSupply.Comment,
                        idStoppageType = newOilSupply.OilSupplyType.Code == "C" ? newOilSupply.StoppageType.idStoppageType : (int ?)null,
                        StoppageTime = newOilSupply.OilSupplyType.Code == "C" && newOilSupply.StoppageType != null && newOilSupply.StoppageType.Code == "PNO" ? newOilSupply.StoppageTime : (int?)null,
                        @CreatedBy = newOilSupply.CreatedBy
                    }).Single();

                    transactionScope.Complete();

                    return new
                    {
                        result = new
                        {
                            id = newOilSupplyCreated.idOilSupply,
                            date = newOilSupplyCreated.SupplyDateTime,
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

        public object Update(OilSupply oilSupply, string cdUser)
        {
            try
            {
                using (var transactionScope = new TransactionScope())
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    var editOilSupply = sqlConn.Query(@"
                        UPDATE [OilManagement].[OilSupply]
                        SET [Quantity]  = @Quantity,
	                        [idOilType] = @idOilType,
	                        [idOilSupplyType] = @idOilSupplyType,
                    	    [SupplyDateTime] = @SupplyDateTime,
	                        [IsReuseOil] = @IsReuseOil,
	                        [Shift] = @Shift,
                            [idStoppageType] = @idStoppageType,
                            [StoppageTime] = @StoppageTime,
                            [ModificatedBy] = @ModificatedBy,
                            [UpdDateTime] = GETDATE()
                        WHERE [idOilSupply] = @idOilSupply;
                        SELECT [SupplyDateTime] FROM [OilManagement].[OilSupply] WHERE [idOilSupply] = @idOilSupply;",
                    new
                    {
                            idOilSupply = oilSupply.idOilSupply,
                            Quantity  = oilSupply.Quantity,
                            idOilType = oilSupply.OilType.idOilType,
	                        idOilSupplyType = oilSupply.OilSupplyType.idOilSupplyType,
                    	    SupplyDateTime = oilSupply.SupplyDateTime,
                            IsReuseOil = oilSupply.IsReuseOil,
	                        Shift = oilSupply.Shift,
                            idStoppageType = oilSupply.OilSupplyType.Code == "C" ? oilSupply.StoppageType.idStoppageType : (int?)null,
                            StoppageTime = oilSupply.OilSupplyType.Code == "C" && oilSupply.StoppageType != null && oilSupply.StoppageType.Code == "PNO" ? oilSupply.StoppageTime : (int?)null,
                            ModificatedBy = cdUser
                    }).Single();

                    transactionScope.Complete();

                    return new
                    {
                        result = new
                        {
                            id = oilSupply.idOilSupply,
                            date = editOilSupply.SupplyDateTime
                        },
                        status = true,
                        message = "Abastecimento alterado com sucesso !"
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
