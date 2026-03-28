using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain;
using Tenaris.Confab.SGM.Domain.Entities;
using Dapper;
using System.Data.SqlClient;
using System.Transactions;

namespace Tenaris.Confab.SGM.Repositories
{
    
    public class EquipmentBackupRepositorie : IEquipmentBackupRepositorie
    {


        public List<EquipmentBackup> GetBackups()
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var equips = sqlConn.Query<EquipmentBackup>(@"
                        select top 3 e.* 
                        from [Equipment].[EquipmentBackup] e order by e.id desc").ToList();



                    return equips;

                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }


        public List<EquipmentBackupDetails> GetBackupsDetails(int idBackup)
        {
            try
            {
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var equips = sqlConn.Query<EquipmentBackupDetails>(@"
                        select e.* 
                        from [Equipment].[EquipmentBackupDetails] e where e.cdEquipmentBackup = @cdEquipmentBackup",
                        new
                        {
                            cdEquipmentBackup = idBackup,
                        }
                        ).ToList();



                    return equips;

                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);

            }


        }


        public object Backup()
        {

            try
            {
                using (var transactionScope = new TransactionScope())
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {

                    var id = sqlConn.Query<int>(@"
                            Insert into [Equipment].[EquipmentBackup] (dateInput, cdUser) values (@dateInput, @cdUser);
                            select cast(SCOPE_IDENTITY() as int)",
                    new
                    {
                        dateInput = DateTime.Now,
                        cdUser = ""
                    }).Single();


                    sqlConn.Execute(@"
                        Insert into [Equipment].[EquipmentBackupDetails]  select @cdEquipmentBackup, e.* from [Equipment].[Equipment] e ",
                        new
                        {
                            cdEquipmentBackup = id,

                        });



                    transactionScope.Complete();



                    return new { status = true, message = "Backup realizado com sucesso !" };

                }
            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }


        }


        public object Restore(int idBackup)
        {

            try
            {
                using (var transactionScope = new TransactionScope())
                using (var sqlConn = new SqlConnection(Properties.Settings.Default.dbConnection))
                {
                    var equipRepo = new EquipmentRepositorie();

                    //Passo 1: Atualizar os nós que existe na ávore atual e no backup
                    equipRepo.UpdateEquipment(sqlConn, idBackup);


                    //Passo 2: Retorna nós que não existem no backup                   
                    var newsEquips = equipRepo.GetNewsEquipments(sqlConn, idBackup);


                    //Passo 3: Com a lista de nós do Passo 2, deletar os registros sem laudo e atualizar os nós com laudo para inativo.
                    IEnumerable<KeyValuePair<int, EquipmentAux>> ids = new Dictionary<int, EquipmentAux>();
                    ids = GetIds(newsEquips);

                    //Deleta
                    foreach (var id in ids.Where(w => w.Value.Inactivate == false).OrderByDescending(x => x.Key))
                        equipRepo.DeleteEquipment(sqlConn, id.Value.idEquipment);

                    //Desabilita
                    foreach (var id in ids.Where(w => w.Value.Inactivate))                    
                        equipRepo.DisableEquipment(sqlConn, id.Value.idEquipment);


                    //Passo 4: Retorna nós que não existem na ávore atual
                    var newsEquipsBkp = GetNewsEquipmentsBackup(sqlConn, idBackup);

                    //Inserir
                    foreach (var equip in newsEquipsBkp)
                        equipRepo.InsertEquipment(sqlConn, equip);




                    transactionScope.Complete();

                    return new { status = true, message = "Dados restaurados com sucesso !" };

                }
            }

            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }

        public List<Equipment> GetNewsEquipmentsBackup(SqlConnection sqlConn, int idBackup)
        {

            return sqlConn.Query<Equipment>(@"
                       Select    e.[idEquipment]
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
                        from[Equipment].[EquipmentBackupDetails] e
                          where 
	                        e.cdEquipmentBackup = @cdEquipmentBackup and 
	                        e.idEquipment not in (select idEquipment from [Equipment].[Equipment])",
                              new
                              {
                                  cdEquipmentBackup = idBackup
                              }).ToList();
        }

        private IEnumerable<KeyValuePair<int, EquipmentAux>> GetIds(List<Equipment> newsEquips)
        {
            int count = 0;
            foreach (var equip in newsEquips)
            {
                yield return new KeyValuePair<int, EquipmentAux>(count++, GetEquipmenstAux(equip, newsEquips));
            }
        }

        private EquipmentAux GetEquipmenstAux(Equipment equip, List<Equipment> newsEquips)
        {
            List<Equipment> childsEquip = newsEquips.Where(g => g.cdEquipment == equip.idEquipment)?.ToList();
            if (equip.CountReports == 0 && ((childsEquip == null || childsEquip.Count() == 0) || !checkChildReport(childsEquip, newsEquips)))
            {
                return new EquipmentAux { idEquipment = equip.idEquipment, Inactivate = false };
            }
            else
                return new EquipmentAux { idEquipment = equip.idEquipment, Inactivate = true }; ;
        }

        private bool checkChildReport(List<Equipment> childsEquip, List<Equipment> newsEquips)
        {
            foreach (var child in childsEquip)
            {
                if (child.CountReports > 0 || checkChildReport(newsEquips.Where(g => g.cdEquipment == child.idEquipment).ToList(), newsEquips))
                    return true;
            }
            return false;
        }

        private class EquipmentAux
        {

            public int idEquipment { get; set; }
            public bool Inactivate { get; set; }

        }


    }


}
