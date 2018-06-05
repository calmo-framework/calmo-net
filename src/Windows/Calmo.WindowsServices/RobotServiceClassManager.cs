using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using Calmo.WindowsServices.Properties;

namespace Calmo.WindowsServices
{
    public delegate void ServiceClassStepEventHandler(string stepMessage);
    public delegate void ServiceClassErrorEventHandler(Exception exception);
    public delegate void ServiceClassCompleteEventHandler(ServiceClassDefinition def);

    internal enum ServiceClassExecutionType
    {
        ByDefinition,
        ByClass
    }

    public class RobotServiceClassManager
    {
        private readonly ServiceClassExecutionType _executionType;
        private readonly ServiceClassDefinition _classDefinition;
        private readonly string _appPath;

        private event ServiceClassStepEventHandler _step;
        private event ServiceClassErrorEventHandler _error;
        private event ServiceClassCompleteEventHandler _complete;

        private IRobotServiceClass _processClass;

        private readonly bool _enforceExecution;
        private readonly bool _enforceDefined;

        public event ServiceClassStepEventHandler Step
        {
            add { _step += value; }
            remove { _step -= value; }
        }

        public event ServiceClassErrorEventHandler Error
        {
            add { _error += value; }
            remove { _error -= value; }
        }

        public event ServiceClassCompleteEventHandler Complete
        {
            add { _complete += value; }
            remove { _complete -= value; }
        }

        public RobotServiceClassManager(ServiceClassDefinition classDefinition, string appPath)
        {
            _executionType = ServiceClassExecutionType.ByDefinition;
            _classDefinition = classDefinition;
            _appPath = appPath;
            _enforceDefined = false;
        }

        public RobotServiceClassManager(ServiceClassDefinition classDefinition, string appPath, bool enforceExecution)
        {
            _executionType = ServiceClassExecutionType.ByDefinition;
            _classDefinition = classDefinition;
            _appPath = appPath;
            _enforceExecution = enforceExecution;
            _enforceDefined = true;
        }

        public RobotServiceClassManager(IRobotServiceClass processClass)
        {
            _executionType = ServiceClassExecutionType.ByClass;
            _processClass = processClass;
            _enforceDefined = false;
        }

        public RobotServiceClassManager(IRobotServiceClass processClass, bool enforceExecution)
        {
            _executionType = ServiceClassExecutionType.ByClass;
            _processClass = processClass;
            _enforceExecution = enforceExecution;
            _enforceDefined = true;
        }

        private void OnStep(string stepMessage)
        {
            if (_step != null)
                _step(stepMessage);
        }

        private void OnError(Exception exception)
        {
            if (_error != null)
                _error(exception);
        }

        private void OnComplete(ServiceClassDefinition def)
        {
            if (_complete != null)
                _complete(def);
        }

