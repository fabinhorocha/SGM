using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain;
using Tenaris.Confab.SGM.Domain.Entities;
using Dapper;
using System.Data.SqlClient;

namespace Tenaris.Confab.SGM.Repositories
{
    public class EquipmentRepositorie : IEquipmentRepositorie
    {

        public Equipment GetEquipment(int id)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {


                    return sqlConn.Query<Equipment>(@"
                        select e.* 
                        from [Equipment].[Equipment] e                                        
                        where e.Active = 1 and e.idEquipment = @idParent", new { id = id }).SingleOrDefault();
                    ;

                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }


        public List<Equipment> GetEquipments()
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var equips = sqlConn.Query<Equipment>(@"
                        select e.* 
                        from [Equipment].[VwEquipment] e").ToList();



                    return GetEquipments(equips);

                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }

        public List<Equipment> GetEquipmentsWithReports(DateTime startDate, DateTime endDate, int id)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                   
                    var equips = sqlConn.Query<Equipment>(@"
                         select   e.[idEquipment]
                                ,e.[Sheet]
                                ,e.[Name]
                                ,e.[idType]
                                ,e.[idArea]
                                ,e.[idCenterCost]
                                ,e.[idPart]
                                ,e.[cdEquipment]
                                ,e.[Active]
                                ,e.[InsDateTime]
                                ,e.[UpdDateTime]
                                ,e.cdIntegrate 
                                ,(SELECT COUNT(*) FROM [Report].[Report] WHERE [cdEquipment] = e.[idEquipment] and [dateMeasure] between @startDate and @endDate) as CountReports
                            from [equipment].[equipment] e
                            where e.Active = 1", new { startDate = startDate, endDate = endDate }).ToList();

                    //Carrega os equipamentos filhos
                    var equip = GetEquipments(equips, id);

                    //Carrega os equipamentos com laudos
                    var equipsWithRpt = new List<Equipment>();                    
                    GetEquipmentsWithReports(equip, ref equipsWithRpt);

                    return equipsWithRpt;

                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


         }


        public List<Equipment> GetAllEquipments(bool? status)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var equips = new List<Equipment>();

                    if (status == null)
                    {
                        equips = sqlConn.Query<Equipment>(@"
                               select  e.[idEquipment]
                                      ,e.[Sheet]
                                      ,e.[Name]
                                      ,e.[idType]
                                      ,e.[idArea]
                                      ,e.[idCenterCost]
                                      ,e.[idPart]
                                      ,e.[cdEquipment]
                                      ,e.[Active]
                                      ,e.[InsDateTime]
                                      ,e.[UpdDateTime]
                                      ,(SELECT COUNT(*) FROM [Equipment].[Equipment] WHERE [cdEquipment] = e.[idEquipment]) as CountChilds 
                                      ,e.cdIntegrate
                                from [Equipment].[Equipment] e"
                      ).ToList();
                    }

                    else {
                        equips = sqlConn.Query<Equipment>(@"
                               select  e.[idEquipment]
                                      ,e.[Sheet]
                                      ,e.[Name]
                                      ,e.[idType]
                                      ,e.[idArea]
                                      ,e.[idCenterCost]
                                      ,e.[idPart]
                                      ,e.[cdEquipment]
                                      ,e.[Active]
                                      ,e.[InsDateTime]
                                      ,e.[UpdDateTime]
                                      ,(SELECT COUNT(*) FROM [Equipment].[Equipment] WHERE [cdEquipment] = e.[idEquipment] and Active = @Active) as CountChilds 
                                      ,e.cdIntegrate
                                from [Equipment].[Equipment] e where Active = @Active",
                        new
                        {
                            Active = status,

                        }).ToList();
                    }


                    return equips;


                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }


        public List<Equipment> GetNewsEquipments(SqlConnection sqlConn, int idBackup)
        {

            return sqlConn.Query<Equipment>(@"
                        select   e.[idEquipment]
                                ,e.[Sheet]
                                ,e.[Name]
                                ,e.[idType]
                                ,e.[idArea]
                                ,e.[idCenterCost]
                                ,e.[idPart]
                                ,e.[cdEquipment]
                                ,e.[Active]
                                ,e.[InsDateTime]
                                ,e.[UpdDateTime]
                                ,(SELECT COUNT(*) FROM [Equipment].[Equipment] WHERE [cdEquipment] = e.[idEquipment]) as CountChilds 
                                ,e.cdIntegrate 
                                ,(SELECT COUNT(*) FROM [Report].[Report] WHERE [cdEquipment] = e.[idEquipment]) as CountReports
                            from [equipment].[equipment] e
                            where idEquipment not in (
                                Select idEquipment from[Equipment].[EquipmentBackupDetails] where cdEquipmentBackup = @cdEquipmentBackup)",
                              new
                              {
                                  cdEquipmentBackup = idBackup
                              }).ToList();
        }


        private List<Equipment> GetEquipments(List<Equipment> items)
        {
            items.ForEach(i => i.EquipmentsChilds = items.Where(ch => ch.cdEquipment == i.idEquipment).ToList());
            return items.Where(i => i.cdEquipment == null).ToList();
        }

        private Equipment GetEquipments(List<Equipment> items, int id)
        {
            items.ForEach(i => i.EquipmentsChilds = items.Where(ch => ch.cdEquipment == i.idEquipment).ToList());
            return items.Where(i => i.idEquipment == id).Single();
        }

        private void GetEquipmentsWithReports(Equipment equip, ref List<Equipment> listEquips)
        {

            if (equip.CountReports > 0)
                listEquips.Add(equip);

            foreach (var e in equip.EquipmentsChilds)
            {
                GetEquipmentsWithReports(e, ref listEquips);

            }
        }

