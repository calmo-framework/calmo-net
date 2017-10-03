using System;
using System.Web;
using System.Security.Principal;
using System.ServiceModel;
using Calmo.Core;

namespace Calmo.Services
{
    /// <summary>
    /// Representa um cliente genérico para consumo de serviços do sistema.
    /// </summary>
    /// <typeparam name="TContract">Interface que representa o contrato para com o serviço.</typeparam>
    public class Client<TContract> : ClientBase<TContract>, IDisposable where TContract : class
    {
        private OperationContextScope operationContext;
        private WindowsImpersonationContext impersonationContext = null;
        private WindowsIdentity impersonationWindowsIdentity = null;

        /// <summary>
        /// Instancia um novo objeto da classe <see cref="Client{TContract}" />.
        /// </summary>
        public Client()
        {
            if (CustomConfiguration.Settings.Services().ImpersonateByWindowsAuthentication)
            {
                impersonationWindowsIdentity = (WindowsIdentity)HttpContext.Current.User.Identity;
                impersonationContext = impersonationWindowsIdentity.Impersonate();
            }

            if (OperationContext.Current == null)
            {
                operationContext = new OperationContextScope(this.InnerChannel);
            }
        }

        /// <summary>
        /// Instancia um novo client indicando se deve ser configurado pelo nome.
        /// </summary>
        /// <param name="endpointConfigurationName">Nome do endpoint</param>
        public Client(string endpointConfigurationName)
            : base(endpointConfigurationName)
        {
        }

        /// <summary>
        /// Obtém o canal de comunicação com o contrato estipulado.
        /// </summary>
        public TContract Proxy
        {
            get
            {
                return base.Channel;
            }
        }

        /// <summary>
        /// Libera os recursos utilizados pela classe.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (this.operationContext != null)
            {
                this.operationContext.Dispose();
            }

            this.Close();

            if (impersonationContext != null)
                impersonationContext.Undo();
        }
    }
}