        public void Execute()
        {
            try
            {
                if (_executionType == ServiceClassExecutionType.ByDefinition)
                    _processClass = this.GetServiceClassInstance(_classDefinition, _appPath);

                if (_processClass == null)
                    throw new NullReferenceException(Resources.ProcessClassNotFoundErrorMessage);

                // Caso a propriedade de forçar a execução tenha sido setada no Manager ele sobrepõe a mesma propriedade configurada na classe de serviço.
                if (_enforceDefined)
                    _processClass.EnforceExecution = _enforceExecution;

                var classType = _processClass.GetType();

                OnStep(String.Format(Resources.Class_Executing, classType.FullName));

                // Solicita os processos a serem executados
                OnStep(String.Format(Resources.Class_RequestingProcesses, classType.FullName));
                var processes = _processClass.GetCurrentProcesses();

                if (processes != null && processes.Any())
                {
                    // Varre os processos e executa um a um
                    OnStep(String.Format(Resources.Class_EnumeratingProcesses, classType.FullName));

                    foreach (RobotServiceItemClass process in processes)
                    {
                        if (String.IsNullOrEmpty(process.Identificator))
                            process.Identificator = classType.FullName;

                        OnStep(String.Format(Resources.Process_ValidatingExecution, process.Identificator));

                        var executeProcess = false;

                        // Verifica se esta esperando retorno caso esteja executa de qualquer forma

                        OnStep(String.Format(Resources.Process_ValidatingStatus, process.Identificator));

                        if (process.Status == RobotServiceItemStatus.WaitingReturn)
                        {
                            executeProcess = true;
                        }
                        else
                        {
                            if (_processClass.EnforceExecution)
                            {
                                executeProcess = true;
                            }
                            else
                            {
                                OnStep(String.Format(Resources.Process_ValidatingExecutionTimeInterval, process.Identificator));

                                // Valida se foi informado uma hora ou um intervalo de execução
                                if (process.ExecuteTime == null && process.ExecuteInterval == null)
                                {
                                    process.Status = RobotServiceItemStatus.NotProcessedWithErrors;
                                    process.ErrorMessage = String.Format(Resources.ExecutionTimeNotFoundErrorMessage, process.Identificator);
                                    _processClass.UpdateProcessStatus(process);
                                }
                                else
                                {
                                    // Pega a hora base para validação
                                    var baseTime = process.LastExecutionTime == null ? process.CreateTime : process.LastExecutionTime.Value;

                                    // Valida se é a hora de executar o processo
                                    if (process.ExecuteTime != null)
                                    {
                                        // Verifica se á exatamente a mesma hora da execução
                                        var nowTime = DateTime.Now.ToShortTimeString();
                                        var executeTime = process.ExecuteTime.Value.ToShortTimeString();

                                        if (nowTime == executeTime)
                                            executeProcess = true;

                                        // Verifica se a hora da execução já passou e nada foi executado ainda
                                        if (!executeProcess)
                                        {
                                            var nowDateTime = DateTime.Now;
                                            var executeDateTime = new DateTime(nowDateTime.Year, nowDateTime.Month, nowDateTime.Day, process.ExecuteTime.Value.Hour, process.ExecuteTime.Value.Minute, process.ExecuteTime.Value.Second);

                                            // Verifica se a hora de execução é menor que a hora atual (já passou a hora de ser executado)
                                            if (executeDateTime < nowDateTime)
                                            {
                                                // Verifica se a hora da ultima execução é menor que a hora de executar (ainda não foi executado)
                                                if (baseTime < executeDateTime)
                                                    executeProcess = true;
                                            }
                                        }
                                    }
                                    else if (process.ExecuteInterval != null)
                                    {
                                        // Pega o intervalo entre a ultima execução e a hora atual
                                        var lastExecuteInterval = DateTime.Now - baseTime;

                                        // Verifica se o intervalo de execução já passou
                                        if (lastExecuteInterval >= process.ExecuteInterval)
                                            executeProcess = true;
                                    }
                                }
                            }
                        }

                        OnStep(String.Format(Resources.Process_VerifyingExecution, process.Identificator));

                        // Executa o processo caso necessário
                        if (executeProcess)
                        {
                            // Verificar se o processo é síncrono
                            if (process.Type != RobotServiceItemType.Asynchronous)
                            {
                                // Executa o processo de forma síncrona
                                OnStep(String.Format(Resources.Process_ExecutingSyncProcess, process.Identificator));

                                ExecuteProcess(_processClass, process);
                            }
                            else
                            {
                                // Executa o processo de forma assíncrona
                                OnStep(String.Format(Resources.Process_ExecutingAsyncProcess, process.Identificator));

                                var asyncProcess = new AsyncThreadCaller();
                                asyncProcess.MethodCall += asyncProcess_MethodCall;

                                var parameters = new Dictionary<string, object>();
                                parameters["processClass"] = _processClass;
                                parameters["process"] = process;

                                asyncProcess.Start(parameters);
                            }
                        }
                    }
                }

                OnStep(String.Format(Resources.Class_ExecutionFinalized, classType.FullName));
                OnComplete(this._classDefinition);
            }
            catch (Exception exp)
            {
                OnError(exp);
                OnComplete(this._classDefinition);
            }
        }

        private void asyncProcess_MethodCall(Dictionary<string, object> parameters)
        {
            var processClass = (IRobotServiceClass)parameters["processClass"];
            var process = (RobotServiceItemClass)parameters["process"];

            ExecuteProcess(processClass, process);
        }