        public object InsertEquipment(Equipment equipment)
        {

            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    sqlConn.Execute(@"
                        Insert into [Equipment].[Equipment] values 
                        (@idEquipment, @Sheet, @Name, @idType, @idArea, @idCenterCost, @idPart, @cdEquipment, @Active, @InsDateTime, @UpdDateTime, @cdIntegrate, @cdUser)",
                    new
                    {
                        idEquipment = GetLastIdEquipment() + 1,
                        Sheet = equipment.Sheet,
                        Name = equipment.Name,
                        idType = equipment.idType,
                        idArea = equipment.idArea,
                        idCenterCost = equipment.idCenterCost,
                        idPart = equipment.idPart,
                        cdEquipment = equipment.cdEquipment,
                        Active = equipment.Active,
                        InsDateTime = DateTime.Now,
                        UpdDateTime = DateTime.Now,
                        cdIntegrate = equipment.cdIntegrate,
                        cdUser = equipment.cdUser
                    });

                    return new { id = GetLastIdEquipment(), status = true, message = "Dados inseridos com sucesso !" };

                }
            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }


        }

        public object InsertEquipment(SqlConnection sqlConn, Equipment equipment)
        {

            try
            {
                sqlConn.Execute(@"
                        Insert into [Equipment].[Equipment] values 
                        (@idEquipment, @Sheet, @Name, @idType, @idArea, @idCenterCost, @idPart, @cdEquipment, @Active, @InsDateTime, @UpdDateTime, @cdIntegrate, @cdUser)",
                new
                {
                    idEquipment = equipment.idEquipment,
                    Sheet = equipment.Sheet,
                    Name = equipment.Name,
                    idType = equipment.idType,
                    idArea = equipment.idArea,
                    idCenterCost = equipment.idCenterCost,
                    idPart = equipment.idPart,
                    cdEquipment = equipment.cdEquipment,
                    Active = equipment.Active,
                    InsDateTime = DateTime.Now,
                    UpdDateTime = DateTime.Now,
                    cdIntegrate = equipment.cdIntegrate,
                    cdUser = equipment.cdUser
                });

                    return new { id = equipment.idEquipment, status = true, message = "Dados inseridos com sucesso !" };
                
            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }


        }

        public object UpdateEquipment(Equipment equipment)
        {

            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    sqlConn.Execute(@"
                        update [Equipment].[Equipment] set  
                            Sheet = @Sheet, Name = @Name, idType = @idType, idArea = @idArea, idCenterCost = @idCenterCost, 
                            idPart = @idPart, cdEquipment = @cdEquipment, Active = @Active, UpdDateTime = @UpdDateTime, cdIntegrate = @cdIntegrate, cdUser = @cdUser
                        where idEquipment = @idEquipment",
                    new
                    {

                        Sheet = equipment.Sheet,
                        Name = equipment.Name,
                        idType = equipment.idType,
                        idArea = equipment.idArea,
                        idCenterCost = equipment.idCenterCost,
                        idPart = equipment.idPart,
                        cdEquipment = equipment.cdEquipment,
                        Active = equipment.Active,
                        UpdDateTime = DateTime.Now,
                        idEquipment = equipment.idEquipment,
                        cdIntegrate = equipment.cdIntegrate,
                        cdUser = equipment.cdUser
                    });

                    return new { id = equipment.idEquipment, status = true, message = "Dados atualizados com sucesso !" };


                }
            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }


        public void DeleteEquipment(int idEquipment)
        {

            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    sqlConn.Execute(@"Delete [Equipment].[Equipment] where idEquipment = @idEquipment", new { idEquiment = idEquipment });
                }
            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }


        public void DeleteEquipment(SqlConnection sqlConn, int idEquip)
        {

            try
            {

                sqlConn.Execute(@"Delete [Equipment].[Equipment] where idEquipment = @idEquipment", new { idEquipment = idEquip });

            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }


        public void DisableEquipment(SqlConnection sqlConn, int idEquip)
        {

            try
            {

                sqlConn.Execute(@"update [Equipment].[Equipment] set Active = 0 where idEquipment = @idEquipment", new { idEquipment = idEquip });

            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }


        private int GetLastIdEquipment()
        {

            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    return sqlConn.Query<int>(@"
                       select max(idEquipment) from (
                        select idEquipment from [Equipment].[Equipment]                      
                        union 
                        select idEquipment from [Equipment].[EquipmentBackupDetails]) T").First();


                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }

        }


        public void UpdateEquipment(SqlConnection sqlConn, int idBackup)
        {
            sqlConn.Execute(@"
                            update e set                                         
                                        e.Sheet = d.Sheet, 
                                        e.Name = d.Name, 
                                        e.idType = d.idType, 
                                        e.idArea = d.idArea, 
                                        e.idCenterCost = d.idCenterCost, 
                                        e.idPart = d.idPart, 
                                        e.cdEquipment = d.cdEquipment, 
                                        e.Active = d.Active, 
                                        e.InsDateTime = d.InsDateTime, 
                                        e.UpdDateTime = d.UpdDateTime, 
                                        e.cdIntegrate = d.cdIntegrate
                                from [Equipment].[Equipment] e inner join [Equipment].[EquipmentBackupDetails] d on e.idEquipment = d.idEquipment
                                where d.cdEquipmentBackup = @cdEquipmentBackup",
                        new
                        {
                            cdEquipmentBackup = idBackup
                        });
        }

    }
}
