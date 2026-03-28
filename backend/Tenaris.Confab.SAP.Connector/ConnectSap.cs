using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAP.Middleware.Connector;

namespace Tenaris.Confab.SAP.Connector
{
    public class ConnectSap
    {
        public bool Connect(string Name, string appServerHost, string systemNumber, string Client, string User, string Password, string Language, string PoolSize, ref RfcDestination rfcDestination, ref string Message)
        {

            try {
                // Cria parametros de conexão
                var parameters = new RfcConfigParameters();

                // Alimenta parametros SAP Logon
                parameters.Add(RfcConfigParameters.Name, Name);
                parameters.Add(RfcConfigParameters.AppServerHost, appServerHost);
                parameters.Add(RfcConfigParameters.SystemNumber, systemNumber);

                // Alimenta parametros SAP GUI
                parameters.Add(RfcConfigParameters.Client, Client);
                parameters.Add(RfcConfigParameters.User, User);
                parameters.Add(RfcConfigParameters.Password, Password);
                parameters.Add(RfcConfigParameters.Language, Language);
                parameters.Add(RfcConfigParameters.PoolSize, PoolSize);

                // Cria destino
                rfcDestination = RfcDestinationManager.GetDestination(parameters);

                rfcDestination.Ping();
                

                Message = "SAP - Conectado com sucesso.";

                return true;
            }
            catch(Exception ex)
            {
                Message = "SAP - Desconectado: " + ex.Message;
                return false;
            }

        }
    }
}