        private void ExecuteProcess(IRobotServiceClass processClass, RobotServiceItemClass process)
        {
            try
            {
                OnStep(String.Format(Resources.Process_ManagingExecutionStatus, process.Identificator));

                // Armazena o status inicial de execução
                var initialStatus = process.Status;

                // Caso o status seja aguardando início ou retorno
                if (initialStatus != RobotServiceItemStatus.WaitingStart &&
                    initialStatus != RobotServiceItemStatus.WaitingReturn) return;

                // Trata atualização de status
                if (initialStatus == RobotServiceItemStatus.WaitingStart)
                {
                    process.Status = RobotServiceItemStatus.Executing;
                    processClass.UpdateProcessStatus(process);
                }
                else if (initialStatus == RobotServiceItemStatus.WaitingReturn)
                {
                    process.Status = RobotServiceItemStatus.RequestingReturn;
                    processClass.UpdateProcessStatus(process);
                }

                try
                {
                    // Processa
                    OnStep(String.Format(Resources.Process_Processing, process.Identificator));
                    var executed = processClass.Process(process);

                    // Trata atualização de status após processamento
                    OnStep(String.Format(Resources.Process_ManagingPostExecutionStatus, process.Identificator));
                    if (initialStatus == RobotServiceItemStatus.WaitingStart)
                    {
                        process.Status = process.HasReturn
                                             ? RobotServiceItemStatus.WaitingReturn
                                             : RobotServiceItemStatus.Processed;
                    }
                    else if (initialStatus == RobotServiceItemStatus.WaitingReturn)
                    {
                        process.Status = executed
                                             ? RobotServiceItemStatus.Processed
                                             : RobotServiceItemStatus.WaitingReturn;
                    }

                    OnStep(String.Format(Resources.Process_ExecutionFinalized, process.Identificator));
                }
                catch (Exception exp)
                {
                    process.Status = RobotServiceItemStatus.ProcessedWithErrors;
                    process.ErrorMessage = exp.Message;
                    OnError(exp);
                }
                finally
                {
                    processClass.UpdateProcessStatus(process);
                }
            }
            catch (Exception exp)
            {
                OnError(exp);
            }
        }

        public IRobotServiceClass GetServiceClassInstance(ServiceClassDefinition classDefinition, string appPath)
        {
            IRobotServiceClass processClass = null;

            OnStep(String.Format(Resources.Class_InitializingProcessment, classDefinition.TypeFullName));
            try
            {
                // Pega o caminho do assembly
                OnStep(String.Format(Resources.Assembly_Locating, classDefinition.AssemblyName));

                string assemblyPath = null;
                if (File.Exists(classDefinition.AssemblyName))
                    assemblyPath = classDefinition.AssemblyName;
                else if (File.Exists(Path.Combine(appPath, classDefinition.AssemblyName)))
                    assemblyPath = Path.Combine(appPath, classDefinition.AssemblyName);

                // Tenta carregar o assembly do caminho encontrado ou do GAC
                Assembly assembly;
                try
                {
                    if (!String.IsNullOrEmpty(assemblyPath))
                    {
                        OnStep(String.Format(Resources.Assembly_Loading, assemblyPath));
                        assembly = Assembly.LoadFrom(assemblyPath);
                    }
                    else
                    {
                        OnStep(String.Format(Resources.Assembly_Loading, classDefinition.AssemblyName));
                        assembly = Assembly.ReflectionOnlyLoadFrom(classDefinition.AssemblyName);
                    }
                }
                catch
                {
                    assembly = null;
                }

                // Verifica se o assembly foi carregado
                if (assembly != null)
                {
                    OnStep(String.Format(Resources.Assembly_Loaded, classDefinition.AssemblyName));

                    // Tenta carregar a classe configurada
                    var type = assembly.GetType(classDefinition.TypeFullName);

                    // Verifica se a classe foi encontrada
                    OnStep(String.Format(Resources.Assembly_VerifyingClassExists, classDefinition.AssemblyName, classDefinition.TypeFullName));
                    if (type != null)
                    {
                        // Verifica se a classe implementa a interface IRobotServiceClass
                        OnStep(String.Format(Resources.Class_VerifyingInterfaceImplementation, classDefinition.TypeFullName));

                        var classInterfaceType = typeof(IRobotServiceClass);
                        var interfaces = type.GetInterfaces();
                        var implementsInfertace = interfaces.Any(interfaceType => interfaceType.FullName == classInterfaceType.FullName);

                        if (implementsInfertace)
                        {
                            //Instancia a classe
                            OnStep(String.Format(Resources.Class_Instantiation, classDefinition.TypeFullName));
                            processClass = (IRobotServiceClass)assembly.CreateInstance(type.FullName);
                            OnStep(String.Format(Resources.Class_Instantiated, classDefinition.TypeFullName));
                        }
                        else
                            throw new NotImplementedException(String.Format(Resources.Class_NotImplementErrorMessage, classDefinition.TypeFullName, classInterfaceType.FullName));
                    }
                    else
                        throw new Exception(String.Format(Resources.Assembly_TypeNotFoundErrorMessage, classDefinition.AssemblyName, classDefinition.TypeFullName));
                }
                else
                    throw new FileNotFoundException(String.Format(Resources.Assembly_AssemblyNotFoundErrorMessage, classDefinition.AssemblyName));

                return processClass;
            }
            catch (Exception exp)
            {
                OnError(exp);
            }

            return processClass;
        }
    }
}