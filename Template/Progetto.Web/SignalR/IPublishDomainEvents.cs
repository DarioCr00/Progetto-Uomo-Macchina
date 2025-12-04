using System.Threading.Tasks;

namespace Progetto.Web.SignalR
{
    public interface IPublishDomainEvents
    {
        Task Publish(object evnt);
    }
}
