using System.Web.Mvc;

namespace Calmo.Web.Session
{
    public class ManagedSession
    {
        private readonly Controller _controller;
        public Controller Controller
        {
            get { return this._controller; }
        }

        public ManagedSession(Controller controller)
        {
            this._controller = controller;
        }
    }
}
