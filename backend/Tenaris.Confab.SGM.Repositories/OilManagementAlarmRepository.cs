using System;
using System.Collections.Generic;
using System.Linq;
using Tenaris.Confab.SGM.Domain;
using Tenaris.Confab.SGM.Domain.Entities;
using Dapper;
using System.Data.SqlClient;
using System.Transactions;

namespace Tenaris.Confab.SGM.Repositories
{
    public class OilManagementAlarmRepository : IOilManagementAlarmRepository
    {
        public OilManagementAlarm GetAlarm(int idAlarm)
        {
            try
            {
                var lookup = new Dictionary<int, OilManagementAlarm>();

                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    sqlConn.Query<OilManagementAlarm>(@"
                    SELECT al.*, c.*, e.*, a.*, f.*, at.*
                    FROM [OilManagement].[Alarm] al WITH(NOLOCK)
                    JOIN [OilManagement].[Component] c WITH(NOLOCK) ON al.idComponent = c.idComponent
                    JOIN [OilManagement].[Equipment] e WITH(NOLOCK) ON c.idEquipment = e.idEquipment
                    JOIN [OilManagement].[Area] a WITH(NOLOCK) ON a.idArea = e.idArea
                    JOIN [OilManagement].[Factory] f WITH(NOLOCK) ON f.idFactory = a.idFactory
                    JOIN [OilManagement].[AlarmType] at WITH(NOLOCK) ON at.idAlarmType = al.idAlarm
                    WHERE al.idAlarm = @idAlarm
                    ",
                    new[]
                        {
                            typeof(OilManagementAlarm),
                            typeof(Component),
                            typeof(OilManagementEquipment),
                            typeof(OilManagementArea),
                            typeof(OilManagementFactory),
                            typeof(OilManagementAlarmType)
                        },
                    obj =>
                    {
                        OilManagementAlarm al = obj[0] as OilManagementAlarm;
                        Component c = obj[1] as Component;
                        OilManagementEquipment e = obj[2] as OilManagementEquipment;
                        OilManagementArea a = obj[3] as OilManagementArea;
                        OilManagementFactory f = obj[4] as OilManagementFactory;
                        OilManagementAlarmType at = obj[5] as OilManagementAlarmType;

                        OilManagementAlarm alarm;

                        alarm = al;
                        alarm.Component = c;
                        alarm.Component.Equipment = e;
                        alarm.Component.Equipment.Area = a;
                        alarm.Component.Equipment.Area.Factory = f;
                        alarm.AlarmType = at;

                        lookup.Add(al.idAlarm, alarm);

                        return alarm;
                    }
                   ,
                    new
                    {
                        idAlarm = idAlarm
                    },
                    splitOn: "idAlarm,idComponent,idEquipment,idArea,idFactory,idAlarmType"
                    ).AsQueryable();
                }

                return lookup.Values.Cast<OilManagementAlarm>().ToList().FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }
        public List<OilManagementAlarm> GetAlarmsByComponent(int idComponent)
        {
            try
            {
                int idAlarmDummy = -1;
                var lookup = new Dictionary<int, OilManagementAlarm>();
                var lookupByAlarmType = new Dictionary<int, OilManagementAlarm>();
                var lookupAlarmTypes = new Dictionary<int, OilManagementAlarmType>();

                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    sqlConn.Query<OilManagementAlarm>(@"
                    SELECT c.*, e.*, a.*, f.*, al.*, at.*, s.*, sv.Value, ag.*, alg.TypeAlarmByAlarmGroup, aac.*
                    FROM [OilManagement].[Component] c WITH(NOLOCK)
                    JOIN [OilManagement].[Equipment] e WITH(NOLOCK) ON c.idEquipment = e.idEquipment
                    JOIN [OilManagement].[Area] a WITH(NOLOCK) ON a.idArea = e.idArea
                    JOIN [OilManagement].[Factory] f WITH(NOLOCK) ON f.idFactory = a.idFactory
                    JOIN [OilManagement].[AlarmType] at WITH(NOLOCK) ON at.idAlarmType = at.idAlarmType
                    JOIN [OilManagement].[AlarmSetting] s WITH(NOLOCK) ON at.idAlarmType = s.idAlarmType
                    LEFT JOIN [OilManagement].[Alarm] al WITH(NOLOCK) ON s.idAlarmType = al.idAlarmType AND c.idComponent = al.idComponent
                    LEFT JOIN [OilManagement].[AlarmSettingValue] sv WITH(NOLOCK) ON sv.idAlarmSetting = s.idAlarmSetting AND sv.idAlarm = al.idAlarm
                    LEFT JOIN [OilManagement].[AlarmByAlarmGroup] alg WITH(NOLOCK) ON alg.idAlarm = al.idAlarm
                    LEFT JOIN [OilManagement].[AlarmGroup] ag WITH(NOLOCK) ON ag.idAlarmGroup = alg.idAlarmGroup
                    LEFT JOIN [OilManagement].[AlarmAlertConfig] aac WITH(NOLOCK) ON al.idAlarm = aac.idAlarm
                    WHERE c.idComponent = @idComponent
                    ORDER BY at.idAlarmType",
                    new[]
                        {
                            typeof(Component),
                            typeof(OilManagementEquipment),
                            typeof(OilManagementArea),
                            typeof(OilManagementFactory),
                            typeof(OilManagementAlarm),
                            typeof(OilManagementAlarmType),
                            typeof(OilManagementAlarmSetting),
                            typeof(OilManagementAlarmGroup),
                            typeof(OilManagementAlarmAlertConfig)
                        },
                    obj =>
                    {
                        Component c = obj[0] as Component;
                        OilManagementEquipment e = obj[1] as OilManagementEquipment;
                        OilManagementArea a = obj[2] as OilManagementArea;
                        OilManagementFactory f = obj[3] as OilManagementFactory;
                        OilManagementAlarm al = obj[4] as OilManagementAlarm;
                        OilManagementAlarmType at = obj[5] as OilManagementAlarmType;
                        OilManagementAlarmSetting s = obj[6] as OilManagementAlarmSetting;
                        OilManagementAlarmGroup ag = obj[7] as OilManagementAlarmGroup;
                        OilManagementAlarmAlertConfig aac = obj[8] as OilManagementAlarmAlertConfig;

                        OilManagementAlarm alarm;
                        OilManagementAlarmType alarmType;
                        OilManagementAlarmGroup alarmGroup;

                        if (al == null)
                        {
                            if (!lookupByAlarmType.TryGetValue(at.idAlarmType, out alarm))
                            {
                                alarm = new OilManagementAlarm()
                                {
                                    idAlarm = idAlarmDummy,
                                    Active = false,
                                };
                                idAlarmDummy--;

                                alarm.Groups = new List<OilManagementAlarmGroup>();
                                alarm.GroupsCC = new List<OilManagementAlarmGroup>();
                                alarm.AlertConfig = new OilManagementAlarmAlertConfig();
                                lookupByAlarmType.Add(at.idAlarmType, alarm);
                                lookup.Add(alarm.idAlarm, alarm);
                            }
                        }
                        else
                        {
                            if (!lookup.TryGetValue(al.idAlarm, out alarm))
                            {
                                alarm = al;
                                alarm.AlertConfig = aac;
                                alarm.Groups = new List<OilManagementAlarmGroup>();
                                alarm.GroupsCC = new List<OilManagementAlarmGroup>();
                                lookup.Add(al.idAlarm, alarm);
                            }
                        }

                        alarm.Component = c;
                        alarm.Component.Equipment = e;
                        alarm.Component.Equipment.Area = a;
                        alarm.Component.Equipment.Area.Factory = f;

                        if (!lookupAlarmTypes.TryGetValue(at.idAlarmType, out alarmType))
                        {
                            alarmType = at;
                            alarmType.AlarmSettings = new List<OilManagementAlarmSetting>();
                            lookupAlarmTypes.Add(at.idAlarmType, alarmType);
                        }

                        alarm.AlarmType = alarmType;

                        if (s != null)
                        {
                            var alarmSetting = alarm.AlarmType.AlarmSettings.Where(x => x.idAlarmSetting == s.idAlarmSetting).FirstOrDefault();
                            if (alarmSetting == null)
                            {
                                alarmSetting = s;
                                alarm.AlarmType.AlarmSettings.Add(alarmSetting);
                            }
                        }

                        if (alarm.idAlarm > 0)
                        {
                            if (ag != null)
                            {
                                alarmGroup = alarm.Groups.Where(x => x.idAlarmGroup == ag.idAlarmGroup).FirstOrDefault();
                                if (alarmGroup == null)
                                {
                                    alarmGroup = alarm.GroupsCC.Where(x => x.idAlarmGroup == ag.idAlarmGroup).FirstOrDefault();
                                    if (alarmGroup == null)
                                    {
                                        if (ag.TypeAlarmByAlarmGroup == "TO")
                                        {
                                            alarmGroup = ag;
                                            alarm.Groups.Add(alarmGroup);
                                        }
                                        else if (ag.TypeAlarmByAlarmGroup == "CC")
                                        {
                                            alarmGroup = ag;
                                            alarm.GroupsCC.Add(alarmGroup);
                                        }
                                    }
                                }
                            }
                        }

                        return alarm;
                    }
                   ,
                    new
                    {
                        idComponent = idComponent
                    },
                    splitOn: "idComponent,idEquipment,idArea,idFactory,idAlarm,idAlarmType, idAlarmSetting, idAlarmGroup,idAlarmAlertConfig"
                    ).AsQueryable();
                }

                return lookup.Values.Cast<OilManagementAlarm>().ToList();
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }
        public List<OilManagementAlarm> GetAlarms(int? idFactory, int? idArea, int? idEquipment, int? idComponent, bool onlyActive)
        {
            try
            {
                var lookup = new Dictionary<int, OilManagementAlarm>();

                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    sqlConn.Query<OilManagementAlarm>(@"
                    SELECT al.*, c.*, e.*, a.*, f.*, at.*
                    FROM [OilManagement].[Alarm] al WITH(NOLOCK)
                    JOIN [OilManagement].[Component] c WITH(NOLOCK) ON al.idComponent = c.idComponent
                    JOIN [OilManagement].[Equipment] e WITH(NOLOCK) ON c.idEquipment = e.idEquipment
                    JOIN [OilManagement].[Area] a WITH(NOLOCK) ON a.idArea = e.idArea
                    JOIN [OilManagement].[Factory] f WITH(NOLOCK) ON f.idFactory = a.idFactory
                    JOIN [OilManagement].[AlarmType] at WITH(NOLOCK) ON at.idAlarmType = al.idAlarmType
                        WHERE (@idFactory IS NULL OR (NOT @idFactory IS NULL AND f.idFactory = @idFactory))
	                        AND (@idArea IS NULL OR (NOT @idArea IS NULL AND a.idArea = @idArea))
	                        AND (@idEquipment IS NULL OR (NOT @idEquipment IS NULL AND e.idEquipment = @idEquipment))
	                        AND (@idComponent IS NULL OR (NOT @idComponent IS NULL AND c.idComponent = @idComponent))
                            AND (@onlyActive = 0 OR (@onlyActive = 1 AND al.Active = @onlyActive))
                        ORDER BY f.Name, a.Name, e.Name, c.Name",
                    new[]
                        {
                            typeof(OilManagementAlarm),
                            typeof(Component),
                            typeof(OilManagementEquipment),
                            typeof(OilManagementArea),
                            typeof(OilManagementFactory),
                            typeof(OilManagementAlarmType)
                        },
                    obj =>
                    {
                        OilManagementAlarm al = obj[0] as OilManagementAlarm;
                        Component c = obj[1] as Component;
                        OilManagementEquipment e = obj[2] as OilManagementEquipment;
                        OilManagementArea a = obj[3] as OilManagementArea;
                        OilManagementFactory f = obj[4] as OilManagementFactory;
                        OilManagementAlarmType at = obj[5] as OilManagementAlarmType;

                        OilManagementAlarm alarm;

                        alarm = al;
                        alarm.Component = c;
                        alarm.Component.Equipment = e;
                        alarm.Component.Equipment.Area = a;
                        alarm.Component.Equipment.Area.Factory = f;
                        alarm.AlarmType = at;

                        lookup.Add(al.idAlarm, alarm);

                        return alarm;
                    }
                   ,
                    new
                    {
                        idFactory = idFactory,
                        idArea = idArea,
                        idEquipment = idEquipment,
                        idComponent = idComponent,
                        onlyActive = onlyActive
                    },
                    splitOn: "idAlarm,idComponent,idEquipment,idArea,idFactory,idAlarmType"
                    ).AsQueryable();
                }

                return lookup.Values.Cast<OilManagementAlarm>().ToList();
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }
        public List<OilManagementAlarmHistory> GetAlarmsHistory(int? idFactory, int? idArea, int? idEquipment, int? idComponent, bool onlyActive, DateTime startDate, DateTime endDate)
        {
            try
            {
                var lookup = new Dictionary<int, OilManagementAlarmHistory>();

                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    sqlConn.Query<OilManagementAlarmHistory>(@"
                    SELECT ah.*, al.*, at.*, os.*, c.*, e.*, a.*, f.*
                    FROM [OilManagement].[AlarmHistory] ah WITH(NOLOCK)
                    JOIN [OilManagement].[Alarm] al WITH(NOLOCK) ON al.idAlarm = ah.idAlarm
                    JOIN [OilManagement].[AlarmType] at WITH(NOLOCK) ON at.idAlarmType = al.idAlarmType
                    JOIN [OilManagement].[OilSupply] os WITH(NOLOCK) ON os.idOilSupply = ah.idOilSupply
                    JOIN [OilManagement].[Component] c WITH(NOLOCK) ON al.idComponent = c.idComponent
                    JOIN [OilManagement].[Equipment] e WITH(NOLOCK) ON c.idEquipment = e.idEquipment
                    JOIN [OilManagement].[Area] a WITH(NOLOCK) ON a.idArea = e.idArea
                    JOIN [OilManagement].[Factory] f WITH(NOLOCK) ON f.idFactory = a.idFactory
                        WHERE (@idFactory IS NULL OR (NOT @idFactory IS NULL AND f.idFactory = @idFactory))
	                        AND (@idArea IS NULL OR (NOT @idArea IS NULL AND a.idArea = @idArea))
	                        AND (@idEquipment IS NULL OR (NOT @idEquipment IS NULL AND e.idEquipment = @idEquipment))
	                        AND (@idComponent IS NULL OR (NOT @idComponent IS NULL AND c.idComponent = @idComponent))
                            AND (@onlyActive = 0 OR (@onlyActive = 1 AND al.Active = @onlyActive))
		                    AND (@startDate IS NULL OR (NOT @startDate IS NULL AND ah.InsDateTime >= @startDate))
		                    AND (@endDate IS NULL OR (NOT @endDate IS NULL AND ah.InsDateTime <= @endDate))
                    ORDER BY ah.idAlarmHistory
                    ",
                    new[]
                        {
                            typeof(OilManagementAlarmHistory),
                            typeof(OilManagementAlarm),
                            typeof(OilManagementAlarmType),
                            typeof(OilSupply),
                            typeof(Component),
                            typeof(OilManagementEquipment),
                            typeof(OilManagementArea),
                            typeof(OilManagementFactory)

                        },
                    obj =>
                    {
                        OilManagementAlarmHistory ah = obj[0] as OilManagementAlarmHistory;
                        OilManagementAlarm al = obj[1] as OilManagementAlarm;
                        OilManagementAlarmType at = obj[2] as OilManagementAlarmType;
                        OilSupply os = obj[3] as OilSupply;
                        Component c = obj[4] as Component;
                        OilManagementEquipment e = obj[5] as OilManagementEquipment;
                        OilManagementArea a = obj[6] as OilManagementArea;
                        OilManagementFactory f = obj[7] as OilManagementFactory;

                        OilManagementAlarmHistory alarmHistory;

                        alarmHistory = ah;
                        alarmHistory.Alarm = al;
                        alarmHistory.OilSupply = os;
                        alarmHistory.Alarm.Component = c;
                        alarmHistory.Alarm.Component.Equipment = e;
                        alarmHistory.Alarm.Component.Equipment.Area = a;
                        alarmHistory.Alarm.Component.Equipment.Area.Factory = f;
                        alarmHistory.Alarm.AlarmType = at;

                        lookup.Add(ah.idAlarmHistory, alarmHistory);

                        return alarmHistory;
                    }
                   ,
                    new
                    {
                        idFactory = idFactory,
                        idArea = idArea,
                        idEquipment = idEquipment,
                        idComponent = idComponent,
                        onlyActive = onlyActive,
                        startDate = startDate,
                        endDate = endDate
                    },
                    splitOn: "idAlarmHistory,idAlarm,idAlarmType,idOilSupply,idComponent,idEquipment,idArea,idFactory"
                    ).AsQueryable();
                }

                return lookup.Values.Cast<OilManagementAlarmHistory>().ToList();
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }
        public List<OilManagementAlarmType> GetAlarmTypes()
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    return sqlConn.Query<OilManagementAlarmType>(@"
                        SELECT at.* 
                        FROM [OilManagement].[AlarmType] at WITH(NOLOCK)"
                    ).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }
        public OilManagementAlarmGroup GetAlarmGroup(int idAlarmGroup)
        {
            try
            {
                var lookup = new Dictionary<int, OilManagementAlarmGroup>();

                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    sqlConn.Query<OilManagementAlarmGroup>(@"
                    SELECT ag.*, u.idUser 
                    FROM [OilManagement].[AlarmGroup] ag WITH(NOLOCK)
                    LEFT JOIN [OilManagement].[UserByAlarmGroup] u WITH(NOLOCK) ON ag.idAlarmGroup = u.idAlarmGroup
                    WHERE ag.idAlarmGroup = @idAlarmGroup
                    ",
                    new[]
                        {
                            typeof(OilManagementAlarmGroup),
                            typeof(OilManagementUser)
                        },
                    obj =>
                    {
                        OilManagementAlarmGroup ag = obj[0] as OilManagementAlarmGroup;
                        OilManagementUser u = obj[1] as OilManagementUser;

                        OilManagementAlarmGroup alarmGroup;
                        OilManagementUser user;

                        if (!lookup.TryGetValue(ag.idAlarmGroup, out alarmGroup))
                        {
                            alarmGroup = ag;
                            alarmGroup.Users = new List<OilManagementUser>();
                            lookup.Add(ag.idAlarmGroup, alarmGroup);
                        }

                        if (u != null)
                        {
                            user = alarmGroup.Users.Where(x => x.idUser == u.idUser).FirstOrDefault();
                            if (user == null)
                            {
                                user = u;
                                alarmGroup.Users.Add(user);
                            }
                        }

                        return alarmGroup;
                    }
                   ,
                    new
                    {
                        idAlarmGroup = idAlarmGroup
                    },
                    splitOn: "idAlarmGroup,idUser"
                    ).AsQueryable();
                }

                return lookup.Values.Cast<OilManagementAlarmGroup>().ToList().FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }
        public List<OilManagementAlarmGroup> GetAlarmGroups()
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    return sqlConn.Query<OilManagementAlarmGroup>(@"
                        SELECT g.* FROM [OilManagement].[AlarmGroup] g WITH(NOLOCK)"
                    ).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }
        public object Insert(ref OilManagementAlarm newAlarm)
        {
            try
            {
                using (var transactionScope = new TransactionScope())
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    var newAlarmCreated = sqlConn.Query(@"
                        [OilManagement].[InsOrUpdAlarm]
                            @idAlarm,
                            @idComponent,
                            @idAlarmType,
                            @Active,
                            @CreatedBy,
                            @ModificatedBy",
                    new
                    {
                        idAlarm = newAlarm.idAlarm,
                        idComponent = newAlarm.Component.idComponent,
                        idAlarmType = newAlarm.AlarmType.idAlarmType,
                        Active = newAlarm.Active,
                        CreatedBy = newAlarm.CreatedBy,
                        ModificatedBy = newAlarm.ModificatedBy
                    }).Single();

                    foreach (var setting in newAlarm.AlarmType.AlarmSettings)
                    {
                        sqlConn.Query(@"
                        [OilManagement].[InsOrUpdAlarmSetting]
                            @idAlarm,
                            @idAlarmSetting,
	                        @Value,
                            @CreatedBy,
                            @ModificatedBy",
                        new
                        {
                            idAlarm = newAlarmCreated.idAlarm,
                            idAlarmSetting = setting.idAlarmSetting,
                            Value = setting.Value,
                            CreatedBy = newAlarm.CreatedBy,
                            ModificatedBy = newAlarm.ModificatedBy
                        });
                    }

                    transactionScope.Complete();

                    newAlarm.idAlarm = newAlarmCreated.idAlarm;

                    return new
                    {
                        result = new
                        {
                            id = newAlarmCreated.idAlarm,
                            date = newAlarmCreated.InsDateTime,
                        },
                        status = true,
                        message = "Alarme criada com sucesso !"
                    };
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }
        public object Update(OilManagementAlarm editedAlarm)
        {
            try
            {
                using (var transactionScope = new TransactionScope())
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    var alarmAltered = sqlConn.Query(@"
                        [OilManagement].[InsOrUpdAlarm]
                            @idAlarm,
                            @idComponent,
                            @idAlarmType,
                            @Active,
                            @CreatedBy,
                            @ModificatedBy",
                    new
                    {
                        idAlarm = editedAlarm.idAlarm,
                        idComponent = editedAlarm.Component.idComponent,
                        idAlarmType = editedAlarm.AlarmType.idAlarmType,
                        Active = editedAlarm.Active,
                        CreatedBy = editedAlarm.CreatedBy,
                        ModificatedBy = editedAlarm.ModificatedBy
                    }).Single();

                    foreach (var setting in editedAlarm.AlarmType.AlarmSettings)
                    {
                        sqlConn.Query(@"
                        [OilManagement].[InsOrUpdAlarmSetting]
                            @idAlarm,
                            @idAlarmSetting,
	                        @Value,
                            @CreatedBy,
                            @ModificatedBy",
                        new
                        {
                            idAlarm = alarmAltered.idAlarm,
                            idAlarmSetting = setting.idAlarmSetting,
                            Value = setting.Value,
                            CreatedBy = editedAlarm.CreatedBy,
                            ModificatedBy = editedAlarm.ModificatedBy
                        });
                    }

                    transactionScope.Complete();

                    return new
                    {
                        result = new
                        {
                            id = alarmAltered.idAlarm,
                            date = alarmAltered.InsDateTime,
                        },
                        status = true,
                        message = "Alarme alterada com sucesso !"
                    };
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }
        public object UpdateGroups(OilManagementAlarm editedAlarm)
        {
            try
            {
                using (var transactionScope = new TransactionScope())
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    sqlConn.Query(@"
                    DELETE FROM [OilManagement].[AlarmByAlarmGroup]
                    WHERE idAlarm = @idAlarm
                    ",
                    new
                    {
                        idAlarm = editedAlarm.idAlarm
                    });

                    foreach (var group in editedAlarm.Groups)
                    {
                        sqlConn.Query(@"
                        INSERT INTO [OilManagement].[AlarmByAlarmGroup]
                        (
                            [idAlarm],
                            [idAlarmGroup],
                            [TypeAlarmByAlarmGroup],
                            [Active]
                        )
                        VALUES
                        (
                            @idAlarm,
                            @idAlarmGroup,
                            'TO',
                            1           
                        )",
                        new
                        {
                            idAlarm = editedAlarm.idAlarm,
                            idAlarmGroup = group.idAlarmGroup
                        });
                    }

                    foreach (var group in editedAlarm.GroupsCC)
                    {
                        sqlConn.Query(@"
                        INSERT INTO [OilManagement].[AlarmByAlarmGroup]
                        (
                            [idAlarm],
                            [idAlarmGroup],
                            [TypeAlarmByAlarmGroup],
                            [Active]
                        )
                        VALUES
                        (
                            @idAlarm,
                            @idAlarmGroup,
                            'CC',
                            1           
                        )",
                        new
                        {
                            idAlarm = editedAlarm.idAlarm,
                            idAlarmGroup = group.idAlarmGroup
                        });
                    }

                    sqlConn.Query(@"
                        [OilManagement].[InsOrUpdAlarmAlertConfig]
                            @idAlarm,
	                        @SubjectMask,
	                        @ContentMask,
                            @CreatedBy,
                            @ModificatedBy",
                    new
                    {
                        idAlarm = editedAlarm.idAlarm,
                        SubjectMask = editedAlarm.AlertConfig.SubjectMask,
                        ContentMask = editedAlarm.AlertConfig.ContentMask,
                        CreatedBy = editedAlarm.CreatedBy,
                        ModificatedBy = editedAlarm.ModificatedBy
                    });

                    transactionScope.Complete();

                    return new
                    {
                        result = new
                        {
                            id = editedAlarm.idAlarm,
                            date = DateTime.Now,
                        },
                        status = true,
                        message = "Grupos da alarme alterados com sucesso !"
                    };
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }
        public object Insert(OilManagementAlarmGroup newAlarmGroup)
        {
            try
            {
                using (var transactionScope = new TransactionScope())
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    var newAlarmGroupCreated = sqlConn.Query(@"
                        INSERT INTO [OilManagement].[AlarmGroup]
                        (
                            [Name],
	                        [Active],
	                        [CreatedBy],
	                        [InsDateTime]
                        )
                        VALUES
                        (
                            @Name,
                            1,
                            @CreatedBy,
                            GETDATE()
                        );
                        SELECT SCOPE_IDENTITY() idAlarmGroup, GETDATE() InsDateTime;
                        ",
                        new
                        {
                            Name = newAlarmGroup.Name,
                            CreatedBy = newAlarmGroup.CreatedBy
                        }).Single();

                    foreach (var u in newAlarmGroup.Users)
                    {
                        sqlConn.Query(@"
                        INSERT INTO [OilManagement].[UserByAlarmGroup]
                        (
                            [idUser],
                            [idAlarmGroup],
	                        [Active]
                        )
                        VALUES 
                        (
                            @idUser,
                            @idAlarmGroup,
                            1
                        )",
                        new
                        {
                            idUser = u.idUser,
                            idAlarmGroup = newAlarmGroupCreated.idAlarmGroup
                        });
                    }

                    transactionScope.Complete();

                    return new
                    {
                        result = new
                        {
                            id = newAlarmGroupCreated.idAlarmGroup,
                            date = newAlarmGroupCreated.InsDateTime,
                        },
                        status = true,
                        message = "Grupo criado com sucesso !"
                    };
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }
        public object Update(OilManagementAlarmGroup editedAlarmGroup)
        {
            try
            {
                using (var transactionScope = new TransactionScope())
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    sqlConn.Query(@"
                        UPDATE [OilManagement].[AlarmGroup]
                        SET [Name] = @Name,
	                        [Active] = @Active,
	                        [ModificatedBy] = CASE WHEN @Active <> [Active] OR [Name] <> @Name THEN @ModificatedBy ELSE [ModificatedBy] END,
	                        [UpdDateTime] = CASE WHEN @Active <> [Active] OR [Name] <> @Name THEN @UpdDateTime ELSE [UpdDateTime] END
                        WHERE idAlarmGroup = @idAlarmGroup;",
                    new
                    {
                        idAlarmGroup = editedAlarmGroup.idAlarmGroup,
                        Name = editedAlarmGroup.Name,
                        Active = editedAlarmGroup.Active,
                        ModificatedBy = editedAlarmGroup.ModificatedBy,
                        UpdDateTime = editedAlarmGroup.UpdDateTime
                    });

                    sqlConn.Query(@"
                    DELETE FROM [OilManagement].[UserByAlarmGroup]
                    WHERE [idAlarmGroup] = @idAlarmGroup",
                    new
                    {
                        idAlarmGroup = editedAlarmGroup.idAlarmGroup
                    });

                    foreach (var u in editedAlarmGroup.Users)
                    {
                        sqlConn.Query(@"
                        INSERT INTO[OilManagement].[UserByAlarmGroup]
                        (
                            [idUser],
                            [idAlarmGroup],
                            [Active]
                        )
                        VALUES
                        (
                            @idUser,
                            @idAlarmGroup,
                            1
                        )",
                        new
                        {
                            idUser = u.idUser,
                            idAlarmGroup = editedAlarmGroup.idAlarmGroup
                        });
                    }

                    transactionScope.Complete();

                    return new
                    {
                        result = new
                        {
                            id = editedAlarmGroup.idAlarmGroup,
                            date = DateTime.Now
                        },
                        status = true,
                        message = "Grupo alterado com sucesso !"
                    };
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }
        public List<OilManagementUser> GetUsersAll()
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnectionSAB))
                {
                    return sqlConn.Query<OilManagementUser>(@"
                        SELECT	u.*
                        FROM [Security].[User] u WITH(NOLOCK)
                        WHERE u.Active = 1
                        ORDER BY Name
                        ").ToList();
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }

        public List<OilManagementAlarmSendStatus> GetAllPendingsAlertToSend()
        {
            try
            {
                var lookup = new Dictionary<int, OilManagementAlarmSendStatus>();
                var lookupHistory = new Dictionary<int, OilManagementAlarmHistory>();

                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    sqlConn.Query<OilManagementAlarmSendStatus>(@"
                    SELECT ss.*, ah.*, al.*,os.*, c.*,  e.*, a.*, f.*, at.*, s.*, sv.Value, ag.*, alg.TypeAlarmByAlarmGroup, ac.*
                    FROM [OilManagement].[AlarmSendStatus] ss WITH(NOLOCK)
                    JOIN [OilManagement].[AlarmHistory] ah WITH(NOLOCK) ON ah.idAlarmHistory = ss.idAlarmHistory
                    JOIN [OilManagement].[Alarm] al WITH(NOLOCK) ON ah.idAlarm = al.idAlarm
                    JOIN [OilManagement].[OilSupply] os WITH(NOLOCK) ON ah.idOilSupply = os.idOilSupply
                    JOIN [OilManagement].[Component] c WITH(NOLOCK) ON c.idComponent = al.idComponent
                    JOIN [OilManagement].[Equipment] e WITH(NOLOCK) ON c.idEquipment = e.idEquipment
                    JOIN [OilManagement].[Area] a WITH(NOLOCK) ON a.idArea = e.idArea
                    JOIN [OilManagement].[Factory] f WITH(NOLOCK) ON f.idFactory = a.idFactory
                    JOIN [OilManagement].[AlarmType] at WITH(NOLOCK) ON at.idAlarmType = al.idAlarmType
                    JOIN [OilManagement].[AlarmSetting] s WITH(NOLOCK) ON at.idAlarmType = s.idAlarmType
                    JOIN [OilManagement].[AlarmSettingValue] sv WITH(NOLOCK) ON sv.idAlarmSetting = s.idAlarmSetting AND sv.idAlarm = al.idAlarm
                    JOIN [OilManagement].[AlarmByAlarmGroup] alg WITH(NOLOCK) ON alg.idAlarm = al.idAlarm
                    JOIN [OilManagement].[AlarmGroup] ag WITH(NOLOCK) ON ag.idAlarmGroup = alg.idAlarmGroup
                    JOIN [OilManagement].[AlarmAlertConfig] ac WITH(NOLOCK) ON ac.idAlarm = al.idAlarm
                    WHERE ss.[Sent] = 0
	                    AND al.[SendAlert] = 1
                    ",
                    new[]
                        {
                            typeof(OilManagementAlarmSendStatus),
                            typeof(OilManagementAlarmHistory),
                            typeof(OilManagementAlarm),
                            typeof(OilSupply),
                            typeof(Component),
                            typeof(OilManagementEquipment),
                            typeof(OilManagementArea),
                            typeof(OilManagementFactory),
                            typeof(OilManagementAlarmType),
                            typeof(OilManagementAlarmSetting),
                            typeof(OilManagementAlarmGroup),
                            typeof(OilManagementAlarmAlertConfig)
                        },
                    obj =>
                    {
                        OilManagementAlarmSendStatus ss = obj[0] as OilManagementAlarmSendStatus;
                        OilManagementAlarmHistory ah = obj[1] as OilManagementAlarmHistory;
                        OilManagementAlarm al = obj[2] as OilManagementAlarm;
                        OilSupply os = obj[3] as OilSupply;
                        Component c = obj[4] as Component;
                        OilManagementEquipment e = obj[5] as OilManagementEquipment;
                        OilManagementArea a = obj[6] as OilManagementArea;
                        OilManagementFactory f = obj[7] as OilManagementFactory;
                        OilManagementAlarmType at = obj[8] as OilManagementAlarmType;
                        OilManagementAlarmSetting s = obj[9] as OilManagementAlarmSetting;
                        OilManagementAlarmGroup ag = obj[10] as OilManagementAlarmGroup;
                        OilManagementAlarmAlertConfig ac = obj[11] as OilManagementAlarmAlertConfig;

                        OilManagementAlarmSendStatus sendStatus;
                        OilManagementAlarmSetting setting;
                        OilManagementAlarmGroup group;

                        if (!lookup.TryGetValue(ah.idAlarmHistory, out sendStatus))
                        {
                            sendStatus = ss;
                            sendStatus.AlarmHistory = ah;
                            sendStatus.AlarmHistory.Alarm = al;
                            sendStatus.AlarmHistory.OilSupply = os;
                            sendStatus.AlarmHistory.Alarm.Component = c;
                            sendStatus.AlarmHistory.Alarm.Component.Equipment = e;
                            sendStatus.AlarmHistory.Alarm.Component.Equipment.Area = a;
                            sendStatus.AlarmHistory.Alarm.Component.Equipment.Area.Factory = f;
                            sendStatus.AlarmHistory.Alarm.AlarmType = at;
                            sendStatus.AlarmHistory.Alarm.AlarmType.AlarmSettings = new List<OilManagementAlarmSetting>();
                            sendStatus.AlarmHistory.Alarm.Groups = new List<OilManagementAlarmGroup>();
                            sendStatus.AlarmHistory.Alarm.GroupsCC = new List<OilManagementAlarmGroup>();

                            lookup.Add(ah.idAlarmHistory, sendStatus);
                        }

                        setting = sendStatus.AlarmHistory.Alarm.AlarmType.AlarmSettings.Where(x => x.idAlarmSetting == s.idAlarmSetting).FirstOrDefault();
                        if (setting == null)
                        {
                            setting = s;
                            sendStatus.AlarmHistory.Alarm.AlarmType.AlarmSettings.Add(setting);
                        }

                        group = sendStatus.AlarmHistory.Alarm.Groups.Where(x => x.idAlarmGroup == ag.idAlarmGroup).FirstOrDefault();
                        if (group == null)
                        {
                            group = sendStatus.AlarmHistory.Alarm.GroupsCC.Where(x => x.idAlarmGroup == ag.idAlarmGroup).FirstOrDefault();
                            if (group == null)
                            {
                                if (ag.TypeAlarmByAlarmGroup == "TO")
                                {
                                    group = ag;
                                    sendStatus.AlarmHistory.Alarm.Groups.Add(group);
                                }
                                else if (ag.TypeAlarmByAlarmGroup == "TO")
                                {
                                    group = ag;
                                    sendStatus.AlarmHistory.Alarm.GroupsCC.Add(group);
                                }
                            }
                        }

                        if (ac != null)
                        {
                            sendStatus.AlarmHistory.Alarm.AlertConfig = ac;
                        }

                        return sendStatus;
                    },
                    splitOn: "idAlarmHistory,idAlarmHistory,idAlarm,idOilSupply,idComponent,idEquipment,idArea,idFactory,idAlarmType,idAlarmSetting,idAlarmGroup,idAlarmAlertConfig"
                    ).AsQueryable();
                }

                return lookup.Values.Cast<OilManagementAlarmSendStatus>().ToList();
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }

        public object Update(OilManagementAlarmSendStatus sendStatus)
        {
            try
            {
                using (var transactionScope = new TransactionScope())
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    sqlConn.Query(@"
                        UPDATE [OilManagement].[AlarmSendStatus]
                        SET [Sent] = @Sent,
	                        [AttemptsNumber] = @AttemptsNumber,
	                        [UpdDateTime] = GETDATE()
                        WHERE idAlarmHistory = @idAlarmHistory;",
                    new
                    {
                        idAlarmHistory = sendStatus.AlarmHistory.idAlarmHistory,
                        Sent = sendStatus.Sent,
                        AttemptsNumber = sendStatus.AttemptsNumber
                    });
                    transactionScope.Complete();

                    return new
                    {
                        result = new
                        {
                            id = sendStatus.AlarmHistory.idAlarmHistory,
                            date = DateTime.Now
                        },
                        status = true,
                        message = "Envio de alarme atualizado com sucesso!"
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
